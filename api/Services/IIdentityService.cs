using System.Collections.Generic;
using System.Threading.Tasks;
using API.Contracts;
using Microsoft.AspNetCore.Identity;

namespace API.Services
{
    public interface IIdentityService
    {
        Task<IdentityUser> GetUserByIdAsync(string id);
        Task<IdentityUser> GetUserByEmailAsync(string email);
        // Task<IEnumerable<IdentityUser>> GetAllUsersAsync();
        IEnumerable<IdentityUser> GetAllUsers();
        Task DeleteUserAsync(string id);
        Task UpdateUserAsync(IdentityUser update);
        Task<IdentityUser> CreateUserAsync(IdentityUser newUser, string password);
        Task<string> LoginAsync(string email, string password);
        Task SetUserPasswordAsync(IdentityUser user, string password);
        Task<bool> UserExistsWithIdAsync(string id);
        Task<bool> UserExistsWithEmailAsync(string email);
        Task<IdentityUser> GetUserFromToken(string jwt);
        Task<bool> ValidateTokenAsync(string jwt);
        Task SetUserRolesAsync(IdentityUser user, IEnumerable<string> roles);
        Task<bool> RoleExistsAsync(string role);
        Task<IdentityRole> GetRoleAsync(string role);
        Task CreateRoleAsync(string role);
        Task DeleteRoleAsync(string role);
        Task AddRoleToUserAsync(IdentityUser user, string role);
        Task RemoveRoleFromUserAsync(IdentityUser user, string role);
        Task<IEnumerable<string>> GetUserRolesAsync(IdentityUser user);
    }
}