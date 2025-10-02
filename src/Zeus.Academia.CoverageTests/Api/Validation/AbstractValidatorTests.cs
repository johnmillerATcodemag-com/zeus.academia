using Xunit;
using Zeus.Academia.Api.Validation;

namespace Zeus.Academia.CoverageTests.Api.Validation;

/// <summary>
/// Test model for AbstractValidator testing.
/// </summary>
public class TestModel
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public int Age { get; set; }
    public decimal? Salary { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// Test validator implementation for testing AbstractValidator.
/// </summary>
public class TestModelValidator : AbstractValidator<TestModel>
{
    public TestModelValidator()
    {
        // Basic string validation
        NotEmpty(nameof(TestModel.Name), model => model.Name, "Name is required");
        MinLength(nameof(TestModel.Name), model => model.Name, 2, "Name must be at least 2 characters");
        MaxLength(nameof(TestModel.Name), model => model.Name, 50, "Name cannot exceed 50 characters");

        // Email validation
        EmailAddress(nameof(TestModel.Email), model => model.Email, "Email must be a valid email address");

        // Range validation
        Range(nameof(TestModel.Age), model => model.Age, 0, 120, "Age must be between 0 and 120");

        // Custom rule
        RuleFor(nameof(TestModel.Salary),
            model => !model.Salary.HasValue || model.Salary.Value > 0,
            "Salary must be positive if provided");

        // Pattern matching
        Matches(nameof(TestModel.Description),
            model => model.Description,
            @"^[a-zA-Z0-9\s]*$",
            "Description can only contain letters, numbers, and spaces");
    }
}

/// <summary>
/// Comprehensive unit tests for AbstractValidator.
/// </summary>
public class AbstractValidatorTests
{
    private readonly TestModelValidator _validator;

    public AbstractValidatorTests()
    {
        _validator = new TestModelValidator();
    }

    [Fact]
    public void Validate_WithNullModel_ReturnsValidationError()
    {
        // Act
        var result = _validator.Validate(null!);

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Model", result.Errors[0].PropertyName);
        Assert.Equal("Model cannot be null", result.Errors[0].ErrorMessage);
    }

    [Fact]
    public void Validate_WithValidModel_ReturnsSuccess()
    {
        // Arrange
        var model = new TestModel
        {
            Name = "John Doe",
            Email = "john.doe@example.com",
            Age = 30,
            Salary = 50000,
            Description = "Valid description"
        };

        // Act
        var result = _validator.Validate(model);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_WithEmptyName_ReturnsValidationError()
    {
        // Arrange
        var model = new TestModel
        {
            Name = "",
            Email = "john.doe@example.com",
            Age = 30
        };

        // Act
        var result = _validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(TestModel.Name) &&
                                          e.ErrorMessage == "Name is required");
    }

    [Fact]
    public void Validate_WithShortName_ReturnsValidationError()
    {
        // Arrange
        var model = new TestModel
        {
            Name = "A",
            Email = "john.doe@example.com",
            Age = 30
        };

        // Act
        var result = _validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(TestModel.Name) &&
                                          e.ErrorMessage == "Name must be at least 2 characters");
    }

    [Fact]
    public void Validate_WithLongName_ReturnsValidationError()
    {
        // Arrange
        var model = new TestModel
        {
            Name = new string('A', 51), // 51 characters
            Email = "john.doe@example.com",
            Age = 30
        };

        // Act
        var result = _validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(TestModel.Name) &&
                                          e.ErrorMessage == "Name cannot exceed 50 characters");
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    [InlineData("user.name@")]
    public void Validate_WithInvalidEmail_ReturnsValidationError(string invalidEmail)
    {
        // Arrange
        var model = new TestModel
        {
            Name = "John Doe",
            Email = invalidEmail,
            Age = 30
        };

        // Act
        var result = _validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(TestModel.Email) &&
                                          e.ErrorMessage == "Email must be a valid email address");
    }

    [Theory]
    [InlineData("user@example.com")]
    [InlineData("test.user@domain.co.uk")]
    [InlineData("user123@test-domain.org")]
    public void Validate_WithValidEmail_PassesValidation(string validEmail)
    {
        // Arrange
        var model = new TestModel
        {
            Name = "John Doe",
            Email = validEmail,
            Age = 30
        };

        // Act
        var result = _validator.Validate(model);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(121)]
    [InlineData(500)]
    public void Validate_WithInvalidAge_ReturnsValidationError(int invalidAge)
    {
        // Arrange
        var model = new TestModel
        {
            Name = "John Doe",
            Email = "john.doe@example.com",
            Age = invalidAge
        };

        // Act
        var result = _validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(TestModel.Age) &&
                                          e.ErrorMessage == "Age must be between 0 and 120");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(25)]
    [InlineData(65)]
    [InlineData(120)]
    public void Validate_WithValidAge_PassesValidation(int validAge)
    {
        // Arrange
        var model = new TestModel
        {
            Name = "John Doe",
            Email = "john.doe@example.com",
            Age = validAge
        };

        // Act
        var result = _validator.Validate(model);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithNegativeSalary_ReturnsValidationError()
    {
        // Arrange
        var model = new TestModel
        {
            Name = "John Doe",
            Email = "john.doe@example.com",
            Age = 30,
            Salary = -1000
        };

        // Act
        var result = _validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(TestModel.Salary) &&
                                          e.ErrorMessage == "Salary must be positive if provided");
    }

    [Fact]
    public void Validate_WithNullSalary_PassesValidation()
    {
        // Arrange
        var model = new TestModel
        {
            Name = "John Doe",
            Email = "john.doe@example.com",
            Age = 30,
            Salary = null
        };

        // Act
        var result = _validator.Validate(model);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("Valid description 123")]
    [InlineData("Another valid description")]
    [InlineData(null)] // Null should pass pattern matching
    [InlineData("")] // Empty should pass pattern matching
    public void Validate_WithValidDescription_PassesValidation(string? description)
    {
        // Arrange
        var model = new TestModel
        {
            Name = "John Doe",
            Email = "john.doe@example.com",
            Age = 30,
            Description = description
        };

        // Act
        var result = _validator.Validate(model);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("Invalid@description")]
    [InlineData("Description with $ symbol")]
    [InlineData("Text with % character")]
    public void Validate_WithInvalidDescription_ReturnsValidationError(string invalidDescription)
    {
        // Arrange
        var model = new TestModel
        {
            Name = "John Doe",
            Email = "john.doe@example.com",
            Age = 30,
            Description = invalidDescription
        };

        // Act
        var result = _validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(TestModel.Description) &&
                                          e.ErrorMessage == "Description can only contain letters, numbers, and spaces");
    }

    [Fact]
    public void Validate_WithMultipleErrors_ReturnsAllErrors()
    {
        // Arrange
        var model = new TestModel
        {
            Name = "", // Empty name
            Email = "invalid-email", // Invalid email
            Age = 150, // Invalid age
            Salary = -1000, // Negative salary
            Description = "Invalid@description" // Invalid description
        };

        // Act
        var result = _validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(6, result.Errors.Count);

        // Check that all expected errors are present
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(TestModel.Name));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(TestModel.Email));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(TestModel.Age));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(TestModel.Salary));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(TestModel.Description));
    }

    [Fact]
    public async Task ValidateAsync_WithValidModel_ReturnsSuccess()
    {
        // Arrange
        var model = new TestModel
        {
            Name = "John Doe",
            Email = "john.doe@example.com",
            Age = 30
        };

        // Act
        var result = await _validator.ValidateAsync(model);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task ValidateAsync_WithInvalidModel_ReturnsValidationErrors()
    {
        // Arrange
        var model = new TestModel
        {
            Name = "",
            Email = "invalid-email",
            Age = 150
        };

        // Act
        var result = await _validator.ValidateAsync(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.Errors.Count > 0);
    }

    [Fact]
    public async Task ValidateAsync_WithNullModel_ReturnsValidationError()
    {
        // Act
        var result = await _validator.ValidateAsync(null!);

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Model", result.Errors[0].PropertyName);
    }
}