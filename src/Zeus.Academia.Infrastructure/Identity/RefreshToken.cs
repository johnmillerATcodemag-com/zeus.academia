using System.ComponentModel.DataAnnotations;
using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.Infrastructure.Identity;

/// <summary>
/// Entity representing a refresh token for JWT authentication
/// </summary>
public class RefreshToken : BaseEntity
{

    /// <summary>
    /// The refresh token value
    /// </summary>
    [Required]
    [StringLength(500)]
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// User ID this token belongs to
    /// </summary>
    [Required]
    public int UserId { get; set; }

    /// <summary>
    /// Navigation property to the user
    /// </summary>
    public AcademiaUser User { get; set; } = null!;

    /// <summary>
    /// When the token was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When the token expires
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Whether the token has been revoked
    /// </summary>
    public bool IsRevoked { get; set; }

    /// <summary>
    /// When the token was revoked (if applicable)
    /// </summary>
    public DateTime? RevokedAt { get; set; }

    /// <summary>
    /// Reason for revocation (if applicable)
    /// </summary>
    [StringLength(200)]
    public string? RevocationReason { get; set; }

    /// <summary>
    /// The JWT token ID (jti claim) this refresh token was issued for
    /// </summary>
    [StringLength(100)]
    public string? JwtId { get; set; }

    /// <summary>
    /// IP address where the token was created
    /// </summary>
    [StringLength(50)]
    public string? CreatedByIp { get; set; }

    /// <summary>
    /// IP address where the token was revoked (if applicable)
    /// </summary>
    [StringLength(50)]
    public string? RevokedByIp { get; set; }

    /// <summary>
    /// Device identifier for the token (optional)
    /// </summary>
    [StringLength(100)]
    public string? DeviceId { get; set; }

    /// <summary>
    /// When the token was last used (optional)
    /// </summary>
    public DateTime? LastUsedDate { get; set; }

    /// <summary>
    /// Check if the refresh token is currently active
    /// </summary>
    public bool IsActive => !IsRevoked && DateTime.UtcNow < ExpiresAt;

    /// <summary>
    /// Check if the refresh token has expired
    /// </summary>
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    /// <summary>
    /// Revoke the refresh token
    /// </summary>
    /// <param name="reason">Reason for revocation</param>
    /// <param name="revokedByIp">IP address that revoked the token</param>
    public void Revoke(string reason = "Revoked without reason", string? revokedByIp = null)
    {
        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
        RevocationReason = reason;
        RevokedByIp = revokedByIp;
    }
}