namespace Zeus.Academia.Infrastructure.Enums;

/// <summary>
/// Enumeration for catalog status
/// </summary>
public enum CatalogStatus
{
    /// <summary>
    /// Catalog is in draft state
    /// </summary>
    Draft = 0,

    /// <summary>
    /// Catalog is under review
    /// </summary>
    UnderReview = 1,

    /// <summary>
    /// Catalog is approved but not yet published
    /// </summary>
    Approved = 2,

    /// <summary>
    /// Catalog is published and active
    /// </summary>
    Published = 3,

    /// <summary>
    /// Catalog has been archived
    /// </summary>
    Archived = 4,

    /// <summary>
    /// Catalog has been withdrawn
    /// </summary>
    Withdrawn = 5
}

/// <summary>
/// Enumeration for catalog types
/// </summary>
public enum CatalogType
{
    /// <summary>
    /// Undergraduate course catalog
    /// </summary>
    Undergraduate = 0,

    /// <summary>
    /// Graduate course catalog
    /// </summary>
    Graduate = 1,

    /// <summary>
    /// Professional/Continuing education catalog
    /// </summary>
    Professional = 2,

    /// <summary>
    /// Summer session catalog
    /// </summary>
    Summer = 3,

    /// <summary>
    /// Online programs catalog
    /// </summary>
    Online = 4,

    /// <summary>
    /// Combined comprehensive catalog
    /// </summary>
    Comprehensive = 5
}

/// <summary>
/// Enumeration for approval stages in course approval workflow
/// </summary>
public enum ApprovalStage
{
    /// <summary>
    /// Initial department review
    /// </summary>
    DepartmentReview = 0,

    /// <summary>
    /// Curriculum committee review
    /// </summary>
    CurriculumCommittee = 1,

    /// <summary>
    /// Academic senate pending review
    /// </summary>
    AcademicSenatePending = 2,

    /// <summary>
    /// Academic senate review
    /// </summary>
    AcademicSenateReview = 3,

    /// <summary>
    /// Provost approval
    /// </summary>
    ProvostApproval = 4,

    /// <summary>
    /// Board of trustees approval (for major changes)
    /// </summary>
    BoardApproval = 5,

    /// <summary>
    /// External accreditation review
    /// </summary>
    AccreditationReview = 6,

    /// <summary>
    /// Final approval and catalog inclusion
    /// </summary>
    FinalApproval = 7
}

/// <summary>
/// Enumeration for approval status
/// </summary>
public enum ApprovalStatus
{
    /// <summary>
    /// Not yet started
    /// </summary>
    NotStarted = 0,

    /// <summary>
    /// Currently pending review
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Currently under review
    /// </summary>
    InReview = 2,

    /// <summary>
    /// Approved
    /// </summary>
    Approved = 3,

    /// <summary>
    /// Rejected
    /// </summary>
    Rejected = 4,

    /// <summary>
    /// Conditional approval with required changes
    /// </summary>
    ConditionalApproval = 5,

    /// <summary>
    /// Withdrawn by submitter
    /// </summary>
    Withdrawn = 6,

    /// <summary>
    /// Deferred to next cycle
    /// </summary>
    Deferred = 7
}

/// <summary>
/// Enumeration for workflow status
/// </summary>
public enum WorkflowStatus
{
    /// <summary>
    /// Workflow not yet started
    /// </summary>
    NotStarted = 0,

    /// <summary>
    /// Workflow is in progress
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// Workflow completed successfully
    /// </summary>
    Completed = 2,

    /// <summary>
    /// Workflow was cancelled
    /// </summary>
    Cancelled = 3,

    /// <summary>
    /// Workflow failed
    /// </summary>
    Failed = 4,

    /// <summary>
    /// Workflow is on hold
    /// </summary>
    OnHold = 5
}

/// <summary>
/// Enumeration for Bloom's taxonomy levels
/// </summary>
public enum BloomsTaxonomyLevel
{
    /// <summary>
    /// Remember - recall facts and basic concepts
    /// </summary>
    Remember = 0,

    /// <summary>
    /// Understand - explain ideas or concepts
    /// </summary>
    Understand = 1,

    /// <summary>
    /// Apply - use information in new situations
    /// </summary>
    Apply = 2,

    /// <summary>
    /// Analyze - draw connections among ideas
    /// </summary>
    Analyze = 3,

    /// <summary>
    /// Evaluate - justify a stand or decision
    /// </summary>
    Evaluate = 4,

    /// <summary>
    /// Create - produce new or original work
    /// </summary>
    Create = 5
}

/// <summary>
/// Enumeration for difficulty levels
/// </summary>
public enum DifficultyLevel
{
    /// <summary>
    /// Beginner level
    /// </summary>
    Beginner = 0,

    /// <summary>
    /// Intermediate level
    /// </summary>
    Intermediate = 1,

    /// <summary>
    /// Advanced level
    /// </summary>
    Advanced = 2,

    /// <summary>
    /// Expert level
    /// </summary>
    Expert = 3
}

/// <summary>
/// Enumeration for mapping strength between course and program outcomes
/// </summary>
public enum MappingStrength
{
    /// <summary>
    /// Weak relationship
    /// </summary>
    Weak = 0,

    /// <summary>
    /// Moderate relationship
    /// </summary>
    Moderate = 1,

    /// <summary>
    /// Strong relationship
    /// </summary>
    Strong = 2
}

/// <summary>
/// Enumeration for contribution level to program outcomes
/// </summary>
public enum ContributionLevel
{
    /// <summary>
    /// Minor contribution
    /// </summary>
    Minor = 0,

    /// <summary>
    /// Moderate contribution
    /// </summary>
    Moderate = 1,

    /// <summary>
    /// Major contribution
    /// </summary>
    Major = 2,

    /// <summary>
    /// Primary contribution
    /// </summary>
    Primary = 3
}

/// <summary>
/// Enumeration for publication formats
/// </summary>
public enum PublicationFormat
{
    /// <summary>
    /// PDF format
    /// </summary>
    PDF = 0,

    /// <summary>
    /// HTML web format
    /// </summary>
    HTML = 1,

    /// <summary>
    /// Mobile-optimized format
    /// </summary>
    Mobile = 2,

    /// <summary>
    /// EPUB e-book format
    /// </summary>
    EPUB = 3,

    /// <summary>
    /// Print-ready format
    /// </summary>
    Print = 4,

    /// <summary>
    /// Interactive web application
    /// </summary>
    Interactive = 5,

    /// <summary>
    /// API/Data format
    /// </summary>
    API = 6
}

/// <summary>
/// Enumeration for distribution channels
/// </summary>
public enum DistributionChannel
{
    /// <summary>
    /// University website
    /// </summary>
    Website = 0,

    /// <summary>
    /// Student portal
    /// </summary>
    StudentPortal = 1,

    /// <summary>
    /// Faculty portal
    /// </summary>
    FacultyPortal = 2,

    /// <summary>
    /// Mobile application
    /// </summary>
    MobileApp = 3,

    /// <summary>
    /// Print copy
    /// </summary>
    PrintCopy = 4,

    /// <summary>
    /// Email distribution
    /// </summary>
    Email = 5,

    /// <summary>
    /// External partners
    /// </summary>
    ExternalPartners = 6,

    /// <summary>
    /// Public API
    /// </summary>
    PublicAPI = 7
}

/// <summary>
/// Enumeration for publication status
/// </summary>
public enum PublicationStatus
{
    /// <summary>
    /// Draft status
    /// </summary>
    Draft = 0,

    /// <summary>
    /// In preparation
    /// </summary>
    InPreparation = 1,

    /// <summary>
    /// Ready for publication
    /// </summary>
    Ready = 2,

    /// <summary>
    /// Published and active
    /// </summary>
    Published = 3,

    /// <summary>
    /// Temporarily suspended
    /// </summary>
    Suspended = 4,

    /// <summary>
    /// Archived
    /// </summary>
    Archived = 5
}

/// <summary>
/// Enumeration for distribution status
/// </summary>
public enum DistributionStatus
{
    /// <summary>
    /// Distribution is pending
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Distribution is active
    /// </summary>
    Active = 1,

    /// <summary>
    /// Distribution is inactive
    /// </summary>
    Inactive = 2,

    /// <summary>
    /// Distribution is suspended
    /// </summary>
    Suspended = 3,

    /// <summary>
    /// Distribution failed
    /// </summary>
    Failed = 4
}

/// <summary>
/// Enumeration for version types
/// </summary>
public enum VersionType
{
    /// <summary>
    /// Major version change
    /// </summary>
    Major = 0,

    /// <summary>
    /// Minor version change
    /// </summary>
    Minor = 1,

    /// <summary>
    /// Patch version change
    /// </summary>
    Patch = 2,

    /// <summary>
    /// Hot fix
    /// </summary>
    Hotfix = 3
}

/// <summary>
/// Enumeration for change types in version control
/// </summary>
public enum ChangeType
{
    /// <summary>
    /// Course added
    /// </summary>
    CourseAdded = 0,

    /// <summary>
    /// Course modified
    /// </summary>
    CourseModified = 1,

    /// <summary>
    /// Course removed
    /// </summary>
    CourseRemoved = 2,

    /// <summary>
    /// Prerequisites changed
    /// </summary>
    PrerequisitesChanged = 3,

    /// <summary>
    /// Credit hours changed
    /// </summary>
    CreditHoursChanged = 4,

    /// <summary>
    /// Course description changed
    /// </summary>
    DescriptionChanged = 5,

    /// <summary>
    /// Learning outcomes changed
    /// </summary>
    LearningOutcomesChanged = 6,

    /// <summary>
    /// Course status changed
    /// </summary>
    StatusChanged = 7
}

/// <summary>
/// Enumeration for comparison types
/// </summary>
public enum ComparisonType
{
    /// <summary>
    /// Difference comparison showing changes
    /// </summary>
    Diff = 0,

    /// <summary>
    /// Sequential comparison between adjacent versions
    /// </summary>
    Sequential = 1,

    /// <summary>
    /// Non-sequential comparison between any two versions
    /// </summary>
    NonSequential = 2,

    /// <summary>
    /// Baseline comparison against a reference version
    /// </summary>
    Baseline = 3
}

/// <summary>
/// Enumeration for difference types
/// </summary>
public enum DifferenceType
{
    /// <summary>
    /// Course was added
    /// </summary>
    CourseAdded = 0,

    /// <summary>
    /// Course was removed
    /// </summary>
    CourseRemoved = 1,

    /// <summary>
    /// Course was modified
    /// </summary>
    CourseModified = 2,

    /// <summary>
    /// Prerequisites changed
    /// </summary>
    PrerequisiteChanged = 3,

    /// <summary>
    /// Learning outcomes changed
    /// </summary>
    LearningOutcomeChanged = 4,

    /// <summary>
    /// Course numbering changed
    /// </summary>
    NumberingChanged = 5
}

/// <summary>
/// Enumeration for impact levels
/// </summary>
public enum ImpactLevel
{
    /// <summary>
    /// Low impact
    /// </summary>
    Low = 0,

    /// <summary>
    /// Medium impact
    /// </summary>
    Medium = 1,

    /// <summary>
    /// High impact
    /// </summary>
    High = 2,

    /// <summary>
    /// Critical impact
    /// </summary>
    Critical = 3
}