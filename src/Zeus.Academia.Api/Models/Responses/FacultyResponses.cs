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

// FileUploadResponse is defined in StudentResponses.cs to avoid duplication