using System;

namespace API.Models
{
    public interface IServerStatisticsTable
    {
        string ServerName { get; set; }
        DateTime[] StartTimes { get; set; }
        int[] Messages { get; set; }
        int[] Connections { get; set; }
    }
}