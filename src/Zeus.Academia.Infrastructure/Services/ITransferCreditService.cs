using Zeus.Academia.Infrastructure.Models;

namespace Zeus.Academia.Infrastructure.Services;

/// <summary>
/// Interface for transfer credit evaluation services.
/// </summary>
public interface ITransferCreditService
{
    /// <summary>
    /// Evaluate transfer credits automatically using equivalency mappings.
    /// </summary>
    /// <param name="transferRequest">Transfer credit request</param>
    /// <param name="policies">Optional transfer policies to apply</param>
    /// <returns>Transfer credit evaluation result</returns>
    Task<TransferCreditEvaluation> EvaluateTransferCreditsAsync(TransferCreditRequest transferRequest, TransferCreditPolicies? policies = null);

    /// <summary>
    /// Get transfer credit policies for the institution.
    /// </summary>
    /// <returns>Current transfer credit policies</returns>
    Task<TransferCreditPolicies> GetTransferCreditPoliciesAsync();

    /// <summary>
    /// Update transfer credit policies.
    /// </summary>
    /// <param name="policies">Updated policies</param>
    /// <returns>True if successful</returns>
    Task<bool> UpdateTransferCreditPoliciesAsync(TransferCreditPolicies policies);

    /// <summary>
    /// Get transfer credit history for a student.
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <returns>List of transfer credit evaluations</returns>
    Task<List<TransferCreditEvaluation>> GetStudentTransferCreditsAsync(int studentId);

    /// <summary>
    /// Apply approved transfer credits to student record.
    /// </summary>
    /// <param name="evaluationId">Evaluation ID</param>
    /// <param name="approvedBy">User who approved the credits</param>
    /// <returns>True if successful</returns>
    Task<bool> ApplyTransferCreditsAsync(int evaluationId, string approvedBy);

    /// <summary>
    /// Validate external course information for transfer evaluation.
    /// </summary>
    /// <param name="externalCourse">External course to validate</param>
    /// <returns>Validation result</returns>
    Task<CourseValidationResult> ValidateExternalCourseAsync(ExternalCourse externalCourse);

    /// <summary>
    /// Get institution mapping for transfer credit evaluation.
    /// </summary>
    /// <param name="institutionCode">Institution code</param>
    /// <returns>Institution mapping information</returns>
    Task<InstitutionMapping?> GetInstitutionMappingAsync(string institutionCode);

    /// <summary>
    /// Create or update institution mapping for transfer credits.
    /// </summary>
    /// <param name="mapping">Institution mapping</param>
    /// <returns>Updated mapping</returns>
    Task<InstitutionMapping> UpdateInstitutionMappingAsync(InstitutionMapping mapping);
}

/// <summary>
/// Institution mapping for transfer credit evaluation
/// </summary>
public class InstitutionMapping
{
    public int Id { get; set; }
    public string InstitutionCode { get; set; } = string.Empty;
    public string InstitutionName { get; set; } = string.Empty;
    public string AccreditationStatus { get; set; } = string.Empty;
    public bool IsApprovedForTransfer { get; set; }
    public decimal TransferCreditMultiplier { get; set; } = 1.0m;
    public List<string> RestrictedSubjects { get; set; } = new();
    public DateTime EffectiveDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
}