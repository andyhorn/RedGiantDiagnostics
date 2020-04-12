using System.Collections.Generic;
using System.Threading.Tasks;
using API.Models;

namespace API.Services
{
    public interface ILogService
    {
        Task<ILogFile> CreateAsync(ILogFile log);
        Task<IEnumerable<ILogFile>> GetAllLogsAsync();
        Task<ILogFile> GetByIdAsync(string id);
        Task<IEnumerable<ILogFile>> GetForUserAsync(string userId);
        Task<bool> DeleteAsync(string id);
        ILogFile Parse(string data);
    }
}