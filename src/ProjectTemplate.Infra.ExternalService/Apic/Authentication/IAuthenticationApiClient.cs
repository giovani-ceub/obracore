using ProjectTemplate.Infra.ExternalService.Apic.Authentication.GetAccessToken;
using Refit;

namespace ProjectTemplate.Infra.ExternalService.Apic.Authentication
{
    internal interface IAuthenticationApiClient
    {
        [Post("/oauth-end/oauth2/token")]
        Task<GetAccessTokenResponse> GetTokenAsync([Body(BodySerializationMethod.UrlEncoded)] GetAccessTokenRequest request);
    }
}
