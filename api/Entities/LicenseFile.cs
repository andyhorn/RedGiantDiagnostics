using System.Collections.Generic;
using API.Models;

namespace API.Entities
{
    public class LicenseFile : ILicenseFile
    {
        public string Name { get; set; }
        public string MemberName { get; set; }
        public string UUID { get; set; }
        public string Host { get; set; }
        public IEnumerable<ILicense> Licenses { get; set; }
        public bool IsRoaming { get; set; }
    }
}