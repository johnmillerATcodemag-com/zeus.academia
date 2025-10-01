using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.Tests.Entities;

/// <summary>
/// Unit tests for core entity models and their properties, inheritance, and validation.
/// </summary>
public class EntityValidationTests
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;

    public EntityValidationTests()
    {
        var services = new ServiceCollection();

        var configurationBuilder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true);

        _configuration = configurationBuilder.Build();

        services.AddSingleton(_configuration);
        _serviceProvider = services.BuildServiceProvider();
    }

    [Fact]
    public void BaseEntity_Should_Have_Required_Audit_Properties()
    {
        // Arrange & Act
        var academic = new Professor
        {
            EmpNr = 1,
            Name = "Test Professor",
            CreatedBy = "System",
            ModifiedBy = "System"
        };

        // Assert
        Assert.True(academic.CreatedDate != default);
        Assert.True(academic.ModifiedDate != default);
        Assert.Equal("System", academic.CreatedBy);
        Assert.Equal("System", academic.ModifiedBy);
    }

    [Fact]
    public void Academic_Should_Have_Required_Properties()
    {
        // Arrange & Act
        var academic = new Professor
        {
            EmpNr = 123,
            Name = "John Doe",
            PhoneNumber = "555-1234",
            Salary = 75000m,
            CreatedBy = "Test",
            ModifiedBy = "Test"
        };

        // Assert
        Assert.Equal(123, academic.EmpNr);
        Assert.Equal("John Doe", academic.Name);
        Assert.Equal("555-1234", academic.PhoneNumber);
        Assert.Equal(75000m, academic.Salary);
        Assert.NotNull(academic.AcademicDegrees);
    }

    [Fact]
    public void Professor_Should_Inherit_From_Academic()
    {
        // Arrange & Act
        var professor = new Professor
        {
            EmpNr = 456,
            Name = "Dr. Smith",
            RankCode = "P",
            DepartmentName = "Computer Science",
            HasTenure = true,
            ResearchArea = "Machine Learning",
            CreatedBy = "Test",
            ModifiedBy = "Test"
        };

        // Assert
        Assert.IsAssignableFrom<Academic>(professor);
        Assert.Equal(456, professor.EmpNr);
        Assert.Equal("Dr. Smith", professor.Name);
        Assert.Equal("P", professor.RankCode);
        Assert.Equal("Computer Science", professor.DepartmentName);
        Assert.True(professor.HasTenure);
        Assert.Equal("Machine Learning", professor.ResearchArea);
        Assert.NotNull(professor.CommitteeMembers);
        Assert.NotNull(professor.Teachings);
    }

    [Fact]
    public void Teacher_Should_Inherit_From_Academic()
    {
        // Arrange & Act
        var teacher = new Teacher
        {
            EmpNr = 789,
            Name = "Jane Johnson",
            DepartmentName = "Mathematics",
            Specialization = "Calculus",
            EmploymentType = "Full-time",
            MaxCourseLoad = 5,
            CreatedBy = "Test",
            ModifiedBy = "Test"
        };

        // Assert
        Assert.IsAssignableFrom<Academic>(teacher);
        Assert.Equal(789, teacher.EmpNr);
        Assert.Equal("Jane Johnson", teacher.Name);
        Assert.Equal("Mathematics", teacher.DepartmentName);
        Assert.Equal("Calculus", teacher.Specialization);
        Assert.Equal("Full-time", teacher.EmploymentType);
        Assert.Equal(5, teacher.MaxCourseLoad);
        Assert.NotNull(teacher.Teachings);
        Assert.NotNull(teacher.TeacherRatings);
    }

    [Fact]
    public void TeachingProf_Should_Inherit_From_Academic()
    {
        // Arrange & Act
        var teachingProf = new TeachingProf
        {
            EmpNr = 101,
            Name = "Dr. Wilson",
            RankCode = "AP",
            DepartmentName = "Physics",
            HasTenure = false,
            ResearchArea = "Quantum Physics",
            Specialization = "Modern Physics",
            EmploymentType = "Full-time",
            MaxCourseLoad = 4,
            TeachingPercentage = 60m,
            ResearchPercentage = 40m,
            CreatedBy = "Test",
            ModifiedBy = "Test"
        };

        // Assert
        Assert.IsAssignableFrom<Academic>(teachingProf);
        Assert.Equal(101, teachingProf.EmpNr);
        Assert.Equal("Dr. Wilson", teachingProf.Name);
        Assert.Equal("AP", teachingProf.RankCode);
        Assert.Equal("Physics", teachingProf.DepartmentName);
        Assert.False(teachingProf.HasTenure);
        Assert.Equal("Quantum Physics", teachingProf.ResearchArea);
        Assert.Equal("Modern Physics", teachingProf.Specialization);
        Assert.Equal("Full-time", teachingProf.EmploymentType);
        Assert.Equal(4, teachingProf.MaxCourseLoad);
        Assert.Equal(60m, teachingProf.TeachingPercentage);
        Assert.Equal(40m, teachingProf.ResearchPercentage);
        Assert.NotNull(teachingProf.CommitteeMembers);
        Assert.NotNull(teachingProf.Teachings);
        Assert.NotNull(teachingProf.TeacherRatings);
    }

    [Fact]
    public void Student_Should_Inherit_From_Academic()
    {
        // Arrange & Act
        var student = new Student
        {
            EmpNr = 202,
            Name = "Alice Brown",
            StudentId = "STU202",
            Program = "Computer Science",
            DegreeCode = "BS",
            DepartmentName = "Computer Science",
            YearOfStudy = 3,
            GPA = 3.75m,
            EnrollmentDate = DateTime.Now.AddYears(-2),
            ExpectedGraduationDate = DateTime.Now.AddYears(1),
            IsActive = true,
            CreatedBy = "Test",
            ModifiedBy = "Test"
        };

        // Assert
        Assert.IsAssignableFrom<Academic>(student);
        Assert.Equal(202, student.EmpNr);
        Assert.Equal("Alice Brown", student.Name);
        Assert.Equal("STU202", student.StudentId);
        Assert.Equal("Computer Science", student.Program);
        Assert.Equal("BS", student.DegreeCode);
        Assert.Equal("Computer Science", student.DepartmentName);
        Assert.Equal(3, student.YearOfStudy);
        Assert.Equal(3.75m, student.GPA);
        Assert.True(student.IsActive);
        Assert.NotNull(student.Enrollments);
    }

    [Fact]
    public void Chair_Should_Have_Required_Properties()
    {
        // Arrange & Act
        var chair = new Chair
        {
            Name = "Excellence Chair",
            Description = "Chair for excellence in teaching",
            DepartmentName = "Computer Science",
            AcademicEmpNr = 123,
            AppointmentStartDate = DateTime.Now,
            AppointmentEndDate = DateTime.Now.AddYears(3),
            IsActive = true,
            CreatedBy = "Test",
            ModifiedBy = "Test"
        };

        // Assert
        Assert.Equal("Excellence Chair", chair.Name);
        Assert.Equal("Chair for excellence in teaching", chair.Description);
        Assert.Equal("Computer Science", chair.DepartmentName);
        Assert.Equal(123, chair.AcademicEmpNr);
        Assert.True(chair.IsActive);
    }

    [Fact]
    public void Department_Should_Have_Required_Properties()
    {
        // Arrange & Act
        var department = new Department
        {
            Name = "CS",
            FullName = "Computer Science",
            Description = "Department of Computer Science",
            HeadEmpNr = 456,
            Budget = 1500000m,
            CreatedBy = "Test",
            ModifiedBy = "Test"
        };

        // Assert
        Assert.Equal("CS", department.Name);
        Assert.Equal("Computer Science", department.FullName);
        Assert.Equal("Department of Computer Science", department.Description);
        Assert.Equal(456, department.HeadEmpNr);
        Assert.Equal(1500000m, department.Budget);
        Assert.NotNull(department.Professors);
        Assert.NotNull(department.Teachers);
        Assert.NotNull(department.TeachingProfs);
        Assert.NotNull(department.Students);
        Assert.NotNull(department.Chairs);
        Assert.NotNull(department.Subjects);
    }

    [Fact]
    public void Degree_Should_Have_Required_Properties()
    {
        // Arrange & Act
        var degree = new Degree
        {
            Code = "PHD",
            Title = "Doctor of Philosophy",
            Level = "Doctorate",
            Description = "Highest academic degree",
            CreatedBy = "Test",
            ModifiedBy = "Test"
        };

        // Assert
        Assert.Equal("PHD", degree.Code);
        Assert.Equal("Doctor of Philosophy", degree.Title);
        Assert.Equal("Doctorate", degree.Level);
        Assert.Equal("Highest academic degree", degree.Description);
        Assert.NotNull(degree.AcademicDegrees);
        Assert.NotNull(degree.Students);
    }

    [Fact]
    public void University_Should_Have_Required_Properties()
    {
        // Arrange & Act
        var university = new University
        {
            Code = "MIT",
            Name = "Massachusetts Institute of Technology",
            Location = "Cambridge, MA",
            Website = "https://mit.edu",
            AccreditationStatus = "Accredited",
            CreatedBy = "Test",
            ModifiedBy = "Test"
        };

        // Assert
        Assert.Equal("MIT", university.Code);
        Assert.Equal("Massachusetts Institute of Technology", university.Name);
        Assert.Equal("Cambridge, MA", university.Location);
        Assert.Equal("https://mit.edu", university.Website);
        Assert.Equal("Accredited", university.AccreditationStatus);
        Assert.NotNull(university.AcademicDegrees);
    }

    [Fact]
    public void Rank_Should_Have_Required_Properties()
    {
        // Arrange & Act
        var rank = new Rank
        {
            Code = "AP",
            Title = "Associate Professor",
            Level = 2,
            Description = "Mid-level academic rank",
            MinSalary = 70000m,
            MaxSalary = 120000m,
            CreatedBy = "Test",
            ModifiedBy = "Test"
        };

        // Assert
        Assert.Equal("AP", rank.Code);
        Assert.Equal("Associate Professor", rank.Title);
        Assert.Equal(2, rank.Level);
        Assert.Equal("Mid-level academic rank", rank.Description);
        Assert.Equal(70000m, rank.MinSalary);
        Assert.Equal(120000m, rank.MaxSalary);
        Assert.NotNull(rank.Professors);
        Assert.NotNull(rank.TeachingProfs);
    }

    [Fact]
    public void Subject_Should_Have_Required_Properties()
    {
        // Arrange & Act
        var subject = new Subject
        {
            Code = "CS101",
            Title = "Introduction to Computer Science",
            Description = "Basic concepts of computer science",
            CreditHours = 3,
            DepartmentName = "Computer Science",
            Level = "Undergraduate",
            CreatedBy = "Test",
            ModifiedBy = "Test"
        };

        // Assert
        Assert.Equal("CS101", subject.Code);
        Assert.Equal("Introduction to Computer Science", subject.Title);
        Assert.Equal("Basic concepts of computer science", subject.Description);
        Assert.Equal(3, subject.CreditHours);
        Assert.Equal("Computer Science", subject.DepartmentName);
        Assert.Equal("Undergraduate", subject.Level);
        Assert.NotNull(subject.Teachings);
    }

    // ========== Task 3: Enhanced Academic Structure Entity Tests ==========

    [Fact]
    public void Department_Enhanced_Should_Have_All_New_Properties()
    {
        // Arrange & Act
        var department = new Department
        {
            Name = "Computer Science",
            Description = "Department of Computer Science and Engineering",
            Budget = 500000m,
            HeadEmpNr = 123,
            EstablishedDate = new DateTime(1985, 9, 1),
            IsActive = true,
            Location = "Building A, Floor 3",
            PhoneNumber = "+1-555-123-4567",
            Email = "cs@university.edu",
            CreatedBy = "Test",
            ModifiedBy = "Test"
        };

        // Assert
        Assert.Equal("Computer Science", department.Name);
        Assert.Equal("Department of Computer Science and Engineering", department.Description);
        Assert.Equal(500000m, department.Budget);
        Assert.Equal(123, department.HeadEmpNr);
        Assert.Equal(new DateTime(1985, 9, 1), department.EstablishedDate);
        Assert.True(department.IsActive);
        Assert.Equal("Building A, Floor 3", department.Location);
        Assert.Equal("+1-555-123-4567", department.PhoneNumber);
        Assert.Equal("cs@university.edu", department.Email);
    }

    [Fact]
    public void Subject_Enhanced_Should_Have_All_New_Properties()
    {
        // Arrange & Act
        var subject = new Subject
        {
            Code = "CS2010A",
            Title = "Advanced Data Structures and Algorithms",
            Description = "In-depth study of complex data structures and algorithmic techniques",
            CreditHours = 4,
            DepartmentName = "Computer Science",
            Level = "Graduate",
            Prerequisites = "CS101,MATH201",
            IsActive = true,
            TypicalSemester = "Fall",
            MaxEnrollment = 30,
            CreatedBy = "Test",
            ModifiedBy = "Test"
        };

        // Assert
        Assert.Equal("CS2010A", subject.Code);
        Assert.Equal("Advanced Data Structures and Algorithms", subject.Title);
        Assert.Equal("In-depth study of complex data structures and algorithmic techniques", subject.Description);
        Assert.Equal(4, subject.CreditHours);
        Assert.Equal("Computer Science", subject.DepartmentName);
        Assert.Equal("Graduate", subject.Level);
        Assert.Equal("CS101,MATH201", subject.Prerequisites);
        Assert.True(subject.IsActive);
        Assert.Equal("Fall", subject.TypicalSemester);
        Assert.Equal(30, subject.MaxEnrollment);
    }

    [Fact]
    public void Degree_Enhanced_Should_Have_All_New_Properties()
    {
        // Arrange & Act
        var degree = new Degree
        {
            Code = "BSCS",
            Title = "Bachelor of Science in Computer Science",
            Level = "Bachelor",
            Description = "Comprehensive undergraduate program in computer science",
            TotalCreditHours = 120,
            DurationYears = 4.0m,
            IsActive = true,
            PrimaryDepartment = "Computer Science",
            Specialization = "Software Engineering",
            MinimumGPA = 2.0m,
            CreatedBy = "Test",
            ModifiedBy = "Test"
        };

        // Assert
        Assert.Equal("BSCS", degree.Code);
        Assert.Equal("Bachelor of Science in Computer Science", degree.Title);
        Assert.Equal("Bachelor", degree.Level);
        Assert.Equal("Comprehensive undergraduate program in computer science", degree.Description);
        Assert.Equal(120, degree.TotalCreditHours);
        Assert.Equal(4.0m, degree.DurationYears);
        Assert.True(degree.IsActive);
        Assert.Equal("Computer Science", degree.PrimaryDepartment);
        Assert.Equal("Software Engineering", degree.Specialization);
        Assert.Equal(2.0m, degree.MinimumGPA);
    }

    [Fact]
    public void University_Enhanced_Should_Have_All_New_Properties()
    {
        // Arrange & Act
        var university = new University
        {
            Code = "MIT",
            Name = "Massachusetts Institute of Technology",
            Location = "Cambridge, Massachusetts",
            Website = "https://www.mit.edu",
            AccreditationStatus = "Accredited",
            EstablishedYear = 1861,
            UniversityType = "Private",
            Country = "United States",
            StateProvince = "Massachusetts",
            City = "Cambridge",
            PostalCode = "02139",
            PhoneNumber = "+1-617-253-1000",
            Email = "info@mit.edu",
            IsActive = true,
            StudentEnrollment = 11934,
            CreatedBy = "Test",
            ModifiedBy = "Test"
        };

        // Assert
        Assert.Equal("MIT", university.Code);
        Assert.Equal("Massachusetts Institute of Technology", university.Name);
        Assert.Equal("Cambridge, Massachusetts", university.Location);
        Assert.Equal("https://www.mit.edu", university.Website);
        Assert.Equal("Accredited", university.AccreditationStatus);
        Assert.Equal(1861, university.EstablishedYear);
        Assert.Equal("Private", university.UniversityType);
        Assert.Equal("United States", university.Country);
        Assert.Equal("Massachusetts", university.StateProvince);
        Assert.Equal("Cambridge", university.City);
        Assert.Equal("02139", university.PostalCode);
        Assert.Equal("+1-617-253-1000", university.PhoneNumber);
        Assert.Equal("info@mit.edu", university.Email);
        Assert.True(university.IsActive);
        Assert.Equal(11934, university.StudentEnrollment);
    }

    [Fact]
    public void Rank_Enhanced_Should_Have_All_New_Properties()
    {
        // Arrange & Act
        var rank = new Rank
        {
            Code = "PROF",
            Title = "Full Professor",
            Level = 1,
            Description = "Senior faculty member with tenure and extensive research experience",
            MinSalary = 80000m,
            MaxSalary = 150000m,
            Category = "Faculty",
            RequiresTenure = true,
            IsActive = true,
            MinExperienceYears = 10,
            MinDegreeLevel = "Doctorate",
            AllowsTeaching = true,
            AllowsResearch = true,
            CreatedBy = "Test",
            ModifiedBy = "Test"
        };

        // Assert
        Assert.Equal("PROF", rank.Code);
        Assert.Equal("Full Professor", rank.Title);
        Assert.Equal(1, rank.Level);
        Assert.Equal("Senior faculty member with tenure and extensive research experience", rank.Description);
        Assert.Equal(80000m, rank.MinSalary);
        Assert.Equal(150000m, rank.MaxSalary);
        Assert.Equal("Faculty", rank.Category);
        Assert.True(rank.RequiresTenure);
        Assert.True(rank.IsActive);
        Assert.Equal(10, rank.MinExperienceYears);
        Assert.Equal("Doctorate", rank.MinDegreeLevel);
        Assert.True(rank.AllowsTeaching);
        Assert.True(rank.AllowsResearch);
    }

    [Fact]
    public void Subject_Code_Validation_Should_Accept_Valid_Formats()
    {
        // Test various valid subject code formats
        var validCodes = new[] { "CS101", "MATH2010", "PHYS301A", "ENGL1001" };

        foreach (var code in validCodes)
        {
            var subject = new Subject
            {
                Code = code,
                Title = "Test Subject Title",
                Level = "Undergraduate",
                CreatedBy = "Test",
                ModifiedBy = "Test"
            };

            Assert.Equal(code, subject.Code);
        }
    }

    [Fact]
    public void Degree_Level_Should_Accept_Valid_Values()
    {
        // Test valid degree levels
        var validLevels = new[] { "Associate", "Bachelor", "Master", "Doctorate", "Certificate", "Diploma" };

        foreach (var level in validLevels)
        {
            var degree = new Degree
            {
                Code = "TEST",
                Title = "Test Degree Title Program",
                Level = level,
                CreatedBy = "Test",
                ModifiedBy = "Test"
            };

            Assert.Equal(level, degree.Level);
        }
    }

    [Fact]
    public void University_Type_Should_Accept_Valid_Values()
    {
        // Test valid university types
        var validTypes = new[] { "Public", "Private", "For-Profit", "Community", "Technical", "Online" };

        foreach (var type in validTypes)
        {
            var university = new University
            {
                Code = "TEST",
                Name = "Test University System",
                UniversityType = type,
                CreatedBy = "Test",
                ModifiedBy = "Test"
            };

            Assert.Equal(type, university.UniversityType);
        }
    }

    [Fact]
    public void Rank_Category_Should_Accept_Valid_Values()
    {
        // Test valid rank categories
        var validCategories = new[] { "Faculty", "Staff", "Administrator", "Research", "Adjunct", "Emeritus" };

        foreach (var category in validCategories)
        {
            var rank = new Rank
            {
                Code = "TEST",
                Title = "Test Academic Rank",
                Category = category,
                CreatedBy = "Test",
                ModifiedBy = "Test"
            };

            Assert.Equal(category, rank.Category);
        }
    }

    [Fact]
    public void Rank_Salary_Range_Should_Be_Logical()
    {
        // Arrange & Act
        var rank = new Rank
        {
            Code = "ASSOC",
            Title = "Associate Professor",
            Category = "Faculty",
            MinSalary = 60000m,
            MaxSalary = 90000m,
            CreatedBy = "Test",
            ModifiedBy = "Test"
        };

        // Assert
        Assert.True(rank.MinSalary <= rank.MaxSalary);
        Assert.Equal(60000m, rank.MinSalary);
        Assert.Equal(90000m, rank.MaxSalary);
    }

    [Fact]
    public void Subject_Level_Should_Accept_Valid_Academic_Levels()
    {
        // Test valid subject levels
        var validLevels = new[] { "Undergraduate", "Graduate", "Doctoral", "Continuing Education" };

        foreach (var level in validLevels)
        {
            var subject = new Subject
            {
                Code = "TEST101",
                Title = "Test Subject Course",
                Level = level,
                CreatedBy = "Test",
                ModifiedBy = "Test"
            };

            Assert.Equal(level, subject.Level);
        }
    }

    [Fact]
    public void University_Accreditation_Should_Accept_Valid_Statuses()
    {
        // Test valid accreditation statuses
        var validStatuses = new[] { "Accredited", "Provisional", "Candidate", "Not Accredited", "Under Review" };

        foreach (var status in validStatuses)
        {
            var university = new University
            {
                Code = "TEST",
                Name = "Test University Institute",
                AccreditationStatus = status,
                CreatedBy = "Test",
                ModifiedBy = "Test"
            };

            Assert.Equal(status, university.AccreditationStatus);
        }
    }
}