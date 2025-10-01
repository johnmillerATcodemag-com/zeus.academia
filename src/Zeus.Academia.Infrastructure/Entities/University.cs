using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing a university or educational institution.
/// Enhanced for Task 3: Academic Structure Entities with comprehensive validation and institutional management features.
/// </summary>
public class University : BaseEntity
{
    /// <summary>
    /// Gets or sets the university code - the primary identifier.
    /// </summary>
    [Key]
    [Required(ErrorMessage = "University code is required")]
    [MaxLength(10, ErrorMessage = "University code cannot exceed 10 characters")]
    [RegularExpression(@"^[A-Z]{2,6}[0-9]{0,4}$", ErrorMessage = "University code must be 2-6 uppercase letters optionally followed by up to 4 numbers (e.g., MIT, CALTECH, HARVRD, UCLA1)")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the university name.
    /// </summary>
    [Required(ErrorMessage = "University name is required")]
    [MaxLength(100, ErrorMessage = "University name cannot exceed 100 characters")]
    [MinLength(5, ErrorMessage = "University name must be at least 5 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the university location/address.
    /// </summary>
    [MaxLength(200, ErrorMessage = "University location cannot exceed 200 characters")]
    public string? Location { get; set; }

    /// <summary>
    /// Gets or sets the university website URL.
    /// </summary>
    [MaxLength(100, ErrorMessage = "Website URL cannot exceed 100 characters")]
    [Url(ErrorMessage = "Please enter a valid URL")]
    public string? Website { get; set; }

    /// <summary>
    /// Gets or sets the accreditation status.
    /// </summary>
    [MaxLength(50, ErrorMessage = "Accreditation status cannot exceed 50 characters")]
    [RegularExpression(@"^(Accredited|Provisional|Candidate|Not Accredited|Under Review)$", ErrorMessage = "Accreditation status must be one of: Accredited, Provisional, Candidate, Not Accredited, Under Review")]
    public string? AccreditationStatus { get; set; }

    /// <summary>
    /// Gets or sets the year the university was established.
    /// </summary>
    [Range(1000, 2030, ErrorMessage = "Established year must be between 1000 and 2030")]
    public int? EstablishedYear { get; set; }

    /// <summary>
    /// Gets or sets the university type (Public, Private, For-Profit, etc.).
    /// </summary>
    [MaxLength(20, ErrorMessage = "University type cannot exceed 20 characters")]
    [RegularExpression(@"^(Public|Private|For-Profit|Community|Technical|Online)$", ErrorMessage = "University type must be one of: Public, Private, For-Profit, Community, Technical, Online")]
    public string? UniversityType { get; set; }

    /// <summary>
    /// Gets or sets the country where the university is located.
    /// </summary>
    [MaxLength(50, ErrorMessage = "Country cannot exceed 50 characters")]
    public string? Country { get; set; }

    /// <summary>
    /// Gets or sets the state or province where the university is located.
    /// </summary>
    [MaxLength(50, ErrorMessage = "State/Province cannot exceed 50 characters")]
    public string? StateProvince { get; set; }

    /// <summary>
    /// Gets or sets the city where the university is located.
    /// </summary>
    [MaxLength(50, ErrorMessage = "City cannot exceed 50 characters")]
    public string? City { get; set; }

    /// <summary>
    /// Gets or sets the postal code of the university.
    /// </summary>
    [MaxLength(10, ErrorMessage = "Postal code cannot exceed 10 characters")]
    public string? PostalCode { get; set; }

    /// <summary>
    /// Gets or sets the main contact phone number.
    /// </summary>
    [Phone(ErrorMessage = "Please enter a valid phone number")]
    [MaxLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets the main contact email address.
    /// </summary>
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    [MaxLength(100, ErrorMessage = "Email address cannot exceed 100 characters")]
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets whether the university is currently active.
    /// </summary>
    [Required]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the current student enrollment count.
    /// </summary>
    [Range(0, 1000000, ErrorMessage = "Student enrollment must be between 0 and 1,000,000")]
    public int? StudentEnrollment { get; set; }

    /// <summary>
    /// Navigation property for academics with degrees from this university.
    /// </summary>
    public virtual ICollection<AcademicDegree> AcademicDegrees { get; set; } = new List<AcademicDegree>();
}