using System.Collections.Generic;
using System.Threading.Tasks;
using API.Contracts.Requests;
using API.Contracts.Requests.Admin;
using API.Entities;
using API.Models;

namespace API.Services
{
    public interface ILogsService
    {
        Task<LogFile> CreateAsync(LogFile log);
        Task<IEnumerable<LogFile>> GetAllLogsAsync();
        Task<LogFile> GetByIdAsync(string id);
        Task<bool> LogExists(string id);
        Task<IEnumerable<LogFile>> GetForUserAsync(string userId);
        Task<LogFile> UpdateAsync(string id, LogUpdateRequest update);
        // Task<LogFile> UpdateAsync(string id, AdminLogUpdateRequest update);
        Task DeleteAsync(string id);
        LogFile Parse(string data);
    }
}