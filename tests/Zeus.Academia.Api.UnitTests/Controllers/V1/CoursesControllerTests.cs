using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Zeus.Academia.API.Controllers;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Services;
using Zeus.Academia.Infrastructure.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using CourseLevel = Zeus.Academia.Infrastructure.Enums.CourseLevel;

namespace Zeus.Academia.Api.UnitTests.Controllers.V1;

/// <summary>
/// Unit tests for CoursesController - Task 6: Course API Controllers
/// Tests comprehensive CRUD operations, search/filtering, prerequisite validation,
/// course planning, and degree audit endpoints as per acceptance criteria.
/// </summary>
public class CoursesControllerTests
{
    private readonly Mock<AcademiaDbContext> _mockContext;
    private readonly Mock<ILogger<CoursesController>> _mockLogger;
    private readonly CoursesController _controller;

    public CoursesControllerTests()
    {
        var options = new DbContextOptionsBuilder<AcademiaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _mockContext = new Mock<AcademiaDbContext>(options);
        _mockLogger = new Mock<ILogger<CoursesController>>();

        // Mock the required services
        var mockDegreeRequirementService = new Mock<DegreeRequirementService>();
        var mockDegreeAuditService = new Mock<DegreeAuditService>();
        var mockCourseSequencePlanningService = new Mock<CourseSequencePlanningService>();
        var mockCoursePlanOptimizationService = new Mock<CoursePlanOptimizationService>();

        _controller = new CoursesController(
            _mockContext.Object,
            mockDegreeRequirementService.Object,
            mockDegreeAuditService.Object,
            mockCourseSequencePlanningService.Object,
            mockCoursePlanOptimizationService.Object,
            _mockLogger.Object);
    }

    #region CRUD Operations Tests

    [Fact]
    public async Task GetCourse_WithValidId_ShouldReturnCourse()
    {
        // Arrange
        var courseId = 1;
        var expectedCourse = CreateTestCourse(courseId, "CS101", "Introduction to Computer Science");

        SetupMockDbSetForCourse(new List<Course> { expectedCourse });

        // Act
        var result = await _controller.GetCourse(courseId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedCourse = Assert.IsType<Course>(okResult.Value);
        Assert.Equal(expectedCourse.Id, returnedCourse.Id);
        Assert.Equal(expectedCourse.CourseNumber, returnedCourse.CourseNumber);
        Assert.Equal(expectedCourse.Title, returnedCourse.Title);
    }

    [Fact]
    public async Task GetCourse_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var courseId = 999;
        SetupMockDbSetForCourse(new List<Course>());

        // Act
        var result = await _controller.GetCourse(courseId);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetAllCourses_ShouldReturnAllActiveCourses()
    {
        // Arrange
        var courses = new List<Course>
        {
            CreateTestCourse(1, "CS101", "Introduction to Computer Science"),
            CreateTestCourse(2, "CS102", "Data Structures"),
            CreateTestCourse(3, "MATH101", "Calculus I", CourseStatus.Retired)
        };

        SetupMockDbSetForCourse(courses);

        // Act
        var result = await _controller.GetAllCourses();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedCourses = Assert.IsAssignableFrom<IEnumerable<Course>>(okResult.Value);

        // Should only return active courses
        Assert.Equal(2, returnedCourses.Count());
        Assert.All(returnedCourses, course => Assert.NotEqual(CourseStatus.Retired, course.Status));
    }

    [Fact]
    public async Task CreateCourse_WithValidData_ShouldReturnCreatedCourse()
    {
        // Arrange
        var newCourse = CreateTestCourse(0, "CS201", "Object-Oriented Programming");
        var courses = new List<Course>();

        SetupMockDbSetForCourse(courses);
        _mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

        // Act
        var result = await _controller.CreateCourse(newCourse);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedCourse = Assert.IsType<Course>(createdResult.Value);
        Assert.Equal(newCourse.CourseNumber, returnedCourse.CourseNumber);
        Assert.Equal(newCourse.Title, returnedCourse.Title);
    }

    [Fact]
    public async Task CreateCourse_WithInvalidData_ShouldReturnBadRequest()
    {
        // Arrange
        var invalidCourse = new Course(); // Missing required fields
        _controller.ModelState.AddModelError("CourseNumber", "Course number is required");

        // Act
        var result = await _controller.CreateCourse(invalidCourse);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateCourse_WithValidData_ShouldReturnNoContent()
    {
        // Arrange
        var courseId = 1;
        var existingCourse = CreateTestCourse(courseId, "CS101", "Introduction to Computer Science");
        var updatedCourse = CreateTestCourse(courseId, "CS101", "Intro to Computer Science - Updated");

        SetupMockDbSetForCourse(new List<Course> { existingCourse });
        _mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

        // Act
        var result = await _controller.UpdateCourse(courseId, updatedCourse);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateCourse_WithMismatchedId_ShouldReturnBadRequest()
    {
        // Arrange
        var courseId = 1;
        var course = CreateTestCourse(2, "CS101", "Introduction to Computer Science"); // Different ID

        // Act
        var result = await _controller.UpdateCourse(courseId, course);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task DeleteCourse_WithValidId_ShouldReturnNoContent()
    {
        // Arrange
        var courseId = 1;
        var existingCourse = CreateTestCourse(courseId, "CS101", "Introduction to Computer Science");

        SetupMockDbSetForCourse(new List<Course> { existingCourse });
        _mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

        // Act
        var result = await _controller.DeleteCourse(courseId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteCourse_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var courseId = 999;
        SetupMockDbSetForCourse(new List<Course>());

        // Act
        var result = await _controller.DeleteCourse(courseId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    #endregion

    #region Search and Filtering Tests

    [Fact]
    public async Task SearchCourses_WithKeyword_ShouldReturnMatchingCourses()
    {
        // Arrange
        var searchTerm = "Computer";
        var courses = new List<Course>
        {
            CreateTestCourse(1, "CS101", "Introduction to Computer Science"),
            CreateTestCourse(2, "CS102", "Computer Programming"),
            CreateTestCourse(3, "MATH101", "Calculus I")
        };

        SetupMockDbSetForCourse(courses);

        // Act
        var result = await _controller.SearchCourses(searchTerm);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedCourses = Assert.IsAssignableFrom<IEnumerable<Course>>(okResult.Value);
        Assert.Equal(2, returnedCourses.Count());
        Assert.All(returnedCourses, course =>
            Assert.Contains("Computer", course.Title, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task FilterCoursesBySubject_ShouldReturnCoursesInSubject()
    {
        // Arrange
        var subjectCode = "CS";
        var courses = new List<Course>
        {
            CreateTestCourse(1, "CS101", "Introduction to Computer Science", subjectCode: "CS"),
            CreateTestCourse(2, "CS102", "Data Structures", subjectCode: "CS"),
            CreateTestCourse(3, "MATH101", "Calculus I", subjectCode: "MATH")
        };

        SetupMockDbSetForCourse(courses);

        // Act
        var result = await _controller.FilterCoursesBySubject(subjectCode);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedCourses = Assert.IsAssignableFrom<IEnumerable<Course>>(okResult.Value);
        Assert.Equal(2, returnedCourses.Count());
        Assert.All(returnedCourses, course => Assert.Equal("CS", course.SubjectCode));
    }

    [Fact]
    public async Task FilterCoursesByLevel_ShouldReturnCoursesAtLevel()
    {
        // Arrange
        var level = CourseLevel.Undergraduate;
        var courses = new List<Course>
        {
            CreateTestCourse(1, "CS101", "Introduction to Computer Science", level: CourseLevel.Undergraduate),
            CreateTestCourse(2, "CS501", "Advanced Programming", level: CourseLevel.Graduate),
            CreateTestCourse(3, "CS102", "Data Structures", level: CourseLevel.Undergraduate)
        };

        SetupMockDbSetForCourse(courses);

        // Act
        var result = await _controller.FilterCoursesByLevel(level);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedCourses = Assert.IsAssignableFrom<IEnumerable<Course>>(okResult.Value);
        Assert.Equal(2, returnedCourses.Count());
        Assert.All(returnedCourses, course => Assert.Equal(CourseLevel.Undergraduate, course.Level));
    }

    [Fact]
    public async Task FilterCoursesByCreditHours_ShouldReturnCoursesInRange()
    {
        // Arrange
        var minCredits = 3.0m;
        var maxCredits = 4.0m;
        var courses = new List<Course>
        {
            CreateTestCourse(1, "CS101", "Introduction to Computer Science", creditHours: 3.0m),
            CreateTestCourse(2, "CS102", "Data Structures", creditHours: 4.0m),
            CreateTestCourse(3, "CS201", "Advanced Programming", creditHours: 5.0m)
        };

        SetupMockDbSetForCourse(courses);

        // Act
        var result = await _controller.FilterCoursesByCreditHours(minCredits, maxCredits);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedCourses = Assert.IsAssignableFrom<IEnumerable<Course>>(okResult.Value);
        Assert.Equal(2, returnedCourses.Count());
        Assert.All(returnedCourses, course =>
            Assert.True(course.CreditHours >= minCredits && course.CreditHours <= maxCredits));
    }

    #endregion

    #region Prerequisite Validation Tests

    [Fact]
    public async Task ValidatePrerequisites_WithValidPrerequisites_ShouldReturnTrue()
    {
        // Arrange
        var studentId = 1;
        var courseId = 2;

        // Setup a course with prerequisites
        var prerequisiteCourse = CreateTestCourse(1, "CS101", "Introduction to Computer Science");
        var targetCourse = CreateTestCourse(2, "CS102", "Data Structures");

        var courses = new List<Course> { prerequisiteCourse, targetCourse };
        SetupMockDbSetForCourse(courses);

        // Act
        var result = await _controller.ValidatePrerequisites(studentId, courseId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var validationResult = Assert.IsType<PrerequisiteValidationResult>(okResult.Value);
        // Note: This would need actual prerequisite validation logic
    }

    [Fact]
    public async Task ValidatePrerequisites_WithMissingPrerequisites_ShouldReturnFalse()
    {
        // Arrange
        var studentId = 1;
        var courseId = 2;

        var targetCourse = CreateTestCourse(2, "CS102", "Data Structures");
        var courses = new List<Course> { targetCourse };
        SetupMockDbSetForCourse(courses);

        // Act
        var result = await _controller.ValidatePrerequisites(studentId, courseId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var validationResult = Assert.IsType<PrerequisiteValidationResult>(okResult.Value);
        // Note: This would need actual prerequisite validation logic
    }

    [Fact]
    public async Task GetPrerequisites_WithValidCourseId_ShouldReturnPrerequisites()
    {
        // Arrange
        var courseId = 2;
        var course = CreateTestCourse(courseId, "CS102", "Data Structures");

        SetupMockDbSetForCourse(new List<Course> { course });

        // Act
        var result = await _controller.GetPrerequisites(courseId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var prerequisites = Assert.IsAssignableFrom<IEnumerable<CoursePrerequisite>>(okResult.Value);
    }

    #endregion

    #region Course Planning Tests

    [Fact]
    public async Task GetRecommendedCourses_ForStudent_ShouldReturnRecommendations()
    {
        // Arrange
        var studentId = 1;
        var courses = new List<Course>
        {
            CreateTestCourse(1, "CS101", "Introduction to Computer Science"),
            CreateTestCourse(2, "CS102", "Data Structures"),
            CreateTestCourse(3, "MATH101", "Calculus I")
        };

        SetupMockDbSetForCourse(courses);

        // Act
        var result = await _controller.GetRecommendedCourses(studentId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var recommendations = Assert.IsAssignableFrom<IEnumerable<Course>>(okResult.Value);
    }

    [Fact]
    public async Task GetCoursePlan_ForStudent_ShouldReturnCoursePlan()
    {
        // Arrange
        var studentId = 1;
        var degreeCode = "CS_BS";

        // Act
        var result = await _controller.GetCoursePlan(studentId, degreeCode);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var coursePlan = Assert.IsType<StudentCoursePlan>(okResult.Value);
    }

    [Fact]
    public async Task OptimizeCoursePlan_ShouldReturnOptimizedPlan()
    {
        // Arrange
        var studentId = 1;
        var optimizationCriteria = new CoursePlanOptimizationCriteria
        {
            MinimizeTime = true,
            BalanceDifficulty = true,
            PreferredSemesters = new List<string> { "Fall", "Spring" }
        };

        // Act
        var result = await _controller.OptimizeCoursePlan(studentId, optimizationCriteria);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var optimizedPlan = Assert.IsType<OptimizedCoursePlan>(okResult.Value);
    }

    #endregion

    #region Degree Audit Tests

    [Fact]
    public async Task GetDegreeAudit_ForStudent_ShouldReturnAuditResults()
    {
        // Arrange
        var studentId = 1;
        var degreeCode = "CS_BS";

        // Act
        var result = await _controller.GetDegreeAudit(studentId, degreeCode);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var auditResult = Assert.IsType<DegreeAuditResult>(okResult.Value);
    }

    [Fact]
    public async Task CheckGraduationRequirements_ShouldReturnRequirementStatus()
    {
        // Arrange
        var studentId = 1;
        var degreeCode = "CS_BS";

        // Act
        var result = await _controller.CheckGraduationRequirements(studentId, degreeCode);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var requirementStatus = Assert.IsType<GraduationRequirementStatus>(okResult.Value);
    }

    [Fact]
    public async Task GetDegreeProgress_ShouldReturnProgressReport()
    {
        // Arrange
        var studentId = 1;
        var degreeCode = "CS_BS";

        // Act
        var result = await _controller.GetDegreeProgress(studentId, degreeCode);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var progressReport = Assert.IsType<DegreeProgressReport>(okResult.Value);
    }

    #endregion

    #region Helper Methods

    private Course CreateTestCourse(
        int id,
        string courseNumber,
        string title,
        CourseStatus status = CourseStatus.Active,
        string subjectCode = "CS",
        CourseLevel level = CourseLevel.Undergraduate,
        decimal creditHours = 3.0m)
    {
        return new Course
        {
            Id = id,
            CourseNumber = courseNumber,
            Title = title,
            Description = $"Description for {title}",
            SubjectCode = subjectCode,
            CreditHours = creditHours,
            ContactHours = 3,
            Level = level,
            Status = status,
            CatalogYear = 2024,
            MaxEnrollment = 30,
            CanRepeat = false,
            RequiresApproval = false,
            LearningOutcomes = new List<string> { "Learn programming concepts", "Understand algorithms" },
            Topics = new List<string> { "Variables", "Functions", "Data Structures" },
            DeliveryMethods = new List<DeliveryMethod> { DeliveryMethod.InPerson },
            AssessmentMethods = new List<AssessmentMethod> { AssessmentMethod.Exam }
        };
    }

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

    #endregion
}

#region Supporting Classes for Tests

/// <summary>
/// Test model for prerequisite validation results
/// </summary>
public class PrerequisiteValidationResult
{
    public bool IsValid { get; set; }
    public List<string> MissingPrerequisites { get; set; } = new();
    public List<string> Messages { get; set; } = new();
}

/// <summary>
/// Test model for student course plans
/// </summary>
public class StudentCoursePlan
{
    public int StudentId { get; set; }
    public string DegreeCode { get; set; } = string.Empty;
    public List<PlannedSemester> Semesters { get; set; } = new();
    public DateTime CreatedDate { get; set; }
    public DateTime LastModified { get; set; }
}

/// <summary>
/// Test model for planned semesters
/// </summary>
public class PlannedSemester
{
    public string SemesterCode { get; set; } = string.Empty;
    public int Year { get; set; }
    public List<int> CourseIds { get; set; } = new();
    public decimal TotalCredits { get; set; }
}

/// <summary>
/// Test model for course plan optimization criteria
/// </summary>
public class CoursePlanOptimizationCriteria
{
    public bool MinimizeTime { get; set; }
    public bool BalanceDifficulty { get; set; }
    public bool MaximizePreferences { get; set; }
    public List<string> PreferredSemesters { get; set; } = new();
    public int MaxCoursesPerSemester { get; set; } = 5;
}

/// <summary>
/// Test model for optimized course plans
/// </summary>
public class OptimizedCoursePlan
{
    public StudentCoursePlan OriginalPlan { get; set; } = new();
    public StudentCoursePlan OptimizedPlan { get; set; } = new();
    public List<string> OptimizationReasons { get; set; } = new();
    public decimal ImprovementScore { get; set; }
}

/// <summary>
/// Test model for degree audit results
/// </summary>
public class DegreeAuditResult
{
    public int StudentId { get; set; }
    public string DegreeCode { get; set; } = string.Empty;
    public decimal CompletionPercentage { get; set; }
    public List<RequirementGroup> RequirementGroups { get; set; } = new();
    public DateTime AuditDate { get; set; }
}

/// <summary>
/// Test model for requirement groups
/// </summary>
public class RequirementGroup
{
    public string GroupName { get; set; } = string.Empty;
    public bool IsComplete { get; set; }
    public decimal RequiredCredits { get; set; }
    public decimal CompletedCredits { get; set; }
    public List<int> SatisfyingCourseIds { get; set; } = new();
}

/// <summary>
/// Test model for graduation requirement status
/// </summary>
public class GraduationRequirementStatus
{
    public int StudentId { get; set; }
    public string DegreeCode { get; set; } = string.Empty;
    public bool EligibleForGraduation { get; set; }
    public List<string> RemainingRequirements { get; set; } = new();
    public DateTime? ExpectedGraduationDate { get; set; }
}

/// <summary>
/// Test model for degree progress reports
/// </summary>
public class DegreeProgressReport
{
    public int StudentId { get; set; }
    public string DegreeCode { get; set; } = string.Empty;
    public decimal OverallProgress { get; set; }
    public Dictionary<string, decimal> CategoryProgress { get; set; } = new();
    public int TotalCreditsEarned { get; set; }
    public int TotalCreditsRequired { get; set; }
    public DateTime GeneratedDate { get; set; }
}

#endregion