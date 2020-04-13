using System.Collections.Generic;
using API.Models;
using MongoDB.Driver;

namespace API.Data
{
    public class LogsRepository : ILogsRepository
    {
        private readonly IMongoCollection<ILogFile> _logs;

        public LogsRepository(ILogsDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _logs = database.GetCollection<ILogFile>(settings.LogsCollectionName);
        }
        public List<ILogFile> GetAllLogs()
        {
            throw new System.NotImplementedException();
        }

        public ILogFile GetById(string id)
        {
            throw new System.NotImplementedException();
        }

        public void Remove(string id)
        {
            throw new System.NotImplementedException();
        }

        public ILogFile Save(ILogFile file)
        {
            throw new System.NotImplementedException();
        }

        public void Update(ILogFile update)
        {
            throw new System.NotImplementedException();
        }
    }
}