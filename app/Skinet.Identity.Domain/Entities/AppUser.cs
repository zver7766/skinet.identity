using Microsoft.AspNetCore.Identity;

namespace Skinet.Identity.Domain.Entities
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; }
        public Address Address { get; set; }

        public void ChangeAddress(Address address)
        {
            Address = address;
        }
    public AppUser() { }
    }
}