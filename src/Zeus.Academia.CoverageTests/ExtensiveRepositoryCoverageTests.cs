using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Repositories;
using Zeus.Academia.Infrastructure.Repositories.Interfaces;
using Xunit;

namespace Zeus.Academia.CoverageTests;

public class ExtensiveRepositoryCoverageTests
{
    private AcademiaDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AcademiaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var mockConfig = new Mock<IConfiguration>();
        return new AcademiaDbContext(options, mockConfig.Object);
    }

    #region Academic Repository Tests

    [Fact]
    public async Task AcademicRepository_GetByDepartmentAsync_Success()
    {
        using var context = CreateInMemoryContext();
        var logger = Mock.Of<ILogger<AcademicRepository>>();
        var repository = new AcademicRepository(context, logger);

        // Create department
        var department = new Department { Name = "CS", FullName = "Computer Science" };
        context.Departments.Add(department);
        await context.SaveChangesAsync();

        // Create professors in the department
        var professors = new List<Professor>
        {
            new Professor { EmpNr = 1001, Name = "Prof A", DepartmentName = "CS", PhoneNumber = "555-1001", Salary = 70000 },
            new Professor { EmpNr = 1002, Name = "Prof B", DepartmentName = "CS", PhoneNumber = "555-1002", Salary = 75000 }
        };
        await repository.AddRangeAsync(professors);
        await context.SaveChangesAsync();

        // Test GetByDepartmentAsync
        var result = await repository.GetByDepartmentAsync(department.Name);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task AcademicRepository_SearchByNameAsync_Success()
    {
        using var context = CreateInMemoryContext();
        var logger = Mock.Of<ILogger<AcademicRepository>>();
        var repository = new AcademicRepository(context, logger);

        // Add professors with different names
        var professors = new List<Professor>
        {
            new Professor { EmpNr = 2001, Name = "Dr. John Smith", PhoneNumber = "555-2001", Salary = 70000 },
            new Professor { EmpNr = 2002, Name = "Dr. Jane Johnson", PhoneNumber = "555-2002", Salary = 75000 },
            new Professor { EmpNr = 2003, Name = "Prof. Michael Brown", PhoneNumber = "555-2003", Salary = 80000 }
        };
        await repository.AddRangeAsync(professors);
        await context.SaveChangesAsync();

        // Test SearchByNameAsync
        var johnResults = await repository.SearchByNameAsync("John");
        Assert.Equal(2, johnResults.Count()); // Should find both John Smith and Jane Johnson

        var drResults = await repository.SearchByNameAsync("Dr.");
        Assert.Equal(2, drResults.Count()); // Should find both doctors
    }

    [Fact]
    public async Task AcademicRepository_GetProfessorsAsync_Success()
    {
        using var context = CreateInMemoryContext();
        var logger = Mock.Of<ILogger<AcademicRepository>>();
        var repository = new AcademicRepository(context, logger);

        // Add mixed academic types
        var academics = new List<Academic>
        {
            new Professor { EmpNr = 3001, Name = "Prof A", PhoneNumber = "555-3001", Salary = 70000 },
            new Teacher { EmpNr = 3002, Name = "Teacher B", PhoneNumber = "555-3002", Salary = 50000 },
            new Student { EmpNr = 3003, Name = "Student C", PhoneNumber = "555-3003", StudentId = "STU001" }
        };
        await repository.AddRangeAsync(academics);
        await context.SaveChangesAsync();

        // Test GetProfessorsAsync
        var professors = await repository.GetProfessorsAsync();
        Assert.Single(professors);
        Assert.Equal("Prof A", professors.First().Name);
    }

    [Fact]
    public async Task AcademicRepository_GetTeachersAsync_Success()
    {
        using var context = CreateInMemoryContext();
        var logger = Mock.Of<ILogger<AcademicRepository>>();
        var repository = new AcademicRepository(context, logger);

        // Add mixed academic types
        var academics = new List<Academic>
        {
            new Professor { EmpNr = 4001, Name = "Prof A", PhoneNumber = "555-4001", Salary = 70000 },
            new Teacher { EmpNr = 4002, Name = "Teacher B", PhoneNumber = "555-4002", Salary = 50000 },
            new Teacher { EmpNr = 4003, Name = "Teacher C", PhoneNumber = "555-4003", Salary = 52000 }
        };
        await repository.AddRangeAsync(academics);
        await context.SaveChangesAsync();

        // Test GetTeachersAsync
        var teachers = await repository.GetTeachersAsync();
        Assert.Equal(2, teachers.Count());
    }

    [Fact]
    public async Task AcademicRepository_GetStudentsAsync_Success()
    {
        using var context = CreateInMemoryContext();
        var logger = Mock.Of<ILogger<AcademicRepository>>();
        var repository = new AcademicRepository(context, logger);

        // Add mixed academic types
        var academics = new List<Academic>
        {
            new Professor { EmpNr = 5001, Name = "Prof A", PhoneNumber = "555-5001", Salary = 70000 },
            new Student { EmpNr = 5002, Name = "Student B", PhoneNumber = "555-5002", StudentId = "STU001" },
            new Student { EmpNr = 5003, Name = "Student C", PhoneNumber = "555-5003", StudentId = "STU002" }
        };
        await repository.AddRangeAsync(academics);
        await context.SaveChangesAsync();

        // Test GetStudentsAsync
        var students = await repository.GetStudentsAsync();
        Assert.Equal(2, students.Count());
    }

    #endregion

    #region Department Repository Tests

    [Fact]
    public async Task DepartmentRepository_SearchByNameAsync_Success()
    {
        using var context = CreateInMemoryContext();
        var logger = Mock.Of<ILogger<DepartmentRepository>>();
        var repository = new DepartmentRepository(context, logger);

        // Add departments
        var departments = new List<Department>
        {
            new Department { Name = "CS", FullName = "Computer Science Department" },
            new Department { Name = "MATH", FullName = "Mathematics Department" },
            new Department { Name = "PHYS", FullName = "Physical Science Department" }
        };
        await repository.AddRangeAsync(departments);
        await context.SaveChangesAsync();

        // Test SearchByNameAsync
        var scienceResults = await repository.SearchByNameAsync("Science");
        Assert.Equal(2, scienceResults.Count()); // Computer Science and Physical Science

        var mathResults = await repository.SearchByNameAsync("Math");
        Assert.Single(mathResults);
    }

    [Fact]
    public async Task DepartmentRepository_GetWithAcademicsCountAsync_Success()
    {
        using var context = CreateInMemoryContext();
        var departmentLogger = Mock.Of<ILogger<DepartmentRepository>>();
        var academicLogger = Mock.Of<ILogger<AcademicRepository>>();

        var departmentRepo = new DepartmentRepository(context, departmentLogger);
        var academicRepo = new AcademicRepository(context, academicLogger);

        // Create department
        var department = new Department { Name = "CS", FullName = "Computer Science Department" };
        await departmentRepo.AddAsync(department);
        await context.SaveChangesAsync();

        // Add academics to the department
        var academics = new List<Professor>
        {
            new Professor { EmpNr = 6001, Name = "Prof A", DepartmentName = "CS", PhoneNumber = "555-6001", Salary = 70000 },
            new Professor { EmpNr = 6002, Name = "Prof B", DepartmentName = "CS", PhoneNumber = "555-6002", Salary = 75000 }
        };
        await academicRepo.AddRangeAsync(academics);
        await context.SaveChangesAsync();

        // Test GetWithAcademicsCountAsync
        var result = await departmentRepo.GetWithAcademicsCountAsync();
        Assert.Single(result);
    }

    #endregion

    #region Subject Repository Tests

    [Fact]
    public async Task SubjectRepository_GetByCodeAsync_Success()
    {
        using var context = CreateInMemoryContext();
        var logger = Mock.Of<ILogger<SubjectRepository>>();
        var repository = new SubjectRepository(context, logger);

        // Create department
        var department = new Department { Name = "CS", FullName = "Computer Science Department" };
        context.Departments.Add(department);
        await context.SaveChangesAsync();

        // Add subject
        var subject = new Subject
        {
            Code = "CS101",
            Title = "Introduction to Programming",
            CreditHours = 3,
            DepartmentName = department.Name
        };
        await repository.AddAsync(subject);
        await context.SaveChangesAsync();

        // Test GetByCodeAsync
        var result = await repository.GetByCodeAsync("CS101");
        Assert.NotNull(result);
        Assert.Equal("Introduction to Programming", result.Title);
    }

    [Fact]
    public async Task SubjectRepository_SearchByNameAsync_Success()
    {
        using var context = CreateInMemoryContext();
        var logger = Mock.Of<ILogger<SubjectRepository>>();
        var repository = new SubjectRepository(context, logger);

        // Create department
        var department = new Department { Name = "CS", FullName = "Computer Science Department" };
        context.Departments.Add(department);
        await context.SaveChangesAsync();

        // Add subjects
        var subjects = new List<Subject>
        {
            new Subject { Code = "CS101", Title = "Introduction to Programming", CreditHours = 3, DepartmentName = department.Name },
            new Subject { Code = "CS201", Title = "Advanced Programming", CreditHours = 4, DepartmentName = department.Name },
            new Subject { Code = "CS301", Title = "Data Structures", CreditHours = 3, DepartmentName = department.Name }
        };
        await repository.AddRangeAsync(subjects);
        await context.SaveChangesAsync();

        // Test SearchByNameAsync
        var programmingResults = await repository.SearchByNameAsync("Programming");
        Assert.Equal(2, programmingResults.Count());

        var dataResults = await repository.SearchByNameAsync("Data");
        Assert.Single(dataResults);
    }

    [Fact]
    public async Task SubjectRepository_GetByCreditRangeAsync_Success()
    {
        using var context = CreateInMemoryContext();
        var logger = Mock.Of<ILogger<SubjectRepository>>();
        var repository = new SubjectRepository(context, logger);

        // Create department
        var department = new Department { Name = "MATH", FullName = "Mathematics Department" };
        context.Departments.Add(department);
        await context.SaveChangesAsync();

        // Add subjects with different credit hours
        var subjects = new List<Subject>
        {
            new Subject { Code = "MATH101", Title = "Algebra", CreditHours = 3, DepartmentName = department.Name },
            new Subject { Code = "MATH201", Title = "Calculus I", CreditHours = 4, DepartmentName = department.Name },
            new Subject { Code = "MATH301", Title = "Advanced Calculus", CreditHours = 5, DepartmentName = department.Name },
            new Subject { Code = "MATH401", Title = "Linear Algebra", CreditHours = 3, DepartmentName = department.Name }
        };
        await repository.AddRangeAsync(subjects);
        await context.SaveChangesAsync();

        // Test GetByCreditRangeAsync
        var threeFourCredits = await repository.GetByCreditRangeAsync(3, 4);
        Assert.Equal(3, threeFourCredits.Count()); // 3, 4, and 3 credit courses

        var highCredits = await repository.GetByCreditRangeAsync(4, 6);
        Assert.Equal(2, highCredits.Count()); // 4 and 5 credit courses
    }

    #endregion

    #region Unit of Work Tests

    [Fact]
    public async Task UnitOfWork_MultipleRepositoryOperations_Success()
    {
        using var context = CreateInMemoryContext();
        var logger = Mock.Of<ILogger<UnitOfWork>>();
        var academicLogger = Mock.Of<ILogger<AcademicRepository>>();
        var departmentLogger = Mock.Of<ILogger<DepartmentRepository>>();
        var subjectLogger = Mock.Of<ILogger<SubjectRepository>>();

        var academicRepo = new AcademicRepository(context, academicLogger);
        var departmentRepo = new DepartmentRepository(context, departmentLogger);
        var subjectRepo = new SubjectRepository(context, subjectLogger);
        var mockUserRepository = Mock.Of<IUserRepository>();
        var mockRoleRepository = Mock.Of<IRoleRepository>();
        var mockRefreshTokenRepository = Mock.Of<IRefreshTokenRepository>();

        var unitOfWork = new UnitOfWork(context, logger, academicRepo, departmentRepo, subjectRepo,
            mockUserRepository, mockRoleRepository, mockRefreshTokenRepository);

        // Create department
        var department = new Department { Name = "ENG", FullName = "Engineering Department" };
        await unitOfWork.Departments.AddAsync(department);
        await unitOfWork.SaveChangesAsync();

        // Create professor in that department
        var professor = new Professor
        {
            EmpNr = 7001,
            Name = "Dr. Engineering Prof",
            DepartmentName = department.Name,
            PhoneNumber = "555-7001",
            Salary = 85000
        };
        await unitOfWork.Academics.AddAsync(professor);

        // Create subject in that department
        var subject = new Subject
        {
            Code = "ENG101",
            Title = "Introduction to Engineering",
            CreditHours = 3,
            DepartmentName = department.Name
        };
        await unitOfWork.Subjects.AddAsync(subject);

        // Save all changes at once
        var changesSaved = await unitOfWork.SaveChangesAsync();
        Assert.True(changesSaved > 0);

        // Verify all entities were saved (checking each separately)
        var savedDepartment = await unitOfWork.Departments.GetSingleAsync(d => d.Name == department.Name);
        Assert.NotNull(savedDepartment);

        var savedProfessor = await unitOfWork.Academics.GetSingleAsync(p => p.EmpNr == professor.EmpNr);
        Assert.NotNull(savedProfessor);

        var allSubjects = await unitOfWork.Subjects.GetAllAsync();
        var savedSubject = await unitOfWork.Subjects.GetSingleAsync(s => s.Code == subject.Code);
        Assert.NotNull(savedSubject);
    }

    [Fact]
    public void UnitOfWork_PropertyAccess_Success()
    {
        using var context = CreateInMemoryContext();
        var logger = Mock.Of<ILogger<UnitOfWork>>();
        var academicLogger = Mock.Of<ILogger<AcademicRepository>>();
        var departmentLogger = Mock.Of<ILogger<DepartmentRepository>>();
        var subjectLogger = Mock.Of<ILogger<SubjectRepository>>();

        var academicRepo = new AcademicRepository(context, academicLogger);
        var departmentRepo = new DepartmentRepository(context, departmentLogger);
        var subjectRepo = new SubjectRepository(context, subjectLogger);
        var mockUserRepository = Mock.Of<IUserRepository>();
        var mockRoleRepository = Mock.Of<IRoleRepository>();
        var mockRefreshTokenRepository = Mock.Of<IRefreshTokenRepository>();

        var unitOfWork = new UnitOfWork(context, logger, academicRepo, departmentRepo, subjectRepo,
            mockUserRepository, mockRoleRepository, mockRefreshTokenRepository);

        // Test property accessors
        Assert.NotNull(unitOfWork.Academics);
        Assert.NotNull(unitOfWork.Departments);
        Assert.NotNull(unitOfWork.Subjects);
        Assert.Same(academicRepo, unitOfWork.Academics);
        Assert.Same(departmentRepo, unitOfWork.Departments);
        Assert.Same(subjectRepo, unitOfWork.Subjects);
    }

    #endregion

    #region Error Handling and Edge Cases

    [Fact]
    public async Task Repository_GetByIdAsync_NotFound_ReturnsNull()
    {
        using var context = CreateInMemoryContext();
        var logger = Mock.Of<ILogger<AcademicRepository>>();
        var repository = new AcademicRepository(context, logger);

        // Test with non-existent ID
        var result = await repository.GetByIdAsync(999999);
        Assert.Null(result);
    }

    [Fact]
    public async Task Repository_FindAsync_NoResults_ReturnsEmpty()
    {
        using var context = CreateInMemoryContext();
        var logger = Mock.Of<ILogger<AcademicRepository>>();
        var repository = new AcademicRepository(context, logger);

        // Test with predicate that matches nothing
        var result = await repository.FindAsync(a => a.Name == "Non-existent Person");
        Assert.Empty(result);
    }

    [Fact]
    public async Task Repository_CountAsync_EmptyDatabase_ReturnsZero()
    {
        using var context = CreateInMemoryContext();
        var logger = Mock.Of<ILogger<AcademicRepository>>();
        var repository = new AcademicRepository(context, logger);

        // Test count on empty database
        var count = await repository.CountAsync();
        Assert.Equal(0, count);

        var conditionalCount = await repository.CountAsync(a => a.Salary > 100000);
        Assert.Equal(0, conditionalCount);
    }

    [Fact]
    public async Task Repository_ExistsAsync_NoMatch_ReturnsFalse()
    {
        using var context = CreateInMemoryContext();
        var logger = Mock.Of<ILogger<AcademicRepository>>();
        var repository = new AcademicRepository(context, logger);

        // Test exists with no matching records
        var exists = await repository.ExistsAsync(a => a.Name == "Non-existent Person");
        Assert.False(exists);
    }

    [Fact]
    public async Task Repository_GetPagedAsync_EmptyDatabase_ReturnsEmpty()
    {
        using var context = CreateInMemoryContext();
        var logger = Mock.Of<ILogger<AcademicRepository>>();
        var repository = new AcademicRepository(context, logger);

        // Test paging on empty database
        var result = await repository.GetPagedAsync(1, 10);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Repository_GetSingleAsync_NoMatch_ReturnsNull()
    {
        using var context = CreateInMemoryContext();
        var logger = Mock.Of<ILogger<AcademicRepository>>();
        var repository = new AcademicRepository(context, logger);

        // Test GetSingleAsync with no matching records
        var result = await repository.GetSingleAsync(a => a.Name == "Non-existent Person");
        Assert.Null(result);
    }

    #endregion

    #region Performance and Large Dataset Tests

    [Fact]
    public async Task Repository_LargeDatasetOperations_Success()
    {
        using var context = CreateInMemoryContext();
        var logger = Mock.Of<ILogger<AcademicRepository>>();
        var repository = new AcademicRepository(context, logger);

        // Create a large dataset
        var professors = new List<Professor>();
        for (int i = 1; i <= 100; i++)
        {
            professors.Add(new Professor
            {
                EmpNr = 8000 + i,
                Name = $"Professor {i}",
                PhoneNumber = $"555-{8000 + i}",
                Salary = 50000 + (i * 500)
            });
        }

        // Test AddRangeAsync with large dataset
        await repository.AddRangeAsync(professors);
        await context.SaveChangesAsync();

        // Test various operations on large dataset
        var totalCount = await repository.CountAsync();
        Assert.Equal(100, totalCount);

        var highSalaryCount = await repository.CountAsync(p => p.Salary >= 75000);
        Assert.True(highSalaryCount > 0);

        var page1 = await repository.GetPagedAsync(1, 10);
        Assert.Equal(10, page1.Count());

        var lastPage = await repository.GetPagedAsync(10, 10);
        Assert.Equal(10, lastPage.Count());

        var nameSearch = await repository.FindAsync(p => p.Name.Contains("Professor 5"));
        Assert.True(nameSearch.Count() >= 10); // Should find Professor 5, 50, 51, 52, etc.
    }

    #endregion
}