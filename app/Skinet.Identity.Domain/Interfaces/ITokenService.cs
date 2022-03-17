using Skinet.Identity.Domain.Entities;

namespace Skinet.Identity.Domain.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}