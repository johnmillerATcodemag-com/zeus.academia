using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Zeus.Academia.Infrastructure.Data;

/// <summary>
/// Entity Framework DbContext for the Zeus Academia System
/// </summary>
public class AcademiaDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    public AcademiaDbContext(DbContextOptions<AcademiaDbContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    // DbSets will be added as entities are created in subsequent tasks
    // Example: public DbSet<Academic> Academics { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (!string.IsNullOrEmpty(connectionString))
            {
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Entity configurations will be added here as entities are created
        // Apply all entity configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AcademiaDbContext).Assembly);
    }
}