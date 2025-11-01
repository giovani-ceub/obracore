using Microsoft.AspNetCore.Mvc;
using ProjectTemplate.Api.Configurations;
using ProjectTemplate.Api.Services;
using ProjectTemplate.Application.Common.Interfaces;

namespace ProjectTemplate.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddScoped<IUser, CurrentUser>();

        services.AddHttpContextAccessor();

        services.AddExceptionHandler<CustomExceptionHandler>();

        services.AddRazorPages();

        // Customize default API behavior
        services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        services.AddEndpointsApiExplorer();
        services.AddSwaggerConfiguration();

        return services;
    }

    public static WebApplicationBuilder AddLogAndTelemetry(this WebApplicationBuilder builder)
    {
        builder.WebHost.UseSentry();
        builder.Logging.AddSentry();
        builder.Services.AddApplicationInsightsTelemetry();

        return builder;
    }
}
