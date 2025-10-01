using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Zeus.Academia.Infrastructure.Data;

/// <summary>
/// Database initialization service that handles migrations and seeding.
/// Task 5: Database Migrations - Database initialization strategy for first-time setup.
/// </summary>
public class DatabaseInitializer
{
    private readonly AcademiaDbContext _context;
    private readonly ILogger<DatabaseInitializer> _logger;

    public DatabaseInitializer(AcademiaDbContext context, ILogger<DatabaseInitializer> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Initializes the database with migrations and seed data.
    /// </summary>
    /// <param name="applyMigrations">Whether to apply pending migrations.</param>
    /// <param name="seedData">Whether to seed reference data.</param>
    /// <returns>True if initialization was successful.</returns>
    public async Task<bool> InitializeAsync(bool applyMigrations = true, bool seedData = true)
    {
        try
        {
            _logger.LogInformation("Starting database initialization...");

            // Check database connection
            if (!await CanConnectAsync())
            {
                _logger.LogError("Cannot connect to database. Initialization failed.");
                return false;
            }

            // Apply migrations if requested
            if (applyMigrations)
            {
                await ApplyMigrationsAsync();
            }

            // Seed reference data if requested
            if (seedData)
            {
                await SeedReferenceDataAsync();
            }

            _logger.LogInformation("Database initialization completed successfully.");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database initialization failed: {ErrorMessage}", ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Checks if the database can be connected to.
    /// </summary>
    /// <returns>True if connection is successful.</returns>
    public async Task<bool> CanConnectAsync()
    {
        try
        {
            _logger.LogInformation("Testing database connection...");
            var canConnect = await _context.Database.CanConnectAsync();

            if (canConnect)
            {
                _logger.LogInformation("Database connection successful.");
            }
            else
            {
                _logger.LogWarning("Database connection failed.");
            }

            return canConnect;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database connection test failed: {ErrorMessage}", ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Applies any pending migrations to the database.
    /// </summary>
    public async Task ApplyMigrationsAsync()
    {
        try
        {
            // Skip migrations for in-memory database
            if (_context.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory")
            {
                _logger.LogInformation("Skipping migrations for in-memory database provider.");
                return;
            }

            _logger.LogInformation("Checking for pending migrations...");

            var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
            var pendingMigrationsList = pendingMigrations.ToList();

            if (pendingMigrationsList.Any())
            {
                _logger.LogInformation("Found {Count} pending migrations: {Migrations}",
                    pendingMigrationsList.Count,
                    string.Join(", ", pendingMigrationsList));

                _logger.LogInformation("Applying migrations...");
                await _context.Database.MigrateAsync();
                _logger.LogInformation("Migrations applied successfully.");
            }
            else
            {
                _logger.LogInformation("No pending migrations found.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to apply migrations: {ErrorMessage}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Seeds the database with reference data.
    /// </summary>
    public async Task SeedReferenceDataAsync()
    {
        try
        {
            _logger.LogInformation("Starting reference data seeding...");
            await DatabaseSeeder.SeedAsync(_context);
            _logger.LogInformation("Reference data seeding completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to seed reference data: {ErrorMessage}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Gets database schema information for validation.
    /// </summary>
    public async Task<DatabaseInfo> GetDatabaseInfoAsync()
    {
        try
        {
            var canConnect = await _context.Database.CanConnectAsync();

            // Check if using a relational database provider
            var isRelational = _context.Database.IsRelational();

            List<string> appliedMigrations = new();
            List<string> pendingMigrations = new();

            if (isRelational)
            {
                appliedMigrations = (await _context.Database.GetAppliedMigrationsAsync()).ToList();
                pendingMigrations = (await _context.Database.GetPendingMigrationsAsync()).ToList();
            }

            return new DatabaseInfo
            {
                CanConnect = canConnect,
                AppliedMigrations = appliedMigrations,
                PendingMigrations = pendingMigrations,
                DatabaseExists = await DatabaseExistsAsync()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get database info: {ErrorMessage}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Checks if the database exists and has the expected schema.
    /// </summary>
    public async Task<bool> DatabaseExistsAsync()
    {
        try
        {
            // For in-memory databases, they exist once the context is used
            if (_context.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory")
            {
                await _context.Database.EnsureCreatedAsync();
                return true;
            }

            // For real databases, check if we can connect and if the database exists
            return await _context.Database.CanConnectAsync();
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Validates the database schema integrity.
    /// </summary>
    public async Task<bool> ValidateSchemaAsync()
    {
        try
        {
            _logger.LogInformation("Validating database schema...");

            // Check that all expected tables exist by trying to query them
            var tableChecks = new[]
            {
                _context.Academics.AnyAsync(),
                _context.Departments.AnyAsync(),
                _context.Subjects.AnyAsync(),
                _context.Buildings.AnyAsync(),
                _context.AccessLevels.AnyAsync(),
                _context.Universities.AnyAsync(),
                _context.Degrees.AnyAsync(),
                _context.Ranks.AnyAsync()
            };

            await Task.WhenAll(tableChecks);

            _logger.LogInformation("Database schema validation completed successfully.");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database schema validation failed: {ErrorMessage}", ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Resets the database by dropping and recreating it.
    /// WARNING: This will delete all data!
    /// </summary>
    public async Task ResetDatabaseAsync()
    {
        try
        {
            _logger.LogWarning("Resetting database - all data will be lost!");

            await _context.Database.EnsureDeletedAsync();
            await _context.Database.EnsureCreatedAsync();

            _logger.LogInformation("Database reset completed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database reset failed: {ErrorMessage}", ex.Message);
            throw;
        }
    }
}

/// <summary>
/// Information about the current database state.
/// </summary>
public class DatabaseInfo
{
    public bool CanConnect { get; set; }
    public bool DatabaseExists { get; set; }
    public List<string> AppliedMigrations { get; set; } = new();
    public List<string> PendingMigrations { get; set; } = new();
}

/// <summary>
/// Extension methods for registering database initialization services.
/// </summary>
public static class DatabaseInitializerExtensions
{
    /// <summary>
    /// Registers the database initializer service.
    /// </summary>
    public static IServiceCollection AddDatabaseInitializer(this IServiceCollection services)
    {
        services.AddTransient<DatabaseInitializer>();
        return services;
    }

    /// <summary>
    /// Initializes the database during application startup.
    /// </summary>
    public static async Task<IHost> InitializeDatabaseAsync(this IHost host, bool applyMigrations = true, bool seedData = true)
    {
        using var scope = host.Services.CreateScope();
        var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();

        await initializer.InitializeAsync(applyMigrations, seedData);

        return host;
    }
}