using Xunit;
using Zeus.Academia.Api.Extensions;
using Zeus.Academia.Api.Models.Requests;
using Zeus.Academia.Api.Models.Responses;

namespace Zeus.Academia.CoverageTests.Api.ResponseFormatting;

/// <summary>
/// Unit tests for PaginationExtensions
/// </summary>
public class PaginationExtensionsTests
{
    private static readonly List<TestItem> _testData = GenerateTestData();

    [Fact]
    public void ToPaginatedList_IQueryable_ShouldReturnCorrectPage()
    {
        // Arrange
        var queryable = _testData.AsQueryable();
        var parameters = new Zeus.Academia.Api.Models.Common.PaginationParameters { PageNumber = 2, PageSize = 5 };

        // Act
        var result = queryable.ToPaginatedList(parameters);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.Items.Count());
        Assert.NotNull(result.Pagination);
        Assert.Equal(2, result.Pagination.CurrentPage);
        Assert.Equal(5, result.Pagination.PageSize);
        Assert.Equal(_testData.Count, result.Pagination.TotalItems);

        // Verify correct items are returned (items 5-9 for page 2)
        var expectedItems = _testData.Skip(5).Take(5);
        Assert.Equal(expectedItems.Select(x => x.Id), result.Items.Select(x => x.Id));
    }

    [Fact]
    public void ToPaginatedList_IEnumerable_ShouldReturnCorrectPage()
    {
        // Arrange
        var queryable = _testData.AsQueryable();
        var parameters = new Zeus.Academia.Api.Models.Common.PaginationParameters { PageNumber = 1, PageSize = 3 };

        // Act
        var result = queryable.ToPaginatedList(parameters);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Items.Count());
        Assert.NotNull(result.Pagination);
        Assert.Equal(1, result.Pagination.CurrentPage);
        Assert.Equal(3, result.Pagination.PageSize);
        Assert.Equal(_testData.Count, result.Pagination.TotalItems);

        // Verify correct items are returned (first 3 items)
        var expectedItems = _testData.Take(3);
        Assert.Equal(expectedItems.Select(x => x.Id), result.Items.Select(x => x.Id));
    }

    [Fact]
    public void ToPaginatedList_FirstPage_ShouldCalculateCorrectMetadata()
    {
        // Arrange
        var queryable = _testData.AsQueryable();
        var parameters = new Zeus.Academia.Api.Models.Common.PaginationParameters { PageNumber = 1, PageSize = 5 };

        // Act
        var result = queryable.ToPaginatedList(parameters);

        // Assert
        Assert.NotNull(result.Pagination);
        Assert.Equal(1, result.Pagination.CurrentPage);
        Assert.Equal(5, result.Pagination.PageSize);
        Assert.Equal(_testData.Count, result.Pagination.TotalItems);
        Assert.Equal(2, result.Pagination.TotalPages); // 10 items / 5 per page = 2 pages
        Assert.False(result.Pagination.HasPreviousPage);
        Assert.True(result.Pagination.HasNextPage);
        Assert.Null(result.Pagination.PreviousPage);
        Assert.Equal(2, result.Pagination.NextPage);
    }

    [Fact]
    public void ToPaginatedList_LastPage_ShouldCalculateCorrectMetadata()
    {
        // Arrange
        var queryable = _testData.AsQueryable();
        var parameters = new Zeus.Academia.Api.Models.Common.PaginationParameters { PageNumber = 2, PageSize = 5 };

        // Act
        var result = queryable.ToPaginatedList(parameters);

        // Assert
        Assert.NotNull(result.Pagination);
        Assert.Equal(2, result.Pagination.CurrentPage);
        Assert.Equal(5, result.Pagination.PageSize);
        Assert.Equal(_testData.Count, result.Pagination.TotalItems);
        Assert.Equal(2, result.Pagination.TotalPages);
        Assert.True(result.Pagination.HasPreviousPage);
        Assert.False(result.Pagination.HasNextPage);
        Assert.Equal(1, result.Pagination.PreviousPage);
        Assert.Null(result.Pagination.NextPage);
    }

    [Fact]
    public void ToPaginatedList_EmptyCollection_ShouldReturnEmptyResult()
    {
        // Arrange
        var emptyQueryable = Enumerable.Empty<TestItem>().AsQueryable();
        var parameters = new Zeus.Academia.Api.Models.Common.PaginationParameters { PageNumber = 1, PageSize = 5 };

        // Act
        var result = emptyQueryable.ToPaginatedList(parameters);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.NotNull(result.Pagination);
        Assert.Equal(1, result.Pagination.CurrentPage);
        Assert.Equal(5, result.Pagination.PageSize);
        Assert.Equal(0, result.Pagination.TotalItems);
        Assert.Equal(0, result.Pagination.TotalPages);
        Assert.False(result.Pagination.HasPreviousPage);
        Assert.False(result.Pagination.HasNextPage);
    }

    [Fact]
    public void ToPaginatedList_PageBeyondData_ShouldReturnEmptyResults()
    {
        // Arrange
        var queryable = _testData.AsQueryable();
        var parameters = new Zeus.Academia.Api.Models.Common.PaginationParameters { PageNumber = 5, PageSize = 5 }; // Page 5 with 5 items per page, but only 10 total items

        // Act
        var result = queryable.ToPaginatedList(parameters);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.NotNull(result.Pagination);
        Assert.Equal(5, result.Pagination.CurrentPage);
        Assert.Equal(5, result.Pagination.PageSize);
        Assert.Equal(_testData.Count, result.Pagination.TotalItems);
        Assert.Equal(2, result.Pagination.TotalPages);
    }

    [Fact]
    public void ToApiResponse_ShouldCreateCorrectPagedApiResponse()
    {
        // Arrange
        var pagedResult = new PagedResult<TestItem>
        {
            Items = _testData.Take(3),
            Pagination = PaginationMetadata.Create(1, 3, _testData.Count)
        };
        // Act
        var response = pagedResult.ToPagedResponse();

        // Assert
        Assert.Equal(pagedResult.Items, response.Data);
        Assert.Equal(pagedResult.Pagination?.CurrentPage, response.PageNumber);
        Assert.Equal(pagedResult.Pagination?.PageSize, response.PageSize);
        Assert.Equal(pagedResult.Pagination?.TotalItems, response.TotalCount);
        Assert.Equal(pagedResult.Pagination?.TotalPages, response.TotalPages);
    }

    [Fact]
    public void ToPagedApiResponse_IQueryable_ShouldCreateCorrectResponse()
    {
        // Arrange
        var queryable = _testData.AsQueryable();
        var parameters = new Zeus.Academia.Api.Models.Common.PaginationParameters { PageNumber = 1, PageSize = 4 };


        // Act
        var pagedResult = queryable.ToPaginatedList(parameters);
        var response = pagedResult.ToPagedResponse();

        // Assert
        Assert.Equal(4, response.Data.Count());
        Assert.Equal(1, response.PageNumber);
        Assert.Equal(4, response.PageSize);
        Assert.Equal(_testData.Count, response.TotalCount);
        Assert.Equal(3, response.TotalPages); // 10 items / 4 per page = 3 pages (rounded up)
    }

    [Fact]
    public void ToPagedApiResponse_IEnumerable_ShouldCreateCorrectResponse()
    {
        // Arrange
        var queryable = _testData.AsQueryable();
        var parameters = new Zeus.Academia.Api.Models.Common.PaginationParameters { PageNumber = 2, PageSize = 3 };

        // Act
        var pagedResult = queryable.ToPaginatedList(parameters);
        var response = pagedResult.ToPagedResponse();

        // Assert
        Assert.Equal(3, response.Data.Count());
        Assert.Equal(2, response.PageNumber);
        Assert.Equal(3, response.PageSize);
        Assert.Equal(_testData.Count, response.TotalCount);
        Assert.Equal(4, response.TotalPages); // 10 items / 3 per page = 4 pages (rounded up)
    }

    [Theory]
    [InlineData(1, 10, 10, 1)]
    [InlineData(2, 5, 10, 2)]
    [InlineData(3, 3, 10, 4)]
    [InlineData(1, 20, 10, 1)]
    public void ToPaginatedList_VariousPageSizes_ShouldCalculateCorrectly(int page, int pageSize, int totalItems, int expectedTotalPages)
    {
        // Arrange
        var data = Enumerable.Range(1, totalItems).Select(i => new TestItem { Id = i, Name = $"Item {i}" }).ToList();
        var queryable = data.AsQueryable();
        var parameters = new Zeus.Academia.Api.Models.Common.PaginationParameters { PageNumber = page, PageSize = pageSize };

        // Act
        var result = queryable.ToPaginatedList(parameters);

        // Assert
        Assert.NotNull(result.Pagination);
        Assert.Equal(page, result.Pagination.CurrentPage);
        Assert.Equal(pageSize, result.Pagination.PageSize);
        Assert.Equal(totalItems, result.Pagination.TotalItems);
        Assert.Equal(expectedTotalPages, result.Pagination.TotalPages);

        var expectedItemCount = Math.Min(pageSize, Math.Max(0, totalItems - (page - 1) * pageSize));
        Assert.Equal(expectedItemCount, result.Items.Count());
    }

    private static List<TestItem> GenerateTestData()
    {
        return Enumerable.Range(1, 10)
            .Select(i => new TestItem { Id = i, Name = $"Test Item {i}" })
            .ToList();
    }

    private class TestItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}