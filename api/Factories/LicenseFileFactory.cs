using API.Entities;
using API.Models;

namespace API.Factories
{
    public class LicenseFileFactory : ILicenseFileFactory
    {
        public ILicenseFile New() => new LicenseFile();

        public ILicenseFile Parse(string[] data)
        {
            var licenseFile = New();

            licenseFile.Name = GetLicenseName(data);
            licenseFile.UUID = GetLicenseUuid(data);
            licenseFile.HostAddress = GetLicenseHostAddress(data);
            licenseFile.HostMac = GetLicenseHostMac(data);

            return licenseFile;
        }

        private string GetLicenseName(string[] data)
        {
            string name = string.Empty;

            foreach (var line in data)
            {
                if (line.Contains("LICENSE FILE:"))
                {
                    name = line.Substring("LICENSE FILE: ".Length);
                    if (name.Contains(" ---- contents"))
                        name = name.Substring(0, name.Length - " ---- contents".Length);
                    break;
                }
            }

            return name;
        }

        private string GetLicenseUuid(string[] data)
        {
            string uuid = string.Empty;

            foreach (var line in data)
            {
                if (line.Contains("license uuid"))
                {
                    var sections = line.Split(" ");
                    uuid = sections[3].Trim();
                    break;
                }
            }

            return uuid;
        }

        private string GetLicenseHostAddress(string[] data)
        {
            string address = string.Empty;

            foreach (var line in data)
            {
                if (line.Contains("HOST"))
                {
                    var sections = line.Split(" ");
                    address = sections[1].Trim();
                    break;
                }
            }

            return address;
        }

        private string GetLicenseHostMac(string[] data)
        {
            string mac = string.Empty;

            foreach (var line in data)
            {
                if (line.Contains("HOST"))
                {
                    var sections = line.Split(" ");
                    mac = sections[2].Trim();
                    break;
                }
            }

            if (mac.Contains("ether="))
            {
                mac = mac.Substring("ether=".Length);
            }

            mac = MakeMacAddress(mac);

            return mac;
        }

        private string MakeMacAddress(string mac)
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