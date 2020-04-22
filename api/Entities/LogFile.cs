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
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string OwnerId { get; set; }
        public DateTime Date { get; set; }
        public IDictionary<string, string> EnvironmentVariables { get; set; }
        public string RlmVersion { get; set; }
        public IEnumerable<string> HostMacList { get; set; }
        public IEnumerable<string> HostIpList { get; set; }
        public string PrimaryHost 
        { 
            get
            {
                if (HostMacList.Count() > 0)
                {
                    return HostMacList.ToList()[0];
                }
                return string.Empty;
            }
        }
        public string Hostname { get; set; }
        public IEnumerable<LicenseFile> Licenses { get; set; }
        public RlmStatisticsTable RlmStatistics { get; set; }
        public IEnumerable<IsvStatistics> IsvStatistics { get; set; }
        public IDebugLog RlmLog { get; set; }
        public IEnumerable<DebugLog> IsvLogs { get; set; }
        public IEnumerable<RlmInstance> RlmInstances { get; set; }
        public IEnumerable<AnalysisResult> AnalysisResults { get; set; }
    }
}