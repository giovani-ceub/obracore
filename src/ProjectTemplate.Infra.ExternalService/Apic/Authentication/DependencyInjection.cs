using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectTemplate.Application.Abstrations.Apic.Authentication;
using ProjectTemplate.Infra.CrossCutting.Shared.Extensions;
using ProjectTemplate.Infra.ExternalService.Apic.Authentication.Settings;
using Refit;
using System.Diagnostics.CodeAnalysis;


namespace ProjectTemplate.Infra.ExternalService.Apic.Authentication
{
    [ExcludeFromCodeCoverage]
    internal static class DependencyInjection
    {
        public static void ConfigureServices(this IServiceCollection services, IConfigurationManager configuration)
        {
            AddSettingsConfiguration(services);
            AddAuthenticationApiClientServices(services, configuration);
            AddServices(services, configuration);
        }

        private static IServiceCollection AddServices(IServiceCollection services, IConfigurationManager configuration)
        {
            services.AddSingleton<IAuthenticationService, AuthenticationService>();

            return services;
        }

        private static IServiceCollection AddSettingsConfiguration(IServiceCollection services)
        {
            services.RegisterOption<ApicAuthenticationSettings, ApicAuthenticationSettingsValidator>();

            return services;
        }

        private static void AddAuthenticationApiClientServices(IServiceCollection services, IConfigurationManager configuration)
        {
            string? authenticationApiClientUri = configuration.GetSection("Apic:Authentication:BaseUri").Value;

            if (string.IsNullOrEmpty(authenticationApiClientUri))
            {
                throw new ArgumentNullException("Apic Authentication BaseUri should not be null");
            }

            services
                .AddRefitClient<IAuthenticationApiClient>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(authenticationApiClientUri));
        }
    }
}
