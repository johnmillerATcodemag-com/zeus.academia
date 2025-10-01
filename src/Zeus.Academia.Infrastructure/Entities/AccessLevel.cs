using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing system access levels and permissions.
/// Task 4: Infrastructure Entities - System access control and permission management.
/// </summary>
public class AccessLevel : BaseEntity
{
    /// <summary>
    /// Gets or sets the access level code - the primary identifier.
    /// </summary>
    [Key]
    [Required(ErrorMessage = "Access level code is required")]
    [MaxLength(10, ErrorMessage = "Access level code cannot exceed 10 characters")]
    [RegularExpression(@"^[A-Z]{2,8}[0-9]{0,2}$", ErrorMessage = "Access level code must be 2-8 uppercase letters optionally followed by up to 2 numbers (e.g., ADMIN, USER, GUEST1)")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the access level name or title.
    /// </summary>
    [Required(ErrorMessage = "Access level name is required")]
    [MaxLength(50, ErrorMessage = "Access level name cannot exceed 50 characters")]
    [MinLength(3, ErrorMessage = "Access level name must be at least 3 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the access level and its permissions.
    /// </summary>
    [MaxLength(300, ErrorMessage = "Access level description cannot exceed 300 characters")]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the hierarchical level of access (1 = highest access).
    /// </summary>
    [Range(1, 100, ErrorMessage = "Access level must be between 1 (highest) and 100 (lowest)")]
    public int? Level { get; set; }

    /// <summary>
    /// Gets or sets whether this access level is currently active.
    /// </summary>
    [Required]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets whether this access level can read data.
    /// </summary>
    [Required]
    public bool CanRead { get; set; } = true;

    /// <summary>
    /// Gets or sets whether this access level can create new data.
    /// </summary>
    [Required]
    public bool CanCreate { get; set; } = false;

    /// <summary>
    /// Gets or sets whether this access level can update existing data.
    /// </summary>
    [Required]
    public bool CanUpdate { get; set; } = false;

    /// <summary>
    /// Gets or sets whether this access level can delete data.
    /// </summary>
    [Required]
    public bool CanDelete { get; set; } = false;

    /// <summary>
    /// Gets or sets whether this access level can execute administrative functions.
    /// </summary>
    [Required]
    public bool CanExecute { get; set; } = false;

    /// <summary>
    /// Gets or sets whether this access level can modify system configurations.
    /// </summary>
    [Required]
    public bool CanModifySystem { get; set; } = false;

    /// <summary>
    /// Gets or sets whether this access level can access financial data.
    /// </summary>
    [Required]
    public bool CanAccessFinancial { get; set; } = false;

    /// <summary>
    /// Gets or sets whether this access level can access student records (FERPA sensitive).
    /// </summary>
    [Required]
    public bool CanAccessStudentRecords { get; set; } = false;

    /// <summary>
    /// Gets or sets whether this access level can access faculty records.
    /// </summary>
    [Required]
    public bool CanAccessFacultyRecords { get; set; } = false;

    /// <summary>
    /// Gets or sets whether this access level can generate reports.
    /// </summary>
    [Required]
    public bool CanGenerateReports { get; set; } = false;

    /// <summary>
    /// Gets or sets the category or type of access level.
    /// </summary>
    [Required(ErrorMessage = "Access category is required")]
    [MaxLength(20, ErrorMessage = "Access category cannot exceed 20 characters")]
    [RegularExpression(@"^(System|Academic|Administrative|Student|Faculty|Staff|Guest|Service)$",
        ErrorMessage = "Access category must be one of: System, Academic, Administrative, Student, Faculty, Staff, Guest, Service")]
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the maximum number of concurrent sessions allowed.
    /// </summary>
    [Range(1, 100, ErrorMessage = "Maximum sessions must be between 1 and 100")]
    public int? MaxConcurrentSessions { get; set; }

    /// <summary>
    /// Gets or sets the session timeout in minutes.
    /// </summary>
    [Range(5, 1440, ErrorMessage = "Session timeout must be between 5 minutes and 24 hours")]
    public int? SessionTimeoutMinutes { get; set; }

    /// <summary>
    /// Gets or sets whether this access level requires two-factor authentication.
    /// </summary>
    [Required]
    public bool RequiresTwoFactor { get; set; } = false;

    /// <summary>
    /// Gets or sets whether password changes are required periodically.
    /// </summary>
    [Required]
    public bool RequiresPasswordChange { get; set; } = true;

    /// <summary>
    /// Gets or sets how often password changes are required (in days).
    /// </summary>
    [Range(30, 365, ErrorMessage = "Password change frequency must be between 30 and 365 days")]
    public int? PasswordChangeFrequencyDays { get; set; }

    /// <summary>
    /// Gets or sets special permissions or notes for this access level.
    /// </summary>
    [MaxLength(500, ErrorMessage = "Special permissions cannot exceed 500 characters")]
    public string? SpecialPermissions { get; set; }
}