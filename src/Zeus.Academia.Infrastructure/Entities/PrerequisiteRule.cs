using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Zeus.Academia.Infrastructure.Enums;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing a prerequisite rule for a course with complex logic support.
/// Manages AND/OR/XOR logic for combining multiple prerequisite requirements.
/// </summary>
public class PrerequisiteRule : BaseEntity
{
    /// <summary>
    /// Gets or sets the course ID this rule applies to.
    /// </summary>
    [Required]
    public int CourseId { get; set; }

    /// <summary>
    /// Gets or sets the descriptive name of this prerequisite rule.
    /// </summary>
    [Required]
    [StringLength(200)]
    public string RuleName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of this prerequisite rule.
    /// </summary>
    [StringLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the logical operator for combining requirements.
    /// </summary>
    [Required]
    public PrerequisiteLogicOperator LogicOperator { get; set; }

    /// <summary>
    /// Gets or sets whether this rule is currently active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the priority order for rule evaluation.
    /// </summary>
    public int Priority { get; set; } = 1;

    /// <summary>
    /// Gets or sets the effective date when this rule becomes active.
    /// </summary>
    public DateTime? EffectiveDate { get; set; }

    /// <summary>
    /// Gets or sets the expiration date when this rule is no longer active.
    /// </summary>
    public DateTime? ExpirationDate { get; set; }

    /// <summary>
    /// Gets or sets additional metadata about this rule.
    /// </summary>
    [StringLength(2000)]
    public string? RuleMetadata { get; set; }

    // Navigation properties
    /// <summary>
    /// Gets or sets the course this rule applies to.
    /// </summary>
    [ForeignKey(nameof(CourseId))]
    public Course Course { get; set; } = null!;

    /// <summary>
    /// Gets or sets the individual requirements that make up this rule.
    /// </summary>
    public List<PrerequisiteRequirement> Requirements { get; set; } = new();

    /// <summary>
    /// Gets or sets nested rules for complex logic scenarios.
    /// </summary>
    public List<PrerequisiteRule> NestedRules { get; set; } = new();

    /// <summary>
    /// Gets or sets validation results for this rule.
    /// </summary>
    public List<RuleValidationHistory> ValidationHistory { get; set; } = new();
}

/// <summary>
/// Entity representing an individual prerequisite requirement within a rule.
/// </summary>
public class PrerequisiteRequirement : BaseEntity
{
    /// <summary>
    /// Gets or sets the prerequisite rule ID this requirement belongs to.
    /// </summary>
    [Required]
    public int PrerequisiteRuleId { get; set; }

    /// <summary>
    /// Gets or sets the type of prerequisite requirement.
    /// </summary>
    [Required]
    public PrerequisiteType RequirementType { get; set; }

    /// <summary>
    /// Gets or sets whether this requirement is mandatory within its rule.
    /// </summary>
    public bool IsRequired { get; set; } = true;

    /// <summary>
    /// Gets or sets the sequence order of this requirement.
    /// </summary>
    public int SequenceOrder { get; set; } = 1;

    // Course-specific properties
    /// <summary>
    /// Gets or sets the required course ID (for Course type requirements).
    /// </summary>
    public int? RequiredCourseId { get; set; }

    /// <summary>
    /// Gets or sets the minimum grade required (for Course type requirements).
    /// </summary>
    [StringLength(5)]
    public string? MinimumGrade { get; set; }

    /// <summary>
    /// Gets or sets whether the course must be completed or can be concurrent.
    /// </summary>
    public bool MustBeCompleted { get; set; } = true;

    // Credit hours properties
    /// <summary>
    /// Gets or sets the minimum credit hours required (for CreditHours type requirements).
    /// </summary>
    public int? MinimumCreditHours { get; set; }

    /// <summary>
    /// Gets or sets the subject area for credit hours (for CreditHours type requirements).
    /// </summary>
    [StringLength(10)]
    public string? CreditHoursSubjectArea { get; set; }

    // Class standing properties
    /// <summary>
    /// Gets or sets the required class standing (for ClassStanding type requirements).
    /// </summary>
    public ClassStanding? RequiredClassStanding { get; set; }

    // GPA properties
    /// <summary>
    /// Gets or sets the minimum GPA required (for GPA type requirements).
    /// </summary>
    [Column(TypeName = "decimal(4,3)")]
    public decimal? MinimumGPA { get; set; }

    /// <summary>
    /// Gets or sets the GPA calculation scope (for GPA type requirements).
    /// </summary>
    [StringLength(50)]
    public string? GPAScope { get; set; } // Overall, Major, LastSemester, etc.

    // Permission properties
    /// <summary>
    /// Gets or sets the required permission (for PermissionRequired type requirements).
    /// </summary>
    [StringLength(100)]
    public string? RequiredPermission { get; set; }

    /// <summary>
    /// Gets or sets the permission level required.
    /// </summary>
    public PermissionLevel? PermissionLevel { get; set; }

    // Test score properties
    /// <summary>
    /// Gets or sets the test name (for TestScore type requirements).
    /// </summary>
    [StringLength(100)]
    public string? TestName { get; set; }

    /// <summary>
    /// Gets or sets the minimum test score required.
    /// </summary>
    public int? MinimumTestScore { get; set; }

    /// <summary>
    /// Gets or sets the maximum age of test score in months.
    /// </summary>
    public int? TestScoreValidityMonths { get; set; }

    // General properties
    /// <summary>
    /// Gets or sets alternative ways to satisfy this requirement.
    /// </summary>
    [StringLength(1000)]
    public string? AlternativeSatisfactionMethods { get; set; }

    /// <summary>
    /// Gets or sets additional notes about this requirement.
    /// </summary>
    [StringLength(500)]
    public string? RequirementNotes { get; set; }

    // Navigation properties
    /// <summary>
    /// Gets or sets the prerequisite rule this requirement belongs to.
    /// </summary>
    [ForeignKey(nameof(PrerequisiteRuleId))]
    public PrerequisiteRule PrerequisiteRule { get; set; } = null!;

    /// <summary>
    /// Gets or sets the required course (for Course type requirements).
    /// </summary>
    [ForeignKey(nameof(RequiredCourseId))]
    public Course? RequiredCourse { get; set; }
}

/// <summary>
/// Entity representing a corequisite rule for courses that must be taken together.
/// </summary>
public class CorequisiteRule : BaseEntity
{
    /// <summary>
    /// Gets or sets the course ID this corequisite rule applies to.
    /// </summary>
    [Required]
    public int CourseId { get; set; }

    /// <summary>
    /// Gets or sets the descriptive name of this corequisite rule.
    /// </summary>
    [Required]
    [StringLength(200)]
    public string RuleName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of this corequisite rule.
    /// </summary>
    [StringLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the enforcement type for this corequisite rule.
    /// </summary>
    [Required]
    public CorequisiteEnforcementType EnforcementType { get; set; }

    /// <summary>
    /// Gets or sets whether this rule is currently active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the effective date when this rule becomes active.
    /// </summary>
    public DateTime? EffectiveDate { get; set; }

    /// <summary>
    /// Gets or sets the expiration date when this rule is no longer active.
    /// </summary>
    public DateTime? ExpirationDate { get; set; }

    // Navigation properties
    /// <summary>
    /// Gets or sets the course this rule applies to.
    /// </summary>
    [ForeignKey(nameof(CourseId))]
    public Course Course { get; set; } = null!;

    /// <summary>
    /// Gets or sets the corequisite requirements for this rule.
    /// </summary>
    public List<CorequisiteRequirement> CorequisiteRequirements { get; set; } = new();
}

/// <summary>
/// Entity representing an individual corequisite requirement.
/// </summary>
public class CorequisiteRequirement : BaseEntity
{
    /// <summary>
    /// Gets or sets the corequisite rule ID this requirement belongs to.
    /// </summary>
    [Required]
    public int CorequisiteRuleId { get; set; }

    /// <summary>
    /// Gets or sets the required course ID that must be taken with the main course.
    /// </summary>
    [Required]
    public int RequiredCourseId { get; set; }

    /// <summary>
    /// Gets or sets the enrollment relationship type.
    /// </summary>
    [Required]
    public CorequisiteRelationship EnrollmentRelationship { get; set; }

    /// <summary>
    /// Gets or sets whether this corequisite can be waived.
    /// </summary>
    public bool IsWaivable { get; set; } = false;

    /// <summary>
    /// Gets or sets the permission required to waive this corequisite.
    /// </summary>
    [StringLength(100)]
    public string? WaiverRequiredPermission { get; set; }

    /// <summary>
    /// Gets or sets the action to take when this corequisite requirement fails.
    /// </summary>
    [Required]
    public CorequisiteFailureAction FailureAction { get; set; }

    /// <summary>
    /// Gets or sets additional notes about this corequisite requirement.
    /// </summary>
    [StringLength(500)]
    public string? RequirementNotes { get; set; }

    // Navigation properties
    /// <summary>
    /// Gets or sets the corequisite rule this requirement belongs to.
    /// </summary>
    [ForeignKey(nameof(CorequisiteRuleId))]
    public CorequisiteRule CorequisiteRule { get; set; } = null!;

    /// <summary>
    /// Gets or sets the required course that must be taken concurrently.
    /// </summary>
    [ForeignKey(nameof(RequiredCourseId))]
    public Course RequiredCourse { get; set; } = null!;
}

/// <summary>
/// Entity representing enrollment restrictions for courses.
/// </summary>
public class EnrollmentRestriction : BaseEntity
{
    /// <summary>
    /// Gets or sets the course ID this restriction applies to.
    /// </summary>
    [Required]
    public int CourseId { get; set; }

    /// <summary>
    /// Gets or sets the type of restriction.
    /// </summary>
    [Required]
    public RestrictionType RestrictionType { get; set; }

    /// <summary>
    /// Gets or sets whether this restriction is currently active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the priority of this restriction for evaluation order.
    /// </summary>
    public int Priority { get; set; } = 1;

    /// <summary>
    /// Gets or sets the enforcement level of this restriction.
    /// </summary>
    [Required]
    public RestrictionEnforcementLevel EnforcementLevel { get; set; }

    /// <summary>
    /// Gets or sets the descriptive name of this restriction.
    /// </summary>
    [StringLength(200)]
    public string? RestrictionName { get; set; }

    /// <summary>
    /// Gets or sets the description of this restriction.
    /// </summary>
    [StringLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the effective date when this restriction becomes active.
    /// </summary>
    public DateTime? EffectiveDate { get; set; }

    /// <summary>
    /// Gets or sets the expiration date when this restriction is no longer active.
    /// </summary>
    public DateTime? ExpirationDate { get; set; }

    /// <summary>
    /// Gets or sets the violation message shown when restriction is triggered.
    /// </summary>
    [StringLength(500)]
    public string? ViolationMessage { get; set; }

    // Navigation properties
    /// <summary>
    /// Gets or sets the course this restriction applies to.
    /// </summary>
    [ForeignKey(nameof(CourseId))]
    public Course Course { get; set; } = null!;

    /// <summary>
    /// Gets or sets the major restrictions for this enrollment restriction.
    /// </summary>
    public List<MajorRestriction> MajorRestrictions { get; set; } = new();

    /// <summary>
    /// Gets or sets the class standing restrictions for this enrollment restriction.
    /// </summary>
    public List<ClassStandingRestriction> ClassStandingRestrictions { get; set; } = new();

    /// <summary>
    /// Gets or sets the permission restrictions for this enrollment restriction.
    /// </summary>
    public List<PermissionRestriction> PermissionRestrictions { get; set; } = new();
}

/// <summary>
/// Entity representing major-specific enrollment restrictions.
/// </summary>
public class MajorRestriction : BaseEntity
{
    /// <summary>
    /// Gets or sets the enrollment restriction ID this belongs to.
    /// </summary>
    [Required]
    public int EnrollmentRestrictionId { get; set; }

    /// <summary>
    /// Gets or sets the required major code.
    /// </summary>
    [Required]
    [StringLength(10)]
    public string RequiredMajorCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of major required.
    /// </summary>
    [Required]
    public MajorType MajorType { get; set; }

    /// <summary>
    /// Gets or sets whether this major is included (true) or excluded (false).
    /// </summary>
    public bool IsIncluded { get; set; } = true;

    /// <summary>
    /// Gets or sets the minimum progress percentage in major required.
    /// </summary>
    [Column(TypeName = "decimal(5,2)")]
    public decimal? MinimumMajorProgress { get; set; }

    // Navigation properties
    /// <summary>
    /// Gets or sets the enrollment restriction this belongs to.
    /// </summary>
    [ForeignKey(nameof(EnrollmentRestrictionId))]
    public EnrollmentRestriction EnrollmentRestriction { get; set; } = null!;
}

/// <summary>
/// Entity representing class standing enrollment restrictions.
/// </summary>
public class ClassStandingRestriction : BaseEntity
{
    /// <summary>
    /// Gets or sets the enrollment restriction ID this belongs to.
    /// </summary>
    [Required]
    public int EnrollmentRestrictionId { get; set; }

    /// <summary>
    /// Gets or sets the required class standing.
    /// </summary>
    [Required]
    public ClassStanding RequiredClassStanding { get; set; }

    /// <summary>
    /// Gets or sets whether this class standing is included (true) or excluded (false).
    /// </summary>
    public bool IsIncluded { get; set; } = true;

    /// <summary>
    /// Gets or sets the minimum credit hours for this class standing.
    /// </summary>
    public int? MinimumCreditHours { get; set; }

    /// <summary>
    /// Gets or sets the maximum credit hours for this class standing.
    /// </summary>
    public int? MaximumCreditHours { get; set; }

    // Navigation properties
    /// <summary>
    /// Gets or sets the enrollment restriction this belongs to.
    /// </summary>
    [ForeignKey(nameof(EnrollmentRestrictionId))]
    public EnrollmentRestriction EnrollmentRestriction { get; set; } = null!;
}

/// <summary>
/// Entity representing permission-based enrollment restrictions.
/// </summary>
public class PermissionRestriction : BaseEntity
{
    /// <summary>
    /// Gets or sets the enrollment restriction ID this belongs to.
    /// </summary>
    [Required]
    public int EnrollmentRestrictionId { get; set; }

    /// <summary>
    /// Gets or sets the required permission.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string RequiredPermission { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the permission level required.
    /// </summary>
    [Required]
    public PermissionLevel PermissionLevel { get; set; }

    /// <summary>
    /// Gets or sets whether documentation is required for this permission.
    /// </summary>
    public bool RequiresDocumentation { get; set; } = false;

    /// <summary>
    /// Gets or sets the documentation requirements.
    /// </summary>
    [StringLength(500)]
    public string? DocumentationRequirements { get; set; }

    /// <summary>
    /// Gets or sets the expiration period for this permission in days.
    /// </summary>
    public int? PermissionValidityDays { get; set; }

    // Navigation properties
    /// <summary>
    /// Gets or sets the enrollment restriction this belongs to.
    /// </summary>
    [ForeignKey(nameof(EnrollmentRestrictionId))]
    public EnrollmentRestriction EnrollmentRestriction { get; set; } = null!;
}

/// <summary>
/// Entity for tracking validation history of prerequisite rules.
/// </summary>
public class RuleValidationHistory : BaseEntity
{
    /// <summary>
    /// Gets or sets the prerequisite rule ID this history entry belongs to.
    /// </summary>
    [Required]
    public int PrerequisiteRuleId { get; set; }

    /// <summary>
    /// Gets or sets when this validation occurred.
    /// </summary>
    [Required]
    public DateTime ValidationDate { get; set; }

    /// <summary>
    /// Gets or sets who initiated the validation.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string ValidatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the validation passed.
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Gets or sets any validation errors found.
    /// </summary>
    [StringLength(2000)]
    public string? ValidationErrors { get; set; }

    /// <summary>
    /// Gets or sets the validation result details.
    /// </summary>
    [StringLength(4000)]
    public string? ValidationDetails { get; set; }

    // Navigation properties
    /// <summary>
    /// Gets or sets the prerequisite rule this validation history belongs to.
    /// </summary>
    [ForeignKey(nameof(PrerequisiteRuleId))]
    public PrerequisiteRule PrerequisiteRule { get; set; } = null!;
}