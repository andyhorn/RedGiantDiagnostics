using System;
using System.Collections.Generic;
using API.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace API.Entities
{
    public class LogFile : ILogFile
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string OwnerId { get; set; }
        public DateTime Date { get; set; }
        public IDictionary<string, string> EnvironmentVariables { get; set; }
        public string RlmVersion { get; set; }
        public IEnumerable<string> HostIdList { get; set; }
        public string PrimaryHost { get; set; }
        public string Hostname { get; set; }
        public IEnumerable<ILicenseFile> Licenses { get; set; }
        public IRlmStatisticsTable RlmStatistics { get; set; }
        public IEnumerable<IIsvStatisticsTable> IsvStatistics { get; set; }
        public IDebugLog RlmLog { get; set; }
        public IEnumerable<IDebugLog> IsvLogs { get; set; }
        public IEnumerable<IRlmInstance> RlmInstances { get; set; }
        public IEnumerable<IAnalysisResult> AnalysisResults { get; set; }
    }
}