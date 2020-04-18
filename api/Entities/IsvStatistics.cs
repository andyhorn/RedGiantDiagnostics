using System;
using System.Collections.Generic;
using API.Models;

namespace API.Entities
{
    public class IsvStatistics : IIsvStatistics
    {
        public int[] Checkouts { get; set; }
        public int[] Denials { get; set; }
        public int[] LicenseRemovals { get; set; }
        public string ServerName { get; set; }
        public DateTime[] StartTimes { get; set; }
        public int[] Messages { get; set; }
        public int[] Connections { get; set; }
        public IEnumerable<ILicensePool> LicensePools { get; set; }
    }
}