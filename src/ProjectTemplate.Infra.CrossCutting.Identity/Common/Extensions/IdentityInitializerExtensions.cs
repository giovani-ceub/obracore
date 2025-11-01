using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ProjectTemplate.Infra.Data;

namespace ProjectTemplate.Infra.CrossCutting.Identity.Common.Extensions;

public static class IdentityInitializerExtensions
{
    public static async Task InitializeIdentityDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initializer = scope.ServiceProvider.GetRequiredService<IdentityDbContextInitializer>();

        await initializer.InitializeAsync();

        await initializer.SeedAsync();
    }
}
