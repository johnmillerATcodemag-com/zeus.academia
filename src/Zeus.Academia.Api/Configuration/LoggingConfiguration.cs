using System.ComponentModel.DataAnnotations;

namespace Zeus.Academia.Api.Configuration;

/// <summary>
/// Logging configuration settings
/// </summary>
public class LoggingConfiguration
{
    public const string SectionName = "AppLogging";

    /// <summary>
    /// Default log level
    /// </summary>
    public string DefaultLevel { get; set; } = "Information";

    /// <summary>
    /// Enable request/response logging
    /// </summary>
    public bool EnableRequestLogging { get; set; } = true;

    /// <summary>
    /// Enable performance logging
    /// </summary>
    public bool EnablePerformanceLogging { get; set; } = true;

    /// <summary>
    /// Enable security event logging
    /// </summary>
    public bool EnableSecurityLogging { get; set; } = true;

    /// <summary>
    /// Log file path (optional)
    /// </summary>
    public string? LogFilePath { get; set; }

    /// <summary>
    /// Maximum log file size in MB
    /// </summary>
    [Range(1, 1000)]
    public int MaxLogFileSizeMB { get; set; } = 50;

    /// <summary>
    /// Number of log files to retain
    /// </summary>
    [Range(1, 100)]
    public int RetainedFileCountLimit { get; set; } = 10;

    /// <summary>
    /// Enable structured logging
    /// </summary>
    public bool EnableStructuredLogging { get; set; } = true;

    /// <summary>
    /// Include scopes in logging
    /// </summary>
    public bool IncludeScopes { get; set; } = true;

    /// <summary>
    /// Sensitive data logging settings
    /// </summary>
    public SensitiveDataLogging SensitiveData { get; set; } = new();
}

/// <summary>
/// Sensitive data logging configuration
/// </summary>
public class SensitiveDataLogging
{
    /// <summary>
    /// Enable logging of request bodies (development only)
    /// </summary>
    public bool LogRequestBodies { get; set; } = false;

    /// <summary>
    /// Enable logging of response bodies (development only)
    /// </summary>
    public bool LogResponseBodies { get; set; } = false;

    /// <summary>
    /// Enable logging of headers (development only)
    /// </summary>
    public bool LogHeaders { get; set; } = false;

    /// <summary>
    /// Headers to exclude from logging
    /// </summary>
    public string[] ExcludedHeaders { get; set; } =
    {
        "Authorization",
        "Cookie",
        "Set-Cookie",
        "X-API-Key"
    };
}