using System;
using API.Models;

namespace API.Entities
{
    public class ProductLicense
    {
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string ProductName { get; set; }
        public int Seats { get; set; }
        public bool IsRenderOnly { get => ProductName.Contains("-ro"); }
    }
}