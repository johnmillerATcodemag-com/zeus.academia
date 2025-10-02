using System.ComponentModel.DataAnnotations;

namespace Zeus.Academia.Api.Configuration;

/// <summary>
/// Database configuration settings
/// </summary>
public class DatabaseConfiguration
{
    public const string SectionName = "Database";

    /// <summary>
    /// Connection string for the main database
    /// </summary>
    [Required]
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Database command timeout in seconds
    /// </summary>
    [Range(1, 300)]
    public int CommandTimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Enable sensitive data logging (development only)
    /// </summary>
    public bool EnableSensitiveDataLogging { get; set; } = false;

    /// <summary>
    /// Enable detailed errors (development only)
    /// </summary>
    public bool EnableDetailedErrors { get; set; } = false;

    /// <summary>
    /// Maximum retry count for database operations
    /// </summary>
    [Range(0, 10)]
    public int MaxRetryCount { get; set; } = 3;

    /// <summary>
    /// Maximum delay between retries in seconds
    /// </summary>
    [Range(1, 60)]
    public int MaxRetryDelaySeconds { get; set; } = 30;

    /// <summary>
    /// Enable connection pooling
    /// </summary>
    public bool EnablePooling { get; set; } = true;

    /// <summary>
    /// Minimum pool size
    /// </summary>
    [Range(0, 100)]
    public int MinPoolSize { get; set; } = 0;

    /// <summary>
    /// Maximum pool size
    /// </summary>
    [Range(1, 1000)]
    public int MaxPoolSize { get; set; } = 100;
}