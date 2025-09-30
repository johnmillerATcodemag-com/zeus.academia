using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Extensions;
using Xunit;

namespace Zeus.Academia.Tests.Infrastructure;

/// <summary>
/// Tests for Entity Framework Core setup and configuration
/// </summary>
public class DatabaseSetupTests
{
    [Fact]
    public void AcademiaDbContext_Should_Be_Created_With_Valid_Configuration()
    {
        // Arrange
        var configuration = CreateTestConfiguration();
        var services = new ServiceCollection();

        // Act & Assert - Should not throw exception
        services.AddInfrastructureServices(configuration);
        var serviceProvider = services.BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AcademiaDbContext>();

        Assert.NotNull(dbContext);
    }

    [Fact]
    public void DbContext_Configuration_Should_Be_Valid()
    {
        // Arrange
        var configuration = CreateTestConfiguration();

        // Act & Assert - Should not throw exception
        var services = new ServiceCollection();
        services.AddInfrastructureServices(configuration);

        // Verify that the service registration was successful
        var serviceProvider = services.BuildServiceProvider();
        var dbContext = serviceProvider.GetService<AcademiaDbContext>();

        Assert.NotNull(dbContext);
    }

    [Fact]
    public void ServiceCollectionExtensions_Should_Throw_Exception_When_Connection_String_Missing()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(
            () => services.AddInfrastructureServices(configuration));

        Assert.Contains("Database connection string 'DefaultConnection' is not configured",
            exception.Message);
    }

    private static IConfiguration CreateTestConfiguration()
    {
        var configurationData = new Dictionary<string, string?>
        {
            {"ConnectionStrings:DefaultConnection", "Server=(localdb)\\mssqllocaldb;Database=TestDb;Trusted_Connection=true"}
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(configurationData)
            .Build();
    }
}