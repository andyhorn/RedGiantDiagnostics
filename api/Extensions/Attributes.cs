using System.Linq;
using API.Services;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API
{
    public class AdminOrCurrentUserAttribute : ActionFilterAttribute
    {
        private IIdentityService _identityService;
        
        public AdminOrCurrentUserAttribute(IIdentityService identity)
        {
            _identityService = identity;
        }

        public async override void OnActionExecuting(ActionExecutingContext context)
        {
            var currentUser = await context.HttpContext.CurrentUser(_identityService);
            var requestUserId = (string)context.RouteData.Values["id"];

            var roles = await _identityService.GetUserRolesAsync(currentUser);

            if (currentUser.Id != requestUserId && !roles.Contains(Contracts.Roles.Admin))
            {
                context.HttpContext.Response.StatusCode = 403;
            }
        }
    }
}