using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Models;

namespace API.Services
{
    public class LogsService : ILogsService
    {
        private readonly ILogsRepository _logs;
        public LogsService(ILogsRepository logs)
        {
            _logs = logs;
        }

        public async Task<ILogFile> CreateAsync(ILogFile log)
        {
            if (log == null) 
            {
                return null;
            }

            await _logs.SaveAsync(log);
            return log;
        }

        public async Task DeleteAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return;
            }

            await _logs.RemoveAsync(id);
            return;
        }

        public async Task<ILogFile> UpdateAsync(ILogFile update)
        {
            if (update == null)
            {
                return null;
            }

            return await _logs.UpdateAsync(update);
        }

        public async Task<IEnumerable<ILogFile>> GetAllLogsAsync()
        {
            return await _logs.GetAllLogsAsync();
        }

        public async Task<ILogFile> GetByIdAsync(string id)
        {
            return await _logs.GetByIdAsync(id);
        }

        public async Task<IEnumerable<ILogFile>> GetForUserAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return null;
            }
            
            var logs = await _logs.GetAllLogsAsync();
            var userLogs = logs.Where(log => log.OwnerId == userId);
            return userLogs;
        }

        public ILogFile Parse(string data)
        {
            throw new System.NotImplementedException();
        }
    }
}