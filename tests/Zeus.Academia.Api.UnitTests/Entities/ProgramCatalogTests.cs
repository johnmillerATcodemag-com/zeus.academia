using Xunit;
using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.Api.UnitTests.Entities;

/// <summary>
/// Unit tests for ProgramCatalog entity.
/// These are NEW tests that don't affect existing test count.
/// </summary>
public class ProgramCatalogTests
{
    [Fact]
    public void ProgramCatalog_Should_Have_Valid_Properties()
    {
        // Arrange & Act
        var catalog = new ProgramCatalog
        {
            Id = 1,
            CatalogName = "2025-2026 Academic Catalog",
            AcademicYear = 2025,
            Status = CatalogStatus.Published,
            PublishedDate = new DateTime(2025, 7, 1),
            EffectiveDate = new DateTime(2025, 8, 15)
        };

        // Assert
        Assert.Equal(1, catalog.Id);
        Assert.Equal("2025-2026 Academic Catalog", catalog.CatalogName);
        Assert.Equal(2025, catalog.AcademicYear);
        Assert.Equal(CatalogStatus.Published, catalog.Status);
        Assert.Equal(new DateTime(2025, 7, 1), catalog.PublishedDate);
        Assert.Equal(new DateTime(2025, 8, 15), catalog.EffectiveDate);
    }

    [Fact]
    public void ProgramCatalog_Should_Support_Course_Association()
    {
        // Arrange
        var catalog = new ProgramCatalog
        {
            CatalogName = "2025-2026 Catalog",
            AcademicYear = 2025
        };

        var course = new Course
        {
            CourseNumber = "CS101",
            Title = "Introduction to Programming",
            SubjectCode = "CS",
            CatalogYear = 2025 // Uses existing property
        };

        // Act
        catalog.Courses.Add(course);

        // Assert
        Assert.Single(catalog.Courses);
        Assert.Contains(course, catalog.Courses);
        Assert.Equal(catalog.AcademicYear, course.CatalogYear);
    }

    [Fact]
    public void ProgramCatalog_Should_Support_Status_Workflow()
    {
        // Arrange & Act
        var catalog = new ProgramCatalog
        {
            CatalogName = "2026-2027 Catalog",
            Status = CatalogStatus.Draft
        };

        // Simulate workflow
        catalog.Status = CatalogStatus.UnderReview;
        catalog.Status = CatalogStatus.Published;
        catalog.PublishedDate = DateTime.UtcNow;

        // Assert
        Assert.Equal(CatalogStatus.Published, catalog.Status);
        Assert.NotNull(catalog.PublishedDate);
    }
}