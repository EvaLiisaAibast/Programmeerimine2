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
    public class GetMyOrderQueryHandler : IRequestHandler<GetMyOrderQuery, OperationResult<MyOrderDto>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly CurrentCustomer _currentCustomer;

        public GetMyOrderQueryHandler(ApplicationDbContext dbContext, CurrentCustomer currentCustomer)
        {
            _dbContext = dbContext;
            _currentCustomer = currentCustomer;
        }

        public async Task<OperationResult<MyOrderDto>> Handle(GetMyOrderQuery request, CancellationToken cancellationToken)
        {
            var response = new OperationResult<MyOrderDto>();

            var customer = await _currentCustomer.GetAsync(cancellationToken);

            if (customer == null)
            {
                return response.AddError(PublicMessages.CustomerNotFound);
            }

            var order = await _dbContext.Orders
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.CustomerId == customer.Id, cancellationToken);

            if (order == null)
            {
                return response.AddError("Tellimust ei leitud.");
            }

            var invoiceAddress = await _dbContext.Addresses
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == order.InvoiceAddressId, cancellationToken);

            var deliveryAddress = await _dbContext.Addresses
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == order.DeliveryAddressId, cancellationToken);

            var dto = new MyOrderDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                OrderDate = order.OrderDate,
                Status = order.Status,
                Subtotal = order.Subtotal,
                Discount = order.Discount,
                Vat = order.Vat,
                ShippingCost = order.ShippingCost,
                Amount = order.Amount,
                PaidAt = order.PaidAt,
                SentAt = order.SentAt,
                DeliveredAt = order.DeliveredAt,
                InvoiceAddress = FormatAddress(invoiceAddress),
                DeliveryAddress = FormatAddress(deliveryAddress),
                Lines = await _dbContext.OrderLines
                    .AsNoTracking()
                    .Where(x => x.OrderId == order.Id)
                    .OrderBy(x => x.Id)
                    .Select(x => new MyOrderLineDto
                    {
                        Id = x.Id,
                        ProductId = x.ProductId,
                        ProductName = x.ProductName,
                        VariantCode = x.VariantCode,
                        Quantity = x.Quantity,
                        UnitPrice = x.UnitPrice,
                        Discount = x.Discount,
                        Amount = x.Amount
                    })
                    .ToListAsync(cancellationToken),
                Payments = await _dbContext.Payments
                    .AsNoTracking()
                    .Where(x => x.OrderId == order.Id)
                    .OrderByDescending(x => x.CreatedAt)
                    .Select(x => new MyPaymentDto
                    {
                        Id = x.Id,
                        Amount = x.Amount,
                        Method = x.Method,
                        Status = x.Status,
                        TransactionReference = x.TransactionReference,
                        CreatedAt = x.CreatedAt
                    })
                    .ToListAsync(cancellationToken)
            };

            var shipment = await _dbContext.Shipments
                .AsNoTracking()
                .Where(x => x.OrderId == order.Id)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (shipment != null)
            {
                dto.Shipment = new MyShipmentDto
                {
                    TrackingNumber = shipment.TrackingNumber,
                    CarrierName = await _dbContext.Carriers
                        .Where(x => x.Id == shipment.CarrierId)
                        .Select(x => x.Name)
                        .FirstOrDefaultAsync(cancellationToken),
                    Status = shipment.Status,
                    SentAt = shipment.SentAt,
                    EstimatedDelivery = shipment.EstimatedDelivery,
                    DeliveredAt = shipment.DeliveredAt
                };
            }

            response.Value = dto;

            return response;
        }

        private static string FormatAddress(Address address)
        {
            if (address == null)
            {
                return null;
            }

            var line = address.Street + " " + address.HouseNumber;

            if (!string.IsNullOrWhiteSpace(address.Apartment))
            {
                line += "-" + address.Apartment;
            }

            return line + ", " + address.City + " " + address.PostalCode + ", " + address.Country;
        }
    }
}
