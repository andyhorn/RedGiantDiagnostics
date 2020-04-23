using API.Data;
using API.Factories;
using API.Helpers;
using API.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace API
{
    public static class DependencyInjection
    {
        public static void SetupDependencyInjection(this IServiceCollection services)
        {
            // Set up the Logs Database Settings for Dependency Injection
            services.AddSingleton<ILogsDatabaseSettings>(service => service.GetRequiredService<IOptions<LogsDatabaseSettings>>().Value);

            // DI the Logs Repository
            services.AddScoped<ILogsRepository, LogsRepository>();

            // DI the Logs MongoDB Context
            services.AddScoped<IDataContext, DataContext>();

            InjectHelpers(services);

            InjectServices(services);

            InjectFactories(services);
        }

        private static void InjectHelpers(IServiceCollection services)
        {
            services.AddScoped<IUtilities, Utilities>();
        }

        private static void InjectServices(IServiceCollection services)
        {
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<ILogsService, LogsService>();
        }

        private static void InjectFactories(IServiceCollection services)
        {
            services.AddScoped<IDebugLogFactory, DebugLogFactory>();
            services.AddScoped<IIsvStatisticsFactory, IsvStatisticsFactory>();
            services.AddScoped<ILicenseFileFactory, LicenseFileFactory>();
            services.AddScoped<ILicensePoolFactory, LicensePoolFactory>();
            services.AddScoped<ILogFactory, LogFactory>();
            services.AddScoped<ILogParserFactory, LogParserFactory>();
            services.AddScoped<IProductLicenseFactory, ProductLicenseFactory>();
            services.AddScoped<IRlmInstanceFactory, RlmInstanceFactory>();
            services.AddScoped<IRlmStatisticsTableFactory, RlmStatisticsTableFactory>();
            services.AddScoped<IServerStatusFactory, ServerStatusFactory>();
        }
    }
}