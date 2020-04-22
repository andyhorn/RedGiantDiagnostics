using System.Collections.Generic;
using API.Models;

namespace API.Entities
{
    public class DebugLog
    {
        public string Filename { get; set; }

        public IEnumerable<string> Lines { get; set; }
    }
}