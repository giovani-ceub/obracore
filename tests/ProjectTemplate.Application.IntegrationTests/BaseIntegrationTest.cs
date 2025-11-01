using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ProjectTemplate.Application.Common.Interfaces;
using ProjectTemplate.Application.IntegrationTests.Authentication;
using ProjectTemplate.Application.IntegrationTests.Fixtures;
using ProjectTemplate.Domain.Constants;
using ProjectTemplate.Infra.CrossCutting.Identity;
using ProjectTemplate.Infra.Data;
using Respawn;

namespace ProjectTemplate.Application.IntegrationTests
{
    public abstract class BaseIntegrationTest : IClassFixture<TestDatabaseFixture>,
                                                IClassFixture<TestRedisFixture>,
                                                IAsyncLifetime
    {
        protected IServiceScope ServiceScope;
        protected ISender Sender => ServiceScope.ServiceProvider.GetRequiredService<ISender>();
        protected IntegrationTestWebAppFactory Factory;

        protected readonly ApplicationDbContext DbContext;
        protected readonly TestDatabaseFixture TestDatabaseFixture;
        protected readonly TestRedisFixture TestRedisFixture;

        private Respawner _applicationDBContextCheckpoint = null!;
        private Respawner _identityDBContextCheckpoint = null!;

        protected BaseIntegrationTest(TestDatabaseFixture databaseFixture, TestRedisFixture redisFixture)
        {
            TestDatabaseFixture = databaseFixture;
            TestRedisFixture = redisFixture;
            Factory = new IntegrationTestWebAppFactory(TestDatabaseFixture, TestRedisFixture);
            ServiceScope = Factory.Services.CreateScope();
            DbContext = ServiceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        }

        public Task InitializeAsync() => InitializeDatabaseCheckpoint();
        public Task DisposeAsync() => ResetDatabaseCheckpointAsync();

        public async Task<string> RunAsDefaultUserAsync() => await RunAsUserAsync("test@local", "Testing1234!", Array.Empty<string>());
        public async Task<string> RunAsAdminUserAsync() => await RunAsUserAsync("administrator@local", "Administrator1234!", new[] { Roles.Administrator });
        public async Task<string> RunAsUserAsync(string userName, string password, IEnumerable<string> roles)
        {
            var userManager = ServiceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var applicationUserFounded = await userManager.FindByNameAsync(userName);

            if (applicationUserFounded is not null)
            {
                await userManager.DeleteAsync(applicationUserFounded);
            }

            var applicationUser = new ApplicationUser { UserName = userName, Email = userName };
            var result = await userManager.CreateAsync(applicationUser, password);

            if (roles.Any())
            {
                await userManager.AddToRolesAsync(applicationUser, roles);
            }

            if (result.Succeeded)
            {
                SetupScopeUsingDeterminateUser(new UserMock(applicationUser.Id, userName, userName, password, roles));
                return applicationUser.Id;
            }

            var errors = string.Join(Environment.NewLine, result.Errors.SelectMany(e => e.Description));

            throw new Exception($"Unable to create {applicationUser.UserName}.{Environment.NewLine}{errors}");
        }

        protected async Task InitializeDatabaseCheckpoint()
        {
            _applicationDBContextCheckpoint = await Respawner.CreateAsync(TestDatabaseFixture.GetApplicationConnectionString(), new RespawnerOptions
            {
                TablesToIgnore = ["__EFMigrationsHistory"]
            });
            _identityDBContextCheckpoint = await Respawner.CreateAsync(TestDatabaseFixture.GetIdentityConnectionString(), new RespawnerOptions
            {
                TablesToIgnore = ["__EFMigrationsHistory"]
            });
        }

        protected async Task ResetDatabaseCheckpointAsync()
        {
            await Task.WhenAll(
            _applicationDBContextCheckpoint.ResetAsync(TestDatabaseFixture.GetApplicationConnectionString()),
            _identityDBContextCheckpoint.ResetAsync(TestDatabaseFixture.GetIdentityConnectionString()));
        }

        private void SetupScopeUsingDeterminateUser(IUser user)
        {
            Factory = new IntegrationTestWebAppFactory(TestDatabaseFixture, TestRedisFixture, user);
            ServiceScope = Factory.Services.CreateScope();
        }
    }
}


