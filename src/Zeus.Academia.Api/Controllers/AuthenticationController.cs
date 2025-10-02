using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Identity;
using Zeus.Academia.Infrastructure.Services;

namespace Zeus.Academia.Api.Controllers;

/// <summary>
/// Authentication controller for handling login, logout, and token management
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly AcademiaDbContext _context;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IRoleHierarchyService _roleHierarchyService;
    private readonly IPasswordService _passwordService;
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(
        AcademiaDbContext context,
        IJwtTokenService jwtTokenService,
        IRefreshTokenService refreshTokenService,
        IRoleHierarchyService roleHierarchyService,
        IPasswordService passwordService,
        ILogger<AuthenticationController> logger)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
        _refreshTokenService = refreshTokenService;
        _roleHierarchyService = roleHierarchyService;
        _passwordService = passwordService;
        _logger = logger;
    }

    /// <summary>
    /// Authenticate user and return access and refresh tokens
    /// </summary>
    /// <param name="request">Login request containing username/email and password</param>
    /// <returns>Authentication response with tokens</returns>
    [HttpPost("login")]
    public async Task<ActionResult<AuthenticationResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Find user by username or email
            var user = await _context.Users
                .Include(u => u.Academic)
                .FirstOrDefaultAsync(u =>
                    u.UserName == request.UsernameOrEmail ||
                    u.Email == request.UsernameOrEmail);

            if (user == null)
            {
                _logger.LogWarning("Login attempt with invalid username/email: {UsernameOrEmail}", request.UsernameOrEmail);
                return Unauthorized(new { message = "Invalid username or password" });
            }

            // Check if user is active
            if (!user.IsActive)
            {
                _logger.LogWarning("Login attempt for inactive user: {UserId}", user.Id);
                return Unauthorized(new { message = "Account is inactive" });
            }

            // Check if user is locked out
            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow)
            {
                _logger.LogWarning("Login attempt for locked out user: {UserId}", user.Id);
                return Unauthorized(new { message = "Account is locked out" });
            }

            // Verify password using the password service
            if (!_passwordService.VerifyPassword(request.Password, user.PasswordHash ?? string.Empty))
            {
                // Increment failed access attempts
                user.AccessFailedCount++;

                // Lock account if too many failed attempts
                if (user.AccessFailedCount >= 5)
                {
                    user.LockoutEnd = DateTimeOffset.UtcNow.AddMinutes(15);
                    _logger.LogWarning("User {UserId} locked out due to too many failed login attempts", user.Id);
                }

                await _context.SaveChangesAsync();
                return Unauthorized(new { message = "Invalid username or password" });
            }

            // Reset failed access count on successful login
            user.AccessFailedCount = 0;
            user.LastLoginDate = DateTime.UtcNow;
            user.LastLoginIpAddress = GetClientIpAddress();

            // Get user roles and permissions
            var userPermissions = _roleHierarchyService.GetUserPermissions(user);
            var userRoles = await _context.UserRoles
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == user.Id && ur.IsCurrentlyEffective())
                .Select(ur => ur.Role.Name)
                .Where(name => name != null)
                .Cast<string>()
                .ToListAsync();

            // Generate JWT token
            var jwtId = Guid.NewGuid().ToString();
            var accessToken = await _jwtTokenService.GenerateAccessTokenAsync(user, userRoles, userPermissions);

            // Create refresh token
            var refreshToken = await _refreshTokenService.CreateRefreshTokenAsync(
                user.Id,
                jwtId,
                GetClientIpAddress(),
                _jwtTokenService.RefreshTokenExpiration);

            await _context.SaveChangesAsync();

            _logger.LogInformation("User {UserId} successfully authenticated", user.Id);

            return Ok(new AuthenticationResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                ExpiresIn = (int)_jwtTokenService.AccessTokenExpiration.TotalSeconds,
                TokenType = "Bearer",
                User = new UserInfo
                {
                    Id = user.Id.ToString(),
                    UserName = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    DisplayName = user.DisplayName,
                    Roles = userRoles,
                    Permissions = userPermissions.ToString()
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for {UsernameOrEmail}", request.UsernameOrEmail);
            return StatusCode(500, new { message = "An error occurred during login" });
        }
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    /// <param name="request">Refresh token request</param>
    /// <returns>New access and refresh tokens</returns>
    [HttpPost("refresh")]
    public async Task<ActionResult<AuthenticationResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate refresh token
            var refreshToken = await _refreshTokenService.GetRefreshTokenAsync(request.RefreshToken);
            if (refreshToken == null)
            {
                _logger.LogWarning("Invalid refresh token used");
                return Unauthorized(new { message = "Invalid refresh token" });
            }

            var user = refreshToken.User;

            // Check if user is still active
            if (!user.IsActive)
            {
                await _refreshTokenService.RevokeRefreshTokenAsync(request.RefreshToken, "User inactive", GetClientIpAddress());
                return Unauthorized(new { message = "Account is inactive" });
            }

            // Get updated user roles and permissions
            var userPermissions = _roleHierarchyService.GetUserPermissions(user);
            var userRoles = await _context.UserRoles
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == user.Id && ur.IsCurrentlyEffective())
                .Select(ur => ur.Role.Name)
                .Where(name => name != null)
                .Cast<string>()
                .ToListAsync();

            // Generate new JWT token
            var jwtId = Guid.NewGuid().ToString();
            var accessToken = await _jwtTokenService.GenerateAccessTokenAsync(user, userRoles, userPermissions);

            // Create new refresh token
            var newRefreshToken = await _refreshTokenService.CreateRefreshTokenAsync(
                user.Id,
                jwtId,
                GetClientIpAddress(),
                _jwtTokenService.RefreshTokenExpiration);

            // Revoke old refresh token
            await _refreshTokenService.RevokeRefreshTokenAsync(request.RefreshToken, "Token refreshed", GetClientIpAddress());

            _logger.LogInformation("Token refreshed for user {UserId}", user.Id);

            return Ok(new AuthenticationResponse
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken.Token,
                ExpiresIn = (int)_jwtTokenService.AccessTokenExpiration.TotalSeconds,
                TokenType = "Bearer",
                User = new UserInfo
                {
                    Id = user.Id.ToString(),
                    UserName = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    DisplayName = user.DisplayName,
                    Roles = userRoles,
                    Permissions = userPermissions.ToString()
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return StatusCode(500, new { message = "An error occurred during token refresh" });
        }
    }

    /// <summary>
    /// Revoke refresh token (logout)
    /// </summary>
    /// <param name="request">Revoke token request</param>
    /// <returns>Success response</returns>
    [HttpPost("revoke")]
    [Authorize]
    public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequest request)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            bool revoked;
            if (!string.IsNullOrEmpty(request.RefreshToken))
            {
                // Revoke specific refresh token
                revoked = await _refreshTokenService.RevokeRefreshTokenAsync(request.RefreshToken, "User logout", GetClientIpAddress());
            }
            else
            {
                // Revoke all refresh tokens for user
                var revokedCount = await _refreshTokenService.RevokeAllUserTokensAsync(int.Parse(userId), "User logout all", GetClientIpAddress());
                revoked = revokedCount > 0;
            }

            if (revoked)
            {
                _logger.LogInformation("Token(s) revoked for user {UserId}", userId);
                return Ok(new { message = "Token(s) revoked successfully" });
            }
            else
            {
                return BadRequest(new { message = "No tokens to revoke or token not found" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token revocation");
            return StatusCode(500, new { message = "An error occurred during token revocation" });
        }
    }

    /// <summary>
    /// Validate current token and return user information
    /// </summary>
    /// <returns>Current user information</returns>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserInfo>> GetCurrentUser()
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user = await _context.Users
                .Include(u => u.Academic)
                .FirstOrDefaultAsync(u => u.Id.ToString() == userId);

            if (user == null || !user.IsActive)
            {
                return Unauthorized();
            }

            var userPermissions = _roleHierarchyService.GetUserPermissions(user);
            var userRoles = await _context.UserRoles
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == user.Id && ur.IsCurrentlyEffective())
                .Select(ur => ur.Role.Name)
                .Where(name => name != null)
                .Cast<string>()
                .ToListAsync();

            return Ok(new UserInfo
            {
                Id = user.Id.ToString(),
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DisplayName = user.DisplayName,
                Roles = userRoles,
                Permissions = userPermissions.ToString()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current user information");
            return StatusCode(500, new { message = "An error occurred while getting user information" });
        }
    }



    private string? GetClientIpAddress()
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        if (Request.Headers.ContainsKey("X-Forwarded-For"))
        {
            ipAddress = Request.Headers["X-Forwarded-For"].FirstOrDefault()?.Split(',').FirstOrDefault()?.Trim();
        }
        return ipAddress;
    }
}

/// <summary>
/// Login request model
/// </summary>
public class LoginRequest
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string UsernameOrEmail { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; } = false;
}

/// <summary>
/// Refresh token request model
/// </summary>
public class RefreshTokenRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}

/// <summary>
/// Revoke token request model
/// </summary>
public class RevokeTokenRequest
{
    public string? RefreshToken { get; set; }
}

/// <summary>
/// Authentication response model
/// </summary>
public class AuthenticationResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
    public string TokenType { get; set; } = "Bearer";
    public UserInfo User { get; set; } = new();
}

/// <summary>
/// User information model
/// </summary>
public class UserInfo
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? DisplayName { get; set; }
    public List<string> Roles { get; set; } = new();
    public string Permissions { get; set; } = string.Empty;
}