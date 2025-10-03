using System.ComponentModel.DataAnnotations;
using Zeus.Academia.Api.Models.Common;
using Zeus.Academia.Infrastructure.Services.Interfaces;

namespace Zeus.Academia.Api.Models.Requests;

/// <summary>
/// Request model for creating a new faculty member.
/// </summary>
public class CreateFacultyRequest
{
    /// <summary>
    /// The faculty member's full name - required and must be between 2-100 characters.
    /// </summary>
    [Required(ErrorMessage = "Faculty name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Faculty name must be between 2 and 100 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The faculty member's employee number - required and must be positive.
    /// </summary>
    [Required(ErrorMessage = "Employee number is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Employee number must be positive")]
    public int EmpNr { get; set; }

    /// <summary>
    /// Optional phone number - must be in valid format if provided.
    /// </summary>
    [Phone(ErrorMessage = "Invalid phone number format")]
    [StringLength(20, ErrorMessage = "Phone number must not exceed 20 characters")]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Optional salary for the faculty member.
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Salary must be non-negative")]
    public decimal? Salary { get; set; }

    /// <summary>
    /// The type of faculty member (Professor, TeachingProf).
    /// </summary>
    [Required(ErrorMessage = "Faculty type is required")]
    public FacultyType FacultyType { get; set; }

    /// <summary>
    /// The academic rank code of the faculty member.
    /// </summary>
    [StringLength(10, ErrorMessage = "Rank code must not exceed 10 characters")]
    public string? RankCode { get; set; }

    /// <summary>
    /// The department name where the faculty member works.
    /// </summary>
    [StringLength(100, ErrorMessage = "Department name must not exceed 100 characters")]
    public string? DepartmentName { get; set; }

    /// <summary>
    /// Whether the faculty member has tenure.
    /// </summary>
    public bool? HasTenure { get; set; }

    /// <summary>
    /// The faculty member's research area or specialty.
    /// </summary>
    [StringLength(200, ErrorMessage = "Research area must not exceed 200 characters")]
    public string? ResearchArea { get; set; }

    /// <summary>
    /// The department ID where the faculty member works.
    /// </summary>
    public int? DepartmentId { get; set; }
}

/// <summary>
/// Request model for updating an existing faculty member.
/// </summary>
public class UpdateFacultyRequest
{
    /// <summary>
    /// The faculty member's full name - required and must be between 2-100 characters.
    /// </summary>
    [Required(ErrorMessage = "Faculty name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Faculty name must be between 2 and 100 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional phone number - must be in valid format if provided.
    /// </summary>
    [Phone(ErrorMessage = "Invalid phone number format")]
    [StringLength(20, ErrorMessage = "Phone number must not exceed 20 characters")]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Optional salary for the faculty member.
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Salary must be non-negative")]
    public decimal? Salary { get; set; }

    /// <summary>
    /// The academic rank code of the faculty member.
    /// </summary>
    [StringLength(10, ErrorMessage = "Rank code must not exceed 10 characters")]
    public string? RankCode { get; set; }

    /// <summary>
    /// The department name where the faculty member works.
    /// </summary>
    [StringLength(100, ErrorMessage = "Department name must not exceed 100 characters")]
    public string? DepartmentName { get; set; }

    /// <summary>
    /// Whether the faculty member has tenure.
    /// </summary>
    public bool? HasTenure { get; set; }

    /// <summary>
    /// The faculty member's research area or specialty.
    /// </summary>
    [StringLength(200, ErrorMessage = "Research area must not exceed 200 characters")]
    public string? ResearchArea { get; set; }

    /// <summary>
    /// The department ID where the faculty member works.
    /// </summary>
    public int? DepartmentId { get; set; }
}

/// <summary>
/// Request model for searching faculty with various criteria.
/// </summary>
public class FacultySearchRequest : PaginationParameters
{
    /// <summary>
    /// General search term (searches name, research area).
    /// </summary>
    [StringLength(100, ErrorMessage = "Search term must not exceed 100 characters")]
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Filter by department name.
    /// </summary>
    [StringLength(100, ErrorMessage = "Department name must not exceed 100 characters")]
    public string? DepartmentName { get; set; }

    /// <summary>
    /// Filter by academic rank code.
    /// </summary>
    [StringLength(10, ErrorMessage = "Rank code must not exceed 10 characters")]
    public string? RankCode { get; set; }

    /// <summary>
    /// Filter by tenure status.
    /// </summary>
    public bool? HasTenure { get; set; }

    /// <summary>
    /// Filter by research area.
    /// </summary>
    [StringLength(200, ErrorMessage = "Research area must not exceed 200 characters")]
    public string? ResearchArea { get; set; }

    /// <summary>
    /// Filter by faculty type.
    /// </summary>
    public FacultyType? FacultyType { get; set; }

    /// <summary>
    /// Filter by department ID.
    /// </summary>
    public int? DepartmentId { get; set; }

    /// <summary>
    /// Filter by minimum salary.
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Min salary must be non-negative")]
    public decimal? MinSalary { get; set; }

    /// <summary>
    /// Filter by maximum salary.
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Max salary must be non-negative")]
    public decimal? MaxSalary { get; set; }

    /// <summary>
    /// Filter by active status.
    /// </summary>
    public bool? IsActive { get; set; }
}

/// <summary>
/// Request model for updating faculty tenure status.
/// </summary>
public class UpdateTenureStatusRequest
{
    /// <summary>
    /// The new tenure status.
    /// </summary>
    [Required(ErrorMessage = "Tenure status is required")]
    public bool HasTenure { get; set; }

    /// <summary>
    /// Optional notes about the tenure change.
    /// </summary>
    [StringLength(500, ErrorMessage = "Notes must not exceed 500 characters")]
    public string? Notes { get; set; }
}

/// <summary>
/// Request model for updating faculty rank.
/// </summary>
public class UpdateRankRequest
{
    /// <summary>
    /// The new rank code.
    /// </summary>
    [Required(ErrorMessage = "Rank code is required")]
    [StringLength(10, ErrorMessage = "Rank code must not exceed 10 characters")]
    public string RankCode { get; set; } = string.Empty;

    /// <summary>
    /// The effective date of the rank change.
    /// </summary>
    [Required(ErrorMessage = "Effective date is required")]
    [DataType(DataType.Date)]
    public DateTime EffectiveDate { get; set; }

    /// <summary>
    /// Optional notes about the rank change.
    /// </summary>
    [StringLength(500, ErrorMessage = "Notes must not exceed 500 characters")]
    public string? Notes { get; set; }
}

/// <summary>
/// Request model for creating or updating faculty profile.
/// </summary>
public class SaveFacultyProfileRequest
{
    /// <summary>
    /// The faculty member's professional title (Dr., Prof., etc.).
    /// </summary>
    [StringLength(20, ErrorMessage = "Professional title must not exceed 20 characters")]
    public string? ProfessionalTitle { get; set; }

    /// <summary>
    /// The faculty member's preferred name for display.
    /// </summary>
    [StringLength(100, ErrorMessage = "Preferred name must not exceed 100 characters")]
    public string? PreferredName { get; set; }

    /// <summary>
    /// The faculty member's professional email address.
    /// </summary>
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(100, ErrorMessage = "Professional email must not exceed 100 characters")]
    public string? ProfessionalEmail { get; set; }

    /// <summary>
    /// The faculty member's professional website URL.
    /// </summary>
    [Url(ErrorMessage = "Invalid website URL format")]
    [StringLength(200, ErrorMessage = "Professional website must not exceed 200 characters")]
    public string? ProfessionalWebsite { get; set; }

    /// <summary>
    /// The faculty member's biography.
    /// </summary>
    [StringLength(2000, ErrorMessage = "Biography must not exceed 2000 characters")]
    public string? Biography { get; set; }

    /// <summary>
    /// The faculty member's research interests.
    /// </summary>
    [StringLength(1000, ErrorMessage = "Research interests must not exceed 1000 characters")]
    public string? ResearchInterests { get; set; }

    /// <summary>
    /// The faculty member's teaching philosophy.
    /// </summary>
    [StringLength(1000, ErrorMessage = "Teaching philosophy must not exceed 1000 characters")]
    public string? TeachingPhilosophy { get; set; }

    /// <summary>
    /// The faculty member's awards and honors.
    /// </summary>
    [StringLength(1000, ErrorMessage = "Awards must not exceed 1000 characters")]
    public string? Awards { get; set; }

    /// <summary>
    /// The faculty member's professional memberships.
    /// </summary>
    [StringLength(1000, ErrorMessage = "Professional memberships must not exceed 1000 characters")]
    public string? ProfessionalMemberships { get; set; }

    /// <summary>
    /// The faculty member's current research projects.
    /// </summary>
    [StringLength(1000, ErrorMessage = "Current research projects must not exceed 1000 characters")]
    public string? CurrentResearchProjects { get; set; }

    /// <summary>
    /// The faculty member's consultation availability.
    /// </summary>
    [StringLength(500, ErrorMessage = "Consultation availability must not exceed 500 characters")]
    public string? ConsultationAvailability { get; set; }

    /// <summary>
    /// The faculty member's office hours.
    /// </summary>
    [StringLength(500, ErrorMessage = "Office hours must not exceed 500 characters")]
    public string? OfficeHours { get; set; }

    /// <summary>
    /// Whether the profile is public or private.
    /// </summary>
    public bool IsPublicProfile { get; set; } = true;
}

/// <summary>
/// Request model for adding research expertise to a faculty member.
/// </summary>
public class AddResearchExpertiseRequest
{
    /// <summary>
    /// The research area ID.
    /// </summary>
    [Required(ErrorMessage = "Research area ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Research area ID must be positive")]
    public int ResearchAreaId { get; set; }

    /// <summary>
    /// The level of expertise.
    /// </summary>
    [Required(ErrorMessage = "Expertise level is required")]
    [StringLength(50, ErrorMessage = "Expertise level must not exceed 50 characters")]
    public string ExpertiseLevel { get; set; } = string.Empty;

    /// <summary>
    /// Years of experience in this research area.
    /// </summary>
    [Range(0, 100, ErrorMessage = "Years of experience must be between 0 and 100")]
    public int? YearsOfExperience { get; set; }
}

/// <summary>
/// Request model for uploading faculty documents.
/// </summary>
public class UploadFacultyDocumentRequest
{
    /// <summary>
    /// The type of document being uploaded.
    /// </summary>
    [Required(ErrorMessage = "Document type is required")]
    [StringLength(50, ErrorMessage = "Document type must not exceed 50 characters")]
    public string DocumentType { get; set; } = string.Empty;

    /// <summary>
    /// The file name.
    /// </summary>
    [Required(ErrorMessage = "File name is required")]
    [StringLength(255, ErrorMessage = "File name must not exceed 255 characters")]
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// The file content as base64 encoded string.
    /// </summary>
    [Required(ErrorMessage = "File content is required")]
    public string FileContent { get; set; } = string.Empty;

    /// <summary>
    /// Whether the document is public or private.
    /// </summary>
    public bool IsPublic { get; set; } = false;
}

/// <summary>
/// Request model for uploading faculty photos.
/// </summary>
public class UploadFacultyPhotoRequest
{
    /// <summary>
    /// The file name.
    /// </summary>
    [Required(ErrorMessage = "File name is required")]
    [StringLength(255, ErrorMessage = "File name must not exceed 255 characters")]
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// The photo content as base64 encoded string.
    /// </summary>
    [Required(ErrorMessage = "Photo content is required")]
    public string PhotoContent { get; set; } = string.Empty;
}

/// <summary>
/// Request model for assigning faculty to administrative positions.
/// </summary>
public class AssignAdministrativeRoleRequest
{
    /// <summary>
    /// The administrative role title (Dean, Associate Dean, Department Chair, etc.).
    /// </summary>
    [Required(ErrorMessage = "Role title is required")]
    [StringLength(100, ErrorMessage = "Role title must not exceed 100 characters")]
    public string RoleTitle { get; set; } = string.Empty;

    /// <summary>
    /// The department or unit for the assignment.
    /// </summary>
    [StringLength(100, ErrorMessage = "Department must not exceed 100 characters")]
    public string? Department { get; set; }

    /// <summary>
    /// The start date of the assignment.
    /// </summary>
    [Required(ErrorMessage = "Assignment start date is required")]
    public DateTime AssignmentStartDate { get; set; }

    /// <summary>
    /// The end date of the assignment (optional for indefinite assignments).
    /// </summary>
    public DateTime? AssignmentEndDate { get; set; }

    /// <summary>
    /// Additional notes about the assignment.
    /// </summary>
    [StringLength(500, ErrorMessage = "Notes must not exceed 500 characters")]
    public string? Notes { get; set; }
}

/// <summary>
/// Request model for assigning faculty to committees.
/// </summary>
public class AssignCommitteeRequest
{
    /// <summary>
    /// The committee ID.
    /// </summary>
    [Required(ErrorMessage = "Committee ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Committee ID must be positive")]
    public int CommitteeId { get; set; }

    /// <summary>
    /// The role on the committee (Chair, Member, Ex-Officio, etc.).
    /// </summary>
    [Required(ErrorMessage = "Committee role is required")]
    [StringLength(50, ErrorMessage = "Committee role must not exceed 50 characters")]
    public string CommitteeRole { get; set; } = string.Empty;

    /// <summary>
    /// The start date of the assignment.
    /// </summary>
    [Required(ErrorMessage = "Assignment start date is required")]
    public DateTime AssignmentStartDate { get; set; }

    /// <summary>
    /// The end date of the assignment (optional for indefinite assignments).
    /// </summary>
    public DateTime? AssignmentEndDate { get; set; }

    /// <summary>
    /// Whether this is a primary committee assignment.
    /// </summary>
    public bool IsPrimaryAssignment { get; set; } = false;

    /// <summary>
    /// Expected time commitment in hours per week.
    /// </summary>
    [Range(0, 40, ErrorMessage = "Time commitment must be between 0 and 40 hours per week")]
    public decimal? ExpectedTimeCommitment { get; set; }
}

/// <summary>
/// Request model for assigning courses to faculty.
/// </summary>
public class AssignCourseRequest
{
    /// <summary>
    /// The course ID to assign.
    /// </summary>
    [Required(ErrorMessage = "Course ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Course ID must be positive")]
    public int CourseId { get; set; }

    /// <summary>
    /// The semester/term for the assignment.
    /// </summary>
    [Required(ErrorMessage = "Semester/term is required")]
    [StringLength(20, ErrorMessage = "Semester must not exceed 20 characters")]
    public string Semester { get; set; } = string.Empty;

    /// <summary>
    /// The academic year for the assignment.
    /// </summary>
    [Required(ErrorMessage = "Academic year is required")]
    [Range(2020, 2050, ErrorMessage = "Academic year must be between 2020 and 2050")]
    public int AcademicYear { get; set; }

    /// <summary>
    /// The section number if applicable.
    /// </summary>
    [StringLength(10, ErrorMessage = "Section must not exceed 10 characters")]
    public string? Section { get; set; }

    /// <summary>
    /// Credit hours for this course assignment.
    /// </summary>
    [Range(0.5, 10.0, ErrorMessage = "Credit hours must be between 0.5 and 10.0")]
    public decimal CreditHours { get; set; }

    /// <summary>
    /// Teaching modality (In-Person, Online, Hybrid).
    /// </summary>
    [StringLength(20, ErrorMessage = "Teaching modality must not exceed 20 characters")]
    public string? TeachingModality { get; set; }

    /// <summary>
    /// Maximum enrollment for this section.
    /// </summary>
    [Range(1, 500, ErrorMessage = "Max enrollment must be between 1 and 500")]
    public int? MaxEnrollment { get; set; }
}

/// <summary>
/// Request model for updating faculty teaching preferences.
/// </summary>
public class UpdateTeachingPreferencesRequest
{
    /// <summary>
    /// Preferred courses to teach.
    /// </summary>
    public List<int> PreferredCourseIds { get; set; } = new();

    /// <summary>
    /// Preferred teaching times (Morning, Afternoon, Evening).
    /// </summary>
    public List<string> PreferredTimes { get; set; } = new();

    /// <summary>
    /// Preferred teaching days.
    /// </summary>
    public List<string> PreferredDays { get; set; } = new();

    /// <summary>
    /// Maximum preferred teaching load in credit hours.
    /// </summary>
    [Range(0, 40, ErrorMessage = "Max teaching load must be between 0 and 40 credit hours")]
    public decimal? MaxPreferredLoad { get; set; }

    /// <summary>
    /// Minimum preferred teaching load in credit hours.
    /// </summary>
    [Range(0, 40, ErrorMessage = "Min teaching load must be between 0 and 40 credit hours")]
    public decimal? MinPreferredLoad { get; set; }

    /// <summary>
    /// Preferred teaching modalities.
    /// </summary>
    public List<string> PreferredModalities { get; set; } = new();

    /// <summary>
    /// Whether willing to teach overload courses.
    /// </summary>
    public bool WillingToTeachOverload { get; set; } = false;

    /// <summary>
    /// Additional notes about teaching preferences.
    /// </summary>
    [StringLength(1000, ErrorMessage = "Notes must not exceed 1000 characters")]
    public string? AdditionalNotes { get; set; }
}

/// <summary>
/// Request model for faculty workload analysis.
/// </summary>
public class FacultyWorkloadRequest
{
    /// <summary>
    /// The academic year for workload analysis.
    /// </summary>
    [Required(ErrorMessage = "Academic year is required")]
    [Range(2020, 2050, ErrorMessage = "Academic year must be between 2020 and 2050")]
    public int AcademicYear { get; set; }

    /// <summary>
    /// The semester/term for analysis (optional, if null returns full year).
    /// </summary>
    [StringLength(20, ErrorMessage = "Semester must not exceed 20 characters")]
    public string? Semester { get; set; }

    /// <summary>
    /// Whether to include service activities in workload calculation.
    /// </summary>
    public bool IncludeServiceActivities { get; set; } = true;

    /// <summary>
    /// Whether to include research activities in workload calculation.
    /// </summary>
    public bool IncludeResearchActivities { get; set; } = true;

    /// <summary>
    /// Whether to include administrative duties in workload calculation.
    /// </summary>
    public bool IncludeAdministrativeDuties { get; set; } = true;
}

/// <summary>
/// Request model for advanced faculty search.
/// </summary>
public class AdvancedFacultySearchRequest : PaginationParameters
{
    /// <summary>
    /// Search by faculty name or email.
    /// </summary>
    [StringLength(100, ErrorMessage = "Name search must not exceed 100 characters")]
    public string? NameSearch { get; set; }

    /// <summary>
    /// Filter by multiple departments.
    /// </summary>
    public List<string> DepartmentNames { get; set; } = new();

    /// <summary>
    /// Filter by multiple ranks.
    /// </summary>
    public List<string> RankCodes { get; set; } = new();

    /// <summary>
    /// Filter by tenure status.
    /// </summary>
    public bool? HasTenure { get; set; }

    /// <summary>
    /// Filter by multiple research areas.
    /// </summary>
    public List<string> ResearchAreas { get; set; } = new();

    /// <summary>
    /// Filter by faculty types.
    /// </summary>
    public List<FacultyType> FacultyTypes { get; set; } = new();

    /// <summary>
    /// Filter by administrative roles.
    /// </summary>
    public List<string> AdministrativeRoles { get; set; } = new();

    /// <summary>
    /// Filter by committee memberships.
    /// </summary>
    public List<int> CommitteeIds { get; set; } = new();

    /// <summary>
    /// Filter by minimum years of experience.
    /// </summary>
    [Range(0, 50, ErrorMessage = "Min experience must be between 0 and 50 years")]
    public int? MinYearsExperience { get; set; }

    /// <summary>
    /// Filter by maximum years of experience.
    /// </summary>
    [Range(0, 50, ErrorMessage = "Max experience must be between 0 and 50 years")]
    public int? MaxYearsExperience { get; set; }

    /// <summary>
    /// Filter by active status.
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Sort by field (Name, Rank, Department, YearsOfService).
    /// </summary>
    [StringLength(20, ErrorMessage = "Sort by must not exceed 20 characters")]
    public string? SortByField { get; set; }

    /// <summary>
    /// Sort direction (ASC or DESC).
    /// </summary>
    [StringLength(4, ErrorMessage = "Sort direction must be ASC or DESC")]
    public string SortOrder { get; set; } = "ASC";
}