using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Zeus.Academia.Infrastructure.Data;

/// <summary>
/// Design-time factory for AcademiaDbContext to support EF Core migrations.
/// Task 5: Database Migrations - Design-time DbContext factory for migration generation.
/// </summary>
public class AcademiaDbContextFactory : IDesignTimeDbContextFactory<AcademiaDbContext>
{
    public AcademiaDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AcademiaDbContext>();

        // Use a default connection string for design-time operations
        // This will be overridden at runtime by the actual configuration
        var connectionString = "Server=(localdb)\\mssqllocaldb;Database=ZeusAcademiaDb;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true";
        
        optionsBuilder.UseSqlServer(connectionString, options =>
        {
            options.MigrationsAssembly("Zeus.Academia.Infrastructure");
            options.CommandTimeout(300); // 5 minutes for large migrations
        });

        // Enable sensitive data logging for development (design-time only)
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.EnableDetailedErrors();

        // Create a minimal configuration for design-time use
        var configurationData = new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = connectionString
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationData)
            .Build();

        return new AcademiaDbContext(optionsBuilder.Options, configuration);
    }
}