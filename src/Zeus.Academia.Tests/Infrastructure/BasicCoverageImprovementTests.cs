using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Repositories;
using Zeus.Academia.Infrastructure.Entities;
using Xunit;

namespace Zeus.Academia.Tests.Infrastructure;

/// <summary>
/// Simple comprehensive tests to improve code coverage for basic functionality
/// </summary>
public class BasicCoverageImprovementTests
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

    #region Repository Basic Operations Coverage

    [Fact]
    public async Task Repository_AddAsync_Should_Add_University()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateTestLogger<Repository<University>>();
        var repository = new Repository<University>(context, logger);

        var university = new University { Code = "TEST", Name = "Test University" };

        // Act
        await repository.AddAsync(university);
        await context.SaveChangesAsync();

        // Assert
        var saved = await context.Universities.FirstAsync();
        Assert.Equal("TEST", saved.Code);
        Assert.Equal("Test University", saved.Name);
    }

    [Fact]
    public async Task Repository_GetByIdAsync_Should_Return_Entity()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateTestLogger<Repository<University>>();
        var repository = new Repository<University>(context, logger);

        var university = new University { Code = "TEST", Name = "Test University" };
        context.Universities.Add(university);
        await context.SaveChangesAsync();

        // Act
        var retrievedEntity = await repository.GetSingleAsync(u => u.Code == university.Code);

        // Assert
        Assert.NotNull(retrievedEntity);
        Assert.Equal("TEST", retrievedEntity.Code);
    }

    [Fact]
    public async Task Repository_Update_Should_Modify_Entity()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateTestLogger<Repository<University>>();
        var repository = new Repository<University>(context, logger);

        var university = new University { Code = "TEST", Name = "Test University" };
        context.Universities.Add(university);
        await context.SaveChangesAsync();

        // Act
        university.Name = "Updated University";
        await repository.UpdateAsync(university);
        await context.SaveChangesAsync();

        // Assert
        var updated = await context.Universities.FirstAsync();
        Assert.Equal("Updated University", updated.Name);
    }

    [Fact]
    public async Task Repository_Remove_Should_Delete_Entity()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateTestLogger<Repository<University>>();
        var repository = new Repository<University>(context, logger);

        var university = new University { Code = "TEST", Name = "Test University" };
        context.Universities.Add(university);
        await context.SaveChangesAsync();

        // Act
        await repository.RemoveAsync(university);
        await context.SaveChangesAsync();

        // Assert
        var count = await context.Universities.CountAsync();
        Assert.Equal(0, count);
    }

    [Fact]
    public async Task Repository_FindAsync_Should_Filter_Entities()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateTestLogger<Repository<University>>();
        var repository = new Repository<University>(context, logger);

        var universities = new[]
        {
            new University { Code = "UJ", Name = "University of Johannesburg" },
            new University { Code = "UCT", Name = "University of Cape Town" },
            new University { Code = "UP", Name = "University of Pretoria" }
        };

        context.Universities.AddRange(universities);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.FindAsync(u => u.Name.Contains("University"));

        // Assert
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task Repository_GetPagedAsync_Should_Return_Paged_Results()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateTestLogger<Repository<Department>>();
        var repository = new Repository<Department>(context, logger);

        var departments = Enumerable.Range(1, 10)
            .Select(i => new Department
            {
                Name = $"DEPT{i:D2}",
                FullName = $"Department {i}"
            }).ToArray();

        context.Departments.AddRange(departments);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetPagedAsync(3, 2); // Skip 3, take 2

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task Repository_CountAsync_With_Predicate_Should_Return_Filtered_Count()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateTestLogger<Repository<Professor>>();
        var repository = new Repository<Professor>(context, logger);

        var professors = new[]
        {
            new Professor { EmpNr = 1, Name = "Dr. John Smith", DepartmentName = "CS", RankCode = "PROF" },
            new Professor { EmpNr = 2, Name = "Dr. Jane Doe", DepartmentName = "MATH", RankCode = "ASSOC" },
            new Professor { EmpNr = 3, Name = "Dr. Bob Johnson", DepartmentName = "CS", RankCode = "ASSIST" }
        };

        context.Professors.AddRange(professors);
        await context.SaveChangesAsync();

        // Act
        var csCount = await repository.CountAsync(p => p.DepartmentName == "CS");

        // Assert
        Assert.Equal(2, csCount);
    }

    #endregion

    #region DepartmentRepository Tests

    [Fact]
    public async Task DepartmentRepository_GetByCodeAsync_Should_Work()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateTestLogger<DepartmentRepository>();
        var repository = new DepartmentRepository(context, logger);

        var department = new Department
        {
            Name = "CS",
            FullName = "Computer Science Department"
        };

        context.Departments.Add(department);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByCodeAsync("CS");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("CS", result.Name);
    }

    // TODO: Re-enable when IsCodeAvailableAsync method is implemented in DepartmentRepository
    /*[Fact]
    public async Task DepartmentRepository_IsCodeAvailableAsync_Should_Work()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateTestLogger<DepartmentRepository>();
        var repository = new DepartmentRepository(context, logger);

        // Act
        var isAvailable = await repository.IsCodeAvailableAsync("NEWDEPT");

        // Assert
        Assert.True(isAvailable);
    }*/

    #endregion

    #region SubjectRepository Tests

    [Fact]
    public async Task SubjectRepository_GetByCodeAsync_Should_Work()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateTestLogger<SubjectRepository>();
        var repository = new SubjectRepository(context, logger);

        var subject = new Subject
        {
            Code = "CS101",
            Title = "Introduction to Computer Science",
            DepartmentName = "CS",
            CreditHours = 3
        };

        context.Subjects.Add(subject);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByCodeAsync("CS101");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("CS101", result.Code);
    }

    #endregion

    #region Entity Property Tests

    [Fact]
    public void University_Properties_Should_Be_Set_Correctly()
    {
        // Arrange & Act
        var university = new University
        {
            Code = "TEST",
            Name = "Test University",
            Location = "123 Test St",
            PhoneNumber = "555-1234",
            Email = "test@university.edu"
        };

        // Assert
        Assert.Equal("TEST", university.Code);
        Assert.Equal("Test University", university.Name);
        Assert.Equal("123 Test St", university.Location);
        Assert.Equal("555-1234", university.PhoneNumber);
        Assert.Equal("test@university.edu", university.Email);
    }

    [Fact]
    public void Department_Properties_Should_Be_Set_Correctly()
    {
        // Arrange & Act
        var department = new Department
        {
            Name = "CS",
            FullName = "Computer Science",
            Description = "Computer Science Department",
            PhoneNumber = "555-2345"
        };

        // Assert
        Assert.Equal("CS", department.Name);
        Assert.Equal("Computer Science", department.FullName);
        Assert.Equal("Computer Science Department", department.Description);
        Assert.Equal("555-2345", department.PhoneNumber);
    }

    [Fact]
    public void Professor_Properties_Should_Be_Set_Correctly()
    {
        // Arrange & Act
        var professor = new Professor
        {
            EmpNr = 12345,
            Name = "Dr. John Smith",
            PhoneNumber = "555-3456",
            Salary = 75000m,
            DepartmentName = "CS",
            RankCode = "PROF"
        };

        // Assert
        Assert.Equal(12345, professor.EmpNr);
        Assert.Equal("Dr. John Smith", professor.Name);
        Assert.Equal("555-3456", professor.PhoneNumber);
        Assert.Equal(75000m, professor.Salary);
        Assert.Equal("CS", professor.DepartmentName);
        Assert.Equal("PROF", professor.RankCode);
    }

    [Fact]
    public void Student_Properties_Should_Be_Set_Correctly()
    {
        // Arrange & Act
        var student = new Student
        {
            EmpNr = 54321,
            Name = "Jane Doe",
            StudentId = "STU001",
            Program = "Computer Science",
            DegreeCode = "BSC001",
            DepartmentName = "CS",
            YearOfStudy = 3,
            GPA = 3.75m,
            IsActive = true
        };

        // Assert
        Assert.Equal(54321, student.EmpNr);
        Assert.Equal("Jane Doe", student.Name);
        Assert.Equal("STU001", student.StudentId);
        Assert.Equal("Computer Science", student.Program);
        Assert.Equal("BSC001", student.DegreeCode);
        Assert.Equal("CS", student.DepartmentName);
        Assert.Equal(3, student.YearOfStudy);
        Assert.Equal(3.75m, student.GPA);
        Assert.True(student.IsActive);
    }

    #endregion

    #region Database Context Tests

    [Fact]
    public async Task DbContext_Should_Track_Changes()
    {
        // Arrange
        using var context = CreateInMemoryContext();

        var university = new University { Code = "TEST", Name = "Test University" };
        context.Universities.Add(university);
        await context.SaveChangesAsync();

        // Act
        university.Name = "Modified University";
        var modifiedEntries = context.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified).Count();

        // Assert
        Assert.Equal(1, modifiedEntries);
    }

    [Fact]
    public async Task DbContext_Should_Save_Multiple_Entities()
    {
        // Arrange
        using var context = CreateInMemoryContext();

        var entities = new object[]
        {
            new University { Code = "UJ", Name = "University of Johannesburg" },
            new Department { Name = "CS", FullName = "Computer Science" },
            new Rank { Title = "Professor", Description = "Full Professor" },
            new Subject { Code = "CS101", Title = "Intro to CS", DepartmentName = "CS" }
        };

        // Act
        foreach (var entity in entities)
        {
            context.Add(entity);
        }
        await context.SaveChangesAsync();

        // Assert
        Assert.Equal(1, await context.Universities.CountAsync());
        Assert.Equal(1, await context.Departments.CountAsync());
        Assert.Equal(1, await context.Ranks.CountAsync());
        Assert.Equal(1, await context.Subjects.CountAsync());
    }

    #endregion
}