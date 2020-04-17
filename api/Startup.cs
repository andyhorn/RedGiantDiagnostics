using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Factories;
using API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
            // Set up the Logs Database Settings for Dependency Injection
            services.AddSingleton<ILogsDatabaseSettings>(service => service.GetRequiredService<IOptions<LogsDatabaseSettings>>().Value);

            // Add a singleton of the Logs Repository
            services.AddScoped<ILogsRepository, LogsRepository>();
            // services.AddSingleton<ILogsRepository>(service => service.GetRequiredService<LogsRepository>());

            // Add a singleton of the Logs Service
            services.AddScoped<ILogsService, LogsService>();
            // services.AddSingleton<ILogsService>(service => service.GetRequiredService<LogsService>());

            // Add a singleton of the Logs MongoDB Context
            services.AddScoped<IDataContext, DataContext>();
            // services.AddSingleton<IDataContext>(service => service.GetRequiredService<DataContext>());

            // Add a singleton of the ILogFactory interface
            services.AddScoped<ILogFactory, LogFactory>();
            // services.AddSingleton<ILogFactory>(service => service.GetRequiredService<LogFactory>());
            
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
