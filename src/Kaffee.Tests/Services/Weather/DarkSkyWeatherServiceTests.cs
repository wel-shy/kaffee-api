using Xunit;
using Kaffee.Services;
using Kaffee.Models.ApiResponses.DarkSky;
using System.IO;
using System;
using Kaffee.Settings;
using System.Threading.Tasks;

namespace Kaffee.Tests.Services.Weather
{
    public class DarkSkyWeatherServiceTests
    {
        private DarkSkySettings settings = new DarkSkySettings
        {
            Url = "https://api.darksky.net/forecast/",
            Token = "abc"
        };
        [Fact]
        public void DeserialiseWeatherTest()
        {
            var darkSkyWeatherService = new DarkSkyWeatherService(settings);
            var serialisedWeather = File.ReadAllText(@"../../../Resources/weather.json");
            GetWeather weather = darkSkyWeatherService.ConvertResponse(serialisedWeather);

            var expectedTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(1561806000);
            var firstHour = weather.Hourly.Data[0];

            Assert.Equal(expectedTime, firstHour.Time);
            Assert.Equal("Mostly Cloudy", firstHour.Summary);

            var delta = Math.Abs(22.64 - firstHour.Temperature);
            Assert.True(0.001 > delta);
        }

        [Fact]
        public async Task GetWeather()
        {
            //Given
            var darkSkyWeatherService = new DarkSkyWeatherService(settings);
            var weather = await darkSkyWeatherService.GetWeather(1, 1);

            Assert.True(weather.Date > DateTime.Today);
        }
    }
}