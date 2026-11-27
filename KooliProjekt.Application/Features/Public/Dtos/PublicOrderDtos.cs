using System;
using System.Collections.Generic;

namespace KooliProjekt.Application.Features.Public.Dtos
{
    public class MyOrderListItemDto
    {
        public int Id { get; set; }
        public int OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
        public int LineCount { get; set; }
    }

    public class MyOrderDto
    {
        public int Id { get; set; }
        public int OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Vat { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Amount { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime? SentAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public string InvoiceAddress { get; set; }
        public string DeliveryAddress { get; set; }
        public IList<MyOrderLineDto> Lines { get; set; }
        public IList<MyPaymentDto> Payments { get; set; }
        public MyShipmentDto Shipment { get; set; }

        public MyOrderDto()
        {
            Lines = new List<MyOrderLineDto>();
            Payments = new List<MyPaymentDto>();
        }
    }

    public class MyOrderLineDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string VariantCode { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal Amount { get; set; }
    }

    public class MyPaymentDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; }
        public string Status { get; set; }
        public string TransactionReference { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class MyShipmentDto
    {
        public string TrackingNumber { get; set; }
        public string CarrierName { get; set; }
        public string Status { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime EstimatedDelivery { get; set; }
        public DateTime DeliveredAt { get; set; }
    }
}
