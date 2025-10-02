using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Zeus.Academia.Api.Validation;

/// <summary>
/// Action filter that automatically validates request models before controller actions execute.
/// </summary>
public class ValidateModelAttribute : ActionFilterAttribute
{
    /// <summary>
    /// Called before the action method is invoked to validate the model.
    /// </summary>
    /// <param name="context">The action executing context</param>
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = new List<ValidationError>();

            foreach (var modelError in context.ModelState)
            {
                var propertyName = modelError.Key;
                foreach (var error in modelError.Value.Errors)
                {
                    errors.Add(new ValidationError(propertyName, error.ErrorMessage));
                }
            }

            var validationResult = ValidationResult.Failure(errors);
            throw validationResult.ToException();
        }

        base.OnActionExecuting(context);
    }
}

/// <summary>
/// Action filter that validates specific request models using custom validators.
/// </summary>
/// <typeparam name="T">The type of model to validate</typeparam>
public class ValidateModelAttribute<T> : ActionFilterAttribute where T : class
{
    private readonly IValidationService _validationService;

    public ValidateModelAttribute(IValidationService validationService)
    {
        _validationService = validationService;
    }

    /// <summary>
    /// Called before the action method is invoked to validate the specific model type.
    /// </summary>
    /// <param name="context">The action executing context</param>
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // First check ModelState for basic validation
        if (!context.ModelState.IsValid)
        {
            var errors = new List<ValidationError>();

            foreach (var modelError in context.ModelState)
            {
                var propertyName = modelError.Key;
                foreach (var error in modelError.Value.Errors)
                {
                    errors.Add(new ValidationError(propertyName, error.ErrorMessage));
                }
            }

            var modelStateResult = ValidationResult.Failure(errors);
            throw modelStateResult.ToException();
        }

        // Then run custom validation
        var model = context.ActionArguments.Values.OfType<T>().FirstOrDefault();
        if (model != null)
        {
            var validationResult = _validationService.Validate(model);
            if (!validationResult.IsValid)
            {
                throw validationResult.ToException();
            }
        }

        base.OnActionExecuting(context);
    }
}

/// <summary>
/// Async action filter that validates specific request models using custom validators.
/// </summary>
/// <typeparam name="T">The type of model to validate</typeparam>
public class ValidateModelAsyncAttribute<T> : IAsyncActionFilter where T : class
{
    private readonly IValidationService _validationService;

    public ValidateModelAsyncAttribute(IValidationService validationService)
    {
        _validationService = validationService;
    }

    /// <summary>
    /// Called asynchronously before the action method is invoked to validate the specific model type.
    /// </summary>
    /// <param name="context">The action executing context</param>
    /// <param name="next">The next action execution delegate</param>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // First check ModelState for basic validation
        if (!context.ModelState.IsValid)
        {
            var errors = new List<ValidationError>();

            foreach (var modelError in context.ModelState)
            {
                var propertyName = modelError.Key;
                foreach (var error in modelError.Value.Errors)
                {
                    errors.Add(new ValidationError(propertyName, error.ErrorMessage));
                }
            }

            var modelStateResult = ValidationResult.Failure(errors);
            throw modelStateResult.ToException();
        }

        // Then run custom validation asynchronously
        var model = context.ActionArguments.Values.OfType<T>().FirstOrDefault();
        if (model != null)
        {
            var validationResult = await _validationService.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                throw validationResult.ToException();
            }
        }

        await next();
    }
}