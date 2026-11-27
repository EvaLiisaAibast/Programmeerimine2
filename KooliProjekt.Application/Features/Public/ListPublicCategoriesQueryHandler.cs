using System.Collections.Generic;
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
    public class ListPublicCategoriesQueryHandler : IRequestHandler<ListPublicCategoriesQuery, OperationResult<IList<PublicCategoryDto>>>
    {
        private readonly ApplicationDbContext _dbContext;

        public ListPublicCategoriesQueryHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OperationResult<IList<PublicCategoryDto>>> Handle(ListPublicCategoriesQuery request, CancellationToken cancellationToken)
        {
            var response = new OperationResult<IList<PublicCategoryDto>>();

            response.Value = await _dbContext.Categories
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new PublicCategoryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    ProductCount = _dbContext.Products.Count(p => p.CategoryId == x.Id && p.Status == "Active")
                })
                .ToListAsync(cancellationToken);

            return response;
        }
    }
}
