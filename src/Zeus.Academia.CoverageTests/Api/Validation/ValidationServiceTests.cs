using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Zeus.Academia.Api.Models.Requests;
using Zeus.Academia.Api.Validation;
using Zeus.Academia.Api.Validation.Validators;

namespace Zeus.Academia.CoverageTests.Api.Validation;

/// <summary>
/// Comprehensive unit tests for ValidationService.
/// </summary>
public class ValidationServiceTests
{
    private readonly ServiceProvider _serviceProvider;
    private readonly ValidationService _validationService;

    public ValidationServiceTests()
    {
        var services = new ServiceCollection();

        // Register validators
        services.AddTransient<IValidator<CreateProfessorRequest>, CreateProfessorRequestValidator>();
        services.AddTransient<IValidator<UpdateProfessorRequest>, UpdateProfessorRequestValidator>();
        services.AddTransient<IValidator<CreateStudentRequest>, CreateStudentRequestValidator>();
        services.AddTransient<IValidator<UpdateStudentRequest>, UpdateStudentRequestValidator>();

        // Register validation service
        services.AddTransient<ValidationService>();

        _serviceProvider = services.BuildServiceProvider();
        _validationService = _serviceProvider.GetRequiredService<ValidationService>();
    }

    [Fact]
    public void Validate_WithValidCreateProfessorRequest_ReturnsSuccess()
    {
        // Arrange
        var request = new CreateProfessorRequest
        {
            EmpNr = 123456,
            Name = "Dr. John Smith",
            PhoneNumber = "555-123-4567",
            RankCode = "ASSOC",
            Salary = 85000
        };

        // Act
        var result = _validationService.Validate(request);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_WithInvalidCreateProfessorRequest_ReturnsValidationErrors()
    {
        // Arrange
        var request = new CreateProfessorRequest
        {
            EmpNr = 12345, // Invalid - should be 6 digits
            Name = "", // Invalid - required
            PhoneNumber = "123", // Invalid format
            RankCode = "INVALID", // Invalid rank
            Salary = -1000 // Invalid - negative
        };

        // Act
        var result = _validationService.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.Errors.Count >= 5); // Should have multiple errors

        // Verify specific errors
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateProfessorRequest.EmpNr));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateProfessorRequest.Name));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateProfessorRequest.PhoneNumber));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateProfessorRequest.RankCode));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateProfessorRequest.Salary));
    }

    [Fact]
    public void Validate_WithValidUpdateProfessorRequest_ReturnsSuccess()
    {
        // Arrange
        var request = new UpdateProfessorRequest
        {
            Name = "Dr. Jane Doe",
            PhoneNumber = "555-987-6543",
            RankCode = "FULL",
            Salary = 120000
        };

        // Act
        var result = _validationService.Validate(request);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_WithValidCreateStudentRequest_ReturnsSuccess()
    {
        // Arrange
        var request = new CreateStudentRequest
        {
            StudentId = "1234567",
            Name = "Alice Johnson",
            PhoneNumber = "555-111-2222",
            Class = "Senior",
            GPA = 3.75m
        };

        // Act
        var result = _validationService.Validate(request);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_WithInvalidCreateStudentRequest_ReturnsValidationErrors()
    {
        // Arrange
        var request = new CreateStudentRequest
        {
            StudentId = "-1", // Invalid - negative
            Name = "", // Invalid - required
            PhoneNumber = "123", // Invalid format
            GPA = 5.0m // Invalid - exceeds 4.0
        };

        // Act
        var result = _validationService.Validate(request);

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
    public void Validate_WithValidUpdateStudentRequest_ReturnsSuccess()
    {
        // Arrange
        var request = new UpdateStudentRequest
        {
            Name = "Bob Wilson",
            PhoneNumber = "555-333-4444",
            GPA = 3.25m,
            Class = "Senior"
        };

        // Act
        var result = _validationService.Validate(request);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task ValidateAsync_WithValidModel_ReturnsSuccess()
    {
        // Arrange
        var request = new CreateProfessorRequest
        {
            EmpNr = 789012,
            Name = "Dr. Sarah Brown",
            PhoneNumber = "555-777-8888",
            RankCode = "ASSIST",
            Salary = 70000
        };

        // Act
        var result = await _validationService.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task ValidateAsync_WithInvalidModel_ReturnsValidationErrors()
    {
        // Arrange
        var request = new CreateProfessorRequest
        {
            EmpNr = 12345, // Invalid - should be 6 digits
            Name = "", // Invalid - required
            PhoneNumber = "invalid-phone" // Invalid format
        };

        // Act
        var result = await _validationService.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.Errors.Count > 0);
    }

    [Fact]
    public void Validate_WithUnsupportedType_ReturnsSuccess()
    {
        // Arrange
        var unsupportedModel = new { Name = "Test" };

        // Act
        var result = _validationService.Validate(unsupportedModel);

        // Assert - Should return success when no validator is registered (design decision)
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task ValidateAsync_WithUnsupportedType_ReturnsSuccess()
    {
        // Arrange
        var unsupportedModel = new { Name = "Test" };

        // Act
        var result = await _validationService.ValidateAsync(unsupportedModel);

        // Assert - Should return success when no validator is registered (design decision)
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_WithNullModel_ReturnsValidationError()
    {
        // Act
        var result = _validationService.Validate<CreateProfessorRequest>(null!);

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Model", result.Errors[0].PropertyName);
        Assert.Equal("Model cannot be null", result.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task ValidateAsync_WithNullModel_ReturnsValidationError()
    {
        // Act
        var result = await _validationService.ValidateAsync<CreateProfessorRequest>(null!);

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Model", result.Errors[0].PropertyName);
        Assert.Equal("Model cannot be null", result.Errors[0].ErrorMessage);
    }

    private void Dispose()
    {
        _serviceProvider?.Dispose();
    }
}
