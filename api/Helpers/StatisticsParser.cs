using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Helpers
{
    public interface IStatisticsParser
    {
        DateTime[] ParseTableDates(string[] data);
        int[] GetColumnValues(string rowHeader, string[] data);
    }

    public class StatisticsParser : IStatisticsParser
    {
        public DateTime[] ParseTableDates(string[] data)
        {
            var times = new List<DateTime>();

            if (data == null || data.Length == 0)
            {
                return times.ToArray();
            }

            // If the data doesn't contain the line we're looking for, 
            // return an empty DateTime array
            if (!ListContains(data, "Start time"))
            {
                return times.ToArray();
            }

            // Find the "Start time" line, split it into a string array,
            // and remove any empty/whitespace cells
            var dataLine = data
                .ToList()
                .FirstOrDefault(x => x.Contains("Start time"))
                .Split(" ")                                 // Get the string value, split on spaces
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
            times.Add(sinceStartTime);
            times.Add(sinceMidnightTime);
            times.Add(recentTime);

            return times.ToArray();
        }

        private bool ListContains(string[] list, string searchTerm)
        {
            return list.ToList().Any(x => x.Contains(searchTerm));
        }

        public int[] GetColumnValues(string rowHeader, string[] data)
        {
            var values = new int[3]
            {
                0,
                0,
                0
            };

            if (data == null || data.Length == 0 || string.IsNullOrWhiteSpace(rowHeader))
            {
                return values;
            }

            var dataLine = data
                .Where(x => x.Contains(rowHeader))  // Find the desired row
                .ToList()[0]                        // Get the string value
                .Replace(rowHeader, "")             // Remove the row header
                .Split(" ")                         // Split by spaces
                .Where(x => !string.IsNullOrWhiteSpace(x))  // Remove empty/whitespace cells
                .ToArray();                         // Convert to string array

            // Try to parse the three columns
            if (dataLine.Length > 0)
            {
                // Get the "Start" column value
                int.TryParse(dataLine[0], out values[0]);
            }

            if (dataLine.Length > 3)
            {
                // Get the "Midnight" column value
                int.TryParse(dataLine[3], out values[1]);
            }

            if (dataLine.Length > 6)
            {
                // Get the "Recent" column value
                int.TryParse(dataLine[6], out values[2]);
            }

            return values;
        }
    }
}