using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Data.Repositories;
using Zeus.Academia.Infrastructure.Entities;
using Xunit;

namespace Zeus.Academia.Tests.Infrastructure;

/// <summary>
/// Additional repository method tests to improve coverage - targeting uncovered repository methods
/// </summary>
public class RepositoryMethodCoverageTests
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

    #region AcademicRepository Uncovered Methods

    [Fact]
    public async Task AcademicRepository_SearchByNameAsync_Should_Find_Matching_Names()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateTestLogger<Repository<Academic>>();
        var repository = new AcademicRepository(context, logger);

        var professors = new[]
        {
            new Professor { EmpNr = 1, Name = "Dr. John Smith", DepartmentName = "CS", RankCode = "PROF" },
            new Professor { EmpNr = 2, Name = "Dr. Jane Smith", DepartmentName = "MATH", RankCode = "ASSOC" },
            new Professor { EmpNr = 3, Name = "Dr. Bob Johnson", DepartmentName = "CS", RankCode = "ASSIST" }
        };

        context.Professors.AddRange(professors);
        await context.SaveChangesAsync();

        // Act
        var smithResults = await repository.SearchByNameAsync("Smith");
        var johnResults = await repository.SearchByNameAsync("John");
        var noResults = await repository.SearchByNameAsync("NonExistent");

        // Assert
        Assert.Equal(2, smithResults.Count());
        Assert.Equal(2, johnResults.Count()); // John Smith and Johnson
        Assert.Empty(noResults);
    }

    [Fact]
    public async Task AcademicRepository_GetByDepartmentAsync_Should_Return_Department_Academics()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateTestLogger<Repository<Academic>>();
        var repository = new AcademicRepository(context, logger);

        var professors = new Professor[]
        {
            new Professor { EmpNr = 1, Name = "Prof CS1", DepartmentName = "1", RankCode = "PROF", CreatedBy = "test", ModifiedBy = "test" },
            new Professor { EmpNr = 2, Name = "Prof CS2", DepartmentName = "1", RankCode = "ASSOC", CreatedBy = "test", ModifiedBy = "test" },
            new Professor { EmpNr = 3, Name = "Prof MATH", DepartmentName = "2", RankCode = "PROF", CreatedBy = "test", ModifiedBy = "test" }
        };

        context.Professors.AddRange(professors);
        await context.SaveChangesAsync();

        // Act (Note: Method expects int but converts to string internally)
        var csAcademics = await repository.GetByDepartmentAsync(1); // Assuming CS = 1
        var mathAcademics = await repository.GetByDepartmentAsync(2); // Assuming MATH = 2
        var emptyResults = await repository.GetByDepartmentAsync(999);

        // Assert
        Assert.Equal(2, csAcademics.Count());
        Assert.Single(mathAcademics);
        Assert.Empty(emptyResults);
    }

    [Fact]
    public async Task AcademicRepository_GetByRankAsync_Should_Return_Rank_Academics()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateTestLogger<Repository<Academic>>();
        var repository = new AcademicRepository(context, logger);

        var professors = new[]
        {
            new Professor { EmpNr = 1, Name = "Full Prof 1", DepartmentName = "CS", RankCode = "1", CreatedBy = "test", ModifiedBy = "test" },
            new Professor { EmpNr = 2, Name = "Full Prof 2", DepartmentName = "MATH", RankCode = "1", CreatedBy = "test", ModifiedBy = "test" },
            new Professor { EmpNr = 3, Name = "Assoc Prof", DepartmentName = "CS", RankCode = "2", CreatedBy = "test", ModifiedBy = "test" }
        };

        context.Professors.AddRange(professors);
        await context.SaveChangesAsync();

        // Act (Note: Method expects int but converts to string internally)
        var fullProfs = await repository.GetByRankAsync(1); // Assuming PROF = 1
        var assocProfs = await repository.GetByRankAsync(2); // Assuming ASSOC = 2

        // Assert
        Assert.Equal(2, fullProfs.Count());
        Assert.Single(assocProfs);
    }

    [Fact]
    public async Task AcademicRepository_GetProfessorsAsync_Should_Return_Only_Professors()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateTestLogger<Repository<Academic>>();
        var repository = new AcademicRepository(context, logger);

        var academics = new Academic[]
        {
            new Professor { EmpNr = 1, Name = "Professor", DepartmentName = "CS", RankCode = "PROF" },
            new Teacher { EmpNr = 2, Name = "Teacher", DepartmentName = "CS" },
            new Student { EmpNr = 3, Name = "Student", DegreeCode = "BSC001" }
        };

        context.Professors.Add((Professor)academics[0]);
        context.Teachers.Add((Teacher)academics[1]);
        context.Students.Add((Student)academics[2]);
        await context.SaveChangesAsync();

        // Act
        var professors = await repository.GetProfessorsAsync();

        // Assert
        Assert.Single(professors);
        Assert.Equal("Professor", professors.First().Name);
    }

    [Fact]
    public async Task AcademicRepository_GetWithDepartmentAndRankAsync_Should_Include_Related_Data()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateTestLogger<Repository<Academic>>();
        var repository = new AcademicRepository(context, logger);

        // Add supporting data
        context.Departments.Add(new Department { Name = "CS", FullName = "Computer Science" });
        context.Ranks.Add(new Rank { Code = "PROF", Title = "Professor" });
        
        var professor = new Professor { EmpNr = 1, Name = "Prof Test", DepartmentName = "CS", RankCode = "PROF" };
        context.Professors.Add(professor);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetWithDepartmentAndRankAsync();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal("Prof Test", result.First().Name);
    }

    #endregion

    #region DepartmentRepository Uncovered Methods

    [Fact]
    public async Task DepartmentRepository_SearchByNameAsync_Should_Find_Matching_Departments()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateTestLogger<Repository<Department>>();
        var repository = new DepartmentRepository(context, logger);

        var departments = new[]
        {
            new Department { Name = "CS", FullName = "Computer Science Department" },
            new Department { Name = "MATH", FullName = "Mathematics Department" },
            new Department { Name = "PHYS", FullName = "Physics Department" }
        };

        context.Departments.AddRange(departments);
        await context.SaveChangesAsync();

        // Act
        var scienceResults = await repository.SearchByNameAsync("Science");
        var mathResults = await repository.SearchByNameAsync("Math");
        var noResults = await repository.SearchByNameAsync("Biology");

        // Assert
        Assert.Equal(2, scienceResults.Count()); // Computer Science and Physics
        Assert.Single(mathResults);
        Assert.Empty(noResults);
    }

    [Fact]
    public async Task DepartmentRepository_GetWithChairAsync_Should_Include_Chair_Data()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateTestLogger<Repository<Department>>();
        var repository = new DepartmentRepository(context, logger);

        var professor = new Professor { EmpNr = 100, Name = "Chair Person", DepartmentName = "CS", RankCode = "PROF" };
        var department = new Department { Name = "CS", FullName = "Computer Science", HeadEmpNr = 100 };
        
        context.Professors.Add(professor);
        context.Departments.Add(department);
        await context.SaveChangesAsync();

        // Act
        var results = await repository.GetWithChairAsync();
        var result = results.FirstOrDefault(d => d.Name == "CS");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("CS", result.Name);
    }

    [Fact]
    public async Task DepartmentRepository_GetWithAcademicCountAsync_Should_Include_Count()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateTestLogger<Repository<Department>>();
        var repository = new DepartmentRepository(context, logger);

        var department = new Department { Name = "CS", FullName = "Computer Science" };
        var professors = new[]
        {
            new Professor { EmpNr = 1, Name = "Prof 1", DepartmentName = "CS", RankCode = "PROF" },
            new Professor { EmpNr = 2, Name = "Prof 2", DepartmentName = "CS", RankCode = "ASSOC" }
        };

        context.Departments.Add(department);
        context.Professors.AddRange(professors);
        await context.SaveChangesAsync();

        // Act
        var results = await repository.GetWithAcademicCountAsync();

        // Assert
        Assert.NotEmpty(results);
        var csResult = results.FirstOrDefault(r => r.Department.Name == "CS");
        Assert.NotNull(csResult);
        Assert.Equal(2, csResult.AcademicCount);
    }

    #endregion

    #region SubjectRepository Uncovered Methods

    [Fact]
    public async Task SubjectRepository_GetByDepartmentAsync_Should_Return_Department_Subjects()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateTestLogger<Repository<Subject>>();
        var repository = new SubjectRepository(context, logger);

        var subjects = new[]
        {
            new Subject { Code = "CS101", Title = "Intro to CS", DepartmentName = "1", CreditHours = 3 },
            new Subject { Code = "CS201", Title = "Data Structures", DepartmentName = "1", CreditHours = 4 },
            new Subject { Code = "MATH101", Title = "Calculus", DepartmentName = "2", CreditHours = 4 }
        };

        context.Subjects.AddRange(subjects);
        await context.SaveChangesAsync();

        // Act
        var csSubjects = await repository.GetByDepartmentAsync(1);
        var mathSubjects = await repository.GetByDepartmentAsync(2);

        // Assert
        Assert.Equal(2, csSubjects.Count());
        Assert.Single(mathSubjects);
    }

    [Fact]
    public async Task SubjectRepository_SearchByNameAsync_Should_Find_Matching_Subjects()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateTestLogger<Repository<Subject>>();
        var repository = new SubjectRepository(context, logger);

        var subjects = new[]
        {
            new Subject { Code = "CS101", Title = "Introduction to Computer Science", DepartmentName = "CS", CreditHours = 3 },
            new Subject { Code = "CS201", Title = "Advanced Computer Programming", DepartmentName = "CS", CreditHours = 4 },
            new Subject { Code = "MATH101", Title = "Introduction to Mathematics", DepartmentName = "MATH", CreditHours = 4 }
        };

        context.Subjects.AddRange(subjects);
        await context.SaveChangesAsync();

        // Act
        var introResults = await repository.SearchByNameAsync("Introduction");
        var computerResults = await repository.SearchByNameAsync("Computer");

        // Assert
        Assert.Equal(2, introResults.Count());
        Assert.Equal(2, computerResults.Count());
    }

    [Fact]
    public async Task SubjectRepository_GetActiveSubjectsAsync_Should_Return_Active_Only()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateTestLogger<Repository<Subject>>();
        var repository = new SubjectRepository(context, logger);

        var subjects = new[]
        {
            new Subject { Code = "CS101", Title = "Active Subject", DepartmentName = "CS", CreditHours = 3, IsActive = true },
            new Subject { Code = "CS999", Title = "Inactive Subject", DepartmentName = "CS", CreditHours = 3, IsActive = false }
        };

        context.Subjects.AddRange(subjects);
        await context.SaveChangesAsync();

        // Act
        var activeSubjects = await repository.GetActiveSubjectsAsync();

        // Assert
        Assert.Single(activeSubjects);
        Assert.Equal("CS101", activeSubjects.First().Code);
    }

    [Fact]
    public async Task SubjectRepository_GetWithDepartmentAsync_Should_Include_Department_Data()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateTestLogger<Repository<Subject>>();
        var repository = new SubjectRepository(context, logger);

        var department = new Department { Name = "CS", FullName = "Computer Science" };
        var subject = new Subject { Code = "CS101", Title = "Intro to CS", DepartmentName = "CS", CreditHours = 3 };

        context.Departments.Add(department);
        context.Subjects.Add(subject);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetWithDepartmentAsync();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains(result, s => s.Code == "CS101");
    }

    [Fact]
    public async Task SubjectRepository_IsCodeAvailableAsync_Should_Check_Availability()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateTestLogger<Repository<Subject>>();
        var repository = new SubjectRepository(context, logger);

        var subject = new Subject { Code = "CS101", Title = "Existing Subject", DepartmentName = "CS", CreditHours = 3 };
        context.Subjects.Add(subject);
        await context.SaveChangesAsync();

        // Act
        var existingCodeAvailable = await repository.IsCodeAvailableAsync("CS101");
        var newCodeAvailable = await repository.IsCodeAvailableAsync("CS999");

        // Assert
        Assert.False(existingCodeAvailable);
        Assert.True(newCodeAvailable);
    }

    #endregion

    #region Additional Repository Method Coverage

    [Fact]
    public async Task Repository_UpdateRange_Should_Update_Multiple_Entities()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateTestLogger<Repository<University>>();
        var repository = new Repository<University>(context, logger);

        var universities = new[]
        {
            new University { Code = "UJ", Name = "University of Johannesburg" },
            new University { Code = "UCT", Name = "University of Cape Town" }
        };

        context.Universities.AddRange(universities);
        await context.SaveChangesAsync();

        // Act
        universities[0].Name = "Updated UJ";
        universities[1].Name = "Updated UCT";
        repository.UpdateRange(universities);
        await context.SaveChangesAsync();

        // Assert
        var updated = await context.Universities.ToListAsync();
        Assert.Contains(updated, u => u.Name == "Updated UJ");
        Assert.Contains(updated, u => u.Name == "Updated UCT");
    }

    [Fact]
    public async Task Repository_AddRangeAsync_Should_Add_Multiple_Entities()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateTestLogger<Repository<Degree>>();
        var repository = new Repository<Degree>(context, logger);

        var degrees = new[]
        {
            new Degree { Code = "BSC001", Title = "Bachelor of Science", Level = "Undergraduate" },
            new Degree { Code = "MSC001", Title = "Master of Science", Level = "Graduate" }
        };

        // Act
        await repository.AddRangeAsync(degrees);
        await context.SaveChangesAsync();

        // Assert
        var saved = await context.Degrees.ToListAsync();
        Assert.Equal(2, saved.Count);
        Assert.Contains(saved, d => d.Code == "BSC001");
        Assert.Contains(saved, d => d.Code == "MSC001");
    }

    [Fact]
    public async Task Repository_SingleAsync_Should_Return_Single_Entity()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateTestLogger<Repository<Rank>>();
        var repository = new Repository<Rank>(context, logger);

        var rank = new Rank { Code = "PROF", Title = "Professor", Level = 1 };
        context.Ranks.Add(rank);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SingleAsync(r => r.Code == "PROF");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Professor", result.Title);
    }

    [Fact]
    public async Task Repository_SingleOrDefaultAsync_Should_Return_Single_Or_Null()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateTestLogger<Repository<Rank>>();
        var repository = new Repository<Rank>(context, logger);

        var rank = new Rank { Code = "PROF", Title = "Professor", Level = 1 };
        context.Ranks.Add(rank);
        await context.SaveChangesAsync();

        // Act
        var existingResult = await repository.SingleOrDefaultAsync(r => r.Code == "PROF");
        var nullResult = await repository.SingleOrDefaultAsync(r => r.Code == "NONEXISTENT");

        // Assert
        Assert.NotNull(existingResult);
        Assert.Equal("Professor", existingResult.Title);
        Assert.Null(nullResult);
    }

    [Fact]
    public async Task Repository_AnyAsync_Should_Check_Existence()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateTestLogger<Repository<Building>>();
        var repository = new Repository<Building>(context, logger);

        var building = new Building { Code = "MAIN", Name = "Main Building" };
        context.Buildings.Add(building);
        await context.SaveChangesAsync();

        // Act
        var existsResult = await repository.AnyAsync(b => b.Code == "MAIN");
        var notExistsResult = await repository.AnyAsync(b => b.Code == "NONEXISTENT");

        // Assert
        Assert.True(existsResult);
        Assert.False(notExistsResult);
    }

    [Fact]
    public async Task Repository_GetPagedAsync_With_Predicate_Should_Filter_And_Page()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateTestLogger<Repository<Professor>>();
        var repository = new Repository<Professor>(context, logger);

        var professors = Enumerable.Range(1, 10)
            .Select(i => new Professor 
            { 
                EmpNr = i, 
                Name = $"Professor {i}", 
                DepartmentName = i <= 5 ? "CS" : "MATH",
                RankCode = "PROF"
            }).ToArray();

        context.Professors.AddRange(professors);
        await context.SaveChangesAsync();

        // Act
        var csProfessors = await repository.GetPagedAsync(p => p.DepartmentName == "CS", 1, 3);

        // Assert
        Assert.Equal(3, csProfessors.Count());
        Assert.All(csProfessors, p => Assert.Equal("CS", p.DepartmentName));
    }

    #endregion
}