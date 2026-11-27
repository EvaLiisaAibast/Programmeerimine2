using System;
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
    public class GetPublicProductQueryHandler : IRequestHandler<GetPublicProductQuery, OperationResult<PublicProductDto>>
    {
        private readonly ApplicationDbContext _dbContext;

        public GetPublicProductQueryHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OperationResult<PublicProductDto>> Handle(GetPublicProductQuery request, CancellationToken cancellationToken)
        {
            var response = new OperationResult<PublicProductDto>();

            var product = await _dbContext.Products
                .AsNoTracking()
                .Where(x => x.Id == request.Id && x.Status == "Active")
                .Select(x => new PublicProductDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    CategoryName = _dbContext.Categories.Where(t => t.Id == x.CategoryId).Select(t => t.Name).FirstOrDefault(),
                    BrandName = _dbContext.Brands.Where(t => t.Id == x.BrandId).Select(t => t.Name).FirstOrDefault(),
                    Price = x.Price,
                    ImageUrl = x.ImageUrl,
                    Variants = _dbContext.ProductVariants
                        .Where(v => v.ProductId == x.Id && v.Status == "Active")
                        .OrderBy(v => v.Id)
                        .Select(v => new PublicProductVariantDto
                        {
                            Id = v.Id,
                            ProductCode = v.ProductCode,
                            Price = v.Price,
                            AvailableQuantity = _dbContext.StockLevels
                                .Where(s => s.VariantId == v.Id)
                                .Sum(s => (int?)s.Quantity) ?? 0
                        })
                        .ToList(),
                    Images = _dbContext.ProductImages
                        .Where(i => i.ProductId == x.Id)
                        .OrderBy(i => i.SortOrder)
                        .Select(i => new PublicProductImageDto
                        {
                            Id = i.Id,
                            Url = i.Url,
                            AltText = i.AltText,
                            IsMain = i.IsMain
                        })
                        .ToList(),
                    Reviews = _dbContext.Reviews
                        .Where(r => r.ProductId == x.Id && r.Status == "Kinnitatud")
                        .OrderByDescending(r => r.CreatedAt)
                        .Select(r => new PublicReviewDto
                        {
                            Id = r.Id,
                            Rating = r.Rating,
                            Title = r.Title,
                            Content = r.Content,
                            CreatedAt = r.CreatedAt,
                            AuthorName = _dbContext.Customers
                                .Where(c => c.Id == r.CustomerId)
                                .Select(c => c.FirstName + " " + c.LastName)
                                .FirstOrDefault()
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (product == null)
            {
                return response;
            }

            product.ReviewCount = product.Reviews.Count;
            product.AverageRating = product.ReviewCount == 0
                ? 0
                : Math.Round(product.Reviews.Average(x => (decimal)x.Rating), 1);

            response.Value = product;

            return response;
        }
    }
}
