using System.Collections.Generic;
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
            string mac = string.Empty;

            mac = _utilities.GetLineValue("HOST", 2, data);

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

            isvPort = _utilities.GetLineValue("ISV", 2, data);

            if (isvPort.Contains("port="))
                isvPort = isvPort.Substring("port=".Length);

            return isvPort;
        }

        private IEnumerable<ProductLicense> GetLicenseProducts(string[] data)
        {
            var productData = new List<string[]>();
            var productLicenses = new List<ProductLicense>();

            for (var i = 0; i < data.Length; i++)
            {
                if (data[i].Contains("LICENSE") && !data[i].Contains("FILE:"))
                {
                    var section = new List<string>();

                    for (var j = i; j < data.Length; j++)
                    {
                        section.Add(data[j]);

                        if (data[j].Contains("sig="))
                        {
                            i = j;
                            break;
                        }
                    }

                    productData.Add(section.ToArray());
                }
            }

            foreach (var product in productData)
            {
                var newProductLicense = _productLicenseFactory.Parse(product);
                productLicenses.Add(newProductLicense);
            }

            return productLicenses;
        }
    }
}