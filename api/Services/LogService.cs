using System.Collections.Generic;
using System.Threading.Tasks;
using API.Models;

namespace API.Services
{
    public class LogService : ILogService
    {
        public LogService()
        public Task<ILogFile> CreateAsync(ILogFile log)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> DeleteAsync(string id)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<ILogFile>> GetAllLogsAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<ILogFile> GetByIdAsync(string id)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<ILogFile>> GetForUserAsync(string userId)
        {
            throw new System.NotImplementedException();
        }

        public ILogFile Parse(string data)
        {
            throw new System.NotImplementedException();
        }
    }
}