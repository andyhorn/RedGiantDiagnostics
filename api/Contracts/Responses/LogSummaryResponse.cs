using System;
using API.Entities;

namespace API.Contracts
{
    public class LogSummaryResponse
    {
        public string LogId { get; set; }
        public string OwnerId { get; set; }
        public string LogTitle { get; set; }
        public string Comments { get; set; }
        public DateTime UploadDate { get; set; }

        public LogSummaryResponse(LogFile log)
        {
            LogId = log.Id;
            OwnerId = log.OwnerId;
            LogTitle = log.Title;
            Comments = log.Comments;
            UploadDate = log.UploadDate;
        }
    }
}