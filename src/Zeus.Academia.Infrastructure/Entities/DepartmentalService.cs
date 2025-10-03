using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing departmental service assignments and tracking.
/// Manages faculty service obligations, workload distribution, and reporting for department operations.
/// </summary>
public class DepartmentalService : BaseEntity
{
    /// <summary>
    /// Gets or sets the employee number of the faculty member providing service.
    /// </summary>
    [Required]
    public int FacultyEmpNr { get; set; }

    /// <summary>
    /// Gets or sets the department where service is provided.
    /// </summary>
    [Required]
    [StringLength(15)]
    public string DepartmentName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the service type (Committee, Administrative, Editorial, External).
    /// </summary>
    [Required]
    [StringLength(30)]
    public string ServiceType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the specific service title or position.
    /// </summary>
    [Required]
    [StringLength(200)]
    public string ServiceTitle { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the service level (Department, College, University, Professional, External).
    /// </summary>
    [Required]
    [StringLength(20)]
    public string ServiceLevel { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the service category for reporting purposes.
    /// </summary>
    [StringLength(50)]
    public string? ServiceCategory { get; set; } // Governance, Curriculum, Search, Accreditation, etc.

    /// <summary>
    /// Gets or sets the organization or entity being served.
    /// </summary>
    [StringLength(200)]
    public string? Organization { get; set; }

    /// <summary>
    /// Gets or sets the start date of the service assignment.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime ServiceStartDate { get; set; }

    /// <summary>
    /// Gets or sets the end date of the service assignment.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? ServiceEndDate { get; set; }

    /// <summary>
    /// Gets or sets whether this service assignment is currently active.
    /// </summary>
    [Required]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the academic year for this service.
    /// </summary>
    [Required]
    [StringLength(9)]
    public string AcademicYear { get; set; } = string.Empty; // e.g., "2024-2025"

    /// <summary>
    /// Gets or sets the service role (Chair, Member, Secretary, Coordinator, etc.).
    /// </summary>
    [StringLength(50)]
    public string? ServiceRole { get; set; }

    /// <summary>
    /// Gets or sets the estimated hours per year for this service.
    /// </summary>
    [Column(TypeName = "decimal(8,2)")]
    public decimal? EstimatedHoursPerYear { get; set; }

    /// <summary>
    /// Gets or sets the actual hours logged for this service.
    /// </summary>
    [Column(TypeName = "decimal(8,2)")]
    public decimal? ActualHoursLogged { get; set; }

    /// <summary>
    /// Gets or sets the service weight or credit value.
    /// </summary>
    [Column(TypeName = "decimal(5,2)")]
    public decimal? ServiceWeight { get; set; } // For workload calculations

    /// <summary>
    /// Gets or sets the appointment method (Elected, Appointed, Volunteer, Assigned).
    /// </summary>
    [StringLength(20)]
    public string? AppointmentMethod { get; set; }

    /// <summary>
    /// Gets or sets who made the appointment or assignment.
    /// </summary>
    [StringLength(100)]
    public string? AppointedBy { get; set; }

    /// <summary>
    /// Gets or sets the confirmation or acceptance date.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? ConfirmationDate { get; set; }

    /// <summary>
    /// Gets or sets whether the service includes leadership responsibilities.
    /// </summary>
    public bool IncludesLeadership { get; set; } = false;

    /// <summary>
    /// Gets or sets whether the service involves budget oversight.
    /// </summary>
    public bool InvolvesBudgetOversight { get; set; } = false;

    /// <summary>
    /// Gets or sets the budget or resources managed (if applicable).
    /// </summary>
    [Column(TypeName = "decimal(15,2)")]
    public decimal? BudgetOversight { get; set; }

    /// <summary>
    /// Gets or sets whether the service involves personnel decisions.
    /// </summary>
    public bool InvolvestPersonnelDecisions { get; set; } = false;

    /// <summary>
    /// Gets or sets whether the service involves student interactions.
    /// </summary>
    public bool InvolvesStudentInteractions { get; set; } = false;

    /// <summary>
    /// Gets or sets whether the service involves external stakeholders.
    /// </summary>
    public bool InvolvesExternalStakeholders { get; set; } = false;

    /// <summary>
    /// Gets or sets the meeting frequency for this service.
    /// </summary>
    [StringLength(50)]
    public string? MeetingFrequency { get; set; } // Weekly, Monthly, Quarterly, As-needed

    /// <summary>
    /// Gets or sets the number of meetings attended.
    /// </summary>
    [Range(0, 1000)]
    public int MeetingsAttended { get; set; } = 0;

    /// <summary>
    /// Gets or sets the total number of meetings scheduled.
    /// </summary>
    [Range(0, 1000)]
    public int TotalMeetingsScheduled { get; set; } = 0;

    /// <summary>
    /// Gets or sets whether additional compensation is provided.
    /// </summary>
    public bool ReceivesCompensation { get; set; } = false;

    /// <summary>
    /// Gets or sets the compensation amount.
    /// </summary>
    [Column(TypeName = "decimal(10,2)")]
    public decimal? CompensationAmount { get; set; }

    /// <summary>
    /// Gets or sets whether course release is provided for this service.
    /// </summary>
    public bool ReceivesCourseRelease { get; set; } = false;

    /// <summary>
    /// Gets or sets the number of courses released.
    /// </summary>
    [Range(0, 6)]
    public int CourseReleaseCount { get; set; } = 0;

    /// <summary>
    /// Gets or sets whether administrative support is provided.
    /// </summary>
    public bool ReceivesAdminSupport { get; set; } = false;

    /// <summary>
    /// Gets or sets the specific administrative support provided.
    /// </summary>
    [StringLength(500)]
    public string? AdminSupportDetails { get; set; }

    /// <summary>
    /// Gets or sets key responsibilities for this service role.
    /// </summary>
    [StringLength(2000)]
    public string? KeyResponsibilities { get; set; }

    /// <summary>
    /// Gets or sets major accomplishments during this service period.
    /// </summary>
    [StringLength(2000)]
    public string? MajorAccomplishments { get; set; }

    /// <summary>
    /// Gets or sets challenges faced during this service assignment.
    /// </summary>
    [StringLength(1000)]
    public string? ChallengesFaced { get; set; }

    /// <summary>
    /// Gets or sets skills or expertise gained from this service.
    /// </summary>
    [StringLength(1000)]
    public string? SkillsGained { get; set; }

    /// <summary>
    /// Gets or sets the performance evaluation for this service.
    /// </summary>
    [StringLength(2000)]
    public string? PerformanceEvaluation { get; set; }

    /// <summary>
    /// Gets or sets the date of the last performance review.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime? LastReviewDate { get; set; }

    /// <summary>
    /// Gets or sets recognition or awards received for this service.
    /// </summary>
    [StringLength(500)]
    public string? RecognitionReceived { get; set; }

    /// <summary>
    /// Gets or sets the impact or outcomes of this service.
    /// </summary>
    [StringLength(2000)]
    public string? ServiceImpact { get; set; }

    /// <summary>
    /// Gets or sets whether this service counts toward promotion requirements.
    /// </summary>
    public bool CountsTowardPromotion { get; set; } = true;

    /// <summary>
    /// Gets or sets whether this service fulfills tenure requirements.
    /// </summary>
    public bool FulfillsTenureRequirements { get; set; } = true;

    /// <summary>
    /// Gets or sets the annual review rating for this service.
    /// </summary>
    [Range(1, 5)]
    public int? AnnualReviewRating { get; set; } // 1=Unsatisfactory, 5=Outstanding

    /// <summary>
    /// Gets or sets whether the faculty member would serve again in this capacity.
    /// </summary>
    public bool? WillServeAgain { get; set; }

    /// <summary>
    /// Gets or sets recommendations for future service assignments.
    /// </summary>
    [StringLength(1000)]
    public string? FutureServiceRecommendations { get; set; }

    /// <summary>
    /// Gets or sets the reason for service termination (if applicable).
    /// </summary>
    [StringLength(200)]
    public string? TerminationReason { get; set; }

    /// <summary>
    /// Gets or sets transition notes for successor or replacement.
    /// </summary>
    [StringLength(2000)]
    public string? TransitionNotes { get; set; }

    /// <summary>
    /// Gets or sets general notes about this service assignment.
    /// </summary>
    [StringLength(2000)]
    public string? ServiceNotes { get; set; }

    /// <summary>
    /// Navigation property to the faculty member providing service.
    /// </summary>
    public virtual Academic Faculty { get; set; } = null!;

    /// <summary>
    /// Navigation property to the department.
    /// </summary>
    public virtual Department Department { get; set; } = null!;

    // Computed properties
    /// <summary>
    /// Gets whether the service assignment is currently active.
    /// </summary>
    [NotMapped]
    public bool IsCurrentlyActive => IsActive && (ServiceEndDate == null || ServiceEndDate > DateTime.Today);

    /// <summary>
    /// Gets the total duration of service in days.
    /// </summary>
    [NotMapped]
    public int ServiceDurationDays => ((ServiceEndDate ?? DateTime.Today) - ServiceStartDate).Days;

    /// <summary>
    /// Gets the total years of service.
    /// </summary>
    [NotMapped]
    public decimal ServiceDurationYears => Math.Round((decimal)ServiceDurationDays / 365.25m, 2);

    /// <summary>
    /// Gets the attendance percentage for meetings.
    /// </summary>
    [NotMapped]
    public decimal AttendancePercentage => TotalMeetingsScheduled > 0
        ? Math.Round((decimal)MeetingsAttended / TotalMeetingsScheduled * 100, 2)
        : 0;

    /// <summary>
    /// Gets whether the faculty member has good attendance (>75%).
    /// </summary>
    [NotMapped]
    public bool HasGoodAttendance => AttendancePercentage >= 75;

    /// <summary>
    /// Gets the variance between estimated and actual hours.
    /// </summary>
    [NotMapped]
    public decimal? HoursVariance => EstimatedHoursPerYear.HasValue && ActualHoursLogged.HasValue
        ? ActualHoursLogged.Value - EstimatedHoursPerYear.Value
        : null;

    /// <summary>
    /// Gets whether the service involves significant time commitment (>100 hours/year).
    /// </summary>
    [NotMapped]
    public bool IsSignificantTimeCommitment => EstimatedHoursPerYear >= 100 || ActualHoursLogged >= 100;

    /// <summary>
    /// Gets whether this is a leadership service role.
    /// </summary>
    [NotMapped]
    public bool IsLeadershipRole => IncludesLeadership || ServiceRole?.Contains("Chair") == true || ServiceRole?.Contains("Director") == true;

    /// <summary>
    /// Gets the service load score for workload calculations.
    /// </summary>
    [NotMapped]
    public decimal ServiceLoadScore => ServiceWeight ?? (EstimatedHoursPerYear / 40) ?? 1.0m; // Default calculation

    /// <summary>
    /// Gets whether this service is eligible for performance bonus.
    /// </summary>
    [NotMapped]
    public bool IsEligibleForBonus => AnnualReviewRating >= 4 && IsLeadershipRole && ServiceDurationYears >= 1;

    /// <summary>
    /// Gets the service level priority for scheduling (higher values = higher priority).
    /// </summary>
    [NotMapped]
    public int ServicePriority => ServiceLevel switch
    {
        "University" => 4,
        "College" => 3,
        "Department" => 2,
        "Professional" => 1,
        _ => 0
    };
}

/// <summary>
/// Entity representing service load summary for faculty members by academic year.
/// Provides aggregated view of service obligations for workload balancing and reporting.
/// </summary>
public class ServiceLoadSummary : BaseEntity
{
    /// <summary>
    /// Gets or sets the employee number of the faculty member.
    /// </summary>
    [Required]
    public int FacultyEmpNr { get; set; }

    /// <summary>
    /// Gets or sets the department name.
    /// </summary>
    [Required]
    [StringLength(15)]
    public string DepartmentName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the academic year for this summary.
    /// </summary>
    [Required]
    [StringLength(9)]
    public string AcademicYear { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the total number of active service assignments.
    /// </summary>
    [Range(0, 50)]
    public int TotalServiceAssignments { get; set; } = 0;

    /// <summary>
    /// Gets or sets the number of leadership service roles.
    /// </summary>
    [Range(0, 20)]
    public int LeadershipRoles { get; set; } = 0;

    /// <summary>
    /// Gets or sets the number of committee memberships.
    /// </summary>
    [Range(0, 30)]
    public int CommitteeMemberships { get; set; } = 0;

    /// <summary>
    /// Gets or sets the number of external service commitments.
    /// </summary>
    [Range(0, 20)]
    public int ExternalServiceCommitments { get; set; } = 0;

    /// <summary>
    /// Gets or sets the total estimated hours per year across all services.
    /// </summary>
    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalEstimatedHours { get; set; } = 0;

    /// <summary>
    /// Gets or sets the total actual hours logged across all services.
    /// </summary>
    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalActualHours { get; set; } = 0;

    /// <summary>
    /// Gets or sets the total service weight across all assignments.
    /// </summary>
    [Column(TypeName = "decimal(8,2)")]
    public decimal TotalServiceWeight { get; set; } = 0;

    /// <summary>
    /// Gets or sets the total compensation received for service.
    /// </summary>
    [Column(TypeName = "decimal(12,2)")]
    public decimal TotalServiceCompensation { get; set; } = 0;

    /// <summary>
    /// Gets or sets the total course releases received for service.
    /// </summary>
    [Range(0, 12)]
    public int TotalCourseReleases { get; set; } = 0;

    /// <summary>
    /// Gets or sets the service load category (Light, Normal, Heavy, Overloaded).
    /// </summary>
    [Required]
    [StringLength(20)]
    public string ServiceLoadCategory { get; set; } = "Normal";

    /// <summary>
    /// Gets or sets the department's service load ranking for this faculty member.
    /// </summary>
    [Range(1, 100)]
    public int? DepartmentServiceRanking { get; set; }

    /// <summary>
    /// Gets or sets whether the service load is balanced according to department standards.
    /// </summary>
    public bool IsServiceLoadBalanced { get; set; } = true;

    /// <summary>
    /// Gets or sets recommendations for service load adjustment.
    /// </summary>
    [StringLength(1000)]
    public string? ServiceLoadRecommendations { get; set; }

    /// <summary>
    /// Gets or sets the average annual review rating across all services.
    /// </summary>
    [Column(TypeName = "decimal(3,2)")]
    public decimal? AverageServiceRating { get; set; }

    /// <summary>
    /// Gets or sets the service excellence awards or recognition received.
    /// </summary>
    [StringLength(500)]
    public string? ServiceRecognition { get; set; }

    /// <summary>
    /// Gets or sets the date this summary was calculated.
    /// </summary>
    [Column(TypeName = "date")]
    public DateTime SummaryCalculationDate { get; set; } = DateTime.Today;

    /// <summary>
    /// Gets or sets general notes about the faculty member's service contributions.
    /// </summary>
    [StringLength(2000)]
    public string? ServiceContributionNotes { get; set; }

    /// <summary>
    /// Navigation property to the faculty member.
    /// </summary>
    public virtual Academic Faculty { get; set; } = null!;

    /// <summary>
    /// Navigation property to the department.
    /// </summary>
    public virtual Department Department { get; set; } = null!;

    // Computed properties
    /// <summary>
    /// Gets the service hours variance between estimated and actual.
    /// </summary>
    [NotMapped]
    public decimal ServiceHoursVariance => TotalActualHours - TotalEstimatedHours;

    /// <summary>
    /// Gets the average hours per service assignment.
    /// </summary>
    [NotMapped]
    public decimal AverageHoursPerAssignment => TotalServiceAssignments > 0
        ? Math.Round(TotalEstimatedHours / TotalServiceAssignments, 2)
        : 0;

    /// <summary>
    /// Gets whether the faculty member is service-heavy.
    /// </summary>
    [NotMapped]
    public bool IsServiceHeavy => ServiceLoadCategory == "Heavy" || ServiceLoadCategory == "Overloaded";

    /// <summary>
    /// Gets whether the faculty member has leadership responsibilities.
    /// </summary>
    [NotMapped]
    public bool HasLeadershipResponsibilities => LeadershipRoles > 0;

    /// <summary>
    /// Gets the service diversity score (different types of service).
    /// </summary>
    [NotMapped]
    public decimal ServiceDiversityScore => (CommitteeMemberships + ExternalServiceCommitments + LeadershipRoles) / 3.0m;

    /// <summary>
    /// Gets whether service recognition was received this year.
    /// </summary>
    [NotMapped]
    public bool ReceivedServiceRecognition => !string.IsNullOrWhiteSpace(ServiceRecognition);
}