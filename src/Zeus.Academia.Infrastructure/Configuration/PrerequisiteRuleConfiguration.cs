using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.Infrastructure.Configuration;

/// <summary>
/// Entity Framework configuration for PrerequisiteRule entity.
/// </summary>
public class PrerequisiteRuleConfiguration : IEntityTypeConfiguration<PrerequisiteRule>
{
    public void Configure(EntityTypeBuilder<PrerequisiteRule> builder)
    {
        builder.ToTable("PrerequisiteRules");

        builder.HasKey(pr => pr.Id);

        builder.Property(pr => pr.RuleName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(pr => pr.Description)
            .HasMaxLength(1000);

        builder.Property(pr => pr.LogicOperator)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(pr => pr.IsActive)
            .HasDefaultValue(true);

        builder.Property(pr => pr.Priority)
            .HasDefaultValue(1);

        builder.Property(pr => pr.RuleMetadata)
            .HasMaxLength(2000);

        // Indexes
        builder.HasIndex(pr => pr.CourseId)
            .HasDatabaseName("IX_PrerequisiteRules_CourseId");

        builder.HasIndex(pr => pr.IsActive)
            .HasDatabaseName("IX_PrerequisiteRules_IsActive");

        builder.HasIndex(pr => pr.Priority)
            .HasDatabaseName("IX_PrerequisiteRules_Priority");

        builder.HasIndex(pr => new { pr.CourseId, pr.IsActive, pr.Priority })
            .HasDatabaseName("IX_PrerequisiteRules_Course_Active_Priority");

        // Relationships
        builder.HasOne(pr => pr.Course)
            .WithMany()
            .HasForeignKey(pr => pr.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(pr => pr.Requirements)
            .WithOne(r => r.PrerequisiteRule)
            .HasForeignKey(r => r.PrerequisiteRuleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(pr => pr.NestedRules)
            .WithOne()
            .HasForeignKey("ParentRuleId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(pr => pr.ValidationHistory)
            .WithOne(vh => vh.PrerequisiteRule)
            .HasForeignKey(vh => vh.PrerequisiteRuleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>
/// Entity Framework configuration for PrerequisiteRequirement entity.
/// </summary>
public class PrerequisiteRequirementConfiguration : IEntityTypeConfiguration<PrerequisiteRequirement>
{
    public void Configure(EntityTypeBuilder<PrerequisiteRequirement> builder)
    {
        builder.ToTable("PrerequisiteRequirements");

        builder.HasKey(pr => pr.Id);

        builder.Property(pr => pr.RequirementType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(pr => pr.IsRequired)
            .HasDefaultValue(true);

        builder.Property(pr => pr.SequenceOrder)
            .HasDefaultValue(1);

        builder.Property(pr => pr.MinimumGrade)
            .HasMaxLength(5);

        builder.Property(pr => pr.MustBeCompleted)
            .HasDefaultValue(true);

        builder.Property(pr => pr.CreditHoursSubjectArea)
            .HasMaxLength(10);

        builder.Property(pr => pr.RequiredClassStanding)
            .HasConversion<int>();

        builder.Property(pr => pr.MinimumGPA)
            .HasColumnType("decimal(4,3)");

        builder.Property(pr => pr.GPAScope)
            .HasMaxLength(50);

        builder.Property(pr => pr.RequiredPermission)
            .HasMaxLength(100);

        builder.Property(pr => pr.PermissionLevel)
            .HasConversion<int>();

        builder.Property(pr => pr.TestName)
            .HasMaxLength(100);

        builder.Property(pr => pr.AlternativeSatisfactionMethods)
            .HasMaxLength(1000);

        builder.Property(pr => pr.RequirementNotes)
            .HasMaxLength(500);

        // Indexes
        builder.HasIndex(pr => pr.PrerequisiteRuleId)
            .HasDatabaseName("IX_PrerequisiteRequirements_RuleId");

        builder.HasIndex(pr => pr.RequirementType)
            .HasDatabaseName("IX_PrerequisiteRequirements_Type");

        builder.HasIndex(pr => pr.RequiredCourseId)
            .HasDatabaseName("IX_PrerequisiteRequirements_RequiredCourseId");

        builder.HasIndex(pr => new { pr.PrerequisiteRuleId, pr.SequenceOrder })
            .HasDatabaseName("IX_PrerequisiteRequirements_Rule_Sequence");

        // Relationships
        builder.HasOne(pr => pr.PrerequisiteRule)
            .WithMany(r => r.Requirements)
            .HasForeignKey(pr => pr.PrerequisiteRuleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pr => pr.RequiredCourse)
            .WithMany()
            .HasForeignKey(pr => pr.RequiredCourseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

/// <summary>
/// Entity Framework configuration for CorequisiteRule entity.
/// </summary>
public class CorequisiteRuleConfiguration : IEntityTypeConfiguration<CorequisiteRule>
{
    public void Configure(EntityTypeBuilder<CorequisiteRule> builder)
    {
        builder.ToTable("CorequisiteRules");

        builder.HasKey(cr => cr.Id);

        builder.Property(cr => cr.RuleName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(cr => cr.Description)
            .HasMaxLength(1000);

        builder.Property(cr => cr.EnforcementType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(cr => cr.IsActive)
            .HasDefaultValue(true);

        // Indexes
        builder.HasIndex(cr => cr.CourseId)
            .HasDatabaseName("IX_CorequisiteRules_CourseId");

        builder.HasIndex(cr => cr.IsActive)
            .HasDatabaseName("IX_CorequisiteRules_IsActive");

        builder.HasIndex(cr => new { cr.CourseId, cr.IsActive })
            .HasDatabaseName("IX_CorequisiteRules_Course_Active");

        // Relationships
        builder.HasOne(cr => cr.Course)
            .WithMany()
            .HasForeignKey(cr => cr.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(cr => cr.CorequisiteRequirements)
            .WithOne(r => r.CorequisiteRule)
            .HasForeignKey(r => r.CorequisiteRuleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>
/// Entity Framework configuration for CorequisiteRequirement entity.
/// </summary>
public class CorequisiteRequirementConfiguration : IEntityTypeConfiguration<CorequisiteRequirement>
{
    public void Configure(EntityTypeBuilder<CorequisiteRequirement> builder)
    {
        builder.ToTable("CorequisiteRequirements");

        builder.HasKey(cr => cr.Id);

        builder.Property(cr => cr.EnrollmentRelationship)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(cr => cr.IsWaivable)
            .HasDefaultValue(false);

        builder.Property(cr => cr.WaiverRequiredPermission)
            .HasMaxLength(100);

        builder.Property(cr => cr.FailureAction)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(cr => cr.RequirementNotes)
            .HasMaxLength(500);

        // Indexes
        builder.HasIndex(cr => cr.CorequisiteRuleId)
            .HasDatabaseName("IX_CorequisiteRequirements_RuleId");

        builder.HasIndex(cr => cr.RequiredCourseId)
            .HasDatabaseName("IX_CorequisiteRequirements_RequiredCourseId");

        builder.HasIndex(cr => new { cr.CorequisiteRuleId, cr.RequiredCourseId })
            .HasDatabaseName("IX_CorequisiteRequirements_Rule_Course");

        // Relationships
        builder.HasOne(cr => cr.CorequisiteRule)
            .WithMany(r => r.CorequisiteRequirements)
            .HasForeignKey(cr => cr.CorequisiteRuleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cr => cr.RequiredCourse)
            .WithMany()
            .HasForeignKey(cr => cr.RequiredCourseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

/// <summary>
/// Entity Framework configuration for EnrollmentRestriction entity.
/// </summary>
public class EnrollmentRestrictionConfiguration : IEntityTypeConfiguration<EnrollmentRestriction>
{
    public void Configure(EntityTypeBuilder<EnrollmentRestriction> builder)
    {
        builder.ToTable("EnrollmentRestrictions");

        builder.HasKey(er => er.Id);

        builder.Property(er => er.RestrictionType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(er => er.IsActive)
            .HasDefaultValue(true);

        builder.Property(er => er.Priority)
            .HasDefaultValue(1);

        builder.Property(er => er.EnforcementLevel)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(er => er.RestrictionName)
            .HasMaxLength(200);

        builder.Property(er => er.Description)
            .HasMaxLength(1000);

        builder.Property(er => er.ViolationMessage)
            .HasMaxLength(500);

        // Indexes
        builder.HasIndex(er => er.CourseId)
            .HasDatabaseName("IX_EnrollmentRestrictions_CourseId");

        builder.HasIndex(er => er.RestrictionType)
            .HasDatabaseName("IX_EnrollmentRestrictions_Type");

        builder.HasIndex(er => er.IsActive)
            .HasDatabaseName("IX_EnrollmentRestrictions_IsActive");

        builder.HasIndex(er => new { er.CourseId, er.IsActive, er.Priority })
            .HasDatabaseName("IX_EnrollmentRestrictions_Course_Active_Priority");

        // Relationships
        builder.HasOne(er => er.Course)
            .WithMany()
            .HasForeignKey(er => er.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(er => er.MajorRestrictions)
            .WithOne(mr => mr.EnrollmentRestriction)
            .HasForeignKey(mr => mr.EnrollmentRestrictionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(er => er.ClassStandingRestrictions)
            .WithOne(csr => csr.EnrollmentRestriction)
            .HasForeignKey(csr => csr.EnrollmentRestrictionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(er => er.PermissionRestrictions)
            .WithOne(pr => pr.EnrollmentRestriction)
            .HasForeignKey(pr => pr.EnrollmentRestrictionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>
/// Entity Framework configuration for MajorRestriction entity.
/// </summary>
public class MajorRestrictionConfiguration : IEntityTypeConfiguration<MajorRestriction>
{
    public void Configure(EntityTypeBuilder<MajorRestriction> builder)
    {
        builder.ToTable("MajorRestrictions");

        builder.HasKey(mr => mr.Id);

        builder.Property(mr => mr.RequiredMajorCode)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(mr => mr.MajorType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(mr => mr.IsIncluded)
            .HasDefaultValue(true);

        builder.Property(mr => mr.MinimumMajorProgress)
            .HasColumnType("decimal(5,2)");

        // Indexes
        builder.HasIndex(mr => mr.EnrollmentRestrictionId)
            .HasDatabaseName("IX_MajorRestrictions_RestrictionId");

        builder.HasIndex(mr => mr.RequiredMajorCode)
            .HasDatabaseName("IX_MajorRestrictions_MajorCode");

        builder.HasIndex(mr => new { mr.EnrollmentRestrictionId, mr.RequiredMajorCode })
            .HasDatabaseName("IX_MajorRestrictions_Restriction_Major");

        // Relationships
        builder.HasOne(mr => mr.EnrollmentRestriction)
            .WithMany(er => er.MajorRestrictions)
            .HasForeignKey(mr => mr.EnrollmentRestrictionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>
/// Entity Framework configuration for ClassStandingRestriction entity.
/// </summary>
public class ClassStandingRestrictionConfiguration : IEntityTypeConfiguration<ClassStandingRestriction>
{
    public void Configure(EntityTypeBuilder<ClassStandingRestriction> builder)
    {
        builder.ToTable("ClassStandingRestrictions");

        builder.HasKey(csr => csr.Id);

        builder.Property(csr => csr.RequiredClassStanding)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(csr => csr.IsIncluded)
            .HasDefaultValue(true);

        // Indexes
        builder.HasIndex(csr => csr.EnrollmentRestrictionId)
            .HasDatabaseName("IX_ClassStandingRestrictions_RestrictionId");

        builder.HasIndex(csr => csr.RequiredClassStanding)
            .HasDatabaseName("IX_ClassStandingRestrictions_ClassStanding");

        // Relationships
        builder.HasOne(csr => csr.EnrollmentRestriction)
            .WithMany(er => er.ClassStandingRestrictions)
            .HasForeignKey(csr => csr.EnrollmentRestrictionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>
/// Entity Framework configuration for PermissionRestriction entity.
/// </summary>
public class PermissionRestrictionConfiguration : IEntityTypeConfiguration<PermissionRestriction>
{
    public void Configure(EntityTypeBuilder<PermissionRestriction> builder)
    {
        builder.ToTable("PermissionRestrictions");

        builder.HasKey(pr => pr.Id);

        builder.Property(pr => pr.RequiredPermission)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(pr => pr.PermissionLevel)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(pr => pr.RequiresDocumentation)
            .HasDefaultValue(false);

        builder.Property(pr => pr.DocumentationRequirements)
            .HasMaxLength(500);

        // Indexes
        builder.HasIndex(pr => pr.EnrollmentRestrictionId)
            .HasDatabaseName("IX_PermissionRestrictions_RestrictionId");

        builder.HasIndex(pr => pr.RequiredPermission)
            .HasDatabaseName("IX_PermissionRestrictions_Permission");

        // Relationships
        builder.HasOne(pr => pr.EnrollmentRestriction)
            .WithMany(er => er.PermissionRestrictions)
            .HasForeignKey(pr => pr.EnrollmentRestrictionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>
/// Entity Framework configuration for RuleValidationHistory entity.
/// </summary>
public class RuleValidationHistoryConfiguration : IEntityTypeConfiguration<RuleValidationHistory>
{
    public void Configure(EntityTypeBuilder<RuleValidationHistory> builder)
    {
        builder.ToTable("RuleValidationHistory");

        builder.HasKey(rvh => rvh.Id);

        builder.Property(rvh => rvh.ValidationDate)
            .IsRequired();

        builder.Property(rvh => rvh.ValidatedBy)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(rvh => rvh.ValidationErrors)
            .HasMaxLength(2000);

        builder.Property(rvh => rvh.ValidationDetails)
            .HasMaxLength(4000);

        // Indexes
        builder.HasIndex(rvh => rvh.PrerequisiteRuleId)
            .HasDatabaseName("IX_RuleValidationHistory_RuleId");

        builder.HasIndex(rvh => rvh.ValidationDate)
            .HasDatabaseName("IX_RuleValidationHistory_ValidationDate");

        builder.HasIndex(rvh => new { rvh.PrerequisiteRuleId, rvh.ValidationDate })
            .HasDatabaseName("IX_RuleValidationHistory_Rule_Date");

        // Relationships
        builder.HasOne(rvh => rvh.PrerequisiteRule)
            .WithMany(pr => pr.ValidationHistory)
            .HasForeignKey(rvh => rvh.PrerequisiteRuleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}