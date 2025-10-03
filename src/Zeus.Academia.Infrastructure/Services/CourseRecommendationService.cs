using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Services;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Enums;

namespace Zeus.Academia.Infrastructure.Services;

/// <summary>
/// Course recommendation service implementation
/// </summary>
public class CourseRecommendationService : ICourseRecommendationService
{
    private readonly AcademiaDbContext _context;
    private readonly ILogger<CourseRecommendationService> _logger;

    public CourseRecommendationService(AcademiaDbContext context, ILogger<CourseRecommendationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<CourseRecommendation>> RecommendCoursesAsync(StudentAcademicProfile studentProfile)
    {
        try
        {
            _logger.LogInformation("Generating course recommendations for student {StudentId}", studentProfile.StudentId);

            var recommendations = new List<CourseRecommendation>();

            // Get available courses excluding already completed ones
            var availableCourses = await GetAvailableCoursesAsync(studentProfile);

            // Apply different recommendation strategies
            var degreeRecommendations = await GetDegreeBasedRecommendations(studentProfile, availableCourses);
            var interestRecommendations = await GetInterestBasedRecommendations(studentProfile, availableCourses);
            var performanceRecommendations = await GetPerformanceBasedRecommendations(studentProfile, availableCourses);
            var sequenceRecommendations = await GetSequenceBasedRecommendations(studentProfile, availableCourses);

            recommendations.AddRange(degreeRecommendations);
            recommendations.AddRange(interestRecommendations);
            recommendations.AddRange(performanceRecommendations);
            recommendations.AddRange(sequenceRecommendations);

            // Merge and rank recommendations
            var finalRecommendations = MergeAndRankRecommendations(recommendations);

            _logger.LogInformation("Generated {Count} course recommendations for student {StudentId}",
                finalRecommendations.Count, studentProfile.StudentId);

            return finalRecommendations.Take(10).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating course recommendations for student {StudentId}", studentProfile.StudentId);
            throw;
        }
    }

    public async Task<List<CourseRecommendation>> RecommendCoursesForDegreeAsync(
        StudentAcademicProfile studentProfile,
        DegreeRequirements degreeRequirements)
    {
        try
        {
            _logger.LogInformation("Generating degree-specific recommendations for student {StudentId} and degree {DegreeCode}",
                studentProfile.StudentId, studentProfile.DegreeCode);

            var recommendations = new List<CourseRecommendation>();
            var completedCourses = studentProfile.CompletedCourses?.ToHashSet() ?? new HashSet<int>();

            // Required courses not yet completed
            var missingRequiredCourses = degreeRequirements.RequiredCourses
                .Where(courseId => !completedCourses.Contains(courseId))
                .ToList();

            if (missingRequiredCourses.Any())
            {
                var requiredCourses = await _context.Courses
                    .Where(c => missingRequiredCourses.Contains(c.Id))
                    .Include(c => c.Prerequisites)
                    .Include(c => c.Subject)
                    .ToListAsync();

                foreach (var course in requiredCourses)
                {
                    var eligibilityScore = CalculatePrerequisiteEligibility(course, completedCourses);

                    recommendations.Add(new CourseRecommendation
                    {
                        Course = course,
                        RecommendationScore = 0.9f * eligibilityScore,
                        RecommendationReasons = new List<string> { "Required for degree completion" },
                        RecommendationType = "Required"
                    });
                }
            }

            // Elective recommendations
            foreach (var electiveCategory in degreeRequirements.ElectiveCategories)
            {
                var electiveRecommendations = await GetElectiveRecommendations(
                    studentProfile, electiveCategory, completedCourses);
                recommendations.AddRange(electiveRecommendations);
            }

            return recommendations.OrderByDescending(r => r.RecommendationScore).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating degree-specific recommendations for student {StudentId}", studentProfile.StudentId);
            throw;
        }
    }

    public async Task<List<CourseRecommendation>> GetPersonalizedRecommendationsAsync(
        StudentAcademicProfile studentProfile,
        int maxRecommendations = 10)
    {
        try
        {
            _logger.LogInformation("Generating personalized recommendations for student {StudentId}", studentProfile.StudentId);

            var recommendations = new List<CourseRecommendation>();

            // Get student's academic history and patterns
            var academicHistory = await GetStudentAcademicHistory(studentProfile.StudentId);
            var strongSubjects = IdentifyStrongSubjects(academicHistory, studentProfile);
            var preferredDifficulty = CalculatePreferredDifficulty(academicHistory);

            // Get courses that match student's success patterns
            var personalizedCourses = await _context.Courses
                .Where(c => !studentProfile.CompletedCourses!.Contains(c.Id))
                .Where(c => strongSubjects.Contains(c.SubjectCode) ||
                           (studentProfile.Interests != null &&
                            studentProfile.Interests.Any(interest =>
                                c.Title.Contains(interest) || (c.Description != null && c.Description.Contains(interest)))))
                .Include(c => c.Subject)
                .Include(c => c.Prerequisites)
                .Take(maxRecommendations * 2) // Get more than needed for filtering
                .ToListAsync();

            foreach (var course in personalizedCourses)
            {
                var personalizedScore = CalculatePersonalizedScore(course, studentProfile, academicHistory);

                if (personalizedScore > 0.3f) // Minimum threshold
                {
                    var reasons = GeneratePersonalizedReasons(course, studentProfile, strongSubjects);

                    recommendations.Add(new CourseRecommendation
                    {
                        Course = course,
                        RecommendationScore = personalizedScore,
                        RecommendationReasons = reasons,
                        RecommendationType = "Personalized",
                        RecommendationMetadata = new Dictionary<string, object>
                        {
                            ["MatchedInterests"] = studentProfile.Interests?.Where(i =>
                                course.Title.Contains(i) || (course.Description?.Contains(i) ?? false)).ToList() ?? new List<string>(),
                            ["SubjectStrength"] = strongSubjects.Contains(course.SubjectCode),
                            ["DifficultyMatch"] = Math.Abs(GetCourseDifficulty(course) - preferredDifficulty) < 0.3f
                        }
                    });
                }
            }

            return recommendations
                .OrderByDescending(r => r.RecommendationScore)
                .Take(maxRecommendations)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating personalized recommendations for student {StudentId}", studentProfile.StudentId);
            throw;
        }
    }

    #region Private Helper Methods

    private async Task<List<Course>> GetAvailableCoursesAsync(StudentAcademicProfile studentProfile)
    {
        var completedCourseIds = studentProfile.CompletedCourses?.ToHashSet() ?? new HashSet<int>();

        return await _context.Courses
            .Where(c => !completedCourseIds.Contains(c.Id))
            .Where(c => c.Status == CourseStatus.Active)
            .Include(c => c.Subject)
            .Include(c => c.Prerequisites)
            .Include(c => c.Offerings)
            .ToListAsync();
    }

    private Task<List<CourseRecommendation>> GetDegreeBasedRecommendations(
        StudentAcademicProfile studentProfile,
        List<Course> availableCourses)
    {
        var recommendations = new List<CourseRecommendation>();

        if (string.IsNullOrEmpty(studentProfile.Major))
            return Task.FromResult(recommendations);

        // Get courses commonly taken by students in the same major
        var majorCourses = availableCourses
            .Where(c => IsCourseRelevantToMajor(c, studentProfile.Major))
            .ToList();

        foreach (var course in majorCourses)
        {
            var relevanceScore = CalculateMajorRelevance(course, studentProfile.Major);

            if (relevanceScore > 0.5f)
            {
                recommendations.Add(new CourseRecommendation
                {
                    Course = course,
                    RecommendationScore = relevanceScore,
                    RecommendationReasons = new List<string> { $"Commonly taken by {studentProfile.Major} majors" },
                    RecommendationType = "Degree"
                });
            }
        }

        return Task.FromResult(recommendations);
    }

    private Task<List<CourseRecommendation>> GetInterestBasedRecommendations(
        StudentAcademicProfile studentProfile,
        List<Course> availableCourses)
    {
        var recommendations = new List<CourseRecommendation>();

        if (studentProfile.Interests?.Any() != true)
            return Task.FromResult(recommendations);

        foreach (var course in availableCourses)
        {
            var interestScore = CalculateInterestMatch(course, studentProfile.Interests);

            if (interestScore > 0.4f)
            {
                var matchedInterests = studentProfile.Interests
                    .Where(interest => course.Title.Contains(interest, StringComparison.OrdinalIgnoreCase) ||
                                     (course.Description?.Contains(interest, StringComparison.OrdinalIgnoreCase) ?? false))
                    .ToList();

                recommendations.Add(new CourseRecommendation
                {
                    Course = course,
                    RecommendationScore = interestScore,
                    RecommendationReasons = new List<string>
                    {
                        $"Matches your interests: {string.Join(", ", matchedInterests)}"
                    },
                    RecommendationType = "Interest"
                });
            }
        }

        return Task.FromResult(recommendations);
    }

    private Task<List<CourseRecommendation>> GetPerformanceBasedRecommendations(
        StudentAcademicProfile studentProfile,
        List<Course> availableCourses)
    {
        var recommendations = new List<CourseRecommendation>();

        if (studentProfile.StrengthAreas?.Any() != true)
            return Task.FromResult(recommendations);

        foreach (var course in availableCourses)
        {
            var strengthMatch = studentProfile.StrengthAreas
                .Any(strength => course.Subject.Title.Contains(strength, StringComparison.OrdinalIgnoreCase) ||
                               course.Title.Contains(strength, StringComparison.OrdinalIgnoreCase));

            if (strengthMatch)
            {
                var confidenceScore = CalculateConfidenceScore(course, studentProfile);

                recommendations.Add(new CourseRecommendation
                {
                    Course = course,
                    RecommendationScore = confidenceScore,
                    RecommendationReasons = new List<string>
                    {
                        "Builds on your academic strengths",
                        $"Strong performance in related area: {course.Subject.Title}"
                    },
                    RecommendationType = "Performance"
                });
            }
        }

        return Task.FromResult(recommendations);
    }

    private Task<List<CourseRecommendation>> GetSequenceBasedRecommendations(
        StudentAcademicProfile studentProfile,
        List<Course> availableCourses)
    {
        var recommendations = new List<CourseRecommendation>();
        var completedCourses = studentProfile.CompletedCourses?.ToHashSet() ?? new HashSet<int>();

        foreach (var course in availableCourses)
        {
            if (IsLogicalNextCourse(course, completedCourses))
            {
                var sequenceScore = CalculateSequenceScore(course, completedCourses);

                recommendations.Add(new CourseRecommendation
                {
                    Course = course,
                    RecommendationScore = sequenceScore,
                    RecommendationReasons = new List<string>
                    {
                        "Natural progression from completed courses",
                        "Prerequisites satisfied"
                    },
                    RecommendationType = "Sequence"
                });
            }
        }

        return Task.FromResult(recommendations);
    }

    private async Task<List<CourseRecommendation>> GetElectiveRecommendations(
        StudentAcademicProfile studentProfile,
        ElectiveCategory electiveCategory,
        HashSet<int> completedCourses)
    {
        var electiveCourses = await _context.Courses
            .Where(c => electiveCategory.SubjectCodes.Contains(c.SubjectCode) ||
                       electiveCategory.ApprovedCourses.Contains(c.Id))
            .Where(c => !completedCourses.Contains(c.Id))
            .Include(c => c.Subject)
            .ToListAsync();

        var recommendations = new List<CourseRecommendation>();

        foreach (var course in electiveCourses)
        {
            var electiveScore = CalculateElectiveScore(course, studentProfile, electiveCategory);

            recommendations.Add(new CourseRecommendation
            {
                Course = course,
                RecommendationScore = electiveScore,
                RecommendationReasons = new List<string>
                {
                    $"Fulfills {electiveCategory.Name} requirement"
                },
                RecommendationType = "Elective"
            });
        }

        return recommendations;
    }

    private List<CourseRecommendation> MergeAndRankRecommendations(List<CourseRecommendation> recommendations)
    {
        // Group by course and merge recommendations
        var merged = recommendations
            .GroupBy(r => r.Course.Id)
            .Select(group =>
            {
                var first = group.First();
                var mergedReasons = group.SelectMany(r => r.RecommendationReasons).Distinct().ToList();
                var avgScore = group.Average(r => r.RecommendationScore);
                var types = string.Join(", ", group.Select(r => r.RecommendationType).Distinct());

                return new CourseRecommendation
                {
                    Course = first.Course,
                    RecommendationScore = avgScore,
                    RecommendationReasons = mergedReasons,
                    RecommendationType = types,
                    RecommendationMetadata = first.RecommendationMetadata
                };
            })
            .OrderByDescending(r => r.RecommendationScore)
            .ToList();

        return merged;
    }

    private Task<List<StudentCourseHistory>> GetStudentAcademicHistory(int studentId)
    {
        // This would typically query course enrollments and grades
        // For now, returning a mock structure
        return Task.FromResult(new List<StudentCourseHistory>());
    }

    private List<string> IdentifyStrongSubjects(List<StudentCourseHistory> history, StudentAcademicProfile profile)
    {
        // Analyze grade patterns to identify strong subjects
        var strongSubjects = new List<string>();

        if (profile.StrengthAreas?.Any() == true)
        {
            // Map strength areas to subject codes
            foreach (var strength in profile.StrengthAreas)
            {
                if (strength.Contains("Programming", StringComparison.OrdinalIgnoreCase))
                    strongSubjects.Add("CS");
                if (strength.Contains("Mathematics", StringComparison.OrdinalIgnoreCase))
                    strongSubjects.Add("MATH");
                if (strength.Contains("Science", StringComparison.OrdinalIgnoreCase))
                    strongSubjects.AddRange(new[] { "PHYS", "CHEM", "BIOL" });
            }
        }

        return strongSubjects.Distinct().ToList();
    }

    private float CalculatePreferredDifficulty(List<StudentCourseHistory> history)
    {
        // Analyze historical performance to determine preferred difficulty level
        // Return a value between 0.0 (easy) and 1.0 (difficult)
        return 0.6f; // Default to moderate difficulty
    }

    private float CalculatePersonalizedScore(Course course, StudentAcademicProfile profile, List<StudentCourseHistory> history)
    {
        var score = 0f;

        // Interest match
        if (profile.Interests?.Any() == true)
        {
            var interestMatch = CalculateInterestMatch(course, profile.Interests);
            score += interestMatch * 0.4f;
        }

        // GPA and performance alignment
        if (profile.CurrentGPA > 0)
        {
            var performanceBonus = profile.CurrentGPA > 3.5m ? 0.2f : 0.1f;
            score += performanceBonus;
        }

        // Subject strength alignment
        if (profile.StrengthAreas?.Any(s => course.Subject.Title.Contains(s, StringComparison.OrdinalIgnoreCase)) == true)
        {
            score += 0.3f;
        }

        // Level appropriateness
        var levelScore = CalculateLevelAppropriateness(course, profile.ClassStanding);
        score += levelScore * 0.1f;

        return Math.Min(score, 1.0f);
    }

    private List<string> GeneratePersonalizedReasons(Course course, StudentAcademicProfile profile, List<string> strongSubjects)
    {
        var reasons = new List<string>();

        if (profile.Interests?.Any(i => course.Title.Contains(i, StringComparison.OrdinalIgnoreCase)) == true)
        {
            reasons.Add("Matches your stated interests");
        }

        if (strongSubjects.Contains(course.SubjectCode))
        {
            reasons.Add("Builds on your academic strengths");
        }

        if (profile.CurrentGPA >= 3.5m)
        {
            reasons.Add("Challenging course suitable for high-performing students");
        }

        if (reasons.Count == 0)
        {
            reasons.Add("Recommended based on your academic profile");
        }

        return reasons;
    }

    #endregion

    #region Scoring Helper Methods

    private float CalculatePrerequisiteEligibility(Course course, HashSet<int> completedCourses)
    {
        if (!course.Prerequisites.Any())
            return 1.0f;

        var satisfiedCount = 0;
        var totalCount = course.Prerequisites.Count;

        foreach (var prereq in course.Prerequisites)
        {
            if (int.TryParse(prereq.RequiredCourseNumber, out int prereqId) &&
                completedCourses.Contains(prereqId))
            {
                satisfiedCount++;
            }
        }

        return (float)satisfiedCount / totalCount;
    }

    private bool IsCourseRelevantToMajor(Course course, string major)
    {
        // Simple heuristic - could be enhanced with more sophisticated mapping
        return major.Contains("Computer", StringComparison.OrdinalIgnoreCase) && course.SubjectCode == "CS" ||
               major.Contains("Math", StringComparison.OrdinalIgnoreCase) && course.SubjectCode == "MATH" ||
               course.Subject.DepartmentName?.Contains(major, StringComparison.OrdinalIgnoreCase) == true;
    }

    private float CalculateMajorRelevance(Course course, string major)
    {
        if (IsCourseRelevantToMajor(course, major))
            return 0.8f;

        return 0.3f;
    }

    private float CalculateInterestMatch(Course course, string[] interests)
    {
        var matchCount = interests.Count(interest =>
            course.Title.Contains(interest, StringComparison.OrdinalIgnoreCase) ||
            (course.Description?.Contains(interest, StringComparison.OrdinalIgnoreCase) ?? false));

        return (float)matchCount / interests.Length;
    }

    private float CalculateConfidenceScore(Course course, StudentAcademicProfile profile)
    {
        var baseScore = 0.6f;

        if (profile.CurrentGPA >= 3.5m)
            baseScore += 0.2f;
        else if (profile.CurrentGPA >= 3.0m)
            baseScore += 0.1f;

        return Math.Min(baseScore, 1.0f);
    }

    private bool IsLogicalNextCourse(Course course, HashSet<int> completedCourses)
    {
        // Check if this course's prerequisites are mostly satisfied
        var prereqsSatisfied = course.Prerequisites.Count == 0 ||
                              course.Prerequisites.Count(p =>
                                  int.TryParse(p.RequiredCourseNumber, out int id) &&
                                  completedCourses.Contains(id)) >= course.Prerequisites.Count * 0.75;

        return prereqsSatisfied;
    }

    private float CalculateSequenceScore(Course course, HashSet<int> completedCourses)
    {
        var eligibility = CalculatePrerequisiteEligibility(course, completedCourses);
        return 0.7f + (eligibility * 0.3f);
    }

    private float CalculateElectiveScore(Course course, StudentAcademicProfile profile, ElectiveCategory category)
    {
        var baseScore = 0.5f;

        // Bonus for matching interests
        if (profile.Interests?.Any() == true)
        {
            var interestMatch = CalculateInterestMatch(course, profile.Interests);
            baseScore += interestMatch * 0.3f;
        }

        // Bonus for appropriate level
        var levelBonus = CalculateLevelAppropriateness(course, profile.ClassStanding);
        baseScore += levelBonus * 0.2f;

        return Math.Min(baseScore, 1.0f);
    }

    private float CalculateLevelAppropriateness(Course course, ClassStanding classStanding)
    {
        if (course.CourseNumber.Length < 3)
            return 0.5f;

        var courseLevel = int.Parse(course.CourseNumber.Substring(course.CourseNumber.Length - 3));

        return classStanding switch
        {
            ClassStanding.Freshman => courseLevel < 200 ? 1.0f : courseLevel < 300 ? 0.7f : 0.3f,
            ClassStanding.Sophomore => courseLevel < 300 ? 1.0f : courseLevel < 400 ? 0.8f : 0.4f,
            ClassStanding.Junior => courseLevel >= 200 && courseLevel < 500 ? 1.0f : 0.6f,
            ClassStanding.Senior => courseLevel >= 300 ? 1.0f : 0.7f,
            _ => 0.5f
        };
    }

    private float GetCourseDifficulty(Course course)
    {
        // Estimate course difficulty based on level and prerequisites
        var baseLevel = course.CourseNumber.Length >= 3 ?
            int.Parse(course.CourseNumber.Substring(course.CourseNumber.Length - 3)) : 100;

        var difficulty = baseLevel / 1000f; // Normalize to 0-1 range
        difficulty += course.Prerequisites.Count * 0.1f; // Add complexity for prerequisites

        return Math.Min(difficulty, 1.0f);
    }

    #endregion
}