using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace API.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IdentityService(UserManager<IdentityUser> user, SignInManager<IdentityUser> signIn, RoleManager<IdentityRole> role)
        {
            _userManager = user;
            _signInManager = signIn;
            _roleManager = role;
        }
        public Task<IdentityUser> CreateUserAsync(string email, string password)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteUserAsync(string id)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<IdentityUser>> GetAllUsersAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<IdentityUser> GetUserByIdAsync(string id)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateUserAsync(IdentityUser update)
        {
            throw new System.NotImplementedException();
        }
    }
}