using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectTemplate.Application.Common.Interfaces;
using ProjectTemplate.Domain.Constants;
using ProjectTemplate.Infra.Data;

namespace ProjectTemplate.Infra.CrossCutting.Identity
{
    public static class DependencyInjection
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            RegisterServices(services);
            AddAuthenticationAndAuthorizationConfiguration(services, configuration);
            AddIdentityEntityFramework(services, configuration);

            return services;
        }

        private static void AddAuthenticationAndAuthorizationConfiguration(IServiceCollection services, IConfiguration configuration)
        {

#if (IsAuthenticatedByAzureAD)
                AddAuthenticationViaAzureAd(services, configuration);
                AddAuthenticationViaAzureAdB2C(services, configuration);
#endif

            AddAuthenticationViaIdentityDB(services);

            services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.CanPurge, policy => policy.RequireRole(Roles.Administrator));
            });
        }

        private static void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<ApplicationUserDbContext>();
            services.AddScoped<IdentityDbContextInitializer>();
            services.AddTransient<IIdentityService, IdentityService>();
        }

        private static void AddIdentityEntityFramework(IServiceCollection services, IConfiguration configuration)
        {
            string? connectionString = configuration.GetConnectionString("projecttemplate-identity-db");
            Guard.Against.Null(connectionString, message: "Connection string 'sqlServer' not found.");

            services.AddDbContext<ApplicationUserDbContext>(options =>
                options.UseSqlServer(connectionString,
                    b => b.MigrationsAssembly("ProjectTemplate.Infra.CrossCutting.Identity"))
            );
        }

        private static void AddAuthenticationViaIdentityDB(IServiceCollection services)
        {
            services.AddAuthentication()
                                .AddBearerToken(IdentityConstants.BearerScheme);

            services.AddAuthorizationBuilder();

            services
                .AddIdentityCore<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationUserDbContext>()
                .AddApiEndpoints();
        }

#if (IsAuthenticatedByAzureAD)
        private static void AddAuthenticationViaAzureAd(IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication()
                    .AddMicrosoftIdentityWebApi(configuration, "AzureAd", "Bearer")
                    .EnableTokenAcquisitionToCallDownstreamApi()
                    .AddInMemoryTokenCaches();
        }

        private static void AddAuthenticationViaAzureAdB2C(IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication()
                    .AddMicrosoftIdentityWebApi(configuration, "AzureAdB2C", "B2CScheme")
                    .EnableTokenAcquisitionToCallDownstreamApi();
        }
#endif
    }
}
