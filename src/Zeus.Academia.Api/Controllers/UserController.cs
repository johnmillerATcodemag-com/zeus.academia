using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Infrastructure.Services;
using Zeus.Academia.Infrastructure.Data;

namespace Zeus.Academia.Api.Controllers
{
    /// <summary>
    /// User management controller for profile operations and administration
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        private readonly AcademiaDbContext _context;

        public UserController(IUserService userService, ILogger<UserController> logger, AcademiaDbContext context)
        {
            _userService = userService;
            _logger = logger;
            _context = context;
        }

        #region User Profile Management

        /// <summary>
        /// Get current user's profile
        /// </summary>
        /// <returns>User profile information</returns>
        [HttpGet("profile")]
        public async Task<ActionResult<UserProfile>> GetProfile()
        {
            try
            {
                var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst("UserId")?.Value;
                if (!int.TryParse(userIdClaim, out var userId))
                {
                    return BadRequest("Invalid user ID in token");
                }

                var profile = await _userService.GetUserProfileAsync(userId);
                if (profile == null)
                {
                    return NotFound("User profile not found");
                }

                return Ok(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user profile");
                return StatusCode(500, "An error occurred while retrieving profile");
            }
        }

        /// <summary>
        /// Update current user's profile
        /// </summary>
        /// <param name="request">Profile update request</param>
        /// <returns>Update result</returns>
        [HttpPut("profile")]
        public async Task<ActionResult<UserUpdateResult>> UpdateProfile(UserProfileUpdateRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst("UserId")?.Value;
                if (!int.TryParse(userIdClaim, out var userId))
                {
                    return BadRequest("Invalid user ID in token");
                }

                var result = await _userService.UpdateUserProfileAsync(userId, request);

                if (!result.Success)
                {
                    return BadRequest(new { Message = "Profile update failed", Errors = result.Errors });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile");
                return StatusCode(500, "An error occurred while updating profile");
            }
        }

        /// <summary>
        /// Change current user's password
        /// </summary>
        /// <param name="request">Password change request</param>
        /// <returns>Password change result</returns>
        [HttpPost("change-password")]
        public async Task<ActionResult<PasswordChangeResult>> ChangePassword(PasswordChangeRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst("UserId")?.Value;
                if (!int.TryParse(userIdClaim, out var userId))
                {
                    return BadRequest("Invalid user ID in token");
                }

                var result = await _userService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);

                if (!result.Success)
                {
                    return BadRequest(new { Message = "Password change failed", Errors = result.Errors });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                return StatusCode(500, "An error occurred while changing password");
            }
        }

        /// <summary>
        /// Confirm user's email address
        /// </summary>
        /// <param name="request">Email confirmation request</param>
        /// <returns>Confirmation result</returns>
        [HttpPost("confirm-email")]
        [AllowAnonymous]
        public async Task<ActionResult<EmailConfirmationResult>> ConfirmEmail(EmailConfirmationRequest request)
        {
            try
            {
                var result = await _userService.ConfirmEmailAsync(request.UserId, request.Token);

                if (!result.Success)
                {
                    return BadRequest(new { Message = "Email confirmation failed", Error = result.Error });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming email");
                return StatusCode(500, "An error occurred while confirming email");
            }
        }

        /// <summary>
        /// Resend email confirmation
        /// </summary>
        /// <param name="request">Resend confirmation request</param>
        /// <returns>Result</returns>
        [HttpPost("resend-confirmation")]
        [AllowAnonymous]
        public async Task<ActionResult<EmailOperationResult>> ResendConfirmation(ResendConfirmationRequest request)
        {
            try
            {
                // Find user by email first
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (user == null)
                {
                    return BadRequest(new { Message = "User not found" });
                }

                var result = await _userService.SendEmailConfirmationAsync(user.Id);

                if (!result.Success)
                {
                    return BadRequest(new { Message = "Failed to resend confirmation", Error = result.Error });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resending email confirmation");
                return StatusCode(500, "An error occurred while resending confirmation");
            }
        }

        #endregion

        #region User Administration

        /// <summary>
        /// Search users (Admin only)
        /// </summary>
        /// <param name="request">Search criteria</param>
        /// <returns>Paged user results</returns>
        [HttpPost("search")]
        [Authorize(Roles = "Administrator,UserManager")]
        public async Task<ActionResult<PagedResult<UserSummary>>> SearchUsers(UserSearchRequest request)
        {
            try
            {
                var result = await _userService.SearchUsersAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching users");
                return StatusCode(500, "An error occurred while searching users");
            }
        }

        /// <summary>
        /// Get user details (Admin only)
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>User administrative details</returns>
        [HttpGet("{userId:int}/details")]
        [Authorize(Roles = "Administrator,UserManager")]
        public async Task<ActionResult<UserAdminDetails>> GetUserDetails(int userId)
        {
            try
            {
                var result = await _userService.GetUserDetailsAsync(userId);
                if (result == null)
                {
                    return NotFound("User not found");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user details for user {UserId}", userId);
                return StatusCode(500, "An error occurred while retrieving user details");
            }
        }

        /// <summary>
        /// Set user active status (Admin only)
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="request">Status change request</param>
        /// <returns>Status change result</returns>
        [HttpPut("{userId:int}/active-status")]
        [Authorize(Roles = "Administrator,UserManager")]
        public async Task<ActionResult<UserStatusChangeResult>> SetActiveStatus(int userId, SetActiveStatusRequest request)
        {
            try
            {
                var result = await _userService.SetUserActiveStatusAsync(userId, request.IsActive, request.Reason);

                if (!result.Success)
                {
                    return BadRequest(new { Message = "Status change failed", Error = result.Error });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing active status for user {UserId}", userId);
                return StatusCode(500, "An error occurred while changing user status");
            }
        }

        /// <summary>
        /// Set user lock status (Admin only)
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="request">Lock status request</param>
        /// <returns>Status change result</returns>
        [HttpPut("{userId:int}/lock-status")]
        [Authorize(Roles = "Administrator,UserManager")]
        public async Task<ActionResult<UserStatusChangeResult>> SetLockStatus(int userId, SetLockStatusRequest request)
        {
            try
            {
                var result = await _userService.SetUserLockStatusAsync(userId, request.LockUntil, request.Reason);

                if (!result.Success)
                {
                    return BadRequest(new { Message = "Lock status change failed", Error = result.Error });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing lock status for user {UserId}", userId);
                return StatusCode(500, "An error occurred while changing user lock status");
            }
        }

        /// <summary>
        /// Get user roles (Admin only)
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>User roles</returns>
        [HttpGet("{userId:int}/roles")]
        [Authorize(Roles = "Administrator,UserManager")]
        public async Task<ActionResult<List<UserRoleInfo>>> GetUserRoles(int userId)
        {
            try
            {
                var roles = await _userService.GetUserRolesAsync(userId);
                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting roles for user {UserId}", userId);
                return StatusCode(500, "An error occurred while retrieving user roles");
            }
        }

        /// <summary>
        /// Assign role to user (Admin only)
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="request">Role assignment request</param>
        /// <returns>Assignment result</returns>
        [HttpPost("{userId:int}/roles")]
        [Authorize(Roles = "Administrator,UserManager")]
        public async Task<ActionResult<RoleAssignmentResult>> AssignRole(int userId, RoleAssignmentRequest request)
        {
            try
            {
                var currentUserIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst("UserId")?.Value;
                var assignedBy = int.TryParse(currentUserIdClaim, out var currentUserId) ? currentUserId : (int?)null;

                var result = await _userService.AssignRoleToUserAsync(
                    userId,
                    request.RoleId,
                    request.DepartmentContext,
                    request.ExpirationDate,
                    assignedBy,
                    request.Reason);

                if (!result.Success)
                {
                    return BadRequest(new { Message = "Role assignment failed", Errors = result.Errors });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning role to user {UserId}", userId);
                return StatusCode(500, "An error occurred while assigning role");
            }
        }

        /// <summary>
        /// Remove role from user (Admin only)
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="roleId">Role ID</param>
        /// <param name="request">Role removal request</param>
        /// <returns>Removal result</returns>
        [HttpDelete("{userId:int}/roles/{roleId:int}")]
        [Authorize(Roles = "Administrator,UserManager")]
        public async Task<ActionResult<RoleAssignmentResult>> RemoveRole(int userId, int roleId, RoleRemovalRequest request)
        {
            try
            {
                var result = await _userService.RemoveRoleFromUserAsync(userId, roleId, request.DepartmentContext, request.Reason);

                if (!result.Success)
                {
                    return BadRequest(new { Message = "Role removal failed", Errors = result.Errors });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing role from user {UserId}", userId);
                return StatusCode(500, "An error occurred while removing role");
            }
        }

        /// <summary>
        /// Import users from file (Admin only)
        /// </summary>
        /// <param name="request">Import request</param>
        /// <returns>Import result</returns>
        [HttpPost("import")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<UserImportResult>> ImportUsers(List<UserImportRequest> request)
        {
            try
            {
                var result = await _userService.ImportUsersAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing users");
                return StatusCode(500, "An error occurred while importing users");
            }
        }

        /// <summary>
        /// Export users (Admin only)
        /// </summary>
        /// <param name="request">Export criteria</param>
        /// <param name="format">Export format</param>
        /// <returns>Export file</returns>
        [HttpPost("export")]
        [Authorize(Roles = "Administrator,UserManager")]
        public async Task<IActionResult> ExportUsers(UserSearchRequest request, [FromQuery] ExportFormat format = ExportFormat.Csv)
        {
            try
            {
                var result = await _userService.ExportUsersAsync(request, format);

                if (!result.Success)
                {
                    return BadRequest(new { Message = "Export failed", Error = result.Error });
                }

                return File(result.Data!, result.ContentType!, result.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting users");
                return StatusCode(500, "An error occurred while exporting users");
            }
        }

        #endregion
    }

    #region Request DTOs for Controller

    public class PasswordChangeRequest
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class EmailConfirmationRequest
    {
        public int UserId { get; set; }
        public string Token { get; set; } = string.Empty;
    }

    public class ResendConfirmationRequest
    {
        public string Email { get; set; } = string.Empty;
    }

    public class SetActiveStatusRequest
    {
        public bool IsActive { get; set; }
        public string? Reason { get; set; }
    }

    public class SetLockStatusRequest
    {
        public DateTimeOffset? LockUntil { get; set; }
        public string? Reason { get; set; }
    }

    public class RoleAssignmentRequest
    {
        public int RoleId { get; set; }
        public string? DepartmentContext { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string? Reason { get; set; }
    }

    public class RoleRemovalRequest
    {
        public string? DepartmentContext { get; set; }
        public string? Reason { get; set; }
    }

    #endregion
}