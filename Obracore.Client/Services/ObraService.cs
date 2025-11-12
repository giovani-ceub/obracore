using System.Net.Http.Json;
using Obracore.Client.Models;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http;

namespace Obracore.Client.Services
{
    public class ObraService
    {
        private readonly HttpClient _http;

        // Assumimos que no Program.cs, você está injetando o cliente nomeado "API"
        // Ex: builder.Services.AddScoped<ObraService>(sp => 
        //         new ObraService(sp.GetRequiredService<IHttpClientFactory>().CreateClient("API")));
        public ObraService(HttpClient http)
        {
            _http = http;
        }

        // --- MÉTODOS EXISTENTES ---

        public async Task<List<ObraModel>?> GetObrasAsync()
        {
            return await _http.GetFromJsonAsync<List<ObraModel>>("api/obras");
        }

        // Obtém apenas as obras do usuário logado
        public async Task<List<ObraModel>?> GetObrasDoUsuarioAsync()
        {
            // Chama o endpoint que filtra pelo token JWT
            return await _http.GetFromJsonAsync<List<ObraModel>>("api/obras/me");
        }

        public async Task<ObraModel?> GetObraAsync(int id)
        {
            return await _http.GetFromJsonAsync<ObraModel>($"api/obras/{id}");
        }

        // 🚨 MÉTODO SIMPLIFICADO: Para o caso de POST JSON SEM IMAGEM.
        public async Task<bool> AddObraAsync(ObraModel obra)
        {
            // Nota: Este método é mais adequado se você não enviar o MultipartFormDataContent.
            var response = await _http.PostAsJsonAsync("api/obras", obra);
            return response.IsSuccessStatusCode;
        }
        public async Task<HttpResponseMessage> PostObraWithFormDataAsync(MultipartFormDataContent content)
        {
            // O HttpClient (_http) já possui a BaseAddress e o Handler de Token anexados (via Program.cs)
            return await _http.PostAsync("api/obras", content);
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

        // --- MÉTODO COM IMAGEM para lidar com PUT ---

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

                // Arquivo de imagem com limite de 5MB
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