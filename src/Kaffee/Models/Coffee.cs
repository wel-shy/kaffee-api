using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

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
        [JsonConverter(typeof(StringEnumConverter))]  // JSON.Net
        [BsonRepresentation(BsonType.String)]  
        public CoffeeType Type { get; set; }

        [RegularExpression(@"^(\-?\d+(\.\d+)?)$", ErrorMessage = "Long/Lat Wrong Format")]
        public string Longitude { get; set; }
        
        [RegularExpression(@"^(\-?\d+(\.\d+)?)$", ErrorMessage = "Long/Lat Wrong Format")]
        public string Latitude { get; set; }
        
        public Weather Weather { get; set; }
    }
}
