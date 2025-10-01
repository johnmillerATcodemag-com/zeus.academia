using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Identity;
using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.Infrastructure.Services;

/// <summary>
/// User service implementation for managing user operations including registration, profile management, and administration
/// </summary>
public class UserService : IUserService
{
    private readonly AcademiaDbContext _context;
    private readonly IPasswordService _passwordService;
    private readonly IRoleAssignmentService _roleAssignmentService;
    private readonly IRoleHierarchyService _roleHierarchyService;
    private readonly IEmailService _emailService;
    private readonly ILogger<UserService> _logger;

    public UserService(
        AcademiaDbContext context,
        IPasswordService passwordService,
        IRoleAssignmentService roleAssignmentService,
        IRoleHierarchyService roleHierarchyService,
        IEmailService emailService,
        ILogger<UserService> logger)
    {
        _context = context;
        _passwordService = passwordService;
        _roleAssignmentService = roleAssignmentService;
        _roleHierarchyService = roleHierarchyService;
        _emailService = emailService;
        _logger = logger;
    }

    #region User Registration and Creation

    public async Task<UserRegistrationResult> RegisterUserAsync(UserRegistrationRequest request)
    {
        var result = new UserRegistrationResult();

        try
        {
            // Validate request
            var validationErrors = ValidateRegistrationRequest(request);
            if (validationErrors.Any())
            {
                result.Errors = validationErrors;
                return result;
            }

            // Check if user already exists
            if (await UserExistsAsync(request.Email) || await UserExistsAsync(request.UserName))
            {
                result.Errors.Add("A user with this email or username already exists");
                return result;
            }

            // Create user entity
            var user = new AcademiaUser
            {
                UserName = request.UserName,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                DisplayName = $"{request.FirstName} {request.LastName}".Trim(),
                AcademicId = request.AcademicId,
                PasswordHash = _passwordService.HashPassword(request.Password),
                EmailConfirmed = false,
                IsActive = true,
                LockoutEnabled = true,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };

            // Add user to database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Assign automatic roles based on academic entity type
            if (request.AcademicId.HasValue)
            {
                await AssignAutomaticRolesAsync(user);
            }

            // Generate email confirmation token if requested
            if (request.SendConfirmationEmail)
            {
                var confirmationToken = GenerateEmailConfirmationToken();
                // Store token (in real implementation, store in database or cache)
                var emailResult = await _emailService.SendEmailConfirmationAsync(user.Email, user.DisplayName ?? user.UserName, confirmationToken);

                if (emailResult.Success)
                {
                    result.ConfirmationToken = confirmationToken;
                }
                else
                {
                    _logger.LogWarning("Failed to send confirmation email for user {UserId}: {Error}", user.Id, emailResult.Error);
                }
            }

            result.Success = true;
            result.User = user;

            _logger.LogInformation("User {UserId} ({Email}) registered successfully", user.Id, user.Email);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user with email {Email}", request.Email);
            result.Errors.Add("An error occurred during registration");
            return result;
        }
    }

    public async Task<UserCreationResult> CreateUserForAcademicAsync(int academicId, string email, string? temporaryPassword = null)
    {
        var result = new UserCreationResult();

        try
        {
            // Validate academic entity exists
            var academic = await _context.Academics.FindAsync(academicId);
            if (academic == null)
            {
                result.Errors.Add("Academic entity not found");
                return result;
            }

            // Check if user already exists
            if (await UserExistsAsync(email))
            {
                result.Errors.Add("A user with this email already exists");
                return result;
            }

            // Generate temporary password if not provided
            var tempPassword = temporaryPassword ?? GenerateTemporaryPassword();

            // Parse name from Academic.Name field
            var nameParts = academic.Name.Split(' ', 2);
            var firstName = nameParts.Length > 0 ? nameParts[0] : string.Empty;
            var lastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;

            // Create user entity
            var user = new AcademiaUser
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                DisplayName = academic.Name,
                AcademicId = academicId,
                PasswordHash = _passwordService.HashPassword(tempPassword),
                EmailConfirmed = false,
                IsActive = true,
                LockoutEnabled = true,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };

            // Add user to database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Assign automatic roles based on academic entity type
            await _roleAssignmentService.AssignAutomaticRolesAsync(user);

            result.Success = true;
            result.User = user;
            result.TemporaryPassword = tempPassword;

            _logger.LogInformation("User {UserId} created for academic {AcademicId}", user.Id, academicId);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user for academic {AcademicId}", academicId);
            result.Errors.Add("An error occurred during user creation");
            return result;
        }
    }

    #endregion

    #region User Authentication and Validation

    public async Task<UserAuthenticationResult> ValidateUserAsync(string usernameOrEmail, string password)
    {
        var result = new UserAuthenticationResult();

        try
        {
            var user = await _context.Users
                .Include(u => u.Academic)
                .FirstOrDefaultAsync(u => u.UserName == usernameOrEmail || u.Email == usernameOrEmail);

            if (user == null)
            {
                result.FailureReason = "Invalid username or password";
                return result;
            }

            // Check if user is active
            if (!user.IsActive)
            {
                result.FailureReason = "Account is inactive";
                return result;
            }

            // Check if user is locked out
            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow)
            {
                result.IsLockedOut = true;
                result.FailureReason = "Account is locked out";
                return result;
            }

            // Verify password
            if (!_passwordService.VerifyPassword(password, user.PasswordHash ?? string.Empty))
            {
                // Increment failed access attempts
                user.AccessFailedCount++;

                // Lock account if too many failed attempts
                if (user.AccessFailedCount >= 5)
                {
                    user.LockoutEnd = DateTimeOffset.UtcNow.AddMinutes(15);
                    result.IsLockedOut = true;
                    _logger.LogWarning("User {UserId} locked out due to too many failed login attempts", user.Id);
                }

                await _context.SaveChangesAsync();
                result.FailureReason = "Invalid username or password";
                return result;
            }

            // Reset failed access count on successful authentication
            user.AccessFailedCount = 0;
            user.LastLoginDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            result.Success = true;
            result.User = user;

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating user {UsernameOrEmail}", usernameOrEmail);
            result.FailureReason = "An error occurred during authentication";
            return result;
        }
    }

    public async Task<bool> UserExistsAsync(string usernameOrEmail)
    {
        return await _context.Users
            .AnyAsync(u => u.UserName == usernameOrEmail || u.Email == usernameOrEmail);
    }

    #endregion

    #region Profile Management

    public async Task<UserProfile?> GetUserProfileAsync(int userId)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.Academic)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return null;

            var roles = user.UserRoles
                .Where(ur => ur.IsCurrentlyEffective())
                .Select(ur => new UserRoleInfo
                {
                    RoleId = ur.RoleId,
                    RoleName = ur.Role.Name,
                    DepartmentContext = ur.DepartmentContextName,
                    EffectiveDate = ur.EffectiveDate,
                    ExpirationDate = ur.ExpirationDate,
                    IsActive = ur.IsActive,
                    IsPrimary = ur.IsPrimary,
                    AssignedBy = ur.AssignedBy,
                    AssignmentReason = ur.AssignmentReason
                })
                .ToList();

            return new UserProfile
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                EmailConfirmed = user.EmailConfirmed,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DisplayName = user.DisplayName,
                IsActive = user.IsActive,
                LastLoginDate = user.LastLoginDate,
                Academic = user.Academic,
                Roles = roles
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user profile for user {UserId}", userId);
            return null;
        }
    }

    public async Task<UserUpdateResult> UpdateUserProfileAsync(int userId, UserProfileUpdateRequest profile)
    {
        var result = new UserUpdateResult();

        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                result.Errors.Add("User not found");
                return result;
            }

            // Update fields if provided
            var hasChanges = false;

            if (!string.IsNullOrEmpty(profile.FirstName) && profile.FirstName != user.FirstName)
            {
                user.FirstName = profile.FirstName;
                hasChanges = true;
            }

            if (!string.IsNullOrEmpty(profile.LastName) && profile.LastName != user.LastName)
            {
                user.LastName = profile.LastName;
                hasChanges = true;
            }

            if (!string.IsNullOrEmpty(profile.DisplayName) && profile.DisplayName != user.DisplayName)
            {
                user.DisplayName = profile.DisplayName;
                hasChanges = true;
            }

            if (!string.IsNullOrEmpty(profile.UserName) && profile.UserName != user.UserName)
            {
                // Check if username is already taken
                if (await _context.Users.AnyAsync(u => u.Id != userId && u.UserName == profile.UserName))
                {
                    result.Errors.Add("Username is already taken");
                    return result;
                }
                user.UserName = profile.UserName;
                hasChanges = true;
            }

            if (!string.IsNullOrEmpty(profile.Email) && profile.Email != user.Email)
            {
                // Check if email is already taken
                if (await _context.Users.AnyAsync(u => u.Id != userId && u.Email == profile.Email))
                {
                    result.Errors.Add("Email is already taken");
                    return result;
                }
                user.Email = profile.Email;
                user.EmailConfirmed = false; // Require re-confirmation
                result.RequiresEmailConfirmation = true;
                hasChanges = true;
            }

            if (hasChanges)
            {
                user.ModifiedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation("User {UserId} profile updated", userId);
            }

            result.Success = true;
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user profile for user {UserId}", userId);
            result.Errors.Add("An error occurred during profile update");
            return result;
        }
    }

    public async Task<PasswordChangeResult> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        var result = new PasswordChangeResult();

        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                result.Errors.Add("User not found");
                return result;
            }

            // Verify current password
            if (!_passwordService.VerifyPassword(currentPassword, user.PasswordHash ?? string.Empty))
            {
                result.Errors.Add("Current password is incorrect");
                return result;
            }

            // Validate new password
            var passwordValidation = ValidatePassword(newPassword);
            if (passwordValidation.Any())
            {
                result.Errors = passwordValidation;
                return result;
            }

            // Update password
            user.PasswordHash = _passwordService.HashPassword(newPassword);
            user.ModifiedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            result.Success = true;

            _logger.LogInformation("Password changed for user {UserId}", userId);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for user {UserId}", userId);
            result.Errors.Add("An error occurred during password change");
            return result;
        }
    }

    #endregion

    #region Private Helper Methods

    private List<string> ValidateRegistrationRequest(UserRegistrationRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Email))
            errors.Add("Email is required");
        else if (!IsValidEmail(request.Email))
            errors.Add("Invalid email format");

        if (string.IsNullOrWhiteSpace(request.UserName))
            errors.Add("Username is required");
        else if (request.UserName.Length < 3)
            errors.Add("Username must be at least 3 characters long");

        if (string.IsNullOrWhiteSpace(request.FirstName))
            errors.Add("First name is required");

        if (string.IsNullOrWhiteSpace(request.LastName))
            errors.Add("Last name is required");

        var passwordErrors = ValidatePassword(request.Password);
        errors.AddRange(passwordErrors);

        return errors;
    }

    private List<string> ValidatePassword(string password)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(password))
        {
            errors.Add("Password is required");
            return errors;
        }

        if (password.Length < 8)
            errors.Add("Password must be at least 8 characters long");

        if (!password.Any(char.IsUpper))
            errors.Add("Password must contain at least one uppercase letter");

        if (!password.Any(char.IsLower))
            errors.Add("Password must contain at least one lowercase letter");

        if (!password.Any(char.IsDigit))
            errors.Add("Password must contain at least one number");

        if (!password.Any(c => !char.IsLetterOrDigit(c)))
            errors.Add("Password must contain at least one special character");

        return errors;
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private async Task AssignAutomaticRolesAsync(AcademiaUser user)
    {
        try
        {
            // Use the existing role assignment service
            await _roleAssignmentService.AssignAutomaticRolesAsync(user);

            _logger.LogInformation("Automatic roles assigned for user {UserId}", user.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning automatic roles for user {UserId}", user.Id);
        }
    }

    private string GenerateEmailConfirmationToken()
    {
        var bytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
    }

    private string GenerateTemporaryPassword()
    {
        const string chars = "ABCDEFGHJKMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789";
        var random = new Random();
        var password = new StringBuilder();

        // Ensure at least one of each required character type
        password.Append(chars[random.Next(0, 26)]); // Uppercase
        password.Append(chars[random.Next(26, 52)]); // Lowercase
        password.Append(chars[random.Next(52, 62)]); // Number
        password.Append("!@#$%"[random.Next(0, 5)]); // Special character

        // Fill the rest randomly
        for (int i = 4; i < 12; i++)
        {
            password.Append(chars[random.Next(chars.Length)]);
        }

        // Shuffle the password
        var passwordArray = password.ToString().ToCharArray();
        for (int i = passwordArray.Length - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (passwordArray[i], passwordArray[j]) = (passwordArray[j], passwordArray[i]);
        }

        return new string(passwordArray);
    }

    private string GeneratePasswordResetToken()
    {
        var bytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
    }

    #endregion

    #region Email Confirmation and Password Reset

    public async Task<EmailOperationResult> SendEmailConfirmationAsync(int userId)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return new EmailOperationResult { Success = false, Error = "User not found" };
            }

            if (user.EmailConfirmed)
            {
                return new EmailOperationResult { Success = false, Error = "Email is already confirmed" };
            }

            var token = GenerateEmailConfirmationToken();

            // In a real implementation, store the token in database or cache with expiration
            // For now, we'll include it in the result for testing

            var emailResult = await _emailService.SendEmailConfirmationAsync(
                user.Email ?? string.Empty,
                user.DisplayName ?? user.UserName ?? "User",
                token);

            if (emailResult.Success)
            {
                _logger.LogInformation("Email confirmation sent to user {UserId}", userId);
                return new EmailOperationResult { Success = true, Token = token };
            }
            else
            {
                _logger.LogWarning("Failed to send email confirmation to user {UserId}: {Error}", userId, emailResult.Error);
                return new EmailOperationResult { Success = false, Error = emailResult.Error };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email confirmation for user {UserId}", userId);
            return new EmailOperationResult { Success = false, Error = "An error occurred while sending confirmation email" };
        }
    }

    public async Task<EmailConfirmationResult> ConfirmEmailAsync(int userId, string token)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return new EmailConfirmationResult { Success = false, Error = "User not found" };
            }

            if (user.EmailConfirmed)
            {
                return new EmailConfirmationResult { Success = false, Error = "Email is already confirmed" };
            }

            // In a real implementation, validate the token against stored token with expiration check
            // For now, we'll accept any non-empty token
            if (string.IsNullOrWhiteSpace(token))
            {
                return new EmailConfirmationResult { Success = false, Error = "Invalid confirmation token" };
            }

            user.EmailConfirmed = true;
            user.ModifiedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Email confirmed for user {UserId}", userId);

            return new EmailConfirmationResult { Success = true };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error confirming email for user {UserId}", userId);
            return new EmailConfirmationResult { Success = false, Error = "An error occurred while confirming email" };
        }
    }

    public async Task<PasswordResetRequestResult> RequestPasswordResetAsync(string usernameOrEmail)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.UserName == usernameOrEmail || u.Email == usernameOrEmail);

            // Don't reveal whether user exists or not for security
            if (user == null)
            {
                _logger.LogWarning("Password reset requested for non-existent user: {UsernameOrEmail}", usernameOrEmail);
                return new PasswordResetRequestResult { Success = true }; // Pretend success
            }

            if (!user.IsActive)
            {
                _logger.LogWarning("Password reset requested for inactive user {UserId}", user.Id);
                return new PasswordResetRequestResult { Success = true }; // Pretend success
            }

            var token = GeneratePasswordResetToken();

            // In a real implementation, store the token in database or cache with short expiration (1 hour)

            var emailResult = await _emailService.SendPasswordResetAsync(
                user.Email ?? string.Empty,
                user.DisplayName ?? user.UserName ?? "User",
                token);

            if (emailResult.Success)
            {
                _logger.LogInformation("Password reset email sent for user {UserId}", user.Id);
                return new PasswordResetRequestResult { Success = true, Token = token };
            }
            else
            {
                _logger.LogError("Failed to send password reset email for user {UserId}: {Error}", user.Id, emailResult.Error);
                return new PasswordResetRequestResult { Success = false, Error = "Failed to send reset email" };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error requesting password reset for {UsernameOrEmail}", usernameOrEmail);
            return new PasswordResetRequestResult { Success = false, Error = "An error occurred while processing reset request" };
        }
    }

    public async Task<PasswordResetResult> ResetPasswordAsync(string token, string newPassword)
    {
        var result = new PasswordResetResult();

        try
        {
            // In a real implementation, validate token and get associated user from database/cache
            // For now, we'll simulate token validation
            if (string.IsNullOrWhiteSpace(token))
            {
                result.Errors.Add("Invalid reset token");
                return result;
            }

            // Validate new password
            var passwordErrors = ValidatePassword(newPassword);
            if (passwordErrors.Any())
            {
                result.Errors = passwordErrors;
                return result;
            }

            // In a real implementation, find user by token
            // For demonstration, we'll need a different approach
            // This would typically involve storing tokens in a separate table with user references

            _logger.LogWarning("Password reset functionality requires token storage implementation");
            result.Errors.Add("Password reset functionality requires full token storage implementation");
            await Task.CompletedTask; // Satisfy async requirement
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting password with token");
            result.Errors.Add("An error occurred while resetting password");
            return result;
        }
    }

    #endregion

    #region Administration Functions

    public async Task<PagedResult<UserSummary>> SearchUsersAsync(UserSearchRequest searchRequest)
    {
        try
        {
            var query = _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(searchRequest.SearchTerm))
            {
                var searchTerm = searchRequest.SearchTerm.ToLower();
                query = query.Where(u =>
                    u.UserName!.ToLower().Contains(searchTerm) ||
                    u.Email!.ToLower().Contains(searchTerm) ||
                    u.FirstName!.ToLower().Contains(searchTerm) ||
                    u.LastName!.ToLower().Contains(searchTerm) ||
                    u.DisplayName!.ToLower().Contains(searchTerm));
            }

            if (!string.IsNullOrWhiteSpace(searchRequest.Email))
            {
                query = query.Where(u => u.Email!.ToLower().Contains(searchRequest.Email.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(searchRequest.Department))
            {
                query = query.Where(u => u.UserRoles.Any(ur =>
                    ur.DepartmentContextName == searchRequest.Department && ur.IsCurrentlyEffective()));
            }

            if (!string.IsNullOrWhiteSpace(searchRequest.Role))
            {
                query = query.Where(u => u.UserRoles.Any(ur =>
                    ur.Role.Name == searchRequest.Role && ur.IsCurrentlyEffective()));
            }

            if (searchRequest.IsActive.HasValue)
            {
                query = query.Where(u => u.IsActive == searchRequest.IsActive.Value);
            }

            if (searchRequest.EmailConfirmed.HasValue)
            {
                query = query.Where(u => u.EmailConfirmed == searchRequest.EmailConfirmed.Value);
            }

            if (searchRequest.CreatedAfter.HasValue)
            {
                query = query.Where(u => u.CreatedDate >= searchRequest.CreatedAfter.Value);
            }

            if (searchRequest.CreatedBefore.HasValue)
            {
                query = query.Where(u => u.CreatedDate <= searchRequest.CreatedBefore.Value);
            }

            if (searchRequest.LastLoginAfter.HasValue)
            {
                query = query.Where(u => u.LastLoginDate >= searchRequest.LastLoginAfter.Value);
            }

            if (searchRequest.LastLoginBefore.HasValue)
            {
                query = query.Where(u => u.LastLoginDate <= searchRequest.LastLoginBefore.Value);
            }

            // Apply sorting
            query = searchRequest.SortBy.ToLower() switch
            {
                "username" => searchRequest.SortDirection.ToLower() == "desc"
                    ? query.OrderByDescending(u => u.UserName)
                    : query.OrderBy(u => u.UserName),
                "email" => searchRequest.SortDirection.ToLower() == "desc"
                    ? query.OrderByDescending(u => u.Email)
                    : query.OrderBy(u => u.Email),
                "firstname" => searchRequest.SortDirection.ToLower() == "desc"
                    ? query.OrderByDescending(u => u.FirstName)
                    : query.OrderBy(u => u.FirstName),
                "lastname" => searchRequest.SortDirection.ToLower() == "desc"
                    ? query.OrderByDescending(u => u.LastName)
                    : query.OrderBy(u => u.LastName),
                "lastlogin" => searchRequest.SortDirection.ToLower() == "desc"
                    ? query.OrderByDescending(u => u.LastLoginDate)
                    : query.OrderBy(u => u.LastLoginDate),
                _ => searchRequest.SortDirection.ToLower() == "desc"
                    ? query.OrderByDescending(u => u.CreatedDate)
                    : query.OrderBy(u => u.CreatedDate)
            };

            // Get total count
            var totalCount = await query.CountAsync();

            // Apply pagination
            var users = await query
                .Skip((searchRequest.Page - 1) * searchRequest.PageSize)
                .Take(searchRequest.PageSize)
                .Select(u => new UserSummary
                {
                    Id = u.Id,
                    UserName = u.UserName ?? string.Empty,
                    Email = u.Email ?? string.Empty,
                    FullName = $"{u.FirstName} {u.LastName}".Trim(),
                    IsActive = u.IsActive,
                    EmailConfirmed = u.EmailConfirmed,
                    LastLoginDate = u.LastLoginDate,
                    CreatedDate = u.CreatedDate,
                    Roles = u.UserRoles
                        .Where(ur => ur.IsCurrentlyEffective())
                        .Select(ur => ur.Role.Name)
                        .ToList()
                })
                .ToListAsync();

            return new PagedResult<UserSummary>
            {
                Items = users,
                TotalCount = totalCount,
                Page = searchRequest.Page,
                PageSize = searchRequest.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching users");
            return new PagedResult<UserSummary>();
        }
    }

    public async Task<UserAdminDetails?> GetUserDetailsAsync(int userId)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.Academic)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return null;

            var activeTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && rt.IsActive)
                .ToListAsync();

            var roles = user.UserRoles
                .Select(ur => new UserRoleInfo
                {
                    RoleId = ur.RoleId,
                    RoleName = ur.Role.Name,
                    DepartmentContext = ur.DepartmentContextName,
                    EffectiveDate = ur.EffectiveDate,
                    ExpirationDate = ur.ExpirationDate,
                    IsActive = ur.IsActive,
                    IsPrimary = ur.IsPrimary,
                    AssignedBy = ur.AssignedBy,
                    AssignmentReason = ur.AssignmentReason
                })
                .ToList();

            return new UserAdminDetails
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                EmailConfirmed = user.EmailConfirmed,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DisplayName = user.DisplayName,
                IsActive = user.IsActive,
                CreatedDate = user.CreatedDate,
                ModifiedDate = user.ModifiedDate,
                LastLoginDate = user.LastLoginDate,
                LastLoginIpAddress = user.LastLoginIpAddress,
                AccessFailedCount = user.AccessFailedCount,
                LockoutEnd = user.LockoutEnd,
                LockoutEnabled = user.LockoutEnabled,
                Academic = user.Academic,
                Roles = roles,
                ActiveTokens = activeTokens
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user details for user {UserId}", userId);
            return null;
        }
    }

    public async Task<UserStatusChangeResult> SetUserActiveStatusAsync(int userId, bool isActive, string? reason = null)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return new UserStatusChangeResult { Success = false, Error = "User not found" };
            }

            if (user.IsActive == isActive)
            {
                return new UserStatusChangeResult { Success = true }; // No change needed
            }

            user.IsActive = isActive;
            user.ModifiedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Send notification email
            var statusChange = isActive ? "activated" : "deactivated";
            await _emailService.SendAccountStatusChangeAsync(
                user.Email ?? string.Empty,
                user.DisplayName ?? user.UserName ?? "User",
                $"Your account has been {statusChange}. {reason ?? ""}".Trim());

            _logger.LogInformation("User {UserId} status changed to {Status}. Reason: {Reason}",
                userId, isActive ? "Active" : "Inactive", reason ?? "No reason provided");

            return new UserStatusChangeResult { Success = true };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing active status for user {UserId}", userId);
            return new UserStatusChangeResult { Success = false, Error = "An error occurred while changing user status" };
        }
    }

    public async Task<UserStatusChangeResult> SetUserLockStatusAsync(int userId, DateTimeOffset? lockUntil, string? reason = null)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return new UserStatusChangeResult { Success = false, Error = "User not found" };
            }

            user.LockoutEnd = lockUntil;
            user.ModifiedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Send notification email
            var statusChange = lockUntil.HasValue && lockUntil > DateTimeOffset.UtcNow
                ? $"locked until {lockUntil:yyyy-MM-dd HH:mm} UTC"
                : "unlocked";

            await _emailService.SendAccountStatusChangeAsync(
                user.Email ?? string.Empty,
                user.DisplayName ?? user.UserName ?? "User",
                $"Your account has been {statusChange}. {reason ?? ""}".Trim());

            _logger.LogInformation("User {UserId} lock status changed. Lock until: {LockUntil}. Reason: {Reason}",
                userId, lockUntil?.ToString() ?? "Unlocked", reason ?? "No reason provided");

            return new UserStatusChangeResult { Success = true };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing lock status for user {UserId}", userId);
            return new UserStatusChangeResult { Success = false, Error = "An error occurred while changing user lock status" };
        }
    }

    public async Task<List<UserRoleInfo>> GetUserRolesAsync(int userId)
    {
        try
        {
            var roles = await _context.UserRoles
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == userId)
                .Select(ur => new UserRoleInfo
                {
                    RoleId = ur.RoleId,
                    RoleName = ur.Role.Name,
                    DepartmentContext = ur.DepartmentContextName,
                    EffectiveDate = ur.EffectiveDate,
                    ExpirationDate = ur.ExpirationDate,
                    IsActive = ur.IsActive,
                    IsPrimary = ur.IsPrimary,
                    AssignedBy = ur.AssignedBy,
                    AssignmentReason = ur.AssignmentReason
                })
                .ToListAsync();

            return roles;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user roles for user {UserId}", userId);
            return new List<UserRoleInfo>();
        }
    }

    public async Task<RoleAssignmentResult> AssignRoleToUserAsync(int userId, int roleId, string? departmentContext = null,
        DateTime? expirationDate = null, int? assignedBy = null, string? reason = null)
    {
        var result = new RoleAssignmentResult();

        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                result.Errors.Add("User not found");
                return result;
            }

            var role = await _context.Roles.FindAsync(roleId);
            if (role == null)
            {
                result.Errors.Add("Role not found");
                return result;
            }

            // Check if assignment already exists
            var existingAssignment = await _context.UserRoles
                .FirstOrDefaultAsync(ur =>
                    ur.UserId == userId &&
                    ur.RoleId == roleId &&
                    ur.DepartmentContextName == departmentContext &&
                    ur.IsCurrentlyEffective());

            if (existingAssignment != null)
            {
                result.Errors.Add("User already has this role assignment");
                return result;
            }

            // Create new role assignment
            var userRole = new AcademiaUserRole
            {
                UserId = userId,
                RoleId = roleId,
                DepartmentContextName = departmentContext,
                EffectiveDate = DateTime.UtcNow,
                ExpirationDate = expirationDate,
                AssignedBy = assignedBy?.ToString(),
                AssignmentReason = reason,
                IsActive = true,
                IsPrimary = false
            };

            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();

            // Send notification email
            await _emailService.SendRoleAssignmentAsync(
                user.Email ?? string.Empty,
                user.DisplayName ?? user.UserName ?? "User",
                role.Name,
                departmentContext);

            result.Success = true;

            _logger.LogInformation("Role {RoleId} assigned to user {UserId} in department {Department}",
                roleId, userId, departmentContext ?? "Global");

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning role {RoleId} to user {UserId}", roleId, userId);
            result.Errors.Add("An error occurred while assigning role");
            return result;
        }
    }

    public async Task<RoleAssignmentResult> RemoveRoleFromUserAsync(int userId, int roleId, string? departmentContext = null, string? reason = null)
    {
        var result = new RoleAssignmentResult();

        try
        {
            var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur =>
                    ur.UserId == userId &&
                    ur.RoleId == roleId &&
                    ur.DepartmentContextName == departmentContext &&
                    ur.IsCurrentlyEffective());

            if (userRole == null)
            {
                result.Errors.Add("Role assignment not found");
                return result;
            }

            // Deactivate the role assignment
            userRole.IsActive = false;
            userRole.ExpirationDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            result.Success = true;

            _logger.LogInformation("Role {RoleId} removed from user {UserId} in department {Department}. Reason: {Reason}",
                roleId, userId, departmentContext ?? "Global", reason ?? "No reason provided");

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing role {RoleId} from user {UserId}", roleId, userId);
            result.Errors.Add("An error occurred while removing role");
            return result;
        }
    }

    public async Task<UserImportResult> ImportUsersAsync(List<UserImportRequest> users)
    {
        var result = new UserImportResult
        {
            TotalProcessed = users.Count
        };

        try
        {
            for (int i = 0; i < users.Count; i++)
            {
                var userRequest = users[i];
                try
                {
                    // Validate user data
                    var validationErrors = ValidateImportUser(userRequest);
                    if (validationErrors.Any())
                    {
                        result.Errors.Add(new UserImportError
                        {
                            RowNumber = i + 1,
                            Email = userRequest.Email,
                            Errors = validationErrors
                        });
                        result.ErrorCount++;
                        continue;
                    }

                    // Check if user already exists
                    if (await UserExistsAsync(userRequest.Email) || await UserExistsAsync(userRequest.UserName))
                    {
                        result.Errors.Add(new UserImportError
                        {
                            RowNumber = i + 1,
                            Email = userRequest.Email,
                            Errors = new List<string> { "User already exists" }
                        });
                        result.ErrorCount++;
                        continue;
                    }

                    // Create user
                    var registrationRequest = new UserRegistrationRequest
                    {
                        Email = userRequest.Email,
                        UserName = userRequest.UserName,
                        FirstName = userRequest.FirstName ?? string.Empty,
                        LastName = userRequest.LastName ?? string.Empty,
                        Password = GenerateTemporaryPassword(),
                        AcademicId = userRequest.AcademicId,
                        DepartmentName = userRequest.DepartmentName,
                        SendConfirmationEmail = false
                    };

                    var registrationResult = await RegisterUserAsync(registrationRequest);
                    if (registrationResult.Success)
                    {
                        result.SuccessCount++;
                    }
                    else
                    {
                        result.Errors.Add(new UserImportError
                        {
                            RowNumber = i + 1,
                            Email = userRequest.Email,
                            Errors = registrationResult.Errors
                        });
                        result.ErrorCount++;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error importing user at row {Row}", i + 1);
                    result.Errors.Add(new UserImportError
                    {
                        RowNumber = i + 1,
                        Email = userRequest.Email,
                        Errors = new List<string> { "An error occurred during import" }
                    });
                    result.ErrorCount++;
                }
            }

            _logger.LogInformation("User import completed. Success: {Success}, Errors: {Errors}",
                result.SuccessCount, result.ErrorCount);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during bulk user import");
            result.Errors.Add(new UserImportError
            {
                RowNumber = 0,
                Email = "System",
                Errors = new List<string> { "System error during import" }
            });
            return result;
        }
    }

    public async Task<UserExportResult> ExportUsersAsync(UserSearchRequest searchCriteria, ExportFormat format)
    {
        try
        {
            var users = await SearchUsersAsync(searchCriteria);

            return format switch
            {
                ExportFormat.Csv => await ExportToCsvAsync(users.Items),
                ExportFormat.Excel => await ExportToExcelAsync(users.Items),
                ExportFormat.Json => await ExportToJsonAsync(users.Items),
                _ => new UserExportResult { Success = false, Error = "Unsupported export format" }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting users");
            return new UserExportResult { Success = false, Error = "An error occurred during export" };
        }
    }

    #endregion

    #region Private Helper Methods for Administration

    private List<string> ValidateImportUser(UserImportRequest user)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(user.Email))
            errors.Add("Email is required");
        else if (!IsValidEmail(user.Email))
            errors.Add("Invalid email format");

        if (string.IsNullOrWhiteSpace(user.UserName))
            errors.Add("Username is required");

        return errors;
    }

    private Task<UserExportResult> ExportToCsvAsync(List<UserSummary> users)
    {
        try
        {
            var csv = new StringBuilder();
            csv.AppendLine("Id,UserName,Email,FullName,IsActive,EmailConfirmed,LastLoginDate,CreatedDate,Roles");

            foreach (var user in users)
            {
                csv.AppendLine($"{user.Id},{user.UserName},{user.Email},{user.FullName},{user.IsActive},{user.EmailConfirmed},{user.LastLoginDate},{user.CreatedDate:yyyy-MM-dd},\"{string.Join("; ", user.Roles)}\"");
            }

            var data = Encoding.UTF8.GetBytes(csv.ToString());

            return Task.FromResult(new UserExportResult
            {
                Success = true,
                Data = data,
                FileName = $"users_export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv",
                ContentType = "text/csv"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting to CSV");
            return Task.FromResult(new UserExportResult { Success = false, Error = "Error creating CSV export" });
        }
    }

    private async Task<UserExportResult> ExportToExcelAsync(List<UserSummary> users)
    {
        // Excel export would require additional libraries like EPPlus or similar
        // For now, return CSV format
        return await ExportToCsvAsync(users);
    }

    private Task<UserExportResult> ExportToJsonAsync(List<UserSummary> users)
    {
        try
        {
            var json = System.Text.Json.JsonSerializer.Serialize(users, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });

            var data = Encoding.UTF8.GetBytes(json);

            return Task.FromResult(new UserExportResult
            {
                Success = true,
                Data = data,
                FileName = $"users_export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json",
                ContentType = "application/json"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting to JSON");
            return Task.FromResult(new UserExportResult { Success = false, Error = "Error creating JSON export" });
        }
    }

    #endregion
}