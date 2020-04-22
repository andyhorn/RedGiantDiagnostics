using API.Models;
using API.Entities;
using System;
using API.Helpers;

namespace API.Factories
{
    public static class ProductLicenseFactory
    {
        public static ProductLicense New { get => new ProductLicense(); }

        public static ProductLicense Parse(string[] data)
        {
            var product = New;

            product.IssueDate = GetProductIssueDate(data);
            product.ExpirationDate = GetProductExpirationDate(data);
            product.ProductName = GetProductName(data);
            product.Seats = GetProductSeats(data);

            return product;
        }

        private static DateTime? GetProductIssueDate(string[] data)
        {
            var issueDate = HelperMethods.GetLineValue("start=", 1, data);

            if (issueDate == null || issueDate.Length == 0)
            {
                return null;
            }

            if (issueDate.Contains("start="))
            {
                issueDate = issueDate.Substring("start=".Length);
            } 

            var date = HelperMethods.GetDateTimeFrom(issueDate);

            return date;
        }

        private static DateTime? GetProductExpirationDate(string[] data)
        {
            var expirationDate = HelperMethods.GetLineValue("LICENSE", 4, data);

            var date = HelperMethods.GetDateTimeFrom(expirationDate);

            return date;
        }

        private static string GetProductName(string[] data)
        {
            var name = HelperMethods.GetLineValue("LICENSE", 2, data);

            return name;
        }

        private static int GetProductSeats(string[] data)
        {
            var seats = HelperMethods.GetLineValue("LICENSE", 5, data);
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