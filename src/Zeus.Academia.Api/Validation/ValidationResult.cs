namespace Zeus.Academia.Api.Validation;

/// <summary>
/// Represents the result of a validation operation.
/// </summary>
public class ValidationResult
{
    private readonly List<ValidationError> _errors = new();

    /// <summary>
    /// Gets a value indicating whether the validation was successful (no errors).
    /// </summary>
    public bool IsValid => !_errors.Any();

    /// <summary>
    /// Gets the collection of validation errors.
    /// </summary>
    public IReadOnlyList<ValidationError> Errors => _errors.AsReadOnly();

    /// <summary>
    /// Creates a successful validation result.
    /// </summary>
    /// <returns>A validation result with no errors</returns>
    public static ValidationResult Success() => new();

    /// <summary>
    /// Creates a failed validation result with the specified error.
    /// </summary>
    /// <param name="error">The validation error</param>
    /// <returns>A validation result containing the error</returns>
    public static ValidationResult Failure(ValidationError error)
    {
        var result = new ValidationResult();
        result._errors.Add(error);
        return result;
    }

    /// <summary>
    /// Creates a failed validation result with the specified errors.
    /// </summary>
    /// <param name="errors">The validation errors</param>
    /// <returns>A validation result containing the errors</returns>
    public static ValidationResult Failure(IEnumerable<ValidationError> errors)
    {
        var result = new ValidationResult();
        result._errors.AddRange(errors);
        return result;
    }

    /// <summary>
    /// Creates a failed validation result with a single error message.
    /// </summary>
    /// <param name="propertyName">The name of the property that failed validation</param>
    /// <param name="errorMessage">The error message</param>
    /// <returns>A validation result containing the error</returns>
    public static ValidationResult Failure(string propertyName, string errorMessage)
    {
        return Failure(new ValidationError(propertyName, errorMessage));
    }

    /// <summary>
    /// Adds a validation error to the result.
    /// </summary>
    /// <param name="error">The validation error to add</param>
    public void AddError(ValidationError error)
    {
        _errors.Add(error);
    }

    /// <summary>
    /// Adds a validation error to the result.
    /// </summary>
    /// <param name="propertyName">The name of the property that failed validation</param>
    /// <param name="errorMessage">The error message</param>
    public void AddError(string propertyName, string errorMessage)
    {
        AddError(new ValidationError(propertyName, errorMessage));
    }

    /// <summary>
    /// Combines this validation result with another validation result.
    /// </summary>
    /// <param name="other">The other validation result to combine</param>
    /// <returns>A new validation result containing errors from both results</returns>
    public ValidationResult Combine(ValidationResult other)
    {
        var combined = new ValidationResult();
        combined._errors.AddRange(_errors);
        combined._errors.AddRange(other._errors);
        return combined;
    }

    /// <summary>
    /// Converts this validation result to a ValidationException that can be thrown.
    /// </summary>
    /// <param name="message">Optional custom message for the exception</param>
    /// <returns>A ValidationException containing all validation errors</returns>
    public Zeus.Academia.Api.Exceptions.ValidationException ToException(string? message = null)
    {
        var exception = new Zeus.Academia.Api.Exceptions.ValidationException(
            message ?? "One or more validation errors occurred.",
            Zeus.Academia.Api.Exceptions.ValidationException.ErrorCodes.ModelValidation);

        foreach (var error in _errors)
        {
            if (!exception.ValidationErrors.ContainsKey(error.PropertyName))
            {
                exception.ValidationErrors[error.PropertyName] = new List<string>();
            }
            exception.ValidationErrors[error.PropertyName].Add(error.ErrorMessage);
        }

        return exception;
    }
}

/// <summary>
/// Represents a single validation error.
/// </summary>
public class ValidationError
{
    /// <summary>
    /// Initializes a new instance of the ValidationError class.
    /// </summary>
    /// <param name="propertyName">The name of the property that failed validation</param>
    /// <param name="errorMessage">The error message describing the validation failure</param>
    /// <param name="attemptedValue">The value that was attempted to be set (optional)</param>
    public ValidationError(string propertyName, string errorMessage, object? attemptedValue = null)
    {
        PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
        ErrorMessage = errorMessage ?? throw new ArgumentNullException(nameof(errorMessage));
        AttemptedValue = attemptedValue;
    }

    /// <summary>
    /// Gets the name of the property that failed validation.
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    /// Gets the error message describing the validation failure.
    /// </summary>
    public string ErrorMessage { get; }

    /// <summary>
    /// Gets the value that was attempted to be set.
    /// </summary>
    public object? AttemptedValue { get; }

    /// <summary>
    /// Returns a string representation of the validation error.
    /// </summary>
    public override string ToString()
    {
        return $"{PropertyName}: {ErrorMessage}";
    }
}