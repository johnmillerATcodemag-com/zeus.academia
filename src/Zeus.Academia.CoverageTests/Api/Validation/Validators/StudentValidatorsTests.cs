using Xunit;
using Zeus.Academia.Api.Models.Requests;
using Zeus.Academia.Api.Validation.Validators;

namespace Zeus.Academia.CoverageTests.Api.Validation.Validators;

/// <summary>
/// Comprehensive unit tests for Student validators.
/// </summary>
public class StudentValidatorsTests
{
    [Fact]
    public void CreateStudentRequestValidator_WithValidRequest_ReturnsSuccess()
    {
        // Arrange
        var validator = new CreateStudentRequestValidator();
        var request = new CreateStudentRequest
        {
            StudentId = 1234567,
            Name = "Alice Johnson",
            PhoneNumber = "555-111-2222",
            Class = "Senior",
            GPA = 3.75m,
            DepartmentName = "CS"
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CreateStudentRequestValidator_WithInvalidStudentId_ReturnsValidationError(int invalidStudentId)
    {
        // Arrange
        var validator = new CreateStudentRequestValidator();
        var request = new CreateStudentRequest
        {
            StudentId = invalidStudentId,
            Name = "Alice Johnson",
            PhoneNumber = "555-111-2222",
            GPA = 3.75m
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateStudentRequest.StudentId));
    }

    [Theory]
    [InlineData("")]
    [InlineData("A")] // Too short
    public void CreateStudentRequestValidator_WithInvalidName_ReturnsValidationError(string invalidName)
    {
        // Arrange
        var validator = new CreateStudentRequestValidator();
        var request = new CreateStudentRequest
        {
            StudentId = 123456,
            Name = invalidName,
            PhoneNumber = "555-111-2222",
            GPA = 3.75m
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateStudentRequest.Name));
    }

    [Theory]
    [InlineData("123")]
    [InlineData("invalid-phone")]
    [InlineData("123-456-7890-1234-5678")] // Too long
    public void CreateStudentRequestValidator_WithInvalidPhoneNumber_ReturnsValidationError(string invalidPhone)
    {
        // Arrange
        var validator = new CreateStudentRequestValidator();
        var request = new CreateStudentRequest
        {
            StudentId = 123456,
            Name = "Alice Johnson",
            PhoneNumber = invalidPhone,
            GPA = 3.75m
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateStudentRequest.PhoneNumber));
    }

    [Theory]
    [InlineData("555-123-4567")]
    [InlineData("(555) 123-4567")]
    [InlineData("555.123.4567")]
    public void CreateStudentRequestValidator_WithValidPhoneNumber_PassesValidation(string validPhone)
    {
        // Arrange
        var validator = new CreateStudentRequestValidator();
        var request = new CreateStudentRequest
        {
            StudentId = 1234567,
            Name = "Alice Johnson",
            PhoneNumber = validPhone,
            Class = "Junior",
            GPA = 3.75m
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(4.1)]
    [InlineData(10.0)]
    public void CreateStudentRequestValidator_WithInvalidGPA_ReturnsValidationError(decimal invalidGPA)
    {
        // Arrange
        var validator = new CreateStudentRequestValidator();
        var request = new CreateStudentRequest
        {
            StudentId = 123456,
            Name = "Alice Johnson",
            PhoneNumber = "555-111-2222",
            GPA = invalidGPA
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateStudentRequest.GPA));
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(2.5)]
    [InlineData(3.75)]
    [InlineData(4.0)]
    public void CreateStudentRequestValidator_WithValidGPA_PassesValidation(decimal validGPA)
    {
        // Arrange
        var validator = new CreateStudentRequestValidator();
        var request = new CreateStudentRequest
        {
            StudentId = 1234567,
            Name = "Alice Johnson",
            PhoneNumber = "555-111-2222",
            Class = "Sophomore",
            GPA = validGPA
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void UpdateStudentRequestValidator_WithValidRequest_ReturnsSuccess()
    {
        // Arrange
        var validator = new UpdateStudentRequestValidator();
        var request = new UpdateStudentRequest
        {
            Name = "Bob Wilson",
            PhoneNumber = "555-333-4444",
            GPA = 3.25m,
            Class = "Junior",
            DepartmentName = "CS"
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void UpdateStudentRequestValidator_WithInvalidOptionalFields_ReturnsValidationErrors()
    {
        // Arrange
        var validator = new UpdateStudentRequestValidator();
        var request = new UpdateStudentRequest
        {
            Name = "A", // Too short
            PhoneNumber = "123", // Invalid format
            GPA = 5.0m // Invalid GPA
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateStudentRequest.Name));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateStudentRequest.PhoneNumber));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateStudentRequest.GPA));
    }

    [Fact]
    public void CreateStudentRequestValidator_WithMultipleErrors_ReturnsAllErrors()
    {
        // Arrange
        var validator = new CreateStudentRequestValidator();
        var request = new CreateStudentRequest
        {
            StudentId = -1, // Invalid
            Name = "", // Invalid - empty
            PhoneNumber = "123", // Invalid format
            GPA = 5.0m // Invalid - too high
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.Errors.Count >= 4); // Should have multiple errors

        // Verify specific errors
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateStudentRequest.StudentId));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateStudentRequest.Name));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateStudentRequest.PhoneNumber));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateStudentRequest.GPA));
    }

    [Fact]
    public async Task CreateStudentRequestValidator_ValidateAsync_WithValidRequest_ReturnsSuccess()
    {
        // Arrange
        var validator = new CreateStudentRequestValidator();
        var request = new CreateStudentRequest
        {
            StudentId = 7890123,
            Name = "Carol Davis",
            PhoneNumber = "555-555-5555",
            GPA = 3.9m,
            Class = "Graduate",
            DepartmentName = "Math"
        };

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task UpdateStudentRequestValidator_ValidateAsync_WithValidRequest_ReturnsSuccess()
    {
        // Arrange
        var validator = new UpdateStudentRequestValidator();
        var request = new UpdateStudentRequest
        {
            Name = "David Miller",
            GPA = 3.6m,
            Class = "Senior",
            DepartmentName = "Physics"
        };

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void CreateStudentRequestValidator_WithValidOptionalFields_PassesValidation()
    {
        // Arrange
        var validator = new CreateStudentRequestValidator();
        var request = new CreateStudentRequest
        {
            StudentId = 1234567,
            Name = "Alice Johnson",
            PhoneNumber = "555-111-2222",
            GPA = 3.75m,
            Class = null, // Optional
            DepartmentName = null // Optional
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
}
