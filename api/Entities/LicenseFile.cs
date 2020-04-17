using System.Collections.Generic;
using API.Models;

namespace API.Entities
{
    public class LicenseFile : ILicenseFile
    {
        public string Name { get; set; }
        public string IsvName { get; set; }
        public string UUID { get; set; }
        public string HostAddress { get; set; }
        public string HostMac { get; set; }
        public string HostPort { get; set; }
        public string IsvPort { get; set; }
        public IEnumerable<IProductLicense> ProductLicenses { get; set; }
    }
}