using System.Collections.Generic;
using System.Threading.Tasks;
using API.Factories;
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
        public async Task<List<ILogFile>> GetAllLogsAsync()
        {
            var logs = await _logs.FindAsync(log => true);
            var list = await logs.ToListAsync();
            return list;
        }

        public async Task<ILogFile> GetByIdAsync(string id)
        {
            var find = await _logs.FindAsync(log => log.Id == id);
            var log = await find.SingleOrDefaultAsync();
            return log;
        }

        public async Task RemoveAsync(string id)
        {
            await _logs.FindOneAndDeleteAsync(log => log.Id == id);
        }

        public async Task<ILogFile> SaveAsync(ILogFile file)
        {
            await _logs.InsertOneAsync(file);
            return file;
        }

        public async Task<ILogFile> UpdateAsync(ILogFile update)
        {
            await _logs.FindOneAndReplaceAsync(log => log.Id == update.Id, update);
            var updated = await (await _logs.FindAsync(log => log.Id == update.Id)).FirstOrDefaultAsync();
            return updated;
        }
    }
}