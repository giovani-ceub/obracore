using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using Obracore.Client;
using Obracore.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Root components
builder.RootComponents.Add<App>("#app"); // Lógica de roteamento e layout (App.razor)
builder.RootComponents.Add<HeadOutlet>("head::after"); // Metatags <head> do HTML

// LocalStorage
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<AuthService>(); // Serviço de autenticação
builder.Services.AddScoped<CustomAuthStateProvider>(); // Permite o blazor renderizar apenas os serviços que denpenda de autenticação utilizando <AuthorizeView>
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<CustomAuthStateProvider>());

// Define a URL base que aponta para API (HttpClient)
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7119/")
});

builder.Services.AddScoped<HttpService>(); // Centraliza a lógica de fazer requisições HTTP. Lê e desserializa a resposta JSON em um único lugar
builder.Services.AddScoped<ObraService>();
builder.Services.AddScoped<ToastService>(); // Utilização de mensagens dinâmicas utilizando o ToastMessage


builder.Services.AddAuthorizationCore(); // Avaliar políticas de autorização para Blazor WASM

await builder.Build().RunAsync(); // Aguarda até que essa Task seja concluída
