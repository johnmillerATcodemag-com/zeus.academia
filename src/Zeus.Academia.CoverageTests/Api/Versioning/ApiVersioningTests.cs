using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using Xunit;
using Zeus.Academia.Api.Versioning;

namespace Zeus.Academia.CoverageTests.Api.Versioning;

/// <summary>
/// Unit tests for the ApiVersionService
/// </summary>
public class ApiVersionServiceTests
{
    private readonly ApiVersionService _apiVersionService;

    public ApiVersionServiceTests()
    {
        _apiVersionService = new ApiVersionService();
    }

    [Fact]
    public void GetAvailableVersions_ReturnsAllVersions()
    {
        // Act
        var versions = _apiVersionService.GetAvailableVersions();

        // Assert
        Assert.NotNull(versions);
        Assert.NotEmpty(versions);
        Assert.Contains(versions, v => v.Version == "1.0");
        Assert.Contains(versions, v => v.Version == "2.0");
    }

    [Fact]
    public void GetLatestVersion_ReturnsHighestNonDeprecatedVersion()
    {
        // Act
        var latestVersion = _apiVersionService.GetLatestVersion();

        // Assert
        Assert.NotNull(latestVersion);
        Assert.Equal("2.0", latestVersion);
    }

    [Fact]
    public void GetCurrentVersion_WithVersionInHttpContext_ReturnsContextVersion()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Items["ApiVersion"] = "1.1";

        // Act
        var currentVersion = _apiVersionService.GetCurrentVersion(context);

        // Assert
        Assert.Equal("1.1", currentVersion);
    }

    [Fact]
    public void GetCurrentVersion_WithVersionHeader_ReturnsHeaderVersion()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Headers["X-API-Version"] = "2.0";

        // Act
        var currentVersion = _apiVersionService.GetCurrentVersion(context);

        // Assert
        Assert.Equal("2.0", currentVersion);
    }

    [Fact]
    public void GetCurrentVersion_WithoutVersion_ReturnsDefaultVersion()
    {
        // Arrange
        var context = new DefaultHttpContext();

        // Act
        var currentVersion = _apiVersionService.GetCurrentVersion(context);

        // Assert
        Assert.Equal("1.0", currentVersion);
    }

    [Theory]
    [InlineData("1.0", false)]
    [InlineData("1.1", false)]
    [InlineData("2.0", false)]
    [InlineData("3.0", false)] // Non-existent version should return false
    public void IsVersionDeprecated_WithVariousVersions_ReturnsCorrectResult(string version, bool expectedDeprecated)
    {
        // Act
        var isDeprecated = _apiVersionService.IsVersionDeprecated(version);

        // Assert
        Assert.Equal(expectedDeprecated, isDeprecated);
    }
}

/// <summary>
/// Unit tests for the ApiVersioningMiddleware
/// </summary>
public class ApiVersioningMiddlewareTests
{
    private readonly Mock<ILogger<ApiVersioningMiddleware>> _mockLogger;
    private readonly Mock<IApiVersionService> _mockVersionService;
    private readonly ApiVersioningMiddleware _middleware;
    private readonly Mock<RequestDelegate> _mockNext;

    public ApiVersioningMiddlewareTests()
    {
        _mockLogger = new Mock<ILogger<ApiVersioningMiddleware>>();
        _mockVersionService = new Mock<IApiVersionService>();
        _mockNext = new Mock<RequestDelegate>();
        _middleware = new ApiVersioningMiddleware(_mockNext.Object, _mockLogger.Object, _mockVersionService.Object);
    }

    [Fact]
    public async Task InvokeAsync_WithNonApiRoute_SkipsVersioning()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Path = "/health";
        _mockNext.Setup(x => x(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        _mockNext.Verify(x => x(context), Times.Once);
        _mockVersionService.Verify(x => x.GetAvailableVersions(), Times.Never);
    }

    [Fact]
    public async Task InvokeAsync_WithVersionDiscoveryRoute_ReturnsVersionInfo()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/versions";
        context.Response.Body = new MemoryStream();

        var versions = new List<ApiVersionInfo>
        {
            new("1.0", "Initial release", false, DateTime.Parse("2024-01-01")),
            new("2.0", "Enhanced version", false, DateTime.Parse("2024-10-01"))
        };

        _mockVersionService.Setup(x => x.GetAvailableVersions()).Returns(versions);
        _mockVersionService.Setup(x => x.GetLatestVersion()).Returns("2.0");

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(200, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);
    }

    [Fact]
    public async Task InvokeAsync_WithValidApiRoute_ProcessesVersioning()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/test";
        context.Request.Headers["X-API-Version"] = "1.0";

        var versions = new List<ApiVersionInfo>
        {
            new("1.0", "Initial release", false, DateTime.Parse("2024-01-01")),
            new("2.0", "Enhanced version", false, DateTime.Parse("2024-10-01"))
        };

        _mockVersionService.Setup(x => x.GetAvailableVersions()).Returns(versions);
        _mockVersionService.Setup(x => x.GetLatestVersion()).Returns("2.0");
        _mockVersionService.Setup(x => x.IsVersionDeprecated("1.0")).Returns(false);
        _mockNext.Setup(x => x(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        Assert.Equal("1.0", context.Items["ApiVersion"]);
        Assert.True(context.Response.Headers.ContainsKey("X-API-Version"));
        Assert.True(context.Response.Headers.ContainsKey("X-API-Supported-Versions"));
        _mockNext.Verify(x => x(context), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_WithUnsupportedVersion_ReturnsBadRequest()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/test";
        context.Request.Headers["X-API-Version"] = "3.0";
        context.Response.Body = new MemoryStream();

        var versions = new List<ApiVersionInfo>
        {
            new("1.0", "Initial release", false, DateTime.Parse("2024-01-01")),
            new("2.0", "Enhanced version", false, DateTime.Parse("2024-10-01"))
        };

        _mockVersionService.Setup(x => x.GetAvailableVersions()).Returns(versions);
        _mockVersionService.Setup(x => x.GetLatestVersion()).Returns("2.0");

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(400, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);
        _mockNext.Verify(x => x(It.IsAny<HttpContext>()), Times.Never);
    }

    [Fact]
    public async Task InvokeAsync_WithDeprecatedVersion_AddsDeprecationHeaders()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/test";
        context.Request.Headers["X-API-Version"] = "1.0";

        var versions = new List<ApiVersionInfo>
        {
            new("1.0", "Initial release", true, DateTime.Parse("2024-01-01")), // Deprecated
            new("2.0", "Enhanced version", false, DateTime.Parse("2024-10-01"))
        };

        _mockVersionService.Setup(x => x.GetAvailableVersions()).Returns(versions);
        _mockVersionService.Setup(x => x.GetLatestVersion()).Returns("2.0");
        _mockVersionService.Setup(x => x.IsVersionDeprecated("1.0")).Returns(true);
        _mockNext.Setup(x => x(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        Assert.Equal("1.0", context.Items["ApiVersion"]);
        Assert.True(context.Response.Headers.ContainsKey("X-API-Deprecated"));
        Assert.True(context.Response.Headers.ContainsKey("X-API-Latest-Version"));
        Assert.Equal("true", context.Response.Headers["X-API-Deprecated"]);
        Assert.Equal("2.0", context.Response.Headers["X-API-Latest-Version"]);
        _mockNext.Verify(x => x(context), Times.Once);
    }
}