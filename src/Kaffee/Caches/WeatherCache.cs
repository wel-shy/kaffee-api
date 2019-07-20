using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Kaffee.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace Kaffee.Caches
{
    public class WeatherCache : IWeatherCache
    {
        private readonly IDistributedCache _distributedCache;

        public WeatherCache(IDistributedCache _distributedCache)
        {
            this._distributedCache = _distributedCache;
        }

        /// <summary>
        /// Get a forecast from the cache.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public async Task<Weather> GetForecast(float latitude, float longitude)
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

        /// <summary>
        /// Store weather in distributed cache.
        /// </summary>
        /// <param name="weatherForecast"></param>
        /// <returns></returns>
        public async Task CacheForecasts(Weather[] weatherForecast)
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

        /// <summary>
        /// Get the key for the cache.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="time"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Convert from byte array to weather when
        /// fetching from the cache.
        /// </summary>
        /// <param name="arrBytes"></param>
        /// <returns></returns>
        private Weather ByteArrayToWeather(byte[] arrBytes)
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