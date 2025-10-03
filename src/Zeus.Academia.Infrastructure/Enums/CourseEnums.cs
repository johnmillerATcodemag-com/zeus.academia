namespace Zeus.Academia.Infrastructure.Enums;

/// <summary>
/// Enumeration for subject types in the hierarchical categorization system.
/// </summary>
public enum SubjectType
{
    /// <summary>
    /// A broad subject area (e.g., "Computer Science", "Mathematics")
    /// </summary>
    SubjectArea = 1,

    /// <summary>
    /// A specific course within a subject area
    /// </summary>
    Course = 2,

    /// <summary>
    /// A topic or module within a course
    /// </summary>
    Topic = 3
}

/// <summary>
/// Enumeration for course levels in the academic hierarchy.
/// </summary>
public enum CourseLevel
{
    /// <summary>
    /// General undergraduate level
    /// </summary>
    Undergraduate = 1,

    /// <summary>
    /// Lower division undergraduate (typically 1000-2999 level courses)
    /// </summary>
    LowerDivision = 2,

    /// <summary>
    /// Upper division undergraduate (typically 3000-4999 level courses)
    /// </summary>
    UpperDivision = 3,

    /// <summary>
    /// Graduate level courses (typically 5000-6999 level courses)
    /// </summary>
    Graduate = 4,

    /// <summary>
    /// Doctoral level courses (typically 7000+ level courses)
    /// </summary>
    Doctoral = 5,

    /// <summary>
    /// Continuing education or professional development courses
    /// </summary>
    ContinuingEducation = 6
}

/// <summary>
/// Enumeration for course status in the lifecycle management system.
/// </summary>
public enum CourseStatus
{
    /// <summary>
    /// Course is under development or review
    /// </summary>
    UnderReview = 1,

    /// <summary>
    /// Course is active and available for enrollment
    /// </summary>
    Active = 2,

    /// <summary>
    /// Course is temporarily inactive but may be reactivated
    /// </summary>
    Inactive = 3,

    /// <summary>
    /// Course has been retired and is no longer offered
    /// </summary>
    Retired = 4,

    /// <summary>
    /// Course is archived for historical purposes only
    /// </summary>
    Archived = 5
}

/// <summary>
/// Enumeration for prerequisite types.
/// </summary>
public enum PrerequisiteType
{
    /// <summary>
    /// Prerequisite is a specific course
    /// </summary>
    Course = 1,

    /// <summary>
    /// Prerequisite is a GPA requirement
    /// </summary>
    GPA = 2,

    /// <summary>
    /// Prerequisite is class standing (Freshman, Sophomore, etc.)
    /// </summary>
    ClassStanding = 3,

    /// <summary>
    /// Prerequisite is test score (SAT, ACT, placement test, etc.)
    /// </summary>
    TestScore = 4,

    /// <summary>
    /// Prerequisite requires instructor permission
    /// </summary>
    Permission = 5,

    /// <summary>
    /// Prerequisite requires specific permission
    /// </summary>
    PermissionRequired = 6,

    /// <summary>
    /// Prerequisite is completion of a certain number of credit hours
    /// </summary>
    CreditHours = 7
}

/// <summary>
/// Enumeration for logical operators in prerequisite chains.
/// </summary>
public enum LogicalOperator
{
    /// <summary>
    /// AND operator - all conditions must be met
    /// </summary>
    And = 1,

    /// <summary>
    /// OR operator - at least one condition must be met
    /// </summary>
    Or = 2
}

/// <summary>
/// Enumeration for restriction types.
/// </summary>
public enum RestrictionType
{
    /// <summary>
    /// Restriction based on student's major
    /// </summary>
    Major = 1,

    /// <summary>
    /// Restriction based on class level (Freshman, Sophomore, Junior, Senior, Graduate)
    /// </summary>
    ClassLevel = 2,

    /// <summary>
    /// Restriction requiring special permission
    /// </summary>
    Permission = 3,

    /// <summary>
    /// Restriction based on GPA requirement
    /// </summary>
    GPA = 4,

    /// <summary>
    /// Restriction based on college or school within university
    /// </summary>
    College = 5,

    /// <summary>
    /// Restriction based on completed credit hours
    /// </summary>
    CreditHours = 6,

    /// <summary>
    /// Restriction based on enrollment capacity
    /// </summary>
    Enrollment = 7,

    /// <summary>
    /// Restriction requiring specific major
    /// </summary>
    MajorRequired = 8,

    /// <summary>
    /// Restriction based on class standing requirement
    /// </summary>
    ClassStandingRequired = 9,

    /// <summary>
    /// Restriction requiring specific permission
    /// </summary>
    PermissionRequired = 10,

    /// <summary>
    /// Time-based restriction for enrollment windows
    /// </summary>
    TimeRestriction = 11
}

/// <summary>
/// Enumeration for credit types in the credit system.
/// </summary>
public enum CreditTypeEnum
{
    /// <summary>
    /// Traditional lecture credit hours
    /// </summary>
    Lecture = 1,

    /// <summary>
    /// Laboratory credit hours
    /// </summary>
    Laboratory = 2,

    /// <summary>
    /// Clinical practice credit hours
    /// </summary>
    Clinical = 3,

    /// <summary>
    /// Practicum or field experience credit hours
    /// </summary>
    Practicum = 4,

    /// <summary>
    /// Studio or workshop credit hours
    /// </summary>
    Studio = 5,

    /// <summary>
    /// Discussion or recitation credit hours
    /// </summary>
    Discussion = 6,

    /// <summary>
    /// Seminar credit hours
    /// </summary>
    Seminar = 7,

    /// <summary>
    /// Independent study credit hours
    /// </summary>
    IndependentStudy = 8,

    /// <summary>
    /// Internship credit hours
    /// </summary>
    Internship = 9,

    /// <summary>
    /// Research credit hours
    /// </summary>
    Research = 10
}

/// <summary>
/// Enumeration for delivery methods.
/// </summary>
public enum DeliveryMethod
{
    /// <summary>
    /// Traditional in-person instruction
    /// </summary>
    InPerson = 1,

    /// <summary>
    /// Fully online instruction
    /// </summary>
    Online = 2,

    /// <summary>
    /// Hybrid of in-person and online
    /// </summary>
    Hybrid = 3,

    /// <summary>
    /// Synchronous online (live virtual sessions)
    /// </summary>
    Synchronous = 4,

    /// <summary>
    /// Asynchronous online (self-paced)
    /// </summary>
    Asynchronous = 5
}

/// <summary>
/// Enumeration for assessment methods.
/// </summary>
public enum AssessmentMethod
{
    /// <summary>
    /// Traditional written examinations
    /// </summary>
    WrittenExam = 1,

    /// <summary>
    /// Practical or hands-on examinations
    /// </summary>
    PracticalExam = 2,

    /// <summary>
    /// Laboratory reports and assignments
    /// </summary>
    LabReports = 3,

    /// <summary>
    /// Research papers and written assignments
    /// </summary>
    Papers = 4,

    /// <summary>
    /// Project-based assessments
    /// </summary>
    Projects = 5,

    /// <summary>
    /// Oral presentations and examinations
    /// </summary>
    Presentations = 6,

    /// <summary>
    /// Portfolio-based assessment
    /// </summary>
    Portfolio = 7,

    /// <summary>
    /// Peer review and assessment
    /// </summary>
    PeerReview = 8
}