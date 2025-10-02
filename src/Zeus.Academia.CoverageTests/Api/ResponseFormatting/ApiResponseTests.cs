using Xunit;
using Zeus.Academia.Api.Models.Responses;

namespace Zeus.Academia.CoverageTests.Api.ResponseFormatting;

/// <summary>
/// Unit tests for ApiResponse and ApiResponse&lt;T&gt; classes
/// </summary>
public class ApiResponseTests
{
    [Fact]
    public void ApiResponse_CreateSuccess_ShouldSetCorrectProperties()
    {
        // Arrange
        var message = "Operation successful";
        var correlationId = "test-correlation-id";
        var version = "1.0";

        // Act
        var response = ApiResponse.CreateSuccess(message, correlationId, version);

        // Assert
        Assert.True(response.Success);
        Assert.Equal(message, response.Message);
        Assert.Equal(correlationId, response.CorrelationId);
        Assert.Equal(version, response.Version);
        Assert.Null(response.Errors);
        Assert.True(response.Timestamp <= DateTime.UtcNow);
        Assert.True(response.Timestamp > DateTime.UtcNow.AddMinutes(-1));
    }

    [Fact]
    public void ApiResponse_CreateSuccess_WithoutOptionalParameters_ShouldSetDefaults()
    {
        // Act
        var response = ApiResponse.CreateSuccess();

        // Assert
        Assert.True(response.Success);
        Assert.Null(response.Message);
        Assert.Null(response.CorrelationId);
        Assert.Null(response.Version);
        Assert.Null(response.Errors);
    }

    [Fact]
    public void ApiResponse_CreateError_ShouldSetCorrectProperties()
    {
        // Arrange
        var message = "Operation failed";
        var errors = new { Field = "Error message" };
        var correlationId = "test-correlation-id";
        var version = "1.0";

        // Act
        var response = ApiResponse.CreateError(message, errors, correlationId, version);

        // Assert
        Assert.False(response.Success);
        Assert.Equal(message, response.Message);
        Assert.Equal(correlationId, response.CorrelationId);
        Assert.Equal(version, response.Version);
        Assert.Equal(errors, response.Errors);
        Assert.True(response.Timestamp <= DateTime.UtcNow);
        Assert.True(response.Timestamp > DateTime.UtcNow.AddMinutes(-1));
    }

    [Fact]
    public void ApiResponseT_CreateSuccess_ShouldSetCorrectProperties()
    {
        // Arrange
        var data = new { Id = 1, Name = "Test" };
        var message = "Data retrieved successfully";
        var correlationId = "test-correlation-id";
        var version = "1.0";

        // Act
        var response = ApiResponse<object>.CreateSuccess(data, message, correlationId, version);

        // Assert
        Assert.True(response.Success);
        Assert.Equal(data, response.Data);
        Assert.Equal(message, response.Message);
        Assert.Equal(correlationId, response.CorrelationId);
        Assert.Equal(version, response.Version);
        Assert.Null(response.Errors);
    }

    [Fact]
    public void ApiResponseT_CreateError_ShouldSetCorrectProperties()
    {
        // Arrange
        var message = "Operation failed";
        var errors = new[] { "Error 1", "Error 2" };
        var correlationId = "test-correlation-id";
        var version = "1.0";

        // Act
        var response = ApiResponse<object>.CreateError(message, errors, correlationId, version);

        // Assert
        Assert.False(response.Success);
        Assert.Null(response.Data);
        Assert.Equal(message, response.Message);
        Assert.Equal(correlationId, response.CorrelationId);
        Assert.Equal(version, response.Version);
        Assert.Equal(errors, response.Errors);
    }

    [Fact]
    public void ApiResponse_Inheritance_ShouldWork()
    {
        // Arrange
        var typedResponse = new ApiResponse<string>
        {
            Success = true,
            Data = "test data",
            Message = "test message",
            CorrelationId = "test-id",
            Version = "1.0"
        };

        // Act & Assert - Verify inheritance works correctly
        Assert.IsAssignableFrom<ApiResponse>(typedResponse);
        Assert.True(typedResponse.Success);
        Assert.Equal("test data", typedResponse.Data);
        Assert.Equal("test message", typedResponse.Message);
        Assert.Equal("test-id", typedResponse.CorrelationId);
        Assert.Equal("1.0", typedResponse.Version);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Custom message")]
    public void ApiResponse_CreateSuccess_ShouldHandleVariousMessageValues(string? message)
    {
        // Act
        var response = ApiResponse.CreateSuccess(message);

        // Assert
        Assert.True(response.Success);
        Assert.Equal(message, response.Message);
    }

    [Theory]
    [InlineData("v1.0")]
    [InlineData("v2.0")]
    [InlineData("beta")]
    [InlineData(null)]
    public void ApiResponse_CreateSuccess_ShouldHandleVariousVersionValues(string? version)
    {
        // Act
        var response = ApiResponse.CreateSuccess(version: version);

        // Assert
        Assert.True(response.Success);
        Assert.Equal(version, response.Version);
    }
}