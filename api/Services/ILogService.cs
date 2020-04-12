using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services
{
    public interface ILogService
    {
        Task<bool> CreateAsync();
        Task<IEnumerable<int>> GetAllLogsAsync();
        Task GetByIdAsync(string id);
        Task GetForUserAsync(string userId);
        Task<bool> DeleteAsync(string id);
        bool Parse();
    }
}