using Microsoft.EntityFrameworkCore;
using Obracore.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Obracore.Services;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// ✅ Adiciona Controllers
builder.Services.AddControllers();

// ✅ Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Serviço de token JWT
builder.Services.AddScoped<TokenService>();

// ✅ Configurações JWT
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

// ✅ Conexão com banco de dados MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
           .EnableDetailedErrors()
           .EnableSensitiveDataLogging()
           .LogTo(Console.WriteLine, LogLevel.Information)
);

// ✅ Política CORS para permitir acesso do Blazor Client
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins, policy =>
    {
        policy.WithOrigins("https://localhost:7195", "http://localhost:5089") // Blazor Client
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials() // permite cookies/autenticação
              .WithExposedHeaders("Content-Disposition"); // útil para downloads
    });
});

// ✅ Configuração JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// ✅ Ativa CORS logo no início do pipeline
app.UseCors(MyAllowSpecificOrigins);

// ✅ Swagger (somente em desenvolvimento)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Obracore API v1");
        c.RoutePrefix = string.Empty;
    });
}

// ✅ Habilita arquivos estáticos (como /images/obras)
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
    RequestPath = "", // deixa o acesso direto, ex: /images/obras/arquivo.png
    OnPrepareResponse = ctx =>
    {
        // Define cabeçalhos HTTP para permitir exibição entre domínios
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "https://localhost:7195");
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Credentials", "true");
    }
});

// ✅ Ordem correta do pipeline
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
