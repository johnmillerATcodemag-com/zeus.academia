using System.ComponentModel.DataAnnotations;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing a Teaching Professor in the academic system.
/// Based on the ORM model, TeachingProf is a subtype of both Teacher and Professor.
/// This represents academics who have both teaching and professorial responsibilities.
/// </summary>
public class TeachingProf : Academic
{
    /// <summary>
    /// Gets or sets the academic rank of the teaching professor.
    /// </summary>
    [MaxLength(10)]
    public string? RankCode { get; set; }

    /// <summary>
    /// Gets or sets the department code where the teaching professor works.
    /// </summary>
    [MaxLength(15)]
    public string? DepartmentName { get; set; }

    /// <summary>
    /// Gets or sets whether the teaching professor has tenure.
    /// </summary>
    public bool? HasTenure { get; set; }

    /// <summary>
    /// Gets or sets the teaching professor's research area or specialty.
    /// </summary>
    [MaxLength(100)]
    public string? ResearchArea { get; set; }

    /// <summary>
    /// Gets or sets the teaching professor's specialization or subject area.
    /// </summary>
    [MaxLength(100)]
    public string? Specialization { get; set; }

    /// <summary>
    /// Gets or sets the employment type (full-time, part-time, etc.).
    /// </summary>
    [MaxLength(20)]
    public string? EmploymentType { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of courses the teaching professor can teach per semester.
    /// </summary>
    public int? MaxCourseLoad { get; set; }

    /// <summary>
    /// Gets or sets the percentage of time dedicated to teaching vs research.
    /// </summary>
    public decimal? TeachingPercentage { get; set; }

    /// <summary>
    /// Gets or sets the percentage of time dedicated to research vs teaching.
    /// </summary>
    public decimal? ResearchPercentage { get; set; }

    /// <summary>
    /// Navigation property to the department.
    /// </summary>
    public virtual Department? Department { get; set; }

    /// <summary>
    /// Gets the department ID from the associated department.
    /// </summary>
    public override int? DepartmentId => Department?.Id;

    /// <summary>
    /// Navigation property to the rank.
    /// </summary>
    public virtual Rank? Rank { get; set; }

    /// <summary>
    /// Navigation property for committees where this teaching professor serves.
    /// </summary>
    public virtual ICollection<CommitteeMember> CommitteeMembers { get; set; } = new List<CommitteeMember>();

    /// <summary>
    /// Navigation property for subjects this teaching professor teaches.
    /// </summary>
    public virtual ICollection<Teaching> Teachings { get; set; } = new List<Teaching>();

    /// <summary>
    /// Navigation property for ratings given to the teaching professor.
    /// </summary>
    public virtual ICollection<TeacherRating> TeacherRatings { get; set; } = new List<TeacherRating>();
}