using System;
using System.Collections.Generic;
using API.Entities;
using API.Models;

namespace API.Factories
{
    public static class LogParser
    {
        private static string[] LinesBetween(string begin, string end, string[] data, int offset = 0)
        {
            List<string> lines = new List<string>();

            // Find the beginning marker from the given/default offset
            int i = offset;
            while (data[i] != begin && i < data.Length) { i++; }

            // We are now sitting on the "begin" marker, advance one and begin collection
            i++;

            for (; i < data.Length; i++)
            {
                if (data[i] == end)
                {
                    break;
                }
                else
                {
                    lines.Add(data[i]);
                }
            }

            return lines.ToArray();
        }
        public static Dictionary<string, string> ParseMetadata(string[] data)
        {
            string date = data[0].Split(" ")[4];
            string time = data[0].Split(" ")[5];
            string rlmVersion = data[1].Split(" ")[2];
            string hostname = data[5].Split(" ")[1];

            return new Dictionary<string, string>
            {
                { "Date", date },
                { "Time", time },
                { "Rlm Version", rlmVersion },
                { "Hostname", hostname }
            };
        }

        public static Dictionary<string, string> ParseEnvironmentVariables(string[] data)
        {
            var varLines = LinesBetween("Environment:", "RLM hostid list:", data);
            var envVariables = new Dictionary<string, string>();

            foreach (var pair in varLines)
            {
                var parts = pair.Split("=");
                envVariables.Add(parts[0], parts[1]);
            }

            return envVariables;
        }

        public static void ParseHostIdList(string[] data, ref ILogFile log)
        {
            throw new NotImplementedException();
        }

        public static void ParseLicenseFileList(string[] data, ref ILogFile log)
        {
            throw new NotImplementedException();
        }

        public static void ParseLicenseFiles(string[] data, ref ILogFile log)
        {
            throw new NotImplementedException();
        }

        public static void ParseRlmStatistics(string[] data, ref ILogFile log)
        {

        }
    }
}