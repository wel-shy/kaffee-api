using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Kaffee.Mappers
{
    public class HttpResponseMapper : IHttpResponseMapper
    {
        public async Task<T> ParseResponse<T>(HttpResponseMessage msg)
        {
            var json = await msg.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}