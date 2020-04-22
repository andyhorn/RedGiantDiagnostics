using System.Linq;
using System.Threading;
using API.Entities;
using API.Models;

namespace API.Factories
{
    public class LogFactory : ILogFactory
    {
        public LogFile New() => new LogFile();

        public LogFile Parse(string rawData)
        {
            // Prepare the raw data for parsing
            string[] data = PrepareData(rawData);

            // Instantiate a new log parser with a new LogFile and
            // the cleansed data
            var parser = new LogParser(New(), data);

            // Start the log parser's Parse method on a new thread
            // and wait for it to complete
            var parseThread = new Thread(parser.Parse);
            parseThread.Start();
            parseThread.Join();

            // Retrieve the parsed log from the log parser
            var log = parser.Log;

            // Return the LogFile
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
    }
}