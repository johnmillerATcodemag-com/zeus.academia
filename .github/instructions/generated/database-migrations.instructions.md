# Database Migrations

## Purpose

This document establishes comprehensive database migration strategies for the Academic Management System, defining Entity Framework Core migration practices, forward-only migration principles, seed data management, production deployment procedures, rollback strategies, and data validation processes to ensure consistent and reliable database schema evolution.

## Scope

This document covers:

- Entity Framework Core migration workflows and best practices
- Forward-only migration strategy implementation
- Seed data management and environment-specific data handling
- Production deployment procedures with zero-downtime strategies
- Rollback and recovery procedures for failed migrations
- Data validation and integrity testing post-migration

This document does not cover:

- Database performance tuning and optimization (covered in monitoring-observability.instructions.md)
- Database security and encryption (covered in security-compliance.instructions.md)
- Backup and disaster recovery procedures (covered in deployment-operations.instructions.md)
- Database infrastructure provisioning (covered in azure-infrastructure.instructions.md)

## Prerequisites

- Understanding of Entity Framework Core and Code First approach
- Knowledge of SQL Server database administration
- Familiarity with Azure SQL Database features and limitations
- Understanding of blue-green deployment strategies
- Knowledge of CI/CD pipeline integration

## Entity Framework Core Migration Strategy

### Migration Development Workflow

```csharp
// 1. Make changes to entity models in the Domain layer
public class Student : AggregateRoot<StudentId>
{
    // Add new property
    public DateTimeOffset? LastLoginAt { get; private set; }

    // Add method to update last login
    public void RecordLogin(DateTimeOffset loginTime)
    {
        LastLoginAt = loginTime;
        RaiseDomainEvent(new StudentLoginRecordedDomainEvent(Id, loginTime));
    }
}

// 2. Update DbContext configuration
public class AcademiaDbContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>(entity =>
        {
            // Configure new column with default value for existing records
            entity.Property(e => e.LastLoginAt)
                .HasColumnName("LastLoginAt")
                .IsRequired(false)
                .HasDefaultValue(null);
        });

        base.OnModelCreating(modelBuilder);
    }
}

// 3. Generate migration using EF CLI tools
// dotnet ef migrations add AddLastLoginToStudent --project src/Academia.Infrastructure --startup-project src/Academia.API
```

### Migration Generation Commands

```powershell
# Generate new migration
dotnet ef migrations add [MigrationName] --project src/Academia.Infrastructure --startup-project src/Academia.API --configuration Release

# Review generated migration before applying
dotnet ef migrations script [FromMigration] [ToMigration] --project src/Academia.Infrastructure --startup-project src/Academia.API

# Apply migrations to local database
dotnet ef database update --project src/Academia.Infrastructure --startup-project src/Academia.API

# Generate SQL script for production deployment
dotnet ef migrations script --project src/Academia.Infrastructure --startup-project src/Academia.API --output scripts/deploy-migrations.sql --idempotent
```

### Migration Naming Conventions

```csharp
// Follow descriptive naming pattern: [Verb][Entity][Property/Action]
// Examples:
20240115120000_AddLastLoginToStudent.cs
20240115120100_CreateCoursePrerequisiteTable.cs
20240115120200_UpdateEnrollmentStatusEnum.cs
20240115120300_RemoveObsoleteStudentFields.cs
20240115120400_AddIndexToStudentEmail.cs
20240115120500_SeedInitialAdministratorRoles.cs

// Migration class structure
[DbContext(typeof(AcademiaDbContext))]
[Migration("20240115120000_AddLastLoginToStudent")]
partial class AddLastLoginToStudent
{
    protected override void BuildTargetModel(ModelBuilder modelBuilder)
    {
        // Auto-generated model snapshot
    }
}

// Up and Down methods in migration
public partial class AddLastLoginToStudent : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "LastLoginAt",
            table: "Students",
            type: "datetimeoffset",
            nullable: true,
            defaultValue: null);

        // Add index for performance
        migrationBuilder.CreateIndex(
            name: "IX_Students_LastLoginAt",
            table: "Students",
            column: "LastLoginAt");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Students_LastLoginAt",
            table: "Students");

        migrationBuilder.DropColumn(
            name: "LastLoginAt",
            table: "Students");
    }
}
```

## Forward-Only Migration Strategy

### Core Principles

```csharp
// 1. Never modify existing migrations after they've been deployed
// 2. Always create new migrations for changes
// 3. Use additive changes when possible
// 4. Plan for backwards compatibility

// Example: Renaming a column (forward-only approach)

// Step 1: Add new column with data migration
public partial class AddNewStudentEmailColumn : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Add new column
        migrationBuilder.AddColumn<string>(
            name: "EmailAddress",
            table: "Students",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);

        // Copy data from old column
        migrationBuilder.Sql(@"
            UPDATE Students
            SET EmailAddress = Email
            WHERE Email IS NOT NULL");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "EmailAddress",
            table: "Students");
    }
}

// Step 2: Update application code to use new column
// Deploy application changes

// Step 3: Remove old column in subsequent deployment
public partial class RemoveOldStudentEmailColumn : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Email",
            table: "Students");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Email",
            table: "Students",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);

        // Copy data back for rollback
        migrationBuilder.Sql(@"
            UPDATE Students
            SET Email = EmailAddress
            WHERE EmailAddress IS NOT NULL");
    }
}
```

### Breaking Change Handling

```csharp
// Safe approach for potentially breaking changes
public class SafeMigrationHelper
{
    /// <summary>
    /// Safely adds a non-nullable column by first adding it as nullable,
    /// populating data, then changing to non-nullable.
    /// </summary>
    public static void AddRequiredColumn(
        MigrationBuilder migrationBuilder,
        string table,
        string column,
        string type,
        string defaultValue)
    {
        // Step 1: Add as nullable
        migrationBuilder.AddColumn<string>(
            name: column,
            table: table,
            type: type,
            nullable: true);

        // Step 2: Populate with default value
        migrationBuilder.Sql($@"
            UPDATE {table}
            SET {column} = '{defaultValue}'
            WHERE {column} IS NULL");

        // Step 3: Make non-nullable
        migrationBuilder.AlterColumn<string>(
            name: column,
            table: table,
            type: type,
            nullable: false,
            oldNullable: true);
    }

    /// <summary>
    /// Safely removes a column by first renaming it, then removing in a later migration.
    /// </summary>
    public static void SafelyRemoveColumn(
        MigrationBuilder migrationBuilder,
        string table,
        string column)
    {
        // Rename column to mark for deletion
        migrationBuilder.RenameColumn(
            name: column,
            table: table,
            newName: $"{column}_ToDelete_{DateTime.UtcNow:yyyyMMdd}");

        // Note: Actual deletion should happen in a subsequent migration
        // after confirming no applications are using the old column
    }
}
```

## Seed Data Management

### Development Environment Seed Data

```csharp
// Seed data configuration for development and testing
public static class SeedDataConfiguration
{
    public static void SeedDevelopmentData(this ModelBuilder modelBuilder)
    {
        // Seed departments
        var departments = new[]
        {
            new { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Computer Science", Code = "CS", CreatedAt = DateTime.UtcNow },
            new { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Mathematics", Code = "MATH", CreatedAt = DateTime.UtcNow },
            new { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Physics", Code = "PHYS", CreatedAt = DateTime.UtcNow }
        };

        modelBuilder.Entity("Department").HasData(departments);

        // Seed courses with relationships
        var courses = new[]
        {
            new
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Code = "CS101",
                Title = "Introduction to Computer Science",
                Credits = 3,
                DepartmentId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                CreatedAt = DateTime.UtcNow
            },
            new
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                Code = "CS201",
                Title = "Data Structures and Algorithms",
                Credits = 4,
                DepartmentId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                CreatedAt = DateTime.UtcNow
            }
        };

        modelBuilder.Entity("Course").HasData(courses);
    }

    public static void SeedReferenceData(this ModelBuilder modelBuilder)
    {
        // Seed static reference data that should exist in all environments
        var academicTerms = new[]
        {
            new { Id = 1, Code = "FALL2024", Name = "Fall 2024", StartDate = new DateTime(2024, 8, 15), EndDate = new DateTime(2024, 12, 15) },
            new { Id = 2, Code = "SPRING2025", Name = "Spring 2025", StartDate = new DateTime(2025, 1, 15), EndDate = new DateTime(2025, 5, 15) },
            new { Id = 3, Code = "SUMMER2025", Name = "Summer 2025", StartDate = new DateTime(2025, 6, 1), EndDate = new DateTime(2025, 8, 1) }
        };

        modelBuilder.Entity("AcademicTerm").HasData(academicTerms);

        // Seed grade scales
        var gradeScales = new[]
        {
            new { Id = 1, Grade = "A", GradePoints = 4.0m, MinPercentage = 90, MaxPercentage = 100 },
            new { Id = 2, Grade = "A-", GradePoints = 3.7m, MinPercentage = 87, MaxPercentage = 89 },
            new { Id = 3, Grade = "B+", GradePoints = 3.3m, MinPercentage = 83, MaxPercentage = 86 },
            new { Id = 4, Grade = "B", GradePoints = 3.0m, MinPercentage = 80, MaxPercentage = 82 },
            new { Id = 5, Grade = "B-", GradePoints = 2.7m, MinPercentage = 77, MaxPercentage = 79 },
            new { Id = 6, Grade = "C+", GradePoints = 2.3m, MinPercentage = 73, MaxPercentage = 76 },
            new { Id = 7, Grade = "C", GradePoints = 2.0m, MinPercentage = 70, MaxPercentage = 72 },
            new { Id = 8, Grade = "C-", GradePoints = 1.7m, MinPercentage = 67, MaxPercentage = 69 },
            new { Id = 9, Grade = "D+", GradePoints = 1.3m, MinPercentage = 63, MaxPercentage = 66 },
            new { Id = 10, Grade = "D", GradePoints = 1.0m, MinPercentage = 60, MaxPercentage = 62 },
            new { Id = 11, Grade = "F", GradePoints = 0.0m, MinPercentage = 0, MaxPercentage = 59 }
        };

        modelBuilder.Entity("GradeScale").HasData(gradeScales);
    }
}

// Migration for seed data
public partial class SeedReferenceData : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Insert academic terms
        migrationBuilder.InsertData(
            table: "AcademicTerms",
            columns: new[] { "Id", "Code", "Name", "StartDate", "EndDate", "CreatedAt" },
            values: new object[,]
            {
                { 1, "FALL2024", "Fall 2024", new DateTime(2024, 8, 15), new DateTime(2024, 12, 15), DateTime.UtcNow },
                { 2, "SPRING2025", "Spring 2025", new DateTime(2025, 1, 15), new DateTime(2025, 5, 15), DateTime.UtcNow },
                { 3, "SUMMER2025", "Summer 2025", new DateTime(2025, 6, 1), new DateTime(2025, 8, 1), DateTime.UtcNow }
            });

        // Create stored procedure for data validation
        migrationBuilder.Sql(@"
            CREATE OR ALTER PROCEDURE ValidateReferenceData
            AS
            BEGIN
                -- Validate academic terms
                IF NOT EXISTS (SELECT 1 FROM AcademicTerms WHERE Code = 'FALL2024')
                    THROW 50001, 'Missing required academic term: FALL2024', 1;

                -- Validate grade scales
                IF NOT EXISTS (SELECT 1 FROM GradeScales WHERE Grade = 'A' AND GradePoints = 4.0)
                    THROW 50002, 'Missing required grade scale: A (4.0)', 1;

                PRINT 'Reference data validation passed';
            END");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP PROCEDURE IF EXISTS ValidateReferenceData");

        migrationBuilder.DeleteData(
            table: "GradeScales",
            keyColumn: "Id",
            keyValues: new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 });

        migrationBuilder.DeleteData(
            table: "AcademicTerms",
            keyColumn: "Id",
            keyValues: new object[] { 1, 2, 3 });
    }
}
```

### Environment-Specific Data Management

```csharp
// Environment-specific data seeding service
public interface IDataSeedingService
{
    Task SeedEnvironmentDataAsync(string environment, CancellationToken cancellationToken = default);
    Task ValidateDataIntegrityAsync(CancellationToken cancellationToken = default);
}

public class DataSeedingService : IDataSeedingService
{
    private readonly AcademiaDbContext _context;
    private readonly ILogger<DataSeedingService> _logger;
    private readonly IConfiguration _configuration;

    public DataSeedingService(
        AcademiaDbContext context,
        ILogger<DataSeedingService> logger,
        IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task SeedEnvironmentDataAsync(string environment, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting data seeding for environment: {Environment}", environment);

        try
        {
            switch (environment.ToLowerInvariant())
            {
                case "development":
                    await SeedDevelopmentDataAsync(cancellationToken);
                    break;
                case "staging":
                    await SeedStagingDataAsync(cancellationToken);
                    break;
                case "production":
                    await SeedProductionDataAsync(cancellationToken);
                    break;
                default:
                    _logger.LogWarning("Unknown environment: {Environment}. Skipping environment-specific seeding.", environment);
                    break;
            }

            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Data seeding completed successfully for environment: {Environment}", environment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Data seeding failed for environment: {Environment}", environment);
            throw;
        }
    }

    private async Task SeedDevelopmentDataAsync(CancellationToken cancellationToken)
    {
        // Seed test students
        if (!await _context.Students.AnyAsync(cancellationToken))
        {
            var testStudents = new[]
            {
                Student.Create(
                    StudentId.New(),
                    PersonalInfo.Create("John", "Doe", new DateTime(2000, 1, 1)),
                    ContactInfo.Create("john.doe@student.university.edu", "555-0101"),
                    ProgramId.New()),
                Student.Create(
                    StudentId.New(),
                    PersonalInfo.Create("Jane", "Smith", new DateTime(1999, 6, 15)),
                    ContactInfo.Create("jane.smith@student.university.edu", "555-0102"),
                    ProgramId.New())
            };

            _context.Students.AddRange(testStudents);
        }
    }

    private async Task SeedStagingDataAsync(CancellationToken cancellationToken)
    {
        // Seed realistic test data for staging environment
        await SeedMinimalProductionLikeDataAsync(cancellationToken);
    }

    private async Task SeedProductionDataAsync(CancellationToken cancellationToken)
    {
        // Only seed essential reference data for production
        await ValidateReferenceDataExistsAsync(cancellationToken);
    }

    public async Task ValidateDataIntegrityAsync(CancellationToken cancellationToken = default)
    {
        var validationErrors = new List<string>();

        // Validate required reference data exists
        if (!await _context.AcademicTerms.AnyAsync(cancellationToken))
            validationErrors.Add("No academic terms found");

        if (!await _context.GradeScales.AnyAsync(cancellationToken))
            validationErrors.Add("No grade scales found");

        // Validate data consistency
        var orphanedEnrollments = await _context.Enrollments
            .Where(e => !_context.Students.Any(s => s.Id == e.StudentId) ||
                       !_context.Courses.Any(c => c.Id == e.CourseId))
            .CountAsync(cancellationToken);

        if (orphanedEnrollments > 0)
            validationErrors.Add($"Found {orphanedEnrollments} orphaned enrollments");

        if (validationErrors.Any())
        {
            var errorMessage = "Data integrity validation failed:\n" + string.Join("\n", validationErrors);
            _logger.LogError("Data integrity validation failed: {Errors}", errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        _logger.LogInformation("Data integrity validation passed");
    }
}
```

## Production Deployment Procedures

### Zero-Downtime Migration Strategy

```csharp
// Blue-Green deployment migration approach
public class ProductionMigrationService
{
    private readonly string _connectionString;
    private readonly ILogger<ProductionMigrationService> _logger;

    public async Task<bool> CanDeployMigrationsAsync()
    {
        try
        {
            // Check if database is accessible
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // Verify no blocking operations
            var blockingQueries = await CheckForBlockingQueriesAsync(connection);
            if (blockingQueries.Any())
            {
                _logger.LogWarning("Blocking queries detected: {BlockingQueries}", blockingQueries);
                return false;
            }

            // Check database load
            var currentLoad = await GetDatabaseLoadAsync(connection);
            if (currentLoad > 80) // 80% CPU threshold
            {
                _logger.LogWarning("Database load too high for migration: {CurrentLoad}%", currentLoad);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate deployment readiness");
            return false;
        }
    }

    public async Task<MigrationResult> DeployMigrationsAsync(bool dryRun = false)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new MigrationResult { StartTime = DateTime.UtcNow };

        try
        {
            // Pre-deployment validation
            await ValidatePreDeploymentConditionsAsync();

            // Generate migration script
            var migrationScript = await GenerateMigrationScriptAsync();
            result.MigrationScript = migrationScript;

            if (dryRun)
            {
                _logger.LogInformation("Dry run completed. Migration script generated but not executed.");
                result.Success = true;
                return result;
            }

            // Create database snapshot before migration (Azure SQL)
            var snapshotName = await CreateDatabaseSnapshotAsync();
            result.SnapshotName = snapshotName;

            // Execute migration with monitoring
            await ExecuteMigrationWithMonitoringAsync(migrationScript, result);

            // Post-deployment validation
            await ValidatePostDeploymentAsync();

            result.Success = true;
            _logger.LogInformation("Migration deployment completed successfully in {Duration}ms", stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Error = ex.Message;
            _logger.LogError(ex, "Migration deployment failed after {Duration}ms", stopwatch.ElapsedMilliseconds);

            // Attempt automatic rollback
            if (!string.IsNullOrEmpty(result.SnapshotName))
            {
                await AttemptAutomaticRollbackAsync(result.SnapshotName);
            }
        }
        finally
        {
            result.EndTime = DateTime.UtcNow;
            result.Duration = stopwatch.Elapsed;
        }

        return result;
    }

    private async Task ExecuteMigrationWithMonitoringAsync(string script, MigrationResult result)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var commands = SplitMigrationScript(script);
        var executedCommands = new List<string>();

        try
        {
            foreach (var command in commands)
            {
                var commandStopwatch = Stopwatch.StartNew();

                using var sqlCommand = new SqlCommand(command, connection);
                sqlCommand.CommandTimeout = 300; // 5 minutes timeout

                await sqlCommand.ExecuteNonQueryAsync();
                executedCommands.Add(command);

                _logger.LogDebug("Executed migration command in {Duration}ms", commandStopwatch.ElapsedMilliseconds);

                // Monitor database performance during execution
                var currentLoad = await GetDatabaseLoadAsync(connection);
                if (currentLoad > 95) // Emergency brake at 95% load
                {
                    _logger.LogWarning("Database load critical during migration: {Load}%. Pausing execution.", currentLoad);
                    await Task.Delay(5000); // Wait 5 seconds
                }
            }
        }
        catch (Exception ex)
        {
            result.ExecutedCommands = executedCommands;
            _logger.LogError(ex, "Migration failed after executing {CommandCount} commands", executedCommands.Count);
            throw;
        }
    }

    private async Task<string> CreateDatabaseSnapshotAsync()
    {
        // For Azure SQL Database, use point-in-time restore capability
        var snapshotName = $"migration-snapshot-{DateTime.UtcNow:yyyyMMddHHmmss}";

        // Log snapshot creation for operational procedures
        _logger.LogInformation("Database snapshot created: {SnapshotName}. Available for point-in-time restore.", snapshotName);

        return snapshotName;
    }

    private async Task ValidatePostDeploymentAsync()
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        // Execute validation stored procedure created in migrations
        using var command = new SqlCommand("EXEC ValidateReferenceData", connection);
        await command.ExecuteNonQueryAsync();

        // Validate specific business rules
        await ValidateBusinessRulesAsync(connection);

        _logger.LogInformation("Post-deployment validation completed successfully");
    }
}

public class MigrationResult
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public TimeSpan Duration { get; set; }
    public bool Success { get; set; }
    public string? Error { get; set; }
    public string? MigrationScript { get; set; }
    public string? SnapshotName { get; set; }
    public List<string> ExecutedCommands { get; set; } = new();
}
```

### CI/CD Pipeline Integration

```yaml
# Azure DevOps pipeline for database migrations
stages:
  - stage: DatabaseMigration
    displayName: "Database Migration"
    jobs:
      - job: ValidateMigrations
        displayName: "Validate Migrations"
        steps:
          - task: UseDotNet@2
            inputs:
              packageType: "sdk"
              version: "8.0.x"

          - script: |
              dotnet ef migrations list --project src/Academia.Infrastructure --startup-project src/Academia.API
              dotnet ef migrations script --project src/Academia.Infrastructure --startup-project src/Academia.API --output $(Build.ArtifactStagingDirectory)/migration-script.sql --idempotent
            displayName: "Generate Migration Script"

          - task: PublishBuildArtifacts@1
            inputs:
              PathtoPublish: "$(Build.ArtifactStagingDirectory)"
              ArtifactName: "migration-artifacts"
            displayName: "Publish Migration Artifacts"

      - job: DeployMigrations
        displayName: "Deploy Migrations"
        dependsOn: ValidateMigrations
        condition: and(succeeded(), eq(variables['Build.SourceBranch'], 'refs/heads/main'))
        steps:
          - task: DownloadBuildArtifacts@0
            inputs:
              artifactName: "migration-artifacts"
              downloadPath: "$(System.ArtifactsDirectory)"

          - task: SqlAzureDacpacDeployment@1
            inputs:
              azureSubscription: "Azure Production"
              ServerName: "$(SqlServer.Name)"
              DatabaseName: "$(SqlDatabase.Name)"
              deployType: "SqlTask"
              SqlFile: "$(System.ArtifactsDirectory)/migration-artifacts/migration-script.sql"
              IpDetectionMethod: "AutoDetect"
            displayName: "Deploy Database Migrations"

          - script: |
              dotnet run --project tools/MigrationValidator -- --connection-string "$(SqlDatabase.ConnectionString)" --validate-post-deployment
            displayName: "Validate Post-Deployment"
```

## Rollback and Recovery Procedures

### Automated Rollback Strategies

```csharp
public class MigrationRollbackService
{
    private readonly string _connectionString;
    private readonly ILogger<MigrationRollbackService> _logger;

    public async Task<RollbackResult> RollbackMigrationAsync(string targetMigration)
    {
        var result = new RollbackResult { StartTime = DateTime.UtcNow };

        try
        {
            // Validate rollback safety
            var safetyCheck = await ValidateRollbackSafetyAsync(targetMigration);
            if (!safetyCheck.IsSafe)
            {
                throw new InvalidOperationException($"Rollback is not safe: {safetyCheck.Reason}");
            }

            // Generate rollback script
            var rollbackScript = await GenerateRollbackScriptAsync(targetMigration);

            // Create backup before rollback
            var backupName = await CreatePreRollbackBackupAsync();
            result.BackupName = backupName;

            // Execute rollback
            await ExecuteRollbackScriptAsync(rollbackScript);

            // Validate rollback success
            await ValidateRollbackSuccessAsync(targetMigration);

            result.Success = true;
            _logger.LogInformation("Migration rollback completed successfully to {TargetMigration}", targetMigration);
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Error = ex.Message;
            _logger.LogError(ex, "Migration rollback failed for target {TargetMigration}", targetMigration);
        }
        finally
        {
            result.EndTime = DateTime.UtcNow;
        }

        return result;
    }

    private async Task<SafetyCheckResult> ValidateRollbackSafetyAsync(string targetMigration)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        // Check for data loss potential
        var dataLossRisk = await AssessDataLossRiskAsync(connection, targetMigration);
        if (dataLossRisk.HasRisk)
        {
            return SafetyCheckResult.Unsafe($"Data loss risk detected: {dataLossRisk.Reason}");
        }

        // Check for breaking changes
        var breakingChanges = await IdentifyBreakingChangesAsync(connection, targetMigration);
        if (breakingChanges.Any())
        {
            return SafetyCheckResult.Unsafe($"Breaking changes detected: {string.Join(", ", breakingChanges)}");
        }

        return SafetyCheckResult.Safe();
    }

    private async Task<string> GenerateRollbackScriptAsync(string targetMigration)
    {
        // Use EF CLI to generate rollback script
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"ef migrations script {targetMigration} --project src/Academia.Infrastructure --startup-project src/Academia.API --idempotent",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        var script = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"Failed to generate rollback script. Exit code: {process.ExitCode}");
        }

        return script;
    }
}

public class RollbackResult
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool Success { get; set; }
    public string? Error { get; set; }
    public string? BackupName { get; set; }
}

public class SafetyCheckResult
{
    public bool IsSafe { get; private set; }
    public string? Reason { get; private set; }

    public static SafetyCheckResult Safe() => new() { IsSafe = true };
    public static SafetyCheckResult Unsafe(string reason) => new() { IsSafe = false, Reason = reason };
}
```

### Emergency Recovery Procedures

```csharp
public class EmergencyRecoveryService
{
    /// <summary>
    /// Emergency procedure to restore database to last known good state
    /// </summary>
    public async Task<bool> ExecuteEmergencyRecoveryAsync(EmergencyRecoveryOptions options)
    {
        _logger.LogCritical("EMERGENCY RECOVERY INITIATED - Target: {RecoveryPoint}", options.RecoveryPoint);

        try
        {
            // Step 1: Stop application traffic (if possible)
            await NotifyApplicationShutdownAsync();

            // Step 2: Create emergency backup of current state
            var emergencyBackup = await CreateEmergencyBackupAsync();
            _logger.LogInformation("Emergency backup created: {BackupName}", emergencyBackup);

            // Step 3: Restore to recovery point
            switch (options.RecoveryMethod)
            {
                case RecoveryMethod.PointInTimeRestore:
                    await ExecutePointInTimeRestoreAsync(options.RecoveryPoint);
                    break;
                case RecoveryMethod.BackupRestore:
                    await RestoreFromBackupAsync(options.BackupName);
                    break;
                case RecoveryMethod.ReplicationFailover:
                    await FailoverToReplicaAsync(options.ReplicaName);
                    break;
            }

            // Step 4: Validate recovery
            await ValidateRecoveryStateAsync();

            // Step 5: Restart application services
            await NotifyApplicationStartupAsync();

            _logger.LogInformation("Emergency recovery completed successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Emergency recovery failed - Manual intervention required");
            return false;
        }
    }

    /// <summary>
    /// Validates database state after recovery operation
    /// </summary>
    private async Task ValidateRecoveryStateAsync()
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        // Check critical tables exist and have data
        var criticalTables = new[] { "Students", "Courses", "Enrollments", "AcademicTerms" };

        foreach (var table in criticalTables)
        {
            var count = await GetTableRowCountAsync(connection, table);
            if (count == 0)
            {
                throw new InvalidOperationException($"Critical table {table} is empty after recovery");
            }
            _logger.LogInformation("Table {Table} has {Count} rows", table, count);
        }

        // Execute business rule validations
        await ExecuteBusinessRuleValidationsAsync(connection);

        _logger.LogInformation("Recovery state validation completed successfully");
    }
}

public class EmergencyRecoveryOptions
{
    public RecoveryMethod RecoveryMethod { get; set; }
    public DateTime RecoveryPoint { get; set; }
    public string? BackupName { get; set; }
    public string? ReplicaName { get; set; }
}

public enum RecoveryMethod
{
    PointInTimeRestore,
    BackupRestore,
    ReplicationFailover
}
```

## Data Validation and Testing

### Migration Testing Framework

```csharp
[TestFixture]
public class MigrationTests : IntegrationTestBase
{
    [Test]
    public async Task Migration_AddLastLoginToStudent_ShouldPreserveExistingData()
    {
        // Arrange: Create test data before migration
        await SeedTestDataAsync();
        var originalStudentCount = await CountStudentsAsync();

        // Act: Apply migration
        await ApplyMigrationAsync("AddLastLoginToStudent");

        // Assert: Verify data integrity
        var newStudentCount = await CountStudentsAsync();
        Assert.AreEqual(originalStudentCount, newStudentCount, "Student count should remain unchanged");

        // Verify new column exists and is nullable
        var hasLastLoginColumn = await ColumnExistsAsync("Students", "LastLoginAt");
        Assert.IsTrue(hasLastLoginColumn, "LastLoginAt column should exist");

        var isNullable = await IsColumnNullableAsync("Students", "LastLoginAt");
        Assert.IsTrue(isNullable, "LastLoginAt column should be nullable");
    }

    [Test]
    public async Task Migration_Rollback_ShouldRestoreOriginalState()
    {
        // Arrange: Apply migration and modify data
        await ApplyMigrationAsync("AddLastLoginToStudent");
        await UpdateStudentLastLoginAsync();

        // Act: Rollback migration
        await RollbackMigrationAsync("AddLastLoginToStudent");

        // Assert: Verify rollback success
        var hasLastLoginColumn = await ColumnExistsAsync("Students", "LastLoginAt");
        Assert.IsFalse(hasLastLoginColumn, "LastLoginAt column should not exist after rollback");

        var studentCount = await CountStudentsAsync();
        Assert.IsTrue(studentCount > 0, "Original student data should be preserved");
    }

    [Test]
    public async Task Migration_Performance_ShouldCompleteWithinTimeout()
    {
        // Arrange: Seed large dataset
        await SeedLargeDatasetAsync(10000); // 10k records

        var stopwatch = Stopwatch.StartNew();

        // Act: Apply migration
        await ApplyMigrationAsync("AddLastLoginToStudent");

        stopwatch.Stop();

        // Assert: Migration should complete within reasonable time
        Assert.Less(stopwatch.ElapsedMilliseconds, 30000, "Migration should complete within 30 seconds");
    }
}

[TestFixture]
public class DataValidationTests : IntegrationTestBase
{
    [Test]
    public async Task SeedData_ReferenceData_ShouldBeConsistent()
    {
        // Arrange & Act
        await SeedReferenceDataAsync();

        // Assert: Validate academic terms
        var academicTerms = await GetAcademicTermsAsync();
        Assert.IsTrue(academicTerms.Any(t => t.Code == "FALL2024"), "FALL2024 term should exist");
        Assert.IsTrue(academicTerms.Any(t => t.Code == "SPRING2025"), "SPRING2025 term should exist");

        // Assert: Validate grade scales
        var gradeScales = await GetGradeScalesAsync();
        Assert.IsTrue(gradeScales.Any(g => g.Grade == "A" && g.GradePoints == 4.0m), "A grade with 4.0 points should exist");
        Assert.IsTrue(gradeScales.Any(g => g.Grade == "F" && g.GradePoints == 0.0m), "F grade with 0.0 points should exist");
    }

    [Test]
    public async Task DataIntegrity_ForeignKeys_ShouldBeEnforced()
    {
        // Arrange
        await SeedMinimalDataAsync();

        // Act & Assert: Attempt to create orphaned enrollment
        var invalidEnrollment = CreateInvalidEnrollment(); // References non-existent student

        var exception = await Assert.ThrowsAsync<DbUpdateException>(
            async () => await SaveEnrollmentAsync(invalidEnrollment));

        Assert.That(exception.InnerException?.Message,
            Does.Contain("FOREIGN KEY constraint"),
            "Foreign key constraint should be enforced");
    }
}
```

### Production Validation Scripts

```sql
-- Production data validation script
-- Execute after each migration deployment

-- Validate table structures
DECLARE @ValidationErrors TABLE (ErrorType VARCHAR(50), ErrorMessage VARCHAR(MAX));

-- Check for missing tables
INSERT INTO @ValidationErrors (ErrorType, ErrorMessage)
SELECT 'MissingTable', 'Required table missing: ' + TableName
FROM (VALUES
    ('Students'), ('Courses'), ('Enrollments'),
    ('AcademicTerms'), ('GradeScales'), ('Departments')
) AS RequiredTables(TableName)
WHERE NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.TABLES
    WHERE TABLE_NAME = RequiredTables.TableName
);

-- Check for missing columns
INSERT INTO @ValidationErrors (ErrorType, ErrorMessage)
SELECT 'MissingColumn', 'Required column missing: ' + TableName + '.' + ColumnName
FROM (VALUES
    ('Students', 'Id'), ('Students', 'FirstName'), ('Students', 'LastName'),
    ('Courses', 'Id'), ('Courses', 'Code'), ('Courses', 'Title'),
    ('Enrollments', 'Id'), ('Enrollments', 'StudentId'), ('Enrollments', 'CourseId')
) AS RequiredColumns(TableName, ColumnName)
WHERE NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = RequiredColumns.TableName
    AND COLUMN_NAME = RequiredColumns.ColumnName
);

-- Check for orphaned records
INSERT INTO @ValidationErrors (ErrorType, ErrorMessage)
SELECT 'OrphanedRecord', 'Orphaned enrollments found: ' + CAST(COUNT(*) AS VARCHAR)
FROM Enrollments e
WHERE NOT EXISTS (SELECT 1 FROM Students s WHERE s.Id = e.StudentId)
   OR NOT EXISTS (SELECT 1 FROM Courses c WHERE c.Id = e.CourseId)
HAVING COUNT(*) > 0;

-- Check data constraints
INSERT INTO @ValidationErrors (ErrorType, ErrorMessage)
SELECT 'InvalidData', 'Students with invalid email format: ' + CAST(COUNT(*) AS VARCHAR)
FROM Students
WHERE Email NOT LIKE '%@%.%' AND Email IS NOT NULL
HAVING COUNT(*) > 0;

-- Report results
IF EXISTS (SELECT 1 FROM @ValidationErrors)
BEGIN
    SELECT ErrorType, ErrorMessage FROM @ValidationErrors;
    THROW 50000, 'Data validation failed. See error details.', 1;
END
ELSE
BEGIN
    PRINT 'Data validation completed successfully at ' + CONVERT(VARCHAR, GETDATE(), 120);
END;

-- Performance check
DECLARE @SlowQueries TABLE (QueryType VARCHAR(50), Duration INT);

INSERT INTO @SlowQueries (QueryType, Duration)
SELECT 'StudentLookup', DATEDIFF(ms, GETDATE(), GETDATE())
FROM Students WHERE Id = (SELECT TOP 1 Id FROM Students);

-- Log validation results
INSERT INTO MigrationValidationLog (ValidationDate, Status, Details)
VALUES (GETDATE(), 'SUCCESS', 'Production validation completed successfully');
```

## Related Documentation References

- [Azure Infrastructure](./azure-infrastructure.instructions.md)
- [Configuration Management](./configuration-management.instructions.md)
- [Testing Requirements](./testing-requirements.instructions.md)
- [Deployment Operations](./deployment-operations.instructions.md)
- [Monitoring and Observability](./monitoring-observability.instructions.md)

## Validation Checklist

Before considering the database migration implementation complete, verify:

- [ ] Entity Framework Core migration workflow is established with proper naming conventions
- [ ] Forward-only migration strategy is implemented with rollback safety checks
- [ ] Seed data management handles environment-specific data requirements
- [ ] Production deployment procedures include zero-downtime strategies and monitoring
- [ ] Automated rollback and recovery procedures are tested and documented
- [ ] Migration testing framework validates data integrity and performance
- [ ] CI/CD pipeline integration automates migration deployment and validation
- [ ] Emergency recovery procedures are documented and tested
- [ ] Data validation scripts check referential integrity and business rules
- [ ] Production validation includes automated checks for missing tables, columns, and orphaned data
- [ ] Performance monitoring during migrations prevents database overload
- [ ] Backup and snapshot strategies protect against data loss during migrations
