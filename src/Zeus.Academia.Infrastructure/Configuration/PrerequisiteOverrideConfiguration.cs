using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.Infrastructure.Configuration;

/// <summary>
/// Entity Framework configuration for PrerequisiteOverride entity.
/// </summary>
public class PrerequisiteOverrideConfiguration : IEntityTypeConfiguration<PrerequisiteOverride>
{
    public void Configure(EntityTypeBuilder<PrerequisiteOverride> builder)
    {
        builder.ToTable("PrerequisiteOverrides");

        builder.HasKey(po => po.Id);

        builder.Property(po => po.OverrideType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(po => po.OverrideScope)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(po => po.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(po => po.OverrideReason)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(po => po.RequestedBy)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(po => po.RequestedDate)
            .IsRequired();

        builder.Property(po => po.ApprovedBy)
            .HasMaxLength(100);

        builder.Property(po => po.ApproverAuthority)
            .HasConversion<int>();

        builder.Property(po => po.IsActive)
            .HasDefaultValue(true);

        builder.Property(po => po.Conditions)
            .HasMaxLength(1000);

        builder.Property(po => po.Priority)
            .HasDefaultValue(1);

        builder.Property(po => po.RequiresPeriodicReview)
            .HasDefaultValue(false);

        builder.Property(po => po.Notes)
            .HasMaxLength(2000);

        builder.Property(po => po.OverrideMetadata)
            .HasMaxLength(1000);

        // Indexes
        builder.HasIndex(po => po.StudentId)
            .HasDatabaseName("IX_PrerequisiteOverrides_StudentId");

        builder.HasIndex(po => po.CourseId)
            .HasDatabaseName("IX_PrerequisiteOverrides_CourseId");

        builder.HasIndex(po => po.AcademicTermId)
            .HasDatabaseName("IX_PrerequisiteOverrides_AcademicTermId");

        builder.HasIndex(po => po.OverrideType)
            .HasDatabaseName("IX_PrerequisiteOverrides_Type");

        builder.HasIndex(po => po.Status)
            .HasDatabaseName("IX_PrerequisiteOverrides_Status");

        builder.HasIndex(po => po.IsActive)
            .HasDatabaseName("IX_PrerequisiteOverrides_IsActive");

        builder.HasIndex(po => po.RequestedDate)
            .HasDatabaseName("IX_PrerequisiteOverrides_RequestedDate");

        builder.HasIndex(po => po.ExpirationDate)
            .HasDatabaseName("IX_PrerequisiteOverrides_ExpirationDate");

        builder.HasIndex(po => new { po.StudentId, po.CourseId, po.AcademicTermId })
            .HasDatabaseName("IX_PrerequisiteOverrides_Student_Course_Term");

        builder.HasIndex(po => new { po.StudentId, po.IsActive })
            .HasDatabaseName("IX_PrerequisiteOverrides_Student_Active");

        // Relationships
        builder.HasOne(po => po.Student)
            .WithMany()
            .HasForeignKey(po => po.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(po => po.Course)
            .WithMany()
            .HasForeignKey(po => po.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(po => po.AcademicTerm)
            .WithMany()
            .HasForeignKey(po => po.AcademicTermId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(po => po.AffectedRules)
            .WithOne(orm => orm.PrerequisiteOverride)
            .HasForeignKey(orm => orm.PrerequisiteOverrideId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(po => po.ApprovalWorkflow)
            .WithOne(oas => oas.PrerequisiteOverride)
            .HasForeignKey(oas => oas.PrerequisiteOverrideId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(po => po.AuditTrail)
            .WithOne(oae => oae.PrerequisiteOverride)
            .HasForeignKey(oae => oae.PrerequisiteOverrideId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(po => po.AttachedDocuments)
            .WithOne(od => od.PrerequisiteOverride)
            .HasForeignKey(od => od.PrerequisiteOverrideId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>
/// Entity Framework configuration for PrerequisiteWaiver entity.
/// </summary>
public class PrerequisiteWaiverConfiguration : IEntityTypeConfiguration<PrerequisiteWaiver>
{
    public void Configure(EntityTypeBuilder<PrerequisiteWaiver> builder)
    {
        builder.ToTable("PrerequisiteWaivers");

        builder.HasKey(pw => pw.Id);

        builder.Property(pw => pw.WaiverType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(pw => pw.WaiverScope)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(pw => pw.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(pw => pw.WaiverReason)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(pw => pw.Justification)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(pw => pw.RequestedBy)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(pw => pw.RequestedDate)
            .IsRequired();

        builder.Property(pw => pw.ApprovedBy)
            .HasMaxLength(100);

        builder.Property(pw => pw.IsActive)
            .HasDefaultValue(true);

        builder.Property(pw => pw.IsPermanent)
            .HasDefaultValue(false);

        builder.Property(pw => pw.Conditions)
            .HasMaxLength(1000);

        builder.Property(pw => pw.AcademicConsequences)
            .HasMaxLength(1000);

        builder.Property(pw => pw.StudentAcknowledged)
            .HasDefaultValue(false);

        builder.Property(pw => pw.Notes)
            .HasMaxLength(2000);

        builder.Property(pw => pw.WaiverMetadata)
            .HasMaxLength(1000);

        // Indexes
        builder.HasIndex(pw => pw.StudentId)
            .HasDatabaseName("IX_PrerequisiteWaivers_StudentId");

        builder.HasIndex(pw => pw.CourseId)
            .HasDatabaseName("IX_PrerequisiteWaivers_CourseId");

        builder.HasIndex(pw => pw.WaiverType)
            .HasDatabaseName("IX_PrerequisiteWaivers_Type");

        builder.HasIndex(pw => pw.Status)
            .HasDatabaseName("IX_PrerequisiteWaivers_Status");

        builder.HasIndex(pw => pw.IsActive)
            .HasDatabaseName("IX_PrerequisiteWaivers_IsActive");

        builder.HasIndex(pw => pw.IsPermanent)
            .HasDatabaseName("IX_PrerequisiteWaivers_IsPermanent");

        builder.HasIndex(pw => pw.RequestedDate)
            .HasDatabaseName("IX_PrerequisiteWaivers_RequestedDate");

        builder.HasIndex(pw => pw.ExpirationDate)
            .HasDatabaseName("IX_PrerequisiteWaivers_ExpirationDate");

        builder.HasIndex(pw => new { pw.StudentId, pw.CourseId })
            .HasDatabaseName("IX_PrerequisiteWaivers_Student_Course");

        builder.HasIndex(pw => new { pw.StudentId, pw.IsActive })
            .HasDatabaseName("IX_PrerequisiteWaivers_Student_Active");

        // Relationships
        builder.HasOne(pw => pw.Student)
            .WithMany()
            .HasForeignKey(pw => pw.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pw => pw.Course)
            .WithMany()
            .HasForeignKey(pw => pw.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(pw => pw.AffectedRules)
            .WithOne(wrm => wrm.PrerequisiteWaiver)
            .HasForeignKey(wrm => wrm.PrerequisiteWaiverId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(pw => pw.AttachedDocuments)
            .WithOne(wd => wd.PrerequisiteWaiver)
            .HasForeignKey(wd => wd.PrerequisiteWaiverId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>
/// Entity Framework configuration for OverrideRuleMapping entity.
/// </summary>
public class OverrideRuleMappingConfiguration : IEntityTypeConfiguration<OverrideRuleMapping>
{
    public void Configure(EntityTypeBuilder<OverrideRuleMapping> builder)
    {
        builder.ToTable("OverrideRuleMappings");

        builder.HasKey(orm => orm.Id);

        builder.Property(orm => orm.IsCompleteOverride)
            .HasDefaultValue(true);

        builder.Property(orm => orm.PartialOverrideConditions)
            .HasMaxLength(500);

        // Indexes
        builder.HasIndex(orm => orm.PrerequisiteOverrideId)
            .HasDatabaseName("IX_OverrideRuleMappings_OverrideId");

        builder.HasIndex(orm => orm.PrerequisiteRuleId)
            .HasDatabaseName("IX_OverrideRuleMappings_PrerequisiteRuleId");

        builder.HasIndex(orm => orm.CorequisiteRuleId)
            .HasDatabaseName("IX_OverrideRuleMappings_CorequisiteRuleId");

        builder.HasIndex(orm => orm.EnrollmentRestrictionId)
            .HasDatabaseName("IX_OverrideRuleMappings_RestrictionId");

        // Relationships
        builder.HasOne(orm => orm.PrerequisiteOverride)
            .WithMany(po => po.AffectedRules)
            .HasForeignKey(orm => orm.PrerequisiteOverrideId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(orm => orm.PrerequisiteRule)
            .WithMany()
            .HasForeignKey(orm => orm.PrerequisiteRuleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(orm => orm.CorequisiteRule)
            .WithMany()
            .HasForeignKey(orm => orm.CorequisiteRuleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(orm => orm.EnrollmentRestriction)
            .WithMany()
            .HasForeignKey(orm => orm.EnrollmentRestrictionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

/// <summary>
/// Entity Framework configuration for WaiverRuleMapping entity.
/// </summary>
public class WaiverRuleMappingConfiguration : IEntityTypeConfiguration<WaiverRuleMapping>
{
    public void Configure(EntityTypeBuilder<WaiverRuleMapping> builder)
    {
        builder.ToTable("WaiverRuleMappings");

        builder.HasKey(wrm => wrm.Id);

        builder.Property(wrm => wrm.IsCompleteWaiver)
            .HasDefaultValue(true);

        builder.Property(wrm => wrm.PartialWaiverConditions)
            .HasMaxLength(500);

        // Indexes
        builder.HasIndex(wrm => wrm.PrerequisiteWaiverId)
            .HasDatabaseName("IX_WaiverRuleMappings_WaiverId");

        builder.HasIndex(wrm => wrm.PrerequisiteRuleId)
            .HasDatabaseName("IX_WaiverRuleMappings_PrerequisiteRuleId");

        builder.HasIndex(wrm => wrm.CorequisiteRuleId)
            .HasDatabaseName("IX_WaiverRuleMappings_CorequisiteRuleId");

        // Relationships
        builder.HasOne(wrm => wrm.PrerequisiteWaiver)
            .WithMany(pw => pw.AffectedRules)
            .HasForeignKey(wrm => wrm.PrerequisiteWaiverId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(wrm => wrm.PrerequisiteRule)
            .WithMany()
            .HasForeignKey(wrm => wrm.PrerequisiteRuleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(wrm => wrm.CorequisiteRule)
            .WithMany()
            .HasForeignKey(wrm => wrm.CorequisiteRuleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

/// <summary>
/// Entity Framework configuration for OverrideApprovalStep entity.
/// </summary>
public class OverrideApprovalStepConfiguration : IEntityTypeConfiguration<OverrideApprovalStep>
{
    public void Configure(EntityTypeBuilder<OverrideApprovalStep> builder)
    {
        builder.ToTable("OverrideApprovalSteps");

        builder.HasKey(oas => oas.Id);

        builder.Property(oas => oas.StepNumber)
            .IsRequired();

        builder.Property(oas => oas.StepName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(oas => oas.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(oas => oas.RequiredAuthority)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(oas => oas.AssignedTo)
            .HasMaxLength(100);

        builder.Property(oas => oas.ApprovedBy)
            .HasMaxLength(100);

        builder.Property(oas => oas.ApproverComments)
            .HasMaxLength(1000);

        builder.Property(oas => oas.CanDelegate)
            .HasDefaultValue(false);

        builder.Property(oas => oas.DelegatedTo)
            .HasMaxLength(100);

        // Indexes
        builder.HasIndex(oas => oas.PrerequisiteOverrideId)
            .HasDatabaseName("IX_OverrideApprovalSteps_OverrideId");

        builder.HasIndex(oas => oas.Status)
            .HasDatabaseName("IX_OverrideApprovalSteps_Status");

        builder.HasIndex(oas => oas.DueDate)
            .HasDatabaseName("IX_OverrideApprovalSteps_DueDate");

        builder.HasIndex(oas => new { oas.PrerequisiteOverrideId, oas.StepNumber })
            .HasDatabaseName("IX_OverrideApprovalSteps_Override_Step");

        // Relationships
        builder.HasOne(oas => oas.PrerequisiteOverride)
            .WithMany(po => po.ApprovalWorkflow)
            .HasForeignKey(oas => oas.PrerequisiteOverrideId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>
/// Entity Framework configuration for OverrideAuditEntry entity.
/// </summary>
public class OverrideAuditEntryConfiguration : IEntityTypeConfiguration<OverrideAuditEntry>
{
    public void Configure(EntityTypeBuilder<OverrideAuditEntry> builder)
    {
        builder.ToTable("OverrideAuditEntries");

        builder.HasKey(oae => oae.Id);

        builder.Property(oae => oae.AuditDate)
            .IsRequired();

        builder.Property(oae => oae.PerformedBy)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(oae => oae.ActionType)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(oae => oae.ActionDescription)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(oae => oae.OldValues)
            .HasMaxLength(2000);

        builder.Property(oae => oae.NewValues)
            .HasMaxLength(2000);

        builder.Property(oae => oae.IPAddress)
            .HasMaxLength(45);

        builder.Property(oae => oae.UserAgent)
            .HasMaxLength(500);

        // Indexes
        builder.HasIndex(oae => oae.PrerequisiteOverrideId)
            .HasDatabaseName("IX_OverrideAuditEntries_OverrideId");

        builder.HasIndex(oae => oae.AuditDate)
            .HasDatabaseName("IX_OverrideAuditEntries_AuditDate");

        builder.HasIndex(oae => oae.PerformedBy)
            .HasDatabaseName("IX_OverrideAuditEntries_PerformedBy");

        builder.HasIndex(oae => oae.ActionType)
            .HasDatabaseName("IX_OverrideAuditEntries_ActionType");

        // Relationships
        builder.HasOne(oae => oae.PrerequisiteOverride)
            .WithMany(po => po.AuditTrail)
            .HasForeignKey(oae => oae.PrerequisiteOverrideId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>
/// Entity Framework configuration for OverrideDocument entity.
/// </summary>
public class OverrideDocumentConfiguration : IEntityTypeConfiguration<OverrideDocument>
{
    public void Configure(EntityTypeBuilder<OverrideDocument> builder)
    {
        builder.ToTable("OverrideDocuments");

        builder.HasKey(od => od.Id);

        builder.Property(od => od.FileName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(od => od.ContentType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(od => od.FileSize)
            .IsRequired();

        builder.Property(od => od.StoragePath)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(od => od.Description)
            .HasMaxLength(500);

        builder.Property(od => od.UploadedBy)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(od => od.UploadedDate)
            .IsRequired();

        builder.Property(od => od.IsRequired)
            .HasDefaultValue(false);

        // Indexes
        builder.HasIndex(od => od.PrerequisiteOverrideId)
            .HasDatabaseName("IX_OverrideDocuments_OverrideId");

        builder.HasIndex(od => od.UploadedDate)
            .HasDatabaseName("IX_OverrideDocuments_UploadedDate");

        builder.HasIndex(od => od.UploadedBy)
            .HasDatabaseName("IX_OverrideDocuments_UploadedBy");

        // Relationships
        builder.HasOne(od => od.PrerequisiteOverride)
            .WithMany(po => po.AttachedDocuments)
            .HasForeignKey(od => od.PrerequisiteOverrideId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>
/// Entity Framework configuration for WaiverDocument entity.
/// </summary>
public class WaiverDocumentConfiguration : IEntityTypeConfiguration<WaiverDocument>
{
    public void Configure(EntityTypeBuilder<WaiverDocument> builder)
    {
        builder.ToTable("WaiverDocuments");

        builder.HasKey(wd => wd.Id);

        builder.Property(wd => wd.FileName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(wd => wd.ContentType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(wd => wd.FileSize)
            .IsRequired();

        builder.Property(wd => wd.StoragePath)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(wd => wd.Description)
            .HasMaxLength(500);

        builder.Property(wd => wd.UploadedBy)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(wd => wd.UploadedDate)
            .IsRequired();

        builder.Property(wd => wd.IsRequired)
            .HasDefaultValue(false);

        // Indexes
        builder.HasIndex(wd => wd.PrerequisiteWaiverId)
            .HasDatabaseName("IX_WaiverDocuments_WaiverId");

        builder.HasIndex(wd => wd.UploadedDate)
            .HasDatabaseName("IX_WaiverDocuments_UploadedDate");

        builder.HasIndex(wd => wd.UploadedBy)
            .HasDatabaseName("IX_WaiverDocuments_UploadedBy");

        // Relationships
        builder.HasOne(wd => wd.PrerequisiteWaiver)
            .WithMany(pw => pw.AttachedDocuments)
            .HasForeignKey(wd => wd.PrerequisiteWaiverId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}