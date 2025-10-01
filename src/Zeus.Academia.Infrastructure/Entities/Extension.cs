using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing a phone extension for contact information management.
/// Task 4: Infrastructure Entities - Contact information and communication management.
/// </summary>
public class Extension : BaseEntity
{
    /// <summary>
    /// Gets or sets the extension number - the primary identifier.
    /// </summary>
    [Key]
    [Required(ErrorMessage = "Extension number is required")]
    [MaxLength(10, ErrorMessage = "Extension number cannot exceed 10 characters")]
    [RegularExpression(@"^[0-9]{3,10}$", ErrorMessage = "Extension number must be 3-10 digits (e.g., 1234, 55501)")]
    public string Number { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description or purpose of the extension.
    /// </summary>
    [MaxLength(200, ErrorMessage = "Extension description cannot exceed 200 characters")]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the department or office associated with this extension.
    /// </summary>
    [MaxLength(100, ErrorMessage = "Department name cannot exceed 100 characters")]
    public string? Department { get; set; }

    /// <summary>
    /// Gets or sets the location of the extension (building and room).
    /// </summary>
    [MaxLength(50, ErrorMessage = "Location cannot exceed 50 characters")]
    public string? Location { get; set; }

    /// <summary>
    /// Gets or sets the extension type or category.
    /// </summary>
    [Required(ErrorMessage = "Extension type is required")]
    [MaxLength(20, ErrorMessage = "Extension type cannot exceed 20 characters")]
    [RegularExpression(@"^(Office|Department|Emergency|Maintenance|Security|Reception|Conference|Other)$",
        ErrorMessage = "Extension type must be one of: Office, Department, Emergency, Maintenance, Security, Reception, Conference, Other")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the extension is currently active and in service.
    /// </summary>
    [Required]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the extension supports voicemail.
    /// </summary>
    [Required]
    public bool HasVoicemail { get; set; } = true;

    /// <summary>
    /// Gets or sets whether calls to this extension can be forwarded.
    /// </summary>
    [Required]
    public bool AllowsForwarding { get; set; } = true;

    /// <summary>
    /// Gets or sets the priority level for emergency or important extensions.
    /// </summary>
    [Range(1, 5, ErrorMessage = "Priority level must be between 1 (highest) and 5 (lowest)")]
    public int? Priority { get; set; }

    /// <summary>
    /// Gets or sets the primary contact person for this extension.
    /// </summary>
    [MaxLength(100, ErrorMessage = "Primary contact name cannot exceed 100 characters")]
    public string? PrimaryContact { get; set; }

    /// <summary>
    /// Gets or sets the employee number of the person responsible for this extension.
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Employee number must be positive")]
    public int? ResponsibleEmployeeNr { get; set; }

    /// <summary>
    /// Gets or sets special instructions or notes for the extension.
    /// </summary>
    [MaxLength(300, ErrorMessage = "Special instructions cannot exceed 300 characters")]
    public string? SpecialInstructions { get; set; }

    /// <summary>
    /// Gets or sets the operating hours for the extension.
    /// </summary>
    [MaxLength(100, ErrorMessage = "Operating hours cannot exceed 100 characters")]
    public string? OperatingHours { get; set; }

    /// <summary>
    /// Gets or sets the date when the extension was installed or activated.
    /// </summary>
    [DataType(DataType.Date)]
    public DateTime? InstallationDate { get; set; }

    /// <summary>
    /// Gets or sets the last maintenance or service date.
    /// </summary>
    [DataType(DataType.Date)]
    public DateTime? LastServiceDate { get; set; }

    /// <summary>
    /// Navigation property to the responsible employee (Academic).
    /// </summary>
    public virtual Academic? ResponsibleEmployee { get; set; }
}