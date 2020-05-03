using System.Linq;
using System.Threading.Tasks;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace API.Security
{
    public class CanAccessOwnedResourceHandler : AuthorizationHandler<ResourceOwnerRequirement>
    {
        private IIdentityService _identityService;
        private ILogsService _logsService;
        private IHttpContextAccessor _httpContext;

        public CanAccessOwnedResourceHandler(IIdentityService identity, ILogsService logs, IHttpContextAccessor httpContext)
        {
            _identityService = identity;
            _logsService = logs;
            _httpContext = httpContext;
        }

        protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context, 
            ResourceOwnerRequirement requirement)
        {
            var logId = _httpContext.HttpContext.Request.RouteValues["id"].ToString();
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