using System.Collections.Generic;
using System.Linq;
using API.Entities;
using API.Helpers;
using API.Models;

namespace API.Factories
{
    public interface ILicenseFileFactory
    {
        LicenseFile New { get; }
        LicenseFile Parse(string[] data);
    }

    public class LicenseFileFactory : ILicenseFileFactory
    {
        private IUtilities _utilities;
        private IProductLicenseFactory _productLicenseFactory;
        public LicenseFile New { get => new LicenseFile(); }

        public LicenseFileFactory(IUtilities utilities, IProductLicenseFactory productLicenseFactory)
        {
            _utilities = utilities;
            _productLicenseFactory = productLicenseFactory;
        }

        public LicenseFile Parse(string[] data)
        {
            var licenseFile = New;

            // licenseFile.Name = GetLicenseName(data);
            licenseFile.Name = _utilities.GetLineValue("LICENSE FILE", 2, data);
            // licenseFile.UUID = GetLicenseUuid(data);
            licenseFile.UUID = _utilities.GetLineValue("license uuid", 3, data);
            // licenseFile.HostAddress = GetLicenseHostAddress(data);
            licenseFile.HostAddress = _utilities.GetLineValue("HOST", 1, data);
            licenseFile.HostMac = GetLicenseHostMac(data);
            // licenseFile.HostPort = GetLicenseHostPort(data);
            licenseFile.HostPort = _utilities.GetLineValue("HOST", 3, data);
            // licenseFile.IsvName = GetLicenseIsvName(data);
            licenseFile.IsvName = _utilities.GetLineValue("ISV", 1, data);
            licenseFile.IsvPort = GetLicenseIsvPort(data);
            licenseFile.ProductLicenses = GetLicenseProducts(data);

            return licenseFile;
        }

        private string GetLicenseHostMac(string[] data)
        {
            var mac = _utilities.GetLineValue("HOST", 2, data);

            // Validate data
            if (string.IsNullOrWhiteSpace(mac))
            {
                return string.Empty;
            }

            if (mac.Contains("ether="))
            {
                mac = mac.Substring("ether=".Length);
            }

            mac = _utilities.MakeMac(mac);

            return mac;
        }

        private string GetLicenseIsvPort(string[] data)
        {
            string isvPort = string.Empty;

            // Get the string containing the isv port number
            var result = _utilities.GetLineValue("ISV", 2, data);

            // Validate result
            if (string.IsNullOrWhiteSpace(result))
            {
                return isvPort;
            }

            // Remove the "port=" header
            if (result.Contains("port="))
                result = result.Substring("port=".Length);

            isvPort = result;

            return isvPort;
        }

        private IEnumerable<ProductLicense> GetLicenseProducts(string[] data)
        {
            var productList = _utilities.GetSubsections("[0-9]+-seat license", "[0-9]+-seat license", data);
            var productLicenses = new List<ProductLicense>();
            
            foreach (var product in productList)
            {
                var newProductLicense = _productLicenseFactory.Parse(product.ToArray());
                productLicenses.Add(newProductLicense);
            }

            return productLicenses;
        }
    }
}