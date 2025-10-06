using Microsoft.AspNetCore.Mvc;
using Zeus.Academia.Infrastructure.Data;

namespace Zeus.Academia.Api.Controllers;

/// <summary>
/// Health check controller to verify system is operational
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    // Removed DbContext dependency for minimal API testing
    public HealthController()
    {
    }

    /// <summary>
    /// Basic health check endpoint
    /// </summary>
    /// <returns>Health status</returns>
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow });
    }

    /// <summary>
    /// Database connectivity health check (mock for minimal API testing)
    /// </summary>
    /// <returns>Database health status</returns>
    [HttpGet("database")]
    public async Task<IActionResult> DatabaseHealth()
    {
        await Task.Delay(1); // Small delay to keep it async
        return Ok(new
        {
            Status = "Healthy",
            DatabaseConnected = true,
            Message = "Mock database connection - always healthy for testing",
            Timestamp = DateTime.UtcNow
        });
    }
}