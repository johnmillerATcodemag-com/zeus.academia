using System.Text.Json.Serialization;

namespace Zeus.Academia.Api.Models.Responses;

/// <summary>
/// Standardized error response model for all API errors
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Gets or sets the error type/category
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the error code for programmatic handling
    /// </summary>
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the human-readable error message
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the correlation ID for tracking this error
    /// </summary>
    [JsonPropertyName("correlationId")]
    public string CorrelationId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp when the error occurred
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets the HTTP status code
    /// </summary>
    [JsonPropertyName("status")]
    public int Status { get; set; }

    /// <summary>
    /// Gets or sets the path where the error occurred
    /// </summary>
    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets additional context information about the error
    /// </summary>
    [JsonPropertyName("context")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, object>? Context { get; set; }

    /// <summary>
    /// Gets or sets validation errors (for validation exceptions)
    /// </summary>
    [JsonPropertyName("validationErrors")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, List<string>>? ValidationErrors { get; set; }

    /// <summary>
    /// Gets or sets detailed error information (only in development)
    /// </summary>
    [JsonPropertyName("details")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ErrorDetails? Details { get; set; }

    /// <summary>
    /// Gets or sets helpful links or documentation references
    /// </summary>
    [JsonPropertyName("links")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, string>? Links { get; set; }
}

/// <summary>
/// Detailed error information for development environments
/// </summary>
public class ErrorDetails
{
    /// <summary>
    /// Gets or sets the full exception type name
    /// </summary>
    [JsonPropertyName("exceptionType")]
    public string ExceptionType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the stack trace
    /// </summary>
    [JsonPropertyName("stackTrace")]
    public string StackTrace { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the source of the exception
    /// </summary>
    [JsonPropertyName("source")]
    public string Source { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets information about the inner exception
    /// </summary>
    [JsonPropertyName("innerException")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public InnerExceptionInfo? InnerException { get; set; }
}

/// <summary>
/// Information about inner exceptions
/// </summary>
public class InnerExceptionInfo
{
    /// <summary>
    /// Gets or sets the inner exception type
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the inner exception message
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the inner exception stack trace
    /// </summary>
    [JsonPropertyName("stackTrace")]
    public string StackTrace { get; set; } = string.Empty;
}

/// <summary>
/// Factory class for creating standardized error responses
/// </summary>
public static class ErrorResponseFactory
{
    /// <summary>
    /// Error type constants
    /// </summary>
    public static class ErrorTypes
    {
        public const string ValidationError = "validation_error";
        public const string BusinessLogicError = "business_logic_error";
        public const string AuthorizationError = "authorization_error";
        public const string AuthenticationError = "authentication_error";
        public const string NotFoundError = "not_found_error";
        public const string ConflictError = "conflict_error";
        public const string InternalServerError = "internal_server_error";
        public const string ServiceUnavailableError = "service_unavailable_error";
        public const string TimeoutError = "timeout_error";
    }

    /// <summary>
    /// Creates a validation error response
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="code">Error code</param>
    /// <param name="correlationId">Correlation ID</param>
    /// <param name="path">Request path</param>
    /// <param name="validationErrors">Validation errors</param>
    /// <param name="context">Additional context</param>
    /// <returns>ErrorResponse instance</returns>
    public static ErrorResponse CreateValidationError(
        string message,
        string code,
        string correlationId,
        string path,
        Dictionary<string, List<string>>? validationErrors = null,
        Dictionary<string, object>? context = null)
    {
        return new ErrorResponse
        {
            Type = ErrorTypes.ValidationError,
            Code = code,
            Message = message,
            CorrelationId = correlationId,
            Path = path,
            Status = 400,
            ValidationErrors = validationErrors,
            Context = context
        };
    }

    /// <summary>
    /// Creates a business logic error response
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="code">Error code</param>
    /// <param name="correlationId">Correlation ID</param>
    /// <param name="path">Request path</param>
    /// <param name="context">Additional context</param>
    /// <returns>ErrorResponse instance</returns>
    public static ErrorResponse CreateBusinessLogicError(
        string message,
        string code,
        string correlationId,
        string path,
        Dictionary<string, object>? context = null)
    {
        return new ErrorResponse
        {
            Type = ErrorTypes.BusinessLogicError,
            Code = code,
            Message = message,
            CorrelationId = correlationId,
            Path = path,
            Status = 400,
            Context = context
        };
    }

    /// <summary>
    /// Creates an authorization error response
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="code">Error code</param>
    /// <param name="correlationId">Correlation ID</param>
    /// <param name="path">Request path</param>
    /// <param name="context">Additional context</param>
    /// <returns>ErrorResponse instance</returns>
    public static ErrorResponse CreateAuthorizationError(
        string message,
        string code,
        string correlationId,
        string path,
        Dictionary<string, object>? context = null)
    {
        return new ErrorResponse
        {
            Type = ErrorTypes.AuthorizationError,
            Code = code,
            Message = message,
            CorrelationId = correlationId,
            Path = path,
            Status = 403,
            Context = context
        };
    }

    /// <summary>
    /// Creates an internal server error response
    /// </summary>
    /// <param name="correlationId">Correlation ID</param>
    /// <param name="path">Request path</param>
    /// <param name="includeDetails">Whether to include detailed error information</param>
    /// <param name="exception">The exception that occurred</param>
    /// <returns>ErrorResponse instance</returns>
    public static ErrorResponse CreateInternalServerError(
        string correlationId,
        string path,
        bool includeDetails = false,
        Exception? exception = null)
    {
        var response = new ErrorResponse
        {
            Type = ErrorTypes.InternalServerError,
            Code = "INTERNAL_SERVER_ERROR",
            Message = "An internal server error occurred. Please try again later.",
            CorrelationId = correlationId,
            Path = path,
            Status = 500
        };

        if (includeDetails && exception != null)
        {
            response.Details = new ErrorDetails
            {
                ExceptionType = exception.GetType().FullName ?? string.Empty,
                StackTrace = exception.StackTrace ?? string.Empty,
                Source = exception.Source ?? string.Empty,
                InnerException = exception.InnerException != null
                    ? new InnerExceptionInfo
                    {
                        Type = exception.InnerException.GetType().FullName ?? string.Empty,
                        Message = exception.InnerException.Message,
                        StackTrace = exception.InnerException.StackTrace ?? string.Empty
                    }
                    : null
            };
        }

        return response;
    }
}