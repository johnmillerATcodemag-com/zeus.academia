using Zeus.Academia.Api.Models.Requests;
using Zeus.Academia.Api.Validation;

namespace Zeus.Academia.Api.Validation.Validators;

/// <summary>
/// Validator for CreateStudentRequest with academic-specific validation rules.
/// </summary>
public class CreateStudentRequestValidator : AbstractValidator<CreateStudentRequest>
{
    public CreateStudentRequestValidator()
    {
        // Student ID validation
        NotEmpty(nameof(CreateStudentRequest.StudentId),
            request => request.StudentId,
            "Student ID is required");

        MinLength(nameof(CreateStudentRequest.StudentId),
            request => request.StudentId,
            3,
            "Student ID must be at least 3 characters long");

        MaxLength(nameof(CreateStudentRequest.StudentId),
            request => request.StudentId,
            20,
            "Student ID cannot exceed 20 characters");

        // Name validation
        NotEmpty(nameof(CreateStudentRequest.Name),
            request => request.Name,
            "Student name is required");

        MinLength(nameof(CreateStudentRequest.Name),
            request => request.Name,
            2,
            "Student name must be at least 2 characters long");

        MaxLength(nameof(CreateStudentRequest.Name),
            request => request.Name,
            50,
            "Student name cannot exceed 50 characters");

        // Phone number validation (if provided)
        RuleFor(nameof(CreateStudentRequest.PhoneNumber),
            request => string.IsNullOrEmpty(request.PhoneNumber) || IsValidPhoneNumber(request.PhoneNumber),
            "Phone number must be in format XXX-XXX-XXXX or (XXX) XXX-XXXX");

        // Class validation (if provided)
        RuleFor(nameof(CreateStudentRequest.Class),
            request => string.IsNullOrEmpty(request.Class) || IsValidClassLevel(request.Class),
            "Class must be a valid level (Freshman, Sophomore, Junior, Senior, Graduate)");

        MaxLength(nameof(CreateStudentRequest.Class),
            request => request.Class,
            20,
            "Class cannot exceed 20 characters");

        // GPA validation (if provided)
        RuleFor(nameof(CreateStudentRequest.GPA),
            request => !request.GPA.HasValue || (request.GPA.Value >= 0.0m && request.GPA.Value <= 4.0m),
            "GPA must be between 0.0 and 4.0");

        // Department name validation (if provided)
        MaxLength(nameof(CreateStudentRequest.DepartmentName),
            request => request.DepartmentName,
            15,
            "Department name cannot exceed 15 characters");
    }

    private static bool IsValidPhoneNumber(string phoneNumber)
    {
        // Supports formats: XXX-XXX-XXXX, (XXX) XXX-XXXX, XXX.XXX.XXXX
        var phonePattern = @"^(\(\d{3}\)\s?|\d{3}[-.]?)\d{3}[-.]?\d{4}$";
        return System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, phonePattern);
    }

    private static bool IsValidClassLevel(string classLevel)
    {
        var validLevels = new[] { "FRESHMAN", "SOPHOMORE", "JUNIOR", "SENIOR", "GRADUATE", "UNDERGRADUATE", "POSTGRADUATE" };
        return validLevels.Contains(classLevel.ToUpper());
    }
}

/// <summary>
/// Validator for UpdateStudentRequest with academic-specific validation rules.
/// </summary>
public class UpdateStudentRequestValidator : AbstractValidator<UpdateStudentRequest>
{
    public UpdateStudentRequestValidator()
    {
        // Name validation
        NotEmpty(nameof(UpdateStudentRequest.Name),
            request => request.Name,
            "Student name is required");

        MinLength(nameof(UpdateStudentRequest.Name),
            request => request.Name,
            2,
            "Student name must be at least 2 characters long");

        MaxLength(nameof(UpdateStudentRequest.Name),
            request => request.Name,
            50,
            "Student name cannot exceed 50 characters");

        // Phone number validation (if provided)
        RuleFor(nameof(UpdateStudentRequest.PhoneNumber),
            request => string.IsNullOrEmpty(request.PhoneNumber) || IsValidPhoneNumber(request.PhoneNumber),
            "Phone number must be in format XXX-XXX-XXXX or (XXX) XXX-XXXX");

        // Class validation (if provided)
        RuleFor(nameof(UpdateStudentRequest.Class),
            request => string.IsNullOrEmpty(request.Class) || IsValidClassLevel(request.Class),
            "Class must be a valid level (Freshman, Sophomore, Junior, Senior, Graduate)");

        MaxLength(nameof(UpdateStudentRequest.Class),
            request => request.Class,
            20,
            "Class cannot exceed 20 characters");

        // GPA validation (if provided)
        RuleFor(nameof(UpdateStudentRequest.GPA),
            request => !request.GPA.HasValue || (request.GPA.Value >= 0.0m && request.GPA.Value <= 4.0m),
            "GPA must be between 0.0 and 4.0");

        // Department name validation (if provided)
        MaxLength(nameof(UpdateStudentRequest.DepartmentName),
            request => request.DepartmentName,
            15,
            "Department name cannot exceed 15 characters");
    }

    private static bool IsValidPhoneNumber(string phoneNumber)
    {
        var phonePattern = @"^(\(\d{3}\)\s?|\d{3}[-.]?)\d{3}[-.]?\d{4}$";
        return System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, phonePattern);
    }

    private static bool IsValidClassLevel(string classLevel)
    {
        var validLevels = new[] { "FRESHMAN", "SOPHOMORE", "JUNIOR", "SENIOR", "GRADUATE", "UNDERGRADUATE", "POSTGRADUATE" };
        return validLevels.Contains(classLevel.ToUpper());
    }
}