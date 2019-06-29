using System;
using System.Net.Http;
using System.Threading.Tasks;
using Kaffee.Mappers.Weather;
using Kaffee.Models.ApiResponses.DarkSky;
using Kaffee.Models;
using Kaffee.Settings;
using Newtonsoft.Json;
using Kaffee.Models.Http;
using Kaffee.Mappers;

namespace Kaffee.Services
{
    /// <summary>
    /// Weather service using Dark Sky as the data source.
    /// </summary>
    public class DarkSkyWeatherService : IWeatherService
    {
        private readonly DarkSkySettings _darkSkySettings;
        private readonly IHttpClient _httpClient;
        private readonly IHttpResponseMapper _httpMapper;
        private readonly string _weatherUnit = "si";

        public DarkSkyWeatherService(
            DarkSkySettings _darkSkySettings,
            IHttpClient _httpClient,
            IHttpResponseMapper _httpMapper
        )
        {
            this._darkSkySettings = _darkSkySettings;
            this._httpClient = _httpClient;
            this._httpMapper = _httpMapper;
        }

        /// <summary>
        /// Get the weather for a location.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public async Task<Weather> GetWeather(float latitude, float longitude)
        {
            var uri = string.Format
                (
                    "{0}{1}/{2},{3}?units={4}",
                    _darkSkySettings.Url,
                    _darkSkySettings.Token,
                    latitude,
                    longitude,
                    _weatherUnit
                );
            Console.WriteLine(uri);
            GetWeather weather = null;
            using (_httpClient)
            {
                var response = await _httpClient.GetAsync(uri);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(string.Format("DarkSky returned with code {0}", response.StatusCode));
                }
                weather = await _httpMapper.ParseResponse<GetWeather>(response);
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
    }
}