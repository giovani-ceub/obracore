using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectTemplate.Infra.IoC;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfigurationManager configuration)
    {
        Data.DependencyInjection.ConfigureServices(services, configuration);
        CrossCutting.Identity.DependencyInjection.ConfigureServices(services, configuration);
        ExternalService.DependencyInjection.ConfigureServices(services, configuration);

        RegisterServices(services, configuration);

        return services;
    }

    private static void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(TimeProvider.System);
    }
}
