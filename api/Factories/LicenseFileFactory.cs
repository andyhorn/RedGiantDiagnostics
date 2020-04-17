using System.Collections.Generic;
using API.Entities;
using API.Models;

namespace API.Factories
{
    public static class LicenseFileFactory
    {
        public static ILicenseFile New() => new LicenseFile();

        public static ILicenseFile Parse(string[] data)
        {
            var licenseFile = New();

            licenseFile.Name = GetLicenseName(data);
            licenseFile.UUID = GetLicenseUuid(data);
            licenseFile.HostAddress = GetLicenseHostAddress(data);
            licenseFile.HostMac = GetLicenseHostMac(data);
            licenseFile.HostPort = GetLicenseHostPort(data);
            licenseFile.IsvName = GetLicenseIsvName(data);
            licenseFile.IsvPort = GetLicenseIsvPort(data);
            licenseFile.ProductLicenses = GetLicenseProducts(data);

            return licenseFile;
        }

        private static string GetLicenseName(string[] data)
        {
            string name = string.Empty;

            name = GetLineValue("LICENSE FILE", 2, data);

            return name;
        }

        private static string GetLicenseUuid(string[] data)
        {
            string uuid = string.Empty;

            uuid = GetLineValue("license uuid", 3, data);

            return uuid;
        }

        private static string GetLicenseHostAddress(string[] data)
        {
            string address = string.Empty;

            address = GetLineValue("HOST", 1, data);

            return address;
        }

        private static string GetLicenseHostMac(string[] data)
        {
            string mac = string.Empty;

            mac = GetLineValue("HOST", 2, data);

            if (mac.Contains("ether="))
            {
                mac = mac.Substring("ether=".Length);
            }

            mac = MakeMacAddress(mac);

            return mac;
        }

        private static string GetLicenseHostPort(string[] data)
        {
            string port = string.Empty;

            port = GetLineValue("HOST", 3, data);

            return port;
        }

        private static string GetLicenseIsvName(string[] data)
        {
            string isvName = string.Empty;

            isvName = GetLineValue("ISV", 1, data);

            return isvName;
        }

        private static string GetLicenseIsvPort(string[] data)
        {
            string isvPort = string.Empty;

            isvPort = GetLineValue("ISV", 2, data);

            if (isvPort.Contains("port="))
                isvPort = isvPort.Substring("port=".Length);

            return isvPort;
        }

        private static IEnumerable<IProductLicense> GetLicenseProducts(string[] data)
        {
            var productData = new List<string[]>();
            var productLicenses = new List<IProductLicense>();

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
                var newProductLicense = ProductLicenseFactory.Parse(product);
                productLicenses.Add(newProductLicense);
            }

            return productLicenses;
        }

        private static string GetLineValue(string searchTerm, int section, string[] data)
        {
            string value = string.Empty;

            foreach (var line in data)
            {
                if (line.Contains(searchTerm))
                {
                    var sections = line.Split(" ");

                    if (section < sections.Length)
                        value = sections[section].Trim();

                    break;
                }
            }

            return value;
        }

        private static string MakeMacAddress(string mac)
        {
            mac = mac.Replace(":", "");
            mac = mac.Replace("-", "");
            mac = mac.Replace(".", "");

            string newMac = string.Empty;

            for (var i = 0; i < mac.Length; i++)
            {
                if (i % 2 == 0 && i > 1)
                {
                    newMac += ":";
                }

                newMac += mac[i].ToString().ToUpper();
            }

            return newMac;
        }
    }
}