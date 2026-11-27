using System;
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
    public class ListMyNotificationsQueryHandler : IRequestHandler<ListMyNotificationsQuery, OperationResult<PagedResult<MyNotificationDto>>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly CurrentCustomer _currentCustomer;

        public ListMyNotificationsQueryHandler(ApplicationDbContext dbContext, CurrentCustomer currentCustomer)
        {
            _dbContext = dbContext;
            _currentCustomer = currentCustomer;
        }

        public async Task<OperationResult<PagedResult<MyNotificationDto>>> Handle(ListMyNotificationsQuery request, CancellationToken cancellationToken)
        {
            var response = new OperationResult<PagedResult<MyNotificationDto>>();

            var customer = await _currentCustomer.GetAsync(cancellationToken);

            if (customer == null)
            {
                return response.AddError(PublicMessages.CustomerNotFound);
            }

            var query = _dbContext.Notifications
                .AsNoTracking()
                .Where(x => x.CustomerId == customer.Id);

            if (request.UnreadOnly)
            {
                query = query.Where(x => x.ReadAt == DateTime.MinValue);
            }

            var page = await query
                .OrderByDescending(x => x.CreatedAt)
                .GetPagedAsync(request.Page, request.PageSize);

            response.Value = new PagedResult<MyNotificationDto>
            {
                CurrentPage = page.CurrentPage,
                PageCount = page.PageCount,
                PageSize = page.PageSize,
                RowCount = page.RowCount,
                Results = page.Results
                    .Select(x => new MyNotificationDto
                    {
                        Id = x.Id,
                        Type = x.Type,
                        Title = x.Title,
                        Message = x.Message,
                        CreatedAt = x.CreatedAt,
                        IsRead = x.ReadAt != DateTime.MinValue,
                        ReadAt = x.ReadAt == DateTime.MinValue ? (DateTime?)null : x.ReadAt
                    })
                    .ToList()
            };

            return response;
        }
    }
}
