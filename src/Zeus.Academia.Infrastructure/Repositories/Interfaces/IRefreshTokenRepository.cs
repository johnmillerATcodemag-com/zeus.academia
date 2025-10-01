using Zeus.Academia.Infrastructure.Identity;

namespace Zeus.Academia.Infrastructure.Repositories.Interfaces;

/// <summary>
/// Repository interface for refresh token management operations.
/// Extends the generic repository with token-specific functionality.
/// </summary>
public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    #region Token Queries

    /// <summary>
    /// Gets a refresh token by token value asynchronously.
    /// </summary>
    /// <param name="tokenValue">The token value</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The refresh token if found, otherwise null</returns>
    Task<RefreshToken?> GetByTokenAsync(string tokenValue, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets active refresh tokens for a user asynchronously.
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Active refresh tokens for the user</returns>
    Task<IEnumerable<RefreshToken>> GetActiveTokensByUserAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all refresh tokens for a user asynchronously.
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="includeInactive">Whether to include inactive tokens</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Refresh tokens for the user</returns>
    Task<IEnumerable<RefreshToken>> GetTokensByUserAsync(int userId, bool includeInactive = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets refresh tokens by device asynchronously.
    /// </summary>
    /// <param name="deviceId">The device ID</param>
    /// <param name="includeInactive">Whether to include inactive tokens</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Refresh tokens for the device</returns>
    Task<IEnumerable<RefreshToken>> GetTokensByDeviceAsync(string deviceId, bool includeInactive = false, CancellationToken cancellationToken = default);

    #endregion

    #region Token Management

    /// <summary>
    /// Revokes all active tokens for a user asynchronously.
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="reason">Reason for revocation</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of tokens revoked</returns>
    Task<int> RevokeAllUserTokensAsync(int userId, string? reason = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes tokens for a specific device asynchronously.
    /// </summary>
    /// <param name="deviceId">The device ID</param>
    /// <param name="reason">Reason for revocation</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of tokens revoked</returns>
    Task<int> RevokeDeviceTokensAsync(string deviceId, string? reason = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes a specific token asynchronously.
    /// </summary>
    /// <param name="tokenId">The token ID</param>
    /// <param name="reason">Reason for revocation</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if token was revoked, false if not found</returns>
    Task<bool> RevokeTokenAsync(int tokenId, string? reason = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes a token by token value asynchronously.
    /// </summary>
    /// <param name="tokenValue">The token value</param>
    /// <param name="reason">Reason for revocation</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if token was revoked, false if not found</returns>
    Task<bool> RevokeTokenByValueAsync(string tokenValue, string? reason = null, CancellationToken cancellationToken = default);

    #endregion

    #region Token Validation

    /// <summary>
    /// Validates if a token is active and not expired asynchronously.
    /// </summary>
    /// <param name="tokenValue">The token value</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if token is valid, false otherwise</returns>
    Task<bool> IsTokenValidAsync(string tokenValue, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user has exceeded the maximum number of active tokens asynchronously.
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="maxTokens">Maximum allowed tokens</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if limit is exceeded, false otherwise</returns>
    Task<bool> HasExceededTokenLimitAsync(int userId, int maxTokens, CancellationToken cancellationToken = default);

    #endregion

    #region Token Cleanup

    /// <summary>
    /// Removes expired tokens asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of tokens removed</returns>
    Task<int> RemoveExpiredTokensAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes tokens older than specified days asynchronously.
    /// </summary>
    /// <param name="olderThanDays">Days threshold</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of tokens removed</returns>
    Task<int> RemoveOldTokensAsync(int olderThanDays, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets expired tokens for cleanup asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Expired tokens</returns>
    Task<IEnumerable<RefreshToken>> GetExpiredTokensAsync(CancellationToken cancellationToken = default);

    #endregion

    #region Token Statistics

    /// <summary>
    /// Gets refresh token statistics asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Token statistics</returns>
    Task<RefreshTokenStatistics> GetTokenStatisticsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets active token count for a user asynchronously.
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of active tokens</returns>
    Task<int> GetActiveTokenCountAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets token usage statistics for a date range asynchronously.
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Token usage statistics</returns>
    Task<TokenUsageStatistics> GetTokenUsageStatisticsAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    #endregion
}

/// <summary>
/// Refresh token statistics data transfer object.
/// </summary>
public class RefreshTokenStatistics
{
    public int TotalTokens { get; set; }
    public int ActiveTokens { get; set; }
    public int ExpiredTokens { get; set; }
    public int RevokedTokens { get; set; }
    public int TokensCreatedToday { get; set; }
    public int TokensCreatedThisWeek { get; set; }
    public int TokensCreatedThisMonth { get; set; }
    public int UniqueUsersWithTokens { get; set; }
    public int UniqueDevicesWithTokens { get; set; }
    public Dictionary<string, int> TokensByDevice { get; set; } = new();
}

/// <summary>
/// Token usage statistics data transfer object.
/// </summary>
public class TokenUsageStatistics
{
    public int TokensCreated { get; set; }
    public int TokensUsed { get; set; }
    public int TokensRevoked { get; set; }
    public int TokensExpired { get; set; }
    public Dictionary<DateTime, int> DailyTokenCreation { get; set; } = new();
    public Dictionary<DateTime, int> DailyTokenUsage { get; set; } = new();
    public Dictionary<string, int> TokensByDevice { get; set; } = new();
}