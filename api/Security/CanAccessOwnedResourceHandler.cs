using System.Linq;
using System.Threading.Tasks;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Security
{
    public class CanAccessOwnedResourceHandler : AuthorizationHandler<ResourceOwnerRequirement>
    {
        private IIdentityService _identityService;
        private ILogsService _logsService;

        public CanAccessOwnedResourceHandler(IIdentityService identity, ILogsService logs)
        {
            _identityService = identity;
            _logsService = logs;
        }

        protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context, 
            ResourceOwnerRequirement requirement)
        {
            var filterContext = context.Resource as AuthorizationFilterContext;
            if (filterContext == null)
            {
                return;
            }

            var logId = filterContext.HttpContext.Request.Query["id"].ToString();
            var userId = context.User.Claims.FirstOrDefault(x => x.Type.Equals(Contracts.Claims.UserId)).Value;

            var log = await _logsService.GetByIdAsync(logId);

            if (log.OwnerId == userId)
            {
                context.Succeed(requirement);
            }

            return;
        }
    }
}