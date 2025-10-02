using System.Text.RegularExpressions;

namespace Zeus.Academia.Api.Validation;

/// <summary>
/// Abstract base class for model validators providing common validation functionality.
/// </summary>
/// <typeparam name="T">The type of model to validate</typeparam>
public abstract class AbstractValidator<T> : IValidator<T>
{
    private readonly List<ValidationRule<T>> _rules = new();

    /// <summary>
    /// Validates the specified model synchronously.
    /// </summary>
    /// <param name="model">The model to validate</param>
    /// <returns>Validation result containing any errors found</returns>
    public virtual ValidationResult Validate(T model)
    {
        if (model == null)
        {
            return ValidationResult.Failure("Model", "Model cannot be null");
        }

        var result = new ValidationResult();

        foreach (var rule in _rules)
        {
            var ruleResult = rule.Validate(model);
            if (!ruleResult.IsValid)
            {
                result = result.Combine(ruleResult);
            }
        }

        return result;
    }

    /// <summary>
    /// Validates the specified model asynchronously.
    /// </summary>
    /// <param name="model">The model to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task containing validation result with any errors found</returns>
    public virtual async Task<ValidationResult> ValidateAsync(T model, CancellationToken cancellationToken = default)
    {
        if (model == null)
        {
            return ValidationResult.Failure("Model", "Model cannot be null");
        }

        var result = new ValidationResult();

        foreach (var rule in _rules)
        {
            var ruleResult = await rule.ValidateAsync(model, cancellationToken);
            if (!ruleResult.IsValid)
            {
                result = result.Combine(ruleResult);
            }
        }

        return result;
    }

    /// <summary>
    /// Adds a validation rule for a specific property.
    /// </summary>
    /// <param name="propertyName">The name of the property</param>
    /// <param name="predicate">The validation predicate</param>
    /// <param name="errorMessage">The error message if validation fails</param>
    protected void RuleFor(string propertyName, Func<T, bool> predicate, string errorMessage)
    {
        _rules.Add(new ValidationRule<T>(propertyName, predicate, errorMessage));
    }

    /// <summary>
    /// Adds an asynchronous validation rule for a specific property.
    /// </summary>
    /// <param name="propertyName">The name of the property</param>
    /// <param name="predicate">The asynchronous validation predicate</param>
    /// <param name="errorMessage">The error message if validation fails</param>
    protected void RuleForAsync(string propertyName, Func<T, CancellationToken, Task<bool>> predicate, string errorMessage)
    {
        _rules.Add(new ValidationRule<T>(propertyName, predicate, errorMessage));
    }

    /// <summary>
    /// Validates that a string property is not null or empty.
    /// </summary>
    /// <param name="propertyName">The name of the property</param>
    /// <param name="valueSelector">Function to select the property value</param>
    /// <param name="customMessage">Custom error message (optional)</param>
    protected void NotEmpty(string propertyName, Func<T, string?> valueSelector, string? customMessage = null)
    {
        var message = customMessage ?? $"{propertyName} is required and cannot be empty";
        RuleFor(propertyName, model => !string.IsNullOrWhiteSpace(valueSelector(model)), message);
    }

    /// <summary>
    /// Validates that a string property matches a specific pattern.
    /// </summary>
    /// <param name="propertyName">The name of the property</param>
    /// <param name="valueSelector">Function to select the property value</param>
    /// <param name="pattern">The regular expression pattern</param>
    /// <param name="customMessage">Custom error message (optional)</param>
    protected void Matches(string propertyName, Func<T, string?> valueSelector, string pattern, string? customMessage = null)
    {
        var message = customMessage ?? $"{propertyName} format is invalid";
        RuleFor(propertyName, model =>
        {
            var value = valueSelector(model);
            return string.IsNullOrEmpty(value) || Regex.IsMatch(value, pattern);
        }, message);
    }

    /// <summary>
    /// Validates that a string property has a maximum length.
    /// </summary>
    /// <param name="propertyName">The name of the property</param>
    /// <param name="valueSelector">Function to select the property value</param>
    /// <param name="maxLength">Maximum allowed length</param>
    /// <param name="customMessage">Custom error message (optional)</param>
    protected void MaxLength(string propertyName, Func<T, string?> valueSelector, int maxLength, string? customMessage = null)
    {
        var message = customMessage ?? $"{propertyName} cannot exceed {maxLength} characters";
        RuleFor(propertyName, model =>
        {
            var value = valueSelector(model);
            return value == null || value.Length <= maxLength;
        }, message);
    }

    /// <summary>
    /// Validates that a string property has a minimum length.
    /// </summary>
    /// <param name="propertyName">The name of the property</param>
    /// <param name="valueSelector">Function to select the property value</param>
    /// <param name="minLength">Minimum required length</param>
    /// <param name="customMessage">Custom error message (optional)</param>
    protected void MinLength(string propertyName, Func<T, string?> valueSelector, int minLength, string? customMessage = null)
    {
        var message = customMessage ?? $"{propertyName} must be at least {minLength} characters long";
        RuleFor(propertyName, model =>
        {
            var value = valueSelector(model);
            return value != null && value.Length >= minLength;
        }, message);
    }

    /// <summary>
    /// Validates that a numeric property is within a specific range.
    /// </summary>
    /// <param name="propertyName">The name of the property</param>
    /// <param name="valueSelector">Function to select the property value</param>
    /// <param name="min">Minimum value (inclusive)</param>
    /// <param name="max">Maximum value (inclusive)</param>
    /// <param name="customMessage">Custom error message (optional)</param>
    protected void Range<TValue>(string propertyName, Func<T, TValue> valueSelector, TValue min, TValue max, string? customMessage = null)
        where TValue : IComparable<TValue>
    {
        var message = customMessage ?? $"{propertyName} must be between {min} and {max}";
        RuleFor(propertyName, model =>
        {
            var value = valueSelector(model);
            return value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0;
        }, message);
    }

    /// <summary>
    /// Validates that an email address is in a valid format.
    /// </summary>
    /// <param name="propertyName">The name of the property</param>
    /// <param name="valueSelector">Function to select the property value</param>
    /// <param name="customMessage">Custom error message (optional)</param>
    protected void EmailAddress(string propertyName, Func<T, string?> valueSelector, string? customMessage = null)
    {
        var message = customMessage ?? $"{propertyName} must be a valid email address";
        const string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        Matches(propertyName, valueSelector, emailPattern, message);
    }
}

/// <summary>
/// Represents a validation rule for a specific property.
/// </summary>
/// <typeparam name="T">The type of model being validated</typeparam>
internal class ValidationRule<T>
{
    private readonly string _propertyName;
    private readonly Func<T, bool>? _syncPredicate;
    private readonly Func<T, CancellationToken, Task<bool>>? _asyncPredicate;
    private readonly string _errorMessage;

    public ValidationRule(string propertyName, Func<T, bool> predicate, string errorMessage)
    {
        _propertyName = propertyName;
        _syncPredicate = predicate;
        _errorMessage = errorMessage;
    }

    public ValidationRule(string propertyName, Func<T, CancellationToken, Task<bool>> predicate, string errorMessage)
    {
        _propertyName = propertyName;
        _asyncPredicate = predicate;
        _errorMessage = errorMessage;
    }

    public ValidationResult Validate(T model)
    {
        bool isValid;

        if (_syncPredicate != null)
        {
            isValid = _syncPredicate(model);
        }
        else if (_asyncPredicate != null)
        {
            // For synchronous validation, we'll run async rules synchronously
            isValid = _asyncPredicate(model, CancellationToken.None).GetAwaiter().GetResult();
        }
        else
        {
            return ValidationResult.Success();
        }

        return isValid ? ValidationResult.Success() : ValidationResult.Failure(_propertyName, _errorMessage);
    }

    public async Task<ValidationResult> ValidateAsync(T model, CancellationToken cancellationToken)
    {
        bool isValid;

        if (_asyncPredicate != null)
        {
            isValid = await _asyncPredicate(model, cancellationToken);
        }
        else if (_syncPredicate != null)
        {
            isValid = _syncPredicate(model);
        }
        else
        {
            return ValidationResult.Success();
        }

        return isValid ? ValidationResult.Success() : ValidationResult.Failure(_propertyName, _errorMessage);
    }
}