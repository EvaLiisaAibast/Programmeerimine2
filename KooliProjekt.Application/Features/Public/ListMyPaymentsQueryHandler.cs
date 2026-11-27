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
    public class ListMyPaymentsQueryHandler : IRequestHandler<ListMyPaymentsQuery, OperationResult<PagedResult<MyPaymentDto>>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly CurrentCustomer _currentCustomer;

        public ListMyPaymentsQueryHandler(ApplicationDbContext dbContext, CurrentCustomer currentCustomer)
        {
            _dbContext = dbContext;
            _currentCustomer = currentCustomer;
        }

        public async Task<OperationResult<PagedResult<MyPaymentDto>>> Handle(ListMyPaymentsQuery request, CancellationToken cancellationToken)
        {
            var response = new OperationResult<PagedResult<MyPaymentDto>>();

            var customer = await _currentCustomer.GetAsync(cancellationToken);

            if (customer == null)
            {
                return response.AddError(PublicMessages.CustomerNotFound);
            }

            var customerOrderIds = _dbContext.Orders
                .Where(x => x.CustomerId == customer.Id)
                .Select(x => x.Id);

            response.Value = await _dbContext.Payments
                .AsNoTracking()
                .Where(x => customerOrderIds.Contains(x.OrderId))
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new MyPaymentDto
                {
                    Id = x.Id,
                    Amount = x.Amount,
                    Method = x.Method,
                    Status = x.Status,
                    TransactionReference = x.TransactionReference,
                    CreatedAt = x.CreatedAt
                })
                .GetPagedAsync(request.Page, request.PageSize);

            return response;
        }
    }
}
