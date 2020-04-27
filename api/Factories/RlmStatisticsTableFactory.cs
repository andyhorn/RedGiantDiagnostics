using System;
using System.Collections.Generic;
using System.Linq;
using API.Entities;
using API.Helpers;
using API.Models;

namespace API.Factories
{
    public interface IRlmStatisticsTableFactory
    {
        RlmStatisticsTable New { get; }
        RlmStatisticsTable Parse(string[] data);
    }

    public class RlmStatisticsTableFactory : IRlmStatisticsTableFactory
    {
        private IUtilities _utilities;
        private IServerStatusFactory _serverStatusFactory;
        private IStatisticsParser _statisticsParsers;
        public RlmStatisticsTable New { get => new RlmStatisticsTable(); }

        public RlmStatisticsTableFactory(IUtilities utilities, IServerStatusFactory serverStatusFactory, IStatisticsParser statisticsParsers)
        {
            _utilities = utilities;
            _serverStatusFactory = serverStatusFactory;
            _statisticsParsers = statisticsParsers;
        }
        
        public RlmStatisticsTable Parse(string[] data)
        {
            if (data == null || data.Length == 0)
            {
                return null;
            }

            var table = New;

            table.ServerName = GetServerName(data);
            table.StartTimes = GetStartTimes(data);
            table.Messages = GetMessages(data);
            table.Connections = GetConnections(data);
            table.Servers = GetServers(data);

            return table;
        }

        private IEnumerable<ServerStatus> GetServers(string[] data)
        {
            // Get the table section
            var dataLines = _utilities.GetLinesBetween("ISV Servers", "=========", data);

            if (dataLines == null || dataLines.Length == 0)
            {
                return new List<ServerStatus>();
            }

            // Remove the column header line if present
            if (dataLines[0].Contains("Name") && dataLines[0].Contains("port"))
            {
                dataLines = dataLines.TakeLast(dataLines.Length - 1).ToArray();
            }

            var servers = new List<ServerStatus>();

            foreach (var serverData in dataLines)
            {
                var server = _serverStatusFactory.Parse(serverData);

                if (server != null)
                {
                    servers.Add(server);
                }
            }

            return servers;
        }

        private int[] GetConnections(string[] data)
        {
            var connections = _statisticsParsers.GetColumnValues("Connections:", data);
            return connections;
        }

        private int[] GetMessages(string[] data)
        {
            var messages = _statisticsParsers.GetColumnValues("Messages:", data);
            return messages;
        }

        private DateTime[] GetStartTimes(string[] data)
        {
            var times = _statisticsParsers.ParseTableDates(data);
            return times;
        }

        private string GetServerName(string[] data)
        {
            // This should almost always result in "rlm"
            var name = _utilities.GetLineValue("Status for \"rlm\"", 2, data);

            if (string.IsNullOrWhiteSpace(name))
            {
                return string.Empty;
            }
            else if (name.Contains("\""))
            {
                name = name.Replace("\"", "");
            }

            return name;
        }
    }
}