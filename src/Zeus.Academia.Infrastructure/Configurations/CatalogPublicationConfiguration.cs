using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Enums;

namespace Zeus.Academia.Infrastructure.Configurations;

/// <summary>
/// Entity Framework configuration for CatalogPublication entity.
/// </summary>
public class CatalogPublicationConfiguration : IEntityTypeConfiguration<CatalogPublication>
{
    public void Configure(EntityTypeBuilder<CatalogPublication> builder)
    {
        builder.ToTable("CatalogPublications");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(p => p.FilePath)
            .HasMaxLength(500);

        builder.Property(p => p.AccessUrl)
            .HasMaxLength(500);

        builder.Property(p => p.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.PublishedBy)
            .HasMaxLength(100);

        builder.Property(p => p.Checksum)
            .HasMaxLength(100);

        builder.Property(p => p.Version)
            .HasMaxLength(10);

        builder.Property(p => p.Format)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(p => p.Status)
            .HasConversion<int>()
            .IsRequired();

        // Convert lists to JSON
        builder.Property(p => p.DistributionChannels)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<List<DistributionChannel>>(v, (JsonSerializerOptions)null!) ?? new List<DistributionChannel>(),
                new ValueComparer<List<DistributionChannel>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()))
            .HasColumnType("nvarchar(max)");

        // Convert dictionaries to JSON
        builder.Property(p => p.SecuritySettings)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions)null!) ?? new Dictionary<string, string>(),
                new ValueComparer<Dictionary<string, string>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)))
            .HasColumnType("nvarchar(max)");

        builder.Property(p => p.Metadata)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions)null!) ?? new Dictionary<string, string>(),
                new ValueComparer<Dictionary<string, string>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)))
            .HasColumnType("nvarchar(max)");

        builder.Property(p => p.PublicationSettings)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions)null!) ?? new Dictionary<string, object>(),
                new ValueComparer<Dictionary<string, object>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)))
            .HasColumnType("nvarchar(max)");

        // Relationships
        builder.HasOne(p => p.Catalog)
            .WithMany(c => c.Publications)
            .HasForeignKey(p => p.CatalogId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(p => p.CatalogId)
            .HasDatabaseName("IX_CatalogPublications_CatalogId");

        builder.HasIndex(p => p.Format)
            .HasDatabaseName("IX_CatalogPublications_Format");

        builder.HasIndex(p => p.Status)
            .HasDatabaseName("IX_CatalogPublications_Status");

        builder.HasIndex(p => p.IsActive)
            .HasDatabaseName("IX_CatalogPublications_IsActive");

        builder.HasIndex(p => p.PublicationDate)
            .HasDatabaseName("IX_CatalogPublications_PublicationDate");

        // Unique constraint for catalog-format combination (only one active publication per format per catalog)
        builder.HasIndex(p => new { p.CatalogId, p.Format })
            .HasDatabaseName("IX_CatalogPublications_CatalogId_Format_Unique")
            .HasFilter("[IsActive] = 1")
            .IsUnique();
    }
}

/// <summary>
/// Entity Framework configuration for PublicationDistribution entity.
/// </summary>
public class PublicationDistributionConfiguration : IEntityTypeConfiguration<PublicationDistribution>
{
    public void Configure(EntityTypeBuilder<PublicationDistribution> builder)
    {
        builder.ToTable("PublicationDistributions");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.DistributedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.TargetAudience)
            .HasMaxLength(200);

        builder.Property(d => d.ErrorMessage)
            .HasMaxLength(1000);

        builder.Property(d => d.Channel)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(d => d.Status)
            .HasConversion<int>()
            .IsRequired();

        // Convert dictionary to JSON
        builder.Property(d => d.Metrics)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<Dictionary<string, decimal>>(v, (JsonSerializerOptions)null!) ?? new Dictionary<string, decimal>(),
                new ValueComparer<Dictionary<string, decimal>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)))
            .HasColumnType("nvarchar(max)");

        // Relationships
        builder.HasOne(d => d.CatalogPublication)
            .WithMany(p => p.Distributions)
            .HasForeignKey(d => d.CatalogPublicationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(d => d.CatalogPublicationId)
            .HasDatabaseName("IX_PublicationDistributions_CatalogPublicationId");

        builder.HasIndex(d => d.Channel)
            .HasDatabaseName("IX_PublicationDistributions_Channel");

        builder.HasIndex(d => d.Status)
            .HasDatabaseName("IX_PublicationDistributions_Status");

        builder.HasIndex(d => d.DistributionDate)
            .HasDatabaseName("IX_PublicationDistributions_DistributionDate");
    }
}

/// <summary>
/// Entity Framework configuration for PublicationAccessLog entity.
/// </summary>
public class PublicationAccessLogConfiguration : IEntityTypeConfiguration<PublicationAccessLog>
{
    public void Configure(EntityTypeBuilder<PublicationAccessLog> builder)
    {
        builder.ToTable("PublicationAccessLogs");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.AccessType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(l => l.AccessedBy)
            .HasMaxLength(100);

        builder.Property(l => l.IpAddress)
            .HasMaxLength(45); // IPv6 support

        builder.Property(l => l.UserAgent)
            .HasMaxLength(500);

        builder.Property(l => l.Referrer)
            .HasMaxLength(500);

        builder.Property(l => l.SessionId)
            .HasMaxLength(100);

        builder.Property(l => l.ErrorMessage)
            .HasMaxLength(500);

        // Relationships
        builder.HasOne(l => l.CatalogPublication)
            .WithMany(p => p.AccessLogs)
            .HasForeignKey(l => l.CatalogPublicationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(l => l.CatalogPublicationId)
            .HasDatabaseName("IX_PublicationAccessLogs_CatalogPublicationId");

        builder.HasIndex(l => l.AccessDateTime)
            .HasDatabaseName("IX_PublicationAccessLogs_AccessDateTime");

        builder.HasIndex(l => l.AccessType)
            .HasDatabaseName("IX_PublicationAccessLogs_AccessType");

        builder.HasIndex(l => l.IsSuccessful)
            .HasDatabaseName("IX_PublicationAccessLogs_IsSuccessful");

        builder.HasIndex(l => l.AccessedBy)
            .HasDatabaseName("IX_PublicationAccessLogs_AccessedBy");

        // Partitioning hint for large logs (commented for now)
        // builder.HasPartitionFunction("PF_AccessDateTime", p => p.AccessDateTime);
    }
}