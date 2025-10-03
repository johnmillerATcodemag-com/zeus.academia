using Xunit;
using Zeus.Academia.Api.Models.Requests;
using Zeus.Academia.Api.Validation.Validators;

namespace Zeus.Academia.CoverageTests.Api.Validation.Validators;

/// <summary>
/// Debug test to understand validation errors.
/// </summary>
public class ValidationDebugTests
{
    [Fact]
    public void Debug_CreateProfessorRequestValidator_ShowErrors()
    {
        // Arrange
        var validator = new CreateProfessorRequestValidator();
        var request = new CreateProfessorRequest
        {
            EmpNr = 123456,
            Name = "Dr. John Smith",
            PhoneNumber = "555-123-4567",
            RankCode = "ASSOC",
            Salary = 85000,
            ResearchArea = "Computer Science, AI"
        };

        // Act
        var result = validator.Validate(request);

        // Assert - Debug output
        if (!result.IsValid)
        {
            var errorMessages = string.Join(", ", result.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
            throw new Exception($"Validation failed with errors: {errorMessages}");
        }

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Debug_CreateStudentRequestValidator_ShowErrors()
    {
        // Arrange
        var validator = new CreateStudentRequestValidator();
        var request = new CreateStudentRequest
        {
            StudentId = "1234567",
            Name = "Alice Johnson",
            PhoneNumber = "555-111-2222",
            Class = "Senior",
            GPA = 3.75m,
            DepartmentName = "CS"
        };

        // Act
        var result = validator.Validate(request);

        // Assert - Debug output
        if (!result.IsValid)
        {
            var errorMessages = string.Join(", ", result.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
            throw new Exception($"Validation failed with errors: {errorMessages}");
        }

        Assert.True(result.IsValid);
    }
}