using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.Infrastructure.Identity;

/// <summary>
/// Custom user entity that integrates with Identity and links to the Academic entity.
/// Represents user accounts in the Zeus Academia System with academic integration.
/// Inherits from IdentityUser to provide authentication and authorization capabilities.
/// </summary>
public class AcademiaUser : IdentityUser<int>
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
    /// Gets or sets whether the user account is active.
    /// Inactive accounts cannot log in to the system.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the date when the entity was created.
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the identifier of the user who created the entity.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the date when the entity was last modified.
    /// </summary>
    public DateTime? ModifiedDate { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who last modified the entity.
    /// </summary>
    public string? ModifiedBy { get; set; }

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
    /// Navigation property to the roles assigned to this user.
    /// Users can have multiple roles with different contexts and effective dates.
    /// </summary>
    public virtual ICollection<AcademiaUserRole> UserRoles { get; set; } = new List<AcademiaUserRole>();

    /// <summary>
    /// Navigation property to the refresh tokens for this user.
    /// </summary>
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

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

    /// <summary>
    /// Gets all currently effective roles for this user.
    /// </summary>
    /// <returns>Collection of effective user role assignments</returns>
    public IEnumerable<AcademiaUserRole> GetEffectiveUserRoles()
    {
        return UserRoles.Where(ur => ur.IsCurrentlyEffective());
    }

    /// <summary>
    /// Gets all currently effective roles for this user.
    /// </summary>
    /// <returns>Collection of effective roles</returns>
    public IEnumerable<AcademiaRole> GetEffectiveRoles()
    {
        return GetEffectiveUserRoles().Select(ur => ur.Role);
    }

    /// <summary>
    /// Gets the primary role for this user.
    /// </summary>
    /// <returns>The primary role if one exists, otherwise null</returns>
    public AcademiaRole? GetPrimaryRole()
    {
        return GetEffectiveUserRoles()
            .FirstOrDefault(ur => ur.IsPrimary)?.Role;
    }

    /// <summary>
    /// Checks if the user has a specific role type.
    /// </summary>
    /// <param name="roleType">The role type to check for</param>
    /// <returns>True if the user has an effective role of the specified type</returns>
    public bool HasRole(AcademicRoleType roleType)
    {
        return GetEffectiveRoles().Any(r => r.RoleType == roleType);
    }

    /// <summary>
    /// Checks if the user has any of the specified role types.
    /// </summary>
    /// <param name="roleTypes">The role types to check for</param>
    /// <returns>True if the user has any of the specified roles</returns>
    public bool HasAnyRole(params AcademicRoleType[] roleTypes)
    {
        var userRoleTypes = GetEffectiveRoles().Select(r => r.RoleType).ToHashSet();
        return roleTypes.Any(rt => userRoleTypes.Contains(rt));
    }

    /// <summary>
    /// Gets the highest priority role for this user.
    /// </summary>
    /// <returns>The role with the highest priority, or null if no roles</returns>
    public AcademiaRole? GetHighestPriorityRole()
    {
        return GetEffectiveRoles()
            .OrderByDescending(r => r.Priority)
            .FirstOrDefault();
    }

    /// <summary>
    /// Determines if this user can manage another user based on role hierarchy.
    /// </summary>
    /// <param name="targetUser">The user to check management permissions for</param>
    /// <returns>True if this user can manage the target user</returns>
    public bool CanManage(AcademiaUser targetUser)
    {
        var myHighestRole = GetHighestPriorityRole();
        var targetHighestRole = targetUser.GetHighestPriorityRole();

        if (myHighestRole == null || targetHighestRole == null)
            return false;

        return myHighestRole.CanManage(targetHighestRole);
    }

    /// <summary>
    /// Gets a description of all effective roles for this user.
    /// </summary>
    /// <returns>Formatted string describing the user's roles</returns>
    public string GetRolesDescription()
    {
        var effectiveRoles = GetEffectiveUserRoles().ToList();

        if (!effectiveRoles.Any())
            return "No active roles";

        var descriptions = effectiveRoles
            .Select(ur => ur.GetAssignmentDescription())
            .ToList();

        return string.Join(", ", descriptions);
    }
}