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
            
            var checkoutLines = 
                _utilities.GetLinesBetween("Usage for pool", null, data)
                .Where(x => !x.Contains("======="));

            foreach (var checkoutLine in checkoutLines)
            {
                var checkout = GetCheckout(checkoutLine);
                collection.Add(checkout);
            }

            return collection;
        }

        private string GetCheckout(string data)
        {
            var columns = data.Split(":");
            var user = columns[0];
            var host = columns[1];
            var time = columns[8];

            var value = $"{user}:{host} - {time}";
            return value;
        }

        private int GetInUseSeats(string[] data)
        {
            var inUse = _utilities.GetLineValue("Pool", 7, data);
            inUse = inUse.Substring("inuse:".Length);

            var inUseSeats = int.Parse(inUse);
            return inUseSeats;
        }

        private int GetTotalSeats(string[] data)
        {
            var total = _utilities.GetLineValue("Pool", 6, data);
            total = total.Substring("Soft:".Length);

            var totalSeats = int.Parse(total);
            return totalSeats;
        }

        private string GetProductName(string[] data)
        {
            var name = _utilities.GetLineValue("Pool", 2, data);
            return name;
        }
    }
}