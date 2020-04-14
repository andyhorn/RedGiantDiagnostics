using System.Collections.Generic;

namespace API.Models
{
    public interface ILogFile
    {
        string Id { get; set; }
        string OwnerId { get; set; }
        IDictionary<string, string> EnvironmentVariables { get; set; }
        string RlmVersion { get; set; }
        IEnumerable<string> HostIdList { get; set; }
        string PrimaryHost { get; set; }
        string Hostname { get; set; }
        IEnumerable<ILicenseFile> Licenses { get; set; }
        IRlmStatisticsTable RlmStatistics { get; set; }
        IEnumerable<IIsvStatisticsTable> IsvStatistics { get; set; }
        IDebugLog RlmLog { get; set; }
        IEnumerable<IDebugLog> IsvLogs { get; set; }
        IEnumerable<IRlmInstance> RlmInstances { get; set; }
        IEnumerable<IAnalysisResult> AnalysisResults { get; set; }
        // void ReadData(string data); Move this to a Factory
        void Analyze();
    }
}