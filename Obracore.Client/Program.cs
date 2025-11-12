using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Http; // Necessário para AddHttpClient e IHttpClientFactory
using Microsoft.Extensions.DependencyInjection; // Necessário para IHttpClientFactory
using Microsoft.JSInterop; // Necessário para injetar IJSRuntime
using Blazored.LocalStorage;
using Obracore.Client;
using Obracore.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Root components
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// LocalStorage
builder.Services.AddBlazoredLocalStorage();

// Serviços de Autorização
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<CustomAuthStateProvider>(); 
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<CustomAuthStateProvider>());


// --- Configuração do HTTP Client com Token Interceptor ---

// Registra o Handler (interceptor)
builder.Services.AddScoped<JwtAuthorizationMessageHandler>();

// Configura o HttpClient que será usado por TODOS os serviços de API
builder.Services.AddHttpClient("API", client => 
{
    // Define a URL base que aponta para API
    client.BaseAddress = new Uri("https://localhost:7119/");
})
// ANEXA o Handler para que ele seja executado antes de cada requisição
.AddHttpMessageHandler<JwtAuthorizationMessageHandler>(); 


// --- Registro dos Serviços de API (Usando IHttpClientFactory) ---

// 💡 AuthService: Precisa do IHttpClientFactory para o _http e do IJSRuntime
builder.Services.AddScoped<AuthService>(sp => 
    new AuthService(
        sp.GetRequiredService<IHttpClientFactory>(),
        sp.GetRequiredService<IJSRuntime>()
    ));

// 💡 HttpService: Precisa do IHttpClientFactory
builder.Services.AddScoped<HttpService>(sp => 
    new HttpService(sp.GetRequiredService<IHttpClientFactory>().CreateClient("API")));

// 💡 ObraService: Precisa do IHttpClientFactory
builder.Services.AddScoped<ObraService>(sp =>
    new ObraService(sp.GetRequiredService<IHttpClientFactory>().CreateClient("API")));

builder.Services.AddScoped<PerfilService>(sp =>
    new PerfilService(
        sp.GetRequiredService<IHttpClientFactory>().CreateClient("API"),
        sp.GetRequiredService<ToastService>()
));

builder.Services.AddScoped<UsuarioService>(sp =>
    new UsuarioService(
        sp.GetRequiredService<IHttpClientFactory>().CreateClient("API"),
        sp.GetRequiredService<ToastService>()
));


builder.Services.AddScoped<ToastService>(); 

await builder.Build().RunAsync();