using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Enums;

namespace Zeus.Academia.Infrastructure.Services;

public enum SearchField
{
    CourseNumber,
    Title,
    Description,
    SubjectCode,
    All
}

public enum WaitlistStatus
{
    Active,
    Offered,
    Enrolled,
    Expired,
    Cancelled,
    Removed
}

public enum NotificationTiming
{
    Immediate,
    Daily,
    Weekly,
    Custom
}

public enum PrerequisiteValidationStatus
{
    Valid,
    Invalid,
    Warning,
    Pending
}

#region Student and Academic Profile Models

public class StudentAcademicProfile
{
    public int StudentId { get; set; }
    public string? Major { get; set; }
    public string? DegreeCode { get; set; }
    public ClassStanding ClassStanding { get; set; }
    public decimal CurrentGPA { get; set; }
    public ICollection<int>? CompletedCourses { get; set; }
    public string[]? Interests { get; set; }
    public string[]? StrengthAreas { get; set; }
}

public class ComparisonCriteria
{
    public float WeightCredits { get; set; } = 0.2f;
    public float WeightDifficulty { get; set; } = 0.2f;
    public float WeightSchedule { get; set; } = 0.2f;
    public float WeightSubject { get; set; } = 0.2f;
    public float WeightPrerequisites { get; set; } = 0.2f;
}

#endregion

/// <summary>
/// Result classes for Course Search and Discovery functionality
/// </summary>
public class CourseSearchResult
{
    public List<Course> Courses { get; set; } = new();
    public List<Course> Results { get; set; } = new();
    public Dictionary<string, List<string>> Filters { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public long SearchDurationMs { get; set; }
    public Dictionary<string, object> SearchMetadata { get; set; } = new();
    public long ProcessingTime { get; set; }
    public Dictionary<string, int> FilterSummary { get; set; } = new();
    public List<string>? SearchSuggestions { get; set; }
    public List<FuzzyMatch>? FuzzyMatches { get; set; }
}

/// <summary>
/// Individual course search result item for API responses
/// </summary>
public class CourseSearchResultItem
{
    public int Id { get; set; }
    public string CourseNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string SubjectCode { get; set; } = string.Empty;
    public decimal CreditHours { get; set; }
    public Enums.CourseLevel Level { get; set; }
    public CourseStatus Status { get; set; }
    public float? RelevanceScore { get; set; }
    public List<string> MatchedFields { get; set; } = new();
}

public class CourseFilterResult
{
    public List<Course> Courses { get; set; } = new();
    public int TotalCount { get; set; }
    public Dictionary<string, List<string>> AppliedFilters { get; set; } = new();
    public Dictionary<string, List<string>> AvailableFilters { get; set; } = new();
    public List<CourseEligibilityResult>? EligibilityResults { get; set; }
    public Dictionary<string, object> FilterMetadata { get; set; } = new();
}

public class CourseRecommendation
{
    public Course Course { get; set; } = null!;
    public float RecommendationScore { get; set; }
    public List<string> RecommendationReasons { get; set; } = new();
    public string RecommendationType { get; set; } = string.Empty;
    public Dictionary<string, object> RecommendationMetadata { get; set; } = new();
}

public class CourseComparison
{
    public List<CourseComparisonDetails> Courses { get; set; } = new();
    public Dictionary<string, Dictionary<int, object>> ComparisonMatrix { get; set; } = new();
    public Dictionary<string, float> SimilarityScores { get; set; } = new();
    public List<KeyDifference> KeyDifferences { get; set; } = new();
}

public class CourseWaitlistResult
{
    public bool Success { get; set; }
    public int WaitlistPosition { get; set; }
    public TimeSpan? EstimatedWaitTime { get; set; }
    public WaitlistStatus Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? NotificationSentAt { get; set; }
}

public class WaitlistProcessingResult
{
    public int StudentId { get; set; }
    public bool? NotificationSent { get; set; }
}

public class CourseEligibilityResult
{
    public int CourseId { get; set; }
    public bool IsEligible { get; set; }
    public PrerequisiteValidationStatus PrerequisiteStatus { get; set; }
    public List<string> MissingRequirements { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}

public class CourseComparisonDetails
{
    public Course Course { get; set; } = null!;
    public PrerequisiteInfo? PrerequisiteInfo { get; set; }
    public WorkloadEstimate? WorkloadEstimate { get; set; }
    public DifficultyRating? DifficultyRating { get; set; }
    public ScheduleInfo? ScheduleInfo { get; set; }
    public List<LearningOutcome>? LearningOutcomes { get; set; }
    public InstructorRating? InstructorRating { get; set; }
}

public class KeyDifference
{
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public float SignificanceScore { get; set; }
    public Dictionary<int, object> CourseValues { get; set; } = new();
}

public class PrerequisiteInfo
{
    public List<Course> RequiredCourses { get; set; } = new();
    public List<Course> RecommendedCourses { get; set; } = new();
    public string? ComplexityLevel { get; set; }
    public bool HasCorequisites { get; set; }
}

public class WorkloadEstimate
{
    public int WeeklyHours { get; set; }
    public string WorkloadLevel { get; set; } = string.Empty;
    public Dictionary<string, int> ActivityBreakdown { get; set; } = new();
}

public class DifficultyRating
{
    public float OverallDifficulty { get; set; }
    public Dictionary<string, float> DifficultyAspects { get; set; } = new();
    public string DifficultyDescription { get; set; } = string.Empty;
}

public class ScheduleInfo
{
    public List<CourseOffering> Offerings { get; set; } = new();
    public bool HasFlexibleScheduling { get; set; }
    public List<string> DeliveryMethods { get; set; } = new();
}

public class InstructorRating
{
    public string InstructorName { get; set; } = string.Empty;
    public float OverallRating { get; set; }
    public Dictionary<string, float> RatingCategories { get; set; } = new();
    public int ReviewCount { get; set; }
}

public class LearningOutcome
{
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int BloomLevel { get; set; }
}

#region Additional Search and Filter Models

public class CourseSearchCriteria
{
    public string? Query { get; set; }
    public Dictionary<string, List<string>> Filters { get; set; } = new();
    public List<string>? SubjectCodes { get; set; }
    public decimal? MinCredits { get; set; }
    public decimal? MaxCredits { get; set; }
    public CourseLevel? Level { get; set; }
    public List<string>? Keywords { get; set; }
    public List<SearchField>? SearchFields { get; set; }
    public string? InstructorName { get; set; }
    public string? Semester { get; set; }
    public int? AcademicYear { get; set; }
    public bool? HasPrerequisites { get; set; }
    public bool? AvailableSeats { get; set; }
    public int PageNumber { get; set; } = 1;
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; }
    public bool IncludeInactive { get; set; } = false;
    public bool EnableFuzzySearch { get; set; } = false;
    public float FuzzySearchThreshold { get; set; } = 0.8f;
}

public class CourseFilterCriteria
{
    public SubjectFilter? SubjectHierarchy { get; set; }
    public LevelFilter? AcademicLevel { get; set; }
    public CreditFilter? CreditHours { get; set; }
    public PrerequisiteFilter? PrerequisiteFilter { get; set; }
    public ScheduleFilter? Schedule { get; set; }
    public List<string>? SubjectCodes { get; set; }
    public List<string>? CourseLevels { get; set; }
    public List<string>? CreditRanges { get; set; }
    public List<string>? Prerequisites { get; set; }
    public List<string>? Schedules { get; set; }
    public List<string>? Instructors { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public bool AvailabilityOnly { get; set; } = false;
}

public class SubjectFilter
{
    public string? DepartmentName { get; set; }
    public List<string>? SubjectCodes { get; set; }
    public bool IncludeRelatedSubjects { get; set; } = false;
}

public class LevelFilter
{
    public CourseLevel? MinLevel { get; set; }
    public CourseLevel? MaxLevel { get; set; }
}

public class CreditFilter
{
    public string Range { get; set; } = string.Empty;
    public decimal MinCredits { get; set; }
    public decimal MaxCredits { get; set; }
}

public class PrerequisiteFilter
{
    public bool CheckEligibility { get; set; } = false;
    public int? StudentId { get; set; }
    public List<string>? CompletedCourses { get; set; }
}

public class ScheduleFilter
{
    public TimeSpan? StartTimeAfter { get; set; }
    public TimeSpan? EndTimeBefore { get; set; }
    public List<DayOfWeek>? DaysOfWeek { get; set; }
}

public class WaitlistRequest
{
    public int StudentId { get; set; }
    public int SectionId { get; set; }
    public WaitlistPriority Priority { get; set; } = WaitlistPriority.Normal;
    public bool AutoEnroll { get; set; } = true;
    public NotificationPreferences? NotificationPreferences { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public ClassStanding StudentClassStanding { get; set; }
    public bool IsRequiredForDegree { get; set; }
    public bool IsGraduatingStudent { get; set; }
}

public class WaitlistEntry
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int SectionId { get; set; }
    public CourseSection Section { get; set; } = null!;
    public WaitlistPriority Priority { get; set; }
    public int Position { get; set; }
    public WaitlistStatus Status { get; set; }
    public DateTime RequestDate { get; set; }
    public NotificationPreferences NotificationPreferences { get; set; } = new();
    public bool AutoEnroll { get; set; } = true;
    public DateTime? ExpirationDate { get; set; }
    public DateTime? LastNotificationDate { get; set; }
    public DateTime? RemovalDate { get; set; }
    public string? RemovalReason { get; set; }
    public DateTime? EnrollmentDate { get; set; }
    public DateTime? OfferDate { get; set; }
    public DateTime? OfferExpirationDate { get; set; }
}

public class WaitlistSummary
{
    public int SectionId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public string SectionCode { get; set; } = string.Empty;
    public int MaxCapacity { get; set; }
    public int CurrentEnrollment { get; set; }
    public int AvailableSeats { get; set; }
    public int WaitlistCount { get; set; }
    public List<WaitlistEntry> WaitlistEntries { get; set; } = new();
    public TimeSpan EstimatedProcessingTime { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class CourseSection
{
    public int Id { get; set; }
    public string SectionCode { get; set; } = string.Empty;
    public int MaxCapacity { get; set; }
    public CourseOffering CourseOffering { get; set; } = null!;
}

public class FuzzyMatch
{
    public string Term { get; set; } = string.Empty;
    public float Confidence { get; set; }
    public string Suggestion { get; set; } = string.Empty;
    public string OriginalTerm { get; set; } = string.Empty;
    public string SuggestedTerm { get; set; } = string.Empty;
    public float MatchScore { get; set; }
}

public class FuzzySearchResultItem
{
    public Course Course { get; set; } = null!;
    public float SimilarityScore { get; set; }
    public List<string> MatchedFields { get; set; } = new();
}

public class NotificationPreferences
{
    public bool EmailNotifications { get; set; } = true;
    public bool SMSNotifications { get; set; } = false;
    public bool PushNotifications { get; set; } = true;
    public bool EmailNotification { get; set; }
    public bool SmsNotification { get; set; }
    public bool InAppNotification { get; set; }
    public NotificationTiming NotificationTiming { get; set; }
    public string? EmailAddress { get; set; }
    public string? PhoneNumber { get; set; }
    public TimeSpan? QuietHoursStart { get; set; }
    public TimeSpan? QuietHoursEnd { get; set; }
}

// Missing model classes for interface compatibility
public class CourseWaitlistRequest
{
    public int StudentId { get; set; }
    public int CourseOfferingId { get; set; }
    public WaitlistPriority Priority { get; set; } = WaitlistPriority.Normal;
    public NotificationPreferences NotificationPreferences { get; set; } = new();
    public bool AutoEnroll { get; set; } = true;
    public DateTime? ExpirationDate { get; set; }
}

public class CourseWaitlistEntry
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public int CourseOfferingId { get; set; }
    public int WaitlistPosition { get; set; }
    public WaitlistPriority Priority { get; set; }
    public WaitlistStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public NotificationPreferences? NotificationPreferences { get; set; }
}

public class CourseComparisonCriteria
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

#endregion

// DegreeRequirements for Course Search Services
public class DegreeRequirements
{
    public string DegreeCode { get; set; } = string.Empty;
    public List<int> RequiredCourses { get; set; } = new();
    public List<ElectiveCategory> ElectiveCategories { get; set; } = new();
    public int TotalCreditsRequired { get; set; }
    public Dictionary<string, object> AdditionalRequirements { get; set; } = new();
}

public class ElectiveCategory
{
    public string Name { get; set; } = string.Empty;
    public int RequiredCredits { get; set; }
    public string[] SubjectCodes { get; set; } = Array.Empty<string>();
    public List<int> ApprovedCourses { get; set; } = new();
    public string? Description { get; set; }
}

public class NotificationResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public NotificationType NotificationType { get; set; }
}

public enum NotificationType
{
    Email,
    SMS,
    InApp,
    Push
}