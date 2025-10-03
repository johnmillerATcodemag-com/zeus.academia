using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Enums;

namespace Zeus.Academia.Infrastructure.Services.Interfaces;

/// <summary>
/// Interface for student management services
/// </summary>
public interface IStudentService
{
    /// <summary>
    /// Gets a student by their ID
    /// </summary>
    /// <param name="studentId">The student ID</param>
    /// <returns>The student entity if found</returns>
    Task<Student?> GetStudentByIdAsync(int studentId);

    /// <summary>
    /// Gets a student by their student ID
    /// </summary>
    /// <param name="studentNumber">The student ID</param>
    /// <returns>The student entity if found</returns>
    Task<Student?> GetStudentByStudentNumberAsync(string studentNumber);

    /// <summary>
    /// Gets all students with pagination support
    /// </summary>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>Paginated list of students</returns>
    Task<(IEnumerable<Student> Students, int TotalCount)> GetStudentsAsync(int pageNumber = 1, int pageSize = 10);

    /// <summary>
    /// Searches students by various criteria
    /// </summary>
    /// <param name="searchTerm">General search term (name, email, student ID)</param>
    /// <param name="departmentName">Filter by department</param>
    /// <param name="enrollmentStatus">Filter by enrollment status</param>
    /// <param name="academicStanding">Filter by academic standing</param>
    /// <param name="program">Filter by program</param>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>Filtered and paginated list of students</returns>
    Task<(IEnumerable<Student> Students, int TotalCount)> SearchStudentsAsync(
        string? searchTerm = null,
        string? departmentName = null,
        EnrollmentStatus? enrollmentStatus = null,
        AcademicStanding? academicStanding = null,
        string? program = null,
        int pageNumber = 1,
        int pageSize = 10);

    /// <summary>
    /// Creates a new student
    /// </summary>
    /// <param name="student">The student to create</param>
    /// <returns>The created student</returns>
    Task<Student> CreateStudentAsync(Student student);

    /// <summary>
    /// Updates an existing student
    /// </summary>
    /// <param name="student">The student to update</param>
    /// <returns>The updated student</returns>
    Task<Student> UpdateStudentAsync(Student student);

    /// <summary>
    /// Deletes a student
    /// </summary>
    /// <param name="studentId">The ID of the student to delete</param>
    /// <returns>True if deletion was successful</returns>
    Task<bool> DeleteStudentAsync(int studentId);

    /// <summary>
    /// Updates a student's enrollment status
    /// </summary>
    /// <param name="studentId">The student ID</param>
    /// <param name="newStatus">The new enrollment status</param>
    /// <param name="notes">Optional notes about the status change</param>
    /// <returns>True if update was successful</returns>
    Task<bool> UpdateEnrollmentStatusAsync(int studentId, EnrollmentStatus newStatus, string? notes = null);

    /// <summary>
    /// Updates a student's academic standing
    /// </summary>
    /// <param name="studentId">The student ID</param>
    /// <param name="newStanding">The new academic standing</param>
    /// <param name="notes">Optional notes about the standing change</param>
    /// <returns>True if update was successful</returns>
    Task<bool> UpdateAcademicStandingAsync(int studentId, AcademicStanding newStanding, string? notes = null);

    /// <summary>
    /// Calculates and updates a student's GPA based on their course enrollments
    /// </summary>
    /// <param name="studentId">The student ID</param>
    /// <returns>The calculated GPA</returns>
    Task<decimal?> RecalculateGPAAsync(int studentId);

    /// <summary>
    /// Gets students by academic standing
    /// </summary>
    /// <param name="academicStanding">The academic standing to filter by</param>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>Filtered and paginated list of students</returns>
    Task<(IEnumerable<Student> Students, int TotalCount)> GetStudentsByAcademicStandingAsync(
        AcademicStanding academicStanding,
        int pageNumber = 1,
        int pageSize = 10);

    /// <summary>
    /// Gets students by enrollment status
    /// </summary>
    /// <param name="enrollmentStatus">The enrollment status to filter by</param>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>Filtered and paginated list of students</returns>
    Task<(IEnumerable<Student> Students, int TotalCount)> GetStudentsByEnrollmentStatusAsync(
        EnrollmentStatus enrollmentStatus,
        int pageNumber = 1,
        int pageSize = 10);

    /// <summary>
    /// Gets students by department
    /// </summary>
    /// <param name="departmentName">The department name</param>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>Filtered and paginated list of students</returns>
    Task<(IEnumerable<Student> Students, int TotalCount)> GetStudentsByDepartmentAsync(
        string departmentName,
        int pageNumber = 1,
        int pageSize = 10);

    /// <summary>
    /// Validates if an enrollment status transition is allowed
    /// </summary>
    /// <param name="currentStatus">Current enrollment status</param>
    /// <param name="newStatus">Proposed new enrollment status</param>
    /// <returns>True if transition is valid</returns>
    bool IsValidEnrollmentStatusTransition(EnrollmentStatus currentStatus, EnrollmentStatus newStatus);

    /// <summary>
    /// Validates if an academic standing change is allowed
    /// </summary>
    /// <param name="currentGPA">Current student GPA</param>
    /// <param name="newStanding">Proposed academic standing</param>
    /// <returns>True if standing is appropriate for the GPA</returns>
    bool IsValidAcademicStanding(decimal? currentGPA, AcademicStanding newStanding);

    /// <summary>
    /// Checks if a student exists
    /// </summary>
    /// <param name="studentId">The student ID to check</param>
    /// <returns>True if student exists</returns>
    Task<bool> StudentExistsAsync(int studentId);

    /// <summary>
    /// Gets the total count of students
    /// </summary>
    /// <returns>Total number of students</returns>
    Task<int> GetTotalStudentCountAsync();

    /// <summary>
    /// Gets students requiring academic review (GPA-based)
    /// </summary>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>Students requiring academic review</returns>
    Task<(IEnumerable<Student> Students, int TotalCount)> GetStudentsRequiringAcademicReviewAsync(
        int pageNumber = 1,
        int pageSize = 10);

    // Enrollment Management Methods

    /// <summary>
    /// Submits an enrollment application for a student
    /// </summary>
    /// <param name="studentId">The student ID</param>
    /// <param name="programCode">Program to apply for</param>
    /// <param name="preferredStartDate">Preferred start date</param>
    /// <param name="applicationDocuments">Supporting documents</param>
    /// <returns>The created enrollment application</returns>
    Task<EnrollmentApplication> SubmitApplicationAsync(
        int studentId,
        string programCode,
        DateTime preferredStartDate,
        List<ApplicationDocument>? applicationDocuments = null);

    /// <summary>
    /// Processes an admission decision for an enrollment application
    /// </summary>
    /// <param name="applicationId">The application ID</param>
    /// <param name="decision">Admission decision</param>
    /// <param name="decisionReason">Reason for the decision</param>
    /// <param name="decisionMadeBy">Person making the decision</param>
    /// <param name="conditionalRequirements">Any conditional requirements</param>
    /// <returns>True if processed successfully</returns>
    Task<bool> ProcessAdmissionDecisionAsync(
        int applicationId,
        AdmissionDecision decision,
        string decisionReason,
        string decisionMadeBy,
        string? conditionalRequirements = null);

    /// <summary>
    /// Processes enrollment for an admitted student
    /// </summary>
    /// <param name="applicationId">The application ID</param>
    /// <param name="enrollmentDate">Date of enrollment</param>
    /// <param name="academicTermId">Academic term for enrollment</param>
    /// <param name="notes">Enrollment notes</param>
    /// <returns>True if enrollment processed successfully</returns>
    Task<bool> ProcessEnrollmentAsync(
        int applicationId,
        DateTime enrollmentDate,
        int academicTermId,
        string? notes = null);

    /// <summary>
    /// Gets enrollment applications for a student
    /// </summary>
    /// <param name="studentId">The student ID</param>
    /// <param name="status">Optional status filter</param>
    /// <returns>List of enrollment applications</returns>
    Task<IEnumerable<EnrollmentApplication>> GetStudentApplicationsAsync(
        int studentId,
        ApplicationStatus? status = null);

    /// <summary>
    /// Gets enrollment history for a student
    /// </summary>
    /// <param name="studentId">The student ID</param>
    /// <param name="includeDetails">Whether to include detailed information</param>
    /// <returns>List of enrollment history entries</returns>
    Task<IEnumerable<EnrollmentHistory>> GetStudentEnrollmentHistoryAsync(
        int studentId,
        bool includeDetails = false);

    /// <summary>
    /// Gets pending applications requiring admission review
    /// </summary>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>Pending applications</returns>
    Task<(IEnumerable<EnrollmentApplication> Applications, int TotalCount)> GetPendingApplicationsAsync(
        int pageNumber = 1,
        int pageSize = 10);

    /// <summary>
    /// Gets all students (alternative method for controller compatibility)
    /// </summary>
    /// <returns>All students</returns>
    Task<IEnumerable<Student>> GetAllStudentsAsync();

    /// <summary>
    /// Gets a student by their employee number
    /// </summary>
    /// <param name="empNr">Employee number</param>
    /// <returns>The student if found</returns>
    Task<Student?> GetStudentByEmpNrAsync(string empNr);

    /// <summary>
    /// Gets a student by their student ID (alternative method name)
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <returns>The student if found</returns>
    Task<Student?> GetStudentByStudentIdAsync(string studentId);

    /// <summary>
    /// Deactivates a student
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <returns>Success status</returns>
    Task<bool> DeactivateStudentAsync(int studentId);

    /// <summary>
    /// Submits an enrollment application
    /// </summary>
    /// <param name="application">Application details</param>
    /// <returns>Created application</returns>
    Task<EnrollmentApplication> SubmitEnrollmentApplicationAsync(EnrollmentApplication application);

    /// <summary>
    /// Gets an enrollment application by ID
    /// </summary>
    /// <param name="applicationId">Application ID</param>
    /// <returns>The application if found</returns>
    Task<EnrollmentApplication?> GetEnrollmentApplicationAsync(int applicationId);

    /// <summary>
    /// Gets enrollment history for a student (alternative method name)
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <returns>Enrollment history</returns>
    Task<IEnumerable<EnrollmentHistory>> GetEnrollmentHistoryAsync(int studentId);

    /// <summary>
    /// Gets student statistics
    /// </summary>
    /// <returns>Student statistics</returns>
    Task<object> GetStudentStatisticsAsync();

    /// <summary>
    /// Uploads a document for a student
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <param name="documentType">Document type</param>
    /// <param name="fileName">File name</param>
    /// <param name="fileContent">File content</param>
    /// <returns>Upload result</returns>
    Task<object> UploadDocumentAsync(int studentId, string documentType, string fileName, byte[] fileContent);

    /// <summary>
    /// Uploads a photo for a student
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <param name="fileName">File name</param>
    /// <param name="fileContent">File content</param>
    /// <returns>Upload result</returns>
    Task<object> UploadPhotoAsync(int studentId, string fileName, byte[] fileContent);
}