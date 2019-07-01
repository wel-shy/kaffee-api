using System.Diagnostics;
using System;
using System.Threading.Tasks;
using Kaffee.Mappers.Weather;
using Kaffee.Models.ApiResponses.DarkSky;
using Kaffee.Models;
using Kaffee.Settings;
using Kaffee.Models.Http;
using Kaffee.Mappers;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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
        private readonly IDistributedCache _distributedCache;
        private readonly string _weatherUnit = "si";

        public DarkSkyWeatherService(
            DarkSkySettings _darkSkySettings,
            IHttpClient _httpClient,
            IHttpResponseMapper _httpMapper,
            IDistributedCache _distributedCache
        )
        {
            this._darkSkySettings = _darkSkySettings;
            this._httpClient = _httpClient;
            this._httpMapper = _httpMapper;
            this._distributedCache = _distributedCache;
        }

        /// <summary>
        /// Get the weather for a location.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public async Task<Weather> GetWeather(float latitude, float longitude)
        {
            var cachedWeather = await FetchFromCache(latitude, longitude);
            if (cachedWeather != null) 
            {
                Debug.WriteLine("Fetched from cache: " + cachedWeather.ToString());
                return cachedWeather;
            }

            Debug.WriteLine("Fall back to DarkSky api");

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

            await CacheWeather(weatherCollection.ToArray());
            return weatherCollection[2];
        }

        /// <summary>
        /// Store weather in distributed cache.
        /// </summary>
        /// <param name="weatherForecast"></param>
        /// <returns></returns>
        public async Task CacheWeather(Weather[] weatherForecast)
        {
            foreach (var weatherReading in weatherForecast)
            {
                // Generate key
                var key = GetCacheKey(
                    weatherReading.Latitude, 
                    weatherReading.Longitude, 
                    weatherReading.Date
                );

                await _distributedCache.SetAsync(key, weatherReading.ToByteArray());                
            }
        }

        public async Task<Weather> FetchFromCache(float latitude, float longitude)
        {
            var pastHour = DateTime.Today.AddHours(DateTime.Now.Hour);
            var key = GetCacheKey(latitude, longitude, pastHour);

            var weatherBytes = await _distributedCache.GetAsync(key);
            if (weatherBytes == null || weatherBytes.Length == 0)
            {
                return null;
            }
            return ByteArrayToWeather(weatherBytes);
        }

        private string GetCacheKey(float latitude, float longitude, DateTime time)
        {
            var unixTime = (time.Date - new DateTime(1970, 1, 1,0,0,0,DateTimeKind.Utc)).TotalSeconds;
            return string.Format
            (
                "{0}_{1}_{2}",
                Math.Round((decimal) latitude, 2),
                Math.Round((decimal) longitude, 2),
                unixTime
            ); 
        }

        public Weather ByteArrayToWeather(byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);
                return (Weather)obj;
            }
        }
    }
}