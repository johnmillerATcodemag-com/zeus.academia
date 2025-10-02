using Microsoft.AspNetCore.Mvc;
using Zeus.Academia.Api.Controllers;
using Zeus.Academia.Api.Services;

namespace Zeus.Academia.Api.Controllers.V1;

/// <summary>
/// Controller for testing and demonstrating request/response logging functionality.
/// This controller provides endpoints to verify that logging middleware captures
/// requests, responses, performance metrics, and correlation IDs correctly.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class LoggingTestController : BaseApiController
{
    private readonly ILogger<LoggingTestController> _logger;
    private readonly ICorrelationIdService _correlationIdService;

    public LoggingTestController(
        ILogger<LoggingTestController> logger,
        ICorrelationIdService correlationIdService)
    {
        _logger = logger;
        _correlationIdService = correlationIdService;
    }

    /// <summary>
    /// Simple GET endpoint to test basic request/response logging.
    /// </summary>
    /// <returns>A simple response with correlation ID</returns>
    [HttpGet("simple")]
    public async Task<IActionResult> SimpleRequest()
    {
        _logger.LogInformation("Processing simple request with correlation ID: {CorrelationId}",
            _correlationIdService.CorrelationId);

        await Task.Delay(100); // Simulate some processing time

        return Ok(new
        {
            Message = "Simple request processed successfully",
            CorrelationId = _correlationIdService.CorrelationId,
            Timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// POST endpoint to test request body logging.
    /// </summary>
    /// <param name="request">Test request payload</param>
    /// <returns>Response with correlation ID and processed data</returns>
    [HttpPost("with-body")]
    public async Task<IActionResult> RequestWithBody([FromBody] TestRequest request)
    {
        _logger.LogInformation("Processing request with body. Name: {Name}, Email: {Email}, CorrelationId: {CorrelationId}",
            request.Name, request.Email, _correlationIdService.CorrelationId);

        await Task.Delay(200); // Simulate processing time

        return Ok(new TestResponse
        {
            Id = Guid.NewGuid(),
            ProcessedName = request.Name?.ToUpper(),
            ProcessedEmail = request.Email?.ToLower(),
            ProcessedAt = DateTime.UtcNow,
            CorrelationId = _correlationIdService.CorrelationId
        });
    }

    /// <summary>
    /// Slow endpoint to test performance monitoring and slow request detection.
    /// </summary>
    /// <param name="delayMs">Delay in milliseconds to simulate slow processing</param>
    /// <returns>Response after specified delay</returns>
    [HttpGet("slow")]
    public async Task<IActionResult> SlowRequest([FromQuery] int delayMs = 2000)
    {
        _logger.LogInformation("Starting slow request with {DelayMs}ms delay, CorrelationId: {CorrelationId}",
            delayMs, _correlationIdService.CorrelationId);

        await Task.Delay(delayMs);

        _logger.LogInformation("Completed slow request after {DelayMs}ms, CorrelationId: {CorrelationId}",
            delayMs, _correlationIdService.CorrelationId);

        return Ok(new
        {
            Message = $"Slow request completed after {delayMs}ms",
            CorrelationId = _correlationIdService.CorrelationId,
            DelayMs = delayMs,
            Timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Endpoint that generates structured log entries with various log levels.
    /// </summary>
    /// <returns>Response confirming log generation</returns>
    [HttpGet("log-levels")]
    public async Task<IActionResult> LogLevels()
    {
        var correlationId = _correlationIdService.CorrelationId;

        _logger.LogDebug("Debug message for correlation ID: {CorrelationId}", correlationId);
        _logger.LogInformation("Information message for correlation ID: {CorrelationId}", correlationId);
        _logger.LogWarning("Warning message for correlation ID: {CorrelationId}", correlationId);

        await Task.Delay(50);

        return Ok(new
        {
            Message = "Various log levels generated",
            CorrelationId = correlationId,
            LogLevels = new[] { "Debug", "Information", "Warning" },
            Timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Endpoint to test correlation ID propagation across multiple method calls.
    /// </summary>
    /// <returns>Response showing correlation ID consistency</returns>
    [HttpGet("correlation-test")]
    public async Task<IActionResult> CorrelationTest()
    {
        var initialCorrelationId = _correlationIdService.CorrelationId;

        _logger.LogInformation("Starting correlation test with ID: {CorrelationId}", initialCorrelationId);

        var result1 = await ProcessStepOne();
        var result2 = await ProcessStepTwo();

        var finalCorrelationId = _correlationIdService.CorrelationId;

        _logger.LogInformation("Completed correlation test. Initial: {InitialId}, Final: {FinalId}",
            initialCorrelationId, finalCorrelationId);

        return Ok(new
        {
            Message = "Correlation ID propagation test completed",
            InitialCorrelationId = initialCorrelationId,
            FinalCorrelationId = finalCorrelationId,
            ConsistentCorrelationId = initialCorrelationId == finalCorrelationId,
            StepResults = new[] { result1, result2 },
            Timestamp = DateTime.UtcNow
        });
    }

    private async Task<string> ProcessStepOne()
    {
        var correlationId = _correlationIdService.CorrelationId;
        _logger.LogInformation("Processing step one with correlation ID: {CorrelationId}", correlationId);

        await Task.Delay(100);

        return $"Step 1 completed with {correlationId}";
    }

    private async Task<string> ProcessStepTwo()
    {
        var correlationId = _correlationIdService.CorrelationId;
        _logger.LogInformation("Processing step two with correlation ID: {CorrelationId}", correlationId);

        await Task.Delay(150);

        return $"Step 2 completed with {correlationId}";
    }
}

/// <summary>
/// Test request model for demonstrating request body logging.
/// </summary>
public class TestRequest
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public int Age { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// Test response model for demonstrating response body logging.
/// </summary>
public class TestResponse
{
    public Guid Id { get; set; }
    public string? ProcessedName { get; set; }
    public string? ProcessedEmail { get; set; }
    public DateTime ProcessedAt { get; set; }
    public string? CorrelationId { get; set; }
}