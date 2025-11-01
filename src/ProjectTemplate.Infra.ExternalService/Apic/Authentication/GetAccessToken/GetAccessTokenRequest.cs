using Refit;

namespace ProjectTemplate.Infra.ExternalService.Apic.Authentication.GetAccessToken
{
    internal record GetAccessTokenRequest
    {
        [AliasAs("grant_type")]
        public string GrantType { get; set; } = "client_credentials";

        [AliasAs("scope")]
        public required string Scope { get; set; }

        [AliasAs("client_id")]
        public required string ClientId { get; set; }

        [AliasAs("client_secret")]
        public required string ClientSecret { get; set; }
    }
}
