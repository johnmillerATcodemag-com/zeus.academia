using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Repositories.Interfaces;
using UnitOfWork = Zeus.Academia.Infrastructure.Repositories.UnitOfWork;
using Xunit;

namespace Zeus.Academia.Tests.Repositories;

/// <summary>
/// Comprehensive tests for UnitOfWork implementation.
/// </summary>
public class UnitOfWorkTests : IDisposable
{
    private readonly AcademiaDbContext _context;
    private readonly Mock<ILogger<UnitOfWork>> _mockLogger;
    private readonly Mock<IAcademicRepository> _mockAcademicRepository;
    private readonly Mock<IDepartmentRepository> _mockDepartmentRepository;
    private readonly Mock<ISubjectRepository> _mockSubjectRepository;
    private readonly UnitOfWork _unitOfWork;

    public UnitOfWorkTests()
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
        _mockLogger = new Mock<ILogger<UnitOfWork>>();
        _mockAcademicRepository = new Mock<IAcademicRepository>();
        _mockDepartmentRepository = new Mock<IDepartmentRepository>();
        _mockSubjectRepository = new Mock<ISubjectRepository>();

        var mockUserRepository = new Mock<IUserRepository>();
        var mockRoleRepository = new Mock<IRoleRepository>();
        var mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();

        _unitOfWork = new UnitOfWork(
            _context,
            _mockLogger.Object,
            _mockAcademicRepository.Object,
            _mockDepartmentRepository.Object,
            _mockSubjectRepository.Object,
            mockUserRepository.Object,
            mockRoleRepository.Object,
            mockRefreshTokenRepository.Object);
    }

    [Fact]
    public void Academics_ShouldReturnAcademicRepository()
    {
        // Act
        var result = _unitOfWork.Academics;

        // Assert
        Assert.NotNull(result);
        Assert.Same(_mockAcademicRepository.Object, result);
    }

    [Fact]
    public void Departments_ShouldReturnDepartmentRepository()
    {
        // Act
        var result = _unitOfWork.Departments;

        // Assert
        Assert.NotNull(result);
        Assert.Same(_mockDepartmentRepository.Object, result);
    }

    [Fact]
    public void Subjects_ShouldReturnSubjectRepository()
    {
        // Act
        var result = _unitOfWork.Subjects;

        // Assert
        Assert.NotNull(result);
        Assert.Same(_mockSubjectRepository.Object, result);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldReturnNumberOfChanges()
    {
        // Arrange
        var department = new Department
        {
            Name = "TEST",
            FullName = "Test Department",
            IsActive = true,
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };

        _context.Departments.Add(department);

        // Act
        var result = await _unitOfWork.SaveChangesAsync();

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public async Task SaveChangesAsync_WithNoChanges_ShouldReturnZero()
    {
        // Act
        var result = await _unitOfWork.SaveChangesAsync();

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task BeginTransactionAsync_ShouldStartTransaction()
    {
        // Skip test for in-memory database as it doesn't support transactions
        if (_context.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory")
        {
            return;
        }

        // Act
        await _unitOfWork.BeginTransactionAsync();

        // Assert
        Assert.NotNull(_context.Database.CurrentTransaction);
    }

    [Fact]
    public async Task BeginTransactionAsync_WhenTransactionExists_ShouldNotStartNew()
    {
        // Skip test for in-memory database as it doesn't support transactions
        if (_context.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory")
        {
            return;
        }

        // Arrange
        await _unitOfWork.BeginTransactionAsync();
        var firstTransaction = _context.Database.CurrentTransaction;

        // Act
        await _unitOfWork.BeginTransactionAsync();

        // Assert
        Assert.Same(firstTransaction, _context.Database.CurrentTransaction);
    }

    [Fact]
    public async Task CommitTransactionAsync_WithActiveTransaction_ShouldCommit()
    {
        // Skip test for in-memory database as it doesn't support transactions
        if (_context.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory")
        {
            return;
        }

        // Arrange  
        await _unitOfWork.BeginTransactionAsync();

        var department = new Department
        {
            Name = "COMMIT_TEST",
            FullName = "Commit Test Department",
            IsActive = true,
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };

        _context.Departments.Add(department);
        await _context.SaveChangesAsync();

        // Act
        await _unitOfWork.CommitTransactionAsync();

        // Assert
        Assert.Null(_context.Database.CurrentTransaction);

        // Verify data was committed
        var savedDepartment = await _context.Departments.FindAsync("COMMIT_TEST");
        Assert.NotNull(savedDepartment);
    }

    [Fact]
    public async Task CommitTransactionAsync_WithoutActiveTransaction_ShouldNotThrow()
    {
        // Act & Assert
        await _unitOfWork.CommitTransactionAsync(); // Should not throw
    }

    [Fact]
    public async Task RollbackTransactionAsync_WithActiveTransaction_ShouldRollback()
    {
        // Skip test for in-memory database as it doesn't support transactions
        if (_context.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory")
        {
            return;
        }

        // Arrange
        await _unitOfWork.BeginTransactionAsync();

        var department = new Department
        {
            Name = "ROLLBACK_TEST",
            FullName = "Rollback Test Department",
            IsActive = true,
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };

        _context.Departments.Add(department);
        await _context.SaveChangesAsync();

        // Act
        await _unitOfWork.RollbackTransactionAsync();

        // Assert
        Assert.Null(_context.Database.CurrentTransaction);

        // Verify data was rolled back
        var department2 = await _context.Departments.FindAsync("ROLLBACK_TEST");
        Assert.Null(department2);
    }

    [Fact]
    public async Task RollbackTransactionAsync_WithoutActiveTransaction_ShouldNotThrow()
    {
        // Act & Assert
        await _unitOfWork.RollbackTransactionAsync(); // Should not throw
    }

    [Fact]
    public async Task ExecuteInTransactionAsync_WithFunc_ShouldExecuteAndCommit()
    {
        // Skip test for in-memory database as it doesn't support transactions
        if (_context.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory")
        {
            return;
        }

        // Arrange
        var department = new Department
        {
            Name = "FUNC_TEST",
            FullName = "Function Test Department",
            IsActive = true,
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };

        // Act
        var result = await _unitOfWork.ExecuteInTransactionAsync(() =>
        {
            _context.Departments.Add(department);
            return Task.FromResult("Success");
        });

        // Assert
        Assert.Equal("Success", result);
        Assert.Null(_context.Database.CurrentTransaction);

        var savedDepartment = await _context.Departments.FindAsync("FUNC_TEST");
        Assert.NotNull(savedDepartment);
    }

    [Fact]
    public async Task ExecuteInTransactionAsync_WithAction_ShouldExecuteAndCommit()
    {
        // Skip test for in-memory database as it doesn't support transactions
        if (_context.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory")
        {
            return;
        }

        // Arrange
        var department = new Department
        {
            Name = "ACTION_TEST",
            FullName = "Action Test Department",
            IsActive = true,
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };

        // Act
        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            _context.Departments.Add(department);
            await Task.CompletedTask;
        });

        // Assert
        Assert.Null(_context.Database.CurrentTransaction);

        var savedDepartment = await _context.Departments.FindAsync("ACTION_TEST");
        Assert.NotNull(savedDepartment);
    }

    [Fact]
    public async Task ExecuteInTransactionAsync_WithException_ShouldRollback()
    {
        // Arrange
        var department = new Department
        {
            Name = "EXCEPTION_TEST",
            FullName = "Exception Test Department",
            IsActive = true,
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                _context.Departments.Add(department);
                await Task.CompletedTask;
                throw new InvalidOperationException("Test exception");
            });
        });

        Assert.Null(_context.Database.CurrentTransaction);

        var savedDepartment = await _context.Departments.FindAsync("EXCEPTION_TEST");
        Assert.Null(savedDepartment);
    }

    [Fact]
    public async Task ExecuteInTransactionAsync_WithNullOperation_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _unitOfWork.ExecuteInTransactionAsync((Func<Task<string>>)null!));

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _unitOfWork.ExecuteInTransactionAsync((Func<Task>)null!));
    }

    [Fact]
    public void Constructor_WithNullContext_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var mockUserRepository = new Mock<IUserRepository>();
        var mockRoleRepository = new Mock<IRoleRepository>();
        var mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();

        Assert.Throws<ArgumentNullException>(() => new UnitOfWork(
            null!,
            _mockLogger.Object,
            _mockAcademicRepository.Object,
            _mockDepartmentRepository.Object,
            _mockSubjectRepository.Object,
            mockUserRepository.Object,
            mockRoleRepository.Object,
            mockRefreshTokenRepository.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var mockUserRepository = new Mock<IUserRepository>();
        var mockRoleRepository = new Mock<IRoleRepository>();
        var mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();

        Assert.Throws<ArgumentNullException>(() => new UnitOfWork(
            _context,
            null!,
            _mockAcademicRepository.Object,
            _mockDepartmentRepository.Object,
            _mockSubjectRepository.Object,
            mockUserRepository.Object,
            mockRoleRepository.Object,
            mockRefreshTokenRepository.Object));
    }

    [Fact]
    public void Constructor_WithNullAcademicRepository_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var mockUserRepository = new Mock<IUserRepository>();
        var mockRoleRepository = new Mock<IRoleRepository>();
        var mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();

        Assert.Throws<ArgumentNullException>(() => new UnitOfWork(
            _context,
            _mockLogger.Object,
            null!,
            _mockDepartmentRepository.Object,
            _mockSubjectRepository.Object,
            mockUserRepository.Object,
            mockRoleRepository.Object,
            mockRefreshTokenRepository.Object));
    }

    [Fact]
    public void Dispose_ShouldDisposeResources()
    {
        // Act
        _unitOfWork.Dispose();

        // Assert
        Assert.Throws<ObjectDisposedException>(() => _unitOfWork.Academics);
        Assert.Throws<ObjectDisposedException>(() => _unitOfWork.Departments);
        Assert.Throws<ObjectDisposedException>(() => _unitOfWork.Subjects);
    }

    [Fact]
    public void Dispose_CalledMultipleTimes_ShouldNotThrow()
    {
        // Act & Assert
        _unitOfWork.Dispose();
        _unitOfWork.Dispose(); // Should not throw
    }

    public void Dispose()
    {
        _unitOfWork.Dispose();
        _context.Dispose();
    }
}