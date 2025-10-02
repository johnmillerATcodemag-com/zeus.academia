using Xunit;
using Zeus.Academia.Api.Models.Requests;

namespace Zeus.Academia.CoverageTests.Api.ResponseFormatting;

/// <summary>
/// Unit tests for PaginationParameters class
/// </summary>
public class PaginationParametersTests
{
    [Fact]
    public void PaginationParameters_Default_ShouldSetCorrectValues()
    {
        // Act
        var parameters = new PaginationParameters();

        // Assert
        Assert.Equal(1, parameters.Page);
        Assert.Equal(20, parameters.PageSize);
        Assert.Equal(0, parameters.Skip);
        Assert.Equal(20, parameters.Take);
        Assert.Null(parameters.SortBy);
        Assert.Equal("asc", parameters.SortDirection);
        Assert.False(parameters.IsSortDescending);
        Assert.Null(parameters.Search);
    }

    [Fact]
    public void PaginationParameters_Create_ShouldSetPageAndSize()
    {
        // Act
        var parameters = PaginationParameters.Create(3, 15);

        // Assert
        Assert.Equal(3, parameters.Page);
        Assert.Equal(15, parameters.PageSize);
        Assert.Equal(30, parameters.Skip); // (3-1) * 15
        Assert.Equal(15, parameters.Take);
    }

    [Fact]
    public void PaginationParameters_CreateSkipTake_ShouldSetSkipAndTake()
    {
        // Act
        var parameters = PaginationParameters.CreateSkipTake(40, 10);

        // Assert
        Assert.Equal(10, parameters.Take);
        Assert.Equal(40, parameters.Skip);
        Assert.Equal(5, parameters.Page); // (40 / 10) + 1
        Assert.Equal(10, parameters.PageSize);
    }

    [Theory]
    [InlineData(1, 10, 0, 10)]
    [InlineData(2, 10, 10, 10)]
    [InlineData(3, 15, 30, 15)]
    [InlineData(5, 20, 80, 20)]
    public void PaginationParameters_SetPage_ShouldUpdateSkip(int page, int pageSize, int expectedSkip, int expectedTake)
    {
        // Arrange
        var parameters = new PaginationParameters { PageSize = pageSize };

        // Act
        parameters.Page = page;

        // Assert
        Assert.Equal(page, parameters.Page);
        Assert.Equal(expectedSkip, parameters.Skip);
        Assert.Equal(expectedTake, parameters.Take);
    }

    [Theory]
    [InlineData(5, 3, 10, 5)]
    [InlineData(25, 2, 25, 25)]
    [InlineData(50, 1, 0, 50)]
    public void PaginationParameters_SetPageSize_ShouldUpdateTakeAndSkip(int pageSize, int page, int expectedSkip, int expectedTake)
    {
        // Arrange
        var parameters = new PaginationParameters { Page = page };

        // Act
        parameters.PageSize = pageSize;

        // Assert
        Assert.Equal(pageSize, parameters.PageSize);
        Assert.Equal(expectedTake, parameters.Take);
        Assert.Equal(expectedSkip, parameters.Skip);
    }

    [Theory]
    [InlineData(0, 5, 1)] // Should clamp to page 1
    [InlineData(30, 10, 4)] // 30 / 10 + 1 = 4
    [InlineData(25, 10, 3)] // 25 / 10 + 1 = 3.5 -> 3
    public void PaginationParameters_SetSkip_ShouldUpdatePage(int skip, int pageSize, int expectedPage)
    {
        // Arrange
        var parameters = new PaginationParameters { PageSize = pageSize };

        // Act
        parameters.Skip = skip;

        // Assert
        Assert.Equal(Math.Max(0, skip), parameters.Skip);
        Assert.Equal(expectedPage, parameters.Page);
        Assert.Equal(pageSize, parameters.Take);
    }

    [Theory]
    [InlineData(5, 20, 5, 5)]   // skip 20, take 5 -> page = (20/5)+1 = 5
    [InlineData(15, 40, 3, 15)] // skip 40, take 15 -> page = (40/15)+1 = 2.67 + 1 = 3
    [InlineData(30, 10, 1, 30)] // skip 10, take 30 -> page = (10/30)+1 = 1.33 -> 1
    public void PaginationParameters_SetTake_ShouldUpdatePageSizeAndPage(int take, int skip, int expectedPage, int expectedTake)
    {
        // Arrange
        var parameters = new PaginationParameters { Skip = skip };

        // Act
        parameters.Take = take;

        // Assert
        Assert.Equal(Math.Clamp(take, 1, 100), expectedTake);
        Assert.Equal(expectedTake, parameters.PageSize);
        Assert.Equal(expectedPage, parameters.Page);
    }

    [Theory]
    [InlineData("asc", false)]
    [InlineData("desc", true)]
    [InlineData("descending", true)]
    [InlineData("ASC", false)]
    [InlineData("DESC", true)]
    [InlineData("DESCENDING", true)]
    [InlineData("invalid", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void PaginationParameters_IsSortDescending_ShouldDetectDirection(string? sortDirection, bool expectedDescending)
    {
        // Arrange
        var parameters = new PaginationParameters();

        // Act
        if (sortDirection != null)
        {
            parameters.SortDirection = sortDirection;
        }

        // Assert
        Assert.Equal(expectedDescending, parameters.IsSortDescending);
    }

    [Theory]
    [InlineData(0, 1)] // Minimum page is 1
    [InlineData(-5, 1)] // Negative pages become 1
    [InlineData(1000, 1000)] // Large pages are preserved
    public void PaginationParameters_SetPage_ShouldEnforceMinimum(int inputPage, int expectedPage)
    {
        // Arrange
        var parameters = new PaginationParameters();

        // Act
        parameters.Page = inputPage;

        // Assert
        Assert.Equal(expectedPage, parameters.Page);
    }

    [Theory]
    [InlineData(0, 1)] // Minimum page size is 1
    [InlineData(-10, 1)] // Negative page sizes become 1
    [InlineData(50, 50)] // Valid page sizes are preserved
    [InlineData(150, 100)] // Maximum page size is 100
    public void PaginationParameters_SetPageSize_ShouldEnforceLimits(int inputPageSize, int expectedPageSize)
    {
        // Arrange
        var parameters = new PaginationParameters();

        // Act
        parameters.PageSize = inputPageSize;

        // Assert
        Assert.Equal(expectedPageSize, parameters.PageSize);
        Assert.Equal(expectedPageSize, parameters.Take);
    }

    [Theory]
    [InlineData(-10, 0)] // Negative skip becomes 0
    [InlineData(0, 0)] // Zero skip is preserved
    [InlineData(100, 100)] // Positive skip is preserved
    public void PaginationParameters_SetSkip_ShouldEnforceMinimum(int inputSkip, int expectedSkip)
    {
        // Arrange
        var parameters = new PaginationParameters();

        // Act
        parameters.Skip = inputSkip;

        // Assert
        Assert.Equal(expectedSkip, parameters.Skip);
    }

    [Theory]
    [InlineData(0, 1)] // Minimum take is 1
    [InlineData(-5, 1)] // Negative take becomes 1
    [InlineData(50, 50)] // Valid take is preserved
    [InlineData(150, 100)] // Maximum take is 100
    public void PaginationParameters_SetTake_ShouldEnforceLimits(int inputTake, int expectedTake)
    {
        // Arrange
        var parameters = new PaginationParameters();

        // Act
        parameters.Take = inputTake;

        // Assert
        Assert.Equal(expectedTake, parameters.Take);
        Assert.Equal(expectedTake, parameters.PageSize);
    }

    [Fact]
    public void PaginationParameters_DefaultFactory_ShouldReturnDefaultInstance()
    {
        // Act
        var parameters = PaginationParameters.Default();

        // Assert
        Assert.Equal(1, parameters.Page);
        Assert.Equal(20, parameters.PageSize);
        Assert.Equal(0, parameters.Skip);
        Assert.Equal(20, parameters.Take);
    }
}