using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using Zeus.Academia.Api.Configuration;
using Zeus.Academia.Api.Services;

namespace Zeus.Academia.Api.Extensions;

/// <summary>
/// Extension methods for configuring application services
/// </summary>
public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// Add and configure application settings
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration instance</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddApplicationConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure and validate API settings
        services.Configure<ApiConfiguration>(configuration.GetSection(ApiConfiguration.SectionName));
        services.AddSingleton<IValidateOptions<ApiConfiguration>, ValidateOptionsConfiguration<ApiConfiguration>>();

        // Configure and validate Database settings
        services.Configure<DatabaseConfiguration>(configuration.GetSection(DatabaseConfiguration.SectionName));
        services.AddSingleton<IValidateOptions<DatabaseConfiguration>, ValidateOptionsConfiguration<DatabaseConfiguration>>();

        // Configure and validate Logging settings
        services.Configure<LoggingConfiguration>(configuration.GetSection(LoggingConfiguration.SectionName));
        services.AddSingleton<IValidateOptions<LoggingConfiguration>, ValidateOptionsConfiguration<LoggingConfiguration>>();

        return services;
    }

    /// <summary>
    /// Add API-specific services
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        // Add HTTP context accessor
        services.AddHttpContextAccessor();

        // Add memory cache
        services.AddMemoryCache();

        // Add correlation ID service for request tracking
        services.AddSingleton<ICorrelationIdService, CorrelationIdService>();

        // Add API versioning services
        services.AddApiVersioningServices();

        // Add validation services
        services.AddValidationServices();

        // Add health checks
        services.AddHealthChecks();

        // Configure controllers
        services.AddControllers(options =>
        {
            // Add custom filters here if needed
        })
        .ConfigureApiBehaviorOptions(options =>
        {
            // Customize automatic model validation response
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? new string[0]
                    );

                var response = new
                {
                    Message = "Validation failed",
                    Errors = errors,
                    Timestamp = DateTime.UtcNow,
                    TraceId = System.Diagnostics.Activity.Current?.Id ?? context.HttpContext.TraceIdentifier
                };

                return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(response);
            };
        });

        return services;
    }

    /// <summary>
    /// Configure environment-specific settings
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="environment">Web host environment</param>
    /// <param name="configuration">Configuration instance</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection ConfigureEnvironmentSettings(
        this IServiceCollection services,
        IWebHostEnvironment environment,
        IConfiguration configuration)
    {
        if (environment.IsDevelopment())
        {
            // Development-specific configurations
            services.Configure<ApiConfiguration>(config =>
            {
                config.ShowDetailedErrors = true;
            });

            services.Configure<DatabaseConfiguration>(config =>
            {
                config.EnableSensitiveDataLogging = true;
                config.EnableDetailedErrors = true;
            });

            services.Configure<LoggingConfiguration>(config =>
            {
                config.SensitiveData.LogRequestBodies = true;
                config.SensitiveData.LogResponseBodies = true;
                config.SensitiveData.LogHeaders = true;
            });
        }
        else if (environment.IsStaging())
        {
            // Staging-specific configurations
            services.Configure<LoggingConfiguration>(config =>
            {
                config.EnablePerformanceLogging = true;
                config.EnableSecurityLogging = true;
            });
        }
        else if (environment.IsProduction())
        {
            // Production-specific configurations
            services.Configure<ApiConfiguration>(config =>
            {
                config.ShowDetailedErrors = false;
            });

            services.Configure<DatabaseConfiguration>(config =>
            {
                config.EnableSensitiveDataLogging = false;
                config.EnableDetailedErrors = false;
            });

            services.Configure<LoggingConfiguration>(config =>
            {
                config.SensitiveData.LogRequestBodies = false;
                config.SensitiveData.LogResponseBodies = false;
                config.SensitiveData.LogHeaders = false;
            });
        }

        return services;
    }
}

/// <summary>
/// Generic options validator using data annotations
/// </summary>
/// <typeparam name="T">Options type to validate</typeparam>
public class ValidateOptionsConfiguration<T> : IValidateOptions<T> where T : class
{
    public ValidateOptionsResult Validate(string? name, T options)
    {
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(options);

        if (!Validator.TryValidateObject(options, context, validationResults, true))
        {
            var failures = validationResults
                .Select(r => $"{string.Join(",", r.MemberNames)}: {r.ErrorMessage}")
                .ToList();

            return ValidateOptionsResult.Fail(failures);
        }

        return ValidateOptionsResult.Success;
    }
}

/// <summary>
/// Extension methods for configuring validation services
/// </summary>
public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds validation services and validators to the service collection.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddValidationServices(this IServiceCollection services)
    {
        // Add the validation service
        services.AddScoped<Zeus.Academia.Api.Validation.IValidationService, Zeus.Academia.Api.Validation.ValidationService>();

        // Register all validators
        services.AddValidators();

        return services;
    }

    /// <summary>
    /// Registers all validators in the application.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    private static IServiceCollection AddValidators(this IServiceCollection services)
    {
        // Register professor validators
        services.AddScoped<Zeus.Academia.Api.Validation.IValidator<Zeus.Academia.Api.Models.Requests.CreateProfessorRequest>,
            Zeus.Academia.Api.Validation.Validators.CreateProfessorRequestValidator>();
        services.AddScoped<Zeus.Academia.Api.Validation.IValidator<Zeus.Academia.Api.Models.Requests.UpdateProfessorRequest>,
            Zeus.Academia.Api.Validation.Validators.UpdateProfessorRequestValidator>();

        // Register student validators
        services.AddScoped<Zeus.Academia.Api.Validation.IValidator<Zeus.Academia.Api.Models.Requests.CreateStudentRequest>,
            Zeus.Academia.Api.Validation.Validators.CreateStudentRequestValidator>();
        services.AddScoped<Zeus.Academia.Api.Validation.IValidator<Zeus.Academia.Api.Models.Requests.UpdateStudentRequest>,
            Zeus.Academia.Api.Validation.Validators.UpdateStudentRequestValidator>();

        return services;
    }

    /// <summary>
    /// Adds API versioning services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddApiVersioningServices(this IServiceCollection services)
    {
        // Register the API version service
        services.AddSingleton<Zeus.Academia.Api.Versioning.IApiVersionService, Zeus.Academia.Api.Versioning.ApiVersionService>();

        return services;
    }
}