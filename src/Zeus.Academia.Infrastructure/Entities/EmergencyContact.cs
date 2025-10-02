using System.ComponentModel.DataAnnotations;
using Zeus.Academia.Infrastructure.Enums;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing an emergency contact for a student
/// </summary>
public class EmergencyContact : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the emergency contact.
    /// </summary>
    [Key]
    public new int Id { get; set; }

    /// <summary>
    /// Gets or sets the student's employee number this contact belongs to.
    /// </summary>
    public int StudentEmpNr { get; set; }

    /// <summary>
    /// Gets or sets the full name of the emergency contact.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string ContactName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the relationship to the student.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Relationship { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the primary phone number.
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string PrimaryPhone { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the secondary phone number.
    /// </summary>
    [MaxLength(20)]
    public string? SecondaryPhone { get; set; }

    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    [MaxLength(255)]
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the complete address.
    /// </summary>
    [MaxLength(500)]
    public string? Address { get; set; }

    /// <summary>
    /// Gets or sets the city.
    /// </summary>
    [MaxLength(100)]
    public string? City { get; set; }

    /// <summary>
    /// Gets or sets the state or province.
    /// </summary>
    [MaxLength(50)]
    public string? State { get; set; }

    /// <summary>
    /// Gets or sets the postal/zip code.
    /// </summary>
    [MaxLength(20)]
    public string? PostalCode { get; set; }

    /// <summary>
    /// Gets or sets the country.
    /// </summary>
    [MaxLength(100)]
    public string? Country { get; set; }

    /// <summary>
    /// Gets or sets the priority order for contacting (1 = highest priority).
    /// </summary>
    public int Priority { get; set; } = 1;

    /// <summary>
    /// Gets or sets whether this contact should be notified in case of emergencies.
    /// </summary>
    public bool NotifyInEmergency { get; set; } = true;

    /// <summary>
    /// Gets or sets whether this contact should be notified for academic issues.
    /// </summary>
    public bool NotifyForAcademicIssues { get; set; } = false;

    /// <summary>
    /// Gets or sets whether this contact should be notified for financial matters.
    /// </summary>
    public bool NotifyForFinancialMatters { get; set; } = false;

    /// <summary>
    /// Gets or sets whether this contact is authorized to receive FERPA protected information.
    /// </summary>
    public bool FerpaAuthorized { get; set; } = false;

    /// <summary>
    /// Gets or sets whether this contact is currently active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets preferred contact method.
    /// </summary>
    public ContactMethod PreferredContactMethod { get; set; } = ContactMethod.Phone;

    /// <summary>
    /// Gets or sets any special notes about this contact.
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the date this contact was verified.
    /// </summary>
    public DateTime? LastVerifiedDate { get; set; }

    /// <summary>
    /// Navigation property to the student.
    /// </summary>
    public virtual Student? Student { get; set; }
}