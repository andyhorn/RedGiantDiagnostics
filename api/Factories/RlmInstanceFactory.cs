using System.Collections.Generic;
using System.Linq;
using API.Entities;
using API.Helpers;
using API.Models;

namespace API.Factories
{
    public static class RlmInstanceFactory
    {
        public static RlmInstance New => new RlmInstance();

        public static RlmInstance Parse(string[] data)
        {
            var rlm = New;

            rlm.Version = HelperMethods.GetLineValue("RLM Version:", 2, data);
            rlm.Command = GetAfter("Command:", ":", data);
            rlm.WorkingDirectory = GetAfter("Working Directory:", ":", data);
            rlm.PID = GetNumberFrom("PID:", data);
            rlm.Port = GetNumberFrom("Main TCP/IP port:", data);
            rlm.WebPort = GetNumberFrom("Web interface", data);
            rlm.AlternativePorts = GetAlternativePorts(data);
            rlm.IsvServers = GetIsvServers(data);
            rlm.IsCurrentInstance = IsCurrentInstance(data);

            return rlm;
        }

        private static IEnumerable<int> GetAlternativePorts(string[] data)
        {
            var list = new List<int>();

            var numbers = GetAfter("Alternate", ":", data).Split(" ");

            foreach (var number in numbers)
            {
                int value = 0;
                int.TryParse(number, out value);
                list.Add(value);
            }

            return list;
        }

        private static int GetNumberFrom(string header, string[] data)
        {
            int value = 0;

            var line = GetAfter(header, ":", data);

            int.TryParse(line, out value);

            return value;
        }

        private static bool IsCurrentInstance(string[] data)
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

        private static IEnumerable<string> GetIsvServers(string[] data)
        {
            var servers = GetAfter("ISV servers:", ":", data).Split(" ").ToList();
            return servers;
        }

        private static string GetAfter(string searchTerm, string separator, string[] data)
        {
            string value = string.Empty;

            for (var i = 0; i < data.Length; i++)
            {
                if (data[i].Contains(searchTerm))
                {
                    var line = data[i];
                    var index = line.IndexOf(separator) + separator.Length;
                    value = line.Substring(index).Trim();

                    break;
                }
            }

            return value;
        }
    }
}