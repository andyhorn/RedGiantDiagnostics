using System;
using API.Models;

namespace API.Entities
{
    public class IsvStatisticsTable : IIsvStatisticsTable
    {
        public int[] Checkouts { get; set; }
        public int[] Denials { get; set; }
        public int[] LicenseRemovals { get; set; }
        public string ServerName { get; set; }
        public DateTime[] StartTimes { get; set; }
        public int[] Messages { get; set; }
        public int[] Connections { get; set; }
    }
}