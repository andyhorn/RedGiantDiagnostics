using System.Linq;
using API.Entities;
using API.Models;

namespace API.Factories
{
    public static class ServerStatusFactory
    {
        public static IServerStatus GetServer() => new ServerStatus();

        public static IServerStatus Parse(string data)
        {
            var server = GetServer();

            var columns = data
                .Split(" ")     // Split the line by spaces
                .Where(x => !string.IsNullOrWhiteSpace(x))  // Remove null/whitespace cells
                .ToArray();     // Convert to a string array

            server.Name = columns[0];
            server.Port = int.Parse(columns[1]);
            server.IsRunning = columns[2] == "Yes";
            server.Restarts = int.Parse(columns[3]);

            return server;
        }
    }
}