using System.Collections.Generic;
using System.Linq;
using API.Entities;
using API.Helpers;
using API.Models;

namespace API.Factories
{
    public static class IsvStatisticsFactory
    {
        public static IIsvStatistics New() => new IsvStatistics();

        public static IIsvStatistics Parse(string[] data)
        {
            var isvStatistics = New();

            isvStatistics.ServerName = GetServerName(data);
            isvStatistics.StartTimes = StatisticsParsers.ParseTableDates(data);
            isvStatistics.Checkouts = StatisticsParsers.GetColumnValues("Checkouts:", data);
            isvStatistics.Connections = StatisticsParsers.GetColumnValues("Connections:", data);
            isvStatistics.Denials = StatisticsParsers.GetColumnValues("Denials:", data);
            isvStatistics.LicenseRemovals = StatisticsParsers.GetColumnValues("License removals:", data);
            isvStatistics.Messages = StatisticsParsers.GetColumnValues("Messages:", data);

            isvStatistics.LicensePools = GetLicensePools(data);

            return isvStatistics;
        }

        private static IEnumerable<ILicensePool> GetLicensePools(string[] data)
        {
            var licensePools = new List<ILicensePool>();

            // Get the license pool section from the log file
            var poolData = HelperMethods.GetLinesBetween("License pool status", "ISV", data).ToList();

            // Break the license pools into their own subsections
            var licensePoolSections = HelperMethods.GetSubsections("Pool", "Pool", poolData.ToArray());

            // Build LicensePool objects from each subsection of the file
            foreach (var section in licensePoolSections)
            {
                var licensePool = LicensePoolFactory.Parse(section.ToArray());
                licensePools.Add(licensePool);
            }

            return licensePools;
        }

        private static string GetServerName(string[] data)
        {
            var name = HelperMethods.GetLineValue("ISV .+ status on", 1, data);
            return name;
        }
    }
}