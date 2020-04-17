using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace API.Helpers
{
    public static class HelperMethods
    {
        public static DateTime? GetDateTimeFrom(string dateString)
        {
            if (dateString == null || dateString.Length == 0)
            {
                return null;
            }

            var date = new DateTime();
            if (DateTime.TryParse(dateString, out date))
            {
                return date;
            }

            return null;
        }
        public static string GetLineValue(string searchTerm, int word, string[] data)
        {
            string value = string.Empty;

            foreach (var line in data)
            {
                if (line.Contains(searchTerm))
                {
                    var words = line.Split(" ");

                    if (word < words.Length)
                        value = words[word].Trim();

                    break;
                }
            }

            return value;
        }
        public static string MakeMac(string mac)
        {
            string newMac = string.Empty;
            mac.Replace(":", "");
            mac.Replace("-", "");
            mac.Replace(".", "");

            for (var i = 0; i < mac.Length; i++)
            {
                if (i % 2 == 0 && i > 1)
                {
                    newMac += ":";
                }

                newMac += mac[i].ToString().ToUpper();
            }

            return newMac;
        }

        public static string[] GetLinesBetween(string begin, string end, string[] data, bool inclusive = false)
        {
            List<string> lines = new List<string>();

            var beginMatch = new Regex(begin);
            var endMatch = new Regex(end);

            // Find the beginning marker from the given/default offset
            int i = 0;
            while (!beginMatch.IsMatch(data[i]) && i < data.Length) { i++; }

            // We are now sitting on the "begin" marker, 
            // if not inclusive, advance one and begin collection
            if (!inclusive) i++;

            for (; i < data.Length; i++)
            {
                if (endMatch.IsMatch(data[i]))
                {
                    if (inclusive)
                    {
                        lines.Add(data[i]);
                    }
                    break;
                }
                else
                {
                    lines.Add(data[i]);
                }
            }

            return lines.ToArray();
        }
    }
}