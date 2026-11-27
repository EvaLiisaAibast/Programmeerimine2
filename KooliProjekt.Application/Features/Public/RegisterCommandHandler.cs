using System;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Authentication;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Features.Public
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, OperationResult<int>>
    {
        private readonly ApplicationDbContext _dbContext;

        public RegisterCommandHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OperationResult<int>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var response = new OperationResult<int>();

            var email = request.Email.Trim().ToLowerInvariant();

            var emailTaken = await _dbContext.Customers
                .AnyAsync(x => x.Email == email, cancellationToken);

            if (emailTaken)
            {
                return response.AddError("Selle e-posti aadressiga klient on juba olemas.");
            }

            var userNameTaken = await _dbContext.Users
                .AnyAsync(x => x.UserName == email, cancellationToken);

            if (userNameTaken)
            {
                return response.AddError("See kasutajanimi on juba kasutusel.");
            }

            var passwordHash = PasswordHelper.Hash(request.Password);
            var now = DateTime.UtcNow;

            var customer = new Customer
            {
                Email = email,
                PasswordHash = passwordHash,
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Phone = request.Phone,
                Status = "Aktiivne",
                CreatedAt = now,
                LastLogin = now
            };

            _dbContext.Customers.Add(customer);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var user = new User
            {
                UserName = email,
                PasswordHash = passwordHash,
                IsAdmin = false,
                CustomerId = customer.Id
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync(cancellationToken);

            response.Value = customer.Id;

            return response;
        }
    }
}
