using API.Models;
using API.Entities;
using System;

namespace API.Factories
{
    public class ProductLicenseFactory : IProductLicenseFactory
    {
        public IProductLicense New { get => new ProductLicense(); }

        public IProductLicense Parse(string[] data)
        {
            var product = New;

            product.IssueDate = GetProductIssueDate(data);
            product.ExpirationDate = GetProductExpirationDate(data);
            product.ProductName = GetProductName(data);
            product.Seats = GetProductSeats(data);

            return product;
        }

        private DateTime? GetDateFrom(string dateString)
        {
            if (dateString == null || dateString.Length == 0)
            {
                return null;
            }

            var date = new DateTime();
            if (DateTime.TryParse(dateString, out date))
            {
                return date;
            }

            return null;
        }

        private DateTime? GetProductIssueDate(string[] data)
        {
            var issueDate = GetLineValue("start=", 1, data);

            if (issueDate == null || issueDate.Length == 0)
            {
                return null;
            }

            if (issueDate.Contains("start="))
            {
                issueDate = issueDate.Substring("start=".Length);
            } 

            var date = GetDateFrom(issueDate);

            return date;
        }

        private DateTime? GetProductExpirationDate(string[] data)
        {
            var expirationDate = GetLineValue("LICENSE", 4, data);

            var date = GetDateFrom(expirationDate);

            return date;
        }

        private string GetProductName(string[] data)
        {
            var name = GetLineValue("LICENSE", 2, data);

            return name;
        }

        private int GetProductSeats(string[] data)
        {
            var seats = GetLineValue("LICENSE", 5, data);
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

        private string GetLineValue(string searchTerm, int word, string[] data)
        {
            string value = string.Empty;

            foreach (var line in data)
            {
                if (line.Contains(searchTerm))
                {
                    var words = line.Split(" ");

                    if (word < words.Length)
                        value = words[word].Trim();

                    break;
                }
            }

            return value;
        }
    }
}