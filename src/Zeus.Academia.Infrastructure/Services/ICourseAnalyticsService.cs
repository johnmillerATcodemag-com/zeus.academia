using Zeus.Academia.Infrastructure.Models;

namespace Zeus.Academia.Infrastructure.Services;

/// <summary>
/// Interface for course analytics and reporting services.
/// </summary>
public interface ICourseAnalyticsService
{
    /// <summary>
    /// Generate course performance report.
    /// </summary>
    /// <param name="reportType">Type of report to generate</param>
    /// <param name="parameters">Report parameters</param>
    /// <returns>Course analytics report</returns>
    Task<CourseAnalyticsReport> GenerateCourseReportAsync(CourseReportType reportType, CourseAnalyticsParameters parameters);

    /// <summary>
    /// Get enrollment trends for courses.
    /// </summary>
    /// <param name="courseIds">Course IDs to analyze</param>
    /// <param name="timeRange">Time range for analysis</param>
    /// <returns>Enrollment trend data</returns>
    Task<EnrollmentTrends> GetEnrollmentTrendsAsync(List<int> courseIds, TimeRange timeRange);

    /// <summary>
    /// Get success rate analytics for courses.
    /// </summary>
    /// <param name="courseIds">Course IDs to analyze</param>
    /// <param name="timeRange">Time range for analysis</param>
    /// <returns>Success rate analytics</returns>
    Task<SuccessRateAnalytics> GetSuccessRateAnalyticsAsync(List<int> courseIds, TimeRange timeRange);

    /// <summary>
    /// Get prerequisite effectiveness analysis.
    /// </summary>
    /// <param name="courseId">Course ID to analyze</param>
    /// <returns>Prerequisite effectiveness data</returns>
    Task<PrerequisiteEffectiveness> AnalyzePrerequisiteEffectivenessAsync(int courseId);

    /// <summary>
    /// Get course capacity utilization report.
    /// </summary>
    /// <param name="semesterId">Semester ID</param>
    /// <param name="departmentId">Optional department filter</param>
    /// <returns>Capacity utilization report</returns>
    Task<CapacityUtilizationReport> GetCapacityUtilizationReportAsync(int semesterId, int? departmentId = null);

    /// <summary>
    /// Get student progression analytics for course sequences.
    /// </summary>
    /// <param name="subjectId">Subject ID</param>
    /// <param name="level">Course level</param>
    /// <returns>Progression analytics</returns>
    Task<ProgressionAnalytics> GetProgressionAnalyticsAsync(int subjectId, string level);

    /// <summary>
    /// Get waitlist analytics for courses.
    /// </summary>
    /// <param name="courseIds">Course IDs to analyze</param>
    /// <param name="timeRange">Time range for analysis</param>
    /// <returns>Waitlist analytics</returns>
    Task<WaitlistAnalytics> GetWaitlistAnalyticsAsync(List<int> courseIds, TimeRange timeRange);

    /// <summary>
    /// Generate actionable insights from course data.
    /// </summary>
    /// <param name="analysisType">Type of analysis to perform</param>
    /// <param name="courseIds">Courses to analyze</param>
    /// <returns>List of actionable insights</returns>
    Task<List<CourseInsight>> GenerateActionableInsightsAsync(AnalysisType analysisType, List<int> courseIds);

    /// <summary>
    /// Get predictive analytics for course demand.
    /// </summary>
    /// <param name="subjectId">Subject ID</param>
    /// <param name="futureSemesters">Number of future semesters to predict</param>
    /// <returns>Demand prediction data</returns>
    Task<DemandPrediction> PredictCourseDemandAsync(int subjectId, int futureSemesters = 4);

    /// <summary>
    /// Export analytics data in specified format.
    /// </summary>
    /// <param name="reportId">Report ID to export</param>
    /// <param name="format">Export format</param>
    /// <returns>Exported data as byte array</returns>
    Task<byte[]> ExportAnalyticsDataAsync(int reportId, ExportFormat format);
}

/// <summary>
/// Time range for analytics queries
/// </summary>
public class TimeRange
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public static TimeRange LastSemester() => new()
    {
        StartDate = DateTime.Now.AddMonths(-6),
        EndDate = DateTime.Now.AddMonths(-2)
    };

    public static TimeRange LastYear() => new()
    {
        StartDate = DateTime.Now.AddYears(-1),
        EndDate = DateTime.Now
    };

    public static TimeRange LastTwoYears() => new()
    {
        StartDate = DateTime.Now.AddYears(-2),
        EndDate = DateTime.Now
    };
}

/// <summary>
/// Analysis type for generating insights
/// </summary>
public enum AnalysisType
{
    EnrollmentOptimization,
    PrerequisiteEffectiveness,
    CapacityPlanning,
    StudentSuccess,
    CourseSequencing,
    ResourceAllocation
}

