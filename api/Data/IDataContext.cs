using API.Models;
using MongoDB.Driver;

namespace API.Data
{
    public interface IDataContext
    {
        IMongoCollection<ILogFile> Logs { get; }
    }
}