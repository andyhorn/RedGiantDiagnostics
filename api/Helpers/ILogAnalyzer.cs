using System.Collections.Generic;
using API.Entities;

namespace API.Helpers
{
    public interface ILogAnalyzer
    {
        IEnumerable<AnalysisResult> Analyze(LogFile log);
    }
}