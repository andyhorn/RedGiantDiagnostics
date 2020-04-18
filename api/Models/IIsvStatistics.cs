using System.Collections.Generic;

namespace API.Models
{
    public interface IIsvStatistics : IServerStatistics
    {
        int[] Checkouts { get; set; }
        int[] Denials { get; set; }
        int[] LicenseRemovals { get; set; }
        IEnumerable<ILicensePool> LicensePools { get; set; }
    }
}