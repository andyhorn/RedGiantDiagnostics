using System;
using API.Models;

namespace API.Entities
{
    public class StatisticsTable : IStatisticsTable
    {
        public string ServerName { get; set; }
        public DateTime[] StartTimes { get; set; }
        public int[] Messages { get; set; }
        public int[] Connections { get; set; }
    }
}