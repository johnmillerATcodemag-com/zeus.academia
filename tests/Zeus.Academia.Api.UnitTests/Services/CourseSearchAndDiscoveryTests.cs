using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Services;
using Zeus.Academia.Infrastructure.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Zeus.Academia.Api.UnitTests.Services;

/// <summary>
/// Comprehensive tests for Course Search and Discovery (Task 4).
/// Tests advanced search, filtering, recommendations, comparison, and waitlist functionality.
/// </summary>
public class CourseSearchAndDiscoveryTests
{
    private readonly Mock<AcademiaDbContext> _mockContext;
    private readonly Mock<ILogger<CourseSearchService>> _mockLogger;
    private readonly Mock<ILogger<CourseRecommendationService>> _mockRecommendationLogger;
    private readonly Mock<ILogger<CourseWaitlistService>> _mockWaitlistLogger;

    public CourseSearchAndDiscoveryTests()
    {
        _mockContext = new Mock<AcademiaDbContext>();
        _mockLogger = new Mock<ILogger<CourseSearchService>>();
        _mockRecommendationLogger = new Mock<ILogger<CourseRecommendationService>>();
        _mockWaitlistLogger = new Mock<ILogger<CourseWaitlistService>>();
    }

    #region Advanced Course Search Tests

    [Fact]
    public async Task AdvancedCourseSearch_Should_Handle_Multiple_Criteria()
    {
        // Arrange
        var searchCriteria = new CourseSearchCriteria
        {
            SubjectCodes = new[] { "CS", "MATH" },
            MinCredits = 3,
            MaxCredits = 4,
            Level = CourseLevel.Undergraduate,
            Keywords = "algorithm data structure",
            InstructorName = "Smith",
            Semester = "Fall",
            AcademicYear = 2025,
            HasPrerequisites = true,
            AvailableSeats = true
        };

        var expectedCourses = CreateSampleCourses();
        var service = new CourseSearchService(_mockContext.Object, _mockLogger.Object);

        // Act
        var result = await service.SearchCoursesAsync(searchCriteria);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<CourseSearchResult>(result);
        Assert.True(result.TotalCount >= 0);
        Assert.NotNull(result.Courses);
        Assert.NotNull(result.FilterSummary);
    }

    [Fact]
    public async Task CourseSearch_Should_Support_Complex_Text_Search()
    {
        // Arrange
        var searchCriteria = new CourseSearchCriteria
        {
            Keywords = "\"machine learning\" OR \"artificial intelligence\" AND programming",
            SearchFields = new[] { CourseSearchField.Title, CourseSearchField.Description, CourseSearchField.LearningOutcomes }
        };

        var service = new CourseSearchService(_mockContext.Object, _mockLogger.Object);

        // Act
        var result = await service.SearchCoursesAsync(searchCriteria);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.SearchDurationMs > 0);
        Assert.NotNull(result.SearchSuggestions);
    }

    [Fact]
    public async Task CourseSearch_Should_Handle_Fuzzy_Search()
    {
        // Arrange
        var searchCriteria = new CourseSearchCriteria
        {
            Keywords = "algorthm", // Intentional typo
            EnableFuzzySearch = true,
            FuzzySearchThreshold = 0.8f
        };

        var service = new CourseSearchService(_mockContext.Object, _mockLogger.Object);

        // Act
        var result = await service.SearchCoursesAsync(searchCriteria);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.FuzzyMatches);
        Assert.Contains("algorithm", result.SearchSuggestions ?? new List<string>());
    }

    #endregion

    #region Course Filtering Tests

    [Fact]
    public async Task CourseFilter_Should_Filter_By_Subject_Hierarchy()
    {
        // Arrange
        var filterCriteria = new CourseFilterCriteria
        {
            SubjectHierarchy = new SubjectFilter
            {
                DepartmentName = "Computer Science",
                SubjectCodes = new[] { "CS", "CSE" },
                IncludeRelatedSubjects = true
            }
        };

        var service = new CourseSearchService(_mockContext.Object, _mockLogger.Object);

        // Act
        var result = await service.FilterCoursesAsync(filterCriteria);

        // Assert
        Assert.NotNull(result);
        Assert.All(result.Courses, course =>
            Assert.Contains(course.SubjectCode, new[] { "CS", "CSE" }));
    }

    [Fact]
    public async Task CourseFilter_Should_Filter_By_Academic_Level()
    {
        // Arrange
        var filterCriteria = new CourseFilterCriteria
        {
            AcademicLevel = new LevelFilter
            {
                MinLevel = 200,
                MaxLevel = 400,
                IncludeGraduateLevel = false,
                ClassStanding = new[] { ClassStanding.Sophomore, ClassStanding.Junior }
            }
        };

        var service = new CourseSearchService(_mockContext.Object, _mockLogger.Object);

        // Act
        var result = await service.FilterCoursesAsync(filterCriteria);

        // Assert
        Assert.NotNull(result);
        Assert.All(result.Courses, course =>
        {
            var courseNumber = int.Parse(course.CourseNumber.Substring(2));
            Assert.InRange(courseNumber, 200, 400);
        });
    }

    [Fact]
    public async Task CourseFilter_Should_Filter_By_Credit_Hours()
    {
        // Arrange
        var filterCriteria = new CourseFilterCriteria
        {
            CreditHours = new CreditFilter
            {
                MinCredits = 3,
                MaxCredits = 4,
                CreditTypes = new[] { CreditType.Lecture, CreditType.LectureAndLab }
            }
        };

        var service = new CourseSearchService(_mockContext.Object, _mockLogger.Object);

        // Act
        var result = await service.FilterCoursesAsync(filterCriteria);

        // Assert
        Assert.NotNull(result);
        Assert.All(result.Courses, course =>
            Assert.InRange(course.CreditHours ?? 0, 3, 4));
    }

    [Fact]
    public async Task CourseFilter_Should_Filter_By_Prerequisites()
    {
        // Arrange
        var filterCriteria = new CourseFilterCriteria
        {
            PrerequisiteFilter = new PrerequisiteFilter
            {
                HasPrerequisites = true,
                CompletedCourses = new[] { 101, 102, 201 },
                StudentId = 12345,
                CheckEligibility = true
            }
        };

        var service = new CourseSearchService(_mockContext.Object, _mockLogger.Object);

        // Act
        var result = await service.FilterCoursesAsync(filterCriteria);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.EligibilityResults);
        Assert.All(result.EligibilityResults, eligibility =>
            Assert.NotNull(eligibility.PrerequisiteStatus));
    }

    #endregion

    #region Course Recommendation Tests

    [Fact]
    public async Task CourseRecommendation_Should_Recommend_Based_On_Academic_Profile()
    {
        // Arrange
        var studentProfile = new StudentAcademicProfile
        {
            StudentId = 12345,
            Major = "Computer Science",
            Minor = "Mathematics",
            CompletedCourses = new[] { 101, 102, 201, 250 },
            CurrentGPA = 3.75m,
            AcademicYear = 2025,
            Semester = "Fall",
            ClassStanding = ClassStanding.Junior,
            Interests = new[] { "Machine Learning", "Database Systems", "Software Engineering" },
            CareerGoals = "Software Developer"
        };

        var service = new CourseRecommendationService(_mockContext.Object, _mockRecommendationLogger.Object);

        // Act
        var recommendations = await service.RecommendCoursesAsync(studentProfile);

        // Assert
        Assert.NotNull(recommendations);
        Assert.True(recommendations.Count > 0);
        Assert.All(recommendations, rec =>
        {
            Assert.True(rec.RecommendationScore >= 0 && rec.RecommendationScore <= 1.0f);
            Assert.NotNull(rec.RecommendationReasons);
            Assert.True(rec.RecommendationReasons.Count > 0);
        });
    }

    [Fact]
    public async Task CourseRecommendation_Should_Consider_Degree_Requirements()
    {
        // Arrange
        var studentProfile = new StudentAcademicProfile
        {
            StudentId = 12345,
            Major = "Computer Science",
            DegreeCode = "BS_CS",
            CompletedCourses = new[] { 101, 102, 201 }
        };

        var degreeRequirements = new DegreeRequirements
        {
            RequiredCourses = new[] { 101, 102, 201, 301, 302, 401 },
            ElectiveCategories = new[]
            {
                new ElectiveCategory { Name = "Technical Electives", RequiredCredits = 12, SubjectCodes = new[] { "CS", "CSE" } },
                new ElectiveCategory { Name = "Math Electives", RequiredCredits = 6, SubjectCodes = new[] { "MATH", "STAT" } }
            }
        };

        var service = new CourseRecommendationService(_mockContext.Object, _mockRecommendationLogger.Object);

        // Act
        var recommendations = await service.RecommendCoursesForDegreeAsync(studentProfile, degreeRequirements);

        // Assert
        Assert.NotNull(recommendations);
        Assert.Contains(recommendations, rec => rec.RecommendationReasons.Contains("Required for degree"));
        Assert.Contains(recommendations, rec => rec.RecommendationReasons.Contains("Fulfills elective requirement"));
    }

    [Fact]
    public async Task CourseRecommendation_Should_Consider_Academic_Performance()
    {
        // Arrange
        var studentProfile = new StudentAcademicProfile
        {
            StudentId = 12345,
            CompletedCourses = new[] { 101, 102, 201 },
            CourseGrades = new Dictionary<int, string>
            {
                { 101, "A" },
                { 102, "B+" },
                { 201, "A-" }
            },
            StrengthAreas = new[] { "Programming", "Mathematics" },
            WeaknessAreas = new[] { "Theory", "Hardware" }
        };

        var service = new CourseRecommendationService(_mockContext.Object, _mockRecommendationLogger.Object);

        // Act
        var recommendations = await service.RecommendCoursesAsync(studentProfile);

        // Assert
        Assert.NotNull(recommendations);
        Assert.Contains(recommendations, rec =>
            rec.RecommendationReasons.Any(reason => reason.Contains("strength") || reason.Contains("performance")));
    }

    #endregion

    #region Course Comparison Tests

    [Fact]
    public async Task CourseComparison_Should_Compare_Multiple_Courses()
    {
        // Arrange
        var courseIds = new[] { 101, 102, 201 };
        var comparisonCriteria = new CourseComparisonCriteria
        {
            IncludePrerequisites = true,
            IncludeWorkload = true,
            IncludeDifficulty = true,
            IncludeScheduleInfo = true,
            IncludeLearningOutcomes = true,
            IncludeInstructorRatings = true
        };

        var service = new CourseComparisonService(_mockContext.Object);

        // Act
        var comparison = await service.CompareCoursesAsync(courseIds, comparisonCriteria);

        // Assert
        Assert.NotNull(comparison);
        Assert.Equal(courseIds.Length, comparison.Courses.Count);
        Assert.NotNull(comparison.ComparisonMatrix);
        Assert.NotNull(comparison.SimilarityScores);
        Assert.All(comparison.Courses, course =>
        {
            Assert.NotNull(course.PrerequisiteInfo);
            Assert.NotNull(course.WorkloadEstimate);
            Assert.NotNull(course.DifficultyRating);
        });
    }

    [Fact]
    public async Task CourseComparison_Should_Highlight_Key_Differences()
    {
        // Arrange
        var courseIds = new[] { 301, 302 }; // Similar level courses
        var comparisonCriteria = new CourseComparisonCriteria
        {
            HighlightDifferences = true,
            DifferenceThreshold = 0.3f
        };

        var service = new CourseComparisonService(_mockContext.Object);

        // Act
        var comparison = await service.CompareCoursesAsync(courseIds, comparisonCriteria);

        // Assert
        Assert.NotNull(comparison);
        Assert.NotNull(comparison.KeyDifferences);
        Assert.True(comparison.KeyDifferences.Count > 0);
        Assert.All(comparison.KeyDifferences, diff =>
        {
            Assert.NotNull(diff.Category);
            Assert.NotNull(diff.Description);
            Assert.True(diff.SignificanceScore >= 0 && diff.SignificanceScore <= 1.0f);
        });
    }

    #endregion

    #region Waitlist and Notification Tests

    [Fact]
    public async Task Waitlist_Should_Add_Student_To_Course_Waitlist()
    {
        // Arrange
        var waitlistRequest = new CourseWaitlistRequest
        {
            StudentId = 12345,
            CourseId = 101,
            CourseOfferingId = 501,
            Priority = WaitlistPriority.High,
            NotificationPreferences = new NotificationPreferences
            {
                EmailNotification = true,
                SmsNotification = false,
                InAppNotification = true,
                NotificationTiming = NotificationTiming.Immediate
            }
        };

        var service = new CourseWaitlistService(_mockContext.Object, _mockWaitlistLogger.Object);

        // Act
        var result = await service.AddToWaitlistAsync(waitlistRequest);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.WaitlistPosition > 0);
        Assert.NotNull(result.EstimatedWaitTime);
        Assert.Equal(WaitlistStatus.Active, result.Status);
    }

    [Fact]
    public async Task Waitlist_Should_Process_Seat_Availability()
    {
        // Arrange
        var courseOfferingId = 501;
        var availableSeats = 2;

        var service = new CourseWaitlistService(_mockContext.Object, _mockWaitlistLogger.Object);

        // Act
        var processedStudents = await service.ProcessWaitlistAsync(courseOfferingId, availableSeats);

        // Assert
        Assert.NotNull(processedStudents);
        Assert.True(processedStudents.Count <= availableSeats);
        Assert.All(processedStudents, student =>
        {
            Assert.NotNull(student.NotificationSent);
            Assert.True(student.NotificationSent.Value);
            Assert.NotNull(student.EnrollmentDeadline);
        });
    }

    [Fact]
    public async Task Waitlist_Should_Send_Automated_Notifications()
    {
        // Arrange
        var waitlistEntry = new CourseWaitlistEntry
        {
            StudentId = 12345,
            CourseId = 101,
            CourseOfferingId = 501,
            WaitlistPosition = 1,
            NotificationPreferences = new NotificationPreferences
            {
                EmailNotification = true,
                SmsNotification = true
            }
        };

        var service = new CourseWaitlistService(_mockContext.Object, _mockWaitlistLogger.Object);

        // Act
        var notificationResult = await service.SendWaitlistNotificationAsync(waitlistEntry, NotificationType.SeatAvailable);

        // Assert
        Assert.NotNull(notificationResult);
        Assert.True(notificationResult.EmailSent);
        Assert.True(notificationResult.SmsSent);
        Assert.NotNull(notificationResult.EnrollmentLink);
        Assert.True(notificationResult.EnrollmentDeadline > DateTime.UtcNow);
    }

    #endregion

    #region Performance Tests

    [Fact]
    public async Task CourseSearch_Should_Complete_Within_Performance_Target()
    {
        // Arrange
        var searchCriteria = new CourseSearchCriteria
        {
            Keywords = "computer science programming",
            SubjectCodes = new[] { "CS", "CSE", "CSIS" },
            PageSize = 50
        };

        var service = new CourseSearchService(_mockContext.Object, _mockLogger.Object);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = await service.SearchCoursesAsync(searchCriteria);
        stopwatch.Stop();

        // Assert
        Assert.True(stopwatch.ElapsedMilliseconds < 250, $"Search took {stopwatch.ElapsedMilliseconds}ms, expected < 250ms");
        Assert.NotNull(result);
    }

    [Fact]
    public async Task CourseFilter_Should_Handle_Large_Dataset_Efficiently()
    {
        // Arrange
        var filterCriteria = new CourseFilterCriteria
        {
            SubjectHierarchy = new SubjectFilter { IncludeAllSubjects = true },
            PageSize = 100,
            PageNumber = 1
        };

        var service = new CourseSearchService(_mockContext.Object, _mockLogger.Object);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = await service.FilterCoursesAsync(filterCriteria);
        stopwatch.Stop();

        // Assert
        Assert.True(stopwatch.ElapsedMilliseconds < 500, $"Filtering took {stopwatch.ElapsedMilliseconds}ms, expected < 500ms");
        Assert.NotNull(result);
    }

    #endregion

    #region Helper Methods

    private List<Course> CreateSampleCourses()
    {
        return new List<Course>
        {
            new Course
            {
                Id = 101,
                CourseNumber = "CS101",
                Title = "Introduction to Programming",
                SubjectCode = "CS",
                CreditHours = 3,
                Description = "Basic programming concepts and algorithms"
            },
            new Course
            {
                Id = 102,
                CourseNumber = "CS102",
                Title = "Data Structures and Algorithms",
                SubjectCode = "CS",
                CreditHours = 4,
                Description = "Advanced data structures and algorithm analysis"
            },
            new Course
            {
                Id = 201,
                CourseNumber = "MATH201",
                Title = "Discrete Mathematics",
                SubjectCode = "MATH",
                CreditHours = 3,
                Description = "Mathematical foundations for computer science"
            }
        };
    }

    #endregion
}

#region Supporting Classes and Enums

/// <summary>
/// Comprehensive search criteria for advanced course search
/// </summary>
public class CourseSearchCriteria
{
    public string[]? SubjectCodes { get; set; }
    public int? MinCredits { get; set; }
    public int? MaxCredits { get; set; }
    public CourseLevel? Level { get; set; }
    public string? Keywords { get; set; }
    public string? InstructorName { get; set; }
    public string? Semester { get; set; }
    public int? AcademicYear { get; set; }
    public bool? HasPrerequisites { get; set; }
    public bool? AvailableSeats { get; set; }
    public CourseSearchField[]? SearchFields { get; set; }
    public bool EnableFuzzySearch { get; set; }
    public float FuzzySearchThreshold { get; set; } = 0.8f;
    public int PageSize { get; set; } = 25;
    public int PageNumber { get; set; } = 1;
}

/// <summary>
/// Filter criteria for course filtering
/// </summary>
public class CourseFilterCriteria
{
    public SubjectFilter? SubjectHierarchy { get; set; }
    public LevelFilter? AcademicLevel { get; set; }
    public CreditFilter? CreditHours { get; set; }
    public PrerequisiteFilter? PrerequisiteFilter { get; set; }
    public ScheduleFilter? Schedule { get; set; }
    public int PageSize { get; set; } = 25;
    public int PageNumber { get; set; } = 1;
}

/// <summary>
/// NOTE: StudentAcademicProfile class moved to CoursePlanningAndDegreeRequirementsTests.cs 
/// to avoid duplicate definition conflicts in the test project
/// </summary>

/// <summary>
/// Course waitlist request
/// </summary>
public class CourseWaitlistRequest
{
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public int CourseOfferingId { get; set; }
    public WaitlistPriority Priority { get; set; }
    public NotificationPreferences? NotificationPreferences { get; set; }
}

/// <summary>
/// Supporting enums and classes
/// </summary>
public enum CourseLevel { Undergraduate, Graduate, Doctoral }
public enum CourseSearchField { Title, Description, LearningOutcomes, Prerequisites, Instructor }
public enum ClassStanding { Freshman, Sophomore, Junior, Senior, Graduate }
public enum WaitlistPriority { Low, Normal, High, Emergency }
public enum WaitlistStatus { Active, Inactive, Enrolled, Expired }
public enum NotificationType { SeatAvailable, PositionChanged, CourseUpdate }
public enum NotificationTiming { Immediate, Daily, Weekly }
public enum CreditType { Lecture, Lab, LectureAndLab, Clinical, Practicum }

public class SubjectFilter
{
    public string? DepartmentName { get; set; }
    public string[]? SubjectCodes { get; set; }
    public bool IncludeRelatedSubjects { get; set; }
    public bool IncludeAllSubjects { get; set; }
}

public class LevelFilter
{
    public int MinLevel { get; set; }
    public int MaxLevel { get; set; }
    public bool IncludeGraduateLevel { get; set; }
    public ClassStanding[]? ClassStanding { get; set; }
}

public class CreditFilter
{
    public int MinCredits { get; set; }
    public int MaxCredits { get; set; }
    public CreditType[]? CreditTypes { get; set; }
}

public class PrerequisiteFilter
{
    public bool HasPrerequisites { get; set; }
    public int[]? CompletedCourses { get; set; }
    public int StudentId { get; set; }
    public bool CheckEligibility { get; set; }
}

public class ScheduleFilter
{
    public string[]? DaysOfWeek { get; set; }
    public TimeSpan? StartTimeAfter { get; set; }
    public TimeSpan? EndTimeBefore { get; set; }
}

public class NotificationPreferences
{
    public bool EmailNotification { get; set; }
    public bool SmsNotification { get; set; }
    public bool InAppNotification { get; set; }
    public NotificationTiming NotificationTiming { get; set; }
}

#endregion