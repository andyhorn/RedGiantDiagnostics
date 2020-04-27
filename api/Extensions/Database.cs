using System;
using API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API
{
    public static class DatabaseExtensions
    {
        public static void SetupIdentityDatabaseConnection(this IServiceCollection services)
        {
            var host = Environment.GetEnvironmentVariable("DB_IDENTITY_HOST");
            var username = Environment.GetEnvironmentVariable("DB_USER");
            var password = Environment.GetEnvironmentVariable("DB_PASSWORD");
            var database = Environment.GetEnvironmentVariable("DB_DATABASE");
            var port = Environment.GetEnvironmentVariable("DB_IDENTITY_PORT");

            var connectionString = $"Server={host};Port={port};Database={database};Username={username};Password={password}";

            services.AddDbContext<IdentityContext>(
                options => options.UseNpgsql(connectionString)
            );
        }
    }
}