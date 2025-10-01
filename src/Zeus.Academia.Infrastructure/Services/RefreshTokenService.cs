using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Identity;

namespace Zeus.Academia.Infrastructure.Services;

/// <summary>
/// Service interface for managing refresh tokens
/// </summary>
public interface IRefreshTokenService
{
    /// <summary>
    /// Create a new refresh token for a user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="jwtId">JWT token identifier</param>
    /// <param name="ipAddress">IP address of the request</param>
    /// <param name="expirationTime">Token expiration time</param>
    /// <returns>The created refresh token</returns>
    Task<RefreshToken> CreateRefreshTokenAsync(int userId, string jwtId, string? ipAddress, TimeSpan expirationTime);

    /// <summary>
    /// Get a refresh token by its value
    /// </summary>
    /// <param name="token">Token value</param>
    /// <returns>Refresh token if found and active</returns>
    Task<RefreshToken?> GetRefreshTokenAsync(string token);

    /// <summary>
    /// Revoke a refresh token
    /// </summary>
    /// <param name="token">Token to revoke</param>
    /// <param name="reason">Reason for revocation</param>
    /// <param name="revokedByIp">IP address that revoked the token</param>
    /// <returns>True if token was revoked</returns>
    Task<bool> RevokeRefreshTokenAsync(string token, string reason = "Revoked", string? revokedByIp = null);

    /// <summary>
    /// Revoke all active refresh tokens for a user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="reason">Reason for revocation</param>
    /// <param name="revokedByIp">IP address that revoked the tokens</param>
    /// <returns>Number of tokens revoked</returns>
    Task<int> RevokeAllUserTokensAsync(int userId, string reason = "All tokens revoked", string? revokedByIp = null);

    /// <summary>
    /// Clean up expired refresh tokens
    /// </summary>
    /// <returns>Number of tokens cleaned up</returns>
    Task<int> CleanupExpiredTokensAsync();

    /// <summary>
    /// Get active refresh tokens for a user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <returns>List of active refresh tokens</returns>
    Task<List<RefreshToken>> GetActiveUserTokensAsync(int userId);
}

/// <summary>
/// Service implementation for managing refresh tokens
/// </summary>
public class RefreshTokenService : IRefreshTokenService
{
    private readonly AcademiaDbContext _context;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<RefreshTokenService> _logger;

    public RefreshTokenService(
        AcademiaDbContext context,
        IJwtTokenService jwtTokenService,
        ILogger<RefreshTokenService> logger)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    public async Task<RefreshToken> CreateRefreshTokenAsync(int userId, string jwtId, string? ipAddress, TimeSpan expirationTime)
    {
        var refreshToken = new RefreshToken
        {
            Token = _jwtTokenService.GenerateRefreshToken(),
            UserId = userId,
            JwtId = jwtId,
            CreatedByIp = ipAddress,
            ExpiresAt = DateTime.UtcNow.Add(expirationTime)
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        _logger.LogDebug("Created refresh token for user {UserId} with JWT ID {JwtId}", userId, jwtId);

        return refreshToken;
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
    {
        var refreshToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == token);

        if (refreshToken == null)
        {
            _logger.LogWarning("Refresh token not found: {Token}", token[..Math.Min(token.Length, 20)] + "...");
            return null;
        }

        if (!refreshToken.IsActive)
        {
            _logger.LogWarning("Refresh token is not active: {Token} (User: {UserId})",
                token[..Math.Min(token.Length, 20)] + "...", refreshToken.UserId);
            return null;
        }

        return refreshToken;
    }

    public async Task<bool> RevokeRefreshTokenAsync(string token, string reason = "Revoked", string? revokedByIp = null)
    {
        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == token);

        if (refreshToken == null || refreshToken.IsRevoked)
        {
            _logger.LogWarning("Attempted to revoke non-existent or already revoked token");
            return false;
        }

        refreshToken.Revoke(reason, revokedByIp);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Revoked refresh token for user {UserId}: {Reason}", refreshToken.UserId, reason);

        return true;
    }

    public async Task<int> RevokeAllUserTokensAsync(int userId, string reason = "All tokens revoked", string? revokedByIp = null)
    {
        var activeTokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow)
            .ToListAsync();

        var revokedCount = 0;
        foreach (var token in activeTokens)
        {
            token.Revoke(reason, revokedByIp);
            revokedCount++;
        }

        if (revokedCount > 0)
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Revoked {Count} refresh tokens for user {UserId}: {Reason}",
                revokedCount, userId, reason);
        }

        return revokedCount;
    }

    public async Task<int> CleanupExpiredTokensAsync()
    {
        var expiredTokens = await _context.RefreshTokens
            .Where(rt => rt.ExpiresAt <= DateTime.UtcNow)
            .ToListAsync();

        var deletedCount = expiredTokens.Count;
        if (deletedCount > 0)
        {
            _context.RefreshTokens.RemoveRange(expiredTokens);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Cleaned up {Count} expired refresh tokens", deletedCount);
        }

        return deletedCount;
    }

    public async Task<List<RefreshToken>> GetActiveUserTokensAsync(int userId)
    {
        return await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(rt => rt.CreatedAt)
            .ToListAsync();
    }
}