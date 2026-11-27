using System;
using System.Collections.Generic;

namespace KooliProjekt.Application.Features.Public.Dtos
{
    public class PublicProductListItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
    }

    public class PublicProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public decimal AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public IList<PublicProductVariantDto> Variants { get; set; }
        public IList<PublicProductImageDto> Images { get; set; }
        public IList<PublicReviewDto> Reviews { get; set; }

        public PublicProductDto()
        {
            Variants = new List<PublicProductVariantDto>();
            Images = new List<PublicProductImageDto>();
            Reviews = new List<PublicReviewDto>();
        }
    }

    public class PublicProductVariantDto
    {
        public int Id { get; set; }
        public string ProductCode { get; set; }
        public decimal Price { get; set; }
        public int AvailableQuantity { get; set; }
    }

    public class PublicProductImageDto
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string AltText { get; set; }
        public bool IsMain { get; set; }
    }

    public class PublicReviewDto
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string AuthorName { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PublicCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ProductCount { get; set; }
    }
}
