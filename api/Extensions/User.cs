using API.Contracts;
using Microsoft.AspNetCore.Identity;

namespace API
{
    public static class UserExtensions
    {
        public static IdentityUser Update(this IdentityUser user, UserUpdateRequest model)
        {
            var type = typeof(UserUpdateRequest);
            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                var value = property.GetValue(model);
                if (value != null)
                {
                    typeof(IdentityUser).GetProperty(property.Name).SetValue(user, value);
                }
            }

            return user;
        }
    }
}