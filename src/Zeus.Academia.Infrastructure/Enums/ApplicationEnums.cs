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

/// <summary>
/// Enumeration for preferred contact methods
/// </summary>
public enum ContactMethod
{
    /// <summary>
    /// Contact via phone call
    /// </summary>
    Phone = 0,

    /// <summary>
    /// Contact via email
    /// </summary>
    Email = 1,

    /// <summary>
    /// Contact via SMS text message
    /// </summary>
    SMS = 2,

    /// <summary>
    /// Contact via postal mail
    /// </summary>
    Mail = 3,

    /// <summary>
    /// No preferred method - any method acceptable
    /// </summary>
    Any = 4
}

/// <summary>
/// Enumeration for student document types
/// </summary>
public enum StudentDocumentType
{
    /// <summary>
    /// Student profile photo
    /// </summary>
    ProfilePhoto = 0,

    /// <summary>
    /// Official transcript
    /// </summary>
    Transcript = 1,

    /// <summary>
    /// Diploma or certificate
    /// </summary>
    Diploma = 2,

    /// <summary>
    /// Immigration document (visa, passport, etc.)
    /// </summary>
    Immigration = 3,

    /// <summary>
    /// Medical records or immunization
    /// </summary>
    Medical = 4,

    /// <summary>
    /// Financial aid document
    /// </summary>
    FinancialAid = 5,

    /// <summary>
    /// Letter of recommendation
    /// </summary>
    Recommendation = 6,

    /// <summary>
    /// Personal statement or essay
    /// </summary>
    PersonalStatement = 7,

    /// <summary>
    /// Resume or curriculum vitae
    /// </summary>
    Resume = 8,

    /// <summary>
    /// Portfolio or work samples
    /// </summary>
    Portfolio = 9,

    /// <summary>
    /// Other miscellaneous document
    /// </summary>
    Other = 10
}

/// <summary>
/// Enumeration for document access levels
/// </summary>
public enum DocumentAccessLevel
{
    /// <summary>
    /// Only the student can access
    /// </summary>
    StudentOnly = 0,

    /// <summary>
    /// Student and their advisor can access
    /// </summary>
    StudentAndAdvisor = 1,

    /// <summary>
    /// Student and department staff can access
    /// </summary>
    StudentAndDepartment = 2,

    /// <summary>
    /// Student and faculty can access
    /// </summary>
    StudentAndFaculty = 3,

    /// <summary>
    /// Administrative staff can access
    /// </summary>
    Administrative = 4,

    /// <summary>
    /// Public access (e.g., profile photos)
    /// </summary>
    Public = 5
}

/// <summary>
/// Enumeration for advisor types
/// </summary>
public enum AdvisorType
{
    /// <summary>
    /// Academic advisor for coursework and degree planning
    /// </summary>
    Academic = 0,

    /// <summary>
    /// Faculty advisor for research or thesis work
    /// </summary>
    Faculty = 1,

    /// <summary>
    /// Career counselor or advisor
    /// </summary>
    Career = 2,

    /// <summary>
    /// Financial aid advisor
    /// </summary>
    Financial = 3,

    /// <summary>
    /// Mental health or counseling advisor
    /// </summary>
    Counseling = 4,

    /// <summary>
    /// Disability services advisor
    /// </summary>
    Disability = 5,

    /// <summary>
    /// International student advisor
    /// </summary>
    International = 6
}