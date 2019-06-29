using System;
using Kaffee.Converters;
using Newtonsoft.Json;

namespace Kaffee.Models.ApiResponses.DarkSky
{
    public class GetWeather 
    {
        public HourlyWeather Hourly;
    }

    public class HourlyWeather
    {
        public HourlyWeatherItem[] Data;
    }

    public class HourlyWeatherItem 
    {
        [JsonConverter(typeof(UnixDateConverter))]
        public DateTime Time;
        public string Summary;
        public string Icon;
        public float PrecipProbability;
        public float Temperature;
        public float Humidity;
        public float Pressure;
        public float WindSpeed;
        public float CloudCover;
        public int UVIndex;
    }
}