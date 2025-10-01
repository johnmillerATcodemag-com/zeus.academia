using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Data.Repositories;
using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.Tests.Infrastructure;

/// <summary>
/// Unit tests for repository pattern implementation.
/// Task 6: Repository Pattern Implementation - Basic repository testing.
/// </summary>
public class RepositoryPatternTests : IDisposable
{
    private readonly AcademiaDbContext _context;
    private readonly IServiceProvider _serviceProvider;
    private readonly IUnitOfWork _unitOfWork;

    public RepositoryPatternTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<AcademiaDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        // Setup configuration
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "DataSource=:memory:",
                ["Logging:LogLevel:Default"] = "Information"
            })
            .Build();

        // Setup service collection
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
        services.AddDbContext<AcademiaDbContext>(opt => opt.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}"));
        services.AddInfrastructureData();

        _serviceProvider = services.BuildServiceProvider();
        _context = _serviceProvider.GetRequiredService<AcademiaDbContext>();
        _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();

        // Ensure database is created
        _context.Database.EnsureCreated();

        // Seed test data
        SeedTestData().Wait();
    }

    [Fact]
    public async Task GenericRepository_AddAsync_Should_Add_Entity()
    {
        // Arrange
        var repository = _unitOfWork.Repository<AccessLevel>();
        var accessLevel = new AccessLevel { Code = "TEST", Name = "Test Level", Description = "Test Description" };

        // Act
        var result = await repository.AddAsync(accessLevel);
        await _unitOfWork.SaveChangesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("TEST", result.Code);
        Assert.Equal("Test Level", result.Name);
    }

    [Fact]
    public async Task GenericRepository_GetByIdAsync_Should_Return_Entity()
    {
        // Arrange
        var repository = _unitOfWork.Repository<AccessLevel>();
        var accessLevel = await repository.FirstOrDefaultAsync(a => a.Name == "System Administrator");
        Assert.NotNull(accessLevel);

        // Act
        var result = await repository.GetByIdAsync(accessLevel.Code);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(accessLevel.Code, result.Code);
        Assert.Equal("System Administrator", result.Name);
    }

    [Fact]
    public async Task GenericRepository_GetAllAsync_Should_Return_All_Entities()
    {
        // Arrange
        var repository = _unitOfWork.Repository<AccessLevel>();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count() >= 3); // We seeded 3 access levels
    }

    [Fact]
    public async Task GenericRepository_FindAsync_Should_Return_Matching_Entities()
    {
        // Arrange
        var repository = _unitOfWork.Repository<AccessLevel>();

        // Act
        var result = await repository.FindAsync(a => a.Name.Contains("Admin"));

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count() >= 2); // System Administrator and University Administrator
    }

    [Fact]
    public async Task GenericRepository_Update_Should_Modify_Entity()
    {
        // Arrange
        var repository = _unitOfWork.Repository<AccessLevel>();
        var accessLevel = await repository.FirstOrDefaultAsync(a => a.Code == "SYSADM");
        Assert.NotNull(accessLevel);

        var originalDescription = accessLevel.Description;
        accessLevel.Description = "Updated Description";

        // Act
        var result = repository.Update(accessLevel);
        await _unitOfWork.SaveChangesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Description", result.Description);
        Assert.NotEqual(originalDescription, result.Description);
    }

    [Fact]
    public async Task GenericRepository_Remove_Should_Delete_Entity()
    {
        // Arrange
        var repository = _unitOfWork.Repository<AccessLevel>();
        var accessLevel = new AccessLevel { Code = "DELETE", Name = "ToDelete", Description = "Will be deleted" };
        await repository.AddAsync(accessLevel);
        await _unitOfWork.SaveChangesAsync();

        // Act
        repository.Remove(accessLevel);
        await _unitOfWork.SaveChangesAsync();

        // Assert
        var deletedEntity = await repository.GetByIdAsync(accessLevel.Code);
        Assert.Null(deletedEntity);
    }

    [Fact]
    public async Task GenericRepository_CountAsync_Should_Return_Correct_Count()
    {
        // Arrange
        var repository = _unitOfWork.Repository<AccessLevel>();

        // Act
        var totalCount = await repository.CountAsync(a => true); // Count all
        var filteredCount = await repository.CountAsync(a => a.Name.Contains("Admin"));

        // Assert
        Assert.Equal(3, totalCount);
        Assert.Equal(3, filteredCount); // System Administrator, University Administrator, and Department Administrator
    }

    [Fact]
    public async Task GenericRepository_AnyAsync_Should_Return_Correct_Result()
    {
        // Arrange
        var repository = _unitOfWork.Repository<AccessLevel>();

        // Act
        var exists = await repository.AnyAsync(a => a.Code == "SYSADM");
        var notExists = await repository.AnyAsync(a => a.Code == "NONEXIST");

        // Assert
        Assert.True(exists);
        Assert.False(notExists);
    }

    [Fact]
    public async Task GenericRepository_GetPagedAsync_Should_Return_Paged_Results()
    {
        // Arrange
        var repository = _unitOfWork.Repository<AccessLevel>();

        // Act
        var firstPage = await repository.GetPagedAsync(0, 2);
        var secondPage = await repository.GetPagedAsync(2, 2);

        // Assert
        Assert.NotNull(firstPage);
        Assert.NotNull(secondPage);
        Assert.Equal(2, firstPage.Count());
        Assert.True(secondPage.Any()); // Should have at least some results
    }

    [Fact]
    public async Task AcademicRepository_GetByEmployeeNumberAsync_Should_Return_Academic()
    {
        // Arrange & Act
        var academic = await _unitOfWork.Academics.GetByEmployeeNumberAsync("1001");

        // Assert
        Assert.NotNull(academic);
        Assert.Equal(1001, academic.EmpNr);
        Assert.Equal("Dr. John Smith", academic.Name);
    }

    [Fact]
    public async Task AcademicRepository_SearchByNameAsync_Should_Return_Matching_Academics()
    {
        // Arrange & Act
        var academics = await _unitOfWork.Academics.SearchByNameAsync("Smith");

        // Assert
        Assert.NotNull(academics);
        Assert.True(academics.Any());
        Assert.All(academics, a => Assert.Contains("Smith", a.Name));
    }

    [Fact]
    public async Task AcademicRepository_IsEmployeeNumberAvailableAsync_Should_Return_Correct_Result()
    {
        // Arrange & Act
        var isAvailable = await _unitOfWork.Academics.IsEmployeeNumberAvailableAsync("9999");
        var isNotAvailable = await _unitOfWork.Academics.IsEmployeeNumberAvailableAsync("1001");

        // Assert
        Assert.True(isAvailable);
        Assert.False(isNotAvailable);
    }

    [Fact]
    public async Task DepartmentRepository_GetByCodeAsync_Should_Return_Department()
    {
        // Arrange & Act
        var department = await _unitOfWork.Departments.GetByCodeAsync("Computer Science");

        // Assert
        Assert.NotNull(department);
        Assert.Equal("Computer Science", department.Name);
    }

    [Fact]
    public async Task DepartmentRepository_IsCodeAvailableAsync_Should_Return_Correct_Result()
    {
        // Arrange & Act
        var isAvailable = await _unitOfWork.Departments.IsCodeAvailableAsync("NEW DEPARTMENT");
        var isNotAvailable = await _unitOfWork.Departments.IsCodeAvailableAsync("Computer Science");

        // Assert
        Assert.True(isAvailable);
        Assert.False(isNotAvailable);
    }

    [Fact]
    public async Task SubjectRepository_GetByCodeAsync_Should_Return_Subject()
    {
        // Arrange & Act
        var subject = await _unitOfWork.Subjects.GetByCodeAsync("CS101");

        // Assert
        Assert.NotNull(subject);
        Assert.Equal("CS101", subject.Code);
        Assert.Equal("Introduction to Computer Science", subject.Title);
    }

    [Fact(Skip = "Transactions not supported in InMemory database")]
    public async Task UnitOfWork_Transaction_Should_Commit_Successfully()
    {
        // Arrange
        var accessLevel = new AccessLevel { Code = "TRANST", Name = "TransactionTest", Description = "Testing transaction" };

        // Act
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        await _unitOfWork.Repository<AccessLevel>().AddAsync(accessLevel);
        await _unitOfWork.SaveChangesAsync();
        await transaction.CommitAsync();

        // Assert
        var savedEntity = await _unitOfWork.Repository<AccessLevel>()
            .FirstOrDefaultAsync(a => a.Code == "TRANST");
        Assert.NotNull(savedEntity);
    }

    [Fact(Skip = "Transactions not supported in InMemory database")]
    public async Task UnitOfWork_Transaction_Should_Rollback_Successfully()
    {
        // Arrange
        var accessLevel = new AccessLevel { Code = "ROLLBT", Name = "RollbackTest", Description = "Testing rollback" };

        // Act
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        await _unitOfWork.Repository<AccessLevel>().AddAsync(accessLevel);
        await _unitOfWork.SaveChangesAsync();
        await transaction.RollbackAsync();

        // Assert
        var savedEntity = await _unitOfWork.Repository<AccessLevel>()
            .FirstOrDefaultAsync(a => a.Code == "ROLLBT");
        Assert.Null(savedEntity);
    }

    [Fact]
    public async Task UnitOfWork_SaveChangesAsync_With_UserId_Should_Update_Audit_Fields()
    {
        // Arrange
        var userId = "testuser@example.com";
        var accessLevel = new AccessLevel { Code = "AUDITT", Name = "AuditTest", Description = "Testing audit fields" };

        // Act
        await _unitOfWork.Repository<AccessLevel>().AddAsync(accessLevel);
        await _unitOfWork.SaveChangesAsync(userId);

        // Assert
        Assert.Equal(userId, accessLevel.CreatedBy);
        Assert.Equal(userId, accessLevel.ModifiedBy);
        Assert.True(accessLevel.CreatedDate <= DateTime.UtcNow);
        Assert.True(accessLevel.ModifiedDate <= DateTime.UtcNow);
    }

    private async Task SeedTestData()
    {
        // Seed Universities
        var university = new University
        {
            Code = "UOA",
            Name = "University of Academia",
            Location = "123 Academic Street, Academia City",
            Website = "https://www.uoa.edu"
        };
        _context.Universities.Add(university);

        // Seed Access Levels
        var accessLevels = new[]
        {
            new AccessLevel { Code = "SYSADM", Name = "System Administrator", Description = "System Administrator" },
            new AccessLevel { Code = "UNIADM", Name = "University Administrator", Description = "University Administrator" },
            new AccessLevel { Code = "DPTADM", Name = "Department Administrator", Description = "Department Administrator" }
        };
        _context.AccessLevels.AddRange(accessLevels);

        // Seed Ranks
        var rank = new Rank { Code = "PROF", Title = "Professor", Description = "Full Professor" };
        _context.Ranks.Add(rank);

        // Seed Degrees
        var degree = new Degree { Code = "PHD", Title = "Doctor of Philosophy", Level = "Doctorate", Description = "Doctor of Philosophy" };
        _context.Degrees.Add(degree);

        await _context.SaveChangesAsync();

        // Seed Departments
        var department = new Department
        {
            Name = "Computer Science",
            FullName = "Computer Science Department"
        };
        _context.Departments.Add(department);

        await _context.SaveChangesAsync();

        // Seed Subjects
        var subject = new Subject
        {
            Code = "CS101",
            Title = "Introduction to Computer Science",
            Description = "Basic computer science concepts",
            DepartmentName = department.Name
        };
        _context.Subjects.Add(subject);

        // Seed Academics (using Professor as concrete type)
        var professor = new Professor
        {
            EmpNr = 1001,
            Name = "Dr. John Smith",
            PhoneNumber = "555-0001",
            DepartmentName = department.Name,
            RankCode = rank.Code
        };
        _context.Set<Professor>().Add(professor);

        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context?.Dispose();
        _serviceProvider?.GetService<IServiceScope>()?.Dispose();
    }
}