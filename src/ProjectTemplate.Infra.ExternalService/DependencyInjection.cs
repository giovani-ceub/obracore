using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;


namespace ProjectTemplate.Infra.ExternalService
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjection
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfigurationManager configuration)
        {
            AzureKeyVault.DependencyInjection.ConfigureServices(services, configuration);
            AzureAD.DependencyInjection.ConfigureServices(services);
            Apic.Authentication.DependencyInjection.ConfigureServices(services, configuration);
            Caching.DependencyInjection.ConfigureServices(services, configuration);

            return services;
        }
    }
}
