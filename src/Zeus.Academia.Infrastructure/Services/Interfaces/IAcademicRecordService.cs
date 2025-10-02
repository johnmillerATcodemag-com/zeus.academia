using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Enums;

namespace Zeus.Academia.Infrastructure.Services.Interfaces;

/// <summary>
/// Interface for Academic Record Management Service (Prompt 4 Task 4)
/// Provides comprehensive academic record tracking including courses, grades, and transcripts
/// </summary>
public interface IAcademicRecordService
{
    #region Course Enrollment Management

    /// <summary>
    /// Enrolls a student in a course
    /// </summary>
    /// <param name="studentEmpNr">Student employee number</param>
    /// <param name="subjectCode">Subject code</param>
    /// <param name="sectionId">Section identifier</param>
    /// <param name="academicTermId">Academic term ID</param>
    /// <param name="creditHours">Credit hours for the enrollment</param>
    /// <param name="isAudit">Whether the student is auditing</param>
    /// <returns>The created course enrollment</returns>
    Task<CourseEnrollment?> EnrollStudentInCourseAsync(int studentEmpNr, string subjectCode, string? sectionId = null, int? academicTermId = null, decimal creditHours = 3.0m, bool isAudit = false);

    /// <summary>
    /// Drops a student from a course
    /// </summary>
    /// <param name="enrollmentId">Course enrollment ID</param>
    /// <param name="dropReason">Reason for dropping</param>
    /// <returns>True if successful</returns>
    Task<bool> DropStudentFromCourseAsync(int enrollmentId, string? dropReason = null);

    /// <summary>
    /// Withdraws a student from a course
    /// </summary>
    /// <param name="enrollmentId">Course enrollment ID</param>
    /// <param name="withdrawalReason">Reason for withdrawal</param>
    /// <returns>True if successful</returns>
    Task<bool> WithdrawStudentFromCourseAsync(int enrollmentId, string? withdrawalReason = null);

    /// <summary>
    /// Gets all course enrollments for a student
    /// </summary>
    /// <param name="studentEmpNr">Student employee number</param>
    /// <param name="academicYear">Optional academic year filter</param>
    /// <param name="semester">Optional semester filter</param>
    /// <returns>List of course enrollments</returns>
    Task<IEnumerable<CourseEnrollment>> GetStudentEnrollmentsAsync(int studentEmpNr, int? academicYear = null, string? semester = null);

    /// <summary>
    /// Gets course enrollment history for a student
    /// </summary>
    /// <param name="studentEmpNr">Student employee number</param>
    /// <returns>Complete enrollment history</returns>
    Task<IEnumerable<CourseEnrollment>> GetStudentEnrollmentHistoryAsync(int studentEmpNr);

    #endregion

    #region Grade Management

    /// <summary>
    /// Records a grade for a course enrollment
    /// </summary>
    /// <param name="courseEnrollmentId">Course enrollment ID</param>
    /// <param name="gradeType">Type of grade</param>
    /// <param name="letterGrade">Letter grade</param>
    /// <param name="numericGrade">Numeric grade (0-100)</param>
    /// <param name="gradedBy">Who assigned the grade</param>
    /// <param name="comments">Optional comments</param>
    /// <returns>The created grade record</returns>
    Task<Grade?> RecordGradeAsync(int courseEnrollmentId, GradeType gradeType, string? letterGrade = null, decimal? numericGrade = null, string? gradedBy = null, string? comments = null);

    /// <summary>
    /// Updates an existing grade
    /// </summary>
    /// <param name="gradeId">Grade ID</param>
    /// <param name="letterGrade">New letter grade</param>
    /// <param name="numericGrade">New numeric grade</param>
    /// <param name="comments">Updated comments</param>
    /// <param name="gradedBy">Who updated the grade</param>
    /// <returns>True if successful</returns>
    Task<bool> UpdateGradeAsync(int gradeId, string? letterGrade = null, decimal? numericGrade = null, string? comments = null, string? gradedBy = null);

    /// <summary>
    /// Gets all grades for a student
    /// </summary>
    /// <param name="studentEmpNr">Student employee number</param>
    /// <param name="academicYear">Optional academic year filter</param>
    /// <param name="semester">Optional semester filter</param>
    /// <returns>List of grades</returns>
    Task<IEnumerable<Grade>> GetStudentGradesAsync(int studentEmpNr, int? academicYear = null, string? semester = null);

    /// <summary>
    /// Gets grades for a specific course enrollment
    /// </summary>
    /// <param name="courseEnrollmentId">Course enrollment ID</param>
    /// <returns>List of grades for the enrollment</returns>
    Task<IEnumerable<Grade>> GetCourseEnrollmentGradesAsync(int courseEnrollmentId);

    /// <summary>
    /// Gets the final grade for a course enrollment
    /// </summary>
    /// <param name="courseEnrollmentId">Course enrollment ID</param>
    /// <returns>The final grade or null if not assigned</returns>
    Task<Grade?> GetFinalGradeAsync(int courseEnrollmentId);

    #endregion

    #region GPA Calculation

    /// <summary>
    /// Calculates cumulative GPA for a student
    /// </summary>
    /// <param name="studentEmpNr">Student employee number</param>
    /// <returns>Cumulative GPA</returns>
    Task<decimal> CalculateCumulativeGPAAsync(int studentEmpNr);

    /// <summary>
    /// Calculates term GPA for a student
    /// </summary>
    /// <param name="studentEmpNr">Student employee number</param>
    /// <param name="academicYear">Academic year</param>
    /// <param name="semester">Semester</param>
    /// <returns>Term GPA</returns>
    Task<decimal> CalculateTermGPAAsync(int studentEmpNr, int academicYear, string semester);

    /// <summary>
    /// Calculates major GPA for a student (courses in their major field)
    /// </summary>
    /// <param name="studentEmpNr">Student employee number</param>
    /// <param name="majorDepartment">Major department</param>
    /// <returns>Major GPA</returns>
    Task<decimal> CalculateMajorGPAAsync(int studentEmpNr, string majorDepartment);

    /// <summary>
    /// Gets GPA history for a student by term
    /// </summary>
    /// <param name="studentEmpNr">Student employee number</param>
    /// <returns>GPA history by term</returns>
    Task<IEnumerable<(int AcademicYear, string Semester, decimal GPA, decimal CumulativeGPA)>> GetGPAHistoryAsync(int studentEmpNr);

    #endregion

    #region Transcript Generation

    /// <summary>
    /// Generates an official transcript for a student
    /// </summary>
    /// <param name="studentEmpNr">Student employee number</param>
    /// <param name="includeInProgress">Whether to include in-progress courses</param>
    /// <returns>Transcript data</returns>
    Task<TranscriptData> GenerateTranscriptAsync(int studentEmpNr, bool includeInProgress = false);

    /// <summary>
    /// Generates an unofficial transcript for a student
    /// </summary>
    /// <param name="studentEmpNr">Student employee number</param>
    /// <returns>Unofficial transcript data</returns>
    Task<TranscriptData> GenerateUnofficialTranscriptAsync(int studentEmpNr);

    /// <summary>
    /// Gets transcript summary information
    /// </summary>
    /// <param name="studentEmpNr">Student employee number</param>
    /// <returns>Transcript summary</returns>
    Task<TranscriptSummary> GetTranscriptSummaryAsync(int studentEmpNr);

    #endregion

    #region Academic Honors and Awards

    /// <summary>
    /// Awards an academic honor to a student
    /// </summary>
    /// <param name="studentEmpNr">Student employee number</param>
    /// <param name="honorType">Type of honor</param>
    /// <param name="title">Honor title</param>
    /// <param name="description">Honor description</param>
    /// <param name="academicYear">Academic year</param>
    /// <param name="semester">Semester</param>
    /// <param name="requiredGPA">Required GPA for the honor</param>
    /// <returns>The created academic honor</returns>
    Task<AcademicHonor?> AwardAcademicHonorAsync(int studentEmpNr, HonorType honorType, string title, string? description = null, int? academicYear = null, string? semester = null, decimal? requiredGPA = null);

    /// <summary>
    /// Awards an award to a student
    /// </summary>
    /// <param name="studentEmpNr">Student employee number</param>
    /// <param name="awardType">Type of award</param>
    /// <param name="name">Award name</param>
    /// <param name="description">Award description</param>
    /// <param name="monetaryValue">Monetary value if applicable</param>
    /// <param name="awardingOrganization">Organization giving the award</param>
    /// <returns>The created award</returns>
    Task<Award?> GiveAwardAsync(int studentEmpNr, AwardType awardType, string name, string? description = null, decimal? monetaryValue = null, string? awardingOrganization = null);

    /// <summary>
    /// Gets all academic honors for a student
    /// </summary>
    /// <param name="studentEmpNr">Student employee number</param>
    /// <returns>List of academic honors</returns>
    Task<IEnumerable<AcademicHonor>> GetStudentAcademicHonorsAsync(int studentEmpNr);

    /// <summary>
    /// Gets all awards for a student
    /// </summary>
    /// <param name="studentEmpNr">Student employee number</param>
    /// <returns>List of awards</returns>
    Task<IEnumerable<Award>> GetStudentAwardsAsync(int studentEmpNr);

    #endregion

    #region Degree Progress Tracking

    /// <summary>
    /// Updates degree progress for a student
    /// </summary>
    /// <param name="studentEmpNr">Student employee number</param>
    /// <param name="degreeCode">Degree code</param>
    /// <returns>Updated degree progress</returns>
    Task<DegreeProgress?> UpdateDegreeProgressAsync(int studentEmpNr, string degreeCode);

    /// <summary>
    /// Gets degree progress for a student
    /// </summary>
    /// <param name="studentEmpNr">Student employee number</param>
    /// <returns>Degree progress information</returns>
    Task<DegreeProgress?> GetDegreeProgressAsync(int studentEmpNr);

    /// <summary>
    /// Calculates remaining requirements for degree completion
    /// </summary>
    /// <param name="studentEmpNr">Student employee number</param>
    /// <returns>Remaining requirements information</returns>
    Task<DegreeRequirements> GetRemainingRequirementsAsync(int studentEmpNr);

    /// <summary>
    /// Checks if a student is eligible for graduation
    /// </summary>
    /// <param name="studentEmpNr">Student employee number</param>
    /// <returns>Graduation eligibility information</returns>
    Task<GraduationEligibility> CheckGraduationEligibilityAsync(int studentEmpNr);

    #endregion

    #region Utility Methods

    /// <summary>
    /// Converts letter grade to grade points
    /// </summary>
    /// <param name="letterGrade">Letter grade</param>
    /// <returns>Grade points (0.0-4.0 scale)</returns>
    decimal ConvertLetterGradeToPoints(string letterGrade);

    /// <summary>
    /// Converts numeric grade to letter grade
    /// </summary>
    /// <param name="numericGrade">Numeric grade (0-100)</param>
    /// <returns>Letter grade</returns>
    string ConvertNumericGradeToLetter(decimal numericGrade);

    /// <summary>
    /// Determines academic standing based on GPA
    /// </summary>
    /// <param name="gpa">Student's GPA</param>
    /// <param name="creditHours">Total credit hours completed</param>
    /// <returns>Academic standing</returns>
    AcademicStanding DetermineAcademicStanding(decimal gpa, decimal creditHours);

    #endregion
}

/// <summary>
/// Data structure for transcript information
/// </summary>
public class TranscriptData
{
    public Student Student { get; set; } = null!;
    public IEnumerable<CourseEnrollment> Enrollments { get; set; } = new List<CourseEnrollment>();
    public IEnumerable<Grade> Grades { get; set; } = new List<Grade>();
    public IEnumerable<AcademicHonor> Honors { get; set; } = new List<AcademicHonor>();
    public IEnumerable<Award> Awards { get; set; } = new List<Award>();
    public decimal CumulativeGPA { get; set; }
    public decimal TotalCreditHours { get; set; }
    public decimal QualityPoints { get; set; }
    public AcademicStanding AcademicStanding { get; set; }
    public DateTime GeneratedDate { get; set; }
    public bool IsOfficial { get; set; }
}

/// <summary>
/// Data structure for transcript summary
/// </summary>
public class TranscriptSummary
{
    public int StudentEmpNr { get; set; }
    public decimal CumulativeGPA { get; set; }
    public decimal TotalCreditHours { get; set; }
    public decimal QualityPoints { get; set; }
    public AcademicStanding AcademicStanding { get; set; }
    public int TotalCourses { get; set; }
    public int CompletedCourses { get; set; }
    public int InProgressCourses { get; set; }
    public IEnumerable<(int AcademicYear, string Semester, decimal GPA)> TermGPAs { get; set; } = new List<(int, string, decimal)>();
}

/// <summary>
/// Data structure for degree requirements
/// </summary>
public class DegreeRequirements
{
    public int StudentEmpNr { get; set; }
    public string DegreeCode { get; set; } = string.Empty;
    public decimal RemainingCreditHours { get; set; }
    public IEnumerable<string> RemainingCoreRequirements { get; set; } = new List<string>();
    public IEnumerable<string> RemainingElectiveRequirements { get; set; } = new List<string>();
    public bool CapstoneRequired { get; set; }
    public bool ThesisRequired { get; set; }
    public bool InternshipRequired { get; set; }
    public decimal RequiredGPA { get; set; }
    public decimal CurrentGPA { get; set; }
}

/// <summary>
/// Data structure for graduation eligibility
/// </summary>
public class GraduationEligibility
{
    public int StudentEmpNr { get; set; }
    public bool IsEligible { get; set; }
    public IEnumerable<string> UnmetRequirements { get; set; } = new List<string>();
    public decimal RemainingCreditHours { get; set; }
    public bool MeetsGPARequirement { get; set; }
    public bool MeetsResidencyRequirement { get; set; }
    public DateTime? ProjectedGraduationDate { get; set; }
}