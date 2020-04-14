//  File:           LogsRepository.cs
//  Author:         Andy Horn
//  Description:    The concrete implementation of the ILogsRepository interface.

using System.Collections.Generic;
using System.Threading.Tasks;
using API.Models;
using MongoDB.Driver;

namespace API.Data
{
    public class LogsRepository : ILogsRepository
    {
        /// <summary>
        /// The MongoDB collection of ILogFile objects.
        /// </summary>
        private readonly IMongoCollection<ILogFile> _logs;

        /// <summary>
        /// Constructor receives an IDataContext through Dependency Injection,
        /// retrieves the Logs collection from the context.
        /// </summary>
        /// <param name="context"></param>
        public LogsRepository(IDataContext context)
        {
            _logs = context.Logs;
        }

        public async Task<List<ILogFile>> GetAllLogsAsync()
        {
            // Retrieve all logs from the collection
            var logs = await _logs.FindAsync(log => true);

            // Format into a List and return
            var list = await logs.ToListAsync();
            return list;
        }

        public async Task<ILogFile> GetByIdAsync(string id)
        {
            // Find a log with a matching ID
            var find = await _logs.FindAsync(log => log.Id == id);
            var log = await find.SingleOrDefaultAsync();
            return log;
        }

        public async Task RemoveAsync(string id)
        {
            // Find a log with a matching ID and delete it from the collection
            await _logs.FindOneAndDeleteAsync(log => log.Id == id);
        }

        public async Task<ILogFile> SaveAsync(ILogFile file)
        {
            // Insert the log object into the collection
            await _logs.InsertOneAsync(file);
            return file;
        }

        public async Task<ILogFile> UpdateAsync(ILogFile update)
        {
            // Find a log with a matching ID and replace it with the "update" log
            await _logs.FindOneAndReplaceAsync(log => log.Id == update.Id, update);

            // Return the updated log from the collection
            var updated = await (await _logs.FindAsync(log => log.Id == update.Id)).FirstOrDefaultAsync();
            return updated;
        }
    }
}