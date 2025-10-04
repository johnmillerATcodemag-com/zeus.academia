using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Services;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Enums;
using Zeus.Academia.Infrastructure.Models;
using System.ComponentModel.DataAnnotations;

namespace Zeus.Academia.Api.UnitTests.Services;

/// <summary>
/// Comprehensive tests for Task 7: Course Services and Business Logic
/// Tests course business operations, equivalency management, transfer credit evaluation,
/// capacity management, and course analytics services.
/// </summary>
public class CourseServicesTests
{
    private readonly Mock<AcademiaDbContext> _mockContext;
    private readonly Mock<ILogger<CourseService>> _mockCourseLogger;
    private readonly Mock<ILogger<TransferCreditService>> _mockTransferLogger;
    private readonly Mock<ILogger<CourseAnalyticsService>> _mockAnalyticsLogger;
    private readonly CourseService _courseService;
    private readonly TransferCreditService _transferCreditService;
    private readonly CourseAnalyticsService _analyticsService;

    public CourseServicesTests()
    {
        var options = new DbContextOptionsBuilder<AcademiaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _mockContext = new Mock<AcademiaDbContext>(options);
        _mockCourseLogger = new Mock<ILogger<CourseService>>();
        _mockTransferLogger = new Mock<ILogger<TransferCreditService>>();
        _mockAnalyticsLogger = new Mock<ILogger<CourseAnalyticsService>>();

        _courseService = new CourseService(_mockContext.Object, _mockCourseLogger.Object);
        _transferCreditService = new TransferCreditService(_mockContext.Object, _mockTransferLogger.Object);
        _analyticsService = new CourseAnalyticsService(_mockContext.Object, _mockAnalyticsLogger.Object);
    }

    #region Course Service Business Operations Tests

    [Fact]
    public async Task CourseService_Should_Handle_Complex_Course_Creation()
    {
        // Arrange
        var newCourse = new Course
        {
            CourseNumber = "CS401",
            Title = "Advanced Software Engineering",
            Description = "Advanced topics in software engineering including design patterns, architecture, and project management.",
            SubjectCode = "CS",
            CreditHours = 4,
            ContactHours = 3,
            Level = CourseLevel.Undergraduate,
            Status = CourseStatus.Active,
            MaxEnrollment = 30,
            Prerequisites = new List<CoursePrerequisite>
            {
                new CoursePrerequisite { RequiredCourseNumber = "CS301", LogicType = PrerequisiteLogicType.Required },
                new CoursePrerequisite { RequiredCourseNumber = "CS302", LogicType = PrerequisiteLogicType.Required }
            },
            LearningOutcomes = new List<string>
            {
                "Apply advanced software design patterns",
                "Design scalable software architectures",
                "Manage software development projects"
            }
        };

        SetupMockDbSetForCourse(new List<Course>());
        _mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

        // Act
        var result = await _courseService.CreateCourseAsync(newCourse);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("CS401", result.CourseNumber);
        Assert.Equal("Advanced Software Engineering", result.Title);
        Assert.Equal(2, result.Prerequisites.Count);
        Assert.All(result.Prerequisites, prereq =>
            Assert.Contains(prereq.RequiredCourseNumber, new[] { "CS301", "CS302" }));
    }

    [Fact]
    public async Task ValidatePrerequisites_Should_Check_Complex_Requirements()
    {
        // Arrange
        var studentId = 1;
        var courseId = 401;

        var student = new Student { Id = studentId, FirstName = "John", LastName = "Doe" };
        var targetCourse = new Course
        {
            Id = courseId,
            CourseNumber = "CS401",
            Prerequisites = new List<CoursePrerequisite>
            {
                new CoursePrerequisite { RequiredCourseNumber = "CS301", LogicType = PrerequisiteLogicType.Required },
                new CoursePrerequisite { RequiredCourseNumber = "CS302", LogicType = PrerequisiteLogicType.Or },
                new CoursePrerequisite { RequiredCourseNumber = "CS303", LogicType = PrerequisiteLogicType.Or }
            }
        };

        var studentEnrollments = new List<CourseEnrollment>
        {
            new CourseEnrollment { StudentEmpNr = studentId, SubjectCode = "CS301", FinalGrade = "A" },
            new CourseEnrollment { StudentEmpNr = studentId, SubjectCode = "CS302", FinalGrade = "B+" }
        };

        SetupMockDbSetForStudent(new List<Student> { student });
        SetupMockDbSetForCourse(new List<Course> { targetCourse });
        SetupMockDbSetForEnrollment(studentEnrollments);

        // Act
        var result = await _courseService.ValidatePrerequisitesAsync(studentId, courseId);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsValid);
        Assert.True(result.RequiredPrerequisitesMet);
        Assert.True(result.OrPrerequisitesMet);
        Assert.Empty(result.MissingPrerequisites);
    }

    [Fact]
    public async Task ValidatePrerequisites_Should_Identify_Missing_Requirements()
    {
        // Arrange
        var studentId = 1;
        var courseId = 401;

        var targetCourse = new Course
        {
            Id = courseId,
            CourseNumber = "CS401",
            Prerequisites = new List<CoursePrerequisite>
            {
                new CoursePrerequisite { RequiredCourseNumber = "CS301", LogicType = PrerequisiteLogicType.Required },
                new CoursePrerequisite { RequiredCourseNumber = "CS302", LogicType = PrerequisiteLogicType.Required }
            }
        };

        // Student only completed CS301, missing CS302
        var studentEnrollments = new List<CourseEnrollment>
        {
            new CourseEnrollment { StudentEmpNr = studentId, SubjectCode = "CS301", FinalGrade = "A" }
        };

        SetupMockDbSetForCourse(new List<Course> { targetCourse });
        SetupMockDbSetForEnrollment(studentEnrollments);

        // Act
        var result = await _courseService.ValidatePrerequisitesAsync(studentId, courseId);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsValid);
        Assert.False(result.RequiredPrerequisitesMet);
        Assert.Contains("CS302", result.MissingPrerequisites);
        Assert.Single(result.MissingPrerequisites);
    }

    [Fact]
    public async Task RecommendCourses_Should_Return_Relevant_Suggestions()
    {
        // Arrange
        var studentId = 1;
        var student = new Student
        {
            Id = studentId,
            FirstName = "Jane",
            LastName = "Smith",
            MajorCode = "CS_BS"
        };

        var completedEnrollments = new List<CourseEnrollment>
        {
            new CourseEnrollment { StudentEmpNr = studentId, SubjectCode = "CS101", FinalGrade = "A" },
            new CourseEnrollment { StudentEmpNr = studentId, SubjectCode = "CS201", FinalGrade = "B+" },
            new CourseEnrollment { StudentEmpNr = studentId, SubjectCode = "MATH201", FinalGrade = "A-" }
        };

        var availableCourses = new List<Course>
        {
            new Course { Id = 301, CourseNumber = "CS301", Title = "Data Structures", SubjectCode = "CS", Level = CourseLevel.Undergraduate },
            new Course { Id = 302, CourseNumber = "CS302", Title = "Algorithms", SubjectCode = "CS", Level = CourseLevel.Undergraduate },
            new Course { Id = 401, CourseNumber = "CS401", Title = "Software Engineering", SubjectCode = "CS", Level = CourseLevel.Undergraduate },
            new Course { Id = 105, CourseNumber = "ART105", Title = "Art History", SubjectCode = "ART", Level = CourseLevel.Undergraduate }
        };

        SetupMockDbSetForStudent(new List<Student> { student });
        SetupMockDbSetForCourse(availableCourses);
        SetupMockDbSetForEnrollment(completedEnrollments);

        // Act
        var recommendations = await _courseService.RecommendCoursesAsync(studentId, 5);

        // Assert
        Assert.NotNull(recommendations);
        Assert.NotEmpty(recommendations);

        // Should prioritize CS courses for CS major
        var csCourses = recommendations.Where(r => r.Course.SubjectCode == "CS").ToList();
        Assert.NotEmpty(csCourses);

        // Should have relevance scores
        Assert.All(recommendations, rec => Assert.True(rec.RelevanceScore > 0));

        // Should be ordered by relevance
        var scores = recommendations.Select(r => r.RelevanceScore).ToList();
        Assert.Equal(scores.OrderByDescending(s => s), scores);
    }

    [Fact]
    public async Task CheckDegreeProgress_Should_Calculate_Accurate_Completion()
    {
        // Arrange
        var studentId = 1;
        var degreeCode = "CS_BS";

        var student = new Student
        {
            Id = studentId,
            MajorCode = degreeCode,
            FirstName = "Alice",
            LastName = "Johnson"
        };

        var completedCourses = new List<CourseEnrollment>
        {
            new CourseEnrollment { StudentEmpNr = studentId, SubjectCode = "CS101", CreditHours = 3, FinalGrade = "A" },
            new CourseEnrollment { StudentEmpNr = studentId, SubjectCode = "CS201", CreditHours = 4, FinalGrade = "B+" },
            new CourseEnrollment { StudentEmpNr = studentId, SubjectCode = "MATH201", CreditHours = 4, FinalGrade = "A-" },
            new CourseEnrollment { StudentEmpNr = studentId, SubjectCode = "ENG101", CreditHours = 3, FinalGrade = "B" }
        };

        var degreeRequirements = new DegreeRequirement
        {
            DegreeCode = degreeCode,
            TotalCreditsRequired = 120,
            MajorCreditsRequired = 60,
            GeneralEducationCreditsRequired = 42,
            ElectiveCreditsRequired = 18
        };

        SetupMockDbSetForStudent(new List<Student> { student });
        SetupMockDbSetForEnrollment(completedCourses);
        SetupMockDbSetForDegreeRequirement(new List<DegreeRequirement> { degreeRequirements });

        // Act
        var progress = await _courseService.CheckDegreeProgressAsync(studentId, degreeCode);

        // Assert
        Assert.NotNull(progress);
        Assert.Equal(14, progress.TotalCreditsCompleted); // 3+4+4+3
        Assert.Equal(120, progress.TotalCreditsRequired);
        Assert.Equal(106, progress.RemainingCreditsNeeded);
        Assert.True(progress.CompletionPercentage > 0);
        Assert.True(progress.CompletionPercentage < 100);
        Assert.NotNull(progress.CategoryProgress);
    }

    #endregion

    #region Course Equivalency Management Tests

    [Fact]
    public async Task CourseEquivalency_Should_Handle_Cross_Institution_Mapping()
    {
        // Arrange
        var externalCourse = new ExternalCourse
        {
            InstitutionCode = "TRANSFER_U",
            CourseNumber = "COMP-SCI-101",
            Title = "Introduction to Programming",
            CreditHours = 3,
            Description = "Basic programming concepts"
        };

        var internalCourse = new Course
        {
            Id = 101,
            CourseNumber = "CS101",
            Title = "Programming Fundamentals",
            CreditHours = 3,
            SubjectCode = "CS"
        };

        var equivalency = new CourseEquivalency
        {
            ExternalCourse = externalCourse,
            InternalCourseId = internalCourse.Id,
            EquivalencyType = EquivalencyType.Direct,
            EffectiveDate = DateTime.UtcNow.AddYears(-1),
            ApprovedBy = "Department Chair"
        };

        SetupMockDbSetForEquivalency(new List<CourseEquivalency> { equivalency });
        SetupMockDbSetForCourse(new List<Course> { internalCourse });

        // Act
        var result = await _courseService.FindCourseEquivalencyAsync(externalCourse);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(EquivalencyType.Direct, result.EquivalencyType);
        Assert.Equal(internalCourse.Id, result.InternalCourseId);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task CourseEquivalency_Should_Handle_Partial_Equivalencies()
    {
        // Arrange
        var externalCourse = new ExternalCourse
        {
            InstitutionCode = "TRANSFER_U",
            CourseNumber = "MATH-150",
            Title = "College Algebra",
            CreditHours = 4,
            Description = "Intermediate algebra topics"
        };

        var equivalency = new CourseEquivalency
        {
            ExternalCourse = externalCourse,
            InternalCourseId = 201,
            EquivalencyType = EquivalencyType.Partial,
            CreditHoursAwarded = 3, // 4 external credits = 3 internal credits
            Conditions = "Must complete additional assessment",
            EffectiveDate = DateTime.UtcNow.AddYears(-1)
        };

        SetupMockDbSetForEquivalency(new List<CourseEquivalency> { equivalency });

        // Act
        var result = await _courseService.FindCourseEquivalencyAsync(externalCourse);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(EquivalencyType.Partial, result.EquivalencyType);
        Assert.Equal(3, result.CreditHoursAwarded);
        Assert.Equal("Must complete additional assessment", result.Conditions);
    }

    #endregion

    #region Transfer Credit Service Tests

    [Fact]
    public async Task TransferCreditService_Should_Evaluate_Transfer_Credits_Automatically()
    {
        // Arrange
        var studentId = 1;
        var transferRequest = new TransferCreditRequest
        {
            StudentId = studentId,
            SourceInstitution = "Community College",
            OfficialTranscript = "transcript_data",
            ExternalCourses = new List<ExternalCourse>
            {
                new ExternalCourse
                {
                    InstitutionCode = "CC_STATE",
                    CourseNumber = "ENG-101",
                    Title = "English Composition I",
                    CreditHours = 3,
                    Grade = "A"
                },
                new ExternalCourse
                {
                    InstitutionCode = "CC_STATE",
                    CourseNumber = "MATH-110",
                    Title = "College Algebra",
                    CreditHours = 4,
                    Grade = "B+"
                }
            }
        };

        var equivalencies = new List<CourseEquivalency>
        {
            new CourseEquivalency
            {
                ExternalCourse = transferRequest.ExternalCourses[0],
                InternalCourseId = 101,
                EquivalencyType = EquivalencyType.Direct,
                CreditHoursAwarded = 3
            },
            new CourseEquivalency
            {
                ExternalCourse = transferRequest.ExternalCourses[1],
                InternalCourseId = 201,
                EquivalencyType = EquivalencyType.Partial,
                CreditHoursAwarded = 3,
                Conditions = "Placement test required"
            }
        };

        SetupMockDbSetForEquivalency(equivalencies);

        // Act
        var evaluation = await _transferCreditService.EvaluateTransferCreditsAsync(transferRequest);

        // Assert
        Assert.NotNull(evaluation);
        Assert.Equal(studentId, evaluation.StudentId);
        Assert.Equal(2, evaluation.EvaluatedCourses.Count);

        var directTransfer = evaluation.EvaluatedCourses.First(e => e.EquivalencyType == EquivalencyType.Direct);
        Assert.Equal(3, directTransfer.CreditsAwarded);
        Assert.Equal(TransferStatus.Approved, directTransfer.Status);

        var partialTransfer = evaluation.EvaluatedCourses.First(e => e.EquivalencyType == EquivalencyType.Partial);
        Assert.Equal(3, partialTransfer.CreditsAwarded);
        Assert.Equal(TransferStatus.ConditionalApproval, partialTransfer.Status);
        Assert.Contains("Placement test", partialTransfer.Conditions);
    }

    [Fact]
    public async Task TransferCreditService_Should_Handle_No_Equivalency_Found()
    {
        // Arrange
        var transferRequest = new TransferCreditRequest
        {
            StudentId = 1,
            ExternalCourses = new List<ExternalCourse>
            {
                new ExternalCourse
                {
                    InstitutionCode = "UNKNOWN_U",
                    CourseNumber = "RARE-999",
                    Title = "Uncommon Subject",
                    CreditHours = 3,
                    Grade = "A"
                }
            }
        };

        SetupMockDbSetForEquivalency(new List<CourseEquivalency>()); // No equivalencies

        // Act
        var evaluation = await _transferCreditService.EvaluateTransferCreditsAsync(transferRequest);

        // Assert
        Assert.NotNull(evaluation);
        Assert.Single(evaluation.EvaluatedCourses);

        var noEquivalency = evaluation.EvaluatedCourses.First();
        Assert.Equal(TransferStatus.PendingReview, noEquivalency.Status);
        Assert.Equal(0, noEquivalency.CreditsAwarded);
        Assert.Contains("No equivalency found", noEquivalency.Conditions);
    }

    [Fact]
    public async Task TransferCreditService_Should_Apply_Institution_Policies()
    {
        // Arrange
        var transferRequest = new TransferCreditRequest
        {
            StudentId = 1,
            ExternalCourses = new List<ExternalCourse>
            {
                new ExternalCourse
                {
                    CourseNumber = "OLD-COURSE",
                    CompletionDate = DateTime.UtcNow.AddYears(-8), // 8 years old
                    Grade = "C-" // Below minimum grade
                }
            }
        };

        var transferPolicies = new TransferCreditPolicies
        {
            MaximumCourseAge = 7, // 7 years maximum
            MinimumGrade = "C",
            MaximumTransferCredits = 60
        };

        // Act
        var evaluation = await _transferCreditService.EvaluateTransferCreditsAsync(transferRequest, transferPolicies);

        // Assert
        Assert.NotNull(evaluation);
        var rejectedCourse = evaluation.EvaluatedCourses.First();
        Assert.Equal(TransferStatus.Rejected, rejectedCourse.Status);
        Assert.Contains("course age exceeds policy", rejectedCourse.Conditions.ToLower());
        Assert.Contains("grade below minimum", rejectedCourse.Conditions.ToLower());
    }

    #endregion

    #region Course Capacity and Availability Management Tests

    [Fact]
    public async Task CapacityManagement_Should_Track_Available_Seats()
    {
        // Arrange
        var courseId = 101;
        var course = new Course
        {
            Id = courseId,
            CourseNumber = "CS101",
            MaxEnrollment = 30
        };

        var enrollments = new List<CourseEnrollment>
        {
            new CourseEnrollment { Id = 1, StudentEmpNr = 1 },
            new CourseEnrollment { Id = 2, StudentEmpNr = 2 },
            new CourseEnrollment { Id = 3, StudentEmpNr = 3 }
        };

        SetupMockDbSetForCourse(new List<Course> { course });
        SetupMockDbSetForEnrollment(enrollments);

        // Act
        var availability = await _courseService.GetCourseAvailabilityAsync(courseId);

        // Assert
        Assert.NotNull(availability);
        Assert.Equal(30, availability.MaxCapacity);
        Assert.Equal(3, availability.CurrentEnrollment);
        Assert.Equal(27, availability.AvailableSeats);
        Assert.False(availability.IsFull);
        Assert.True(availability.HasWaitlist == false || availability.WaitlistCount == 0);
    }

    [Fact]
    public async Task CapacityManagement_Should_Handle_Waitlist_When_Full()
    {
        // Arrange
        var courseId = 101;
        var course = new Course
        {
            Id = courseId,
            CourseNumber = "CS101",
            MaxEnrollment = 2 // Small capacity for testing
        };

        var enrollments = new List<CourseEnrollment>
        {
            new CourseEnrollment { Id = 1, StudentEmpNr = 1 },
            new CourseEnrollment { Id = 2, StudentEmpNr = 2 }
        };

        var waitlistEntries = new List<CourseWaitlist>
        {
            new CourseWaitlist { CourseId = courseId, StudentId = 3, Position = 1 },
            new CourseWaitlist { CourseId = courseId, StudentId = 4, Position = 2 }
        };

        SetupMockDbSetForCourse(new List<Course> { course });
        SetupMockDbSetForEnrollment(enrollments);
        SetupMockDbSetForWaitlist(waitlistEntries);

        // Act
        var availability = await _courseService.GetCourseAvailabilityAsync(courseId);

        // Assert
        Assert.NotNull(availability);
        Assert.Equal(2, availability.MaxCapacity);
        Assert.Equal(2, availability.CurrentEnrollment);
        Assert.Equal(0, availability.AvailableSeats);
        Assert.True(availability.IsFull);
        Assert.True(availability.HasWaitlist);
        Assert.Equal(2, availability.WaitlistCount);
    }

    [Fact]
    public async Task CapacityManagement_Should_Handle_Enrollment_Request()
    {
        // Arrange
        var studentId = 1;
        var courseId = 101;
        var enrollmentRequest = new EnrollmentRequest
        {
            StudentId = studentId,
            CourseId = courseId,
            RequestedSemester = "Fall 2024"
        };

        var course = new Course
        {
            Id = courseId,
            MaxEnrollment = 30,
            Prerequisites = new List<CoursePrerequisite>()
        };

        SetupMockDbSetForCourse(new List<Course> { course });
        SetupMockDbSetForEnrollment(new List<CourseEnrollment>()); // No current enrollments
        _mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

        // Act
        var result = await _courseService.ProcessEnrollmentRequestAsync(enrollmentRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(EnrollmentStatus.Approved, result.Status);
        Assert.Equal("Enrollment approved", result.Message);
        Assert.Null(result.WaitlistPosition);
    }

    [Fact]
    public async Task CapacityManagement_Should_Add_To_Waitlist_When_Full()
    {
        // Arrange
        var studentId = 3;
        var courseId = 101;
        var enrollmentRequest = new EnrollmentRequest
        {
            StudentId = studentId,
            CourseId = courseId,
            RequestedSemester = "Fall 2024"
        };

        var course = new Course
        {
            Id = courseId,
            MaxEnrollment = 2,
            Prerequisites = new List<CoursePrerequisite>()
        };

        var existingEnrollments = new List<CourseEnrollment>
        {
            new CourseEnrollment { StudentEmpNr = 1 },
            new CourseEnrollment { StudentEmpNr = 2 }
        };

        var existingWaitlist = new List<CourseWaitlist>();

        SetupMockDbSetForCourse(new List<Course> { course });
        SetupMockDbSetForEnrollment(existingEnrollments);
        SetupMockDbSetForWaitlist(existingWaitlist);
        _mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

        // Act
        var result = await _courseService.ProcessEnrollmentRequestAsync(enrollmentRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(EnrollmentStatus.Waitlisted, result.Status);
        Assert.Contains("added to waitlist", result.Message.ToLower());
        Assert.Equal(1, result.WaitlistPosition);
    }

    #endregion

    #region Course Analytics Service Tests

    [Fact]
    public async Task CourseAnalytics_Should_Provide_Enrollment_Patterns()
    {
        // Arrange
        var courseId = 101;
        var historicalEnrollments = new List<CourseEnrollment>
        {
            new CourseEnrollment { Id = 1, AcademicYear = 2023, Semester = "Fall" },
            new CourseEnrollment { Id = 2, AcademicYear = 2023, Semester = "Fall" },
            new CourseEnrollment { Id = 3, AcademicYear = 2023, Semester = "Spring" },
            new CourseEnrollment { Id = 4, AcademicYear = 2024, Semester = "Fall" },
            new CourseEnrollment { Id = 5, AcademicYear = 2024, Semester = "Fall" },
            new CourseEnrollment { Id = 6, AcademicYear = 2024, Semester = "Fall" }
        };

        SetupMockDbSetForEnrollment(historicalEnrollments);

        // Act
        var analytics = await _analyticsService.GetEnrollmentPatternsAsync(courseId, 2);

        // Assert
        Assert.NotNull(analytics);
        Assert.Equal(courseId, analytics.CourseId);
        Assert.NotNull(analytics.EnrollmentTrends);

        var fallTrends = analytics.EnrollmentTrends.Where(t => t.Semester == "Fall").ToList();
        Assert.Equal(2, fallTrends.Count); // 2023 and 2024

        var fall2023 = fallTrends.First(t => t.Year == 2023);
        Assert.Equal(2, fall2023.EnrollmentCount);

        var fall2024 = fallTrends.First(t => t.Year == 2024);
        Assert.Equal(3, fall2024.EnrollmentCount);

        Assert.True(analytics.EnrollmentGrowthRate > 0); // Increasing enrollment
    }

    [Fact]
    public async Task CourseAnalytics_Should_Calculate_Success_Rates()
    {
        // Arrange
        var courseId = 101;
        var enrollmentsWithGrades = new List<CourseEnrollment>
        {
            new CourseEnrollment { Id = 1, FinalGrade = "A" },  // Success
            new CourseEnrollment { Id = 2, FinalGrade = "B+" }, // Success
            new CourseEnrollment { Id = 3, FinalGrade = "C" },  // Success
            new CourseEnrollment { Id = 4, FinalGrade = "D" },  // Not success
            new CourseEnrollment { Id = 5, FinalGrade = "F" },  // Not success
            new CourseEnrollment { Id = 6, FinalGrade = "W" }   // Withdrawal
        };

        SetupMockDbSetForEnrollment(enrollmentsWithGrades);

        // Act
        var successAnalytics = await _analyticsService.GetCourseSuccessRateAsync(courseId);

        // Assert
        Assert.NotNull(successAnalytics);
        Assert.Equal(courseId, successAnalytics.CourseId);
        Assert.Equal(6, successAnalytics.TotalEnrollments);
        Assert.Equal(3, successAnalytics.SuccessfulCompletions); // A, B+, C
        Assert.Equal(2, successAnalytics.UnsuccessfulCompletions); // D, F
        Assert.Equal(1, successAnalytics.Withdrawals); // W
        Assert.Equal(50.0m, successAnalytics.SuccessRate); // 3/6 = 50%
        Assert.Equal(33.33m, Math.Round(successAnalytics.FailureRate, 2)); // 2/6 = 33.33%
        Assert.Equal(16.67m, Math.Round(successAnalytics.WithdrawalRate, 2)); // 1/6 = 16.67%
    }

    [Fact]
    public async Task CourseAnalytics_Should_Identify_At_Risk_Students()
    {
        // Arrange
        var courseId = 101;
        var currentEnrollments = new List<CourseEnrollment>
        {
            new CourseEnrollment { Id = 1, StudentEmpNr = 1, MidtermGrade = "A" },  // Not at risk
            new CourseEnrollment { Id = 2, StudentEmpNr = 2, MidtermGrade = "D" },  // At risk
            new CourseEnrollment { Id = 3, StudentEmpNr = 3, MidtermGrade = "F" },  // At risk
            new CourseEnrollment { Id = 4, StudentEmpNr = 4, MidtermGrade = "C" }   // Not at risk
        };

        var students = new List<Student>
        {
            new Student { Id = 1, FirstName = "Alice", LastName = "Johnson" },
            new Student { Id = 2, FirstName = "Bob", LastName = "Smith" },
            new Student { Id = 3, FirstName = "Carol", LastName = "Davis" },
            new Student { Id = 4, FirstName = "David", LastName = "Wilson" }
        };

        SetupMockDbSetForEnrollment(currentEnrollments);
        SetupMockDbSetForStudent(students);

        // Act
        var atRiskAnalysis = await _analyticsService.IdentifyAtRiskStudentsAsync(courseId);

        // Assert
        Assert.NotNull(atRiskAnalysis);
        Assert.Equal(courseId, atRiskAnalysis.CourseId);
        Assert.Equal(2, atRiskAnalysis.AtRiskStudents.Count);

        var atRiskStudentIds = atRiskAnalysis.AtRiskStudents.Select(s => s.StudentId).ToList();
        Assert.Contains(2, atRiskStudentIds); // Bob with D
        Assert.Contains(3, atRiskStudentIds); // Carol with F
        Assert.DoesNotContain(1, atRiskStudentIds); // Alice with A
        Assert.DoesNotContain(4, atRiskStudentIds); // David with C

        Assert.All(atRiskAnalysis.AtRiskStudents, student =>
        {
            Assert.NotNull(student.RiskFactors);
            Assert.NotEmpty(student.RiskFactors);
            Assert.NotNull(student.RecommendedActions);
        });
    }

    [Fact]
    public async Task CourseAnalytics_Should_Provide_Capacity_Utilization_Insights()
    {
        // Arrange
        var courseId = 101;
        var course = new Course { Id = courseId, MaxEnrollment = 30 };

        var semesterData = new List<SemesterCapacityData>
        {
            new SemesterCapacityData
            {
                CourseId = courseId,
                Semester = "Fall 2023",
                MaxCapacity = 30,
                FinalEnrollment = 28,
                PeakWaitlistSize = 5
            },
            new SemesterCapacityData
            {
                CourseId = courseId,
                Semester = "Spring 2024",
                MaxCapacity = 30,
                FinalEnrollment = 25,
                PeakWaitlistSize = 2
            }
        };

        SetupMockDbSetForCourse(new List<Course> { course });
        SetupMockDbSetForCapacityData(semesterData);

        // Act
        var capacityAnalytics = await _analyticsService.GetCapacityUtilizationAsync(courseId, 2);

        // Assert
        Assert.NotNull(capacityAnalytics);
        Assert.Equal(courseId, capacityAnalytics.CourseId);
        Assert.Equal(88.33m, Math.Round(capacityAnalytics.AverageUtilizationRate, 2)); // (28+25)/2/30 = 88.33%
        Assert.Equal(3.5m, capacityAnalytics.AverageWaitlistSize); // (5+2)/2 = 3.5
        Assert.True(capacityAnalytics.IsHighDemand); // Average waitlist > 0 indicates high demand
        Assert.Contains("Consider increasing capacity", capacityAnalytics.Recommendations);
    }

    #endregion

    #region Helper Methods

    private void SetupMockDbSetForCourse(List<Course> courses)
    {
        var queryable = courses.AsQueryable();
        var mockSet = new Mock<DbSet<Course>>();

        mockSet.As<IQueryable<Course>>().Setup(m => m.Provider).Returns(queryable.Provider);
        mockSet.As<IQueryable<Course>>().Setup(m => m.Expression).Returns(queryable.Expression);
        mockSet.As<IQueryable<Course>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        mockSet.As<IQueryable<Course>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

        mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
               .Returns<object[]>(ids => ValueTask.FromResult(courses.FirstOrDefault(c => c.Id == (int)ids[0])));

        _mockContext.Setup(c => c.Courses).Returns(mockSet.Object);
    }

    private void SetupMockDbSetForStudent(List<Student> students)
    {
        var queryable = students.AsQueryable();
        var mockSet = new Mock<DbSet<Student>>();

        mockSet.As<IQueryable<Student>>().Setup(m => m.Provider).Returns(queryable.Provider);
        mockSet.As<IQueryable<Student>>().Setup(m => m.Expression).Returns(queryable.Expression);
        mockSet.As<IQueryable<Student>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        mockSet.As<IQueryable<Student>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

        _mockContext.Setup(c => c.Students).Returns(mockSet.Object);
    }

    private void SetupMockDbSetForEnrollment(List<CourseEnrollment> enrollments)
    {
        var queryable = enrollments.AsQueryable();
        var mockSet = new Mock<DbSet<CourseEnrollment>>();

        mockSet.As<IQueryable<CourseEnrollment>>().Setup(m => m.Provider).Returns(queryable.Provider);
        mockSet.As<IQueryable<CourseEnrollment>>().Setup(m => m.Expression).Returns(queryable.Expression);
        mockSet.As<IQueryable<CourseEnrollment>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        mockSet.As<IQueryable<CourseEnrollment>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

        _mockContext.Setup(c => c.CourseEnrollments).Returns(mockSet.Object);
    }

    private void SetupMockDbSetForEquivalency(List<CourseEquivalency> equivalencies)
    {
        var queryable = equivalencies.AsQueryable();
        var mockSet = new Mock<DbSet<CourseEquivalency>>();

        mockSet.As<IQueryable<CourseEquivalency>>().Setup(m => m.Provider).Returns(queryable.Provider);
        mockSet.As<IQueryable<CourseEquivalency>>().Setup(m => m.Expression).Returns(queryable.Expression);
        mockSet.As<IQueryable<CourseEquivalency>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        mockSet.As<IQueryable<CourseEquivalency>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

        _mockContext.Setup(c => c.CourseEquivalencies).Returns(mockSet.Object);
    }

    private void SetupMockDbSetForDegreeRequirement(List<DegreeRequirement> requirements)
    {
        var queryable = requirements.AsQueryable();
        var mockSet = new Mock<DbSet<DegreeRequirement>>();

        mockSet.As<IQueryable<DegreeRequirement>>().Setup(m => m.Provider).Returns(queryable.Provider);
        mockSet.As<IQueryable<DegreeRequirement>>().Setup(m => m.Expression).Returns(queryable.Expression);
        mockSet.As<IQueryable<DegreeRequirement>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        mockSet.As<IQueryable<DegreeRequirement>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

        _mockContext.Setup(c => c.DegreeRequirements).Returns(mockSet.Object);
    }

    private void SetupMockDbSetForWaitlist(List<CourseWaitlist> waitlistEntries)
    {
        var queryable = waitlistEntries.AsQueryable();
        var mockSet = new Mock<DbSet<CourseWaitlist>>();

        mockSet.As<IQueryable<CourseWaitlist>>().Setup(m => m.Provider).Returns(queryable.Provider);
        mockSet.As<IQueryable<CourseWaitlist>>().Setup(m => m.Expression).Returns(queryable.Expression);
        mockSet.As<IQueryable<CourseWaitlist>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        mockSet.As<IQueryable<CourseWaitlist>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

        _mockContext.Setup(c => c.CourseWaitlists).Returns(mockSet.Object);
    }

    private void SetupMockDbSetForCapacityData(List<SemesterCapacityData> capacityData)
    {
        var queryable = capacityData.AsQueryable();
        var mockSet = new Mock<DbSet<SemesterCapacityData>>();

        mockSet.As<IQueryable<SemesterCapacityData>>().Setup(m => m.Provider).Returns(queryable.Provider);
        mockSet.As<IQueryable<SemesterCapacityData>>().Setup(m => m.Expression).Returns(queryable.Expression);
        mockSet.As<IQueryable<SemesterCapacityData>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        mockSet.As<IQueryable<SemesterCapacityData>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

        _mockContext.Setup(c => c.SemesterCapacityData).Returns(mockSet.Object);
    }

    #endregion
}

#region Supporting Classes and Models for Tests

/// <summary>
/// Course recommendation result
/// </summary>
public class CourseRecommendation
{
    public Course Course { get; set; } = new();
    public decimal RelevanceScore { get; set; }
    public List<string> ReasonCodes { get; set; } = new();
    public string RecommendationText { get; set; } = string.Empty;
}

/// <summary>
/// Degree progress tracking
/// </summary>
public class DegreeProgress
{
    public int StudentId { get; set; }
    public string DegreeCode { get; set; } = string.Empty;
    public int TotalCreditsCompleted { get; set; }
    public int TotalCreditsRequired { get; set; }
    public int RemainingCreditsNeeded { get; set; }
    public decimal CompletionPercentage { get; set; }
    public List<CategoryProgress> CategoryProgress { get; set; } = new();
    public DateTime LastUpdated { get; set; }
}

/// <summary>
/// Progress by degree requirement category
/// </summary>
public class CategoryProgress
{
    public string CategoryName { get; set; } = string.Empty;
    public int CreditsCompleted { get; set; }
    public int CreditsRequired { get; set; }
    public decimal CompletionPercentage { get; set; }
    public List<string> CompletedCourses { get; set; } = new();
}

/// <summary>
/// External course for transfer credit evaluation
/// </summary>
public class ExternalCourse
{
    public string InstitutionCode { get; set; } = string.Empty;
    public string CourseNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public decimal CreditHours { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Grade { get; set; } = string.Empty;
    public DateTime CompletionDate { get; set; }
}

/// <summary>
/// Course equivalency mapping
/// </summary>
public class CourseEquivalency
{
    public int Id { get; set; }
    public ExternalCourse ExternalCourse { get; set; } = new();
    public int InternalCourseId { get; set; }
    public EquivalencyType EquivalencyType { get; set; }
    public decimal CreditHoursAwarded { get; set; }
    public string Conditions { get; set; } = string.Empty;
    public DateTime EffectiveDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string ApprovedBy { get; set; } = string.Empty;
    public bool IsActive => ExpirationDate == null || ExpirationDate > DateTime.UtcNow;
}

/// <summary>
/// Transfer credit request
/// </summary>
public class TransferCreditRequest
{
    public int StudentId { get; set; }
    public string SourceInstitution { get; set; } = string.Empty;
    public string OfficialTranscript { get; set; } = string.Empty;
    public List<ExternalCourse> ExternalCourses { get; set; } = new();
    public DateTime RequestDate { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Transfer credit evaluation result
/// </summary>
public class TransferCreditEvaluation
{
    public int StudentId { get; set; }
    public string SourceInstitution { get; set; } = string.Empty;
    public List<TransferCreditResult> EvaluatedCourses { get; set; } = new();
    public int TotalCreditsAwarded { get; set; }
    public DateTime EvaluationDate { get; set; }
    public string EvaluatedBy { get; set; } = string.Empty;
}

/// <summary>
/// Individual transfer credit result
/// </summary>
public class TransferCreditResult
{
    public ExternalCourse ExternalCourse { get; set; } = new();
    public int? InternalCourseId { get; set; }
    public EquivalencyType EquivalencyType { get; set; }
    public TransferStatus Status { get; set; }
    public decimal CreditsAwarded { get; set; }
    public string Conditions { get; set; } = string.Empty;
}

/// <summary>
/// Transfer credit policies
/// </summary>
public class TransferCreditPolicies
{
    public int MaximumCourseAge { get; set; } = 10; // years
    public string MinimumGrade { get; set; } = "C";
    public int MaximumTransferCredits { get; set; } = 60;
    public List<string> AcceptedInstitutions { get; set; } = new();
    public bool RequireOfficialTranscript { get; set; } = true;
}

/// <summary>
/// Course availability information
/// </summary>
public class CourseAvailability
{
    public int CourseId { get; set; }
    public int MaxCapacity { get; set; }
    public int CurrentEnrollment { get; set; }
    public int AvailableSeats { get; set; }
    public bool IsFull { get; set; }
    public bool HasWaitlist { get; set; }
    public int WaitlistCount { get; set; }
    public DateTime LastUpdated { get; set; }
}

/// <summary>
/// Enrollment request
/// </summary>
public class EnrollmentRequest
{
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public string RequestedSemester { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; } = DateTime.UtcNow;
    public bool ForceEnroll { get; set; } = false; // Override capacity limits if authorized
}

/// <summary>
/// Enrollment request result
/// </summary>
public class EnrollmentResult
{
    public EnrollmentStatus Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? WaitlistPosition { get; set; }
    public DateTime ProcessedDate { get; set; }
}

/// <summary>
/// Course waitlist entry
/// </summary>
public class CourseWaitlist
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public int StudentId { get; set; }
    public int Position { get; set; }
    public DateTime AddedDate { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Prerequisite validation result
/// </summary>
public class PrerequisiteValidationResult
{
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public bool IsValid { get; set; }
    public bool RequiredPrerequisitesMet { get; set; }
    public bool OrPrerequisitesMet { get; set; }
    public List<string> MissingPrerequisites { get; set; } = new();
    public List<string> SatisfiedPrerequisites { get; set; } = new();
    public DateTime ValidationDate { get; set; }
}

/// <summary>
/// Course enrollment patterns analytics
/// </summary>
public class EnrollmentPatternAnalytics
{
    public int CourseId { get; set; }
    public List<EnrollmentTrend> EnrollmentTrends { get; set; } = new();
    public decimal EnrollmentGrowthRate { get; set; }
    public string MostPopularSemester { get; set; } = string.Empty;
    public int AverageEnrollment { get; set; }
    public List<string> Insights { get; set; } = new();
}

/// <summary>
/// Enrollment trend data
/// </summary>
public class EnrollmentTrend
{
    public int Year { get; set; }
    public string Semester { get; set; } = string.Empty;
    public int EnrollmentCount { get; set; }
    public decimal CapacityUtilization { get; set; }
}

/// <summary>
/// Course success rate analytics
/// </summary>
public class CourseSuccessAnalytics
{
    public int CourseId { get; set; }
    public int TotalEnrollments { get; set; }
    public int SuccessfulCompletions { get; set; }
    public int UnsuccessfulCompletions { get; set; }
    public int Withdrawals { get; set; }
    public decimal SuccessRate { get; set; }
    public decimal FailureRate { get; set; }
    public decimal WithdrawalRate { get; set; }
    public Dictionary<string, int> GradeDistribution { get; set; } = new();
}

/// <summary>
/// At-risk student analysis
/// </summary>
public class AtRiskStudentAnalysis
{
    public int CourseId { get; set; }
    public List<AtRiskStudent> AtRiskStudents { get; set; } = new();
    public int TotalStudentsAnalyzed { get; set; }
    public decimal AtRiskPercentage { get; set; }
    public DateTime AnalysisDate { get; set; }
}

/// <summary>
/// At-risk student information
/// </summary>
public class AtRiskStudent
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string CurrentGrade { get; set; } = string.Empty;
    public List<string> RiskFactors { get; set; } = new();
    public List<string> RecommendedActions { get; set; } = new();
    public decimal RiskScore { get; set; }
}

/// <summary>
/// Capacity utilization analytics
/// </summary>
public class CapacityUtilizationAnalytics
{
    public int CourseId { get; set; }
    public decimal AverageUtilizationRate { get; set; }
    public decimal AverageWaitlistSize { get; set; }
    public bool IsHighDemand { get; set; }
    public bool IsUnderUtilized { get; set; }
    public List<string> Recommendations { get; set; } = new();
    public List<SemesterUtilization> SemesterData { get; set; } = new();
}

/// <summary>
/// Semester utilization data
/// </summary>
public class SemesterUtilization
{
    public string Semester { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal UtilizationRate { get; set; }
    public int WaitlistSize { get; set; }
}

/// <summary>
/// Semester capacity data for analytics
/// </summary>
public class SemesterCapacityData
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string Semester { get; set; } = string.Empty;
    public int MaxCapacity { get; set; }
    public int FinalEnrollment { get; set; }
    public int PeakWaitlistSize { get; set; }
    public DateTime RecordDate { get; set; }
}

/// <summary>
/// Degree requirement for testing
/// </summary>
public class DegreeRequirement
{
    public int Id { get; set; }
    public string DegreeCode { get; set; } = string.Empty;
    public int TotalCreditsRequired { get; set; }
    public int MajorCreditsRequired { get; set; }
    public int GeneralEducationCreditsRequired { get; set; }
    public int ElectiveCreditsRequired { get; set; }
}

#endregion

#region Enums for Tests

public enum EquivalencyType
{
    Direct,
    Partial,
    Conditional,
    NoEquivalent
}

public enum TransferStatus
{
    Approved,
    ConditionalApproval,
    PendingReview,
    Rejected
}

public enum EnrollmentStatus
{
    Approved,
    Waitlisted,
    Rejected,
    PrerequisitesNotMet
}

#endregion