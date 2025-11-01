using Refit;

namespace ProjectTemplate.Infra.ExternalService.Apic.Authentication.GetAccessToken
{
    internal record GetAccessTokenResponse
    {
        [AliasAs("access_token")]
        public required string AccessToken { get; set; }

        [AliasAs("expires_in")]
        public required string ExpiresIn { get; set; }
        [AliasAs("session_state")]
        public required string SessionState { get; set; }

    }
}
