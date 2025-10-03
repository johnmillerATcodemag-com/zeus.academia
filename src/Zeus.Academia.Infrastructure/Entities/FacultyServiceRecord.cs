using System.ComponentModel.DataAnnotations;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing service workload and committee leadership tracking.
/// Extends committee membership with historical tracking and service contribution calculation.
/// </summary>
public class FacultyServiceRecord : BaseEntity
{
    /// <summary>
    /// Gets or sets the academic employee number.
    /// </summary>
    [Required]
    public int AcademicEmpNr { get; set; }

    /// <summary>
    /// Gets or sets the service type (Committee, Editorial, Review, Administrative, etc.).
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string ServiceType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the service title or position.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string ServiceTitle { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the organization or committee name.
    /// </summary>
    [MaxLength(200)]
    public string? Organization { get; set; }

    /// <summary>
    /// Gets or sets the service level (Department, College, University, Professional, National).
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string ServiceLevel { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the leadership role level (Member, Secretary, Vice-Chair, Chair, etc.).
    /// </summary>
    [MaxLength(50)]
    public string? LeadershipRole { get; set; }

    /// <summary>
    /// Gets or sets the start date of service.
    /// </summary>
    [Required]
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Gets or sets the end date of service (null if ongoing).
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Gets or sets whether this service is currently active.
    /// </summary>
    [Required]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the estimated workload hours per academic year.
    /// </summary>
    public decimal? EstimatedHoursPerYear { get; set; }

    /// <summary>
    /// Gets or sets the service workload weight/multiplier for promotion consideration.
    /// </summary>
    public decimal? ServiceWeight { get; set; } = 1.0m;

    /// <summary>
    /// Gets or sets whether this is major service (significant time commitment).
    /// </summary>
    [Required]
    public bool IsMajorService { get; set; } = false;

    /// <summary>
    /// Gets or sets whether this service involves external organizations.
    /// </summary>
    [Required]
    public bool IsExternalService { get; set; } = false;

    /// <summary>
    /// Gets or sets additional notes about the service.
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets recognition or awards received for this service.
    /// </summary>
    [MaxLength(500)]
    public string? Recognition { get; set; }

    /// <summary>
    /// Navigation property to the academic employee.
    /// </summary>
    public virtual Academic Academic { get; set; } = null!;
}

/// <summary>
/// Entity representing committee leadership history and transitions.
/// Tracks changes in committee leadership roles over time.
/// </summary>
public class CommitteeLeadership : BaseEntity
{
    /// <summary>
    /// Gets or sets the committee name.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string CommitteeName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the academic employee number of the leader.
    /// </summary>
    [Required]
    public int AcademicEmpNr { get; set; }

    /// <summary>
    /// Gets or sets the leadership position (Chair, Vice-Chair, Secretary, etc.).
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Position { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the start date of the leadership role.
    /// </summary>
    [Required]
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Gets or sets the end date of the leadership role (null if current).
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Gets or sets whether this is the current leadership position.
    /// </summary>
    [Required]
    public bool IsCurrent { get; set; } = true;

    /// <summary>
    /// Gets or sets the reason for leadership change (Term completion, Resignation, Promotion, etc.).
    /// </summary>
    [MaxLength(100)]
    public string? ChangeReason { get; set; }

    /// <summary>
    /// Gets or sets who appointed this leader.
    /// </summary>
    [MaxLength(100)]
    public string? AppointedBy { get; set; }

    /// <summary>
    /// Gets or sets the appointment date.
    /// </summary>
    public DateTime? AppointmentDate { get; set; }

    /// <summary>
    /// Gets or sets additional notes about the leadership role.
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Navigation property to the committee.
    /// </summary>
    public virtual Committee Committee { get; set; } = null!;

    /// <summary>
    /// Navigation property to the academic leader.
    /// </summary>
    public virtual Academic Academic { get; set; } = null!;
}