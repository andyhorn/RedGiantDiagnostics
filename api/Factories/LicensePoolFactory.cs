using System.Collections.Generic;
using System.Linq;
using API.Entities;
using API.Helpers;
using API.Models;

namespace API.Factories
{
    public static class LicensePoolFactory
    {
        public static LicensePool New() => new LicensePool();

        public static LicensePool Parse(string[] data)
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
            
            var checkoutLines = 
                HelperMethods.GetLinesBetween("Usage for pool", null, data)
                .Where(x => !x.Contains("======="));

            foreach (var checkoutLine in checkoutLines)
            {
                var checkout = GetCheckout(checkoutLine);
                collection.Add(checkout);
            }

            return collection;
        }

        private static string GetCheckout(string data)
        {
            var columns = data.Split(":");
            var user = columns[0];
            var host = columns[1];
            var time = columns[8];

            var value = $"{user}:{host} - {time}";
            return value;
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