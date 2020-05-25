using API.Helpers;

namespace API.Factories
{
    public static class LogAnalyzerFactory
    {
        public static ILogAnalyzer New { get => new LogAnalyzer(); }
    }
}