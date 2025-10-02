using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Enums;

namespace Zeus.Academia.Infrastructure.Services.Interfaces;

/// <summary>
/// Interface for student profile management services
/// </summary>
public interface IStudentProfileService
{
    /// <summary>
    /// Gets a complete student profile including all related data
    /// </summary>
    /// <param name="studentEmpNr">The student's employee number</param>
    /// <returns>Complete student profile with related data</returns>
    Task<Student?> GetCompleteStudentProfileAsync(int studentEmpNr);

    /// <summary>
    /// Updates student profile information
    /// </summary>
    /// <param name="studentEmpNr">The student's employee number</param>
    /// <param name="profileData">Updated profile data</param>
    /// <returns>True if update was successful</returns>
    Task<bool> UpdateStudentProfileAsync(int studentEmpNr, Student profileData);

    /// <summary>
    /// Calculates and updates the profile completion percentage
    /// </summary>
    /// <param name="studentEmpNr">The student's employee number</param>
    /// <returns>The calculated completion percentage</returns>
    Task<decimal> CalculateProfileCompletionAsync(int studentEmpNr);

    /// <summary>
    /// Gets the profile completion status with details about missing fields
    /// </summary>
    /// <param name="studentEmpNr">The student's employee number</param>
    /// <returns>Profile completion details</returns>
    Task<ProfileCompletionStatus> GetProfileCompletionStatusAsync(int studentEmpNr);

    /// <summary>
    /// Updates the student's profile photo
    /// </summary>
    /// <param name="studentEmpNr">The student's employee number</param>
    /// <param name="photoPath">Path to the new photo</param>
    /// <returns>True if update was successful</returns>
    Task<bool> UpdateProfilePhotoAsync(int studentEmpNr, string photoPath);

    /// <summary>
    /// Adds an emergency contact for a student
    /// </summary>
    /// <param name="studentEmpNr">The student's employee number</param>
    /// <param name="emergencyContact">Emergency contact information</param>
    /// <returns>The created emergency contact</returns>
    Task<EmergencyContact> AddEmergencyContactAsync(int studentEmpNr, EmergencyContact emergencyContact);

    /// <summary>
    /// Updates an emergency contact
    /// </summary>
    /// <param name="contactId">The contact ID</param>
    /// <param name="emergencyContact">Updated contact information</param>
    /// <returns>True if update was successful</returns>
    Task<bool> UpdateEmergencyContactAsync(int contactId, EmergencyContact emergencyContact);

    /// <summary>
    /// Removes an emergency contact
    /// </summary>
    /// <param name="contactId">The contact ID</param>
    /// <returns>True if removal was successful</returns>
    Task<bool> RemoveEmergencyContactAsync(int contactId);

    /// <summary>
    /// Gets all emergency contacts for a student
    /// </summary>
    /// <param name="studentEmpNr">The student's employee number</param>
    /// <returns>List of emergency contacts</returns>
    Task<IEnumerable<EmergencyContact>> GetEmergencyContactsAsync(int studentEmpNr);

    /// <summary>
    /// Assigns an academic advisor to a student
    /// </summary>
    /// <param name="studentEmpNr">The student's employee number</param>
    /// <param name="advisorId">The advisor ID</param>
    /// <param name="advisorType">Type of advisor</param>
    /// <param name="isPrimary">Whether this is the primary advisor</param>
    /// <param name="reason">Reason for assignment</param>
    /// <returns>The created assignment</returns>
    Task<StudentAdvisorAssignment> AssignAdvisorAsync(int studentEmpNr, int advisorId, AdvisorType advisorType, bool isPrimary = true, string? reason = null);

    /// <summary>
    /// Removes an advisor assignment
    /// </summary>
    /// <param name="assignmentId">The assignment ID</param>
    /// <param name="endReason">Reason for ending assignment</param>
    /// <returns>True if removal was successful</returns>
    Task<bool> RemoveAdvisorAssignmentAsync(int assignmentId, string? endReason = null);

    /// <summary>
    /// Gets all current advisor assignments for a student
    /// </summary>
    /// <param name="studentEmpNr">The student's employee number</param>
    /// <returns>List of current advisor assignments</returns>
    Task<IEnumerable<StudentAdvisorAssignment>> GetCurrentAdvisorAssignmentsAsync(int studentEmpNr);

    /// <summary>
    /// Gets the primary academic advisor for a student
    /// </summary>
    /// <param name="studentEmpNr">The student's employee number</param>
    /// <returns>Primary advisor assignment</returns>
    Task<StudentAdvisorAssignment?> GetPrimaryAdvisorAsync(int studentEmpNr);

    /// <summary>
    /// Uploads a document for a student
    /// </summary>
    /// <param name="studentEmpNr">The student's employee number</param>
    /// <param name="document">Document information</param>
    /// <returns>The created document record</returns>
    Task<StudentDocument> UploadDocumentAsync(int studentEmpNr, StudentDocument document);

    /// <summary>
    /// Gets all active documents for a student
    /// </summary>
    /// <param name="studentEmpNr">The student's employee number</param>
    /// <param name="documentType">Optional filter by document type</param>
    /// <returns>List of student documents</returns>
    Task<IEnumerable<StudentDocument>> GetStudentDocumentsAsync(int studentEmpNr, StudentDocumentType? documentType = null);

    /// <summary>
    /// Verifies a student document
    /// </summary>
    /// <param name="documentId">The document ID</param>
    /// <param name="verifiedBy">Who verified the document</param>
    /// <param name="notes">Verification notes</param>
    /// <returns>True if verification was successful</returns>
    Task<bool> VerifyDocumentAsync(int documentId, string verifiedBy, string? notes = null);

    /// <summary>
    /// Deletes a student document
    /// </summary>
    /// <param name="documentId">The document ID</param>
    /// <returns>True if deletion was successful</returns>
    Task<bool> DeleteDocumentAsync(int documentId);

    /// <summary>
    /// Gets students with incomplete profiles
    /// </summary>
    /// <param name="minimumCompletionPercentage">Minimum completion percentage threshold</param>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>Students with incomplete profiles</returns>
    Task<(IEnumerable<Student> Students, int TotalCount)> GetStudentsWithIncompleteProfilesAsync(
        decimal minimumCompletionPercentage = 80m, int pageNumber = 1, int pageSize = 10);

    /// <summary>
    /// Validates profile data before saving
    /// </summary>
    /// <param name="profileData">Profile data to validate</param>
    /// <returns>Validation result</returns>
    Task<ProfileValidationResult> ValidateProfileDataAsync(Student profileData);
}

/// <summary>
/// Profile completion status details
/// </summary>
public class ProfileCompletionStatus
{
    public decimal CompletionPercentage { get; set; }
    public List<string> MissingFields { get; set; } = new();
    public List<string> CompletedFields { get; set; } = new();
    public List<string> RecommendedFields { get; set; } = new();
    public bool HasEmergencyContact { get; set; }
    public bool HasAdvisor { get; set; }
    public bool HasProfilePhoto { get; set; }
    public int RequiredDocumentCount { get; set; }
    public int SubmittedDocumentCount { get; set; }
}

/// <summary>
/// Profile validation result
/// </summary>
public class ProfileValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}