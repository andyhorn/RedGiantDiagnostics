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
        private IStatisticsParser _statisticsParsers;
        public IsvStatistics New { get => new IsvStatistics(); }

        public IsvStatisticsFactory(IUtilities utilities, ILicensePoolFactory licensePoolFactory, IStatisticsParser statisticsParsers)
        {
            _utilities = utilities;
            _licensePoolFactory = licensePoolFactory;
            _statisticsParsers = statisticsParsers;
        }

        public IsvStatistics Parse(string[] data)
        {
            if (data == null || data.Length == 0)
            {
                return null;
            }

            var isvStatistics = New;

            isvStatistics.ServerName = GetServerName(data);
            isvStatistics.StartTimes = _statisticsParsers.ParseTableDates(data);
            isvStatistics.Checkouts = _statisticsParsers.GetColumnValues("Checkouts:", data);
            isvStatistics.Connections = _statisticsParsers.GetColumnValues("Connections:", data);
            isvStatistics.Denials = _statisticsParsers.GetColumnValues("Denials:", data);
            isvStatistics.LicenseRemovals = _statisticsParsers.GetColumnValues("License removals:", data);
            isvStatistics.Messages = _statisticsParsers.GetColumnValues("Messages:", data);

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