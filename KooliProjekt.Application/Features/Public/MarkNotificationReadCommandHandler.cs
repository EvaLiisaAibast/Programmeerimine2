using System;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Features.Public
{
    public class MarkNotificationReadCommandHandler : IRequestHandler<MarkNotificationReadCommand, OperationResult<int>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly CurrentCustomer _currentCustomer;

        public MarkNotificationReadCommandHandler(ApplicationDbContext dbContext, CurrentCustomer currentCustomer)
        {
            _dbContext = dbContext;
            _currentCustomer = currentCustomer;
        }

        public async Task<OperationResult<int>> Handle(MarkNotificationReadCommand request, CancellationToken cancellationToken)
        {
            var response = new OperationResult<int>();

            var customer = await _currentCustomer.GetAsync(cancellationToken);

            if (customer == null)
            {
                return response.AddError(PublicMessages.CustomerNotFound);
            }

            var notification = await _dbContext.Notifications
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.CustomerId == customer.Id, cancellationToken);

            if (notification == null)
            {
                return response.AddError("Teavitust ei leitud.");
            }

            notification.ReadAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(cancellationToken);

            response.Value = notification.Id;

            return response;
        }
    }
}
