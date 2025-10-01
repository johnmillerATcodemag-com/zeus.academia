using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Entities;
using Xunit;

namespace Zeus.Academia.Tests.Infrastructure;

/// <summary>
/// Comprehensive tests for DatabaseSeeder static class.
/// Targeting high-impact database seeding functionality with likely zero coverage.
/// </summary>
public class DatabaseSeederComprehensiveTests : IDisposable
{
    private readonly AcademiaDbContext _context;

    public DatabaseSeederComprehensiveTests()
    {
        var options = new DbContextOptionsBuilder<AcademiaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var configuration = new ConfigurationBuilder().Build();
        _context = new AcademiaDbContext(options, configuration);
        _context.Database.EnsureCreated();
    }

    [Fact]
    public async Task SeedAsync_Should_Seed_All_Reference_Data()
    {
        // Act
        await DatabaseSeeder.SeedAsync(_context);

        // Assert - Verify all main entities were seeded
        var universities = await _context.Universities.CountAsync();
        var accessLevels = await _context.AccessLevels.CountAsync();
        var departments = await _context.Departments.CountAsync();
        var degrees = await _context.Degrees.CountAsync();
        var ranks = await _context.Ranks.CountAsync();
        var subjects = await _context.Subjects.CountAsync();
        var buildings = await _context.Buildings.CountAsync();

        Assert.True(universities > 0, "Universities should be seeded");
        Assert.True(accessLevels > 0, "AccessLevels should be seeded");
        Assert.True(departments > 0, "Departments should be seeded");
        Assert.True(degrees > 0, "Degrees should be seeded");
        Assert.True(ranks > 0, "Ranks should be seeded");
        Assert.True(subjects > 0, "Subjects should be seeded");
        Assert.True(buildings > 0, "Buildings should be seeded");
    }

    [Fact]
    public async Task SeedAsync_Should_Seed_Expected_Universities()
    {
        // Act
        await DatabaseSeeder.SeedAsync(_context);

        // Assert
        var universities = await _context.Universities.ToListAsync();
        Assert.Contains(universities, u => u.Name == "Zeus Academia University");
        Assert.Contains(universities, u => u.Code == "ZAU");

        // Verify university properties
        var zau = universities.First(u => u.Code == "ZAU");
        Assert.NotNull(zau.Name);
        Assert.NotNull(zau.CreatedBy);
        Assert.NotNull(zau.ModifiedBy);
        Assert.Equal("System", zau.CreatedBy);
        Assert.Equal("System", zau.ModifiedBy);
    }

    [Fact]
    public async Task SeedAsync_Should_Seed_Expected_AccessLevels()
    {
        // Act
        await DatabaseSeeder.SeedAsync(_context);

        // Assert
        var accessLevels = await _context.AccessLevels.ToListAsync();

        // Verify required access levels exist
        Assert.Contains(accessLevels, al => al.Code == "ADMIN");
        Assert.Contains(accessLevels, al => al.Code == "FACULT1");
        Assert.Contains(accessLevels, al => al.Code == "STUDENT");
        Assert.Contains(accessLevels, al => al.Code == "STAFF");
        Assert.Contains(accessLevels, al => al.Code == "GUEST");

        // Verify admin access level properties
        var admin = accessLevels.First(al => al.Code == "ADMIN");
        Assert.True(admin.CanRead);
        Assert.True(admin.CanCreate);
        Assert.True(admin.CanUpdate);
        Assert.True(admin.CanDelete);
        Assert.Equal("System Administrator", admin.Name);
    }

    [Fact]
    public async Task SeedAsync_Should_Seed_Expected_Departments()
    {
        // Act
        await DatabaseSeeder.SeedAsync(_context);

        // Assert
        var departments = await _context.Departments.ToListAsync();

        // Verify key departments exist
        Assert.Contains(departments, d => d.Name == "COMP-SCI");
        Assert.Contains(departments, d => d.Name == "MATH");
        Assert.Contains(departments, d => d.Name == "PHYSICS");
        Assert.Contains(departments, d => d.Name == "ENG");

        // Verify department properties
        var cs = departments.First(d => d.Name == "COMP-SCI");
        Assert.NotNull(cs.CreatedBy);
        Assert.Equal("System", cs.CreatedBy);
    }

    [Fact]
    public async Task SeedAsync_Should_Seed_Expected_Degrees()
    {
        // Act
        await DatabaseSeeder.SeedAsync(_context);

        // Assert
        var degrees = await _context.Degrees.ToListAsync();

        // Verify common degrees exist
        Assert.Contains(degrees, d => d.Code == "BS-CS" && d.Title == "Bachelor of Science in Computer Science");
        Assert.Contains(degrees, d => d.Code == "MS-CS" && d.Title == "Master of Science in Computer Science");
        Assert.Contains(degrees, d => d.Code == "PHD-CS" && d.Title == "Doctor of Philosophy in Computer Science");
        Assert.Contains(degrees, d => d.Code == "BS-MATH" && d.Title == "Bachelor of Science in Mathematics");

        // Verify degree properties
        var bs = degrees.First(d => d.Code == "BS-CS");
        Assert.Equal("Bachelor", bs.Level);
        Assert.True(bs.IsActive);
    }

    [Fact]
    public async Task SeedAsync_Should_Seed_Expected_Ranks()
    {
        // Act
        await DatabaseSeeder.SeedAsync(_context);

        // Assert
        var ranks = await _context.Ranks.ToListAsync();

        // Verify academic ranks exist
        Assert.Contains(ranks, r => r.Code == "PROF" && r.Title == "Professor");
        Assert.Contains(ranks, r => r.Code == "ASSOCPROF" && r.Title == "Associate Professor");
        Assert.Contains(ranks, r => r.Code == "ASSTPROF" && r.Title == "Assistant Professor");
        Assert.Contains(ranks, r => r.Code == "LECTURER" && r.Title == "Lecturer");

        // Verify rank properties
        var professor = ranks.First(r => r.Code == "PROF");
        Assert.Equal(1, professor.Level);
        Assert.True(professor.RequiresTenure);
        Assert.True(professor.AllowsTeaching);
        Assert.True(professor.AllowsResearch);
        Assert.Equal("Faculty", professor.Category);
    }

    [Fact]
    public async Task SeedAsync_Should_Seed_Expected_Subjects()
    {
        // Act
        await DatabaseSeeder.SeedAsync(_context);

        // Assert
        var subjects = await _context.Subjects.ToListAsync();

        // Verify core subjects exist
        Assert.Contains(subjects, s => s.Code == "CS101" && s.Title == "Introduction to Computer Science");
        Assert.Contains(subjects, s => s.Code == "CS201" && s.Title == "Data Structures and Algorithms");
        Assert.Contains(subjects, s => s.Code == "MATH101" && s.Title == "Calculus I");
        Assert.Contains(subjects, s => s.Code == "MATH201" && s.Title == "Calculus II");

        // Verify subject properties
        var cs101 = subjects.First(s => s.Code == "CS101");
        Assert.Equal(3, cs101.CreditHours);
        Assert.Equal("Undergraduate", cs101.Level);
        Assert.True(cs101.IsActive);
        Assert.Equal("COMP-SCI", cs101.DepartmentName);
        Assert.Equal("Fall", cs101.TypicalSemester);
    }

    [Fact]
    public async Task SeedAsync_Should_Seed_Expected_Buildings()
    {
        // Act
        await DatabaseSeeder.SeedAsync(_context);

        // Assert
        var buildings = await _context.Buildings.ToListAsync();

        // Verify key buildings exist
        Assert.Contains(buildings, b => b.Code == "ADMIN" && b.Name == "Administration Building");
        Assert.Contains(buildings, b => b.Code == "SCI" && b.Name == "Science Building");
        Assert.Contains(buildings, b => b.Code == "LIB" && b.Name == "University Library");

        // Verify building properties
        var admin = buildings.First(b => b.Code == "ADMIN");
        Assert.Equal("Administrative", admin.BuildingType);
        Assert.True(admin.IsActive);
        Assert.True(admin.HasElevator);
        Assert.True(admin.IsAccessible);
        Assert.Equal(3, admin.NumberOfFloors);
    }

    [Fact]
    public async Task SeedAsync_Should_Not_Duplicate_Data_On_Multiple_Calls()
    {
        // Act - Seed twice
        await DatabaseSeeder.SeedAsync(_context);
        var firstCount = await _context.Universities.CountAsync();

        await DatabaseSeeder.SeedAsync(_context);
        var secondCount = await _context.Universities.CountAsync();

        // Assert - Should not duplicate
        Assert.Equal(firstCount, secondCount);
    }

    [Fact]
    public async Task SeedAsync_Should_Handle_Existing_Data_Gracefully()
    {
        // Arrange - Add some existing data
        var existingUniversity = new University
        {
            Code = "EXISTING",
            Name = "Existing University",
            CreatedBy = "Test",
            ModifiedBy = "Test"
        };
        _context.Universities.Add(existingUniversity);
        await _context.SaveChangesAsync();

        // Act
        await DatabaseSeeder.SeedAsync(_context);

        // Assert - Should not affect existing data and skip seeding since data exists
        var universities = await _context.Universities.ToListAsync();
        Assert.Contains(universities, u => u.Code == "EXISTING");
        // Seeder skips adding new universities when any exist
        Assert.Single(universities);
        Assert.Equal("EXISTING", universities.First().Code);
    }

    [Fact]
    public async Task SeedAsync_Should_Create_Proper_Entity_Relationships()
    {
        // Act
        await DatabaseSeeder.SeedAsync(_context);

        // Assert - Verify department relationships
        var subjects = await _context.Subjects.ToListAsync();
        var departments = await _context.Departments.ToListAsync();

        // Verify subjects reference valid departments
        var csSubjects = subjects.Where(s => s.DepartmentName == "COMP-SCI").ToList();
        var mathSubjects = subjects.Where(s => s.DepartmentName == "MATH").ToList();

        Assert.NotEmpty(csSubjects);
        Assert.NotEmpty(mathSubjects);

        // Verify departments exist for referenced department names
        Assert.Contains(departments, d => d.FullName.Contains("Computer"));
        Assert.Contains(departments, d => d.FullName.Contains("Mathematics"));
    }

    [Fact]
    public async Task SeedAsync_Should_Set_Proper_Audit_Fields()
    {
        // Act
        await DatabaseSeeder.SeedAsync(_context);

        // Assert - Verify all seeded entities have proper audit fields
        var universities = await _context.Universities.ToListAsync();
        var departments = await _context.Departments.ToListAsync();
        var subjects = await _context.Subjects.ToListAsync();

        // Check universities
        Assert.All(universities, u =>
        {
            Assert.Equal("System", u.CreatedBy);
            Assert.Equal("System", u.ModifiedBy);
            Assert.True(u.CreatedDate > DateTime.MinValue);
            Assert.True(u.ModifiedDate > DateTime.MinValue);
        });

        // Check departments
        Assert.All(departments, d =>
        {
            Assert.Equal("System", d.CreatedBy);
            Assert.Equal("System", d.ModifiedBy);
        });

        // Check subjects
        Assert.All(subjects, s =>
        {
            Assert.Equal("System", s.CreatedBy);
            Assert.Equal("System", s.ModifiedBy);
        });
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}