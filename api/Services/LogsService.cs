using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Contracts.Requests;
using API.Data;
using API.Entities;
using API.Exceptions;
using API.Factories;

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
            // Validate the log object
            if (log == null)
            {
                throw new ArgumentNullException();
            }

            // Save the log object
            try
            {
                await _logs.SaveAsync(log);
            }
            catch
            {
                throw new ActionFailedException();
            }

            return log;
        }

        public async Task DeleteAsync(string id)
        {
            // Validate the ID string
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException();
            }

            // Verify a LogFile exists with the ID
            var exists = await LogExists(id);
            if (!exists)
            {
                throw new ResourceNotFoundException();
            }

            // Delete the log object
            try
            {
                await _logs.RemoveAsync(id);
            }
            catch
            {
                throw new ActionFailedException();
            }

            return;
        }

        public async Task<LogFile> UpdateAsync(string id, LogUpdateRequest update)
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

            // log.Update<T>(update);
            if (!string.IsNullOrEmpty(update.Title))
            {
                log.Title = update.Title;
            }

            if (!string.IsNullOrEmpty(update.Comments))
            {
                log.Comments = update.Comments;
            }

            if (!string.IsNullOrEmpty(update.OwnerId))
            {
                log.OwnerId = update.OwnerId;
            }

            return await _logs.UpdateAsync(log);
        }

        public async Task<IEnumerable<LogFile>> GetAllLogsAsync()
        {
            return await _logs.GetAllLogsAsync();
        }

        public async Task<LogFile> GetByIdAsync(string id)
        {
            // Validate the ID string
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException();
            }

            return await _logs.GetByIdAsync(id);
        }

        public async Task<bool> LogExists(string id)
        {
            // Validate the ID string
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException();
            }

            // Retrieve the log object
            var log = await GetByIdAsync(id);

            // Return its existence
            return log != null;
        }

        public async Task<IEnumerable<LogFile>> GetForUserAsync(string userId)
        {
            // Validate the ID string
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentNullException();
            }

            // Get all logs
            var logs = await _logs.GetAllLogsAsync();

            // Filter for logs with a matching OwnerId
            var userLogs = logs.FindAll(x => x.OwnerId == userId);
            return userLogs;
        }

        public LogFile Parse(string data)
        {
            // Validate the data string
            if (string.IsNullOrWhiteSpace(data))
            {
                throw new ArgumentNullException();
            }

            // Return the parsed LogFile object
            return _factory.Parse(data);
        }
    }
}