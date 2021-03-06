using API.Models;

namespace API.Entities
{
    public class ServerStatus
    {
        public string Name { get; set; }
        public bool IsRunning { get; set; }
        public int Port { get; set; }
        public int Restarts { get; set; }
    }
}