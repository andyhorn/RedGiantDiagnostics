using System;

namespace API.Models
{
    public interface IServerStatistics
    {
        string ServerName { get; set; }
        DateTime[] StartTimes { get; set; }
        int[] Messages { get; set; }
        int[] Connections { get; set; }
    }
}