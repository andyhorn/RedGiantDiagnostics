using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace API.Services
{
    public interface IIdentityService
    {
        Task<IdentityUser> GetUserByIdAsync(string id);
        Task<IdentityUser> GetUserByEmailAsync(string email);
        Task<IEnumerable<IdentityUser>> GetAllUsersAsync();
        Task DeleteUserAsync(string id);
        Task UpdateUserAsync(IdentityUser update);
        Task<IdentityUser> CreateUserAsync(string email, string password);
        Task<string> LoginAsync(string email, string password);
        Task<IdentityUser> GetUserFromToken(string jwt);
    }
}