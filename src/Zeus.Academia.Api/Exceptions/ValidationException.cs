namespace Zeus.Academia.Api.Exceptions;

/// <summary>
/// Exception thrown when input validation fails
/// </summary>
public class ValidationException : AcademiaException
{
    public override int HttpStatusCode => 400; // Bad Request

    /// <summary>
    /// Gets the validation errors associated with this exception
    /// </summary>
    public Dictionary<string, List<string>> ValidationErrors { get; } = new();

    /// <summary>
    /// Common error codes for validation exceptions
    /// </summary>
    public static class ErrorCodes
    {
        public const string InvalidInput = "VALIDATION_INVALID_INPUT";
        public const string RequiredField = "VALIDATION_REQUIRED_FIELD";
        public const string InvalidFormat = "VALIDATION_INVALID_FORMAT";
        public const string OutOfRange = "VALIDATION_OUT_OF_RANGE";
        public const string DuplicateValue = "VALIDATION_DUPLICATE_VALUE";
        public const string InvalidLength = "VALIDATION_INVALID_LENGTH";
        public const string InvalidPattern = "VALIDATION_INVALID_PATTERN";
        public const string InvalidEnum = "VALIDATION_INVALID_ENUM";
        public const string InvalidDate = "VALIDATION_INVALID_DATE";
        public const string ModelValidation = "VALIDATION_MODEL_VALIDATION";
    }

    public ValidationException() : base("One or more validation errors occurred.")
    {
        ErrorCode = ErrorCodes.InvalidInput;
    }

    public ValidationException(string message) : base(message)
    {
        ErrorCode = ErrorCodes.InvalidInput;
    }

    public ValidationException(string message, string errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }

    public ValidationException(string message, Exception innerException) : base(message, innerException)
    {
        ErrorCode = ErrorCodes.InvalidInput;
    }

    public ValidationException(string message, Dictionary<string, List<string>> validationErrors) : base(message)
    {
        ErrorCode = ErrorCodes.ModelValidation;
        ValidationErrors = validationErrors ?? new Dictionary<string, List<string>>();
    }

    /// <summary>
    /// Adds a validation error for a specific field
    /// </summary>
    /// <param name="field">The field name</param>
    /// <param name="error">The error message</param>
    /// <returns>This exception instance for fluent configuration</returns>
    public ValidationException AddValidationError(string field, string error)
    {
        if (!ValidationErrors.ContainsKey(field))
        {
            ValidationErrors[field] = new List<string>();
        }
        ValidationErrors[field].Add(error);
        return this;
    }

    /// <summary>
    /// Creates a validation exception for a required field
    /// </summary>
    /// <param name="fieldName">The name of the required field</param>
    /// <returns>ValidationException instance</returns>
    public static ValidationException RequiredField(string fieldName)
    {
        var exception = new ValidationException($"The field '{fieldName}' is required.", ErrorCodes.RequiredField);
        exception.AddValidationError(fieldName, "This field is required.");
        exception.AddContext("field", fieldName);
        return exception;
    }

    /// <summary>
    /// Creates a validation exception for invalid format
    /// </summary>
    /// <param name="fieldName">The name of the field</param>
    /// <param name="expectedFormat">The expected format</param>
    /// <returns>ValidationException instance</returns>
    public static ValidationException InvalidFormat(string fieldName, string expectedFormat)
    {
        var exception = new ValidationException($"The field '{fieldName}' has an invalid format. Expected: {expectedFormat}", ErrorCodes.InvalidFormat);
        exception.AddValidationError(fieldName, $"Invalid format. Expected: {expectedFormat}");
        exception.AddContext("field", fieldName);
        exception.AddContext("expectedFormat", expectedFormat);
        return exception;
    }

    /// <summary>
    /// Creates a validation exception for out of range values
    /// </summary>
    /// <param name="fieldName">The name of the field</param>
    /// <param name="minValue">The minimum allowed value</param>
    /// <param name="maxValue">The maximum allowed value</param>
    /// <param name="actualValue">The actual value that was provided</param>
    /// <returns>ValidationException instance</returns>
    public static ValidationException OutOfRange(string fieldName, object minValue, object maxValue, object actualValue)
    {
        var exception = new ValidationException($"The field '{fieldName}' value '{actualValue}' is out of range. Must be between {minValue} and {maxValue}.", ErrorCodes.OutOfRange);
        exception.AddValidationError(fieldName, $"Value must be between {minValue} and {maxValue}.");
        exception.AddContext("field", fieldName);
        exception.AddContext("minValue", minValue);
        exception.AddContext("maxValue", maxValue);
        exception.AddContext("actualValue", actualValue);
        return exception;
    }

    /// <summary>
    /// Creates a validation exception for duplicate values
    /// </summary>
    /// <param name="fieldName">The name of the field</param>
    /// <param name="value">The duplicate value</param>
    /// <returns>ValidationException instance</returns>
    public static ValidationException DuplicateValue(string fieldName, object value)
    {
        var exception = new ValidationException($"The field '{fieldName}' value '{value}' already exists.", ErrorCodes.DuplicateValue);
        exception.AddValidationError(fieldName, "This value already exists.");
        exception.AddContext("field", fieldName);
        exception.AddContext("value", value);
        return exception;
    }

    /// <summary>
    /// Creates a validation exception for academic-specific validation (e.g., employee number format)
    /// </summary>
    /// <param name="fieldName">The name of the field</param>
    /// <param name="value">The invalid value</param>
    /// <param name="academicRule">The academic rule that was violated</param>
    /// <returns>ValidationException instance</returns>
    public static ValidationException AcademicRule(string fieldName, object value, string academicRule)
    {
        var exception = new ValidationException($"The field '{fieldName}' value '{value}' violates academic rule: {academicRule}", ErrorCodes.InvalidPattern);
        exception.AddValidationError(fieldName, $"Violates academic rule: {academicRule}");
        exception.AddContext("field", fieldName);
        exception.AddContext("value", value);
        exception.AddContext("academicRule", academicRule);
        return exception;
    }
}