using ProjectTemplate.Infra.CrossCutting.Shared.Common;

namespace ProjectTemplate.Infra.ExternalService.Apic.Authentication.Settings
{
    public record ApicAuthenticationSettings : BaseOption
    {
        public ApicAuthenticationSettings()
        {
        }

        public override string OptionKey => "Apic:Authentication";
        public required string GrantType { get; set; } = "client_credentials";
        public required string Scope { get; set; }
        public required string ClientId { get; set; }
        public required string ClientSecret { get; set; }
        public required string BaseUri { get; set; }
    }
}
