using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing a room or space within a building.
/// Task 4: Infrastructure Entities - Room management with capacity and equipment tracking.
/// </summary>
public class Room : BaseEntity
{
    /// <summary>
    /// Gets or sets the room number - part of composite primary key.
    /// </summary>
    [Required(ErrorMessage = "Room number is required")]
    [MaxLength(10, ErrorMessage = "Room number cannot exceed 10 characters")]
    [RegularExpression(@"^[A-Z0-9]{1,10}$", ErrorMessage = "Room number must contain only letters and numbers (e.g., 101, A205, LAB1)")]
    public string Number { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the building code - part of composite primary key and foreign key.
    /// </summary>
    [Required(ErrorMessage = "Building code is required")]
    [MaxLength(10, ErrorMessage = "Building code cannot exceed 10 characters")]
    public string BuildingCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the room name or title.
    /// </summary>
    [MaxLength(100, ErrorMessage = "Room name cannot exceed 100 characters")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the room type or purpose.
    /// </summary>
    [Required(ErrorMessage = "Room type is required")]
    [MaxLength(30, ErrorMessage = "Room type cannot exceed 30 characters")]
    [RegularExpression(@"^(Classroom|Laboratory|Office|Conference|Auditorium|Library|Study|Administrative|Storage|Restroom|Other)$",
        ErrorMessage = "Room type must be one of: Classroom, Laboratory, Office, Conference, Auditorium, Library, Study, Administrative, Storage, Restroom, Other")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the maximum capacity of the room.
    /// </summary>
    [Range(1, 1000, ErrorMessage = "Room capacity must be between 1 and 1000")]
    public int? Capacity { get; set; }

    /// <summary>
    /// Gets or sets the floor number where the room is located.
    /// </summary>
    [Range(0, 50, ErrorMessage = "Floor number must be between 0 and 50")]
    public int? FloorNumber { get; set; }

    /// <summary>
    /// Gets or sets the room area in square feet.
    /// </summary>
    [Range(0, 50000, ErrorMessage = "Room area must be between 0 and 50,000 square feet")]
    [Column(TypeName = "decimal(8,2)")]
    public decimal? AreaSqFt { get; set; }

    /// <summary>
    /// Gets or sets whether the room is currently active and available for use.
    /// </summary>
    [Required]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the room has AV equipment.
    /// </summary>
    [Required]
    public bool HasAVEquipment { get; set; } = false;

    /// <summary>
    /// Gets or sets whether the room has computer/internet access.
    /// </summary>
    [Required]
    public bool HasComputerAccess { get; set; } = false;

    /// <summary>
    /// Gets or sets whether the room has a projector.
    /// </summary>
    [Required]
    public bool HasProjector { get; set; } = false;

    /// <summary>
    /// Gets or sets whether the room has a whiteboard/blackboard.
    /// </summary>
    [Required]
    public bool HasWhiteboard { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the room is accessible for individuals with disabilities.
    /// </summary>
    [Required]
    public bool IsAccessible { get; set; } = true;

    /// <summary>
    /// Gets or sets special equipment or features in the room.
    /// </summary>
    [MaxLength(500, ErrorMessage = "Equipment description cannot exceed 500 characters")]
    public string? SpecialEquipment { get; set; }

    /// <summary>
    /// Gets or sets the room maintenance notes.
    /// </summary>
    [MaxLength(500, ErrorMessage = "Maintenance notes cannot exceed 500 characters")]
    public string? MaintenanceNotes { get; set; }

    /// <summary>
    /// Gets or sets the last maintenance date.
    /// </summary>
    [DataType(DataType.Date)]
    public DateTime? LastMaintenanceDate { get; set; }

    /// <summary>
    /// Gets or sets the room booking or scheduling notes.
    /// </summary>
    [MaxLength(300, ErrorMessage = "Booking notes cannot exceed 300 characters")]
    public string? BookingNotes { get; set; }

    /// <summary>
    /// Navigation property to the building.
    /// </summary>
    public virtual Building? Building { get; set; }
}