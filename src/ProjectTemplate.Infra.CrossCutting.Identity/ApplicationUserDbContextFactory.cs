using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using ProjectTemplate.Infra.CrossCutting.Identity;

public class ApplicationUserDbContextFactory : IDesignTimeDbContextFactory<ApplicationUserDbContext>
{
    public ApplicationUserDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddUserSecrets<ApplicationUserDbContextFactory>() // optional
            .Build();

        var connectionString = config.GetConnectionString("projecttemplate-identity-db");

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationUserDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new ApplicationUserDbContext(optionsBuilder.Options);
    }
}