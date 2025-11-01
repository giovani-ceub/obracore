using Testcontainers.MsSql;

namespace ProjectTemplate.Application.IntegrationTests.Fixtures
{
    public class TestDatabaseFixture : IAsyncLifetime
    {
        private readonly MsSqlContainer _msSqlApplicationDbContextContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2019-latest")
            .Build();

        private readonly MsSqlContainer _msSqlIdentityDbContextContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2019-latest")
            .Build();

        public async Task InitializeAsync()
        {
            await Task.WhenAll(_msSqlApplicationDbContextContainer.StartAsync(),
                               _msSqlIdentityDbContextContainer.StartAsync());
        }

        public Task DisposeAsync()
        {
            return Task
               .WhenAll(
                   _msSqlApplicationDbContextContainer.DisposeAsync().AsTask(),
                   _msSqlIdentityDbContextContainer.DisposeAsync().AsTask());
        }

        public string GetApplicationConnectionString() => _msSqlApplicationDbContextContainer.GetConnectionString();
        public string GetIdentityConnectionString() => _msSqlIdentityDbContextContainer.GetConnectionString();
    }
}
