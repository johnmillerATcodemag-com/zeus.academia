using Xunit;
using Zeus.Academia.Api.Models.Requests;
using Zeus.Academia.Api.Validation.Validators;

namespace Zeus.Academia.CoverageTests.Api.Validation.Validators;

/// <summary>
/// Comprehensive unit tests for Professor validators.
/// </summary>
public class ProfessorValidatorsTests
{
    [Fact]
    public void CreateProfessorRequestValidator_WithValidRequest_ReturnsSuccess()
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

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(99999)] // Too short
    [InlineData(1000000)] // Too long
    public void CreateProfessorRequestValidator_WithInvalidEmpNr_ReturnsValidationError(int invalidEmpNr)
    {
        // Arrange
        var validator = new CreateProfessorRequestValidator();
        var request = new CreateProfessorRequest
        {
            EmpNr = invalidEmpNr,
            Name = "Dr. John Smith",
            PhoneNumber = "555-123-4567",
            RankCode = "ASSOC",
            Salary = 85000
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateProfessorRequest.EmpNr));
    }

    [Theory]
    [InlineData("")]
    [InlineData("A")] // Too short
    public void CreateProfessorRequestValidator_WithInvalidName_ReturnsValidationError(string invalidName)
    {
        // Arrange
        var validator = new CreateProfessorRequestValidator();
        var request = new CreateProfessorRequest
        {
            EmpNr = 123456,
            Name = invalidName,
            PhoneNumber = "555-123-4567",
            RankCode = "ASSOC",
            Salary = 85000
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateProfessorRequest.Name));
    }

    [Theory]
    [InlineData("123")]
    [InlineData("invalid-phone")]
    [InlineData("123-456-7890-1234-5678")] // Too long
    public void CreateProfessorRequestValidator_WithInvalidPhoneNumber_ReturnsValidationError(string invalidPhone)
    {
        // Arrange
        var validator = new CreateProfessorRequestValidator();
        var request = new CreateProfessorRequest
        {
            EmpNr = 123456,
            Name = "Dr. John Smith",
            PhoneNumber = invalidPhone,
            RankCode = "ASSOC",
            Salary = 85000
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateProfessorRequest.PhoneNumber));
    }

    [Theory]
    [InlineData("555-123-4567")]
    [InlineData("(555) 123-4567")]
    [InlineData("555.123.4567")]
    public void CreateProfessorRequestValidator_WithValidPhoneNumber_PassesValidation(string validPhone)
    {
        // Arrange
        var validator = new CreateProfessorRequestValidator();
        var request = new CreateProfessorRequest
        {
            EmpNr = 123456,
            Name = "Dr. John Smith",
            PhoneNumber = validPhone,
            RankCode = "ASSOC",
            Salary = 85000
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("INVALID")]
    [InlineData("Professor")]
    public void CreateProfessorRequestValidator_WithInvalidRankCode_ReturnsValidationError(string invalidRank)
    {
        // Arrange
        var validator = new CreateProfessorRequestValidator();
        var request = new CreateProfessorRequest
        {
            EmpNr = 123456,
            Name = "Dr. John Smith",
            PhoneNumber = "555-123-4567",
            RankCode = invalidRank,
            Salary = 85000
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateProfessorRequest.RankCode));
    }

    [Theory]
    [InlineData("ASSIST")]
    [InlineData("ASSOC")]
    [InlineData("FULL")]
    [InlineData("PROF")]
    public void CreateProfessorRequestValidator_WithValidRankCode_PassesValidation(string validRank)
    {
        // Arrange
        var validator = new CreateProfessorRequestValidator();
        var request = new CreateProfessorRequest
        {
            EmpNr = 123456,
            Name = "Dr. John Smith",
            PhoneNumber = "555-123-4567",
            RankCode = validRank,
            Salary = 85000
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1000001)] // Above maximum
    public void CreateProfessorRequestValidator_WithInvalidSalary_ReturnsValidationError(decimal invalidSalary)
    {
        // Arrange
        var validator = new CreateProfessorRequestValidator();
        var request = new CreateProfessorRequest
        {
            EmpNr = 123456,
            Name = "Dr. John Smith",
            PhoneNumber = "555-123-4567",
            RankCode = "ASSOC",
            Salary = invalidSalary
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateProfessorRequest.Salary));
    }

    [Theory]
    [InlineData(30000)]
    [InlineData(85000)]
    [InlineData(150000)]
    [InlineData(1000000)]
    public void CreateProfessorRequestValidator_WithValidSalary_PassesValidation(decimal validSalary)
    {
        // Arrange
        var validator = new CreateProfessorRequestValidator();
        var request = new CreateProfessorRequest
        {
            EmpNr = 123456,
            Name = "Dr. John Smith",
            PhoneNumber = "555-123-4567",
            RankCode = "ASSOC",
            Salary = validSalary
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void UpdateProfessorRequestValidator_WithValidRequest_ReturnsSuccess()
    {
        // Arrange
        var validator = new UpdateProfessorRequestValidator();
        var request = new UpdateProfessorRequest
        {
            Name = "Dr. Jane Doe",
            PhoneNumber = "555-987-6543",
            RankCode = "FULL",
            Salary = 120000,
            ResearchArea = "Machine Learning, Data Science"
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void UpdateProfessorRequestValidator_WithInvalidOptionalFields_ReturnsValidationErrors()
    {
        // Arrange
        var validator = new UpdateProfessorRequestValidator();
        var request = new UpdateProfessorRequest
        {
            Name = "A", // Too short
            PhoneNumber = "123", // Invalid format
            RankCode = "INVALID", // Invalid rank
            Salary = -1000 // Invalid salary
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateProfessorRequest.Name));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateProfessorRequest.PhoneNumber));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateProfessorRequest.RankCode));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateProfessorRequest.Salary));
    }

    [Fact]
    public async Task CreateProfessorRequestValidator_ValidateAsync_WithValidRequest_ReturnsSuccess()
    {
        // Arrange
        var validator = new CreateProfessorRequestValidator();
        var request = new CreateProfessorRequest
        {
            EmpNr = 789012,
            Name = "Dr. Sarah Brown",
            PhoneNumber = "555-777-8888",
            RankCode = "ASSIST",
            Salary = 70000
        };

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task UpdateProfessorRequestValidator_ValidateAsync_WithValidRequest_ReturnsSuccess()
    {
        // Arrange
        var validator = new UpdateProfessorRequestValidator();
        var request = new UpdateProfessorRequest
        {
            Name = "Dr. Michael Johnson",
            RankCode = "FULL",
            Salary = 135000
        };

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
}
