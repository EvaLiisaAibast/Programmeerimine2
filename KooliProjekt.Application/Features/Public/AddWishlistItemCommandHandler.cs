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
    public class AddWishlistItemCommandHandler : IRequestHandler<AddWishlistItemCommand, OperationResult<int>>
    {
        private const string DefaultWishlistName = "Soovid";

        private readonly ApplicationDbContext _dbContext;
        private readonly CurrentCustomer _currentCustomer;

        public AddWishlistItemCommandHandler(ApplicationDbContext dbContext, CurrentCustomer currentCustomer)
        {
            _dbContext = dbContext;
            _currentCustomer = currentCustomer;
        }

        public async Task<OperationResult<int>> Handle(AddWishlistItemCommand request, CancellationToken cancellationToken)
        {
            var response = new OperationResult<int>();

            var customer = await _currentCustomer.GetAsync(cancellationToken);

            if (customer == null)
            {
                return response.AddError(PublicMessages.CustomerNotFound);
            }

            var variant = await _dbContext.ProductVariants
                .FirstOrDefaultAsync(x => x.Id == request.VariantId && x.Status == "Active", cancellationToken);

            if (variant == null)
            {
                return response.AddError("Valitud toode ei ole saadaval.");
            }

            var product = await _dbContext.Products
                .FirstOrDefaultAsync(x => x.Id == variant.ProductId && x.Status == "Active", cancellationToken);

            if (product == null)
            {
                return response.AddError("Valitud toode ei ole saadaval.");
            }

            var wishlist = await _dbContext.Wishlists
                .Where(x => x.CustomerId == customer.Id)
                .OrderBy(x => x.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (wishlist == null)
            {
                wishlist = new Wishlist
                {
                    CustomerId = customer.Id,
                    Name = DefaultWishlistName,
                    CreatedAt = DateTime.UtcNow
                };

                _dbContext.Wishlists.Add(wishlist);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            var alreadyAdded = await _dbContext.WishlistItems
                .AnyAsync(x => x.WishlistId == wishlist.Id && x.VariantId == variant.Id, cancellationToken);

            if (alreadyAdded)
            {
                return response.AddError("See toode on juba soovinimekirjas.");
            }

            var item = new WishlistItem
            {
                WishlistId = wishlist.Id,
                VariantId = variant.Id,
                AddedAt = DateTime.UtcNow
            };

            _dbContext.WishlistItems.Add(item);
            await _dbContext.SaveChangesAsync(cancellationToken);

            response.Value = item.Id;

            return response;
        }
    }
}
