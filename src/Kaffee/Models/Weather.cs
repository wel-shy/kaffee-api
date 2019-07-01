using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kaffee.Models
{
    [Serializable]
    public class Weather
    {
        [BsonRepresentation(BsonType.String)]
        [JsonConverter(typeof(StringEnumConverter))]
        public WeatherCondition Condition { get; set; }
        public float Temperature { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public DateTime Date { get; set; }
        public float PrecipProbability { get; set; }
        public float Humidity { get; set; }
        public float Pressure { get; set; }
        public float WindSpeed { get; set; }
        public float CloudCover { get; set; }
        public int UVIndex { get; set; }

        public byte[] ToByteArray()
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, this);
                return ms.ToArray();
            }
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}