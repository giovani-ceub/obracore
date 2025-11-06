using System.Net.Http.Json;

namespace Obracore.Client.Services
{
    public class HttpService
    {
        private readonly HttpClient _http;

        public HttpService(HttpClient http)
        {
            _http = http;
        }

        public async Task<T?> GetAsync<T>(string url)
        {
            return await _http.GetFromJsonAsync<T>(url);
        }
    }
}
