namespace Zeus.Academia.Infrastructure.Enums;

/// <summary>
/// Logical operators for combining prerequisite requirements
/// </summary>
public enum PrerequisiteLogicOperator
{
    /// <summary>
    /// All requirements must be satisfied
    /// </summary>
    AND = 1,

    /// <summary>
    /// At least one requirement must be satisfied
    /// </summary>
    OR = 2,

    /// <summary>
    /// Exactly one requirement must be satisfied
    /// </summary>
    XOR = 3
}

/// <summary>
/// Class standing levels for students
/// </summary>
public enum ClassStanding
{
    /// <summary>
    /// First-year student (0-29 credit hours)
    /// </summary>
    Freshman = 1,

    /// <summary>
    /// Second-year student (30-59 credit hours)
    /// </summary>
    Sophomore = 2,

    /// <summary>
    /// Third-year student (60-89 credit hours)
    /// </summary>
    Junior = 3,

    /// <summary>
    /// Fourth-year student (90+ credit hours)
    /// </summary>
    Senior = 4,

    /// <summary>
    /// Graduate student
    /// </summary>
    Graduate = 5,

    /// <summary>
    /// Post-baccalaureate student
    /// </summary>
    PostBaccalaureate = 6,

    /// <summary>
    /// Doctoral student
    /// </summary>
    Doctoral = 7
}

/// <summary>
/// Corequisite enforcement types
/// </summary>
public enum CorequisiteEnforcementType
{
    /// <summary>
    /// Must enroll in corequisite courses at the same time
    /// </summary>
    MustTakeSimultaneously = 1,

    /// <summary>
    /// Must complete corequisite before or with this course
    /// </summary>
    MustTakeBeforeOrWith = 2,

    /// <summary>
    /// Recommended to take together but not required
    /// </summary>
    RecommendedTogether = 3,

    /// <summary>
    /// Strongly recommended but not enforced
    /// </summary>
    StronglyRecommended = 4
}

/// <summary>
/// Relationship types for corequisite courses
/// </summary>
public enum CorequisiteRelationship
{
    /// <summary>
    /// Must enroll in both courses simultaneously
    /// </summary>
    MustEnrollSimultaneously = 1,

    /// <summary>
    /// Must complete before or enroll concurrently
    /// </summary>
    MustCompleteBeforeOrWith = 2,

    /// <summary>
    /// Recommended to take concurrently
    /// </summary>
    RecommendedConcurrent = 3,

    /// <summary>
    /// Should be taken in sequence
    /// </summary>
    PreferredSequence = 4
}

/// <summary>
/// Actions to take when corequisite requirements fail
/// </summary>
public enum CorequisiteFailureAction
{
    /// <summary>
    /// Block enrollment completely
    /// </summary>
    BlockEnrollment = 1,

    /// <summary>
    /// Require advisor approval
    /// </summary>
    RequireAdvisorApproval = 2,

    /// <summary>
    /// Require department permission
    /// </summary>
    RequireDepartmentPermission = 3,

    /// <summary>
    /// Allow with warning
    /// </summary>
    AllowWithWarning = 4,

    /// <summary>
    /// Generate notification only
    /// </summary>
    NotificationOnly = 5
}

/// <summary>
/// Enforcement levels for restrictions
/// </summary>
public enum RestrictionEnforcementLevel
{
    /// <summary>
    /// Hard block - cannot be overridden
    /// </summary>
    Hard = 1,

    /// <summary>
    /// Requires override approval
    /// </summary>
    RequiresOverride = 2,

    /// <summary>
    /// Warning only
    /// </summary>
    Warning = 3,

    /// <summary>
    /// Information only
    /// </summary>
    Information = 4
}

/// <summary>
/// Types of majors for restriction purposes
/// </summary>
public enum MajorType
{
    /// <summary>
    /// Primary/main major
    /// </summary>
    Primary = 1,

    /// <summary>
    /// Secondary major
    /// </summary>
    Secondary = 2,

    /// <summary>
    /// Minor program
    /// </summary>
    Minor = 3,

    /// <summary>
    /// Certificate program
    /// </summary>
    Certificate = 4,

    /// <summary>
    /// Concentration within major
    /// </summary>
    Concentration = 5
}

/// <summary>
/// Permission levels for authorization
/// </summary>
public enum PermissionLevel
{
    /// <summary>
    /// Instructor permission
    /// </summary>
    Instructor = 1,

    /// <summary>
    /// Department permission
    /// </summary>
    Department = 2,

    /// <summary>
    /// College permission
    /// </summary>
    College = 3,

    /// <summary>
    /// Academic affairs permission
    /// </summary>
    AcademicAffairs = 4,

    /// <summary>
    /// Registrar permission
    /// </summary>
    Registrar = 5,

    /// <summary>
    /// Advisor permission
    /// </summary>
    Advisor = 6
}

/// <summary>
/// Status of prerequisite validation
/// </summary>
public enum PrerequisiteValidationStatus
{
    /// <summary>
    /// All requirements satisfied
    /// </summary>
    Satisfied = 1,

    /// <summary>
    /// Requirements not met
    /// </summary>
    Failed = 2,

    /// <summary>
    /// Requirements partially met
    /// </summary>
    PartiallyMet = 3,

    /// <summary>
    /// Pending validation
    /// </summary>
    Pending = 4,

    /// <summary>
    /// Validation error occurred
    /// </summary>
    Error = 5,

    /// <summary>
    /// Requirements waived
    /// </summary>
    Waived = 6
}

/// <summary>
/// Status of individual requirement validation
/// </summary>
public enum RequirementStatus
{
    /// <summary>
    /// Requirement is satisfied
    /// </summary>
    Satisfied = 1,

    /// <summary>
    /// Requirement is not satisfied
    /// </summary>
    NotSatisfied = 2,

    /// <summary>
    /// Requirement is in progress
    /// </summary>
    InProgress = 3,

    /// <summary>
    /// Requirement has been waived
    /// </summary>
    Waived = 4,

    /// <summary>
    /// Requirement has been overridden
    /// </summary>
    Overridden = 5
}

/// <summary>
/// Priority levels for missing requirements
/// </summary>
public enum RequirementPriority
{
    /// <summary>
    /// Critical requirement - must be completed
    /// </summary>
    Critical = 1,

    /// <summary>
    /// High priority requirement
    /// </summary>
    High = 2,

    /// <summary>
    /// Medium priority requirement
    /// </summary>
    Medium = 3,

    /// <summary>
    /// Low priority requirement
    /// </summary>
    Low = 4,

    /// <summary>
    /// Optional requirement
    /// </summary>
    Optional = 5
}

/// <summary>
/// Types of prerequisite overrides
/// </summary>
public enum OverrideType
{
    /// <summary>
    /// Administrative override by authorized personnel
    /// </summary>
    AdministrativeOverride = 1,

    /// <summary>
    /// Academic advisor override
    /// </summary>
    AdvisorOverride = 2,

    /// <summary>
    /// Department chair override
    /// </summary>
    DepartmentOverride = 3,

    /// <summary>
    /// Instructor override
    /// </summary>
    InstructorOverride = 4,

    /// <summary>
    /// Emergency override
    /// </summary>
    EmergencyOverride = 5,

    /// <summary>
    /// System override for special circumstances
    /// </summary>
    SystemOverride = 6
}

/// <summary>
/// Status of override requests
/// </summary>
public enum OverrideStatus
{
    /// <summary>
    /// Override request pending review
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Override approved and active
    /// </summary>
    Approved = 2,

    /// <summary>
    /// Override denied
    /// </summary>
    Denied = 3,

    /// <summary>
    /// Override expired
    /// </summary>
    Expired = 4,

    /// <summary>
    /// Override revoked
    /// </summary>
    Revoked = 5,

    /// <summary>
    /// Override under review
    /// </summary>
    UnderReview = 6
}

/// <summary>
/// Types of waivers
/// </summary>
public enum WaiverType
{
    /// <summary>
    /// Academic waiver for equivalent experience
    /// </summary>
    AcademicException = 1,

    /// <summary>
    /// Transfer credit waiver
    /// </summary>
    TransferCredit = 2,

    /// <summary>
    /// Professional experience waiver
    /// </summary>
    ProfessionalExperience = 3,

    /// <summary>
    /// Military experience waiver
    /// </summary>
    MilitaryExperience = 4,

    /// <summary>
    /// Medical/health related waiver
    /// </summary>
    MedicalWaiver = 5,

    /// <summary>
    /// Hardship waiver
    /// </summary>
    HardshipWaiver = 6
}

/// <summary>
/// Status of waiver requests
/// </summary>
public enum WaiverStatus
{
    /// <summary>
    /// Waiver request pending
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Waiver approved
    /// </summary>
    Approved = 2,

    /// <summary>
    /// Waiver denied
    /// </summary>
    Denied = 3,

    /// <summary>
    /// Waiver under committee review
    /// </summary>
    UnderReview = 4,

    /// <summary>
    /// Additional documentation required
    /// </summary>
    DocumentationRequired = 5,

    /// <summary>
    /// Waiver expired
    /// </summary>
    Expired = 6
}

/// <summary>
/// Types of waiver documents
/// </summary>
public enum WaiverDocumentType
{
    /// <summary>
    /// Official transcript
    /// </summary>
    Transcript = 1,

    /// <summary>
    /// Course equivalency documentation
    /// </summary>
    CourseEquivalency = 2,

    /// <summary>
    /// Professional certification
    /// </summary>
    ProfessionalCertification = 3,

    /// <summary>
    /// Work experience documentation
    /// </summary>
    WorkExperience = 4,

    /// <summary>
    /// Military records
    /// </summary>
    MilitaryRecords = 5,

    /// <summary>
    /// Medical documentation
    /// </summary>
    MedicalDocumentation = 6,

    /// <summary>
    /// Supporting letter
    /// </summary>
    SupportingLetter = 7
}

/// <summary>
/// Enrollment validation status
/// </summary>
public enum EnrollmentValidationStatus
{
    /// <summary>
    /// Enrollment approved
    /// </summary>
    Approved = 1,

    /// <summary>
    /// Enrollment denied
    /// </summary>
    Denied = 2,

    /// <summary>
    /// Enrollment requires approval
    /// </summary>
    RequiresApproval = 3,

    /// <summary>
    /// Enrollment on waitlist
    /// </summary>
    Waitlisted = 4,

    /// <summary>
    /// Enrollment pending prerequisites
    /// </summary>
    PendingPrerequisites = 5,

    /// <summary>
    /// Enrollment blocked by restrictions
    /// </summary>
    Blocked = 6
}

/// <summary>
/// Overall validation status for prerequisite checks.
/// </summary>
public enum ValidationStatus
{
    /// <summary>
    /// All requirements are satisfied.
    /// </summary>
    Satisfied = 1,

    /// <summary>
    /// Some requirements are not satisfied.
    /// </summary>
    NotSatisfied = 2,

    /// <summary>
    /// Requirements are partially satisfied.
    /// </summary>
    PartiallySatisfied = 3,

    /// <summary>
    /// Validation was overridden by administrator.
    /// </summary>
    Overridden = 4,

    /// <summary>
    /// Requirements were waived.
    /// </summary>
    Waived = 5,

    /// <summary>
    /// Validation is pending approval.
    /// </summary>
    PendingApproval = 6,

    /// <summary>
    /// An error occurred during validation.
    /// </summary>
    ValidationError = 7
}

/// <summary>
/// Status for individual prerequisite checks.
/// </summary>
public enum PrerequisiteCheckStatus
{
    /// <summary>
    /// Prerequisite requirement is satisfied.
    /// </summary>
    Satisfied = 1,

    /// <summary>
    /// Prerequisite requirement is not satisfied.
    /// </summary>
    NotSatisfied = 2,

    /// <summary>
    /// Prerequisite requirement is partially satisfied.
    /// </summary>
    PartiallySatisfied = 3,

    /// <summary>
    /// Prerequisite check was skipped due to override.
    /// </summary>
    Skipped = 4,

    /// <summary>
    /// Error occurred during prerequisite check.
    /// </summary>
    Error = 5
}

/// <summary>
/// Status for individual corequisite checks.
/// </summary>
public enum CorequisiteCheckStatus
{
    /// <summary>
    /// Corequisite requirement is satisfied.
    /// </summary>
    Satisfied = 1,

    /// <summary>
    /// Corequisite requirement is not satisfied.
    /// </summary>
    NotSatisfied = 2,

    /// <summary>
    /// Corequisite check was skipped due to waiver.
    /// </summary>
    Waived = 3,

    /// <summary>
    /// Error occurred during corequisite check.
    /// </summary>
    Error = 4
}

/// <summary>
/// Status for individual restriction checks.
/// </summary>
public enum RestrictionCheckStatus
{
    /// <summary>
    /// No restriction violation found.
    /// </summary>
    NoViolation = 1,

    /// <summary>
    /// Restriction violation found.
    /// </summary>
    Violation = 2,

    /// <summary>
    /// Restriction was overridden.
    /// </summary>
    Overridden = 3,

    /// <summary>
    /// Error occurred during restriction check.
    /// </summary>
    Error = 4
}

/// <summary>
/// Severity levels for restriction violations.
/// </summary>
public enum RestrictionViolationSeverity
{
    /// <summary>
    /// Warning level - enrollment may proceed with notification.
    /// </summary>
    Warning = 1,

    /// <summary>
    /// Error level - enrollment blocked but can be overridden.
    /// </summary>
    Error = 2,

    /// <summary>
    /// Critical level - enrollment blocked, requires high-level override.
    /// </summary>
    Critical = 3,

    /// <summary>
    /// Fatal level - enrollment absolutely blocked.
    /// </summary>
    Fatal = 4
}

/// <summary>
/// Severity levels for circular dependency detection.
/// </summary>
public enum CircularDependencySeverity
{
    /// <summary>
    /// Minor circular dependency that can be resolved automatically.
    /// </summary>
    Minor = 1,

    /// <summary>
    /// Moderate circular dependency requiring manual review.
    /// </summary>
    Moderate = 2,

    /// <summary>
    /// Severe circular dependency blocking course setup.
    /// </summary>
    Severe = 3,

    /// <summary>
    /// Critical circular dependency requiring immediate attention.
    /// </summary>
    Critical = 4
}

/// <summary>
/// Scope of override application.
/// </summary>
public enum OverrideScope
{
    /// <summary>
    /// Override applies to a single course.
    /// </summary>
    SingleCourse = 1,

    /// <summary>
    /// Override applies to all courses for the student.
    /// </summary>
    AllCourses = 2,

    /// <summary>
    /// Override applies to courses in a specific department.
    /// </summary>
    Department = 3,

    /// <summary>
    /// Override applies to a specific term.
    /// </summary>
    Term = 4,

    /// <summary>
    /// Override applies globally for the student.
    /// </summary>
    Global = 5
}

/// <summary>
/// Authority levels for approvals.
/// </summary>
public enum AuthorityLevel
{
    /// <summary>
    /// Department-level authority.
    /// </summary>
    Department = 1,

    /// <summary>
    /// College-level authority.
    /// </summary>
    College = 2,

    /// <summary>
    /// University-level authority.
    /// </summary>
    University = 3,

    /// <summary>
    /// Dean-level authority.
    /// </summary>
    Dean = 4,

    /// <summary>
    /// Provost-level authority.
    /// </summary>
    Provost = 5,

    /// <summary>
    /// President-level authority.
    /// </summary>
    President = 6
}

/// <summary>
/// Scope of waiver application.
/// </summary>
public enum WaiverScope
{
    /// <summary>
    /// Waiver applies to a single prerequisite.
    /// </summary>
    SinglePrerequisite = 1,

    /// <summary>
    /// Waiver applies to all prerequisites for a course.
    /// </summary>
    AllPrerequisites = 2,

    /// <summary>
    /// Waiver applies to corequisites.
    /// </summary>
    Corequisites = 3,

    /// <summary>
    /// Waiver applies to both prerequisites and corequisites.
    /// </summary>
    Both = 4
}

/// <summary>
/// Status of approval steps in workflow.
/// </summary>
public enum ApprovalStepStatus
{
    /// <summary>
    /// Step is pending approval.
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Step has been approved.
    /// </summary>
    Approved = 2,

    /// <summary>
    /// Step has been rejected.
    /// </summary>
    Rejected = 3,

    /// <summary>
    /// Step was skipped.
    /// </summary>
    Skipped = 4,

    /// <summary>
    /// Step is on hold.
    /// </summary>
    OnHold = 5
}