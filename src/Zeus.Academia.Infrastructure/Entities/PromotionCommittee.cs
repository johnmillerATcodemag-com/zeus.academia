using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing a promotion committee responsible for reviewing faculty promotion applications.
/// Manages committee composition, voting, and recommendation tracking.
/// </summary>
public class PromotionCommittee : BaseEntity
{
    /// <summary>
    /// Gets or sets the name of the promotion committee.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string CommitteeName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type or level of the committee.
    /// </summary>
    [Required]
    [StringLength(50)]
    public string CommitteeType { get; set; } = string.Empty; // Department, College, University, External

    /// <summary>
    /// Gets or sets the academic year this committee serves.
    /// </summary>
    [Required]
    [StringLength(9)]
    public string AcademicYear { get; set; } = string.Empty; // e.g., "2024-2025"

    /// <summary>
    /// Gets or sets the department code if this is a department-level committee.
    /// </summary>
    [StringLength(10)]
    public string? DepartmentCode { get; set; }

    /// <summary>
    /// Gets or sets the college code if this is a college-level committee.
    /// </summary>
    [StringLength(10)]
    public string? CollegeCode { get; set; }

    /// <summary>
    /// Gets or sets the committee chair's employee number.
    /// </summary>
    [Required]
    public int ChairEmpNr { get; set; }

    /// <summary>
    /// Gets or sets the committee formation date.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime FormationDate { get; set; }

    /// <summary>
    /// Gets or sets the committee dissolution date.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? DissolutionDate { get; set; }

    /// <summary>
    /// Gets or sets whether the committee is currently active.
    /// </summary>
    [Required]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the minimum number of members required for the committee.
    /// </summary>
    public int MinimumMembers { get; set; } = 3;

    /// <summary>
    /// Gets or sets the maximum number of members allowed on the committee.
    /// </summary>
    public int MaximumMembers { get; set; } = 7;

    /// <summary>
    /// Gets or sets the quorum requirement for voting.
    /// </summary>
    public int QuorumRequirement { get; set; } = 3;

    /// <summary>
    /// Gets or sets the voting threshold required for approval (as percentage).
    /// </summary>
    [Column(TypeName = "decimal(5,2)")]
    public decimal VotingThreshold { get; set; } = 50.0m; // 50% for simple majority, 66.67% for 2/3 majority

    /// <summary>
    /// Gets or sets the committee's scope or jurisdiction.
    /// </summary>
    [StringLength(500)]
    public string? CommitteeScope { get; set; }

    /// <summary>
    /// Gets or sets special expertise or requirements for committee members.
    /// </summary>
    [StringLength(500)]
    public string? MembershipRequirements { get; set; }

    /// <summary>
    /// Gets or sets the meeting schedule or frequency.
    /// </summary>
    [StringLength(200)]
    public string? MeetingSchedule { get; set; }

    /// <summary>
    /// Gets or sets the next scheduled meeting date.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? NextMeetingDate { get; set; }

    /// <summary>
    /// Gets or sets confidentiality and conflict of interest guidelines.
    /// </summary>
    [StringLength(1000)]
    public string? ConfidentialityGuidelines { get; set; }

    /// <summary>
    /// Gets or sets the committee's current workload (number of active cases).
    /// </summary>
    public int CurrentWorkload { get; set; } = 0;

    /// <summary>
    /// Gets or sets the maximum workload capacity.
    /// </summary>
    public int MaxWorkloadCapacity { get; set; } = 10;

    /// <summary>
    /// Gets or sets contact information for the committee.
    /// </summary>
    [StringLength(200)]
    public string? ContactInformation { get; set; }

    /// <summary>
    /// Gets or sets additional notes about the committee.
    /// </summary>
    [StringLength(1000)]
    public string? Notes { get; set; }

    // Navigation properties
    /// <summary>
    /// Gets or sets the committee chair.
    /// </summary>
    [ForeignKey(nameof(ChairEmpNr))]
    public Academic Chair { get; set; } = null!;

    /// <summary>
    /// Gets or sets the collection of committee members.
    /// </summary>
    public ICollection<PromotionCommitteeMember> Members { get; set; } = new List<PromotionCommitteeMember>();

    /// <summary>
    /// Gets or sets the collection of promotion applications assigned to this committee.
    /// </summary>
    public ICollection<PromotionApplication> PromotionApplications { get; set; } = new List<PromotionApplication>();

    /// <summary>
    /// Gets or sets the collection of academic ranks approved by this committee.
    /// </summary>
    public ICollection<AcademicRank> ApprovedRanks { get; set; } = new List<AcademicRank>();

    // Computed properties
    /// <summary>
    /// Gets the current number of active members on the committee.
    /// </summary>
    [NotMapped]
    public int CurrentMemberCount => Members?.Count(m => m.IsActive) ?? 0;

    /// <summary>
    /// Gets whether the committee has enough members to function.
    /// </summary>
    [NotMapped]
    public bool HasSufficientMembers => CurrentMemberCount >= MinimumMembers;

    /// <summary>
    /// Gets whether the committee has reached its maximum capacity.
    /// </summary>
    [NotMapped]
    public bool IsAtCapacity => CurrentMemberCount >= MaximumMembers;

    /// <summary>
    /// Gets the committee's workload utilization percentage.
    /// </summary>
    [NotMapped]
    public decimal WorkloadUtilization => MaxWorkloadCapacity > 0
        ? Math.Min((decimal)CurrentWorkload / MaxWorkloadCapacity * 100, 100)
        : 0;

    /// <summary>
    /// Gets whether the committee is overloaded.
    /// </summary>
    [NotMapped]
    public bool IsOverloaded => CurrentWorkload > MaxWorkloadCapacity;

    /// <summary>
    /// Gets whether the committee can accept new cases.
    /// </summary>
    [NotMapped]
    public bool CanAcceptNewCases => IsActive &&
                                    HasSufficientMembers &&
                                    CurrentWorkload < MaxWorkloadCapacity;

    /// <summary>
    /// Gets the committee's effectiveness status.
    /// </summary>
    [NotMapped]
    public string EffectivenessStatus
    {
        get
        {
            if (!IsActive) return "Inactive";
            if (!HasSufficientMembers) return "Understaffed";
            if (IsOverloaded) return "Overloaded";
            if (WorkloadUtilization > 80) return "Busy";
            return "Available";
        }
    }

    /// <summary>
    /// Gets the full committee description.
    /// </summary>
    [NotMapped]
    public string FullDescription => !string.IsNullOrEmpty(DepartmentCode)
        ? $"{CommitteeName} ({DepartmentCode} - {AcademicYear})"
        : $"{CommitteeName} ({AcademicYear})";

    /// <summary>
    /// Gets whether quorum can be achieved with current membership.
    /// </summary>
    [NotMapped]
    public bool CanAchieveQuorum => CurrentMemberCount >= QuorumRequirement;
}