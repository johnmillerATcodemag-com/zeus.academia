using Zeus.Academia.Api.Models.Requests;

namespace Zeus.Academia.Api.Validation.Validators;

/// <summary>
/// Validator for CreateProfessorRequest with academic-specific validation rules.
/// </summary>
public class CreateProfessorRequestValidator : AbstractValidator<CreateProfessorRequest>
{
    public CreateProfessorRequestValidator()
    {
        // Employee Number validation
        RuleFor(nameof(CreateProfessorRequest.EmpNr),
            request => request.EmpNr > 0,
            "Employee number must be a positive integer");

        RuleFor(nameof(CreateProfessorRequest.EmpNr),
            request => request.EmpNr >= 100000 && request.EmpNr <= 999999,
            "Employee number must be a 6-digit number between 100000 and 999999");

        // Name validation
        NotEmpty(nameof(CreateProfessorRequest.Name),
            request => request.Name,
            "Professor name is required");

        MinLength(nameof(CreateProfessorRequest.Name),
            request => request.Name,
            2,
            "Professor name must be at least 2 characters long");

        MaxLength(nameof(CreateProfessorRequest.Name),
            request => request.Name,
            50,
            "Professor name cannot exceed 50 characters");

        // Phone number validation (if provided)
        RuleFor(nameof(CreateProfessorRequest.PhoneNumber),
            request => string.IsNullOrEmpty(request.PhoneNumber) || IsValidPhoneNumber(request.PhoneNumber),
            "Phone number must be in format XXX-XXX-XXXX or (XXX) XXX-XXXX");

        // Salary validation (if provided)
        RuleFor(nameof(CreateProfessorRequest.Salary),
            request => !request.Salary.HasValue || request.Salary.Value > 0,
            "Salary must be a positive amount");

        RuleFor(nameof(CreateProfessorRequest.Salary),
            request => !request.Salary.HasValue || request.Salary.Value <= 1000000,
            "Salary cannot exceed $1,000,000");

        // Rank code validation (if provided)
        RuleFor(nameof(CreateProfessorRequest.RankCode),
            request => string.IsNullOrEmpty(request.RankCode) || IsValidRankCode(request.RankCode),
            "Rank code must be a valid academic rank (e.g., ASSIST, ASSOC, FULL)");

        MaxLength(nameof(CreateProfessorRequest.RankCode),
            request => request.RankCode,
            10,
            "Rank code cannot exceed 10 characters");

        // Department name validation (if provided)
        MaxLength(nameof(CreateProfessorRequest.DepartmentName),
            request => request.DepartmentName,
            15,
            "Department name cannot exceed 15 characters");

        // Research area validation (if provided)
        MaxLength(nameof(CreateProfessorRequest.ResearchArea),
            request => request.ResearchArea,
            100,
            "Research area cannot exceed 100 characters");
    }

    private static bool IsValidPhoneNumber(string phoneNumber)
    {
        // Supports formats: XXX-XXX-XXXX, (XXX) XXX-XXXX, XXX.XXX.XXXX
        var phonePattern = @"^(\(\d{3}\)\s?|\d{3}[-.]?)\d{3}[-.]?\d{4}$";
        return System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, phonePattern);
    }

    private static bool IsValidRankCode(string rankCode)
    {
        // Common academic rank codes
        var validRanks = new[] { "ASSIST", "ASSOC", "FULL", "PROF", "ADJUNCT", "VISITING", "EMERITUS" };
        return validRanks.Contains(rankCode.ToUpper());
    }
}

/// <summary>
/// Validator for UpdateProfessorRequest with academic-specific validation rules.
/// </summary>
public class UpdateProfessorRequestValidator : AbstractValidator<UpdateProfessorRequest>
{
    public UpdateProfessorRequestValidator()
    {
        // Name validation
        NotEmpty(nameof(UpdateProfessorRequest.Name),
            request => request.Name,
            "Professor name is required");

        MinLength(nameof(UpdateProfessorRequest.Name),
            request => request.Name,
            2,
            "Professor name must be at least 2 characters long");

        MaxLength(nameof(UpdateProfessorRequest.Name),
            request => request.Name,
            50,
            "Professor name cannot exceed 50 characters");

        // Phone number validation (if provided)
        RuleFor(nameof(UpdateProfessorRequest.PhoneNumber),
            request => string.IsNullOrEmpty(request.PhoneNumber) || IsValidPhoneNumber(request.PhoneNumber),
            "Phone number must be in format XXX-XXX-XXXX or (XXX) XXX-XXXX");

        // Salary validation (if provided)
        RuleFor(nameof(UpdateProfessorRequest.Salary),
            request => !request.Salary.HasValue || request.Salary.Value > 0,
            "Salary must be a positive amount");

        RuleFor(nameof(UpdateProfessorRequest.Salary),
            request => !request.Salary.HasValue || request.Salary.Value <= 1000000,
            "Salary cannot exceed $1,000,000");

        // Rank code validation (if provided)
        RuleFor(nameof(UpdateProfessorRequest.RankCode),
            request => string.IsNullOrEmpty(request.RankCode) || IsValidRankCode(request.RankCode),
            "Rank code must be a valid academic rank (e.g., ASSIST, ASSOC, FULL)");

        MaxLength(nameof(UpdateProfessorRequest.RankCode),
            request => request.RankCode,
            10,
            "Rank code cannot exceed 10 characters");

        // Department name validation (if provided)
        MaxLength(nameof(UpdateProfessorRequest.DepartmentName),
            request => request.DepartmentName,
            15,
            "Department name cannot exceed 15 characters");

        // Research area validation (if provided)
        MaxLength(nameof(UpdateProfessorRequest.ResearchArea),
            request => request.ResearchArea,
            100,
            "Research area cannot exceed 100 characters");
    }

    private static bool IsValidPhoneNumber(string phoneNumber)
    {
        var phonePattern = @"^(\(\d{3}\)\s?|\d{3}[-.]?)\d{3}[-.]?\d{4}$";
        return System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, phonePattern);
    }

    private static bool IsValidRankCode(string rankCode)
    {
        var validRanks = new[] { "ASSIST", "ASSOC", "FULL", "PROF", "ADJUNCT", "VISITING", "EMERITUS" };
        return validRanks.Contains(rankCode.ToUpper());
    }
}