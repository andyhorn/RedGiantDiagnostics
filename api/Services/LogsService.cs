using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using API.Factories;
using API.Models;

namespace API.Services
{
    public class LogsService : ILogsService
    {
        private readonly ILogsRepository _logs;
        private readonly ILogFactory _factory;
        public LogsService(ILogsRepository logs, ILogFactory factory)
        {
            _logs = logs;
            _factory = factory;
        }

        public async Task<LogFile> CreateAsync(LogFile log)
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

        public async Task<LogFile> UpdateAsync(LogFile update)
        {
            if (update == null)
            {
                return null;
            }

            return await _logs.UpdateAsync(update);
        }

        public async Task<IEnumerable<LogFile>> GetAllLogsAsync()
        {
            return await _logs.GetAllLogsAsync();
        }

        public async Task<LogFile> GetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }
            
            return await _logs.GetByIdAsync(id);
        }

        public async Task<IEnumerable<LogFile>> GetForUserAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return null;
            }
            
            var logs = await _logs.GetAllLogsAsync();
            var userLogs = logs.FindAll(x => x.OwnerId == userId);
            return userLogs;
        }

        public LogFile Parse(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                return null;
            }

            return _factory.Parse(data);
        }
    }
}