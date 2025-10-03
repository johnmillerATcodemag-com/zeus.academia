using System.ComponentModel.DataAnnotations;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing comprehensive faculty profile information.
/// Manages biographical data, contact information, professional interests, and public profile details.
/// </summary>
public class FacultyProfile : BaseEntity
{
    /// <summary>
    /// Gets or sets the academic employee number this profile belongs to.
    /// </summary>
    [Required]
    public int AcademicEmpNr { get; set; }

    /// <summary>
    /// Gets or sets the faculty member's professional title (Dr., Prof., etc.).
    /// </summary>
    [MaxLength(20)]
    public string? ProfessionalTitle { get; set; }

    /// <summary>
    /// Gets or sets the faculty member's preferred name for display.
    /// </summary>
    [MaxLength(100)]
    public string? PreferredName { get; set; }

    /// <summary>
    /// Gets or sets the faculty member's professional email address.
    /// </summary>
    [MaxLength(100)]
    public string? ProfessionalEmail { get; set; }

    /// <summary>
    /// Gets or sets the faculty member's office phone number.
    /// </summary>
    [MaxLength(20)]
    public string? OfficePhone { get; set; }

    /// <summary>
    /// Gets or sets the faculty member's mobile phone number.
    /// </summary>
    [MaxLength(20)]
    public string? MobilePhone { get; set; }

    /// <summary>
    /// Gets or sets the faculty member's personal website URL.
    /// </summary>
    [MaxLength(200)]
    public string? WebsiteUrl { get; set; }

    /// <summary>
    /// Gets or sets the faculty member's ORCID identifier.
    /// </summary>
    [MaxLength(50)]
    public string? OrcidId { get; set; }

    /// <summary>
    /// Gets or sets the faculty member's Google Scholar profile URL.
    /// </summary>
    [MaxLength(200)]
    public string? GoogleScholarUrl { get; set; }

    /// <summary>
    /// Gets or sets the faculty member's LinkedIn profile URL.
    /// </summary>
    [MaxLength(200)]
    public string? LinkedInUrl { get; set; }

    /// <summary>
    /// Gets or sets the faculty member's ResearchGate profile URL.
    /// </summary>
    [MaxLength(200)]
    public string? ResearchGateUrl { get; set; }

    /// <summary>
    /// Gets or sets the faculty member's professional biography.
    /// </summary>
    [MaxLength(2000)]
    public string? Biography { get; set; }

    /// <summary>
    /// Gets or sets the faculty member's research interests summary.
    /// </summary>
    [MaxLength(1000)]
    public string? ResearchInterests { get; set; }

    /// <summary>
    /// Gets or sets the faculty member's teaching philosophy.
    /// </summary>
    [MaxLength(1000)]
    public string? TeachingPhilosophy { get; set; }

    /// <summary>
    /// Gets or sets the faculty member's educational background summary.
    /// </summary>
    [MaxLength(1000)]
    public string? EducationSummary { get; set; }

    /// <summary>
    /// Gets or sets the faculty member's awards and honors summary.
    /// </summary>
    [MaxLength(1000)]
    public string? AwardsHonors { get; set; }

    /// <summary>
    /// Gets or sets the faculty member's professional memberships.
    /// </summary>
    [MaxLength(1000)]
    public string? ProfessionalMemberships { get; set; }

    /// <summary>
    /// Gets or sets the faculty member's current research projects.
    /// </summary>
    [MaxLength(1000)]
    public string? CurrentResearchProjects { get; set; }

    /// <summary>
    /// Gets or sets the faculty member's consultation availability.
    /// </summary>
    [MaxLength(500)]
    public string? ConsultationAvailability { get; set; }

    /// <summary>
    /// Gets or sets the faculty member's office hours.
    /// </summary>
    [MaxLength(500)]
    public string? OfficeHours { get; set; }

    /// <summary>
    /// Gets or sets whether the profile is public and visible to students/external users.
    /// </summary>
    [Required]
    public bool IsPublicProfile { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the faculty member is available for media inquiries.
    /// </summary>
    [Required]
    public bool IsMediaContactAvailable { get; set; } = false;

    /// <summary>
    /// Gets or sets whether the faculty member is accepting new graduate students.
    /// </summary>
    [Required]
    public bool IsAcceptingGraduateStudents { get; set; } = false;

    /// <summary>
    /// Gets or sets the date when the profile was last updated.
    /// </summary>
    public DateTime? LastProfileUpdate { get; set; }

    /// <summary>
    /// Gets or sets the path to the faculty member's profile photo.
    /// </summary>
    [MaxLength(500)]
    public string? ProfilePhotoPath { get; set; }

    /// <summary>
    /// Gets or sets the faculty member's emergency contact information.
    /// </summary>
    [MaxLength(500)]
    public string? EmergencyContact { get; set; }

    /// <summary>
    /// Gets or sets additional notes for internal use.
    /// </summary>
    [MaxLength(1000)]
    public string? InternalNotes { get; set; }

    /// <summary>
    /// Navigation property to the academic employee.
    /// </summary>
    public virtual Academic Academic { get; set; } = null!;

    /// <summary>
    /// Navigation property to faculty documents (CV, publications, etc.).
    /// </summary>
    public virtual ICollection<FacultyDocument> Documents { get; set; } = new List<FacultyDocument>();

    /// <summary>
    /// Navigation property to faculty publications.
    /// </summary>
    public virtual ICollection<FacultyPublication> Publications { get; set; } = new List<FacultyPublication>();

    /// <summary>
    /// Navigation property to office assignments.
    /// </summary>
    public virtual ICollection<OfficeAssignment> OfficeAssignments { get; set; } = new List<OfficeAssignment>();
}