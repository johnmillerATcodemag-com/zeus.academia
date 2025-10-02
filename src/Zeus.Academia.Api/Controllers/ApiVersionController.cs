using Microsoft.AspNetCore.Mvc;
using Zeus.Academia.Api.Versioning;

namespace Zeus.Academia.Api.Controllers;

/// <summary>
/// API Version Discovery Controller
/// </summary>
[ApiController]
[Route("api/versions")]
[Route("api/version")]
public class ApiVersionController : ControllerBase
{
    private readonly IApiVersionService _versionService;

    /// <summary>
    /// Initializes a new instance of the ApiVersionController
    /// </summary>
    /// <param name="versionService">The API version service</param>
    public ApiVersionController(IApiVersionService versionService)
    {
        _versionService = versionService ?? throw new ArgumentNullException(nameof(versionService));
    }

    /// <summary>
    /// Gets all available API versions and their information
    /// </summary>
    /// <returns>List of available API versions with details</returns>
    /// <response code="200">Returns the list of available API versions</response>
    [HttpGet]
    [ProducesResponseType(typeof(object), 200)]
    public async Task<IActionResult> GetAvailableVersionsAsync()
    {
        var versions = _versionService.GetAvailableVersions();
        var currentVersion = _versionService.GetLatestVersion();

        var response = new
        {
            ApiName = "Zeus Academia API",
            CurrentVersion = currentVersion,
            TotalVersions = versions.Count(),
            SupportedVersions = versions.Select(v => new
            {
                v.Version,
                v.Description,
                v.IsDeprecated,
                ReleaseDate = v.ReleaseDate.ToString("yyyy-MM-dd"),
                IsCurrent = v.Version == currentVersion,
                Status = v.IsDeprecated ? "Deprecated" : (v.Version == currentVersion ? "Current" : "Supported")
            }).OrderByDescending(v => v.ReleaseDate),
            VersioningMethods = new
            {
                Header = new
                {
                    Name = "X-API-Version",
                    Description = "Preferred method - specify version in request header",
                    Example = "X-API-Version: 2.0"
                },
                QueryParameter = new
                {
                    Name = "version",
                    Description = "Alternative method - specify version as query parameter",
                    Example = "?version=2.0"
                },
                UrlPath = new
                {
                    Description = "Legacy method - specify version in URL path",
                    Example = "/api/v2/controller"
                }
            },
            Documentation = new
            {
                SwaggerUI = "/swagger",
                OpenAPISpec = "/swagger/v1/swagger.json"
            }
        };

        return await Task.FromResult(Ok(response));
    }

    /// <summary>
    /// Gets the current API version being used by the client
    /// </summary>
    /// <returns>Current version information</returns>
    /// <response code="200">Returns the current version information</response>
    [HttpGet("current")]
    [ProducesResponseType(typeof(object), 200)]
    public async Task<IActionResult> GetCurrentVersionAsync()
    {
        var currentVersion = _versionService.GetCurrentVersion(HttpContext);
        var versionInfo = _versionService.GetAvailableVersions()
            .FirstOrDefault(v => v.Version == currentVersion);

        var response = new
        {
            Version = currentVersion,
            Description = versionInfo?.Description ?? "Unknown version",
            IsDeprecated = versionInfo?.IsDeprecated ?? false,
            ReleaseDate = versionInfo?.ReleaseDate.ToString("yyyy-MM-dd") ?? "Unknown",
            IsLatest = currentVersion == _versionService.GetLatestVersion(),
            RequestMethod = GetVersionRequestMethod(),
            Timestamp = DateTime.UtcNow
        };

        return await Task.FromResult(Ok(response));
    }

    /// <summary>
    /// Determines how the version was requested
    /// </summary>
    /// <returns>The version request method</returns>
    private string GetVersionRequestMethod()
    {
        if (Request.Headers.ContainsKey("X-API-Version"))
        {
            return "Header (X-API-Version)";
        }

        if (Request.Query.ContainsKey("version"))
        {
            return "Query Parameter";
        }

        if (Request.Path.Value?.Contains("/v") == true)
        {
            return "URL Path";
        }

        return "Default (Latest)";
    }
}