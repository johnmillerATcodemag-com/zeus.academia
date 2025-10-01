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
/// Integration tests for repository pattern implementation.
/// </summary>
public class RepositoryIntegrationTests : IDisposable
{
    private readonly AcademiaDbContext _context;
    private readonly Mock<ILogger<Repository<Department>>> _mockLogger;
    private readonly Mock<ILogger<AcademicRepository>> _mockAcademicLogger;
    private readonly Mock<ILogger<DepartmentRepository>> _mockDepartmentLogger;
    private readonly Mock<ILogger<SubjectRepository>> _mockSubjectLogger;
    private readonly DepartmentRepository _departmentRepository;
    private readonly AcademicRepository _academicRepository;
    private readonly SubjectRepository _subjectRepository;

    public RepositoryIntegrationTests()
    {
        // Configure in-memory database
        var options = new DbContextOptionsBuilder<AcademiaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var mockConfiguration = new Mock<IConfiguration>();
        _context = new AcademiaDbContext(options, mockConfiguration.Object);
        _mockLogger = new Mock<ILogger<Repository<Department>>>();
        _mockAcademicLogger = new Mock<ILogger<AcademicRepository>>();
        _mockDepartmentLogger = new Mock<ILogger<DepartmentRepository>>();
        _mockSubjectLogger = new Mock<ILogger<SubjectRepository>>();

        _departmentRepository = new DepartmentRepository(_context, _mockDepartmentLogger.Object);
        _academicRepository = new AcademicRepository(_context, _mockAcademicLogger.Object);
        _subjectRepository = new SubjectRepository(_context, _mockSubjectLogger.Object);

        // Seed test data
        SeedTestData();
    }

    private void SeedTestData()
    {
        // Add test universities
        var universities = new List<University>
        {
            new University
            {
                Name = "Test University",
                Code = "TU",
                CreatedBy = "System",
                ModifiedBy = "System"
            }
        };
        _context.Universities.AddRange(universities);

        // Add test departments
        var departments = new List<Department>
        {
            new Department
            {
                Name = "Computer Science",
                FullName = "Computer Science Department",
                Description = "Computer Science Department",
                CreatedBy = "System",
                ModifiedBy = "System"
            },
            new Department
            {
                Name = "Mathematics",
                FullName = "Mathematics Department",
                Description = "Mathematics Department",
                CreatedBy = "System",
                ModifiedBy = "System"
            }
        };
        _context.Departments.AddRange(departments);

        // Add test ranks
        var ranks = new List<Rank>
        {
            new Rank
            {
                Code = "PROF",
                Title = "Professor",
                Description = "Full Professor",
                Level = 4,
                CreatedBy = "System",
                ModifiedBy = "System"
            },
            new Rank
            {
                Code = "ASSOC",
                Title = "Associate Professor",
                Description = "Associate Professor",
                Level = 3,
                CreatedBy = "System",
                ModifiedBy = "System"
            },
            new Rank
            {
                Code = "ASST",
                Title = "Assistant Professor",
                Description = "Assistant Professor",
                Level = 2,
                CreatedBy = "System",
                ModifiedBy = "System"
            }
        };
        _context.Ranks.AddRange(ranks);

        // Add test subjects
        var subjects = new List<Subject>
        {
            new Subject
            {
                Code = "CS101",
                Title = "Introduction to Computer Science",
                CreditHours = 3,
                Description = "Basic computer science concepts",
                DepartmentName = "Computer Science",
                CreatedBy = "System",
                ModifiedBy = "System"
            },
            new Subject
            {
                Code = "CS201",
                Title = "Data Structures",
                CreditHours = 4,
                Description = "Advanced data structures and algorithms",
                DepartmentName = "Computer Science",
                CreatedBy = "System",
                ModifiedBy = "System"
            },
            new Subject
            {
                Code = "MATH101",
                Title = "Calculus I",
                CreditHours = 4,
                Description = "Introduction to differential calculus",
                DepartmentName = "Mathematics",
                CreatedBy = "System",
                ModifiedBy = "System"
            }
        };
        _context.Subjects.AddRange(subjects);

        _context.SaveChanges();
    }

    [Fact]
    public async Task DepartmentRepository_GetAllAsync_ReturnsAllDepartments()
    {
        // Act
        var departments = await _departmentRepository.GetAllAsync();

        // Assert
        Assert.NotNull(departments);
        Assert.Equal(2, departments.Count());
    }

    [Fact]
    public async Task SubjectRepository_GetAllAsync_ReturnsAllSubjects()
    {
        // Act
        var subjects = await _subjectRepository.GetAllAsync();

        // Assert
        Assert.NotNull(subjects);
        Assert.Equal(3, subjects.Count());
    }

    [Fact]
    public async Task DepartmentRepository_GetSingleAsync_ReturnsDepartment()
    {
        // Act
        var department = await _departmentRepository.GetSingleAsync(d => d.Name == "Computer Science");

        // Assert
        Assert.NotNull(department);
        Assert.Equal("Computer Science", department.Name);
    }

    [Fact]
    public async Task SubjectRepository_GetSingleAsync_ReturnsSubject()
    {
        // Act
        var subject = await _subjectRepository.GetSingleAsync(s => s.Code == "CS101");

        // Assert
        Assert.NotNull(subject);
        Assert.Equal("CS101", subject.Code);
        Assert.Equal("Introduction to Computer Science", subject.Title);
    }

    [Fact]
    public async Task SubjectRepository_GetByDepartmentAsync_ReturnsFilteredSubjects()
    {
        // Act
        var subjects = await _subjectRepository.GetByDepartmentAsync("Computer Science");

        // Assert
        Assert.NotNull(subjects);
        Assert.Equal(2, subjects.Count());
        Assert.All(subjects, s => Assert.Equal("Computer Science", s.DepartmentName));
    }

    [Fact]
    public async Task Repository_AddAsync_AddsNewEntity()
    {
        // Arrange
        var newSubject = new Subject
        {
            Code = "CS301",
            Title = "Software Engineering",
            CreditHours = 3,
            Description = "Introduction to software engineering principles",
            DepartmentName = "Computer Science",
            CreatedBy = "Test",
            ModifiedBy = "Test"
        };

        // Act
        await _subjectRepository.AddAsync(newSubject);
        await _context.SaveChangesAsync();

        // Assert
        var found = await _subjectRepository.GetSingleAsync(s => s.Code == "CS301");
        Assert.NotNull(found);
        Assert.Equal("Software Engineering", found.Title);
    }

    [Fact]
    public async Task Repository_UpdateAsync_UpdatesEntity()
    {
        // Arrange
        var subject = await _subjectRepository.GetSingleAsync(s => s.Code == "CS101");
        Assert.NotNull(subject);

        // Act
        subject.Title = "Updated Title";
        await _subjectRepository.UpdateAsync(subject);
        await _context.SaveChangesAsync();

        // Assert
        var updated = await _subjectRepository.GetSingleAsync(s => s.Code == "CS101");
        Assert.NotNull(updated);
        Assert.Equal("Updated Title", updated.Title);
    }

    [Fact]
    public async Task Repository_RemoveAsync_RemovesEntity()
    {
        // Arrange
        var subject = await _subjectRepository.GetSingleAsync(s => s.Code == "MATH101");
        Assert.NotNull(subject);

        // Act
        await _subjectRepository.RemoveAsync(subject);
        await _context.SaveChangesAsync();

        // Assert
        var deleted = await _subjectRepository.GetSingleAsync(s => s.Code == "MATH101");
        Assert.Null(deleted);
    }

    [Fact]
    public async Task Repository_ExistsAsync_ReturnsTrueForExistingEntity()
    {
        // Act
        var exists = await _subjectRepository.ExistsAsync(s => s.Code == "CS101");

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task Repository_ExistsAsync_ReturnsFalseForNonExistingEntity()
    {
        // Act
        var exists = await _subjectRepository.ExistsAsync(s => s.Code == "NONEXIST");

        // Assert
        Assert.False(exists);
    }

    [Fact]
    public async Task Repository_CountAsync_ReturnsCorrectCount()
    {
        // Act
        var count = await _subjectRepository.CountAsync();

        // Assert
        Assert.Equal(3, count);
    }

    [Fact]
    public async Task Repository_GetPagedAsync_ReturnsPagedResults()
    {
        // Act
        var paged = await _subjectRepository.GetPagedAsync(1, 2);

        // Assert
        Assert.NotNull(paged);
        Assert.Equal(2, paged.Count());
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}