using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Skinet.Identity.Domain.Entities;
using Skinet.Identity.Domain.ValueObjects;

namespace Skinet.Identity.Infrastructure.Identity
{
    public class AppIdentityDbContextSeed
    {
        public static async Task SeedUsersAsync(UserManager<AppUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var users = new AppUser
                {
                    DisplayName = "Bob",
                    Email = "bob@test.com",
                    UserName = "bob@test.com",
                    Address = new Address(
                        UserName.Create("Bob", "Bobbity").Value,
                        DeliveryDetails.Create("10 The Street", "New York", "NY", "90210").Value)
                };

                await userManager.CreateAsync(users, "Pa$$w0rd");
            }
        }
    }
}