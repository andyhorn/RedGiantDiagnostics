using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Contracts.Requests;
using API.Contracts.Requests.Admin;
using API.Data;
using API.Entities;
using API.Exceptions;
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

        public async Task<LogFile> UpdateAsync<T>(string id, T update) where T : ILogUpdateRequest
        {
            // Validate the ID string
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("ID cannot be empty");
            }

            // Validate the update request object
            if (update == null)
            {
                throw new ArgumentNullException("Update data cannot be null");
            }

            var log = await _logs.GetByIdAsync(id);
            if (log == null)
            {
                throw new ResourceNotFoundException();
            }

            log.Update<T>(update);

            return await _logs.UpdateAsync(log);
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

        public async Task<bool> LogExists(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException();
            }

            var log = await GetByIdAsync(id);

            return log != null;
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