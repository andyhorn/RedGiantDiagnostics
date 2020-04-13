using System.Collections.Generic;
using API.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace API.Entities
{
    public class DebugLog : IDebugLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Filename { get; set; }

        public IEnumerable<string> Lines { get; set; }
    }
}