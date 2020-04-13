using System.Collections.Generic;
using API.Models;

namespace API.Data
{
    public interface ILogsRepository
    {
        List<ILogFile> GetAllLogs();
        ILogFile GetById(string id);
        ILogFile Save(ILogFile file);
        void Update(ILogFile update);
        void Remove(string id);
    }
}