namespace Zeus.Academia.Api.Validation;

/// <summary>
/// Interface for model validators providing consistent validation across the API.
/// </summary>
/// <typeparam name="T">The type of model to validate</typeparam>
public interface IValidator<in T>
{
    /// <summary>
    /// Validates the specified model and returns validation results.
    /// </summary>
    /// <param name="model">The model to validate</param>
    /// <returns>Validation result containing any errors found</returns>
    ValidationResult Validate(T model);

    /// <summary>
    /// Asynchronously validates the specified model and returns validation results.
    /// </summary>
    /// <param name="model">The model to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task containing validation result with any errors found</returns>
    Task<ValidationResult> ValidateAsync(T model, CancellationToken cancellationToken = default);
}