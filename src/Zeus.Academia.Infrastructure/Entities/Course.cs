using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Zeus.Academia.Infrastructure.Enums;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing a comprehensive course with prerequisites, corequisites, and restrictions.
/// Task 1: Course and Subject Entity Extensions - Course entity with comprehensive prerequisite system.
/// </summary>
public class Course : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the course.
    /// </summary>
    [Key]
    public new int Id { get; set; }

    /// <summary>
    /// Gets or sets the course number following institutional standards.
    /// </summary>
    [Required(ErrorMessage = "Course number is required")]
    [MaxLength(20, ErrorMessage = "Course number cannot exceed 20 characters")]
    [RegularExpression(@"^[A-Z]{2,4}[0-9]{3,4}[A-Z]?$", ErrorMessage = "Course number must follow institutional format")]
    public string CourseNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the course title.
    /// </summary>
    [Required(ErrorMessage = "Course title is required")]
    [MaxLength(200, ErrorMessage = "Course title cannot exceed 200 characters")]
    [MinLength(5, ErrorMessage = "Course title must be at least 5 characters")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the detailed course description.
    /// </summary>
    [MaxLength(2000, ErrorMessage = "Course description cannot exceed 2000 characters")]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the subject code this course belongs to.
    /// </summary>
    [Required(ErrorMessage = "Subject code is required")]
    [MaxLength(10)]
    public string SubjectCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the total credit hours for this course.
    /// </summary>
    [Required]
    [Range(0.5, 15.0, ErrorMessage = "Credit hours must be between 0.5 and 15.0")]
    public decimal CreditHours { get; set; }

    /// <summary>
    /// Gets or sets the total contact hours per week.
    /// </summary>
    [Range(0, 200, ErrorMessage = "Contact hours must be between 0 and 200")]
    public int ContactHours { get; set; }

    /// <summary>
    /// Gets or sets the course level classification.
    /// </summary>
    [Required]
    public CourseLevel Level { get; set; }

    /// <summary>
    /// Gets or sets the current status of the course.
    /// </summary>
    [Required]
    public CourseStatus Status { get; set; } = CourseStatus.UnderReview;

    /// <summary>
    /// Gets or sets the effective date when the course becomes available.
    /// </summary>
    public DateTime? EffectiveDate { get; set; }

    /// <summary>
    /// Gets or sets the date when the course was retired (if applicable).
    /// </summary>
    public DateTime? RetiredDate { get; set; }

    /// <summary>
    /// Gets or sets the reason for retirement.
    /// </summary>
    [MaxLength(500)]
    public string? RetirementReason { get; set; }

    /// <summary>
    /// Gets or sets the catalog year for this course version.
    /// </summary>
    [Range(2000, 2100, ErrorMessage = "Catalog year must be between 2000 and 2100")]
    public int CatalogYear { get; set; }

    /// <summary>
    /// Gets or sets the maximum enrollment capacity.
    /// </summary>
    [Range(1, 1000, ErrorMessage = "Max enrollment must be between 1 and 1000")]
    public int? MaxEnrollment { get; set; }

    /// <summary>
    /// Gets or sets whether the course can be repeated for credit.
    /// </summary>
    public bool CanRepeat { get; set; } = false;

    /// <summary>
    /// Gets or sets the maximum number of times the course can be repeated.
    /// </summary>
    public int? MaxRepeats { get; set; }

    /// <summary>
    /// Gets or sets whether this course requires special approval to enroll.
    /// </summary>
    public bool RequiresApproval { get; set; } = false;

    /// <summary>
    /// Gets or sets the learning outcomes for this course.
    /// </summary>
    public List<string> LearningOutcomes { get; set; } = new();

    /// <summary>
    /// Gets or sets the topics covered in this course.
    /// </summary>
    public List<string> Topics { get; set; } = new();

    /// <summary>
    /// Gets or sets the delivery methods available for this course.
    /// </summary>
    public List<DeliveryMethod> DeliveryMethods { get; set; } = new();

    /// <summary>
    /// Gets or sets the assessment methods used in this course.
    /// </summary>
    public List<AssessmentMethod> AssessmentMethods { get; set; } = new();

    // Navigation Properties

    /// <summary>
    /// Navigation property to the subject this course belongs to.
    /// </summary>
    [ForeignKey(nameof(SubjectCode))]
    public virtual Subject Subject { get; set; } = null!;

    /// <summary>
    /// Navigation property for course prerequisites.
    /// </summary>
    public virtual ICollection<CoursePrerequisite> Prerequisites { get; set; } = new List<CoursePrerequisite>();

    /// <summary>
    /// Navigation property for course corequisites.
    /// </summary>
    public virtual ICollection<CourseCorequisite> Corequisites { get; set; } = new List<CourseCorequisite>();

    /// <summary>
    /// Navigation property for course restrictions.
    /// </summary>
    public virtual ICollection<CourseRestriction> Restrictions { get; set; } = new List<CourseRestriction>();

    /// <summary>
    /// Navigation property for credit type breakdown.
    /// </summary>
    public virtual ICollection<CreditType> CreditBreakdown { get; set; } = new List<CreditType>();

    /// <summary>
    /// Navigation property for course status history.
    /// </summary>
    public virtual ICollection<CourseStatusHistory> StatusHistory { get; set; } = new List<CourseStatusHistory>();

    /// <summary>
    /// Navigation property for course offerings (sections).
    /// </summary>
    public virtual ICollection<CourseOffering> Offerings { get; set; } = new List<CourseOffering>();
}

/// <summary>
/// Entity representing a course prerequisite with complex logic support.
/// </summary>
public class CoursePrerequisite : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    [Key]
    public new int Id { get; set; }

    /// <summary>
    /// Gets or sets the course ID this prerequisite belongs to.
    /// </summary>
    [Required]
    public int CourseId { get; set; }

    /// <summary>
    /// Gets or sets the type of prerequisite.
    /// </summary>
    [Required]
    public PrerequisiteType PrerequisiteType { get; set; }

    /// <summary>
    /// Gets or sets the required course number (for course prerequisites).
    /// </summary>
    [MaxLength(20)]
    public string? RequiredCourseNumber { get; set; }

    /// <summary>
    /// Gets or sets the minimum grade required (if applicable).
    /// </summary>
    [MaxLength(5)]
    public string? MinimumGrade { get; set; }

    /// <summary>
    /// Gets or sets whether this prerequisite is absolutely required.
    /// </summary>
    public bool IsRequired { get; set; } = true;

    /// <summary>
    /// Gets or sets the logical operator for complex prerequisite chains.
    /// </summary>
    public LogicalOperator LogicalOperator { get; set; } = LogicalOperator.And;

    /// <summary>
    /// Gets or sets the group ID for organizing complex prerequisite logic.
    /// </summary>
    public int? GroupId { get; set; }

    /// <summary>
    /// Gets or sets whether this prerequisite can be waived.
    /// </summary>
    public bool CanBeWaived { get; set; } = false;

    /// <summary>
    /// Gets or sets whether waiver requires approval.
    /// </summary>
    public bool WaiverRequiresApproval { get; set; } = true;

    /// <summary>
    /// Gets or sets alternative options that can satisfy this prerequisite.
    /// </summary>
    public List<string> AlternativeOptions { get; set; } = new();

    /// <summary>
    /// Gets or sets additional notes about this prerequisite.
    /// </summary>
    [MaxLength(500)]
    public string? Notes { get; set; }

    /// <summary>
    /// Navigation property to the course.
    /// </summary>
    [ForeignKey(nameof(CourseId))]
    public virtual Course Course { get; set; } = null!;
}

/// <summary>
/// Entity representing a course corequisite.
/// </summary>
public class CourseCorequisite : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    [Key]
    public new int Id { get; set; }

    /// <summary>
    /// Gets or sets the course ID this corequisite belongs to.
    /// </summary>
    [Required]
    public int CourseId { get; set; }

    /// <summary>
    /// Gets or sets the required course number for the corequisite.
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string RequiredCourseNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the corequisite can be taken before or concurrently.
    /// </summary>
    public bool CanBeTakenBefore { get; set; } = false;

    /// <summary>
    /// Gets or sets additional notes about this corequisite.
    /// </summary>
    [MaxLength(500)]
    public string? Notes { get; set; }

    /// <summary>
    /// Navigation property to the course.
    /// </summary>
    [ForeignKey(nameof(CourseId))]
    public virtual Course Course { get; set; } = null!;
}

/// <summary>
/// Entity representing course enrollment restrictions.
/// </summary>
public class CourseRestriction : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    [Key]
    public new int Id { get; set; }

    /// <summary>
    /// Gets or sets the course ID this restriction belongs to.
    /// </summary>
    [Required]
    public int CourseId { get; set; }

    /// <summary>
    /// Gets or sets the type of restriction.
    /// </summary>
    [Required]
    public RestrictionType RestrictionType { get; set; }

    /// <summary>
    /// Gets or sets the restriction value (major name, GPA threshold, etc.).
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether this restriction is required or just preferred.
    /// </summary>
    public bool IsRequired { get; set; } = true;

    /// <summary>
    /// Gets or sets additional notes about this restriction.
    /// </summary>
    [MaxLength(500)]
    public string? Notes { get; set; }

    /// <summary>
    /// Navigation property to the course.
    /// </summary>
    [ForeignKey(nameof(CourseId))]
    public virtual Course Course { get; set; } = null!;
}

/// <summary>
/// Entity representing the breakdown of credit hours by type.
/// </summary>
public class CreditType : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    [Key]
    public new int Id { get; set; }

    /// <summary>
    /// Gets or sets the course ID this credit type belongs to.
    /// </summary>
    [Required]
    public int CourseId { get; set; }

    /// <summary>
    /// Gets or sets the type of credit.
    /// </summary>
    [Required]
    public CreditTypeEnum Type { get; set; }

    /// <summary>
    /// Gets or sets the number of hours for this credit type.
    /// </summary>
    [Required]
    [Range(0.5, 12.0, ErrorMessage = "Credit hours must be between 0.5 and 12.0")]
    public decimal Hours { get; set; }

    /// <summary>
    /// Gets or sets additional notes about this credit type.
    /// </summary>
    [MaxLength(200)]
    public string? Notes { get; set; }

    /// <summary>
    /// Navigation property to the course.
    /// </summary>
    [ForeignKey(nameof(CourseId))]
    public virtual Course Course { get; set; } = null!;
}

/// <summary>
/// Entity representing course status history for lifecycle management.
/// </summary>
public class CourseStatusHistory : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    [Key]
    public new int Id { get; set; }

    /// <summary>
    /// Gets or sets the course ID this status history belongs to.
    /// </summary>
    [Required]
    public int CourseId { get; set; }

    /// <summary>
    /// Gets or sets the course status.
    /// </summary>
    [Required]
    public CourseStatus Status { get; set; }

    /// <summary>
    /// Gets or sets when this status became effective.
    /// </summary>
    [Required]
    public DateTime EffectiveDate { get; set; }

    /// <summary>
    /// Gets or sets who made this status change.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string ChangedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the reason for the status change.
    /// </summary>
    [MaxLength(500)]
    public string? Reason { get; set; }

    /// <summary>
    /// Navigation property to the course.
    /// </summary>
    [ForeignKey(nameof(CourseId))]
    public virtual Course Course { get; set; } = null!;
}

/// <summary>
/// Entity representing a specific offering/section of a course.
/// </summary>
public class CourseOffering : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    [Key]
    public new int Id { get; set; }

    /// <summary>
    /// Gets or sets the course ID this offering belongs to.
    /// </summary>
    [Required]
    public int CourseId { get; set; }

    /// <summary>
    /// Gets or sets the section identifier.
    /// </summary>
    [Required]
    [MaxLength(10)]
    public string SectionId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the academic term.
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string Term { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the academic year.
    /// </summary>
    [Required]
    public int Year { get; set; }

    /// <summary>
    /// Gets or sets the instructor employee number.
    /// </summary>
    public int? InstructorEmpNr { get; set; }

    /// <summary>
    /// Gets or sets the maximum enrollment for this offering.
    /// </summary>
    [Range(1, 1000)]
    public int? MaxEnrollment { get; set; }

    /// <summary>
    /// Gets or sets the current enrollment count.
    /// </summary>
    [Range(0, 1000)]
    public int CurrentEnrollment { get; set; } = 0;

    /// <summary>
    /// Gets or sets the delivery method for this offering.
    /// </summary>
    public DeliveryMethod DeliveryMethod { get; set; } = DeliveryMethod.InPerson;

    /// <summary>
    /// Navigation property to the course.
    /// </summary>
    [ForeignKey(nameof(CourseId))]
    public virtual Course Course { get; set; } = null!;

    /// <summary>
    /// Navigation property to the instructor.
    /// </summary>
    [ForeignKey(nameof(InstructorEmpNr))]
    public virtual Academic? Instructor { get; set; }
}