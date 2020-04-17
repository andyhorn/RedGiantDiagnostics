using API.Entities;

namespace API.Factories
{
    public static class RlmStatisticsTableFactory
    {
        public static RlmStatisticsTable New { get => new RlmStatisticsTable(); }

        public static RlmStatisticsTable Parse(string[] data)
        {
            var table = New;

            return table;
        }
    }
}