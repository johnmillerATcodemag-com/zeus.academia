using Zeus.Academia.Api.Models.Common;
using Zeus.Academia.Api.Models.Requests;
using Zeus.Academia.Api.Models.Responses;

namespace Zeus.Academia.Api.Extensions;

/// <summary>
/// Extension methods for implementing pagination on collections
/// </summary>
public static class PaginationHelper
{
    /// <summary>
    /// Converts a collection to a paginated list
    /// </summary>
    /// <typeparam name="T">Type of items in the collection</typeparam>
    /// <param name="source">Source collection</param>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>Paginated result</returns>
    public static PagedResult<T> ToPaginatedList<T>(
        this IQueryable<T> source,
        int pageNumber,
        int pageSize)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        var totalCount = source.Count();
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        var items = source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<T>
        {
            Items = items,
            Pagination = PaginationMetadata.Create(pageNumber, pageSize, totalCount)
        };
    }

    /// <summary>
    /// Converts a collection to a paginated list using pagination parameters
    /// </summary>
    /// <typeparam name="T">Type of items in the collection</typeparam>
    /// <param name="source">Source collection</param>
    /// <param name="parameters">Pagination parameters</param>
    /// <returns>Paginated result</returns>
    public static PagedResult<T> ToPaginatedList<T>(
        this IQueryable<T> source,
        Zeus.Academia.Api.Models.Common.PaginationParameters parameters)
    {
        return source.ToPaginatedList(parameters.PageNumber, parameters.PageSize);
    }

    /// <summary>
    /// Converts a PagedResult to a PagedResponse
    /// </summary>
    /// <typeparam name="T">Type of items in the collection</typeparam>
    /// <param name="pagedResult">The paged result to convert</param>
    /// <returns>PagedResponse</returns>
    public static PagedResponse<T> ToPagedResponse<T>(
        this PagedResult<T> pagedResult)
    {
        var pagination = pagedResult.Pagination ?? new PaginationMetadata();
        return new PagedResponse<T>
        {
            Data = pagedResult.Items,
            PageNumber = pagination.CurrentPage,
            PageSize = pagination.PageSize,
            TotalCount = pagination.TotalItems,
            TotalPages = pagination.TotalPages,
            HasPreviousPage = pagination.HasPreviousPage,
            HasNextPage = pagination.HasNextPage
        };
    }
}

/// <summary>
/// Represents a paginated result
/// </summary>
/// <typeparam name="T">Type of items</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// Items for the current page
    /// </summary>
    public IEnumerable<T> Items { get; set; } = new List<T>();

    /// <summary>
    /// Pagination metadata
    /// </summary>
    public PaginationMetadata? Pagination { get; set; }
}