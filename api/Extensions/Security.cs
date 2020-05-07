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
                options.AddPolicy(Contracts.Policies.SelfOwnedResourceExclusionPolicy, policy => {
                    policy.AddRequirements(
                        new SelfOwnedResourceExclusionRequirement()
                    );
                });
            });

            services.AddCors(options => options.AddPolicy(Contracts.Policies.CorsPolicy, builder => {
                builder.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin();
            }));

            services.AddScoped<IAuthorizationHandler, SelfOwnedResourceExclusionHandler>();
            services.AddScoped<IAuthorizationHandler, CanAccessOwnedResourceHandler>();
            services.AddScoped<IAuthorizationHandler, CanAccessAdministrativeResourcesHandler>();
        }
    }
}