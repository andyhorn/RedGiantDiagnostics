using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using API.Entities;
using API.Models;

namespace API.Factories
{
    public class LogFactory : ILogFactory
    {
        private string[] _data;
        private ILogFile _log;
        public ILogFile New() => new LogFile();

        public LogFactory()
        {
            _data = null;
            _log = null;
        }

        public ILogFile Parse(string data)
        {
            _log = New();
            _data = PrepareData(data);

            var thread = new Thread(Exec);
            thread.Start(this);
            thread.Join();

            var log = _log;

            _log = null;
            _data = null;

            return log;
        }

        private string[] PrepareData(string data)
        {
            // Remove all carriage returns from the text
            data = data.Replace("\r", "");

            // Split into a list at all newlines
            var list = data.Split("\n").ToList();

            // Remove all empty lines
            list.RemoveAll(x => string.IsNullOrWhiteSpace(x));

            // Remove all whitespace from the beginning and end
            for (var i = 0; i < list.Count; i++)
            {
                list[i] = list[i].Trim();
            }

            return list.ToArray();
        }

        private static void Exec(object param)
        {
            var factory = param as LogFactory;

            // Parse the log metadata
            factory.ParseMetadata();

            // Parse the Environment Variables
            factory.ParseEnvironmentVariables();
        }

        private void ParseMetadata()
        {
            var md = LogParser.ParseMetadata(_data);
            _log.Hostname = md["Hostname"];
            _log.RlmVersion = md["Rlm Version"];
            _log.Date = DateTime.Parse($"{md["Date"]} {md["Time"]}");
        }

        private void ParseEnvironmentVariables()
        {
            var env = LogParser.ParseEnvironmentVariables(_data);
            _log.EnvironmentVariables = env;
        }
    }
}