using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using Zeus.Academia.Infrastructure.Services;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Enums;
using System.ComponentModel.DataAnnotations;
using CourseComparisonCriteria = Zeus.Academia.Infrastructure.Services.CourseComparisonCriteria;

namespace Zeus.Academia.API.Controllers;

/// <summary>
/// Course search and discovery API controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CourseDiscoveryController : ControllerBase
{
    private readonly ICourseSearchService _searchService;
    private readonly ICourseRecommendationService _recommendationService;
    private readonly ICourseComparisonService _comparisonService;
    private readonly ICourseWaitlistService _waitlistService;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CourseDiscoveryController> _logger;

    public CourseDiscoveryController(
        ICourseSearchService searchService,
        ICourseRecommendationService recommendationService,
        ICourseComparisonService comparisonService,
        ICourseWaitlistService waitlistService,
        IMemoryCache cache,
        ILogger<CourseDiscoveryController> logger)
    {
        _searchService = searchService;
        _recommendationService = recommendationService;
        _comparisonService = comparisonService;
        _waitlistService = waitlistService;
        _cache = cache;
        _logger = logger;
    }

    #region Course Search Endpoints

    /// <summary>
    /// Perform advanced course search with multiple criteria
    /// </summary>
    [HttpPost("search")]
    public async Task<ActionResult<CourseSearchResponse>> SearchCourses([FromBody] CourseSearchRequest request)
    {
        try
        {
            _logger.LogInformation("Course search requested with {FilterCount} filters",
                request.Filters?.Count ?? 0);

            var cacheKey = GenerateSearchCacheKey(request);

            if (_cache.TryGetValue(cacheKey, out CourseSearchResponse? cachedResponse))
            {
                _logger.LogInformation("Returning cached search results");
                return Ok(cachedResponse);
            }

            var searchCriteria = MapToSearchCriteria(request);
            var results = await _searchService.SearchCoursesAsync(searchCriteria);

            var response = new CourseSearchResponse
            {
                Results = results.Results.Select(c => new CourseSearchResultItem
                {
                    Id = c.Id,
                    CourseNumber = c.CourseNumber,
                    Title = c.Title,
                    Description = c.Description ?? string.Empty,
                    SubjectCode = c.SubjectCode,
                    CreditHours = c.CreditHours,
                    Level = c.Level,
                    Status = c.Status
                }).ToList(),
                Filters = results.Filters,
                TotalCount = results.TotalCount,
                PageSize = results.PageSize,
                CurrentPage = results.CurrentPage,
                SearchQuery = request.Query,
                SearchMetadata = results.SearchMetadata,
                ProcessingTime = TimeSpan.FromMilliseconds(results.ProcessingTime)
            };

            // Cache results for 5 minutes
            _cache.Set(cacheKey, response, TimeSpan.FromMinutes(5));

            _logger.LogInformation("Course search completed: {ResultCount} results found", results.TotalCount);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing course search");
            return StatusCode(500, new { error = "An error occurred while searching courses" });
        }
    }

    /// <summary>
    /// Get filtered courses based on specific criteria
    /// </summary>
    [HttpPost("filter")]
    public async Task<ActionResult<CourseFilterResponse>> FilterCourses([FromBody] CourseFilterRequest request)
    {
        try
        {
            _logger.LogInformation("Course filtering requested");

            var filterCriteria = MapToFilterCriteria(request);
            var results = await _searchService.FilterCoursesAsync(filterCriteria);

            var response = new CourseFilterResponse
            {
                Courses = results.Courses,
                AppliedFilters = results.AppliedFilters,
                AvailableFilters = results.AvailableFilters,
                TotalCount = results.TotalCount,
                FilterMetadata = results.FilterMetadata
            };

            _logger.LogInformation("Course filtering completed: {ResultCount} courses found", results.TotalCount);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error filtering courses");
            return StatusCode(500, new { error = "An error occurred while filtering courses" });
        }
    }

    /// <summary>
    /// Perform fuzzy search for courses
    /// </summary>
    [HttpPost("search/fuzzy")]
    public async Task<ActionResult<FuzzySearchResponse>> FuzzySearchCourses([FromBody] FuzzySearchRequest request)
    {
        try
        {
            _logger.LogInformation("Fuzzy search requested for query: {Query}", request.Query);

            if (string.IsNullOrWhiteSpace(request.Query))
            {
                return BadRequest(new { error = "Search query is required" });
            }

            var results = await _searchService.FuzzySearchAsync(request.Query, request.MaxResults ?? 20);

            var response = new FuzzySearchResponse
            {
                Query = request.Query,
                Results = results.Select(r => new FuzzySearchResult
                {
                    Course = r.Course,
                    SimilarityScore = r.SimilarityScore,
                    MatchedFields = r.MatchedFields
                }).ToList()
            };

            _logger.LogInformation("Fuzzy search completed: {ResultCount} results found", results.Count);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing fuzzy search");
            return StatusCode(500, new { error = "An error occurred while performing fuzzy search" });
        }
    }

    #endregion

    #region Course Recommendation Endpoints

    /// <summary>
    /// Get personalized course recommendations for a student
    /// </summary>
    [HttpGet("recommendations/{studentId}")]
    public async Task<ActionResult<CourseRecommendationResponse>> GetRecommendations(
        int studentId,
        [FromQuery] int maxRecommendations = 10)
    {
        try
        {
            _logger.LogInformation("Getting recommendations for student {StudentId}", studentId);

            // In a real implementation, you'd get this from user context or database
            var studentProfile = await GetStudentProfile(studentId);

            if (studentProfile == null)
            {
                return NotFound(new { error = "Student profile not found" });
            }

            var recommendations = await _recommendationService.GetPersonalizedRecommendationsAsync(
                studentProfile, maxRecommendations);

            var response = new CourseRecommendationResponse
            {
                StudentId = studentId,
                Recommendations = recommendations,
                GeneratedAt = DateTime.UtcNow,
                ProfileMetadata = new Dictionary<string, object>
                {
                    ["Major"] = studentProfile.Major ?? "Unknown",
                    ["ClassStanding"] = studentProfile.ClassStanding.ToString(),
                    ["GPA"] = studentProfile.CurrentGPA
                }
            };

            _logger.LogInformation("Generated {Count} recommendations for student {StudentId}",
                recommendations.Count, studentId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recommendations for student {StudentId}", studentId);
            return StatusCode(500, new { error = "An error occurred while generating recommendations" });
        }
    }

    /// <summary>
    /// Get degree-specific recommendations for a student
    /// </summary>
    [HttpGet("recommendations/{studentId}/degree")]
    public async Task<ActionResult<CourseRecommendationResponse>> GetDegreeRecommendations(int studentId)
    {
        try
        {
            _logger.LogInformation("Getting degree recommendations for student {StudentId}", studentId);

            var studentProfile = await GetStudentProfile(studentId);
            if (studentProfile == null)
            {
                return NotFound(new { error = "Student profile not found" });
            }

            var degreeRequirements = await GetDegreeRequirements(studentProfile.DegreeCode);
            if (degreeRequirements == null)
            {
                return BadRequest(new { error = "Degree requirements not found" });
            }

            var recommendations = await _recommendationService.RecommendCoursesForDegreeAsync(
                studentProfile, degreeRequirements);

            var response = new CourseRecommendationResponse
            {
                StudentId = studentId,
                Recommendations = recommendations,
                GeneratedAt = DateTime.UtcNow,
                ProfileMetadata = new Dictionary<string, object>
                {
                    ["DegreeCode"] = studentProfile.DegreeCode ?? "Unknown",
                    ["RequiredCoursesRemaining"] = degreeRequirements.RequiredCourses.Count -
                        (studentProfile.CompletedCourses?.Intersect(degreeRequirements.RequiredCourses).Count() ?? 0)
                }
            };

            _logger.LogInformation("Generated {Count} degree recommendations for student {StudentId}",
                recommendations.Count, studentId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting degree recommendations for student {StudentId}", studentId);
            return StatusCode(500, new { error = "An error occurred while generating degree recommendations" });
        }
    }

    #endregion

    #region Course Comparison Endpoints

    /// <summary>
    /// Compare multiple courses
    /// </summary>
    [HttpPost("comparison")]
    public async Task<ActionResult<CourseComparisonResponse>> CompareCourses([FromBody] CourseComparisonRequest request)
    {
        try
        {
            _logger.LogInformation("Comparing {Count} courses", request.CourseIds.Count);

            if (request.CourseIds.Count < 2)
            {
                return BadRequest(new { error = "At least 2 courses are required for comparison" });
            }

            if (request.CourseIds.Count > 10)
            {
                return BadRequest(new { error = "Maximum 10 courses can be compared at once" });
            }

            CourseComparison comparison;

            // Use provided criteria or default criteria
            var criteria = request.Criteria != null ? new CourseComparisonCriteria
            {
                IncludePrerequisites = request.Criteria.IncludePrerequisites,
                IncludeWorkload = request.Criteria.IncludeWorkload,
                IncludeDifficulty = request.Criteria.IncludeDifficulty,
                IncludeScheduleInfo = request.Criteria.IncludeScheduleInfo,
                IncludeLearningOutcomes = request.Criteria.IncludeLearningOutcomes,
                IncludeInstructorRatings = request.Criteria.IncludeInstructorRatings,
                HighlightDifferences = request.Criteria.HighlightDifferences,
                DifferenceThreshold = request.Criteria.DifferenceThreshold
            } : new CourseComparisonCriteria
            {
                IncludePrerequisites = true,
                IncludeWorkload = true,
                IncludeDifficulty = true,
                IncludeScheduleInfo = false,
                IncludeLearningOutcomes = true,
                IncludeInstructorRatings = false,
                HighlightDifferences = true
            };

            comparison = await _comparisonService.CompareCoursesAsync(request.CourseIds.ToArray(), criteria);

            var response = new CourseComparisonResponse
            {
                Comparison = comparison,
                RequestedCourseIds = request.CourseIds,
                ComparisonCriteria = request.Criteria
            };

            _logger.LogInformation("Course comparison completed for {Count} courses", request.CourseIds.Count);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error comparing courses");
            return StatusCode(500, new { error = "An error occurred while comparing courses" });
        }
    }

    /// <summary>
    /// Find courses similar to a given course
    /// </summary>
    [HttpGet("similar/{courseId}")]
    public async Task<ActionResult<SimilarCoursesResponse>> GetSimilarCourses(
        int courseId,
        [FromQuery] int maxResults = 5)
    {
        try
        {
            _logger.LogInformation("Finding courses similar to {CourseId}", courseId);

            var similarCourses = await _comparisonService.FindSimilarCoursesAsync(courseId, maxResults);

            var response = new SimilarCoursesResponse
            {
                ReferenceCourseId = courseId,
                SimilarCourses = similarCourses.Select(rec => rec.Course).ToList(),
                MaxResults = maxResults
            };

            _logger.LogInformation("Found {Count} similar courses to {CourseId}",
                similarCourses.Count, courseId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding similar courses to {CourseId}", courseId);
            return StatusCode(500, new { error = "An error occurred while finding similar courses" });
        }
    }

    #endregion

    #region Waitlist Endpoints

    /// <summary>
    /// Add student to course waitlist
    /// </summary>
    [HttpPost("waitlist")]
    public async Task<ActionResult<WaitlistResponse>> AddToWaitlist([FromBody] AddToWaitlistRequest request)
    {
        try
        {
            _logger.LogInformation("Adding student {StudentId} to waitlist for section {SectionId}",
                request.StudentId, request.SectionId);

            var waitlistRequest = new CourseWaitlistRequest
            {
                StudentId = request.StudentId,
                CourseOfferingId = request.SectionId, // Assuming SectionId maps to CourseOfferingId
                Priority = WaitlistPriority.Normal, // Could be determined by business rules
                AutoEnroll = request.AutoEnroll,
                NotificationPreferences = request.NotificationPreferences ?? new NotificationPreferences(),
                ExpirationDate = request.ExpirationDate
            };

            var waitlistEntry = await _waitlistService.AddToWaitlistAsync(waitlistRequest);

            var response = new WaitlistResponse
            {
                Success = waitlistEntry.Success,
                WaitlistEntry = waitlistEntry.Success ? new WaitlistEntry
                {
                    Id = 0, // This would be set by the service in a real implementation
                    StudentId = request.StudentId,
                    SectionId = request.SectionId,
                    Position = waitlistEntry.WaitlistPosition,
                    Status = waitlistEntry.Status,
                    RequestDate = waitlistEntry.CreatedAt,
                    NotificationPreferences = request.NotificationPreferences ?? new NotificationPreferences(),
                    AutoEnroll = request.AutoEnroll,
                    ExpirationDate = request.ExpirationDate
                } : null,
                Message = waitlistEntry.Message
            };

            _logger.LogInformation("Student {StudentId} added to waitlist for section {SectionId} at position {Position}",
                request.StudentId, request.SectionId, waitlistEntry.WaitlistPosition);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Invalid waitlist operation for student {StudentId}: {Message}",
                request.StudentId, ex.Message);
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding student {StudentId} to waitlist", request.StudentId);
            return StatusCode(500, new { error = "An error occurred while adding to waitlist" });
        }
    }

    /// <summary>
    /// Remove student from course waitlist
    /// </summary>
    [HttpDelete("waitlist/{studentId}/section/{sectionId}")]
    public async Task<ActionResult<WaitlistResponse>> RemoveFromWaitlist(int studentId, int sectionId)
    {
        try
        {
            _logger.LogInformation("Removing student {StudentId} from waitlist for section {SectionId}",
                studentId, sectionId);

            var success = await _waitlistService.RemoveFromWaitlistAsync(studentId, sectionId);

            var response = new WaitlistResponse
            {
                Success = success,
                Message = success ? "Successfully removed from waitlist" : "No active waitlist entry found"
            };

            if (success)
            {
                _logger.LogInformation("Student {StudentId} removed from waitlist for section {SectionId}",
                    studentId, sectionId);
            }
            else
            {
                _logger.LogWarning("No active waitlist entry found for student {StudentId} and section {SectionId}",
                    studentId, sectionId);
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing student {StudentId} from waitlist", studentId);
            return StatusCode(500, new { error = "An error occurred while removing from waitlist" });
        }
    }

    /// <summary>
    /// Get student's waitlist status
    /// </summary>
    [HttpGet("waitlist/{studentId}")]
    public async Task<ActionResult<StudentWaitlistResponse>> GetWaitlistStatus(int studentId)
    {
        try
        {
            _logger.LogInformation("Getting waitlist status for student {StudentId}", studentId);

            var waitlistEntries = await _waitlistService.GetWaitlistStatusAsync(studentId);

            var response = new StudentWaitlistResponse
            {
                StudentId = studentId,
                WaitlistEntries = waitlistEntries.Select(entry => new WaitlistEntry
                {
                    Id = entry.Id,
                    StudentId = entry.StudentId,
                    SectionId = entry.CourseOfferingId, // Map CourseOfferingId to SectionId
                    Position = entry.WaitlistPosition,
                    Status = entry.Status,
                    RequestDate = entry.CreatedAt,
                    Priority = entry.Priority,
                    NotificationPreferences = entry.NotificationPreferences ?? new NotificationPreferences()
                }).ToList(),
                TotalActiveWaitlists = waitlistEntries.Count
            };

            _logger.LogInformation("Retrieved {Count} waitlist entries for student {StudentId}",
                waitlistEntries.Count, studentId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting waitlist status for student {StudentId}", studentId);
            return StatusCode(500, new { error = "An error occurred while retrieving waitlist status" });
        }
    }

    /// <summary>
    /// Get waitlist summary for a section
    /// </summary>
    [HttpGet("waitlist/section/{sectionId}")]
    public async Task<ActionResult<WaitlistSummaryResponse>> GetWaitlistSummary(int sectionId)
    {
        try
        {
            _logger.LogInformation("Getting waitlist summary for section {SectionId}", sectionId);

            var summary = await _waitlistService.GetWaitlistSummaryAsync(sectionId);

            var response = new WaitlistSummaryResponse
            {
                Summary = summary
            };

            _logger.LogInformation("Retrieved waitlist summary for section {SectionId}: {Count} entries",
                sectionId, summary.WaitlistCount);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Section {SectionId} not found: {Message}", sectionId, ex.Message);
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting waitlist summary for section {SectionId}", sectionId);
            return StatusCode(500, new { error = "An error occurred while retrieving waitlist summary" });
        }
    }

    #endregion

    #region Private Helper Methods

    private string GenerateSearchCacheKey(CourseSearchRequest request)
    {
        var keyComponents = new List<string>
        {
            request.Query ?? "",
            request.PageSize.ToString(),
            request.CurrentPage.ToString()
        };

        if (request.Filters != null)
        {
            foreach (var filter in request.Filters.OrderBy(f => f.Key))
            {
                keyComponents.Add($"{filter.Key}:{string.Join(",", filter.Value)}");
            }
        }

        return $"search:{string.Join(":", keyComponents)}";
    }

    private CourseSearchCriteria MapToSearchCriteria(CourseSearchRequest request)
    {
        return new CourseSearchCriteria
        {
            Query = request.Query,
            Filters = request.Filters ?? new Dictionary<string, List<string>>(),
            PageSize = request.PageSize,
            CurrentPage = request.CurrentPage,
            SortBy = request.SortBy,
            SortDirection = request.SortDirection ?? "asc",
            IncludeInactive = request.IncludeInactive
        };
    }

    private CourseFilterCriteria MapToFilterCriteria(CourseFilterRequest request)
    {
        return new CourseFilterCriteria
        {
            SubjectCodes = request.SubjectCodes,
            CourseLevels = request.CourseLevels,
            CreditRanges = request.CreditRanges,
            Prerequisites = request.Prerequisites,
            Schedules = request.Schedules,
            Instructors = request.Instructors,
            AvailabilityOnly = request.AvailabilityOnly
        };
    }

    private async Task<StudentAcademicProfile?> GetStudentProfile(int studentId)
    {
        // This would typically fetch from a database or student service
        // For now, return a mock profile
        await Task.Delay(10); // Simulate async operation

        return new StudentAcademicProfile
        {
            StudentId = studentId,
            Major = "Computer Science",
            DegreeCode = "BS-CS",
            ClassStanding = ClassStanding.Junior,
            CurrentGPA = 3.45m,
            CompletedCourses = new List<int> { 1, 2, 3, 5, 8, 13, 21 },
            Interests = new[] { "Programming", "Data Science", "Machine Learning" },
            StrengthAreas = new[] { "Mathematics", "Programming", "Problem Solving" }
        };
    }

    private async Task<DegreeRequirements?> GetDegreeRequirements(string? degreeCode)
    {
        // This would typically fetch from a database
        await Task.Delay(10); // Simulate async operation

        if (degreeCode == "BS-CS")
        {
            return new DegreeRequirements
            {
                DegreeCode = degreeCode,
                RequiredCourses = new List<int> { 1, 2, 3, 4, 5, 8, 13, 21, 34, 55 },
                ElectiveCategories = new List<ElectiveCategory>
                {
                    new ElectiveCategory
                    {
                        Name = "Computer Science Electives",
                        RequiredCredits = 12,
                        SubjectCodes = new[] { "CS" },
                        ApprovedCourses = new List<int> { 89, 144, 233, 377 }
                    }
                }
            };
        }

        return null;
    }

    #endregion
}

#region Request/Response Models

public class CourseSearchRequest
{
    public string? Query { get; set; }
    public Dictionary<string, List<string>>? Filters { get; set; }
    public int PageSize { get; set; } = 20;
    public int CurrentPage { get; set; } = 1;
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; }
    public bool IncludeInactive { get; set; } = false;
}

public class CourseSearchResponse
{
    public List<CourseSearchResultItem> Results { get; set; } = new();
    public Dictionary<string, List<string>> Filters { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageSize { get; set; }
    public int CurrentPage { get; set; }
    public string? SearchQuery { get; set; }
    public Dictionary<string, object> SearchMetadata { get; set; } = new();
    public TimeSpan ProcessingTime { get; set; }
}

public class CourseFilterRequest
{
    public List<string>? SubjectCodes { get; set; }
    public List<string>? CourseLevels { get; set; }
    public List<string>? CreditRanges { get; set; }
    public List<string>? Prerequisites { get; set; }
    public List<string>? Schedules { get; set; }
    public List<string>? Instructors { get; set; }
    public bool AvailabilityOnly { get; set; } = false;
}

public class CourseFilterResponse
{
    public List<Course> Courses { get; set; } = new();
    public Dictionary<string, List<string>> AppliedFilters { get; set; } = new();
    public Dictionary<string, List<string>> AvailableFilters { get; set; } = new();
    public int TotalCount { get; set; }
    public Dictionary<string, object> FilterMetadata { get; set; } = new();
}

public class FuzzySearchRequest
{
    [Required]
    public string Query { get; set; } = string.Empty;
    public int? MaxResults { get; set; }
}

public class FuzzySearchResponse
{
    public string Query { get; set; } = string.Empty;
    public List<FuzzySearchResult> Results { get; set; } = new();
}

public class FuzzySearchResult
{
    public Course Course { get; set; } = new();
    public float SimilarityScore { get; set; }
    public List<string> MatchedFields { get; set; } = new();
}

public class CourseRecommendationResponse
{
    public int StudentId { get; set; }
    public List<CourseRecommendation> Recommendations { get; set; } = new();
    public DateTime GeneratedAt { get; set; }
    public Dictionary<string, object> ProfileMetadata { get; set; } = new();
}

public class ComparisonCriteria
{
    public bool IncludePrerequisites { get; set; }
    public bool IncludeWorkload { get; set; }
    public bool IncludeDifficulty { get; set; }
    public bool IncludeScheduleInfo { get; set; }
    public bool IncludeLearningOutcomes { get; set; }
    public bool IncludeInstructorRatings { get; set; }
    public bool HighlightDifferences { get; set; }
    public float DifferenceThreshold { get; set; } = 0.5f;
}

public class CourseComparisonRequest
{
    [Required]
    public List<int> CourseIds { get; set; } = new();
    public ComparisonCriteria? Criteria { get; set; }
}

public class CourseComparisonResponse
{
    public CourseComparison Comparison { get; set; } = new();
    public List<int> RequestedCourseIds { get; set; } = new();
    public ComparisonCriteria? ComparisonCriteria { get; set; }
}

public class SimilarCoursesResponse
{
    public int ReferenceCourseId { get; set; }
    public List<Course> SimilarCourses { get; set; } = new();
    public int MaxResults { get; set; }
}

public class AddToWaitlistRequest
{
    [Required]
    public int StudentId { get; set; }
    [Required]
    public int SectionId { get; set; }
    public bool AutoEnroll { get; set; } = true;
    public NotificationPreferences? NotificationPreferences { get; set; }
    public DateTime? ExpirationDate { get; set; }
}

public class WaitlistResponse
{
    public bool Success { get; set; }
    public WaitlistEntry? WaitlistEntry { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class StudentWaitlistResponse
{
    public int StudentId { get; set; }
    public List<WaitlistEntry> WaitlistEntries { get; set; } = new();
    public int TotalActiveWaitlists { get; set; }
}

public class WaitlistSummaryResponse
{
    public WaitlistSummary Summary { get; set; } = new();
}

#endregion