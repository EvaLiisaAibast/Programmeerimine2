using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Features.Public
{
    public class RemoveWishlistItemCommandHandler : IRequestHandler<RemoveWishlistItemCommand, OperationResult<int>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly CurrentCustomer _currentCustomer;

        public RemoveWishlistItemCommandHandler(ApplicationDbContext dbContext, CurrentCustomer currentCustomer)
        {
            _dbContext = dbContext;
            _currentCustomer = currentCustomer;
        }

        public async Task<OperationResult<int>> Handle(RemoveWishlistItemCommand request, CancellationToken cancellationToken)
        {
            var response = new OperationResult<int>();

            var customer = await _currentCustomer.GetAsync(cancellationToken);

            if (customer == null)
            {
                return response.AddError(PublicMessages.CustomerNotFound);
            }

            var wishlistIds = _dbContext.Wishlists
                .Where(x => x.CustomerId == customer.Id)
                .Select(x => x.Id);

            var item = await _dbContext.WishlistItems
                .FirstOrDefaultAsync(x => x.Id == request.WishlistItemId && wishlistIds.Contains(x.WishlistId), cancellationToken);

            if (item == null)
            {
                return response.AddError("Soovinimekirja rida ei leitud.");
            }

            _dbContext.WishlistItems.Remove(item);
            await _dbContext.SaveChangesAsync(cancellationToken);

            response.Value = request.WishlistItemId;

            return response;
        }
    }
}
