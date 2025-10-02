using System.ComponentModel.DataAnnotations;

namespace Zeus.Academia.Api.Configuration;

/// <summary>
/// Configuration settings for the Zeus Academia API
/// </summary>
public class ApiConfiguration
{
    public const string SectionName = "Api";

    /// <summary>
    /// API version information
    /// </summary>
    [Required]
    public string Version { get; set; } = "1.0";

    /// <summary>
    /// API title for documentation
    /// </summary>
    [Required]
    public string Title { get; set; } = "Zeus Academia API";

    /// <summary>
    /// API description
    /// </summary>
    public string Description { get; set; } = "Academic Management System API";

    /// <summary>
    /// Contact information for API support
    /// </summary>
    public ContactInfo Contact { get; set; } = new();

    /// <summary>
    /// Maximum request size in bytes
    /// </summary>
    [Range(1024, int.MaxValue)]
    public long MaxRequestSize { get; set; } = 10 * 1024 * 1024; // 10MB

    /// <summary>
    /// Request timeout in seconds
    /// </summary>
    [Range(1, 300)]
    public int RequestTimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Enable API metrics collection
    /// </summary>
    public bool EnableMetrics { get; set; } = true;

    /// <summary>
    /// Enable detailed error responses in development
    /// </summary>
    public bool ShowDetailedErrors { get; set; } = false;
}

/// <summary>
/// Contact information for API documentation
/// </summary>
public class ContactInfo
{
    public string Name { get; set; } = "Zeus Academia Support";
    public string Email { get; set; } = "support@zeusacademia.com";
    public string Url { get; set; } = "https://zeusacademia.com/support";
}