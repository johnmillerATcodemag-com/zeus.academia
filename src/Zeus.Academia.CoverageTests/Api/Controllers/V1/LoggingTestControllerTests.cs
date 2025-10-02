using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Zeus.Academia.Api.Controllers.V1;
using Zeus.Academia.Api.Models.Responses;
using Zeus.Academia.Api.Services;

namespace Zeus.Academia.CoverageTests.Api.Controllers.V1;

/// <summary>
/// Comprehensive unit tests for LoggingTestController
/// </summary>
public class LoggingTestControllerTests
{
    private readonly Mock<ILogger<LoggingTestController>> _mockLogger;
    private readonly Mock<ICorrelationIdService> _mockCorrelationIdService;
    private readonly LoggingTestController _controller;

    public LoggingTestControllerTests()
    {
        _mockLogger = new Mock<ILogger<LoggingTestController>>();
        _mockCorrelationIdService = new Mock<ICorrelationIdService>();
        _controller = new LoggingTestController(_mockLogger.Object, _mockCorrelationIdService.Object);
    }

    [Fact]
    public async Task SimpleRequest_ReturnsOkWithCorrelationId()
    {
        // Arrange
        var correlationId = "test-correlation-123";
        _mockCorrelationIdService.Setup(x => x.CorrelationId).Returns(correlationId);

        // Act
        var result = await _controller.SimpleRequest();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = okResult.Value;

        Assert.NotNull(response);

        // Verify logging occurred
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Processing simple request")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task RequestWithBody_WithValidRequest_ReturnsProcessedResponse()
    {
        // Arrange
        var correlationId = "test-correlation-456";
        var request = new TestRequest
        {
            Name = "John Doe",
            Email = "john.doe@example.com",
            Age = 30,
            Description = "Test user"
        };

        _mockCorrelationIdService.Setup(x => x.CorrelationId).Returns(correlationId);

        // Act
        var result = await _controller.RequestWithBody(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var apiResponse = Assert.IsType<ApiResponse<TestResponse>>(okResult.Value);
        var response = apiResponse.Data;

        Assert.NotNull(response);
        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal("JOHN DOE", response.ProcessedName);
        Assert.Equal("john.doe@example.com", response.ProcessedEmail);
        Assert.Equal(correlationId, response.CorrelationId);
        Assert.True(response.ProcessedAt > DateTime.UtcNow.AddMinutes(-1));

        // Verify logging occurred
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Processing request with body")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task RequestWithBody_WithNullName_ProcessesCorrectly()
    {
        // Arrange
        var request = new TestRequest
        {
            Name = null,
            Email = "test@example.com"
        };

        _mockCorrelationIdService.Setup(x => x.CorrelationId).Returns("test-id");

        // Act
        var result = await _controller.RequestWithBody(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var apiResponse = Assert.IsType<ApiResponse<TestResponse>>(okResult.Value);
        var response = apiResponse.Data;

        Assert.NotNull(response);
        Assert.Null(response.ProcessedName);
        Assert.Equal("test@example.com", response.ProcessedEmail);
    }

    [Fact]
    public async Task RequestWithBody_WithNullEmail_ProcessesCorrectly()
    {
        // Arrange
        var request = new TestRequest
        {
            Name = "Test User",
            Email = null
        };

        _mockCorrelationIdService.Setup(x => x.CorrelationId).Returns("test-id");

        // Act
        var result = await _controller.RequestWithBody(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var apiResponse = Assert.IsType<ApiResponse<TestResponse>>(okResult.Value);
        var response = apiResponse.Data;

        Assert.NotNull(response);
        Assert.Equal("TEST USER", response.ProcessedName);
        Assert.Null(response.ProcessedEmail);
    }

    [Fact]
    public async Task SlowRequest_WithDefaultDelay_ReturnsResponseAfterDelay()
    {
        // Arrange
        var correlationId = "slow-test-789";
        _mockCorrelationIdService.Setup(x => x.CorrelationId).Returns(correlationId);

        var startTime = DateTime.UtcNow;

        // Act
        var result = await _controller.SlowRequest();

        // Assert
        var endTime = DateTime.UtcNow;
        var duration = endTime - startTime;

        Assert.True(duration.TotalMilliseconds >= 1900); // Should take at least ~2000ms

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = okResult.Value;
        Assert.NotNull(response);

        // Verify logging occurred twice (start and completion)
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Starting slow request")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Completed slow request")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task SlowRequest_WithCustomDelay_ReturnsResponseAfterSpecifiedDelay()
    {
        // Arrange
        var correlationId = "custom-slow-test";
        var customDelay = 500;
        _mockCorrelationIdService.Setup(x => x.CorrelationId).Returns(correlationId);

        var startTime = DateTime.UtcNow;

        // Act
        var result = await _controller.SlowRequest(customDelay);

        // Assert
        var endTime = DateTime.UtcNow;
        var duration = endTime - startTime;

        Assert.True(duration.TotalMilliseconds >= 450); // Should take at least ~500ms

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task LogLevels_GeneratesMultipleLogLevels()
    {
        // Arrange
        var correlationId = "log-levels-test";
        _mockCorrelationIdService.Setup(x => x.CorrelationId).Returns(correlationId);

        // Act
        var result = await _controller.LogLevels();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = okResult.Value;
        Assert.NotNull(response);

        // Verify all log levels were called
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Debug message")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Information message")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Warning message")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task CorrelationTest_MaintainsCorrelationIdConsistency()
    {
        // Arrange
        var correlationId = "correlation-consistency-test";
        _mockCorrelationIdService.Setup(x => x.CorrelationId).Returns(correlationId);

        // Act
        var result = await _controller.CorrelationTest();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = okResult.Value;
        Assert.NotNull(response);

        // Verify logging occurred for initial, step one, step two, and final
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Starting correlation test")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Processing step one")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Processing step two")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Completed correlation test")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Theory]
    [InlineData("", "", 0, null)]
    [InlineData("Jane Smith", "jane@example.com", 25, "Test description")]
    [InlineData("Special Chars !@#$%", "special@test.com", 40, "Description with special chars")]
    [InlineData("Very Long Name That Exceeds Normal Length", "verylongemail@example.com", 99, "Very long description that contains multiple words and should be processed correctly")]
    public async Task RequestWithBody_WithVariousInputs_ProcessesCorrectly(
        string name, string email, int age, string description)
    {
        // Arrange
        var request = new TestRequest
        {
            Name = string.IsNullOrEmpty(name) ? null : name,
            Email = string.IsNullOrEmpty(email) ? null : email,
            Age = age,
            Description = description
        };

        _mockCorrelationIdService.Setup(x => x.CorrelationId).Returns("theory-test-id");

        // Act
        var result = await _controller.RequestWithBody(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var apiResponse = Assert.IsType<ApiResponse<TestResponse>>(okResult.Value);
        var response = apiResponse.Data;

        Assert.NotNull(response);
        if (string.IsNullOrEmpty(name))
        {
            Assert.Null(response.ProcessedName);
        }
        else
        {
            Assert.Equal(name.ToUpper(), response.ProcessedName);
        }

        if (string.IsNullOrEmpty(email))
        {
            Assert.Null(response.ProcessedEmail);
        }
        else
        {
            Assert.Equal(email.ToLower(), response.ProcessedEmail);
        }

        Assert.Equal("theory-test-id", response.CorrelationId);
    }

    [Fact]
    public async Task SlowRequest_WithZeroDelay_CompletesQuickly()
    {
        // Arrange
        _mockCorrelationIdService.Setup(x => x.CorrelationId).Returns("zero-delay-test");
        var startTime = DateTime.UtcNow;

        // Act
        var result = await _controller.SlowRequest(0);

        // Assert
        var endTime = DateTime.UtcNow;
        var duration = endTime - startTime;

        Assert.True(duration.TotalMilliseconds < 100); // Should complete quickly

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public void Constructor_WithNullLogger_DoesNotThrow()
    {
        // Note: Controller doesn't validate null parameters in constructor
        // Act & Assert - Should not throw
        var controller = new LoggingTestController(null!, _mockCorrelationIdService.Object);
        Assert.NotNull(controller);
    }

    [Fact]
    public void Constructor_WithNullCorrelationIdService_DoesNotThrow()
    {
        // Note: Controller doesn't validate null parameters in constructor
        // Act & Assert - Should not throw
        var controller = new LoggingTestController(_mockLogger.Object, null!);
        Assert.NotNull(controller);
    }

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        // Act
        var controller = new LoggingTestController(_mockLogger.Object, _mockCorrelationIdService.Object);

        // Assert
        Assert.NotNull(controller);
    }
}

/// <summary>
/// Integration tests for TestRequest and TestResponse models
/// </summary>
public class TestModelsTests
{
    [Fact]
    public void TestRequest_Properties_CanBeSetAndGet()
    {
        // Arrange
        var request = new TestRequest();

        // Act
        request.Name = "Test Name";
        request.Email = "test@example.com";
        request.Age = 30;
        request.Description = "Test Description";

        // Assert
        Assert.Equal("Test Name", request.Name);
        Assert.Equal("test@example.com", request.Email);
        Assert.Equal(30, request.Age);
        Assert.Equal("Test Description", request.Description);
    }

    [Fact]
    public void TestRequest_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var request = new TestRequest();

        // Assert
        Assert.Null(request.Name);
        Assert.Null(request.Email);
        Assert.Equal(0, request.Age);
        Assert.Null(request.Description);
    }

    [Fact]
    public void TestResponse_Properties_CanBeSetAndGet()
    {
        // Arrange
        var response = new TestResponse();
        var testId = Guid.NewGuid();
        var testDate = DateTime.UtcNow;

        // Act
        response.Id = testId;
        response.ProcessedName = "PROCESSED NAME";
        response.ProcessedEmail = "processed@example.com";
        response.ProcessedAt = testDate;
        response.CorrelationId = "test-correlation";

        // Assert
        Assert.Equal(testId, response.Id);
        Assert.Equal("PROCESSED NAME", response.ProcessedName);
        Assert.Equal("processed@example.com", response.ProcessedEmail);
        Assert.Equal(testDate, response.ProcessedAt);
        Assert.Equal("test-correlation", response.CorrelationId);
    }

    [Fact]
    public void TestResponse_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var response = new TestResponse();

        // Assert
        Assert.Equal(Guid.Empty, response.Id);
        Assert.Null(response.ProcessedName);
        Assert.Null(response.ProcessedEmail);
        Assert.Equal(default(DateTime), response.ProcessedAt);
        Assert.Null(response.CorrelationId);
    }
}