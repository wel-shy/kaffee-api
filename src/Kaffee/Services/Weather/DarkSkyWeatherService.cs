using System.Diagnostics;
using System;
using System.Threading.Tasks;
using Kaffee.Mappers.Weather;
using Kaffee.Models.ApiResponses.DarkSky;
using Kaffee.Models;
using Kaffee.Settings;
using Kaffee.Models.Http;
using Kaffee.Mappers;
using System.Collections.Generic;
using Kaffee.Caches;
using Microsoft.Extensions.Logging;

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
        private readonly IWeatherCache _weatherCache;
        private readonly ILogger<DarkSkyWeatherService> _logger;
        private readonly string _weatherUnit = "si";

        /// <summary>
        /// Get an instance of the dark sky weather service.
        /// </summary>
        /// <param name="_darkSkySettings"></param>
        /// <param name="_httpClient"></param>
        /// <param name="_httpMapper"></param>
        /// <param name="_weatherCache"></param>
        /// <param name="_logger"></param>
        public DarkSkyWeatherService(
            DarkSkySettings _darkSkySettings,
            IHttpClient _httpClient,
            IHttpResponseMapper _httpMapper,
            IWeatherCache _weatherCache,
            ILogger<DarkSkyWeatherService> _logger
        )
        {
            this._darkSkySettings = _darkSkySettings;
            this._httpClient = _httpClient;
            this._httpMapper = _httpMapper;
            this._weatherCache = _weatherCache;
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
            var cachedWeather = await _weatherCache.GetForecast(latitude, longitude);
            if (cachedWeather != null) 
            {
                return cachedWeather;
            }

            _logger.LogInformation("Fall back to DarkSky api");

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
            using (_httpClient)
            {
                var response = await _httpClient.GetAsync(uri);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(string.Format("DarkSky returned with code {0}", response.StatusCode));
                }
                weather = await _httpMapper.ParseResponse<GetWeather>(response);
            }

            var weatherCollection = new List<Weather>();
            for (int i = 0; i < weather.Hourly.Data.Length; i++)
            {
                var now = weather.Hourly.Data[i];
                weatherCollection.Add( new Weather
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
                    }
                );
            }

            try 
            {
                await _weatherCache.CacheForecasts(weatherCollection.ToArray());
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not cache forecast");
            }
            
            return weatherCollection[2];
        }
    }
}