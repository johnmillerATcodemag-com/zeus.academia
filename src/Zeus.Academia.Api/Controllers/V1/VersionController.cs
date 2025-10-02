using Microsoft.AspNetCore.Mvc;
using Zeus.Academia.Api.Versioning;

namespace Zeus.Academia.Api.Controllers.V1;

/// <summary>
/// Version 1.0 API Controller - Example of versioned endpoint
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Route("api/[controller]")] // Also support header-based versioning
[ApiVersion("1.0")]
public class VersionController : VersionedApiController
{
    /// <summary>
    /// Gets the API version information
    /// </summary>
    /// <returns>Version information for API v1.0</returns>
    /// <response code="200">Returns the version information</response>
    [HttpGet]
    [ProducesResponseType(typeof(object), 200)]
    public async Task<IActionResult> GetVersionAsync()
    {
        var versionInfo = new
        {
            Version = "1.0",
            ApiName = "Zeus Academia API",
            Description = "Core academic management system API",
            Features = new[]
            {
                "User Management",
                "Authentication",
                "Basic Academic Operations",
                "Input Validation",
                "Global Exception Handling"
            },
            SupportedVersioning = new[]
            {
                "Header-based (X-API-Version)",
                "Query parameter (version)",
                "URL-based (/api/v1/)"
            }
        };

        return await Task.FromResult(VersionedResponse(versionInfo, "Version 1.0 information retrieved successfully"));
    }

    /// <summary>
    /// Gets server health status for v1.0
    /// </summary>
    /// <returns>Basic health information</returns>
    [HttpGet("health")]
    public async Task<IActionResult> GetHealthAsync()
    {
        var healthInfo = new
        {
            Status = "Healthy",
            Version = "1.0",
            Timestamp = DateTime.UtcNow,
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"
        };

        return await Task.FromResult(Ok(healthInfo));
    }
}