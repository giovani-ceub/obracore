using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ProjectTemplate.Application.Common.Interfaces;
using ProjectTemplate.Application.IntegrationTests.Fixtures;
using ProjectTemplate.Infra.CrossCutting.Identity;
using ProjectTemplate.Infra.Data;
using System.Security.Claims;

namespace ProjectTemplate.Application.IntegrationTests
{

    public sealed class IntegrationTestWebAppFactory : WebApplicationFactory<Program>
    {
        private readonly string _applicationConnectionString;
        private readonly string _identityConnectionString;
        private readonly string _redisConnectionString;

        public IntegrationTestWebAppFactory(TestDatabaseFixture fixture, TestRedisFixture redisFixture, IUser? user = null)
        {
            _applicationConnectionString = fixture.GetApplicationConnectionString();
            _identityConnectionString = fixture.GetIdentityConnectionString();
            _redisConnectionString = redisFixture.GetConnectionString();
            CurrentUser = user;
        }

        public IUser? CurrentUser { get; set; }
        public ClaimsPrincipal? TestUserPrincipal { get; set; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                if (CurrentUser is not null)
                {
                    services
                        .RemoveAll<IUser>()
                        .AddScoped<IUser>(_ => CurrentUser);
                }

                var dbContextDescriptor = services
                    .SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                var userDbContextDescriptor = services
                    .SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<ApplicationUserDbContext>));

                if (dbContextDescriptor is not null)
                {
                    services.Remove(dbContextDescriptor);
                }

                if (userDbContextDescriptor is not null)
                {
                    services.Remove(userDbContextDescriptor);
                }

                services.AddDbContext<ApplicationDbContext>((sp, options) =>
                {
                    options.UseSqlServer(_applicationConnectionString);
                    options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                });
                services.AddDbContext<ApplicationUserDbContext>(options =>
                {
                    options.UseSqlServer(_identityConnectionString);
                });

                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = _redisConnectionString;
                });
            });

            base.ConfigureWebHost(builder);
        }
    }
}
