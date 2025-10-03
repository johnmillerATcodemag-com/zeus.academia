using Zeus.Academia.Infrastructure.Enums;
using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.Infrastructure.Services;

/// <summary>
/// Service interface for advanced course search functionality
/// </summary>
public interface ICourseSearchService
{
    /// <summary>
    /// Performs advanced course search with multiple criteria
    /// </summary>
    Task<CourseSearchResult> SearchCoursesAsync(CourseSearchCriteria criteria);

    /// <summary>
    /// Filters courses based on specific criteria
    /// </summary>
    Task<CourseFilterResult> FilterCoursesAsync(CourseFilterCriteria criteria);

    /// <summary>
    /// Gets search suggestions based on partial input
    /// </summary>
    Task<List<string>> GetSearchSuggestionsAsync(string partialInput);

    /// <summary>
    /// Performs fuzzy search for misspelled terms
    /// </summary>
    Task<List<string>> GetFuzzySearchSuggestionsAsync(string searchTerm, float threshold = 0.8f);

    /// <summary>
    /// Performs fuzzy search for courses
    /// </summary>
    Task<List<FuzzySearchResultItem>> FuzzySearchAsync(string query, int maxResults = 20);
}

/// <summary>
/// Service interface for course recommendations
/// </summary>
public interface ICourseRecommendationService
{
    /// <summary>
    /// Recommends courses based on student academic profile
    /// </summary>
    Task<List<CourseRecommendation>> RecommendCoursesAsync(StudentAcademicProfile studentProfile);

    /// <summary>
    /// Recommends courses specifically for degree completion
    /// </summary>
    Task<List<CourseRecommendation>> RecommendCoursesForDegreeAsync(
        StudentAcademicProfile studentProfile,
        DegreeRequirements degreeRequirements);

    /// <summary>
    /// Gets personalized course recommendations based on interests and performance
    /// </summary>
    Task<List<CourseRecommendation>> GetPersonalizedRecommendationsAsync(
        StudentAcademicProfile studentProfile,
        int maxRecommendations = 10);
}

/// <summary>
/// Service interface for course comparison functionality
/// </summary>
public interface ICourseComparisonService
{
    /// <summary>
    /// Compares multiple courses side-by-side
    /// </summary>
    Task<CourseComparison> CompareCoursesAsync(int[] courseIds, CourseComparisonCriteria criteria);

    /// <summary>
    /// Compares courses with custom criteria
    /// </summary>
    Task<CourseComparison> CompareCoursesWithCriteriaAsync(List<int> courseIds, ComparisonCriteria criteria);

    /// <summary>
    /// Gets detailed comparison between two specific courses
    /// </summary>
    Task<CourseComparison> CompareCoursesDetailedAsync(int courseId1, int courseId2);

    /// <summary>
    /// Finds similar courses to a given course
    /// </summary>
    Task<List<CourseRecommendation>> FindSimilarCoursesAsync(int courseId, int maxResults = 5);
}

/// <summary>
/// Service interface for course waitlist management
/// </summary>
public interface ICourseWaitlistService
{
    /// <summary>
    /// Adds student to course waitlist
    /// </summary>
    Task<CourseWaitlistResult> AddToWaitlistAsync(CourseWaitlistRequest request);

    /// <summary>
    /// Removes student from course waitlist
    /// </summary>
    Task<bool> RemoveFromWaitlistAsync(int studentId, int courseOfferingId);

    /// <summary>
    /// Processes waitlist when seats become available
    /// </summary>
    Task<List<WaitlistProcessingResult>> ProcessWaitlistAsync(int courseOfferingId, int availableSeats);

    /// <summary>
    /// Sends waitlist notification to student
    /// </summary>
    Task<NotificationResult> SendWaitlistNotificationAsync(CourseWaitlistEntry entry, NotificationType notificationType);

    /// <summary>
    /// Gets student's waitlist status for all courses
    /// </summary>
    Task<List<CourseWaitlistEntry>> GetStudentWaitlistStatusAsync(int studentId);

    /// <summary>
    /// Gets waitlist statistics for a course offering
    /// </summary>
    Task<WaitlistStatistics> GetWaitlistStatisticsAsync(int courseOfferingId);

    /// <summary>
    /// Gets waitlist status for a student
    /// </summary>
    Task<List<CourseWaitlistEntry>> GetWaitlistStatusAsync(int studentId);

    /// <summary>
    /// Gets waitlist summary for a section
    /// </summary>
    Task<WaitlistSummary> GetWaitlistSummaryAsync(int sectionId);
}

/// <summary>
/// Waitlist statistics for course offerings
/// </summary>
public class WaitlistStatistics
{
    public int TotalWaitlisted { get; set; }
    public int AverageWaitTime { get; set; }
    public int EstimatedSeatsAvailable { get; set; }
    public Dictionary<WaitlistPriority, int> PriorityBreakdown { get; set; } = new();
    public float HistoricalEnrollmentRate { get; set; }
}