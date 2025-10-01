namespace Zeus.Academia.Infrastructure.Identity;

/// <summary>
/// Marker interface for authorization requirements in the Zeus Academia System.
/// This will be mapped to IAuthorizationRequirement in the API layer.
/// </summary>
public interface IAcademiaAuthorizationRequirement
{
}

/// <summary>
/// Contains constants for authorization policy names used throughout the Zeus Academia System.
/// These policies define access control rules based on roles and permissions.
/// </summary>
public static class AcademiaPolicyNames
{
    // Role-based policies
    public const string RequireStudentRole = "RequireStudentRole";
    public const string RequireProfessorRole = "RequireProfessorRole";
    public const string RequireTeachingProfRole = "RequireTeachingProfRole";
    public const string RequireChairRole = "RequireChairRole";
    public const string RequireAdministratorRole = "RequireAdministratorRole";
    public const string RequireSystemAdminRole = "RequireSystemAdminRole";

    // Permission-based policies
    public const string CanManageUsers = "CanManageUsers";
    public const string CanViewAcademicRecords = "CanViewAcademicRecords";
    public const string CanEditAcademicRecords = "CanEditAcademicRecords";
    public const string CanManageCourses = "CanManageCourses";
    public const string CanAssignGrades = "CanAssignGrades";
    public const string CanManageDepartments = "CanManageDepartments";
    public const string CanManageResearch = "CanManageResearch";
    public const string CanViewReports = "CanViewReports";
    public const string CanManageSystem = "CanManageSystem";

    // Hierarchical policies
    public const string FacultyOrHigher = "FacultyOrHigher";
    public const string AdministrativeStaff = "AdministrativeStaff";
    public const string DepartmentLeadership = "DepartmentLeadership";
    public const string AcademicStaff = "AcademicStaff";

    // Department-specific policies
    public const string SameDepartmentAccess = "SameDepartmentAccess";
    public const string DepartmentAdminAccess = "DepartmentAdminAccess";

    // Resource-based policies
    public const string CanAccessOwnData = "CanAccessOwnData";
    public const string CanModifyOwnProfile = "CanModifyOwnProfile";
    public const string CanViewOwnGrades = "CanViewOwnGrades";

    // Combined policies
    public const string TeachingStaff = "TeachingStaff";
    public const string ResearchStaff = "ResearchStaff";
    public const string ManagementStaff = "ManagementStaff";
}

/// <summary>
/// Contains authorization requirement classes for custom authorization policies.
/// </summary>
public static class AcademiaRequirements
{
    /// <summary>
    /// Requirement for role-based authorization.
    /// </summary>
    public class RoleRequirement : IAcademiaAuthorizationRequirement
    {
        public AcademicRoleType RequiredRole { get; }
        public bool AllowHigherRoles { get; }

        public RoleRequirement(AcademicRoleType requiredRole, bool allowHigherRoles = true)
        {
            RequiredRole = requiredRole;
            AllowHigherRoles = allowHigherRoles;
        }
    }

    /// <summary>
    /// Requirement for permission-based authorization.
    /// </summary>
    public class PermissionRequirement : IAcademiaAuthorizationRequirement
    {
        public AcademiaPermission RequiredPermission { get; }
        public bool RequireAll { get; }

        public PermissionRequirement(AcademiaPermission requiredPermission, bool requireAll = false)
        {
            RequiredPermission = requiredPermission;
            RequireAll = requireAll;
        }
    }

    /// <summary>
    /// Requirement for department-based authorization.
    /// </summary>
    public class DepartmentRequirement : IAcademiaAuthorizationRequirement
    {
        public string? DepartmentName { get; }
        public bool AllowSystemAdmin { get; }

        public DepartmentRequirement(string? departmentName = null, bool allowSystemAdmin = true)
        {
            DepartmentName = departmentName;
            AllowSystemAdmin = allowSystemAdmin;
        }
    }

    /// <summary>
    /// Requirement for resource ownership authorization.
    /// </summary>
    public class ResourceOwnershipRequirement : IAcademiaAuthorizationRequirement
    {
        public bool AllowDepartmentAccess { get; }
        public bool AllowAdminOverride { get; }

        public ResourceOwnershipRequirement(bool allowDepartmentAccess = false, bool allowAdminOverride = true)
        {
            AllowDepartmentAccess = allowDepartmentAccess;
            AllowAdminOverride = allowAdminOverride;
        }
    }

    /// <summary>
    /// Requirement for minimum role priority authorization.
    /// </summary>
    public class MinimumRolePriorityRequirement : IAcademiaAuthorizationRequirement
    {
        public int MinimumPriority { get; }

        public MinimumRolePriorityRequirement(int minimumPriority)
        {
            MinimumPriority = minimumPriority;
        }
    }
}