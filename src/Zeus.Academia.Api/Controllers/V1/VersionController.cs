using Microsoft.AspNetCore.Mvc;

namespace Zeus.Academia.Api.Controllers.V1;

/// <summary>
/// Version 1.0 API Controller - Example of versioned endpoint
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class VersionController : ControllerBase
{
    /// <summary>
    /// Gets the API version information
    /// </summary>
    /// <returns>Version information for API v1.0</returns>
    [HttpGet]
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
                "Basic Academic Operations"
            },
            Timestamp = DateTime.UtcNow
        };

        return await Task.FromResult(Ok(versionInfo));
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