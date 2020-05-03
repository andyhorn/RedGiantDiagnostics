using API.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace API
{
    public static class SecurityExtensions
    {
        public static void SetupSecurityPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options => {
                options.AddPolicy(Contracts.Policies.AdministrativeAccessPolicy, policy => {
                    policy.AddRequirements(
                        new AdministrativeRightsRequirement()
                    );
                });
                options.AddPolicy(Contracts.Policies.ResourceOwnerPolicy, policy => {
                    policy.AddRequirements(
                        new ResourceOwnerRequirement()
                    );
                });
                options.AddPolicy(Contracts.Policies.RoleChangePolicy, policy => {
                    policy.AddRequirements(
                        new RoleChangeRequirement()
                    );
                });
            });

            services.AddScoped<IAuthorizationHandler, CannotChangeOwnRolesHandler>();
            services.AddScoped<IAuthorizationHandler, CanAccessOwnedResourceHandler>();
            services.AddScoped<IAuthorizationHandler, CanAccessAdministrativeResourcesHandler>();
        }
    }
}