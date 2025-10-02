namespace Zeus.Academia.Infrastructure.Enums;

/// <summary>
/// Enumeration for student academic standing
/// </summary>
public enum AcademicStanding
{
    /// <summary>
    /// Good academic standing with satisfactory progress
    /// </summary>
    Good = 0,

    /// <summary>
    /// Student is on academic probation due to low GPA or other issues
    /// </summary>
    Probation = 1,

    /// <summary>
    /// Student has been suspended for academic reasons
    /// </summary>
    AcademicSuspension = 2,

    /// <summary>
    /// Student is on academic warning (between good and probation)
    /// </summary>
    Warning = 3,

    /// <summary>
    /// Student has achieved dean's list recognition
    /// </summary>
    DeansListqualification = 4,

    /// <summary>
    /// Student has achieved president's list recognition
    /// </summary>
    PresidentsListqualification = 5,

    /// <summary>
    /// Student has been dismissed for academic reasons
    /// </summary>
    AcademicDismissal = 6,

    /// <summary>
    /// New student status (no academic history yet)
    /// </summary>
    NewStudent = 7
}