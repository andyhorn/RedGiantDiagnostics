using System;
using System.Collections.Generic;
using System.Linq;
using API.Entities;
using API.Helpers;
using API.Models;

namespace API.Factories
{
    public static class RlmStatisticsTableFactory
    {
        public static RlmStatisticsTable New { get => new RlmStatisticsTable(); }

        public static RlmStatisticsTable Parse(string[] data)
        {
            var table = New;

            table.ServerName = GetServerName(data);
            table.StartTimes = GetStartTimes(data);
            table.Messages = GetMessages(data);
            table.Connections = GetConnections(data);
            table.Servers = GetServers(data);

            return table;
        }

        private static IEnumerable<IServerStatus> GetServers(string[] data)
        {
            // Get the table section
            var dataLines = HelperMethods.GetLinesBetween("ISV Servers", "=========", data);

            // Remove the column header line
            dataLines = dataLines.TakeLast(dataLines.Length - 1).ToArray();

            var servers = new List<IServerStatus>();

            foreach (var serverData in dataLines)
            {
                var server = ServerStatusFactory.Parse(serverData);
                servers.Add(server);
            }

            return servers;
        }

        private static int[] GetConnections(string[] data)
        {
            var connections = StatisticsParsers.GetColumnValues("Connections:", data);
            return connections;
        }

        private static int[] GetMessages(string[] data)
        {
            var messages = StatisticsParsers.GetColumnValues("Messages:", data);
            return messages;
        }

        private static DateTime[] GetStartTimes(string[] data)
        {
            var times = StatisticsParsers.ParseTableDates(data);
            return times;
        }

        private static string GetServerName(string[] data)
        {
            // This should almost always result in "rlm"
            var name = HelperMethods.GetLineValue("Status for \"rlm\"", 2, data);

            name = name.Replace("\"", "");

            return name;
        }
    }
}