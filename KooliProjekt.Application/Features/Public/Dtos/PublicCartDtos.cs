using System.Collections.Generic;

namespace KooliProjekt.Application.Features.Public.Dtos
{
    public class MyCartDto
    {
        public int Id { get; set; }
        public int TotalQuantity { get; set; }
        public decimal Subtotal { get; set; }
        public decimal EstimatedShipping { get; set; }
        public decimal Total { get; set; }
        public IList<MyCartItemDto> Items { get; set; }

        public MyCartDto()
        {
            Items = new List<MyCartItemDto>();
        }
    }

    public class MyCartItemDto
    {
        public int Id { get; set; }
        public int VariantId { get; set; }
        public string ProductName { get; set; }
        public string VariantCode { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
        public int AvailableQuantity { get; set; }
    }
}
