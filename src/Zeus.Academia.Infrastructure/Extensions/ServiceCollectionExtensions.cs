using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Identity;
using Zeus.Academia.Infrastructure.Services;
using Zeus.Academia.Infrastructure.Repositories;
using Zeus.Academia.Infrastructure.Repositories.Interfaces;

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

        // Identity configuration will be handled in the API project

        // Repository pattern services will be registered here in Task 6

        return services;
    }

    /// <summary>
    /// Adds additional Infrastructure services to the dependency injection container
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration instance</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddInfrastructureIdentityServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Get Identity options from configuration for custom services
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

        // Register role-based authorization services (Task 2)
        services.AddScoped<IRoleHierarchyService, RoleHierarchyService>();
        services.AddScoped<IRoleAssignmentService, RoleAssignmentService>();

        // Register JWT authentication services (Task 3)
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IPasswordService, PasswordService>();

        // Register User Management services (Task 4)
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ICourseService, CourseService>();

        // Register Identity Repositories (Task 5)
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        // Register Security and Audit services (Task 6)
        services.AddScoped<IAuditService, AuditService>();

        return services;
    }
}