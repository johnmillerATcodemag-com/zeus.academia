using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Enums;

namespace Zeus.Academia.Infrastructure.Configurations;

/// <summary>
/// Entity Framework configuration for CatalogVersion entity.
/// </summary>
public class CatalogVersionConfiguration : IEntityTypeConfiguration<CatalogVersion>
{
    public void Configure(EntityTypeBuilder<CatalogVersion> builder)
    {
        builder.ToTable("CatalogVersions");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.VersionNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(v => v.VersionName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(v => v.Description)
            .HasMaxLength(1000);

        builder.Property(v => v.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(v => v.PublishedBy)
            .HasMaxLength(100);

        builder.Property(v => v.ChangeSummary)
            .HasMaxLength(2000);

        builder.Property(v => v.ReleaseNotes)
            .HasMaxLength(5000);

        builder.Property(v => v.ApprovedBy)
            .HasMaxLength(100);

        builder.Property(v => v.VersionType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(v => v.ApprovalStatus)
            .HasConversion<int>()
            .IsRequired();

        // Convert lists and dictionaries to JSON
        builder.Property(v => v.ChangeLog)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null!) ?? new List<string>(),
                new ValueComparer<List<string>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()))
            .HasColumnType("nvarchar(max)");

        builder.Property(v => v.Tags)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null!) ?? new List<string>(),
                new ValueComparer<List<string>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()))
            .HasColumnType("nvarchar(max)");

        builder.Property(v => v.Metadata)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions)null!) ?? new Dictionary<string, string>(),
                new ValueComparer<Dictionary<string, string>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)))
            .HasColumnType("nvarchar(max)");

        // Store snapshot data as compressed JSON
        builder.Property(v => v.SnapshotData)
            .HasColumnType("nvarchar(max)");

        // Relationships
        builder.HasOne(v => v.Catalog)
            .WithMany(c => c.Versions)
            .HasForeignKey(v => v.CatalogId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(v => v.PreviousVersion)
            .WithMany(v => v.SubsequentVersions)
            .HasForeignKey(v => v.PreviousVersionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(v => v.CatalogId)
            .HasDatabaseName("IX_CatalogVersions_CatalogId");

        builder.HasIndex(v => v.VersionNumber)
            .HasDatabaseName("IX_CatalogVersions_VersionNumber");

        builder.HasIndex(v => v.IsCurrent)
            .HasDatabaseName("IX_CatalogVersions_IsCurrent");

        builder.HasIndex(v => v.IsPublished)
            .HasDatabaseName("IX_CatalogVersions_IsPublished");

        builder.HasIndex(v => v.VersionType)
            .HasDatabaseName("IX_CatalogVersions_VersionType");

        builder.HasIndex(v => v.ApprovalStatus)
            .HasDatabaseName("IX_CatalogVersions_ApprovalStatus");

        builder.HasIndex(v => v.CreatedDate)
            .HasDatabaseName("IX_CatalogVersions_CreatedDate");

        // Unique constraint for catalog-version number combination
        builder.HasIndex(v => new { v.CatalogId, v.VersionNumber })
            .IsUnique()
            .HasDatabaseName("IX_CatalogVersions_CatalogId_VersionNumber_Unique");

        // Only one current version per catalog
        builder.HasIndex(v => v.CatalogId)
            .HasDatabaseName("IX_CatalogVersions_CatalogId_IsCurrent_Unique")
            .HasFilter("[IsCurrent] = 1")
            .IsUnique();
    }
}

/// <summary>
/// Entity Framework configuration for VersionChange entity.
/// </summary>
public class VersionChangeConfiguration : IEntityTypeConfiguration<VersionChange>
{
    public void Configure(EntityTypeBuilder<VersionChange> builder)
    {
        builder.ToTable("VersionChanges");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.EntityType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.PropertyName)
            .HasMaxLength(100);

        builder.Property(c => c.Description)
            .HasMaxLength(500);

        builder.Property(c => c.ChangedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.ImpactLevel)
            .HasMaxLength(20);

        builder.Property(c => c.ChangeType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(c => c.ApprovalStatus)
            .HasConversion<int>()
            .IsRequired();

        // Store old/new values as JSON for complex objects
        builder.Property(c => c.OldValue)
            .HasColumnType("nvarchar(max)");

        builder.Property(c => c.NewValue)
            .HasColumnType("nvarchar(max)");

        // Convert dictionary to JSON
        builder.Property(c => c.ChangeMetadata)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions)null!) ?? new Dictionary<string, string>(),
                new ValueComparer<Dictionary<string, string>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)))
            .HasColumnType("nvarchar(max)");

        // Relationships
        builder.HasOne(c => c.CatalogVersion)
            .WithMany(v => v.Changes)
            .HasForeignKey(c => c.CatalogVersionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(c => c.CatalogVersionId)
            .HasDatabaseName("IX_VersionChanges_CatalogVersionId");

        builder.HasIndex(c => c.ChangeType)
            .HasDatabaseName("IX_VersionChanges_ChangeType");

        builder.HasIndex(c => c.EntityType)
            .HasDatabaseName("IX_VersionChanges_EntityType");

        builder.HasIndex(c => c.EntityId)
            .HasDatabaseName("IX_VersionChanges_EntityId");

        builder.HasIndex(c => c.ChangeDate)
            .HasDatabaseName("IX_VersionChanges_ChangeDate");

        builder.HasIndex(c => c.ImpactLevel)
            .HasDatabaseName("IX_VersionChanges_ImpactLevel");

        builder.HasIndex(c => c.RequiresApproval)
            .HasDatabaseName("IX_VersionChanges_RequiresApproval");
    }
}

/// <summary>
/// Entity Framework configuration for VersionComparison entity.
/// </summary>
public class VersionComparisonConfiguration : IEntityTypeConfiguration<VersionComparison>
{
    public void Configure(EntityTypeBuilder<VersionComparison> builder)
    {
        builder.ToTable("VersionComparisons");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.ComparedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.DifferencesSummary)
            .HasMaxLength(2000);

        builder.Property(c => c.SimilarityPercentage)
            .HasColumnType("decimal(5,2)");

        builder.Property(c => c.ComparisonType)
            .HasConversion<int>()
            .IsRequired();

        // Store comparison results as JSON
        builder.Property(c => c.ComparisonResults)
            .HasColumnType("nvarchar(max)");

        // Convert dictionary to JSON
        builder.Property(c => c.ComparisonMetrics)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<Dictionary<string, decimal>>(v, (JsonSerializerOptions)null!) ?? new Dictionary<string, decimal>(),
                new ValueComparer<Dictionary<string, decimal>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)))
            .HasColumnType("nvarchar(max)");

        // Relationships
        builder.HasOne(c => c.SourceVersion)
            .WithMany(v => v.ComparisonsAsSource)
            .HasForeignKey(c => c.SourceVersionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.TargetVersion)
            .WithMany(v => v.ComparisonsAsTarget)
            .HasForeignKey(c => c.TargetVersionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(c => c.SourceVersionId)
            .HasDatabaseName("IX_VersionComparisons_SourceVersionId");

        builder.HasIndex(c => c.TargetVersionId)
            .HasDatabaseName("IX_VersionComparisons_TargetVersionId");

        builder.HasIndex(c => c.ComparisonType)
            .HasDatabaseName("IX_VersionComparisons_ComparisonType");

        builder.HasIndex(c => c.ComparisonDate)
            .HasDatabaseName("IX_VersionComparisons_ComparisonDate");

        builder.HasIndex(c => c.IsArchived)
            .HasDatabaseName("IX_VersionComparisons_IsArchived");

        // Unique constraint for source-target combination
        builder.HasIndex(c => new { c.SourceVersionId, c.TargetVersionId, c.ComparisonType })
            .IsUnique()
            .HasDatabaseName("IX_VersionComparisons_Source_Target_Type_Unique");

        // Check constraints
        builder.ToTable("VersionComparisons", t =>
        {
            t.HasCheckConstraint("CK_VersionComparisons_SimilarityPercentage", "[SimilarityPercentage] >= 0 AND [SimilarityPercentage] <= 100");
            t.HasCheckConstraint("CK_VersionComparisons_SourceTarget", "[SourceVersionId] <> [TargetVersionId]");
        });
    }
}

/// <summary>
/// Entity Framework configuration for ComparisonDetail entity.
/// </summary>
public class ComparisonDetailConfiguration : IEntityTypeConfiguration<ComparisonDetail>
{
    public void Configure(EntityTypeBuilder<ComparisonDetail> builder)
    {
        builder.ToTable("ComparisonDetails");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.EntityType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.PropertyName)
            .HasMaxLength(100);

        builder.Property(d => d.Description)
            .HasMaxLength(500);

        builder.Property(d => d.Significance)
            .HasMaxLength(20);

        builder.Property(d => d.ChangeType)
            .HasConversion<int>()
            .IsRequired();

        // Store old/new values as JSON for complex objects
        builder.Property(d => d.OldValue)
            .HasColumnType("nvarchar(max)");

        builder.Property(d => d.NewValue)
            .HasColumnType("nvarchar(max)");

        // Relationships
        builder.HasOne(d => d.VersionComparison)
            .WithMany(c => c.Details)
            .HasForeignKey(d => d.VersionComparisonId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(d => d.VersionComparisonId)
            .HasDatabaseName("IX_ComparisonDetails_VersionComparisonId");

        builder.HasIndex(d => d.ChangeType)
            .HasDatabaseName("IX_ComparisonDetails_ChangeType");

        builder.HasIndex(d => d.EntityType)
            .HasDatabaseName("IX_ComparisonDetails_EntityType");

        builder.HasIndex(d => d.EntityId)
            .HasDatabaseName("IX_ComparisonDetails_EntityId");

        builder.HasIndex(d => d.Significance)
            .HasDatabaseName("IX_ComparisonDetails_Significance");
    }
}