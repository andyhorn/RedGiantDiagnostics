using System.Collections.Generic;
using System.Linq;
using API.Entities;
using API.Helpers;
using API.Models;

namespace API.Factories
{
    public interface IRlmInstanceFactory
    {
        RlmInstance New { get; }
        RlmInstance Parse(string[] data);
    }

    public class RlmInstanceFactory : IRlmInstanceFactory
    {
        private IUtilities _utilities;
        public RlmInstance New => new RlmInstance();

        public RlmInstanceFactory(IUtilities utilities)
        {
            _utilities = utilities;
        }
        
        public RlmInstance Parse(string[] data)
        {
            if (data == null || data.Length == 0)
            {
                return null;
            }

            var rlm = New;

            rlm.Version = _utilities.GetLineValue("RLM Version:", 2, data);
            rlm.Command = _utilities.GetAfter("Command:", ":", data);
            rlm.WorkingDirectory = _utilities.GetAfter("Working Directory:", ":", data);
            rlm.PID = GetNumberFrom("PID:", data);
            rlm.Port = GetNumberFrom("Main TCP/IP port:", data);
            rlm.WebPort = GetNumberFrom("Web interface", data);
            rlm.AlternativePorts = GetAlternativePorts(data);
            rlm.IsvServers = GetIsvServers(data);
            rlm.IsCurrentInstance = IsCurrentInstance(data);

            return rlm;
        }

        private IEnumerable<int> GetAlternativePorts(string[] data)
        {
            var list = new List<int>();

            var numbers = _utilities.GetAfter("Alternate", ":", data);
            
            if (string.IsNullOrWhiteSpace(numbers))
            {
                return list;
            }

            foreach (var number in numbers.Split(" "))
            {
                int value = 0;
                if (int.TryParse(number, out value))
                {
                    list.Add(value);
                }
            }

            return list;
        }

        private int GetNumberFrom(string header, string[] data)
        {
            int value = 0;

            var line = _utilities.GetAfter(header, ":", data);

            int.TryParse(line, out value);

            return value;
        }

        private bool IsCurrentInstance(string[] data)
        {
            for (var i = data.Length - 1; i >= 0; i--)
            {
                if (data[i].Contains("[this instance of rlm]"))
                {
                    return true;
                }
            }

            return false;
        }

        private IEnumerable<string> GetIsvServers(string[] data)
        {
            var line = _utilities.GetAfter("ISV servers:", ":", data);
            
            if (string.IsNullOrWhiteSpace(line))
            {
                return new string[0];
            }

            var servers = line.Split(" ");

            return servers;
        }
    }
}