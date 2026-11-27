using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Authentication;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Features.Public
{
    public class ChangeMyPasswordCommandHandler : IRequestHandler<ChangeMyPasswordCommand, OperationResult<int>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly CurrentCustomer _currentCustomer;

        public ChangeMyPasswordCommandHandler(ApplicationDbContext dbContext, CurrentCustomer currentCustomer)
        {
            _dbContext = dbContext;
            _currentCustomer = currentCustomer;
        }

        public async Task<OperationResult<int>> Handle(ChangeMyPasswordCommand request, CancellationToken cancellationToken)
        {
            var response = new OperationResult<int>();

            var customer = await _currentCustomer.GetForUpdateAsync(cancellationToken);

            if (customer == null)
            {
                return response.AddError(PublicMessages.CustomerNotFound);
            }

            if (!PasswordHelper.Verify(request.CurrentPassword, customer.PasswordHash))
            {
                return response.AddError("Praegune parool ei ole õige.");
            }

            var passwordHash = PasswordHelper.Hash(request.NewPassword);
            customer.PasswordHash = passwordHash;

            var user = await _dbContext.Users
                .FirstOrDefaultAsync(x => x.CustomerId == customer.Id, cancellationToken);

            if (user != null)
            {
                user.PasswordHash = passwordHash;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            response.Value = customer.Id;

            return response;
        }
    }
}
