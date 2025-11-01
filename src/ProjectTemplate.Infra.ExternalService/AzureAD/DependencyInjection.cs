using Microsoft.Extensions.DependencyInjection;
using ProjectTemplate.Infra.CrossCutting.Shared.Extensions;
using ProjectTemplate.Infra.ExternalService.AzureAD.Settings;
using System.Diagnostics.CodeAnalysis;

namespace ProjectTemplate.Infra.ExternalService.AzureAD
{
    [ExcludeFromCodeCoverage]
    internal static class DependencyInjection
    {
        public static void ConfigureServices(this IServiceCollection services)
        {
            AddSettingsConfiguration(services);
        }

        public static IServiceCollection AddSettingsConfiguration(IServiceCollection services)
        {
            services.RegisterOption<AzureADSettings, AzureADSettingsValidator>();

            return services;
        }
    }
}
