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

public class Task6RepositoryPatternCoverageTests
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
    public async Task ProfessorRepository_BasicCrudOperations_Success()
    {
        using var context = CreateInMemoryContext();
        var logger = Mock.Of<ILogger<AcademicRepository>>();
        var repository = new AcademicRepository(context, logger);

        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Create a Department first (required)
        var department = new Department
        {
            Name = "Computer Science",
            FullName = "Computer Science Department",
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow
        };
        context.Departments.Add(department);
        await context.SaveChangesAsync();

        // Create
        var professor = new Professor
        {
            EmpNr = 12345,
            Name = "Dr. John Doe",
            PhoneNumber = "555-1234",
            Salary = 75000,
            DepartmentName = department.Name,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow
        };

        await repository.AddAsync(professor);
        await context.SaveChangesAsync();

        // Read - Academic entities use EmpNr as primary key
        var found = await repository.GetByIdAsync(professor.EmpNr);
        Assert.NotNull(found);
        Assert.Equal("Dr. John Doe", found.Name);
        Assert.Equal(12345, found.EmpNr);

        // Update
        found.Name = "Dr. Jane Doe";
        await repository.UpdateAsync(found);
        await context.SaveChangesAsync();

        var updated = await repository.GetByIdAsync(professor.EmpNr);
        Assert.NotNull(updated);
        Assert.Equal("Dr. Jane Doe", updated.Name);

        // Delete
        await repository.RemoveAsync(found);
        await context.SaveChangesAsync();

        var deleted = await repository.GetByIdAsync(professor.EmpNr);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task DepartmentRepository_BasicCrudOperations_Success()
    {
        using var context = CreateInMemoryContext();
        var logger = Mock.Of<ILogger<DepartmentRepository>>();
        var repository = new DepartmentRepository(context, logger);

        // Create
        var department = new Department
        {
            Name = "CS",
            FullName = "Computer Science Department"
        };

        await repository.AddAsync(department);
        await context.SaveChangesAsync();

        // Read
        var found = await repository.GetSingleAsync(d => d.Name == department.Name);
        Assert.NotNull(found);
        Assert.Equal("CS", found.Name);

        // Update
        found.FullName = "Information Technology Department";
        await repository.UpdateAsync(found);
        await context.SaveChangesAsync();

        var updated = await repository.GetSingleAsync(d => d.Name == department.Name);
        Assert.Equal("Information Technology Department", updated!.FullName);

        // Delete
        await repository.RemoveAsync(found);
        await context.SaveChangesAsync();

        var deleted = await repository.GetSingleAsync(d => d.Name == department.Name);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task SubjectRepository_BasicCrudOperations_Success()
    {
        using var context = CreateInMemoryContext();
        var logger = Mock.Of<ILogger<SubjectRepository>>();
        var repository = new SubjectRepository(context, logger);

        // Create department first (required for subject)
        var department = new Department
        {
            Name = "MATH",
            FullName = "Mathematics Department"
        };
        context.Departments.Add(department);
        await context.SaveChangesAsync();

        // Create subject
        var subject = new Subject
        {
            Code = "MATH101",
            Title = "Calculus I",
            CreditHours = 3,
            DepartmentName = department.Name
        };

        await repository.AddAsync(subject);
        await context.SaveChangesAsync();

        // Read
        var found = await repository.GetSingleAsync(s => s.Code == subject.Code);
        Assert.NotNull(found);
        Assert.Equal("Calculus I", found.Title);

        // Update
        found.Title = "Advanced Calculus I";
        await repository.UpdateAsync(found);
        await context.SaveChangesAsync();

        var updated = await repository.GetSingleAsync(s => s.Code == subject.Code);
        Assert.Equal("Advanced Calculus I", updated!.Title);

        // Delete
        await repository.RemoveAsync(found);
        await context.SaveChangesAsync();

        var deleted = await repository.GetSingleAsync(s => s.Code == subject.Code);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task UnitOfWork_TransactionManagement_Success()
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

        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Create a Department first (required)
        var department = new Department
        {
            Name = "Engineering",
            FullName = "Engineering Department",
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow
        };
        await unitOfWork.Departments.AddAsync(department);
        await unitOfWork.SaveChangesAsync();

        // Test UnitOfWork SaveChangesAsync and repository coordination
        var professor = new Professor
        {
            EmpNr = 54321,
            Name = "Dr. Alice Smith",
            PhoneNumber = "555-5678",
            Salary = 80000,
            DepartmentName = department.Name,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow
        };

        await unitOfWork.Academics.AddAsync(professor);
        var changesSaved = await unitOfWork.SaveChangesAsync();
        Assert.True(changesSaved > 0);

        // Verify the professor was saved
        var saved = await unitOfWork.Academics.GetByIdAsync(professor.EmpNr);
        Assert.NotNull(saved);
        Assert.Equal("Dr. Alice Smith", saved.Name);

        // Test coordinated operations across multiple repositories
        var subject = new Subject
        {
            Code = "ENG101",
            Title = "Introduction to Engineering",
            CreditHours = 3,
            DepartmentName = department.Name,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow
        };

        await unitOfWork.Subjects.AddAsync(subject);
        var moreChangesSaved = await unitOfWork.SaveChangesAsync();
        Assert.True(moreChangesSaved > 0);

        // Verify both entities exist and are properly coordinated
        var savedSubject = await unitOfWork.Subjects.GetSingleAsync(s => s.Code == "ENG101");
        Assert.NotNull(savedSubject);
        Assert.Equal(department.Name, savedSubject.DepartmentName);

        var savedProfessor = await unitOfWork.Academics.GetByIdAsync(professor.EmpNr);
        Assert.NotNull(savedProfessor);
        Assert.Equal(department.Name, ((Professor)savedProfessor).DepartmentName);
    }

    [Fact]
    public async Task Repository_GetPagedAsync_ReturnsCorrectResults()
    {
        using var context = CreateInMemoryContext();
        var logger = Mock.Of<ILogger<AcademicRepository>>();
        var repository = new AcademicRepository(context, logger);

        // Add test data
        for (int i = 0; i < 10; i++)
        {
            var professor = new Professor
            {
                EmpNr = 10000 + i,
                Name = $"Professor {i}",
                PhoneNumber = $"555-{i:D4}",
                Salary = 50000 + (i * 1000)
            };
            await repository.AddAsync(professor);
        }
        await context.SaveChangesAsync();

        // Test paging
        var result = await repository.GetPagedAsync(pageNumber: 1, pageSize: 5);
        Assert.Equal(5, result.Count());

        var result2 = await repository.GetPagedAsync(pageNumber: 2, pageSize: 5);
        Assert.Equal(5, result2.Count());
    }

    [Fact]
    public async Task Repository_CountAsync_ReturnsCorrectCount()
    {
        using var context = CreateInMemoryContext();
        var logger = Mock.Of<ILogger<DepartmentRepository>>();
        var repository = new DepartmentRepository(context, logger);

        // Add test data
        for (int i = 0; i < 7; i++)
        {
            var department = new Department
            {
                Name = $"DEPT{i}",
                FullName = $"Department {i}"
            };
            await repository.AddAsync(department);
        }
        await context.SaveChangesAsync();

        var totalCount = await repository.CountAsync();
        Assert.Equal(7, totalCount);
    }

    [Fact]
    public async Task Repository_ExistsAsync_ReturnsCorrectResult()
    {
        using var context = CreateInMemoryContext();
        var logger = Mock.Of<ILogger<SubjectRepository>>();
        var repository = new SubjectRepository(context, logger);

        // Create department first
        var department = new Department
        {
            Name = "PHYS",
            FullName = "Physics Department"
        };
        context.Departments.Add(department);
        await context.SaveChangesAsync();

        // Create subject
        var subject = new Subject
        {
            Code = "PHYS101",
            Title = "Physics I",
            CreditHours = 4,
            DepartmentName = department.Name
        };
        await repository.AddAsync(subject);
        await context.SaveChangesAsync();

        // Test exists
        var exists = await repository.ExistsAsync(s => s.Code == "PHYS101");
        Assert.True(exists);

        var notExists = await repository.ExistsAsync(s => s.Code == "PHYS999");
        Assert.False(notExists);
    }

    [Fact]
    public async Task SubjectRepository_GetByDepartmentAsync_ReturnsCorrectResults()
    {
        using var context = CreateInMemoryContext();
        var logger = Mock.Of<ILogger<SubjectRepository>>();
        var repository = new SubjectRepository(context, logger);

        // Create departments
        var mathDept = new Department { Name = "MATH", FullName = "Mathematics Department" };
        var csDept = new Department { Name = "CS", FullName = "Computer Science Department" };
        context.Departments.AddRange(mathDept, csDept);
        await context.SaveChangesAsync();

        // Create subjects
        var mathSubject1 = new Subject { Code = "MATH101", Title = "Calculus I", CreditHours = 3, DepartmentName = mathDept.Name };
        var mathSubject2 = new Subject { Code = "MATH102", Title = "Calculus II", CreditHours = 3, DepartmentName = mathDept.Name };
        var csSubject = new Subject { Code = "CS101", Title = "Programming I", CreditHours = 4, DepartmentName = csDept.Name };

        context.Subjects.AddRange(mathSubject1, mathSubject2, csSubject);
        await context.SaveChangesAsync();

        // Test get by department using the repository method
        var mathResults = await repository.GetByDepartmentAsync(mathDept.Name);
        Assert.Equal(2, mathResults.Count());

        var csResults = await repository.GetByDepartmentAsync(csDept.Name);
        Assert.Single(csResults);
    }

    [Fact]
    public void UnitOfWork_DisposesCorrectly()
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

        // This should not throw
        unitOfWork.Dispose();
    }
}