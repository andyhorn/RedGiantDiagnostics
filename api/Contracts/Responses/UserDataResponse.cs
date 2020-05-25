using Microsoft.AspNetCore.Identity;

namespace API.Contracts.Responses
{
    public class UserDataResponse
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string[] Roles { get; set; }

        public UserDataResponse(IdentityUser user)
        {
            UserId = user.Id;
            Email = user.Email;
        }
    }
}