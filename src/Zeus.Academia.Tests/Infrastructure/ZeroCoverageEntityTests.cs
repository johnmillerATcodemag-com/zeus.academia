using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.Tests.Infrastructure;

/// <summary>
/// Comprehensive tests for zero-coverage entity classes to boost code coverage.
/// Targeting: AcademicDegree, Committee, CommitteeMember, TeacherRating, StudentEnrollment, Teaching entities.
/// </summary>
public class ZeroCoverageEntityTests : IDisposable
{
    private readonly AcademiaDbContext _context;
    private readonly IServiceProvider _serviceProvider;

    public ZeroCoverageEntityTests()
    {
        var options = new DbContextOptionsBuilder<AcademiaDbContext>()
            .UseInMemoryDatabase(databaseName: $"ZeroCoverageEntityDb_{Guid.NewGuid()}")
            .Options;

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "DataSource=:memory:"
            })
            .Build();

        _context = new AcademiaDbContext(options, configuration);
        _context.Database.EnsureCreated();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();
        _serviceProvider = services.BuildServiceProvider();

        // Seed required reference data
        SeedReferenceData().Wait();
    }

    private async Task SeedReferenceData()
    {
        // Add Universities
        if (!_context.Universities.Any())
        {
            _context.Universities.AddRange(
                new University { Code = "MIT", Name = "Massachusetts Institute of Technology", CreatedBy = "System", ModifiedBy = "System" },
                new University { Code = "STAN", Name = "Stanford University", CreatedBy = "System", ModifiedBy = "System" }
            );
        }

        // Add Degrees
        if (!_context.Degrees.Any())
        {
            _context.Degrees.AddRange(
                new Degree { Code = "BS-CS", Title = "Bachelor of Science in Computer Science", Level = "Undergraduate", CreatedBy = "System", ModifiedBy = "System" },
                new Degree { Code = "MS-CS", Title = "Master of Science in Computer Science", Level = "Graduate", CreatedBy = "System", ModifiedBy = "System" }
            );
        }

        // Add Departments
        if (!_context.Departments.Any())
        {
            _context.Departments.Add(new Department { Name = "Computer Science", FullName = "Computer Science Department", CreatedBy = "System", ModifiedBy = "System" });
        }

        // Add Subjects
        if (!_context.Subjects.Any())
        {
            _context.Subjects.AddRange(
                new Subject { Code = "CS101", Title = "Introduction to Programming", CreditHours = 3, DepartmentName = "Computer Science", CreatedBy = "System", ModifiedBy = "System" },
                new Subject { Code = "CS201", Title = "Data Structures", CreditHours = 4, DepartmentName = "Computer Science", CreatedBy = "System", ModifiedBy = "System" }
            );
        }

        // Add AccessLevels
        if (!_context.AccessLevels.Any())
        {
            _context.AccessLevels.Add(new AccessLevel { Code = "ACAD", Name = "Academic", Description = "Academic Access", CreatedBy = "System", ModifiedBy = "System" });
        }

        // Add Ranks
        if (!_context.Ranks.Any())
        {
            _context.Ranks.Add(new Rank { Code = "PROF", Title = "Professor", Description = "Full Professor", CreatedBy = "System", ModifiedBy = "System" });
        }

        // Add Academic (Professor - since Academic is abstract)
        if (!_context.Professors.Any())
        {
            _context.Professors.AddRange(
                new Professor { EmpNr = 12345, Name = "John Smith", CreatedBy = "System", ModifiedBy = "System" },
                new Professor { EmpNr = 67890, Name = "Jane Doe", CreatedBy = "System", ModifiedBy = "System" }
            );
        }

        // Add Student
        if (!_context.Students.Any())
        {
            _context.Students.Add(new Student { EmpNr = 98765, Name = "Alice Johnson", StudentId = "STU001", CreatedBy = "System", ModifiedBy = "System" });
        }

        await _context.SaveChangesAsync();
    }

    #region AcademicDegree Tests

    [Fact]
    public void AcademicDegree_Constructor_Should_Initialize_Properties()
    {
        // Act
        var academicDegree = new AcademicDegree();

        // Assert
        Assert.NotNull(academicDegree);
        Assert.Equal(string.Empty, academicDegree.DegreeCode);
        Assert.Equal(string.Empty, academicDegree.UniversityCode);
        Assert.Equal(0, academicDegree.AcademicEmpNr);
        Assert.Null(academicDegree.DateObtained);
    }

    [Fact]
    public void AcademicDegree_Properties_Should_Set_And_Get_Correctly()
    {
        // Arrange
        var academicDegree = new AcademicDegree();
        var dateObtained = DateTime.Now.Date;

        // Act
        academicDegree.AcademicEmpNr = 12345;
        academicDegree.DegreeCode = "BS-CS";
        academicDegree.UniversityCode = "MIT";
        academicDegree.DateObtained = dateObtained;

        // Assert
        Assert.Equal(12345, academicDegree.AcademicEmpNr);
        Assert.Equal("BS-CS", academicDegree.DegreeCode);
        Assert.Equal("MIT", academicDegree.UniversityCode);
        Assert.Equal(dateObtained, academicDegree.DateObtained);
    }

    [Fact]
    public void AcademicDegree_Navigation_Properties_Should_Initialize()
    {
        // Act
        var academicDegree = new AcademicDegree();

        // Assert
        Assert.Null(academicDegree.Academic);
        Assert.Null(academicDegree.Degree);
        Assert.Null(academicDegree.University);
    }

    [Fact]
    public async Task AcademicDegree_Should_Be_Added_To_Database()
    {
        // Arrange
        var academicDegree = new AcademicDegree
        {
            AcademicEmpNr = 12345,
            DegreeCode = "BS-CS",
            UniversityCode = "MIT",
            DateObtained = DateTime.Now.Date,
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };

        // Act
        _context.AcademicDegrees.Add(academicDegree);
        await _context.SaveChangesAsync();

        // Assert
        var savedDegree = await _context.AcademicDegrees.FirstOrDefaultAsync(ad => ad.AcademicEmpNr == 12345);
        Assert.NotNull(savedDegree);
        Assert.Equal("BS-CS", savedDegree.DegreeCode);
        Assert.Equal("MIT", savedDegree.UniversityCode);
    }

    [Fact]
    public async Task AcademicDegree_With_Navigation_Properties_Should_Load_Correctly()
    {
        // Arrange
        var academicDegree = new AcademicDegree
        {
            AcademicEmpNr = 12345,
            DegreeCode = "BS-CS",
            UniversityCode = "MIT",
            DateObtained = DateTime.Now.Date,
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };

        _context.AcademicDegrees.Add(academicDegree);
        await _context.SaveChangesAsync();

        // Act
        var loaded = await _context.AcademicDegrees
            .Include(ad => ad.Academic)
            .Include(ad => ad.Degree)
            .Include(ad => ad.University)
            .FirstOrDefaultAsync(ad => ad.AcademicEmpNr == 12345);

        // Assert
        Assert.NotNull(loaded);
        Assert.NotNull(loaded.Academic);
        Assert.NotNull(loaded.Degree);
        Assert.NotNull(loaded.University);
    }

    #endregion

    #region Committee Tests

    [Fact]
    public void Committee_Constructor_Should_Initialize_Properties()
    {
        // Act
        var committee = new Committee();

        // Assert
        Assert.NotNull(committee);
        Assert.Equal(string.Empty, committee.Name);
        Assert.Null(committee.Description);
        Assert.Null(committee.Type);
        Assert.True(committee.IsActive);
        Assert.NotNull(committee.Members);
        Assert.Empty(committee.Members);
    }

    [Fact]
    public void Committee_Properties_Should_Set_And_Get_Correctly()
    {
        // Arrange
        var committee = new Committee();

        // Act
        committee.Name = "Academic Standards Committee";
        committee.Description = "Committee responsible for academic standards";
        committee.Type = "Academic";
        committee.IsActive = false;

        // Assert
        Assert.Equal("Academic Standards Committee", committee.Name);
        Assert.Equal("Committee responsible for academic standards", committee.Description);
        Assert.Equal("Academic", committee.Type);
        Assert.False(committee.IsActive);
    }

    [Fact]
    public async Task Committee_Should_Be_Added_To_Database()
    {
        // Arrange
        var committee = new Committee
        {
            Name = "Test Committee",
            Description = "Test Description",
            Type = "Academic",
            IsActive = true,
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };

        // Act
        _context.Committees.Add(committee);
        await _context.SaveChangesAsync();

        // Assert
        var savedCommittee = await _context.Committees.FirstOrDefaultAsync(c => c.Name == "Test Committee");
        Assert.NotNull(savedCommittee);
        Assert.Equal("Test Description", savedCommittee.Description);
        Assert.Equal("Academic", savedCommittee.Type);
        Assert.True(savedCommittee.IsActive);
    }

    #endregion

    #region CommitteeMember Tests

    [Fact]
    public void CommitteeMember_Constructor_Should_Initialize_Properties()
    {
        // Act
        var member = new CommitteeMember();

        // Assert
        Assert.NotNull(member);
        Assert.Equal(0, member.Id);
        Assert.Equal(string.Empty, member.CommitteeName);
        Assert.Equal(0, member.AcademicEmpNr);
        Assert.Null(member.Role);
        Assert.Null(member.StartDate);
        Assert.Null(member.EndDate);
        Assert.True(member.IsActive);
    }

    [Fact]
    public void CommitteeMember_Properties_Should_Set_And_Get_Correctly()
    {
        // Arrange
        var member = new CommitteeMember();
        var startDate = DateTime.Now.Date;
        var endDate = DateTime.Now.AddYears(1).Date;

        // Act
        member.CommitteeName = "Academic Standards Committee";
        member.AcademicEmpNr = 12345;
        member.Role = "Chair";
        member.StartDate = startDate;
        member.EndDate = endDate;
        member.IsActive = false;

        // Assert
        Assert.Equal("Academic Standards Committee", member.CommitteeName);
        Assert.Equal(12345, member.AcademicEmpNr);
        Assert.Equal("Chair", member.Role);
        Assert.Equal(startDate, member.StartDate);
        Assert.Equal(endDate, member.EndDate);
        Assert.False(member.IsActive);
    }

    [Fact]
    public async Task CommitteeMember_Should_Be_Added_To_Database()
    {
        // Arrange
        // First create a committee
        var committee = new Committee
        {
            Name = "Test Committee",
            Description = "Test Description",
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };
        _context.Committees.Add(committee);
        await _context.SaveChangesAsync();

        var member = new CommitteeMember
        {
            CommitteeName = "Test Committee",
            AcademicEmpNr = 12345,
            Role = "Member",
            StartDate = DateTime.Now.Date,
            IsActive = true,
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };

        // Act
        _context.CommitteeMembers.Add(member);
        await _context.SaveChangesAsync();

        // Assert
        var savedMember = await _context.CommitteeMembers
            .FirstOrDefaultAsync(cm => cm.CommitteeName == "Test Committee" && cm.AcademicEmpNr == 12345);
        Assert.NotNull(savedMember);
        Assert.Equal("Member", savedMember.Role);
        Assert.True(savedMember.IsActive);
    }

    [Fact]
    public async Task CommitteeMember_With_Navigation_Properties_Should_Load_Correctly()
    {
        // Arrange
        var committee = new Committee
        {
            Name = "Test Committee Nav",
            Description = "Test Description",
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };
        _context.Committees.Add(committee);

        var member = new CommitteeMember
        {
            CommitteeName = "Test Committee Nav",
            AcademicEmpNr = 12345,
            Role = "Member",
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };
        _context.CommitteeMembers.Add(member);
        await _context.SaveChangesAsync();

        // Act
        var loaded = await _context.CommitteeMembers
            .Include(cm => cm.Committee)
            .Include(cm => cm.Academic)
            .FirstOrDefaultAsync(cm => cm.CommitteeName == "Test Committee Nav");

        // Assert
        Assert.NotNull(loaded);
        Assert.NotNull(loaded.Committee);
        Assert.NotNull(loaded.Academic);
    }

    #endregion

    #region TeacherRating Tests

    [Fact]
    public void TeacherRating_Constructor_Should_Initialize_Properties()
    {
        // Act
        var rating = new TeacherRating();

        // Assert
        Assert.NotNull(rating);
        Assert.Equal(0, rating.Id);
        Assert.Equal(0, rating.AcademicEmpNr);
        Assert.Equal(0, rating.RatingValue);
        Assert.Null(rating.SubjectCode);
        Assert.Null(rating.Semester);
        Assert.Null(rating.AcademicYear);
        Assert.Null(rating.Comments);
        Assert.Null(rating.RatingSource);
    }

    [Fact]
    public void TeacherRating_Properties_Should_Set_And_Get_Correctly()
    {
        // Arrange
        var rating = new TeacherRating();

        // Act
        rating.AcademicEmpNr = 12345;
        rating.RatingValue = 5;
        rating.SubjectCode = "CS101";
        rating.Semester = "Fall 2023";
        rating.AcademicYear = 2023;
        rating.Comments = "Excellent teaching";
        rating.RatingSource = "Student";

        // Assert
        Assert.Equal(12345, rating.AcademicEmpNr);
        Assert.Equal(5, rating.RatingValue);
        Assert.Equal("CS101", rating.SubjectCode);
        Assert.Equal("Fall 2023", rating.Semester);
        Assert.Equal(2023, rating.AcademicYear);
        Assert.Equal("Excellent teaching", rating.Comments);
        Assert.Equal("Student", rating.RatingSource);
    }

    [Fact]
    public async Task TeacherRating_Should_Be_Added_To_Database()
    {
        // Arrange
        var rating = new TeacherRating
        {
            AcademicEmpNr = 12345,
            RatingValue = 4,
            SubjectCode = "CS101",
            Semester = "Fall 2023",
            AcademicYear = 2023,
            Comments = "Good teacher",
            RatingSource = "Student",
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };

        // Act
        _context.TeacherRatings.Add(rating);
        await _context.SaveChangesAsync();

        // Assert
        var savedRating = await _context.TeacherRatings
            .FirstOrDefaultAsync(tr => tr.AcademicEmpNr == 12345 && tr.SubjectCode == "CS101");
        Assert.NotNull(savedRating);
        Assert.Equal(4, savedRating.RatingValue);
        Assert.Equal("Good teacher", savedRating.Comments);
    }

    [Fact]
    public async Task TeacherRating_With_Navigation_Properties_Should_Load_Correctly()
    {
        // Arrange
        var rating = new TeacherRating
        {
            AcademicEmpNr = 12345,
            RatingValue = 5,
            SubjectCode = "CS101",
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };
        _context.TeacherRatings.Add(rating);
        await _context.SaveChangesAsync();

        // Act
        var loaded = await _context.TeacherRatings
            .Include(tr => tr.Academic)
            .Include(tr => tr.Subject)
            .FirstOrDefaultAsync(tr => tr.AcademicEmpNr == 12345);

        // Assert
        Assert.NotNull(loaded);
        Assert.NotNull(loaded.Academic);
        Assert.NotNull(loaded.Subject);
    }

    #endregion

    #region StudentEnrollment Tests

    [Fact]
    public void StudentEnrollment_Constructor_Should_Initialize_Properties()
    {
        // Act
        var enrollment = new StudentEnrollment();

        // Assert
        Assert.NotNull(enrollment);
        Assert.Equal(0, enrollment.Id);
        Assert.Equal(0, enrollment.StudentEmpNr);
        Assert.Equal(string.Empty, enrollment.SubjectCode);
        Assert.Null(enrollment.Semester);
        Assert.Null(enrollment.AcademicYear);
        Assert.Null(enrollment.Grade);
        Assert.Equal("Enrolled", enrollment.Status);
        Assert.Null(enrollment.EnrollmentDate);
    }

    [Fact]
    public void StudentEnrollment_Properties_Should_Set_And_Get_Correctly()
    {
        // Arrange
        var enrollment = new StudentEnrollment();
        var enrollmentDate = DateTime.Now.Date;

        // Act
        enrollment.StudentEmpNr = 98765;
        enrollment.SubjectCode = "CS101";
        enrollment.Semester = "Fall 2023";
        enrollment.AcademicYear = 2023;
        enrollment.Grade = "A";
        enrollment.Status = "Completed";
        enrollment.EnrollmentDate = enrollmentDate;

        // Assert
        Assert.Equal(98765, enrollment.StudentEmpNr);
        Assert.Equal("CS101", enrollment.SubjectCode);
        Assert.Equal("Fall 2023", enrollment.Semester);
        Assert.Equal(2023, enrollment.AcademicYear);
        Assert.Equal("A", enrollment.Grade);
        Assert.Equal("Completed", enrollment.Status);
        Assert.Equal(enrollmentDate, enrollment.EnrollmentDate);
    }

    [Fact]
    public async Task StudentEnrollment_Should_Be_Added_To_Database()
    {
        // Arrange
        var enrollment = new StudentEnrollment
        {
            StudentEmpNr = 98765,
            SubjectCode = "CS101",
            Semester = "Fall 2023",
            AcademicYear = 2023,
            Status = "Enrolled",
            EnrollmentDate = DateTime.Now.Date,
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };

        // Act
        _context.StudentEnrollments.Add(enrollment);
        await _context.SaveChangesAsync();

        // Assert
        var savedEnrollment = await _context.StudentEnrollments
            .FirstOrDefaultAsync(se => se.StudentEmpNr == 98765 && se.SubjectCode == "CS101");
        Assert.NotNull(savedEnrollment);
        Assert.Equal("Enrolled", savedEnrollment.Status);
        Assert.Equal(2023, savedEnrollment.AcademicYear);
    }

    [Fact]
    public async Task StudentEnrollment_With_Navigation_Properties_Should_Load_Correctly()
    {
        // Arrange
        var enrollment = new StudentEnrollment
        {
            StudentEmpNr = 98765,
            SubjectCode = "CS101",
            Status = "Enrolled",
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };
        _context.StudentEnrollments.Add(enrollment);
        await _context.SaveChangesAsync();

        // Act
        var loaded = await _context.StudentEnrollments
            .Include(se => se.Student)
            .Include(se => se.Subject)
            .FirstOrDefaultAsync(se => se.StudentEmpNr == 98765);

        // Assert
        Assert.NotNull(loaded);
        Assert.NotNull(loaded.Student);
        Assert.NotNull(loaded.Subject);
    }

    #endregion

    #region Teaching Tests

    [Fact]
    public void Teaching_Constructor_Should_Initialize_Properties()
    {
        // Act
        var teaching = new Teaching();

        // Assert
        Assert.NotNull(teaching);
        Assert.Equal(0, teaching.Id);
        Assert.Equal(0, teaching.AcademicEmpNr);
        Assert.Equal(string.Empty, teaching.SubjectCode);
        Assert.Null(teaching.Semester);
        Assert.Null(teaching.AcademicYear);
        Assert.Null(teaching.EnrollmentCount);
    }

    [Fact]
    public void Teaching_Properties_Should_Set_And_Get_Correctly()
    {
        // Arrange
        var teaching = new Teaching();

        // Act
        teaching.AcademicEmpNr = 12345;
        teaching.SubjectCode = "CS101";
        teaching.Semester = "Fall 2023";
        teaching.AcademicYear = 2023;
        teaching.EnrollmentCount = 30;

        // Assert
        Assert.Equal(12345, teaching.AcademicEmpNr);
        Assert.Equal("CS101", teaching.SubjectCode);
        Assert.Equal("Fall 2023", teaching.Semester);
        Assert.Equal(2023, teaching.AcademicYear);
        Assert.Equal(30, teaching.EnrollmentCount);
    }

    [Fact]
    public async Task Teaching_Should_Be_Added_To_Database()
    {
        // Arrange
        var teaching = new Teaching
        {
            AcademicEmpNr = 12345,
            SubjectCode = "CS101",
            Semester = "Fall 2023",
            AcademicYear = 2023,
            EnrollmentCount = 25,
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };

        // Act
        _context.Teachings.Add(teaching);
        await _context.SaveChangesAsync();

        // Assert
        var savedTeaching = await _context.Teachings
            .FirstOrDefaultAsync(t => t.AcademicEmpNr == 12345 && t.SubjectCode == "CS101");
        Assert.NotNull(savedTeaching);
        Assert.Equal(25, savedTeaching.EnrollmentCount);
        Assert.Equal("Fall 2023", savedTeaching.Semester);
    }

    [Fact]
    public async Task Teaching_With_Navigation_Properties_Should_Load_Correctly()
    {
        // Arrange
        var teaching = new Teaching
        {
            AcademicEmpNr = 12345,
            SubjectCode = "CS101",
            Semester = "Fall 2023",
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };
        _context.Teachings.Add(teaching);
        await _context.SaveChangesAsync();

        // Act
        var loaded = await _context.Teachings
            .Include(t => t.Academic)
            .Include(t => t.Subject)
            .FirstOrDefaultAsync(t => t.AcademicEmpNr == 12345);

        // Assert
        Assert.NotNull(loaded);
        Assert.NotNull(loaded.Academic);
        Assert.NotNull(loaded.Subject);
    }

    [Fact]
    public void Teaching_Navigation_Properties_Should_Initialize()
    {
        // Act
        var teaching = new Teaching();

        // Assert
        Assert.Null(teaching.Academic);
        Assert.Null(teaching.Subject);
    }

    #endregion

    public void Dispose()
    {
        _context?.Dispose();
        if (_serviceProvider is IDisposable disposableProvider)
        {
            disposableProvider.Dispose();
        }
    }
}