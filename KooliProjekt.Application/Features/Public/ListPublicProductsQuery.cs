using KooliProjekt.Application.Features.Public.Dtos;
using KooliProjekt.Application.Infrastructure.Paging;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.Public
{
    /// <summary>
    /// Public product catalog. Only active products are returned.
    /// </summary>
    public class ListPublicProductsQuery : IRequest<OperationResult<PagedResult<PublicProductListItemDto>>>
    {
        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public string Search { get; set; }

        public int CategoryId { get; set; }
    }
}
