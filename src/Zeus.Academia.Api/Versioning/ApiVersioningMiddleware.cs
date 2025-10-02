using System.Text.Json;

namespace Zeus.Academia.Api.Versioning;

/// <summary>
/// Middleware to handle API versioning
/// </summary>
public class ApiVersioningMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiVersioningMiddleware> _logger;
    private readonly IApiVersionService _versionService;

    /// <summary>
    /// Initializes a new instance of the ApiVersioningMiddleware
    /// </summary>
    /// <param name="next">The next middleware in the pipeline</param>
    /// <param name="logger">The logger</param>
    /// <param name="versionService">The API version service</param>
    public ApiVersioningMiddleware(
        RequestDelegate next,
        ILogger<ApiVersioningMiddleware> logger,
        IApiVersionService versionService)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _versionService = versionService ?? throw new ArgumentNullException(nameof(versionService));
    }

    /// <summary>
    /// Invokes the middleware
    /// </summary>
    /// <param name="context">The HTTP context</param>
    /// <returns>A task representing the async operation</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Skip versioning for non-API routes
            if (!IsApiRoute(context.Request.Path))
            {
                await _next(context);
                return;
            }

            // Handle version discovery endpoint
            if (IsVersionDiscoveryRoute(context.Request.Path))
            {
                await HandleVersionDiscoveryAsync(context);
                return;
            }

            // Process API versioning
            var shouldContinue = await ProcessVersioningAsync(context);
            if (shouldContinue)
            {
                await _next(context);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in API versioning middleware");
            await HandleVersioningErrorAsync(context, ex);
        }
    }

    /// <summary>
    /// Processes API versioning for the request
    /// </summary>
    /// <param name="context">The HTTP context</param>
    /// <returns>True if processing should continue, false if the request was handled</returns>
    private async Task<bool> ProcessVersioningAsync(HttpContext context)
    {
        var requestedVersion = GetRequestedVersion(context);
        var availableVersions = _versionService.GetAvailableVersions().Select(v => v.Version).ToList();

        // If no version specified, use latest
        if (string.IsNullOrEmpty(requestedVersion))
        {
            requestedVersion = _versionService.GetLatestVersion();
            context.Items["ApiVersion"] = requestedVersion;
            _logger.LogDebug("No version specified, using latest version {Version}", requestedVersion);
        }
        else
        {
            // Validate requested version
            if (!availableVersions.Contains(requestedVersion))
            {
                await HandleUnsupportedVersionAsync(context, requestedVersion, availableVersions);
                return false;
            }

            context.Items["ApiVersion"] = requestedVersion;
            _logger.LogDebug("Using requested version {Version}", requestedVersion);
        }

        // Add version headers to response
        context.Response.Headers["X-API-Version"] = requestedVersion;
        context.Response.Headers["X-API-Supported-Versions"] = string.Join(", ", availableVersions);

        // Check for deprecated version
        if (_versionService.IsVersionDeprecated(requestedVersion))
        {
            context.Response.Headers["X-API-Deprecated"] = "true";
            context.Response.Headers["X-API-Latest-Version"] = _versionService.GetLatestVersion();
            _logger.LogWarning("Client is using deprecated API version {Version}", requestedVersion);
        }

        return true;
    }

    /// <summary>
    /// Handles the version discovery endpoint
    /// </summary>
    /// <param name="context">The HTTP context</param>
    private async Task HandleVersionDiscoveryAsync(HttpContext context)
    {
        var versions = _versionService.GetAvailableVersions();
        var response = new
        {
            ApiName = "Zeus Academia API",
            CurrentVersion = _versionService.GetLatestVersion(),
            SupportedVersions = versions.Select(v => new
            {
                v.Version,
                v.Description,
                v.IsDeprecated,
                ReleaseDate = v.ReleaseDate.ToString("yyyy-MM-dd"),
                IsCurrent = v.Version == _versionService.GetLatestVersion()
            })
        };

        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        }));
    }

    /// <summary>
    /// Handles unsupported version requests
    /// </summary>
    /// <param name="context">The HTTP context</param>
    /// <param name="requestedVersion">The requested version</param>
    /// <param name="availableVersions">Available versions</param>
    private async Task HandleUnsupportedVersionAsync(HttpContext context, string requestedVersion, List<string> availableVersions)
    {
        context.Response.StatusCode = 400;
        context.Response.ContentType = "application/json";

        var errorResponse = new
        {
            Error = "Unsupported API Version",
            Message = $"API version '{requestedVersion}' is not supported",
            RequestedVersion = requestedVersion,
            SupportedVersions = availableVersions,
            LatestVersion = _versionService.GetLatestVersion()
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        }));

        _logger.LogWarning("Unsupported API version requested: {RequestedVersion}. Available: {AvailableVersions}",
            requestedVersion, string.Join(", ", availableVersions));
    }

    /// <summary>
    /// Handles versioning errors
    /// </summary>
    /// <param name="context">The HTTP context</param>
    /// <param name="exception">The exception</param>
    private async Task HandleVersioningErrorAsync(HttpContext context, Exception exception)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var errorResponse = new
        {
            Error = "API Versioning Error",
            Message = "An error occurred while processing API version information"
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        }));
    }

    /// <summary>
    /// Gets the requested version from the request
    /// </summary>
    /// <param name="context">The HTTP context</param>
    /// <returns>The requested version or null</returns>
    private static string? GetRequestedVersion(HttpContext context)
    {
        // Priority: Header > Query > Route
        if (context.Request.Headers.TryGetValue("X-API-Version", out var headerValue))
        {
            return headerValue.FirstOrDefault();
        }

        if (context.Request.Query.TryGetValue("version", out var queryValue))
        {
            return queryValue.FirstOrDefault();
        }

        return null;
    }

    /// <summary>
    /// Determines if the request is for an API route
    /// </summary>
    /// <param name="path">The request path</param>
    /// <returns>True if API route, false otherwise</returns>
    private static bool IsApiRoute(PathString path)
    {
        return path.StartsWithSegments("/api");
    }

    /// <summary>
    /// Determines if the request is for version discovery
    /// </summary>
    /// <param name="path">The request path</param>
    /// <returns>True if version discovery route, false otherwise</returns>
    private static bool IsVersionDiscoveryRoute(PathString path)
    {
        return path.StartsWithSegments("/api/versions") || path.StartsWithSegments("/api/version");
    }
}