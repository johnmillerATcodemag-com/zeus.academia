using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Identity;
using Zeus.Academia.Infrastructure.Repositories.Interfaces;

namespace Zeus.Academia.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for refresh token management operations.
/// Provides specialized token queries and management operations extending the base repository.
/// </summary>
public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
{
    /// <summary>
    /// Initializes a new instance of the RefreshTokenRepository class.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="logger">The logger instance</param>
    public RefreshTokenRepository(AcademiaDbContext context, ILogger<RefreshTokenRepository> logger)
        : base(context, logger)
    {
    }

    #region Token Queries

    /// <inheritdoc/>
    public async Task<RefreshToken?> GetByTokenAsync(string tokenValue, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting refresh token by value");
            return await _dbSet
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == tokenValue, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting refresh token by value");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<RefreshToken>> GetActiveTokensByUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting active refresh tokens for user: {UserId}", userId);
            return await _dbSet
                .Where(rt => rt.UserId == userId && rt.IsActive)
                .OrderByDescending(rt => rt.CreatedDate)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active refresh tokens for user: {UserId}", userId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<RefreshToken>> GetTokensByUserAsync(int userId, bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting refresh tokens for user: {UserId}, IncludeInactive: {IncludeInactive}", userId, includeInactive);

            var query = _dbSet.Where(rt => rt.UserId == userId);

            if (!includeInactive)
            {
                query = query.Where(rt => rt.IsActive);
            }

            return await query
                .OrderByDescending(rt => rt.CreatedDate)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting refresh tokens for user: {UserId}", userId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<RefreshToken>> GetTokensByDeviceAsync(string deviceId, bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting refresh tokens for device: {DeviceId}, IncludeInactive: {IncludeInactive}", deviceId, includeInactive);

            var query = _dbSet.Where(rt => rt.DeviceId == deviceId);

            if (!includeInactive)
            {
                query = query.Where(rt => rt.IsActive);
            }

            return await query
                .Include(rt => rt.User)
                .OrderByDescending(rt => rt.CreatedDate)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting refresh tokens for device: {DeviceId}", deviceId);
            throw;
        }
    }

    #endregion

    #region Token Management

    /// <inheritdoc/>
    public async Task<int> RevokeAllUserTokensAsync(int userId, string? reason = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Revoking all tokens for user: {UserId}, Reason: {Reason}", userId, reason);

            var activeTokens = await _dbSet
                .Where(rt => rt.UserId == userId && rt.IsActive)
                .ToListAsync(cancellationToken);

            foreach (var token in activeTokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
                token.RevocationReason = reason;
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Revoked {Count} tokens for user: {UserId}", activeTokens.Count, userId);
            return activeTokens.Count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking all tokens for user: {UserId}", userId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<int> RevokeDeviceTokensAsync(string deviceId, string? reason = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Revoking tokens for device: {DeviceId}, Reason: {Reason}", deviceId, reason);

            var activeTokens = await _dbSet
                .Where(rt => rt.DeviceId == deviceId && rt.IsActive)
                .ToListAsync(cancellationToken);

            foreach (var token in activeTokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
                token.RevocationReason = reason;
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Revoked {Count} tokens for device: {DeviceId}", activeTokens.Count, deviceId);
            return activeTokens.Count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking tokens for device: {DeviceId}", deviceId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> RevokeTokenAsync(int tokenId, string? reason = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Revoking token: {TokenId}, Reason: {Reason}", tokenId, reason);

            var token = await _dbSet.FindAsync(new object[] { tokenId }, cancellationToken);
            if (token == null || !token.IsActive)
            {
                return false;
            }

            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
            token.RevocationReason = reason;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Revoked token: {TokenId}", tokenId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking token: {TokenId}", tokenId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> RevokeTokenByValueAsync(string tokenValue, string? reason = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Revoking token by value, Reason: {Reason}", reason);

            var token = await _dbSet
                .FirstOrDefaultAsync(rt => rt.Token == tokenValue && rt.IsActive, cancellationToken);

            if (token == null)
            {
                return false;
            }

            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
            token.RevocationReason = reason;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Revoked token by value for user: {UserId}", token.UserId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking token by value");
            throw;
        }
    }

    #endregion

    #region Token Validation

    /// <inheritdoc/>
    public async Task<bool> IsTokenValidAsync(string tokenValue, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Validating token");

            var token = await _dbSet
                .FirstOrDefaultAsync(rt => rt.Token == tokenValue, cancellationToken);

            if (token == null)
            {
                return false;
            }

            return token.IsActive && token.ExpiresAt > DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating token");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> HasExceededTokenLimitAsync(int userId, int maxTokens, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Checking token limit for user: {UserId}, MaxTokens: {MaxTokens}", userId, maxTokens);

            var activeTokenCount = await _dbSet
                .CountAsync(rt => rt.UserId == userId && rt.IsActive, cancellationToken);

            return activeTokenCount >= maxTokens;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking token limit for user: {UserId}", userId);
            throw;
        }
    }

    #endregion

    #region Token Cleanup

    /// <inheritdoc/>
    public async Task<int> RemoveExpiredTokensAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Removing expired tokens");

            var now = DateTime.UtcNow;
            var expiredTokens = await _dbSet
                .Where(rt => rt.ExpiresAt <= now)
                .ToListAsync(cancellationToken);

            if (expiredTokens.Any())
            {
                _dbSet.RemoveRange(expiredTokens);
                await _context.SaveChangesAsync(cancellationToken);
            }

            _logger.LogInformation("Removed {Count} expired tokens", expiredTokens.Count);
            return expiredTokens.Count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing expired tokens");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<int> RemoveOldTokensAsync(int olderThanDays, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Removing tokens older than {Days} days", olderThanDays);

            var cutoffDate = DateTime.UtcNow.AddDays(-olderThanDays);
            var oldTokens = await _dbSet
                .Where(rt => rt.CreatedDate <= cutoffDate)
                .ToListAsync(cancellationToken);

            if (oldTokens.Any())
            {
                _dbSet.RemoveRange(oldTokens);
                await _context.SaveChangesAsync(cancellationToken);
            }

            _logger.LogInformation("Removed {Count} old tokens", oldTokens.Count);
            return oldTokens.Count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing old tokens");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<RefreshToken>> GetExpiredTokensAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting expired tokens");

            var now = DateTime.UtcNow;
            return await _dbSet
                .Where(rt => rt.ExpiresAt <= now)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting expired tokens");
            throw;
        }
    }

    #endregion

    #region Token Statistics

    /// <inheritdoc/>
    public async Task<RefreshTokenStatistics> GetTokenStatisticsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting refresh token statistics");

            var now = DateTime.UtcNow;
            var today = now.Date;
            var weekStart = today.AddDays(-(int)today.DayOfWeek);
            var monthStart = new DateTime(today.Year, today.Month, 1);

            var stats = new RefreshTokenStatistics
            {
                TotalTokens = await _dbSet.CountAsync(cancellationToken),
                ActiveTokens = await _dbSet.CountAsync(rt => rt.IsActive && rt.ExpiresAt > now, cancellationToken),
                ExpiredTokens = await _dbSet.CountAsync(rt => rt.ExpiresAt <= now, cancellationToken),
                RevokedTokens = await _dbSet.CountAsync(rt => !rt.IsActive && rt.RevokedAt.HasValue, cancellationToken),
                TokensCreatedToday = await _dbSet.CountAsync(rt => rt.CreatedDate >= today, cancellationToken),
                TokensCreatedThisWeek = await _dbSet.CountAsync(rt => rt.CreatedDate >= weekStart, cancellationToken),
                TokensCreatedThisMonth = await _dbSet.CountAsync(rt => rt.CreatedDate >= monthStart, cancellationToken),
                UniqueUsersWithTokens = await _dbSet.Select(rt => rt.UserId).Distinct().CountAsync(cancellationToken),
                UniqueDevicesWithTokens = await _dbSet.Where(rt => rt.DeviceId != null).Select(rt => rt.DeviceId).Distinct().CountAsync(cancellationToken)
            };

            // Get tokens by device
            var deviceStats = await _dbSet
                .Where(rt => rt.DeviceId != null && rt.IsActive)
                .GroupBy(rt => rt.DeviceId)
                .Select(g => new { DeviceId = g.Key!, Count = g.Count() })
                .ToListAsync(cancellationToken);

            foreach (var deviceStat in deviceStats)
            {
                stats.TokensByDevice[deviceStat.DeviceId] = deviceStat.Count;
            }

            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting refresh token statistics");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<int> GetActiveTokenCountAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting active token count for user: {UserId}", userId);

            var now = DateTime.UtcNow;
            return await _dbSet
                .CountAsync(rt => rt.UserId == userId && rt.IsActive && rt.ExpiresAt > now, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active token count for user: {UserId}", userId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<TokenUsageStatistics> GetTokenUsageStatisticsAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting token usage statistics from {StartDate} to {EndDate}", startDate, endDate);

            var tokens = await _dbSet
                .Where(rt => rt.CreatedDate >= startDate && rt.CreatedDate <= endDate)
                .ToListAsync(cancellationToken);

            var stats = new TokenUsageStatistics
            {
                TokensCreated = tokens.Count,
                TokensUsed = tokens.Count(rt => rt.LastUsedDate.HasValue),
                TokensRevoked = tokens.Count(rt => !rt.IsActive && rt.RevokedAt.HasValue),
                TokensExpired = tokens.Count(rt => rt.ExpiresAt <= DateTime.UtcNow)
            };

            // Group by date for daily statistics
            var dailyCreation = tokens
                .GroupBy(rt => rt.CreatedDate.Date)
                .ToDictionary(g => g.Key, g => g.Count());

            var dailyUsage = tokens
                .Where(rt => rt.LastUsedDate.HasValue)
                .GroupBy(rt => rt.LastUsedDate!.Value.Date)
                .ToDictionary(g => g.Key, g => g.Count());

            stats.DailyTokenCreation = dailyCreation;
            stats.DailyTokenUsage = dailyUsage;

            // Group by device
            var deviceStats = tokens
                .Where(rt => rt.DeviceId != null)
                .GroupBy(rt => rt.DeviceId!)
                .ToDictionary(g => g.Key, g => g.Count());

            stats.TokensByDevice = deviceStats;

            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting token usage statistics from {StartDate} to {EndDate}", startDate, endDate);
            throw;
        }
    }

    #endregion
}