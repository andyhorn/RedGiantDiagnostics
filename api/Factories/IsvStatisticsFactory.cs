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

            var poolData = HelperMethods.GetLinesBetween("License pool status", "ISV", data).ToList();
            // poolData.RemoveAt(poolData.Count - 1); // Remove the last cell containing "===="

            var licensePoolSections = GetLicensePoolSections(poolData.ToArray());

            foreach (var section in licensePoolSections)
            {
                var licensePool = LicensePoolFactory.Parse(section.ToArray());
                licensePools.Add(licensePool);
            }

            return licensePools;
        }

        private static IEnumerable<IEnumerable<string>> GetLicensePoolSections(string[] data)
        {
            var masterCollection = new List<string[]>();

            // Loop through the full list of license pool data for this ISV server.
            for (var i = 0; i < data.Length; i++)
            {
                // If this line is the marking of a new product's license pool, we 
                // will enter the nested for-loop to gather the related lines of data.
                if (data[i].Contains("Pool"))
                {
                    // Create a new collection of strings
                    var collection = new List<string>();

                    // Add this header line to the collection
                    collection.Add(data[i]);

                    // Start the nested for-loop, beginning on the line after the header.
                    for (var j = i + 1; j < data.Length; j++)
                    {
                        // If the current line marks the beginning of a new product's
                        // license pool section, we will move the outer for-loop to the
                        // current line and then break this inner for-loop.
                        if (data[j].Contains("Pool"))
                        {
                            i = j - 1;
                            break;
                        }

                        // Otherwise, we will add the current line to this current
                        // collection of license pool data.
                        collection.Add(data[j]);
                    }

                    // Once we have finished the nested for-loop, collecting lines of data
                    // for an individual product's license pool, we will add that collection
                    // to the parent collection.
                    masterCollection.Add(collection.ToArray());
                }
            }

            return masterCollection;
        }

        private static string GetServerName(string[] data)
        {
            var name = HelperMethods.GetLineValue("ISV .+ status on", 1, data);
            return name;
        }
    }
}