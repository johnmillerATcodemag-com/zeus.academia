namespace Zeus.Academia.Infrastructure.Enums;

/// <summary>
/// Enumeration for application status
/// </summary>
public enum ApplicationStatus
{
    /// <summary>
    /// Application has been submitted but not yet reviewed
    /// </summary>
    Submitted = 0,

    /// <summary>
    /// Application is under review by admissions committee
    /// </summary>
    UnderReview = 1,

    /// <summary>
    /// Application requires additional documents or information
    /// </summary>
    IncompleteDocuments = 2,

    /// <summary>
    /// Application is on hold pending additional review
    /// </summary>
    OnHold = 3,

    /// <summary>
    /// Application has been approved for admission
    /// </summary>
    Approved = 4,

    /// <summary>
    /// Application has been rejected
    /// </summary>
    Rejected = 5,

    /// <summary>
    /// Application has been withdrawn by the applicant
    /// </summary>
    Withdrawn = 6,

    /// <summary>
    /// Application has expired due to inactivity
    /// </summary>
    Expired = 7
}

/// <summary>
/// Enumeration for admission decision
/// </summary>
public enum AdmissionDecision
{
    /// <summary>
    /// Student has been admitted with full acceptance
    /// </summary>
    Admitted = 0,

    /// <summary>
    /// Student has been conditionally admitted (conditions must be met)
    /// </summary>
    ConditionallyAdmitted = 1,

    /// <summary>
    /// Student has been placed on the waiting list
    /// </summary>
    Waitlisted = 2,

    /// <summary>
    /// Application has been rejected
    /// </summary>
    Rejected = 3,

    /// <summary>
    /// Application has been deferred to a later term
    /// </summary>
    Deferred = 4
}

/// <summary>
/// Enumeration for application priority
/// </summary>
public enum ApplicationPriority
{
    /// <summary>
    /// Low priority application
    /// </summary>
    Low = 0,

    /// <summary>
    /// Normal priority application
    /// </summary>
    Normal = 1,

    /// <summary>
    /// High priority application (e.g., early admission, scholarship candidate)
    /// </summary>
    High = 2,

    /// <summary>
    /// Urgent priority application (e.g., late application with special circumstances)
    /// </summary>
    Urgent = 3
}

/// <summary>
/// Enumeration for term types
/// </summary>
public enum TermType
{
    /// <summary>
    /// Fall semester/term
    /// </summary>
    Fall = 0,

    /// <summary>
    /// Spring semester/term
    /// </summary>
    Spring = 1,

    /// <summary>
    /// Summer semester/term
    /// </summary>
    Summer = 2,

    /// <summary>
    /// Winter intersession term
    /// </summary>
    Winter = 3,

    /// <summary>
    /// Full academic year
    /// </summary>
    FullYear = 4
}

/// <summary>
/// Enumeration for enrollment history event types
/// </summary>
public enum EnrollmentEventType
{
    /// <summary>
    /// Application submitted
    /// </summary>
    ApplicationSubmitted = 0,

    /// <summary>
    /// Application reviewed
    /// </summary>
    ApplicationReviewed = 1,

    /// <summary>
    /// Admission decision made
    /// </summary>
    AdmissionDecision = 2,

    /// <summary>
    /// Student enrolled
    /// </summary>
    Enrolled = 3,

    /// <summary>
    /// Enrollment status changed
    /// </summary>
    StatusChanged = 4,

    /// <summary>
    /// Student graduated
    /// </summary>
    Graduated = 5,

    /// <summary>
    /// Student withdrew
    /// </summary>
    Withdrew = 6,

    /// <summary>
    /// Student suspended
    /// </summary>
    Suspended = 7,

    /// <summary>
    /// Student dismissed
    /// </summary>
    Dismissed = 8,

    /// <summary>
    /// Student took leave of absence
    /// </summary>
    LeaveOfAbsence = 9,

    /// <summary>
    /// Student returned from leave
    /// </summary>
    ReturnedFromLeave = 10
}