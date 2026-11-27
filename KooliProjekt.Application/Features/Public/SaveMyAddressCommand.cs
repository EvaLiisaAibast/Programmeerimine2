using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.Public
{
    /// <summary>
    /// Adds a new address to the profile or updates an existing one.
    /// </summary>
    public class SaveMyAddressCommand : IRequest<OperationResult<int>>
    {
        public int Id { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public string PostalCode { get; set; }

        public string Street { get; set; }

        public string HouseNumber { get; set; }

        public string Apartment { get; set; }

        public bool IsDefault { get; set; }
    }
}
