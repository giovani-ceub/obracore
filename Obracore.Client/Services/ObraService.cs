using System.Net.Http.Json;
using Obracore.Client.Models;
using Microsoft.AspNetCore.Components.Forms;

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

        public async Task<bool> UpdateObraWithImageAsync(ObraModel obra, IBrowserFile imagem)
        {
            try
            {
                using var content = new MultipartFormDataContent();

                // Campos da obra (form-data)
                content.Add(new StringContent(obra.Id.ToString()), "Id");
                content.Add(new StringContent(obra.Nome ?? string.Empty), "Nome");
                content.Add(new StringContent(obra.Descricao ?? string.Empty), "Descricao");
                content.Add(new StringContent(obra.StatusObra ?? string.Empty), "StatusObra");
                content.Add(new StringContent(obra.DtInicio?.ToString("o") ?? string.Empty), "DtInicio");
                content.Add(new StringContent(obra.DtFimPrevista?.ToString("o") ?? string.Empty), "DtFimPrevista");
                content.Add(new StringContent(obra.DtFim?.ToString("o") ?? string.Empty), "DtFim");
                content.Add(new StringContent(DateTime.Now.ToString("o")), "DtEdicao");

                // Arquivo de imagem
                using var stream = imagem.OpenReadStream(maxAllowedSize: 5_000_000);
                var fileContent = new StreamContent(stream);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(imagem.ContentType);
                content.Add(fileContent, "foto", imagem.Name);

                // Chama o endpoint /api/obras/{id}/with-image
                var response = await _http.PutAsync($"api/obras/{obra.Id}/with-image", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar imagem: {ex.Message}");
                return false;
            }
        }

    }
}
