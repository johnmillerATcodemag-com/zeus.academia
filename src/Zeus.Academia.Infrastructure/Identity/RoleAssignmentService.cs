using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.Infrastructure.Identity;

/// <summary>
/// Service for managing role assignments in the Zeus Academia System.
/// Handles automatic role assignment based on Academic entity types and manual role management.
/// </summary>
public interface IRoleAssignmentService
{
    /// <summary>
    /// Automatically assigns roles to a user based on their Academic entity type.
    /// </summary>
    /// <param name="user">The user to assign roles to</param>
    /// <param name="assignedBy">Who is making the assignment</param>
    /// <returns>List of assigned user roles</returns>
    Task<IList<AcademiaUserRole>> AssignAutomaticRolesAsync(AcademiaUser user, string? assignedBy = null);

    /// <summary>
    /// Manually assigns a role to a user.
    /// </summary>
    /// <param name="user">The user to assign the role to</param>
    /// <param name="role">The role to assign</param>
    /// <param name="assignedBy">Who is making the assignment</param>
    /// <param name="reason">Reason for the assignment</param>
    /// <param name="departmentContext">Optional department context</param>
    /// <param name="effectiveDate">When the assignment becomes effective</param>
    /// <param name="expirationDate">When the assignment expires</param>
    /// <param name="isPrimary">Whether this is the primary role</param>
    /// <returns>The created user role assignment</returns>
    Task<AcademiaUserRole> AssignRoleAsync(
        AcademiaUser user,
        AcademiaRole role,
        string? assignedBy = null,
        string? reason = null,
        string? departmentContext = null,
        DateTime? effectiveDate = null,
        DateTime? expirationDate = null,
        bool isPrimary = false);

    /// <summary>
    /// Removes a role assignment from a user.
    /// </summary>
    /// <param name="userRole">The user role assignment to remove</param>
    /// <param name="modifiedBy">Who is removing the assignment</param>
    /// <param name="reason">Reason for removal</param>
    /// <returns>True if successful</returns>
    Task<bool> RemoveRoleAssignmentAsync(AcademiaUserRole userRole, string? modifiedBy = null, string? reason = null);

    /// <summary>
    /// Updates an existing role assignment.
    /// </summary>
    /// <param name="userRole">The user role assignment to update</param>
    /// <param name="modifiedBy">Who is making the update</param>
    /// <param name="newExpirationDate">New expiration date</param>
    /// <param name="newReason">Updated reason</param>
    /// <param name="makeActive">Whether to activate the assignment</param>
    /// <returns>The updated user role assignment</returns>
    Task<AcademiaUserRole> UpdateRoleAssignmentAsync(
        AcademiaUserRole userRole,
        string? modifiedBy = null,
        DateTime? newExpirationDate = null,
        string? newReason = null,
        bool? makeActive = null);

    /// <summary>
    /// Sets a role assignment as the primary role for a user.
    /// </summary>
    /// <param name="userRole">The user role to set as primary</param>
    /// <param name="modifiedBy">Who is making the change</param>
    /// <returns>True if successful</returns>
    Task<bool> SetPrimaryRoleAsync(AcademiaUserRole userRole, string? modifiedBy = null);

    /// <summary>
    /// Gets all effective roles for a user.
    /// </summary>
    /// <param name="userId">The user ID to get roles for</param>
    /// <returns>List of effective user role assignments</returns>
    Task<IList<AcademiaUserRole>> GetUserEffectiveRolesAsync(int userId);

    /// <summary>
    /// Gets all available roles that can be assigned to a user.
    /// </summary>
    /// <param name="assignerUserId">The ID of the user making the assignment</param>
    /// <param name="targetUserId">The ID of the user receiving the assignment</param>
    /// <returns>List of assignable roles</returns>
    Task<IList<AcademiaRole>> GetAssignableRolesAsync(int assignerUserId, int targetUserId);

    /// <summary>
    /// Validates if a role assignment is allowed.
    /// </summary>
    /// <param name="assignerUserId">The ID of the user making the assignment</param>
    /// <param name="targetUserId">The ID of the user receiving the assignment</param>
    /// <param name="roleId">The ID of the role being assigned</param>
    /// <returns>True if the assignment is valid</returns>
    Task<bool> ValidateRoleAssignmentAsync(int assignerUserId, int targetUserId, int roleId);

    /// <summary>
    /// Creates default system roles if they don't exist.
    /// </summary>
    /// <param name="createdBy">Who is creating the roles</param>
    /// <returns>List of created roles</returns>
    Task<IList<AcademiaRole>> EnsureSystemRolesExistAsync(string? createdBy = null);
}

/// <summary>
/// Implementation of role assignment service for the Zeus Academia System.
/// </summary>
public class RoleAssignmentService : IRoleAssignmentService
{
    private readonly AcademiaDbContext _context;
    private readonly IRoleHierarchyService _roleHierarchyService;

    public RoleAssignmentService(AcademiaDbContext context, IRoleHierarchyService roleHierarchyService)
    {
        _context = context;
        _roleHierarchyService = roleHierarchyService;
    }

    /// <summary>
    /// Automatically assigns roles to a user based on their Academic entity type.
    /// </summary>
    public async Task<IList<AcademiaUserRole>> AssignAutomaticRolesAsync(AcademiaUser user, string? assignedBy = null)
    {
        var assignedRoles = new List<AcademiaUserRole>();

        if (user.Academic == null)
        {
            // No academic record, can't determine automatic roles
            return assignedRoles;
        }

        // Determine role type based on Academic entity type
        var roleType = DetermineRoleTypeFromAcademic(user.Academic);
        if (roleType == null)
        {
            return assignedRoles;
        }

        // Find or create the appropriate role
        var departmentName = GetAcademicDepartmentName(user.Academic);
        var role = await GetOrCreateRoleAsync(roleType.Value, departmentName, assignedBy);

        // Check if user already has this role
        var existingAssignment = user.UserRoles
            .FirstOrDefault(ur => ur.RoleId == role.Id && ur.IsCurrentlyEffective());

        if (existingAssignment == null)
        {
            var userRole = await AssignRoleAsync(
                user,
                role,
                assignedBy ?? "System",
                "Automatic assignment based on Academic record",
                departmentName,
                isPrimary: true);

            assignedRoles.Add(userRole);
        }

        return assignedRoles;
    }

    /// <summary>
    /// Manually assigns a role to a user.
    /// </summary>
    public async Task<AcademiaUserRole> AssignRoleAsync(
        AcademiaUser user,
        AcademiaRole role,
        string? assignedBy = null,
        string? reason = null,
        string? departmentContext = null,
        DateTime? effectiveDate = null,
        DateTime? expirationDate = null,
        bool isPrimary = false)
    {
        // If setting as primary, remove primary from other roles
        if (isPrimary)
        {
            var existingPrimary = user.UserRoles.FirstOrDefault(ur => ur.IsPrimary && ur.IsCurrentlyEffective());
            if (existingPrimary != null)
            {
                existingPrimary.RemovePrimary(assignedBy);
            }
        }

        var userRole = new AcademiaUserRole
        {
            UserId = user.Id,
            RoleId = role.Id,
            EffectiveDate = effectiveDate ?? DateTime.UtcNow,
            ExpirationDate = expirationDate,
            AssignedBy = assignedBy,
            AssignmentReason = reason,
            DepartmentContextName = departmentContext,
            IsPrimary = isPrimary,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow,
            CreatedBy = assignedBy ?? "System",
            ModifiedBy = assignedBy ?? "System"
        };

        _context.UserRoles.Add(userRole);
        await _context.SaveChangesAsync();

        return userRole;
    }

    /// <summary>
    /// Removes a role assignment from a user.
    /// </summary>
    public async Task<bool> RemoveRoleAssignmentAsync(AcademiaUserRole userRole, string? modifiedBy = null, string? reason = null)
    {
        userRole.Deactivate(modifiedBy, reason);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Updates an existing role assignment.
    /// </summary>
    public async Task<AcademiaUserRole> UpdateRoleAssignmentAsync(
        AcademiaUserRole userRole,
        string? modifiedBy = null,
        DateTime? newExpirationDate = null,
        string? newReason = null,
        bool? makeActive = null)
    {
        if (newExpirationDate.HasValue)
        {
            userRole.ExtendAssignment(newExpirationDate, modifiedBy);
        }

        if (!string.IsNullOrEmpty(newReason))
        {
            userRole.AssignmentReason = newReason;
            userRole.ModifiedDate = DateTime.UtcNow;
            userRole.ModifiedBy = modifiedBy ?? userRole.ModifiedBy;
        }

        if (makeActive.HasValue)
        {
            if (makeActive.Value)
            {
                userRole.Activate(modifiedBy);
            }
            else
            {
                userRole.Deactivate(modifiedBy);
            }
        }

        await _context.SaveChangesAsync();
        return userRole;
    }

    /// <summary>
    /// Sets a role assignment as the primary role for a user.
    /// </summary>
    public async Task<bool> SetPrimaryRoleAsync(AcademiaUserRole userRole, string? modifiedBy = null)
    {
        // Remove primary from other roles for this user
        var otherPrimaryRoles = _context.UserRoles
            .Where(ur => ur.UserId == userRole.UserId && ur.Id != userRole.Id && ur.IsPrimary)
            .ToList();

        foreach (var otherRole in otherPrimaryRoles)
        {
            otherRole.RemovePrimary(modifiedBy);
        }

        // Set this role as primary
        userRole.SetAsPrimary(modifiedBy);

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Gets all effective roles for a user.
    /// </summary>
    public async Task<IList<AcademiaUserRole>> GetUserEffectiveRolesAsync(int userId)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Where(ur => ur.IsActive &&
                         ur.EffectiveDate <= DateTime.UtcNow &&
                         (ur.ExpirationDate == null || ur.ExpirationDate > DateTime.UtcNow))
            .Include(ur => ur.Role)
            .Include(ur => ur.DepartmentContext)
            .ToListAsync();
    }

    /// <summary>
    /// Gets all available roles that can be assigned to a user.
    /// </summary>
    public async Task<IList<AcademiaRole>> GetAssignableRolesAsync(int assignerUserId, int targetUserId)
    {
        var assignerUser = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == assignerUserId);

        if (assignerUser == null)
            return new List<AcademiaRole>();

        var assignerHighestRole = _roleHierarchyService.GetUserHighestRole(assignerUser);
        if (assignerHighestRole == null)
            return new List<AcademiaRole>();

        var allRoles = await _context.Roles.Where(r => r.IsActive).ToListAsync();
        return _roleHierarchyService.GetManageableRoles(assignerHighestRole, allRoles).ToList();
    }

    /// <summary>
    /// Validates if a role assignment is allowed.
    /// </summary>
    public async Task<bool> ValidateRoleAssignmentAsync(int assignerUserId, int targetUserId, int roleId)
    {
        var assignerUser = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .Include(u => u.Academic)
            .FirstOrDefaultAsync(u => u.Id == assignerUserId);

        var targetUser = await _context.Users
            .Include(u => u.Academic)
            .FirstOrDefaultAsync(u => u.Id == targetUserId);

        var roleToAssign = await _context.Roles.FirstOrDefaultAsync(r => r.Id == roleId);

        if (assignerUser == null || targetUser == null || roleToAssign == null)
            return false;

        var assignerHighestRole = _roleHierarchyService.GetUserHighestRole(assignerUser);
        if (assignerHighestRole == null)
            return false;

        // Use local validation logic since ValidateRoleAssignment is not in IRoleHierarchyService interface
        return ValidateRoleAssignmentLocal(assignerHighestRole, targetUser, roleToAssign);
    }

    /// <summary>
    /// Creates default system roles if they don't exist.
    /// </summary>
    public async Task<IList<AcademiaRole>> EnsureSystemRolesExistAsync(string? createdBy = null)
    {
        var createdRoles = new List<AcademiaRole>();
        var systemRoleTypes = Enum.GetValues<AcademicRoleType>();

        foreach (var roleType in systemRoleTypes)
        {
            var existingRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleType == roleType && r.IsSystemRole && string.IsNullOrEmpty(r.DepartmentName));

            if (existingRole == null)
            {
                var role = _roleHierarchyService.CreateRole(roleType, null, createdBy ?? "System");
                role.IsSystemRole = true;

                _context.Roles.Add(role);
                createdRoles.Add(role);
            }
        }

        if (createdRoles.Any())
        {
            await _context.SaveChangesAsync();
        }

        return createdRoles;
    }

    /// <summary>
    /// Determines the role type based on the Academic entity type.
    /// </summary>
    private AcademicRoleType? DetermineRoleTypeFromAcademic(Academic academic)
    {
        // This is a simplified determination - in reality, you might need more complex logic
        // based on the specific Academic entity types and business rules

        return academic switch
        {
            Student => AcademicRoleType.Student,
            Professor => AcademicRoleType.Professor,
            TeachingProf => AcademicRoleType.TeachingProf,
            // Add more mappings based on your Academic entity hierarchy
            _ => null
        };
    }

    /// <summary>
    /// Gets an existing role or creates a new one for the specified type and department.
    /// </summary>
    private async Task<AcademiaRole> GetOrCreateRoleAsync(AcademicRoleType roleType, string? departmentName, string? createdBy)
    {
        var existingRole = await _context.Roles
            .FirstOrDefaultAsync(r => r.RoleType == roleType && r.DepartmentName == departmentName);

        if (existingRole != null)
            return existingRole;

        var newRole = _roleHierarchyService.CreateRole(roleType, departmentName, createdBy ?? "System");
        _context.Roles.Add(newRole);
        await _context.SaveChangesAsync();

        return newRole;
    }

    /// <summary>
    /// Gets the department name from an Academic entity based on its specific type.
    /// </summary>
    private string? GetAcademicDepartmentName(Academic academic)
    {
        return academic switch
        {
            Professor professor => professor.DepartmentName,
            TeachingProf teachingProf => teachingProf.DepartmentName,
            Student student => student.DepartmentName,
            // Add other Academic types as needed
            _ => null
        };
    }

    /// <summary>
    /// Local validation logic for role assignments.
    /// </summary>
    private bool ValidateRoleAssignmentLocal(AcademiaRole assignerRole, AcademiaUser targetUser, AcademiaRole roleToAssign)
    {
        // Check if the assigner can manage the role being assigned
        if (!_roleHierarchyService.CanManageRole(assignerRole, roleToAssign))
            return false;

        // Check if the target user is active
        if (!targetUser.IsActive)
            return false;

        // Business rule: Students should have Academic records
        if (roleToAssign.RoleType == AcademicRoleType.Student && targetUser.Academic == null)
            return false;

        // Business rule: Professor/Chair roles should have Academic records
        if ((roleToAssign.RoleType == AcademicRoleType.Professor ||
             roleToAssign.RoleType == AcademicRoleType.Chair ||
             roleToAssign.RoleType == AcademicRoleType.TeachingProf) &&
            targetUser.Academic == null)
            return false;

        return true;
    }
}