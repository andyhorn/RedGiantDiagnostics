using System.Collections.Generic;
using System.Threading.Tasks;
using API.Entities;
using API.Models;

namespace API.Services
{
    public interface ILogsService
    {
        Task<LogFile> CreateAsync(LogFile log);
        Task<IEnumerable<LogFile>> GetAllLogsAsync();
        Task<LogFile> GetByIdAsync(string id);
        Task<IEnumerable<LogFile>> GetForUserAsync(string userId);
        Task<LogFile> UpdateAsync(LogFile update);
        Task DeleteAsync(string id);
        LogFile Parse(string data);
    }
}