using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Zeus.Academia.Api.Controllers.V2;

/// <summary>
/// Version 2.0 API Controller - Enhanced version with more features
/// </summary>
[ApiController]
[Route("api/v2/[controller]")]
public class VersionController : ControllerBase
{
    /// <summary>
    /// Gets the API version information with enhanced details
    /// </summary>
    /// <returns>Enhanced version information for API v2.0</returns>
    [HttpGet]
    public async Task<IActionResult> GetVersionAsync()
    {
        var versionInfo = new
        {
            Version = "2.0",
            ApiName = "Zeus Academia API",
            Description = "Enhanced academic management system API with advanced features",
            Features = new[]
            {
                "Advanced User Management",
                "Multi-Factor Authentication",
                "Academic Operations",
                "Reporting & Analytics",
                "Integration APIs",
                "Advanced Security"
            },
            Capabilities = new
            {
                MaxFileUploadSize = "100MB",
                SupportedFormats = new[] { "JSON", "XML", "CSV" },
                RateLimiting = true,
                Caching = true,
                Monitoring = true
            },
            Timestamp = DateTime.UtcNow,
            BuildNumber = Environment.GetEnvironmentVariable("BUILD_NUMBER") ?? "dev",
            CommitHash = Environment.GetEnvironmentVariable("COMMIT_HASH") ?? "local"
        };

        return await Task.FromResult(Ok(versionInfo));
    }

    /// <summary>
    /// Gets comprehensive server health status for v2.0
    /// </summary>
    /// <returns>Detailed health information with metrics</returns>
    [HttpGet("health")]
    public async Task<IActionResult> GetHealthAsync()
    {
        var healthInfo = new
        {
            Status = "Healthy",
            Version = "2.0",
            Timestamp = DateTime.UtcNow,
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
            SystemInfo = new
            {
                MachineName = Environment.MachineName,
                ProcessorCount = Environment.ProcessorCount,
                WorkingSet = GC.GetTotalMemory(false),
                OSVersion = Environment.OSVersion.ToString(),
                Framework = Environment.Version.ToString()
            },
            ApiMetrics = new
            {
                Uptime = DateTime.UtcNow.Subtract(Process.GetCurrentProcess().StartTime.ToUniversalTime()),
                ThreadCount = Process.GetCurrentProcess().Threads.Count,
                HandleCount = Process.GetCurrentProcess().HandleCount
            }
        };

        return await Task.FromResult(Ok(healthInfo));
    }
}