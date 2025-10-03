using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Services;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Enums;

namespace Zeus.Academia.Infrastructure.Services;

/// <summary>
/// Course comparison service implementation
/// </summary>
public class CourseComparisonService : ICourseComparisonService
{
    private readonly AcademiaDbContext _context;
    private readonly ILogger<CourseComparisonService> _logger;

    public CourseComparisonService(AcademiaDbContext context, ILogger<CourseComparisonService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<CourseComparison> CompareCoursesAsync(int[] courseIds, CourseComparisonCriteria criteria)
    {
        try
        {
            _logger.LogInformation("Comparing {Count} courses", courseIds.Length);

            var courses = await _context.Courses
                .Where(c => courseIds.Contains(c.Id))
                .Include(c => c.Subject)
                .Include(c => c.Prerequisites)
                .Include(c => c.Offerings)
                    .ThenInclude(o => o.Instructor)
                .ToListAsync();

            if (courses.Count != courseIds.Length)
            {
                var missingIds = courseIds.Except(courses.Select(c => c.Id)).ToList();
                _logger.LogWarning("Some courses not found: {MissingIds}", string.Join(", ", missingIds));
            }

            var courseDetails = courses.Select(c => new CourseComparisonDetails { Course = c }).ToList();

            var comparison = new CourseComparison
            {
                Courses = courseDetails,
                ComparisonMatrix = BuildComparisonMatrix(courses),
                SimilarityScores = CalculateSimilarityScores(courses).ToDictionary(kvp => kvp.Key.ToString(), kvp => kvp.Value),
                KeyDifferences = new List<KeyDifference>()
            };

            _logger.LogInformation("Course comparison completed for {Count} courses", courses.Count);
            return comparison;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error comparing courses with IDs: {CourseIds}", string.Join(", ", courseIds));
            throw;
        }
    }

    public async Task<CourseComparison> CompareCoursesWithCriteriaAsync(
        List<int> courseIds,
        ComparisonCriteria criteria)
    {
        try
        {
            _logger.LogInformation("Comparing {Count} courses with custom criteria", courseIds.Count);

            var baseComparison = await CompareCoursesAsync(courseIds.ToArray(), new CourseComparisonCriteria());

            // Apply custom criteria weighting
            var actualCourses = baseComparison.Courses.Select(cd => cd.Course).ToList();
            baseComparison.SimilarityScores = RecalculateSimilarityWithCriteria(actualCourses, criteria).ToDictionary(kvp => kvp.Key.ToString(), kvp => kvp.Value);
            baseComparison.KeyDifferences = new List<KeyDifference>();

            return baseComparison;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error comparing courses with criteria for IDs: {CourseIds}", string.Join(", ", courseIds));
            throw;
        }
    }

    public async Task<CourseComparison> CompareCoursesDetailedAsync(int courseId1, int courseId2)
    {
        try
        {
            _logger.LogInformation("Comparing courses {CourseId1} and {CourseId2} in detail", courseId1, courseId2);

            var criteria = new CourseComparisonCriteria
            {
                IncludePrerequisites = true,
                IncludeScheduleInfo = true
            };

            return await CompareCoursesAsync(new[] { courseId1, courseId2 }, criteria);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error comparing courses {CourseId1} and {CourseId2} in detail", courseId1, courseId2);
            throw;
        }
    }

    public async Task<List<CourseRecommendation>> FindSimilarCoursesAsync(int courseId, int maxResults = 5)
    {
        try
        {
            _logger.LogInformation("Finding courses similar to course {CourseId}", courseId);

            var targetCourse = await _context.Courses
                .Include(c => c.Subject)
                .Include(c => c.Prerequisites)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (targetCourse == null)
            {
                _logger.LogWarning("Course {CourseId} not found", courseId);
                return new List<CourseRecommendation>();
            }

            var allCourses = await _context.Courses
                .Where(c => c.Id != courseId && c.Status == CourseStatus.Active)
                .Include(c => c.Subject)
                .Include(c => c.Prerequisites)
                .ToListAsync();

            var similarCourses = allCourses
                .Select(course => new
                {
                    Course = course,
                    SimilarityScore = CalculateCourseSimilarity(targetCourse, course)
                })
                .Where(x => x.SimilarityScore > 0.3f) // Minimum similarity threshold
                .OrderByDescending(x => x.SimilarityScore)
                .Take(maxResults)
                .Select(x => new CourseRecommendation
                {
                    Course = x.Course,
                    RecommendationScore = x.SimilarityScore,
                    RecommendationReasons = new List<string> { "Similar course content and structure" },
                    RecommendationType = "Similarity",
                    RecommendationMetadata = new Dictionary<string, object> { ["SimilarityScore"] = x.SimilarityScore }
                })
                .ToList();

            _logger.LogInformation("Found {Count} similar courses to {CourseId}", similarCourses.Count, courseId);
            return similarCourses;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding similar courses to {CourseId}", courseId);
            throw;
        }
    }

    #region Private Helper Methods

    private Dictionary<string, Dictionary<int, object>> BuildComparisonMatrix(List<Course> courses)
    {
        var matrix = new Dictionary<string, Dictionary<int, object>>();

        // Basic course information
        matrix["Title"] = courses.ToDictionary(c => c.Id, c => (object)c.Title);
        matrix["Subject"] = courses.ToDictionary(c => c.Id, c => (object)c.Subject.Title);
        matrix["CourseNumber"] = courses.ToDictionary(c => c.Id, c => (object)c.CourseNumber);
        matrix["Credits"] = courses.ToDictionary(c => c.Id, c => (object)c.CreditHours);
        matrix["Description"] = courses.ToDictionary(c => c.Id, c => (object)TruncateDescription(c.Description ?? "No description"));

        // Prerequisites
        matrix["Prerequisites"] = courses.ToDictionary(c => c.Id, c => (object)c.Prerequisites.Count());
        matrix["PrerequisiteDetails"] = courses.ToDictionary(c => c.Id, c =>
            (object)string.Join(", ", c.Prerequisites.Select(p => p.RequiredCourseNumber ?? "Unknown")));

        // Course level and difficulty
        matrix["CourseLevel"] = courses.ToDictionary(c => c.Id, c => (object)GetCourseLevel(c));
        matrix["EstimatedDifficulty"] = courses.ToDictionary(c => c.Id, c => (object)EstimateCourseDifficulty(c));

        // Availability and scheduling
        matrix["AvailableSections"] = courses.ToDictionary(c => c.Id, c =>
            (object)c.Offerings.Count());
        matrix["ScheduleFlexibility"] = courses.ToDictionary(c => c.Id, c =>
            (object)CalculateScheduleFlexibility(c));

        // Academic requirements
        matrix["FulfillsRequirements"] = courses.ToDictionary(c => c.Id, c =>
            (object)GetRequirementsFulfilled(c));

        return matrix;
    }

    private Dictionary<int, float> CalculateSimilarityScores(List<Course> courses)
    {
        var scores = new Dictionary<int, float>();

        if (courses.Count < 2)
            return scores;

        var referenceCourse = courses.First(); // Use first course as reference

        foreach (var course in courses.Skip(1))
        {
            scores[course.Id] = CalculateCourseSimilarity(referenceCourse, course);
        }

        return scores;
    }

    private int? DetermineRecommendedChoice(List<Course> courses)
    {
        if (!courses.Any())
            return null;

        // Score courses based on multiple factors
        var scoredCourses = courses.Select(course => new
        {
            Course = course,
            Score = CalculateOverallCourseScore(course)
        }).OrderByDescending(x => x.Score);

        return scoredCourses.First().Course.Id;
    }

    private string GenerateComparisonSummary(List<Course> courses)
    {
        if (!courses.Any())
            return "No courses to compare.";

        var summary = new List<string>();

        // Credit comparison
        var credits = courses.Select(c => c.CreditHours).ToList();
        if (credits.Distinct().Count() > 1)
        {
            summary.Add($"Credit hours range from {credits.Min()} to {credits.Max()}");
        }

        // Subject diversity
        var subjects = courses.Select(c => c.Subject.Title).Distinct().ToList();
        if (subjects.Count > 1)
        {
            summary.Add($"Courses span {subjects.Count} different subjects: {string.Join(", ", subjects)}");
        }
        else
        {
            summary.Add($"All courses are in {subjects.First()}");
        }

        // Prerequisite complexity
        var prereqCounts = courses.Select(c => c.Prerequisites.Count).ToList();
        if (prereqCounts.Any(c => c > 0))
        {
            summary.Add($"Prerequisites range from {prereqCounts.Min()} to {prereqCounts.Max()} courses");
        }

        // Course levels
        var levels = courses.Select(GetCourseLevel).Distinct().ToList();
        if (levels.Count > 1)
        {
            summary.Add($"Course levels range from {levels.Min()} to {levels.Max()}");
        }

        return string.Join(". ", summary) + ".";
    }

    private Dictionary<int, float> RecalculateSimilarityWithCriteria(List<Course> courses, ComparisonCriteria criteria)
    {
        var scores = new Dictionary<int, float>();

        if (courses.Count < 2)
            return scores;

        var referenceCourse = courses.First();

        foreach (var course in courses.Skip(1))
        {
            var score = 0f;

            // Apply weighted criteria
            if (criteria.WeightCredits > 0)
            {
                var creditSimilarity = CalculateCreditSimilarity(referenceCourse, course);
                score += creditSimilarity * criteria.WeightCredits;
            }

            if (criteria.WeightDifficulty > 0)
            {
                var difficultySimilarity = CalculateDifficultySimilarity(referenceCourse, course);
                score += difficultySimilarity * criteria.WeightDifficulty;
            }

            if (criteria.WeightSchedule > 0)
            {
                var scheduleSimilarity = CalculateScheduleSimilarity(referenceCourse, course);
                score += scheduleSimilarity * criteria.WeightSchedule;
            }

            if (criteria.WeightSubject > 0)
            {
                var subjectSimilarity = CalculateSubjectSimilarity(referenceCourse, course);
                score += subjectSimilarity * criteria.WeightSubject;
            }

            if (criteria.WeightPrerequisites > 0)
            {
                var prereqSimilarity = CalculatePrerequisiteSimilarity(referenceCourse, course);
                score += prereqSimilarity * criteria.WeightPrerequisites;
            }

            // Normalize score
            var totalWeight = criteria.WeightCredits + criteria.WeightDifficulty +
                            criteria.WeightSchedule + criteria.WeightSubject + criteria.WeightPrerequisites;

            if (totalWeight > 0)
            {
                score /= totalWeight;
            }

            scores[course.Id] = Math.Min(score, 1.0f);
        }

        return scores;
    }

    private int? DetermineRecommendedChoiceWithCriteria(List<Course> courses, ComparisonCriteria criteria)
    {
        if (!courses.Any())
            return null;

        var scoredCourses = courses.Select(course => new
        {
            Course = course,
            Score = CalculateCriteriaBasedScore(course, criteria)
        }).OrderByDescending(x => x.Score);

        return scoredCourses.First().Course.Id;
    }

    private string GenerateComparisonSummaryWithCriteria(List<Course> courses, ComparisonCriteria criteria)
    {
        var baseSummary = GenerateComparisonSummary(courses);
        var criteriaNote = $" Analysis weighted by: ";

        var weights = new List<string>();
        if (criteria.WeightCredits > 0) weights.Add($"Credits ({criteria.WeightCredits:P0})");
        if (criteria.WeightDifficulty > 0) weights.Add($"Difficulty ({criteria.WeightDifficulty:P0})");
        if (criteria.WeightSchedule > 0) weights.Add($"Schedule ({criteria.WeightSchedule:P0})");
        if (criteria.WeightSubject > 0) weights.Add($"Subject ({criteria.WeightSubject:P0})");
        if (criteria.WeightPrerequisites > 0) weights.Add($"Prerequisites ({criteria.WeightPrerequisites:P0})");

        criteriaNote += string.Join(", ", weights);

        return baseSummary + criteriaNote + ".";
    }

    #endregion

    #region Similarity Calculation Methods

    private float CalculateCourseSimilarity(Course course1, Course course2)
    {
        var similarity = 0f;
        var factors = 0f;

        // Subject similarity (30% weight)
        if (course1.SubjectCode == course2.SubjectCode)
        {
            similarity += 0.3f;
        }
        else if (course1.Subject.DepartmentName == course2.Subject.DepartmentName)
        {
            similarity += 0.15f;
        }
        factors += 0.3f;

        // Level similarity (20% weight)
        var level1 = GetCourseLevel(course1);
        var level2 = GetCourseLevel(course2);
        var levelSimilarity = 1.0f - Math.Abs(level1 - level2) / 400f; // Normalize level difference
        similarity += levelSimilarity * 0.2f;
        factors += 0.2f;

        // Credit similarity (15% weight)
        var creditSimilarity = 1.0f - Math.Abs((float)course1.CreditHours - (float)course2.CreditHours) / Math.Max((float)course1.CreditHours, (float)course2.CreditHours);
        similarity += creditSimilarity * 0.15f;
        factors += 0.15f;

        // Title similarity (20% weight)
        var titleSimilarity = CalculateTextSimilarity(course1.Title, course2.Title);
        similarity += titleSimilarity * 0.2f;
        factors += 0.2f;

        // Description similarity (15% weight)
        var descriptionSimilarity = CalculateTextSimilarity(course1.Description ?? "", course2.Description ?? "");
        similarity += descriptionSimilarity * 0.15f;
        factors += 0.15f;

        return factors > 0 ? similarity / factors : 0f;
    }

    private float CalculateCreditSimilarity(Course course1, Course course2)
    {
        return 1.0f - Math.Abs((float)course1.CreditHours - (float)course2.CreditHours) / Math.Max((float)course1.CreditHours, (float)course2.CreditHours);
    }

    private float CalculateDifficultySimilarity(Course course1, Course course2)
    {
        var difficulty1 = EstimateCourseDifficulty(course1);
        var difficulty2 = EstimateCourseDifficulty(course2);
        return 1.0f - Math.Abs(difficulty1 - difficulty2);
    }

    private float CalculateScheduleSimilarity(Course course1, Course course2)
    {
        var flexibility1 = CalculateScheduleFlexibility(course1);
        var flexibility2 = CalculateScheduleFlexibility(course2);
        return 1.0f - Math.Abs(flexibility1 - flexibility2);
    }

    private float CalculateSubjectSimilarity(Course course1, Course course2)
    {
        if (course1.SubjectCode == course2.SubjectCode)
            return 1.0f;

        if (course1.Subject.DepartmentName == course2.Subject.DepartmentName)
            return 0.7f;

        return 0.0f;
    }

    private float CalculatePrerequisiteSimilarity(Course course1, Course course2)
    {
        var prereq1Count = course1.Prerequisites.Count;
        var prereq2Count = course2.Prerequisites.Count;

        if (prereq1Count == 0 && prereq2Count == 0)
            return 1.0f;

        var maxCount = Math.Max(prereq1Count, prereq2Count);
        return 1.0f - Math.Abs(prereq1Count - prereq2Count) / (float)maxCount;
    }

    private float CalculateTextSimilarity(string text1, string text2)
    {
        if (string.IsNullOrEmpty(text1) || string.IsNullOrEmpty(text2))
            return 0f;

        // Simple word overlap similarity
        var words1 = text1.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries).ToHashSet();
        var words2 = text2.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries).ToHashSet();

        var intersection = words1.Intersect(words2).Count();
        var union = words1.Union(words2).Count();

        return union > 0 ? (float)intersection / union : 0f;
    }

    #endregion

    #region Scoring Helper Methods

    private float CalculateOverallCourseScore(Course course)
    {
        var score = 0f;

        // Base score
        score += 0.5f;

        // Credit value bonus (standard credits get bonus)
        if (course.CreditHours >= 3 && course.CreditHours <= 4)
            score += 0.1f;

        // Availability bonus
        var sectionCount = course.Offerings.Count();
        if (sectionCount > 2)
            score += 0.1f;

        // Level appropriateness (prefer upper-level courses slightly)
        var level = GetCourseLevel(course);
        if (level >= 300 && level < 500)
            score += 0.1f;

        // Prerequisites manageable
        if (course.Prerequisites.Count <= 2)
            score += 0.1f;

        // Title informativeness
        if (course.Title.Length > 10 && course.Title.Length < 50)
            score += 0.1f;

        return Math.Min(score, 1.0f);
    }

    private float CalculateCriteriaBasedScore(Course course, ComparisonCriteria criteria)
    {
        var score = 0f;
        var totalWeight = 0f;

        if (criteria.WeightCredits > 0)
        {
            // Prefer standard credit values
            var creditScore = course.CreditHours >= 3 && course.CreditHours <= 4 ? 1.0f : 0.7f;
            score += creditScore * criteria.WeightCredits;
            totalWeight += criteria.WeightCredits;
        }

        if (criteria.WeightDifficulty > 0)
        {
            // Prefer moderate difficulty
            var difficulty = EstimateCourseDifficulty(course);
            var difficultyScore = 1.0f - Math.Abs(difficulty - 0.6f) / 0.6f;
            score += difficultyScore * criteria.WeightDifficulty;
            totalWeight += criteria.WeightDifficulty;
        }

        if (criteria.WeightSchedule > 0)
        {
            var scheduleScore = CalculateScheduleFlexibility(course);
            score += scheduleScore * criteria.WeightSchedule;
            totalWeight += criteria.WeightSchedule;
        }

        if (criteria.WeightSubject > 0)
        {
            // This would need more context about preferred subjects
            var subjectScore = 0.7f; // Default neutral score
            score += subjectScore * criteria.WeightSubject;
            totalWeight += criteria.WeightSubject;
        }

        if (criteria.WeightPrerequisites > 0)
        {
            // Prefer manageable prerequisites
            var prereqScore = course.Prerequisites.Count <= 2 ? 1.0f : 0.5f;
            score += prereqScore * criteria.WeightPrerequisites;
            totalWeight += criteria.WeightPrerequisites;
        }

        return totalWeight > 0 ? score / totalWeight : 0.5f;
    }

    #endregion

    #region Utility Methods

    private string TruncateDescription(string description)
    {
        if (string.IsNullOrEmpty(description))
            return string.Empty;

        return description.Length > 100 ? description.Substring(0, 100) + "..." : description;
    }

    private int GetCourseLevel(Course course)
    {
        if (course.CourseNumber.Length < 3)
            return 100;

        if (int.TryParse(course.CourseNumber.Substring(course.CourseNumber.Length - 3), out int level))
            return level;

        return 100;
    }

    private float EstimateCourseDifficulty(Course course)
    {
        var difficulty = 0f;

        // Base difficulty from course level
        var level = GetCourseLevel(course);
        difficulty += level / 1000f; // Normalize to 0-1

        // Add complexity from prerequisites
        difficulty += course.Prerequisites.Count * 0.1f;

        // Subject-based difficulty (some subjects are typically harder)
        var hardSubjects = new[] { "MATH", "PHYS", "CHEM", "CS" };
        if (hardSubjects.Contains(course.SubjectCode))
            difficulty += 0.2f;

        return Math.Min(difficulty, 1.0f);
    }

    private float CalculateScheduleFlexibility(Course course)
    {
        var flexibility = 0f;
        var totalSections = course.Offerings.Count();

        // More sections = more flexibility
        if (totalSections > 3)
            flexibility += 0.8f;
        else if (totalSections > 1)
            flexibility += 0.5f;
        else
            flexibility += 0.2f;

        // Time variety would add to flexibility (would need section time data)
        // For now, just return section-based flexibility
        return Math.Min(flexibility, 1.0f);
    }

    private string GetRequirementsFulfilled(Course course)
    {
        // This would typically query against degree requirements
        // For now, return a placeholder based on course characteristics
        var requirements = new List<string>();

        var level = GetCourseLevel(course);
        if (level < 200)
            requirements.Add("General Education");
        else if (level < 300)
            requirements.Add("Lower Division Major");
        else if (level < 500)
            requirements.Add("Upper Division Major");
        else
            requirements.Add("Graduate Level");

        // Add subject-specific requirements
        if (course.SubjectCode == "ENGL")
            requirements.Add("Communication Requirement");
        else if (course.SubjectCode == "MATH")
            requirements.Add("Quantitative Reasoning");

        return string.Join(", ", requirements);
    }

    #endregion
}