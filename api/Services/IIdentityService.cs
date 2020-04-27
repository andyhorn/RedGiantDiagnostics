using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace API.Services
{
    public interface IIdentityService
    {
        Task<IdentityUser> GetUserByIdAsync(string id);
        Task<IEnumerable<IdentityUser>> GetAllUsersAsync();
        Task DeleteUserAsync(string id);
        Task UpdateUserAsync(IdentityUser update);
        Task<IdentityUser> CreateUserAsync(string email, string password);
    }
}