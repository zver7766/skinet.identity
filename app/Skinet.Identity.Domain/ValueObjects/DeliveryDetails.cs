using System.Collections.Generic;
using Skinet.Identity.Domain.Entities;

namespace Skinet.Identity.Domain.ValueObjects
{
    public class DeliveryDetails : ValueObject
    {
        public string Street { get; }
        public string City { get; }
        public string State { get; }
        public string ZipCode { get; }

        private DeliveryDetails(
            string street, 
            string city,
            string state,
            string zipCode)
        {
            Street = street;
            City = city;
            State = state;
            ZipCode = zipCode;
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Street;
            yield return City;
            yield return State;
            yield return ZipCode;
        }
        
        public static Result<DeliveryDetails> Create(string street, string city, string state, string zipCode)
        {
            street = (street ?? string.Empty).Trim();
            city = (city ?? string.Empty).Trim();
            state = (state ?? string.Empty).Trim();
            zipCode = (zipCode ?? string.Empty).Trim();

            if (street.Length == 0)
            {
                return Result.Fail<DeliveryDetails>("Street cannot be empty");
            }            
            if (street.Length > 100)
            {
                return Result.Fail<DeliveryDetails>("Street is too long");
            }
            
            if (city.Length == 0)
            {
                return Result.Fail<DeliveryDetails>("City cannot be empty");
            }            
            if (city.Length > 100)
            {
                return Result.Fail<DeliveryDetails>("City is too long");
            }
            
            if (state.Length == 0)
            {
                return Result.Fail<DeliveryDetails>("State cannot be empty");
            }            
            if (state.Length > 100)
            {
                return Result.Fail<DeliveryDetails>("State is too long");
            }
            
            if (zipCode.Length == 0)
            {
                return Result.Fail<DeliveryDetails>("Zip Code cannot be empty");
            }            
            if (zipCode.Length > 100)
            {
                return Result.Fail<DeliveryDetails>("Zip Code is too long");
            }
            
            return Result.Ok(new DeliveryDetails(street, city, state, zipCode));
        }
    }
}