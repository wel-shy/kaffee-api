using System;
using System.Net.Http;
using System.Threading.Tasks;
using Kaffee.Mappers.Weather;
using Kaffee.Models.ApiResponses.DarkSky;
using Kaffee.Models;
using Kaffee.Settings;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace Kaffee.Services
{
    /// <summary>
    /// Weather service using Dark Sky as the data source.
    /// </summary>
    public class DarkSkyWeatherService : IWeatherService
    {
        private readonly DarkSkySettings _darkSkySettings;
        private readonly ILogger<DarkSkyWeatherService> _logger;
        private readonly string _weatherUnit = "si";

        public DarkSkyWeatherService
        (
            DarkSkySettings _darkSkySettings,
            ILogger<DarkSkyWeatherService> _logger
        )
        {
            this._darkSkySettings = _darkSkySettings;
            this._logger = _logger;
        }

        /// <summary>
        /// Get the weather for a location.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public async Task<Weather> GetWeather(float latitude, float longitude)
        {
             _logger.LogInformation(
                "DarkSkyWeatherService - Getting weather for {0},{1}",
                latitude, 
                longitude
            );
            var uri = string.Format
                (
                    "{0}{1}/{2},{3}?units={4}",
                    _darkSkySettings.Url,
                    _darkSkySettings.Token,
                    latitude,
                    longitude,
                    _weatherUnit
                );
            GetWeather weather = null;
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(uri);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(string.Format("DarkSky returned with code {0}", response.StatusCode));
                }
                var json = await response.Content.ReadAsStringAsync();
                weather = JsonConvert.DeserializeObject<GetWeather>(json);
            }

            var now = weather.Hourly.Data[2];
            return new Weather
            {
                Condition = DarkSkyConditionMapper.GetCondition(now.Icon),
                Temperature = now.Temperature,
                Latitude = latitude,
                Longitude = longitude,
                Date = now.Time,
                PrecipProbability = now.PrecipProbability,
                Humidity = now.Humidity,
                Pressure = now.Pressure,
                WindSpeed = now.WindSpeed,
                CloudCover = now.CloudCover,
                UVIndex = now.UVIndex
            };
        }

        public GetWeather ConvertResponse(string serialisedWeather)
        {
            return JsonConvert.DeserializeObject<GetWeather>(serialisedWeather);
        }
    }
}