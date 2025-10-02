namespace Zeus.Academia.Api.Exceptions;

/// <summary>
/// Base exception for all Zeus Academia API-specific exceptions
/// </summary>
public abstract class AcademiaException : Exception
{
    /// <summary>
    /// Gets the error code associated with this exception
    /// </summary>
    public virtual string ErrorCode { get; protected set; } = string.Empty;

    /// <summary>
    /// Gets additional context data for the exception
    /// </summary>
    public Dictionary<string, object> Context { get; } = new();

    /// <summary>
    /// Gets the HTTP status code that should be returned for this exception
    /// </summary>
    public abstract int HttpStatusCode { get; }

    protected AcademiaException() : base() { }

    protected AcademiaException(string message) : base(message) { }

    protected AcademiaException(string message, Exception innerException) : base(message, innerException) { }

    /// <summary>
    /// Adds context information to the exception
    /// </summary>
    /// <param name="key">Context key</param>
    /// <param name="value">Context value</param>
    /// <returns>This exception instance for fluent configuration</returns>
    public AcademiaException AddContext(string key, object value)
    {
        Context[key] = value;
        return this;
    }

    /// <summary>
    /// Sets the error code for this exception
    /// </summary>
    /// <param name="errorCode">The error code</param>
    /// <returns>This exception instance for fluent configuration</returns>
    public AcademiaException SetErrorCode(string errorCode)
    {
        ErrorCode = errorCode;
        return this;
    }
}