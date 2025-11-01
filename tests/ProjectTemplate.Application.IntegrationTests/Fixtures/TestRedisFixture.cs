using Testcontainers.Redis;

namespace ProjectTemplate.Application.IntegrationTests.Fixtures
{
    public class TestRedisFixture : IAsyncLifetime
    {
        private readonly RedisContainer _redisContainer = new RedisBuilder()
                .WithImage("redis:latest")
                .WithPortBinding(6379, assignRandomHostPort: true) // Map to a random host port
                .Build();

        public async Task InitializeAsync()
        {
            await Task.WhenAll(_redisContainer.StartAsync());
        }

        public Task DisposeAsync()
        {
            return Task
               .WhenAll(_redisContainer.DisposeAsync().AsTask());
        }

        public string GetConnectionString() => _redisContainer.GetConnectionString();
    }
}
