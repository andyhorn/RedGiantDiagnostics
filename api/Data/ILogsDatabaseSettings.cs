//  File:           ILogsDatabaseSettings.cs
//  Author:         Andy Horn
//  Description:    An interface that exposes three properties for connecting to 
//                  a Mongo database: the LogsCollectionName, the DatabaseName, 
//                  and the ConnectionString.

namespace API.Data
{
    public interface ILogsDatabaseSettings
    {
        string LogsCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}