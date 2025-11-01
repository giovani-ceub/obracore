using FastEndpoints;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using ProjectTemplate.Infra.ExternalService.AzureKeyVault.Settings;
using System.Text.Json;

namespace ProjectTemplate.Api.Endpoints.TodoLists
{
    public class TodoListsEndpointGroup : Group
    {
        public TodoListsEndpointGroup()
        {
            string groupName = "Todo/Lists";

            Configure($"/api/todo/lists", ep =>
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
            var keyVaultOptions = TryResolve<IOptions<AzureKeyVaultSettings>>();
            if (keyVaultOptions is null)
            {
                return [""];
            }

            AzureKeyVaultSettings _keyVaultSettings = keyVaultOptions.Value;
            string serializedSettings = JsonSerializer.Serialize(_keyVaultSettings, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            return [_keyVaultSettings.Scopes];
        }
    }
}
