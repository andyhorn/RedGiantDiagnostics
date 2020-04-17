using API.Models;

namespace API.Factories
{
    public interface ILicenseFileFactory
    {
        ILicenseFile New();
        ILicenseFile Parse(string[] data);
    }
}