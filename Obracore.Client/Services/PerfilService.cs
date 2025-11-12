using Obracore.Client.Models;
using System.Net.Http.Json;

namespace Obracore.Client.Services
{
    public class PerfilService
    {
        private readonly HttpClient _http;
        private readonly ToastService _toastService; // Assumindo que você tem um ToastService

        public PerfilService(HttpClient http, ToastService toastService)
        {
            _http = http;
            _toastService = toastService;
        }

        // GET: /api/perfis - Lista todos os perfis
        public async Task<List<PerfilModel>?> GetPerfisAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<List<PerfilModel>>("api/perfis");
            }
            catch (HttpRequestException ex)
            {
                _toastService.ShowError($"Erro ao carregar perfis: {ex.Message}");
                return null;
            }
        }

        // GET: /api/perfis/{id} - Obtém um perfil específico
        public async Task<PerfilModel?> GetPerfilByIdAsync(int id)
        {
            try
            {
                return await _http.GetFromJsonAsync<PerfilModel>($"api/perfis/{id}");
            }
            catch (HttpRequestException ex)
            {
                _toastService.ShowError($"Erro ao carregar perfil: {ex.Message}");
                return null;
            }
        }

        // POST: /api/perfis - Cria um novo perfil
        public async Task<bool> CreatePerfilAsync(PerfilModel perfil)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/perfis", perfil);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (HttpRequestException ex)
            {
                _toastService.ShowError($"Falha ao criar perfil: {ex.Message}");
                return false;
            }
        }

        // PUT: /api/perfis/{id} - Atualiza um perfil
        public async Task<bool> UpdatePerfilAsync(PerfilModel perfil)
        {
            try
            {
                var response = await _http.PutAsJsonAsync($"api/perfis/{perfil.Id}", perfil);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (HttpRequestException ex)
            {
                _toastService.ShowError($"Falha ao atualizar perfil: {ex.Message}");
                return false;
            }
        }

        // DELETE: /api/perfis/{id} - Exclui um perfil
        public async Task<bool> DeletePerfilAsync(int id)
        {
            try
            {
                var response = await _http.DeleteAsync($"api/perfis/{id}");
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (HttpRequestException ex)
            {
                _toastService.ShowError($"Falha ao excluir perfil: {ex.Message}");
                return false;
            }
        }
    }
}