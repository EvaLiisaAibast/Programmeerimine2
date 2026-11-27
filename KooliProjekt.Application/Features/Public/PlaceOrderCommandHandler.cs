using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Features.Public
{
    public class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, OperationResult<int>>
    {
        private const decimal ShippingCost = 4.99m;
        private const decimal FreeShippingFrom = 100m;
        private const decimal VatRate = 0.24m;

        private readonly ApplicationDbContext _dbContext;
        private readonly CurrentCustomer _currentCustomer;

        public PlaceOrderCommandHandler(ApplicationDbContext dbContext, CurrentCustomer currentCustomer)
        {
            _dbContext = dbContext;
            _currentCustomer = currentCustomer;
        }

        public async Task<OperationResult<int>> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
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

            var cart = await _dbContext.ShoppingCarts
                .Where(x => x.CustomerId == customer.Id)
                .OrderByDescending(x => x.UpdatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (cart == null)
            {
                return response.AddError("Ostukorv on tühi. Lisa enne tellimuse esitamist vähemalt üks toode.");
            }

            var cartItems = await _dbContext.CartItems
                .Where(x => x.CartId == cart.Id)
                .OrderBy(x => x.Id)
                .ToListAsync(cancellationToken);

            if (cartItems.Count == 0)
            {
                return response.AddError("Ostukorv on tühi. Lisa enne tellimuse esitamist vähemalt üks toode.");
            }

            var addressQuery = _dbContext.Addresses.Where(x => x.CustomerId == customer.Id);

            var address = request.AddressId > 0
                ? await addressQuery.FirstOrDefaultAsync(x => x.Id == request.AddressId, cancellationToken)
                : await addressQuery
                    .OrderByDescending(x => x.IsDefault)
                    .ThenBy(x => x.Id)
                    .FirstOrDefaultAsync(cancellationToken);

            if (address == null)
            {
                return response.AddError(request.AddressId > 0
                    ? "Valitud aadressi ei leitud."
                    : "Sul puudub aadress. Lisa aadress enne tellimuse esitamist.");
            }

            var variantIds = cartItems.Select(x => x.VariantId).Distinct().ToList();

            var variants = await _dbContext.ProductVariants
                .Where(x => variantIds.Contains(x.Id))
                .ToListAsync(cancellationToken);

            var productIds = variants.Select(x => x.ProductId).Distinct().ToList();

            var products = await _dbContext.Products
                .Where(x => productIds.Contains(x.Id))
                .ToListAsync(cancellationToken);

            var stockLevels = await _dbContext.StockLevels
                .Where(x => variantIds.Contains(x.VariantId))
                .ToListAsync(cancellationToken);

            var lines = new List<OrderLine>();

            foreach (var cartItem in cartItems)
            {
                var variant = variants.FirstOrDefault(x => x.Id == cartItem.VariantId);

                if (variant == null || variant.Status != "Active")
                {
                    return response.AddError("Üks ostukorvis olev toode ei ole enam saadaval. Palun eemalda see ostukorvist.");
                }

                var product = products.FirstOrDefault(x => x.Id == variant.ProductId);

                if (product == null || product.Status != "Active")
                {
                    return response.AddError("Toode \"" + product?.Name + "\" ei ole enam saadaval. Palun eemalda see ostukorvist.");
                }

                var available = stockLevels
                    .Where(x => x.VariantId == variant.Id)
                    .Sum(x => x.Quantity);

                if (available < cartItem.Quantity)
                {
                    return response.AddError("Toodet \"" + product.Name + "\" on laos ainult " + available + " tk.");
                }

                lines.Add(new OrderLine
                {
                    VariantId = variant.Id,
                    ProductId = product.Id,
                    ProductName = product.Name,
                    VariantCode = variant.ProductCode,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.UnitPrice,
                    Discount = 0,
                    Amount = cartItem.Quantity * cartItem.UnitPrice
                });
            }

            var subtotal = lines.Sum(x => x.Amount);
            var discount = 0m;
            var shipping = subtotal >= FreeShippingFrom ? 0m : ShippingCost;
            var vat = Math.Round((subtotal - discount) * VatRate, 2);

            var lastOrderNumber = await _dbContext.Orders
                .MaxAsync(x => (int?)x.OrderNumber, cancellationToken) ?? 2000;

            var order = new Order
            {
                OrderNumber = lastOrderNumber + 1,
                CustomerId = customer.Id,
                InvoiceAddressId = address.Id,
                DeliveryAddressId = address.Id,
                OrderDate = DateTime.UtcNow,
                Subtotal = subtotal,
                Discount = discount,
                Vat = vat,
                ShippingCost = shipping,
                Amount = subtotal - discount + shipping,
                Status = "pending"
            };

            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync(cancellationToken);

            foreach (var line in lines)
            {
                line.OrderId = order.Id;
                _dbContext.OrderLines.Add(line);
            }

            _dbContext.CartItems.RemoveRange(cartItems);

            await _dbContext.SaveChangesAsync(cancellationToken);

            response.Value = order.Id;

            return response;
        }
    }
}
