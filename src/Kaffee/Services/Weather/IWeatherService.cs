using System.Threading.Tasks;
using Kaffee.Models;
namespace Kaffee.Services
{
    public interface IWeatherService 
    {
        Task<Weather> GetWeather(float latitude, float longitude);
    }
}