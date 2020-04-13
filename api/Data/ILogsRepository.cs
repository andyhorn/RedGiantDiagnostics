using System.Collections.Generic;
using System.Threading.Tasks;
using API.Models;

namespace API.Data
{
    public interface ILogsRepository
    {
        Task<List<ILogFile>> GetAllLogsAsync();
        Task<ILogFile> GetByIdAsync(string id);
        Task<ILogFile> SaveAsync(ILogFile file);
        Task<ILogFile> UpdateAsync(ILogFile update);
        Task RemoveAsync(string id);
    }
}