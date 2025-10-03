using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Enums;

namespace Zeus.Academia.Infrastructure.Configurations;

/// <summary>
/// Entity Framework configuration for CourseCatalog entity.
/// </summary>
public class CourseCatalogConfiguration : IEntityTypeConfiguration<CourseCatalog>
{
    public void Configure(EntityTypeBuilder<CourseCatalog> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.CatalogName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Version)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(c => c.Description)
            .HasMaxLength(1000);

        builder.Property(c => c.EffectiveDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(c => c.ExpirationDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(c => c.PublicationDate)
            .HasColumnType("date");

        builder.Property(c => c.CoverImageUrl)
            .HasMaxLength(500);

        builder.Property(c => c.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(c => c.CatalogType)
            .HasConversion<int>()
            .IsRequired();

        // Convert lists to JSON
        builder.Property(c => c.PublicationFormats)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<List<PublicationFormat>>(v, (JsonSerializerOptions)null!) ?? new List<PublicationFormat>(),
                new ValueComparer<List<PublicationFormat>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()))
            .HasColumnType("nvarchar(max)");

        builder.Property(c => c.DistributionChannels)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<List<DistributionChannel>>(v, (JsonSerializerOptions)null!) ?? new List<DistributionChannel>(),
                new ValueComparer<List<DistributionChannel>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()))
            .HasColumnType("nvarchar(max)");

        // Self-referencing relationship
        builder.HasOne(c => c.BasedOnCatalog)
            .WithMany(c => c.DerivedCatalogs)
            .HasForeignKey(c => c.BasedOnCatalogId)
            .OnDelete(DeleteBehavior.Restrict);

        // Index for performance
        builder.HasIndex(c => c.AcademicYear)
            .HasDatabaseName("IX_CourseCatalogs_AcademicYear");

        builder.HasIndex(c => c.Status)
            .HasDatabaseName("IX_CourseCatalogs_Status");

        builder.HasIndex(c => new { c.AcademicYear, c.CatalogType })
            .HasDatabaseName("IX_CourseCatalogs_AcademicYear_CatalogType");

        // Configure table with constraints
        builder.ToTable("CourseCatalogs", t =>
        {
            t.HasCheckConstraint("CK_CourseCatalogs_AcademicYear", "[AcademicYear] >= 2020 AND [AcademicYear] <= 2050");
            t.HasCheckConstraint("CK_CourseCatalogs_Dates", "[EffectiveDate] <= [ExpirationDate]");
        });
    }
}

/// <summary>
/// Entity Framework configuration for CourseApprovalWorkflow entity.
/// </summary>
public class CourseApprovalWorkflowConfiguration : IEntityTypeConfiguration<CourseApprovalWorkflow>
{
    public void Configure(EntityTypeBuilder<CourseApprovalWorkflow> builder)
    {
        builder.ToTable("CourseApprovalWorkflows");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.WorkflowName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(w => w.InitiatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(w => w.Notes)
            .HasMaxLength(2000);

        builder.Property(w => w.Priority)
            .HasMaxLength(20);

        builder.Property(w => w.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(w => w.CurrentStage)
            .HasConversion<int>()
            .IsRequired();

        // Relationships
        builder.HasOne(w => w.Course)
            .WithMany()
            .HasForeignKey(w => w.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(w => w.Catalog)
            .WithMany(c => c.ApprovalWorkflows)
            .HasForeignKey(w => w.CatalogId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(w => w.CourseId)
            .HasDatabaseName("IX_CourseApprovalWorkflows_CourseId");

        builder.HasIndex(w => w.Status)
            .HasDatabaseName("IX_CourseApprovalWorkflows_Status");

        builder.HasIndex(w => w.CurrentStage)
            .HasDatabaseName("IX_CourseApprovalWorkflows_CurrentStage");
    }
}

/// <summary>
/// Entity Framework configuration for ApprovalStep entity.
/// </summary>
public class ApprovalStepConfiguration : IEntityTypeConfiguration<ApprovalStep>
{
    public void Configure(EntityTypeBuilder<ApprovalStep> builder)
    {
        builder.ToTable("ApprovalSteps");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.AssignedTo)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Comments)
            .HasMaxLength(2000);

        builder.Property(s => s.ApprovalStage)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(s => s.Status)
            .HasConversion<int>()
            .IsRequired();

        // Convert lists to JSON
        builder.Property(s => s.RequiredDocuments)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null!) ?? new List<string>(),
                new ValueComparer<List<string>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()))
            .HasColumnType("nvarchar(max)");

        builder.Property(s => s.ReviewCriteria)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null!) ?? new List<string>(),
                new ValueComparer<List<string>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()))
            .HasColumnType("nvarchar(max)");

        // Relationships
        builder.HasOne(s => s.Workflow)
            .WithMany(w => w.ApprovalSteps)
            .HasForeignKey(s => s.WorkflowId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(s => s.WorkflowId)
            .HasDatabaseName("IX_ApprovalSteps_WorkflowId");

        builder.HasIndex(s => new { s.WorkflowId, s.StepOrder })
            .IsUnique()
            .HasDatabaseName("IX_ApprovalSteps_WorkflowId_StepOrder");

        builder.HasIndex(s => s.Status)
            .HasDatabaseName("IX_ApprovalSteps_Status");
    }
}

/// <summary>
/// Entity Framework configuration for ApprovalAttachment entity.
/// </summary>
public class ApprovalAttachmentConfiguration : IEntityTypeConfiguration<ApprovalAttachment>
{
    public void Configure(EntityTypeBuilder<ApprovalAttachment> builder)
    {
        builder.ToTable("ApprovalAttachments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(a => a.FilePath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(a => a.UploadedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Description)
            .HasMaxLength(500);

        // Relationships
        builder.HasOne(a => a.ApprovalStep)
            .WithMany(s => s.Attachments)
            .HasForeignKey(a => a.ApprovalStepId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(a => a.ApprovalStepId)
            .HasDatabaseName("IX_ApprovalAttachments_ApprovalStepId");
    }
}

/// <summary>
/// Entity Framework configuration for CatalogApproval entity.
/// </summary>
public class CatalogApprovalConfiguration : IEntityTypeConfiguration<CatalogApproval>
{
    public void Configure(EntityTypeBuilder<CatalogApproval> builder)
    {
        builder.ToTable("CatalogApprovals");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.ApprovedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Comments)
            .HasMaxLength(1000);

        builder.Property(a => a.ApprovalStage)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(a => a.Status)
            .HasConversion<int>()
            .IsRequired();

        // Relationships
        builder.HasOne(a => a.Catalog)
            .WithMany(c => c.ApprovalHistory)
            .HasForeignKey(a => a.CatalogId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(a => a.CatalogId)
            .HasDatabaseName("IX_CatalogApprovals_CatalogId");

        builder.HasIndex(a => a.ApprovalStage)
            .HasDatabaseName("IX_CatalogApprovals_ApprovalStage");

        builder.HasIndex(a => a.Status)
            .HasDatabaseName("IX_CatalogApprovals_Status");
    }
}