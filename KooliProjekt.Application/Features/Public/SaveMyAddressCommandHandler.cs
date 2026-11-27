using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Features.Public
{
    public class SaveMyAddressCommandHandler : IRequestHandler<SaveMyAddressCommand, OperationResult<int>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly CurrentCustomer _currentCustomer;

        public SaveMyAddressCommandHandler(ApplicationDbContext dbContext, CurrentCustomer currentCustomer)
        {
            _dbContext = dbContext;
            _currentCustomer = currentCustomer;
        }

        public async Task<OperationResult<int>> Handle(SaveMyAddressCommand request, CancellationToken cancellationToken)
        {
            var response = new OperationResult<int>();

            var customer = await _currentCustomer.GetAsync(cancellationToken);

            if (customer == null)
            {
                return response.AddError(PublicMessages.CustomerNotFound);
            }

            Address entity;

            if (request.Id > 0)
            {
                entity = await _dbContext.Addresses
                    .FirstOrDefaultAsync(x => x.Id == request.Id && x.CustomerId == customer.Id, cancellationToken);

                if (entity == null)
                {
                    return response.AddError("Aadressi ei leitud.");
                }
            }
            else
            {
                entity = new Address
                {
                    CustomerId = customer.Id
                };

                _dbContext.Addresses.Add(entity);
            }

            entity.Country = request.Country.Trim();
            entity.City = request.City.Trim();
            entity.PostalCode = request.PostalCode.Trim();
            entity.Street = request.Street.Trim();
            entity.HouseNumber = request.HouseNumber.Trim();
            entity.Apartment = request.Apartment;
            entity.IsDefault = request.IsDefault;

            if (request.IsDefault)
            {
                var otherAddresses = await _dbContext.Addresses
                    .Where(x => x.CustomerId == customer.Id && x.Id != entity.Id)
                    .ToListAsync(cancellationToken);

                foreach (var address in otherAddresses)
                {
                    address.IsDefault = false;
                }
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            response.Value = entity.Id;

            return response;
        }
    }
}
