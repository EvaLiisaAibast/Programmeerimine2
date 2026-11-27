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
    public class GetMyCartQueryHandler : IRequestHandler<GetMyCartQuery, OperationResult<MyCartDto>>
    {
        private const decimal ShippingCost = 4.99m;
        private const decimal FreeShippingFrom = 100m;

        private readonly ApplicationDbContext _dbContext;
        private readonly CurrentCustomer _currentCustomer;

        public GetMyCartQueryHandler(ApplicationDbContext dbContext, CurrentCustomer currentCustomer)
        {
            _dbContext = dbContext;
            _currentCustomer = currentCustomer;
        }

        public async Task<OperationResult<MyCartDto>> Handle(GetMyCartQuery request, CancellationToken cancellationToken)
        {
            var response = new OperationResult<MyCartDto>();

            var customer = await _currentCustomer.GetAsync(cancellationToken);

            if (customer == null)
            {
                return response.AddError(PublicMessages.CustomerNotFound);
            }

            var cart = await _dbContext.ShoppingCarts
                .AsNoTracking()
                .Where(x => x.CustomerId == customer.Id)
                .OrderByDescending(x => x.UpdatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (cart == null)
            {
                response.Value = new MyCartDto();

                return response;
            }

            var items = await (from item in _dbContext.CartItems.AsNoTracking()
                               join variant in _dbContext.ProductVariants on item.VariantId equals variant.Id
                               join product in _dbContext.Products on variant.ProductId equals product.Id
                               where item.CartId == cart.Id
                               orderby item.Id
                               select new MyCartItemDto
                               {
                                   Id = item.Id,
                                   VariantId = item.VariantId,
                                   ProductName = product.Name,
                                   VariantCode = variant.ProductCode,
                                   Quantity = item.Quantity,
                                   UnitPrice = item.UnitPrice,
                                   LineTotal = item.Quantity * item.UnitPrice,
                                   AvailableQuantity = _dbContext.StockLevels
                                       .Where(s => s.VariantId == variant.Id)
                                       .Sum(s => (int?)s.Quantity) ?? 0
                               })
                               .ToListAsync(cancellationToken);

            var subtotal = items.Sum(x => x.LineTotal);

            response.Value = new MyCartDto
            {
                Id = cart.Id,
                Items = items,
                TotalQuantity = items.Sum(x => x.Quantity),
                Subtotal = subtotal,
                EstimatedShipping = subtotal >= FreeShippingFrom || subtotal == 0 ? 0 : ShippingCost,
                Total = subtotal + (subtotal >= FreeShippingFrom || subtotal == 0 ? 0 : ShippingCost)
            };

            return response;
        }
    }
}
