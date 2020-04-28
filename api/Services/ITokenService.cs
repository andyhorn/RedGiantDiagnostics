using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace API.Services
{
    public interface ITokenService
    {
        Task<string> MakeToken(IdentityUser user);
        string GetUserId(string token);
    }
}