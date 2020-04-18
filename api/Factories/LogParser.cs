using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using API.Models;
using API.Helpers;

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

            // Get the detected MAC and IP addresses
            ParseHostMacAndIpLists();

            // Get the license files
            ParseLicenseFiles();

            // Get the main RLM statistics table
            ParseRlmStatistics();

            // Get the statistics for each ISV server
            ParseIsvStatistics();

            // Get and parse each of the debug log sections
            ParseDebugLogs();

            // Get the RLM instances from the log file
            ParseRlmInstances();
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
            var varLines = HelperMethods.GetLinesBetween("Environment:", "RLM hostid list:", _data);
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
        private void ParseHostMacAndIpLists()
        {
            var line = HelperMethods.GetLinesBetween("RLM hostid list:", "License files:", _data);
            var list = line[0].Split(" ");

            var isMac = new Regex("[A-Fa-f0-9]{12}");
            var isIp = new Regex("(?<=ip=)(([0-9]{1,3}.){4})");

            var macList = list.ToList()
                .Where(x => isMac.IsMatch(x))
                .Select(x => HelperMethods.MakeMac(x))
                .ToList();

            var ipList = list.ToList()
                .Where(x => isIp.IsMatch(x))
                .Select(x => isIp.Match(x).Value)
                .ToList();

            _log.HostMacList = macList;
            _log.HostIpList = ipList;
        }

        /// <summary>
        /// Parse the contents of each license file in the log file.
        /// </summary>
        private void ParseLicenseFiles()
        {
            var licenseList = HelperMethods.GetLinesBetween("LICENSE FILE:", "RLM Options", _data, true);

            var licenseData = GetLicenseData(licenseList);

            var licenses = new List<ILicenseFile>();
            foreach (var data in licenseData)
            {
                var license = LicenseFileFactory.Parse(data);
                licenses.Add(license);
            }

            _log.Licenses = licenses;
        }

        private List<string[]> GetLicenseData(string[] data)
        {
            // Create a new list of string arrays - Each string array
            // represents the raw text data for a different license file
            var list = new List<string[]>();

            // Nested for-loops, may need to refactor later.
            // Loop through the full text; At each point of a new license file,
            // signified by "LICENSE FILE: {name}", we will start a new for-loop
            // to gather all the data in this license file - stopping when a new
            // license file section is found or when the RLM Options section is found.
            // Then, we will fast-forward the outer for-loop to when the inner loop ended.
            for (var i = 0; i < data.Length; i++)
            {
                // When a new license file is found, begin the inner collection loop
                if (data[i].Contains("LICENSE FILE:"))
                {
                    // Create a new list of strings and add this first line, containing
                    // the file name, to the list
                    var license = new List<string>();
                    license.Add(data[i]);

                    // Start the inner for-loop, collecting all lines between the file name and
                    // the end of this license file section
                    for (var j = i + 1; j < data.Length; j++)
                    {
                        // If the current line contains a new file name or the RLM Options title,
                        // we have reached the end of the license file data and can break the loop
                        if (data[j].Contains("LICENSE FILE:") || data[j].Contains("RLM Options"))
                        {
                            break;
                        }

                        // Otherwise, add this line of text to the current license file list
                        license.Add(data[j]);
                    }

                    // Once the inner collection has been completed, convert this list to an array
                    // and add it to the parent list
                    list.Add(license.ToArray());
                }
            }

            // Return everything we found
            return list;
        }

        /// <summary>
        /// Parse the main RLM server statistics section from the log file.
        /// </summary>
        private void ParseRlmStatistics()
        {
            var rlmStatisticData = HelperMethods.GetLinesBetween("Status for \"rlm\"", "=============", _data, true);

            var rlmStatistics = RlmStatisticsTableFactory.Parse(rlmStatisticData);

            _log.RlmStatistics = rlmStatistics;
        }

        /// <summary>
        /// Parse the statistics from each ISV server in the log file.
        /// </summary>
        private void ParseIsvStatistics()
        {
            var isvStatistics = new List<IIsvStatistics>();

            // Get the ISV details from the main log data
            var relevantData = HelperMethods.GetLinesBetween("ISV Servers", "rlm debug log file contents", _data);

            // Break the full ISV details section into subsections for each ISV server
            var isvSections = HelperMethods.GetSubsections("ISV .+ status on", "ISV .+ status on", relevantData);

            // Build an ISV Statistics object from each subsection
            foreach (var section in isvSections)
            {
                var isv = IsvStatisticsFactory.Parse(section.ToArray());
                isvStatistics.Add(isv);
            }

            _log.IsvStatistics = isvStatistics;
        }

        /// <summary>
        /// Parse the debug log sections for the RLM server and each of the ISV servers
        /// into their own DebugLog objects.
        /// </summary>
        private void ParseDebugLogs()
        {
            var isvDebugLogs = new List<IDebugLog>();

            // Get all the debug logs from the file
            var debugLogSection = HelperMethods.GetLinesBetween("rlm debug log file contents", "RLM processes running on this machine", _data, true);

            // Split each debug log section into its own collection
            var logSections = HelperMethods.GetSubsections("debug log file contents", "END .+ debug log file contents", debugLogSection);

            foreach (var logSection in logSections)
            {
                var debugLog = DebugLogFactory.Parse(logSection.ToArray());

                if (logSection.ToArray()[0].Contains("rlm debug log"))
                {
                    _log.RlmLog = debugLog;
                }
                else
                {
                    isvDebugLogs.Add(debugLog);
                }
            }

            _log.IsvLogs = isvDebugLogs;
        }

        /// <summary>
        /// Parse the RLM instances section of the log file.
        /// </summary>
        private void ParseRlmInstances()
        {
            var instances = new List<IRlmInstance>();

            // Get the RLM instances section, this should run through the end of the file
            var rlmInstanceSection = HelperMethods.GetLinesBetween("^RLM processes running on this machine", null, _data);

            // Break this into subsections for each instance of RLM detected
            var rlmInstances = HelperMethods.GetSubsections("RLM Version", "RLM Version", rlmInstanceSection);

            foreach (var rlmInstance in rlmInstances)
            {
                var instance = RlmInstanceFactory.Parse(rlmInstance.ToArray());
                instances.Add(instance);
            }

            _log.RlmInstances = instances;
        }
    }
}