namespace API.Data
{
    public interface ILogsDatabaseSettings
    {
        string LogsCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}