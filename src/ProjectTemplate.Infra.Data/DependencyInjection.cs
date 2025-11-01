using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectTemplate.Application.Common.Interfaces;
using ProjectTemplate.Infra.Data.Interceptors;

namespace ProjectTemplate.Infra.Data;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        AddDatabaseConfiguration(services, configuration);
        RegisterHealthChecks(services, configuration);

        return services;
    }

    private static void AddDatabaseConfiguration(IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = GetConnectionString(configuration);

        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
         {
             options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
             options.UseSqlServer(connectionString);
         });
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<ApplicationDbContextInitializer>();
    }

    private static IServiceCollection RegisterHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = GetConnectionString(configuration);
        services.AddHealthChecks()
            .AddSqlServer(connectionString, name: "sql-server");

        return services;
    }
    private static string GetConnectionString(IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("projecttemplate-db");
        Guard.Against.Null(connectionString, message: "Connection string 'projecttemplate-db' not found.");
        return connectionString;
    }
}
