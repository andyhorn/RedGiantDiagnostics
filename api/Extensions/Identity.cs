using API.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using API.Configuration;

namespace API
{
    public static class IdentityExtension
    {
        public static void SetupIdentity(this IServiceCollection services)
        {
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<IdentityContext>();
        }

        public static void SetupAuthentication(this IServiceCollection services)
        {
            var secret = Environment.GetEnvironmentVariable("TOKEN_SECRET");
            var key = Encoding.ASCII.GetBytes(secret);

            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options => {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = TokenValidationParametersFactory.Get(key);
            });
        }
    }
}