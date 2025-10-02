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
        var parameters = PaginationParameters.Create(2, 5);

        // Act
        var result = queryable.ToPaginatedList(parameters);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.Items.Count());
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
        var enumerable = _testData.AsEnumerable();
        var parameters = PaginationParameters.Create(1, 3);

        // Act
        var result = enumerable.ToPaginatedList(parameters);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Items.Count());
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
        var parameters = PaginationParameters.Create(1, 5);

        // Act
        var result = queryable.ToPaginatedList(parameters);

        // Assert
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
        var parameters = PaginationParameters.Create(2, 5);

        // Act
        var result = queryable.ToPaginatedList(parameters);

        // Assert
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
        var parameters = PaginationParameters.Create(1, 5);

        // Act
        var result = emptyQueryable.ToPaginatedList(parameters);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Items);
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
        var parameters = PaginationParameters.Create(5, 5); // Page 5 with 5 items per page, but only 10 total items

        // Act
        var result = queryable.ToPaginatedList(parameters);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Items);
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
        var message = "Data retrieved";
        var correlationId = "test-id";
        var version = "1.0";

        // Act
        var response = pagedResult.ToApiResponse(message, correlationId, version);

        // Assert
        Assert.True(response.Success);
        Assert.Equal(pagedResult.Items, response.Data);
        Assert.Equal(pagedResult.Pagination, response.Pagination);
        Assert.Equal(message, response.Message);
        Assert.Equal(correlationId, response.CorrelationId);
        Assert.Equal(version, response.Version);
    }

    [Fact]
    public void ToPagedApiResponse_IQueryable_ShouldCreateCorrectResponse()
    {
        // Arrange
        var queryable = _testData.AsQueryable();
        var parameters = PaginationParameters.Create(1, 4);
        var message = "Items retrieved";
        var correlationId = "query-test";
        var version = "2.0";

        // Act
        var response = queryable.ToPagedApiResponse(parameters, message, correlationId, version);

        // Assert
        Assert.True(response.Success);
        Assert.Equal(4, response.Data!.Count());
        Assert.Equal(message, response.Message);
        Assert.Equal(correlationId, response.CorrelationId);
        Assert.Equal(version, response.Version);
        Assert.NotNull(response.Pagination);
        Assert.Equal(1, response.Pagination.CurrentPage);
        Assert.Equal(4, response.Pagination.PageSize);
        Assert.Equal(_testData.Count, response.Pagination.TotalItems);
    }

    [Fact]
    public void ToPagedApiResponse_IEnumerable_ShouldCreateCorrectResponse()
    {
        // Arrange
        var enumerable = _testData.AsEnumerable();
        var parameters = PaginationParameters.Create(2, 3);
        var message = "Items retrieved";
        var correlationId = "enum-test";
        var version = "1.5";

        // Act
        var response = enumerable.ToPagedApiResponse(parameters, message, correlationId, version);

        // Assert
        Assert.True(response.Success);
        Assert.Equal(3, response.Data!.Count());
        Assert.Equal(message, response.Message);
        Assert.Equal(correlationId, response.CorrelationId);
        Assert.Equal(version, response.Version);
        Assert.NotNull(response.Pagination);
        Assert.Equal(2, response.Pagination.CurrentPage);
        Assert.Equal(3, response.Pagination.PageSize);
        Assert.Equal(_testData.Count, response.Pagination.TotalItems);
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
        var parameters = PaginationParameters.Create(page, pageSize);

        // Act
        var result = queryable.ToPaginatedList(parameters);

        // Assert
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