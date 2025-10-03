using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Services;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Enums;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Zeus.Academia.Infrastructure.Services;

/// <summary>
/// Advanced course search service implementation
/// </summary>
public class CourseSearchService : ICourseSearchService
{
    private readonly AcademiaDbContext _context;
    private readonly ILogger<CourseSearchService> _logger;

    public CourseSearchService(AcademiaDbContext context, ILogger<CourseSearchService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<CourseSearchResult> SearchCoursesAsync(CourseSearchCriteria criteria)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation("Starting course search with criteria: {@Criteria}", criteria);

            var query = _context.Courses.AsQueryable();

            // Apply subject filter
            if (criteria.SubjectCodes?.Any() == true)
            {
                query = query.Where(c => criteria.SubjectCodes.Contains(c.SubjectCode));
            }

            // Apply credit hours filter
            if (criteria.MinCredits.HasValue)
            {
                query = query.Where(c => c.CreditHours >= criteria.MinCredits.Value);
            }

            if (criteria.MaxCredits.HasValue)
            {
                query = query.Where(c => c.CreditHours <= criteria.MaxCredits.Value);
            }

            // Apply level filter
            if (criteria.Level.HasValue)
            {
                query = ApplyLevelFilter(query, criteria.Level.Value);
            }

            // Apply keyword search
            if (criteria.Keywords?.Any() == true)
            {
                query = ApplyKeywordSearch(query, criteria.Keywords, criteria.SearchFields);
            }

            // Apply instructor filter
            if (!string.IsNullOrWhiteSpace(criteria.InstructorName))
            {
                query = query.Where(c => c.Offerings.Any(o =>
                    o.Instructor != null && o.Instructor.Name.Contains(criteria.InstructorName)));
            }

            // Apply semester/year filter
            if (!string.IsNullOrWhiteSpace(criteria.Semester) || criteria.AcademicYear.HasValue)
            {
                query = ApplySemesterFilter(query, criteria.Semester, criteria.AcademicYear);
            }

            // Apply prerequisite filter
            if (criteria.HasPrerequisites.HasValue)
            {
                if (criteria.HasPrerequisites.Value)
                {
                    query = query.Where(c => c.Prerequisites.Any());
                }
                else
                {
                    query = query.Where(c => !c.Prerequisites.Any());
                }
            }

            // Apply available seats filter
            if (criteria.AvailableSeats == true)
            {
                query = ApplyAvailableSeatsFilter(query);
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var courses = await query
                .Skip((criteria.PageNumber - 1) * criteria.PageSize)
                .Take(criteria.PageSize)
                .Include(c => c.Subject)
                .Include(c => c.Prerequisites)
                .Include(c => c.Offerings)
                    .ThenInclude(o => o.Instructor)
                .ToListAsync();

            stopwatch.Stop();

            var result = new CourseSearchResult
            {
                Courses = courses,
                TotalCount = totalCount,
                PageNumber = criteria.PageNumber,
                PageSize = criteria.PageSize,
                SearchDurationMs = stopwatch.ElapsedMilliseconds,
                FilterSummary = GenerateFilterSummary(courses),
                SearchSuggestions = await GenerateSearchSuggestions(string.Join(" ", criteria.Keywords ?? new List<string>())),
                FuzzyMatches = criteria.EnableFuzzySearch ?
                    GenerateFuzzyMatches(string.Join(" ", criteria.Keywords ?? new List<string>()), criteria.FuzzySearchThreshold) : null
            };

            _logger.LogInformation("Course search completed. Found {Count} results in {Duration}ms",
                totalCount, stopwatch.ElapsedMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during course search with criteria: {@Criteria}", criteria);
            throw;
        }
    }

    public async Task<CourseFilterResult> FilterCoursesAsync(CourseFilterCriteria criteria)
    {
        try
        {
            _logger.LogInformation("Starting course filtering with criteria: {@Criteria}", criteria);

            var query = _context.Courses.AsQueryable();

            // Apply subject hierarchy filter
            if (criteria.SubjectHierarchy != null)
            {
                query = ApplySubjectHierarchyFilter(query, criteria.SubjectHierarchy);
            }

            // Apply academic level filter
            if (criteria.AcademicLevel != null)
            {
                query = ApplyAcademicLevelFilter(query, criteria.AcademicLevel);
            }

            // Apply credit hours filter
            if (criteria.CreditHours != null)
            {
                query = ApplyCreditHoursFilter(query, criteria.CreditHours);
            }

            // Apply prerequisite filter
            List<CourseEligibilityResult>? eligibilityResults = null;
            if (criteria.PrerequisiteFilter != null)
            {
                var filterResult = await ApplyPrerequisiteFilter(query, criteria.PrerequisiteFilter);
                query = filterResult.Query;
                eligibilityResults = filterResult.EligibilityResults;
            }

            // Apply schedule filter
            if (criteria.Schedule != null)
            {
                query = ApplyScheduleFilter(query, criteria.Schedule);
            }

            // Get total count and apply pagination
            var totalCount = await query.CountAsync();
            var courses = await query
                .Skip((criteria.PageNumber - 1) * criteria.PageSize)
                .Take(criteria.PageSize)
                .Include(c => c.Subject)
                .Include(c => c.Prerequisites)
                .Include(c => c.Offerings)
                .ToListAsync();

            var result = new CourseFilterResult
            {
                Courses = courses,
                TotalCount = totalCount,
                EligibilityResults = eligibilityResults,
                FilterMetadata = GenerateFilterMetadata(criteria)
            };

            _logger.LogInformation("Course filtering completed. Found {Count} results", totalCount);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during course filtering with criteria: {@Criteria}", criteria);
            throw;
        }
    }

    public async Task<List<string>> GetSearchSuggestionsAsync(string partialInput)
    {
        if (string.IsNullOrWhiteSpace(partialInput) || partialInput.Length < 2)
            return new List<string>();

        try
        {
            var suggestions = new List<string>();

            // Course title suggestions
            var titleSuggestions = await _context.Courses
                .Where(c => c.Title.Contains(partialInput))
                .Select(c => c.Title)
                .Distinct()
                .Take(5)
                .ToListAsync();
            suggestions.AddRange(titleSuggestions);

            // Subject code suggestions
            var subjectSuggestions = await _context.Subjects
                .Where(s => s.Code.Contains(partialInput) || s.Title.Contains(partialInput))
                .Select(s => s.Code + " - " + s.Title)
                .Distinct()
                .Take(5)
                .ToListAsync();
            suggestions.AddRange(subjectSuggestions);

            // Instructor name suggestions
            var instructorSuggestions = await _context.Academics
                .Where(a => a.Name.Contains(partialInput))
                .Select(a => a.Name)
                .Distinct()
                .Take(3)
                .ToListAsync();
            suggestions.AddRange(instructorSuggestions);

            return suggestions.Take(10).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating search suggestions for input: {Input}", partialInput);
            return new List<string>();
        }
    }

    public async Task<List<string>> GetFuzzySearchSuggestionsAsync(string searchTerm, float threshold = 0.8f)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return new List<string>();

        try
        {
            // Get all course titles and subjects for fuzzy matching
            var courseTerms = await _context.Courses
                .Select(c => c.Title)
                .Union(_context.Subjects.Select(s => s.Title))
                .ToListAsync();

            var fuzzyMatches = new List<string>();

            foreach (var term in courseTerms)
            {
                var similarity = CalculateSimilarity(searchTerm.ToLower(), term.ToLower());
                if (similarity >= threshold)
                {
                    fuzzyMatches.Add(term);
                }
            }

            return fuzzyMatches.OrderByDescending(m => CalculateSimilarity(searchTerm.ToLower(), m.ToLower()))
                              .Take(5)
                              .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating fuzzy search suggestions for term: {Term}", searchTerm);
            return new List<string>();
        }
    }

    #region Private Helper Methods

    private IQueryable<Course> ApplyLevelFilter(IQueryable<Course> query, CourseLevel level)
    {
        return level switch
        {
            CourseLevel.Undergraduate => query.Where(c =>
                c.CourseNumber.Length >= 3 &&
                int.Parse(c.CourseNumber.Substring(c.CourseNumber.Length - 3)) < 500),
            CourseLevel.Graduate => query.Where(c =>
                c.CourseNumber.Length >= 3 &&
                int.Parse(c.CourseNumber.Substring(c.CourseNumber.Length - 3)) >= 500),
            _ => query
        };
    }

    private IQueryable<Course> ApplyKeywordSearch(IQueryable<Course> query, List<string> keywords, List<SearchField>? searchFields)
    {
        if (keywords == null || !keywords.Any())
            return query;

        var allTerms = keywords.SelectMany(k => k.Split(' ', StringSplitOptions.RemoveEmptyEntries)).ToList();

        return query.Where(c =>
            searchFields == null || searchFields.Contains(SearchField.All) ?
                allTerms.Any(term =>
                    c.CourseNumber.Contains(term) ||
                    c.Title.Contains(term) ||
                    (c.Description != null && c.Description.Contains(term)) ||
                    c.SubjectCode.Contains(term)) :
                allTerms.Any(term =>
                    (searchFields.Contains(SearchField.CourseNumber) && c.CourseNumber.Contains(term)) ||
                    (searchFields.Contains(SearchField.Title) && c.Title.Contains(term)) ||
                    (searchFields.Contains(SearchField.Description) && c.Description != null && c.Description.Contains(term)) ||
                    (searchFields.Contains(SearchField.SubjectCode) && c.SubjectCode.Contains(term))));
    }

    private IQueryable<Course> ApplySemesterFilter(IQueryable<Course> query, string? semester, int? academicYear)
    {
        if (!string.IsNullOrWhiteSpace(semester) || academicYear.HasValue)
        {
            query = query.Where(c => c.Offerings.Any(o =>
                (string.IsNullOrEmpty(semester) || o.Term == semester) &&
                (!academicYear.HasValue || o.Year == academicYear)));
        }

        return query;
    }

    private IQueryable<Course> ApplyAvailableSeatsFilter(IQueryable<Course> query)
    {
        return query.Where(c => c.Offerings.Any(o =>
            o.MaxEnrollment.HasValue && o.MaxEnrollment > o.CurrentEnrollment));
    }

    private IQueryable<Course> ApplySubjectHierarchyFilter(IQueryable<Course> query, SubjectFilter filter)
    {
        if (!string.IsNullOrWhiteSpace(filter.DepartmentName))
        {
            query = query.Where(c => c.Subject.DepartmentName == filter.DepartmentName);
        }

        if (filter.SubjectCodes?.Any() == true)
        {
            query = query.Where(c => filter.SubjectCodes.Contains(c.SubjectCode));
        }

        if (filter.IncludeRelatedSubjects)
        {
            // Include related subjects from the same department
            query = query.Where(c => c.Subject != null && c.Subject.Department != null && c.Subject.Department.Subjects.Any(s =>
                filter.SubjectCodes == null || filter.SubjectCodes.Contains(s.Code)));
        }

        return query;
    }

    private IQueryable<Course> ApplyAcademicLevelFilter(IQueryable<Course> query, LevelFilter filter)
    {
        if (filter.MinLevel.HasValue || filter.MaxLevel.HasValue)
        {
            query = query.Where(c =>
                (!filter.MinLevel.HasValue || (int)c.Level >= (int)filter.MinLevel.Value) &&
                (!filter.MaxLevel.HasValue || (int)c.Level <= (int)filter.MaxLevel.Value));
        }

        return query;
    }

    private IQueryable<Course> ApplyCreditHoursFilter(IQueryable<Course> query, CreditFilter filter)
    {
        if (filter.MinCredits > 0)
        {
            query = query.Where(c => c.CreditHours >= filter.MinCredits);
        }

        if (filter.MaxCredits > 0)
        {
            query = query.Where(c => c.CreditHours <= filter.MaxCredits);
        }

        return query;
    }

    private async Task<(IQueryable<Course> Query, List<CourseEligibilityResult> EligibilityResults)> ApplyPrerequisiteFilter(
        IQueryable<Course> query, PrerequisiteFilter filter)
    {
        var eligibilityResults = new List<CourseEligibilityResult>();

        if (filter.CheckEligibility && filter.StudentId > 0)
        {
            var courses = await query.Include(c => c.Prerequisites).ToListAsync();
            var studentCompletedCourses = filter.CompletedCourses?.ToHashSet() ?? new HashSet<string>();

            foreach (var course in courses)
            {
                var isEligible = true;
                var missingRequirements = new List<string>();

                if (course.Prerequisites.Any())
                {
                    // Simplified prerequisite check
                    foreach (var prereq in course.Prerequisites)
                    {
                        if (prereq.RequiredCourseNumber != null && !studentCompletedCourses.Contains(prereq.RequiredCourseNumber))
                        {
                            isEligible = false;
                            missingRequirements.Add($"Course {prereq.RequiredCourseNumber}");
                        }
                    }
                }

                eligibilityResults.Add(new CourseEligibilityResult
                {
                    CourseId = course.Id,
                    IsEligible = isEligible,
                    PrerequisiteStatus = isEligible ? PrerequisiteValidationStatus.Valid : PrerequisiteValidationStatus.Invalid,
                    MissingRequirements = missingRequirements
                });
            }

            // Filter to only eligible courses if requested
            if (filter.CheckEligibility)
            {
                var eligibleCourseIds = eligibilityResults.Where(e => e.IsEligible).Select(e => e.CourseId);
                query = query.Where(c => eligibleCourseIds.Contains(c.Id));
            }
        }

        return (query, eligibilityResults);
    }

    private IQueryable<Course> ApplyScheduleFilter(IQueryable<Course> query, ScheduleFilter filter)
    {
        // Note: CourseOffering doesn't currently have detailed schedule properties
        // Schedule filtering would need to be implemented with additional schedule entities
        // For now, we'll skip detailed schedule filtering

        return query;
    }

    private Dictionary<string, int> GenerateFilterSummary(List<Course> courses)
    {
        return new Dictionary<string, int>
        {
            ["TotalCourses"] = courses.Count,
            ["SubjectCount"] = courses.Select(c => c.SubjectCode).Distinct().Count(),
            ["AvgCreditHours"] = courses.Any() ? (int)courses.Average(c => c.CreditHours) : 0
        };
    }

    private async Task<List<string>> GenerateSearchSuggestions(string? keywords)
    {
        if (string.IsNullOrWhiteSpace(keywords))
            return new List<string>();

        return await GetSearchSuggestionsAsync(keywords);
    }

    private List<FuzzyMatch> GenerateFuzzyMatches(string? searchTerm, float threshold)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return new List<FuzzyMatch>();

        // This would typically use a fuzzy string matching library
        // For now, returning a simple example
        return new List<FuzzyMatch>
        {
            new FuzzyMatch
            {
                OriginalTerm = searchTerm,
                SuggestedTerm = "algorithm",
                MatchScore = 0.85f
            }
        };
    }

    private Dictionary<string, object> GenerateFilterMetadata(CourseFilterCriteria criteria)
    {
        var metadata = new Dictionary<string, object>();

        if (criteria.SubjectHierarchy != null)
        {
            metadata["SubjectFilter"] = new
            {
                criteria.SubjectHierarchy.DepartmentName,
                criteria.SubjectHierarchy.SubjectCodes,
                criteria.SubjectHierarchy.IncludeRelatedSubjects
            };
        }

        metadata["FilterAppliedAt"] = DateTime.UtcNow;
        return metadata;
    }

    private SearchTerms ParseSearchKeywords(string keywords)
    {
        var result = new SearchTerms();

        // Simple parsing - could be enhanced with proper boolean logic parser
        if (keywords.Contains(" OR "))
        {
            result.OrTerms = keywords.Split(new[] { " OR " }, StringSplitOptions.RemoveEmptyEntries)
                                   .Select(t => t.Trim().Trim('"'))
                                   .ToList();
        }
        else if (keywords.Contains(" AND "))
        {
            result.AndTerms = keywords.Split(new[] { " AND " }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(t => t.Trim().Trim('"'))
                                    .ToList();
        }
        else
        {
            result.AndTerms = new List<string> { keywords.Trim().Trim('"') };
        }

        return result;
    }

    private float CalculateSimilarity(string s1, string s2)
    {
        if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2))
            return 0;

        if (s1 == s2)
            return 1;

        // Levenshtein distance calculation
        var distance = LevenshteinDistance(s1, s2);
        var maxLength = Math.Max(s1.Length, s2.Length);

        return 1.0f - (float)distance / maxLength;
    }

    private int LevenshteinDistance(string s1, string s2)
    {
        var matrix = new int[s1.Length + 1, s2.Length + 1];

        for (int i = 0; i <= s1.Length; i++)
            matrix[i, 0] = i;

        for (int j = 0; j <= s2.Length; j++)
            matrix[0, j] = j;

        for (int i = 1; i <= s1.Length; i++)
        {
            for (int j = 1; j <= s2.Length; j++)
            {
                var cost = s1[i - 1] == s2[j - 1] ? 0 : 1;
                matrix[i, j] = Math.Min(
                    Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                    matrix[i - 1, j - 1] + cost);
            }
        }

        return matrix[s1.Length, s2.Length];
    }

    public async Task<List<FuzzySearchResultItem>> FuzzySearchAsync(string query, int maxResults = 20)
    {
        try
        {
            _logger.LogInformation("Performing fuzzy search for query: {Query}", query);

            var courses = await _context.Courses
                .Include(c => c.Subject)
                .Where(c => c.Status == CourseStatus.Active)
                .ToListAsync();

            var fuzzyResults = courses
                .Select(c => new { Course = c, Score = CalculateFuzzyMatch(c, query) })
                .Where(x => x.Score > 0.3f)
                .OrderByDescending(x => x.Score)
                .Take(maxResults)
                .Select(x => new FuzzySearchResultItem
                {
                    Course = x.Course,
                    SimilarityScore = x.Score,
                    MatchedFields = GetMatchedFields(x.Course, query)
                })
                .ToList();

            _logger.LogInformation("Found {Count} fuzzy matches for query: {Query}", fuzzyResults.Count, query);
            return fuzzyResults;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing fuzzy search for query: {Query}", query);
            throw;
        }
    }

    private float CalculateFuzzyMatch(Course course, string query)
    {
        var queryLower = query.ToLower();
        var titleMatch = CalculateStringMatch(course.Title.ToLower(), queryLower);
        var codeMatch = CalculateStringMatch(course.CourseNumber.ToLower(), queryLower);
        var descMatch = course.Description != null ? CalculateStringMatch(course.Description.ToLower(), queryLower) : 0f;

        return Math.Max(titleMatch, Math.Max(codeMatch, descMatch));
    }

    private float CalculateStringMatch(string source, string target)
    {
        if (source.Contains(target)) return 1.0f;

        // Simple fuzzy matching based on character overlap
        var common = source.Intersect(target).Count();
        var total = Math.Max(source.Length, target.Length);
        return total > 0 ? (float)common / total : 0f;
    }

    private List<string> GetMatchedFields(Course course, string query)
    {
        var matchedFields = new List<string>();
        var queryLower = query.ToLower();

        if (course.CourseNumber.ToLower().Contains(queryLower))
            matchedFields.Add("CourseNumber");

        if (course.Title.ToLower().Contains(queryLower))
            matchedFields.Add("Title");

        if (!string.IsNullOrEmpty(course.Description) && course.Description.ToLower().Contains(queryLower))
            matchedFields.Add("Description");

        if (course.SubjectCode.ToLower().Contains(queryLower))
            matchedFields.Add("SubjectCode");

        return matchedFields;
    }

    #endregion
}

#region Helper Classes

public class SearchTerms
{
    public List<string> AndTerms { get; set; } = new();
    public List<string> OrTerms { get; set; } = new();
}

public enum CourseLevel
{
    Undergraduate,
    Graduate,
    Doctoral
}

public enum CourseSearchField
{
    Title,
    Description,
    LearningOutcomes,
    Prerequisites,
    Instructor
}



#endregion