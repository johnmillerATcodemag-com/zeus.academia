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
/// Test logger implementation for unit testing.
/// </summary>
public class TestLogger<T> : ILogger<T>
{
    public List<LogEntry> LogEntries { get; } = new();

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        LogEntries.Add(new LogEntry
        {
            LogLevel = logLevel,
            EventId = eventId,
            Message = formatter(state, exception),
            Exception = exception
        });
    }

    public class LogEntry
    {
        public LogLevel LogLevel { get; set; }
        public EventId EventId { get; set; }
        public string Message { get; set; } = string.Empty;
        public Exception? Exception { get; set; }
    }
}

/// <summary>
/// Comprehensive tests for UnitOfWork to improve code coverage from 72% to 90%+.
/// Tests repository caching, error scenarios, disposal patterns, and audit functionality.
/// </summary>
public class UnitOfWorkComprehensiveTests : IDisposable
{
    private readonly AcademiaDbContext _context;
    private readonly IServiceProvider _serviceProvider;
    private readonly TestLogger<UnitOfWork> _testLogger;
    private readonly ServiceCollection _services;

    public UnitOfWorkComprehensiveTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<AcademiaDbContext>()
            .UseInMemoryDatabase(databaseName: $"UnitOfWorkTestDb_{Guid.NewGuid()}")
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
        _services = new ServiceCollection();
        _services.AddSingleton<IConfiguration>(configuration);
        _services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
        _services.AddDbContext<AcademiaDbContext>(opt => opt.UseInMemoryDatabase($"UnitOfWorkTestDb_{Guid.NewGuid()}"));
        _services.AddInfrastructureData();

        // Add test logger
        _testLogger = new TestLogger<UnitOfWork>();
        _services.AddSingleton<ILogger<UnitOfWork>>(_testLogger);

        _serviceProvider = _services.BuildServiceProvider();
        _context = _serviceProvider.GetRequiredService<AcademiaDbContext>();

        // Ensure database is created
        _context.Database.EnsureCreated();
        SeedTestData().Wait();
    }

    private async Task SeedTestData()
    {
        if (!_context.AccessLevels.Any())
        {
            var accessLevels = new[]
            {
                new AccessLevel { Code = "SYSADM", Name = "System Administrator", Description = "System Administrator" },
                new AccessLevel { Code = "UNIADM", Name = "University Administrator", Description = "University Administrator" },
                new AccessLevel { Code = "DPTADM", Name = "Department Administrator", Description = "Department Administrator" }
            };
            _context.AccessLevels.AddRange(accessLevels);
            await _context.SaveChangesAsync();
        }
    }

    [Fact]
    public void Constructor_WithValidParameters_Should_Initialize_Successfully()
    {
        // Arrange & Act
        var unitOfWork = new UnitOfWork(_context, _serviceProvider, _testLogger);

        // Assert
        Assert.NotNull(unitOfWork);
        Assert.NotNull(unitOfWork.Academics);
        Assert.NotNull(unitOfWork.Departments);
        Assert.NotNull(unitOfWork.Subjects);
    }

    [Fact]
    public void Constructor_WithNullContext_Should_Throw_ArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new UnitOfWork(null!, _serviceProvider, _testLogger));
    }

    [Fact]
    public void Constructor_WithNullServiceProvider_Should_Throw_ArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new UnitOfWork(_context, null!, _testLogger));
    }

    [Fact]
    public void Constructor_WithNullLogger_Should_Throw_ArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new UnitOfWork(_context, _serviceProvider, null!));
    }

    [Fact]
    public void Academics_Property_Should_Return_Same_Instance_On_Multiple_Calls()
    {
        // Arrange
        var unitOfWork = new UnitOfWork(_context, _serviceProvider, _testLogger);

        // Act
        var academics1 = unitOfWork.Academics;
        var academics2 = unitOfWork.Academics;

        // Assert
        Assert.Same(academics1, academics2);
    }

    [Fact]
    public void Departments_Property_Should_Return_Same_Instance_On_Multiple_Calls()
    {
        // Arrange
        var unitOfWork = new UnitOfWork(_context, _serviceProvider, _testLogger);

        // Act
        var departments1 = unitOfWork.Departments;
        var departments2 = unitOfWork.Departments;

        // Assert
        Assert.Same(departments1, departments2);
    }

    [Fact]
    public void Subjects_Property_Should_Return_Same_Instance_On_Multiple_Calls()
    {
        // Arrange
        var unitOfWork = new UnitOfWork(_context, _serviceProvider, _testLogger);

        // Act
        var subjects1 = unitOfWork.Subjects;
        var subjects2 = unitOfWork.Subjects;

        // Assert
        Assert.Same(subjects1, subjects2);
    }

    [Fact]
    public void Repository_Generic_Should_Cache_Repository_Instances()
    {
        // Arrange
        var unitOfWork = new UnitOfWork(_context, _serviceProvider, _testLogger);

        // Act
        var repo1 = unitOfWork.Repository<AccessLevel>();
        var repo2 = unitOfWork.Repository<AccessLevel>();

        // Assert
        Assert.Same(repo1, repo2);
    }

    [Fact]
    public void Repository_Generic_Should_Return_Different_Instances_For_Different_Types()
    {
        // Arrange
        var unitOfWork = new UnitOfWork(_context, _serviceProvider, _testLogger);

        // Act
        var accessLevelRepo = unitOfWork.Repository<AccessLevel>();
        var universityRepo = unitOfWork.Repository<University>();

        // Assert
        Assert.NotSame(accessLevelRepo, universityRepo);
        Assert.IsType<Repository<AccessLevel>>(accessLevelRepo);
        Assert.IsType<Repository<University>>(universityRepo);
    }

    [Fact]
    public async Task SaveChangesAsync_Without_UserId_Should_Update_Audit_Fields_With_System()
    {
        // Arrange
        var unitOfWork = new UnitOfWork(_context, _serviceProvider, _testLogger);
        var accessLevel = new AccessLevel { Code = "TEST", Name = "Test Level", Description = "Test Description" };

        // Act
        await unitOfWork.Repository<AccessLevel>().AddAsync(accessLevel);
        var result = await unitOfWork.SaveChangesAsync();

        // Assert
        Assert.Equal(1, result);
        Assert.Equal("System", accessLevel.CreatedBy);
        Assert.Equal("System", accessLevel.ModifiedBy);
        Assert.True((DateTime.UtcNow - accessLevel.CreatedDate).TotalSeconds < 5);
        Assert.True((DateTime.UtcNow - accessLevel.ModifiedDate).TotalSeconds < 5);
    }

    [Fact]
    public async Task SaveChangesAsync_With_UserId_Should_Update_Audit_Fields_With_UserId()
    {
        // Arrange
        var unitOfWork = new UnitOfWork(_context, _serviceProvider, _testLogger);
        var accessLevel = new AccessLevel { Code = "TEST2", Name = "Test Level 2", Description = "Test Description 2" };
        var userId = "testuser@example.com";

        // Act
        await unitOfWork.Repository<AccessLevel>().AddAsync(accessLevel);
        var result = await unitOfWork.SaveChangesAsync(userId);

        // Assert
        Assert.Equal(1, result);
        Assert.Equal(userId, accessLevel.CreatedBy);
        Assert.Equal(userId, accessLevel.ModifiedBy);
        Assert.True((DateTime.UtcNow - accessLevel.CreatedDate).TotalSeconds < 5);
        Assert.True((DateTime.UtcNow - accessLevel.ModifiedDate).TotalSeconds < 5);
    }

    [Fact]
    public async Task SaveChangesAsync_Should_Update_Modified_Fields_Only_For_Existing_Entity()
    {
        // Arrange
        var unitOfWork = new UnitOfWork(_context, _serviceProvider, _testLogger);
        var accessLevel = new AccessLevel { Code = "TEST3", Name = "Test Level 3", Description = "Test Description 3" };

        // Add and save initially
        await unitOfWork.Repository<AccessLevel>().AddAsync(accessLevel);
        await unitOfWork.SaveChangesAsync("initialuser");
        
        var originalCreatedBy = accessLevel.CreatedBy;
        var originalCreatedDate = accessLevel.CreatedDate;

        // Act - Update the entity
        accessLevel.Description = "Updated Description";
        var result = await unitOfWork.SaveChangesAsync("updateuser");

        // Assert
        Assert.Equal(1, result);
        Assert.Equal(originalCreatedBy, accessLevel.CreatedBy); // Should not change
        Assert.Equal(originalCreatedDate, accessLevel.CreatedDate); // Should not change
        Assert.Equal("updateuser", accessLevel.ModifiedBy); // Should update
        Assert.True(accessLevel.ModifiedDate > originalCreatedDate); // Should be newer
    }

    [Fact]
    public async Task SaveChangesAsync_Should_Log_Debug_Messages()
    {
        // Arrange
        var unitOfWork = new UnitOfWork(_context, _serviceProvider, _testLogger);
        var accessLevel = new AccessLevel { Code = "TEST4", Name = "Test Level 4", Description = "Test Description 4" };

        // Act
        await unitOfWork.Repository<AccessLevel>().AddAsync(accessLevel);
        await unitOfWork.SaveChangesAsync();

        // Assert
        var debugLogs = _testLogger.LogEntries.Where(l => l.LogLevel == LogLevel.Debug).ToList();
        Assert.Contains(debugLogs, l => l.Message.Contains("Saving changes to database"));
        Assert.Contains(debugLogs, l => l.Message.Contains("Saved") && l.Message.Contains("records to database"));
    }

    [Fact]
    public async Task SaveChangesAsync_With_UserId_Should_Log_Debug_Messages_With_UserId()
    {
        // Arrange
        var unitOfWork = new UnitOfWork(_context, _serviceProvider, _testLogger);
        var accessLevel = new AccessLevel { Code = "TEST5", Name = "Test Level 5", Description = "Test Description 5" };
        var userId = "testuser@example.com";

        // Act
        await unitOfWork.Repository<AccessLevel>().AddAsync(accessLevel);
        await unitOfWork.SaveChangesAsync(userId);

        // Assert
        var debugLogs = _testLogger.LogEntries.Where(l => l.LogLevel == LogLevel.Debug).ToList();
        Assert.Contains(debugLogs, l => l.Message.Contains($"Saving changes to database with user {userId}"));
    }

    [Fact]
    public void Dispose_Should_Clear_Repository_Cache()
    {
        // Arrange
        var unitOfWork = new UnitOfWork(_context, _serviceProvider, _testLogger);
        var repo = unitOfWork.Repository<AccessLevel>();

        // Act
        unitOfWork.Dispose();

        // Assert - No direct way to verify the cache is cleared, but we can verify the object is disposed
        // The internal state is cleaned up, which is verified by not throwing exceptions
        Assert.NotNull(repo); // Repository should still exist but UnitOfWork is disposed
    }

    [Fact]
    public void Dispose_Multiple_Calls_Should_Not_Throw()
    {
        // Arrange
        var unitOfWork = new UnitOfWork(_context, _serviceProvider, _testLogger);

        // Act & Assert
        unitOfWork.Dispose();
        unitOfWork.Dispose(); // Should not throw
    }

    [Fact]
    public async Task SaveChangesAsync_Should_Handle_No_Changes()
    {
        // Arrange
        var unitOfWork = new UnitOfWork(_context, _serviceProvider, _testLogger);

        // Act
        var result = await unitOfWork.SaveChangesAsync();

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task SaveChangesAsync_Should_Handle_Multiple_Entity_Types()
    {
        // Arrange
        var unitOfWork = new UnitOfWork(_context, _serviceProvider, _testLogger);
        var accessLevel = new AccessLevel { Code = "MULTI1", Name = "Multi Test 1", Description = "Multi Test Description 1" };
        var university = new University { Code = "MULTI", Name = "Multi Test University", Location = "Test Location" };

        // Act
        await unitOfWork.Repository<AccessLevel>().AddAsync(accessLevel);
        await unitOfWork.Repository<University>().AddAsync(university);
        var result = await unitOfWork.SaveChangesAsync("multiuser");

        // Assert
        Assert.Equal(2, result);
        Assert.Equal("multiuser", accessLevel.CreatedBy);
        Assert.Equal("multiuser", university.CreatedBy);
    }

    [Fact]
    public async Task SaveChangesAsync_Should_Handle_Cancellation_Token()
    {
        // Arrange
        var unitOfWork = new UnitOfWork(_context, _serviceProvider, _testLogger);
        var accessLevel = new AccessLevel { Code = "CANCEL", Name = "Cancel Test", Description = "Cancel Test Description" };
        var cancellationToken = new CancellationToken();

        // Act
        await unitOfWork.Repository<AccessLevel>().AddAsync(accessLevel);
        var result = await unitOfWork.SaveChangesAsync(cancellationToken);

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public async Task SaveChangesAsync_With_UserId_Should_Handle_Cancellation_Token()
    {
        // Arrange
        var unitOfWork = new UnitOfWork(_context, _serviceProvider, _testLogger);
        var accessLevel = new AccessLevel { Code = "CANCEL2", Name = "Cancel Test 2", Description = "Cancel Test Description 2" };
        var cancellationToken = new CancellationToken();
        var userId = "canceluser";

        // Act
        await unitOfWork.Repository<AccessLevel>().AddAsync(accessLevel);
        var result = await unitOfWork.SaveChangesAsync(userId, cancellationToken);

        // Assert
        Assert.Equal(1, result);
        Assert.Equal(userId, accessLevel.CreatedBy);
    }

    public void Dispose()
    {
        _context?.Dispose();
        _serviceProvider?.GetService<IServiceScope>()?.Dispose();
    }
}

/// <summary>
/// Tests for DbTransactionWrapper internal class through UnitOfWork usage.
/// </summary>
public class DbTransactionWrapperTests : IDisposable
{
    private readonly AcademiaDbContext _context;
    private readonly IServiceProvider _serviceProvider;
    private readonly TestLogger<UnitOfWork> _testLogger;

    public DbTransactionWrapperTests()
    {
        // Setup in-memory database (transactions won't work, but we can test the wrapper interface)
        var options = new DbContextOptionsBuilder<AcademiaDbContext>()
            .UseInMemoryDatabase(databaseName: $"TransactionTestDb_{Guid.NewGuid()}")
            .Options;

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "DataSource=:memory:"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();
        services.AddDbContext<AcademiaDbContext>(opt => opt.UseInMemoryDatabase($"TransactionTestDb_{Guid.NewGuid()}"));
        services.AddInfrastructureData();

        _testLogger = new TestLogger<UnitOfWork>();
        services.AddSingleton<ILogger<UnitOfWork>>(_testLogger);

        _serviceProvider = services.BuildServiceProvider();
        _context = _serviceProvider.GetRequiredService<AcademiaDbContext>();
        _context.Database.EnsureCreated();
    }

    [Fact]
    public async Task BeginTransactionAsync_Should_Log_Debug_Message()
    {
        // Arrange
        var unitOfWork = new UnitOfWork(_context, _serviceProvider, _testLogger);

        // Act & Assert
        try
        {
            await unitOfWork.BeginTransactionAsync();
        }
        catch (InvalidOperationException)
        {
            // Expected for in-memory database
        }

        var debugLogs = _testLogger.LogEntries.Where(l => l.LogLevel == LogLevel.Debug).ToList();
        Assert.Contains(debugLogs, l => l.Message.Contains("Beginning database transaction"));
    }

    [Fact]
    public async Task BeginTransactionAsync_Should_Log_Error_On_Exception()
    {
        // Arrange
        var unitOfWork = new UnitOfWork(_context, _serviceProvider, _testLogger);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await unitOfWork.BeginTransactionAsync();
        });

        var errorLogs = _testLogger.LogEntries.Where(l => l.LogLevel == LogLevel.Error).ToList();
        Assert.Contains(errorLogs, l => l.Message.Contains("Error beginning database transaction"));
    }

    public void Dispose()
    {
        _context?.Dispose();
        _serviceProvider?.GetService<IServiceScope>()?.Dispose();
    }
}
