using System;
using System.Collections.Generic;
using System.Linq;
using API.Entities;
using API.Helpers;

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

            return table;
        }

        private static DateTime[] GetStartTimes(string[] data)
        {
            var timeData = new Dictionary<string, string[]>();

            // Find the "Start time" line, split it into a string array,
            // and remove any empty/whitespace cells
            var dataLine = data.ToList()
                .Where(x => x.Contains("Start time"))
                .ToList()[0].Split(" ")
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            var year = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;

            if (DateTime.Parse($"{dataLine[2]}/{year}") > DateTime.Now)
            {
                year -= 1;
            }

            var start = $"{dataLine[2]}/{year} {dataLine[3]}";
            var midnight = $"{dataLine[4]}/{year} {dataLine[5]}";
            var recent = $"{dataLine[6]}/{year} {dataLine[7]}";

            var sinceStartTime = new DateTime();
            var sinceMidnightTime = new DateTime();
            var recentTime = new DateTime();

            DateTime.TryParse(start, out sinceStartTime);
            DateTime.TryParse(midnight, out sinceMidnightTime);
            DateTime.TryParse(recent, out recentTime);

            var times = new DateTime[3]
            {
                sinceStartTime,
                sinceMidnightTime,
                recentTime
            };

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