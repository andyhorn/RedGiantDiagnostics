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
            var server = New;

            var columns = data
                .Split(" ")                                 // Split the line by spaces
                .Where(x => !string.IsNullOrWhiteSpace(x))  // Remove null/whitespace cells
                .ToArray();                                 // Convert to a string array

            server.Name = columns[0];
            server.Port = int.Parse(columns[1]);
            server.IsRunning = columns[2] == "Yes";
            server.Restarts = int.Parse(columns[3]);

            return server;
        }
    }
}