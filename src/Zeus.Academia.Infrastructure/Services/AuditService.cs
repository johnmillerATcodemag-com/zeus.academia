using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Text.Json;

namespace Zeus.Academia.Infrastructure.Services;

/// <summary>
/// Represents an audit log entry.
/// </summary>
public class AuditLogEntry
{
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string? EntityId { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? AdditionalData { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public bool IsSuccess { get; set; } = true;
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Service for creating audit trails of user actions and system events.
/// </summary>
public interface IAuditService
{
    /// <summary>
    /// Logs a user action for audit purposes.
    /// </summary>
    Task LogActionAsync(string action, string entityType, string? entityId = null,
        object? oldValues = null, object? newValues = null, object? additionalData = null,
        string? userId = null, string? userName = null, string? ipAddress = null,
        string? userAgent = null, bool isSuccess = true, string? errorMessage = null);

    /// <summary>
    /// Logs a user login event.
    /// </summary>
    Task LogLoginAsync(string userId, string userName, string? ipAddress = null,
        string? userAgent = null, bool isSuccess = true, string? errorMessage = null);

    /// <summary>
    /// Logs a user logout event.
    /// </summary>
    Task LogLogoutAsync(string userId, string userName, string? ipAddress = null,
        string? userAgent = null);

    /// <summary>
    /// Logs a password change event.
    /// </summary>
    Task LogPasswordChangeAsync(string userId, string userName, string? ipAddress = null,
        string? userAgent = null, bool isSuccess = true, string? errorMessage = null);

    /// <summary>
    /// Logs a role assignment change.
    /// </summary>
    Task LogRoleChangeAsync(string targetUserId, string targetUserName, string action,
        string roleName, string? performedByUserId = null, string? performedByUserName = null,
        string? ipAddress = null, string? userAgent = null, bool isSuccess = true,
        string? errorMessage = null);

    /// <summary>
    /// Logs administrative actions.
    /// </summary>
    Task LogAdminActionAsync(string action, string description, string? targetEntityType = null,
        string? targetEntityId = null, string? performedByUserId = null,
        string? performedByUserName = null, string? ipAddress = null, string? userAgent = null,
        bool isSuccess = true, string? errorMessage = null);

    /// <summary>
    /// Logs data access events for sensitive information.
    /// </summary>
    Task LogDataAccessAsync(string entityType, string entityId, string accessType,
        string? userId = null, string? userName = null, string? ipAddress = null,
        string? userAgent = null, bool isSuccess = true, string? errorMessage = null);
}

/// <summary>
/// Implementation of audit service using structured logging.
/// </summary>
public class AuditService : IAuditService
{
    private readonly ILogger<AuditService> _logger;

    public AuditService(ILogger<AuditService> logger)
    {
        _logger = logger;
    }

    public Task LogActionAsync(string action, string entityType, string? entityId = null,
        object? oldValues = null, object? newValues = null, object? additionalData = null,
        string? userId = null, string? userName = null, string? ipAddress = null,
        string? userAgent = null, bool isSuccess = true, string? errorMessage = null)
    {
        var auditEntry = new AuditLogEntry
        {
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            OldValues = SerializeObject(oldValues),
            NewValues = SerializeObject(newValues),
            AdditionalData = SerializeObject(additionalData),
            UserId = userId,
            UserName = userName,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            IsSuccess = isSuccess,
            ErrorMessage = errorMessage
        };

        LogAuditEntry(auditEntry);
        return Task.CompletedTask;
    }

    public Task LogLoginAsync(string userId, string userName, string? ipAddress = null,
        string? userAgent = null, bool isSuccess = true, string? errorMessage = null)
    {
        var auditEntry = new AuditLogEntry
        {
            Action = isSuccess ? "LOGIN_SUCCESS" : "LOGIN_FAILED",
            EntityType = "User",
            EntityId = userId,
            UserId = userId,
            UserName = userName,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            IsSuccess = isSuccess,
            ErrorMessage = errorMessage
        };

        LogAuditEntry(auditEntry);
        return Task.CompletedTask;
    }

    public Task LogLogoutAsync(string userId, string userName, string? ipAddress = null,
        string? userAgent = null)
    {
        var auditEntry = new AuditLogEntry
        {
            Action = "LOGOUT",
            EntityType = "User",
            EntityId = userId,
            UserId = userId,
            UserName = userName,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            IsSuccess = true
        };

        LogAuditEntry(auditEntry);
        return Task.CompletedTask;
    }

    public Task LogPasswordChangeAsync(string userId, string userName, string? ipAddress = null,
        string? userAgent = null, bool isSuccess = true, string? errorMessage = null)
    {
        var auditEntry = new AuditLogEntry
        {
            Action = isSuccess ? "PASSWORD_CHANGE_SUCCESS" : "PASSWORD_CHANGE_FAILED",
            EntityType = "User",
            EntityId = userId,
            UserId = userId,
            UserName = userName,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            IsSuccess = isSuccess,
            ErrorMessage = errorMessage
        };

        LogAuditEntry(auditEntry);
        return Task.CompletedTask;
    }

    public Task LogRoleChangeAsync(string targetUserId, string targetUserName, string action,
        string roleName, string? performedByUserId = null, string? performedByUserName = null,
        string? ipAddress = null, string? userAgent = null, bool isSuccess = true,
        string? errorMessage = null)
    {
        var auditEntry = new AuditLogEntry
        {
            Action = $"ROLE_{action.ToUpperInvariant()}",
            EntityType = "UserRole",
            EntityId = targetUserId,
            UserId = performedByUserId,
            UserName = performedByUserName,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            IsSuccess = isSuccess,
            ErrorMessage = errorMessage,
            AdditionalData = JsonSerializer.Serialize(new
            {
                TargetUserId = targetUserId,
                TargetUserName = targetUserName,
                RoleName = roleName,
                Action = action
            })
        };

        LogAuditEntry(auditEntry);
        return Task.CompletedTask;
    }

    public Task LogAdminActionAsync(string action, string description, string? targetEntityType = null,
        string? targetEntityId = null, string? performedByUserId = null,
        string? performedByUserName = null, string? ipAddress = null, string? userAgent = null,
        bool isSuccess = true, string? errorMessage = null)
    {
        var auditEntry = new AuditLogEntry
        {
            Action = $"ADMIN_{action.ToUpperInvariant()}",
            EntityType = targetEntityType ?? "System",
            EntityId = targetEntityId,
            UserId = performedByUserId,
            UserName = performedByUserName,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            IsSuccess = isSuccess,
            ErrorMessage = errorMessage,
            AdditionalData = JsonSerializer.Serialize(new { Description = description })
        };

        LogAuditEntry(auditEntry);
        return Task.CompletedTask;
    }

    public Task LogDataAccessAsync(string entityType, string entityId, string accessType,
        string? userId = null, string? userName = null, string? ipAddress = null,
        string? userAgent = null, bool isSuccess = true, string? errorMessage = null)
    {
        var auditEntry = new AuditLogEntry
        {
            Action = $"DATA_{accessType.ToUpperInvariant()}",
            EntityType = entityType,
            EntityId = entityId,
            UserId = userId,
            UserName = userName,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            IsSuccess = isSuccess,
            ErrorMessage = errorMessage
        };

        LogAuditEntry(auditEntry);
        return Task.CompletedTask;
    }

    private void LogAuditEntry(AuditLogEntry entry)
    {
        if (entry.IsSuccess)
        {
            _logger.LogInformation("AUDIT: {Action} on {EntityType}({EntityId}) by {UserName}({UserId}) from {IpAddress} - Success",
                entry.Action, entry.EntityType, entry.EntityId, entry.UserName, entry.UserId, entry.IpAddress);
        }
        else
        {
            _logger.LogWarning("AUDIT: {Action} on {EntityType}({EntityId}) by {UserName}({UserId}) from {IpAddress} - Failed: {ErrorMessage}",
                entry.Action, entry.EntityType, entry.EntityId, entry.UserName, entry.UserId, entry.IpAddress, entry.ErrorMessage);
        }

        // Log full audit entry as structured data
        _logger.LogInformation("AUDIT_ENTRY: {@AuditEntry}", entry);
    }

    private static string? SerializeObject(object? obj)
    {
        if (obj == null) return null;

        try
        {
            return JsonSerializer.Serialize(obj, new JsonSerializerOptions
            {
                WriteIndented = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
        catch
        {
            return obj.ToString();
        }
    }
}

/// <summary>
/// Extension methods for audit service context extraction.
/// </summary>
public static class AuditServiceExtensions
{
    /// <summary>
    /// Extracts user information from ClaimsPrincipal for audit logging.
    /// </summary>
    public static (string? userId, string? userName) GetUserInfo(this ClaimsPrincipal user)
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userName = user.FindFirst(ClaimTypes.Name)?.Value ?? user.FindFirst(ClaimTypes.Email)?.Value;
        return (userId, userName);
    }
}