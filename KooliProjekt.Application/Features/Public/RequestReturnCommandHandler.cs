using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Features.Public
{
    public class RequestReturnCommandHandler : IRequestHandler<RequestReturnCommand, OperationResult<int>>
    {
        private const string RequestedStatus = "Taotletud";

        private readonly ApplicationDbContext _dbContext;
        private readonly CurrentCustomer _currentCustomer;

        public RequestReturnCommandHandler(ApplicationDbContext dbContext, CurrentCustomer currentCustomer)
        {
            _dbContext = dbContext;
            _currentCustomer = currentCustomer;
        }

        public async Task<OperationResult<int>> Handle(RequestReturnCommand request, CancellationToken cancellationToken)
        {
            var response = new OperationResult<int>();

            var customer = await _currentCustomer.GetAsync(cancellationToken);

            if (customer == null)
            {
                return response.AddError(PublicMessages.CustomerNotFound);
            }

            var line = await (from orderLine in _dbContext.OrderLines
                              join order in _dbContext.Orders on orderLine.OrderId equals order.Id
                              where orderLine.Id == request.OrderLineId && order.CustomerId == customer.Id
                              select new
                              {
                                  orderLine.Id,
                                  orderLine.OrderId,
                                  orderLine.Quantity,
                                  order.Status,
                                  order.OrderNumber
                              })
                              .FirstOrDefaultAsync(cancellationToken);

            if (line == null)
            {
                return response.AddError("Tellimuse rida ei leitud.");
            }

            if (line.Status == "pending")
            {
                return response.AddError("Tellimust ei saa veel tagastada, sest see ei ole veel tasutud.");
            }

            var alreadyReturned = await (from returnLine in _dbContext.ReturnLines
                                         join returnRequest in _dbContext.ReturnRequests on returnLine.ReturnRequestId equals returnRequest.Id
                                         where returnRequest.OrderId == line.OrderId && returnLine.OrderLineId == line.Id
                                         select returnLine.Quantity)
                                         .SumAsync(cancellationToken);

            var availableToReturn = line.Quantity - alreadyReturned;

            if (request.Quantity > availableToReturn)
            {
                return response.AddError("Tagastada saab kuni " + availableToReturn + " tk.");
            }

            var returnRequestEntity = await _dbContext.ReturnRequests
                .Where(x => x.OrderId == line.OrderId && x.Status == RequestedStatus)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (returnRequestEntity == null)
            {
                returnRequestEntity = new ReturnRequest
                {
                    OrderId = line.OrderId,
                    Status = RequestedStatus,
                    Reason = request.Reason,
                    CreatedAt = DateTime.UtcNow
                };

                _dbContext.ReturnRequests.Add(returnRequestEntity);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            _dbContext.ReturnLines.Add(new ReturnLine
            {
                ReturnRequestId = returnRequestEntity.Id,
                OrderLineId = line.Id,
                Quantity = request.Quantity,
                Reason = request.Reason
            });

            await _dbContext.SaveChangesAsync(cancellationToken);

            response.Value = returnRequestEntity.Id;

            return response;
        }
    }
}
