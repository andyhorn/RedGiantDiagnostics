using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace API.Helpers
{
    public static class HelperMethods
    {
        /// <summary>
        /// Parses a DateTime object from a datetime-like string
        /// </summary>
        /// <param name="dateString">A datetime-like string.</param>
        /// <returns>A new DateTime object representing the string, or null.</returns>
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

        /// <summary>
        /// Gets a specific value from a line of text.
        /// </summary>
        /// <param name="searchTerm">A word or words to determine the desired array index.</param>
        /// <param name="word">The index to return, once the line is split into separate words.</param>
        /// <param name="data">The array of strings to search through.</param>
        /// <returns>The string from the desired index of the line.</returns>
        public static string GetLineValue(string searchTerm, int word, string[] data)
        {
            // Create an empty string to store the value
            string value = string.Empty;

            // Convert the search term into a Regular Expression object
            var regex = new Regex(searchTerm);

            // Loop through each line in the input data array
            foreach (var line in data)
            {
                // If the current line matches the Regular Expression
                if (regex.IsMatch(line))
                {
                    // Split the line into separate words
                    var words = line.Split(" ");

                    // Verify the given index is not out of range,
                    // get the word from the index and trim any
                    // excess whitespace
                    if (word < words.Length)
                        value = words[word].Trim();

                    break;
                }
            }

            // Return whatever value was found; If the index was out of range
            // or no match was found for the search term, this will return null.
            return value;
        }

        /// <summary>
        /// Formats the input string into a proper MAC address string.
        /// For example, will convert "12.3456-78:90 ab" into "12:34:56:78:90:AB"
        /// </summary>
        /// <param name="mac"></param>
        /// <returns></returns>
        public static string MakeMac(string mac)
        {
            // Create an empty string to store the new MAC address
            string newMac = string.Empty;

            // Strip the old MAC address of any separators
            mac.Replace(":", "");
            mac.Replace("-", "");
            mac.Replace(".", "");
            mac.Replace(" ", "");

            // Loop through the original MAC length (should be 12 characters)
            for (var i = 0; i < mac.Length; i++)
            {
                // Add a colon separator every two characters
                if (i % 2 == 0 && i > 1)
                {
                    newMac += ":";
                }

                // Capitalize each character and add it to the new MAC string
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

        public static IEnumerable<IEnumerable<string>> GetSubsections(string beginMarker, string endMarker, string[] data)
        {
            var beginRe = new Regex(beginMarker);
            var endRe = new Regex(endMarker);

            var masterCollection = new List<List<string>>();

            // Begin the outer for-loop, iterating through the entire string array
            for (var outer = 0; outer < data.Length; outer++)
            {
                // If we find the beginning of a subsection, as given by the begin marker
                // begin the inner for-loop to gather the subsection data
                if (beginRe.IsMatch(data[outer]))
                {
                    var subCollection = new List<string>();
                    subCollection.Add(data[outer]);

                    // Loop through the lines of data
                    for (var inner = outer + 1; inner < data.Length; inner++)
                    {
                        if (endRe.IsMatch(data[inner]))
                        {
                            // If we find the ending marker, we will advance the outer 
                            // for-loop to the current position and break the inner loop

                            // If the begin and end marker are the same, we will advance the
                            // outer for-loop to one cell previous to the current one, so that
                            // it will begin on this cell and not skip over this cell; This would
                            // cause the loop to skip over this subsection entirely.
                            outer = beginMarker == endMarker
                                ? inner - 1
                                : inner;

                            break;
                        }

                        // Add the current line to the subsection collection
                        subCollection.Add(data[inner]);
                    }

                    // Add this subsection to the master collection
                    masterCollection.Add(subCollection);
                }
            }

            return masterCollection;
        }
    }
}