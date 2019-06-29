using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kaffee.Models.Http
{
    public interface IHttpClient : IDisposable
    {
        Task<HttpResponseMessage> GetAsync(string uri);
    }
}