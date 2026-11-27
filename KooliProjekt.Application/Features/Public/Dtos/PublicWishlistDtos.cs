using System;
using System.Collections.Generic;

namespace KooliProjekt.Application.Features.Public.Dtos
{
    public class MyWishlistDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ItemCount { get; set; }
        public IList<MyWishlistItemDto> Items { get; set; }

        public MyWishlistDto()
        {
            Items = new List<MyWishlistItemDto>();
        }
    }

    public class MyWishlistItemDto
    {
        public int Id { get; set; }
        public int VariantId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string VariantCode { get; set; }
        public decimal Price { get; set; }
        public int AvailableQuantity { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime AddedAt { get; set; }
    }
}
