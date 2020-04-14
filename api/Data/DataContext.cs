//  File:           DataContext.cs
//  Author:         Andy Horn
//  Description:    The concrete implementation of the IDataContext interface

using API.Models;
using MongoDB.Driver;

namespace API.Data
{
    public class DataContext : IDataContext
    {
        private readonly ILogsDatabaseSettings _settings;

        /// <summary>
        /// Constructor receives an ILogsDatabaseSettings object through Dependency Injection.
        /// </summary>
        /// <param name="settings"></param>
        public DataContext(ILogsDatabaseSettings settings)
        {
            _settings = settings;
        }

        /// <summary>
        /// Creates a connection to the MongoDB using connection settings from the ILogsDatabaseSettings
        /// object, then returns the Logs collection.
        /// </summary>
        /// <value></value>
        public IMongoCollection<ILogFile> Logs 
        {
            get
            {
                var client = new MongoClient(_settings.ConnectionString);
                var database = client.GetDatabase(_settings.DatabaseName);
                var collection = database.GetCollection<ILogFile>(_settings.LogsCollectionName);
                return collection;
            }
        }
    }
}