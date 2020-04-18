using System.Collections.Generic;
using API.Entities;
using API.Helpers;
using API.Models;

namespace API.Factories
{
    public static class LicensePoolFactory
    {
        public static ILicensePool New() => new LicensePool();

        public static ILicensePool Parse(string[] data)
        {
            var pool = New();

            pool.Product = GetProductName(data);
            pool.TotalSeats = GetTotalSeats(data);
            pool.InUse = GetInUseSeats(data);
            pool.CheckedOutTo = GetCheckedOutToList(data);

            return pool;
        }

        private static IEnumerable<string> GetCheckedOutToList(string[] data)
        {
            var collection = new List<string>();
            for (var i = 2; i < data.Length; i++)
            {
                if (data[i].Contains("No licenses in use"))
                {
                    break;
                }

                collection.Add(data[i]);
            }

            return collection;
        }

        private static int GetInUseSeats(string[] data)
        {
            var inUse = HelperMethods.GetLineValue("Pool", 7, data);
            inUse = inUse.Substring("inuse:".Length);

            var inUseSeats = int.Parse(inUse);
            return inUseSeats;
        }

        private static int GetTotalSeats(string[] data)
        {
            var total = HelperMethods.GetLineValue("Pool", 6, data);
            total = total.Substring("Soft:".Length);

            var totalSeats = int.Parse(total);
            return totalSeats;
        }

        private static string GetProductName(string[] data)
        {
            var name = HelperMethods.GetLineValue("Pool", 2, data);
            return name;
        }
    }
}