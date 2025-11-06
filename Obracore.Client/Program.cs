using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using Obracore.Client;
using Obracore.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Root components
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// LocalStorage
builder.Services.AddBlazoredLocalStorage();

// Auth
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<CustomAuthStateProvider>());

// HttpClient
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7119/")
});

// Outros serviços
builder.Services.AddScoped<HttpService>();
builder.Services.AddScoped<ObraService>();

builder.Services.AddAuthorizationCore(); // ✅ importante para Blazor WASM

await builder.Build().RunAsync();
