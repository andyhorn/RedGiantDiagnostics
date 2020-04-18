using System.Collections.Generic;

namespace API.Models
{
    public interface ILicensePool
    {
        string Product { get; set; }
        int Available { get; set; }
        int InUse { get; set; }
        IEnumerable<string> CheckedOutTo { get; set; }
    }
}