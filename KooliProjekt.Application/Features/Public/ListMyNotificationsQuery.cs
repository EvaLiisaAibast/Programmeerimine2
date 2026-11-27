using KooliProjekt.Application.Features.Public.Dtos;
using KooliProjekt.Application.Infrastructure.Paging;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.Public
{
    /// <summary>
    /// Notifications of the signed in user.
    /// </summary>
    public class ListMyNotificationsQuery : IRequest<OperationResult<PagedResult<MyNotificationDto>>>
    {
        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public bool UnreadOnly { get; set; }
    }
}
