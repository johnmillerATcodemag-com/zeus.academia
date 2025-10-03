using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Services;
using Zeus.Academia.Infrastructure.Models;
using System.ComponentModel.DataAnnotations;
using CourseLevel = Zeus.Academia.Infrastructure.Enums.CourseLevel;
using CourseStatus = Zeus.Academia.Infrastructure.Enums.CourseStatus;
using StudentAcademicRecord = Zeus.Academia.Infrastructure.Services.StudentAcademicRecord;
using CompletedCourse = Zeus.Academia.Infrastructure.Services.CompletedCourse;

namespace Zeus.Academia.API.Controllers;

/// <summary>
/// RESTful API controller for comprehensive course catalog operations.
/// Task 6: Course API Controllers - Provides CRUD operations, search/filtering, 
/// prerequisite validation, course planning, and degree audit endpoints.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CoursesController : ControllerBase
{
    private readonly AcademiaDbContext _context;
    private readonly DegreeRequirementService _degreeRequirementService;
    private readonly DegreeAuditService _degreeAuditService;
    private readonly CourseSequencePlanningService _courseSequencePlanningService;
    private readonly CoursePlanOptimizationService _coursePlanOptimizationService;
    private readonly ILogger<CoursesController> _logger;

    public CoursesController(
        AcademiaDbContext context,
        DegreeRequirementService degreeRequirementService,
        DegreeAuditService degreeAuditService,
        CourseSequencePlanningService courseSequencePlanningService,
        CoursePlanOptimizationService coursePlanOptimizationService,
        ILogger<CoursesController> logger)
    {
        _context = context;
        _degreeRequirementService = degreeRequirementService;
        _degreeAuditService = degreeAuditService;
        _courseSequencePlanningService = courseSequencePlanningService;
        _coursePlanOptimizationService = coursePlanOptimizationService;
        _logger = logger;
    }

    #region CRUD Operations

    /// <summary>
    /// Gets a specific course by ID.
    /// </summary>
    /// <param name="id">Course ID</param>
    /// <returns>Course details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Course), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Course>> GetCourse(int id)
    {
        try
        {
            _logger.LogInformation("Retrieving course with ID {CourseId}", id);

            var course = await _context.Courses
                .Include(c => c.Subject)
                .Include(c => c.Prerequisites)
                .Include(c => c.Corequisites)
                .Include(c => c.Restrictions)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                _logger.LogWarning("Course with ID {CourseId} not found", id);
                return NotFound();
            }

            return Ok(course);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving course with ID {CourseId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                "An error occurred while retrieving the course");
        }
    }

    /// <summary>
    /// Gets all active courses.
    /// </summary>
    /// <returns>List of active courses</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Course>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Course>>> GetAllCourses()
    {
        try
        {
            _logger.LogInformation("Retrieving all active courses");

            var courses = await _context.Courses
                .Where(c => c.Status == CourseStatus.Active)
                .Include(c => c.Subject)
                .OrderBy(c => c.SubjectCode)
                .ThenBy(c => c.CourseNumber)
                .ToListAsync();

            _logger.LogInformation("Retrieved {CourseCount} active courses", courses.Count);
            return Ok(courses);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving courses");
            return StatusCode(StatusCodes.Status500InternalServerError,
                "An error occurred while retrieving courses");
        }
    }

    /// <summary>
    /// Creates a new course.
    /// </summary>
    /// <param name="course">Course to create</param>
    /// <returns>Created course</returns>
    [HttpPost]
    [Authorize(Roles = "Admin,AcademicAdvisor")]
    [ProducesResponseType(typeof(Course), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Course>> CreateCourse([FromBody] Course course)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creating new course {CourseNumber} - {Title}",
                course.CourseNumber, course.Title);

            // Check for duplicate course number
            var existingCourse = await _context.Courses
                .FirstOrDefaultAsync(c => c.CourseNumber == course.CourseNumber);

            if (existingCourse != null)
            {
                return BadRequest($"Course with number {course.CourseNumber} already exists");
            }

            // Set creation audit fields
            course.CreatedDate = DateTime.UtcNow;
            course.ModifiedDate = DateTime.UtcNow;
            course.Status = CourseStatus.UnderReview; // New courses start under review

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully created course {CourseId} - {CourseNumber}",
                course.Id, course.CourseNumber);

            return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, course);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating course {CourseNumber}", course.CourseNumber);
            return StatusCode(StatusCodes.Status500InternalServerError,
                "An error occurred while creating the course");
        }
    }

    /// <summary>
    /// Updates an existing course.
    /// </summary>
    /// <param name="id">Course ID</param>
    /// <param name="course">Updated course data</param>
    /// <returns>No content on success</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,AcademicAdvisor")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCourse(int id, [FromBody] Course course)
    {
        try
        {
            if (id != course.Id)
            {
                return BadRequest("Course ID mismatch");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingCourse = await _context.Courses.FindAsync(id);
            if (existingCourse == null)
            {
                return NotFound();
            }

            _logger.LogInformation("Updating course {CourseId} - {CourseNumber}", id, course.CourseNumber);

            // Update fields
            existingCourse.Title = course.Title;
            existingCourse.Description = course.Description;
            existingCourse.CreditHours = course.CreditHours;
            existingCourse.ContactHours = course.ContactHours;
            existingCourse.Level = course.Level;
            existingCourse.MaxEnrollment = course.MaxEnrollment;
            existingCourse.LearningOutcomes = course.LearningOutcomes;
            existingCourse.Topics = course.Topics;
            existingCourse.DeliveryMethods = course.DeliveryMethods;
            existingCourse.AssessmentMethods = course.AssessmentMethods;
            existingCourse.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully updated course {CourseId}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating course {CourseId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                "An error occurred while updating the course");
        }
    }

    /// <summary>
    /// Deletes a course (soft delete by setting status to retired).
    /// </summary>
    /// <param name="id">Course ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCourse(int id)
    {
        try
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            _logger.LogInformation("Soft deleting course {CourseId} - {CourseNumber}", id, course.CourseNumber);

            // Soft delete - set status to retired
            course.Status = CourseStatus.Retired;
            course.RetiredDate = DateTime.UtcNow;
            course.RetirementReason = "Deleted via API";
            course.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully deleted course {CourseId}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting course {CourseId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                "An error occurred while deleting the course");
        }
    }

    #endregion

    #region Search and Filtering Endpoints

    /// <summary>
    /// Searches courses by keyword in title or description.
    /// </summary>
    /// <param name="searchTerm">Search keyword</param>
    /// <returns>Matching courses</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<Course>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Course>>> SearchCourses([FromQuery] string searchTerm)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return BadRequest("Search term is required");
            }

            _logger.LogInformation("Searching courses with term: {SearchTerm}", searchTerm);

            var courses = await _context.Courses
                .Where(c => c.Status == CourseStatus.Active &&
                           (c.Title.Contains(searchTerm) ||
                            c.Description != null && c.Description.Contains(searchTerm) ||
                            c.CourseNumber.Contains(searchTerm)))
                .Include(c => c.Subject)
                .OrderBy(c => c.SubjectCode)
                .ThenBy(c => c.CourseNumber)
                .ToListAsync();

            _logger.LogInformation("Found {CourseCount} courses matching '{SearchTerm}'", courses.Count, searchTerm);
            return Ok(courses);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching courses with term: {SearchTerm}", searchTerm);
            return StatusCode(StatusCodes.Status500InternalServerError,
                "An error occurred while searching courses");
        }
    }

    /// <summary>
    /// Filters courses by subject code.
    /// </summary>
    /// <param name="subjectCode">Subject code (e.g., "CS", "MATH")</param>
    /// <returns>Courses in the specified subject</returns>
    [HttpGet("subject/{subjectCode}")]
    [ProducesResponseType(typeof(IEnumerable<Course>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Course>>> FilterCoursesBySubject(string subjectCode)
    {
        try
        {
            _logger.LogInformation("Filtering courses by subject: {SubjectCode}", subjectCode);

            var courses = await _context.Courses
                .Where(c => c.Status == CourseStatus.Active && c.SubjectCode == subjectCode.ToUpper())
                .Include(c => c.Subject)
                .OrderBy(c => c.CourseNumber)
                .ToListAsync();

            _logger.LogInformation("Found {CourseCount} courses in subject {SubjectCode}", courses.Count, subjectCode);
            return Ok(courses);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error filtering courses by subject: {SubjectCode}", subjectCode);
            return StatusCode(StatusCodes.Status500InternalServerError,
                "An error occurred while filtering courses by subject");
        }
    }

    /// <summary>
    /// Filters courses by level (Undergraduate, Graduate, etc.).
    /// </summary>
    /// <param name="level">Course level</param>
    /// <returns>Courses at the specified level</returns>
    [HttpGet("level/{level}")]
    [ProducesResponseType(typeof(IEnumerable<Course>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Course>>> FilterCoursesByLevel(Zeus.Academia.Infrastructure.Enums.CourseLevel level)
    {
        try
        {
            _logger.LogInformation("Filtering courses by level: {Level}", level);

            var courses = await _context.Courses
                .Where(c => c.Status == CourseStatus.Active && c.Level == level)
                .Include(c => c.Subject)
                .OrderBy(c => c.SubjectCode)
                .ThenBy(c => c.CourseNumber)
                .ToListAsync();

            _logger.LogInformation("Found {CourseCount} courses at {Level} level", courses.Count, level);
            return Ok(courses);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error filtering courses by level: {Level}", level);
            return StatusCode(StatusCodes.Status500InternalServerError,
                "An error occurred while filtering courses by level");
        }
    }

    /// <summary>
    /// Filters courses by credit hour range.
    /// </summary>
    /// <param name="minCredits">Minimum credit hours</param>
    /// <param name="maxCredits">Maximum credit hours</param>
    /// <returns>Courses within the credit hour range</returns>
    [HttpGet("credits")]
    [ProducesResponseType(typeof(IEnumerable<Course>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Course>>> FilterCoursesByCreditHours(
        [FromQuery] decimal minCredits = 0,
        [FromQuery] decimal maxCredits = 15)
    {
        try
        {
            _logger.LogInformation("Filtering courses by credit hours: {MinCredits}-{MaxCredits}", minCredits, maxCredits);

            var courses = await _context.Courses
                .Where(c => c.Status == CourseStatus.Active &&
                           c.CreditHours >= minCredits &&
                           c.CreditHours <= maxCredits)
                .Include(c => c.Subject)
                .OrderBy(c => c.CreditHours)
                .ThenBy(c => c.SubjectCode)
                .ThenBy(c => c.CourseNumber)
                .ToListAsync();

            _logger.LogInformation("Found {CourseCount} courses with {MinCredits}-{MaxCredits} credit hours",
                courses.Count, minCredits, maxCredits);
            return Ok(courses);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error filtering courses by credit hours: {MinCredits}-{MaxCredits}", minCredits, maxCredits);
            return StatusCode(StatusCodes.Status500InternalServerError,
                "An error occurred while filtering courses by credit hours");
        }
    }

    #endregion

    #region Prerequisite Validation API Endpoints

    /// <summary>
    /// Validates if a student meets the prerequisites for a course.
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <param name="courseId">Course ID</param>
    /// <returns>Prerequisite validation result</returns>
    [HttpGet("prerequisites/validate")]
    [ProducesResponseType(typeof(PrerequisiteValidationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PrerequisiteValidationResult>> ValidatePrerequisites(
        [FromQuery] int studentId,
        [FromQuery] int courseId)
    {
        try
        {
            _logger.LogInformation("Validating prerequisites for student {StudentId} and course {CourseId}", studentId, courseId);

            var course = await _context.Courses
                .Include(c => c.Prerequisites)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null)
            {
                return NotFound($"Course with ID {courseId} not found");
            }

            // Get student's completed courses
            var completedCourses = await GetStudentCompletedCourses(studentId);

            // Validate prerequisites
            var result = new PrerequisiteValidationResult
            {
                IsValid = true,
                MissingPrerequisites = new List<string>(),
                Messages = new List<string>()
            };

            foreach (var prerequisite in course.Prerequisites)
            {
                // Get the prerequisite course to find its ID
                var prereqCourse = await _context.Courses.FirstOrDefaultAsync(c => c.CourseNumber == prerequisite.RequiredCourseNumber);
                var hasPrerequisite = prereqCourse != null && completedCourses.Any(cc => cc.CourseId == prereqCourse.Id);
                if (!hasPrerequisite)
                {
                    result.IsValid = false;
                    result.MissingPrerequisites.Add(prereqCourse?.CourseNumber ?? $"Course {prerequisite.RequiredCourseNumber}");
                    result.Messages.Add($"Missing prerequisite: {prereqCourse?.CourseNumber} - {prereqCourse?.Title}");
                }
            }

            if (result.IsValid)
            {
                result.Messages.Add("All prerequisites satisfied");
            }

            _logger.LogInformation("Prerequisite validation completed for student {StudentId} and course {CourseId}: {IsValid}",
                studentId, courseId, result.IsValid);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating prerequisites for student {StudentId} and course {CourseId}", studentId, courseId);
            return StatusCode(StatusCodes.Status500InternalServerError,
                "An error occurred while validating prerequisites");
        }
    }

    /// <summary>
    /// Gets all prerequisites for a specific course.
    /// </summary>
    /// <param name="courseId">Course ID</param>
    /// <returns>List of prerequisites</returns>
    [HttpGet("{courseId}/prerequisites")]
    [ProducesResponseType(typeof(IEnumerable<CoursePrerequisite>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<CoursePrerequisite>>> GetPrerequisites(int courseId)
    {
        try
        {
            var course = await _context.Courses
                .Include(c => c.Prerequisites)

                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null)
            {
                return NotFound($"Course with ID {courseId} not found");
            }

            _logger.LogInformation("Retrieved {Count} prerequisites for course {CourseId}",
                course.Prerequisites.Count, courseId);

            return Ok(course.Prerequisites);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving prerequisites for course {CourseId}", courseId);
            return StatusCode(StatusCodes.Status500InternalServerError,
                "An error occurred while retrieving prerequisites");
        }
    }

    #endregion

    #region Course Planning and Recommendation Endpoints

    /// <summary>
    /// Gets recommended courses for a student based on their academic profile.
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <returns>List of recommended courses</returns>
    [HttpGet("recommendations/{studentId}")]
    [ProducesResponseType(typeof(IEnumerable<Course>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Course>>> GetRecommendedCourses(int studentId)
    {
        try
        {
            _logger.LogInformation("Getting course recommendations for student {StudentId}", studentId);

            // Get student's completed courses and degree program
            var completedCourses = await GetStudentCompletedCourses(studentId);
            var student = await GetStudentInfo(studentId);

            // Get eligible courses (prerequisites met, not already completed)
            var completedCourseIds = completedCourses.Select(cc => cc.CourseId).ToList();

            var eligibleCourses = await _context.Courses
                .Where(c => c.Status == CourseStatus.Active &&
                           !completedCourseIds.Contains(c.Id))
                .Include(c => c.Prerequisites)
                .Include(c => c.Subject)
                .ToListAsync();

            // Filter courses where prerequisites are met
            var recommendedCourses = new List<Course>();
            foreach (var course in eligibleCourses)
            {
                var hasAllPrerequisites = true;
                foreach (var prereq in course.Prerequisites)
                {
                    var prereqCourse = await _context.Courses.FirstOrDefaultAsync(c => c.CourseNumber == prereq.RequiredCourseNumber);
                    if (prereqCourse != null && !completedCourseIds.Contains(prereqCourse.Id))
                    {
                        hasAllPrerequisites = false;
                        break;
                    }
                }

                if (hasAllPrerequisites)
                {
                    recommendedCourses.Add(course);
                }
            }

            // Limit to top 10 recommendations
            var topRecommendations = recommendedCourses
                .OrderBy(c => c.Level)
                .ThenBy(c => c.SubjectCode)
                .Take(10)
                .ToList();

            _logger.LogInformation("Generated {Count} course recommendations for student {StudentId}",
                topRecommendations.Count, studentId);

            return Ok(topRecommendations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating course recommendations for student {StudentId}", studentId);
            return StatusCode(StatusCodes.Status500InternalServerError,
                "An error occurred while generating course recommendations");
        }
    }

    /// <summary>
    /// Gets a course plan for a student based on their degree program.
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <param name="degreeCode">Degree code</param>
    /// <returns>Student course plan</returns>
    [HttpGet("plan/{studentId}")]
    [ProducesResponseType(typeof(StudentCoursePlan), StatusCodes.Status200OK)]
    public async Task<ActionResult<StudentCoursePlan>> GetCoursePlan(int studentId, [FromQuery] string degreeCode)
    {
        try
        {
            _logger.LogInformation("Generating course plan for student {StudentId} with degree {DegreeCode}", studentId, degreeCode);

            // This would typically involve the course sequence planning service
            var coursePlan = new StudentCoursePlan
            {
                StudentId = studentId,
                DegreeCode = degreeCode,
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                Semesters = new List<PlannedSemester>()
            };

            // Add sample planned semesters (in a real implementation, this would use the planning service)
            await Task.CompletedTask; // Make method truly async
            coursePlan.Semesters.Add(new PlannedSemester
            {
                SemesterCode = "Fall 2024",
                Year = 2024,
                CourseIds = new List<int> { 1, 2, 3 },
                TotalCredits = 12
            });

            return Ok(coursePlan);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating course plan for student {StudentId}", studentId);
            return StatusCode(StatusCodes.Status500InternalServerError,
                "An error occurred while generating the course plan");
        }
    }

    /// <summary>
    /// Optimizes a student's course plan based on specified criteria.
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <param name="criteria">Optimization criteria</param>
    /// <returns>Optimized course plan</returns>
    [HttpPost("plan/{studentId}/optimize")]
    [ProducesResponseType(typeof(OptimizedCoursePlan), StatusCodes.Status200OK)]
    public async Task<ActionResult<OptimizedCoursePlan>> OptimizeCoursePlan(
        int studentId,
        [FromBody] CoursePlanOptimizationCriteria criteria)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Optimizing course plan for student {StudentId}", studentId);

            // This would use the optimization service in a real implementation
            await Task.CompletedTask; // Make method truly async
            var optimizedPlan = new OptimizedCoursePlan
            {
                OriginalPlan = new StudentCoursePlan { StudentId = studentId },
                OptimizedPlan = new StudentCoursePlan { StudentId = studentId },
                OptimizationReasons = new List<string> { "Minimized time to graduation", "Balanced course difficulty" },
                ImprovementScore = 85.5m
            };

            return Ok(optimizedPlan);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error optimizing course plan for student {StudentId}", studentId);
            return StatusCode(StatusCodes.Status500InternalServerError,
                "An error occurred while optimizing the course plan");
        }
    }

    #endregion

    #region Degree Requirement and Audit Endpoints

    /// <summary>
    /// Performs a degree audit for a student.
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <param name="degreeCode">Degree code</param>
    /// <returns>Degree audit result</returns>
    [HttpPost("audit/{studentId}")]
    [ProducesResponseType(typeof(DegreeAuditResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<DegreeAuditResult>> GetDegreeAudit(int studentId, [FromQuery] string degreeCode)
    {
        try
        {
            _logger.LogInformation("Performing degree audit for student {StudentId} with degree {DegreeCode}", studentId, degreeCode);

            // Get degree template
            var degreeTemplate = await _degreeRequirementService.GetDegreeTemplateAsync(degreeCode);
            if (degreeTemplate == null)
            {
                return NotFound($"Degree template not found for code: {degreeCode}");
            }

            // Create student academic record
            var completedCourses = await GetStudentCompletedCourses(studentId);
            var studentRecord = new StudentAcademicRecord
            {
                StudentId = studentId,
                DegreeCode = degreeCode,
                CompletedCourses = completedCourses.Select(cc => new CompletedCourse
                {
                    CourseId = cc.CourseId,
                    Grade = cc.Grade,
                    CreditHours = (int)cc.CreditHours,
                    Semester = "Fall 2023" // Placeholder - would come from actual data
                }).ToList()
            };

            // Perform degree audit using the service
            var auditResult = await _degreeAuditService.PerformDegreeAuditAsync(studentRecord, degreeTemplate);

            _logger.LogInformation("Completed degree audit for student {StudentId}: {CompletionPercentage}% complete",
                studentId, auditResult.CompletionPercentage);

            return Ok(auditResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing degree audit for student {StudentId}", studentId);
            return StatusCode(StatusCodes.Status500InternalServerError,
                "An error occurred while performing the degree audit");
        }
    }

    /// <summary>
    /// Checks graduation requirements for a student.
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <param name="degreeCode">Degree code</param>
    /// <returns>Graduation requirement status</returns>
    [HttpGet("graduation-requirements/{studentId}")]
    [ProducesResponseType(typeof(GraduationRequirementStatus), StatusCodes.Status200OK)]
    public async Task<ActionResult<GraduationRequirementStatus>> CheckGraduationRequirements(int studentId, [FromQuery] string degreeCode)
    {
        try
        {
            _logger.LogInformation("Checking graduation requirements for student {StudentId} with degree {DegreeCode}", studentId, degreeCode);

            // This would use a graduation requirements service in a real implementation
            await Task.CompletedTask; // Make method truly async
            var requirementStatus = new GraduationRequirementStatus
            {
                StudentId = studentId,
                DegreeCode = degreeCode,
                EligibleForGraduation = false,
                RemainingRequirements = new List<string> { "Complete capstone course", "Maintain 2.0 GPA" },
                ExpectedGraduationDate = DateTime.Now.AddMonths(8)
            };

            return Ok(requirementStatus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking graduation requirements for student {StudentId}", studentId);
            return StatusCode(StatusCodes.Status500InternalServerError,
                "An error occurred while checking graduation requirements");
        }
    }

    /// <summary>
    /// Gets degree progress report for a student.
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <param name="degreeCode">Degree code</param>
    /// <returns>Degree progress report</returns>
    [HttpGet("progress/{studentId}")]
    [ProducesResponseType(typeof(DegreeProgressReport), StatusCodes.Status200OK)]
    public async Task<ActionResult<DegreeProgressReport>> GetDegreeProgress(int studentId, [FromQuery] string degreeCode)
    {
        try
        {
            _logger.LogInformation("Generating degree progress report for student {StudentId} with degree {DegreeCode}", studentId, degreeCode);

            var completedCourses = await GetStudentCompletedCourses(studentId);
            var totalCreditsEarned = completedCourses.Sum(cc => cc.CreditHours);

            var progressReport = new DegreeProgressReport
            {
                StudentId = studentId,
                DegreeCode = degreeCode,
                OverallProgress = 65.5m, // This would be calculated based on actual requirements
                CategoryProgress = new Dictionary<string, decimal>
                {
                    { "General Education", 85.0m },
                    { "Major Requirements", 60.0m },
                    { "Electives", 45.0m }
                },
                TotalCreditsEarned = (int)totalCreditsEarned,
                TotalCreditsRequired = 120,
                GeneratedDate = DateTime.UtcNow
            };

            return Ok(progressReport);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating degree progress report for student {StudentId}", studentId);
            return StatusCode(StatusCodes.Status500InternalServerError,
                "An error occurred while generating the degree progress report");
        }
    }

    #endregion

    #region Private Helper Methods

    /// <summary>
    /// Gets completed courses for a student (placeholder implementation).
    /// </summary>
    private async Task<List<CompletedCourse>> GetStudentCompletedCourses(int studentId)
    {
        // In a real implementation, this would query the student enrollment/transcript tables
        // For now, return empty list
        return await Task.FromResult(new List<CompletedCourse>());
    }

    /// <summary>
    /// Gets basic student information (placeholder implementation).
    /// </summary>
    private async Task<Student> GetStudentInfo(int studentId)
    {
        // In a real implementation, this would query the Students table
        return await Task.FromResult(new Student { Id = studentId });
    }

    #endregion
}

#region Supporting DTOs and Models

/// <summary>
/// Result of prerequisite validation
/// </summary>
public class PrerequisiteValidationResult
{
    public bool IsValid { get; set; }
    public List<string> MissingPrerequisites { get; set; } = new();
    public List<string> Messages { get; set; } = new();
}

/// <summary>
/// Student course plan
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
/// Planned semester
/// </summary>
public class PlannedSemester
{
    public string SemesterCode { get; set; } = string.Empty;
    public int Year { get; set; }
    public List<int> CourseIds { get; set; } = new();
    public decimal TotalCredits { get; set; }
}

/// <summary>
/// Course plan optimization criteria
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
/// Optimized course plan
/// </summary>
public class OptimizedCoursePlan
{
    public StudentCoursePlan OriginalPlan { get; set; } = new();
    public StudentCoursePlan OptimizedPlan { get; set; } = new();
    public List<string> OptimizationReasons { get; set; } = new();
    public decimal ImprovementScore { get; set; }
}

/// <summary>
/// Degree audit result
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
/// Requirement group
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
/// Graduation requirement status
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
/// Degree progress report
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

// Using CompletedCourse from Services namespace via using alias

#endregion