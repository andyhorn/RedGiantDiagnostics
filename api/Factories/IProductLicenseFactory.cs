using API.Models;

namespace API.Factories
{
    public interface IProductLicenseFactory
    {
        IProductLicense New { get; }

        IProductLicense Parse(string[] data);
    }
}