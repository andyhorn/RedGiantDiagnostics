using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace API.Security
{
    public class SelfOwnedResourceExclusionHandler 
        : AuthorizationHandler<SelfOwnedResourceExclusionRequirement>
    {
        private IHttpContextAccessor _httpContextAccessor;

        public SelfOwnedResourceExclusionHandler(IHttpContextAccessor http)
        {
            _httpContextAccessor = http;
        }

        protected override Task HandleRequirementAsync
            (AuthorizationHandlerContext context, 
            SelfOwnedResourceExclusionRequirement requirement)
        {
            // Get the current user's ID
            var currentUser = _httpContextAccessor.HttpContext.User;
            var currentUserId = currentUser.Claims.FirstOrDefault(x => x.Type.Equals(Contracts.Claims.UserId)).Value;

            // Get the target ID
            var targetUserId = _httpContextAccessor.HttpContext.Request.RouteValues["id"].ToString();

            // If they are not the same ID, authorize the request
            if (!currentUserId.Equals(targetUserId))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}