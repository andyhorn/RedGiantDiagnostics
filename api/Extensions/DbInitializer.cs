using System;
using Microsoft.AspNetCore.Identity;

namespace API
{
    public static class DbInitializer
    {
        public static void SeedIdentity(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Seed the roles
            SeedRoles(roleManager);

            // Seed the default users
            SeedUsers(userManager, roleManager);
        }

        private static void SeedUsers(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var defaultEmail = GetDefaultEmail();
            var defaultPassword = GetDefaultPassword();

            if (string.IsNullOrEmpty(defaultEmail) || string.IsNullOrEmpty(defaultPassword))
                return;

            var defaultUser = userManager.FindByEmailAsync(defaultEmail).GetAwaiter().GetResult();

            if (defaultUser == null)
            {
                defaultUser = new IdentityUser
                {
                    Email = defaultEmail,
                    UserName = defaultEmail
                };

                userManager.CreateAsync(defaultUser, defaultPassword).Wait();
            }

            if (!userManager.IsInRoleAsync(defaultUser, Contracts.Roles.Admin).GetAwaiter().GetResult())
            {
                userManager.AddToRoleAsync(defaultUser, Contracts.Roles.Admin).Wait();
            }

            if (!userManager.IsInRoleAsync(defaultUser, Contracts.Roles.User).GetAwaiter().GetResult())
            {
                userManager.AddToRoleAsync(defaultUser, Contracts.Roles.User).Wait();
            }
        }

        private static string GetDefaultEmail()
        {
            return Environment.GetEnvironmentVariable("ASPNETCORE_DEFAULT_EMAIL");
        }

        private static string GetDefaultPassword()
        {
            return Environment.GetEnvironmentVariable("ASPNETCORE_DEFAULT_PASSWORD");
        }

        private static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync(Contracts.Roles.Admin).GetAwaiter().GetResult())
            {
                var adminRole = new IdentityRole(Contracts.Roles.Admin);

                roleManager.CreateAsync(adminRole).Wait();
            }

            if (!roleManager.RoleExistsAsync(Contracts.Roles.User).GetAwaiter().GetResult())
            {
                var userRole = new IdentityRole(Contracts.Roles.User);

                roleManager.CreateAsync(userRole).Wait();
            }
        }
    }
}