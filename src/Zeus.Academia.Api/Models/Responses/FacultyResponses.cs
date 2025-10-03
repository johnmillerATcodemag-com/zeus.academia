using Zeus.Academia.Infrastructure.Services.Interfaces;

namespace Zeus.Academia.Api.Models.Responses;

/// <summary>
/// Response model for faculty details.
/// </summary>
public class FacultyDetailsResponse
{
    /// <summary>
    /// The faculty member's employee number (primary key).
    /// </summary>
    public int EmpNr { get; set; }

    /// <summary>
    /// The faculty member's full name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The faculty member's phone number.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// The faculty member's salary.
    /// </summary>
    public decimal? Salary { get; set; }

    /// <summary>
    /// The type of faculty member (Professor, TeachingProf).
    /// </summary>
    public FacultyType FacultyType { get; set; }

    /// <summary>
    /// The academic rank code of the faculty member.
    /// </summary>
    public string? RankCode { get; set; }

    /// <summary>
    /// The rank name/description.
    /// </summary>
    public string? RankName { get; set; }

    /// <summary>
    /// The department name where the faculty member works.
    /// </summary>
    public string? DepartmentName { get; set; }

    /// <summary>
    /// The department ID where the faculty member works.
    /// </summary>
    public int? DepartmentId { get; set; }

    /// <summary>
    /// Whether the faculty member has tenure.
    /// </summary>
    public bool? HasTenure { get; set; }

    /// <summary>
    /// The faculty member's research area or specialty.
    /// </summary>
    public string? ResearchArea { get; set; }

    /// <summary>
    /// The faculty member's profile information.
    /// </summary>
    public FacultyProfileResponse? Profile { get; set; }

    /// <summary>
    /// The faculty member's tenure track information.
    /// </summary>
    public TenureTrackResponse? TenureTrack { get; set; }

    /// <summary>
    /// The faculty member's research expertise areas.
    /// </summary>
    public List<ResearchExpertiseResponse> ResearchExpertise { get; set; } = new();

    /// <summary>
    /// The faculty member's recent publications.
    /// </summary>
    public List<PublicationResponse> RecentPublications { get; set; } = new();

    /// <summary>
    /// The faculty member's office assignments.
    /// </summary>
    public List<OfficeAssignmentResponse> OfficeAssignments { get; set; } = new();

    /// <summary>
    /// The faculty member's committee assignments.
    /// </summary>
    public List<CommitteeAssignmentResponse> CommitteeAssignments { get; set; } = new();

    /// <summary>
    /// The faculty member's administrative assignments.
    /// </summary>
    public List<AdministrativeAssignmentResponse> AdministrativeAssignments { get; set; } = new();

    /// <summary>
    /// When the faculty record was created.
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// When the faculty record was last modified.
    /// </summary>
    public DateTime ModifiedDate { get; set; }

    /// <summary>
    /// Who created the faculty record.
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Who last modified the faculty record.
    /// </summary>
    public string ModifiedBy { get; set; } = string.Empty;
}

/// <summary>
/// Summary response model for faculty lists.
/// </summary>
public class FacultySummaryResponse
{
    /// <summary>
    /// The faculty member's employee number (primary key).
    /// </summary>
    public int EmpNr { get; set; }

    /// <summary>
    /// The faculty member's full name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The type of faculty member (Professor, TeachingProf).
    /// </summary>
    public FacultyType FacultyType { get; set; }

    /// <summary>
    /// The academic rank code of the faculty member.
    /// </summary>
    public string? RankCode { get; set; }

    /// <summary>
    /// The rank name/description.
    /// </summary>
    public string? RankName { get; set; }

    /// <summary>
    /// The department name where the faculty member works.
    /// </summary>
    public string? DepartmentName { get; set; }

    /// <summary>
    /// Whether the faculty member has tenure.
    /// </summary>
    public bool? HasTenure { get; set; }

    /// <summary>
    /// The faculty member's research area or specialty.
    /// </summary>
    public string? ResearchArea { get; set; }

    /// <summary>
    /// The faculty member's professional email (if public).
    /// </summary>
    public string? ProfessionalEmail { get; set; }
}

/// <summary>
/// Response model for faculty profile information.
/// </summary>
public class FacultyProfileResponse
{
    /// <summary>
    /// The profile ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The faculty member's professional title (Dr., Prof., etc.).
    /// </summary>
    public string? ProfessionalTitle { get; set; }

    /// <summary>
    /// The faculty member's preferred name for display.
    /// </summary>
    public string? PreferredName { get; set; }

    /// <summary>
    /// The faculty member's professional email address.
    /// </summary>
    public string? ProfessionalEmail { get; set; }

    /// <summary>
    /// The faculty member's professional website URL.
    /// </summary>
    public string? ProfessionalWebsite { get; set; }

    /// <summary>
    /// The faculty member's biography.
    /// </summary>
    public string? Biography { get; set; }

    /// <summary>
    /// The faculty member's research interests.
    /// </summary>
    public string? ResearchInterests { get; set; }

    /// <summary>
    /// The faculty member's teaching philosophy.
    /// </summary>
    public string? TeachingPhilosophy { get; set; }

    /// <summary>
    /// The faculty member's awards and honors.
    /// </summary>
    public string? Awards { get; set; }

    /// <summary>
    /// The faculty member's professional memberships.
    /// </summary>
    public string? ProfessionalMemberships { get; set; }

    /// <summary>
    /// The faculty member's current research projects.
    /// </summary>
    public string? CurrentResearchProjects { get; set; }

    /// <summary>
    /// The faculty member's consultation availability.
    /// </summary>
    public string? ConsultationAvailability { get; set; }

    /// <summary>
    /// The faculty member's office hours.
    /// </summary>
    public string? OfficeHours { get; set; }

    /// <summary>
    /// Whether the profile is public or private.
    /// </summary>
    public bool IsPublicProfile { get; set; }
}

/// <summary>
/// Response model for tenure track information.
/// </summary>
public class TenureTrackResponse
{
    /// <summary>
    /// The tenure track ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The start date of the tenure track.
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// The expected review date for tenure.
    /// </summary>
    public DateTime ExpectedReviewDate { get; set; }

    /// <summary>
    /// Years on track.
    /// </summary>
    public int YearsOnTrack { get; set; }

    /// <summary>
    /// Remaining years to tenure review.
    /// </summary>
    public int RemainingYears { get; set; }

    /// <summary>
    /// Progress percentage toward tenure review.
    /// </summary>
    public decimal ProgressPercentage { get; set; }

    /// <summary>
    /// Whether approaching tenure review.
    /// </summary>
    public bool IsApproachingTenureReview { get; set; }

    /// <summary>
    /// Whether the track has expired.
    /// </summary>
    public bool IsExpired { get; set; }

    /// <summary>
    /// Whether the faculty member is in good standing.
    /// </summary>
    public bool IsInGoodStanding { get; set; }

    /// <summary>
    /// Current status of the tenure track.
    /// </summary>
    public string? CurrentStatus { get; set; }

    /// <summary>
    /// Notes about the tenure track.
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// Response model for research expertise.
/// </summary>
public class ResearchExpertiseResponse
{
    /// <summary>
    /// The expertise ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The research area ID.
    /// </summary>
    public int ResearchAreaId { get; set; }

    /// <summary>
    /// The research area name.
    /// </summary>
    public string ResearchAreaName { get; set; } = string.Empty;

    /// <summary>
    /// The level of expertise.
    /// </summary>
    public string ExpertiseLevel { get; set; } = string.Empty;

    /// <summary>
    /// Years of experience in this research area.
    /// </summary>
    public int? YearsOfExperience { get; set; }
}

/// <summary>
/// Response model for faculty publications.
/// </summary>
public class PublicationResponse
{
    /// <summary>
    /// The publication ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The publication title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The publication type (Journal, Conference, Book, etc.).
    /// </summary>
    public string? PublicationType { get; set; }

    /// <summary>
    /// The journal or venue name.
    /// </summary>
    public string? JournalName { get; set; }

    /// <summary>
    /// The publication year.
    /// </summary>
    public int? PublicationYear { get; set; }

    /// <summary>
    /// The publication URL or DOI.
    /// </summary>
    public string? PublicationUrl { get; set; }

    /// <summary>
    /// Whether the publication is peer-reviewed.
    /// </summary>
    public bool? IsPeerReviewed { get; set; }
}

/// <summary>
/// Response model for office assignments.
/// </summary>
public class OfficeAssignmentResponse
{
    /// <summary>
    /// The assignment ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The building name.
    /// </summary>
    public string? BuildingName { get; set; }

    /// <summary>
    /// The room number.
    /// </summary>
    public string? RoomNumber { get; set; }

    /// <summary>
    /// The full office location (building + room).
    /// </summary>
    public string? OfficeLocation { get; set; }

    /// <summary>
    /// The assignment start date.
    /// </summary>
    public DateTime? AssignmentStartDate { get; set; }

    /// <summary>
    /// The assignment end date.
    /// </summary>
    public DateTime? AssignmentEndDate { get; set; }

    /// <summary>
    /// The assignment status.
    /// </summary>
    public string? AssignmentStatus { get; set; }
}

/// <summary>
/// Response model for committee assignments.
/// </summary>
public class CommitteeAssignmentResponse
{
    /// <summary>
    /// The assignment ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The committee name.
    /// </summary>
    public string? CommitteeName { get; set; }

    /// <summary>
    /// The role on the committee.
    /// </summary>
    public string? CommitteeRole { get; set; }

    /// <summary>
    /// The assignment start date.
    /// </summary>
    public DateTime? AssignmentStartDate { get; set; }

    /// <summary>
    /// The assignment end date.
    /// </summary>
    public DateTime? AssignmentEndDate { get; set; }

    /// <summary>
    /// The assignment status.
    /// </summary>
    public string? AssignmentStatus { get; set; }
}

/// <summary>
/// Response model for administrative assignments.
/// </summary>
public class AdministrativeAssignmentResponse
{
    /// <summary>
    /// The assignment ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The administrative role title.
    /// </summary>
    public string? RoleTitle { get; set; }

    /// <summary>
    /// The department or unit.
    /// </summary>
    public string? Department { get; set; }

    /// <summary>
    /// The assignment start date.
    /// </summary>
    public DateTime? AssignmentStartDate { get; set; }

    /// <summary>
    /// The assignment end date.
    /// </summary>
    public DateTime? AssignmentEndDate { get; set; }

    /// <summary>
    /// The assignment status.
    /// </summary>
    public string? AssignmentStatus { get; set; }
}

/// <summary>
/// Response model for promotion history.
/// </summary>
public class PromotionHistoryResponse
{
    /// <summary>
    /// The promotion ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The previous rank code.
    /// </summary>
    public string? FromRankCode { get; set; }

    /// <summary>
    /// The previous rank name.
    /// </summary>
    public string? FromRankName { get; set; }

    /// <summary>
    /// The new rank code.
    /// </summary>
    public string? ToRankCode { get; set; }

    /// <summary>
    /// The new rank name.
    /// </summary>
    public string? ToRankName { get; set; }

    /// <summary>
    /// The promotion date.
    /// </summary>
    public DateTime? PromotionDate { get; set; }

    /// <summary>
    /// Notes about the promotion.
    /// </summary>
    public string? PromotionNotes { get; set; }
}

/// <summary>
/// Response model for faculty statistics.
/// </summary>
public class FacultyStatisticsResponse
{
    /// <summary>
    /// Total number of faculty.
    /// </summary>
    public int TotalFaculty { get; set; }

    /// <summary>
    /// Number of professors.
    /// </summary>
    public int TotalProfessors { get; set; }

    /// <summary>
    /// Number of teaching professors.
    /// </summary>
    public int TotalTeachingProfs { get; set; }

    /// <summary>
    /// Number of teachers.
    /// </summary>
    public int TotalTeachers { get; set; }

    /// <summary>
    /// Number of tenured faculty.
    /// </summary>
    public int TenuredCount { get; set; }

    /// <summary>
    /// Number of non-tenured faculty.
    /// </summary>
    public int NonTenuredCount { get; set; }

    /// <summary>
    /// Faculty by department.
    /// </summary>
    public Dictionary<string, int> FacultyByDepartment { get; set; } = new();

    /// <summary>
    /// Faculty by rank.
    /// </summary>
    public Dictionary<string, int> FacultyByRank { get; set; } = new();

    /// <summary>
    /// Faculty by tenure status.
    /// </summary>
    public Dictionary<string, int> FacultyByTenureStatus { get; set; } = new();
}

/// <summary>
/// Response model for department faculty statistics.
/// </summary>
public class DepartmentFacultyStatisticsResponse
{
    /// <summary>
    /// The department name.
    /// </summary>
    public string DepartmentName { get; set; } = string.Empty;

    /// <summary>
    /// Total faculty in the department.
    /// </summary>
    public int TotalFaculty { get; set; }

    /// <summary>
    /// Number of professors in the department.
    /// </summary>
    public int TotalProfessors { get; set; }

    /// <summary>
    /// Number of teaching professors in the department.
    /// </summary>
    public int TotalTeachingProfs { get; set; }

    /// <summary>
    /// Number of tenured faculty in the department.
    /// </summary>
    public int TenuredCount { get; set; }

    /// <summary>
    /// Number of non-tenured faculty in the department.
    /// </summary>
    public int NonTenuredCount { get; set; }
}

/// <summary>
/// Response model for faculty workload information.
/// </summary>
public class FacultyWorkloadResponse
{
    /// <summary>
    /// The faculty member's employee number.
    /// </summary>
    public int EmpNr { get; set; }

    /// <summary>
    /// The faculty member's name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The academic year for this workload.
    /// </summary>
    public int AcademicYear { get; set; }

    /// <summary>
    /// The semester/term for this workload.
    /// </summary>
    public string? Semester { get; set; }

    /// <summary>
    /// Total teaching load in credit hours.
    /// </summary>
    public decimal TotalTeachingLoad { get; set; }

    /// <summary>
    /// Total service load in hours.
    /// </summary>
    public decimal TotalServiceLoad { get; set; }

    /// <summary>
    /// Total research load in hours.
    /// </summary>
    public decimal TotalResearchLoad { get; set; }

    /// <summary>
    /// Total administrative load in hours.
    /// </summary>
    public decimal TotalAdministrativeLoad { get; set; }

    /// <summary>
    /// Overall workload percentage (compared to standard load).
    /// </summary>
    public decimal WorkloadPercentage { get; set; }

    /// <summary>
    /// Whether the faculty member has overload.
    /// </summary>
    public bool HasOverload { get; set; }

    /// <summary>
    /// Course assignments for this period.
    /// </summary>
    public List<CourseAssignmentResponse> CourseAssignments { get; set; } = new();

    /// <summary>
    /// Committee assignments contributing to service load.
    /// </summary>
    public List<CommitteeAssignmentResponse> ActiveCommitteeAssignments { get; set; } = new();

    /// <summary>
    /// Administrative assignments contributing to admin load.
    /// </summary>
    public List<AdministrativeAssignmentResponse> ActiveAdministrativeAssignments { get; set; } = new();
}

/// <summary>
/// Response model for course assignments.
/// </summary>
public class CourseAssignmentResponse
{
    /// <summary>
    /// The assignment ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The course ID.
    /// </summary>
    public int CourseId { get; set; }

    /// <summary>
    /// The course code (e.g., "CS 101").
    /// </summary>
    public string CourseCode { get; set; } = string.Empty;

    /// <summary>
    /// The course title.
    /// </summary>
    public string CourseTitle { get; set; } = string.Empty;

    /// <summary>
    /// The semester/term.
    /// </summary>
    public string Semester { get; set; } = string.Empty;

    /// <summary>
    /// The academic year.
    /// </summary>
    public int AcademicYear { get; set; }

    /// <summary>
    /// The section number.
    /// </summary>
    public string? Section { get; set; }

    /// <summary>
    /// Credit hours for this assignment.
    /// </summary>
    public decimal CreditHours { get; set; }

    /// <summary>
    /// Teaching modality.
    /// </summary>
    public string? TeachingModality { get; set; }

    /// <summary>
    /// Maximum enrollment.
    /// </summary>
    public int? MaxEnrollment { get; set; }

    /// <summary>
    /// Current enrollment.
    /// </summary>
    public int? CurrentEnrollment { get; set; }

    /// <summary>
    /// Assignment status.
    /// </summary>
    public string AssignmentStatus { get; set; } = string.Empty;
}

/// <summary>
/// Response model for teaching preferences.
/// </summary>
public class TeachingPreferencesResponse
{
    /// <summary>
    /// The faculty member's employee number.
    /// </summary>
    public int EmpNr { get; set; }

    /// <summary>
    /// Preferred courses.
    /// </summary>
    public List<CoursePreferenceResponse> PreferredCourses { get; set; } = new();

    /// <summary>
    /// Preferred teaching times.
    /// </summary>
    public List<string> PreferredTimes { get; set; } = new();

    /// <summary>
    /// Preferred teaching days.
    /// </summary>
    public List<string> PreferredDays { get; set; } = new();

    /// <summary>
    /// Maximum preferred teaching load.
    /// </summary>
    public decimal? MaxPreferredLoad { get; set; }

    /// <summary>
    /// Minimum preferred teaching load.
    /// </summary>
    public decimal? MinPreferredLoad { get; set; }

    /// <summary>
    /// Preferred teaching modalities.
    /// </summary>
    public List<string> PreferredModalities { get; set; } = new();

    /// <summary>
    /// Whether willing to teach overload.
    /// </summary>
    public bool WillingToTeachOverload { get; set; }

    /// <summary>
    /// Additional notes.
    /// </summary>
    public string? AdditionalNotes { get; set; }

    /// <summary>
    /// Last updated date.
    /// </summary>
    public DateTime? LastUpdated { get; set; }
}

/// <summary>
/// Response model for course preferences.
/// </summary>
public class CoursePreferenceResponse
{
    /// <summary>
    /// The course ID.
    /// </summary>
    public int CourseId { get; set; }

    /// <summary>
    /// The course code.
    /// </summary>
    public string CourseCode { get; set; } = string.Empty;

    /// <summary>
    /// The course title.
    /// </summary>
    public string CourseTitle { get; set; } = string.Empty;

    /// <summary>
    /// Preference level (1-5, with 5 being most preferred).
    /// </summary>
    public int PreferenceLevel { get; set; }

    /// <summary>
    /// Whether qualified to teach this course.
    /// </summary>
    public bool IsQualified { get; set; }

    /// <summary>
    /// Notes about this preference.
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// Response model for faculty directory entries.
/// </summary>
public class FacultyDirectoryResponse
{
    /// <summary>
    /// The faculty member's employee number.
    /// </summary>
    public int EmpNr { get; set; }

    /// <summary>
    /// The faculty member's name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The faculty member's professional title.
    /// </summary>
    public string? ProfessionalTitle { get; set; }

    /// <summary>
    /// The faculty type.
    /// </summary>
    public FacultyType FacultyType { get; set; }

    /// <summary>
    /// The academic rank.
    /// </summary>
    public string? RankCode { get; set; }

    /// <summary>
    /// The rank name.
    /// </summary>
    public string? RankName { get; set; }

    /// <summary>
    /// The department name.
    /// </summary>
    public string? DepartmentName { get; set; }

    /// <summary>
    /// Professional email address.
    /// </summary>
    public string? ProfessionalEmail { get; set; }

    /// <summary>
    /// Phone number.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Office location.
    /// </summary>
    public string? OfficeLocation { get; set; }

    /// <summary>
    /// Office hours.
    /// </summary>
    public string? OfficeHours { get; set; }

    /// <summary>
    /// Professional website.
    /// </summary>
    public string? ProfessionalWebsite { get; set; }

    /// <summary>
    /// Research areas.
    /// </summary>
    public List<string> ResearchAreas { get; set; } = new();

    /// <summary>
    /// Current administrative roles.
    /// </summary>
    public List<string> AdministrativeRoles { get; set; } = new();

    /// <summary>
    /// Whether the profile is public.
    /// </summary>
    public bool IsPublicProfile { get; set; }

    /// <summary>
    /// Years of service.
    /// </summary>
    public int? YearsOfService { get; set; }
}

/// <summary>
/// Response model for comprehensive faculty analytics.
/// </summary>
public class FacultyAnalyticsResponse
{
    /// <summary>
    /// Overall faculty statistics.
    /// </summary>
    public FacultyStatisticsResponse OverallStatistics { get; set; } = new();

    /// <summary>
    /// Workload distribution by department.
    /// </summary>
    public Dictionary<string, FacultyWorkloadSummary> WorkloadByDepartment { get; set; } = new();

    /// <summary>
    /// Promotion trends over the last 5 years.
    /// </summary>
    public List<PromotionTrendResponse> PromotionTrends { get; set; } = new();

    /// <summary>
    /// Retirement projections for next 10 years.
    /// </summary>
    public List<RetirementProjectionResponse> RetirementProjections { get; set; } = new();

    /// <summary>
    /// Service load distribution.
    /// </summary>
    public Dictionary<string, decimal> ServiceLoadDistribution { get; set; } = new();

    /// <summary>
    /// Teaching load statistics.
    /// </summary>
    public TeachingLoadStatistics TeachingLoadStats { get; set; } = new();
}

/// <summary>
/// Response model for workload summary by department.
/// </summary>
public class FacultyWorkloadSummary
{
    /// <summary>
    /// Average teaching load.
    /// </summary>
    public decimal AverageTeachingLoad { get; set; }

    /// <summary>
    /// Average service load.
    /// </summary>
    public decimal AverageServiceLoad { get; set; }

    /// <summary>
    /// Faculty count with overload.
    /// </summary>
    public int FacultyWithOverload { get; set; }

    /// <summary>
    /// Faculty count under minimum load.
    /// </summary>
    public int FacultyUnderMinimum { get; set; }

    /// <summary>
    /// Total faculty count.
    /// </summary>
    public int TotalFaculty { get; set; }
}

/// <summary>
/// Response model for promotion trends.
/// </summary>
public class PromotionTrendResponse
{
    /// <summary>
    /// The year.
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// Number of promotions to Assistant Professor.
    /// </summary>
    public int PromotionsToAssistant { get; set; }

    /// <summary>
    /// Number of promotions to Associate Professor.
    /// </summary>
    public int PromotionsToAssociate { get; set; }

    /// <summary>
    /// Number of promotions to Full Professor.
    /// </summary>
    public int PromotionsToFull { get; set; }

    /// <summary>
    /// Total promotions for the year.
    /// </summary>
    public int TotalPromotions { get; set; }
}

/// <summary>
/// Response model for retirement projections.
/// </summary>
public class RetirementProjectionResponse
{
    /// <summary>
    /// The projected year.
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// Number of faculty eligible for retirement.
    /// </summary>
    public int EligibleForRetirement { get; set; }

    /// <summary>
    /// Number of expected retirements.
    /// </summary>
    public int ExpectedRetirements { get; set; }

    /// <summary>
    /// Breakdown by department.
    /// </summary>
    public Dictionary<string, int> ByDepartment { get; set; } = new();
}

/// <summary>
/// Response model for teaching load statistics.
/// </summary>
public class TeachingLoadStatistics
{
    /// <summary>
    /// Average teaching load across all faculty.
    /// </summary>
    public decimal AverageLoad { get; set; }

    /// <summary>
    /// Minimum teaching load.
    /// </summary>
    public decimal MinimumLoad { get; set; }

    /// <summary>
    /// Maximum teaching load.
    /// </summary>
    public decimal MaximumLoad { get; set; }

    /// <summary>
    /// Standard deviation of teaching loads.
    /// </summary>
    public decimal StandardDeviation { get; set; }

    /// <summary>
    /// Percentage of faculty with overload.
    /// </summary>
    public decimal OverloadPercentage { get; set; }
}

/// <summary>
/// Response model for workload balancing results.
/// </summary>
public class WorkloadBalancingResult
{
    /// <summary>
    /// Total number of faculty affected by balancing.
    /// </summary>
    public int TotalFacultyAffected { get; set; }

    /// <summary>
    /// Number of courses reassigned.
    /// </summary>
    public int CoursesReassigned { get; set; }

    /// <summary>
    /// Average teaching load before balancing.
    /// </summary>
    public decimal AverageLoadBefore { get; set; }

    /// <summary>
    /// Average teaching load after balancing.
    /// </summary>
    public decimal AverageLoadAfter { get; set; }

    /// <summary>
    /// Standard deviation of loads before balancing.
    /// </summary>
    public decimal StandardDeviationBefore { get; set; }

    /// <summary>
    /// Standard deviation of loads after balancing.
    /// </summary>
    public decimal StandardDeviationAfter { get; set; }

    /// <summary>
    /// List of recommended or applied changes.
    /// </summary>
    public List<string> RecommendedChanges { get; set; } = new();

    /// <summary>
    /// Improvement score from balancing.
    /// </summary>
    public decimal ImprovementScore { get; set; }

    /// <summary>
    /// Whether changes were applied or just recommended.
    /// </summary>
    public bool ChangesApplied { get; set; }
}

/// <summary>
/// Response model for notification results.
/// </summary>
public class NotificationResult
{
    /// <summary>
    /// Total number of intended recipients.
    /// </summary>
    public int TotalRecipients { get; set; }

    /// <summary>
    /// Number of successful deliveries.
    /// </summary>
    public int SuccessfulDeliveries { get; set; }

    /// <summary>
    /// Number of failed deliveries.
    /// </summary>
    public int FailedDeliveryCount { get; set; }

    /// <summary>
    /// Unique notification identifier.
    /// </summary>
    public string NotificationId { get; set; } = string.Empty;

    /// <summary>
    /// Overall delivery status.
    /// </summary>
    public string DeliveryStatus { get; set; } = string.Empty;

    /// <summary>
    /// List of failed recipient details.
    /// </summary>
    public List<FailedDeliveryDetail> FailedDeliveries { get; set; } = new();

    /// <summary>
    /// Timestamp when notification was sent.
    /// </summary>
    public DateTime SentAt { get; set; }
}

/// <summary>
/// Details about failed notification deliveries.
/// </summary>
public class FailedDeliveryDetail
{
    /// <summary>
    /// Faculty member employee number.
    /// </summary>
    public int EmpNr { get; set; }

    /// <summary>
    /// Faculty member name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Reason for delivery failure.
    /// </summary>
    public string FailureReason { get; set; } = string.Empty;

    /// <summary>
    /// Whether delivery will be retried.
    /// </summary>
    public bool WillRetry { get; set; }
}

/// <summary>
/// Response model for advanced faculty analytics.
/// </summary>
public class AdvancedFacultyAnalyticsResponse
{
    /// <summary>
    /// Workload-related analytics.
    /// </summary>
    public WorkloadAnalytics? WorkloadAnalytics { get; set; }

    /// <summary>
    /// Research-related analytics.
    /// </summary>
    public ResearchAnalytics? ResearchAnalytics { get; set; }

    /// <summary>
    /// Service-related analytics.
    /// </summary>
    public ServiceAnalytics? ServiceAnalytics { get; set; }

    /// <summary>
    /// Tenure-related analytics.
    /// </summary>
    public TenureAnalytics? TenureAnalytics { get; set; }

    /// <summary>
    /// Trend data over multiple years.
    /// </summary>
    public List<TrendDataPoint> TrendData { get; set; } = new();

    /// <summary>
    /// Future projections.
    /// </summary>
    public List<ProjectionDataPoint> Projections { get; set; } = new();
}

/// <summary>
/// Workload analytics data.
/// </summary>
public class WorkloadAnalytics
{
    /// <summary>
    /// Average teaching load across faculty.
    /// </summary>
    public decimal AverageTeachingLoad { get; set; }

    /// <summary>
    /// Variance in teaching loads.
    /// </summary>
    public decimal TeachingLoadVariance { get; set; }

    /// <summary>
    /// Number of faculty with overload.
    /// </summary>
    public int OverloadCount { get; set; }

    /// <summary>
    /// Number of faculty with underload.
    /// </summary>
    public int UnderloadCount { get; set; }

    /// <summary>
    /// Course-to-faculty ratio.
    /// </summary>
    public decimal CourseToFacultyRatio { get; set; }

    /// <summary>
    /// Average class size.
    /// </summary>
    public decimal AverageClassSize { get; set; }
}

/// <summary>
/// Research analytics data.
/// </summary>
public class ResearchAnalytics
{
    /// <summary>
    /// Number of active researchers.
    /// </summary>
    public int ActiveResearchers { get; set; }

    /// <summary>
    /// Average publications per faculty per year.
    /// </summary>
    public decimal AveragePublicationsPerYear { get; set; }

    /// <summary>
    /// Total grant funding amount.
    /// </summary>
    public decimal GrantFundingTotal { get; set; }

    /// <summary>
    /// Number of active grants.
    /// </summary>
    public int ActiveGrants { get; set; }

    /// <summary>
    /// Research collaboration rate.
    /// </summary>
    public decimal CollaborationRate { get; set; }

    /// <summary>
    /// Average research impact score.
    /// </summary>
    public decimal AverageImpactScore { get; set; }
}

/// <summary>
/// Service analytics data.
/// </summary>
public class ServiceAnalytics
{
    /// <summary>
    /// Committee participation rate.
    /// </summary>
    public decimal CommitteeParticipation { get; set; }

    /// <summary>
    /// Average service hours per faculty.
    /// </summary>
    public decimal AverageServiceHours { get; set; }

    /// <summary>
    /// Number of faculty in leadership roles.
    /// </summary>
    public int LeadershipRoles { get; set; }

    /// <summary>
    /// External service participation rate.
    /// </summary>
    public decimal ExternalServiceRate { get; set; }

    /// <summary>
    /// Service load distribution fairness score.
    /// </summary>
    public decimal ServiceLoadFairnessScore { get; set; }
}

/// <summary>
/// Tenure analytics data.
/// </summary>
public class TenureAnalytics
{
    /// <summary>
    /// Overall tenure rate.
    /// </summary>
    public decimal TenureRate { get; set; }

    /// <summary>
    /// Number of faculty eligible for tenure.
    /// </summary>
    public int EligibleForTenure { get; set; }

    /// <summary>
    /// Number of faculty in promotion pipeline.
    /// </summary>
    public int PromotionPipeline { get; set; }

    /// <summary>
    /// Time to tenure (average years).
    /// </summary>
    public decimal AverageTimeToTenure { get; set; }

    /// <summary>
    /// Tenure success rate by department.
    /// </summary>
    public Dictionary<string, decimal> TenureRateByDepartment { get; set; } = new();
}

/// <summary>
/// Data point for trend analysis.
/// </summary>
public class TrendDataPoint
{
    /// <summary>
    /// Year of the data point.
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// Metric name.
    /// </summary>
    public string MetricName { get; set; } = string.Empty;

    /// <summary>
    /// Metric value.
    /// </summary>
    public decimal Value { get; set; }

    /// <summary>
    /// Change from previous year.
    /// </summary>
    public decimal ChangeFromPrevious { get; set; }

    /// <summary>
    /// Percentage change from previous year.
    /// </summary>
    public decimal PercentageChange { get; set; }
}

/// <summary>
/// Data point for future projections.
/// </summary>
public class ProjectionDataPoint
{
    /// <summary>
    /// Projected year.
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// Metric name.
    /// </summary>
    public string MetricName { get; set; } = string.Empty;

    /// <summary>
    /// Projected value.
    /// </summary>
    public decimal ProjectedValue { get; set; }

    /// <summary>
    /// Confidence level of projection.
    /// </summary>
    public decimal ConfidenceLevel { get; set; }

    /// <summary>
    /// Projection methodology used.
    /// </summary>
    public string Methodology { get; set; } = string.Empty;
}

/// <summary>
/// Response model for promotion eligibility validation.
/// </summary>
public class PromotionEligibilityResult
{
    /// <summary>
    /// Whether the faculty member is eligible for promotion.
    /// </summary>
    public bool IsEligible { get; set; }

    /// <summary>
    /// List of requirements that are met.
    /// </summary>
    public List<string> RequirementsMet { get; set; } = new();

    /// <summary>
    /// List of requirements that are not met.
    /// </summary>
    public List<string> RequirementsNotMet { get; set; } = new();

    /// <summary>
    /// Overall eligibility score (0-100).
    /// </summary>
    public decimal EligibilityScore { get; set; }

    /// <summary>
    /// Recommended actions to improve eligibility.
    /// </summary>
    public List<string> RecommendedActions { get; set; } = new();

    /// <summary>
    /// Estimated time to meet all requirements.
    /// </summary>
    public TimeSpan? EstimatedTimeToEligibility { get; set; }

    /// <summary>
    /// Detailed assessment by category.
    /// </summary>
    public Dictionary<string, CategoryAssessment> CategoryAssessments { get; set; } = new();
}

/// <summary>
/// Assessment of a specific category for promotion.
/// </summary>
public class CategoryAssessment
{
    /// <summary>
    /// Category name.
    /// </summary>
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// Score for this category (0-100).
    /// </summary>
    public decimal Score { get; set; }

    /// <summary>
    /// Weight of this category in overall assessment.
    /// </summary>
    public decimal Weight { get; set; }

    /// <summary>
    /// Whether this category meets minimum requirements.
    /// </summary>
    public bool MeetsMinimum { get; set; }

    /// <summary>
    /// Detailed feedback for this category.
    /// </summary>
    public string Feedback { get; set; } = string.Empty;
}

/// <summary>
/// Response model for advanced report generation.
/// </summary>
public class AdvancedReportResult
{
    /// <summary>
    /// Unique report identifier.
    /// </summary>
    public string ReportId { get; set; } = string.Empty;

    /// <summary>
    /// Report title.
    /// </summary>
    public string ReportTitle { get; set; } = string.Empty;

    /// <summary>
    /// Date and time the report was generated.
    /// </summary>
    public DateTime GeneratedDate { get; set; }

    /// <summary>
    /// Number of pages in the report.
    /// </summary>
    public int PageCount { get; set; }

    /// <summary>
    /// Number of sections in the report.
    /// </summary>
    public int SectionCount { get; set; }

    /// <summary>
    /// Number of charts included.
    /// </summary>
    public int ChartCount { get; set; }

    /// <summary>
    /// Total number of data points analyzed.
    /// </summary>
    public int DataPoints { get; set; }

    /// <summary>
    /// File path to the generated report.
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// File size in bytes.
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// Report generation status.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Summary of key findings.
    /// </summary>
    public List<string> KeyFindings { get; set; } = new();

    /// <summary>
    /// Time taken to generate the report.
    /// </summary>
    public TimeSpan GenerationTime { get; set; }
}

// FileUploadResponse is defined in StudentResponses.cs to avoid duplication