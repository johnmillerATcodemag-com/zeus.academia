namespace Zeus.Academia.Infrastructure.Identity;

/// <summary>
/// Configuration options for Academia Identity system.
/// </summary>
public class AcademiaIdentityOptions
{
    /// <summary>
    /// Password requirements configuration.
    /// </summary>
    public PasswordRequirements Password { get; set; } = new();

    /// <summary>
    /// User account lockout configuration.
    /// </summary>
    public LockoutSettings Lockout { get; set; } = new();

    /// <summary>
    /// User account requirements.
    /// </summary>
    public UserSettings User { get; set; } = new();
}

/// <summary>
/// Password requirements for user accounts.
/// </summary>
public class PasswordRequirements
{
    /// <summary>
    /// Requires at least one digit in the password.
    /// </summary>
    public bool RequireDigit { get; set; } = true;

    /// <summary>
    /// Requires at least one uppercase letter in the password.
    /// </summary>
    public bool RequireUppercase { get; set; } = true;

    /// <summary>
    /// Requires at least one lowercase letter in the password.
    /// </summary>
    public bool RequireLowercase { get; set; } = true;

    /// <summary>
    /// Requires at least one non-alphanumeric character in the password.
    /// </summary>
    public bool RequireNonAlphanumeric { get; set; } = true;

    /// <summary>
    /// Minimum required length for passwords.
    /// </summary>
    public int RequiredLength { get; set; } = 8;

    /// <summary>
    /// Minimum number of unique characters required in passwords.
    /// </summary>
    public int RequiredUniqueChars { get; set; } = 4;
}

/// <summary>
/// Lockout settings for user accounts.
/// </summary>
public class LockoutSettings
{
    /// <summary>
    /// Default lockout time span when an account is locked.
    /// </summary>
    public TimeSpan DefaultLockoutTimeSpan { get; set; } = TimeSpan.FromMinutes(15);

    /// <summary>
    /// Maximum number of failed access attempts before lockout.
    /// </summary>
    public int MaxFailedAccessAttempts { get; set; } = 5;

    /// <summary>
    /// Whether lockout is allowed for new users.
    /// </summary>
    public bool AllowedForNewUsers { get; set; } = true;
}

/// <summary>
/// User account settings.
/// </summary>
public class UserSettings
{
    /// <summary>
    /// Requires unique email addresses for user accounts.
    /// </summary>
    public bool RequireUniqueEmail { get; set; } = true;

    /// <summary>
    /// Characters allowed in usernames.
    /// </summary>
    public string AllowedUserNameCharacters { get; set; } = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

    /// <summary>
    /// Requires confirmed email before account can be used.
    /// </summary>
    public bool RequireConfirmedEmail { get; set; } = true;

    /// <summary>
    /// Requires confirmed phone number before account can be used.
    /// </summary>
    public bool RequireConfirmedPhoneNumber { get; set; } = false;
}