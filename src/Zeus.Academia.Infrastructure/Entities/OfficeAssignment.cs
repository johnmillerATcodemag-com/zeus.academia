using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zeus.Academia.Infrastructure.Entities;

public class OfficeAssignment
{
    [Key]
    public int OfficeAssignmentId { get; set; }

    [Required]
    public int AcademicId { get; set; }

    [ForeignKey(nameof(AcademicId))]
    public Academic Academic { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string BuildingName { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string RoomNumber { get; set; } = string.Empty;

    [StringLength(50)]
    public string? Floor { get; set; }

    [StringLength(50)]
    public string? Wing { get; set; }

    [StringLength(20)]
    public string? PhoneExtension { get; set; }

    [StringLength(15)]
    public string? DirectPhoneNumber { get; set; }

    [Column(TypeName = "decimal(10,8)")]
    public decimal? Latitude { get; set; }

    [Column(TypeName = "decimal(11,8)")]
    public decimal? Longitude { get; set; }

    [Column(TypeName = "decimal(8,2)")]
    public decimal? OfficeSize { get; set; } // Square footage

    public int? MaxOccupancy { get; set; }

    [StringLength(50)]
    public string? OfficeType { get; set; } // Private, Shared, Cubicle, Suite

    [StringLength(1000)]
    public string? OfficeFeatures { get; set; } // Window, Conference Table, Whiteboard, etc.

    [StringLength(100)]
    public string? AccessibilityFeatures { get; set; }

    [StringLength(200)]
    public string? ParkingSpot { get; set; }

    [StringLength(50)]
    public string? KeyCode { get; set; }

    [StringLength(100)]
    public string? SecurityAccess { get; set; }

    public bool HasWindowView { get; set; }

    public bool IsSharedOffice { get; set; }

    public bool HasConferenceCapability { get; set; }

    public bool IsAccessible { get; set; }

    [Column(TypeName = "date")]
    public DateTime AssignmentStartDate { get; set; }

    [Column(TypeName = "date")]
    public DateTime? AssignmentEndDate { get; set; }

    [StringLength(50)]
    public string AssignmentStatus { get; set; } = "Active"; // Active, Temporary, Inactive

    [StringLength(500)]
    public string? AssignmentReason { get; set; }

    [StringLength(200)]
    public string? EmergencyContactInstructions { get; set; }

    [StringLength(100)]
    public string? DepartmentCode { get; set; }

    [StringLength(100)]
    public string? CostCenter { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    // Audit fields
    public DateTime CreatedDate { get; set; }
    public DateTime LastModifiedDate { get; set; }

    [StringLength(100)]
    public string CreatedBy { get; set; } = string.Empty;

    [StringLength(100)]
    public string LastModifiedBy { get; set; } = string.Empty;

    // Navigation properties
    public FacultyProfile? FacultyProfile { get; set; }

    // Computed properties
    [NotMapped]
    public string FullOfficeLocation => $"{BuildingName} {RoomNumber}";

    [NotMapped]
    public bool IsCurrentAssignment => AssignmentStatus == "Active" &&
                                      (AssignmentEndDate == null || AssignmentEndDate > DateTime.Today);

    [NotMapped]
    public int AssignmentDurationDays => AssignmentEndDate.HasValue
        ? (AssignmentEndDate.Value - AssignmentStartDate).Days
        : (DateTime.Today - AssignmentStartDate).Days;
}