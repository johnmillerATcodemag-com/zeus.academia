using System.ComponentModel.DataAnnotations;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Base entity for all academic personnel in the system.
/// Serves as the parent class for Professor, Teacher, Chair, TeachingProf, and Student.
/// </summary>
public abstract class Academic : BaseEntity
{
    /// <summary>
    /// Gets or sets the employee number - the primary identifier for academic personnel.
    /// </summary>
    [Key]
    [Required]
    public int EmpNr { get; set; }

    /// <summary>
    /// Gets or sets the name of the academic person.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the phone number of the academic person.
    /// </summary>
    [MaxLength(15)]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets the salary of the academic person.
    /// </summary>
    public decimal? Salary { get; set; }

    /// <summary>
    /// Gets the department ID from the associated department (if available).
    /// This property provides a way to access the department's integer ID for authorization purposes.
    /// The actual foreign key relationship uses DepartmentName in derived classes.
    /// </summary>
    public virtual int? DepartmentId
    {
        get
        {
            // This will be overridden in derived classes that have Department navigation properties
            return null;
        }
    }

    /// <summary>
    /// Navigation property for relationships and derived types.
    /// </summary>
    public virtual ICollection<AcademicDegree> AcademicDegrees { get; set; } = new List<AcademicDegree>();

    /// <summary>
    /// Navigation property for faculty employment history records.
    /// </summary>
    public virtual ICollection<FacultyEmploymentHistory> EmploymentHistory { get; set; } = new List<FacultyEmploymentHistory>();

    /// <summary>
    /// Navigation property for faculty promotion records.
    /// </summary>
    public virtual ICollection<FacultyPromotion> Promotions { get; set; } = new List<FacultyPromotion>();

    /// <summary>
    /// Navigation property for faculty research expertise.
    /// </summary>
    public virtual ICollection<FacultyExpertise> ResearchExpertise { get; set; } = new List<FacultyExpertise>();

    /// <summary>
    /// Navigation property for faculty service records.
    /// </summary>
    public virtual ICollection<FacultyServiceRecord> ServiceRecords { get; set; } = new List<FacultyServiceRecord>();

    /// <summary>
    /// Navigation property for committee leadership positions.
    /// </summary>
    public virtual ICollection<CommitteeLeadership> CommitteeLeaderships { get; set; } = new List<CommitteeLeadership>();

    /// <summary>
    /// Navigation property for academic ranks held by this academic.
    /// </summary>
    public virtual ICollection<AcademicRank> AcademicRanks { get; set; } = new List<AcademicRank>();

    /// <summary>
    /// Navigation property for promotion applications submitted by this academic.
    /// </summary>
    public virtual ICollection<PromotionApplication> PromotionApplications { get; set; } = new List<PromotionApplication>();

    /// <summary>
    /// Navigation property for tenure track records for this academic.
    /// </summary>
    public virtual ICollection<TenureTrack> TenureTracks { get; set; } = new List<TenureTrack>();

    /// <summary>
    /// Navigation property for promotion committees chaired by this academic.
    /// </summary>
    public virtual ICollection<PromotionCommittee> ChairedPromotionCommittees { get; set; } = new List<PromotionCommittee>();

    /// <summary>
    /// Navigation property for promotion committee memberships.
    /// </summary>
    public virtual ICollection<PromotionCommitteeMember> PromotionCommitteeMemberships { get; set; } = new List<PromotionCommitteeMember>();

    /// <summary>
    /// Navigation property for promotion votes cast by this academic.
    /// </summary>
    public virtual ICollection<PromotionVote> PromotionVotes { get; set; } = new List<PromotionVote>();

    /// <summary>
    /// Navigation property for promotion workflow steps assigned to this academic.
    /// </summary>
    public virtual ICollection<PromotionWorkflowStep> AssignedPromotionSteps { get; set; } = new List<PromotionWorkflowStep>();

    /// <summary>
    /// Navigation property for tenure milestones reviewed by this academic.
    /// </summary>
    public virtual ICollection<TenureMilestone> ReviewedTenureMilestones { get; set; } = new List<TenureMilestone>();

    // ========== Prompt 5 Task 4: Department Assignment and Administration Navigation Properties ==========

    /// <summary>
    /// Navigation property for department chair assignments held by this academic.
    /// </summary>
    public virtual ICollection<DepartmentChair> DepartmentChairAssignments { get; set; } = new List<DepartmentChair>();

    /// <summary>
    /// Navigation property for committee chair positions held by this academic.
    /// </summary>
    public virtual ICollection<CommitteeChair> CommitteeChairAssignments { get; set; } = new List<CommitteeChair>();

    /// <summary>
    /// Navigation property for committee member assignments for this academic.
    /// </summary>
    public virtual ICollection<CommitteeMemberAssignment> CommitteeMemberAssignments { get; set; } = new List<CommitteeMemberAssignment>();

    /// <summary>
    /// Navigation property for administrative role assignments held by this academic.
    /// </summary>
    public virtual ICollection<AdministrativeAssignment> AdministrativeAssignments { get; set; } = new List<AdministrativeAssignment>();

    /// <summary>
    /// Navigation property for faculty search committees chaired by this academic.
    /// </summary>
    public virtual ICollection<FacultySearchCommittee> FacultySearchCommitteesAsChair { get; set; } = new List<FacultySearchCommittee>();

    /// <summary>
    /// Navigation property for faculty search committee memberships.
    /// </summary>
    public virtual ICollection<FacultySearchCommitteeMember> FacultySearchCommitteeMemberships { get; set; } = new List<FacultySearchCommitteeMember>();

    /// <summary>
    /// Navigation property for departmental services assigned to this academic.
    /// </summary>
    public virtual ICollection<DepartmentalService> DepartmentalServices { get; set; } = new List<DepartmentalService>();

    /// <summary>
    /// Navigation property for service load summaries for this academic.
    /// </summary>
    public virtual ICollection<ServiceLoadSummary> ServiceLoadSummaries { get; set; } = new List<ServiceLoadSummary>();
}

/// <summary>
/// Entity representing the relationship between Academic and their degrees from universities.
/// </summary>
public class AcademicDegree : BaseEntity
{
    /// <summary>
    /// Gets or sets the academic's employee number.
    /// </summary>
    [Required]
    public int AcademicEmpNr { get; set; }

    /// <summary>
    /// Gets or sets the degree code.
    /// </summary>
    [Required]
    [MaxLength(10)]
    public string DegreeCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the university code.
    /// </summary>
    [Required]
    [MaxLength(10)]
    public string UniversityCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date when the degree was obtained.
    /// </summary>
    public DateTime? DateObtained { get; set; }

    /// <summary>
    /// Navigation property to the Academic.
    /// </summary>
    public virtual Academic Academic { get; set; } = null!;

    /// <summary>
    /// Navigation property to the Degree.
    /// </summary>
    public virtual Degree Degree { get; set; } = null!;

    /// <summary>
    /// Navigation property to the University.
    /// </summary>
    public virtual University University { get; set; } = null!;
}