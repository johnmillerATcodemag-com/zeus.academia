using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing faculty search committees for hiring new faculty members.
/// Manages the formation, composition, and activities of faculty search committees.
/// </summary>
public class FacultySearchCommittee : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique search committee code.
    /// </summary>
    [Key]
    [Required]
    [StringLength(20)]
    public string SearchCommitteeCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the search committee name.
    /// </summary>
    [Required]
    [StringLength(200)]
    public string SearchCommitteeName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the position being searched for.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string PositionTitle { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the department conducting the search.
    /// </summary>
    [Required]
    [StringLength(15)]
    public string DepartmentName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the college or school conducting the search.
    /// </summary>
    [StringLength(100)]
    public string? College { get; set; }

    /// <summary>
    /// Gets or sets the position rank or level being hired.
    /// </summary>
    [Required]
    [StringLength(50)]
    public string PositionRank { get; set; } = string.Empty; // Assistant Professor, Associate Professor, Professor, Chair, etc.

    /// <summary>
    /// Gets or sets the position type (Tenure-Track, Clinical, Visiting, Adjunct).
    /// </summary>
    [Required]
    [StringLength(30)]
    public string PositionType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the employment type (Full-Time, Part-Time, Joint Appointment).
    /// </summary>
    [Required]
    [StringLength(20)]
    public string EmploymentType { get; set; } = "Full-Time";

    /// <summary>
    /// Gets or sets the academic year for this search.
    /// </summary>
    [Required]
    [StringLength(9)]
    public string AcademicYear { get; set; } = string.Empty; // e.g., "2024-2025"

    /// <summary>
    /// Gets or sets the search start date.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime SearchStartDate { get; set; }

    /// <summary>
    /// Gets or sets the planned search completion date.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? PlannedCompletionDate { get; set; }

    /// <summary>
    /// Gets or sets the actual search completion date.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? ActualCompletionDate { get; set; }

    /// <summary>
    /// Gets or sets the search status (Active, Completed, Cancelled, On Hold).
    /// </summary>
    [Required]
    [StringLength(20)]
    public string SearchStatus { get; set; } = "Active";

    /// <summary>
    /// Gets or sets the committee chair's employee number.
    /// </summary>
    [Required]
    public int ChairEmpNr { get; set; }

    /// <summary>
    /// Gets or sets the formation date of the committee.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime CommitteeFormationDate { get; set; }

    /// <summary>
    /// Gets or sets the number of committee members required.
    /// </summary>
    [Range(3, 15)]
    public int RequiredMemberCount { get; set; } = 5;

    /// <summary>
    /// Gets or sets the minimum number of external members required.
    /// </summary>
    [Range(0, 10)]
    public int MinExternalMembers { get; set; } = 1;

    /// <summary>
    /// Gets or sets whether diversity requirements must be met.
    /// </summary>
    public bool RequiresDiversityCompliance { get; set; } = true;

    /// <summary>
    /// Gets or sets the budget allocated for the search.
    /// </summary>
    [Column(TypeName = "decimal(15,2)")]
    public decimal? SearchBudget { get; set; }

    /// <summary>
    /// Gets or sets the salary range minimum for the position.
    /// </summary>
    [Column(TypeName = "decimal(12,2)")]
    public decimal? SalaryRangeMin { get; set; }

    /// <summary>
    /// Gets or sets the salary range maximum for the position.
    /// </summary>
    [Column(TypeName = "decimal(12,2)")]
    public decimal? SalaryRangeMax { get; set; }

    /// <summary>
    /// Gets or sets the job posting/advertisement text.
    /// </summary>
    [StringLength(5000)]
    public string? JobDescription { get; set; }

    /// <summary>
    /// Gets or sets the required qualifications.
    /// </summary>
    [StringLength(2000)]
    public string? RequiredQualifications { get; set; }

    /// <summary>
    /// Gets or sets the preferred qualifications.
    /// </summary>
    [StringLength(2000)]
    public string? PreferredQualifications { get; set; }

    /// <summary>
    /// Gets or sets the research areas of interest.
    /// </summary>
    [StringLength(1000)]
    public string? ResearchAreas { get; set; }

    /// <summary>
    /// Gets or sets the teaching responsibilities.
    /// </summary>
    [StringLength(1000)]
    public string? TeachingResponsibilities { get; set; }

    /// <summary>
    /// Gets or sets the service expectations.
    /// </summary>
    [StringLength(1000)]
    public string? ServiceExpectations { get; set; }

    /// <summary>
    /// Gets or sets the application deadline.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? ApplicationDeadline { get; set; }

    /// <summary>
    /// Gets or sets whether applications are still being accepted.
    /// </summary>
    public bool AcceptingApplications { get; set; } = true;

    /// <summary>
    /// Gets or sets the total number of applications received.
    /// </summary>
    [Range(0, 10000)]
    public int ApplicationsReceived { get; set; } = 0;

    /// <summary>
    /// Gets or sets the number of candidates invited for initial interviews.
    /// </summary>
    [Range(0, 100)]
    public int InitialInterviewsScheduled { get; set; } = 0;

    /// <summary>
    /// Gets or sets the number of candidates invited for campus visits.
    /// </summary>
    [Range(0, 20)]
    public int CampusVisitsScheduled { get; set; } = 0;

    /// <summary>
    /// Gets or sets the number of offers extended.
    /// </summary>
    [Range(0, 10)]
    public int OffersExtended { get; set; } = 0;

    /// <summary>
    /// Gets or sets the number of offers accepted.
    /// </summary>
    [Range(0, 10)]
    public int OffersAccepted { get; set; } = 0;

    /// <summary>
    /// Gets or sets whether the position was successfully filled.
    /// </summary>
    public bool? PositionFilled { get; set; }

    /// <summary>
    /// Gets or sets the employee number of the hired candidate.
    /// </summary>
    public int? HiredCandidateEmpNr { get; set; }

    /// <summary>
    /// Gets or sets the hire date for the successful candidate.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? HireDate { get; set; }

    /// <summary>
    /// Gets or sets the starting salary for the hired candidate.
    /// </summary>
    [Column(TypeName = "decimal(12,2)")]
    public decimal? StartingSalary { get; set; }

    /// <summary>
    /// Gets or sets the reason if the search was unsuccessful.
    /// </summary>
    [StringLength(500)]
    public string? UnsuccessfulReason { get; set; }

    /// <summary>
    /// Gets or sets whether the search will be reopened.
    /// </summary>
    public bool? WillReopenSearch { get; set; }

    /// <summary>
    /// Gets or sets compliance notes for diversity and equal opportunity.
    /// </summary>
    [StringLength(2000)]
    public string? ComplianceNotes { get; set; }

    /// <summary>
    /// Gets or sets the search timeline and milestones.
    /// </summary>
    [StringLength(2000)]
    public string? SearchTimeline { get; set; }

    /// <summary>
    /// Gets or sets the evaluation criteria used by the committee.
    /// </summary>
    [StringLength(2000)]
    public string? EvaluationCriteria { get; set; }

    /// <summary>
    /// Gets or sets notes about the search process and outcomes.
    /// </summary>
    [StringLength(5000)]
    public string? SearchNotes { get; set; }

    /// <summary>
    /// Gets or sets confidential notes accessible only to administrators.
    /// </summary>
    [StringLength(2000)]
    public string? ConfidentialNotes { get; set; }

    /// <summary>
    /// Navigation property to the department.
    /// </summary>
    public virtual Department Department { get; set; } = null!;

    /// <summary>
    /// Navigation property to the committee chair.
    /// </summary>
    public virtual Academic Chair { get; set; } = null!;

    /// <summary>
    /// Navigation property to the hired candidate.
    /// </summary>
    public virtual Academic? HiredCandidate { get; set; }

    /// <summary>
    /// Navigation property for search committee members.
    /// </summary>
    public virtual ICollection<FacultySearchCommitteeMember> Members { get; set; } = new List<FacultySearchCommitteeMember>();

    // Computed properties
    /// <summary>
    /// Gets whether the search is currently active.
    /// </summary>
    [NotMapped]
    public bool IsActive => SearchStatus == "Active" && (ActualCompletionDate == null);

    /// <summary>
    /// Gets whether the application deadline has passed.
    /// </summary>
    [NotMapped]
    public bool IsApplicationDeadlinePast => ApplicationDeadline.HasValue && ApplicationDeadline.Value < DateTime.Today;

    /// <summary>
    /// Gets the search duration in days.
    /// </summary>
    [NotMapped]
    public int SearchDurationDays => ActualCompletionDate.HasValue
        ? (ActualCompletionDate.Value - SearchStartDate).Days
        : (DateTime.Today - SearchStartDate).Days;

    /// <summary>
    /// Gets the success rate based on applications to hires.
    /// </summary>
    [NotMapped]
    public decimal SuccessRate => ApplicationsReceived > 0
        ? Math.Round((decimal)OffersAccepted / ApplicationsReceived * 100, 2)
        : 0;

    /// <summary>
    /// Gets whether the search is overdue based on planned completion date.
    /// </summary>
    [NotMapped]
    public bool IsOverdue => PlannedCompletionDate.HasValue
        && PlannedCompletionDate.Value < DateTime.Today
        && ActualCompletionDate == null;

    /// <summary>
    /// Gets the salary range as a formatted string.
    /// </summary>
    [NotMapped]
    public string SalaryRangeDisplay => SalaryRangeMin.HasValue && SalaryRangeMax.HasValue
        ? $"${SalaryRangeMin:N0} - ${SalaryRangeMax:N0}"
        : "Competitive";

    /// <summary>
    /// Gets whether the position is tenure-track.
    /// </summary>
    [NotMapped]
    public bool IsTenureTrack => PositionType.Contains("Tenure");
}

/// <summary>
/// Entity representing members of faculty search committees.
/// Manages committee membership, roles, and participation in faculty searches.
/// </summary>
public class FacultySearchCommitteeMember : BaseEntity
{
    /// <summary>
    /// Gets or sets the search committee code.
    /// </summary>
    [Required]
    [StringLength(20)]
    public string SearchCommitteeCode { get; set; } = string.Empty;

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
    public string MemberRole { get; set; } = "Member"; // Chair, Member, External, Graduate Student, Staff Representative

    /// <summary>
    /// Gets or sets the member's appointment date to the committee.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime AppointmentDate { get; set; }

    /// <summary>
    /// Gets or sets whether this member is currently active on the committee.
    /// </summary>
    [Required]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets whether this member is external to the department.
    /// </summary>
    public bool IsExternalMember { get; set; } = false;

    /// <summary>
    /// Gets or sets the external member's department or affiliation.
    /// </summary>
    [StringLength(100)]
    public string? ExternalAffiliation { get; set; }

    /// <summary>
    /// Gets or sets the member's area of expertise relevant to the search.
    /// </summary>
    [StringLength(200)]
    public string? ExpertiseArea { get; set; }

    /// <summary>
    /// Gets or sets the member's representation category (Faculty, Staff, Student, External).
    /// </summary>
    [Required]
    [StringLength(20)]
    public string RepresentationCategory { get; set; } = "Faculty";

    /// <summary>
    /// Gets or sets the member's rank or title.
    /// </summary>
    [StringLength(50)]
    public string? MemberRank { get; set; }

    /// <summary>
    /// Gets or sets who appointed this member to the committee.
    /// </summary>
    [StringLength(100)]
    public string? AppointedBy { get; set; }

    /// <summary>
    /// Gets or sets the reason for member selection.
    /// </summary>
    [StringLength(200)]
    public string? SelectionReason { get; set; }

    /// <summary>
    /// Gets or sets whether the member attended diversity training.
    /// </summary>
    public bool AttendedDiversityTraining { get; set; } = false;

    /// <summary>
    /// Gets or sets the diversity training completion date.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? DiversityTrainingDate { get; set; }

    /// <summary>
    /// Gets or sets whether the member signed confidentiality agreements.
    /// </summary>
    public bool SignedConfidentialityAgreement { get; set; } = false;

    /// <summary>
    /// Gets or sets the date confidentiality agreement was signed.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? ConfidentialityAgreementDate { get; set; }

    /// <summary>
    /// Gets or sets the member's contact information during the search.
    /// </summary>
    [StringLength(200)]
    public string? ContactInformation { get; set; }

    /// <summary>
    /// Gets or sets the member's availability constraints.
    /// </summary>
    [StringLength(500)]
    public string? AvailabilityConstraints { get; set; }

    /// <summary>
    /// Gets or sets the number of search meetings attended.
    /// </summary>
    [Range(0, 100)]
    public int MeetingsAttended { get; set; } = 0;

    /// <summary>
    /// Gets or sets the total number of search meetings held.
    /// </summary>
    [Range(0, 100)]
    public int TotalMeetingsHeld { get; set; } = 0;

    /// <summary>
    /// Gets or sets the number of candidate interviews participated in.
    /// </summary>
    [Range(0, 1000)]
    public int InterviewsParticipated { get; set; } = 0;

    /// <summary>
    /// Gets or sets whether the member participated in campus visit evaluations.
    /// </summary>
    public bool ParticipatedInCampusVisits { get; set; } = false;

    /// <summary>
    /// Gets or sets the member's evaluation of the search process.
    /// </summary>
    [StringLength(1000)]
    public string? ProcessEvaluation { get; set; }

    /// <summary>
    /// Gets or sets any conflicts of interest declared by the member.
    /// </summary>
    [StringLength(500)]
    public string? ConflictsOfInterest { get; set; }

    /// <summary>
    /// Gets or sets the date the member left the committee (if applicable).
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? DepartureDate { get; set; }

    /// <summary>
    /// Gets or sets the reason for leaving the committee.
    /// </summary>
    [StringLength(200)]
    public string? DepartureReason { get; set; }

    /// <summary>
    /// Gets or sets performance notes for this member's service.
    /// </summary>
    [StringLength(1000)]
    public string? PerformanceNotes { get; set; }

    /// <summary>
    /// Navigation property to the search committee.
    /// </summary>
    public virtual FacultySearchCommittee SearchCommittee { get; set; } = null!;

    /// <summary>
    /// Navigation property to the committee member.
    /// </summary>
    public virtual Academic Member { get; set; } = null!;

    // Computed properties
    /// <summary>
    /// Gets the attendance percentage for committee meetings.
    /// </summary>
    [NotMapped]
    public decimal AttendancePercentage => TotalMeetingsHeld > 0
        ? Math.Round((decimal)MeetingsAttended / TotalMeetingsHeld * 100, 2)
        : 0;

    /// <summary>
    /// Gets whether the member has good attendance (>75%).
    /// </summary>
    [NotMapped]
    public bool HasGoodAttendance => AttendancePercentage >= 75;

    /// <summary>
    /// Gets whether the member is compliant with training requirements.
    /// </summary>
    [NotMapped]
    public bool IsTrainingCompliant => AttendedDiversityTraining && SignedConfidentialityAgreement;

    /// <summary>
    /// Gets whether this member is currently serving on the committee.
    /// </summary>
    [NotMapped]
    public bool IsCurrentlyServing => IsActive && DepartureDate == null;

    /// <summary>
    /// Gets the total days of service on this committee.
    /// </summary>
    [NotMapped]
    public int DaysOfService => ((DepartureDate ?? DateTime.Today) - AppointmentDate).Days;
}