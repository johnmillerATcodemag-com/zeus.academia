using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing an academic rank.
/// Enhanced for Task 3: Academic Structure Entities with comprehensive validation and academic hierarchy management.
/// </summary>
public class Rank : BaseEntity
{
    /// <summary>
    /// Gets or sets the rank code - the primary identifier.
    /// </summary>
    [Key]
    [Required(ErrorMessage = "Rank code is required")]
    [MaxLength(10, ErrorMessage = "Rank code cannot exceed 10 characters")]
    [RegularExpression(@"^[A-Z]{2,8}[0-9]{0,2}$", ErrorMessage = "Rank code must be 2-8 uppercase letters optionally followed by up to 2 numbers (e.g., PROF, ASSOC, ASSIST, LECTR1)")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the rank title or name.
    /// </summary>
    [Required(ErrorMessage = "Rank title is required")]
    [MaxLength(50, ErrorMessage = "Rank title cannot exceed 50 characters")]
    [MinLength(5, ErrorMessage = "Rank title must be at least 5 characters")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the rank level or hierarchy order (1 = highest rank).
    /// </summary>
    [Range(1, 20, ErrorMessage = "Rank level must be between 1 and 20")]
    public int? Level { get; set; }

    /// <summary>
    /// Gets or sets the description of the rank.
    /// </summary>
    [MaxLength(200, ErrorMessage = "Rank description cannot exceed 200 characters")]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the minimum salary for this rank.
    /// </summary>
    [Range(0, 1000000, ErrorMessage = "Minimum salary must be between 0 and 1,000,000")]
    [Column(TypeName = "decimal(10,2)")]
    public decimal? MinSalary { get; set; }

    /// <summary>
    /// Gets or sets the maximum salary for this rank.
    /// </summary>
    [Range(0, 1000000, ErrorMessage = "Maximum salary must be between 0 and 1,000,000")]
    [Column(TypeName = "decimal(10,2)")]
    public decimal? MaxSalary { get; set; }

    /// <summary>
    /// Gets or sets the rank category (Faculty, Staff, Administrator, etc.).
    /// </summary>
    [Required(ErrorMessage = "Rank category is required")]
    [MaxLength(20, ErrorMessage = "Rank category cannot exceed 20 characters")]
    [RegularExpression(@"^(Faculty|Staff|Administrator|Research|Adjunct|Emeritus)$", ErrorMessage = "Rank category must be one of: Faculty, Staff, Administrator, Research, Adjunct, Emeritus")]
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether this rank requires tenure.
    /// </summary>
    [Required]
    public bool RequiresTenure { get; set; } = false;

    /// <summary>
    /// Gets or sets whether this rank is currently active and in use.
    /// </summary>
    [Required]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the minimum years of experience required for this rank.
    /// </summary>
    [Range(0, 50, ErrorMessage = "Minimum experience must be between 0 and 50 years")]
    public int? MinExperienceYears { get; set; }

    /// <summary>
    /// Gets or sets the minimum degree level required for this rank.
    /// </summary>
    [MaxLength(20, ErrorMessage = "Minimum degree level cannot exceed 20 characters")]
    [RegularExpression(@"^(Bachelor|Master|Doctorate)$", ErrorMessage = "Minimum degree level must be one of: Bachelor, Master, Doctorate")]
    public string? MinDegreeLevel { get; set; }

    /// <summary>
    /// Gets or sets whether this rank allows teaching responsibilities.
    /// </summary>
    [Required]
    public bool AllowsTeaching { get; set; } = true;

    /// <summary>
    /// Gets or sets whether this rank allows research responsibilities.
    /// </summary>
    [Required]
    public bool AllowsResearch { get; set; } = true;

    /// <summary>
    /// Navigation property for professors with this rank.
    /// </summary>
    public virtual ICollection<Professor> Professors { get; set; } = new List<Professor>();

    /// <summary>
    /// Navigation property for teaching professors with this rank.
    /// </summary>
    public virtual ICollection<TeachingProf> TeachingProfs { get; set; } = new List<TeachingProf>();
}