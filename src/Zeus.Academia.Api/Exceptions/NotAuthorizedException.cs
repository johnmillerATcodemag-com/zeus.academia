namespace Zeus.Academia.Api.Exceptions;

/// <summary>
/// Exception thrown when a user is not authorized to perform an action
/// </summary>
public class NotAuthorizedException : AcademiaException
{
    public override int HttpStatusCode => 403; // Forbidden

    /// <summary>
    /// Common error codes for authorization exceptions
    /// </summary>
    public static class ErrorCodes
    {
        public const string InsufficientPermissions = "AUTH_INSUFFICIENT_PERMISSIONS";
        public const string RoleRequired = "AUTH_ROLE_REQUIRED";
        public const string ResourceAccess = "AUTH_RESOURCE_ACCESS_DENIED";
        public const string DepartmentAccess = "AUTH_DEPARTMENT_ACCESS_DENIED";
        public const string OperationNotAllowed = "AUTH_OPERATION_NOT_ALLOWED";
        public const string ContextRequired = "AUTH_CONTEXT_REQUIRED";
        public const string ExpiredPermissions = "AUTH_EXPIRED_PERMISSIONS";
        public const string SuspendedAccount = "AUTH_ACCOUNT_SUSPENDED";
    }

    public NotAuthorizedException() : base("You are not authorized to perform this action.")
    {
        ErrorCode = ErrorCodes.InsufficientPermissions;
    }

    public NotAuthorizedException(string message) : base(message)
    {
        ErrorCode = ErrorCodes.InsufficientPermissions;
    }

    public NotAuthorizedException(string message, string errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }

    public NotAuthorizedException(string message, Exception innerException) : base(message, innerException)
    {
        ErrorCode = ErrorCodes.InsufficientPermissions;
    }

    public NotAuthorizedException(string message, string errorCode, Exception innerException) : base(message, innerException)
    {
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Creates an authorization exception for insufficient permissions
    /// </summary>
    /// <param name="requiredPermission">The permission that was required</param>
    /// <param name="resource">The resource being accessed</param>
    /// <returns>NotAuthorizedException instance</returns>
    public static NotAuthorizedException InsufficientPermissions(string requiredPermission, string? resource = null)
    {
        var message = resource != null
            ? $"Insufficient permissions to access '{resource}'. Required permission: {requiredPermission}"
            : $"Insufficient permissions. Required permission: {requiredPermission}";

        var exception = new NotAuthorizedException(message, ErrorCodes.InsufficientPermissions);
        exception.AddContext("requiredPermission", requiredPermission);
        exception.AddContext("resource", resource ?? "unknown");
        return exception;
    }

    /// <summary>
    /// Creates an authorization exception for missing role
    /// </summary>
    /// <param name="requiredRole">The role that was required</param>
    /// <param name="operation">The operation being attempted</param>
    /// <returns>NotAuthorizedException instance</returns>
    public static NotAuthorizedException RoleRequired(string requiredRole, string operation)
    {
        var exception = new NotAuthorizedException($"Role '{requiredRole}' is required to perform '{operation}'.", ErrorCodes.RoleRequired);
        exception.AddContext("requiredRole", requiredRole);
        exception.AddContext("operation", operation);
        return exception;
    }

    /// <summary>
    /// Creates an authorization exception for resource access denial
    /// </summary>
    /// <param name="resourceType">The type of resource</param>
    /// <param name="resourceId">The ID of the resource</param>
    /// <param name="reason">The reason for denial</param>
    /// <returns>NotAuthorizedException instance</returns>
    public static NotAuthorizedException ResourceAccess(string resourceType, string resourceId, string reason)
    {
        var exception = new NotAuthorizedException($"Access denied to {resourceType} '{resourceId}': {reason}", ErrorCodes.ResourceAccess);
        exception.AddContext("resourceType", resourceType);
        exception.AddContext("resourceId", resourceId);
        exception.AddContext("reason", reason);
        return exception;
    }

    /// <summary>
    /// Creates an authorization exception for department access denial
    /// </summary>
    /// <param name="departmentName">The name of the department</param>
    /// <param name="operation">The operation being attempted</param>
    /// <returns>NotAuthorizedException instance</returns>
    public static NotAuthorizedException DepartmentAccess(string departmentName, string operation)
    {
        var exception = new NotAuthorizedException($"Access denied to department '{departmentName}' for operation '{operation}'.", ErrorCodes.DepartmentAccess);
        exception.AddContext("department", departmentName);
        exception.AddContext("operation", operation);
        return exception;
    }

    /// <summary>
    /// Creates an authorization exception for operations not allowed based on current state
    /// </summary>
    /// <param name="operation">The operation being attempted</param>
    /// <param name="currentState">The current state that prevents the operation</param>
    /// <returns>NotAuthorizedException instance</returns>
    public static NotAuthorizedException OperationNotAllowed(string operation, string currentState)
    {
        var exception = new NotAuthorizedException($"Operation '{operation}' is not allowed in current state: {currentState}", ErrorCodes.OperationNotAllowed);
        exception.AddContext("operation", operation);
        exception.AddContext("currentState", currentState);
        return exception;
    }

    /// <summary>
    /// Creates an authorization exception for suspended accounts
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="suspensionReason">The reason for suspension</param>
    /// <returns>NotAuthorizedException instance</returns>
    public static NotAuthorizedException AccountSuspended(string userId, string suspensionReason)
    {
        var exception = new NotAuthorizedException($"Account is suspended: {suspensionReason}", ErrorCodes.SuspendedAccount);
        exception.AddContext("userId", userId);
        exception.AddContext("suspensionReason", suspensionReason);
        return exception;
    }
}