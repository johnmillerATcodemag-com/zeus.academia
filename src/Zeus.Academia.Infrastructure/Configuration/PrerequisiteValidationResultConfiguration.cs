using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.Infrastructure.Configuration;

/// <summary>
/// Entity Framework configuration for PrerequisiteValidationResult entity.
/// </summary>
public class PrerequisiteValidationResultConfiguration : IEntityTypeConfiguration<PrerequisiteValidationResult>
{
    public void Configure(EntityTypeBuilder<PrerequisiteValidationResult> builder)
    {
        builder.ToTable("PrerequisiteValidationResults");
        
        builder.HasKey(pvr => pvr.Id);
        
        builder.Property(pvr => pvr.ValidationDate)
            .IsRequired();
            
        builder.Property(pvr => pvr.OverallStatus)
            .IsRequired()
            .HasConversion<int>();
            
        builder.Property(pvr => pvr.FailureReason)
            .HasMaxLength(500);
            
        builder.Property(pvr => pvr.ValidationNotes)
            .HasMaxLength(2000);
            
        builder.Property(pvr => pvr.ValidationEngineVersion)
            .HasMaxLength(20);
            
        builder.Property(pvr => pvr.IsCurrent)
            .HasDefaultValue(true);
            
        builder.Property(pvr => pvr.ValidationMetadata)
            .HasMaxLength(1000);
        
        // Indexes
        builder.HasIndex(pvr => pvr.StudentId)
            .HasDatabaseName("IX_PrerequisiteValidationResults_StudentId");
            
        builder.HasIndex(pvr => pvr.CourseId)
            .HasDatabaseName("IX_PrerequisiteValidationResults_CourseId");
            
        builder.HasIndex(pvr => pvr.AcademicTermId)
            .HasDatabaseName("IX_PrerequisiteValidationResults_AcademicTermId");
            
        builder.HasIndex(pvr => pvr.ValidationDate)
            .HasDatabaseName("IX_PrerequisiteValidationResults_ValidationDate");
            
        builder.HasIndex(pvr => pvr.OverallStatus)
            .HasDatabaseName("IX_PrerequisiteValidationResults_Status");
            
        builder.HasIndex(pvr => pvr.IsCurrent)
            .HasDatabaseName("IX_PrerequisiteValidationResults_IsCurrent");
            
        builder.HasIndex(pvr => new { pvr.StudentId, pvr.CourseId, pvr.AcademicTermId })
            .HasDatabaseName("IX_PrerequisiteValidationResults_Student_Course_Term");
            
        builder.HasIndex(pvr => new { pvr.StudentId, pvr.IsCurrent })
            .HasDatabaseName("IX_PrerequisiteValidationResults_Student_Current");
        
        // Relationships
        builder.HasOne(pvr => pvr.Student)
            .WithMany()
            .HasForeignKey(pvr => pvr.StudentId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(pvr => pvr.Course)
            .WithMany()
            .HasForeignKey(pvr => pvr.CourseId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(pvr => pvr.CourseOffering)
            .WithMany()
            .HasForeignKey(pvr => pvr.CourseOfferingId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(pvr => pvr.AcademicTerm)
            .WithMany()
            .HasForeignKey(pvr => pvr.AcademicTermId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasMany(pvr => pvr.PrerequisiteResults)
            .WithOne(pcr => pcr.ValidationResult)
            .HasForeignKey(pcr => pcr.ValidationResultId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(pvr => pvr.CorequisiteResults)
            .WithOne(ccr => ccr.ValidationResult)
            .HasForeignKey(ccr => ccr.ValidationResultId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(pvr => pvr.RestrictionResults)
            .WithOne(rcr => rcr.ValidationResult)
            .HasForeignKey(rcr => rcr.ValidationResultId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(pvr => pvr.AppliedOverrides)
            .WithMany()
            .UsingEntity("ValidationResultOverrides");
            
        builder.HasMany(pvr => pvr.AppliedWaivers)
            .WithMany()
            .UsingEntity("ValidationResultWaivers");
    }
}

/// <summary>
/// Entity Framework configuration for PrerequisiteCheckResult entity.
/// </summary>
public class PrerequisiteCheckResultConfiguration : IEntityTypeConfiguration<PrerequisiteCheckResult>
{
    public void Configure(EntityTypeBuilder<PrerequisiteCheckResult> builder)
    {
        builder.ToTable("PrerequisiteCheckResults");
        
        builder.HasKey(pcr => pcr.Id);
        
        builder.Property(pcr => pcr.CheckStatus)
            .IsRequired()
            .HasConversion<int>();
            
        builder.Property(pcr => pcr.FailureReason)
            .HasMaxLength(500);
            
        builder.Property(pcr => pcr.SatisfactionMethod)
            .HasMaxLength(200);
            
        builder.Property(pcr => pcr.SatisfactionPercentage)
            .HasColumnType("decimal(5,2)");
            
        builder.Property(pcr => pcr.SatisfyingCourses)
            .HasMaxLength(500);
            
        builder.Property(pcr => pcr.SatisfyingGrades)
            .HasMaxLength(200);
            
        builder.Property(pcr => pcr.CheckDetails)
            .HasMaxLength(1000);
            
        builder.Property(pcr => pcr.AlternativeMethods)
            .HasMaxLength(1000);
        
        // Indexes
        builder.HasIndex(pcr => pcr.ValidationResultId)
            .HasDatabaseName("IX_PrerequisiteCheckResults_ValidationResultId");
            
        builder.HasIndex(pcr => pcr.PrerequisiteRuleId)
            .HasDatabaseName("IX_PrerequisiteCheckResults_RuleId");
            
        builder.HasIndex(pcr => pcr.CheckStatus)
            .HasDatabaseName("IX_PrerequisiteCheckResults_Status");
            
        builder.HasIndex(pcr => pcr.IsSatisfied)
            .HasDatabaseName("IX_PrerequisiteCheckResults_IsSatisfied");
        
        // Relationships
        builder.HasOne(pcr => pcr.ValidationResult)
            .WithMany(vr => vr.PrerequisiteResults)
            .HasForeignKey(pcr => pcr.ValidationResultId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(pcr => pcr.PrerequisiteRule)
            .WithMany()
            .HasForeignKey(pcr => pcr.PrerequisiteRuleId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasMany(pcr => pcr.RequirementResults)
            .WithOne(rcr => rcr.PrerequisiteCheckResult)
            .HasForeignKey(rcr => rcr.PrerequisiteCheckResultId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>
/// Entity Framework configuration for RequirementCheckResult entity.
/// </summary>
public class RequirementCheckResultConfiguration : IEntityTypeConfiguration<RequirementCheckResult>
{
    public void Configure(EntityTypeBuilder<RequirementCheckResult> builder)
    {
        builder.ToTable("RequirementCheckResults");
        
        builder.HasKey(rcr => rcr.Id);
        
        builder.Property(rcr => rcr.FailureReason)
            .HasMaxLength(500);
            
        builder.Property(rcr => rcr.ActualValue)
            .HasMaxLength(100);
            
        builder.Property(rcr => rcr.RequiredValue)
            .HasMaxLength(100);
            
        builder.Property(rcr => rcr.CheckDetails)
            .HasMaxLength(1000);
        
        // Indexes
        builder.HasIndex(rcr => rcr.PrerequisiteCheckResultId)
            .HasDatabaseName("IX_RequirementCheckResults_CheckResultId");
            
        builder.HasIndex(rcr => rcr.PrerequisiteRequirementId)
            .HasDatabaseName("IX_RequirementCheckResults_RequirementId");
            
        builder.HasIndex(rcr => rcr.IsSatisfied)
            .HasDatabaseName("IX_RequirementCheckResults_IsSatisfied");
        
        // Relationships
        builder.HasOne(rcr => rcr.PrerequisiteCheckResult)
            .WithMany(pcr => pcr.RequirementResults)
            .HasForeignKey(rcr => rcr.PrerequisiteCheckResultId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(rcr => rcr.PrerequisiteRequirement)
            .WithMany()
            .HasForeignKey(rcr => rcr.PrerequisiteRequirementId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

/// <summary>
/// Entity Framework configuration for CorequisiteCheckResult entity.
/// </summary>
public class CorequisiteCheckResultConfiguration : IEntityTypeConfiguration<CorequisiteCheckResult>
{
    public void Configure(EntityTypeBuilder<CorequisiteCheckResult> builder)
    {
        builder.ToTable("CorequisiteCheckResults");
        
        builder.HasKey(ccr => ccr.Id);
        
        builder.Property(ccr => ccr.CheckStatus)
            .IsRequired()
            .HasConversion<int>();
            
        builder.Property(ccr => ccr.FailureReason)
            .HasMaxLength(500);
            
        builder.Property(ccr => ccr.EnforcementAction)
            .HasMaxLength(200);
            
        builder.Property(ccr => ccr.RequiredCorequisites)
            .HasMaxLength(500);
            
        builder.Property(ccr => ccr.EnrolledCorequisites)
            .HasMaxLength(500);
            
        builder.Property(ccr => ccr.CheckDetails)
            .HasMaxLength(1000);
        
        // Indexes
        builder.HasIndex(ccr => ccr.ValidationResultId)
            .HasDatabaseName("IX_CorequisiteCheckResults_ValidationResultId");
            
        builder.HasIndex(ccr => ccr.CorequisiteRuleId)
            .HasDatabaseName("IX_CorequisiteCheckResults_RuleId");
            
        builder.HasIndex(ccr => ccr.CheckStatus)
            .HasDatabaseName("IX_CorequisiteCheckResults_Status");
            
        builder.HasIndex(ccr => ccr.IsSatisfied)
            .HasDatabaseName("IX_CorequisiteCheckResults_IsSatisfied");
        
        // Relationships
        builder.HasOne(ccr => ccr.ValidationResult)
            .WithMany(vr => vr.CorequisiteResults)
            .HasForeignKey(ccr => ccr.ValidationResultId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(ccr => ccr.CorequisiteRule)
            .WithMany()
            .HasForeignKey(ccr => ccr.CorequisiteRuleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

/// <summary>
/// Entity Framework configuration for RestrictionCheckResult entity.
/// </summary>
public class RestrictionCheckResultConfiguration : IEntityTypeConfiguration<RestrictionCheckResult>
{
    public void Configure(EntityTypeBuilder<RestrictionCheckResult> builder)
    {
        builder.ToTable("RestrictionCheckResults");
        
        builder.HasKey(rcr => rcr.Id);
        
        builder.Property(rcr => rcr.CheckStatus)
            .IsRequired()
            .HasConversion<int>();
            
        builder.Property(rcr => rcr.ViolationReason)
            .HasMaxLength(500);
            
        builder.Property(rcr => rcr.EnforcementAction)
            .HasMaxLength(200);
            
        builder.Property(rcr => rcr.ViolationSeverity)
            .IsRequired()
            .HasConversion<int>();
            
        builder.Property(rcr => rcr.OverridePermissionRequired)
            .HasMaxLength(100);
            
        builder.Property(rcr => rcr.CheckDetails)
            .HasMaxLength(1000);
        
        // Indexes
        builder.HasIndex(rcr => rcr.ValidationResultId)
            .HasDatabaseName("IX_RestrictionCheckResults_ValidationResultId");
            
        builder.HasIndex(rcr => rcr.EnrollmentRestrictionId)
            .HasDatabaseName("IX_RestrictionCheckResults_RestrictionId");
            
        builder.HasIndex(rcr => rcr.CheckStatus)
            .HasDatabaseName("IX_RestrictionCheckResults_Status");
            
        builder.HasIndex(rcr => rcr.IsViolated)
            .HasDatabaseName("IX_RestrictionCheckResults_IsViolated");
            
        builder.HasIndex(rcr => rcr.ViolationSeverity)
            .HasDatabaseName("IX_RestrictionCheckResults_Severity");
        
        // Relationships
        builder.HasOne(rcr => rcr.ValidationResult)
            .WithMany(vr => vr.RestrictionResults)
            .HasForeignKey(rcr => rcr.ValidationResultId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(rcr => rcr.EnrollmentRestriction)
            .WithMany()
            .HasForeignKey(rcr => rcr.EnrollmentRestrictionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

/// <summary>
/// Entity Framework configuration for CircularDependencyResult entity.
/// </summary>
public class CircularDependencyResultConfiguration : IEntityTypeConfiguration<CircularDependencyResult>
{
    public void Configure(EntityTypeBuilder<CircularDependencyResult> builder)
    {
        builder.ToTable("CircularDependencyResults");
        
        builder.HasKey(cdr => cdr.Id);
        
        builder.Property(cdr => cdr.DetectionDate)
            .IsRequired();
            
        builder.Property(cdr => cdr.DependencyPath)
            .HasMaxLength(1000);
            
        builder.Property(cdr => cdr.InvolvedCourses)
            .HasMaxLength(500);
            
        builder.Property(cdr => cdr.Severity)
            .HasConversion<int>();
            
        builder.Property(cdr => cdr.ResolutionRecommendations)
            .HasMaxLength(2000);
            
        builder.Property(cdr => cdr.IsResolved)
            .HasDefaultValue(false);
            
        builder.Property(cdr => cdr.DetectionDetails)
            .HasMaxLength(1000);
        
        // Indexes
        builder.HasIndex(cdr => cdr.CourseId)
            .HasDatabaseName("IX_CircularDependencyResults_CourseId");
            
        builder.HasIndex(cdr => cdr.DetectionDate)
            .HasDatabaseName("IX_CircularDependencyResults_DetectionDate");
            
        builder.HasIndex(cdr => cdr.HasCircularDependency)
            .HasDatabaseName("IX_CircularDependencyResults_HasDependency");
            
        builder.HasIndex(cdr => cdr.IsResolved)
            .HasDatabaseName("IX_CircularDependencyResults_IsResolved");
            
        builder.HasIndex(cdr => new { cdr.CourseId, cdr.HasCircularDependency, cdr.IsResolved })
            .HasDatabaseName("IX_CircularDependencyResults_Course_Dependency_Resolved");
        
        // Relationships
        builder.HasOne(cdr => cdr.Course)
            .WithMany()
            .HasForeignKey(cdr => cdr.CourseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}