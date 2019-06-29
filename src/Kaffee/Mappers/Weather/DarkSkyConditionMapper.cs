using System.Collections.Generic;
using Kaffee.Models;

namespace Kaffee.Mappers.Weather
{
    public class DarkSkyConditionMapper
    {
        /// <summary>
        /// Get the weather condition from a string. Returns WeatherCondition.Undefined
        /// if string is not matched.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static WeatherCondition GetCondition(string condition)
        {
            return new Dictionary<string, WeatherCondition> 
            {
                { "clear-day", WeatherCondition.ClearDay },
                { "clear-night", WeatherCondition.ClearNight },
                { "rain", WeatherCondition.Rain },
                { "snow", WeatherCondition.Snow },
                { "sleet", WeatherCondition.Sleet },
                { "wind", WeatherCondition.Wind },
                { "fog", WeatherCondition.Fog },
                { "cloudy", WeatherCondition.Cloudy },
                { "partly-cloudy-day", WeatherCondition.PartlyCloudyDay },
                { "partly-cloudy-night", WeatherCondition.PartlyCloudyNight },
                { "hail", WeatherCondition.Hail },
                { "thunderstorm", WeatherCondition.Thunderstorm },
                { "tornado", WeatherCondition.Tornado }
            }.GetValueOrDefault(condition, WeatherCondition.Undefined);
        }
    }
}