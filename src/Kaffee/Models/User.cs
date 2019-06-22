using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Kaffee.Models
{   
    /// <summary>
    /// Model a user.
    /// </summary>
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Date")]
        public DateTime CreatedAt { get; set; }
        
        [Required, StringLength(100)]
        public string Email { get; set; }

        [Required, StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        public string IV { get; set; }

        public string RefreshToken { get; set; }
    }
}
