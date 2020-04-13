using System.Collections.Generic;

namespace API.Models
{
    public interface IDebugLog
    {
        string Id { get; set; }
        string Filename { get; set; }
        IEnumerable<string> Lines { get; set; }
    }
}