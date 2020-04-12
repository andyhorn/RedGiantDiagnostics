using System;

namespace API.Models
{
    public interface ILicense
    {
        DateTime IssueDate { get; set; }
        DateTime ExpirationDate { get; set; }
        string ProductName { get; set; }
        int Seats { get; set; }
        bool IsRenderOnly { get; set; }
    }
}