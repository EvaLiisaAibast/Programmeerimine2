using KooliProjekt.Application.Features.Public.Dtos;
using KooliProjekt.Application.Infrastructure.Paging;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.Public
{
    /// <summary>
    /// Order history of the signed in user.
    /// </summary>
    public class ListMyOrdersQuery : IRequest<OperationResult<PagedResult<MyOrderListItemDto>>>
    {
        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
