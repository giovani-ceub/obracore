using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Obracore.Client.Models;

namespace Obracore.Client.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;
        private readonly IJSRuntime _js;

        public AuthService(HttpClient http, IJSRuntime js)
        {
            _http = http;
            _js = js;
        }

        // Realiza login e salva o token no localStorage
        public async Task<bool> LoginAsync(LoginRequest login)
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", login);
            if (!response.IsSuccessStatusCode) return false;

            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (loginResponse == null || string.IsNullOrEmpty(loginResponse.Token))
                return false;

            // Salvar token em localStorage
            await _js.InvokeVoidAsync("localStorage.setItem", "authToken", loginResponse.Token);

            return true;
        }

        // Remove token do localStorage
        public async Task LogoutAsync()
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", "authToken");
        }

        // Recupera token do localStorage
        public async Task<string?> GetTokenAsync()
        {
            return await _js.InvokeAsync<string>("localStorage.getItem", "authToken");
        }

        // Verifica se o usuário está logado
        public async Task<bool> IsLoggedInAsync()
        {
            var token = await GetTokenAsync();
            return !string.IsNullOrEmpty(token);
        }

        // Busca o usuário atual usando o token
        public async Task<UsuarioClaims?> GetCurrentUserAsync()
        {
            var token = await GetTokenAsync();
            if (string.IsNullOrEmpty(token)) return null;

            var request = new HttpRequestMessage(HttpMethod.Get, "api/auth/me");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _http.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<UsuarioClaims>();
        }
    }
}
