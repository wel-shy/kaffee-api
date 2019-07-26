using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Kaffee.Models
{
    /// <summary>
    /// Model a leaderboard.
    /// </summary>
    public class Leaderboard 
    {    
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Date")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedAt { get; set; }

        public string[] Administrators { get; set; }

        public string[] Members { get; set; }

        public string Name { get; set; }

        [RegularExpression(@"^#([0-9a-zA-Z]{6})|#([0-9a-zA-Z]){3}$", ErrorMessage = "Must be a valid hex colour.")]
        public string Colour { get; set; }

        public override string ToString() 
        {
            return string.Format("id: {0}, createdAt: {1}, Name: {2}, admin: {3}", Id, CreatedAt, Name, Administrators.Length);
        }
    }
}