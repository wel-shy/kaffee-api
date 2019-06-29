using System;
using System.Net.Http;
using System.Threading.Tasks;
using Kaffee.Mappers.Weather;
using Kaffee.Models.ApiResponses.DarkSky;
using Kaffee.Models.Weather;
using Kaffee.Settings;
using Newtonsoft.Json;

namespace Kaffee.Services
{
    public class DarkSkyWeatherService : IWeatherService
    {
        private readonly DarkSkySettings _darkSkySettings;
        private readonly string _weatherUnit = "si";

        public DarkSkyWeatherService(DarkSkySettings _darkSkySettings)
        {
            this._darkSkySettings = _darkSkySettings;
        }

        public async Task<Weather> GetWeather(float latitude, float longitude)
        {
            var uri = string.Format
                (
                    "{0}{1}/{2},{3}?unit={4}",
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