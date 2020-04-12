namespace API.Models
{
    public interface IServerStatus
    {
        string Name { get; set; }
        bool IsRunning { get; set; }
        int Port { get; set; }
        int Restarts { get; set; }
    }
}