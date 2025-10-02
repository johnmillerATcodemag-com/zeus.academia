using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.Infrastructure.Identity;

/// <summary>
/// Represents the many-to-many relationship between users and roles in the Zeus Academia System.
/// This entity tracks role assignments, including context and audit information.
/// </summary>
public class AcademiaUserRole : IdentityUserRole<int>
{
    // UserId and RoleId are inherited from IdentityUserRole<int>

    /// <summary>
    /// Gets or sets the unique identifier for this user role assignment.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the date when the entity was created.
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the identifier of the user who created the entity.
    /// </summary>
    [MaxLength(50)]
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the date when the entity was last modified.
    /// </summary>
    public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the identifier of the user who last modified the entity.
    /// </summary>
    [MaxLength(50)]
    public string? ModifiedBy { get; set; }

    /// <summary>
    /// Gets or sets when this role assignment becomes effective.
    /// Useful for future-dated role changes or temporary assignments.
    /// </summary>
    public DateTime EffectiveDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets when this role assignment expires.
    /// Null for permanent assignments.
    /// </summary>
    public DateTime? ExpirationDate { get; set; }

    /// <summary>
    /// Gets or sets whether this role assignment is currently active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the reason for this role assignment.
    /// </summary>
    [MaxLength(500)]
    public string? AssignmentReason { get; set; }

    /// <summary>
    /// Gets or sets who assigned this role to the user.
    /// This could be different from CreatedBy in cases of automated assignments.
    /// </summary>
    [MaxLength(100)]
    public string? AssignedBy { get; set; }

    /// <summary>
    /// Gets or sets whether this is a primary role for the user.
    /// Users can have multiple roles, but typically one is considered primary.
    /// </summary>
    public bool IsPrimary { get; set; } = false;

    /// <summary>
    /// Gets or sets the department context for this role assignment.
    /// Some roles may be department-specific even if the role itself is not.
    /// </summary>
    [MaxLength(15)]
    public string? DepartmentContextName { get; set; }

    /// <summary>
    /// Gets or sets additional context or restrictions for this role assignment.
    /// This is a JSON string containing custom permissions or limitations.
    /// </summary>
    public string? AssignmentContext { get; set; }

    /// <summary>
    /// Navigation property to the user who has this role.
    /// </summary>
    public virtual AcademiaUser User { get; set; } = null!;

    /// <summary>
    /// Navigation property to the role that is assigned.
    /// </summary>
    public virtual AcademiaRole Role { get; set; } = null!;

    /// <summary>
    /// Navigation property to the department context if applicable.
    /// </summary>
    public virtual Department? DepartmentContext { get; set; }

    /// <summary>
    /// Determines if this role assignment is currently effective.
    /// Checks if the assignment is active and within the effective date range.
    /// </summary>
    /// <returns>True if the role assignment is currently effective</returns>
    public bool IsCurrentlyEffective()
    {
        var now = DateTime.UtcNow;
        return IsActive &&
               EffectiveDate <= now &&
               (ExpirationDate == null || ExpirationDate > now);
    }

    /// <summary>
    /// Activates this role assignment.
    /// </summary>
    /// <param name="modifiedBy">Who is activating the assignment</param>
    public void Activate(string? modifiedBy = null)
    {
        IsActive = true;
        ModifiedDate = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? ModifiedBy;
    }

    /// <summary>
    /// Deactivates this role assignment.
    /// </summary>
    /// <param name="modifiedBy">Who is deactivating the assignment</param>
    /// <param name="reason">Reason for deactivation</param>
    public void Deactivate(string? modifiedBy = null, string? reason = null)
    {
        IsActive = false;
        ModifiedDate = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? ModifiedBy;

        if (!string.IsNullOrEmpty(reason))
        {
            AssignmentReason = string.IsNullOrEmpty(AssignmentReason)
                ? $"Deactivated: {reason}"
                : $"{AssignmentReason}. Deactivated: {reason}";
        }
    }

    /// <summary>
    /// Sets this assignment as the primary role for the user.
    /// Note: This method does not ensure uniqueness - that should be handled at the service level.
    /// </summary>
    /// <param name="modifiedBy">Who is setting this as primary</param>
    public void SetAsPrimary(string? modifiedBy = null)
    {
        IsPrimary = true;
        ModifiedDate = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? ModifiedBy;
    }

    /// <summary>
    /// Removes the primary designation from this assignment.
    /// </summary>
    /// <param name="modifiedBy">Who is removing the primary designation</param>
    public void RemovePrimary(string? modifiedBy = null)
    {
        IsPrimary = false;
        ModifiedDate = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? ModifiedBy;
    }

    /// <summary>
    /// Extends the expiration date of this role assignment.
    /// </summary>
    /// <param name="newExpirationDate">New expiration date, or null for permanent</param>
    /// <param name="modifiedBy">Who is extending the assignment</param>
    public void ExtendAssignment(DateTime? newExpirationDate, string? modifiedBy = null)
    {
        ExpirationDate = newExpirationDate;
        ModifiedDate = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? ModifiedBy;

        var extension = newExpirationDate?.ToString("yyyy-MM-dd") ?? "permanent";
        AssignmentReason = string.IsNullOrEmpty(AssignmentReason)
            ? $"Extended to: {extension}"
            : $"{AssignmentReason}. Extended to: {extension}";
    }

    /// <summary>
    /// Gets a human-readable description of this role assignment.
    /// </summary>
    /// <returns>Description including role, effective dates, and status</returns>
    public string GetAssignmentDescription()
    {
        var description = $"{Role?.Name ?? "Unknown Role"}";

        if (DepartmentContext != null)
        {
            description += $" in {DepartmentContext.Name}";
        }

        if (ExpirationDate.HasValue)
        {
            description += $" (expires {ExpirationDate:yyyy-MM-dd})";
        }

        if (!IsActive)
        {
            description += " [INACTIVE]";
        }
        else if (!IsCurrentlyEffective())
        {
            description += " [NOT EFFECTIVE]";
        }
        else if (IsPrimary)
        {
            description += " [PRIMARY]";
        }

        return description;
    }
}