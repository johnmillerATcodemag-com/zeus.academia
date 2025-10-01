using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Repositories;
using Xunit;

namespace Zeus.Academia.CoverageTests;

public class ComprehensiveTask6CoverageTests
{
    private AcademiaDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AcademiaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var mockConfig = new Mock<IConfiguration>();
        return new AcademiaDbContext(options, mockConfig.Object);
    }

    [Fact]
    public async Task Repository_GetAllAsync_Success()
    {
        using var context = CreateInMemoryContext();
        var logger = Mock.Of<ILogger<AcademicRepository>>();
        var repository = new AcademicRepository(context, logger);

        // Add test data
        var professor = new Professor
        {
            EmpNr = 12345,
            Name = "Dr. John Doe",
            PhoneNumber = "555-1234",
            Salary = 75000
        };
        await repository.AddAsync(professor);
        await context.SaveChangesAsync();

        // Test GetAllAsync
        var all = await repository.GetAllAsync();
        Assert.Single(all);
        Assert.Equal("Dr. John Doe", all.First().Name);
    }

    [Fact]
    public async Task Repository_FindAsync_Success()
    {
        using var context = CreateInMemoryContext();
        var logger = Mock.Of<ILogger<AcademicRepository>>();
        var repository = new AcademicRepository(context, logger);

        // Add test data
        var professor = new Professor
        {
            EmpNr = 12345,
            Name = "Dr. Jane Smith",
            PhoneNumber = "555-5678",
            Salary = 80000
        };
        await repository.AddAsync(professor);
        await context.SaveChangesAsync();

        // Test FindAsync with predicate
        var found = await repository.FindAsync(a => a.Name.Contains("Jane"));
        Assert.Single(found);
        Assert.Equal("Dr. Jane Smith", found.First().Name);
    }

    [Fact]
    public async Task Repository_GetSingleAsync_Success()
    {
        using var context = CreateInMemoryContext();
        var logger = Mock.Of<ILogger<AcademicRepository>>();
        var repository = new AcademicRepository(context, logger);

        // Add test data
        var professor = new Professor
        {
            EmpNr = 12345,
            Name = "Dr. Unique Person",
            PhoneNumber = "555-9999",
            Salary = 90000
        };
        await repository.AddAsync(professor);
        await context.SaveChangesAsync();

        // Test GetSingleAsync
        var single = await repository.GetSingleAsync(a => a.Name == "Dr. Unique Person");
        Assert.NotNull(single);
        Assert.Equal("Dr. Unique Person", single.Name);
    }

    [Fact]
    public async Task Repository_AddRangeAsync_Success()
    {
        using var context = CreateInMemoryContext();
        var logger = Mock.Of<ILogger<AcademicRepository>>();
        var repository = new AcademicRepository(context, logger);

        // Create multiple professors
        var professors = new List<Professor>
        {
            new Professor { EmpNr = 1001, Name = "Prof A", PhoneNumber = "555-0001", Salary = 70000 },
            new Professor { EmpNr = 1002, Name = "Prof B", PhoneNumber = "555-0002", Salary = 72000 },
            new Professor { EmpNr = 1003, Name = "Prof C", PhoneNumber = "555-0003", Salary = 75000 }
        };

        // Test AddRangeAsync
        await repository.AddRangeAsync(professors);
        await context.SaveChangesAsync();

        var all = await repository.GetAllAsync();
        Assert.Equal(3, all.Count());
    }

    [Fact]
    public async Task Repository_UpdateAsync_Success()
    {
        using var context = CreateInMemoryContext();
        var logger = Mock.Of<ILogger<AcademicRepository>>();
        var repository = new AcademicRepository(context, logger);

        // Add initial data
        var professor = new Professor
        {
            EmpNr = 2001,
            Name = "Dr. Original Name",
            PhoneNumber = "555-1111",
            Salary = 60000
        };
        await repository.AddAsync(professor);
        await context.SaveChangesAsync();

        // Update
        professor.Name = "Dr. Updated Name";
        professor.Salary = 65000;
        await repository.UpdateAsync(professor);
        await context.SaveChangesAsync();

        // Verify update
        var updated = await repository.GetSingleAsync(p => p.EmpNr == professor.EmpNr);
        Assert.Equal("Dr. Updated Name", updated!.Name);
        Assert.Equal(65000, updated.Salary);
    }

    [Fact]
    public async Task Repository_RemoveByIdAsync_Success()
    {
        using var context = CreateInMemoryContext();
        var logger = Mock.Of<ILogger<AcademicRepository>>();
        var repository = new AcademicRepository(context, logger);

        // Add data
        var professor = new Professor
        {
            EmpNr = 3001,
            Name = "Dr. To Be Deleted",
            PhoneNumber = "555-2222",
            Salary = 70000
        };
        await repository.AddAsync(professor);
        await context.SaveChangesAsync();
        var professorId = professor.Id;

        // Remove by ID
        await repository.RemoveByIdAsync(professorId);
        await context.SaveChangesAsync();

        // Verify removal
        var deleted = await repository.GetByIdAsync(professorId);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task Repository_RemoveRangeAsync_Success()
    {
        using var context = CreateInMemoryContext();
        var logger = Mock.Of<ILogger<AcademicRepository>>();
        var repository = new AcademicRepository(context, logger);

        // Add multiple professors
        var professors = new List<Professor>
        {
            new Professor { EmpNr = 4001, Name = "Prof X", PhoneNumber = "555-4001", Salary = 70000 },
            new Professor { EmpNr = 4002, Name = "Prof Y", PhoneNumber = "555-4002", Salary = 72000 }
        };
        await repository.AddRangeAsync(professors);
        await context.SaveChangesAsync();

        // Remove range
        await repository.RemoveRangeAsync(professors);
        await context.SaveChangesAsync();

        // Verify removal
        var remaining = await repository.GetAllAsync();
        Assert.Empty(remaining);
    }

    [Fact]
    public async Task DepartmentRepository_GetAllAsync_Success()
    {
        using var context = CreateInMemoryContext();
        var logger = Mock.Of<ILogger<DepartmentRepository>>();
        var repository = new DepartmentRepository(context, logger);

        // Add test data
        var department = new Department
        {
            Name = "CS",
            FullName = "Computer Science Department"
        };
        await repository.AddAsync(department);
        await context.SaveChangesAsync();

        // Test GetAllAsync
        var all = await repository.GetAllAsync();
        Assert.Single(all);
        Assert.Equal("CS", all.First().Name);
    }

    [Fact]
    public async Task DepartmentRepository_FindAsync_Success()
    {
        using var context = CreateInMemoryContext();
        var logger = Mock.Of<ILogger<DepartmentRepository>>();
        var repository = new DepartmentRepository(context, logger);

        // Add test departments
        var departments = new List<Department>
        {
            new Department { Name = "MATH", FullName = "Mathematics Department" },
            new Department { Name = "PHYS", FullName = "Physics Department" },
            new Department { Name = "CHEM", FullName = "Chemistry Department" }
        };
        await repository.AddRangeAsync(departments);
        await context.SaveChangesAsync();

        // Test FindAsync
        var mathDepts = await repository.FindAsync(d => d.FullName.Contains("Math"));
        Assert.Single(mathDepts);
        Assert.Equal("MATH", mathDepts.First().Name);
    }

    [Fact]
    public async Task SubjectRepository_GetAllAsync_Success()
    {
        using var context = CreateInMemoryContext();
        var logger = Mock.Of<ILogger<SubjectRepository>>();
        var repository = new SubjectRepository(context, logger);

        // Create department first
        var department = new Department
        {
            Name = "CS",
            FullName = "Computer Science Department"
        };
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

        // Test GetAllAsync
        var all = await repository.GetAllAsync();
        Assert.Single(all);
        Assert.Equal("CS101", all.First().Code);
    }

    [Fact]
    public async Task SubjectRepository_CountAsync_WithPredicate_Success()
    {
        using var context = CreateInMemoryContext();
        var logger = Mock.Of<ILogger<SubjectRepository>>();
        var repository = new SubjectRepository(context, logger);

        // Create department
        var department = new Department
        {
            Name = "MATH",
            FullName = "Mathematics Department"
        };
        context.Departments.Add(department);
        await context.SaveChangesAsync();

        // Add subjects with different credit hours
        var subjects = new List<Subject>
        {
            new Subject { Code = "MATH101", Title = "Algebra", CreditHours = 3, DepartmentName = department.Name },
            new Subject { Code = "MATH201", Title = "Calculus", CreditHours = 4, DepartmentName = department.Name },
            new Subject { Code = "MATH301", Title = "Advanced Math", CreditHours = 3, DepartmentName = department.Name }
        };
        await repository.AddRangeAsync(subjects);
        await context.SaveChangesAsync();

        // Test CountAsync with predicate
        var threeCreditCount = await repository.CountAsync(s => s.CreditHours == 3);
        Assert.Equal(2, threeCreditCount);

        var fourCreditCount = await repository.CountAsync(s => s.CreditHours == 4);
        Assert.Equal(1, fourCreditCount);
    }

    [Fact]
    public async Task UnitOfWork_SaveChangesAsync_Success()
    {
        using var context = CreateInMemoryContext();
        var logger = Mock.Of<ILogger<UnitOfWork>>();
        var academicLogger = Mock.Of<ILogger<AcademicRepository>>();
        var departmentLogger = Mock.Of<ILogger<DepartmentRepository>>();
        var subjectLogger = Mock.Of<ILogger<SubjectRepository>>();

        var academicRepo = new AcademicRepository(context, academicLogger);
        var departmentRepo = new DepartmentRepository(context, departmentLogger);
        var subjectRepo = new SubjectRepository(context, subjectLogger);

        var unitOfWork = new UnitOfWork(context, logger, academicRepo, departmentRepo, subjectRepo);

        // Add data through unit of work
        var professor = new Professor
        {
            EmpNr = 5001,
            Name = "Dr. Unit Test",
            PhoneNumber = "555-5001",
            Salary = 85000
        };

        await unitOfWork.Academics.AddAsync(professor);
        var changes = await unitOfWork.SaveChangesAsync();

        Assert.True(changes > 0);

        // Verify it was saved
        var saved = await unitOfWork.Academics.GetSingleAsync(p => p.EmpNr == professor.EmpNr);
        Assert.NotNull(saved);
        Assert.Equal("Dr. Unit Test", saved.Name);
    }

    [Fact]
    public async Task Repository_CountAsync_NoParameters_Success()
    {
        using var context = CreateInMemoryContext();
        var logger = Mock.Of<ILogger<AcademicRepository>>();
        var repository = new AcademicRepository(context, logger);

        // Add professors
        var professors = new List<Professor>
        {
            new Professor { EmpNr = 6001, Name = "Prof 1", PhoneNumber = "555-6001", Salary = 70000 },
            new Professor { EmpNr = 6002, Name = "Prof 2", PhoneNumber = "555-6002", Salary = 75000 },
            new Professor { EmpNr = 6003, Name = "Prof 3", PhoneNumber = "555-6003", Salary = 80000 }
        };
        await repository.AddRangeAsync(professors);
        await context.SaveChangesAsync();

        // Test CountAsync without parameters
        var totalCount = await repository.CountAsync();
        Assert.Equal(3, totalCount);
    }

    [Fact]
    public async Task Repository_ExistsAsync_Success()
    {
        using var context = CreateInMemoryContext();
        var logger = Mock.Of<ILogger<AcademicRepository>>();
        var repository = new AcademicRepository(context, logger);

        // Add professor
        var professor = new Professor
        {
            EmpNr = 7001,
            Name = "Dr. Existence Test",
            PhoneNumber = "555-7001",
            Salary = 70000
        };
        await repository.AddAsync(professor);
        await context.SaveChangesAsync();

        // Test ExistsAsync
        var exists = await repository.ExistsAsync(a => a.Name == "Dr. Existence Test");
        Assert.True(exists);

        var notExists = await repository.ExistsAsync(a => a.Name == "Dr. Non-Existent");
        Assert.False(notExists);
    }

    [Fact]
    public async Task Repository_GetPagedAsync_MultiplePages_Success()
    {
        using var context = CreateInMemoryContext();
        var logger = Mock.Of<ILogger<AcademicRepository>>();
        var repository = new AcademicRepository(context, logger);

        // Add 10 professors
        var professors = new List<Professor>();
        for (int i = 1; i <= 10; i++)
        {
            professors.Add(new Professor
            {
                EmpNr = 8000 + i,
                Name = $"Prof {i}",
                PhoneNumber = $"555-800{i:D1}",
                Salary = 70000 + (i * 1000)
            });
        }
        await repository.AddRangeAsync(professors);
        await context.SaveChangesAsync();

        // Test GetPagedAsync - Page 1 (first 4)
        var page1 = await repository.GetPagedAsync(1, 4);
        Assert.Equal(4, page1.Count());

        // Test GetPagedAsync - Page 2 (next 4)
        var page2 = await repository.GetPagedAsync(2, 4);
        Assert.Equal(4, page2.Count());

        // Test GetPagedAsync - Page 3 (last 2)
        var page3 = await repository.GetPagedAsync(3, 4);
        Assert.Equal(2, page3.Count());
    }
}