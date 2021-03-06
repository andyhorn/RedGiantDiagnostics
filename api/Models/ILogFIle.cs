using System;
using System.Collections.Generic;

namespace API.Models
{
    public interface ILogFile
    {
        string Id { get; set; }
        string OwnerId { get; set; }
        DateTime Date { get; set; }
        IDictionary<string, string> EnvironmentVariables { get; set; }
        string RlmVersion { get; set; }
        IEnumerable<string> HostMacList { get; set; }
        IEnumerable<string> HostIpList { get; set; }
        string PrimaryHost { get; }
        string Hostname { get; set; }
        IEnumerable<ILicenseFile> Licenses { get; set; }
        IRlmStatistics RlmStatistics { get; set; }
        IEnumerable<IIsvStatistics> IsvStatistics { get; set; }
        IDebugLog RlmLog { get; set; }
        IEnumerable<IDebugLog> IsvLogs { get; set; }
        IEnumerable<IRlmInstance> RlmInstances { get; set; }
        IEnumerable<IAnalysisResult> AnalysisResults { get; set; }
    }
}