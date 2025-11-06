using System.Net.Http.Json;
using Obracore.Client.Models;

namespace Obracore.Client.Services
{
    public class ObraService
    {
        private readonly HttpClient _http;

        public ObraService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<ObraModel>?> GetObrasAsync()
        {
            return await _http.GetFromJsonAsync<List<ObraModel>>("api/obras");
        }

        public async Task<ObraModel?> GetObraAsync(int id)
        {
            return await _http.GetFromJsonAsync<ObraModel>($"api/obras/{id}");
        }

        public async Task<bool> AddObraAsync(ObraModel obra)
        {
            var response = await _http.PostAsJsonAsync("api/obras", obra);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateObraAsync(ObraModel obra)
        {
            var response = await _http.PutAsJsonAsync($"api/obras/{obra.Id}", obra);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteObraAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/obras/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
