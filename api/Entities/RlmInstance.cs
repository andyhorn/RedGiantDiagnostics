using System.Collections.Generic;
using API.Models;

namespace API.Entities
{
    public class RlmInstance : IRlmInstance
    {
        public string Version { get; set; }
        public string Command { get; set; }
        public string WorkingDirectory { get; set; }
        public int PID { get; set; }
        public int Port { get; set; }
        public IEnumerable<int> AlternativePorts { get; set; }
        public int WebPort { get; set; }
        public IEnumerable<string> IsvServers { get; set; }
        public bool IsCurrentInstance { get; set; }
    }
}