using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing committee chair assignments and management.
/// Manages the appointment and rotation of committee chairs with structured oversight.
/// </summary>
public class CommitteeChair : BaseEntity
{
    /// <summary>
    /// Gets or sets the committee name that this chair leads.
    /// </summary>
    [Required]
    [StringLength(50)]
    public string CommitteeName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the employee number of the faculty member serving as chair.
    /// </summary>
    [Required]
    public int ChairEmpNr { get; set; }

    /// <summary>
    /// Gets or sets the start date of the chair appointment.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime AppointmentStartDate { get; set; }

    /// <summary>
    /// Gets or sets the end date of the chair appointment.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? AppointmentEndDate { get; set; }

    /// <summary>
    /// Gets or sets whether this is the current active chair assignment.
    /// </summary>
    [Required]
    public bool IsCurrent { get; set; } = true;

    /// <summary>
    /// Gets or sets the term length in years for this chair position.
    /// </summary>
    [Range(1, 10)]
    public int TermLengthYears { get; set; } = 2;

    /// <summary>
    /// Gets or sets the appointment type (Elected, Appointed, Rotating, Interim).
    /// </summary>
    [Required]
    [StringLength(20)]
    public string AppointmentType { get; set; } = "Elected"; // Elected, Appointed, Rotating, Interim

    /// <summary>
    /// Gets or sets who appointed or confirmed this chair.
    /// </summary>
    [StringLength(100)]
    public string? AppointedBy { get; set; }

    /// <summary>
    /// Gets or sets the selection method (Faculty Vote, Administrative Appointment, Rotation).
    /// </summary>
    [StringLength(50)]
    public string? SelectionMethod { get; set; }

    /// <summary>
    /// Gets or sets the number of votes received if elected.
    /// </summary>
    [Range(0, 1000)]
    public int? VotesReceived { get; set; }

    /// <summary>
    /// Gets or sets the total number of votes cast in the election.
    /// </summary>
    [Range(0, 1000)]
    public int? TotalVotesCast { get; set; }

    /// <summary>
    /// Gets or sets whether the chair is eligible for reappointment/re-election.
    /// </summary>
    public bool IsEligibleForRenewal { get; set; } = true;

    /// <summary>
    /// Gets or sets the maximum number of consecutive terms allowed.
    /// </summary>
    [Range(1, 10)]
    public int? MaxConsecutiveTerms { get; set; }

    /// <summary>
    /// Gets or sets the current consecutive term number.
    /// </summary>
    [Range(1, 20)]
    public int ConsecutiveTermNumber { get; set; } = 1;

    /// <summary>
    /// Gets or sets the reason for appointment change.
    /// </summary>
    [StringLength(100)]
    public string? ChangeReason { get; set; }

    /// <summary>
    /// Gets or sets special responsibilities or focus areas for this chair term.
    /// </summary>
    [StringLength(1000)]
    public string? SpecialResponsibilities { get; set; }

    /// <summary>
    /// Gets or sets the committee's meeting frequency under this chair.
    /// </summary>
    [StringLength(50)]
    public string? MeetingFrequency { get; set; } // Weekly, Bi-weekly, Monthly, Quarterly, As-needed

    /// <summary>
    /// Gets or sets the preferred meeting format (In-person, Virtual, Hybrid).
    /// </summary>
    [StringLength(20)]
    public string? MeetingFormat { get; set; }

    /// <summary>
    /// Gets or sets the chair's leadership style or approach.
    /// </summary>
    [StringLength(500)]
    public string? LeadershipApproach { get; set; }

    /// <summary>
    /// Gets or sets key goals or initiatives for this chair term.
    /// </summary>
    [StringLength(2000)]
    public string? TermGoals { get; set; }

    /// <summary>
    /// Gets or sets transition notes from the previous chair.
    /// </summary>
    [StringLength(2000)]
    public string? TransitionNotes { get; set; }

    /// <summary>
    /// Gets or sets performance evaluation notes for the chair's service.
    /// </summary>
    [StringLength(2000)]
    public string? PerformanceNotes { get; set; }

    /// <summary>
    /// Gets or sets the date of the last performance review.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? LastReviewDate { get; set; }

    /// <summary>
    /// Gets or sets additional compensation or stipend for the chair role.
    /// </summary>
    [Column(TypeName = "decimal(10,2)")]
    public decimal? ChairStipend { get; set; }

    /// <summary>
    /// Gets or sets whether the chair receives administrative support.
    /// </summary>
    public bool ReceivesAdminSupport { get; set; } = false;

    /// <summary>
    /// Gets or sets contact information specific to the chair role.
    /// </summary>
    [StringLength(200)]
    public string? ChairContactInfo { get; set; }

    /// <summary>
    /// Navigation property to the committee.
    /// </summary>
    public virtual Committee Committee { get; set; } = null!;

    /// <summary>
    /// Navigation property to the faculty member serving as chair.
    /// </summary>
    public virtual Academic Chair { get; set; } = null!;

    // Computed properties
    /// <summary>
    /// Gets whether the chair appointment is currently active.
    /// </summary>
    [NotMapped]
    public bool IsActive => IsCurrent && (AppointmentEndDate == null || AppointmentEndDate > DateTime.Today);

    /// <summary>
    /// Gets the expected end date based on start date and term length.
    /// </summary>
    [NotMapped]
    public DateTime ExpectedEndDate => AppointmentStartDate.AddYears(TermLengthYears);

    /// <summary>
    /// Gets the number of days remaining in the current term.
    /// </summary>
    [NotMapped]
    public int DaysRemainingInTerm => AppointmentEndDate.HasValue
        ? Math.Max(0, (AppointmentEndDate.Value - DateTime.Today).Days)
        : Math.Max(0, (ExpectedEndDate - DateTime.Today).Days);

    /// <summary>
    /// Gets whether the chair term is nearing expiration.
    /// </summary>
    [NotMapped]
    public bool IsNearingExpiration => DaysRemainingInTerm <= 90 && DaysRemainingInTerm > 0;

    /// <summary>
    /// Gets whether this is an interim or temporary appointment.
    /// </summary>
    [NotMapped]
    public bool IsInterimAppointment => AppointmentType == "Interim";

    /// <summary>
    /// Gets the percentage of votes received if elected.
    /// </summary>
    [NotMapped]
    public decimal? VotePercentage => VotesReceived.HasValue && TotalVotesCast.HasValue && TotalVotesCast > 0
        ? Math.Round((decimal)VotesReceived.Value / TotalVotesCast.Value * 100, 2)
        : null;

    /// <summary>
    /// Gets whether the chair has reached the maximum consecutive terms.
    /// </summary>
    [NotMapped]
    public bool HasReachedTermLimit => MaxConsecutiveTerms.HasValue && ConsecutiveTermNumber >= MaxConsecutiveTerms.Value;

    /// <summary>
    /// Gets the total years of service as chair for this committee.
    /// </summary>
    [NotMapped]
    public int TotalYearsAsChair => (int)Math.Ceiling(((AppointmentEndDate ?? DateTime.Today) - AppointmentStartDate).TotalDays / 365.0);
}

/// <summary>
/// Entity representing enhanced committee membership with role-specific details.
/// Extends the existing CommitteeMember functionality for better management.
/// </summary>
public class CommitteeMemberAssignment : BaseEntity
{
    /// <summary>
    /// Gets or sets the committee name.
    /// </summary>
    [Required]
    [StringLength(50)]
    public string CommitteeName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the employee number of the committee member.
    /// </summary>
    [Required]
    public int MemberEmpNr { get; set; }

    /// <summary>
    /// Gets or sets the member's role on the committee.
    /// </summary>
    [Required]
    [StringLength(50)]
    public string MemberRole { get; set; } = "Member"; // Member, Secretary, Treasurer, Vice-Chair, Ex-Officio

    /// <summary>
    /// Gets or sets the appointment start date for this member.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime AppointmentStartDate { get; set; }

    /// <summary>
    /// Gets or sets the appointment end date for this member.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? AppointmentEndDate { get; set; }

    /// <summary>
    /// Gets or sets whether this is the current active membership.
    /// </summary>
    [Required]
    public bool IsCurrent { get; set; } = true;

    /// <summary>
    /// Gets or sets the term length in years for this membership.
    /// </summary>
    [Range(1, 10)]
    public int TermLengthYears { get; set; } = 3;

    /// <summary>
    /// Gets or sets how the member was selected (Elected, Appointed, Ex-Officio, Volunteer).
    /// </summary>
    [StringLength(20)]
    public string? SelectionMethod { get; set; }

    /// <summary>
    /// Gets or sets who appointed this member.
    /// </summary>
    [StringLength(100)]
    public string? AppointedBy { get; set; }

    /// <summary>
    /// Gets or sets whether the member is eligible for reappointment.
    /// </summary>
    public bool IsEligibleForRenewal { get; set; } = true;

    /// <summary>
    /// Gets or sets the consecutive term number for this member.
    /// </summary>
    [Range(1, 20)]
    public int ConsecutiveTermNumber { get; set; } = 1;

    /// <summary>
    /// Gets or sets the maximum consecutive terms allowed for this role.
    /// </summary>
    [Range(1, 10)]
    public int? MaxConsecutiveTerms { get; set; }

    /// <summary>
    /// Gets or sets the member's expertise or representation area.
    /// </summary>
    [StringLength(200)]
    public string? ExpertiseArea { get; set; }

    /// <summary>
    /// Gets or sets the department or unit the member represents.
    /// </summary>
    [StringLength(100)]
    public string? RepresentingUnit { get; set; }

    /// <summary>
    /// Gets or sets the member's attendance percentage.
    /// </summary>
    [Range(0, 100)]
    public decimal? AttendancePercentage { get; set; }

    /// <summary>
    /// Gets or sets the number of meetings attended.
    /// </summary>
    [Range(0, 1000)]
    public int MeetingsAttended { get; set; } = 0;

    /// <summary>
    /// Gets or sets the total number of meetings held during tenure.
    /// </summary>
    [Range(0, 1000)]
    public int TotalMeetingsHeld { get; set; } = 0;

    /// <summary>
    /// Gets or sets specific responsibilities for this member.
    /// </summary>
    [StringLength(1000)]
    public string? SpecificResponsibilities { get; set; }

    /// <summary>
    /// Gets or sets the member's preferred contact method.
    /// </summary>
    [StringLength(50)]
    public string? PreferredContactMethod { get; set; }

    /// <summary>
    /// Gets or sets availability constraints for this member.
    /// </summary>
    [StringLength(500)]
    public string? AvailabilityConstraints { get; set; }

    /// <summary>
    /// Gets or sets performance notes for this member's service.
    /// </summary>
    [StringLength(2000)]
    public string? PerformanceNotes { get; set; }

    /// <summary>
    /// Gets or sets the reason for membership change or termination.
    /// </summary>
    [StringLength(100)]
    public string? ChangeReason { get; set; }

    /// <summary>
    /// Navigation property to the committee.
    /// </summary>
    public virtual Committee Committee { get; set; } = null!;

    /// <summary>
    /// Navigation property to the faculty member.
    /// </summary>
    public virtual Academic Member { get; set; } = null!;

    // Computed properties
    /// <summary>
    /// Gets whether the membership is currently active.
    /// </summary>
    [NotMapped]
    public bool IsActive => IsCurrent && (AppointmentEndDate == null || AppointmentEndDate > DateTime.Today);

    /// <summary>
    /// Gets the expected end date based on start date and term length.
    /// </summary>
    [NotMapped]
    public DateTime ExpectedEndDate => AppointmentStartDate.AddYears(TermLengthYears);

    /// <summary>
    /// Gets the number of days remaining in the current term.
    /// </summary>
    [NotMapped]
    public int DaysRemainingInTerm => AppointmentEndDate.HasValue
        ? Math.Max(0, (AppointmentEndDate.Value - DateTime.Today).Days)
        : Math.Max(0, (ExpectedEndDate - DateTime.Today).Days);

    /// <summary>
    /// Gets whether the member's term is nearing expiration.
    /// </summary>
    [NotMapped]
    public bool IsNearingExpiration => DaysRemainingInTerm <= 90 && DaysRemainingInTerm > 0;

    /// <summary>
    /// Gets whether the member has reached the maximum consecutive terms.
    /// </summary>
    [NotMapped]
    public bool HasReachedTermLimit => MaxConsecutiveTerms.HasValue && ConsecutiveTermNumber >= MaxConsecutiveTerms.Value;

    /// <summary>
    /// Gets the calculated attendance percentage based on meetings attended.
    /// </summary>
    [NotMapped]
    public decimal CalculatedAttendancePercentage => TotalMeetingsHeld > 0
        ? Math.Round((decimal)MeetingsAttended / TotalMeetingsHeld * 100, 2)
        : 0;

    /// <summary>
    /// Gets whether this member has good attendance (>75%).
    /// </summary>
    [NotMapped]
    public bool HasGoodAttendance => CalculatedAttendancePercentage >= 75;

    /// <summary>
    /// Gets the total years of service on this committee.
    /// </summary>
    [NotMapped]
    public int TotalYearsOfService => (int)Math.Ceiling(((AppointmentEndDate ?? DateTime.Today) - AppointmentStartDate).TotalDays / 365.0);
}