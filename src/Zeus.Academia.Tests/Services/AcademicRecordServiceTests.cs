using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Enums;
using Zeus.Academia.Infrastructure.Services;
using Zeus.Academia.Infrastructure.Services.Interfaces;

namespace Zeus.Academia.Tests.Services;

/// <summary>
/// Unit tests for AcademicRecordService (Prompt 4 Task 4)
/// Tests academic record management functionality including courses, grades, and transcripts
/// </summary>
public class AcademicRecordServiceTests : IDisposable
{
    private readonly AcademiaDbContext _context;
    private readonly Mock<ILogger<AcademicRecordService>> _loggerMock;
    private readonly AcademicRecordService _service;
    private readonly DbContextOptions<AcademiaDbContext> _options;

    public AcademicRecordServiceTests()
    {
        // Create in-memory database for testing
        _options = new DbContextOptionsBuilder<AcademiaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AcademiaDbContext(_options, Mock.Of<Microsoft.Extensions.Configuration.IConfiguration>());
        _loggerMock = new Mock<ILogger<AcademicRecordService>>();
        _service = new AcademicRecordService(_context, _loggerMock.Object);

        // Seed test data
        SeedTestData();
    }

    private void SeedTestData()
    {
        // Create test department
        var department = new Department
        {
            Name = "Computer Science",
            FullName = "Department of Computer Science",
            HeadEmpNr = null,
            IsActive = true,
            CreatedBy = "System",
            ModifiedBy = "System"
        };
        _context.Departments.Add(department);

        // Create test degree
        var degree = new Degree
        {
            Code = "BSCS",
            Title = "Bachelor of Science in Computer Science",
            Level = "Bachelor",
            TotalCreditHours = 120,
            DurationYears = 4,
            IsActive = true,
            CreatedBy = "System",
            ModifiedBy = "System"
        };
        _context.Degrees.Add(degree);

        // Create test student
        var student = new Student
        {
            EmpNr = 1001,
            Name = "John Doe",
            DepartmentName = "Computer Science",
            DegreeCode = "BSCS",
            EnrollmentStatus = EnrollmentStatus.Enrolled,
            IsActive = true,
            CreatedBy = "System",
            ModifiedBy = "System"
        };
        _context.Students.Add(student);

        // Create test subjects
        var subjects = new[]
        {
            new Subject
            {
                Code = "CS101",
                Title = "Introduction to Programming",
                CreditHours = 3,
                DepartmentName = "Computer Science",
                IsActive = true,
                CreatedBy = "System",
                ModifiedBy = "System"
            },
            new Subject
            {
                Code = "MATH201",
                Title = "Calculus I",
                CreditHours = 4,
                DepartmentName = "Mathematics",
                IsActive = true,
                CreatedBy = "System",
                ModifiedBy = "System"
            },
            new Subject
            {
                Code = "CS201",
                Title = "Data Structures",
                CreditHours = 3,
                DepartmentName = "Computer Science",
                IsActive = true,
                CreatedBy = "System",
                ModifiedBy = "System"
            }
        };
        _context.Subjects.AddRange(subjects);

        // Create test academic term
        var term = new AcademicTerm
        {
            TermCode = "FALL2024",
            TermName = "Fall 2024",
            TermType = TermType.Fall,
            AcademicYear = 2024,
            StartDate = new DateTime(2024, 8, 15),
            EndDate = new DateTime(2024, 12, 15),
            IsActive = true,
            CreatedBy = "System",
            ModifiedBy = "System"
        };
        _context.AcademicTerms.Add(term);

        _context.SaveChanges();
    }

    #region Course Enrollment Tests

    [Fact]
    public async Task EnrollStudentInCourseAsync_ValidData_ShouldCreateEnrollment()
    {
        // Arrange
        var studentEmpNr = 1001;
        var subjectCode = "CS101";

        // Act
        var result = await _service.EnrollStudentInCourseAsync(studentEmpNr, subjectCode);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(studentEmpNr, result.StudentEmpNr);
        Assert.Equal(subjectCode, result.SubjectCode);
        Assert.Equal(CourseEnrollmentStatus.Enrolled, result.Status);
        Assert.Equal(3.0m, result.CreditHours);
        Assert.False(result.IsAudit);
        Assert.True(result.CountsTowardDegree);
    }

    [Fact]
    public async Task EnrollStudentInCourseAsync_InvalidStudent_ShouldReturnNull()
    {
        // Arrange
        var invalidStudentEmpNr = 9999;
        var subjectCode = "CS101";

        // Act
        var result = await _service.EnrollStudentInCourseAsync(invalidStudentEmpNr, subjectCode);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task EnrollStudentInCourseAsync_InvalidSubject_ShouldReturnNull()
    {
        // Arrange
        var studentEmpNr = 1001;
        var invalidSubjectCode = "INVALID";

        // Act
        var result = await _service.EnrollStudentInCourseAsync(studentEmpNr, invalidSubjectCode);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task EnrollStudentInCourseAsync_DuplicateEnrollment_ShouldReturnNull()
    {
        // Arrange
        var studentEmpNr = 1001;
        var subjectCode = "CS101";

        // Enroll first time
        await _service.EnrollStudentInCourseAsync(studentEmpNr, subjectCode);

        // Act - try to enroll again
        var result = await _service.EnrollStudentInCourseAsync(studentEmpNr, subjectCode);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DropStudentFromCourseAsync_ValidEnrollment_ShouldUpdateStatus()
    {
        // Arrange
        var enrollment = await _service.EnrollStudentInCourseAsync(1001, "CS101");
        Assert.NotNull(enrollment);

        // Act
        var result = await _service.DropStudentFromCourseAsync(enrollment.Id, "Schedule conflict");

        // Assert
        Assert.True(result);

        var updatedEnrollment = await _context.CourseEnrollments.FindAsync(enrollment.Id);
        Assert.NotNull(updatedEnrollment);
        Assert.Equal(CourseEnrollmentStatus.Dropped, updatedEnrollment.Status);
        Assert.NotNull(updatedEnrollment.DropDate);
        Assert.Contains("Schedule conflict", updatedEnrollment.Notes ?? "");
    }

    [Fact]
    public async Task WithdrawStudentFromCourseAsync_ValidEnrollment_ShouldUpdateStatus()
    {
        // Arrange
        var enrollment = await _service.EnrollStudentInCourseAsync(1001, "CS101");
        Assert.NotNull(enrollment);

        // Act
        var result = await _service.WithdrawStudentFromCourseAsync(enrollment.Id, "Medical reasons");

        // Assert
        Assert.True(result);

        var updatedEnrollment = await _context.CourseEnrollments.FindAsync(enrollment.Id);
        Assert.NotNull(updatedEnrollment);
        Assert.Equal(CourseEnrollmentStatus.Withdrawn, updatedEnrollment.Status);
        Assert.NotNull(updatedEnrollment.WithdrawalDate);
        Assert.Contains("Medical reasons", updatedEnrollment.Notes ?? "");
    }

    [Fact]
    public async Task GetStudentEnrollmentsAsync_ValidStudent_ShouldReturnEnrollments()
    {
        // Arrange
        var studentEmpNr = 1001;
        await _service.EnrollStudentInCourseAsync(studentEmpNr, "CS101");
        await _service.EnrollStudentInCourseAsync(studentEmpNr, "MATH201");

        // Act
        var result = await _service.GetStudentEnrollmentsAsync(studentEmpNr);

        // Assert
        var enrollments = result.ToList();
        Assert.Equal(2, enrollments.Count);
        Assert.All(enrollments, e => Assert.Equal(studentEmpNr, e.StudentEmpNr));
    }

    #endregion

    #region Grade Management Tests

    [Fact]
    public async Task RecordGradeAsync_ValidData_ShouldCreateGrade()
    {
        // Arrange
        var enrollment = await _service.EnrollStudentInCourseAsync(1001, "CS101");
        Assert.NotNull(enrollment);

        // Act
        var result = await _service.RecordGradeAsync(enrollment.Id, GradeType.Final, "A", 94.5m, "Professor Smith");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(enrollment.Id, result.CourseEnrollmentId);
        Assert.Equal(GradeType.Final, result.GradeType);
        Assert.Equal("A", result.LetterGrade);
        Assert.Equal(94.5m, result.NumericGrade);
        Assert.Equal(4.0m, result.GradePoints);
        Assert.Equal(3.0m, result.CreditHours);
        Assert.Equal(12.0m, result.QualityPoints);
        Assert.True(result.IsFinal);
        Assert.Equal("Professor Smith", result.GradedBy);
    }

    [Fact]
    public async Task RecordGradeAsync_LetterGradeOnly_ShouldCalculateNumericGrade()
    {
        // Arrange
        var enrollment = await _service.EnrollStudentInCourseAsync(1001, "CS101");
        Assert.NotNull(enrollment);

        // Act
        var result = await _service.RecordGradeAsync(enrollment.Id, GradeType.Final, "B+");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("B+", result.LetterGrade);
        Assert.Equal(87m, result.NumericGrade);
        Assert.Equal(3.3m, result.GradePoints);
    }

    [Fact]
    public async Task RecordGradeAsync_NumericGradeOnly_ShouldCalculateLetterGrade()
    {
        // Arrange
        var enrollment = await _service.EnrollStudentInCourseAsync(1001, "CS101");
        Assert.NotNull(enrollment);

        // Act
        var result = await _service.RecordGradeAsync(enrollment.Id, GradeType.Final, null, 87.5m);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("B+", result.LetterGrade);
        Assert.Equal(87.5m, result.NumericGrade);
        Assert.Equal(3.3m, result.GradePoints);
    }

    [Fact]
    public async Task UpdateGradeAsync_ValidGrade_ShouldUpdateSuccessfully()
    {
        // Arrange
        var enrollment = await _service.EnrollStudentInCourseAsync(1001, "CS101");
        Assert.NotNull(enrollment);

        var grade = await _service.RecordGradeAsync(enrollment.Id, GradeType.Final, "B", 84m);
        Assert.NotNull(grade);

        // Act
        var result = await _service.UpdateGradeAsync(grade.Id, "A", 94m, "Grade corrected", "Professor Smith");

        // Assert
        Assert.True(result);

        var updatedGrade = await _context.Grades.FindAsync(grade.Id);
        Assert.NotNull(updatedGrade);
        Assert.Equal("A", updatedGrade.LetterGrade);
        Assert.Equal(94m, updatedGrade.NumericGrade);
        Assert.Equal(4.0m, updatedGrade.GradePoints);
        Assert.Equal("Grade corrected", updatedGrade.Comments);
        Assert.Equal(GradeStatus.Changed, updatedGrade.Status);
    }

    [Fact]
    public async Task GetFinalGradeAsync_ExistingFinalGrade_ShouldReturnGrade()
    {
        // Arrange
        var enrollment = await _service.EnrollStudentInCourseAsync(1001, "CS101");
        Assert.NotNull(enrollment);

        var expectedGrade = await _service.RecordGradeAsync(enrollment.Id, GradeType.Final, "A");
        Assert.NotNull(expectedGrade);

        // Act
        var result = await _service.GetFinalGradeAsync(enrollment.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedGrade.Id, result.Id);
        Assert.True(result.IsFinal);
    }

    #endregion

    #region GPA Calculation Tests

    [Fact]
    public async Task CalculateCumulativeGPAAsync_MultipleGrades_ShouldCalculateCorrectly()
    {
        // Arrange
        var studentEmpNr = 1001;

        // Enroll in courses and assign grades
        var enrollment1 = await _service.EnrollStudentInCourseAsync(studentEmpNr, "CS101"); // 3 credits
        var enrollment2 = await _service.EnrollStudentInCourseAsync(studentEmpNr, "MATH201"); // 4 credits

        Assert.NotNull(enrollment1);
        Assert.NotNull(enrollment2);

        await _service.RecordGradeAsync(enrollment1.Id, GradeType.Final, "A"); // 4.0 * 3 = 12 quality points
        await _service.RecordGradeAsync(enrollment2.Id, GradeType.Final, "B"); // 3.0 * 4 = 12 quality points

        // Act
        var result = await _service.CalculateCumulativeGPAAsync(studentEmpNr);

        // Assert
        // Total quality points: 12 + 12 = 24
        // Total credit hours: 3 + 4 = 7
        // GPA: 24 / 7 = 3.43 (rounded to 2 decimal places)
        Assert.Equal(3.43m, result);
    }

    [Fact]
    public async Task CalculateTermGPAAsync_ValidTerm_ShouldCalculateCorrectly()
    {
        // Arrange
        var studentEmpNr = 1001;
        var academicYear = DateTime.Now.Year;
        var semester = "Fall";

        var enrollment = await _service.EnrollStudentInCourseAsync(studentEmpNr, "CS101");
        Assert.NotNull(enrollment);

        // Update enrollment to specific term
        enrollment.AcademicYear = academicYear;
        enrollment.Semester = semester;
        await _context.SaveChangesAsync();

        await _service.RecordGradeAsync(enrollment.Id, GradeType.Final, "A"); // 4.0 GPA

        // Act
        var result = await _service.CalculateTermGPAAsync(studentEmpNr, academicYear, semester);

        // Assert
        Assert.Equal(4.0m, result);
    }

    [Fact]
    public async Task GetGPAHistoryAsync_MultipleTerms_ShouldReturnHistory()
    {
        // Arrange
        var studentEmpNr = 1001;

        // Create enrollments for different terms
        var enrollment1 = await _service.EnrollStudentInCourseAsync(studentEmpNr, "CS101");
        var enrollment2 = await _service.EnrollStudentInCourseAsync(studentEmpNr, "MATH201");

        Assert.NotNull(enrollment1);
        Assert.NotNull(enrollment2);

        // Set different terms
        enrollment1.AcademicYear = 2024;
        enrollment1.Semester = "Fall";
        enrollment2.AcademicYear = 2024;
        enrollment2.Semester = "Spring";
        await _context.SaveChangesAsync();

        await _service.RecordGradeAsync(enrollment1.Id, GradeType.Final, "A");
        await _service.RecordGradeAsync(enrollment2.Id, GradeType.Final, "B");

        // Act
        var result = await _service.GetGPAHistoryAsync(studentEmpNr);

        // Assert
        var history = result.ToList();
        Assert.Equal(2, history.Count);
        Assert.Contains(history, h => h.Semester == "Fall" && h.AcademicYear == 2024);
        Assert.Contains(history, h => h.Semester == "Spring" && h.AcademicYear == 2024);
    }

    #endregion

    #region Transcript Generation Tests

    [Fact]
    public async Task GenerateTranscriptAsync_ValidStudent_ShouldReturnTranscript()
    {
        // Arrange
        var studentEmpNr = 1001;

        var enrollment = await _service.EnrollStudentInCourseAsync(studentEmpNr, "CS101");
        Assert.NotNull(enrollment);

        await _service.RecordGradeAsync(enrollment.Id, GradeType.Final, "A");

        // Act
        var result = await _service.GenerateTranscriptAsync(studentEmpNr);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(studentEmpNr, result.Student.EmpNr);
        Assert.True(result.IsOfficial);
        Assert.Single(result.Enrollments);
        Assert.Single(result.Grades);
        Assert.Equal(4.0m, result.CumulativeGPA);
        Assert.Equal(3.0m, result.TotalCreditHours);
        Assert.Equal(12.0m, result.QualityPoints);
    }

    [Fact]
    public async Task GenerateUnofficialTranscriptAsync_ValidStudent_ShouldReturnUnofficialTranscript()
    {
        // Arrange
        var studentEmpNr = 1001;

        var enrollment = await _service.EnrollStudentInCourseAsync(studentEmpNr, "CS101");
        Assert.NotNull(enrollment);

        await _service.RecordGradeAsync(enrollment.Id, GradeType.Final, "A");

        // Act
        var result = await _service.GenerateUnofficialTranscriptAsync(studentEmpNr);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsOfficial);
        Assert.Equal(studentEmpNr, result.Student.EmpNr);
    }

    [Fact]
    public async Task GetTranscriptSummaryAsync_ValidStudent_ShouldReturnSummary()
    {
        // Arrange
        var studentEmpNr = 1001;

        var enrollment1 = await _service.EnrollStudentInCourseAsync(studentEmpNr, "CS101");
        var enrollment2 = await _service.EnrollStudentInCourseAsync(studentEmpNr, "MATH201");

        Assert.NotNull(enrollment1);
        Assert.NotNull(enrollment2);

        await _service.RecordGradeAsync(enrollment1.Id, GradeType.Final, "A");
        // Leave enrollment2 without final grade (in progress)

        // Act
        var result = await _service.GetTranscriptSummaryAsync(studentEmpNr);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(studentEmpNr, result.StudentEmpNr);
        Assert.Equal(2, result.TotalCourses);
        Assert.Equal(1, result.CompletedCourses);
        Assert.Equal(1, result.InProgressCourses);
        Assert.Equal(4.0m, result.CumulativeGPA);
        Assert.Equal(3.0m, result.TotalCreditHours);
    }

    #endregion

    #region Academic Honors and Awards Tests

    [Fact]
    public async Task AwardAcademicHonorAsync_ValidData_ShouldCreateHonor()
    {
        // Arrange
        var studentEmpNr = 1001;

        // Act
        var result = await _service.AwardAcademicHonorAsync(
            studentEmpNr,
            HonorType.DeansList,
            "Dean's List Fall 2024",
            "Outstanding academic performance",
            2024,
            "Fall",
            3.5m);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(studentEmpNr, result.StudentEmpNr);
        Assert.Equal(HonorType.DeansList, result.HonorType);
        Assert.Equal("Dean's List Fall 2024", result.Title);
        Assert.Equal("Outstanding academic performance", result.Description);
        Assert.Equal(2024, result.AcademicYear);
        Assert.Equal("Fall", result.Semester);
        Assert.Equal(3.5m, result.RequiredGPA);
        Assert.True(result.AppearsOnTranscript);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task GiveAwardAsync_ValidData_ShouldCreateAward()
    {
        // Arrange
        var studentEmpNr = 1001;

        // Act
        var result = await _service.GiveAwardAsync(
            studentEmpNr,
            AwardType.AcademicExcellence,
            "Outstanding Student Award",
            "Awarded for exceptional academic achievement",
            1000.00m,
            "Computer Science Department");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(studentEmpNr, result.StudentEmpNr);
        Assert.Equal(AwardType.AcademicExcellence, result.AwardType);
        Assert.Equal("Outstanding Student Award", result.Name);
        Assert.Equal("Awarded for exceptional academic achievement", result.Description);
        Assert.Equal(1000.00m, result.MonetaryValue);
        Assert.Equal("Computer Science Department", result.AwardingOrganization);
        Assert.True(result.AppearsOnTranscript);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task GetStudentAcademicHonorsAsync_ValidStudent_ShouldReturnHonors()
    {
        // Arrange
        var studentEmpNr = 1001;

        await _service.AwardAcademicHonorAsync(studentEmpNr, HonorType.DeansList, "Dean's List Fall 2024");
        await _service.AwardAcademicHonorAsync(studentEmpNr, HonorType.PresidentsList, "President's List Spring 2024");

        // Act
        var result = await _service.GetStudentAcademicHonorsAsync(studentEmpNr);

        // Assert
        var honors = result.ToList();
        Assert.Equal(2, honors.Count);
        Assert.All(honors, h => Assert.Equal(studentEmpNr, h.StudentEmpNr));
        Assert.All(honors, h => Assert.True(h.IsActive));
    }

    [Fact]
    public async Task GetStudentAwardsAsync_ValidStudent_ShouldReturnAwards()
    {
        // Arrange
        var studentEmpNr = 1001;

        await _service.GiveAwardAsync(studentEmpNr, AwardType.AcademicExcellence, "Excellence Award");
        await _service.GiveAwardAsync(studentEmpNr, AwardType.Leadership, "Leadership Award");

        // Act
        var result = await _service.GetStudentAwardsAsync(studentEmpNr);

        // Assert
        var awards = result.ToList();
        Assert.Equal(2, awards.Count);
        Assert.All(awards, a => Assert.Equal(studentEmpNr, a.StudentEmpNr));
        Assert.All(awards, a => Assert.True(a.IsActive));
    }

    #endregion

    #region Degree Progress Tests

    [Fact]
    public async Task UpdateDegreeProgressAsync_ValidData_ShouldUpdateProgress()
    {
        // Arrange
        var studentEmpNr = 1001;
        var degreeCode = "BSCS"; // Should match our test degree

        // Create some completed coursework
        var enrollment = await _service.EnrollStudentInCourseAsync(studentEmpNr, "CS101");
        Assert.NotNull(enrollment);

        await _service.RecordGradeAsync(enrollment.Id, GradeType.Final, "A");

        // Act
        var result = await _service.UpdateDegreeProgressAsync(studentEmpNr, degreeCode);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(studentEmpNr, result.StudentEmpNr);
        Assert.Equal(degreeCode, result.DegreeCode);
        Assert.Equal(120, result.RequiredCreditHours);
        Assert.Equal(3.0m, result.CompletedCreditHours);
        Assert.Equal(117.0m, result.RemainingCreditHours);
        Assert.Equal(2.5m, result.CompletionPercentage); // 3/120 * 100 = 2.5%
        Assert.Equal(4.0m, result.CumulativeGPA);
        Assert.True(result.MeetsGPARequirement);
        Assert.NotNull(result.ExpectedGraduationDate);
    }

    [Fact]
    public async Task GetDegreeProgressAsync_ExistingProgress_ShouldReturnProgress()
    {
        // Arrange
        var studentEmpNr = 1001;
        var degreeCode = "BSCS";

        var createdProgress = await _service.UpdateDegreeProgressAsync(studentEmpNr, degreeCode);
        Assert.NotNull(createdProgress);

        // Act
        var result = await _service.GetDegreeProgressAsync(studentEmpNr);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(studentEmpNr, result.StudentEmpNr);
        Assert.Equal(degreeCode, result.DegreeCode);
    }

    [Fact]
    public async Task CheckGraduationEligibilityAsync_InsufficientCredits_ShouldReturnNotEligible()
    {
        // Arrange
        var studentEmpNr = 1001;
        var degreeCode = "BSCS";

        // Create minimal progress
        var enrollment = await _service.EnrollStudentInCourseAsync(studentEmpNr, "CS101");
        Assert.NotNull(enrollment);

        await _service.RecordGradeAsync(enrollment.Id, GradeType.Final, "A");
        await _service.UpdateDegreeProgressAsync(studentEmpNr, degreeCode);

        // Act
        var result = await _service.CheckGraduationEligibilityAsync(studentEmpNr);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(studentEmpNr, result.StudentEmpNr);
        Assert.False(result.IsEligible);
        Assert.Contains(result.UnmetRequirements, r => r.Contains("credit hours remaining"));
        Assert.Equal(117.0m, result.RemainingCreditHours);
        Assert.True(result.MeetsGPARequirement);
    }

    #endregion

    #region Utility Method Tests

    [Fact]
    public void ConvertLetterGradeToPoints_ValidGrades_ShouldReturnCorrectPoints()
    {
        // Act & Assert
        Assert.Equal(4.0m, _service.ConvertLetterGradeToPoints("A+"));
        Assert.Equal(4.0m, _service.ConvertLetterGradeToPoints("A"));
        Assert.Equal(3.7m, _service.ConvertLetterGradeToPoints("A-"));
        Assert.Equal(3.3m, _service.ConvertLetterGradeToPoints("B+"));
        Assert.Equal(3.0m, _service.ConvertLetterGradeToPoints("B"));
        Assert.Equal(2.7m, _service.ConvertLetterGradeToPoints("B-"));
        Assert.Equal(2.3m, _service.ConvertLetterGradeToPoints("C+"));
        Assert.Equal(2.0m, _service.ConvertLetterGradeToPoints("C"));
        Assert.Equal(1.7m, _service.ConvertLetterGradeToPoints("C-"));
        Assert.Equal(1.3m, _service.ConvertLetterGradeToPoints("D+"));
        Assert.Equal(1.0m, _service.ConvertLetterGradeToPoints("D"));
        Assert.Equal(0.7m, _service.ConvertLetterGradeToPoints("D-"));
        Assert.Equal(0.0m, _service.ConvertLetterGradeToPoints("F"));
    }

    [Fact]
    public void ConvertNumericGradeToLetter_ValidGrades_ShouldReturnCorrectLetters()
    {
        // Act & Assert
        Assert.Equal("A+", _service.ConvertNumericGradeToLetter(98m));
        Assert.Equal("A", _service.ConvertNumericGradeToLetter(95m));
        Assert.Equal("A-", _service.ConvertNumericGradeToLetter(91m));
        Assert.Equal("B+", _service.ConvertNumericGradeToLetter(88m));
        Assert.Equal("B", _service.ConvertNumericGradeToLetter(85m));
        Assert.Equal("B-", _service.ConvertNumericGradeToLetter(82m));
        Assert.Equal("C+", _service.ConvertNumericGradeToLetter(78m));
        Assert.Equal("C", _service.ConvertNumericGradeToLetter(75m));
        Assert.Equal("C-", _service.ConvertNumericGradeToLetter(72m));
        Assert.Equal("D+", _service.ConvertNumericGradeToLetter(68m));
        Assert.Equal("D", _service.ConvertNumericGradeToLetter(65m));
        Assert.Equal("D-", _service.ConvertNumericGradeToLetter(62m));
        Assert.Equal("F", _service.ConvertNumericGradeToLetter(59m));
    }

    [Fact]
    public void DetermineAcademicStanding_VariousGPAs_ShouldReturnCorrectStanding()
    {
        // Act & Assert
        Assert.Equal(AcademicStanding.DeansListqualification, _service.DetermineAcademicStanding(3.8m, 12m));
        Assert.Equal(AcademicStanding.Good, _service.DetermineAcademicStanding(3.5m, 12m));
        Assert.Equal(AcademicStanding.Good, _service.DetermineAcademicStanding(2.5m, 12m));
        Assert.Equal(AcademicStanding.Warning, _service.DetermineAcademicStanding(1.7m, 12m));
        Assert.Equal(AcademicStanding.Probation, _service.DetermineAcademicStanding(1.2m, 12m));
        Assert.Equal(AcademicStanding.AcademicSuspension, _service.DetermineAcademicStanding(0.8m, 12m));
    }

    #endregion

    public void Dispose()
    {
        _context.Dispose();
    }
}