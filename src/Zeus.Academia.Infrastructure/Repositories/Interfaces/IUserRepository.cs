using System.Linq.Expressions;
using Zeus.Academia.Infrastructure.Identity;
using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.Infrastructure.Repositories.Interfaces;

/// <summary>
/// Repository interface for user management operations.
/// Extends the generic repository with user-specific functionality.
/// </summary>
public interface IUserRepository : IRepository<AcademiaUser>
{
    #region User Queries

    /// <summary>
    /// Gets a user by email address asynchronously.
    /// </summary>
    /// <param name="email">The email address</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The user if found, otherwise null</returns>
    Task<AcademiaUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by username asynchronously.
    /// </summary>
    /// <param name="userName">The username</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The user if found, otherwise null</returns>
    Task<AcademiaUser?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user with their academic information asynchronously.
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The user with academic info if found, otherwise null</returns>
    Task<AcademiaUser?> GetWithAcademicAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user with their roles asynchronously.
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The user with roles if found, otherwise null</returns>
    Task<AcademiaUser?> GetWithRolesAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user with their refresh tokens asynchronously.
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The user with refresh tokens if found, otherwise null</returns>
    Task<AcademiaUser?> GetWithRefreshTokensAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user with all related data (academic, roles, tokens) asynchronously.
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The user with all related data if found, otherwise null</returns>
    Task<AcademiaUser?> GetWithAllDataAsync(int userId, CancellationToken cancellationToken = default);

    #endregion

    #region User Search and Filtering

    /// <summary>
    /// Searches users based on various criteria with pagination.
    /// </summary>
    /// <param name="searchTerm">General search term (searches name, email, username)</param>
    /// <param name="email">Email filter</param>
    /// <param name="isActive">Active status filter</param>
    /// <param name="emailConfirmed">Email confirmed status filter</param>
    /// <param name="role">Role name filter</param>
    /// <param name="department">Department filter</param>
    /// <param name="createdAfter">Created after date filter</param>
    /// <param name="createdBefore">Created before date filter</param>
    /// <param name="lastLoginAfter">Last login after date filter</param>
    /// <param name="lastLoginBefore">Last login before date filter</param>
    /// <param name="orderBy">Order by expression</param>
    /// <param name="orderDescending">Order direction</param>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated user results</returns>
    Task<(IEnumerable<AcademiaUser> Users, int TotalCount)> SearchUsersAsync(
        string? searchTerm = null,
        string? email = null,
        bool? isActive = null,
        bool? emailConfirmed = null,
        string? role = null,
        string? department = null,
        DateTime? createdAfter = null,
        DateTime? createdBefore = null,
        DateTime? lastLoginAfter = null,
        DateTime? lastLoginBefore = null,
        Expression<Func<AcademiaUser, object>>? orderBy = null,
        bool orderDescending = false,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets users by role name asynchronously.
    /// </summary>
    /// <param name="roleName">The role name</param>
    /// <param name="includeInactive">Whether to include inactive users</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Users with the specified role</returns>
    Task<IEnumerable<AcademiaUser>> GetByRoleAsync(string roleName, bool includeInactive = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets users by academic type asynchronously.
    /// </summary>
    /// <param name="academicType">The academic type</param>
    /// <param name="includeInactive">Whether to include inactive users</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Users of the specified academic type</returns>
    Task<IEnumerable<AcademiaUser>> GetByAcademicTypeAsync(Type academicType, bool includeInactive = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets users who haven't confirmed their email asynchronously.
    /// </summary>
    /// <param name="olderThanDays">Optional filter for users registered more than specified days ago</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Users with unconfirmed emails</returns>
    Task<IEnumerable<AcademiaUser>> GetUnconfirmedEmailUsersAsync(int? olderThanDays = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets locked out users asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Currently locked out users</returns>
    Task<IEnumerable<AcademiaUser>> GetLockedOutUsersAsync(CancellationToken cancellationToken = default);

    #endregion

    #region User Validation

    /// <summary>
    /// Checks if a user exists by email asynchronously.
    /// </summary>
    /// <param name="email">The email address</param>
    /// <param name="excludeUserId">Optional user ID to exclude from check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if user exists, otherwise false</returns>
    Task<bool> ExistsByEmailAsync(string email, int? excludeUserId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user exists by username asynchronously.
    /// </summary>
    /// <param name="userName">The username</param>
    /// <param name="excludeUserId">Optional user ID to exclude from check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if user exists, otherwise false</returns>
    Task<bool> ExistsByUserNameAsync(string userName, int? excludeUserId = null, CancellationToken cancellationToken = default);

    #endregion

    #region User Statistics

    /// <summary>
    /// Gets user statistics asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User statistics</returns>
    Task<UserStatistics> GetUserStatisticsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets active user count asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of active users</returns>
    Task<int> GetActiveUserCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets new user registrations for a date range asynchronously.
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>New user registrations</returns>
    Task<IEnumerable<AcademiaUser>> GetNewRegistrationsAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    #endregion
}

/// <summary>
/// User statistics data transfer object.
/// </summary>
public class UserStatistics
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int InactiveUsers { get; set; }
    public int ConfirmedEmailUsers { get; set; }
    public int UnconfirmedEmailUsers { get; set; }
    public int LockedOutUsers { get; set; }
    public int UsersRegisteredToday { get; set; }
    public int UsersRegisteredThisWeek { get; set; }
    public int UsersRegisteredThisMonth { get; set; }
    public Dictionary<string, int> UsersByRole { get; set; } = new();
    public Dictionary<string, int> UsersByAcademicType { get; set; } = new();
}