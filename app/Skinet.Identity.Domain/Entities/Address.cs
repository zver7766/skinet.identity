using Skinet.Identity.Domain.ValueObjects;

namespace Skinet.Identity.Domain.Entities
{
    public class Address : Entity<int>
    { 
        public UserName UserName { get; private set; }
        
        public DeliveryDetails DeliveryDetails { get; private set; }
        
        public Address(
            UserName userName,
            DeliveryDetails deliveryDetails
            )
        {
            UserName = userName;
            DeliveryDetails = deliveryDetails;
        }
        
        protected Address() { }

        
    }
}