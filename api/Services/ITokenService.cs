using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public interface ITokenService
    {
        Task<string> MakeToken(IdentityUser user);
        string GetUserId(string jwt);
        bool IsValid(string jwt);
    }
}