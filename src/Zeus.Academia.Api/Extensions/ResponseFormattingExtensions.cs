namespace Zeus.Academia.Api.Extensions;

/// <summary>
/// Extension methods for configuring response formatting and CORS
/// </summary>
public static class ResponseFormattingExtensions
{
    /// <summary>
    /// Configures response compression middleware
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddCustomResponseCompression(this IServiceCollection services)
    {
        services.AddResponseCompression(options =>
        {
            // Enable compression for both HTTP and HTTPS
            options.EnableForHttps = true;

            // Configure MIME types to compress
            options.MimeTypes = new[]
            {
                // Default types
                "text/plain",
                "text/html",
                "text/css",
                "text/javascript",
                "application/javascript",
                "application/json",
                "application/xml",
                "text/xml",
                
                // API specific types
                "application/hal+json",
                "application/ld+json",
                "application/vnd.api+json",
                
                // Font types
                "font/woff",
                "font/woff2",
                "application/font-woff",
                "application/font-woff2",
                
                // Image types (SVG only - other images are already compressed)
                "image/svg+xml"
            };

            // Configure compression providers
            options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProvider>();
            options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProvider>();
        });

        // Configure Brotli compression (highest compression, modern browsers)
        services.Configure<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProviderOptions>(options =>
        {
            options.Level = System.IO.Compression.CompressionLevel.Optimal;
        });

        // Configure Gzip compression (fallback for older browsers)
        services.Configure<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProviderOptions>(options =>
        {
            options.Level = System.IO.Compression.CompressionLevel.Optimal;
        });

        return services;
    }

    /// <summary>
    /// Configures content negotiation for multiple formats (JSON/XML)
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddContentNegotiation(this IServiceCollection services)
    {
        services.Configure<Microsoft.AspNetCore.Mvc.MvcOptions>(options =>
        {
            // Add XML formatters for content negotiation
            options.OutputFormatters.Add(new Microsoft.AspNetCore.Mvc.Formatters.XmlDataContractSerializerOutputFormatter());
            options.InputFormatters.Add(new Microsoft.AspNetCore.Mvc.Formatters.XmlDataContractSerializerInputFormatter(options));

            // Configure format mappings for query string format parameter
            options.FormatterMappings.SetMediaTypeMappingForFormat("json", "application/json");
            options.FormatterMappings.SetMediaTypeMappingForFormat("xml", "application/xml");

            // Respect Accept header for content negotiation
            options.RespectBrowserAcceptHeader = true;
            options.ReturnHttpNotAcceptable = true; // Return 406 if requested format not supported
        });

        // Configure JSON options
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            options.SerializerOptions.WriteIndented = false; // Compact JSON for better compression
            options.SerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        });

        return services;
    }

    /// <summary>
    /// Configures comprehensive CORS policies for development and production
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration</param>
    /// <param name="environment">Web host environment</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddComprehensiveCors(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.AddCors(options =>
        {
            // Development policy - permissive for testing
            options.AddPolicy("DevelopmentCorsPolicy", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .WithExposedHeaders("X-Pagination", "X-Total-Count", "X-Page-Size", "X-Current-Page");
            });

            // Production policy - restrictive for security
            options.AddPolicy("ProductionCorsPolicy", policy =>
            {
                var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                    ?? new[] { "https://zeus-academia.com", "https://app.zeus-academia.com" };

                var allowedMethods = configuration.GetSection("Cors:AllowedMethods").Get<string[]>()
                    ?? new[] { "GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS" };

                var allowedHeaders = configuration.GetSection("Cors:AllowedHeaders").Get<string[]>()
                    ?? new[] { "Content-Type", "Authorization", "X-Requested-With", "Accept", "Origin", "X-Api-Version" };

                var exposedHeaders = configuration.GetSection("Cors:ExposedHeaders").Get<string[]>()
                    ?? new[] { "X-Pagination", "X-Total-Count", "X-Page-Size", "X-Current-Page", "X-Api-Version" };

                policy.WithOrigins(allowedOrigins)
                      .WithMethods(allowedMethods)
                      .WithHeaders(allowedHeaders)
                      .WithExposedHeaders(exposedHeaders)
                      .AllowCredentials()
                      .SetPreflightMaxAge(TimeSpan.FromHours(1)); // Cache preflight for 1 hour
            });

            // Default policy that switches based on environment
            options.AddPolicy("DefaultCorsPolicy", policy =>
            {
                if (environment.IsDevelopment())
                {
                    // Development: Use permissive policy
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .WithExposedHeaders("X-Pagination", "X-Total-Count", "X-Page-Size", "X-Current-Page", "X-Api-Version");
                }
                else
                {
                    // Production: Use restrictive policy
                    var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                        ?? new[] { "https://zeus-academia.com", "https://app.zeus-academia.com" };

                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .WithExposedHeaders("X-Pagination", "X-Total-Count", "X-Page-Size", "X-Current-Page", "X-Api-Version")
                          .AllowCredentials()
                          .SetPreflightMaxAge(TimeSpan.FromHours(1));
                }
            });
        });

        return services;
    }
}