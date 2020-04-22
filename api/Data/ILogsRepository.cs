//  File:           ILogsRepository.cs
//  Author:         Andy Horn
//  Description:    An interface that exposes methods for interacting
//                  with the MongoDB driver.

using System.Collections.Generic;
using System.Threading.Tasks;
using API.Entities;
using API.Models;

namespace API.Data
{
    public interface ILogsRepository
    {
        /// <summary>
        /// Retrieves a List of all ILogFile objects in the collection.
        /// </summary>
        /// <returns>A List of all ILogFile objects in the collection.</returns>
        Task<List<LogFile>> GetAllLogsAsync();

        /// <summary>
        /// Retrieves an ILogFile object with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>An ILogFile object or null.</returns>
        Task<LogFile> GetByIdAsync(string id);

        /// <summary>
        /// Save an ILogFile object to the database.
        /// </summary>
        /// <param name="file"></param>
        /// <returns>The ILogFile object passed in.</returns>
        Task<LogFile> SaveAsync(LogFile file);

        /// <summary>
        /// Update an existing ILogFile object.
        /// </summary>
        /// <param name="update"></param>
        /// <returns>The ILogFile object passed in.</returns>
        Task<LogFile> UpdateAsync(LogFile update);

        /// <summary>
        /// Remove an existing ILogFile object from the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task RemoveAsync(string id);
    }
}