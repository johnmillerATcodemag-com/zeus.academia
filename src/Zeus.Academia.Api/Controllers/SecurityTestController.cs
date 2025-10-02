using Microsoft.AspNetCore.Mvc;

namespace Zeus.Academia.Api.Controllers;

/// <summary>
/// Test controller for validating security middleware functionality.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SecurityTestController : ControllerBase
{
    /// <summary>
    /// Test endpoint for verifying security headers are applied.
    /// </summary>
    [HttpGet("headers")]
    public IActionResult TestSecurityHeaders()
    {
        return Ok(new
        {
            message = "Security headers should be present in response",
            timestamp = DateTime.UtcNow,
            headers = Response.Headers.Select(h => new { h.Key, Value = h.Value.ToString() })
        });
    }

    /// <summary>
    /// Test endpoint for simulating authentication scenarios (rate limiting test).
    /// </summary>
    [HttpPost("auth-test")]
    public IActionResult TestAuthentication([FromBody] object? request)
    {
        // Simulate authentication endpoint for testing rate limiting
        return Ok(new
        {
            message = "This endpoint simulates authentication for rate limiting tests",
            timestamp = DateTime.UtcNow,
            clientIp = HttpContext.Connection.RemoteIpAddress?.ToString()
        });
    }

    /// <summary>
    /// Test endpoint for CORS verification.
    /// </summary>
    [HttpGet("cors-test")]
    public IActionResult TestCors()
    {
        return Ok(new
        {
            message = "CORS should allow configured origins",
            timestamp = DateTime.UtcNow,
            origin = Request.Headers["Origin"].ToString()
        });
    }
}