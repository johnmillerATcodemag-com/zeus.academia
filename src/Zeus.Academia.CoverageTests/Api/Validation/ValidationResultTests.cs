using Xunit;
using Zeus.Academia.Api.Validation;

namespace Zeus.Academia.CoverageTests.Api.Validation;

/// <summary>
/// Comprehensive unit tests for ValidationResult and ValidationError classes.
/// </summary>
public class ValidationResultTests
{
    [Fact]
    public void ValidationResult_Success_ReturnsValidResult()
    {
        // Act
        var result = ValidationResult.Success();

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void ValidationResult_FailureWithSingleError_ReturnsInvalidResult()
    {
        // Arrange
        var error = new ValidationError("Name", "Name is required");

        // Act
        var result = ValidationResult.Failure(error);

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Name", result.Errors[0].PropertyName);
        Assert.Equal("Name is required", result.Errors[0].ErrorMessage);
    }

    [Fact]
    public void ValidationResult_FailureWithMultipleErrors_ReturnsInvalidResult()
    {
        // Arrange
        var errors = new[]
        {
            new ValidationError("Name", "Name is required"),
            new ValidationError("Email", "Email format is invalid")
        };

        // Act
        var result = ValidationResult.Failure(errors);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(2, result.Errors.Count);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
        Assert.Contains(result.Errors, e => e.PropertyName == "Email");
    }

    [Fact]
    public void ValidationResult_FailureWithPropertyAndMessage_ReturnsInvalidResult()
    {
        // Act
        var result = ValidationResult.Failure("Age", "Age must be positive");

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Age", result.Errors[0].PropertyName);
        Assert.Equal("Age must be positive", result.Errors[0].ErrorMessage);
    }

    [Fact]
    public void AddError_WithValidationError_AddsErrorToResult()
    {
        // Arrange
        var result = ValidationResult.Success();
        var error = new ValidationError("Username", "Username already exists");

        // Act
        result.AddError(error);

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Username", result.Errors[0].PropertyName);
    }

    [Fact]
    public void AddError_WithPropertyAndMessage_AddsErrorToResult()
    {
        // Arrange
        var result = ValidationResult.Success();

        // Act
        result.AddError("Password", "Password is too weak");

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Password", result.Errors[0].PropertyName);
        Assert.Equal("Password is too weak", result.Errors[0].ErrorMessage);
    }

    [Fact]
    public void Combine_WithTwoResults_CombinesAllErrors()
    {
        // Arrange
        var result1 = ValidationResult.Failure("Name", "Name is required");
        var result2 = ValidationResult.Failure("Email", "Email is invalid");

        // Act
        var combined = result1.Combine(result2);

        // Assert
        Assert.False(combined.IsValid);
        Assert.Equal(2, combined.Errors.Count);
        Assert.Contains(combined.Errors, e => e.PropertyName == "Name");
        Assert.Contains(combined.Errors, e => e.PropertyName == "Email");
    }

    [Fact]
    public void ToException_WithValidationErrors_CreatesValidationException()
    {
        // Arrange
        var result = ValidationResult.Failure(new[]
        {
            new ValidationError("Name", "Name is required"),
            new ValidationError("Email", "Email format is invalid")
        });

        // Act
        var exception = result.ToException();

        // Assert
        Assert.NotNull(exception);
        Assert.Equal("One or more validation errors occurred.", exception.Message);
        Assert.Equal(2, exception.ValidationErrors.Count);
        Assert.True(exception.ValidationErrors.ContainsKey("Name"));
        Assert.True(exception.ValidationErrors.ContainsKey("Email"));
        Assert.Contains("Name is required", exception.ValidationErrors["Name"]);
        Assert.Contains("Email format is invalid", exception.ValidationErrors["Email"]);
    }

    [Fact]
    public void ToException_WithCustomMessage_CreatesValidationExceptionWithCustomMessage()
    {
        // Arrange
        var result = ValidationResult.Failure("Age", "Age is invalid");
        var customMessage = "Custom validation failed";

        // Act
        var exception = result.ToException(customMessage);

        // Assert
        Assert.Equal(customMessage, exception.Message);
    }
}

/// <summary>
/// Unit tests for ValidationError class.
/// </summary>
public class ValidationErrorTests
{
    [Fact]
    public void ValidationError_WithPropertyAndMessage_InitializesCorrectly()
    {
        // Arrange
        var propertyName = "TestProperty";
        var errorMessage = "Test error message";

        // Act
        var error = new ValidationError(propertyName, errorMessage);

        // Assert
        Assert.Equal(propertyName, error.PropertyName);
        Assert.Equal(errorMessage, error.ErrorMessage);
        Assert.Null(error.AttemptedValue);
    }

    [Fact]
    public void ValidationError_WithAttemptedValue_InitializesCorrectly()
    {
        // Arrange
        var propertyName = "Age";
        var errorMessage = "Age must be positive";
        var attemptedValue = -5;

        // Act
        var error = new ValidationError(propertyName, errorMessage, attemptedValue);

        // Assert
        Assert.Equal(propertyName, error.PropertyName);
        Assert.Equal(errorMessage, error.ErrorMessage);
        Assert.Equal(attemptedValue, error.AttemptedValue);
    }

    [Fact]
    public void ValidationError_WithNullPropertyName_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ValidationError(null!, "message"));
    }

    [Fact]
    public void ValidationError_WithNullErrorMessage_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ValidationError("property", null!));
    }

    [Fact]
    public void ToString_ReturnsFormattedString()
    {
        // Arrange
        var error = new ValidationError("Email", "Invalid email format");

        // Act
        var result = error.ToString();

        // Assert
        Assert.Equal("Email: Invalid email format", result);
    }

    [Theory]
    [InlineData("Name", "Name is required")]
    [InlineData("Email", "Email format is invalid")]
    [InlineData("Age", "Age must be between 0 and 120")]
    public void ValidationError_WithVariousInputs_InitializesCorrectly(string propertyName, string errorMessage)
    {
        // Act
        var error = new ValidationError(propertyName, errorMessage);

        // Assert
        Assert.Equal(propertyName, error.PropertyName);
        Assert.Equal(errorMessage, error.ErrorMessage);
    }
}