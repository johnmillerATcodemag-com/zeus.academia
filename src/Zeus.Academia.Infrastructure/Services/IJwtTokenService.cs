using System.Security.Claims;
using Zeus.Academia.Infrastructure.Identity;

namespace Zeus.Academia.Infrastructure.Services;

/// <summary>
/// Service for JWT token generation and management
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Generate a JWT access token for the specified user
    /// </summary>
    /// <param name="user">The user to generate token for</param>
    /// <param name="roles">The user's roles</param>
    /// <param name="permissions">The user's permissions</param>
    /// <returns>JWT token string</returns>
    Task<string> GenerateAccessTokenAsync(AcademiaUser user, IList<string> roles, AcademiaPermission permissions);

    /// <summary>
    /// Generate a refresh token
    /// </summary>
    /// <returns>Refresh token string</returns>
    string GenerateRefreshToken();

    /// <summary>
    /// Get the principal from an expired token (for refresh scenarios)
    /// </summary>
    /// <param name="token">The expired JWT token</param>
    /// <returns>Claims principal if token is valid (ignoring expiration)</returns>
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);

    /// <summary>
    /// Validate a JWT token
    /// </summary>
    /// <param name="token">Token to validate</param>
    /// <returns>True if token is valid</returns>
    bool ValidateToken(string token);

    /// <summary>
    /// Get the expiration time for access tokens
    /// </summary>
    TimeSpan AccessTokenExpiration { get; }

    /// <summary>
    /// Get the expiration time for refresh tokens
    /// </summary>
    TimeSpan RefreshTokenExpiration { get; }
}