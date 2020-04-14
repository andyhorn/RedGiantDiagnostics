using API.Models;
using MongoDB.Driver;

namespace API.Data
{
    public class DataContext : IDataContext
    {
        private readonly ILogsDatabaseSettings _settings;

        public DataContext(ILogsDatabaseSettings settings)
        {
            _settings = settings;
        }
        public IMongoCollection<ILogFile> Logs 
        {
            get
            {
                var client = new MongoClient(_settings.ConnectionString);
                var database = client.GetDatabase(_settings.DatabaseName);
                var context = database.GetCollection<ILogFile>(_settings.LogsCollectionName);
                return context;
            }
        }
    }
}