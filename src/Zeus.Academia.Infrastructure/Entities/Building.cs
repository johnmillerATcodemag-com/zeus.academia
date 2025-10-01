using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing a building or facility on campus.
/// Task 4: Infrastructure Entities - Facility management and location tracking.
/// </summary>
public class Building : BaseEntity
{
    /// <summary>
    /// Gets or sets the building code - the primary identifier.
    /// </summary>
    [Key]
    [Required(ErrorMessage = "Building code is required")]
    [MaxLength(10, ErrorMessage = "Building code cannot exceed 10 characters")]
    [RegularExpression(@"^[A-Z]{2,4}[0-9]{0,3}$", ErrorMessage = "Building code must be 2-4 uppercase letters optionally followed by up to 3 numbers (e.g., ENGR, SCI1, ADMIN)")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the building name.
    /// </summary>
    [Required(ErrorMessage = "Building name is required")]
    [MaxLength(100, ErrorMessage = "Building name cannot exceed 100 characters")]
    [MinLength(3, ErrorMessage = "Building name must be at least 3 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the building description or purpose.
    /// </summary>
    [MaxLength(500, ErrorMessage = "Building description cannot exceed 500 characters")]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the building address.
    /// </summary>
    [MaxLength(200, ErrorMessage = "Building address cannot exceed 200 characters")]
    public string? Address { get; set; }

    /// <summary>
    /// Gets or sets the number of floors in the building.
    /// </summary>
    [Range(1, 50, ErrorMessage = "Number of floors must be between 1 and 50")]
    public int? NumberOfFloors { get; set; }

    /// <summary>
    /// Gets or sets the year the building was constructed.
    /// </summary>
    [Range(1800, 2050, ErrorMessage = "Construction year must be between 1800 and 2050")]
    public int? ConstructionYear { get; set; }

    /// <summary>
    /// Gets or sets the total area of the building in square feet.
    /// </summary>
    [Range(0, 10000000, ErrorMessage = "Building area must be between 0 and 10,000,000 square feet")]
    [Column(TypeName = "decimal(10,2)")]
    public decimal? TotalAreaSqFt { get; set; }

    /// <summary>
    /// Gets or sets whether the building is currently active and in use.
    /// </summary>
    [Required]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the building has elevator access.
    /// </summary>
    [Required]
    public bool HasElevator { get; set; } = false;

    /// <summary>
    /// Gets or sets whether the building is ADA accessible.
    /// </summary>
    [Required]
    public bool IsAccessible { get; set; } = true;

    /// <summary>
    /// Gets or sets the building type or category.
    /// </summary>
    [MaxLength(30, ErrorMessage = "Building type cannot exceed 30 characters")]
    [RegularExpression(@"^(Academic|Administrative|Residential|Athletic|Library|Dining|Parking|Maintenance|Other)$",
        ErrorMessage = "Building type must be one of: Academic, Administrative, Residential, Athletic, Library, Dining, Parking, Maintenance, Other")]
    public string? BuildingType { get; set; }

    /// <summary>
    /// Gets or sets the main contact phone number for the building.
    /// </summary>
    [Phone(ErrorMessage = "Please enter a valid phone number")]
    [MaxLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets the building manager or supervisor.
    /// </summary>
    [MaxLength(100, ErrorMessage = "Building manager name cannot exceed 100 characters")]
    public string? BuildingManager { get; set; }

    /// <summary>
    /// Gets or sets emergency contact information for the building.
    /// </summary>
    [MaxLength(200, ErrorMessage = "Emergency contact cannot exceed 200 characters")]
    public string? EmergencyContact { get; set; }

    /// <summary>
    /// Navigation property for rooms in this building.
    /// </summary>
    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}