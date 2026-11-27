using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Features.Public.Dtos;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Features.Public
{
    public class GetMyWishlistQueryHandler : IRequestHandler<GetMyWishlistQuery, OperationResult<MyWishlistDto>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly CurrentCustomer _currentCustomer;

        public GetMyWishlistQueryHandler(ApplicationDbContext dbContext, CurrentCustomer currentCustomer)
        {
            _dbContext = dbContext;
            _currentCustomer = currentCustomer;
        }

        public async Task<OperationResult<MyWishlistDto>> Handle(GetMyWishlistQuery request, CancellationToken cancellationToken)
        {
            var response = new OperationResult<MyWishlistDto>();

            var customer = await _currentCustomer.GetAsync(cancellationToken);

            if (customer == null)
            {
                return response.AddError(PublicMessages.CustomerNotFound);
            }

            var wishlist = await _dbContext.Wishlists
                .AsNoTracking()
                .Where(x => x.CustomerId == customer.Id)
                .OrderBy(x => x.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (wishlist == null)
            {
                response.Value = new MyWishlistDto();

                return response;
            }

            var items = await (from item in _dbContext.WishlistItems.AsNoTracking()
                               join variant in _dbContext.ProductVariants on item.VariantId equals variant.Id
                               join product in _dbContext.Products on variant.ProductId equals product.Id
                               where item.WishlistId == wishlist.Id
                               orderby item.AddedAt descending, item.Id
                               select new MyWishlistItemDto
                               {
                                   Id = item.Id,
                                   VariantId = item.VariantId,
                                   ProductId = product.Id,
                                   ProductName = product.Name,
                                   VariantCode = variant.ProductCode,
                                   Price = variant.Price,
                                   AvailableQuantity = _dbContext.StockLevels
                                       .Where(s => s.VariantId == variant.Id)
                                       .Sum(s => (int?)s.Quantity) ?? 0,
                                   AddedAt = item.AddedAt
                               })
                               .ToListAsync(cancellationToken);

            foreach (var item in items)
            {
                item.IsAvailable = item.AvailableQuantity > 0;
            }

            response.Value = new MyWishlistDto
            {
                Id = wishlist.Id,
                Name = wishlist.Name,
                ItemCount = items.Count,
                Items = items
            };

            return response;
        }
    }
}
