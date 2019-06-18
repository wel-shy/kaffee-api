using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Kaffee.Models
{
    public class Coffee
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Date")]
        public DateTime CreatedAt { get; set; }
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }

        public string From { get; set; }
    }
}
