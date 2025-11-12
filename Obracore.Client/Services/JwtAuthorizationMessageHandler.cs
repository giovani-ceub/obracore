using System.Net.Http.Headers;
using Obracore.Client.Services;
using Microsoft.Extensions.DependencyInjection;

public class JwtAuthorizationMessageHandler : DelegatingHandler
{
    private readonly IServiceProvider _serviceProvider;

    public JwtAuthorizationMessageHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var authService = _serviceProvider.GetRequiredService<AuthService>();
        
        // 1. Obtém o token 
        var token = await authService.GetTokenAsync();

        if (!string.IsNullOrEmpty(token))
        {
            // 2. Adiciona o token
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        
        // 3. Permite que a requisição siga
        return await base.SendAsync(request, cancellationToken);
    }
}