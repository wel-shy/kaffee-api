using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Kaffee.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Date")]
        public DateTime CreatedAt { get; set; }
        
        public string Email { get; set; }

        public string Password { get; set; }

        public string RefreshToken { get; set; }
    }
}
