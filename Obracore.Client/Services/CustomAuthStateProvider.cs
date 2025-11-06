using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Obracore.Client.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly AuthService _authService;

        public CustomAuthStateProvider(AuthService authService)
        {
            _authService = authService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var user = await _authService.GetCurrentUserAsync();

            if (user == null)
            {
                // Usuário não logado
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Nome ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
            };

            if (user.Perfis != null)
            {
                claims.AddRange(user.Perfis.Select(p => new Claim(ClaimTypes.Role, p)));
            }

            var identity = new ClaimsIdentity(claims, "jwt");
            var principal = new ClaimsPrincipal(identity);

            return new AuthenticationState(principal);
        }

        // Notifica mudanças no estado de autenticação
        public void NotifyUserAuthentication()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public void NotifyUserLogout()
        {
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()))));
        }

        // Logout público
        public async Task LogoutAsync()
        {
            await _authService.LogoutAsync();
            NotifyUserLogout();
        }

        // Login público
        public void NotifyLogin()
        {
            NotifyUserAuthentication();
        }
    }
}
