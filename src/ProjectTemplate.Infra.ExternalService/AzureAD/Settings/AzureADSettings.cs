using ProjectTemplate.Infra.CrossCutting.Shared.Common;
using System.ComponentModel.DataAnnotations;

namespace ProjectTemplate.Infra.ExternalService.AzureAD.Settings
{
    public record AzureADSettings : BaseOption
    {
        public AzureADSettings()
        {
        }
        public override string OptionKey => "AzureAd";

        [Url]
        public required string Instance { get; init; }
        public required string Domain { get; init; }
        public Guid TenantId { get; init; }
        public Guid ClientId { get; init; }
        public required string CallbackPath { get; init; }
        public required string Scopes { get; init; }
    }
}
