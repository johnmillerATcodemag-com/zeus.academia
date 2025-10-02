using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Moq;
using Xunit;
using Zeus.Academia.Api.Versioning;

namespace Zeus.Academia.CoverageTests.Api.Versioning;

/// <summary>
/// Unit tests for the ApiVersionAttribute
/// </summary>
public class ApiVersionAttributeTests
{
    [Fact]
    public void Constructor_WithVersion_SetsVersion()
    {
        // Arrange & Act
        var attribute = new ApiVersionAttribute("2.0");

        // Assert
        Assert.Equal("2.0", attribute.Version);
        Assert.False(attribute.IsDeprecated);
    }

    [Fact]
    public void Constructor_WithNullVersion_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ApiVersionAttribute(null!));
    }

    [Fact]
    public void OnActionExecuting_WithNoRequestedVersion_SetsVersionInContext()
    {
        // Arrange
        var attribute = new ApiVersionAttribute("1.0");
        var context = CreateActionExecutingContext();

        // Act
        attribute.OnActionExecuting(context);

        // Assert
        Assert.Equal("1.0", context.HttpContext.Items["ApiVersion"]);
        Assert.Null(context.Result);
    }

    [Fact]
    public void OnActionExecuting_WithMatchingVersion_SetsVersionInContext()
    {
        // Arrange
        var attribute = new ApiVersionAttribute("2.0");
        var context = CreateActionExecutingContext();
        context.HttpContext.Request.Headers["X-API-Version"] = "2.0";

        // Act
        attribute.OnActionExecuting(context);

        // Assert
        Assert.Equal("2.0", context.HttpContext.Items["ApiVersion"]);
        Assert.Null(context.Result);
    }

    [Fact]
    public void OnActionExecuting_WithMismatchedVersion_ReturnsBadRequest()
    {
        // Arrange
        var attribute = new ApiVersionAttribute("1.0");
        var context = CreateActionExecutingContext();
        context.HttpContext.Request.Headers["X-API-Version"] = "2.0";

        // Act
        attribute.OnActionExecuting(context);

        // Assert
        Assert.IsType<BadRequestObjectResult>(context.Result);
        var badRequestResult = (BadRequestObjectResult)context.Result;
        Assert.NotNull(badRequestResult.Value);
    }

    [Fact]
    public void OnActionExecuting_WithDeprecatedVersion_AddsDeprecationHeaders()
    {
        // Arrange
        var attribute = new ApiVersionAttribute("1.0") { IsDeprecated = true };
        var context = CreateActionExecutingContext();
        context.HttpContext.Request.Headers["X-API-Version"] = "1.0";

        // Act
        attribute.OnActionExecuting(context);

        // Assert
        Assert.Equal("1.0", context.HttpContext.Items["ApiVersion"]);
        Assert.True(context.HttpContext.Response.Headers.ContainsKey("X-API-Deprecated"));
        Assert.True(context.HttpContext.Response.Headers.ContainsKey("X-API-Deprecation-Message"));
        Assert.Equal("true", context.HttpContext.Response.Headers["X-API-Deprecated"]);
    }

    [Fact]
    public void OnActionExecuting_WithQueryParameterVersion_SetsVersionInContext()
    {
        // Arrange
        var attribute = new ApiVersionAttribute("1.5");
        var context = CreateActionExecutingContext();
        context.HttpContext.Request.QueryString = new QueryString("?version=1.5");

        // Mock the query collection
        var queryCollection = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
        {
            { "version", "1.5" }
        });
        context.HttpContext.Request.Query = queryCollection;

        // Act
        attribute.OnActionExecuting(context);

        // Assert
        Assert.Equal("1.5", context.HttpContext.Items["ApiVersion"]);
        Assert.Null(context.Result);
    }

    [Fact]
    public void OnActionExecuted_AddsVersionHeader()
    {
        // Arrange
        var attribute = new ApiVersionAttribute("2.0");
        var context = CreateActionExecutedContext();

        // Act
        attribute.OnActionExecuted(context);

        // Assert
        Assert.True(context.HttpContext.Response.Headers.ContainsKey("X-API-Version"));
        Assert.Equal("2.0", context.HttpContext.Response.Headers["X-API-Version"]);
    }

    [Theory]
    [InlineData("1.0")]
    [InlineData("2.0")]
    [InlineData("1.5")]
    public void OnActionExecuting_WithVariousValidVersions_SetsCorrectVersion(string version)
    {
        // Arrange
        var attribute = new ApiVersionAttribute(version);
        var context = CreateActionExecutingContext();
        context.HttpContext.Request.Headers["X-API-Version"] = version;

        // Act
        attribute.OnActionExecuting(context);

        // Assert
        Assert.Equal(version, context.HttpContext.Items["ApiVersion"]);
        Assert.Null(context.Result);
    }

    private static ActionExecutingContext CreateActionExecutingContext()
    {
        var httpContext = new DefaultHttpContext();
        var actionContext = new ActionContext(httpContext, new Microsoft.AspNetCore.Routing.RouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());
        return new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), new Dictionary<string, object?>(), Mock.Of<Controller>());
    }

    private static ActionExecutedContext CreateActionExecutedContext()
    {
        var httpContext = new DefaultHttpContext();
        var actionContext = new ActionContext(httpContext, new Microsoft.AspNetCore.Routing.RouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());
        return new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), Mock.Of<Controller>());
    }
}