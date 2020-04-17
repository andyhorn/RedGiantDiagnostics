using System.Collections.Generic;

namespace API.Models
{
    public interface ILicenseFile
    {
        string Name { get; set; }
        string IsvName { get; set; }
        string UUID { get; set; }
        string HostAddress { get; set; }
        string HostMac { get; set; }
        string HostPort { get; set; }
        string IsvPort { get; set; }
        IEnumerable<IProductLicense> ProductLicenses { get; set; }
        bool IsRoaming { get; set; }
    }
}