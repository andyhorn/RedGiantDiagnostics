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
                    policy.RequireRole(Contracts.Roles.Admin);
                });
                options.AddPolicy(Contracts.Policies.ResourceOwnerPolicy, policy => {
                    policy.AddRequirements(
                        new ResourceOwnerRequirement()
                    );
                });
            });

            services.AddScoped<IAuthorizationHandler, CanAccessOwnedResourceHandler>();
        }
    }
}