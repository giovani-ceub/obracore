using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace ProjectTemplate.Application.IntegrationTests.Authentication
{
    public class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "Test user") };
            claims.Add(new Claim(ClaimTypes.Role, "AdminUser"));
            var claimsIdentity = new ClaimsIdentity(claims, "Test");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            var ticket = new AuthenticationTicket(claimsPrincipal, "TestScheme");

            var result = AuthenticateResult.Success(ticket);

            if (result.Succeeded)
            {
                Context.User = claimsPrincipal;
            }

            return Task.FromResult(result);
        }
    }
}
