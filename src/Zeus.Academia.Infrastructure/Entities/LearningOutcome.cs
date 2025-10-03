using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Zeus.Academia.Infrastructure.Enums;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing a learning outcome.
/// </summary>
public class LearningOutcome : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    [Key]
    public new int Id { get; set; }

    /// <summary>
    /// Gets or sets the course ID this outcome belongs to.
    /// </summary>
    public int? CourseId { get; set; }

    /// <summary>
    /// Gets or sets the subject code this outcome belongs to.
    /// </summary>
    [MaxLength(10)]
    public string? SubjectCode { get; set; }

    /// <summary>
    /// Gets or sets the outcome text/description.
    /// </summary>
    [Required]
    [MaxLength(1000)]
    public string OutcomeText { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the outcome category.
    /// </summary>
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Bloom's taxonomy level.
    /// </summary>
    [Required]
    public BloomsTaxonomyLevel BloomsTaxonomyLevel { get; set; } = BloomsTaxonomyLevel.Remember;

    /// <summary>
    /// Gets or sets the difficulty level.
    /// </summary>
    [Required]
    public DifficultyLevel DifficultyLevel { get; set; } = DifficultyLevel.Beginner;

    /// <summary>
    /// Gets or sets the expected mastery level.
    /// </summary>
    [Range(0, 100)]
    public int ExpectedMasteryLevel { get; set; } = 70;

    /// <summary>
    /// Gets or sets the weight/importance of this outcome.
    /// </summary>
    [Range(0.0, 1.0)]
    public decimal Weight { get; set; } = 1.0m;

    /// <summary>
    /// Gets or sets whether this outcome is measurable.
    /// </summary>
    public bool IsMeasurable { get; set; } = true;

    /// <summary>
    /// Gets or sets whether this outcome is observable.
    /// </summary>
    public bool IsObservable { get; set; } = true;

    /// <summary>
    /// Gets or sets the assessment methods for this outcome.
    /// </summary>
    public List<string> AssessmentMethods { get; set; } = new();

    /// <summary>
    /// Gets or sets the program outcomes this is aligned with.
    /// </summary>
    public List<string> ProgramOutcomeAlignment { get; set; } = new();

    /// <summary>
    /// Gets or sets any prerequisite outcomes.
    /// </summary>
    public List<int> PrerequisiteOutcomeIds { get; set; } = new();

    /// <summary>
    /// Gets or sets the order of this outcome within the course/subject.
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets whether this outcome is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the version of this outcome.
    /// </summary>
    [MaxLength(10)]
    public string Version { get; set; } = "1.0";

    /// <summary>
    /// Gets or sets the last review date.
    /// </summary>
    public DateTime? LastReviewDate { get; set; }

    /// <summary>
    /// Gets or sets the next review date.
    /// </summary>
    public DateTime? NextReviewDate { get; set; }

    // Navigation Properties

    /// <summary>
    /// Navigation property to the course.
    /// </summary>
    [ForeignKey(nameof(CourseId))]
    public virtual Course? Course { get; set; }

    /// <summary>
    /// Navigation property to the subject.
    /// </summary>
    [ForeignKey(nameof(SubjectCode))]
    public virtual Subject? Subject { get; set; }

    /// <summary>
    /// Navigation property to assessments of this outcome.
    /// </summary>
    public virtual ICollection<OutcomeAssessment> Assessments { get; set; } = new List<OutcomeAssessment>();

    /// <summary>
    /// Navigation property to prerequisite outcomes.
    /// </summary>
    public virtual ICollection<LearningOutcome> PrerequisiteOutcomes { get; set; } = new List<LearningOutcome>();

    /// <summary>
    /// Navigation property to dependent outcomes.
    /// </summary>
    public virtual ICollection<LearningOutcome> DependentOutcomes { get; set; } = new List<LearningOutcome>();
}

/// <summary>
/// Entity representing an assessment of a learning outcome.
/// </summary>
public class OutcomeAssessment : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    [Key]
    public new int Id { get; set; }

    /// <summary>
    /// Gets or sets the learning outcome ID.
    /// </summary>
    [Required]
    public int LearningOutcomeId { get; set; }

    /// <summary>
    /// Gets or sets the assessment name.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string AssessmentName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the assessment type.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string AssessmentType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the assessment description.
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the assessment method.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string AssessmentMethod { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the weight of this assessment.
    /// </summary>
    [Range(0.0, 1.0)]
    public decimal Weight { get; set; } = 1.0m;

    /// <summary>
    /// Gets or sets the rubric criteria.
    /// </summary>
    public List<string> RubricCriteria { get; set; } = new();

    /// <summary>
    /// Gets or sets the scoring scale.
    /// </summary>
    [MaxLength(100)]
    public string ScoringScale { get; set; } = "1-5";

    /// <summary>
    /// Gets or sets the minimum passing score.
    /// </summary>
    public decimal MinimumPassingScore { get; set; } = 3.0m;

    /// <summary>
    /// Gets or sets the target score.
    /// </summary>
    public decimal TargetScore { get; set; } = 4.0m;

    /// <summary>
    /// Gets or sets whether this assessment is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the assessment date.
    /// </summary>
    public DateTime? AssessmentDate { get; set; }

    /// <summary>
    /// Gets or sets the due date.
    /// </summary>
    public DateTime? DueDate { get; set; }

    // Navigation Properties

    /// <summary>
    /// Navigation property to the learning outcome.
    /// </summary>
    [ForeignKey(nameof(LearningOutcomeId))]
    public virtual LearningOutcome LearningOutcome { get; set; } = null!;

    /// <summary>
    /// Navigation property to assessment results.
    /// </summary>
    public virtual ICollection<AssessmentResult> Results { get; set; } = new List<AssessmentResult>();
}

/// <summary>
/// Entity representing an assessment result.
/// </summary>
public class AssessmentResult : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    [Key]
    public new int Id { get; set; }

    /// <summary>
    /// Gets or sets the outcome assessment ID.
    /// </summary>
    [Required]
    public int OutcomeAssessmentId { get; set; }

    /// <summary>
    /// Gets or sets the student/entity being assessed.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string AssessedEntity { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the score achieved.
    /// </summary>
    [Required]
    public decimal Score { get; set; }

    /// <summary>
    /// Gets or sets the maximum possible score.
    /// </summary>
    [Required]
    public decimal MaxScore { get; set; }

    /// <summary>
    /// Gets or sets the percentage score.
    /// </summary>
    public decimal PercentageScore { get; private set; }

    /// <summary>
    /// Gets or sets whether the result meets the minimum passing criteria.
    /// </summary>
    public bool PassesCriteria { get; set; }

    /// <summary>
    /// Gets or sets whether the result meets the target score.
    /// </summary>
    public bool MeetsTarget { get; set; }

    /// <summary>
    /// Gets or sets the assessment date.
    /// </summary>
    [Required]
    public DateTime AssessmentDate { get; set; }

    /// <summary>
    /// Gets or sets who conducted the assessment.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string AssessedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets additional comments.
    /// </summary>
    [MaxLength(1000)]
    public string? Comments { get; set; }

    /// <summary>
    /// Gets or sets detailed scoring breakdown.
    /// </summary>
    public Dictionary<string, decimal> DetailedScores { get; set; } = new();

    // Navigation Properties

    /// <summary>
    /// Navigation property to the outcome assessment.
    /// </summary>
    [ForeignKey(nameof(OutcomeAssessmentId))]
    public virtual OutcomeAssessment OutcomeAssessment { get; set; } = null!;
}