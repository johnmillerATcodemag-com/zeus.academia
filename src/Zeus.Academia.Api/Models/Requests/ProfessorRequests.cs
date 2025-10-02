using System.ComponentModel.DataAnnotations;

namespace Zeus.Academia.Api.Models.Requests;

/// <summary>
/// Request model for creating a new professor.
/// </summary>
public class CreateProfessorRequest
{
    /// <summary>
    /// The employee number - must be unique and positive.
    /// </summary>
    [Required]
    public int EmpNr { get; set; }

    /// <summary>
    /// The professor's name - required and must be between 2-50 characters.
    /// </summary>
    [Required]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional phone number - must be in valid format if provided.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Optional salary - must be positive if provided.
    /// </summary>
    public decimal? Salary { get; set; }

    /// <summary>
    /// The academic rank code of the professor.
    /// </summary>
    public string? RankCode { get; set; }

    /// <summary>
    /// The department name where the professor works.
    /// </summary>
    public string? DepartmentName { get; set; }

    /// <summary>
    /// Whether the professor has tenure.
    /// </summary>
    public bool? HasTenure { get; set; }

    /// <summary>
    /// The professor's research area or specialty.
    /// </summary>
    public string? ResearchArea { get; set; }
}

/// <summary>
/// Request model for updating an existing professor.
/// </summary>
public class UpdateProfessorRequest
{
    /// <summary>
    /// The professor's name - required and must be between 2-50 characters.
    /// </summary>
    [Required]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional phone number - must be in valid format if provided.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Optional salary - must be positive if provided.
    /// </summary>
    public decimal? Salary { get; set; }

    /// <summary>
    /// The academic rank code of the professor.
    /// </summary>
    public string? RankCode { get; set; }

    /// <summary>
    /// The department name where the professor works.
    /// </summary>
    public string? DepartmentName { get; set; }

    /// <summary>
    /// Whether the professor has tenure.
    /// </summary>
    public bool? HasTenure { get; set; }

    /// <summary>
    /// The professor's research area or specialty.
    /// </summary>
    public string? ResearchArea { get; set; }
}