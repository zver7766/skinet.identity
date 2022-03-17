using Skinet.Identity.Application.Dtos;
using Skinet.Identity.Domain.Entities;

namespace Skinet.Identity.Application.Mappings
{
    public static class AddressMapping
    {
        public static AddressDto ToDto(this Address address)
        {
            return new(
                firstName: address.UserName.FirstName,
                lastName: address.UserName.LastName,
                street: address.DeliveryDetails.Street,
                city: address.DeliveryDetails.City,
                state: address.DeliveryDetails.State,
                zipCode: address.DeliveryDetails.ZipCode
            );
        }
        
    }
}