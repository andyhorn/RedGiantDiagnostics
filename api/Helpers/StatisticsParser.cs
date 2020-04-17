using System;
using System.Linq;

namespace API.Helpers
{
    public static class StatisticsParsers
    {
        public static DateTime[] ParseTableDates(string[] data)
        {
            // Find the "Start time" line, split it into a string array,
            // and remove any empty/whitespace cells
            var dataLine = data.ToList()
                .Where(x => x.Contains("Start time"))       // Find the "Start time" row
                .ToList()[0].Split(" ")                     // Get the string value, split on spaces
                .Where(x => !string.IsNullOrWhiteSpace(x))  // Remove empty/whitespace cells
                .ToArray();                                 // Convert to a string array

            var year = DateTime.Now.Year;           // Get the current year
            var currentMonth = DateTime.Now.Month;  // Get the current month

            // Verify that the month/year date is not in the future
            if (DateTime.Parse($"{dataLine[2]}/{year}") > DateTime.Now)
            {
                year -= 1;
            }

            var start = $"{dataLine[2]}/{year} {dataLine[3]}";      // Create the start date
            var midnight = $"{dataLine[4]}/{year} {dataLine[5]}";   // Create the midnight date
            var recent = $"{dataLine[6]}/{year} {dataLine[7]}";     // Create the recent date

            // Create empty DateTime objects to be filled next
            var sinceStartTime = new DateTime();
            var sinceMidnightTime = new DateTime();
            var recentTime = new DateTime();

            // Fill each DateTime object by parsing the corresponding string
            DateTime.TryParse(start, out sinceStartTime);
            DateTime.TryParse(midnight, out sinceMidnightTime);
            DateTime.TryParse(recent, out recentTime);

            // Package the DateTime objects into an array and return
            var times = new DateTime[3]
            {
                sinceStartTime,
                sinceMidnightTime,
                recentTime
            };

            return times;
        }

        public static int[] GetColumnValues(string rowHeader, string[] data)
        {
            var dataLine = data
                .Where(x => x.Contains(rowHeader))  // Find the desired row
                .ToList()[0]                        // Get the string value
                .Replace(rowHeader, "")             // Remove the row header
                .Split(" ")                         // Split by spaces
                .Where(x => !string.IsNullOrWhiteSpace(x))  // Remove empty/whitespace cells
                .ToArray();                         // Convert to string array

            var startColumn = dataLine[0];      // Get the start column value
            var midnightColumn = dataLine[3];   // Get the midnight column value
            var recentColumn = dataLine[6];     // Get the recent column value

            // Package the parsed integer values into an array and return
            var columnValues = new int[3]
            {
                int.Parse(startColumn),
                int.Parse(midnightColumn),
                int.Parse(recentColumn)
            };

            return columnValues;
        }
    }
}