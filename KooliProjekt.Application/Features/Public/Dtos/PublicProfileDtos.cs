using System;
using System.Collections.Generic;

namespace KooliProjekt.Application.Features.Public.Dtos
{
    public class MyProfileDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastLogin { get; set; }
        public IList<MyAddressDto> Addresses { get; set; }

        public MyProfileDto()
        {
            Addresses = new List<MyAddressDto>();
        }
    }

    public class MyAddressDto
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
