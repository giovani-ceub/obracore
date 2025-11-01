using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using ProjectTemplate.Api;
using ProjectTemplate.Api.Configurations;
using ProjectTemplate.Application;
using ProjectTemplate.Infra.CrossCutting.Identity.Common.Extensions;
using ProjectTemplate.Infra.Data.Common.Extensions;
using ProjectTemplate.Infra.IoC;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApiServices();
builder.AddLogAndTelemetry();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    await app.InitializeDatabaseAsync();
    await app.InitializeIdentityDatabaseAsync();
    app.UseSwaggerSetup();
}
else
{
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapRazorPages();
app.MapFallbackToFile("index.html");
app.UseExceptionHandler(options => { });
app.Map("/", () => Results.Redirect("/swagger"));
app.MapIdentityEndpoints();

app.MapDefaultEndpoints();
app.Run();

public partial class Program { }