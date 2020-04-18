using System.Collections.Generic;
using API.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace API.Entities
{
    public class DebugLog : IDebugLog
    {

        public string Filename { get; set; }

        public IEnumerable<string> Lines { get; set; }
    }
}