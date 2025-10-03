using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Zeus.Academia.Infrastructure.Enums;
using Zeus.Academia.Infrastructure.Extensions;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Template defining degree requirements for a specific degree program.
/// Contains all categories and requirements needed for graduation.
/// </summary>
[Table("DegreeRequirementTemplates")]
public class DegreeRequirementTemplate
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    public string DegreeCode { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string DegreeName { get; set; } = string.Empty;

    [Range(1, 300)]
    public int TotalCreditsRequired { get; set; }

    [Range(0, 100)]
    public int ResidencyCreditsRequired { get; set; }

    [Range(0.0, 4.0)]
    [Column(TypeName = "decimal(3,2)")]
    public decimal MinimumGPA { get; set; }

    [Range(1, 10)]
    public int MaxTimeToComplete { get; set; } // years

    public DateTime EffectiveDate { get; set; }
    public DateTime? ExpirationDate { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(100)]
    public string CreatedBy { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    [StringLength(100)]
    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual ICollection<RequirementCategory> Categories { get; set; } = new List<RequirementCategory>();
    public virtual ICollection<StudentDegreeAudit> StudentAudits { get; set; } = new List<StudentDegreeAudit>();

    // Helper methods
    public List<DegreeRequirement> GetAllRequirements()
    {
        return Categories.SelectMany(c => c.Requirements).ToList();
    }

    public int GetTotalRequiredCredits()
    {
        return Categories.Sum(c => c.CreditsRequired);
    }

    public bool IsCurrentlyEffective()
    {
        var now = DateTime.UtcNow;
        return IsActive && EffectiveDate <= now && (ExpirationDate == null || ExpirationDate > now);
    }
}

/// <summary>
/// Represents a category of requirements within a degree (e.g., General Education, Major Requirements).
/// Groups related requirements for better organization and tracking.
/// </summary>
[Table("RequirementCategories")]
public class RequirementCategory
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Range(0, 200)]
    public int CreditsRequired { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    [Range(0, 100)]
    public int DisplayOrder { get; set; }

    public bool IsRequired { get; set; } = true;

    // Foreign key
    public int DegreeTemplateId { get; set; }

    // Navigation properties
    [ForeignKey("DegreeTemplateId")]
    public virtual DegreeRequirementTemplate DegreeTemplate { get; set; } = null!;

    public virtual ICollection<DegreeRequirement> Requirements { get; set; } = new List<DegreeRequirement>();

    // Helper methods
    public int GetTotalRequiredCredits()
    {
        return Requirements.Where(r => r.IsRequired).Sum(r => r.CreditsRequired);
    }

    public int GetElectiveCredits()
    {
        return CreditsRequired - GetTotalRequiredCredits();
    }
}

/// <summary>
/// Specific requirement within a category (e.g., specific course, course group, conditional requirement).
/// Supports complex logic for various types of degree requirements.
/// </summary>
[Table("DegreeRequirements")]
public class DegreeRequirement
{
    [Key]
    public int Id { get; set; }

    [Required]
    public RequirementType Type { get; set; }

    [Required]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Range(0, 50)]
    public int CreditsRequired { get; set; }

    [Range(0, 20)]
    public int CoursesRequired { get; set; }

    public bool IsRequired { get; set; } = true;

    public bool SequenceRequired { get; set; } = false;

    public RequirementLogicType LogicType { get; set; } = RequirementLogicType.And;

    [Range(0, 1000)]
    public int MinimumCourseLevel { get; set; }

    [Range(0, 1000)]
    public int MaximumCourseLevel { get; set; } = 999;

    [Range(0.0, 4.0)]
    [Column(TypeName = "decimal(3,2)")]
    public decimal? MinimumGrade { get; set; }

    [StringLength(2000)]
    public string? AdditionalCriteria { get; set; }

    [Range(0, 100)]
    public int DisplayOrder { get; set; }

    // Foreign key
    public int CategoryId { get; set; }

    // Navigation properties
    [ForeignKey("CategoryId")]
    public virtual RequirementCategory Category { get; set; } = null!;

    public virtual ICollection<RequirementCourse> RequiredCourses { get; set; } = new List<RequirementCourse>();
    public virtual ICollection<RequirementSubject> RequiredSubjects { get; set; } = new List<RequirementSubject>();
    public virtual ICollection<ConditionalRequirement> ConditionalRequirements { get; set; } = new List<ConditionalRequirement>();
    public virtual ICollection<PrerequisiteLink> PrerequisiteChain { get; set; } = new List<PrerequisiteLink>();

    // Helper methods
    public bool IsSpecificCourseRequirement()
    {
        return Type == RequirementType.SpecificCourse && RequiredCourses.Any();
    }

    public bool IsSubjectAreaRequirement()
    {
        return Type == RequirementType.CourseGroup && RequiredSubjects.Any();
    }

    public bool HasConditionalLogic()
    {
        return Type == RequirementType.ConditionalGroup && ConditionalRequirements.Any();
    }

    public List<int> GetCourseIds()
    {
        return RequiredCourses.Select(rc => rc.CourseId).ToList();
    }

    public List<string> GetSubjectCodes()
    {
        return RequiredSubjects.Select(rs => rs.SubjectCode).ToList();
    }
}

/// <summary>
/// Links specific courses to degree requirements.
/// Supports many-to-many relationship between requirements and courses.
/// </summary>
[Table("RequirementCourses")]
public class RequirementCourse
{
    [Key]
    public int Id { get; set; }

    // Foreign keys
    public int RequirementId { get; set; }
    public int CourseId { get; set; }

    public bool IsAlternative { get; set; } = false; // Can substitute for required course
    public bool IsPreferred { get; set; } = false;   // Recommended choice among alternatives

    // Navigation properties
    [ForeignKey("RequirementId")]
    public virtual DegreeRequirement Requirement { get; set; } = null!;

    [ForeignKey("CourseId")]
    public virtual Course Course { get; set; } = null!;
}

/// <summary>
/// Links subject areas to degree requirements.
/// Allows requirements based on subject codes (e.g., any MATH course).
/// </summary>
[Table("RequirementSubjects")]
public class RequirementSubject
{
    [Key]
    public int Id { get; set; }

    // Foreign key
    public int RequirementId { get; set; }

    [Required]
    [StringLength(10)]
    public string SubjectCode { get; set; } = string.Empty;

    [Range(0, 1000)]
    public int MinimumLevel { get; set; }

    [Range(0, 1000)]
    public int MaximumLevel { get; set; } = 999;

    public bool ExcludeIntroductory { get; set; } = false;
    public bool ExcludeRemedial { get; set; } = true;

    // Navigation properties
    [ForeignKey("RequirementId")]
    public virtual DegreeRequirement Requirement { get; set; } = null!;

    [ForeignKey("SubjectCode")]
    public virtual Subject Subject { get; set; } = null!;
}

/// <summary>
/// Defines conditional requirements with specific criteria.
/// Supports complex "either/or" and "choose X from Y" scenarios.
/// </summary>
[Table("ConditionalRequirements")]
public class ConditionalRequirement
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(500)]
    public string Condition { get; set; } = string.Empty;

    [Range(0, 50)]
    public int CreditsRequired { get; set; }

    [Range(0, 20)]
    public int CoursesRequired { get; set; }

    [Range(0, 1000)]
    public int MinimumCourseLevel { get; set; }

    [Range(0, 1000)]
    public int MaximumCourseLevel { get; set; } = 999;

    [Range(0.0, 4.0)]
    [Column(TypeName = "decimal(3,2)")]
    public decimal? MinimumGPA { get; set; }

    [StringLength(1000)]
    public string? AdditionalCriteria { get; set; }

    [Range(0, 100)]
    public int Priority { get; set; } = 1; // For ordering alternative conditions

    // Foreign key
    public int RequirementId { get; set; }

    // Navigation properties
    [ForeignKey("RequirementId")]
    public virtual DegreeRequirement Requirement { get; set; } = null!;

    public virtual ICollection<ConditionalRequirementCourse> Courses { get; set; } = new List<ConditionalRequirementCourse>();
    public virtual ICollection<ConditionalRequirementSubject> Subjects { get; set; } = new List<ConditionalRequirementSubject>();

    // Helper methods
    public bool IsSatisfiedBy(List<Course> completedCourses, decimal? studentGPA)
    {
        // Basic implementation - would be expanded with complex logic
        if (MinimumGPA.HasValue && (!studentGPA.HasValue || studentGPA < MinimumGPA))
            return false;

        var applicableCourses = completedCourses.Where(c =>
            c.GetCourseLevel() >= MinimumCourseLevel &&
            c.GetCourseLevel() <= MaximumCourseLevel).ToList();

        var completedCredits = applicableCourses.Sum(c => c.CreditHours);
        var completedCourseCount = applicableCourses.Count;

        return completedCredits >= CreditsRequired && completedCourseCount >= CoursesRequired;
    }
}

/// <summary>
/// Links courses to conditional requirements.
/// </summary>
[Table("ConditionalRequirementCourses")]
public class ConditionalRequirementCourse
{
    [Key]
    public int Id { get; set; }

    // Foreign keys
    public int ConditionalRequirementId { get; set; }
    public int CourseId { get; set; }

    // Navigation properties
    [ForeignKey("ConditionalRequirementId")]
    public virtual ConditionalRequirement ConditionalRequirement { get; set; } = null!;

    [ForeignKey("CourseId")]
    public virtual Course Course { get; set; } = null!;
}

/// <summary>
/// Links subject areas to conditional requirements.
/// </summary>
[Table("ConditionalRequirementSubjects")]
public class ConditionalRequirementSubject
{
    [Key]
    public int Id { get; set; }

    // Foreign key
    public int ConditionalRequirementId { get; set; }

    [Required]
    [StringLength(10)]
    public string SubjectCode { get; set; } = string.Empty;

    // Navigation properties
    [ForeignKey("ConditionalRequirementId")]
    public virtual ConditionalRequirement ConditionalRequirement { get; set; } = null!;

    [ForeignKey("SubjectCode")]
    public virtual Subject Subject { get; set; } = null!;
}

/// <summary>
/// Defines prerequisite relationships within degree requirements.
/// Supports complex prerequisite chains for sequenced courses.
/// </summary>
[Table("PrerequisiteLinks")]
public class PrerequisiteLink
{
    [Key]
    public int Id { get; set; }

    // Foreign keys
    public int RequirementId { get; set; }
    public int CourseId { get; set; }
    public int? PrerequisiteCourseId { get; set; }

    public PrerequisiteLogicType LogicType { get; set; } = PrerequisiteLogicType.And;

    [Range(0, 100)]
    public int SequenceOrder { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    // Navigation properties
    [ForeignKey("RequirementId")]
    public virtual DegreeRequirement Requirement { get; set; } = null!;

    [ForeignKey("CourseId")]
    public virtual Course Course { get; set; } = null!;

    [ForeignKey("PrerequisiteCourseId")]
    public virtual Course? PrerequisiteCourse { get; set; }

    // Helper methods
    public bool IsStartingCourse()
    {
        return PrerequisiteCourseId == null;
    }

    public bool HasPrerequisite()
    {
        return PrerequisiteCourseId.HasValue;
    }
}

/// <summary>
/// Tracks individual student's progress toward degree completion.
/// Links to degree template and maintains audit history.
/// </summary>
[Table("StudentDegreeAudits")]
public class StudentDegreeAudit
{
    [Key]
    public int Id { get; set; }

    // Foreign keys
    public int StudentId { get; set; }
    public int DegreeTemplateId { get; set; }

    public DateTime AuditDate { get; set; } = DateTime.UtcNow;

    [Range(0, 300)]
    public int TotalCreditsCompleted { get; set; }

    [Range(0, 300)]
    public int TotalCreditsRequired { get; set; }

    [Range(0, 300)]
    public int RemainingCreditsNeeded { get; set; }

    [Range(0.0, 100.0)]
    [Column(TypeName = "decimal(5,2)")]
    public decimal CompletionPercentage { get; set; }

    [Range(0.0, 4.0)]
    [Column(TypeName = "decimal(3,2)")]
    public decimal? CurrentGPA { get; set; }

    [Range(0.0, 4.0)]
    [Column(TypeName = "decimal(3,2)")]
    public decimal? MajorGPA { get; set; }

    public bool IsEligibleForGraduation { get; set; } = false;

    public DateTime? EstimatedGraduationDate { get; set; }

    [StringLength(2000)]
    public string? Notes { get; set; }

    [StringLength(100)]
    public string AuditedBy { get; set; } = string.Empty;

    // Navigation properties
    [ForeignKey("StudentId")]
    public virtual Student Student { get; set; } = null!;

    [ForeignKey("DegreeTemplateId")]
    public virtual DegreeRequirementTemplate DegreeTemplate { get; set; } = null!;

    public virtual ICollection<CategoryProgress> CategoryProgress { get; set; } = new List<CategoryProgress>();
    public virtual ICollection<RequirementFulfillment> RequirementFulfillments { get; set; } = new List<RequirementFulfillment>();
    public virtual ICollection<CourseSubstitution> ApprovedSubstitutions { get; set; } = new List<CourseSubstitution>();

    // Helper methods
    public void UpdateCompletionPercentage()
    {
        if (TotalCreditsRequired > 0)
        {
            CompletionPercentage = Math.Round((decimal)TotalCreditsCompleted / TotalCreditsRequired * 100, 2);
        }
        else
        {
            CompletionPercentage = 0;
        }
    }

    public void UpdateRemainingCredits()
    {
        RemainingCreditsNeeded = Math.Max(0, TotalCreditsRequired - TotalCreditsCompleted);
    }

    public bool IsNearingCompletion(int thresholdCredits = 15)
    {
        return RemainingCreditsNeeded <= thresholdCredits;
    }
}

/// <summary>
/// Tracks progress within specific requirement categories.
/// </summary>
[Table("CategoryProgress")]
public class CategoryProgress
{
    [Key]
    public int Id { get; set; }

    // Foreign keys
    public int AuditId { get; set; }
    public int CategoryId { get; set; }

    [Range(0, 200)]
    public int CreditsCompleted { get; set; }

    [Range(0, 200)]
    public int CreditsRequired { get; set; }

    [Range(0, 200)]
    public int CreditsRemaining { get; set; }

    [Range(0.0, 100.0)]
    [Column(TypeName = "decimal(5,2)")]
    public decimal CompletionPercentage { get; set; }

    public bool IsComplete { get; set; } = false;

    [StringLength(1000)]
    public string? Notes { get; set; }

    // Navigation properties
    [ForeignKey("AuditId")]
    public virtual StudentDegreeAudit Audit { get; set; } = null!;

    [ForeignKey("CategoryId")]
    public virtual RequirementCategory Category { get; set; } = null!;

    // Helper methods
    public void UpdateProgress()
    {
        CreditsRemaining = Math.Max(0, CreditsRequired - CreditsCompleted);
        CompletionPercentage = CreditsRequired > 0 ? Math.Round((decimal)CreditsCompleted / CreditsRequired * 100, 2) : 0;
        IsComplete = CreditsCompleted >= CreditsRequired;
    }
}

/// <summary>
/// Tracks how specific requirements are fulfilled by completed courses.
/// </summary>
[Table("RequirementFulfillments")]
public class RequirementFulfillment
{
    [Key]
    public int Id { get; set; }

    // Foreign keys
    public int AuditId { get; set; }
    public int RequirementId { get; set; }
    public int? FulfillingCourseId { get; set; }

    public bool IsFulfilled { get; set; } = false;
    public bool IsPartiallyFulfilled { get; set; } = false;

    [Range(0, 50)]
    public int CreditsFulfilled { get; set; }

    [Range(0, 50)]
    public int CreditsStillNeeded { get; set; }

    [StringLength(1000)]
    public string? FulfillmentMethod { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    public DateTime FulfillmentDate { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("AuditId")]
    public virtual StudentDegreeAudit Audit { get; set; } = null!;

    [ForeignKey("RequirementId")]
    public virtual DegreeRequirement Requirement { get; set; } = null!;

    [ForeignKey("FulfillingCourseId")]
    public virtual Course? FulfillingCourse { get; set; }

    // Helper methods
    public void MarkAsFulfilled(Course course, int creditsApplied)
    {
        FulfillingCourseId = course.Id;
        CreditsFulfilled = creditsApplied;
        CreditsStillNeeded = Math.Max(0, Requirement.CreditsRequired - creditsApplied);
        IsFulfilled = CreditsStillNeeded == 0;
        IsPartiallyFulfilled = CreditsFulfilled > 0 && CreditsStillNeeded > 0;
        FulfillmentDate = DateTime.UtcNow;
    }
}

/// <summary>
/// Represents approved course substitutions for degree requirements.
/// Allows flexibility in degree completion while maintaining academic integrity.
/// </summary>
[Table("CourseSubstitutions")]
public class CourseSubstitution
{
    [Key]
    public int Id { get; set; }

    // Foreign keys
    public int? AuditId { get; set; }
    public int StudentId { get; set; }
    public int OriginalCourseId { get; set; }
    public int SubstituteCourseId { get; set; }

    [Required]
    [StringLength(500)]
    public string Reason { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string ApprovedBy { get; set; } = string.Empty;

    public DateTime ApprovalDate { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;

    [StringLength(1000)]
    public string? AdditionalConditions { get; set; }

    public DateTime? ExpirationDate { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    // Navigation properties
    [ForeignKey("AuditId")]
    public virtual StudentDegreeAudit? Audit { get; set; }

    [ForeignKey("StudentId")]
    public virtual Student Student { get; set; } = null!;

    [ForeignKey("OriginalCourseId")]
    public virtual Course OriginalCourse { get; set; } = null!;

    [ForeignKey("SubstituteCourseId")]
    public virtual Course SubstituteCourse { get; set; } = null!;

    // Helper methods
    public bool IsCurrentlyValid()
    {
        return IsActive && (ExpirationDate == null || ExpirationDate > DateTime.UtcNow);
    }

    public bool AppliesTo(int courseId)
    {
        return IsCurrentlyValid() && OriginalCourseId == courseId;
    }
}