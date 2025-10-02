using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Zeus.Academia.Api.Models.Responses;

/// <summary>
/// Generic API response wrapper with typed data payload
/// </summary>
/// <typeparam name="T">Type of response data</typeparam>
/// <remarks>
/// Extends the base ApiResponse to include strongly-typed data.
/// Supports both JSON and XML serialization for content negotiation.
/// </remarks>
[XmlRoot("Response")]
public class ApiResponse<T> : ApiResponse
{
    /// <summary>
    /// Response data payload
    /// </summary>
    [JsonPropertyName("data")]
    [XmlElement("Data")]
    public T? Data { get; set; }

    /// <summary>
    /// Creates a successful response with data
    /// </summary>
    /// <param name="data">Response data</param>
    /// <param name="message">Success message</param>
    /// <param name="correlationId">Request correlation ID</param>
    /// <param name="version">API version</param>
    /// <returns>Success response with data</returns>
    public static ApiResponse<T> CreateSuccess(T data, string? message = null, string? correlationId = null, string? version = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message,
            CorrelationId = correlationId,
            Version = version,
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates an error response without data
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="errors">Detailed error information</param>
    /// <param name="correlationId">Request correlation ID</param>
    /// <param name="version">API version</param>
    /// <returns>Error response</returns>
    public static new ApiResponse<T> CreateError(string message, object? errors = null, string? correlationId = null, string? version = null)
    {
        return new ApiResponse<T>
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