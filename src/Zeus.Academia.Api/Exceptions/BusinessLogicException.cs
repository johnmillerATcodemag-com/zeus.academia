namespace Zeus.Academia.Api.Exceptions;

/// <summary>
/// Exception thrown when business logic rules are violated
/// </summary>
public class BusinessLogicException : AcademiaException
{
    public override int HttpStatusCode => 400; // Bad Request

    /// <summary>
    /// Common error codes for business logic exceptions
    /// </summary>
    public static class ErrorCodes
    {
        public const string InvalidOperation = "BUSINESS_INVALID_OPERATION";
        public const string RuleViolation = "BUSINESS_RULE_VIOLATION";
        public const string StateConflict = "BUSINESS_STATE_CONFLICT";
        public const string DependencyViolation = "BUSINESS_DEPENDENCY_VIOLATION";
        public const string InsufficientPermissions = "BUSINESS_INSUFFICIENT_PERMISSIONS";
        public const string ResourceLimit = "BUSINESS_RESOURCE_LIMIT";
        public const string WorkflowViolation = "BUSINESS_WORKFLOW_VIOLATION";
    }

    public BusinessLogicException() : base("A business logic error occurred.")
    {
        ErrorCode = ErrorCodes.RuleViolation;
    }

    public BusinessLogicException(string message) : base(message)
    {
        ErrorCode = ErrorCodes.RuleViolation;
    }

    public BusinessLogicException(string message, string errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }

    public BusinessLogicException(string message, Exception innerException) : base(message, innerException)
    {
        ErrorCode = ErrorCodes.RuleViolation;
    }

    public BusinessLogicException(string message, string errorCode, Exception innerException) : base(message, innerException)
    {
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Creates a business logic exception for invalid operations
    /// </summary>
    /// <param name="operation">The operation that was attempted</param>
    /// <param name="reason">The reason why the operation is invalid</param>
    /// <returns>BusinessLogicException instance</returns>
    public static BusinessLogicException InvalidOperation(string operation, string reason)
    {
        var exception = new BusinessLogicException($"Invalid operation '{operation}': {reason}", ErrorCodes.InvalidOperation);
        exception.AddContext("operation", operation);
        exception.AddContext("reason", reason);
        return exception;
    }

    /// <summary>
    /// Creates a business logic exception for rule violations
    /// </summary>
    /// <param name="rule">The rule that was violated</param>
    /// <param name="details">Additional details about the violation</param>
    /// <returns>BusinessLogicException instance</returns>
    public static BusinessLogicException RuleViolation(string rule, string details)
    {
        var exception = new BusinessLogicException($"Business rule violation: {rule}. {details}", ErrorCodes.RuleViolation);
        exception.AddContext("rule", rule);
        exception.AddContext("details", details);
        return exception;
    }

    /// <summary>
    /// Creates a business logic exception for state conflicts
    /// </summary>
    /// <param name="currentState">The current state</param>
    /// <param name="requiredState">The required state</param>
    /// <returns>BusinessLogicException instance</returns>
    public static BusinessLogicException StateConflict(string currentState, string requiredState)
    {
        var exception = new BusinessLogicException($"State conflict: Current state '{currentState}' does not allow this operation. Required state: '{requiredState}'", ErrorCodes.StateConflict);
        exception.AddContext("currentState", currentState);
        exception.AddContext("requiredState", requiredState);
        return exception;
    }

    /// <summary>
    /// Creates a business logic exception for dependency violations
    /// </summary>
    /// <param name="resource">The resource with dependencies</param>
    /// <param name="dependencies">The blocking dependencies</param>
    /// <returns>BusinessLogicException instance</returns>
    public static BusinessLogicException DependencyViolation(string resource, string dependencies)
    {
        var exception = new BusinessLogicException($"Cannot modify '{resource}' due to existing dependencies: {dependencies}", ErrorCodes.DependencyViolation);
        exception.AddContext("resource", resource);
        exception.AddContext("dependencies", dependencies);
        return exception;
    }
}