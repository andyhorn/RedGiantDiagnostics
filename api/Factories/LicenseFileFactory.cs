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
    }
}