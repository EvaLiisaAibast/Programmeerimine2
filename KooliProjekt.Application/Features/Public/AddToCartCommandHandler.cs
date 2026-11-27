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
    public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, OperationResult<int>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly CurrentCustomer _currentCustomer;

        public AddToCartCommandHandler(ApplicationDbContext dbContext, CurrentCustomer currentCustomer)
        {
            _dbContext = dbContext;
            _currentCustomer = currentCustomer;
        }

        public async Task<OperationResult<int>> Handle(AddToCartCommand request, CancellationToken cancellationToken)
        {
            var response = new OperationResult<int>();

            var customer = await _currentCustomer.GetAsync(cancellationToken);

            if (customer == null)
            {
                return response.AddError(PublicMessages.CustomerNotFound);
            }

            if (customer.Status != "Aktiivne")
            {
                return response.AddError(PublicMessages.AccountBlocked);
            }

            var variant = await _dbContext.ProductVariants
                .FirstOrDefaultAsync(x => x.Id == request.VariantId && x.Status == "Active", cancellationToken);

            if (variant == null)
            {
                return response.AddError("Valitud toode ei ole saadaval.");
            }

            var productIsActive = await _dbContext.Products
                .AnyAsync(x => x.Id == variant.ProductId && x.Status == "Active", cancellationToken);

            if (!productIsActive)
            {
                return response.AddError("Valitud toode ei ole saadaval.");
            }

            var available = await _dbContext.StockLevels
                .Where(x => x.VariantId == variant.Id)
                .SumAsync(x => (int?)x.Quantity, cancellationToken) ?? 0;

            if (available < request.Quantity)
            {
                return response.AddError("Laos on ainult " + available + " tk.");
            }

            var now = DateTime.UtcNow;

            var cart = await _dbContext.ShoppingCarts
                .Where(x => x.CustomerId == customer.Id)
                .OrderByDescending(x => x.UpdatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (cart == null)
            {
                cart = new ShoppingCart
                {
                    CustomerId = customer.Id,
                    CreatedAt = now,
                    UpdatedAt = now
                };

                _dbContext.ShoppingCarts.Add(cart);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            var alreadyInCart = await _dbContext.CartItems
                .AnyAsync(x => x.CartId == cart.Id && x.VariantId == variant.Id, cancellationToken);

            if (alreadyInCart)
            {
                return response.AddError("See toode on juba ostukorvis. Koguse muutmiseks kasuta ostukorvi rea uuendamist.");
            }

            var item = new CartItem
            {
                CartId = cart.Id,
                VariantId = variant.Id,
                Quantity = request.Quantity,
                UnitPrice = variant.Price
            };

            _dbContext.CartItems.Add(item);
            cart.UpdatedAt = now;

            await _dbContext.SaveChangesAsync(cancellationToken);

            response.Value = item.Id;

            return response;
        }
    }
}
