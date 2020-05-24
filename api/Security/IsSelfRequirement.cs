using System.Linq;
using System.Threading.Tasks;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace API.Security
{
    public class IsSelfRequirement : IAuthorizationRequirement
    {

    }

    public class IsSelfRequirementHandler : AuthorizationHandler<IsSelfRequirement>
    {
        IHttpContextAccessor _httpContextAccessor;
        public IsSelfRequirementHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsSelfRequirement requirement)
        {
            var targetedUserId = _httpContextAccessor.HttpContext.Request.RouteValues["id"].ToString();
            var loggedInUserId = context.User.Claims.FirstOrDefault(x => x.Type.Equals(Contracts.Claims.UserId))?.Value;

            if (string.IsNullOrEmpty(targetedUserId) || string.IsNullOrEmpty(loggedInUserId))
            {
                return;
            }

            if (targetedUserId == loggedInUserId)
            {
                context.Succeed(requirement);
            }

            return;
        }
    }
}