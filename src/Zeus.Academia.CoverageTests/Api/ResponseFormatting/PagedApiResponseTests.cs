using Xunit;
using Zeus.Academia.Api.Models.Responses;

namespace Zeus.Academia.CoverageTests.Api.ResponseFormatting;

/// <summary>
/// Unit tests for PagedApiResponse class
/// </summary>
public class PagedApiResponseTests
{
    [Fact]
    public void PagedApiResponse_CreateSuccess_WithPaginationMetadata_ShouldSetCorrectProperties()
    {
        // Arrange
        var data = new[] { "item1", "item2", "item3" };
        var pagination = PaginationMetadata.Create(1, 10, 25);
        var message = "Data retrieved successfully";
        var correlationId = "test-correlation-id";
        var version = "1.0";

        // Act
        var response = PagedApiResponse<string>.CreateSuccess(data, pagination, message, correlationId, version);

        // Assert
        Assert.True(response.Success);
        Assert.Equal(data, response.Data);
        Assert.Equal(pagination, response.Pagination);
        Assert.Equal(message, response.Message);
        Assert.Equal(correlationId, response.CorrelationId);
        Assert.Equal(version, response.Version);
        Assert.Null(response.Errors);
    }

    [Fact]
    public void PagedApiResponse_CreateSuccess_WithParameters_ShouldCreatePaginationMetadata()
    {
        // Arrange
        var data = new[] { 1, 2, 3, 4, 5 };
        var currentPage = 2;
        var pageSize = 5;
        var totalItems = 20;
        var message = "Page retrieved";
        var correlationId = "test-id";
        var version = "2.0";

        // Act
        var response = PagedApiResponse<int>.CreateSuccess(data, currentPage, pageSize, totalItems, message, correlationId, version);

        // Assert
        Assert.True(response.Success);
        Assert.Equal(data, response.Data);
        Assert.NotNull(response.Pagination);
        Assert.Equal(currentPage, response.Pagination.CurrentPage);
        Assert.Equal(pageSize, response.Pagination.PageSize);
        Assert.Equal(totalItems, response.Pagination.TotalItems);
        Assert.Equal(4, response.Pagination.TotalPages);
        Assert.True(response.Pagination.HasPreviousPage);
        Assert.True(response.Pagination.HasNextPage);
        Assert.Equal(message, response.Message);
        Assert.Equal(correlationId, response.CorrelationId);
        Assert.Equal(version, response.Version);
    }

    [Fact]
    public void PagedApiResponse_CreateError_ShouldSetCorrectProperties()
    {
        // Arrange
        var message = "Pagination failed";
        var errors = new { Page = "Invalid page number" };
        var correlationId = "error-correlation-id";
        var version = "1.0";

        // Act
        var response = PagedApiResponse<string>.CreateError(message, errors, correlationId, version);

        // Assert
        Assert.False(response.Success);
        Assert.Null(response.Data);
        Assert.Null(response.Pagination);
        Assert.Equal(message, response.Message);
        Assert.Equal(errors, response.Errors);
        Assert.Equal(correlationId, response.CorrelationId);
        Assert.Equal(version, response.Version);
    }

    [Fact]
    public void PagedApiResponse_CreateSuccess_WithEmptyData_ShouldHandleCorrectly()
    {
        // Arrange
        var emptyData = Array.Empty<string>();
        var pagination = PaginationMetadata.Create(1, 10, 0);

        // Act
        var response = PagedApiResponse<string>.CreateSuccess(emptyData, pagination);

        // Assert
        Assert.True(response.Success);
        Assert.Empty(response.Data!);
        Assert.Equal(0, response.Pagination!.TotalItems);
        Assert.Equal(0, response.Pagination.TotalPages);
        Assert.False(response.Pagination.HasPreviousPage);
        Assert.False(response.Pagination.HasNextPage);
    }

    [Fact]
    public void PagedApiResponse_CreateSuccess_WithoutOptionalParameters_ShouldSetDefaults()
    {
        // Arrange
        var data = new[] { "test" };
        var pagination = PaginationMetadata.Create(1, 1, 1);

        // Act
        var response = PagedApiResponse<string>.CreateSuccess(data, pagination);

        // Assert
        Assert.True(response.Success);
        Assert.Equal(data, response.Data);
        Assert.Equal(pagination, response.Pagination);
        Assert.Null(response.Message);
        Assert.Null(response.CorrelationId);
        Assert.Null(response.Version);
        Assert.Null(response.Errors);
    }

    [Fact]
    public void PagedApiResponse_Inheritance_ShouldWork()
    {
        // Arrange
        var data = new[] { 1, 2, 3 };
        var response = new PagedApiResponse<int>
        {
            Success = true,
            Data = data,
            Pagination = PaginationMetadata.Create(1, 10, 3),
            Message = "Test message",
            CorrelationId = "test-id",
            Version = "1.0"
        };

        // Act & Assert - Verify inheritance works correctly
        Assert.IsAssignableFrom<ApiResponse>(response);
        Assert.True(response.Success);
        Assert.Equal(data, response.Data);
        Assert.NotNull(response.Pagination);
        Assert.Equal("Test message", response.Message);
        Assert.Equal("test-id", response.CorrelationId);
        Assert.Equal("1.0", response.Version);
    }

    [Theory]
    [InlineData(1, 10, 50)]
    [InlineData(5, 20, 100)]
    [InlineData(1, 5, 3)]
    public void PagedApiResponse_CreateSuccess_ShouldPreservePaginationCalculations(int page, int pageSize, int totalItems)
    {
        // Arrange
        var data = Enumerable.Range(1, Math.Min(pageSize, totalItems)).ToArray();

        // Act
        var response = PagedApiResponse<int>.CreateSuccess(data, page, pageSize, totalItems);

        // Assert
        var expectedTotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
        Assert.Equal(page, response.Pagination!.CurrentPage);
        Assert.Equal(pageSize, response.Pagination.PageSize);
        Assert.Equal(totalItems, response.Pagination.TotalItems);
        Assert.Equal(expectedTotalPages, response.Pagination.TotalPages);
    }
}