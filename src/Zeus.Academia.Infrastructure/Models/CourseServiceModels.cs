using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Enums;

namespace Zeus.Academia.Infrastructure.Models;

#region Course Service Models

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
/// Course search criteria
/// </summary>
public class CourseSearchCriteria
{
    public string? Keyword { get; set; }
    public string? SubjectCode { get; set; }
    public CourseLevel? Level { get; set; }
    public decimal? MinCreditHours { get; set; }
    public decimal? MaxCreditHours { get; set; }
    public List<string>? DeliveryMethods { get; set; }
    public bool? HasPrerequisites { get; set; }
    public CourseStatus? Status { get; set; }
    public int? MaxResults { get; set; } = 50;
}

/// <summary>
/// Course validation result
/// </summary>
public class CourseValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}

#endregion

#region Analytics Models

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
/// Department analytics summary
/// </summary>
public class DepartmentAnalytics
{
    public string SubjectCode { get; set; } = string.Empty;
    public int TotalCourses { get; set; }
    public int TotalEnrollments { get; set; }
    public decimal AverageSuccessRate { get; set; }
    public decimal AverageCapacityUtilization { get; set; }
    public List<CoursePerformanceSummary> CoursePerformances { get; set; } = new();
    public List<string> Insights { get; set; } = new();
}

/// <summary>
/// Course performance summary
/// </summary>
public class CoursePerformanceSummary
{
    public int CourseId { get; set; }
    public string CourseNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public decimal SuccessRate { get; set; }
    public decimal CapacityUtilization { get; set; }
    public int TotalEnrollments { get; set; }
    public string PerformanceCategory { get; set; } = string.Empty; // High, Medium, Low
}

/// <summary>
/// Course comparison analytics
/// </summary>
public class CourseComparisonAnalytics
{
    public List<int> CourseIds { get; set; } = new();
    public List<CoursePerformanceSummary> CoursePerformances { get; set; } = new();
    public ComparisonInsights Insights { get; set; } = new();
    public DateTime GeneratedDate { get; set; }
}

/// <summary>
/// Comparison insights
/// </summary>
public class ComparisonInsights
{
    public CoursePerformanceSummary BestPerforming { get; set; } = new();
    public CoursePerformanceSummary LowestPerforming { get; set; } = new();
    public List<string> KeyDifferences { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
}

/// <summary>
/// Course performance report
/// </summary>
public class CoursePerformanceReport
{
    public int CourseId { get; set; }
    public CourseReportType ReportType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty; // HTML or formatted content
    public Dictionary<string, object> Data { get; set; } = new(); // Raw data for charts/tables
    public DateTime GeneratedDate { get; set; }
    public string GeneratedBy { get; set; } = string.Empty;
}

#endregion

#region Enums

/// <summary>
/// Course equivalency types
/// </summary>
public enum EquivalencyType
{
    Direct,
    Partial,
    Conditional,
    NoEquivalent
}

/// <summary>
/// Transfer credit status
/// </summary>
public enum TransferStatus
{
    Approved,
    ConditionalApproval,
    PendingReview,
    Rejected
}

/// <summary>
/// Enrollment request status
/// </summary>
public enum EnrollmentStatus
{
    Approved,
    Waitlisted,
    Rejected,
    PrerequisitesNotMet
}

/// <summary>
/// Course report types
/// </summary>
public enum CourseReportType
{
    EnrollmentTrends,
    SuccessRates,
    CapacityUtilization,
    StudentDemographics,
    GradeDistribution,
    ComprehensiveSummary
}

#endregion

#region Missing Analytics Models

/// <summary>
/// Enrollment trends analytics data
/// </summary>
public class EnrollmentTrends
{
    public List<int> CourseIds { get; set; } = new();
    public Dictionary<string, List<int>> TrendData { get; set; } = new();
    public decimal GrowthRate { get; set; }
    public string TrendDirection { get; set; } = string.Empty;
    public List<string> SeasonalPatterns { get; set; } = new();
    public DateTime AnalysisDate { get; set; }
}

/// <summary>
/// Success rate analytics data
/// </summary>
public class SuccessRateAnalytics
{
    public List<int> CourseIds { get; set; } = new();
    public Dictionary<int, decimal> SuccessRates { get; set; } = new();
    public decimal AverageSuccessRate { get; set; }
    public Dictionary<string, decimal> SuccessRateByDemographic { get; set; } = new();
    public List<string> SuccessFactors { get; set; } = new();
    public List<string> RiskFactors { get; set; } = new();
}

/// <summary>
/// Prerequisite effectiveness analysis
/// </summary>
public class PrerequisiteEffectiveness
{
    public int CourseId { get; set; }
    public List<int> PrerequisiteCourseIds { get; set; } = new();
    public Dictionary<int, decimal> EffectivenessScores { get; set; } = new();
    public decimal OverallEffectiveness { get; set; }
    public List<string> Recommendations { get; set; } = new();
    public DateTime AnalysisDate { get; set; }
}

/// <summary>
/// Capacity utilization report
/// </summary>
public class CapacityUtilizationReport
{
    public int SemesterId { get; set; }
    public int? DepartmentId { get; set; }
    public decimal OverallUtilization { get; set; }
    public Dictionary<int, decimal> CourseUtilization { get; set; } = new();
    public List<int> OverenrolledCourses { get; set; } = new();
    public List<int> UnderenrolledCourses { get; set; } = new();
    public List<string> OptimizationRecommendations { get; set; } = new();
}

/// <summary>
/// Progression analytics for course sequences
/// </summary>
public class ProgressionAnalytics
{
    public int SubjectId { get; set; }
    public string Level { get; set; } = string.Empty;
    public decimal CompletionRate { get; set; }
    public decimal AverageTimeToComplete { get; set; }
    public Dictionary<string, decimal> BottleneckAnalysis { get; set; } = new();
    public List<string> CommonDropoutPoints { get; set; } = new();
    public List<string> ImprovementSuggestions { get; set; } = new();
}

/// <summary>
/// Waitlist analytics data
/// </summary>
public class WaitlistAnalytics
{
    public List<int> CourseIds { get; set; } = new();
    public Dictionary<int, int> AverageWaitlistSize { get; set; } = new();
    public Dictionary<int, decimal> ConversionRates { get; set; } = new();
    public Dictionary<int, int> AverageWaitTime { get; set; } = new();
    public List<string> WaitlistPatterns { get; set; } = new();
    public List<string> ReductionStrategies { get; set; } = new();
}

/// <summary>
/// Course insight for actionable recommendations
/// </summary>
public class CourseInsight
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Priority { get; set; }
    public List<int> AffectedCourses { get; set; } = new();
    public string RecommendedAction { get; set; } = string.Empty;
    public decimal PotentialImpact { get; set; }
    public DateTime GeneratedDate { get; set; }
}

/// <summary>
/// Demand prediction for courses
/// </summary>
public class DemandPrediction
{
    public int SubjectId { get; set; }
    public int PredictionHorizon { get; set; }
    public Dictionary<string, int> PredictedEnrollment { get; set; } = new();
    public decimal ConfidenceLevel { get; set; }
    public List<string> InfluencingFactors { get; set; } = new();
    public Dictionary<string, string> SeasonalAdjustments { get; set; } = new();
    public DateTime PredictionDate { get; set; }
}

/// <summary>
/// Course analytics report
/// </summary>
public class CourseAnalyticsReport
{
    public int Id { get; set; }
    public CourseReportType ReportType { get; set; }
    public DateTime GeneratedDate { get; set; }
    public CourseAnalyticsParameters Parameters { get; set; } = new();
    public string Title { get; set; } = string.Empty;
    public CourseAnalyticsSummary Summary { get; set; } = new();
    public string Data { get; set; } = string.Empty;
}

/// <summary>
/// Course analytics parameters
/// </summary>
public class CourseAnalyticsParameters
{
    public List<int> CourseIds { get; set; } = new();
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int? SemesterId { get; set; }
    public int? DepartmentId { get; set; }
}

/// <summary>
/// Course analytics summary
/// </summary>
public class CourseAnalyticsSummary
{
    public Dictionary<string, object> KeyMetrics { get; set; } = new();
    public List<string> Highlights { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
}

/// <summary>
/// Course completion record
/// </summary>
public class CourseCompletion
{
    public int CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public decimal Credits { get; set; }
    public decimal Grade { get; set; }
    public DateTime CompletionDate { get; set; }
}



#endregion