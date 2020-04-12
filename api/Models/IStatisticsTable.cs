using System;

namespace API.Models
{
    public interface IStatisticsTable
    {
        string ServerName { get; set; }
        DateTime[] StartTimes { get; set; }
        int[] Messages { get; set; }
        int[] Connections { get; set; }
    }
}