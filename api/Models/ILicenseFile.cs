using System.Collections.Generic;

namespace API.Models
{
    public interface ILicenseFile
    {
        string Name { get; set; }
        string MemberName { get; set; }
        string UUID { get; set; }
        string Host { get; set; }
        IEnumerable<ILicense> Licenses { get; set; }
        bool IsRoaming { get; set; }
    }
}