using System.Net.Http;
using System.Threading.Tasks;

namespace Kaffee.Mappers
{
    public interface IHttpResponseMapper 
    {
        Task<T> ParseResponse<T>(HttpResponseMessage msg);
    }
}