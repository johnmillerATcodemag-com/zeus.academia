using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing a member of a promotion committee.
/// Manages committee membership, roles, and participation tracking.
/// </summary>
public class PromotionCommitteeMember : BaseEntity
{
    /// <summary>
    /// Gets or sets the promotion committee ID.
    /// </summary>
    [Required]
    public int PromotionCommitteeId { get; set; }

    /// <summary>
    /// Gets or sets the employee number of the committee member.
    /// </summary>
    [Required]
    public int EmpNr { get; set; }

    /// <summary>
    /// Gets or sets the member's role on the committee.
    /// </summary>
    [Required]
    [StringLength(50)]
    public string MemberRole { get; set; } = string.Empty; // Chair, Member, Secretary, External, Ex-Officio

    /// <summary>
    /// Gets or sets the member's appointment date to the committee.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime AppointmentDate { get; set; }

    /// <summary>
    /// Gets or sets the member's term end date.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? TermEndDate { get; set; }

    /// <summary>
    /// Gets or sets whether the member is currently active.
    /// </summary>
    [Required]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the member's status.
    /// </summary>
    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "Active"; // Active, Inactive, Recused, On Leave

    /// <summary>
    /// Gets or sets the member's voting privileges.
    /// </summary>
    [Required]
    public bool HasVotingPrivileges { get; set; } = true;

    /// <summary>
    /// Gets or sets the member's expertise areas.
    /// </summary>
    [StringLength(500)]
    public string? ExpertiseAreas { get; set; }

    /// <summary>
    /// Gets or sets the member's department affiliation.
    /// </summary>
    [StringLength(10)]
    public string? DepartmentCode { get; set; }

    /// <summary>
    /// Gets or sets the member's college affiliation.
    /// </summary>
    [StringLength(10)]
    public string? CollegeCode { get; set; }

    /// <summary>
    /// Gets or sets whether the member is external to the institution.
    /// </summary>
    public bool IsExternalMember { get; set; } = false;

    /// <summary>
    /// Gets or sets the external member's institution.
    /// </summary>
    [StringLength(200)]
    public string? ExternalInstitution { get; set; }

    /// <summary>
    /// Gets or sets the member's rank or title.
    /// </summary>
    [StringLength(100)]
    public string? MemberRank { get; set; }

    /// <summary>
    /// Gets or sets conflict of interest declarations.
    /// </summary>
    [StringLength(1000)]
    public string? ConflictOfInterestDeclaration { get; set; }

    /// <summary>
    /// Gets or sets the training completion date for committee service.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? TrainingCompletionDate { get; set; }

    /// <summary>
    /// Gets or sets the member's confidentiality agreement status.
    /// </summary>
    public bool ConfidentialityAgreementSigned { get; set; } = false;

    /// <summary>
    /// Gets or sets the attendance rate for committee meetings.
    /// </summary>
    [Column(TypeName = "decimal(5,2)")]
    public decimal AttendanceRate { get; set; } = 100.0m;

    /// <summary>
    /// Gets or sets the number of meetings attended.
    /// </summary>
    public int MeetingsAttended { get; set; } = 0;

    /// <summary>
    /// Gets or sets the total number of meetings scheduled during membership.
    /// </summary>
    public int TotalMeetingsScheduled { get; set; } = 0;

    /// <summary>
    /// Gets or sets the workload assigned to this member.
    /// </summary>
    public int AssignedCases { get; set; } = 0;

    /// <summary>
    /// Gets or sets the number of cases completed by this member.
    /// </summary>
    public int CompletedCases { get; set; } = 0;

    /// <summary>
    /// Gets or sets the member's preferred contact method.
    /// </summary>
    [StringLength(50)]
    public string? PreferredContactMethod { get; set; } = "Email";

    /// <summary>
    /// Gets or sets emergency contact information.
    /// </summary>
    [StringLength(200)]
    public string? EmergencyContact { get; set; }

    /// <summary>
    /// Gets or sets notes about the member's service.
    /// </summary>
    [StringLength(1000)]
    public string? ServiceNotes { get; set; }

    // Navigation properties
    /// <summary>
    /// Gets or sets the promotion committee.
    /// </summary>
    [ForeignKey(nameof(PromotionCommitteeId))]
    public PromotionCommittee PromotionCommittee { get; set; } = null!;

    /// <summary>
    /// Gets or sets the academic member.
    /// </summary>
    [ForeignKey(nameof(EmpNr))]
    public Academic Academic { get; set; } = null!;

    /// <summary>
    /// Gets or sets the collection of votes cast by this member.
    /// </summary>
    public ICollection<PromotionVote> Votes { get; set; } = new List<PromotionVote>();

    // Computed properties
    /// <summary>
    /// Gets whether the member's term is current.
    /// </summary>
    [NotMapped]
    public bool IsCurrentTerm => IsActive &&
                                (TermEndDate == null || TermEndDate >= DateTime.Today);

    /// <summary>
    /// Gets the member's service duration in days.
    /// </summary>
    [NotMapped]
    public int ServiceDurationDays => (int)(DateTime.Today - AppointmentDate).TotalDays;

    /// <summary>
    /// Gets the member's service duration in years.
    /// </summary>
    [NotMapped]
    public decimal ServiceDurationYears => ServiceDurationDays / 365.25m;

    /// <summary>
    /// Gets whether the member has completed required training.
    /// </summary>
    [NotMapped]
    public bool HasCompletedTraining => TrainingCompletionDate != null &&
                                       TrainingCompletionDate <= DateTime.Today;

    /// <summary>
    /// Gets whether the member is ready for service.
    /// </summary>
    [NotMapped]
    public bool IsReadyForService => IsActive &&
                                    IsCurrentTerm &&
                                    HasCompletedTraining &&
                                    ConfidentialityAgreementSigned;

    /// <summary>
    /// Gets the member's performance score based on attendance and completion rate.
    /// </summary>
    [NotMapped]
    public decimal PerformanceScore
    {
        get
        {
            var completionRate = AssignedCases > 0 ? (decimal)CompletedCases / AssignedCases * 100 : 100;
            return (AttendanceRate + completionRate) / 2;
        }
    }

    /// <summary>
    /// Gets the member's performance rating.
    /// </summary>
    [NotMapped]
    public string PerformanceRating
    {
        get
        {
            var score = PerformanceScore;
            return score switch
            {
                >= 90 => "Excellent",
                >= 80 => "Good",
                >= 70 => "Satisfactory",
                >= 60 => "Needs Improvement",
                _ => "Unsatisfactory"
            };
        }
    }

    /// <summary>
    /// Gets the member's full description with role and affiliation.
    /// </summary>
    [NotMapped]
    public string FullDescription
    {
        get
        {
            var description = $"{MemberRole}";
            if (!string.IsNullOrEmpty(DepartmentCode))
                description += $" ({DepartmentCode})";
            if (IsExternalMember && !string.IsNullOrEmpty(ExternalInstitution))
                description += $" - {ExternalInstitution}";
            return description;
        }
    }

    /// <summary>
    /// Gets whether the member has potential conflicts of interest.
    /// </summary>
    [NotMapped]
    public bool HasPotentialConflicts => !string.IsNullOrEmpty(ConflictOfInterestDeclaration);

    /// <summary>
    /// Gets the member's current availability status.
    /// </summary>
    [NotMapped]
    public string AvailabilityStatus
    {
        get
        {
            if (!IsActive) return "Inactive";
            if (!IsCurrentTerm) return "Term Expired";
            if (!IsReadyForService) return "Not Ready";
            if (Status != "Active") return Status;
            return "Available";
        }
    }
}