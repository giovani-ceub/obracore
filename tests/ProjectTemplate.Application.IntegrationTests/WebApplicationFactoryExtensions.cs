using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using ProjectTemplate.Application.IntegrationTests.Authentication;
using System.Net.Http.Headers;

namespace ProjectTemplate.Application.IntegrationTests
{
    internal static class WebApplicationFactoryExtensions
    {
        public static WebApplicationFactory<T> WithAuthentication<T>(this WebApplicationFactory<T> factory) where T : class
        {
            return factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication(defaultScheme: "TestScheme")
                            .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                                authenticationScheme: "TestScheme",
                                options => { });
                });
            });
        }

        public static HttpClient CreateClientWithTestAuthentication<T>(this WebApplicationFactory<T> factory) where T : class
        {
            var client = factory.WithAuthentication().CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "TestScheme");

            return client;
        }
    }
}
