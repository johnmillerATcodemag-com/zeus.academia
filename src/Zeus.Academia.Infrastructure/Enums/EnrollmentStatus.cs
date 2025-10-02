namespace Zeus.Academia.Infrastructure.Enums;

/// <summary>
/// Enumeration for student enrollment status
/// </summary>
public enum EnrollmentStatus
{
    /// <summary>
    /// Student has applied but not yet admitted
    /// </summary>
    Applied = 0,

    /// <summary>
    /// Student has been admitted but not yet enrolled
    /// </summary>
    Admitted = 1,

    /// <summary>
    /// Student is currently enrolled and active
    /// </summary>
    Enrolled = 2,

    /// <summary>
    /// Student has temporarily suspended their studies
    /// </summary>
    Suspended = 3,

    /// <summary>
    /// Student has withdrawn from their program
    /// </summary>
    Withdrawn = 4,

    /// <summary>
    /// Student has completed their program and graduated
    /// </summary>
    Graduated = 5,

    /// <summary>
    /// Student has been dismissed for academic or disciplinary reasons
    /// </summary>
    Dismissed = 6,

    /// <summary>
    /// Student is on leave of absence
    /// </summary>
    LeaveOfAbsence = 7
}