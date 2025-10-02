using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Zeus.Academia.Api.Models.Responses;

/// <summary>
/// Pagination metadata for paged API responses
/// </summary>
/// <remarks>
/// Provides comprehensive pagination information including current page,
/// total counts, and navigation flags for client applications.
/// </remarks>
[XmlRoot("Pagination")]
public class PaginationMetadata
{
    /// <summary>
    /// Current page number (1-based)
    /// </summary>
    [JsonPropertyName("currentPage")]
    [XmlElement("CurrentPage")]
    public int CurrentPage { get; set; }

    /// <summary>
    /// Number of items per page
    /// </summary>
    [JsonPropertyName("pageSize")]
    [XmlElement("PageSize")]
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of items across all pages
    /// </summary>
    [JsonPropertyName("totalItems")]
    [XmlElement("TotalItems")]
    public int TotalItems { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    [JsonPropertyName("totalPages")]
    [XmlElement("TotalPages")]
    public int TotalPages { get; set; }

    /// <summary>
    /// Indicates if there is a previous page
    /// </summary>
    [JsonPropertyName("hasPreviousPage")]
    [XmlElement("HasPreviousPage")]
    public bool HasPreviousPage { get; set; }

    /// <summary>
    /// Indicates if there is a next page
    /// </summary>
    [JsonPropertyName("hasNextPage")]
    [XmlElement("HasNextPage")]
    public bool HasNextPage { get; set; }

    /// <summary>
    /// Previous page number (null if no previous page)
    /// </summary>
    [JsonPropertyName("previousPage")]
    [XmlElement("PreviousPage")]
    public int? PreviousPage { get; set; }

    /// <summary>
    /// Next page number (null if no next page)
    /// </summary>
    [JsonPropertyName("nextPage")]
    [XmlElement("NextPage")]
    public int? NextPage { get; set; }

    /// <summary>
    /// First item index on current page (0-based)
    /// </summary>
    [JsonPropertyName("firstItemIndex")]
    [XmlElement("FirstItemIndex")]
    public int FirstItemIndex { get; set; }

    /// <summary>
    /// Last item index on current page (0-based)
    /// </summary>
    [JsonPropertyName("lastItemIndex")]
    [XmlElement("LastItemIndex")]
    public int LastItemIndex { get; set; }

    /// <summary>
    /// Creates pagination metadata from pagination parameters and total count
    /// </summary>
    /// <param name="currentPage">Current page number (1-based)</param>
    /// <param name="pageSize">Items per page</param>
    /// <param name="totalItems">Total number of items</param>
    /// <returns>Pagination metadata</returns>
    public static PaginationMetadata Create(int currentPage, int pageSize, int totalItems)
    {
        var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
        var hasPreviousPage = currentPage > 1;
        var hasNextPage = currentPage < totalPages;
        var firstItemIndex = (currentPage - 1) * pageSize;
        var lastItemIndex = Math.Min(firstItemIndex + pageSize - 1, totalItems - 1);

        return new PaginationMetadata
        {
            CurrentPage = currentPage,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages,
            HasPreviousPage = hasPreviousPage,
            HasNextPage = hasNextPage,
            PreviousPage = hasPreviousPage ? currentPage - 1 : null,
            NextPage = hasNextPage ? currentPage + 1 : null,
            FirstItemIndex = firstItemIndex,
            LastItemIndex = lastItemIndex < 0 ? 0 : lastItemIndex
        };
    }
}