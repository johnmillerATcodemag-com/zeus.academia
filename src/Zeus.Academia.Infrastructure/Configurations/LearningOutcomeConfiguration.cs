using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Enums;

namespace Zeus.Academia.Infrastructure.Configurations;

/// <summary>
/// Entity Framework configuration for LearningOutcome entity.
/// </summary>
public class LearningOutcomeConfiguration : IEntityTypeConfiguration<LearningOutcome>
{
    public void Configure(EntityTypeBuilder<LearningOutcome> builder)
    {
        builder.ToTable("LearningOutcomes");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.OutcomeText)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(o => o.Category)
            .HasMaxLength(100);

        builder.Property(o => o.Version)
            .HasMaxLength(10);

        builder.Property(o => o.BloomsTaxonomyLevel)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(o => o.DifficultyLevel)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(o => o.Weight)
            .HasColumnType("decimal(5,4)");

        // Convert lists to JSON
        builder.Property(o => o.AssessmentMethods)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null!) ?? new List<string>(),
                new ValueComparer<List<string>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()))
            .HasColumnType("nvarchar(max)");

        builder.Property(o => o.ProgramOutcomeAlignment)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null!) ?? new List<string>(),
                new ValueComparer<List<string>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()))
            .HasColumnType("nvarchar(max)");

        builder.Property(o => o.PrerequisiteOutcomeIds)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<List<int>>(v, (JsonSerializerOptions)null!) ?? new List<int>(),
                new ValueComparer<List<int>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()))
            .HasColumnType("nvarchar(max)");

        // Relationships
        builder.HasOne(o => o.Course)
            .WithMany()
            .HasForeignKey(o => o.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(o => o.Subject)
            .WithMany()
            .HasForeignKey(o => o.SubjectCode)
            .OnDelete(DeleteBehavior.Cascade);

        // Many-to-many for prerequisite outcomes (self-referencing)
        builder.HasMany(o => o.PrerequisiteOutcomes)
            .WithMany(o => o.DependentOutcomes)
            .UsingEntity<Dictionary<string, object>>(
                "LearningOutcomePrerequisites",
                j => j.HasOne<LearningOutcome>().WithMany().HasForeignKey("PrerequisiteId"),
                j => j.HasOne<LearningOutcome>().WithMany().HasForeignKey("DependentId"),
                j =>
                {
                    j.HasKey("PrerequisiteId", "DependentId");
                    j.ToTable("LearningOutcomePrerequisites");
                });

        // Indexes
        builder.HasIndex(o => o.CourseId)
            .HasDatabaseName("IX_LearningOutcomes_CourseId");

        builder.HasIndex(o => o.SubjectCode)
            .HasDatabaseName("IX_LearningOutcomes_SubjectCode");

        builder.HasIndex(o => o.BloomsTaxonomyLevel)
            .HasDatabaseName("IX_LearningOutcomes_BloomsTaxonomyLevel");

        builder.HasIndex(o => o.DifficultyLevel)
            .HasDatabaseName("IX_LearningOutcomes_DifficultyLevel");

        builder.HasIndex(o => o.IsActive)
            .HasDatabaseName("IX_LearningOutcomes_IsActive");

        // Check constraints
        builder.ToTable("LearningOutcomes", t =>
        {
            t.HasCheckConstraint("CK_LearningOutcomes_ExpectedMasteryLevel", "[ExpectedMasteryLevel] >= 0 AND [ExpectedMasteryLevel] <= 100");
            t.HasCheckConstraint("CK_LearningOutcomes_Weight", "[Weight] >= 0 AND [Weight] <= 1");
        });
    }
}

/// <summary>
/// Entity Framework configuration for OutcomeAssessment entity.
/// </summary>
public class OutcomeAssessmentConfiguration : IEntityTypeConfiguration<OutcomeAssessment>
{
    public void Configure(EntityTypeBuilder<OutcomeAssessment> builder)
    {
        builder.ToTable("OutcomeAssessments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.AssessmentName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.AssessmentType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.Description)
            .HasMaxLength(1000);

        builder.Property(a => a.AssessmentMethod)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.ScoringScale)
            .HasMaxLength(100);

        builder.Property(a => a.Weight)
            .HasColumnType("decimal(5,4)");

        builder.Property(a => a.MinimumPassingScore)
            .HasColumnType("decimal(5,2)");

        builder.Property(a => a.TargetScore)
            .HasColumnType("decimal(5,2)");

        // Convert lists to JSON
        builder.Property(a => a.RubricCriteria)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null!) ?? new List<string>(),
                new ValueComparer<List<string>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()))
            .HasColumnType("nvarchar(max)");

        // Relationships
        builder.HasOne(a => a.LearningOutcome)
            .WithMany(o => o.Assessments)
            .HasForeignKey(a => a.LearningOutcomeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(a => a.LearningOutcomeId)
            .HasDatabaseName("IX_OutcomeAssessments_LearningOutcomeId");

        builder.HasIndex(a => a.AssessmentType)
            .HasDatabaseName("IX_OutcomeAssessments_AssessmentType");

        builder.HasIndex(a => a.IsActive)
            .HasDatabaseName("IX_OutcomeAssessments_IsActive");

        // Check constraints
        builder.ToTable("OutcomeAssessments", t =>
        {
            t.HasCheckConstraint("CK_OutcomeAssessments_Weight", "[Weight] >= 0 AND [Weight] <= 1");
        });
    }
}

/// <summary>
/// Entity Framework configuration for AssessmentResult entity.
/// </summary>
public class AssessmentResultConfiguration : IEntityTypeConfiguration<AssessmentResult>
{
    public void Configure(EntityTypeBuilder<AssessmentResult> builder)
    {
        builder.ToTable("AssessmentResults");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.AssessedEntity)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.AssessedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.Comments)
            .HasMaxLength(1000);

        builder.Property(r => r.Score)
            .HasColumnType("decimal(10,4)")
            .IsRequired();

        builder.Property(r => r.MaxScore)
            .HasColumnType("decimal(10,4)")
            .IsRequired();

        // Configure percentage score column
        builder.Property(r => r.PercentageScore)
            .HasColumnType("decimal(7,4)");

        // Convert dictionary to JSON
        builder.Property(r => r.DetailedScores)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<Dictionary<string, decimal>>(v, (JsonSerializerOptions)null!) ?? new Dictionary<string, decimal>(),
                new ValueComparer<Dictionary<string, decimal>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)))
            .HasColumnType("nvarchar(max)");

        // Relationships
        builder.HasOne(r => r.OutcomeAssessment)
            .WithMany(a => a.Results)
            .HasForeignKey(r => r.OutcomeAssessmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(r => r.OutcomeAssessmentId)
            .HasDatabaseName("IX_AssessmentResults_OutcomeAssessmentId");

        builder.HasIndex(r => r.AssessmentDate)
            .HasDatabaseName("IX_AssessmentResults_AssessmentDate");

        builder.HasIndex(r => r.PassesCriteria)
            .HasDatabaseName("IX_AssessmentResults_PassesCriteria");

        builder.HasIndex(r => r.MeetsTarget)
            .HasDatabaseName("IX_AssessmentResults_MeetsTarget");

        // Check constraints
        builder.ToTable("AssessmentResults", t =>
        {
            t.HasCheckConstraint("CK_AssessmentResults_Score", "[Score] >= 0");
            t.HasCheckConstraint("CK_AssessmentResults_MaxScore", "[MaxScore] > 0");
        });
    }
}