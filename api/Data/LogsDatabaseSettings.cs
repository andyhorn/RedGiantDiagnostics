using System;

namespace API.Data
{
    public class LogsDatabaseSettings : ILogsDatabaseSettings
    {
        public string LogsCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }

        public LogsDatabaseSettings()
        {
            LogsCollectionName = "Logs";
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