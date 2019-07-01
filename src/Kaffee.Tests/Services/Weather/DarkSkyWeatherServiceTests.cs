using System.Net;
using Xunit;
using Kaffee.Services;
using Kaffee.Models.ApiResponses.DarkSky;
using System.IO;
using System;
using Kaffee.Settings;
using System.Threading.Tasks;
using Kaffee.Models.Http;
using Moq;
using System.Net.Http;
using Kaffee.Mappers;
using Newtonsoft.Json;
using Kaffee.Caches;

namespace Kaffee.Tests.Services.Weather
{
    public class DarkSkyWeatherServiceTests
    {
        private Mock<IHttpClient> _mockHttpClient;
        private Mock<IHttpResponseMapper> _mockHttpResponseMapper;
        private Mock<IWeatherCache> _mockCache;

        public DarkSkyWeatherServiceTests()
        {
            _mockHttpClient = new Mock<IHttpClient>();
            _mockHttpResponseMapper = new Mock<IHttpResponseMapper>();
            _mockCache = new Mock<IWeatherCache>();
        }
        private DarkSkySettings settings = new DarkSkySettings
        {
            Url = "https://api.darksky.net/forecast/",
            Token = "abc"
        };

        [Fact]
        public async Task GetWeather()
        {
            String sentUrl = null;
            String expectedUrl = string.Format
            (
                "{0}{1}/{2},{3}?units={4}",
                settings.Url,
                settings.Token,
                1,
                1,
                "si"
            );

            var serialisedWeather = File.ReadAllText(@"../../../Resources/weather.json");
            GetWeather weather = JsonConvert.DeserializeObject<GetWeather>(serialisedWeather);
            var expectedFirst = weather.Hourly.Data[2];

            _mockHttpClient.Setup(cli => cli.GetAsync(It.IsAny<string>()))
                .Callback<string>(url => sentUrl = url)
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK });
            _mockHttpResponseMapper.Setup(mpr => mpr.ParseResponse<GetWeather>(It.IsAny<HttpResponseMessage>()))
                .ReturnsAsync(weather);
            
            var darkSkyWeatherService = new DarkSkyWeatherService
                (
                    settings, 
                    _mockHttpClient.Object,
                    _mockHttpResponseMapper.Object,
                    _mockCache.Object
                );
            var fetchedWeather = await darkSkyWeatherService.GetWeather(1, 1);

            Assert.Equal(expectedFirst.Time, fetchedWeather.Date);
            Assert.Equal(expectedUrl, sentUrl);
        }
    }
}