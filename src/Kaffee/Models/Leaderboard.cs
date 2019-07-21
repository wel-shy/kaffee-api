using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Kaffee.Models
{
    public class Leaderboard 
    {    
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Date")]
        public DateTime CreatedAt { get; set; }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string[] Administrators { get; set; }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string[] Members { get; set; }

        public string Name { get; set; }
    }
}