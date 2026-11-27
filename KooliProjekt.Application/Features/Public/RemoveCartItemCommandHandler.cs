using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Features.Public
{
    public class RemoveCartItemCommandHandler : IRequestHandler<RemoveCartItemCommand, OperationResult<int>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly CurrentCustomer _currentCustomer;

        public RemoveCartItemCommandHandler(ApplicationDbContext dbContext, CurrentCustomer currentCustomer)
        {
            _dbContext = dbContext;
            _currentCustomer = currentCustomer;
        }

        public async Task<OperationResult<int>> Handle(RemoveCartItemCommand request, CancellationToken cancellationToken)
        {
            var response = new OperationResult<int>();

            var customer = await _currentCustomer.GetAsync(cancellationToken);

            if (customer == null)
            {
                return response.AddError(PublicMessages.CustomerNotFound);
            }

            var cartIds = _dbContext.ShoppingCarts
                .Where(x => x.CustomerId == customer.Id)
                .Select(x => x.Id);

            var item = await _dbContext.CartItems
                .FirstOrDefaultAsync(x => x.Id == request.CartItemId && cartIds.Contains(x.CartId), cancellationToken);

            if (item == null)
            {
                return response.AddError("Ostukorvi rida ei leitud.");
            }

            _dbContext.CartItems.Remove(item);

            var cart = await _dbContext.ShoppingCarts
                .FirstOrDefaultAsync(x => x.Id == item.CartId, cancellationToken);

            if (cart != null)
            {
                cart.UpdatedAt = DateTime.UtcNow;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            response.Value = request.CartItemId;

            return response;
        }
    }
}
