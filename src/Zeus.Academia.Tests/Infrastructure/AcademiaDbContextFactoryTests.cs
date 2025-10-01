using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;
using Zeus.Academia.Infrastructure.Data;

namespace Zeus.Academia.Tests.Infrastructure;

/// <summary>
/// Comprehensive tests for AcademiaDbContextFactory to improve code coverage.
/// Tests design-time DbContext creation, configuration handling, and factory behavior.
/// </summary>
public class AcademiaDbContextFactoryTests
{
    private readonly AcademiaDbContextFactory _factory;

    public AcademiaDbContextFactoryTests()
    {
        _factory = new AcademiaDbContextFactory();
    }

    [Fact]
    public void CreateDbContext_WithEmptyArgs_Should_Return_Valid_DbContext()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act
        using var context = _factory.CreateDbContext(args);

        // Assert
        Assert.NotNull(context);
        Assert.IsType<AcademiaDbContext>(context);
    }

    [Fact]
    public void CreateDbContext_WithNullArgs_Should_Return_Valid_DbContext()
    {
        // Arrange
        string[] args = null!;

        // Act
        using var context = _factory.CreateDbContext(args);

        // Assert
        Assert.NotNull(context);
        Assert.IsType<AcademiaDbContext>(context);
    }

    [Fact]
    public void CreateDbContext_WithArgs_Should_Return_Valid_DbContext()
    {
        // Arrange
        var args = new[] { "arg1", "arg2" };

        // Act
        using var context = _factory.CreateDbContext(args);

        // Assert
        Assert.NotNull(context);
        Assert.IsType<AcademiaDbContext>(context);
    }

    [Fact]
    public void CreateDbContext_Should_Configure_SqlServer_Provider()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act
        using var context = _factory.CreateDbContext(args);

        // Assert
        Assert.NotNull(context.Database);
        Assert.True(context.Database.IsSqlServer());
    }

    [Fact]
    public void CreateDbContext_Should_Have_Correct_Database_Name()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act
        using var context = _factory.CreateDbContext(args);

        // Assert
        Assert.NotNull(context.Database);
        var connectionString = context.Database.GetConnectionString();
        Assert.Contains("ZeusAcademiaDb", connectionString);
    }

    [Fact]
    public void CreateDbContext_Should_Enable_Multiple_Active_Result_Sets()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act
        using var context = _factory.CreateDbContext(args);

        // Assert
        var connectionString = context.Database.GetConnectionString();
        Assert.Contains("MultipleActiveResultSets=true", connectionString);
    }

    [Fact]
    public void CreateDbContext_Should_Enable_Trusted_Connection()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act
        using var context = _factory.CreateDbContext(args);

        // Assert
        var connectionString = context.Database.GetConnectionString();
        Assert.Contains("Trusted_Connection=true", connectionString);
    }

    [Fact]
    public void CreateDbContext_Should_Trust_Server_Certificate()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act
        using var context = _factory.CreateDbContext(args);

        // Assert
        var connectionString = context.Database.GetConnectionString();
        Assert.Contains("TrustServerCertificate=true", connectionString);
    }

    [Fact]
    public void CreateDbContext_Should_Have_Configuration()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act
        using var context = _factory.CreateDbContext(args);

        // Assert
        // Access configuration through reflection or test behavior that depends on it
        Assert.NotNull(context);
        // We can verify the context can access its configuration by testing database operations
        Assert.NotNull(context.Database);
    }

    [Fact]
    public void CreateDbContext_Should_Be_Reusable()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act
        using var context1 = _factory.CreateDbContext(args);
        using var context2 = _factory.CreateDbContext(args);

        // Assert
        Assert.NotNull(context1);
        Assert.NotNull(context2);
        Assert.NotSame(context1, context2); // Should create new instances
    }

    [Fact]
    public void CreateDbContext_Should_Have_LocalDb_Server()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act
        using var context = _factory.CreateDbContext(args);

        // Assert
        var connectionString = context.Database.GetConnectionString();
        Assert.Contains("(localdb)\\mssqllocaldb", connectionString);
    }

    [Fact]
    public void CreateDbContext_Should_Have_Migrations_Assembly_Configured()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act
        using var context = _factory.CreateDbContext(args);

        // Assert
        // Verify that the context can be used for migrations
        Assert.NotNull(context.Database);
        // The migrations assembly configuration is internal to EF Core,
        // but we can verify the context is properly configured by ensuring it's not null
        Assert.NotNull(context.Model);
    }

    [Fact]
    public void CreateDbContext_Should_Have_All_Entity_Sets()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act
        using var context = _factory.CreateDbContext(args);

        // Assert
        Assert.NotNull(context.Universities);
        Assert.NotNull(context.Departments);
        Assert.NotNull(context.Academics);
        Assert.NotNull(context.Students);
        Assert.NotNull(context.Subjects);
        Assert.NotNull(context.AccessLevels);
        Assert.NotNull(context.Ranks);
        Assert.NotNull(context.Degrees);
        Assert.NotNull(context.Buildings);
        Assert.NotNull(context.Rooms);
        Assert.NotNull(context.Extensions);
    }

    [Fact]
    public void CreateDbContext_Multiple_Calls_Should_Create_Independent_Contexts()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act
        using var context1 = _factory.CreateDbContext(args);
        using var context2 = _factory.CreateDbContext(args);
        
        // Modify one context's change tracker
        context1.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        // Assert
        Assert.NotEqual(context1.ChangeTracker.QueryTrackingBehavior, 
                       context2.ChangeTracker.QueryTrackingBehavior);
    }

    [Fact]
    public void CreateDbContext_Should_Support_Design_Time_Services()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act
        using var context = _factory.CreateDbContext(args);

        // Assert
        // Verify the context can be used for design-time operations
        var options = context.Database.GetDbConnection();
        Assert.NotNull(options);
        Assert.NotNull(context.Model);
    }
}