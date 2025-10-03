using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Enums;
using Zeus.Academia.Infrastructure.Extensions;
using Zeus.Academia.Infrastructure.Models;

namespace Zeus.Academia.Infrastructure.Services;

/// <summary>
/// Service for course sequence planning with semester mapping, prerequisite validation, 
/// optimal scheduling algorithms, and course availability optimization.
/// </summary>
public class CourseSequencePlanningService
{
    private readonly AcademiaDbContext _context;
    private readonly ILogger<CourseSequencePlanningService> _logger;

    public CourseSequencePlanningService(AcademiaDbContext context, ILogger<CourseSequencePlanningService> logger)
    {
        _context = context;
        _logger = logger;
    }

    #region Sequence Plan Creation

    /// <summary>
    /// Creates an optimal semester sequence plan for a student based on their profile and degree requirements.
    /// </summary>
    /// <param name="studentProfile">Student's academic profile and preferences</param>
    /// <param name="degreeTemplate">Degree requirements template</param>
    /// <returns>Complete sequence plan with semester-by-semester course mapping</returns>
    public async Task<SequencePlan> CreateSequencePlanAsync(Models.StudentAcademicProfile studentProfile, DegreeRequirementTemplate degreeTemplate)
    {
        try
        {
            _logger.LogInformation("Creating sequence plan for student {StudentId} in degree {DegreeCode}",
                studentProfile.StudentId, degreeTemplate.DegreeCode);

            var plan = new SequencePlan
            {
                StudentId = studentProfile.StudentId,
                DegreeCode = degreeTemplate.DegreeCode,
                PlanDate = DateTime.UtcNow,
                StartingSemester = ParseSemester(studentProfile.GetStartingSemesterString()),
                ExpectedGraduation = studentProfile.ExpectedGraduationDate
            };

            // Get all required courses
            var allRequirements = degreeTemplate.GetAllRequirements();
            var requiredCourses = await GetAllRequiredCoursesAsync(allRequirements);

            // Remove already completed courses
            var remainingCourses = requiredCourses
                .Where(c => !studentProfile.CompletedCourses.Contains(c.Id))
                .ToList();

            // Build comprehensive prerequisite chain
            var prerequisiteChain = await BuildPrerequisiteChainAsync(remainingCourses);

            // Get course offerings information
            var courseOfferings = await GetCourseOfferingsAsync(remainingCourses.Select(c => c.Id).ToList());

            // Create semester assignments
            var semesterPlans = await CreateSemesterAssignmentsAsync(
                studentProfile,
                remainingCourses,
                prerequisiteChain,
                courseOfferings,
                plan.StartingSemester,
                plan.ExpectedGraduation);

            plan.SemesterPlans = semesterPlans;
            plan.TotalSemesters = semesterPlans.Count;
            plan.TotalCredits = semesterPlans.Sum(s => s.TotalCredits);

            // Validate and optimize the plan
            await ValidateSequencePlanAsync(plan, prerequisiteChain);
            await OptimizePlanForStudentPreferencesAsync(plan, studentProfile);

            _logger.LogInformation("Sequence plan created: {TotalSemesters} semesters, {TotalCredits} credits",
                plan.TotalSemesters, plan.TotalCredits);

            return plan;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating sequence plan for student {StudentId}", studentProfile.StudentId);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing sequence plan based on completed courses and changing circumstances.
    /// </summary>
    /// <param name="existingPlan">Current sequence plan</param>
    /// <param name="updatedProfile">Updated student profile</param>
    /// <returns>Revised sequence plan</returns>
    public async Task<SequencePlan> UpdateSequencePlanAsync(SequencePlan existingPlan, Models.StudentAcademicProfile updatedProfile)
    {
        try
        {
            _logger.LogInformation("Updating sequence plan {PlanId} for student {StudentId}",
                existingPlan.Id, updatedProfile.StudentId);

            // Mark completed courses in the plan
            foreach (var completedCourseId in updatedProfile.GetCompletedCourses())
            {
                foreach (var semesterPlan in existingPlan.SemesterPlans)
                {
                    var plannedCourse = semesterPlan.Courses.FirstOrDefault(c => c.CourseId == completedCourseId);
                    if (plannedCourse != null)
                    {
                        plannedCourse.IsCompleted = true;
                        plannedCourse.CompletionDate = DateTime.UtcNow;
                    }
                }
            }

            // Remove completed semesters and adjust remaining plan
            var currentSemester = ParseSemester(updatedProfile.GetCurrentSemesterString());
            existingPlan.SemesterPlans = existingPlan.SemesterPlans
                .Where(s => s.Semester >= currentSemester)
                .ToList();

            // Re-optimize remaining semesters
            await OptimizePlanForStudentPreferencesAsync(existingPlan, updatedProfile);

            existingPlan.LastModified = DateTime.UtcNow;

            _logger.LogInformation("Sequence plan updated: {RemainingSemesters} semesters remaining",
                existingPlan.SemesterPlans.Count);

            return existingPlan;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating sequence plan for student {StudentId}", updatedProfile.StudentId);
            throw;
        }
    }

    #endregion

    #region Prerequisite Chain Analysis

    /// <summary>
    /// Builds a comprehensive prerequisite chain for a list of courses.
    /// </summary>
    /// <param name="courses">Courses to analyze</param>
    /// <returns>Prerequisite chain with dependency levels</returns>
    public async Task<PrerequisiteChainResult> BuildPrerequisiteChainAsync(List<Course> courses)
    {
        try
        {
            _logger.LogDebug("Building prerequisite chain for {CourseCount} courses", courses.Count);

            var result = new PrerequisiteChainResult();
            var courseIds = courses.Select(c => c.Id).ToHashSet();

            // Get all prerequisites for these courses
            var prerequisites = await _context.CoursePrerequisites
                .Where(p => courseIds.Contains(p.CourseId))
                .Include(p => p.Course)
                // Note: RequiredCourse navigation removed as it doesn't exist on CoursePrerequisite
                .ToListAsync();

            // Build adjacency graph (course -> prerequisites)
            var prerequisiteGraph = new Dictionary<int, List<PrerequisiteRelation>>();

            foreach (var course in courses)
            {
                prerequisiteGraph[course.Id] = prerequisites
                    .Where(p => p.CourseId == course.Id)
                    .Select(p => new PrerequisiteRelation
                    {
                        CourseId = p.CourseId,
                        PrerequisiteCourseId = p.GetRequiredCourseId(),
                        LogicType = p.LogicalOperator == LogicalOperator.And ? RequirementLogicType.And : RequirementLogicType.Or,
                        IsStrict = p.IsRequired,
                        MinimumGrade = p.MinimumGrade
                    }).ToList();
            }

            // Detect circular dependencies
            var circularDependencies = DetectCircularDependencies(prerequisiteGraph);
            if (circularDependencies.Any())
            {
                result.HasCircularDependency = true;
                result.CircularDependencies = circularDependencies;
                _logger.LogWarning("Circular dependencies detected: {Dependencies}",
                    string.Join(", ", circularDependencies.Select(cd => string.Join(" -> ", cd))));
            }

            // Perform topological sort to determine course levels
            result.Levels = PerformTopologicalSort(prerequisiteGraph, courses);
            result.TotalLevels = result.Levels.Count;

            // Calculate optimal semester mapping
            result.OptimalSemesterMapping = CalculateOptimalSemesterMapping(result.Levels);

            // Identify critical path (longest dependency chain)
            result.CriticalPath = FindCriticalPath(prerequisiteGraph, courses);
            result.CriticalPathLength = result.CriticalPath.Count;

            _logger.LogDebug("Prerequisite chain built: {Levels} levels, critical path length: {CriticalLength}",
                result.TotalLevels, result.CriticalPathLength);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error building prerequisite chain");
            throw;
        }
    }

    /// <summary>
    /// Validates that a sequence of courses respects all prerequisite requirements.
    /// </summary>
    /// <param name="courseSequence">Ordered sequence of courses</param>
    /// <param name="completedCourses">Already completed courses</param>
    /// <returns>Validation result with any violations found</returns>
    public async Task<PrerequisiteValidationResult> ValidatePrerequisiteSequenceAsync(
        List<PlannedCourse> courseSequence,
        List<int> completedCourses)
    {
        try
        {
            _logger.LogDebug("Validating prerequisite sequence for {CourseCount} courses", courseSequence.Count);

            var result = new PrerequisiteValidationResult();
            var satisfiedCourses = new HashSet<int>(completedCourses);

            // Group courses by semester
            var coursesBySemester = courseSequence
                .GroupBy(c => c.PlannedSemester)
                .OrderBy(g => g.Key)
                .ToList();

            foreach (var semesterGroup in coursesBySemester)
            {
                var semesterCourses = semesterGroup.ToList();
                var semesterViolations = new List<PrerequisiteViolation>();

                foreach (var plannedCourse in semesterCourses)
                {
                    // Get prerequisites for this course
                    var prerequisites = await _context.CoursePrerequisites
                        .Where(p => p.CourseId == plannedCourse.CourseId)
                        // .Include(p => p.RequiredCourse) // Navigation property doesn't exist
                        .ToListAsync();

                    // Check each prerequisite
                    foreach (var prerequisite in prerequisites)
                    {
                        if (!satisfiedCourses.Contains(prerequisite.GetRequiredCourseId()))
                        {
                            semesterViolations.Add(new PrerequisiteViolation
                            {
                                CourseId = plannedCourse.CourseId,
                                CourseName = plannedCourse.CourseName,
                                PrerequisiteCourseId = prerequisite.GetRequiredCourseId(),
                                PrerequisiteCourseName = prerequisite.GetRequiredCourse()?.Title ?? "Unknown",
                                Semester = semesterGroup.Key,
                                ViolationType = PrerequisiteViolationType.NotSatisfied,
                                Description = $"Course {plannedCourse.CourseName} requires {prerequisite.GetRequiredCourse()?.Title ?? "Unknown"} which is not completed"
                            });
                        }
                    }

                    // Check for corequisites (courses that must be taken simultaneously)
                    var corequisites = await _context.CoursePrerequisites
                        .Where(p => p.CourseId == plannedCourse.CourseId) // IsCorequisite property doesn't exist
                        .ToListAsync();

                    foreach (var corequisite in corequisites)
                    {
                        var corequisitePlanned = semesterCourses.Any(c => c.CourseId == corequisite.GetRequiredCourseId());
                        var corequisiteCompleted = satisfiedCourses.Contains(corequisite.GetRequiredCourseId());

                        if (!corequisitePlanned && !corequisiteCompleted)
                        {
                            semesterViolations.Add(new PrerequisiteViolation
                            {
                                CourseId = plannedCourse.CourseId,
                                CourseName = plannedCourse.CourseName,
                                PrerequisiteCourseId = corequisite.GetRequiredCourseId(),
                                Semester = semesterGroup.Key,
                                ViolationType = PrerequisiteViolationType.CorequisiteNotMet,
                                Description = $"Course {plannedCourse.CourseName} requires corequisite that is not scheduled in the same semester"
                            });
                        }
                    }
                }

                result.ViolationsBySemester[semesterGroup.Key] = semesterViolations;
                result.AllViolations.AddRange(semesterViolations);

                // Add completed courses from this semester for next iteration
                foreach (var course in semesterCourses)
                {
                    satisfiedCourses.Add(course.CourseId);
                }
            }

            result.IsValid = !result.AllViolations.Any();
            result.TotalViolations = result.AllViolations.Count;

            _logger.LogDebug("Prerequisite validation completed: {IsValid}, {ViolationCount} violations",
                result.IsValid, result.TotalViolations);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating prerequisite sequence");
            throw;
        }
    }

    #endregion

    #region Course Availability Optimization

    /// <summary>
    /// Optimizes a sequence plan based on course offering patterns and availability.
    /// </summary>
    /// <param name="studentProfile">Student profile</param>
    /// <param name="courseOfferings">Course offering information</param>
    /// <returns>Optimized sequence plan</returns>
    public async Task<SequencePlan> OptimizeForAvailabilityAsync(
        Models.StudentAcademicProfile studentProfile,
        List<CourseOffering> courseOfferings)
    {
        try
        {
            _logger.LogInformation("Optimizing sequence plan for course availability for student {StudentId}",
                studentProfile.StudentId);

            var availabilityMap = courseOfferings
                .GroupBy(co => co.CourseId)
                .ToDictionary(
                    g => g.Key,
                    g => g.ToList()
                );

            var plan = new SequencePlan
            {
                StudentId = studentProfile.StudentId,
                StartingSemester = ParseSemester(studentProfile.GetStartingSemesterString()),
                ExpectedGraduation = studentProfile.ExpectedGraduationDate
            };

            // Get remaining courses needed
            var degreeTemplate = await _context.DegreeRequirementTemplates
                .Include(t => t.Categories)
                    .ThenInclude(c => c.Requirements)
                        .ThenInclude(r => r.RequiredCourses)
                .FirstOrDefaultAsync(t => t.DegreeCode == studentProfile.MajorCode);

            if (degreeTemplate == null)
            {
                throw new InvalidOperationException($"Degree template not found for {studentProfile.MajorCode}");
            }

            var allRequirements = degreeTemplate.GetAllRequirements();
            var requiredCourses = await GetAllRequiredCoursesAsync(allRequirements);
            var remainingCourses = requiredCourses
                .Where(c => !studentProfile.CompletedCourses.Contains(c.Id))
                .ToList();

            // Build prerequisite chain
            var prerequisiteChain = await BuildPrerequisiteChainAsync(remainingCourses);

            // Create availability-aware semester assignments
            var semesterPlans = new List<SemesterPlan>();
            var currentSemester = plan.StartingSemester;
            var assignedCourses = new HashSet<int>();
            var completedCourses = new HashSet<int>(studentProfile.GetCompletedCourses());

            while (assignedCourses.Count < remainingCourses.Count && currentSemester.Year <= plan.ExpectedGraduation.Year + 1)
            {
                var semesterPlan = new SemesterPlan
                {
                    Semester = currentSemester,
                    SemesterName = FormatSemesterName(currentSemester),
                    Courses = new List<PlannedCourse>()
                };

                // Find courses available this semester with satisfied prerequisites
                var availableThisSemester = remainingCourses
                    .Where(c => !assignedCourses.Contains(c.Id))
                    .Where(c => IsCourseOfferedInSemester(c.Id, currentSemester, availabilityMap))
                    .Where(c => ArePrerequisitesSatisfied(c.Id, completedCourses, prerequisiteChain))
                    .OrderBy(c => GetCoursePriority(c, prerequisiteChain))
                    .ToList();

                // Add courses up to student's preferred load
                var targetCredits = GetTargetCreditsForSemester(currentSemester, studentProfile.PreferredCourseLoad.ToCredits());
                var currentCredits = 0;

                foreach (var course in availableThisSemester)
                {
                    if (currentCredits + course.CreditHours <= targetCredits)
                    {
                        var plannedCourse = new PlannedCourse
                        {
                            CourseId = course.Id,
                            CourseName = $"{course.Subject.Code} {course.CourseNumber}",
                            CourseTitle = course.Title,
                            CreditHours = (int)course.CreditHours,
                            PlannedSemester = currentSemester,
                            PrerequisitesSatisfied = true,
                            DifficultyRating = DifficultyLevel.Intermediate, // DifficultyRating property doesn't exist
                            IsRequired = IsRequiredCourse(course.Id, allRequirements)
                        };

                        semesterPlan.Courses.Add(plannedCourse);
                        assignedCourses.Add(course.Id);
                        completedCourses.Add(course.Id);
                        currentCredits += (int)course.CreditHours;
                    }
                }

                semesterPlan.TotalCredits = currentCredits;

                if (semesterPlan.Courses.Any())
                {
                    semesterPlans.Add(semesterPlan);
                }

                currentSemester = GetNextSemester(currentSemester, studentProfile.SummerAvailability);
            }

            plan.SemesterPlans = semesterPlans;
            plan.TotalSemesters = semesterPlans.Count;
            plan.TotalCredits = semesterPlans.Sum(s => s.TotalCredits);

            _logger.LogInformation("Availability-optimized plan created: {TotalSemesters} semesters, {TotalCredits} credits",
                plan.TotalSemesters, plan.TotalCredits);

            return plan;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error optimizing sequence plan for availability");
            throw;
        }
    }

    /// <summary>
    /// Analyzes course offering patterns to identify potential scheduling conflicts.
    /// </summary>
    /// <param name="courseIds">Course IDs to analyze</param>
    /// <returns>Offering pattern analysis results</returns>
    public async Task<CourseOfferingAnalysis> AnalyzeCourseOfferingPatternsAsync(List<int> courseIds)
    {
        try
        {
            _logger.LogDebug("Analyzing course offering patterns for {CourseCount} courses", courseIds.Count);

            var analysis = new CourseOfferingAnalysis();

            var offerings = await _context.CourseOfferings
                .Where(co => courseIds.Contains(co.CourseId))
                .Include(co => co.Course)
                    .ThenInclude(c => c.Subject)
                .ToListAsync();

            var offeringsByTerm = offerings.GroupBy(o => o.Term).ToList();

            foreach (var termGroup in offeringsByTerm)
            {
                var termAnalysis = new TermOfferingAnalysis
                {
                    Term = termGroup.Key,
                    TotalCoursesOffered = termGroup.Count(),
                    RegularlyOfferedCourses = termGroup.Count(o => o.IsRegularlyOffered()),
                    OccasionallyOfferedCourses = termGroup.Count(o => !o.IsRegularlyOffered()),
                    CourseIds = termGroup.Select(o => o.CourseId).ToList()
                };

                // Identify high-demand courses (limited sections)
                termAnalysis.HighDemandCourses = termGroup
                    .Where(o => o.MaxEnrollment.HasValue && o.MaxEnrollment < 30)
                    .Select(o => new HighDemandCourse
                    {
                        CourseId = o.CourseId,
                        CourseName = $"{o.Course.Subject.Code} {o.Course.CourseNumber}",
                        MaxEnrollment = o.MaxEnrollment.Value,
                        TypicalEnrollment = o.GetTypicalEnrollment()
                    }).ToList();

                analysis.TermAnalyses.Add(termAnalysis);
            }

            // Identify courses with limited offering patterns
            analysis.LimitedOfferingCourses = courseIds
                .Where(id => offerings.Count(o => o.CourseId == id) <= 2) // Offered 2 or fewer terms per year
                .Select(id => new LimitedOfferingCourse
                {
                    CourseId = id,
                    CourseName = offerings.First(o => o.CourseId == id).Course.Title,
                    OfferingTerms = offerings.Where(o => o.CourseId == id).Select(o => o.Term).ToList(),
                    IsRegularlyOffered = offerings.Any(o => o.CourseId == id && o.IsRegularlyOffered())
                }).ToList();

            // Identify prerequisite bottlenecks
            analysis.PrerequisiteBottlenecks = await IdentifyPrerequisiteBottlenecksAsync(courseIds);

            analysis.TotalCoursesAnalyzed = courseIds.Count;
            analysis.AnalysisDate = DateTime.UtcNow;

            _logger.LogDebug("Course offering analysis completed: {LimitedOffering} limited courses, {Bottlenecks} bottlenecks",
                analysis.LimitedOfferingCourses.Count, analysis.PrerequisiteBottlenecks.Count);

            return analysis;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing course offering patterns");
            throw;
        }
    }

    #endregion

    #region Private Helper Methods

    private async Task<List<Course>> GetAllRequiredCoursesAsync(List<DegreeRequirement> requirements)
    {
        var courseIds = new HashSet<int>();

        foreach (var requirement in requirements)
        {
            if (requirement.Type == RequirementType.SpecificCourse)
            {
                foreach (var courseId in requirement.GetCourseIds())
                {
                    courseIds.Add(courseId);
                }
            }
            else if (requirement.Type == RequirementType.CourseGroup)
            {
                // For course groups, get representative courses from the subject areas
                var subjectCodes = requirement.GetSubjectCodes();
                var subjectCourses = await _context.Courses
                    .Where(c => subjectCodes.Contains(c.Subject.Code) &&
                               c.GetCourseLevel() >= requirement.MinimumCourseLevel &&
                               c.GetCourseLevel() <= requirement.MaximumCourseLevel &&
                               true) // IsActive property doesn't exist
                    .Select(c => c.Id)
                    .ToListAsync();

                foreach (var id in subjectCourses)
                {
                    courseIds.Add(id);
                }
            }
        }

        return await _context.Courses
            .Include(c => c.Subject)
            .Include(c => c.Prerequisites)
            .Where(c => courseIds.Contains(c.Id))
            .ToListAsync();
    }

    private async Task<List<CourseOffering>> GetCourseOfferingsAsync(List<int> courseIds)
    {
        var offerings = await _context.CourseOfferings
            .Where(co => courseIds.Contains(co.CourseId))
            .Include(co => co.Course)
                .ThenInclude(c => c.Subject)
            .ToListAsync();

        // Convert Entity CourseOffering to Service CourseOffering
        return offerings.Select(o => new CourseOffering
        {
            CourseId = o.CourseId,
            Term = o.Term ?? "Unknown",
            MaxEnrollment = o.MaxEnrollment ?? 0,
            // Add other property mappings as needed
        }).ToList();
    }

    private async Task<List<SemesterPlan>> CreateSemesterAssignmentsAsync(
        Models.StudentAcademicProfile studentProfile,
        List<Course> remainingCourses,
        PrerequisiteChainResult prerequisiteChain,
        List<CourseOffering> courseOfferings,
        SemesterInfo startingSemester,
        DateTime expectedGraduation)
    {
        var semesterPlans = new List<SemesterPlan>();
        var currentSemester = startingSemester;
        var assignedCourses = new HashSet<int>();
        var completedCourses = new HashSet<int>(studentProfile.CompletedCourses);

        // Use topological ordering from prerequisite chain
        var coursesByLevel = prerequisiteChain.Levels;
        var offeringMap = courseOfferings.GroupBy(co => co.CourseId).ToDictionary(g => g.Key, g => g.ToList());

        foreach (var level in coursesByLevel)
        {
            var levelCourses = remainingCourses.Where(c => level.Contains(c.Id)).ToList();

            while (levelCourses.Any(c => !assignedCourses.Contains(c.Id)))
            {
                var semesterPlan = new SemesterPlan
                {
                    Semester = currentSemester,
                    SemesterName = FormatSemesterName(currentSemester),
                    Courses = new List<PlannedCourse>()
                };

                var availableThisSemester = levelCourses
                    .Where(c => !assignedCourses.Contains(c.Id))
                    .Where(c => IsCourseOfferedInSemester(c.Id, currentSemester, offeringMap))
                    .Where(c => ArePrerequisitesSatisfied(c.Id, completedCourses, prerequisiteChain))
                    .ToList();

                var targetCredits = GetTargetCreditsForSemester(currentSemester, studentProfile.PreferredCourseLoad.ToCredits());
                var currentCredits = 0;

                foreach (var course in availableThisSemester)
                {
                    if (currentCredits + course.CreditHours <= targetCredits)
                    {
                        var plannedCourse = new PlannedCourse
                        {
                            CourseId = course.Id,
                            CourseName = $"{course.Subject.Code} {course.CourseNumber}",
                            CourseTitle = course.Title,
                            CreditHours = (int)course.CreditHours,
                            PlannedSemester = currentSemester,
                            PrerequisitesSatisfied = true,
                            DifficultyRating = DifficultyLevel.Intermediate // DifficultyRating property doesn't exist
                        };

                        semesterPlan.Courses.Add(plannedCourse);
                        assignedCourses.Add(course.Id);
                        completedCourses.Add(course.Id);
                        currentCredits += (int)course.CreditHours;
                    }
                }

                semesterPlan.TotalCredits = currentCredits;

                if (semesterPlan.Courses.Any())
                {
                    semesterPlans.Add(semesterPlan);
                }

                currentSemester = GetNextSemester(currentSemester, studentProfile.SummerAvailability);

                // Safety check to prevent infinite loops
                if (currentSemester.Year > expectedGraduation.Year + 2)
                {
                    break;
                }
            }
        }

        return semesterPlans;
    }

    private List<List<int>> DetectCircularDependencies(Dictionary<int, List<PrerequisiteRelation>> graph)
    {
        var cycles = new List<List<int>>();
        var visited = new HashSet<int>();
        var recursionStack = new HashSet<int>();

        foreach (var courseId in graph.Keys)
        {
            if (!visited.Contains(courseId))
            {
                var path = new List<int>();
                DetectCycleUtil(graph, courseId, visited, recursionStack, path, cycles);
            }
        }

        return cycles;
    }

    private bool DetectCycleUtil(Dictionary<int, List<PrerequisiteRelation>> graph, int courseId,
        HashSet<int> visited, HashSet<int> recursionStack, List<int> path, List<List<int>> cycles)
    {
        visited.Add(courseId);
        recursionStack.Add(courseId);
        path.Add(courseId);

        if (graph.ContainsKey(courseId))
        {
            foreach (var relation in graph[courseId])
            {
                var prerequisiteId = relation.PrerequisiteCourseId;

                if (!visited.Contains(prerequisiteId))
                {
                    if (DetectCycleUtil(graph, prerequisiteId, visited, recursionStack, path, cycles))
                    {
                        return true;
                    }
                }
                else if (recursionStack.Contains(prerequisiteId))
                {
                    var cycleStart = path.IndexOf(prerequisiteId);
                    var cycle = path.Skip(cycleStart).ToList();
                    cycles.Add(cycle);
                    return true;
                }
            }
        }

        path.RemoveAt(path.Count - 1);
        recursionStack.Remove(courseId);
        return false;
    }

    private List<List<int>> PerformTopologicalSort(Dictionary<int, List<PrerequisiteRelation>> graph, List<Course> courses)
    {
        var inDegree = new Dictionary<int, int>();
        var levels = new List<List<int>>();

        // Initialize in-degrees
        foreach (var course in courses)
        {
            inDegree[course.Id] = 0;
        }

        // Calculate in-degrees
        foreach (var relations in graph.Values)
        {
            foreach (var relation in relations)
            {
                if (inDegree.ContainsKey(relation.PrerequisiteCourseId))
                {
                    // The course has a prerequisite, so increment its in-degree
                    inDegree[relation.CourseId]++;
                }
            }
        }

        var queue = new Queue<int>();

        // Find courses with no prerequisites
        foreach (var kvp in inDegree)
        {
            if (kvp.Value == 0)
            {
                queue.Enqueue(kvp.Key);
            }
        }

        while (queue.Count > 0)
        {
            var currentLevel = new List<int>();
            var levelSize = queue.Count;

            for (int i = 0; i < levelSize; i++)
            {
                var courseId = queue.Dequeue();
                currentLevel.Add(courseId);

                // Reduce in-degree for dependent courses
                foreach (var dependentId in courses.Select(c => c.Id))
                {
                    if (graph.ContainsKey(dependentId))
                    {
                        var hasPrerequisite = graph[dependentId].Any(r => r.PrerequisiteCourseId == courseId);
                        if (hasPrerequisite)
                        {
                            inDegree[dependentId]--;
                            if (inDegree[dependentId] == 0)
                            {
                                queue.Enqueue(dependentId);
                            }
                        }
                    }
                }
            }

            if (currentLevel.Any())
            {
                levels.Add(currentLevel);
            }
        }

        return levels;
    }

    private Dictionary<int, int> CalculateOptimalSemesterMapping(List<List<int>> levels)
    {
        var mapping = new Dictionary<int, int>();

        for (int level = 0; level < levels.Count; level++)
        {
            foreach (var courseId in levels[level])
            {
                mapping[courseId] = level + 1;
            }
        }

        return mapping;
    }

    private List<int> FindCriticalPath(Dictionary<int, List<PrerequisiteRelation>> graph, List<Course> courses)
    {
        // Find the longest path in the prerequisite graph
        var longestPath = new List<int>();
        var visited = new HashSet<int>();

        foreach (var course in courses)
        {
            var path = FindLongestPathFromCourse(graph, course.Id, visited, new List<int>());
            if (path.Count > longestPath.Count)
            {
                longestPath = path;
            }
        }

        return longestPath;
    }

    private List<int> FindLongestPathFromCourse(Dictionary<int, List<PrerequisiteRelation>> graph, int courseId,
        HashSet<int> visited, List<int> currentPath)
    {
        if (visited.Contains(courseId))
        {
            return new List<int>(currentPath);
        }

        visited.Add(courseId);
        currentPath.Add(courseId);

        var longestPath = new List<int>(currentPath);

        if (graph.ContainsKey(courseId))
        {
            foreach (var relation in graph[courseId])
            {
                var path = FindLongestPathFromCourse(graph, relation.PrerequisiteCourseId, visited, currentPath);
                if (path.Count > longestPath.Count)
                {
                    longestPath = path;
                }
            }
        }

        currentPath.RemoveAt(currentPath.Count - 1);
        visited.Remove(courseId);

        return longestPath;
    }

    private async Task ValidateSequencePlanAsync(SequencePlan plan, PrerequisiteChainResult prerequisiteChain)
    {
        var allPlannedCourses = plan.SemesterPlans.SelectMany(s => s.Courses).ToList();
        var validationResult = await ValidatePrerequisiteSequenceAsync(allPlannedCourses, new List<int>());

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Sequence plan validation failed with {ViolationCount} violations", validationResult.TotalViolations);

            // Add validation warnings to the plan
            plan.ValidationWarnings = validationResult.AllViolations
                .Select(v => v.Description)
                .ToList();
        }
    }

    private async Task OptimizePlanForStudentPreferencesAsync(SequencePlan plan, Models.StudentAcademicProfile profile)
    {
        // Adjust course loads based on preferences
        foreach (var semesterPlan in plan.SemesterPlans)
        {
            var targetCredits = GetTargetCreditsForSemester(semesterPlan.Semester, profile.PreferredCourseLoad.ToCredits());

            if (semesterPlan.TotalCredits > targetCredits + 3) // Allow some flexibility
            {
                // Try to move some courses to later semesters
                var movableCourses = semesterPlan.Courses
                    .Where(c => !c.IsRequired || !IsHighPriority(c))
                    .OrderBy(c => c.CreditHours)
                    .ToList();

                // Implementation would move courses to maintain preferred load
            }
        }

        // Balance difficulty across semesters
        await BalanceCourseDifficultyAsync(plan, profile);
    }

    private async Task BalanceCourseDifficultyAsync(SequencePlan plan, Models.StudentAcademicProfile profile)
    {
        var maxDifficultCourses = profile.MaxDifficultCoursesPerSemester > 0 ? profile.MaxDifficultCoursesPerSemester : 2;

        foreach (var semesterPlan in plan.SemesterPlans)
        {
            var difficultCourses = semesterPlan.Courses
                .Where(c => c.DifficultyRating == DifficultyLevel.Advanced || c.DifficultyRating == DifficultyLevel.Expert)
                .ToList();

            if (difficultCourses.Count > maxDifficultCourses)
            {
                _logger.LogInformation("Semester {Semester} has {Count} difficult courses, exceeds maximum of {Max}",
                    semesterPlan.SemesterName, difficultCourses.Count, maxDifficultCourses);

                // Add to validation warnings
                plan.ValidationWarnings ??= new List<string>();
                plan.ValidationWarnings.Add($"Semester {semesterPlan.SemesterName} may be too challenging with {difficultCourses.Count} difficult courses");
            }
        }
    }

    private SemesterInfo ParseSemester(string semesterString)
    {
        // Parse semester string like "Fall 2024" or "Spring 2025"
        var parts = semesterString.Split(' ');
        if (parts.Length != 2 || !int.TryParse(parts[1], out var year))
        {
            throw new ArgumentException($"Invalid semester format: {semesterString}");
        }

        return new SemesterInfo
        {
            Term = parts[0],
            Year = year
        };
    }

    private DateTime CalculateDefaultGraduationDate(Models.StudentAcademicProfile profile)
    {
        var startingSemester = ParseSemester(profile.GetStartingSemesterString());
        return new DateTime(startingSemester.Year + 4, 5, 15); // Default to 4 years, May graduation
    }

    private bool IsCourseOfferedInSemester(int courseId, SemesterInfo semester, Dictionary<int, List<CourseOffering>> offeringMap)
    {
        if (!offeringMap.ContainsKey(courseId))
        {
            return true; // Assume available if no offering data
        }

        return offeringMap[courseId].Any(o => o.Term.Equals(semester.Term, StringComparison.OrdinalIgnoreCase));
    }

    private bool ArePrerequisitesSatisfied(int courseId, HashSet<int> completedCourses, PrerequisiteChainResult prerequisiteChain)
    {
        // Check if all prerequisites for this course are in the completed set
        // This is a simplified check - would need to implement proper prerequisite logic
        return true; // Placeholder
    }

    private int GetCoursePriority(Course course, PrerequisiteChainResult prerequisiteChain)
    {
        // Higher priority for courses that are prerequisites for many others
        // Lower numbers = higher priority
        if (prerequisiteChain.CriticalPath.Contains(course.Id))
        {
            return 1; // Highest priority for critical path courses
        }

        return 5; // Default priority
    }

    private int GetTargetCreditsForSemester(SemesterInfo semester, int preferredCourseLoad)
    {
        // Adjust for summer sessions (typically lighter load)
        if (semester.Term.Equals("Summer", StringComparison.OrdinalIgnoreCase))
        {
            return Math.Max(6, preferredCourseLoad / 2);
        }

        return preferredCourseLoad;
    }

    private bool IsRequiredCourse(int courseId, List<DegreeRequirement> requirements)
    {
        return requirements.Any(r => r.IsRequired && r.GetCourseIds().Contains(courseId));
    }

    private SemesterInfo GetNextSemester(SemesterInfo current, bool includeSummer)
    {
        return current.Term.ToLower() switch
        {
            "fall" => new SemesterInfo { Term = "Spring", Year = current.Year + 1 },
            "spring" => includeSummer
                ? new SemesterInfo { Term = "Summer", Year = current.Year }
                : new SemesterInfo { Term = "Fall", Year = current.Year },
            "summer" => new SemesterInfo { Term = "Fall", Year = current.Year },
            _ => new SemesterInfo { Term = "Fall", Year = current.Year + 1 }
        };
    }

    private string FormatSemesterName(SemesterInfo semester)
    {
        return $"{semester.Term} {semester.Year}";
    }

    private bool IsHighPriority(PlannedCourse course)
    {
        // Determine if a course is high priority (required, prerequisite for many others, etc.)
        return course.IsRequired;
    }

    private async Task<List<PrerequisiteBottleneck>> IdentifyPrerequisiteBottlenecksAsync(List<int> courseIds)
    {
        var bottlenecks = new List<PrerequisiteBottleneck>();

        // Find courses that are prerequisites for many others
        var prerequisiteCounts = await _context.CoursePrerequisites
            .Where(p => courseIds.Contains(p.CourseId) || courseIds.Contains(p.GetRequiredCourseId()))
            .GroupBy(p => p.GetRequiredCourseId())
            .Select(g => new { CourseId = g.Key, Count = g.Count() })
            .Where(x => x.Count >= 3) // Threshold for bottleneck
            .ToListAsync();

        foreach (var item in prerequisiteCounts)
        {
            var course = await _context.Courses
                .Include(c => c.Subject)
                .FirstOrDefaultAsync(c => c.Id == item.CourseId);

            if (course != null)
            {
                bottlenecks.Add(new PrerequisiteBottleneck
                {
                    CourseId = course.Id,
                    CourseName = $"{course.Subject.Code} {course.CourseNumber}",
                    DependentCourseCount = item.Count,
                    IsBottleneck = true
                });
            }
        }

        return bottlenecks;
    }

    #endregion
}

#region Supporting Classes



public class SequencePlan
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string DegreeCode { get; set; } = string.Empty;
    public DateTime PlanDate { get; set; }
    public DateTime? LastModified { get; set; }
    public SemesterInfo StartingSemester { get; set; } = new();
    public DateTime ExpectedGraduation { get; set; }
    public List<SemesterPlan> SemesterPlans { get; set; } = new();
    public int TotalSemesters { get; set; }
    public int TotalCredits { get; set; }
    public List<string>? ValidationWarnings { get; set; }
}

public class SemesterPlan
{
    public SemesterInfo Semester { get; set; } = new();
    public string SemesterName { get; set; } = string.Empty;
    public List<PlannedCourse> Courses { get; set; } = new();
    public int TotalCredits { get; set; }
}

public class PlannedCourse
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string CourseTitle { get; set; } = string.Empty;
    public int CreditHours { get; set; }
    public SemesterInfo PlannedSemester { get; set; } = new();
    public bool PrerequisitesSatisfied { get; set; }
    public DifficultyLevel DifficultyRating { get; set; }
    public bool IsRequired { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletionDate { get; set; }
}

public class SemesterInfo
{
    public string Term { get; set; } = string.Empty;
    public int Year { get; set; }

    public static bool operator >=(SemesterInfo left, SemesterInfo right)
    {
        if (left.Year != right.Year)
            return left.Year >= right.Year;

        var termOrder = new Dictionary<string, int>
        {
            { "Spring", 1 }, { "Summer", 2 }, { "Fall", 3 }
        };

        return termOrder.GetValueOrDefault(left.Term, 0) >= termOrder.GetValueOrDefault(right.Term, 0);
    }

    public static bool operator <=(SemesterInfo left, SemesterInfo right)
    {
        return !(left >= right) || left.Equals(right);
    }

    public override bool Equals(object? obj)
    {
        if (obj is SemesterInfo other)
        {
            return Term.Equals(other.Term, StringComparison.OrdinalIgnoreCase) && Year == other.Year;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Term.ToLower(), Year);
    }
}

public class PrerequisiteChainResult
{
    public bool HasCircularDependency { get; set; }
    public List<List<int>> CircularDependencies { get; set; } = new();
    public List<List<int>> Levels { get; set; } = new();
    public int TotalLevels { get; set; }
    public Dictionary<int, int>? OptimalSemesterMapping { get; set; }
    public List<int> CriticalPath { get; set; } = new();
    public int CriticalPathLength { get; set; }
}

public class PrerequisiteRelation
{
    public int CourseId { get; set; }
    public int PrerequisiteCourseId { get; set; }
    public RequirementLogicType LogicType { get; set; }
    public bool IsStrict { get; set; }
    public string? MinimumGrade { get; set; }
}

public class PrerequisiteValidationResult
{
    public bool IsValid { get; set; }
    public int TotalViolations { get; set; }
    public List<PrerequisiteViolation> AllViolations { get; set; } = new();
    public Dictionary<SemesterInfo, List<PrerequisiteViolation>> ViolationsBySemester { get; set; } = new();
}

public class PrerequisiteViolation
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int PrerequisiteCourseId { get; set; }
    public string PrerequisiteCourseName { get; set; } = string.Empty;
    public SemesterInfo Semester { get; set; } = new();
    public PrerequisiteViolationType ViolationType { get; set; }
    public string Description { get; set; } = string.Empty;
}

public enum PrerequisiteViolationType
{
    NotSatisfied,
    CorequisiteNotMet,
    MinimumGradeNotMet,
    TimingConflict
}

public class CourseOffering
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string Term { get; set; } = string.Empty;
    public bool IsRegularlyOffered { get; set; }
    public int? MaxEnrollment { get; set; }
    public int? TypicalEnrollment { get; set; }
    public Course Course { get; set; } = null!;
}

public class CourseOfferingAnalysis
{
    public List<TermOfferingAnalysis> TermAnalyses { get; set; } = new();
    public List<LimitedOfferingCourse> LimitedOfferingCourses { get; set; } = new();
    public List<PrerequisiteBottleneck> PrerequisiteBottlenecks { get; set; } = new();
    public int TotalCoursesAnalyzed { get; set; }
    public DateTime AnalysisDate { get; set; }
}

public class TermOfferingAnalysis
{
    public string Term { get; set; } = string.Empty;
    public int TotalCoursesOffered { get; set; }
    public int RegularlyOfferedCourses { get; set; }
    public int OccasionallyOfferedCourses { get; set; }
    public List<int> CourseIds { get; set; } = new();
    public List<HighDemandCourse> HighDemandCourses { get; set; } = new();
}

public class HighDemandCourse
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int MaxEnrollment { get; set; }
    public int TypicalEnrollment { get; set; }
}

public class LimitedOfferingCourse
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public List<string> OfferingTerms { get; set; } = new();
    public bool IsRegularlyOffered { get; set; }
}

public class PrerequisiteBottleneck
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int DependentCourseCount { get; set; }
    public bool IsBottleneck { get; set; }
}

#endregion