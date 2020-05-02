using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Security
{
    public class CanAccessAdministrativeResourcesHandler
        : AuthorizationHandler<AdministrativeRightsRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdministrativeRightsRequirement requirement)
        {
            var isAdmin = context.User.IsInRole(Contracts.Roles.Admin);

            if (isAdmin)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}