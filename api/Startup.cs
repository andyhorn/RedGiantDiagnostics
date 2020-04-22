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

            // DI the Logs Repository
            services.AddScoped<ILogsRepository, LogsRepository>();

            // DI the Logs Service
            services.AddScoped<ILogsService, LogsService>();

            // DI the Logs MongoDB Context
            services.AddScoped<IDataContext, DataContext>();

            // DI the ILogFactory interface
            services.AddScoped<ILogFactory, LogFactory>();

            // DI the IFileService interface
            services.AddScoped<IFileService, FileService>();
            
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

            app.UseCors();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
