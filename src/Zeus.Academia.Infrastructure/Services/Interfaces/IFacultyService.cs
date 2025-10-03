using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Enums;

namespace Zeus.Academia.Infrastructure.Services.Interfaces;

/// <summary>
/// Interface for faculty management services
/// </summary>
public interface IFacultyService
{
    // Basic CRUD Operations

    /// <summary>
    /// Gets faculty by their employee number
    /// </summary>
    /// <param name="empNr">The employee number</param>
    /// <returns>The faculty entity if found</returns>
    Task<Academic?> GetFacultyByEmpNrAsync(int empNr);

    /// <summary>
    /// Gets all faculty with pagination support
    /// </summary>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>Paginated list of faculty</returns>
    Task<(IEnumerable<Academic> Faculty, int TotalCount)> GetFacultyAsync(int pageNumber = 1, int pageSize = 10);

    /// <summary>
    /// Searches faculty by various criteria
    /// </summary>
    /// <param name="searchTerm">General search term (name, email, research area)</param>
    /// <param name="departmentName">Filter by department</param>
    /// <param name="rankCode">Filter by academic rank</param>
    /// <param name="hasTenure">Filter by tenure status</param>
    /// <param name="researchArea">Filter by research area</param>
    /// <param name="facultyType">Filter by faculty type (Professor, TeachingProf)</param>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>Filtered and paginated list of faculty</returns>
    Task<(IEnumerable<Academic> Faculty, int TotalCount)> SearchFacultyAsync(
        string? searchTerm = null,
        string? departmentName = null,
        string? rankCode = null,
        bool? hasTenure = null,
        string? researchArea = null,
        FacultyType? facultyType = null,
        int pageNumber = 1,
        int pageSize = 10);

    /// <summary>
    /// Creates a new faculty member
    /// </summary>
    /// <param name="faculty">The faculty to create</param>
    /// <returns>The created faculty</returns>
    Task<Academic> CreateFacultyAsync(Academic faculty);

    /// <summary>
    /// Updates an existing faculty member
    /// </summary>
    /// <param name="faculty">The faculty to update</param>
    /// <returns>The updated faculty</returns>
    Task<Academic> UpdateFacultyAsync(Academic faculty);

    /// <summary>
    /// Deletes a faculty member
    /// </summary>
    /// <param name="empNr">The employee number of the faculty to delete</param>
    /// <returns>True if deletion was successful</returns>
    Task<bool> DeleteFacultyAsync(int empNr);

    // Faculty-Specific Operations

    /// <summary>
    /// Gets all professors
    /// </summary>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>Paginated list of professors</returns>
    Task<(IEnumerable<Professor> Professors, int TotalCount)> GetProfessorsAsync(int pageNumber = 1, int pageSize = 10);

    /// <summary>
    /// Gets all teaching professors
    /// </summary>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>Paginated list of teaching professors</returns>
    Task<(IEnumerable<TeachingProf> TeachingProfs, int TotalCount)> GetTeachingProfsAsync(int pageNumber = 1, int pageSize = 10);

    /// <summary>
    /// Gets faculty by department
    /// </summary>
    /// <param name="departmentName">The department name</param>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>Filtered and paginated list of faculty</returns>
    Task<(IEnumerable<Academic> Faculty, int TotalCount)> GetFacultyByDepartmentAsync(
        string departmentName,
        int pageNumber = 1,
        int pageSize = 10);

    /// <summary>
    /// Gets faculty by rank
    /// </summary>
    /// <param name="rankCode">The rank code</param>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>Filtered and paginated list of faculty</returns>
    Task<(IEnumerable<Academic> Faculty, int TotalCount)> GetFacultyByRankAsync(
        string rankCode,
        int pageNumber = 1,
        int pageSize = 10);

    /// <summary>
    /// Gets tenured faculty
    /// </summary>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>Filtered and paginated list of tenured faculty</returns>
    Task<(IEnumerable<Academic> Faculty, int TotalCount)> GetTenuredFacultyAsync(int pageNumber = 1, int pageSize = 10);

    /// <summary>
    /// Gets non-tenured faculty
    /// </summary>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>Filtered and paginated list of non-tenured faculty</returns>
    Task<(IEnumerable<Academic> Faculty, int TotalCount)> GetNonTenuredFacultyAsync(int pageNumber = 1, int pageSize = 10);

    /// <summary>
    /// Gets faculty by research area
    /// </summary>
    /// <param name="researchArea">The research area</param>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>Filtered and paginated list of faculty</returns>
    Task<(IEnumerable<Academic> Faculty, int TotalCount)> GetFacultyByResearchAreaAsync(
        string researchArea,
        int pageNumber = 1,
        int pageSize = 10);

    // Tenure Management

    /// <summary>
    /// Updates tenure status for a faculty member
    /// </summary>
    /// <param name="empNr">Faculty employee number</param>
    /// <param name="hasTenure">New tenure status</param>
    /// <param name="notes">Optional notes about the tenure change</param>
    /// <returns>True if update was successful</returns>
    Task<bool> UpdateTenureStatusAsync(int empNr, bool hasTenure, string? notes = null);

    /// <summary>
    /// Gets faculty eligible for tenure review
    /// </summary>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>Faculty eligible for tenure review</returns>
    Task<(IEnumerable<Academic> Faculty, int TotalCount)> GetFacultyEligibleForTenureReviewAsync(
        int pageNumber = 1,
        int pageSize = 10);

    /// <summary>
    /// Gets tenure track information for a faculty member
    /// </summary>
    /// <param name="empNr">Faculty employee number</param>
    /// <returns>Tenure track information if found</returns>
    Task<TenureTrack?> GetTenureTrackAsync(int empNr);

    // Promotion Management

    /// <summary>
    /// Updates rank for a faculty member
    /// </summary>
    /// <param name="empNr">Faculty employee number</param>
    /// <param name="newRankCode">New rank code</param>
    /// <param name="effectiveDate">Effective date of promotion</param>
    /// <param name="notes">Optional notes about the promotion</param>
    /// <returns>True if update was successful</returns>
    Task<bool> UpdateRankAsync(int empNr, string newRankCode, DateTime effectiveDate, string? notes = null);

    /// <summary>
    /// Gets faculty eligible for promotion
    /// </summary>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>Faculty eligible for promotion</returns>
    Task<(IEnumerable<Academic> Faculty, int TotalCount)> GetFacultyEligibleForPromotionAsync(
        int pageNumber = 1,
        int pageSize = 10);

    /// <summary>
    /// Gets promotion history for a faculty member
    /// </summary>
    /// <param name="empNr">Faculty employee number</param>
    /// <returns>List of promotion history entries</returns>
    Task<IEnumerable<FacultyPromotion>> GetPromotionHistoryAsync(int empNr);

    // Profile Management

    /// <summary>
    /// Gets faculty profile information
    /// </summary>
    /// <param name="empNr">Faculty employee number</param>
    /// <returns>Faculty profile if found</returns>
    Task<FacultyProfile?> GetFacultyProfileAsync(int empNr);

    /// <summary>
    /// Creates or updates faculty profile
    /// </summary>
    /// <param name="profile">Faculty profile to create or update</param>
    /// <returns>The created or updated profile</returns>
    Task<FacultyProfile> SaveFacultyProfileAsync(FacultyProfile profile);

    /// <summary>
    /// Gets faculty documents
    /// </summary>
    /// <param name="empNr">Faculty employee number</param>
    /// <param name="documentType">Optional document type filter</param>
    /// <returns>List of faculty documents</returns>
    Task<IEnumerable<FacultyDocument>> GetFacultyDocumentsAsync(int empNr, string? documentType = null);

    /// <summary>
    /// Gets faculty publications
    /// </summary>
    /// <param name="empNr">Faculty employee number</param>
    /// <param name="publicationType">Optional publication type filter</param>
    /// <returns>List of faculty publications</returns>
    Task<IEnumerable<FacultyPublication>> GetFacultyPublicationsAsync(int empNr, string? publicationType = null);

    /// <summary>
    /// Gets office assignments for a faculty member
    /// </summary>
    /// <param name="empNr">Faculty employee number</param>
    /// <returns>List of office assignments</returns>
    Task<IEnumerable<OfficeAssignment>> GetOfficeAssignmentsAsync(int empNr);

    // Research Management

    /// <summary>
    /// Gets research expertise for a faculty member
    /// </summary>
    /// <param name="empNr">Faculty employee number</param>
    /// <returns>List of research expertise areas</returns>
    Task<IEnumerable<FacultyExpertise>> GetResearchExpertiseAsync(int empNr);

    /// <summary>
    /// Adds research expertise for a faculty member
    /// </summary>
    /// <param name="empNr">Faculty employee number</param>
    /// <param name="researchAreaId">Research area ID</param>
    /// <param name="expertiseLevel">Level of expertise</param>
    /// <param name="yearsOfExperience">Years of experience in this area</param>
    /// <returns>The created expertise record</returns>
    Task<FacultyExpertise> AddResearchExpertiseAsync(int empNr, int researchAreaId, string expertiseLevel, int? yearsOfExperience = null);

    /// <summary>
    /// Removes research expertise for a faculty member
    /// </summary>
    /// <param name="empNr">Faculty employee number</param>
    /// <param name="expertiseId">Expertise record ID</param>
    /// <returns>True if removal was successful</returns>
    Task<bool> RemoveResearchExpertiseAsync(int empNr, int expertiseId);

    // Service Management

    /// <summary>
    /// Gets service records for a faculty member
    /// </summary>
    /// <param name="empNr">Faculty employee number</param>
    /// <param name="serviceType">Optional service type filter</param>
    /// <returns>List of service records</returns>
    Task<IEnumerable<FacultyServiceRecord>> GetServiceRecordsAsync(int empNr, string? serviceType = null);

    /// <summary>
    /// Gets committee assignments for a faculty member
    /// </summary>
    /// <param name="empNr">Faculty employee number</param>
    /// <param name="activeOnly">Whether to return only active assignments</param>
    /// <returns>List of committee assignments</returns>
    Task<IEnumerable<CommitteeMemberAssignment>> GetCommitteeAssignmentsAsync(int empNr, bool activeOnly = true);

    /// <summary>
    /// Gets administrative assignments for a faculty member
    /// </summary>
    /// <param name="empNr">Faculty employee number</param>
    /// <param name="activeOnly">Whether to return only active assignments</param>
    /// <returns>List of administrative assignments</returns>
    Task<IEnumerable<AdministrativeAssignment>> GetAdministrativeAssignmentsAsync(int empNr, bool activeOnly = true);

    // Statistics and Reporting

    /// <summary>
    /// Gets faculty statistics
    /// </summary>
    /// <returns>Faculty statistics object</returns>
    Task<object> GetFacultyStatisticsAsync();

    /// <summary>
    /// Gets department faculty statistics
    /// </summary>
    /// <param name="departmentName">Department name</param>
    /// <returns>Department-specific faculty statistics</returns>
    Task<object> GetDepartmentFacultyStatisticsAsync(string departmentName);

    // Utility Methods

    /// <summary>
    /// Checks if faculty exists
    /// </summary>
    /// <param name="empNr">The employee number to check</param>
    /// <returns>True if faculty exists</returns>
    Task<bool> FacultyExistsAsync(int empNr);

    /// <summary>
    /// Gets the total count of faculty
    /// </summary>
    /// <returns>Total number of faculty</returns>
    Task<int> GetTotalFacultyCountAsync();

    /// <summary>
    /// Validates if a rank change is allowed
    /// </summary>
    /// <param name="currentRankCode">Current rank code</param>
    /// <param name="newRankCode">Proposed new rank code</param>
    /// <returns>True if rank change is valid</returns>
    bool IsValidRankTransition(string? currentRankCode, string newRankCode);

    /// <summary>
    /// Gets all faculty (alternative method for controller compatibility)
    /// </summary>
    /// <returns>All faculty</returns>
    Task<IEnumerable<Academic>> GetAllFacultyAsync();

    /// <summary>
    /// Deactivates a faculty member
    /// </summary>
    /// <param name="empNr">Faculty employee number</param>
    /// <returns>Success status</returns>
    Task<bool> DeactivateFacultyAsync(int empNr);

    /// <summary>
    /// Uploads a document for a faculty member
    /// </summary>
    /// <param name="empNr">Faculty employee number</param>
    /// <param name="documentType">Document type</param>
    /// <param name="fileName">File name</param>
    /// <param name="fileContent">File content</param>
    /// <returns>Upload result</returns>
    Task<object> UploadDocumentAsync(int empNr, string documentType, string fileName, byte[] fileContent);

    /// <summary>
    /// Uploads a photo for a faculty member
    /// </summary>
    /// <param name="empNr">Faculty employee number</param>
    /// <param name="fileName">File name</param>
    /// <param name="fileContent">File content</param>
    /// <returns>Upload result</returns>
    Task<object> UploadPhotoAsync(int empNr, string fileName, byte[] fileContent);
}

/// <summary>
/// Enumeration for different types of faculty members
/// </summary>
public enum FacultyType
{
    /// <summary>
    /// Regular professor
    /// </summary>
    Professor = 1,

    /// <summary>
    /// Teaching professor
    /// </summary>
    TeachingProf = 2,

    /// <summary>
    /// All faculty types
    /// </summary>
    All = 0
}