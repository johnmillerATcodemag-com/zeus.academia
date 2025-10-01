using Zeus.Academia.Infrastructure.Identity;
using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.Infrastructure.Services;

/// <summary>
/// Service interface for user management operations including registration, profile management, and administration
/// </summary>
public interface IUserService
{
    // User Registration and Creation
    /// <summary>
    /// Register a new user with automatic role assignment based on academic entity type
    /// </summary>
    /// <param name="request">User registration request</param>
    /// <returns>Registration result with user information or errors</returns>
    Task<UserRegistrationResult> RegisterUserAsync(UserRegistrationRequest request);

    /// <summary>
    /// Create a user account linked to an existing academic entity
    /// </summary>
    /// <param name="academicId">Academic entity ID to link to</param>
    /// <param name="email">User email address</param>
    /// <param name="temporaryPassword">Temporary password (will require change on first login)</param>
    /// <returns>Created user or error result</returns>
    Task<UserCreationResult> CreateUserForAcademicAsync(int academicId, string email, string? temporaryPassword = null);

    // User Authentication and Validation
    /// <summary>
    /// Validate user credentials and return user information
    /// </summary>
    /// <param name="usernameOrEmail">Username or email address</param>
    /// <param name="password">Password</param>
    /// <returns>Authentication result with user information</returns>
    Task<UserAuthenticationResult> ValidateUserAsync(string usernameOrEmail, string password);

    /// <summary>
    /// Check if a user exists by username or email
    /// </summary>
    /// <param name="usernameOrEmail">Username or email to check</param>
    /// <returns>True if user exists</returns>
    Task<bool> UserExistsAsync(string usernameOrEmail);

    // Profile Management
    /// <summary>
    /// Get user profile information by user ID
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>User profile information</returns>
    Task<UserProfile?> GetUserProfileAsync(int userId);

    /// <summary>
    /// Update user profile information
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="profile">Updated profile information</param>
    /// <returns>Update result</returns>
    Task<UserUpdateResult> UpdateUserProfileAsync(int userId, UserProfileUpdateRequest profile);

    /// <summary>
    /// Change user password
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="currentPassword">Current password</param>
    /// <param name="newPassword">New password</param>
    /// <returns>Password change result</returns>
    Task<PasswordChangeResult> ChangePasswordAsync(int userId, string currentPassword, string newPassword);

    // Email Confirmation and Password Reset
    /// <summary>
    /// Send email confirmation token to user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Result of email sending operation</returns>
    Task<EmailOperationResult> SendEmailConfirmationAsync(int userId);

    /// <summary>
    /// Confirm user email with token
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="token">Confirmation token</param>
    /// <returns>Confirmation result</returns>
    Task<EmailConfirmationResult> ConfirmEmailAsync(int userId, string token);

    /// <summary>
    /// Send password reset token to user
    /// </summary>
    /// <param name="usernameOrEmail">Username or email address</param>
    /// <returns>Result of password reset request</returns>
    Task<PasswordResetRequestResult> RequestPasswordResetAsync(string usernameOrEmail);

    /// <summary>
    /// Reset password using reset token
    /// </summary>
    /// <param name="token">Reset token</param>
    /// <param name="newPassword">New password</param>
    /// <returns>Password reset result</returns>
    Task<PasswordResetResult> ResetPasswordAsync(string token, string newPassword);

    // User Administration
    /// <summary>
    /// Search users with filtering and pagination
    /// </summary>
    /// <param name="searchRequest">Search criteria and pagination</param>
    /// <returns>Paged list of users</returns>
    Task<PagedResult<UserSummary>> SearchUsersAsync(UserSearchRequest searchRequest);

    /// <summary>
    /// Get detailed user information for administration
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Detailed user information</returns>
    Task<UserAdminDetails?> GetUserDetailsAsync(int userId);

    /// <summary>
    /// Activate or deactivate user account
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="isActive">Active status</param>
    /// <param name="reason">Reason for status change</param>
    /// <returns>Status change result</returns>
    Task<UserStatusChangeResult> SetUserActiveStatusAsync(int userId, bool isActive, string? reason = null);

    /// <summary>
    /// Lock or unlock user account
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="lockUntil">Lock until date (null to unlock)</param>
    /// <param name="reason">Reason for lock/unlock</param>
    /// <returns>Lock status change result</returns>
    Task<UserStatusChangeResult> SetUserLockStatusAsync(int userId, DateTimeOffset? lockUntil, string? reason = null);

    /// <summary>
    /// Get user's current roles
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>List of user roles with context</returns>
    Task<List<UserRoleInfo>> GetUserRolesAsync(int userId);

    /// <summary>
    /// Assign role to user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="roleId">Role ID</param>
    /// <param name="departmentContext">Optional department context</param>
    /// <param name="expirationDate">Optional expiration date</param>
    /// <param name="assignedBy">User ID of person making assignment</param>
    /// <param name="reason">Reason for assignment</param>
    /// <returns>Role assignment result</returns>
    Task<RoleAssignmentResult> AssignRoleToUserAsync(int userId, int roleId, string? departmentContext = null,
        DateTime? expirationDate = null, int? assignedBy = null, string? reason = null);

    /// <summary>
    /// Remove role from user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="roleId">Role ID</param>
    /// <param name="departmentContext">Department context (if role is department-specific)</param>
    /// <param name="reason">Reason for removal</param>
    /// <returns>Role removal result</returns>
    Task<RoleAssignmentResult> RemoveRoleFromUserAsync(int userId, int roleId, string? departmentContext = null, string? reason = null);

    // Bulk Operations
    /// <summary>
    /// Import users from external source (e.g., CSV, Active Directory)
    /// </summary>
    /// <param name="users">List of users to import</param>
    /// <returns>Import result with success/failure details</returns>
    Task<UserImportResult> ImportUsersAsync(List<UserImportRequest> users);

    /// <summary>
    /// Export users to specified format
    /// </summary>
    /// <param name="searchCriteria">Search criteria for users to export</param>
    /// <param name="format">Export format (CSV, Excel, etc.)</param>
    /// <returns>Export result with data</returns>
    Task<UserExportResult> ExportUsersAsync(UserSearchRequest searchCriteria, ExportFormat format);
}

// Supporting DTOs and Result Types

/// <summary>
/// User registration request
/// </summary>
public class UserRegistrationRequest
{
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int? AcademicId { get; set; }
    public string? DepartmentName { get; set; }
    public bool SendConfirmationEmail { get; set; } = true;
}

/// <summary>
/// User registration result
/// </summary>
public class UserRegistrationResult
{
    public bool Success { get; set; }
    public AcademiaUser? User { get; set; }
    public List<string> Errors { get; set; } = new();
    public string? ConfirmationToken { get; set; }
}

/// <summary>
/// User creation result
/// </summary>
public class UserCreationResult
{
    public bool Success { get; set; }
    public AcademiaUser? User { get; set; }
    public List<string> Errors { get; set; } = new();
    public string? TemporaryPassword { get; set; }
}

/// <summary>
/// User authentication result
/// </summary>
public class UserAuthenticationResult
{
    public bool Success { get; set; }
    public AcademiaUser? User { get; set; }
    public string? FailureReason { get; set; }
    public bool IsLockedOut { get; set; }
    public bool RequiresPasswordChange { get; set; }
}

/// <summary>
/// User profile information
/// </summary>
public class UserProfile
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? DisplayName { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public Academic? Academic { get; set; }
    public List<UserRoleInfo> Roles { get; set; } = new();
}

/// <summary>
/// User profile update request
/// </summary>
public class UserProfileUpdateRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? DisplayName { get; set; }
    public string? Email { get; set; }
    public string? UserName { get; set; }
}

/// <summary>
/// User update result
/// </summary>
public class UserUpdateResult
{
    public bool Success { get; set; }
    public List<string> Errors { get; set; } = new();
    public bool RequiresEmailConfirmation { get; set; }
}

/// <summary>
/// Password change result
/// </summary>
public class PasswordChangeResult
{
    public bool Success { get; set; }
    public List<string> Errors { get; set; } = new();
}

/// <summary>
/// Email operation result
/// </summary>
public class EmailOperationResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public string? Token { get; set; }
}

/// <summary>
/// Email confirmation result
/// </summary>
public class EmailConfirmationResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
}

/// <summary>
/// Password reset request result
/// </summary>
public class PasswordResetRequestResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public string? Token { get; set; }
}

/// <summary>
/// Password reset result
/// </summary>
public class PasswordResetResult
{
    public bool Success { get; set; }
    public List<string> Errors { get; set; } = new();
}

/// <summary>
/// User search request
/// </summary>
public class UserSearchRequest
{
    public string? SearchTerm { get; set; }
    public string? Email { get; set; }
    public string? Department { get; set; }
    public string? Role { get; set; }
    public bool? IsActive { get; set; }
    public bool? EmailConfirmed { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
    public DateTime? LastLoginAfter { get; set; }
    public DateTime? LastLoginBefore { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string SortBy { get; set; } = "CreatedDate";
    public string SortDirection { get; set; } = "desc";
}

/// <summary>
/// Paged result wrapper
/// </summary>
public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}

/// <summary>
/// User summary for lists
/// </summary>
public class UserSummary
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public bool IsActive { get; set; }
    public bool EmailConfirmed { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public List<string> Roles { get; set; } = new();
}

/// <summary>
/// Detailed user information for administration
/// </summary>
public class UserAdminDetails
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? DisplayName { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public string? LastLoginIpAddress { get; set; }
    public int AccessFailedCount { get; set; }
    public DateTimeOffset? LockoutEnd { get; set; }
    public bool LockoutEnabled { get; set; }
    public Academic? Academic { get; set; }
    public List<UserRoleInfo> Roles { get; set; } = new();
    public List<RefreshToken> ActiveTokens { get; set; } = new();
}

/// <summary>
/// User role information
/// </summary>
public class UserRoleInfo
{
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string? DepartmentContext { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public bool IsActive { get; set; }
    public bool IsPrimary { get; set; }
    public string? AssignedBy { get; set; }
    public string? AssignmentReason { get; set; }
}

/// <summary>
/// User status change result
/// </summary>
public class UserStatusChangeResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
}

/// <summary>
/// Role assignment result
/// </summary>
public class RoleAssignmentResult
{
    public bool Success { get; set; }
    public List<string> Errors { get; set; } = new();
}

/// <summary>
/// User import request
/// </summary>
public class UserImportRequest
{
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int? AcademicId { get; set; }
    public string? DepartmentName { get; set; }
    public List<string> Roles { get; set; } = new();
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// User import result
/// </summary>
public class UserImportResult
{
    public int TotalProcessed { get; set; }
    public int SuccessCount { get; set; }
    public int ErrorCount { get; set; }
    public List<UserImportError> Errors { get; set; } = new();
}

/// <summary>
/// User import error
/// </summary>
public class UserImportError
{
    public int RowNumber { get; set; }
    public string Email { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
}

/// <summary>
/// User export result
/// </summary>
public class UserExportResult
{
    public bool Success { get; set; }
    public byte[]? Data { get; set; }
    public string? FileName { get; set; }
    public string? ContentType { get; set; }
    public string? Error { get; set; }
}

/// <summary>
/// Export format enumeration
/// </summary>
public enum ExportFormat
{
    Csv,
    Excel,
    Json
}