using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Security.Claims;
using Xunit;
using Zeus.Academia.Api.Controllers;
using Zeus.Academia.Api.Extensions;
using Zeus.Academia.Api.Models.Responses;
using Zeus.Academia.Api.Models.Common;

#pragma warning disable CS0618 // Type or member is obsolete
namespace Zeus.Academia.CoverageTests.Api.ResponseFormatting;

/// <summary>
/// Unit tests for BaseApiController response methods
/// </summary>
public class BaseApiControllerTests
{
    private readonly TestBaseApiController _controller;
    private readonly Mock<HttpContext> _mockHttpContext;

    public BaseApiControllerTests()
    {
        _mockHttpContext = new Mock<HttpContext>();
        _controller = new TestBaseApiController();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = _mockHttpContext.Object
        };

        // Setup default HttpContext items
        _mockHttpContext.Setup(x => x.TraceIdentifier).Returns("test-trace-id");
        _mockHttpContext.Setup(x => x.Items).Returns(new Dictionary<object, object?>());

        var mockRequest = new Mock<HttpRequest>();
        var mockHeaders = new HeaderDictionary();
        var mockQuery = new QueryCollection();

        mockRequest.Setup(x => x.Headers).Returns(mockHeaders);
        mockRequest.Setup(x => x.Query).Returns(mockQuery);
        _mockHttpContext.Setup(x => x.Request).Returns(mockRequest.Object);
    }

    [Fact]
    public void Success_WithData_ShouldReturnCorrectApiResponse()
    {
        // Arrange
        var testData = new TestDataModel { Id = 1, Name = "Test" };
        var message = "Operation successful";

        // Act
        var result = _controller.TestSuccess(testData, message);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<TestDataModel>>(okResult.Value);

        Assert.True(response.Success);
        Assert.Equal(testData.Id, response.Data!.Id);
        Assert.Equal(testData.Name, response.Data.Name);
        Assert.Equal(message, response.Message);
        Assert.Equal("test-trace-id", response.CorrelationId);
        Assert.NotNull(response.Version);
        Assert.Null(response.Errors);
    }

    [Fact]
    public void Success_WithoutData_ShouldReturnCorrectApiResponse()
    {
        // Arrange
        var message = "Operation completed";

        // Act
        var result = _controller.TestSuccess(message);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse>(okResult.Value);

        Assert.True(response.Success);
        Assert.Equal(message, response.Message);
        Assert.Equal("test-trace-id", response.CorrelationId);
        Assert.NotNull(response.Version);
        Assert.Null(response.Errors);
    }

    [Fact]
    public void Error_ShouldReturnCorrectApiResponse()
    {
        // Arrange
        var message = "Operation failed";
        var statusCode = 400;
        var errors = new Dictionary<string, string[]> { ["Field"] = new[] { "Error message" } };

        // Act
        var result = _controller.TestError(message, statusCode, errors);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(statusCode, objectResult.StatusCode);

        var response = Assert.IsType<ApiResponse>(objectResult.Value);
        Assert.False(response.Success);
        Assert.Equal(message, response.Message);
        Assert.Equal(errors, response.Errors);
        Assert.Equal("test-trace-id", response.CorrelationId);
        Assert.NotNull(response.Version);
    }

    [Fact]
    public void PagedSuccess_WithParameters_ShouldReturnCorrectPagedResponse()
    {
        // Arrange
        var data = new[] { "item1", "item2", "item3" };
        var currentPage = 1;
        var pageSize = 3;
        var totalItems = 10;
        var message = "Data retrieved";

        // Act
        var result = _controller.TestPagedSuccess(data, currentPage, pageSize, totalItems, message);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<PagedApiResponse<string>>(okResult.Value);

        Assert.True(response.Success);
        Assert.Equal(data, response.Data);
        Assert.Equal(message, response.Message);
        Assert.Equal("test-trace-id", response.CorrelationId);
        Assert.NotNull(response.Version);

        Assert.NotNull(response.Pagination);
        Assert.Equal(currentPage, response.Pagination.CurrentPage);
        Assert.Equal(pageSize, response.Pagination.PageSize);
        Assert.Equal(totalItems, response.Pagination.TotalItems);
    }

    [Fact]
    public void PagedSuccess_WithPagedResult_ShouldReturnCorrectPagedResponse()
    {
        // Arrange
        var data = new[] { 1, 2, 3 };
        var pagedResult = new PagedResult<int>
        {
            Items = data,
            Pagination = PaginationMetadata.Create(2, 3, 15)
        };
        var message = "Page retrieved";

        // Act
        var result = _controller.TestPagedSuccess(pagedResult, message);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<PagedApiResponse<int>>(okResult.Value);

        Assert.True(response.Success);
        Assert.Equal(data, response.Data);
        Assert.Equal(message, response.Message);
        Assert.Equal("test-trace-id", response.CorrelationId);
        Assert.NotNull(response.Version);
        Assert.Equal(pagedResult.Pagination, response.Pagination);
    }

    [Fact]
    public void NotFound_ShouldReturnCorrectErrorResponse()
    {
        // Arrange
        var message = "Resource not found";

        // Act
        var result = _controller.TestNotFound(message);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(404, objectResult.StatusCode);

        var response = Assert.IsType<ApiResponse>(objectResult.Value);
        Assert.False(response.Success);
        Assert.Equal(message, response.Message);
    }

    [Fact]
    public void Unauthorized_ShouldReturnCorrectErrorResponse()
    {
        // Arrange
        var message = "Access denied";

        // Act
        var result = _controller.TestUnauthorized(message);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(401, objectResult.StatusCode);

        var response = Assert.IsType<ApiResponse>(objectResult.Value);
        Assert.False(response.Success);
        Assert.Equal(message, response.Message);
    }

    [Fact]
    public void Forbidden_ShouldReturnCorrectErrorResponse()
    {
        // Arrange
        var message = "Insufficient permissions";

        // Act
        var result = _controller.TestForbidden(message);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(403, objectResult.StatusCode);

        var response = Assert.IsType<ApiResponse>(objectResult.Value);
        Assert.False(response.Success);
        Assert.Equal(message, response.Message);
    }

    [Fact]
    public void GetApiVersion_WithHttpContextItem_ShouldReturnVersion()
    {
        // Arrange
        var expectedVersion = "2.0";
        var items = new Dictionary<object, object?> { { "ApiVersion", expectedVersion } };
        _mockHttpContext.Setup(x => x.Items).Returns(items);

        // Act
        var version = _controller.TestGetApiVersion();

        // Assert
        Assert.Equal(expectedVersion, version);
    }

    [Fact]
    public void GetApiVersion_WithHeader_ShouldReturnVersion()
    {
        // Arrange
        var expectedVersion = "v1.5";
        var mockHeaders = new HeaderDictionary { { "X-Api-Version", expectedVersion } };
        var mockRequest = new Mock<HttpRequest>();
        mockRequest.Setup(x => x.Headers).Returns(mockHeaders);
        mockRequest.Setup(x => x.Query).Returns(new QueryCollection());
        _mockHttpContext.Setup(x => x.Request).Returns(mockRequest.Object);

        // Act
        var version = _controller.TestGetApiVersion();

        // Assert
        Assert.Equal(expectedVersion, version);
    }

    [Fact]
    public void GetApiVersion_WithQueryParameter_ShouldReturnVersion()
    {
        // Arrange
        var expectedVersion = "beta";
        var queryParams = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
        {
            { "version", expectedVersion }
        };
        var mockQuery = new QueryCollection(queryParams);
        var mockRequest = new Mock<HttpRequest>();
        mockRequest.Setup(x => x.Headers).Returns(new HeaderDictionary());
        mockRequest.Setup(x => x.Query).Returns(mockQuery);
        _mockHttpContext.Setup(x => x.Request).Returns(mockRequest.Object);

        // Act
        var version = _controller.TestGetApiVersion();

        // Assert
        Assert.Equal(expectedVersion, version);
    }

    [Fact]
    public void GetApiVersion_WithoutVersion_ShouldReturnDefault()
    {
        // Act
        var version = _controller.TestGetApiVersion();

        // Assert
        Assert.Equal("1.0", version);
    }

    [Fact]
    public void CorrelationId_ShouldReturnTraceIdentifier()
    {
        // Act
        var correlationId = _controller.TestCorrelationId;

        // Assert
        Assert.Equal("test-trace-id", correlationId);
    }

    [Fact]
    public void CorrelationId_WhenTraceIdentifierEmpty_ShouldReturnEmptyString()
    {
        // Arrange - Setup empty trace identifier 
        _mockHttpContext.Setup(x => x.TraceIdentifier).Returns(string.Empty);

        // Act
        var correlationId = _controller.TestCorrelationId;

        // Assert
        Assert.NotNull(correlationId);
        Assert.Equal(string.Empty, correlationId);
    }

    // Test data model
    private class TestDataModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    // Test controller to expose protected methods
    private class TestBaseApiController : BaseApiController
    {
        public IActionResult TestSuccess<T>(T data, string? message = null) => Success(data, message);
        public IActionResult TestSuccess(string message) => Success(message);
        public IActionResult TestError(string message, int statusCode, object? errors) => Error(message, statusCode, errors);
        public IActionResult TestPagedSuccess<T>(IEnumerable<T> data, int currentPage, int pageSize, int totalItems, string? message)
            => PagedSuccess(data, currentPage, pageSize, totalItems, message);
        public IActionResult TestPagedSuccess<T>(PagedResult<T> pagedResult, string? message) => PagedSuccess(pagedResult, message);
        public IActionResult TestNotFound(string message) => NotFound(message);
        public IActionResult TestUnauthorized(string message) => Unauthorized(message);
        public IActionResult TestForbidden(string message) => Forbidden(message);
        public string? TestGetApiVersion() => GetApiVersion();
        public string TestCorrelationId => CorrelationId;
    }
}