//  File:           LogsRepository.cs
//  Author:         Andy Horn
//  Description:    The concrete implementation of the ILogsRepository interface.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Entities;
using API.Models;
using MongoDB.Driver;

namespace API.Data
{
    public class LogsRepository : ILogsRepository
    {
        /// <summary>
        /// The MongoDB collection of ILogFile objects.
        /// </summary>
        private readonly IMongoCollection<LogFile> _logs;

        /// <summary>
        /// Constructor receives an IDataContext through Dependency Injection,
        /// retrieves the Logs collection from the context.
        /// </summary>
        /// <param name="context"></param>
        public LogsRepository(IDataContext context)
        {
            _logs = context.Logs;
        }

        /// <summary>
        /// Retrieves all LogFile objects from the data store
        /// </summary>
        /// <returns>A list of LogFile objects</returns>
        public async Task<List<LogFile>> GetAllLogsAsync()
        {
            // Retrieve all logs from the collection
            var logs = await _logs.FindAsync(log => true);

            // Format into a List and return
            var list = await logs.ToListAsync();
            return list;
        }

        /// <summary>
        /// Retrieves a LogFile object with a matching ID
        /// </summary>
        /// <param name="id">The ID of the LogFile to retrieve</param>
        /// <returns>A LogFile object or null</returns>
        public async Task<LogFile> GetByIdAsync(string id)
        {
            // Validate the ID string
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException();
            }
            
            // Find a log with a matching ID
            var find = await _logs.FindAsync(log => log.Id == id);
            var log = await find.SingleOrDefaultAsync();
            return log;
        }

        /// <summary>
        /// Removes a LogFile object from the data store
        /// </summary>
        /// <param name="id">The ID of the LogFile object to remove</param>
        /// <returns></returns>
        public async Task RemoveAsync(string id)
        {
            // Validate the ID string
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException();
            }

            // Find a log with a matching ID and delete it from the collection
            await _logs.FindOneAndDeleteAsync(log => log.Id == id);
        }

        /// <summary>
        /// Saves a new LogFile object to the data store
        /// </summary>
        /// <param name="file">The LogFile to be saved</param>
        /// <returns>The same LogFile, updated with an ID</returns>
        public async Task<LogFile> SaveAsync(LogFile file)
        {
            // Validate the LogFile object
            if (file == null)
            {
                throw new ArgumentNullException();
            }

            // Insert the log object into the collection
            await _logs.InsertOneAsync(file);
            return file;
        }

        /// <summary>
        /// Updates an existing LogFile object in the data store
        /// </summary>
        /// <param name="update">The LogFile with updated data</param>
        /// <returns>The updated LogFile object</returns>
        public async Task<LogFile> UpdateAsync(LogFile update)
        {
            // Validate the LogFile object
            if (update == null)
            {
                throw new ArgumentNullException();
            }

            // Find a log with a matching ID and replace it with the "update" log
            var updated = await _logs.FindOneAndReplaceAsync(log => log.Id == update.Id, update);

            return updated;
        }
    }
}