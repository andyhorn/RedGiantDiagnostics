using System;
using System.Collections.Generic;

namespace API.Contracts.Responses
{
    public class AnalyticsResponse
    {
        public int NumberOfLogs { get; set; }
        public List<DateTime> LogSaveDates { get; set; }
        public Dictionary<string, int> ErrorFrequency { get; set; }
        public Dictionary<string, int> WarningFrequency { get; set; }
        public Dictionary<string, int> SuggestionFrequency { get; set; }

        public AnalyticsResponse()
        {
            LogSaveDates = new List<DateTime>();
            ErrorFrequency = new Dictionary<string, int>();
            WarningFrequency = new Dictionary<string, int>();
            SuggestionFrequency = new Dictionary<string, int>();
        }
    }
}