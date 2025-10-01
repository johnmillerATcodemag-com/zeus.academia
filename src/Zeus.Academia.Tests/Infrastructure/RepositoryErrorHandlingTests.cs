using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Data.Repositories;
using Zeus.Academia.Infrastructure.Entities;
using Xunit;

namespace Zeus.Academia.Tests.Infrastructure;

/// <summary>
/// Repository error handling and edge case tests for improved code coverage
/// </summary>
public class RepositoryErrorHandlingTests
{
    private AcademiaDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AcademiaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var configuration = new ConfigurationBuilder().Build();
        return new AcademiaDbContext(options, configuration);
    }

    private TestLogger<T> CreateTestLogger<T>()
    {
        return new TestLogger<T>();
    }

    #region Basic Repository Tests

    [Fact]
    public async Task Repository_GetByIdAsync_Should_Return_Null_For_Nonexistent_Entity()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateTestLogger<Repository<University>>();
        var repository = new Repository<University>(context, logger);

        // Act
        var result = await repository.GetByIdAsync("NONEXISTENT");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Repository_GetAllAsync_Should_Return_Empty_For_Empty_Database()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateTestLogger<Repository<University>>();
        var repository = new Repository<University>(context, logger);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Repository_AddAsync_Should_Throw_For_Null_Entity()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateTestLogger<Repository<University>>();
        var repository = new Repository<University>(context, logger);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => repository.AddAsync(null!));
    }

    [Fact]
    public async Task Repository_AddRangeAsync_Should_Throw_For_Null_Collection()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateTestLogger<Repository<University>>();
        var repository = new Repository<University>(context, logger);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => repository.AddRangeAsync(null!));
    }

    [Fact]
    public async Task Repository_AddRangeAsync_Should_Handle_Empty_Collection()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateTestLogger<Repository<University>>();
        var repository = new Repository<University>(context, logger);

        // Act
        await repository.AddRangeAsync(new List<University>());

        // Assert - Should not throw
        Assert.True(true);
    }

    [Fact]
    public async Task Repository_GetPagedAsync_Should_Handle_Valid_Parameters()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateTestLogger<Repository<University>>();
        var repository = new Repository<University>(context, logger);

        // Add test data
        await repository.AddAsync(new University { Code = "TEST1", Name = "Test University 1" });
        await repository.AddAsync(new University { Code = "TEST2", Name = "Test University 2" });
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetPagedAsync(skip: 0, take: 1);

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public async Task Repository_FindAsync_Should_Handle_Valid_Predicate()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateTestLogger<Repository<University>>();
        var repository = new Repository<University>(context, logger);

        // Add test data
        await repository.AddAsync(new University { Code = "FIND1", Name = "Find Test University" });
        await context.SaveChangesAsync();

        // Act
        var result = await repository.FindAsync(u => u.Name.Contains("Find"));

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public async Task Repository_FindAsync_Should_Throw_For_Null_Predicate()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateTestLogger<Repository<University>>();
        var repository = new Repository<University>(context, logger);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => repository.FindAsync(null!));
    }

    [Fact]
    public async Task Repository_CountAsync_Should_Return_Zero_For_Empty_Database()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateTestLogger<Repository<University>>();
        var repository = new Repository<University>(context, logger);

        // Act
        var result = await repository.CountAsync(u => u.IsActive);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task Repository_AnyAsync_Should_Return_False_For_Empty_Database()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateTestLogger<Repository<University>>();
        var repository = new Repository<University>(context, logger);

        // Act
        var result = await repository.AnyAsync(u => u.IsActive);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Repository_Update_Should_Handle_Valid_Entity()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateTestLogger<Repository<University>>();
        var repository = new Repository<University>(context, logger);

        var university = new University { Code = "UPD", Name = "Update Test" };

        // Act
        var result = repository.Update(university);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(university, result);
    }

    [Fact]
    public void Repository_Remove_Should_Handle_Valid_Entity()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateTestLogger<Repository<University>>();
        var repository = new Repository<University>(context, logger);

        var university = new University { Code = "REM", Name = "Remove Test" };

        // Act
        repository.Remove(university);

        // Assert - Should not throw
        Assert.True(true);
    }

    [Fact]
    public void Repository_UpdateRange_Should_Handle_Valid_Entities()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateTestLogger<Repository<University>>();
        var repository = new Repository<University>(context, logger);

        var universities = new List<University>
        {
            new University { Code = "UPR1", Name = "Update Range 1" },
            new University { Code = "UPR2", Name = "Update Range 2" }
        };

        // Act
        repository.UpdateRange(universities);

        // Assert - Should not throw
        Assert.True(true);
    }

    [Fact]
    public void Repository_RemoveRange_Should_Handle_Valid_Entities()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateTestLogger<Repository<University>>();
        var repository = new Repository<University>(context, logger);

        var universities = new List<University>
        {
            new University { Code = "RER1", Name = "Remove Range 1" },
            new University { Code = "RER2", Name = "Remove Range 2" }
        };

        // Act
        repository.RemoveRange(universities);

        // Assert - Should not throw
        Assert.True(true);
    }

    [Fact]
    public async Task Repository_Should_Handle_Large_Skip_Value()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateTestLogger<Repository<University>>();
        var repository = new Repository<University>(context, logger);

        // Act
        var result = await repository.GetPagedAsync(skip: 1000, take: 10);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task Repository_Should_Handle_Complex_Predicate()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateTestLogger<Repository<University>>();
        var repository = new Repository<University>(context, logger);

        // Add test data
        await repository.AddAsync(new University
        {
            Code = "COMPLEX",
            Name = "Complex Test University",
            Country = "USA",
            EstablishedYear = 2000
        });
        await context.SaveChangesAsync();

        // Act
        var result = await repository.FindAsync(u =>
            u.Name.Contains("Complex") &&
            u.Country == "USA" &&
            u.EstablishedYear > 1999);

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public async Task Repository_Should_Handle_Unicode_Data()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateTestLogger<Repository<University>>();
        var repository = new Repository<University>(context, logger);

        var unicode_university = new University
        {
            Code = "UNI",
            Name = "大学" // Chinese characters
        };

        // Act
        await repository.AddAsync(unicode_university);
        await context.SaveChangesAsync();

        var result = await repository.FindAsync(u => u.Name.Contains("大学"));

        // Assert
        Assert.Single(result);
        Assert.Equal("大学", result.First().Name);
    }

    #endregion
}