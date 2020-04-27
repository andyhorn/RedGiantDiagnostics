using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace API.Services
{
    public class IdentityService : IIdentityService
    {
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