using Microsoft.AspNetCore.Identity;

namespace API.Services
{
    public interface ITokenService
    {
        string MakeToken(IdentityUser user);
        bool AuthenticateToken(string token);
        string GetUserId(string token);
    }
}