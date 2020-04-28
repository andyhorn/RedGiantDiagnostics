using Microsoft.AspNetCore.Identity;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        public bool AuthenticateToken(string token)
        {
            throw new System.NotImplementedException();
        }

        public string GetUserId(string token)
        {
            throw new System.NotImplementedException();
        }

        public string MakeToken(IdentityUser user)
        {
            throw new System.NotImplementedException();
        }
    }
}