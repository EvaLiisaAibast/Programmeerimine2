using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Features.Public
{
    public class UpdateMyProfileCommandHandler : IRequestHandler<UpdateMyProfileCommand, OperationResult<int>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly CurrentCustomer _currentCustomer;

        public UpdateMyProfileCommandHandler(ApplicationDbContext dbContext, CurrentCustomer currentCustomer)
        {
            _dbContext = dbContext;
            _currentCustomer = currentCustomer;
        }

        public async Task<OperationResult<int>> Handle(UpdateMyProfileCommand request, CancellationToken cancellationToken)
        {
            var response = new OperationResult<int>();

            var customer = await _currentCustomer.GetForUpdateAsync(cancellationToken);

            if (customer == null)
            {
                return response.AddError(PublicMessages.CustomerNotFound);
            }

            var email = request.Email.Trim().ToLowerInvariant();
            var oldEmail = customer.Email;

            if (email != oldEmail)
            {
                var emailTaken = await _dbContext.Customers
                    .AnyAsync(x => x.Email == email && x.Id != customer.Id, cancellationToken);

                if (emailTaken)
                {
                    return response.AddError("See e-posti aadress on juba kasutusel.");
                }

                customer.Email = email;
            }

            customer.FirstName = request.FirstName.Trim();
            customer.LastName = request.LastName.Trim();
            customer.Phone = request.Phone;

            // The login name is the e-mail address, so it has to follow the new address.
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(x => x.CustomerId == customer.Id, cancellationToken);

            if (user != null && user.UserName == oldEmail)
            {
                user.UserName = email;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            response.Value = customer.Id;

            return response;
        }
    }
}
