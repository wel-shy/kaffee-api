using System;
using Kaffee.Models.ApiResponses.DarkSky;

namespace Kaffee.Models.Weather
{
    public class Weather
    {
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
    }
}