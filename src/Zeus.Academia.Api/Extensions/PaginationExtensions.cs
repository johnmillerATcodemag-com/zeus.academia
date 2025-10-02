using Zeus.Academia.Api.Models.Requests;
using Zeus.Academia.Api.Models.Responses;

namespace Zeus.Academia.Api.Extensions;

/// <summary>
/// Extension methods for implementing pagination on IQueryable and IEnumerable collections
/// </summary>
public static class PaginationExtensions
{
    /// <summary>
    /// Applies pagination to an IQueryable collection
    /// </summary>
    /// <typeparam name="T">Type of items in the collection</typeparam>
    /// <param name="source">Source queryable collection</param>
    /// <param name="parameters">Pagination parameters</param>
    /// <returns>Paged result with items and metadata</returns>
    public static PagedResult<T> ToPaginatedList<T>(
        this IQueryable<T> source,
        PaginationParameters parameters)
    {
        var totalItems = source.Count();
        var items = source
            .Skip(parameters.Skip)
            .Take(parameters.Take)
            .ToList();

        var pagination = PaginationMetadata.Create(parameters.Page, parameters.PageSize, totalItems);

        return new PagedResult<T>
        {
            Items = items,
            Pagination = pagination
        };
    }

    /// <summary>
    /// Applies pagination to an IEnumerable collection
    /// </summary>
    /// <typeparam name="T">Type of items in the collection</typeparam>
    /// <param name="source">Source enumerable collection</param>
    /// <param name="parameters">Pagination parameters</param>
    /// <returns>Paged result with items and metadata</returns>
    public static PagedResult<T> ToPaginatedList<T>(
        this IEnumerable<T> source,
        PaginationParameters parameters)
    {
        var sourceList = source.ToList();
        var totalItems = sourceList.Count;
        var items = sourceList
            .Skip(parameters.Skip)
            .Take(parameters.Take)
            .ToList();

        var pagination = PaginationMetadata.Create(parameters.Page, parameters.PageSize, totalItems);

        return new PagedResult<T>
        {
            Items = items,
            Pagination = pagination
        };
    }

    /// <summary>
    /// Creates a paged API response from a paged result
    /// </summary>
    /// <typeparam name="T">Type of items in the collection</typeparam>
    /// <param name="pagedResult">Paged result</param>
    /// <param name="message">Optional success message</param>
    /// <param name="correlationId">Request correlation ID</param>
    /// <param name="version">API version</param>
    /// <returns>Paged API response</returns>
    public static PagedApiResponse<T> ToApiResponse<T>(
        this PagedResult<T> pagedResult,
        string? message = null,
        string? correlationId = null,
        string? version = null)
    {
        return PagedApiResponse<T>.CreateSuccess(
            pagedResult.Items,
            pagedResult.Pagination,
            message,
            correlationId,
            version);
    }

    /// <summary>
    /// Creates a paged API response directly from a queryable
    /// </summary>
    /// <typeparam name="T">Type of items in the collection</typeparam>
    /// <param name="source">Source queryable collection</param>
    /// <param name="parameters">Pagination parameters</param>
    /// <param name="message">Optional success message</param>
    /// <param name="correlationId">Request correlation ID</param>
    /// <param name="version">API version</param>
    /// <returns>Paged API response</returns>
    public static PagedApiResponse<T> ToPagedApiResponse<T>(
        this IQueryable<T> source,
        PaginationParameters parameters,
        string? message = null,
        string? correlationId = null,
        string? version = null)
    {
        var pagedResult = source.ToPaginatedList(parameters);
        return pagedResult.ToApiResponse(message, correlationId, version);
    }

    /// <summary>
    /// Creates a paged API response directly from an enumerable
    /// </summary>
    /// <typeparam name="T">Type of items in the collection</typeparam>
    /// <param name="source">Source enumerable collection</param>
    /// <param name="parameters">Pagination parameters</param>
    /// <param name="message">Optional success message</param>
    /// <param name="correlationId">Request correlation ID</param>
    /// <param name="version">API version</param>
    /// <returns>Paged API response</returns>
    public static PagedApiResponse<T> ToPagedApiResponse<T>(
        this IEnumerable<T> source,
        PaginationParameters parameters,
        string? message = null,
        string? correlationId = null,
        string? version = null)
    {
        var pagedResult = source.ToPaginatedList(parameters);
        return pagedResult.ToApiResponse(message, correlationId, version);
    }
}

/// <summary>
/// Container for paged query results
/// </summary>
/// <typeparam name="T">Type of items in the paged result</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// Items for the current page
    /// </summary>
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

    /// <summary>
    /// Pagination metadata
    /// </summary>
    public PaginationMetadata Pagination { get; set; } = null!;
}