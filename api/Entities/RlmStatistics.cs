using System;
using System.Collections.Generic;
using API.Models;

namespace API.Entities
{
    public class RlmStatisticsTable
    {
        public IEnumerable<ServerStatus> Servers { get; set; }
        public string ServerName { get; set; }
        public DateTime[] StartTimes { get; set; }
        public int[] Messages { get; set; }
        public int[] Connections { get; set; }
    }
}