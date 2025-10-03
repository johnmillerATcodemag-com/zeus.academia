using Zeus.Academia.Infrastructure.Enums;

namespace Zeus.Academia.Infrastructure.Models;

/// <summary>
/// Model representing a student's academic profile for course planning services.
/// </summary>
public class StudentAcademicProfile
{
    public int StudentId { get; set; }
    public string MajorCode { get; set; } = string.Empty;
    public DateTime StartingSemester { get; set; }
    public DateTime ExpectedGraduationDate { get; set; }
    public DateTime CurrentSemester { get; set; }
    public CourseLoadPreference PreferredCourseLoad { get; set; }
    public bool SummerAvailability { get; set; }
    public int MaxDifficultCoursesPerSemester { get; set; } = 2;
    public List<int> CompletedCourses { get; set; } = new();
}

/// <summary>
/// Model representing a student's academic record for graduation requirements.
/// </summary>
public class StudentAcademicRecord
{
    public int StudentId { get; set; }
    public decimal TotalCreditsEarned { get; set; }
    public decimal OverallGPA { get; set; }
    public decimal MajorGPA { get; set; }
    public int SummerCoursesTaken { get; set; }
    public decimal ResidencyCredits { get; set; }
    public DateTime StartDate { get; set; }
    public List<int> CompletedRequiredCourses { get; set; } = new();
    public string LowestMajorGrade { get; set; } = string.Empty;
    public bool CapstoneCompleted { get; set; }
    public List<int> FailedCourseRetakes { get; set; } = new();
    public bool GraduationApplicationSubmitted { get; set; }
    public DateTime? ApplicationSubmissionDate { get; set; }
    public int CurrentSemesterLoad { get; set; }
}