using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Features.Public.Dtos;
using KooliProjekt.Application.Infrastructure.Paging;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Features.Public
{
    public class ListPublicProductsQueryHandler : IRequestHandler<ListPublicProductsQuery, OperationResult<PagedResult<PublicProductListItemDto>>>
    {
        private readonly ApplicationDbContext _dbContext;

        public ListPublicProductsQueryHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OperationResult<PagedResult<PublicProductListItemDto>>> Handle(ListPublicProductsQuery request, CancellationToken cancellationToken)
        {
            var response = new OperationResult<PagedResult<PublicProductListItemDto>>();

            var query = _dbContext.Products
                .AsNoTracking()
                .Where(x => x.Status == "Active");

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim();
                query = query.Where(x => x.Name.Contains(search));
            }

            if (request.CategoryId > 0)
            {
                query = query.Where(x => x.CategoryId == request.CategoryId);
            }

            response.Value = await query
                .OrderBy(x => x.Name)
                .Select(x => new PublicProductListItemDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    CategoryName = _dbContext.Categories.Where(t => t.Id == x.CategoryId).Select(t => t.Name).FirstOrDefault(),
                    BrandName = _dbContext.Brands.Where(t => t.Id == x.BrandId).Select(t => t.Name).FirstOrDefault(),
                    Price = x.Price,
                    ImageUrl = x.ImageUrl
                })
                .GetPagedAsync(request.Page, request.PageSize);

            return response;
        }
    }
}
