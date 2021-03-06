using System.Linq;
using System.Threading;
using API.Entities;
using API.Models;

namespace API.Factories
{
    public interface ILogFactory
    {
        LogFile New { get; }
        LogFile Parse(string data);
    }

    public class LogFactory : ILogFactory
    {
        private ILogParserFactory _logParserFactory;
        public LogFile New { get => new LogFile(); }


        public LogFactory(ILogParserFactory logParserFactory)
        {
            _logParserFactory = logParserFactory;
        }

        public LogFile Parse(string rawData)
        {
            // Validate the incoming data
            if (string.IsNullOrWhiteSpace(rawData))
            {
                return null;
            }

            // Prepare the raw data for parsing
            string[] data = PrepareData(rawData);

            // Instantiate a new log parser with a new LogFile and
            // the cleansed data
            var parser = _logParserFactory.New;

            // Run the parse, which will take place on a background thread
            var log = parser.Parse(New, data);

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