using System.ComponentModel;

namespace Zeus.Academia.Infrastructure.Identity;

/// <summary>
/// Defines the comprehensive set of permissions available in the Zeus Academia System.
/// Permissions are organized by functional area and can be combined to create roles.
/// </summary>
[Flags]
public enum AcademiaPermission : long
{
    // No permissions
    None = 0,

    // User Management Permissions (1-99)
    [Description("View user profiles")]
    ViewUsers = 1L << 0,

    [Description("Create new user accounts")]
    CreateUsers = 1L << 1,

    [Description("Edit user profiles")]
    EditUsers = 1L << 2,

    [Description("Delete user accounts")]
    DeleteUsers = 1L << 3,

    [Description("Assign roles to users")]
    AssignRoles = 1L << 4,

    [Description("Reset user passwords")]
    ResetPasswords = 1L << 5,

    [Description("Lock/unlock user accounts")]
    ManageUserLockouts = 1L << 6,

    // Academic Records Permissions (100-199)
    [Description("View academic records")]
    ViewAcademicRecords = 1L << 7,

    [Description("Edit academic records")]
    EditAcademicRecords = 1L << 8,

    [Description("Create academic records")]
    CreateAcademicRecords = 1L << 9,

    [Description("Delete academic records")]
    DeleteAcademicRecords = 1L << 10,

    [Description("View student grades")]
    ViewGrades = 1L << 11,

    [Description("Assign grades to students")]
    AssignGrades = 1L << 12,

    [Description("Modify existing grades")]
    EditGrades = 1L << 13,

    // Course Management Permissions (200-299)
    [Description("View course information")]
    ViewCourses = 1L << 14,

    [Description("Create new courses")]
    CreateCourses = 1L << 15,

    [Description("Edit course details")]
    EditCourses = 1L << 16,

    [Description("Delete courses")]
    DeleteCourses = 1L << 17,

    [Description("Assign instructors to courses")]
    AssignInstructors = 1L << 18,

    [Description("Manage course enrollment")]
    ManageEnrollment = 1L << 19,

    [Description("View course schedules")]
    ViewSchedules = 1L << 20,

    [Description("Edit course schedules")]
    EditSchedules = 1L << 21,

    // Department Management Permissions (300-399)
    [Description("View department information")]
    ViewDepartments = 1L << 22,

    [Description("Edit department details")]
    EditDepartments = 1L << 23,

    [Description("Create new departments")]
    CreateDepartments = 1L << 24,

    [Description("Delete departments")]
    DeleteDepartments = 1L << 25,

    [Description("Manage department faculty")]
    ManageDepartmentFaculty = 1L << 26,

    [Description("Manage department budget")]
    ManageDepartmentBudget = 1L << 27,

    // Research Permissions (400-499)
    [Description("View research projects")]
    ViewResearch = 1L << 28,

    [Description("Create research projects")]
    CreateResearch = 1L << 29,

    [Description("Edit research projects")]
    EditResearch = 1L << 30,

    [Description("Delete research projects")]
    DeleteResearch = 1L << 31,

    [Description("Manage research funding")]
    ManageResearchFunding = 1L << 32,

    // Committee Permissions (500-599)
    [Description("View committee information")]
    ViewCommittees = 1L << 33,

    [Description("Create committees")]
    CreateCommittees = 1L << 34,

    [Description("Edit committee details")]
    EditCommittees = 1L << 35,

    [Description("Delete committees")]
    DeleteCommittees = 1L << 36,

    [Description("Manage committee membership")]
    ManageCommitteeMembers = 1L << 37,

    // Infrastructure Permissions (600-699)
    [Description("Manage building and room assignments")]
    ManageInfrastructure = 1L << 38,

    [Description("Manage access levels")]
    ManageAccessLevels = 1L << 39,

    [Description("View system reports")]
    ViewReports = 1L << 40,

    [Description("Generate system reports")]
    GenerateReports = 1L << 41,

    // System Administration Permissions (700-799)
    [Description("Access system configuration")]
    SystemConfiguration = 1L << 42,

    [Description("View system logs")]
    ViewSystemLogs = 1L << 43,

    [Description("Manage system backup")]
    ManageBackup = 1L << 44,

    [Description("Perform system maintenance")]
    SystemMaintenance = 1L << 45,

    [Description("Full system administration")]
    FullSystemAdmin = 1L << 46,

    // Personal Data Permissions (800-899)
    [Description("View own profile")]
    ViewOwnProfile = 1L << 47,

    [Description("Edit own profile")]
    EditOwnProfile = 1L << 48,

    [Description("View own grades")]
    ViewOwnGrades = 1L << 49,

    [Description("View own courses")]
    ViewOwnCourses = 1L << 50,

    // Special Permissions (900-999)
    [Description("Impersonate other users")]
    ImpersonateUsers = 1L << 51,

    [Description("Override system restrictions")]
    OverrideRestrictions = 1L << 52,

    [Description("Access audit trails")]
    ViewAuditTrails = 1L << 53,

    // Permission Groups for Convenience
    AllUserManagement = ViewUsers | CreateUsers | EditUsers | DeleteUsers | AssignRoles | ResetPasswords | ManageUserLockouts,
    AllAcademicRecords = ViewAcademicRecords | EditAcademicRecords | CreateAcademicRecords | DeleteAcademicRecords | ViewGrades | AssignGrades | EditGrades,
    AllCourseManagement = ViewCourses | CreateCourses | EditCourses | DeleteCourses | AssignInstructors | ManageEnrollment | ViewSchedules | EditSchedules,
    AllDepartmentManagement = ViewDepartments | EditDepartments | CreateDepartments | DeleteDepartments | ManageDepartmentFaculty | ManageDepartmentBudget,
    AllResearchManagement = ViewResearch | CreateResearch | EditResearch | DeleteResearch | ManageResearchFunding,
    AllCommitteeManagement = ViewCommittees | CreateCommittees | EditCommittees | DeleteCommittees | ManageCommitteeMembers,
    AllInfrastructure = ManageInfrastructure | ManageAccessLevels | ViewReports | GenerateReports,
    AllSystemAdmin = SystemConfiguration | ViewSystemLogs | ManageBackup | SystemMaintenance | FullSystemAdmin,
    AllPersonal = ViewOwnProfile | EditOwnProfile | ViewOwnGrades | ViewOwnCourses,

    // Role-based Permission Sets
    StudentPermissions = AllPersonal | ViewCourses | ViewSchedules,
    ProfessorPermissions = StudentPermissions | AllCourseManagement | ViewAcademicRecords | AssignGrades | EditGrades | ViewResearch | CreateResearch | EditResearch,
    TeachingProfPermissions = StudentPermissions | ViewCourses | EditCourses | AssignGrades | EditGrades | ViewSchedules | EditSchedules | ManageEnrollment,
    ChairPermissions = ProfessorPermissions | AllDepartmentManagement | AllCommitteeManagement | ViewUsers | EditUsers | AssignRoles,
    AdministratorPermissions = AllAcademicRecords | AllCourseManagement | ViewUsers | EditUsers | ViewDepartments | ManageEnrollment | AllInfrastructure,
    SystemAdminPermissions = AllUserManagement | AllAcademicRecords | AllCourseManagement | AllDepartmentManagement | AllResearchManagement | AllCommitteeManagement | AllInfrastructure | AllSystemAdmin | ImpersonateUsers | OverrideRestrictions | ViewAuditTrails
}

/// <summary>
/// Extension methods for AcademiaPermission enum to provide additional functionality.
/// </summary>
public static class AcademiaPermissionExtensions
{
    /// <summary>
    /// Gets the description attribute value for a permission.
    /// </summary>
    /// <param name="permission">The permission to get the description for</param>
    /// <returns>The description of the permission</returns>
    public static string GetDescription(this AcademiaPermission permission)
    {
        var field = permission.GetType().GetField(permission.ToString());
        if (field == null) return permission.ToString();

        var attribute = (DescriptionAttribute?)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
        return attribute?.Description ?? permission.ToString();
    }

    /// <summary>
    /// Checks if the current permissions include the specified permission.
    /// </summary>
    /// <param name="currentPermissions">The current permission flags</param>
    /// <param name="requiredPermission">The permission to check for</param>
    /// <returns>True if the required permission is granted</returns>
    public static bool HasPermission(this AcademiaPermission currentPermissions, AcademiaPermission requiredPermission)
    {
        return (currentPermissions & requiredPermission) == requiredPermission;
    }

    /// <summary>
    /// Checks if the current permissions include any of the specified permissions.
    /// </summary>
    /// <param name="currentPermissions">The current permission flags</param>
    /// <param name="requiredPermissions">The permissions to check for</param>
    /// <returns>True if any of the required permissions are granted</returns>
    public static bool HasAnyPermission(this AcademiaPermission currentPermissions, params AcademiaPermission[] requiredPermissions)
    {
        return requiredPermissions.Any(permission => currentPermissions.HasPermission(permission));
    }

    /// <summary>
    /// Gets all individual permissions from a combined permission flag.
    /// </summary>
    /// <param name="permissions">The combined permissions</param>
    /// <returns>List of individual permissions</returns>
    public static IEnumerable<AcademiaPermission> GetIndividualPermissions(this AcademiaPermission permissions)
    {
        return Enum.GetValues<AcademiaPermission>()
            .Where(p => p != AcademiaPermission.None && permissions.HasPermission(p))
            .Where(p => IsSinglePermission(p));
    }

    /// <summary>
    /// Gets the default permissions for a specific role type.
    /// </summary>
    /// <param name="roleType">The role type to get permissions for</param>
    /// <returns>The default permissions for the role</returns>
    public static AcademiaPermission GetDefaultPermissions(this AcademicRoleType roleType)
    {
        return roleType switch
        {
            AcademicRoleType.Student => AcademiaPermission.StudentPermissions,
            AcademicRoleType.Professor => AcademiaPermission.ProfessorPermissions,
            AcademicRoleType.TeachingProf => AcademiaPermission.TeachingProfPermissions,
            AcademicRoleType.Chair => AcademiaPermission.ChairPermissions,
            AcademicRoleType.Administrator => AcademiaPermission.AdministratorPermissions,
            AcademicRoleType.SystemAdmin => AcademiaPermission.SystemAdminPermissions,
            _ => AcademiaPermission.None
        };
    }

    /// <summary>
    /// Determines if a permission represents a single permission (not a combination).
    /// </summary>
    /// <param name="permission">The permission to check</param>
    /// <returns>True if it's a single permission</returns>
    private static bool IsSinglePermission(AcademiaPermission permission)
    {
        // Check if the permission is a power of 2 (single bit set)
        return permission != AcademiaPermission.None && (((long)permission & ((long)permission - 1)) == 0);
    }
}