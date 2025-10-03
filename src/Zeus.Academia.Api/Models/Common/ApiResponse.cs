namespace Zeus.Academia.Api.Models.Common;

/// <summary>
/// Standard API response model.
/// </summary>
public class ApiResponse
{
    /// <summary>
    /// Indicates whether the operation was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Response message.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Validation or other errors.
    /// </summary>
    public Dictionary<string, string[]>? Errors { get; set; }

    /// <summary>
    /// Response timestamp.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Request trace identifier.
    /// </summary>
    public string? TraceId { get; set; }

    /// <summary>
    /// Correlation identifier for tracking requests across services.
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// API version.
    /// </summary>
    public string? Version { get; set; }

    /// <summary>
    /// Creates a successful API response.
    /// </summary>
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
    /// Creates an error API response.
    /// </summary>
    public static ApiResponse CreateError(string message, Dictionary<string, string[]>? errors = null, string? correlationId = null, string? version = null)
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

/// <summary>
/// API response model with data.
/// </summary>
/// <typeparam name="T">Type of the response data.</typeparam>
public class ApiResponse<T> : ApiResponse
{
    /// <summary>
    /// Response data.
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Creates a successful API response with data.
    /// </summary>
    public static ApiResponse<T> CreateSuccess(T data, string? message = null, string? correlationId = null, string? version = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            CorrelationId = correlationId,
            Version = version,
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates an error API response with data.
    /// </summary>
    public static new ApiResponse<T> CreateError(string message, Dictionary<string, string[]>? errors = null, string? correlationId = null, string? version = null)
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

/// <summary>
/// Paginated API response model.
/// </summary>
/// <typeparam name="T">Type of the response data items.</typeparam>
public class PagedResponse<T>
{
    /// <summary>
    /// The paginated data items.
    /// </summary>
    public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();

    /// <summary>
    /// Current page number (1-based).
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Number of items per page.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of items across all pages.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Total number of pages.
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Indicates whether there is a previous page.
    /// </summary>
    public bool HasPreviousPage { get; set; }

    /// <summary>
    /// Indicates whether there is a next page.
    /// </summary>
    public bool HasNextPage { get; set; }
}

/// <summary>
/// Base pagination parameters.
/// </summary>
public class PaginationParameters
{
    private const int MaxPageSize = 100;
    private int _pageSize = 10;

    /// <summary>
    /// Page number (1-based). Default is 1.
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Page size. Default is 10, maximum is 100.
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }
}