//  File:           LogsDatabaseSettings.cs
//  Author:         Andy Horn
//  Description:    Concrete implementation of the ILogsDatabaseSettings interface.

using System;

namespace API.Data
{
    public class LogsDatabaseSettings : ILogsDatabaseSettings
    {
        public string LogsCollectionName { get; set; } = "Logs";
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }

        /// <summary>
        /// Constructor retrieves database connection settings from environment variables,
        /// then creates the connection string.
        /// </summary>
        public LogsDatabaseSettings()
        {
            DatabaseName = Environment.GetEnvironmentVariable("DB_DATABASE");

            var host = Environment.GetEnvironmentVariable("DB_HOST");
            var user = Environment.GetEnvironmentVariable("DB_USER");
            var pwd = Environment.GetEnvironmentVariable("DB_PASSWORD");
            var port = Environment.GetEnvironmentVariable("DB_PORT");
            var connectionString = $"mongodb://{user}:{pwd}@{host}:{port}";
            ConnectionString = connectionString;
        }
    }
}