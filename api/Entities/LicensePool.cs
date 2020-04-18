using System.Collections.Generic;
using API.Models;

namespace API.Entities
{
    public class LicensePool : ILicensePool
    {
        public string Product { get; set; }
        public int Available { get => TotalSeats - InUse; }
        public int TotalSeats { get; set; }
        public int InUse { get; set; }
        public IEnumerable<string> CheckedOutTo { get; set; }
    }
}