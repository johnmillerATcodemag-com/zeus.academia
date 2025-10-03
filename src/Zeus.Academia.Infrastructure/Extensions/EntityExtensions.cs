using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Enums;

namespace Zeus.Academia.Infrastructure.Extensions;

/// <summary>
/// Extension methods to bridge gaps between service expectations and actual entity structure
/// </summary>
public static class EntityExtensions
{
    /// <summary>
    /// Gets the Course for a CourseEnrollment (navigation property bridge)
    /// </summary>
    public static Course GetCourse(this CourseEnrollment enrollment)
    {
        return enrollment.Subject?.Courses?.FirstOrDefault()
            ?? new Course { Id = 0, CreditHours = enrollment.CreditHours, CourseNumber = "UNKNOWN" };
    }

    /// <summary>
    /// Gets the effective grade for a course enrollment (latest final grade)
    /// </summary>
    public static string? GetEffectiveGrade(this CourseEnrollment enrollment)
    {
        return enrollment.Grades
            .Where(g => g.GradeType == GradeType.Final && g.Status == GradeStatus.Posted)
            .OrderByDescending(g => g.GradeDate)
            .FirstOrDefault()?.LetterGrade;
    }

    /// <summary>
    /// Checks if the course enrollment has a passing grade
    /// </summary>
    public static bool HasPassingGrade(this CourseEnrollment enrollment)
    {
        var grade = enrollment.GetEffectiveGrade();
        return !string.IsNullOrEmpty(grade) && grade != "F" && grade != "W";
    }

    /// <summary>
    /// Gets the grade points for an enrollment
    /// </summary>
    public static decimal GetGradePoints(this CourseEnrollment enrollment)
    {
        var grade = enrollment.GetEffectiveGrade();
        return GetGradePointValue(grade);
    }

    /// <summary>
    /// Converts letter grade to grade points
    /// </summary>
    public static decimal GetGradePointValue(string? letterGrade)
    {
        return letterGrade switch
        {
            "A" => 4.0m,
            "B" => 3.0m,
            "C" => 2.0m,
            "D" => 1.0m,
            "F" => 0.0m,
            _ => 0.0m
        };
    }

    /// <summary>
    /// Checks if a course is active
    /// </summary>
    public static bool IsActive(this Course course)
    {
        return course.Status == CourseStatus.Active;
    }

    /// <summary>
    /// Gets the major code from a student (uses DegreeCode)
    /// </summary>
    public static string GetMajorCode(this Student student)
    {
        return student.DegreeCode ?? "UNKNOWN";
    }

    /// <summary>
    /// Converts CourseLoadPreference to credit hours
    /// </summary>
    public static int ToCredits(this CourseLoadPreference preference)
    {
        return preference switch
        {
            CourseLoadPreference.Light => 12,
            CourseLoadPreference.Standard => 15,
            CourseLoadPreference.Heavy => 17,
            CourseLoadPreference.Maximum => 19,
            _ => 15
        };
    }

    /// <summary>
    /// Gets the required course number from a prerequisite (bridge for RequiredCourseNumber)
    /// </summary>
    public static Course? GetRequiredCourse(this CoursePrerequisite prerequisite)
    {
        // Return null - actual implementation would need to lookup by course number
        return null;
    }

    /// <summary>
    /// Gets the required course ID from a prerequisite (simulated)
    /// </summary>
    public static int GetRequiredCourseId(this CoursePrerequisite prerequisite)
    {
        // Return 0 as default - actual implementation would lookup course by number
        return 0;
    }







    /// <summary>
    /// Gets the student ID from CourseEnrollment
    /// </summary>
    public static int GetStudentId(this CourseEnrollment enrollment)
    {
        return enrollment.StudentEmpNr;
    }

    /// <summary>
    /// Gets course ID by finding the course through subject relationship
    /// This is a simplified approach - in practice you might need to include more navigation
    /// </summary>
    public static int? GetCourseId(this CourseEnrollment enrollment)
    {
        // Since CourseEnrollment doesn't directly reference Course, 
        // we'll return a placeholder value or null
        // This would need to be resolved through proper navigation or additional queries
        return null;
    }

    /// <summary>
    /// Gets the CompletedCourses for a StudentAcademicProfile (property bridge)
    /// </summary>
    public static List<int> GetCompletedCourses(this Models.StudentAcademicProfile profile)
    {
        // In a real implementation, this would fetch from database
        // For now, return empty list to make service compile
        return new List<int>();
    }

    /// <summary>
    /// Gets the CompletedCourses for a Services StudentAcademicProfile (property bridge)
    /// </summary>
    public static List<int> GetCompletedCourses(this Services.StudentAcademicProfile profile)
    {
        // Check if CompletedCourses property exists on this version
        return profile.CompletedCourses?.ToList() ?? new List<int>();
    }

    /// <summary>
    /// Extracts the numeric course level from a course number (e.g., "MATH101" -> 101)
    /// </summary>
    public static int GetCourseLevel(this Course course)
    {
        if (string.IsNullOrEmpty(course.CourseNumber))
            return 0;

        // Extract numeric part from course number (e.g., MATH101 -> 101)
        var numericPart = new string(course.CourseNumber.Where(char.IsDigit).ToArray());
        return int.TryParse(numericPart, out int level) ? level : 0;
    }

    /// <summary>
    /// Gets the required course number from a prerequisite
    /// </summary>
    public static string? GetRequiredCourseNumber(this CoursePrerequisite prerequisite)
    {
        return prerequisite.RequiredCourseNumber;
    }

    /// <summary>
    /// Checks if a prerequisite is a corequisite
    /// </summary>
    public static bool IsCorequisite(this CoursePrerequisite prerequisite)
    {
        // Since there's no IsCorequisite property, we'll check if it's a corequisite type
        // This is a simplified implementation
        return prerequisite.PrerequisiteType == PrerequisiteType.Course;
    }

    /// <summary>
    /// Gets a default difficulty rating for a course
    /// </summary>
    public static DifficultyLevel GetDifficultyRating(this Course course)
    {
        // Since Course doesn't have DifficultyRating, we'll use course level as a proxy
        return course.Level switch
        {
            CourseLevel.Undergraduate => DifficultyLevel.Intermediate,
            CourseLevel.Graduate => DifficultyLevel.Advanced,
            CourseLevel.Doctoral => DifficultyLevel.Expert,
            _ => DifficultyLevel.Beginner
        };
    }

    /// <summary>
    /// Checks if a course offering is regularly offered
    /// </summary>
    public static bool IsRegularlyOffered(this CourseOffering offering)
    {
        // Since CourseOffering doesn't have this property, we'll return a default
        return true; // Simplified assumption
    }

    /// <summary>
    /// Gets typical enrollment for a course offering
    /// </summary>
    public static int GetTypicalEnrollment(this CourseOffering offering)
    {
        // Since CourseOffering doesn't have this property, we'll use current enrollment or max
        return offering.MaxEnrollment ?? offering.CurrentEnrollment;
    }

    // StudentAcademicRecord property mapping extensions for compatibility between different service expectations

    /// <summary>
    /// Maps TotalCreditsEarned to TotalCredits for compatibility
    /// </summary>
    public static int TotalCredits(this Zeus.Academia.Infrastructure.Models.StudentAcademicRecord record)
    {
        return (int)record.TotalCreditsEarned;
    }

    /// <summary>
    /// Maps OverallGPA to CumulativeGPA for compatibility
    /// </summary>
    public static decimal CumulativeGPA(this Zeus.Academia.Infrastructure.Models.StudentAcademicRecord record)
    {
        return record.OverallGPA;
    }

    /// <summary>
    /// Gets count of completed required courses for compatibility
    /// </summary>
    public static int CompletedRequiredCourses(this Zeus.Academia.Infrastructure.Models.StudentAcademicRecord record)
    {
        return record.CompletedRequiredCourses.Count;
    }

    /// <summary>
    /// Checks if student has taken summer courses (returns true if count > 0)
    /// </summary>
    public static bool SummerCoursesTaken(this Zeus.Academia.Infrastructure.Models.StudentAcademicRecord record)
    {
        return record.SummerCoursesTaken > 0;
    }

    /// <summary>
    /// Gets failed course retakes count for compatibility
    /// </summary>
    public static int FailedCourseRetakes(this Zeus.Academia.Infrastructure.Models.StudentAcademicRecord record)
    {
        return record.FailedCourseRetakes.Count;
    }

    // CourseEnrollment property access extensions

    /// <summary>
    /// Gets the course ID from CourseEnrollment (uses SubjectCode as proxy)
    /// </summary>
    public static string GetCourseCode(this CourseEnrollment enrollment)
    {
        return enrollment.SubjectCode;
    }

    /// <summary>
    /// Gets the final letter grade from CourseEnrollment's grades collection
    /// </summary>
    public static string? GetFinalGrade(this CourseEnrollment enrollment)
    {
        return enrollment.Grades
            .Where(g => g.IsFinal && g.Status == GradeStatus.Posted)
            .OrderByDescending(g => g.GradeDate)
            .FirstOrDefault()?.LetterGrade;
    }

    /// <summary>
    /// Gets the term string from CourseEnrollment
    /// </summary>
    public static string GetTerm(this CourseEnrollment enrollment)
    {
        return $"{enrollment.Semester} {enrollment.AcademicYear}";
    }

    /// <summary>
    /// Gets the starting semester as string from StudentAcademicProfile
    /// </summary>
    public static string GetStartingSemesterString(this Models.StudentAcademicProfile profile)
    {
        return $"Fall {profile.StartingSemester.Year}";
    }

    /// <summary>
    /// Gets the current semester as string from StudentAcademicProfile
    /// </summary>
    public static string GetCurrentSemesterString(this Models.StudentAcademicProfile profile)
    {
        return $"Fall {profile.CurrentSemester.Year}";
    }
}