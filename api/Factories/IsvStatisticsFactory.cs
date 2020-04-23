using System.Collections.Generic;
using System.Linq;
using API.Entities;
using API.Helpers;
using API.Models;

namespace API.Factories
{
    public interface IIsvStatisticsFactory
    {
        IsvStatistics New { get; }
        IsvStatistics Parse(string[] data);
    }

    public class IsvStatisticsFactory : IIsvStatisticsFactory
    {
        private IUtilities _utilities;
        private ILicensePoolFactory _licensePoolFactory;
        public IsvStatistics New { get => new IsvStatistics(); }

        public IsvStatisticsFactory(IUtilities utilities, ILicensePoolFactory licensePoolFactory)
        {
            _utilities = utilities;
            _licensePoolFactory = licensePoolFactory;
        }

        public IsvStatistics Parse(string[] data)
        {
            if (data == null || data.Length == 0)
            {
                return null;
            }

            var isvStatistics = New;

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

        private IEnumerable<LicensePool> GetLicensePools(string[] data)
        {
            var licensePools = new List<LicensePool>();

            // Get the license pool section from the log file
            var poolData = _utilities.GetLinesBetween("License pool status", "ISV", data).ToList();

            // Break the license pools into their own subsections
            var licensePoolSections = _utilities.GetSubsections("Pool", "Pool", poolData.ToArray());

            // Build LicensePool objects from each subsection of the file
            foreach (var section in licensePoolSections)
            {
                var licensePool = _licensePoolFactory.Parse(section.ToArray());
                licensePools.Add(licensePool);
            }

            return licensePools;
        }

        private string GetServerName(string[] data)
        {
            var name = _utilities.GetLineValue("ISV .+ status on", 1, data);
            return name;
        }
    }
}