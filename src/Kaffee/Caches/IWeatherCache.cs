using System.Threading.Tasks;
using Kaffee.Models;

namespace Kaffee.Caches
{
    public interface IWeatherCache
    {
        Task CacheForecasts(Weather[] forecast);
        Task<Weather> GetForecast(float latitude, float longitude);
    }
}