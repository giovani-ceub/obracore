using FastEndpoints;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using ProjectTemplate.Infra.ExternalService.AzureAD.Settings;
using System.Text.Json;

namespace ProjectTemplate.Api.Endpoints.WeatherForecasts
{
    public class WeatherForecastsEndpointGroup : Group
    {
        public WeatherForecastsEndpointGroup()
        {
            string groupName = "WeatherForecasts";

            Configure($"/api/weatherForecasts", ep =>
            {
                ep.Description(x => x
                  .RequireAuthorization()
                  .RequireScope(GetRequiredScopes())
                  .Produces(TypedResults.Unauthorized().StatusCode)
                  .WithGroupName(groupName)
                  .WithTags(groupName)
                  .WithOpenApi());
            });
        }

        private string[] GetRequiredScopes()
        {
            var keyVaultOptions = TryResolve<IOptions<AzureADSettings>>();
            if (keyVaultOptions is null)
            {
                return [""];
            }

            AzureADSettings _keyVaultSettings = keyVaultOptions.Value;
            string serializedSettings = JsonSerializer.Serialize(_keyVaultSettings, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            return [_keyVaultSettings.Scopes];
        }
    }
}
