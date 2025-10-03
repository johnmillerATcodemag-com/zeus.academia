namespace Zeus.Academia.Infrastructure.Services;

/// <summary>
/// Supporting classes for course recommendation system
/// </summary>
public class StudentCourseHistory
{
    public int CourseId { get; set; }
    public string SubjectCode { get; set; } = string.Empty;
    public string Grade { get; set; } = string.Empty;
    public decimal GradePoints { get; set; }
    public string Semester { get; set; } = string.Empty;
    public int AcademicYear { get; set; }
}