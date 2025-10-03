using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Zeus.Academia.Api.Models.Common;
using System.Net;

namespace Zeus.Academia.Api.Controllers.Base;

/// <summary>
/// Base controller providing common functionality for all API controllers.
/// </summary>
[ApiController]
public abstract class BaseApiController : ControllerBase
{
    /// <summary>
    /// Creates a successful API response.
    /// </summary>
    /// <param name="message">Success message.</param>
    /// <returns>API response indicating success.</returns>
    protected ApiResponse CreateSuccessResponse(string message = "Operation completed successfully")
    {
        return new ApiResponse
        {
            Success = true,
            Message = message,
            Timestamp = DateTime.UtcNow,
            TraceId = HttpContext.TraceIdentifier
        };
    }

    /// <summary>
    /// Creates a successful API response with data.
    /// </summary>
    /// <typeparam name="T">Type of the data.</typeparam>
    /// <param name="data">The response data.</param>
    /// <param name="message">Success message.</param>
    /// <returns>API response with data.</returns>
    protected ApiResponse<T> CreateSuccessResponse<T>(T data, string message = "Operation completed successfully")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Timestamp = DateTime.UtcNow,
            TraceId = HttpContext.TraceIdentifier
        };
    }

    /// <summary>
    /// Creates an error API response.
    /// </summary>
    /// <param name="message">Error message.</param>
    /// <param name="statusCode">HTTP status code.</param>
    /// <returns>API response indicating error.</returns>
    protected ApiResponse CreateErrorResponse(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            Timestamp = DateTime.UtcNow,
            TraceId = HttpContext.TraceIdentifier
        };
    }

    /// <summary>
    /// Creates a validation error response from ModelState.
    /// </summary>
    /// <param name="modelState">The model state containing validation errors.</param>
    /// <returns>API response with validation errors.</returns>
    protected ApiResponse CreateValidationErrorResponse(ModelStateDictionary modelState)
    {
        var errors = modelState
            .Where(x => x.Value?.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
            );

        return new ApiResponse
        {
            Success = false,
            Message = "Validation failed",
            Errors = errors,
            Timestamp = DateTime.UtcNow,
            TraceId = HttpContext.TraceIdentifier
        };
    }

    /// <summary>
    /// Creates an error API response with data.
    /// </summary>
    /// <typeparam name="T">Type of the data.</typeparam>
    /// <param name="message">Error message.</param>
    /// <param name="data">Optional error data.</param>
    /// <param name="statusCode">HTTP status code.</param>
    /// <returns>API response with error and data.</returns>
    protected ApiResponse<T> CreateErrorResponse<T>(string message, T? data = default, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Data = data,
            Timestamp = DateTime.UtcNow,
            TraceId = HttpContext.TraceIdentifier
        };
    }
}