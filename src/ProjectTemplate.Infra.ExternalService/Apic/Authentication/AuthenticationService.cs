using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProjectTemplate.Application.Abstrations.Apic.Authentication;
using ProjectTemplate.Infra.ExternalService.Apic.Authentication.GetAccessToken;
using ProjectTemplate.Infra.ExternalService.Apic.Authentication.Settings;
using Refit;

namespace ProjectTemplate.Infra.ExternalService.Apic.Authentication
{
    internal class AuthenticationService : IAuthenticationService
    {
        private const string ErrorMessage = "An error occurred while retrieving the {0}.";
        private readonly ILogger<AuthenticationService> _logger;
        private readonly ApicAuthenticationSettings _apicSettings;
        private readonly IAuthenticationApiClient _apiClient;

        public AuthenticationService(IOptions<ApicAuthenticationSettings> apicSettings, IAuthenticationApiClient apiClient, ILogger<AuthenticationService> logger)
        {
            _apicSettings = apicSettings.Value;
            _apiClient = apiClient;
            _logger = logger;
            //_apiClient = RestService.For<IAuthenticationApiClient>(_apicSettings.BaseUri);            
        }

        public async Task<GetAccessTokenResponseDto> GetAccessTokenAsync()
        {
            var request = new GetAccessTokenRequest
            {
                ClientId = _apicSettings.ClientId,
                ClientSecret = _apicSettings.ClientSecret,
                Scope = _apicSettings.Scope,
                GrantType = _apicSettings.GrantType,
            };

            try
            {
                GetAccessTokenResponse response = await _apiClient.GetTokenAsync(request);
                return new GetAccessTokenResponseDto
                {
                    AccessToken = response.AccessToken,
                    ExpiresIn = response.ExpiresIn,
                    SessionState = response.SessionState,
                };
            }
            catch (ApiException ex)
            {
                _logger.LogCritical(ex, ErrorMessage, nameof(GetAccessTokenAsync));
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, ErrorMessage, nameof(GetAccessTokenAsync));
                throw;
            }
        }
    }
}
