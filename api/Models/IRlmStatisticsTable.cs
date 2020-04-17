using System;
using System.Collections.Generic;

namespace API.Models
{
    public interface IRlmStatisticsTable
    {
        string ServerName { get; set; }
        DateTime[] StartTimes { get; set; }
        int[] Messages { get; set; }
        int[] Connections { get; set; }
        IEnumerable<IServerStatus> Servers { get; set; }
    }
}