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
    public class ListMyOrdersQueryHandler : IRequestHandler<ListMyOrdersQuery, OperationResult<PagedResult<MyOrderListItemDto>>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly CurrentCustomer _currentCustomer;

        public ListMyOrdersQueryHandler(ApplicationDbContext dbContext, CurrentCustomer currentCustomer)
        {
            _dbContext = dbContext;
            _currentCustomer = currentCustomer;
        }

        public async Task<OperationResult<PagedResult<MyOrderListItemDto>>> Handle(ListMyOrdersQuery request, CancellationToken cancellationToken)
        {
            var response = new OperationResult<PagedResult<MyOrderListItemDto>>();

            var customer = await _currentCustomer.GetAsync(cancellationToken);

            if (customer == null)
            {
                return response.AddError(PublicMessages.CustomerNotFound);
            }

            response.Value = await _dbContext.Orders
                .AsNoTracking()
                .Where(x => x.CustomerId == customer.Id)
                .OrderByDescending(x => x.OrderDate)
                .Select(x => new MyOrderListItemDto
                {
                    Id = x.Id,
                    OrderNumber = x.OrderNumber,
                    OrderDate = x.OrderDate,
                    Status = x.Status,
                    Amount = x.Amount,
                    LineCount = _dbContext.OrderLines.Count(l => l.OrderId == x.Id)
                })
                .GetPagedAsync(request.Page, request.PageSize);

            return response;
        }
    }
}
