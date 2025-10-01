using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Identity;

namespace Zeus.Academia.Infrastructure.Extensions;

/// <summary>
/// Extension methods for configuring Entity Framework services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Entity Framework services to the dependency injection container
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration instance</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register configuration if not already registered
        if (!services.Any(d => d.ServiceType == typeof(IConfiguration)))
        {
            services.AddSingleton(configuration);
        }

        // Validate connection string early
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                "Database connection string 'DefaultConnection' is not configured. " +
                "Please check your appsettings.json file.");
        }

        // Register Entity Framework DbContext
        services.AddDbContext<AcademiaDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                // Configure SQL Server specific options
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);

                // Set command timeout
                sqlOptions.CommandTimeout(30);
            });

            // Enable sensitive data logging in development
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (environment == "Development")
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }
        });

        // Configure Identity services
        ConfigureIdentityServices(services, configuration);

        // Repository pattern services will be registered here in Task 6

        return services;
    }

    /// <summary>
    /// Configures Identity services for the Zeus Academia System
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration instance</param>
    private static void ConfigureIdentityServices(
        IServiceCollection services,
        IConfiguration configuration)
    {
        // Get Identity options from configuration
        var identityOptions = configuration.GetSection("Identity");
        var passwordOptions = identityOptions.GetSection("Password");
        var lockoutOptions = identityOptions.GetSection("Lockout");
        var userOptions = identityOptions.GetSection("User");

        // Configure password requirements from configuration or use defaults
        var passwordRequirements = new PasswordRequirements
        {
            RequiredLength = passwordOptions.GetValue<int>("RequiredLength", 8),
            RequireDigit = passwordOptions.GetValue<bool>("RequireDigit", true),
            RequireUppercase = passwordOptions.GetValue<bool>("RequireUppercase", true),
            RequireLowercase = passwordOptions.GetValue<bool>("RequireLowercase", true),
            RequireNonAlphanumeric = passwordOptions.GetValue<bool>("RequireNonAlphanumeric", true),
            RequiredUniqueChars = passwordOptions.GetValue<int>("RequiredUniqueChars", 6)
        };

        var lockoutSettings = new LockoutSettings
        {
            DefaultLockoutTimeSpan = TimeSpan.FromMinutes(lockoutOptions.GetValue<int>("DefaultLockoutMinutes", 15)),
            MaxFailedAccessAttempts = lockoutOptions.GetValue<int>("MaxFailedAccessAttempts", 5),
            AllowedForNewUsers = lockoutOptions.GetValue<bool>("AllowedForNewUsers", true)
        };

        var userSettings = new UserSettings
        {
            AllowedUserNameCharacters = userOptions.GetValue<string>("AllowedUserNameCharacters",
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+"),
            RequireUniqueEmail = userOptions.GetValue<bool>("RequireUniqueEmail", true)
        };

        // Register configuration objects
        services.AddSingleton(passwordRequirements);
        services.AddSingleton(lockoutSettings);
        services.AddSingleton(userSettings);

        // Note: Authentication and Authorization services will be configured in the API project
        // Custom Identity services (UserService, PasswordHasher, etc.) will be added in Task 4
    }
}