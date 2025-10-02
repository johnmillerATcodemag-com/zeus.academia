namespace Zeus.Academia.Api.Versioning;

/// <summary>
/// Service for managing API versioning information
/// </summary>
public interface IApiVersionService
{
    /// <summary>
    /// Gets all available API versions
    /// </summary>
    /// <returns>List of available versions</returns>
    IEnumerable<ApiVersionInfo> GetAvailableVersions();

    /// <summary>
    /// Gets the current API version from the request context
    /// </summary>
    /// <param name="httpContext">The HTTP context</param>
    /// <returns>The current version or default if not specified</returns>
    string GetCurrentVersion(HttpContext httpContext);

    /// <summary>
    /// Gets the latest available API version
    /// </summary>
    /// <returns>The latest version</returns>
    string GetLatestVersion();

    /// <summary>
    /// Checks if a version is deprecated
    /// </summary>
    /// <param name="version">The version to check</param>
    /// <returns>True if deprecated, false otherwise</returns>
    bool IsVersionDeprecated(string version);
}

/// <summary>
/// Implementation of the API versioning service
/// </summary>
public class ApiVersionService : IApiVersionService
{
    private readonly List<ApiVersionInfo> _availableVersions;
    private readonly string _defaultVersion;

    /// <summary>
    /// Initializes a new instance of the ApiVersionService
    /// </summary>
    public ApiVersionService()
    {
        _defaultVersion = "1.0";
        _availableVersions = new List<ApiVersionInfo>
        {
            new("1.0", "Initial release with core functionality", false, DateTime.Parse("2024-01-01")),
            new("1.1", "Enhanced user management and validation", false, DateTime.Parse("2024-06-01")),
            new("2.0", "Major update with advanced features", false, DateTime.Parse("2024-10-01"))
        };
    }

    /// <inheritdoc />
    public IEnumerable<ApiVersionInfo> GetAvailableVersions()
    {
        return _availableVersions.AsReadOnly();
    }

    /// <inheritdoc />
    public string GetCurrentVersion(HttpContext httpContext)
    {
        if (httpContext.Items.TryGetValue("ApiVersion", out var version) && version is string versionString)
        {
            return versionString;
        }

        // Check request headers
        if (httpContext.Request.Headers.TryGetValue("X-API-Version", out var headerValue))
        {
            var requestedVersion = headerValue.FirstOrDefault();
            if (!string.IsNullOrEmpty(requestedVersion))
            {
                return requestedVersion;
            }
        }

        return _defaultVersion;
    }

    /// <inheritdoc />
    public string GetLatestVersion()
    {
        return _availableVersions
            .Where(v => !v.IsDeprecated)
            .OrderByDescending(v => v.ReleaseDate)
            .Select(v => v.Version)
            .FirstOrDefault() ?? _defaultVersion;
    }

    /// <inheritdoc />
    public bool IsVersionDeprecated(string version)
    {
        return _availableVersions
            .FirstOrDefault(v => v.Version == version)?.IsDeprecated ?? false;
    }
}

/// <summary>
/// Information about an API version
/// </summary>
/// <param name="Version">The version number</param>
/// <param name="Description">Description of the version</param>
/// <param name="IsDeprecated">Whether the version is deprecated</param>
/// <param name="ReleaseDate">When the version was released</param>
public record ApiVersionInfo(
    string Version,
    string Description,
    bool IsDeprecated,
    DateTime ReleaseDate
);