using API.Models;
using API.Entities;
using System;
using API.Helpers;

namespace API.Factories
{
    public interface IProductLicenseFactory
    {
        ProductLicense New { get; }
        ProductLicense Parse(string[] data);
    }
    public class ProductLicenseFactory : IProductLicenseFactory
    {
        private IUtilities _utilities;
        public ProductLicense New { get => new ProductLicense(); }

        public ProductLicenseFactory(IUtilities utilities)
        {
            _utilities = utilities;
        }
        public ProductLicense Parse(string[] data)
        {
            var product = New;

            product.IssueDate = GetProductIssueDate(data);
            product.ExpirationDate = GetProductExpirationDate(data);
            product.ProductName = GetProductName(data);
            product.Seats = GetProductSeats(data);

            return product;
        }

        private DateTime? GetProductIssueDate(string[] data)
        {
            var issueDate = _utilities.GetLineValue("start=", 1, data);

            if (issueDate == null || issueDate.Length == 0)
            {
                return null;
            }

            if (issueDate.Contains("start="))
            {
                issueDate = issueDate.Substring("start=".Length);
            } 

            var date = _utilities.GetDateTimeFrom(issueDate);

            return date;
        }

        private DateTime? GetProductExpirationDate(string[] data)
        {
            var expirationDate = _utilities.GetLineValue("LICENSE", 4, data);

            var date = _utilities.GetDateTimeFrom(expirationDate);

            return date;
        }

        private string GetProductName(string[] data)
        {
            var name = _utilities.GetLineValue("LICENSE", 2, data);

            return name;
        }

        private int GetProductSeats(string[] data)
        {
            var seats = _utilities.GetLineValue("LICENSE", 5, data);
            int num = 0;

            if (seats == "uncounted")
            {
                num = -1;
            }
            else
            {
                num = int.Parse(seats);
            }

            return num;
        }
    }
}