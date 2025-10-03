using Microsoft.AspNetCore.Mvc;
using Zeus.Academia.Api.Controllers;
using Zeus.Academia.Api.Models.Requests;
using Zeus.Academia.Api.Models.Responses;
using Zeus.Academia.Api.Extensions;

namespace Zeus.Academia.Api.Controllers.V1;

/// <summary>
/// Sample controller demonstrating response formatting and pagination features
/// </summary>
/// <remarks>
/// This controller demonstrates the implementation of Task 6 features:
/// - Standardized response wrappers
/// - Pagination with skip/take parameters
/// - Content negotiation (JSON/XML)
/// - Proper error handling
/// </remarks>
[ApiController]
[Route("api/v1/[controller]")]
public class SampleDataController : BaseApiController
{
    private readonly ILogger<SampleDataController> _logger;

    // Sample data for demonstration
    private static readonly List<SampleItem> _sampleData = GenerateSampleData();

    public SampleDataController(ILogger<SampleDataController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get a single sample item by ID
    /// </summary>
    /// <param name="id">Item ID</param>
    /// <returns>Sample item or not found</returns>
    /// <response code="200">Returns the sample item</response>
    /// <response code="404">Item not found</response>
    [HttpGet("{id:int}")]
    [Produces("application/json", "application/xml")]
    public IActionResult GetSampleItem(int id)
    {
        _logger.LogInformation("Getting sample item with ID: {Id}", id);

        var item = _sampleData.FirstOrDefault(x => x.Id == id);
        if (item == null)
        {
            return NotFound($"Sample item with ID {id} not found");
        }

        return Success(item, "Sample item retrieved successfully");
    }

    /// <summary>
    /// Get paginated list of sample items
    /// </summary>
    /// <param name="pagination">Pagination parameters</param>
    /// <returns>Paginated list of sample items</returns>
    /// <response code="200">Returns paginated sample items</response>
    /// <response code="400">Invalid pagination parameters</response>
    [HttpGet]
    [Produces("application/json", "application/xml")]
    public IActionResult GetSampleItems([FromQuery] PaginationParameters pagination)
    {
        _logger.LogInformation("Getting sample items - Page: {Page}, PageSize: {PageSize}, Skip: {Skip}, Take: {Take}",
            pagination.Page, pagination.PageSize, pagination.Skip, pagination.Take);

        var query = _sampleData.AsQueryable();

        // Apply search filter if provided
        if (!string.IsNullOrWhiteSpace(pagination.Search))
        {
            query = query.Where(x => x.Name.Contains(pagination.Search, StringComparison.OrdinalIgnoreCase) ||
                                   x.Description.Contains(pagination.Search, StringComparison.OrdinalIgnoreCase));
        }

        // Apply sorting if provided
        if (!string.IsNullOrWhiteSpace(pagination.SortBy))
        {
            query = pagination.SortBy.ToLowerInvariant() switch
            {
                "name" => pagination.IsSortDescending ?
                    query.OrderByDescending(x => x.Name) :
                    query.OrderBy(x => x.Name),
                "createdat" => pagination.IsSortDescending ?
                    query.OrderByDescending(x => x.CreatedAt) :
                    query.OrderBy(x => x.CreatedAt),
                "value" => pagination.IsSortDescending ?
                    query.OrderByDescending(x => x.Value) :
                    query.OrderBy(x => x.Value),
                _ => query.OrderBy(x => x.Id) // Default sort by ID
            };
        }
        else
        {
            query = query.OrderBy(x => x.Id); // Default sort
        }

        // Apply pagination (simple implementation for now)
        var totalCount = query.Count();
        var items = query
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToList();

        return PagedSuccess(
            items,
            pagination.Page,
            pagination.PageSize,
            totalCount,
            $"Retrieved {items.Count} items from page {pagination.Page}");
    }

    /// <summary>
    /// Create a new sample item
    /// </summary>
    /// <param name="item">Sample item to create</param>
    /// <returns>Created sample item</returns>
    /// <response code="201">Sample item created successfully</response>
    /// <response code="400">Invalid item data</response>
    [HttpPost]
    [Produces("application/json", "application/xml")]
    [Consumes("application/json", "application/xml")]
    public IActionResult CreateSampleItem([FromBody] CreateSampleItemRequest item)
    {
        _logger.LogInformation("Creating new sample item: {Name}", item.Name);

        if (string.IsNullOrWhiteSpace(item.Name))
        {
            return Error("Item name is required", 400, new { Name = "Name field is required" });
        }

        var newItem = new SampleItem
        {
            Id = _sampleData.Max(x => x.Id) + 1,
            Name = item.Name,
            Description = item.Description ?? string.Empty,
            Value = item.Value,
            CreatedAt = DateTime.UtcNow
        };

        _sampleData.Add(newItem);

        return Success(newItem, "Sample item created successfully");
    }

    /// <summary>
    /// Update an existing sample item
    /// </summary>
    /// <param name="id">Item ID</param>
    /// <param name="item">Updated item data</param>
    /// <returns>Updated sample item</returns>
    /// <response code="200">Sample item updated successfully</response>
    /// <response code="404">Item not found</response>
    /// <response code="400">Invalid item data</response>
    [HttpPut("{id:int}")]
    [Produces("application/json", "application/xml")]
    [Consumes("application/json", "application/xml")]
    public IActionResult UpdateSampleItem(int id, [FromBody] UpdateSampleItemRequest item)
    {
        _logger.LogInformation("Updating sample item with ID: {Id}", id);

        var existingItem = _sampleData.FirstOrDefault(x => x.Id == id);
        if (existingItem == null)
        {
            return NotFound($"Sample item with ID {id} not found");
        }

        if (string.IsNullOrWhiteSpace(item.Name))
        {
            return Error("Item name is required", 400, new { Name = "Name field is required" });
        }

        existingItem.Name = item.Name;
        existingItem.Description = item.Description ?? string.Empty;
        existingItem.Value = item.Value;

        return Success(existingItem, "Sample item updated successfully");
    }

    /// <summary>
    /// Delete a sample item
    /// </summary>
    /// <param name="id">Item ID</param>
    /// <returns>Deletion confirmation</returns>
    /// <response code="200">Sample item deleted successfully</response>
    /// <response code="404">Item not found</response>
    [HttpDelete("{id:int}")]
    [Produces("application/json", "application/xml")]
    public IActionResult DeleteSampleItem(int id)
    {
        _logger.LogInformation("Deleting sample item with ID: {Id}", id);

        var existingItem = _sampleData.FirstOrDefault(x => x.Id == id);
        if (existingItem == null)
        {
            return NotFound($"Sample item with ID {id} not found");
        }

        _sampleData.Remove(existingItem);

        return Success($"Sample item with ID {id} deleted successfully");
    }

    /// <summary>
    /// Demonstrate error response formatting
    /// </summary>
    /// <returns>Error response</returns>
    /// <response code="400">Demonstrates error response format</response>
    [HttpGet("error-demo")]
    [Produces("application/json", "application/xml")]
    public IActionResult ErrorDemo()
    {
        _logger.LogWarning("Error demo endpoint called");

        var validationErrors = new
        {
            Name = new[] { "Name is required", "Name must be at least 3 characters" },
            Email = new[] { "Email must be valid" },
            Age = new[] { "Age must be between 18 and 100" }
        };

        return Error("Validation failed", 400, validationErrors);
    }

    /// <summary>
    /// Generate sample data for demonstration
    /// </summary>
    private static List<SampleItem> GenerateSampleData()
    {
        var items = new List<SampleItem>();
        var random = new Random(42); // Fixed seed for consistent data

        for (int i = 1; i <= 50; i++)
        {
            items.Add(new SampleItem
            {
                Id = i,
                Name = $"Sample Item {i}",
                Description = $"This is a description for sample item number {i}. It contains some sample text to demonstrate pagination and search functionality.",
                Value = random.Next(1, 1000),
                CreatedAt = DateTime.UtcNow.AddDays(-random.Next(0, 365))
            });
        }

        return items;
    }
}

/// <summary>
/// Sample item model for demonstration
/// </summary>
public class SampleItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Value { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Request model for creating sample items
/// </summary>
public class CreateSampleItemRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Value { get; set; }
}

/// <summary>
/// Request model for updating sample items
/// </summary>
public class UpdateSampleItemRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Value { get; set; }
}