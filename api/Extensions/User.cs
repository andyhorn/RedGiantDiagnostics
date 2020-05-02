using System.Linq;
using API.Contracts;
using API.Contracts.Requests;
using API.Contracts.Requests.Admin;
using Microsoft.AspNetCore.Identity;

namespace API
{
    public static class UserExtensions
    {
        public static IdentityUser Map<T>(this IdentityUser user, T model) where T : class
        {
            var type = typeof(T);
            var properties = type.GetProperties();
            var identityType = typeof(IdentityUser);
            var identityProperties = identityType.GetProperties();

            foreach (var property in properties)
            {
                if (!identityProperties.Contains(property))
                {
                    continue;
                }

                var value = property.GetValue(model);
                if (value != null)
                {
                    identityType.GetProperty(property.Name).SetValue(user, value);
                }
            }

            return user;
        }
    }
}