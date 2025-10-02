using System.Diagnostics;

namespace Zeus.Academia.Api.Services;

/// <summary>
/// Service for managing correlation IDs across HTTP requests.
/// Correlation IDs help track requests across different parts of the system for debugging and monitoring.
/// </summary>
public interface ICorrelationIdService
{
    /// <summary>
    /// Gets the current correlation ID for this request.
    /// </summary>
    string CorrelationId { get; }

    /// <summary>
    /// Sets the correlation ID for this request.
    /// </summary>
    /// <param name="correlationId">The correlation ID to set</param>
    void SetCorrelationId(string correlationId);

    /// <summary>
    /// Generates a new correlation ID.
    /// </summary>
    /// <returns>A new correlation ID</returns>
    string GenerateCorrelationId();
}

/// <summary>
/// Implementation of correlation ID service using AsyncLocal for thread-safe storage.
/// </summary>
public class CorrelationIdService : ICorrelationIdService
{
    private static readonly AsyncLocal<string> _correlationId = new AsyncLocal<string>();

    /// <summary>
    /// Gets the current correlation ID for this request.
    /// If no correlation ID is set, returns a default value.
    /// </summary>
    public string CorrelationId => _correlationId.Value ?? "unknown";

    /// <summary>
    /// Sets the correlation ID for this request.
    /// </summary>
    /// <param name="correlationId">The correlation ID to set</param>
    public void SetCorrelationId(string correlationId)
    {
        _correlationId.Value = correlationId;

        // Also set it in the current activity for OpenTelemetry compatibility
        Activity.Current?.SetTag("correlation-id", correlationId);
    }

    /// <summary>
    /// Generates a new correlation ID using a combination of timestamp and GUID.
    /// Format: {timestamp}-{short-guid} for readability and uniqueness.
    /// </summary>
    /// <returns>A new correlation ID</returns>
    public string GenerateCorrelationId()
    {
        var timestamp = DateTimeOffset.UtcNow.ToString("yyyyMMdd-HHmmss");
        var shortGuid = Guid.NewGuid().ToString("N")[..8]; // First 8 characters
        return $"{timestamp}-{shortGuid}";
    }
}