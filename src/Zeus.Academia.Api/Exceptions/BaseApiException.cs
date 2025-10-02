using System.Net;

namespace Zeus.Academia.Api.Exceptions;

/// <summary>
/// Base class for all custom application exceptions with HTTP status code support
/// </summary>
public abstract class BaseApiException : Exception
{
    /// <summary>
    /// HTTP status code to return for this exception
    /// </summary>
    public abstract HttpStatusCode StatusCode { get; }

    /// <summary>
    /// Error code for client identification
    /// </summary>
    public abstract string ErrorCode { get; }

    /// <summary>
    /// Additional context data for the error
    /// </summary>
    public Dictionary<string, object> Context { get; }

    /// <summary>
    /// Correlation ID for tracking this error across systems
    /// </summary>
    public string? CorrelationId { get; set; }

    protected BaseApiException(string message) : base(message)
    {
        Context = new Dictionary<string, object>();
    }

    protected BaseApiException(string message, Exception innerException) : base(message, innerException)
    {
        Context = new Dictionary<string, object>();
    }

    /// <summary>
    /// Add context information to the exception
    /// </summary>
    public BaseApiException WithContext(string key, object value)
    {
        Context[key] = value;
        return this;
    }

    /// <summary>
    /// Set the correlation ID for this exception
    /// </summary>
    public BaseApiException WithCorrelationId(string correlationId)
    {
        CorrelationId = correlationId;
        return this;
    }
}