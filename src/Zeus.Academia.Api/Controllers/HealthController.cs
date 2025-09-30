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
    private readonly AcademiaDbContext _context;

    public HealthController(AcademiaDbContext context)
    {
        _context = context;
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
    /// Database connectivity health check
    /// </summary>
    /// <returns>Database health status</returns>
    [HttpGet("database")]
    public async Task<IActionResult> DatabaseHealth()
    {
        try
        {
            var canConnect = await _context.Database.CanConnectAsync();
            return Ok(new
            {
                Status = canConnect ? "Healthy" : "Unhealthy",
                DatabaseConnected = canConnect,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Status = "Unhealthy",
                Error = ex.Message,
                Timestamp = DateTime.UtcNow
            });
        }
    }
}