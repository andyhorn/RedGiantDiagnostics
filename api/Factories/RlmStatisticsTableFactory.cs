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
        public RlmStatisticsTable New { get => new RlmStatisticsTable(); }

        public RlmStatisticsTableFactory(IUtilities utilities, IServerStatusFactory serverStatusFactory)
        {
            _utilities = utilities;
            _serverStatusFactory = serverStatusFactory;
        }
        
        public RlmStatisticsTable Parse(string[] data)
        {
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

            // Remove the column header line
            dataLines = dataLines.TakeLast(dataLines.Length - 1).ToArray();

            var servers = new List<ServerStatus>();

            foreach (var serverData in dataLines)
            {
                var server = _serverStatusFactory.Parse(serverData);
                servers.Add(server);
            }

            return servers;
        }

        private int[] GetConnections(string[] data)
        {
            var connections = StatisticsParsers.GetColumnValues("Connections:", data);
            return connections;
        }

        private int[] GetMessages(string[] data)
        {
            var messages = StatisticsParsers.GetColumnValues("Messages:", data);
            return messages;
        }

        private DateTime[] GetStartTimes(string[] data)
        {
            var times = StatisticsParsers.ParseTableDates(data);
            return times;
        }

        private string GetServerName(string[] data)
        {
            // This should almost always result in "rlm"
            var name = _utilities.GetLineValue("Status for \"rlm\"", 2, data);

            name = name.Replace("\"", "");

            return name;
        }
    }
}