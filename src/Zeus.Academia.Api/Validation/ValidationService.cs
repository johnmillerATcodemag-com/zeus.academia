namespace Zeus.Academia.Api.Validation;

/// <summary>
/// Service for performing model validation using registered validators.
/// </summary>
public interface IValidationService
{
    /// <summary>
    /// Validates the specified model using the appropriate validator.
    /// </summary>
    /// <typeparam name="T">The type of model to validate</typeparam>
    /// <param name="model">The model to validate</param>
    /// <returns>Validation result containing any errors found</returns>
    ValidationResult Validate<T>(T model);

    /// <summary>
    /// Asynchronously validates the specified model using the appropriate validator.
    /// </summary>
    /// <typeparam name="T">The type of model to validate</typeparam>
    /// <param name="model">The model to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task containing validation result with any errors found</returns>
    Task<ValidationResult> ValidateAsync<T>(T model, CancellationToken cancellationToken = default);
}

/// <summary>
/// Implementation of validation service that uses dependency injection to resolve validators.
/// </summary>
public class ValidationService : IValidationService
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Validates the specified model using the appropriate validator.
    /// </summary>
    /// <typeparam name="T">The type of model to validate</typeparam>
    /// <param name="model">The model to validate</param>
    /// <returns>Validation result containing any errors found</returns>
    public ValidationResult Validate<T>(T model)
    {
        var validator = _serviceProvider.GetService<IValidator<T>>();

        if (validator == null)
        {
            // No validator registered for this type - consider it valid
            return ValidationResult.Success();
        }

        return validator.Validate(model);
    }

    /// <summary>
    /// Asynchronously validates the specified model using the appropriate validator.
    /// </summary>
    /// <typeparam name="T">The type of model to validate</typeparam>
    /// <param name="model">The model to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task containing validation result with any errors found</returns>
    public async Task<ValidationResult> ValidateAsync<T>(T model, CancellationToken cancellationToken = default)
    {
        var validator = _serviceProvider.GetService<IValidator<T>>();

        if (validator == null)
        {
            // No validator registered for this type - consider it valid
            return ValidationResult.Success();
        }

        return await validator.ValidateAsync(model, cancellationToken);
    }
}