using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectTemplate.Infra.CrossCutting.Shared.Extensions;
using ProjectTemplate.Infra.ExternalService.AzureKeyVault.Settings;
using System.Diagnostics.CodeAnalysis;


namespace ProjectTemplate.Infra.ExternalService.AzureKeyVault
{
    [ExcludeFromCodeCoverage]
    internal static class DependencyInjection
    {
        private const string KEYVAULT_URI_SETTINGS_KEY = "AzureKeyVaultSettings:KeyVaultUri";

        public static void ConfigureServices(this IServiceCollection services, IConfigurationManager configuration)
        {
            AddKeyVaultIfConfigured(services, configuration);
            AddSettingsConfiguration(services);
        }

        public static IServiceCollection AddKeyVaultIfConfigured(this IServiceCollection services, IConfigurationManager configuration)
        {
            string? keyVaultUri = configuration.GetSection(KEYVAULT_URI_SETTINGS_KEY).Value;

            if (!string.IsNullOrWhiteSpace(keyVaultUri))
            {
                configuration.AddAzureKeyVault(
                    new Uri(keyVaultUri),
                    new DefaultAzureCredential());
            }

            return services;
        }

        public static IServiceCollection AddSettingsConfiguration(IServiceCollection services)
        {
            services.RegisterOption<AzureKeyVaultSettings, AzureKeyVaultSettingsValidator>();

            return services;
        }
    }
}
