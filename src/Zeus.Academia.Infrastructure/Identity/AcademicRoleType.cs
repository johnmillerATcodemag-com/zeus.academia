namespace Zeus.Academia.Infrastructure.Identity;

/// <summary>
/// Defines the different types of roles available in the Zeus Academia System.
/// These roles correspond to the academic hierarchy and administrative functions.
/// </summary>
public enum AcademicRoleType
{
    /// <summary>
    /// Student role - Basic user with limited access to course materials and assignments.
    /// Can view enrolled courses, submit assignments, access grades.
    /// </summary>
    Student = 1,

    /// <summary>
    /// Professor role - Faculty member with teaching and research responsibilities.
    /// Can manage courses, grade assignments, access student records for their classes.
    /// </summary>
    Professor = 2,

    /// <summary>
    /// Teaching Professor role - Faculty focused primarily on teaching.
    /// Similar to Professor but may have different research/administrative access.
    /// </summary>
    TeachingProf = 3,

    /// <summary>
    /// Department Chair role - Administrative leader of an academic department.
    /// Can manage department faculty, courses, budgets, and strategic planning.
    /// Inherits Professor permissions plus department administration.
    /// </summary>
    Chair = 4,

    /// <summary>
    /// Administrator role - Non-academic administrative staff.
    /// Can manage academic records, enrollment, scheduling, and student services.
    /// </summary>
    Administrator = 5,

    /// <summary>
    /// System Administrator role - Technical administrative role.
    /// Full system access for maintenance, user management, and system configuration.
    /// Highest level of access in the system.
    /// </summary>
    SystemAdmin = 6
}

/// <summary>
/// Extension methods for AcademicRoleType enum to provide additional functionality.
/// </summary>
public static class AcademicRoleTypeExtensions
{
    /// <summary>
    /// Gets the display name for the role type.
    /// </summary>
    /// <param name="roleType">The role type to get the display name for</param>
    /// <returns>A user-friendly display name for the role</returns>
    public static string GetDisplayName(this AcademicRoleType roleType)
    {
        return roleType switch
        {
            AcademicRoleType.Student => "Student",
            AcademicRoleType.Professor => "Professor",
            AcademicRoleType.TeachingProf => "Teaching Professor",
            AcademicRoleType.Chair => "Department Chair",
            AcademicRoleType.Administrator => "Administrator",
            AcademicRoleType.SystemAdmin => "System Administrator",
            _ => roleType.ToString()
        };
    }

    /// <summary>
    /// Gets the description for the role type.
    /// </summary>
    /// <param name="roleType">The role type to get the description for</param>
    /// <returns>A description of the role's responsibilities</returns>
    public static string GetDescription(this AcademicRoleType roleType)
    {
        return roleType switch
        {
            AcademicRoleType.Student => "Access to enrolled courses, assignments, and grades",
            AcademicRoleType.Professor => "Teaching, research, and course management capabilities",
            AcademicRoleType.TeachingProf => "Primary focus on teaching and course delivery",
            AcademicRoleType.Chair => "Department leadership and administrative oversight",
            AcademicRoleType.Administrator => "Academic records and student services management",
            AcademicRoleType.SystemAdmin => "Full system access and technical administration",
            _ => "Custom role with specific permissions"
        };
    }

    /// <summary>
    /// Gets the priority level of the role (higher number = higher priority/authority).
    /// Used for role hierarchy determination.
    /// </summary>
    /// <param name="roleType">The role type to get the priority for</param>
    /// <returns>Priority level from 1 (lowest) to 10 (highest)</returns>
    public static int GetPriority(this AcademicRoleType roleType)
    {
        return roleType switch
        {
            AcademicRoleType.Student => 1,
            AcademicRoleType.Professor => 5,
            AcademicRoleType.TeachingProf => 4,
            AcademicRoleType.Chair => 7,
            AcademicRoleType.Administrator => 6,
            AcademicRoleType.SystemAdmin => 10,
            _ => 1
        };
    }

    /// <summary>
    /// Determines if this role can manage users with the target role.
    /// </summary>
    /// <param name="currentRole">The current user's role</param>
    /// <param name="targetRole">The role of the user being managed</param>
    /// <returns>True if the current role can manage the target role</returns>
    public static bool CanManage(this AcademicRoleType currentRole, AcademicRoleType targetRole)
    {
        return currentRole.GetPriority() > targetRole.GetPriority();
    }
}