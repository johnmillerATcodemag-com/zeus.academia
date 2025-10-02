using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Zeus.Academia.Api.Models.Responses;

/// <summary>
/// Base API response wrapper providing consistent response format
/// </summary>
/// <remarks>
/// All API responses should use this wrapper to ensure consistency across the API.
/// Includes support for both JSON and XML serialization for content negotiation.
/// </remarks>
[XmlRoot("Response")]
public class ApiResponse
{
    /// <summary>
    /// Indicates if the operation was successful
    /// </summary>
    [JsonPropertyName("success")]
    [XmlElement("Success")]
    public bool Success { get; set; }

    /// <summary>
    /// Response message providing additional context
    /// </summary>
    [JsonPropertyName("message")]
    [XmlElement("Message")]
    public string? Message { get; set; }

    /// <summary>
    /// Error details (only populated on failure)
    /// </summary>
    /// <remarks>
    /// Can contain validation errors, exception details, or other error information
    /// </remarks>
    [JsonPropertyName("errors")]
    [XmlElement("Errors")]
    public object? Errors { get; set; }

    /// <summary>
    /// Response timestamp in UTC
    /// </summary>
    [JsonPropertyName("timestamp")]
    [XmlElement("Timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Correlation ID for request tracking and debugging
    /// </summary>
    [JsonPropertyName("correlationId")]
    [XmlElement("CorrelationId")]
    public string? CorrelationId { get; set; }

    /// <summary>
    /// API version that processed the request
    /// </summary>
    [JsonPropertyName("version")]
    [XmlElement("Version")]
    public string? Version { get; set; }

    /// <summary>
    /// Creates a successful response
    /// </summary>
    /// <param name="message">Success message</param>
    /// <param name="correlationId">Request correlation ID</param>
    /// <param name="version">API version</param>
    /// <returns>Success response</returns>
    public static ApiResponse CreateSuccess(string? message = null, string? correlationId = null, string? version = null)
    {
        return new ApiResponse
        {
            Success = true,
            Message = message,
            CorrelationId = correlationId,
            Version = version,
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates an error response
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="errors">Detailed error information</param>
    /// <param name="correlationId">Request correlation ID</param>
    /// <param name="version">API version</param>
    /// <returns>Error response</returns>
    public static ApiResponse CreateError(string message, object? errors = null, string? correlationId = null, string? version = null)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            Errors = errors,
            CorrelationId = correlationId,
            Version = version,
            Timestamp = DateTime.UtcNow
        };
    }
}