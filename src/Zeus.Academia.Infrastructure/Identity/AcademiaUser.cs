using System.ComponentModel.DataAnnotations;
using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.Infrastructure.Identity;

/// <summary>
/// Custom user entity that will integrate with Identity and links to the Academic entity.
/// Represents user accounts in the Zeus Academia System with academic integration.
/// Note: This will be updated to inherit from IdentityUser once Identity packages are properly configured.
/// </summary>
public class AcademiaUser : BaseEntity
{
    /// <summary>
    /// Gets or sets the academic employee number that this user account is linked to.
    /// This establishes the relationship between the user account and their academic record.
    /// </summary>
    public int? AcademicId { get; set; }

    /// <summary>
    /// Gets or sets the first name of the user.
    /// </summary>
    [MaxLength(100)]
    public string? FirstName { get; set; }

    /// <summary>
    /// Gets or sets the last name of the user.
    /// </summary>
    [MaxLength(100)]
    public string? LastName { get; set; }

    /// <summary>
    /// Gets or sets the full display name of the user.
    /// This is automatically computed from FirstName and LastName.
    /// </summary>
    [MaxLength(200)]
    public string? DisplayName { get; set; }

    /// <summary>
    /// Gets or sets the username for the user account.
    /// </summary>
    [MaxLength(256)]
    public string? UserName { get; set; }

    /// <summary>
    /// Gets or sets the email address for the user account.
    /// </summary>
    [MaxLength(256)]
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets whether the email address has been confirmed.
    /// </summary>
    public bool EmailConfirmed { get; set; } = false;

    /// <summary>
    /// Gets or sets the hashed password for the user account.
    /// </summary>
    public string? PasswordHash { get; set; }

    /// <summary>
    /// Gets or sets whether the user account is active.
    /// Inactive accounts cannot log in to the system.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the number of failed login attempts.
    /// </summary>
    public int AccessFailedCount { get; set; } = 0;

    /// <summary>
    /// Gets or sets whether the account is locked out.
    /// </summary>
    public bool LockoutEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the date when the lockout ends (if locked out).
    /// </summary>
    public DateTimeOffset? LockoutEnd { get; set; }

    /// <summary>
    /// Gets or sets the date when the user last logged in.
    /// </summary>
    public DateTime? LastLoginDate { get; set; }

    /// <summary>
    /// Gets or sets the IP address from the user's last login.
    /// </summary>
    [MaxLength(45)] // IPv6 max length
    public string? LastLoginIpAddress { get; set; }

    /// <summary>
    /// Navigation property to the associated Academic entity.
    /// This allows accessing the academic information (professor, student, etc.) for this user.
    /// </summary>
    public virtual Academic? Academic { get; set; }

    /// <summary>
    /// Gets the computed full name of the user.
    /// </summary>
    public string FullName => !string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName)
        ? $"{FirstName} {LastName}"
        : DisplayName ?? UserName ?? "Unknown User";

    /// <summary>
    /// Updates the display name based on first and last names.
    /// </summary>
    public void UpdateDisplayName()
    {
        DisplayName = FullName;
        ModifiedDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Records a successful login attempt.
    /// </summary>
    /// <param name="ipAddress">The IP address of the login attempt</param>
    public void RecordLogin(string? ipAddress = null)
    {
        LastLoginDate = DateTime.UtcNow;
        LastLoginIpAddress = ipAddress;
        ModifiedDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Deactivates the user account.
    /// </summary>
    /// <param name="modifiedBy">Who is deactivating the account</param>
    public void Deactivate(string? modifiedBy = null)
    {
        IsActive = false;
        ModifiedDate = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? ModifiedBy;
    }

    /// <summary>
    /// Activates the user account.
    /// </summary>
    /// <param name="modifiedBy">Who is activating the account</param>
    public void Activate(string? modifiedBy = null)
    {
        IsActive = true;
        ModifiedDate = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? ModifiedBy;
    }
}