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
    public class ListMyReturnsQueryHandler : IRequestHandler<ListMyReturnsQuery, OperationResult<IList<MyReturnDto>>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly CurrentCustomer _currentCustomer;

        public ListMyReturnsQueryHandler(ApplicationDbContext dbContext, CurrentCustomer currentCustomer)
        {
            _dbContext = dbContext;
            _currentCustomer = currentCustomer;
        }

        public async Task<OperationResult<IList<MyReturnDto>>> Handle(ListMyReturnsQuery request, CancellationToken cancellationToken)
        {
            var response = new OperationResult<IList<MyReturnDto>>();

            var customer = await _currentCustomer.GetAsync(cancellationToken);

            if (customer == null)
            {
                return response.AddError(PublicMessages.CustomerNotFound);
            }

            var returns = await (from returnRequest in _dbContext.ReturnRequests.AsNoTracking()
                                 join order in _dbContext.Orders on returnRequest.OrderId equals order.Id
                                 where order.CustomerId == customer.Id
                                 orderby returnRequest.CreatedAt descending
                                 select new MyReturnDto
                                 {
                                     Id = returnRequest.Id,
                                     OrderId = returnRequest.OrderId,
                                     OrderNumber = order.OrderNumber,
                                     Status = returnRequest.Status,
                                     Reason = returnRequest.Reason,
                                     CreatedAt = returnRequest.CreatedAt
                                 })
                                 .ToListAsync(cancellationToken);

            if (returns.Count == 0)
            {
                response.Value = returns;

                return response;
            }

            var returnIds = returns.Select(x => x.Id).ToList();

            var lines = await (from returnLine in _dbContext.ReturnLines.AsNoTracking()
                               join orderLine in _dbContext.OrderLines on returnLine.OrderLineId equals orderLine.Id
                               where returnIds.Contains(returnLine.ReturnRequestId)
                               select new
                               {
                                   returnLine.ReturnRequestId,
                                   Line = new MyReturnLineDto
                                   {
                                       Id = returnLine.Id,
                                       ProductName = orderLine.ProductName,
                                       Quantity = returnLine.Quantity,
                                       Reason = returnLine.Reason
                                   }
                               })
                               .ToListAsync(cancellationToken);

            foreach (var group in lines.GroupBy(x => x.ReturnRequestId))
            {
                var target = returns.FirstOrDefault(x => x.Id == group.Key);

                if (target == null)
                {
                    continue;
                }

                foreach (var row in group)
                {
                    target.Lines.Add(row.Line);
                }
            }

            response.Value = returns;

            return response;
        }
    }
}
