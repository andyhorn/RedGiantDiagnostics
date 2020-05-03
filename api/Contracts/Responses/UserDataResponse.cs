using Microsoft.AspNetCore.Identity;

namespace API.Contracts.Responses
{
    public class UserDataResponse
    {
        public string UserId { get; set; }
        public string Email { get; set; }

        public UserDataResponse()
        {

        }

        public UserDataResponse(IdentityUser user)
        {
            UserId = user.Id;
            Email = user.Email;
        }
    }
}