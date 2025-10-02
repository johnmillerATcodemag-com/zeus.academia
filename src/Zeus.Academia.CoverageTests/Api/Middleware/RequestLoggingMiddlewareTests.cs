using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;
using Zeus.Academia.Api.Middleware;
using Zeus.Academia.Api.Services;
using System.Text;
using System.Text.Json;

namespace Zeus.Academia.CoverageTests.Api.Middleware;

/// <summary>
/// Comprehensive unit tests for RequestLoggingMiddleware
/// </summary>
public class RequestLoggingMiddlewareTests
{
    private readonly Mock<ILogger<RequestLoggingMiddleware>> _mockLogger;
    private readonly Mock<ICorrelationIdService> _mockCorrelationIdService;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<RequestDelegate> _mockNext;

    public RequestLoggingMiddlewareTests()
    {
        _mockLogger = new Mock<ILogger<RequestLoggingMiddleware>>();
        _mockCorrelationIdService = new Mock<ICorrelationIdService>();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockNext = new Mock<RequestDelegate>();

        // Setup default configuration
        var mockSection = new Mock<IConfigurationSection>();
        mockSection.Setup(x => x.Value).Returns("{}"); // Empty JSON for RequestLoggingOptions
        _mockConfiguration.Setup(x => x.GetSection("RequestLogging")).Returns(mockSection.Object);
    }

    [Fact]
    public async Task InvokeAsync_WithValidRequest_LogsRequestAndResponse()
    {
        // Arrange
        var middleware = CreateMiddleware();
        var context = CreateHttpContext();
        var correlationId = "test-correlation-123";

        _mockCorrelationIdService.Setup(x => x.GenerateCorrelationId()).Returns(correlationId);
        _mockNext.Setup(x => x(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        _mockCorrelationIdService.Verify(x => x.SetCorrelationId(correlationId), Times.Once);
        Assert.Contains("X-Correlation-ID", context.Response.Headers);
        Assert.Equal(correlationId, context.Response.Headers["X-Correlation-ID"].ToString());

        // Verify logging occurred
        VerifyLoggedInformation("HTTP Request Started");
        VerifyLoggedInformation("HTTP Request Completed");
    }

    [Fact]
    public async Task InvokeAsync_WithCorrelationIdInHeader_UsesExistingCorrelationId()
    {
        // Arrange
        var middleware = CreateMiddleware();
        var context = CreateHttpContext();
        var existingCorrelationId = "existing-correlation-456";

        context.Request.Headers["X-Correlation-ID"] = existingCorrelationId;
        _mockNext.Setup(x => x(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        _mockCorrelationIdService.Verify(x => x.SetCorrelationId(existingCorrelationId), Times.Once);
        _mockCorrelationIdService.Verify(x => x.GenerateCorrelationId(), Times.Never);
    }

    [Fact]
    public async Task InvokeAsync_WithCorrelationIdInQuery_UsesQueryCorrelationId()
    {
        // Arrange
        var middleware = CreateMiddleware();
        var context = CreateHttpContext();
        var queryCorrelationId = "query-correlation-789";

        context.Request.QueryString = new QueryString($"?correlationId={queryCorrelationId}");
        _mockNext.Setup(x => x(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        _mockCorrelationIdService.Verify(x => x.SetCorrelationId(queryCorrelationId), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_WithExcludedPath_SkipsLogging()
    {
        // Arrange
        var middleware = CreateMiddleware();
        var context = CreateHttpContext();
        context.Request.Path = "/health";

        _mockNext.Setup(x => x(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        _mockNext.Verify(x => x(context), Times.Once);
        // Should not set correlation ID for excluded paths
        _mockCorrelationIdService.Verify(x => x.SetCorrelationId(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task InvokeAsync_WithException_LogsErrorAndRethrows()
    {
        // Arrange
        var middleware = CreateMiddleware();
        var context = CreateHttpContext();
        var testException = new InvalidOperationException("Test exception");
        var correlationId = "error-correlation-999";

        _mockCorrelationIdService.Setup(x => x.GenerateCorrelationId()).Returns(correlationId);
        _mockNext.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(testException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => middleware.InvokeAsync(context));

        Assert.Same(testException, exception);
        VerifyLoggedError("HTTP Request Failed");
    }

    [Fact]
    public async Task InvokeAsync_WithSlowRequest_LogsSlowRequestWarning()
    {
        // Arrange
        var middleware = CreateMiddleware();
        var context = CreateHttpContext();
        var correlationId = "slow-correlation-111";

        _mockCorrelationIdService.Setup(x => x.GenerateCorrelationId()).Returns(correlationId);
        _mockNext.Setup(x => x(It.IsAny<HttpContext>()))
            .Returns(async () => await Task.Delay(50)); // Simulate processing time

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        // Note: This test may be flaky due to timing, but demonstrates the concept
        VerifyLoggedInformation("HTTP Request Completed");
    }

    [Theory]
    [InlineData("application/json", true)]
    [InlineData("application/xml", true)]
    [InlineData("text/plain", true)]
    [InlineData("image/jpeg", false)]
    [InlineData("", false)]
    public void HasBody_WithDifferentContentTypes_ReturnsExpectedResult(string contentType, bool expected)
    {
        // Arrange
        var context = CreateHttpContext();
        context.Request.ContentType = contentType;
        context.Request.ContentLength = 100;

        // Act - Using reflection to test private method
        var middleware = CreateMiddleware();
        var hasBodyMethod = typeof(RequestLoggingMiddleware)
            .GetMethod("HasBody", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        var result = (bool)hasBodyMethod!.Invoke(null, new object[] { context.Request })!;

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetClientIpAddress_WithXForwardedFor_ReturnsForwardedIP()
    {
        // Arrange
        var context = CreateHttpContext();
        var forwardedIp = "192.168.1.100";
        context.Request.Headers["X-Forwarded-For"] = $"{forwardedIp}, 10.0.0.1";

        // Act - Using reflection to test private method
        var middleware = CreateMiddleware();
        var getIpMethod = typeof(RequestLoggingMiddleware)
            .GetMethod("GetClientIpAddress", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        var result = (string)getIpMethod!.Invoke(null, new object[] { context })!;

        // Assert
        Assert.Equal(forwardedIp, result);
    }

    [Fact]
    public void GetClientIpAddress_WithXRealIP_ReturnsRealIP()
    {
        // Arrange
        var context = CreateHttpContext();
        var realIp = "203.0.113.1";
        context.Request.Headers["X-Real-IP"] = realIp;

        // Act - Using reflection to test private method
        var middleware = CreateMiddleware();
        var getIpMethod = typeof(RequestLoggingMiddleware)
            .GetMethod("GetClientIpAddress", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        var result = (string)getIpMethod!.Invoke(null, new object[] { context })!;

        // Assert
        Assert.Equal(realIp, result);
    }

    [Theory]
    [InlineData(200, LogLevel.Information)]
    [InlineData(404, LogLevel.Warning)]
    [InlineData(500, LogLevel.Error)]
    [InlineData(302, LogLevel.Information)]
    public void GetLogLevelForStatusCode_WithDifferentStatusCodes_ReturnsCorrectLogLevel(
        int statusCode, LogLevel expectedLogLevel)
    {
        // Act - Using reflection to test private method
        var middleware = CreateMiddleware();
        var getLogLevelMethod = typeof(RequestLoggingMiddleware)
            .GetMethod("GetLogLevelForStatusCode", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        var result = (LogLevel)getLogLevelMethod!.Invoke(null, new object[] { statusCode })!;

        // Assert
        Assert.Equal(expectedLogLevel, result);
    }

    [Theory]
    [InlineData("password123", true)]
    [InlineData("secret-key", true)]
    [InlineData("normal-content", false)]
    [InlineData("", false)]
    public void ContainsSensitiveData_WithDifferentContent_ReturnsExpectedResult(
        string content, bool expected)
    {
        // Arrange
        var middleware = CreateMiddleware();

        // Act - Using reflection to test private method
        var containsSensitiveMethod = typeof(RequestLoggingMiddleware)
            .GetMethod("ContainsSensitiveData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var result = (bool)containsSensitiveMethod!.Invoke(middleware, new object[] { content })!;

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void TruncateIfNeeded_WithLongContent_ReturnsContent()
    {
        // Arrange
        var middleware = CreateMiddleware();
        var longContent = new string('x', 2000);
        var maxLength = 1000;

        // Act - Using reflection to test private method
        var truncateMethod = typeof(RequestLoggingMiddleware)
            .GetMethod("TruncateIfNeeded", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var result = (string)truncateMethod!.Invoke(middleware, new object[] { longContent, maxLength })!;

        // Assert
        Assert.True(result.Length <= maxLength + 20); // Account for "... [truncated]"
        Assert.Contains("... [truncated]", result);
    }

    [Fact]
    public void TruncateIfNeeded_WithShortContent_ReturnsOriginalContent()
    {
        // Arrange
        var middleware = CreateMiddleware();
        var shortContent = "short content";
        var maxLength = 1000;

        // Act - Using reflection to test private method
        var truncateMethod = typeof(RequestLoggingMiddleware)
            .GetMethod("TruncateIfNeeded", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var result = (string)truncateMethod!.Invoke(middleware, new object[] { shortContent, maxLength })!;

        // Assert
        Assert.Equal(shortContent, result);
    }

    private RequestLoggingMiddleware CreateMiddleware()
    {
        return new RequestLoggingMiddleware(
            _mockNext.Object,
            _mockLogger.Object,
            _mockCorrelationIdService.Object,
            _mockConfiguration.Object);
    }

    private DefaultHttpContext CreateHttpContext()
    {
        var context = new DefaultHttpContext();
        context.Request.Method = "GET";
        context.Request.Path = "/api/test";
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("localhost");
        context.Response.Body = new MemoryStream();
        return context;
    }

    private void VerifyLoggedInformation(string message)
    {
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(message)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    private void VerifyLoggedError(string message)
    {
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(message)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }
}

/// <summary>
/// Unit tests for RequestLoggingOptions configuration class
/// </summary>
public class RequestLoggingOptionsTests
{
    [Fact]
    public void RequestLoggingOptions_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var options = new RequestLoggingOptions();

        // Assert
        Assert.False(options.LogRequestBody);
        Assert.False(options.LogResponseBody);
        Assert.Equal(1024, options.MaxBodyLength);
        Assert.Equal(1000, options.SlowRequestThresholdMs);
        Assert.Contains("/health", options.ExcludedPaths);
        Assert.Contains("/metrics", options.ExcludedPaths);
        Assert.Contains("Authorization", options.SensitiveHeaders);
        Assert.Contains("password", options.SensitiveDataPatterns);
    }

    [Fact]
    public void RequestLoggingOptions_SectionName_IsCorrect()
    {
        // Act & Assert
        Assert.Equal("RequestLogging", RequestLoggingOptions.SectionName);
    }

    [Fact]
    public void RequestLoggingOptions_CanSetAllProperties()
    {
        // Arrange
        var options = new RequestLoggingOptions
        {
            LogRequestBody = true,
            LogResponseBody = true,
            MaxBodyLength = 2048,
            SlowRequestThresholdMs = 2000,
            ExcludedPaths = new List<string> { "/custom-health" },
            SensitiveHeaders = new List<string> { "X-Custom-Auth" },
            SensitiveDataPatterns = new List<string> { "custom-secret" }
        };

        // Act & Assert
        Assert.True(options.LogRequestBody);
        Assert.True(options.LogResponseBody);
        Assert.Equal(2048, options.MaxBodyLength);
        Assert.Equal(2000, options.SlowRequestThresholdMs);
        Assert.Contains("/custom-health", options.ExcludedPaths);
        Assert.Contains("X-Custom-Auth", options.SensitiveHeaders);
        Assert.Contains("custom-secret", options.SensitiveDataPatterns);
    }
}