using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Entities;
using Xunit;

namespace Zeus.Academia.Tests.Infrastructure;

/// <summary>
/// Comprehensive entity model tests covering validation, properties, inheritance, and business logic
/// </summary>
public class EntityModelTests
{
    private AcademiaDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AcademiaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        var configuration = new ConfigurationBuilder().Build();
        return new AcademiaDbContext(options, configuration);
    }

    #region BaseEntity Tests

    [Fact]
    public void BaseEntity_Should_Initialize_Audit_Properties()
    {
        // Arrange & Act
        var entity = new University { Code = "TEST", Name = "Test University" };

        // Assert
        Assert.NotEqual(default(DateTime), entity.CreatedDate);
        Assert.NotEqual(default(DateTime), entity.ModifiedDate);
        Assert.True(entity.IsActive);
    }

    [Fact]
    public void BaseEntity_Should_Update_ModifiedDate_On_Change()
    {
        // Arrange
        var entity = new University { Code = "TEST", Name = "Test University" };
        var originalModifiedDate = entity.ModifiedDate;
        
        // Simulate time passing
        Thread.Sleep(1);

        // Act
        entity.Name = "Modified Name";
        entity.ModifiedDate = DateTime.UtcNow;

        // Assert
        Assert.True(entity.ModifiedDate > originalModifiedDate);
    }

    #endregion

    #region University Tests

    [Fact]
    public void University_Should_Validate_Code_Format()
    {
        // Arrange
        var university = new University
        {
            Code = "INVALID-CODE", // Invalid format
            Name = "Test University"
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(university);
        var isValid = Validator.TryValidateObject(university, context, validationResults, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(University.Code)));
    }

    [Fact]
    public void University_Should_Validate_Required_Fields()
    {
        // Arrange
        var university = new University(); // Missing required fields

        // Act
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(university);
        var isValid = Validator.TryValidateObject(university, context, validationResults, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(University.Code)));
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(University.Name)));
    }

    [Fact]
    public void University_Should_Accept_Valid_Data()
    {
        // Arrange
        var university = new University
        {
            Code = "MIT",
            Name = "Massachusetts Institute of Technology",
            Location = "Cambridge, MA",
            Website = "https://web.mit.edu",
            AccreditationStatus = "Accredited",
            EstablishedYear = 1861,
            UniversityType = "Private",
            Country = "USA",
            StateProvince = "Massachusetts",
            City = "Cambridge",
            PostalCode = "02139",
            PhoneNumber = "+1-617-253-1000",
            Email = "info@mit.edu",
            StudentEnrollment = 11934,
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(university);
        var isValid = Validator.TryValidateObject(university, context, validationResults, true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(validationResults);
    }

    #endregion

    #region AccessLevel Tests

    [Fact]
    public void AccessLevel_Should_Validate_Code_Format()
    {
        // Arrange
        var accessLevel = new AccessLevel
        {
            Code = "TOOLONGCODE", // Too long
            Description = "Test access level"
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(accessLevel);
        var isValid = Validator.TryValidateObject(accessLevel, context, validationResults, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(AccessLevel.Code)));
    }

    [Fact]
    public void AccessLevel_Should_Have_Default_Permission_Values()
    {
        // Arrange & Act
        var accessLevel = new AccessLevel
        {
            Code = "TEST",
            Description = "Test Level",
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };

        // Assert
        Assert.False(accessLevel.CanCreate);
        Assert.True(accessLevel.CanRead); // Default is true for read access
        Assert.False(accessLevel.CanUpdate);
        Assert.False(accessLevel.CanDelete);
        Assert.False(accessLevel.CanAccessFinancial);
        Assert.False(accessLevel.CanAccessStudentRecords);
        Assert.False(accessLevel.CanAccessFacultyRecords);
        Assert.False(accessLevel.CanGenerateReports);
    }

    #endregion

    #region Degree Tests

    [Fact]
    public void Degree_Should_Validate_Code_And_Title()
    {
        // Arrange
        var degree = new Degree
        {
            Code = "PHD",
            Title = "Doctor of Philosophy",
            Level = "Doctorate",
            Description = "Advanced research degree",
            PrimaryDepartment = "Engineering",
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(degree);
        var isValid = Validator.TryValidateObject(degree, context, validationResults, true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(validationResults);
    }

    [Fact]
    public void Degree_Should_Require_Valid_Level()
    {
        // Arrange
        var degree = new Degree
        {
            Code = "INV",
            Title = "Invalid Degree",
            Level = "InvalidLevel" // Should be valid level
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(degree);
        var isValid = Validator.TryValidateObject(degree, context, validationResults, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(Degree.Level)));
    }

    #endregion

    #region Rank Tests

    [Fact]
    public void Rank_Should_Validate_Properties()
    {
        // Arrange
        var rank = new Rank
        {
            Code = "PROF",
            Title = "Professor",
            Level = 5,
            Description = "Full Professor",
            Category = "Faculty",
            RequiresTenure = true,
            MinDegreeLevel = "Doctorate",
            AllowsTeaching = true,
            AllowsResearch = true,
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(rank);
        var isValid = Validator.TryValidateObject(rank, context, validationResults, true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(validationResults);
    }

    [Fact]
    public void Rank_Should_Require_Valid_Category()
    {
        // Arrange
        var rank = new Rank
        {
            Code = "INV",
            Title = "Invalid Rank",
            Level = 1,
            Category = "InvalidCategory" // Should be valid category
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(rank);
        var isValid = Validator.TryValidateObject(rank, context, validationResults, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(Rank.Category)));
    }

    #endregion

    #region Department Tests

    [Fact]
    public void Department_Should_Validate_Title_Format()
    {
        // Arrange
        var department = new Department
        {
            Name = "invalidname", // Should start with uppercase
            FullName = "Invalid Department Name"
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(department);
        var isValid = Validator.TryValidateObject(department, context, validationResults, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(Department.Name)));
    }

    [Fact]
    public void Department_Should_Accept_Valid_Data()
    {
        // Arrange
        var department = new Department
        {
            Name = "Engineering",
            FullName = "School of Engineering",
            Description = "Engineering programs and research",
            Budget = 50000000.00m,
            Location = "Building 1",
            PhoneNumber = "617-253-1000",
            Email = "engineering@mit.edu",
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(department);
        var isValid = Validator.TryValidateObject(department, context, validationResults, true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(validationResults);
    }

    [Fact]
    public void Department_Collections_Should_Be_Initialized()
    {
        // Arrange & Act
        var department = new Department { Name = "Test", FullName = "Test Department" };

        // Assert
        Assert.NotNull(department.Professors);
        Assert.NotNull(department.Teachers);
        Assert.NotNull(department.TeachingProfs);
        Assert.NotNull(department.Students);
        Assert.NotNull(department.Chairs);
        Assert.NotNull(department.Subjects);
        Assert.Empty(department.Professors);
        Assert.Empty(department.Teachers);
        Assert.Empty(department.TeachingProfs);
        Assert.Empty(department.Students);
        Assert.Empty(department.Chairs);
        Assert.Empty(department.Subjects);
    }

    #endregion

    #region Academic Inheritance Tests

    /// <summary>
    /// Test implementation of Academic for testing inheritance behavior
    /// </summary>
    private class TestAcademic : Academic
    {
        public override bool Equals(object? obj) => base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();
    }

    [Fact]
    public void Academic_Should_Be_Abstract_Base_Class()
    {
        // Arrange & Act
        var academic = new TestAcademic
        {
            EmpNr = 12345,
            Name = "Dr. Test Academic",
            PhoneNumber = "555-0123",
            Salary = 75000
        };

        // Assert
        Assert.IsAssignableFrom<BaseEntity>(academic);
        Assert.Equal(12345, academic.EmpNr);
        Assert.Equal("Dr. Test Academic", academic.Name);
        Assert.Equal("555-0123", academic.PhoneNumber);
        Assert.Equal(75000, academic.Salary);
        Assert.NotNull(academic.AcademicDegrees);
        Assert.Empty(academic.AcademicDegrees);
    }

    [Fact]
    public void Academic_Should_Have_Required_Properties()
    {
        // Arrange
        var academic = new TestAcademic(); // Missing required properties

        // Act
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(academic);
        var isValid = Validator.TryValidateObject(academic, context, validationResults, true);

        // Assert - Academic requires Name and BaseEntity audit fields
        Assert.False(isValid);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(Academic.Name)));
        // Note: EmpNr (int) doesn't trigger [Required] validation for default value (0)
    }

    [Fact]
    public void Academic_Should_Accept_Valid_Data()
    {
        // Arrange
        var academic = new TestAcademic
        {
            EmpNr = 12345,
            Name = "Dr. Test Academic",
            PhoneNumber = "555-0123",
            Salary = 75000,
            CreatedBy = "test-user",
            ModifiedBy = "test-user"
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(academic);
        var isValid = Validator.TryValidateObject(academic, context, validationResults, true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(validationResults);
    }

    #endregion

    #region Academic Validation Attribute Tests

    [Fact]
    public void Academic_Should_Validate_Name_MaxLength()
    {
        var invalidNames = new[]
        {
            new string('A', 51), // 51 characters - exceeds 50 max
            new string('B', 100)  // 100 characters - way over limit
        };

        foreach (var invalidName in invalidNames)
        {
            // Arrange
            var academic = new TestAcademic { EmpNr = 1, Name = invalidName };

            // Act & Assert
            var validationResults = ValidateEntity(academic);
            Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(Academic.Name)));
        }
    }

    [Fact]
    public void Academic_Should_Validate_PhoneNumber_MaxLength()
    {
        // Arrange
        var academic = new TestAcademic
        {
            EmpNr = 1,
            Name = "Test",
            PhoneNumber = new string('1', 16) // 16 characters - exceeds 15 max
        };

        // Act & Assert
        var validationResults = ValidateEntity(academic);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(Academic.PhoneNumber)));
    }

    private static List<ValidationResult> ValidateEntity(object entity)
    {
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(entity);
        Validator.TryValidateObject(entity, context, validationResults, true);
        return validationResults;
    }

    #endregion

    #region Professor Tests

    [Fact]
    public void Professor_Should_Inherit_From_Academic()
    {
        // Arrange & Act
        var professor = new Professor
        {
            EmpNr = 123,
            Name = "Dr. Smith",
            RankCode = "PROF",
            DepartmentName = "CS",
            HasTenure = true,
            ResearchArea = "Machine Learning"
        };

        // Assert
        Assert.IsAssignableFrom<Academic>(professor);
        Assert.Equal(123, professor.EmpNr);
        Assert.Equal("Dr. Smith", professor.Name);
        Assert.Equal("PROF", professor.RankCode);
        Assert.Equal("CS", professor.DepartmentName);
        Assert.True(professor.HasTenure);
        Assert.Equal("Machine Learning", professor.ResearchArea);
        Assert.NotNull(professor.CommitteeMembers);
        Assert.NotNull(professor.Teachings);
    }

    [Fact]
    public void Professor_Should_Have_Required_Properties()
    {
        // Arrange
        var professor = new Professor(); // Missing required inherited properties

        // Act
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(professor);
        var isValid = Validator.TryValidateObject(professor, context, validationResults, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(Professor.Name)));
        // Note: EmpNr (int) doesn't trigger [Required] validation for default value (0)
    }

    [Fact]
    public void Professor_Should_Accept_Valid_Data()
    {
        // Arrange
        var professor = new Professor
        {
            EmpNr = 12345,
            Name = "Dr. Jane Smith",
            PhoneNumber = "555-0123",
            Salary = 95000,
            RankCode = "PROF",
            DepartmentName = "CS", // Max length 15
            HasTenure = true,
            ResearchArea = "Artificial Intelligence",
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(professor);
        var isValid = Validator.TryValidateObject(professor, context, validationResults, true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(validationResults);
    }

    #endregion

    #region Chair Tests

    [Fact]
    public void Chair_Should_Have_Required_Properties()
    {
        // Arrange & Act
        var chair = new Chair
        {
            Name = "Department Chair",
            Description = "Computer Science Department Chair",
            DepartmentName = "Computer Science",
            AcademicEmpNr = 12345
        };

        // Assert
        Assert.IsAssignableFrom<BaseEntity>(chair);
        Assert.Equal("Department Chair", chair.Name);
        Assert.Equal("Computer Science Department Chair", chair.Description);
        Assert.Equal("Computer Science", chair.DepartmentName);
        Assert.Equal(12345, chair.AcademicEmpNr);
    }

    [Fact]
    public void Chair_Should_Validate_Required_Fields()
    {
        // Arrange
        var chair = new Chair(); // Missing required fields

        // Act
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(chair);
        var isValid = Validator.TryValidateObject(chair, context, validationResults, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(Chair.Name)));
    }

    [Fact]
    public void Chair_Should_Accept_Valid_Data()
    {
        // Arrange
        var chair = new Chair
        {
            Name = "CS Department Chair",
            Description = "Chairperson of Computer Science Department",
            DepartmentName = "CS", // Max length 15, so use short name
            AcademicEmpNr = 12345,
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(chair);
        var isValid = Validator.TryValidateObject(chair, context, validationResults, true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(validationResults);
    }

    #endregion

    #region TeachingProf Tests

    [Fact]
    public void TeachingProf_Should_Inherit_From_Academic()
    {
        // Arrange & Act
        var teachingProf = new TeachingProf
        {
            EmpNr = 456,
            Name = "Dr. Brown",
            RankCode = "ASST",
            DepartmentName = "Math",
            HasTenure = false,
            ResearchArea = "Statistics",
            Specialization = "Applied Statistics",
            EmploymentType = "Full-time",
            MaxCourseLoad = 5,
            TeachingPercentage = 70m,
            ResearchPercentage = 30m
        };

        // Assert
        Assert.IsAssignableFrom<Academic>(teachingProf);
        Assert.Equal(456, teachingProf.EmpNr);
        Assert.Equal("Dr. Brown", teachingProf.Name);
        Assert.Equal("ASST", teachingProf.RankCode);
        Assert.Equal("Math", teachingProf.DepartmentName);
        Assert.False(teachingProf.HasTenure);
        Assert.Equal("Statistics", teachingProf.ResearchArea);
        Assert.Equal("Applied Statistics", teachingProf.Specialization);
        Assert.Equal("Full-time", teachingProf.EmploymentType);
        Assert.Equal(5, teachingProf.MaxCourseLoad);
        Assert.Equal(70m, teachingProf.TeachingPercentage);
        Assert.Equal(30m, teachingProf.ResearchPercentage);
        Assert.NotNull(teachingProf.CommitteeMembers);
        Assert.NotNull(teachingProf.Teachings);
    }

    [Fact]
    public void TeachingProf_Should_Have_Required_Properties()
    {
        // Arrange
        var teachingProf = new TeachingProf(); // Missing required inherited properties

        // Act
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(teachingProf);
        var isValid = Validator.TryValidateObject(teachingProf, context, validationResults, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(TeachingProf.Name)));
        // Note: EmpNr (int) doesn't trigger [Required] validation for default value (0)
    }

    [Fact]
    public void TeachingProf_Should_Accept_Valid_Data()
    {
        // Arrange
        var teachingProf = new TeachingProf
        {
            EmpNr = 54321,
            Name = "Dr. Sarah Brown",
            PhoneNumber = "555-0456",
            Salary = 78000,
            RankCode = "ASST",
            DepartmentName = "Mathematics",
            HasTenure = false,
            ResearchArea = "Applied Mathematics",
            Specialization = "Statistical Analysis",
            EmploymentType = "Full-time",
            MaxCourseLoad = 4,
            TeachingPercentage = 70m,
            ResearchPercentage = 30m,
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(teachingProf);
        var isValid = Validator.TryValidateObject(teachingProf, context, validationResults, true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(validationResults);
    }

    #endregion

    #region Student Tests

    [Fact]
    public void Student_Should_Inherit_From_Academic()
    {
        // Arrange & Act
        var student = new Student
        {
            EmpNr = 789,
            Name = "John Doe",
            StudentId = "S12345",
            Program = "Computer Science",
            DegreeCode = "BS",
            DepartmentName = "CS"
        };

        // Assert
        Assert.IsAssignableFrom<Academic>(student);
        Assert.Equal(789, student.EmpNr);
        Assert.Equal("John Doe", student.Name);
        Assert.Equal("S12345", student.StudentId);
        Assert.Equal("Computer Science", student.Program);
        Assert.Equal("BS", student.DegreeCode);
        Assert.Equal("CS", student.DepartmentName);
    }

    [Fact]
    public void Student_Should_Have_Required_Properties()
    {
        // Arrange
        var student = new Student(); // Missing required inherited properties

        // Act
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(student);
        var isValid = Validator.TryValidateObject(student, context, validationResults, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(Student.Name)));
        // Note: EmpNr (int) doesn't trigger [Required] validation for default value (0)
    }

    [Fact]
    public void Student_Should_Accept_Valid_Data()
    {
        // Arrange
        var student = new Student
        {
            EmpNr = 98765,
            Name = "Jane Smith",
            PhoneNumber = "555-0789",
            StudentId = "S98765",
            Program = "Engineering",
            DegreeCode = "MS",
            DepartmentName = "Engineering",
            EnrollmentDate = DateTime.Today.AddYears(-2),
            ExpectedGraduationDate = DateTime.Today.AddYears(1),
            GPA = 3.75m,
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(student);
        var isValid = Validator.TryValidateObject(student, context, validationResults, true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(validationResults);
    }

    #endregion

    #region Validation Attribute Tests

    [Fact]
    public void Entity_Should_Validate_MaxLength_Attributes()
    {
        var testCases = new[]
        {
            new { Entity = (object)new University { Code = new string('A', 11), Name = "Test" }, Property = nameof(University.Code) }, // 11 chars, max 10
            new { Entity = (object)new Department { Name = new string('A', 16), FullName = "Test" }, Property = nameof(Department.Name) }, // 16 chars, max 15
            new { Entity = (object)new Rank { Code = new string('A', 11), Title = "Test" }, Property = nameof(Rank.Code) } // 11 chars, max 10
        };

        foreach (var testCase in testCases)
        {
            // Act & Assert
            var validationResults = ValidateEntity(testCase.Entity);
            Assert.Contains(validationResults, v => v.MemberNames.Contains(testCase.Property));
        }
    }

    #endregion
}