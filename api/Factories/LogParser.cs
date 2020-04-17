using System;
using System.Collections.Generic;
using API.Models;

namespace API.Factories
{
    /// <summary>
    /// Each LogParser will receive an ILogFile object and an array
    /// of strings, representing the log file's cleansed and prepped data.
    /// The Parse method will then build a LogFile object from this data.
    /// </summary>
    public class LogParser
    {
        private ILogFile _log;
        private string[] _data;
        public ILogFile Log
        {
            get
            {
                return _log;
            }
        }

        public LogParser(ILogFile log, string[] data)
        {
            _log = log;
            _data = data;
        }

        /// <summary>
        /// Drives the parsing of the log file data, each subroutine 
        /// parses its share of information from the data and
        /// adds it to the LogFile object.
        /// </summary>
        public void Parse()
        {
            // Get the Metadata
            ParseMetadata();

            // Get the Environment Variables
            ParseEnvironmentVariables();
        }

        /// <summary>
        /// Returns the text between two lines in the rlmdiag.txt file.
        /// </summary>
        /// <param name="begin">The string marking the beginning of the section to collect.</param>
        /// <param name="end">The string marking the end of the section to collect.</param>
        /// <returns>Returns a string array containing all lines between the begin and end markers.</returns>
        private string[] LinesBetween(string begin, string end)
        {
            List<string> lines = new List<string>();

            // Find the beginning marker from the given/default offset
            int i = 0;
            while (_data[i] != begin && i < _data.Length) { i++; }

            // We are now sitting on the "begin" marker, advance one and begin collection
            i++;

            for (; i < _data.Length; i++)
            {
                if (_data[i] == end)
                {
                    break;
                }
                else
                {
                    lines.Add(_data[i]);
                }
            }

            return lines.ToArray();
        }

        /// <summary>
        /// Parse the metadata information from the top of the log file.
        /// </summary>
        private void ParseMetadata()
        {
            string date = _data[0].Split(" ")[4];
            string time = _data[0].Split(" ")[5];
            string rlmVersion = _data[1].Split(" ")[2];
            string hostname = _data[5].Split(" ")[1];

            var logDate = DateTime.Parse($"{date} {time}");

            _log.Date = logDate;
            _log.RlmVersion = rlmVersion;
            _log.Hostname = hostname;
        }

        /// <summary>
        /// Parse the environment variable section from the log file.
        /// </summary>
        private void ParseEnvironmentVariables()
        {
            var varLines = LinesBetween("Environment:", "RLM hostid list:");
            var envVariables = new Dictionary<string, string>();

            foreach (var pair in varLines)
            {
                var parts = pair.Split("=");
                envVariables.Add(parts[0], parts[1]);
            }

            _log.EnvironmentVariables = envVariables;
        }

        /// <summary>
        /// Parse the host id list line from the log file.
        /// </summary>
        private void ParseHostIdList()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Parse the list of license files from the log file.
        /// </summary>
        private void ParseLicenseFileList()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Parse the contents of each license file in the log file.
        /// </summary>
        private void ParseLicenseFiles()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Parse the main RLM server statistics section from the log file.
        /// </summary>
        private void ParseRlmStatistics()
        {

        }
    }
}