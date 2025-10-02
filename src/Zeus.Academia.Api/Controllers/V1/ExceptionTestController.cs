using Microsoft.AspNetCore.Mvc;
using Zeus.Academia.Api.Controllers;
using Zeus.Academia.Api.Exceptions;

namespace Zeus.Academia.Api.Controllers.V1;

/// <summary>
/// Controller for testing exception handling functionality
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class ExceptionTestController : BaseApiController
{
    private readonly ILogger<ExceptionTestController> _logger;

    public ExceptionTestController(ILogger<ExceptionTestController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Test business logic exception handling
    /// </summary>
    [HttpGet("business-logic")]
    public IActionResult TestBusinessLogicException()
    {
        throw BusinessLogicException.InvalidOperation("enroll_student", "Student has not met prerequisites");
    }

    /// <summary>
    /// Test validation exception handling
    /// </summary>
    [HttpGet("validation")]
    public IActionResult TestValidationException()
    {
        var validationEx = new ValidationException("Model validation failed");
        validationEx.AddValidationError("Email", "Email address is required");
        validationEx.AddValidationError("Email", "Email address must be valid");
        validationEx.AddValidationError("Password", "Password must be at least 8 characters");
        throw validationEx;
    }

    /// <summary>
    /// Test authorization exception handling
    /// </summary>
    [HttpGet("authorization")]
    public IActionResult TestAuthorizationException()
    {
        throw NotAuthorizedException.InsufficientPermissions("manage_courses", "Department access required");
    }

    /// <summary>
    /// Test unhandled exception handling
    /// </summary>
    [HttpGet("unhandled")]
    public IActionResult TestUnhandledException()
    {
        throw new InvalidOperationException("This is an unhandled exception for testing purposes");
    }

    /// <summary>
    /// Test exception with context
    /// </summary>
    [HttpGet("with-context")]
    public IActionResult TestExceptionWithContext()
    {
        var exception = new BusinessLogicException("Course enrollment failed")
            .AddContext("courseId", "CRS001")
            .AddContext("studentId", "STU001")
            .AddContext("semester", "Fall 2025")
            .AddContext("enrollmentDate", DateTime.UtcNow)
            .SetErrorCode("ENROLLMENT_FAILED");

        throw exception;
    }

    /// <summary>
    /// Test successful response (for comparison)
    /// </summary>
    [HttpGet("success")]
    public IActionResult TestSuccessResponse()
    {
        return Ok(new
        {
            message = "This endpoint works correctly",
            timestamp = DateTime.UtcNow,
            data = new
            {
                testId = Guid.NewGuid(),
                status = "success"
            }
        });
    }
}