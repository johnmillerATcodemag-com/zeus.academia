using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Repositories;
using Zeus.Academia.Infrastructure.Repositories.Interfaces;
using Xunit;

namespace Zeus.Academia.Tests;

/// <summary>
/// Basic validation tests for Task 6 repository pattern implementation.
/// These tests verify the core functionality works correctly.
/// </summary>
public class Task6ValidationTests : IDisposable
{
    private readonly AcademiaDbContext _context;
    private readonly Mock<ILogger<Repository<Department>>> _mockRepositoryLogger;
    private readonly Mock<ILogger<DepartmentRepository>> _mockDepartmentLogger;
    private readonly Mock<ILogger<AcademicRepository>> _mockAcademicLogger;
    private readonly Mock<ILogger<SubjectRepository>> _mockSubjectLogger;
    private readonly Mock<ILogger<UnitOfWork>> _mockUnitOfWorkLogger;

    public Task6ValidationTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<AcademiaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var mockConfiguration = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
        _context = new AcademiaDbContext(options, mockConfiguration.Object);

        // Setup mocks
        _mockRepositoryLogger = new Mock<ILogger<Repository<Department>>>();
        _mockDepartmentLogger = new Mock<ILogger<DepartmentRepository>>();
        _mockAcademicLogger = new Mock<ILogger<AcademicRepository>>();
        _mockSubjectLogger = new Mock<ILogger<SubjectRepository>>();
        _mockUnitOfWorkLogger = new Mock<ILogger<UnitOfWork>>();

        // Seed test data
        SeedTestData();
    }

    private void SeedTestData()
    {
        var department = new Department
        {
            Name = "Computer Science",
            FullName = "Department of Computer Science",
            Description = "Computer Science Department",
            Budget = 1000000m
        };

        var subject = new Subject
        {
            Code = "CS101",
            Title = "Introduction to Programming",
            CreditHours = 3
        };

        _context.Departments.Add(department);
        _context.Subjects.Add(subject);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GenericRepository_CRUD_Operations_Should_Work()
    {
        // Arrange
        var repository = new Repository<Department>(_context, _mockRepositoryLogger.Object);

        // Act & Assert - Read
        var departments = await repository.GetAllAsync();
        Assert.NotEmpty(departments);

        var department = departments.First();
        Assert.Equal("Computer Science", department.Name);

        // Act & Assert - Get by Name (primary key)
        var departmentByName = await repository.FindAsync(d => d.Name == department.Name);
        var departmentById = departmentByName.FirstOrDefault();
        Assert.NotNull(departmentById);
        Assert.Equal("Computer Science", departmentById.Name);

        // Act & Assert - Add
        var newDepartment = new Department
        {
            Name = "Mathematics",
            FullName = "Department of Mathematics",
            Description = "Mathematics Department",
            Budget = 800000m
        };

        await repository.AddAsync(newDepartment);
        await _context.SaveChangesAsync();

        var allDepartments = await repository.GetAllAsync();
        Assert.Equal(2, allDepartments.Count());

        // Act & Assert - Update
        newDepartment.Budget = 900000m;
        await repository.UpdateAsync(newDepartment);
        await _context.SaveChangesAsync();

        var updatedDepartments = await repository.FindAsync(d => d.Name == newDepartment.Name);
        var updatedDepartment = updatedDepartments.FirstOrDefault();
        Assert.NotNull(updatedDepartment);
        Assert.Equal(900000m, updatedDepartment.Budget);

        // Act & Assert - Delete
        await repository.RemoveAsync(newDepartment);
        await _context.SaveChangesAsync();

        var departmentsAfterDelete = await repository.GetAllAsync();
        Assert.Single(departmentsAfterDelete);
    }

    [Fact]
    public async Task SpecificRepositories_Should_Work()
    {
        // Arrange
        var departmentRepo = new DepartmentRepository(_context, _mockDepartmentLogger.Object);
        var academicRepo = new AcademicRepository(_context, _mockAcademicLogger.Object);
        var subjectRepo = new SubjectRepository(_context, _mockSubjectLogger.Object);

        // Act & Assert - Department Repository
        var departments = await departmentRepo.GetAllAsync();
        Assert.NotEmpty(departments);

        // Act & Assert - Subject Repository  
        var subjects = await subjectRepo.GetAllAsync();
        Assert.NotEmpty(subjects);

        var subject = subjects.First();
        Assert.Equal("CS101", subject.Code);
        Assert.Equal("Introduction to Programming", subject.Title);

        // Act & Assert - Academic Repository (empty but should not throw)
        var academics = await academicRepo.GetAllAsync();
        Assert.Empty(academics); // No academics in seed data
    }

    [Fact]
    public async Task UnitOfWork_Should_Work()
    {
        // Arrange
        var academicRepo = new AcademicRepository(_context, _mockAcademicLogger.Object);
        var departmentRepo = new DepartmentRepository(_context, _mockDepartmentLogger.Object);
        var subjectRepo = new SubjectRepository(_context, _mockSubjectLogger.Object);

        var mockUserRepository = Mock.Of<IUserRepository>();
        var mockRoleRepository = Mock.Of<IRoleRepository>();
        var mockRefreshTokenRepository = Mock.Of<IRefreshTokenRepository>();

        var unitOfWork = new UnitOfWork(_context, _mockUnitOfWorkLogger.Object,
            academicRepo, departmentRepo, subjectRepo,
            mockUserRepository, mockRoleRepository, mockRefreshTokenRepository);

        // Act & Assert - Repository access
        Assert.NotNull(unitOfWork.Academics);
        Assert.NotNull(unitOfWork.Departments);
        Assert.NotNull(unitOfWork.Subjects);

        // Act & Assert - Save changes
        var newDepartment = new Department
        {
            Name = "Physics",
            FullName = "Department of Physics",
            Description = "Physics Department",
            Budget = 750000m
        };

        await unitOfWork.Departments.AddAsync(newDepartment);
        var result = await unitOfWork.SaveChangesAsync();
        Assert.True(result > 0);

        // Verify it was saved
        var physics = (await unitOfWork.Departments.FindAsync(d => d.Name == "Physics")).FirstOrDefault();
        Assert.NotNull(physics);
        Assert.Equal("Department of Physics", physics.FullName);
    }

    [Fact]
    public async Task Repository_FindAsync_Should_Work()
    {
        // Arrange
        var repository = new Repository<Department>(_context, _mockRepositoryLogger.Object);

        // Act
        var departments = await repository.FindAsync(d => d.Name == "Computer Science");
        var department = departments.FirstOrDefault();

        // Assert
        Assert.NotNull(department);
        Assert.Equal("Computer Science", department.Name);
        Assert.Equal("Department of Computer Science", department.FullName);
    }

    [Fact]
    public async Task Repository_GetPagedAsync_Should_Work()
    {
        // Arrange
        var repository = new Repository<Department>(_context, _mockRepositoryLogger.Object);

        // Add more departments for paging test
        await repository.AddAsync(new Department { Name = "Math", FullName = "Mathematics", Budget = 600000m });
        await repository.AddAsync(new Department { Name = "Physics", FullName = "Physics Dept", Budget = 700000m });
        await _context.SaveChangesAsync();

        // Act
        var pagedResult = await repository.GetPagedAsync(1, 2);

        // Assert
        Assert.NotNull(pagedResult);
        Assert.Equal(2, pagedResult.Count());
    }

    [Fact]
    public async Task Repository_CountAsync_Should_Work()
    {
        // Arrange
        var repository = new Repository<Department>(_context, _mockRepositoryLogger.Object);

        // Act
        var count = await repository.CountAsync();

        // Assert
        Assert.True(count > 0);
    }

    [Fact]
    public async Task Repository_ExistsAsync_Should_Work()
    {
        // Arrange
        var repository = new Repository<Department>(_context, _mockRepositoryLogger.Object);

        // Act
        var exists = await repository.ExistsAsync(d => d.Name == "Computer Science");
        var notExists = await repository.ExistsAsync(d => d.Name == "NonExistent");

        // Assert
        Assert.True(exists);
        Assert.False(notExists);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}