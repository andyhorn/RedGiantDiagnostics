using System.Linq;
using API.Entities;
using API.Models;

namespace API.Factories
{
    public interface IServerStatusFactory
    {
        ServerStatus New { get; }
        ServerStatus Parse(string data);
    }

    public class ServerStatusFactory : IServerStatusFactory
    {
        public ServerStatus New { get => new ServerStatus(); }

        public ServerStatus Parse(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                return null;
            }
            
            var server = New;

            var columns = data
                .Split(" ")                                 // Split the line by spaces
                .Where(x => !string.IsNullOrWhiteSpace(x))  // Remove null/whitespace cells
                .ToArray();                                 // Convert to a string array

            if (columns.Length > 0)
            {
                server.Name = columns[0];
            }

            if (columns.Length > 1)
            {
                int value = 0;
                int.TryParse(columns[1], out value);
                server.Port = value;
            }
            
            if (columns.Length > 2)
            {
                server.IsRunning = columns[2] == "Yes";
            }
            
            if (columns.Length > 3)
            {
                int value = 0;
                int.TryParse(columns[3], out value);
                server.Restarts = value;
            }

            return server;
        }
    }
}