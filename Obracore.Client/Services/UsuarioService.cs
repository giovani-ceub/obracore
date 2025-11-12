using Obracore.Client.Models;
using System.Net.Http.Json;

namespace Obracore.Client.Services
{
    // Este serviço usa o HttpClient que está configurado com BaseAddress e JwtAuthorizationMessageHandler
    public class UsuarioService
    {
        private readonly HttpClient _http;
        private readonly ToastService _toastService;

        public UsuarioService(HttpClient http, ToastService toastService)
        {
            // O 'http' aqui é o cliente nomeado "API" configurado no Program.cs.
            _http = http;
            _toastService = toastService;
        }

        // GET: /api/usuarios - Lista todos os usuários (Usa o DTO de listagem/tabela)
        public async Task<List<UsuarioModel>?> GetUsuariosAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<List<UsuarioModel>>("api/usuarios");
            }
            catch (HttpRequestException ex)
            {
                _toastService.ShowError($"Erro ao carregar usuários: {ex.Message}");
                return null;
            }
        }

        // GET: /api/usuarios/{id} - Obtém um usuário específico para EDIÇÃO (Retorna DTO de Edição)
        public async Task<UsuarioEditDto?> GetUsuarioByIdAsync(int id)
        {
            try
            {
                // Usa UsuarioEditDto, que é o tipo esperado pelo UsuariosEdit.razor
                var response = await _http.GetAsync($"api/usuarios/{id}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<UsuarioEditDto>();
            }
            catch (HttpRequestException ex)
            {
                _toastService.ShowError($"Erro ao carregar usuário: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                 // Erro de desserialização ou outro erro inesperado
                _toastService.ShowError($"Erro inesperado ao buscar usuário: {ex.Message}");
                return null;
            }
        }

        // POST: /api/usuarios - Cria um novo usuário (Usa DTO de Criação)
        public async Task<bool> CreateUsuarioAsync(UsuarioCreateDto dto)
        {
            try
            {
                // Usa UsuarioCreateDto, que é o tipo esperado pelo UsuariosCreate.razor
                var response = await _http.PostAsJsonAsync("api/usuarios", dto);
                
                if (!response.IsSuccessStatusCode)
                {
                    // Lida com erros da API (como e-mail duplicado, validação)
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _toastService.ShowError($"Falha ao criar usuário: {response.ReasonPhrase}. Detalhe: {errorContent}");
                    return false;
                }

                _toastService.ShowSuccess("Usuário criado com sucesso!");
                return true;
            }
            catch (Exception ex)
            {
                _toastService.ShowError($"Ocorreu um erro inesperado: {ex.Message}");
                return false;
            }
        }

        // PUT: /api/usuarios/{id} - Atualiza um usuário (Usa DTO de Atualização)
        public async Task<bool> UpdateUsuarioAsync(int id, UsuarioUpdateDto dto)
        {
            try
            {
                // Usa UsuarioUpdateDto, que inclui a NovaSenha opcional, esperado pelo UsuariosEdit.razor
                var response = await _http.PutAsJsonAsync($"api/usuarios/{id}", dto);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _toastService.ShowError($"Falha ao atualizar usuário: {response.ReasonPhrase}. Detalhe: {errorContent}");
                    return false;
                }
                
                _toastService.ShowSuccess("Usuário atualizado com sucesso!");
                return true;
            }
            catch (Exception ex)
            {
                _toastService.ShowError($"Ocorreu um erro inesperado: {ex.Message}");
                return false;
            }
        }

        // DELETE: /api/usuarios/{id} - Exclui um usuário
        public async Task<bool> DeleteUsuarioAsync(int id)
        {
            try
            {
                var response = await _http.DeleteAsync($"api/usuarios/{id}");
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _toastService.ShowError($"Falha ao excluir usuário: {response.ReasonPhrase}. Detalhe: {errorContent}");
                    return false;
                }

                _toastService.ShowSuccess("Usuário excluído com sucesso!");
                return true;
            }
            catch (Exception ex)
            {
                _toastService.ShowError($"Ocorreu um erro inesperado ao excluir: {ex.Message}");
                return false;
            }
        }
    }
}