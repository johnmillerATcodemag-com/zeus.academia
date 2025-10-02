using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Zeus.Academia.Api.Models.Requests;

/// <summary>
/// Pagination parameters for API requests
/// </summary>
/// <remarks>
/// Provides standardized pagination parameters with validation.
/// Supports both page-based (page/pageSize) and offset-based (skip/take) pagination.
/// </remarks>
public class PaginationParameters
{
    private int _page = 1;
    private int _pageSize = 20;
    private int _skip = 0;
    private int _take = 20;

    /// <summary>
    /// Page number (1-based)
    /// </summary>
    [FromQuery(Name = "page")]
    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
    public int Page
    {
        get => _page;
        set
        {
            _page = Math.Max(1, value);
            _skip = (_page - 1) * _pageSize;
        }
    }

    /// <summary>
    /// Number of items per page
    /// </summary>
    [FromQuery(Name = "pageSize")]
    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
    public int PageSize
    {
        get => _pageSize;
        set
        {
            _pageSize = Math.Clamp(value, 1, 100);
            _take = _pageSize;
            _skip = (_page - 1) * _pageSize;
        }
    }

    /// <summary>
    /// Number of items to skip (alternative to page-based pagination)
    /// </summary>
    [FromQuery(Name = "skip")]
    [Range(0, int.MaxValue, ErrorMessage = "Skip must be greater than or equal to 0")]
    public int Skip
    {
        get => _skip;
        set
        {
            _skip = Math.Max(0, value);
            _page = (_skip / _pageSize) + 1;
        }
    }

    /// <summary>
    /// Number of items to take (alternative to pageSize)
    /// </summary>
    [FromQuery(Name = "take")]
    [Range(1, 100, ErrorMessage = "Take must be between 1 and 100")]
    public int Take
    {
        get => _take;
        set
        {
            _take = Math.Clamp(value, 1, 100);
            _pageSize = _take;
            _page = (_skip / _pageSize) + 1;
        }
    }

    /// <summary>
    /// Sort field name
    /// </summary>
    [FromQuery(Name = "sortBy")]
    public string? SortBy { get; set; }

    /// <summary>
    /// Sort direction (asc or desc)
    /// </summary>
    [FromQuery(Name = "sortDirection")]
    public string SortDirection { get; set; } = "asc";

    /// <summary>
    /// Indicates if sorting is descending
    /// </summary>
    public bool IsSortDescending =>
        string.Equals(SortDirection, "desc", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(SortDirection, "descending", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Search query for filtering results
    /// </summary>
    [FromQuery(Name = "search")]
    public string? Search { get; set; }

    /// <summary>
    /// Creates default pagination parameters
    /// </summary>
    /// <returns>Default pagination parameters</returns>
    public static PaginationParameters Default() => new();

    /// <summary>
    /// Creates pagination parameters with specific page and size
    /// </summary>
    /// <param name="page">Page number (1-based)</param>
    /// <param name="pageSize">Items per page</param>
    /// <returns>Pagination parameters</returns>
    public static PaginationParameters Create(int page, int pageSize)
    {
        return new PaginationParameters
        {
            Page = page,
            PageSize = pageSize
        };
    }

    /// <summary>
    /// Creates pagination parameters with skip/take values
    /// </summary>
    /// <param name="skip">Items to skip</param>
    /// <param name="take">Items to take</param>
    /// <returns>Pagination parameters</returns>
    public static PaginationParameters CreateSkipTake(int skip, int take)
    {
        return new PaginationParameters
        {
            Skip = skip,
            Take = take
        };
    }
}