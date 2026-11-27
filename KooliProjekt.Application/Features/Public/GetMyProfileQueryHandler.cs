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
    public class GetMyProfileQueryHandler : IRequestHandler<GetMyProfileQuery, OperationResult<MyProfileDto>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly CurrentCustomer _currentCustomer;

        public GetMyProfileQueryHandler(ApplicationDbContext dbContext, CurrentCustomer currentCustomer)
        {
            _dbContext = dbContext;
            _currentCustomer = currentCustomer;
        }

        public async Task<OperationResult<MyProfileDto>> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
        {
            var response = new OperationResult<MyProfileDto>();

            var customer = await _currentCustomer.GetAsync(cancellationToken);

            if (customer == null)
            {
                return response.AddError(PublicMessages.CustomerNotFound);
            }

            response.Value = new MyProfileDto
            {
                Id = customer.Id,
                Email = customer.Email,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Phone = customer.Phone,
                Status = customer.Status,
                CreatedAt = customer.CreatedAt,
                LastLogin = customer.LastLogin,
                Addresses = await _dbContext.Addresses
                    .AsNoTracking()
                    .Where(x => x.CustomerId == customer.Id)
                    .OrderByDescending(x => x.IsDefault)
                    .ThenBy(x => x.Id)
                    .Select(x => new MyAddressDto
                    {
                        Id = x.Id,
                        Country = x.Country,
                        City = x.City,
                        PostalCode = x.PostalCode,
                        Street = x.Street,
                        HouseNumber = x.HouseNumber,
                        Apartment = x.Apartment,
                        IsDefault = x.IsDefault
                    })
                    .ToListAsync(cancellationToken)
            };

            return response;
        }
    }
}
