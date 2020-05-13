using API.Services;
using Microsoft.AspNetCore.Identity;

namespace API.Contracts.Responses
{
    public class UserDataResponse
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string[] Roles { get; set; }

        public UserDataResponse()
        {
            
        }

        public UserDataResponse(IdentityUser user)
        {
            UserId = user.Id;
            Email = user.Email;
        }

        public UserDataResponse(IdentityUser user, IIdentityService identity)
        {
            UserId = user.Id;
            Email = user.Email;
            Roles = user.GetRoles(identity);
        }
    }
}