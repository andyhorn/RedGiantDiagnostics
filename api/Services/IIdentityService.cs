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
        void DeleteUserAsync(string id);
        void UpdateUserAsync(IdentityUser update);
        Task<IdentityUser> CreateUserAsync(string email, string password);
        string Login(string email, string password);
    }
}