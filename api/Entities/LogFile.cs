using System;
using System.Collections.Generic;
using System.Linq;
using API.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace API.Entities
{
    public class LogFile
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null;
        public string OwnerId { get; set; } = null;
        public string Title { get; set; } = null;
        public string Comments { get; set; } = null;
        public DateTime UploadDate { get; set; } = DateTime.Now;
        public DateTime? Date { get; set; } = null;
        public IDictionary<string, string> EnvironmentVariables { get; set; } = new Dictionary<string, string>();
        public string RlmVersion { get; set; } = null;
        public IEnumerable<string> HostMacList { get; set; } = new List<string>();
        public IEnumerable<string> HostIpList { get; set; } = new List<string>();
        public string PrimaryHostAddress
        {
            get
            {
                if (HostIpList.Count() > 0)
                {
                    return HostIpList.ElementAtOrDefault(0);
                }

                return string.Empty;
            }
        }
        public string PrimaryHostMac 
        { 
            get
            {
                if (HostMacList.Count() > 0)
                {
                    return HostMacList.ElementAtOrDefault(0);
                }

                return string.Empty;
            }
        }
        public string Hostname { get; set; } = null;
        public IEnumerable<LicenseFile> Licenses { get; set; } = new List<LicenseFile>();
        public RlmStatisticsTable RlmStatistics { get; set; } = null;
        public IEnumerable<IsvStatistics> IsvStatistics { get; set; } = new List<IsvStatistics>();
        public DebugLog RlmLog { get; set; } = null;
        public IEnumerable<DebugLog> IsvLogs { get; set; } = new List<DebugLog>();
        public IEnumerable<RlmInstance> RlmInstances { get; set; } = new List<RlmInstance>();
        public IEnumerable<AnalysisResult> AnalysisResults { get; set; } = new List<AnalysisResult>();
    }
}