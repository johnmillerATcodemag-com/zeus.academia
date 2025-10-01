using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.Tests.Infrastructure;

/// <summary>
/// Unit tests for database migration functionality.
/// Task 5: Database Migrations - Test coverage for migration operations and database initialization.
/// </summary>
public class DatabaseMigrationTests : IDisposable
{
    private readonly AcademiaDbContext _context;
    private readonly DatabaseInitializer _initializer;
    private readonly ILogger<DatabaseInitializer> _logger;
    private bool _disposed = false;

    public DatabaseMigrationTests()
    {
        // Create in-memory database for testing
        var options = new DbContextOptionsBuilder<AcademiaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        // Create mock configuration
        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = "Data Source=:memory:"
        });
        var configuration = configurationBuilder.Build();

        // Create context and initializer
        _context = new AcademiaDbContext(options, configuration);
        
        // Create logger
        var serviceProvider = new ServiceCollection()
            .AddLogging(builder => builder.AddConsole())
            .BuildServiceProvider();
        _logger = serviceProvider.GetRequiredService<ILogger<DatabaseInitializer>>();
        
        _initializer = new DatabaseInitializer(_context, _logger);
    }

    [Fact]
    public async Task DatabaseInitializer_Should_Connect_Successfully()
    {
        // Act
        var canConnect = await _initializer.CanConnectAsync();

        // Assert
        Assert.True(canConnect);
    }

    [Fact]
    public async Task DatabaseInitializer_Should_Initialize_Successfully()
    {
        // Act
        var result = await _initializer.InitializeAsync(applyMigrations: false, seedData: true);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DatabaseInitializer_Should_Validate_Schema_Successfully()
    {
        // Arrange
        await _initializer.InitializeAsync(applyMigrations: false, seedData: false);

        // Act
        var isValid = await _initializer.ValidateSchemaAsync();

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public async Task DatabaseSeeder_Should_Seed_AccessLevels()
    {
        // Act
        await DatabaseSeeder.SeedAsync(_context);

        // Assert
        var accessLevels = await _context.AccessLevels.ToListAsync();
        Assert.True(accessLevels.Count >= 5);
        Assert.Contains(accessLevels, al => al.Code == "ADMIN");
        Assert.Contains(accessLevels, al => al.Code == "FACULT1");
        Assert.Contains(accessLevels, al => al.Code == "STUDENT");
        Assert.Contains(accessLevels, al => al.Code == "STAFF");
        Assert.Contains(accessLevels, al => al.Code == "GUEST");
    }

    [Fact]
    public async Task DatabaseSeeder_Should_Seed_Universities()
    {
        // Act
        await DatabaseSeeder.SeedAsync(_context);

        // Assert
        var universities = await _context.Universities.ToListAsync();
        Assert.True(universities.Count >= 1);
        Assert.Contains(universities, u => u.Name == "Zeus Academia University");
        Assert.Contains(universities, u => u.Code == "ZAU");
    }

    [Fact]
    public async Task DatabaseSeeder_Should_Seed_Degrees()
    {
        // Act
        await DatabaseSeeder.SeedAsync(_context);

        // Assert
        var degrees = await _context.Degrees.ToListAsync();
        Assert.True(degrees.Count >= 4);
        Assert.Contains(degrees, d => d.Code == "BS-CS");
        Assert.Contains(degrees, d => d.Code == "MS-CS");
        Assert.Contains(degrees, d => d.Code == "PHD-CS");
        Assert.Contains(degrees, d => d.Code == "BS-MATH");
    }

    [Fact]
    public async Task DatabaseSeeder_Should_Seed_Ranks()
    {
        // Act
        await DatabaseSeeder.SeedAsync(_context);

        // Assert
        var ranks = await _context.Ranks.ToListAsync();
        Assert.True(ranks.Count >= 4);
        Assert.Contains(ranks, r => r.Code == "PROF");
        Assert.Contains(ranks, r => r.Code == "ASSOCPROF");
        Assert.Contains(ranks, r => r.Code == "ASSTPROF");
        Assert.Contains(ranks, r => r.Code == "LECTURER");
    }

    [Fact]
    public async Task DatabaseSeeder_Should_Seed_Subjects()
    {
        // Act
        await DatabaseSeeder.SeedAsync(_context);

        // Assert
        var subjects = await _context.Subjects.ToListAsync();
        Assert.True(subjects.Count >= 4);
        Assert.Contains(subjects, s => s.Code == "CS101");
        Assert.Contains(subjects, s => s.Code == "CS201");
        Assert.Contains(subjects, s => s.Code == "MATH101");
        Assert.Contains(subjects, s => s.Code == "MATH201");
    }

    [Fact]
    public async Task DatabaseSeeder_Should_Seed_Buildings()
    {
        // Act
        await DatabaseSeeder.SeedAsync(_context);

        // Assert
        var buildings = await _context.Buildings.ToListAsync();
        Assert.True(buildings.Count >= 3);
        Assert.Contains(buildings, b => b.Code == "ADMIN");
        Assert.Contains(buildings, b => b.Code == "SCI");
        Assert.Contains(buildings, b => b.Code == "LIB");
    }

    [Fact]
    public async Task DatabaseSeeder_Should_Not_Duplicate_Seed_Data()
    {
        // Act - Run seeder twice
        await DatabaseSeeder.SeedAsync(_context);
        var firstCount = await _context.AccessLevels.CountAsync();
        
        await DatabaseSeeder.SeedAsync(_context);
        var secondCount = await _context.AccessLevels.CountAsync();

        // Assert - Count should remain the same
        Assert.Equal(firstCount, secondCount);
    }

    [Fact]
    public async Task DatabaseInfo_Should_Return_Correct_Information()
    {
        // Act
        var dbInfo = await _initializer.GetDatabaseInfoAsync();

        // Assert
        Assert.NotNull(dbInfo);
        Assert.True(dbInfo.CanConnect);
    }

    [Fact]
    public async Task AccessLevel_Seed_Data_Should_Have_Proper_Hierarchy()
    {
        // Act
        await DatabaseSeeder.SeedAsync(_context);
        var accessLevels = await _context.AccessLevels.OrderBy(al => al.Level).ToListAsync();

        // Assert
        var admin = accessLevels.First(al => al.Code == "ADMIN");
        var student = accessLevels.First(al => al.Code == "STUDENT");
        var guest = accessLevels.First(al => al.Code == "GUEST");

        Assert.True(admin.Level < student.Level); // Lower level = higher access
        Assert.True(student.Level < guest.Level);
        Assert.True(admin.CanModifySystem);
        Assert.False(student.CanModifySystem);
        Assert.False(guest.CanModifySystem);
    }

    [Fact]
    public async Task Subject_Prerequisites_Should_Be_Properly_Set()
    {
        // Act
        await DatabaseSeeder.SeedAsync(_context);

        // Assert
        var cs101 = await _context.Subjects.FirstAsync(s => s.Code == "CS101");
        var cs201 = await _context.Subjects.FirstAsync(s => s.Code == "CS201");
        var math101 = await _context.Subjects.FirstAsync(s => s.Code == "MATH101");
        var math201 = await _context.Subjects.FirstAsync(s => s.Code == "MATH201");

        Assert.Null(cs101.Prerequisites); // No prerequisites
        Assert.Equal("CS101", cs201.Prerequisites); // Requires CS101
        Assert.Null(math101.Prerequisites); // No prerequisites
        Assert.Equal("MATH101", math201.Prerequisites); // Requires MATH101
    }

    [Fact]
    public async Task Rank_Salary_Ranges_Should_Be_Logical()
    {
        // Act
        await DatabaseSeeder.SeedAsync(_context);

        // Assert
        var professor = await _context.Ranks.FirstAsync(r => r.Code == "PROF");
        var assocProf = await _context.Ranks.FirstAsync(r => r.Code == "ASSOCPROF");
        var asstProf = await _context.Ranks.FirstAsync(r => r.Code == "ASSTPROF");
        var lecturer = await _context.Ranks.FirstAsync(r => r.Code == "LECTURER");

        // Verify salary hierarchy
        Assert.True(professor.MinSalary >= assocProf.MinSalary);
        Assert.True(assocProf.MinSalary >= asstProf.MinSalary);
        Assert.True(asstProf.MinSalary >= lecturer.MinSalary);

        // Verify salary ranges are valid
        Assert.True(professor.MinSalary <= professor.MaxSalary);
        Assert.True(assocProf.MinSalary <= assocProf.MaxSalary);
        Assert.True(asstProf.MinSalary <= asstProf.MaxSalary);
        Assert.True(lecturer.MinSalary <= lecturer.MaxSalary);
    }

    [Fact]
    public async Task Building_Properties_Should_Be_Valid()
    {
        // Act
        await DatabaseSeeder.SeedAsync(_context);

        // Assert
        var buildings = await _context.Buildings.ToListAsync();
        
        foreach (var building in buildings)
        {
            Assert.NotNull(building.Code);
            Assert.NotNull(building.Name);
            Assert.True(building.IsActive);
            Assert.True(building.IsAccessible);
            
            if (building.NumberOfFloors.HasValue)
            {
                Assert.True(building.NumberOfFloors.Value > 0);
            }
            
            if (building.TotalAreaSqFt.HasValue)
            {
                Assert.True(building.TotalAreaSqFt.Value > 0);
            }
        }
    }

    [Fact]
    public void DatabaseContext_Should_Have_All_Required_DbSets()
    {
        // Assert - Verify all entity DbSets are properly configured
        Assert.NotNull(_context.Academics);
        Assert.NotNull(_context.Professors);
        Assert.NotNull(_context.Teachers);
        Assert.NotNull(_context.TeachingProfs);
        Assert.NotNull(_context.Students);
        Assert.NotNull(_context.Departments);
        Assert.NotNull(_context.Subjects);
        Assert.NotNull(_context.Degrees);
        Assert.NotNull(_context.Universities);
        Assert.NotNull(_context.Ranks);
        Assert.NotNull(_context.Buildings);
        Assert.NotNull(_context.Rooms);
        Assert.NotNull(_context.Extensions);
        Assert.NotNull(_context.AccessLevels);
        Assert.NotNull(_context.Committees);
        Assert.NotNull(_context.AcademicDegrees);
        Assert.NotNull(_context.Teachings);
        Assert.NotNull(_context.CommitteeMembers);
        Assert.NotNull(_context.TeacherRatings);
        Assert.NotNull(_context.Chairs);
        Assert.NotNull(_context.StudentEnrollments);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _context?.Dispose();
            _disposed = true;
        }
    }
}