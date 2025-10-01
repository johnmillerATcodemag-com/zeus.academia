using System.ComponentModel.DataAnnotations;
using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.Infrastructure.Identity;

/// <summary>
/// Represents a role in the Zeus Academia System.
/// Roles define sets of permissions and capabilities for users within the academic context.
/// </summary>
public class AcademiaRole : BaseEntity
{
    /// <summary>
    /// Gets or sets the type of academic role.
    /// This determines the base permissions and capabilities.
    /// </summary>
    public AcademicRoleType RoleType { get; set; }

    /// <summary>
    /// Gets or sets the name of the role.
    /// This is typically the same as the RoleType display name but can be customized.
    /// </summary>
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the normalized name of the role for case-insensitive searches.
    /// </summary>
    [MaxLength(100)]
    public string NormalizedName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the role and its responsibilities.
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets whether this role is active and can be assigned to users.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets whether this is a system role that cannot be deleted.
    /// System roles are the core academic roles defined by AcademicRoleType.
    /// </summary>
    public bool IsSystemRole { get; set; } = false;

    /// <summary>
    /// Gets or sets the department name if this role is department-specific.
    /// Null for system-wide roles.
    /// </summary>
    [MaxLength(15)]
    public string? DepartmentName { get; set; }

    /// <summary>
    /// Gets or sets the priority level of this role for hierarchy determination.
    /// Higher values indicate higher authority levels.
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// Gets or sets additional permissions granted to this role.
    /// This is a JSON string containing custom permissions beyond the base role type.
    /// </summary>
    public string? AdditionalPermissions { get; set; }

    /// <summary>
    /// Navigation property to the department if this is a department-specific role.
    /// </summary>
    public virtual Department? Department { get; set; }

    /// <summary>
    /// Navigation property to the users assigned to this role.
    /// </summary>
    public virtual ICollection<AcademiaUserRole> UserRoles { get; set; } = new List<AcademiaUserRole>();

    /// <summary>
    /// Initializes the role with default values based on the role type.
    /// </summary>
    /// <param name="roleType">The type of academic role</param>
    /// <param name="departmentId">Optional department ID for department-specific roles</param>
    public void InitializeFromRoleType(AcademicRoleType roleType, string? departmentName = null)
    {
        RoleType = roleType;
        Name = roleType.GetDisplayName();
        NormalizedName = Name.ToUpperInvariant();
        Description = roleType.GetDescription();
        Priority = roleType.GetPriority();
        DepartmentName = departmentName;
        IsSystemRole = departmentName == null; // System roles are not department-specific
        ModifiedDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the normalized name to match the current name.
    /// Call this whenever the Name property is changed.
    /// </summary>
    public void UpdateNormalizedName()
    {
        NormalizedName = Name.ToUpperInvariant();
        ModifiedDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Activates the role so it can be assigned to users.
    /// </summary>
    /// <param name="modifiedBy">Who is activating the role</param>
    public void Activate(string? modifiedBy = null)
    {
        IsActive = true;
        ModifiedDate = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? ModifiedBy;
    }

    /// <summary>
    /// Deactivates the role so it cannot be assigned to new users.
    /// Existing assignments remain but new ones are prevented.
    /// </summary>
    /// <param name="modifiedBy">Who is deactivating the role</param>
    public void Deactivate(string? modifiedBy = null)
    {
        IsActive = false;
        ModifiedDate = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? ModifiedBy;
    }

    /// <summary>
    /// Determines if this role can manage users with the specified target role.
    /// </summary>
    /// <param name="targetRole">The role to check management permissions for</param>
    /// <returns>True if this role can manage the target role</returns>
    public bool CanManage(AcademiaRole targetRole)
    {
        // System admins can manage everyone
        if (RoleType == AcademicRoleType.SystemAdmin)
            return true;

        // Chairs can manage within their department
        if (RoleType == AcademicRoleType.Chair && !string.IsNullOrEmpty(DepartmentName))
        {
            // Can manage same department or lower priority roles
            return (targetRole.DepartmentName == DepartmentName) || (Priority > targetRole.Priority);
        }

        // Use priority-based management for other roles
        return Priority > targetRole.Priority;
    }

    /// <summary>
    /// Gets the display name for this role, including department context if applicable.
    /// </summary>
    /// <returns>A formatted display name for the role</returns>
    public string GetFullDisplayName()
    {
        if (!string.IsNullOrEmpty(DepartmentName) && Department != null)
        {
            return $"{Name} - {Department.Name}";
        }

        return Name;
    }
}