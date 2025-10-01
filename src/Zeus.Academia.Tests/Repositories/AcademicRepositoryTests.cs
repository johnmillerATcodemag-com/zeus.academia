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
/// Comprehensive tests for AcademicRepository implementation.
/// </summary>
public class AcademicRepositoryTests : IDisposable
{
    private readonly AcademiaDbContext _context;
    private readonly Mock<ILogger<AcademicRepository>> _mockLogger;
    private readonly AcademicRepository _repository;

    public AcademicRepositoryTests()
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
        _mockLogger = new Mock<ILogger<AcademicRepository>>();
        _repository = new AcademicRepository(_context, _mockLogger.Object);

        // Seed test data
        SeedTestData();
    }

    private void SeedTestData()
    {
        // Add departments first
        var departments = new List<Department>
        {
            new Department
            {
                Name = "CS",
                FullName = "Computer Science",
                IsActive = true,
                CreatedBy = "TestUser",
                ModifiedBy = "TestUser"
            },
            new Department
            {
                Name = "MATH",
                FullName = "Mathematics",
                IsActive = true,
                CreatedBy = "TestUser",
                ModifiedBy = "TestUser"
            }
        };

        _context.Departments.AddRange(departments);

        // Add ranks
        var ranks = new List<Rank>
        {
            new Rank
            {
                Code = "PROF",
                Title = "Professor",
                Description = "Full Professor",
                CreatedBy = "TestUser",
                ModifiedBy = "TestUser"
            },
            new Rank
            {
                Code = "ASSOC",
                Title = "Associate Professor",
                Description = "Associate Professor",
                CreatedBy = "TestUser",
                ModifiedBy = "TestUser"
            }
        };

        _context.Ranks.AddRange(ranks);

        // Add professors
        var professors = new List<Professor>
        {
            new Professor
            {
                EmpNr = 1,
                Name = "John Smith",
                DepartmentName = "CS",
                RankCode = "PROF",
                HasTenure = true,
                ResearchArea = "AI",
                PhoneNumber = "123-456-7890",
                Salary = 95000,
                CreatedBy = "TestUser",
                ModifiedBy = "TestUser"
            },
            new Professor
            {
                EmpNr = 2,
                Name = "Jane Doe",
                DepartmentName = "MATH",
                RankCode = "ASSOC",
                HasTenure = false,
                ResearchArea = "Statistics",
                PhoneNumber = "123-456-7891",
                Salary = 85000,
                CreatedBy = "TestUser",
                ModifiedBy = "TestUser"
            }
        };

        _context.Professors.AddRange(professors);

        // Add teachers
        var teachers = new List<Teacher>
        {
            new Teacher
            {
                EmpNr = 3,
                Name = "Bob Martin",
                DepartmentName = "CS",
                PhoneNumber = "123-456-7892",
                Salary = 60000,
                CreatedBy = "TestUser",
                ModifiedBy = "TestUser"
            }
        };

        _context.Teachers.AddRange(teachers);

        // Add students
        var students = new List<Student>
        {
            new Student
            {
                EmpNr = 4,
                Name = "Alice Brown",
                DepartmentName = "CS",
                DegreeCode = "BS",
                PhoneNumber = "123-456-7893",
                CreatedBy = "TestUser",
                ModifiedBy = "TestUser"
            }
        };

        _context.Students.AddRange(students);

        _context.SaveChanges();
    }

    [Fact]
    public async Task GetByDepartmentAsync_WithValidDepartmentName_ShouldReturnAcademics()
    {
        // Arrange
        var csDepartment = await _context.Departments.FirstAsync(d => d.Name == "CS");

        // Act
        var result = await _repository.GetByDepartmentAsync(csDepartment.Name);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Any());
        // Note: The implementation queries by department name, so this test validates the approach
    }

    [Fact]
    public async Task GetByDepartmentAsync_WithInvalidDepartmentName_ShouldReturnEmpty()
    {
        // Act
        var result = await _repository.GetByDepartmentAsync("NONEXISTENT");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByEmployeeNumberAsync_WithValidEmployeeNumber_ShouldReturnAcademic()
    {
        // Act
        var result = await _repository.GetByEmployeeNumberAsync("1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.EmpNr);
        Assert.Equal("John Smith", result.Name);
    }

    [Fact]
    public async Task GetByEmployeeNumberAsync_WithInvalidEmployeeNumber_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByEmployeeNumberAsync("9999");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmployeeNumberAsync_WithInvalidFormat_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByEmployeeNumberAsync("invalid");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmployeeNumberAsync_WithNullOrWhiteSpace_ShouldThrowArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _repository.GetByEmployeeNumberAsync(""));
        await Assert.ThrowsAsync<ArgumentException>(() => _repository.GetByEmployeeNumberAsync("   "));
    }

    [Fact]
    public async Task SearchByNameAsync_WithValidPattern_ShouldReturnMatching()
    {
        // Act
        var result = await _repository.SearchByNameAsync("John");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Contains(result, a => a.Name.Contains("John"));
    }

    [Fact]
    public async Task SearchByNameAsync_WithNoMatches_ShouldReturnEmpty()
    {
        // Act
        var result = await _repository.SearchByNameAsync("NonExistent");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchByNameAsync_WithNullOrWhiteSpace_ShouldThrowArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _repository.SearchByNameAsync(""));
        await Assert.ThrowsAsync<ArgumentException>(() => _repository.SearchByNameAsync("   "));
    }

    [Fact]
    public async Task GetByRankAsync_WithValidRankId_ShouldReturnAcademics()
    {
        // Arrange
        var profRank = await _context.Ranks.FirstAsync(r => r.Code == "PROF");

        // Act
        var result = await _repository.GetByRankAsync(profRank.Id);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Any());
    }

    [Fact]
    public async Task GetByRankAsync_WithInvalidRankId_ShouldReturnEmpty()
    {
        // Act
        var result = await _repository.GetByRankAsync(999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetProfessorsAsync_ShouldReturnAllProfessors()
    {
        // Act
        var result = await _repository.GetProfessorsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, p => Assert.IsType<Professor>(p));
    }

    [Fact]
    public async Task GetTeachersAsync_ShouldReturnAllTeachers()
    {
        // Act
        var result = await _repository.GetTeachersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.All(result, t => Assert.IsType<Teacher>(t));
    }

    [Fact]
    public async Task GetStudentsAsync_ShouldReturnAllStudents()
    {
        // Act
        var result = await _repository.GetStudentsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.All(result, s => Assert.IsType<Student>(s));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllAcademics()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(4, result.Count()); // 2 professors + 1 teacher + 1 student
    }

    [Fact]
    public async Task AddAsync_WithValidProfessor_ShouldAddProfessor()
    {
        // Arrange
        var newProfessor = new Professor
        {
            EmpNr = 1003,
            Name = "New Professor",
            DepartmentName = "CS",
            RankCode = "PROF",
            HasTenure = false,
            ResearchArea = "ML",
            PhoneNumber = "123-456-7894",
            Salary = 90000,
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };

        // Act
        var result = await _repository.AddAsync(newProfessor);
        await _context.SaveChangesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1003, result.EmpNr);

        var savedProfessor = await _context.Professors.FindAsync(1003);
        Assert.NotNull(savedProfessor);
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateRepository()
    {
        // Act & Assert
        Assert.NotNull(_repository);
    }

    [Fact]
    public void Constructor_WithNullContext_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new AcademicRepository(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new AcademicRepository(_context, null!));
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}