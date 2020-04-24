using System.Collections.Generic;
using System.Linq;
using API.Entities;
using API.Helpers;
using API.Models;

namespace API.Factories
{
    public interface ILicensePoolFactory
    {
        LicensePool New { get; }
        LicensePool Parse (string[] data);
    }

    public class LicensePoolFactory : ILicensePoolFactory
    {
        private IUtilities _utilities;
        public LicensePool New { get => new LicensePool(); }

        public LicensePoolFactory(IUtilities utilities)
        {
            _utilities = utilities;
        }

        public LicensePool Parse(string[] data)
        {
            if (data == null || data.Length == 0)
            {
                return null;
            }

            var pool = New;

            pool.Product = GetProductName(data);
            pool.TotalSeats = GetTotalSeats(data);
            pool.InUse = GetInUseSeats(data);
            pool.CheckedOutTo = GetCheckedOutToList(data);

            return pool;
        }

        private IEnumerable<string> GetCheckedOutToList(string[] data)
        {
            var collection = new List<string>();
            
            // Get the usage sections
            var checkoutLines = _utilities.GetLinesBetween("Usage for pool", null, data);

            // Validate the data
            if (checkoutLines == null || checkoutLines.Length == 0)
            {
                return collection;
            }

            // Filter out the separators
            checkoutLines = checkoutLines.Where(x => !x.Contains("======")).ToArray();

            // Parse the checkout data from each line
            foreach (var checkoutLine in checkoutLines)
            {
                var checkout = GetCheckout(checkoutLine);

                // If valid data was returned, add it to the collection
                if (!string.IsNullOrWhiteSpace(checkout))
                    collection.Add(checkout);
            }

            return collection;
        }

        private string GetCheckout(string data)
        {
            var user = string.Empty;
            var host = string.Empty;
            var time = string.Empty;

            // Split the string
            var columns = data.Split(":");

            // Check the length of the array and 
            // extract the available data
            if (columns.Length > 0)
                user = columns[0];
            
            if (columns.Length > 1)
                host = columns[1];

            if (columns.Length > 8)
                time = columns[8];

            var value = $"{user}:{host} - {time}";
            return value;
        }

        private int GetInUseSeats(string[] data)
        {
            int value = 0;

            // Get the desired string
            var inUse = _utilities.GetLineValue("Pool", 7, data);

            // Validate the data
            if (string.IsNullOrWhiteSpace(inUse))
            {
                return value;
            }

            // If the header exists, remove it
            if (inUse.Contains("inuse:"))
            {
                inUse = inUse.Substring("inuse:".Length);
            }

            int.TryParse(inUse, out value);
            return value;
        }

        private int GetTotalSeats(string[] data)
        {
            int value = 0;

            // Get the desired string
            var total = _utilities.GetLineValue("Pool", 6, data);

            // Validate the data
            if (string.IsNullOrWhiteSpace(total))
            {
                return value;
            }

            // If the header exists, remove it
            if (total.Contains("Soft:"))
            {
                total = total.Substring("Soft:".Length);
            }

            // Try to parse the integer
            int.TryParse(total, out value);

            return value;
        }

        private string GetProductName(string[] data)
        {
            var name = _utilities.GetLineValue("Pool", 2, data) ?? string.Empty;
            return name;
        }
    }
}