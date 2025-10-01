using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Zeus.Academia.Infrastructure.Identity;

namespace Zeus.Academia.Infrastructure.Services;

/// <summary>
/// JWT token service implementation for generating and validating JWT tokens
/// </summary>
public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<JwtTokenService> _logger;
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly TimeSpan _accessTokenExpiration;
    private readonly TimeSpan _refreshTokenExpiration;

    public JwtTokenService(IConfiguration configuration, ILogger<JwtTokenService> logger)
    {
        _configuration = configuration;
        _logger = logger;

        // Get JWT settings from configuration
        _secretKey = _configuration["JwtSettings:SecretKey"] ?? GenerateSecretKey();
        _issuer = _configuration["JwtSettings:Issuer"] ?? "Zeus.Academia.Api";
        _audience = _configuration["JwtSettings:Audience"] ?? "Zeus.Academia.Client";

        // Configure token expiration times
        var accessTokenMinutes = _configuration.GetValue("JwtSettings:AccessTokenExpirationMinutes", 60);
        var refreshTokenDays = _configuration.GetValue("JwtSettings:RefreshTokenExpirationDays", 7);

        _accessTokenExpiration = TimeSpan.FromMinutes(accessTokenMinutes);
        _refreshTokenExpiration = TimeSpan.FromDays(refreshTokenDays);

        _logger.LogInformation("JWT Token Service initialized with {AccessTokenExpiration} access token expiration and {RefreshTokenExpiration} refresh token expiration",
            _accessTokenExpiration, _refreshTokenExpiration);
    }

    public TimeSpan AccessTokenExpiration => _accessTokenExpiration;
    public TimeSpan RefreshTokenExpiration => _refreshTokenExpiration;

    public async Task<string> GenerateAccessTokenAsync(AcademiaUser user, IList<string> roles, AcademiaPermission permissions)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_secretKey);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName ?? user.Email ?? user.Id.ToString()),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new("jti", Guid.NewGuid().ToString()),
            new("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        // Add role claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Add permission claims (as individual flags)
        var permissionFlags = Enum.GetValues<AcademiaPermission>()
            .Where(p => p != AcademiaPermission.None && permissions.HasFlag(p))
            .ToList();

        foreach (var permission in permissionFlags)
        {
            claims.Add(new Claim("permission", permission.ToString()));
        }

        // Add academic context if user is linked to an academic entity
        if (user.AcademicId.HasValue)
        {
            claims.Add(new Claim("academic_id", user.AcademicId.Value.ToString()));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(_accessTokenExpiration),
            Issuer = _issuer,
            Audience = _audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        _logger.LogDebug("Generated JWT token for user {UserId} with {RoleCount} roles and {PermissionCount} permissions",
            user.Id, roles.Count, permissionFlags.Count);

        return tokenString;
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey)),
            ValidateLifetime = false, // Don't validate expiration for expired token scenarios
            ValidIssuer = _issuer,
            ValidAudience = _audience
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

            if (validatedToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.LogWarning("Invalid JWT token algorithm or format");
                return null;
            }

            return principal;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get principal from expired token");
            return null;
        }
    }

    public bool ValidateToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey)),
            ValidateLifetime = true,
            ValidIssuer = _issuer,
            ValidAudience = _audience,
            ClockSkew = TimeSpan.FromMinutes(1) // Allow 1 minute clock skew
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Token validation failed");
            return false;
        }
    }

    /// <summary>
    /// Generate a secure secret key for development environments
    /// </summary>
    private static string GenerateSecretKey()
    {
        var key = new byte[32]; // 256 bits
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(key);
        return Convert.ToBase64String(key);
    }
}