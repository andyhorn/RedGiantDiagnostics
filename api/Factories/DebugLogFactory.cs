using System.Collections.Generic;
using API.Entities;
using API.Helpers;
using API.Models;

namespace API.Factories
{
    public static class DebugLogFactory
    {
        public static IDebugLog New => new DebugLog();

        public static IDebugLog Parse(string[] data)
        {
            var log = New;

            log.Filename = GetLogFilename(data);
            log.Lines = GetLogLines(data);

            return log;
        }

        private static IEnumerable<string> GetLogLines(string[] data)
        {
            var lines = HelperMethods.GetLinesBetween("====", "====", data);
            return lines;
        }

        private static string GetLogFilename(string[] data)
        {
            string name = string.Empty;

            for (var i = 0; i < data.Length; i++)
            {
                // If the current line contains "debug log file contents", this line
                // should also contain the log's filename
                if (data[i].Contains("debug log file contents"))
                {
                    // Get the current line (for ease of programming and debugging)
                    var line = data[i];

                    // Get the index of the word "contents" - This will ensure that 
                    // this function works regardless of how many words come before
                    // the word "contents"
                    var index = line.LastIndexOf("contents") + "contents".Length;

                    // Pull a substring of everything after the word "contents" and
                    // remove any extra whitespace
                    name = line.Substring(index).Trim();

                    // Remove the parentheses from around the filename
                    name = name.Replace("(", "");
                    name = name.Replace(")", "");

                    // We have the name, so we can break the loop
                    break;
                }
            }

            return name;
        }
    }
}