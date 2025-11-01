using ProjectTemplate.Infra.CrossCutting.Shared.Common;
using System.ComponentModel.DataAnnotations;

namespace ProjectTemplate.Infra.ExternalService.AzureKeyVault.Settings
{
    public record AzureKeyVaultSettings : BaseOption
    {
        public AzureKeyVaultSettings()
        {
        }
        public override string OptionKey => nameof(AzureKeyVaultSettings);

        public string? KeyVaultUri { get; set; }

        [Url]
        public required string Instance { get; init; }
        public required string Domain { get; init; }
        public Guid TenantId { get; init; }
        public Guid ClientId { get; init; }
        public required string CallbackPath { get; init; }
        public required string Scopes { get; init; }
    }
}
