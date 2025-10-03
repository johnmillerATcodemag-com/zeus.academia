using System.Text.Json.Serialization;
using System.Xml.Serialization;
using Zeus.Academia.Api.Models.Common;

namespace Zeus.Academia.Api.Models.Responses;

/// <summary>
/// Legacy paged API response wrapper - use PagedResponse&lt;T&gt; from Common instead
/// </summary>
/// <typeparam name="T">Type of items in the paged collection</typeparam>
[Obsolete("Use PagedResponse<T> from Zeus.Academia.Api.Models.Common instead")]
[XmlRoot("PagedResponse")]
public class PagedApiResponse<T> : ApiResponse<IEnumerable<T>>
{
    /// <summary>
    /// Pagination metadata
    /// </summary>
    [JsonPropertyName("pagination")]
    [XmlElement("Pagination")]
    public PaginationMetadata? Pagination { get; set; }

    /// <summary>
    /// Creates a successful paged response
    /// </summary>
    /// <param name="data">Page data</param>
    /// <param name="pagination">Pagination metadata</param>
    /// <param name="message">Success message</param>
    /// <param name="correlationId">Request correlation ID</param>
    /// <param name="version">API version</param>
    /// <returns>Successful paged response</returns>
    public static PagedApiResponse<T> CreateSuccess(
        IEnumerable<T> data,
        PaginationMetadata pagination,
        string? message = null,
        string? correlationId = null,
        string? version = null)
    {
        return new PagedApiResponse<T>
        {
            Success = true,
            Data = data,
            Pagination = pagination,
            Message = message,
            CorrelationId = correlationId,
            Version = version,
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates a successful paged response from pagination parameters
    /// </summary>
    /// <param name="data">Page data</param>
    /// <param name="currentPage">Current page number (1-based)</param>
    /// <param name="pageSize">Items per page</param>
    /// <param name="totalItems">Total number of items</param>
    /// <param name="message">Success message</param>
    /// <param name="correlationId">Request correlation ID</param>
    /// <param name="version">API version</param>
    /// <returns>Successful paged response</returns>
    public static PagedApiResponse<T> CreateSuccess(
        IEnumerable<T> data,
        int currentPage,
        int pageSize,
        int totalItems,
        string? message = null,
        string? correlationId = null,
        string? version = null)
    {
        var pagination = PaginationMetadata.Create(currentPage, pageSize, totalItems);
        return CreateSuccess(data, pagination, message, correlationId, version);
    }

    /// <summary>
    /// Creates an error paged response
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="errors">Detailed error information</param>
    /// <param name="correlationId">Request correlation ID</param>
    /// <param name="version">API version</param>
    /// <returns>Error paged response</returns>
    public static new PagedApiResponse<T> CreateError(string message, Dictionary<string, string[]>? errors = null, string? correlationId = null, string? version = null)
    {
        return new PagedApiResponse<T>
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