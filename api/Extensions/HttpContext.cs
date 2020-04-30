using System.Collections.Generic;
using System.Linq;
using API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Identity;

namespace API
{
    public static class HttpContextExtensions
    {
        public static async System.Threading.Tasks.Task<IdentityUser> CurrentUser(this HttpContext http, IIdentityService identity)
        {
            var auth = http.Request.Headers.SingleOrDefault(x => x.Key.Equals("Authorization"));
            if (auth.Equals(new KeyValuePair<string, StringValues>()))
            {
                return null;
            }

            var token = auth.Value.ToString().Substring("Bearer ".Length);
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            var user = await identity.GetUserFromToken(token);
            return user;
        }
    }
}