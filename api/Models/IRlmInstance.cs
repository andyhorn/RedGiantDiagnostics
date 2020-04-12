using System.Collections.Generic;

namespace API.Models
{
    public interface IRlmInstance
    {
        string Version { get; set; }
        string Command { get; set; }
        string WorkingDirectory { get; set; }
        int PID { get; set; }
        int Port { get; set; }
        IEnumerable<int> AlternativePorts { get; set; }
        int WebPort { get; set; }
        IEnumerable<string> IsvServers { get; set; }
        bool IsCurrentInstance { get; set; }
    }
}