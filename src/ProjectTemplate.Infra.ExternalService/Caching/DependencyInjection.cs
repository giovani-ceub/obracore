using Ardalis.GuardClauses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectTemplate.Application.Abstrations.Caching;
using System.Diagnostics.CodeAnalysis;


namespace ProjectTemplate.Infra.ExternalService.Caching
{
    [ExcludeFromCodeCoverage]
    internal static class DependencyInjection
    {
        public static void ConfigureServices(this IServiceCollection services, IConfigurationManager configuration)
        {
            services.AddDistributedMemoryCache();
            AddServices(services, configuration);
            AddRedis(services, configuration);
        }

        private static void AddServices(IServiceCollection services, IConfigurationManager configuration)
        {
            services.AddSingleton<ICacheService, CacheService>();
        }

        private static void AddRedis(IServiceCollection services, IConfigurationManager configuration)
        {
            string? redisConnection = configuration.GetConnectionString("projecttemplate-mq");
            Guard.Against.Null(redisConnection, message: "Connection string 'redis' not found.");

            services.AddStackExchangeRedisCache(redisOptions =>
            {
                redisOptions.Configuration = redisConnection;
            });

            services.AddHealthChecks()
                .AddRedis(redisConnection, name: "redis");
        }
    }
}
