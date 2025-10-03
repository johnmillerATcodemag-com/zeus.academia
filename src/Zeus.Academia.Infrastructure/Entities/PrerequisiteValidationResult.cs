using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Zeus.Academia.Infrastructure.Enums;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing the result of a prerequisite validation check.
/// </summary>
public class PrerequisiteValidationResult : BaseEntity
{
    /// <summary>
    /// Gets or sets the student ID this validation was performed for.
    /// </summary>
    [Required]
    public int StudentId { get; set; }

    /// <summary>
    /// Gets or sets the course ID being validated for enrollment.
    /// </summary>
    [Required]
    public int CourseId { get; set; }

    /// <summary>
    /// Gets or sets the course offering ID if validation is for a specific offering.
    /// </summary>
    public int? CourseOfferingId { get; set; }

    /// <summary>
    /// Gets or sets the academic term ID for which validation is being performed.
    /// </summary>
    [Required]
    public int AcademicTermId { get; set; }

    /// <summary>
    /// Gets or sets when this validation was performed.
    /// </summary>
    [Required]
    public DateTime ValidationDate { get; set; }

    /// <summary>
    /// Gets or sets the overall validation status.
    /// </summary>
    [Required]
    public ValidationStatus OverallStatus { get; set; }

    /// <summary>
    /// Gets or sets whether the student can enroll based on this validation.
    /// </summary>
    public bool CanEnroll { get; set; }

    /// <summary>
    /// Gets or sets the primary reason for validation failure if applicable.
    /// </summary>
    [StringLength(500)]
    public string? FailureReason { get; set; }

    /// <summary>
    /// Gets or sets additional validation notes.
    /// </summary>
    [StringLength(2000)]
    public string? ValidationNotes { get; set; }

    /// <summary>
    /// Gets or sets the validation engine version used.
    /// </summary>
    [StringLength(20)]
    public string? ValidationEngineVersion { get; set; }

    /// <summary>
    /// Gets or sets whether this validation result is still current.
    /// </summary>
    public bool IsCurrent { get; set; } = true;

    /// <summary>
    /// Gets or sets when this validation result expires.
    /// </summary>
    public DateTime? ExpirationDate { get; set; }

    /// <summary>
    /// Gets or sets the validation processing time in milliseconds.
    /// </summary>
    public int? ProcessingTimeMs { get; set; }

    /// <summary>
    /// Gets or sets metadata about the validation context.
    /// </summary>
    [StringLength(1000)]
    public string? ValidationMetadata { get; set; }

    // Navigation properties
    /// <summary>
    /// Gets or sets the student this validation applies to.
    /// </summary>
    [ForeignKey(nameof(StudentId))]
    public Student Student { get; set; } = null!;

    /// <summary>
    /// Gets or sets the course being validated for enrollment.
    /// </summary>
    [ForeignKey(nameof(CourseId))]
    public Course Course { get; set; } = null!;

    /// <summary>
    /// Gets or sets the specific course offering if applicable.
    /// </summary>
    [ForeignKey(nameof(CourseOfferingId))]
    public CourseOffering? CourseOffering { get; set; }

    /// <summary>
    /// Gets or sets the academic term for enrollment.
    /// </summary>
    [ForeignKey(nameof(AcademicTermId))]
    public AcademicTerm AcademicTerm { get; set; } = null!;

    /// <summary>
    /// Gets or sets the detailed prerequisite check results.
    /// </summary>
    public List<PrerequisiteCheckResult> PrerequisiteResults { get; set; } = new();

    /// <summary>
    /// Gets or sets the detailed corequisite check results.
    /// </summary>
    public List<CorequisiteCheckResult> CorequisiteResults { get; set; } = new();

    /// <summary>
    /// Gets or sets the detailed restriction check results.
    /// </summary>
    public List<RestrictionCheckResult> RestrictionResults { get; set; } = new();

    /// <summary>
    /// Gets or sets any overrides applied to this validation.
    /// </summary>
    public List<PrerequisiteOverride> AppliedOverrides { get; set; } = new();

    /// <summary>
    /// Gets or sets any waivers applied to this validation.
    /// </summary>
    public List<PrerequisiteWaiver> AppliedWaivers { get; set; } = new();

    /// <summary>
    /// Gets or sets whether the validation passed.
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Gets or sets detailed requirement results.
    /// </summary>
    public string? RequirementResults { get; set; }

    /// <summary>
    /// Gets or sets missing requirements details.
    /// </summary>
    public string? MissingRequirements { get; set; }

    /// <summary>
    /// Gets or sets logic evaluation results for complex prerequisite chains.
    /// </summary>
    public string? LogicEvaluationResults { get; set; }
}

/// <summary>
/// Entity representing the result of checking a specific prerequisite rule.
/// </summary>
public class PrerequisiteCheckResult : BaseEntity
{
    /// <summary>
    /// Gets or sets the validation result ID this check belongs to.
    /// </summary>
    [Required]
    public int ValidationResultId { get; set; }

    /// <summary>
    /// Gets or sets the prerequisite rule ID being checked.
    /// </summary>
    [Required]
    public int PrerequisiteRuleId { get; set; }

    /// <summary>
    /// Gets or sets the status of this prerequisite check.
    /// </summary>
    [Required]
    public PrerequisiteCheckStatus CheckStatus { get; set; }

    /// <summary>
    /// Gets or sets whether this prerequisite requirement is satisfied.
    /// </summary>
    public bool IsSatisfied { get; set; }

    /// <summary>
    /// Gets or sets the reason for failure if not satisfied.
    /// </summary>
    [StringLength(500)]
    public string? FailureReason { get; set; }

    /// <summary>
    /// Gets or sets the satisfaction method used.
    /// </summary>
    [StringLength(200)]
    public string? SatisfactionMethod { get; set; }

    /// <summary>
    /// Gets or sets the satisfaction percentage (for partial satisfaction).
    /// </summary>
    [Column(TypeName = "decimal(5,2)")]
    public decimal? SatisfactionPercentage { get; set; }

    /// <summary>
    /// Gets or sets the courses that satisfied this prerequisite.
    /// </summary>
    [StringLength(500)]
    public string? SatisfyingCourses { get; set; }

    /// <summary>
    /// Gets or sets the grades achieved in satisfying courses.
    /// </summary>
    [StringLength(200)]
    public string? SatisfyingGrades { get; set; }

    /// <summary>
    /// Gets or sets when the satisfying requirements were completed.
    /// </summary>
    public DateTime? SatisfactionDate { get; set; }

    /// <summary>
    /// Gets or sets additional details about this check.
    /// </summary>
    [StringLength(1000)]
    public string? CheckDetails { get; set; }

    /// <summary>
    /// Gets or sets the alternative methods available to satisfy this prerequisite.
    /// </summary>
    [StringLength(1000)]
    public string? AlternativeMethods { get; set; }

    // Navigation properties
    /// <summary>
    /// Gets or sets the validation result this check belongs to.
    /// </summary>
    [ForeignKey(nameof(ValidationResultId))]
    public PrerequisiteValidationResult ValidationResult { get; set; } = null!;

    /// <summary>
    /// Gets or sets the prerequisite rule being checked.
    /// </summary>
    [ForeignKey(nameof(PrerequisiteRuleId))]
    public PrerequisiteRule PrerequisiteRule { get; set; } = null!;

    /// <summary>
    /// Gets or sets the individual requirement check results.
    /// </summary>
    public List<RequirementCheckResult> RequirementResults { get; set; } = new();
}

/// <summary>
/// Entity representing the result of checking an individual prerequisite requirement.
/// </summary>
public class RequirementCheckResult : BaseEntity
{
    /// <summary>
    /// Gets or sets the prerequisite check result ID this belongs to.
    /// </summary>
    [Required]
    public int PrerequisiteCheckResultId { get; set; }

    /// <summary>
    /// Gets or sets the prerequisite requirement ID being checked.
    /// </summary>
    [Required]
    public int PrerequisiteRequirementId { get; set; }

    /// <summary>
    /// Gets or sets whether this requirement is satisfied.
    /// </summary>
    public bool IsSatisfied { get; set; }

    /// <summary>
    /// Gets or sets the reason for failure if not satisfied.
    /// </summary>
    [StringLength(500)]
    public string? FailureReason { get; set; }

    /// <summary>
    /// Gets or sets the actual value found (grade, GPA, credit hours, etc.).
    /// </summary>
    [StringLength(100)]
    public string? ActualValue { get; set; }

    /// <summary>
    /// Gets or sets the required value for comparison.
    /// </summary>
    [StringLength(100)]
    public string? RequiredValue { get; set; }

    /// <summary>
    /// Gets or sets additional details about this requirement check.
    /// </summary>
    [StringLength(1000)]
    public string? CheckDetails { get; set; }

    /// <summary>
    /// Gets or sets when this requirement was satisfied (if applicable).
    /// </summary>
    public DateTime? SatisfactionDate { get; set; }

    // Navigation properties
    /// <summary>
    /// Gets or sets the prerequisite check result this belongs to.
    /// </summary>
    [ForeignKey(nameof(PrerequisiteCheckResultId))]
    public PrerequisiteCheckResult PrerequisiteCheckResult { get; set; } = null!;

    /// <summary>
    /// Gets or sets the prerequisite requirement being checked.
    /// </summary>
    [ForeignKey(nameof(PrerequisiteRequirementId))]
    public PrerequisiteRequirement PrerequisiteRequirement { get; set; } = null!;
}

/// <summary>
/// Entity representing the result of checking corequisite requirements.
/// </summary>
public class CorequisiteCheckResult : BaseEntity
{
    /// <summary>
    /// Gets or sets the validation result ID this check belongs to.
    /// </summary>
    [Required]
    public int ValidationResultId { get; set; }

    /// <summary>
    /// Gets or sets the corequisite rule ID being checked.
    /// </summary>
    [Required]
    public int CorequisiteRuleId { get; set; }

    /// <summary>
    /// Gets or sets the status of this corequisite check.
    /// </summary>
    [Required]
    public CorequisiteCheckStatus CheckStatus { get; set; }

    /// <summary>
    /// Gets or sets whether this corequisite requirement is satisfied.
    /// </summary>
    public bool IsSatisfied { get; set; }

    /// <summary>
    /// Gets or sets the reason for failure if not satisfied.
    /// </summary>
    [StringLength(500)]
    public string? FailureReason { get; set; }

    /// <summary>
    /// Gets or sets the enforcement action taken.
    /// </summary>
    [StringLength(200)]
    public string? EnforcementAction { get; set; }

    /// <summary>
    /// Gets or sets the corequisite courses that are required.
    /// </summary>
    [StringLength(500)]
    public string? RequiredCorequisites { get; set; }

    /// <summary>
    /// Gets or sets the corequisite courses that are enrolled.
    /// </summary>
    [StringLength(500)]
    public string? EnrolledCorequisites { get; set; }

    /// <summary>
    /// Gets or sets additional details about this corequisite check.
    /// </summary>
    [StringLength(1000)]
    public string? CheckDetails { get; set; }

    // Navigation properties
    /// <summary>
    /// Gets or sets the validation result this check belongs to.
    /// </summary>
    [ForeignKey(nameof(ValidationResultId))]
    public PrerequisiteValidationResult ValidationResult { get; set; } = null!;

    /// <summary>
    /// Gets or sets the corequisite rule being checked.
    /// </summary>
    [ForeignKey(nameof(CorequisiteRuleId))]
    public CorequisiteRule CorequisiteRule { get; set; } = null!;
}

/// <summary>
/// Entity representing the result of checking enrollment restrictions.
/// </summary>
public class RestrictionCheckResult : BaseEntity
{
    /// <summary>
    /// Gets or sets the validation result ID this check belongs to.
    /// </summary>
    [Required]
    public int ValidationResultId { get; set; }

    /// <summary>
    /// Gets or sets the enrollment restriction ID being checked.
    /// </summary>
    [Required]
    public int EnrollmentRestrictionId { get; set; }

    /// <summary>
    /// Gets or sets the status of this restriction check.
    /// </summary>
    [Required]
    public RestrictionCheckStatus CheckStatus { get; set; }

    /// <summary>
    /// Gets or sets whether this restriction is violated.
    /// </summary>
    public bool IsViolated { get; set; }

    /// <summary>
    /// Gets or sets the reason for the restriction violation.
    /// </summary>
    [StringLength(500)]
    public string? ViolationReason { get; set; }

    /// <summary>
    /// Gets or sets the enforcement action taken for the violation.
    /// </summary>
    [StringLength(200)]
    public string? EnforcementAction { get; set; }

    /// <summary>
    /// Gets or sets the severity level of the restriction violation.
    /// </summary>
    [Required]
    public RestrictionViolationSeverity ViolationSeverity { get; set; }

    /// <summary>
    /// Gets or sets whether the restriction can be overridden.
    /// </summary>
    public bool CanBeOverridden { get; set; }

    /// <summary>
    /// Gets or sets the permission required to override this restriction.
    /// </summary>
    [StringLength(100)]
    public string? OverridePermissionRequired { get; set; }

    /// <summary>
    /// Gets or sets additional details about this restriction check.
    /// </summary>
    [StringLength(1000)]
    public string? CheckDetails { get; set; }

    // Navigation properties
    /// <summary>
    /// Gets or sets the validation result this check belongs to.
    /// </summary>
    [ForeignKey(nameof(ValidationResultId))]
    public PrerequisiteValidationResult ValidationResult { get; set; } = null!;

    /// <summary>
    /// Gets or sets the enrollment restriction being checked.
    /// </summary>
    [ForeignKey(nameof(EnrollmentRestrictionId))]
    public EnrollmentRestriction EnrollmentRestriction { get; set; } = null!;
}

/// <summary>
/// Entity representing circular dependency detection results.
/// </summary>
public class CircularDependencyResult : BaseEntity
{
    /// <summary>
    /// Gets or sets the course ID where circular dependency was detected.
    /// </summary>
    [Required]
    public int CourseId { get; set; }

    /// <summary>
    /// Gets or sets when this detection was performed.
    /// </summary>
    [Required]
    public DateTime DetectionDate { get; set; }

    /// <summary>
    /// Gets or sets whether a circular dependency was found.
    /// </summary>
    public bool HasCircularDependency { get; set; }

    /// <summary>
    /// Gets or sets the circular dependency path found.
    /// </summary>
    [StringLength(1000)]
    public string? DependencyPath { get; set; }

    /// <summary>
    /// Gets or sets the courses involved in the circular dependency.
    /// </summary>
    [StringLength(500)]
    public string? InvolvedCourses { get; set; }

    /// <summary>
    /// Gets or sets the severity of the circular dependency.
    /// </summary>
    public CircularDependencySeverity? Severity { get; set; }

    /// <summary>
    /// Gets or sets recommendations to resolve the circular dependency.
    /// </summary>
    [StringLength(2000)]
    public string? ResolutionRecommendations { get; set; }

    /// <summary>
    /// Gets or sets whether this issue has been resolved.
    /// </summary>
    public bool IsResolved { get; set; } = false;

    /// <summary>
    /// Gets or sets when this issue was resolved.
    /// </summary>
    public DateTime? ResolutionDate { get; set; }

    /// <summary>
    /// Gets or sets additional details about the circular dependency.
    /// </summary>
    [StringLength(1000)]
    public string? DetectionDetails { get; set; }

    // Navigation properties
    /// <summary>
    /// Gets or sets the course where circular dependency was detected.
    /// </summary>
    [ForeignKey(nameof(CourseId))]
    public Course Course { get; set; } = null!;
}