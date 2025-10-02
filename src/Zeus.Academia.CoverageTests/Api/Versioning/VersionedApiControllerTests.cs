using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Zeus.Academia.Api.Controllers;

namespace Zeus.Academia.CoverageTests.Api.Versioning;

/// <summary>
/// Test controller inheriting from VersionedApiController for testing
/// </summary>
public class TestVersionedController : VersionedApiController
{
    public IActionResult TestVersionedResponse()
    {
        var data = new { Message = "Test data" };
        return VersionedResponse(data, "Test message");
    }

    public IActionResult TestErrorResponse()
    {
        return VersionedErrorResponse("Test error", 400);
    }

    public IActionResult TestErrorResponseWithStatusCode(int statusCode)
    {
        return VersionedErrorResponse("Test error", statusCode);
    }

    public IActionResult TestVersionedResponseWithNullMessage()
    {
        var data = new { Test = "value" };
        return VersionedResponse(data, null);
    }

    public string GetCurrentVersionForTesting()
    {
        return CurrentApiVersion;
    }

    public object GetVersionInfoForTesting()
    {
        return GetVersionInfo();
    }
}

/// <summary>
/// Unit tests for the VersionedApiController
/// </summary>
public class VersionedApiControllerTests
{
    private readonly TestVersionedController _controller;

    public VersionedApiControllerTests()
    {
        _controller = new TestVersionedController();
    }

    [Fact]
    public void CurrentApiVersion_WithoutHttpContext_ReturnsDefault()
    {
        // Act
        var version = _controller.GetCurrentVersionForTesting();

        // Assert
        Assert.Equal("1.0", version);
    }

    [Fact]
    public void CurrentApiVersion_WithVersionInHttpContext_ReturnsContextVersion()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.Items["ApiVersion"] = "2.0";
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var version = _controller.GetCurrentVersionForTesting();

        // Assert
        Assert.Equal("2.0", version);
    }

    [Fact]
    public void GetVersionInfo_ReturnsVersionInformation()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.TraceIdentifier = "test-trace-id";
        httpContext.Items["ApiVersion"] = "1.5";
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var versionInfo = _controller.GetVersionInfoForTesting();

        // Assert
        Assert.NotNull(versionInfo);
        var versionInfoDict = versionInfo.GetType().GetProperties()
            .ToDictionary(p => p.Name, p => p.GetValue(versionInfo));

        Assert.Equal("1.5", versionInfoDict["ApiVersion"]);
        Assert.Equal("test-trace-id", versionInfoDict["RequestId"]);
        Assert.NotNull(versionInfoDict["Timestamp"]);
    }

    [Fact]
    public void VersionedResponse_WithData_ReturnsOkResultWithVersionedFormat()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.TraceIdentifier = "test-trace-id";
        httpContext.Items["ApiVersion"] = "2.0";
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var result = _controller.TestVersionedResponse();

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = (OkObjectResult)result;
        Assert.NotNull(okResult.Value);

        // Verify the response structure
        var responseType = okResult.Value.GetType();
        var dataProperty = responseType.GetProperty("Data");
        var messageProperty = responseType.GetProperty("Message");
        var successProperty = responseType.GetProperty("Success");
        var versionProperty = responseType.GetProperty("Version");

        Assert.NotNull(dataProperty);
        Assert.NotNull(messageProperty);
        Assert.NotNull(successProperty);
        Assert.NotNull(versionProperty);

        Assert.Equal("Test message", messageProperty.GetValue(okResult.Value));
        Assert.Equal(true, successProperty.GetValue(okResult.Value));
    }

    [Fact]
    public void VersionedErrorResponse_WithError_ReturnsErrorResultWithVersionedFormat()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.TraceIdentifier = "error-trace-id";
        httpContext.Items["ApiVersion"] = "1.0";
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var result = _controller.TestErrorResponse();

        // Assert
        Assert.IsType<ObjectResult>(result);
        var objectResult = (ObjectResult)result;
        Assert.Equal(400, objectResult.StatusCode);
        Assert.NotNull(objectResult.Value);

        // Verify the error response structure
        var responseType = objectResult.Value.GetType();
        var errorProperty = responseType.GetProperty("Error");
        var successProperty = responseType.GetProperty("Success");
        var versionProperty = responseType.GetProperty("Version");

        Assert.NotNull(errorProperty);
        Assert.NotNull(successProperty);
        Assert.NotNull(versionProperty);

        Assert.Equal("Test error", errorProperty.GetValue(objectResult.Value));
        Assert.Equal(false, successProperty.GetValue(objectResult.Value));
    }

    [Theory]
    [InlineData(200)]
    [InlineData(404)]
    [InlineData(500)]
    public void VersionedErrorResponse_WithVariousStatusCodes_ReturnsCorrectStatusCode(int statusCode)
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var result = _controller.TestErrorResponseWithStatusCode(statusCode);

        // Assert
        Assert.IsType<ObjectResult>(result);
        var objectResult = (ObjectResult)result;
        Assert.Equal(statusCode, objectResult.StatusCode);
    }

    [Fact]
    public void VersionedResponse_WithNullMessage_HandlesNullMessage()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var result = _controller.TestVersionedResponseWithNullMessage();

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = (OkObjectResult)result;
        var messageProperty = okResult.Value?.GetType().GetProperty("Message");
        Assert.Null(messageProperty?.GetValue(okResult.Value));
    }
}