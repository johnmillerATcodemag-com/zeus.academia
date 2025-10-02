using System.ComponentModel.DataAnnotations;

namespace Zeus.Academia.Api.Models.Requests;

/// <summary>
/// Request model for creating a new student.
/// </summary>
public class CreateStudentRequest
{
    /// <summary>
    /// The student ID number - must be unique and positive.
    /// </summary>
    [Required]
    public int StudentId { get; set; }

    /// <summary>
    /// The student's name - required and must be between 2-50 characters.
    /// </summary>
    [Required]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional phone number - must be in valid format if provided.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// The student's class or year level.
    /// </summary>
    public string? Class { get; set; }

    /// <summary>
    /// The student's grade point average.
    /// </summary>
    public decimal? GPA { get; set; }

    /// <summary>
    /// The name of the department the student belongs to.
    /// </summary>
    public string? DepartmentName { get; set; }
}

/// <summary>
/// Request model for updating an existing student.
/// </summary>
public class UpdateStudentRequest
{
    /// <summary>
    /// The student's name - required and must be between 2-50 characters.
    /// </summary>
    [Required]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional phone number - must be in valid format if provided.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// The student's class or year level.
    /// </summary>
    public string? Class { get; set; }

    /// <summary>
    /// The student's grade point average.
    /// </summary>
    public decimal? GPA { get; set; }

    /// <summary>
    /// The name of the department the student belongs to.
    /// </summary>
    public string? DepartmentName { get; set; }
}