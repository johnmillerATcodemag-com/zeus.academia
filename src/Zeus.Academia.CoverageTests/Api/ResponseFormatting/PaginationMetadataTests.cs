using Xunit;
using Zeus.Academia.Api.Models.Responses;

namespace Zeus.Academia.CoverageTests.Api.ResponseFormatting;

/// <summary>
/// Unit tests for PaginationMetadata class
/// </summary>
public class PaginationMetadataTests
{
    [Theory]
    [InlineData(1, 10, 50, 1, 5, false, true, null, 2, 0, 9)]
    [InlineData(3, 10, 50, 3, 5, true, true, 2, 4, 20, 29)]
    [InlineData(5, 10, 50, 5, 5, true, false, 4, null, 40, 49)]
    [InlineData(1, 20, 15, 1, 1, false, false, null, null, 0, 14)]
    [InlineData(2, 10, 5, 2, 1, true, false, 1, null, 10, 4)] // Edge case: requesting beyond available data
    public void PaginationMetadata_Create_ShouldCalculateCorrectValues(
        int currentPage, int pageSize, int totalItems,
        int expectedCurrentPage, int expectedTotalPages,
        bool expectedHasPrevious, bool expectedHasNext,
        int? expectedPreviousPage, int? expectedNextPage,
        int expectedFirstItemIndex, int expectedLastItemIndex)
    {
        // Act
        var metadata = PaginationMetadata.Create(currentPage, pageSize, totalItems);

        // Assert
        Assert.Equal(expectedCurrentPage, metadata.CurrentPage);
        Assert.Equal(pageSize, metadata.PageSize);
        Assert.Equal(totalItems, metadata.TotalItems);
        Assert.Equal(expectedTotalPages, metadata.TotalPages);
        Assert.Equal(expectedHasPrevious, metadata.HasPreviousPage);
        Assert.Equal(expectedHasNext, metadata.HasNextPage);
        Assert.Equal(expectedPreviousPage, metadata.PreviousPage);
        Assert.Equal(expectedNextPage, metadata.NextPage);
        Assert.Equal(expectedFirstItemIndex, metadata.FirstItemIndex);
        Assert.Equal(expectedLastItemIndex, metadata.LastItemIndex);
    }

    [Fact]
    public void PaginationMetadata_Create_WithZeroItems_ShouldHandleCorrectly()
    {
        // Act
        var metadata = PaginationMetadata.Create(1, 10, 0);

        // Assert
        Assert.Equal(1, metadata.CurrentPage);
        Assert.Equal(10, metadata.PageSize);
        Assert.Equal(0, metadata.TotalItems);
        Assert.Equal(0, metadata.TotalPages);
        Assert.False(metadata.HasPreviousPage);
        Assert.False(metadata.HasNextPage);
        Assert.Null(metadata.PreviousPage);
        Assert.Null(metadata.NextPage);
        Assert.Equal(0, metadata.FirstItemIndex);
        Assert.Equal(0, metadata.LastItemIndex);
    }

    [Fact]
    public void PaginationMetadata_Create_WithSingleItem_ShouldHandleCorrectly()
    {
        // Act
        var metadata = PaginationMetadata.Create(1, 10, 1);

        // Assert
        Assert.Equal(1, metadata.CurrentPage);
        Assert.Equal(10, metadata.PageSize);
        Assert.Equal(1, metadata.TotalItems);
        Assert.Equal(1, metadata.TotalPages);
        Assert.False(metadata.HasPreviousPage);
        Assert.False(metadata.HasNextPage);
        Assert.Null(metadata.PreviousPage);
        Assert.Null(metadata.NextPage);
        Assert.Equal(0, metadata.FirstItemIndex);
        Assert.Equal(0, metadata.LastItemIndex);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(5, 5)]
    [InlineData(50, 50)]
    public void PaginationMetadata_Create_WithPageSizeEqualToTotalItems_ShouldHandleCorrectly(int pageSize, int totalItems)
    {
        // Act
        var metadata = PaginationMetadata.Create(1, pageSize, totalItems);

        // Assert
        Assert.Equal(1, metadata.CurrentPage);
        Assert.Equal(pageSize, metadata.PageSize);
        Assert.Equal(totalItems, metadata.TotalItems);
        Assert.Equal(1, metadata.TotalPages);
        Assert.False(metadata.HasPreviousPage);
        Assert.False(metadata.HasNextPage);
        Assert.Null(metadata.PreviousPage);
        Assert.Null(metadata.NextPage);
        Assert.Equal(0, metadata.FirstItemIndex);
        Assert.Equal(totalItems - 1, metadata.LastItemIndex);
    }

    [Fact]
    public void PaginationMetadata_Create_WithLargeNumbers_ShouldHandleCorrectly()
    {
        // Arrange
        int currentPage = 1000;
        int pageSize = 100;
        int totalItems = 150000;

        // Act
        var metadata = PaginationMetadata.Create(currentPage, pageSize, totalItems);

        // Assert
        Assert.Equal(1000, metadata.CurrentPage);
        Assert.Equal(100, metadata.PageSize);
        Assert.Equal(150000, metadata.TotalItems);
        Assert.Equal(1500, metadata.TotalPages);
        Assert.True(metadata.HasPreviousPage);
        Assert.True(metadata.HasNextPage);
        Assert.Equal(999, metadata.PreviousPage);
        Assert.Equal(1001, metadata.NextPage);
        Assert.Equal(99900, metadata.FirstItemIndex);
        Assert.Equal(99999, metadata.LastItemIndex);
    }

    [Theory]
    [InlineData(2, 10, 15)] // Partial last page
    [InlineData(3, 5, 12)]  // Partial last page
    public void PaginationMetadata_Create_WithPartialLastPage_ShouldCalculateLastItemIndexCorrectly(
        int currentPage, int pageSize, int totalItems)
    {
        // Act
        var metadata = PaginationMetadata.Create(currentPage, pageSize, totalItems);

        // Assert
        var expectedFirstIndex = (currentPage - 1) * pageSize;
        var expectedLastIndex = Math.Min(expectedFirstIndex + pageSize - 1, totalItems - 1);

        Assert.Equal(expectedFirstIndex, metadata.FirstItemIndex);
        Assert.Equal(expectedLastIndex, metadata.LastItemIndex);
    }
}