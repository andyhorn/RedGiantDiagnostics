using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.SetupDependencyInjection();

            services.SetupIdentityDatabaseConnection();

            services.SetupIdentity();

            services.SetupAuthentication();

            services.SetupSecurityPolicies();

            services.AddControllers();

            services.AddMvcCore().AddDataAnnotations();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // app.UseHttpsRedirection();

            app.UseCors(Contracts.Policies.CorsPolicy);

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                // Any requests not containing "/api" should be returned the index file
                endpoints.MapFallbackToFile("index.html");
            });

            app.UseDefaultFiles();
            app.UseStaticFiles();

            DbInitializer.SeedIdentity(userManager, roleManager);
        }
    }
}
