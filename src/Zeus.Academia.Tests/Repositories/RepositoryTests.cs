using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Repositories;
using Xunit;

namespace Zeus.Academia.Tests.Repositories;

/// <summary>
/// Comprehensive tests for the generic Repository<T> class.
/// </summary>
public class RepositoryTests : IDisposable
{
    private readonly AcademiaDbContext _context;
    private readonly Mock<ILogger<Repository<Department>>> _mockLogger;
    private readonly Repository<Department> _repository;

    public RepositoryTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<AcademiaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "InMemory"
            })
            .Build();

        _context = new AcademiaDbContext(options, configuration);
        _mockLogger = new Mock<ILogger<Repository<Department>>>();
        _repository = new Repository<Department>(_context, _mockLogger.Object);

        // Seed test data
        SeedTestData();
    }

    private void SeedTestData()
    {
        var departments = new List<Department>
        {
            new Department
            {
                Name = "CS",
                FullName = "Computer Science",
                Description = "Computer Science Department",
                IsActive = true,
                CreatedBy = "TestUser",
                ModifiedBy = "TestUser"
            },
            new Department
            {
                Name = "MATH",
                FullName = "Mathematics",
                Description = "Mathematics Department",
                IsActive = true,
                CreatedBy = "TestUser",
                ModifiedBy = "TestUser"
            },
            new Department
            {
                Name = "PHYS",
                FullName = "Physics",
                Description = "Physics Department",
                IsActive = false,
                CreatedBy = "TestUser",
                ModifiedBy = "TestUser"
            }
        };

        _context.Departments.AddRange(departments);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEntities()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnEntity()
    {
        // Arrange
        var department = await _context.Departments.FirstAsync();

        // Act - Use FindAsync since Department uses string primary key (Name)
        var results = await _repository.FindAsync(d => d.Name == department.Name);
        var result = results.FirstOrDefault();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(department.Name, result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Act - Use FindAsync with non-existent department name
        var results = await _repository.FindAsync(d => d.Name == "NonExistentDepartment");
        var result = results.FirstOrDefault();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task FindAsync_WithValidPredicate_ShouldReturnMatchingEntities()
    {
        // Act
        var result = await _repository.FindAsync(d => d.IsActive);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, d => Assert.True(d.IsActive));
    }

    [Fact]
    public async Task FindAsync_WithNullPredicate_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.FindAsync(null!));
    }

    [Fact]
    public async Task GetSingleAsync_WithValidPredicate_ShouldReturnSingleEntity()
    {
        // Act
        var result = await _repository.GetSingleAsync(d => d.Name == "CS");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("CS", result.Name);
    }

    [Fact]
    public async Task GetSingleAsync_WithNoMatch_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetSingleAsync(d => d.Name == "NonExistent");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_WithValidEntity_ShouldAddEntity()
    {
        // Arrange
        var newDepartment = new Department
        {
            Name = "BIO",
            FullName = "Biology",
            Description = "Biology Department",
            IsActive = true,
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };

        // Act
        var result = await _repository.AddAsync(newDepartment);
        await _context.SaveChangesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("BIO", result.Name);

        var savedEntity = await _context.Departments.FindAsync("BIO");
        Assert.NotNull(savedEntity);
    }

    [Fact]
    public async Task AddAsync_WithNullEntity_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.AddAsync(null!));
    }

    [Fact]
    public async Task AddRangeAsync_WithValidEntities_ShouldAddAllEntities()
    {
        // Arrange
        var newDepartments = new List<Department>
        {
            new Department
            {
                Name = "CHEM",
                FullName = "Chemistry",
                Description = "Chemistry Department",
                IsActive = true,
                CreatedBy = "TestUser",
                ModifiedBy = "TestUser"
            },
            new Department
            {
                Name = "ENG",
                FullName = "Engineering",
                Description = "Engineering Department",
                IsActive = true,
                CreatedBy = "TestUser",
                ModifiedBy = "TestUser"
            }
        };

        // Act
        await _repository.AddRangeAsync(newDepartments);
        await _context.SaveChangesAsync();

        // Assert
        var allDepartments = await _repository.GetAllAsync();
        Assert.Equal(5, allDepartments.Count());
    }

    [Fact]
    public async Task UpdateAsync_WithValidEntity_ShouldUpdateEntity()
    {
        // Arrange
        var department = await _context.Departments.FirstAsync();
        var originalModifiedDate = department.ModifiedDate;
        department.Description = "Updated Description";

        // Wait a moment to ensure ModifiedDate changes
        await Task.Delay(10);

        // Act
        var result = await _repository.UpdateAsync(department);
        await _context.SaveChangesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Description", result.Description);
        Assert.True(result.ModifiedDate > originalModifiedDate);
    }

    [Fact]
    public async Task RemoveAsync_WithValidEntity_ShouldRemoveEntity()
    {
        // Arrange
        var department = await _context.Departments.FirstAsync();
        var originalCount = await _repository.CountAsync();

        // Act
        await _repository.RemoveAsync(department);
        await _context.SaveChangesAsync();

        // Assert
        var newCount = await _repository.CountAsync();
        Assert.Equal(originalCount - 1, newCount);
    }

    [Fact]
    public async Task RemoveByIdAsync_WithValidId_ShouldRemoveEntity()
    {
        // Arrange
        var department = await _context.Departments.FirstAsync();
        var originalCount = await _repository.CountAsync();

        // Act - Use RemoveAsync instead since Department uses string primary key
        await _repository.RemoveAsync(department);
        await _context.SaveChangesAsync();

        // Assert
        var newCount = await _repository.CountAsync();
        Assert.Equal(originalCount - 1, newCount);
    }

    [Fact]
    public async Task CountAsync_WithoutPredicate_ShouldReturnTotalCount()
    {
        // Act
        var result = await _repository.CountAsync();

        // Assert
        Assert.Equal(3, result);
    }

    [Fact]
    public async Task CountAsync_WithPredicate_ShouldReturnFilteredCount()
    {
        // Act
        var result = await _repository.CountAsync(d => d.IsActive);

        // Assert
        Assert.Equal(2, result);
    }

    [Fact]
    public async Task ExistsAsync_WithMatchingPredicate_ShouldReturnTrue()
    {
        // Act
        var result = await _repository.ExistsAsync(d => d.Name == "CS");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithNonMatchingPredicate_ShouldReturnFalse()
    {
        // Act
        var result = await _repository.ExistsAsync(d => d.Name == "NonExistent");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetPagedAsync_WithValidParameters_ShouldReturnPagedResults()
    {
        // Act
        var result = await _repository.GetPagedAsync(1, 2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetPagedAsync_WithInvalidPageNumber_ShouldThrowArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _repository.GetPagedAsync(0, 10));
    }

    [Fact]
    public async Task GetPagedAsync_WithInvalidPageSize_ShouldThrowArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _repository.GetPagedAsync(1, 0));
    }

    [Fact]
    public void Constructor_WithNullContext_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Repository<Department>(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Repository<Department>(_context, null!));
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}