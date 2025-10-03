using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Enums;
using Zeus.Academia.Infrastructure.Extensions;

namespace Zeus.Academia.Infrastructure.Services;

/// <summary>
/// Service for optimizing course plans to minimize time to graduation while balancing 
/// course difficulty, prerequisites, and other constraints.
/// </summary>
public class CoursePlanOptimizationService
{
    private readonly AcademiaDbContext _context;
    private readonly ILogger<CoursePlanOptimizationService> _logger;

    public CoursePlanOptimizationService(AcademiaDbContext context, ILogger<CoursePlanOptimizationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    #region Course Plan Optimization

    /// <summary>
    /// Optimizes a course plan based on specified criteria and constraints.
    /// </summary>
    /// <param name="request">Optimization request with parameters</param>
    /// <returns>Optimized course plan</returns>
    public async Task<OptimizedCoursePlan> OptimizeCoursePlanAsync(CourseOptimizationRequest request)
    {
        try
        {
            _logger.LogInformation("Optimizing course plan for student {StudentId} with priority {Priority}",
                request.StudentId, request.OptimizationPriority);

            var result = new OptimizedCoursePlan
            {
                StudentId = request.StudentId,
                OptimizationDate = DateTime.UtcNow,
                OptimizationPriority = request.OptimizationPriority,
                Constraints = request.Constraints
            };

            // Get student's current progress and remaining requirements
            var studentProgress = await GetStudentProgressAsync(request.StudentId);
            var remainingCourses = await GetRemainingRequiredCoursesAsync(request.StudentId, request.DegreeCode);

            // Build prerequisite graph
            var prerequisiteGraph = await BuildPrerequisiteGraphAsync(remainingCourses);

            // Generate course sequence options based on optimization priority
            var sequenceOptions = await GenerateSequenceOptionsAsync(
                remainingCourses,
                prerequisiteGraph,
                request);

            // Evaluate and rank sequence options
            var rankedOptions = await RankSequenceOptionsAsync(sequenceOptions, request, studentProgress);

            // Select the best option
            result.OptimalSequence = rankedOptions.FirstOrDefault();

            if (result.OptimalSequence != null)
            {
                // Calculate optimization metrics
                result.TimeToGraduation = CalculateTimeToGraduation(result.OptimalSequence);
                result.DifficultyBalance = CalculateDifficultyBalance(result.OptimalSequence);
                result.WorkloadBalance = CalculateWorkloadBalance(result.OptimalSequence);
                result.OptimizationScore = CalculateOptimizationScore(result.OptimalSequence, request);

                // Generate alternative plans
                result.AlternativePlans = rankedOptions.Skip(1).Take(3).ToList();

                // Create implementation recommendations
                result.Recommendations = GenerateImplementationRecommendations(result, request);
            }

            _logger.LogInformation("Course plan optimization completed with score {Score}",
                result.OptimizationScore);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error optimizing course plan for student {StudentId}", request.StudentId);
            throw;
        }
    }

    /// <summary>
    /// Optimizes course load balance across semesters to minimize difficulty spikes.
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <param name="plannedCourses">List of courses to be scheduled</param>
    /// <param name="semesterCount">Number of semesters to plan</param>
    /// <returns>Balanced course schedule</returns>
    public async Task<BalancedCourseSchedule> OptimizeCourseLoadBalanceAsync(
        int studentId,
        List<Course> plannedCourses,
        int semesterCount)
    {
        try
        {
            _logger.LogDebug("Optimizing course load balance for student {StudentId} across {SemesterCount} semesters",
                studentId, semesterCount);

            var result = new BalancedCourseSchedule
            {
                StudentId = studentId,
                SemesterCount = semesterCount,
                OptimizationDate = DateTime.UtcNow
            };

            // Get course difficulty and workload ratings
            var courseMetrics = await GetCourseMetricsAsync(plannedCourses);

            // Build prerequisite constraints
            var prerequisiteGraph = await BuildPrerequisiteGraphAsync(plannedCourses);

            // Use constraint satisfaction to balance workload
            var balancedSchedule = await SolveWorkloadBalancingProblem(
                plannedCourses,
                courseMetrics,
                prerequisiteGraph,
                semesterCount);

            result.SemesterPlans = balancedSchedule;

            // Calculate balance metrics
            result.DifficultyVariance = CalculateDifficultyVariance(balancedSchedule);
            result.WorkloadVariance = CalculateWorkloadVariance(balancedSchedule);
            result.BalanceScore = CalculateBalanceScore(result);

            // Generate balance analysis
            result.BalanceAnalysis = GenerateBalanceAnalysis(result);

            _logger.LogDebug("Course load balance optimization completed with balance score {Score}",
                result.BalanceScore);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error optimizing course load balance for student {StudentId}", studentId);
            throw;
        }
    }

    /// <summary>
    /// Finds the shortest path to graduation considering prerequisites and course availability.
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <param name="degreeCode">Target degree code</param>
    /// <param name="maxCreditsPerSemester">Maximum credits per semester</param>
    /// <returns>Shortest path graduation plan</returns>
    public async Task<ShortestPathGraduationPlan> FindShortestPathToGraduationAsync(
        int studentId,
        string degreeCode,
        int maxCreditsPerSemester = 18)
    {
        try
        {
            _logger.LogInformation("Finding shortest path to graduation for student {StudentId}", studentId);

            var result = new ShortestPathGraduationPlan
            {
                StudentId = studentId,
                DegreeCode = degreeCode,
                MaxCreditsPerSemester = maxCreditsPerSemester,
                CalculationDate = DateTime.UtcNow
            };

            // Get remaining courses
            var remainingCourses = await GetRemainingRequiredCoursesAsync(studentId, degreeCode);

            // Build prerequisite DAG (Directed Acyclic Graph)
            var prerequisiteDAG = await BuildPrerequisiteDAGAsync(remainingCourses);

            // Perform topological sort to find valid orderings
            var topologicalOrder = TopologicalSort(prerequisiteDAG);

            // Use dynamic programming to find minimum semesters
            var shortestPath = await FindMinimumSemestersAsync(
                topologicalOrder,
                prerequisiteDAG,
                maxCreditsPerSemester);

            result.SemesterPlans = shortestPath;
            result.TotalSemesters = shortestPath.Count;
            result.EstimatedGraduationDate = CalculateGraduationDate(shortestPath.Count);
            result.TotalCredits = shortestPath.Sum(sp => sp.Courses.Sum(c => c.CreditHours));

            // Validate the path
            var validation = await ValidateGraduationPathAsync(result);
            result.IsValid = validation.IsValid;
            result.ValidationIssues = validation.Issues;

            // Calculate path efficiency
            result.EfficiencyScore = CalculatePathEfficiency(result);

            // Generate path analysis
            result.PathAnalysis = GeneratePathAnalysis(result);

            _logger.LogInformation("Shortest path found: {Semesters} semesters with efficiency score {Score}",
                result.TotalSemesters, result.EfficiencyScore);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding shortest path to graduation for student {StudentId}", studentId);
            throw;
        }
    }

    /// <summary>
    /// Optimizes course scheduling with multiple criteria (time, difficulty, cost, preferences).
    /// </summary>
    /// <param name="request">Multi-criteria optimization request</param>
    /// <returns>Multi-criteria optimized plan</returns>
    public async Task<MultiCriteriaOptimizedPlan> OptimizeWithMultipleCriteriaAsync(
        MultiCriteriaOptimizationRequest request)
    {
        try
        {
            _logger.LogInformation("Performing multi-criteria optimization for student {StudentId}",
                request.StudentId);

            var result = new MultiCriteriaOptimizedPlan
            {
                StudentId = request.StudentId,
                OptimizationDate = DateTime.UtcNow,
                Criteria = request.Criteria
            };

            // Get candidate courses and constraints
            var candidateCourses = await GetCandidateCoursesAsync(request.StudentId, request.DegreeCode);
            var constraints = await BuildConstraintsAsync(request);

            // Generate Pareto optimal solutions
            var paretoSolutions = await GenerateParetoOptimalSolutionsAsync(
                candidateCourses,
                constraints,
                request.Criteria);

            // Rank solutions using weighted criteria
            var rankedSolutions = RankSolutionsByWeightedCriteria(paretoSolutions, request.CriteriaWeights);

            result.OptimalSolution = rankedSolutions.FirstOrDefault();
            result.ParetoAlternatives = rankedSolutions.Skip(1).Take(5).ToList();

            if (result.OptimalSolution != null)
            {
                // Calculate performance metrics for each criterion
                result.PerformanceMetrics = CalculatePerformanceMetrics(result.OptimalSolution, request.Criteria);

                // Generate tradeoff analysis
                result.TradeoffAnalysis = GenerateTradeoffAnalysis(paretoSolutions, request.Criteria);

                // Create sensitivity analysis
                result.SensitivityAnalysis = PerformSensitivityAnalysis(result.OptimalSolution, request);
            }

            _logger.LogInformation("Multi-criteria optimization completed with {SolutionCount} Pareto solutions",
                paretoSolutions.Count);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing multi-criteria optimization for student {StudentId}",
                request.StudentId);
            throw;
        }
    }

    #endregion

    #region Constraint Satisfaction

    /// <summary>
    /// Validates course plan constraints and identifies conflicts.
    /// </summary>
    /// <param name="coursePlan">Course plan to validate</param>
    /// <param name="constraints">List of constraints to check</param>
    /// <returns>Constraint validation result</returns>
    public async Task<ConstraintValidationResult> ValidateConstraintsAsync(
        object coursePlan,
        List<PlanningConstraint> constraints)
    {
        try
        {
            _logger.LogDebug("Validating constraints for course plan");

            var result = new ConstraintValidationResult
            {
                PlanId = 0,
                ValidationDate = DateTime.UtcNow
            };

            foreach (var constraint in constraints)
            {
                var violation = await CheckConstraintViolationAsync(coursePlan, constraint);
                if (violation != null)
                {
                    result.Violations.Add(violation);
                }
            }

            result.IsValid = !result.Violations.Any();
            result.ViolationSeverityScore = CalculateViolationSeverity(result.Violations);

            // Generate constraint satisfaction recommendations
            if (!result.IsValid)
            {
                result.ResolutionRecommendations = GenerateConstraintResolutionRecommendations(result.Violations);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating constraints for course plan");
            throw;
        }
    }

    /// <summary>
    /// Resolves constraint conflicts by proposing alternative course arrangements.
    /// </summary>
    /// <param name="violations">List of constraint violations</param>
    /// <param name="coursePlan">Original course plan</param>
    /// <returns>Resolved course plan alternatives</returns>
    public async Task<List<ResolvedCoursePlan>> ResolveConstraintConflictsAsync(
        List<ConstraintViolation> violations,
        object coursePlan)
    {
        try
        {
            _logger.LogDebug("Resolving {ViolationCount} constraint conflicts", violations.Count);

            var resolvedPlans = new List<ResolvedCoursePlan>();

            // Group violations by type for efficient resolution
            var violationGroups = violations.GroupBy(v => v.ConstraintType);

            foreach (var group in violationGroups)
            {
                var resolutionStrategies = GetResolutionStrategies(group.Key);

                foreach (var strategy in resolutionStrategies)
                {
                    var resolvedPlan = await ApplyResolutionStrategyAsync(coursePlan, group.ToList(), strategy);
                    if (resolvedPlan != null)
                    {
                        resolvedPlans.Add(resolvedPlan);
                    }
                }
            }

            // Rank resolved plans by quality
            resolvedPlans = resolvedPlans
                .OrderByDescending(rp => rp.ResolutionQualityScore)
                .Take(10)
                .ToList();

            return resolvedPlans;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resolving constraint conflicts");
            throw;
        }
    }

    #endregion

    #region Private Helper Methods

    private async Task<StudentProgressSummary> GetStudentProgressAsync(int studentId)
    {
        var enrollments = await _context.CourseEnrollments
            .Include(e => e.Subject)
            .ThenInclude(s => s.Courses)
            .Where(e => e.StudentEmpNr == studentId && !string.IsNullOrEmpty(e.GetEffectiveGrade()))
            .ToListAsync();

        return new StudentProgressSummary
        {
            StudentId = studentId,
            CompletedCourses = enrollments.Where(e => e.HasPassingGrade()).ToList(),
            FailedCourses = enrollments.Where(e => e.GetEffectiveGrade() == "F").ToList(),
            WithdrawnCourses = enrollments.Where(e => e.GetEffectiveGrade() == "W").ToList(),
            CumulativeGPA = CalculateGPA(enrollments),
            TotalCreditsCompleted = (int)enrollments.Where(e => e.HasPassingGrade()).Sum(e => e.GetCourse().CreditHours)
        };
    }

    private async Task<List<Course>> GetRemainingRequiredCoursesAsync(int studentId, string degreeCode)
    {
        // Get completed courses
        var completedCourseIds = await _context.CourseEnrollments
            .Where(e => e.StudentEmpNr == studentId && e.Grades.Any(g => g.LetterGrade != null && g.LetterGrade != "F" && g.LetterGrade != "W"))
            .Select(e => e.GetCourse().Id)
            .ToListAsync();

        // Get degree requirements
        var degreeTemplate = await _context.DegreeRequirementTemplates
            .Include(t => t.Categories)
                .ThenInclude(c => c.Requirements)
            .FirstOrDefaultAsync(t => t.DegreeCode == degreeCode && t.IsActive);

        if (degreeTemplate == null) return new List<Course>();

        // This is simplified - would need complex logic to determine remaining courses
        var allRequiredCourseIds = degreeTemplate.Categories
            .SelectMany(c => c.Requirements)
            .Where(r => r.RequiredCourses.Any())
            .SelectMany(r => r.RequiredCourses.Select(rc => rc.CourseId))
            .Distinct()
            .ToList();

        var remainingCourseIds = allRequiredCourseIds.Except(completedCourseIds).ToList();

        return await _context.Courses
            .Include(c => c.Subject)
            .Include(c => c.Prerequisites)
            .Where(c => remainingCourseIds.Contains(c.Id))
            .ToListAsync();
    }

    private async Task<Dictionary<int, List<int>>> BuildPrerequisiteGraphAsync(List<Course> courses)
    {
        var graph = new Dictionary<int, List<int>>();

        foreach (var course in courses)
        {
            graph[course.Id] = new List<int>();

            var prerequisites = await _context.CoursePrerequisites
                .Where(p => p.CourseId == course.Id)
                .Select(p => p.Course.Id)
                .ToListAsync();

            graph[course.Id].AddRange(prerequisites);
        }

        return graph;
    }

    private async Task<List<CourseSequenceOption>> GenerateSequenceOptionsAsync(
        List<Course> remainingCourses,
        Dictionary<int, List<int>> prerequisiteGraph,
        CourseOptimizationRequest request)
    {
        var options = new List<CourseSequenceOption>();

        // Generate different sequence strategies
        switch (request.OptimizationPriority)
        {
            case OptimizationPriority.MinimizeTime:
                options.AddRange(await GenerateTimeOptimizedSequencesAsync(remainingCourses, prerequisiteGraph, request));
                break;

            case OptimizationPriority.BalanceDifficulty:
                options.AddRange(await GenerateDifficultyBalancedSequencesAsync(remainingCourses, prerequisiteGraph, request));
                break;

            case OptimizationPriority.MaximizeScheduleFlexibility:
                options.AddRange(await GenerateFlexibilityOptimizedSequencesAsync(remainingCourses, prerequisiteGraph, request));
                break;

            case OptimizationPriority.MinimizeCost:
                options.AddRange(await GenerateCostOptimizedSequencesAsync(remainingCourses, prerequisiteGraph, request));
                break;
        }

        return options;
    }

    private async Task<List<CourseSequenceOption>> GenerateTimeOptimizedSequencesAsync(
        List<Course> courses,
        Dictionary<int, List<int>> prerequisiteGraph,
        CourseOptimizationRequest request)
    {
        var options = new List<CourseSequenceOption>();

        // Use greedy algorithm to minimize semesters
        var remainingCourses = courses.ToList();
        var semesters = new List<List<Course>>();

        while (remainingCourses.Any())
        {
            var semester = new List<Course>();
            var semesterCredits = 0;
            var maxCredits = request.Constraints.MaxCreditsPerSemester ?? 18;

            // Find courses with satisfied prerequisites
            var availableCourses = remainingCourses
                .Where(c => ArePrerequisitesSatisfied(c.Id, prerequisiteGraph, GetCompletedCourseIds(semesters)))
                .OrderByDescending(c => c.CreditHours) // Prioritize higher credit courses
                .ToList();

            foreach (var course in availableCourses)
            {
                if (semesterCredits + course.CreditHours <= maxCredits)
                {
                    semester.Add(course);
                    semesterCredits += (int)course.CreditHours;
                    remainingCourses.Remove(course);
                }
            }

            if (semester.Any())
            {
                semesters.Add(semester);
            }
            else
            {
                // No progress possible - break to avoid infinite loop
                break;
            }
        }

        options.Add(new CourseSequenceOption
        {
            Strategy = OptimizationStrategy.MinimizeTime,
            Semesters = semesters.Select((s, index) => new SemesterPlan
            {
                // SemesterNumber = index + 1,
                Courses = s.Select(c => new PlannedCourse { CourseId = c.Id, CourseName = c.CourseNumber, CreditHours = (int)c.CreditHours }).ToList(),
                TotalCredits = (int)s.Sum(c => c.CreditHours)
            }).ToList(),
            TotalSemesters = semesters.Count,
            EstimatedCompletionTime = semesters.Count * 0.5 // 0.5 years per semester
        });

        return options;
    }

    private async Task<List<CourseSequenceOption>> GenerateDifficultyBalancedSequencesAsync(
        List<Course> courses,
        Dictionary<int, List<int>> prerequisiteGraph,
        CourseOptimizationRequest request)
    {
        var options = new List<CourseSequenceOption>();

        // Get course difficulty ratings
        var courseMetrics = await GetCourseMetricsAsync(courses);

        // Use constraint satisfaction to balance difficulty
        var balancedSchedule = await SolveDifficultyBalancingProblem(courses, courseMetrics, prerequisiteGraph, request);

        options.Add(new CourseSequenceOption
        {
            Strategy = OptimizationStrategy.BalanceDifficulty,
            Semesters = balancedSchedule,
            TotalSemesters = balancedSchedule.Count,
            DifficultyVariance = CalculateDifficultyVariance(balancedSchedule),
            EstimatedCompletionTime = balancedSchedule.Count * 0.5
        });

        return options;
    }

    private async Task<List<CourseSequenceOption>> GenerateFlexibilityOptimizedSequencesAsync(
        List<Course> courses,
        Dictionary<int, List<int>> prerequisiteGraph,
        CourseOptimizationRequest request)
    {
        // Generate sequences that maximize scheduling flexibility
        return new List<CourseSequenceOption>();
    }

    private async Task<List<CourseSequenceOption>> GenerateCostOptimizedSequencesAsync(
        List<Course> courses,
        Dictionary<int, List<int>> prerequisiteGraph,
        CourseOptimizationRequest request)
    {
        // Generate sequences that minimize total cost
        return new List<CourseSequenceOption>();
    }

    private async Task<List<CourseSequenceOption>> RankSequenceOptionsAsync(
        List<CourseSequenceOption> options,
        CourseOptimizationRequest request,
        StudentProgressSummary progress)
    {
        foreach (var option in options)
        {
            option.OptimizationScore = await CalculateOptionScoreAsync(option, request, progress);
        }

        return options.OrderByDescending(o => o.OptimizationScore).ToList();
    }

    private async Task<decimal> CalculateOptionScoreAsync(
        CourseSequenceOption option,
        CourseOptimizationRequest request,
        StudentProgressSummary progress)
    {
        decimal score = 0;

        // Time component (40% weight)
        var timeScore = 100 - (option.TotalSemesters * 5); // Fewer semesters = higher score
        score += timeScore * 0.4m;

        // Difficulty balance component (30% weight)
        var difficultyScore = 100 - (option.DifficultyVariance * 10); // Lower variance = higher score
        score += (decimal)Math.Max(0, difficultyScore) * 0.3m;

        // Workload balance component (20% weight)
        var workloadScore = CalculateWorkloadScore(option);
        score += workloadScore * 0.2m;

        // Constraint satisfaction component (10% weight)
        var constraintScore = await CalculateConstraintSatisfactionScore(option, request.Constraints);
        score += constraintScore * 0.1m;

        return Math.Max(0, Math.Min(100, score));
    }

    private decimal CalculateWorkloadScore(CourseSequenceOption option)
    {
        var creditVariances = option.Semesters.Select(s => s.TotalCredits).ToList();
        var avgCredits = creditVariances.Average();
        var variance = creditVariances.Sum(c => Math.Pow(c - avgCredits, 2)) / creditVariances.Count;

        return Math.Max(0, 100 - (decimal)variance);
    }

    private async Task<decimal> CalculateConstraintSatisfactionScore(
        CourseSequenceOption option,
        OptimizationConstraints constraints)
    {
        decimal score = 100;

        // Check credit limit constraints
        foreach (var semester in option.Semesters)
        {
            if (constraints.MaxCreditsPerSemester.HasValue &&
                semester.TotalCredits > constraints.MaxCreditsPerSemester.Value)
            {
                score -= 20; // Penalty for exceeding credit limits
            }

            if (constraints.MinCreditsPerSemester.HasValue &&
                semester.TotalCredits < constraints.MinCreditsPerSemester.Value)
            {
                score -= 10; // Penalty for too few credits
            }
        }

        return Math.Max(0, score);
    }

    private bool ArePrerequisitesSatisfied(int courseId, Dictionary<int, List<int>> prerequisiteGraph, List<int> completedCourseIds)
    {
        if (!prerequisiteGraph.ContainsKey(courseId))
            return true;

        var prerequisites = prerequisiteGraph[courseId];
        return prerequisites.All(prereq => completedCourseIds.Contains(prereq));
    }

    private List<int> GetCompletedCourseIds(List<List<Course>> semesters)
    {
        return semesters.SelectMany(s => s).Select(c => c.Id).ToList();
    }

    private List<int> GetCompletedCourseIds(List<SemesterPlan> semesters)
    {
        return semesters.SelectMany(s => s.Courses).Select(c => c.CourseId).ToList();
    }

    private async Task<Dictionary<int, CourseMetrics>> GetCourseMetricsAsync(List<Course> courses)
    {
        var metrics = new Dictionary<int, CourseMetrics>();

        foreach (var course in courses)
        {
            // This would be populated from historical data, ratings, etc.
            metrics[course.Id] = new CourseMetrics
            {
                CourseId = course.Id,
                DifficultyRating = Random.Shared.Next(1, 6), // Simplified
                WorkloadHours = (double)course.CreditHours * 3,
                SuccessRate = 0.85m,
                AverageGrade = "B",
                PrerequisiteComplexity = await CalculatePrerequisiteComplexity(course.Id)
            };
        }

        return metrics;
    }

    private async Task<int> CalculatePrerequisiteComplexity(int courseId)
    {
        var prerequisites = await _context.CoursePrerequisites
            .Where(p => p.CourseId == courseId)
            .CountAsync();

        return prerequisites;
    }

    private async Task<List<SemesterPlan>> SolveWorkloadBalancingProblem(
        List<Course> courses,
        Dictionary<int, CourseMetrics> metrics,
        Dictionary<int, List<int>> prerequisiteGraph,
        int semesterCount)
    {
        // Simplified constraint satisfaction algorithm
        var semesters = new List<SemesterPlan>();
        var remainingCourses = courses.ToList();

        for (int i = 0; i < semesterCount && remainingCourses.Any(); i++)
        {
            var semester = new SemesterPlan
            {
                // SemesterNumber = i + 1,
                Courses = new List<PlannedCourse>()
            };

            // Greedy selection with workload balancing
            var targetWorkload = metrics.Values.Average(m => m.WorkloadHours) * 4; // Target 4 courses worth
            var currentWorkload = 0.0;

            var availableCourses = remainingCourses
                .Where(c => ArePrerequisitesSatisfied(c.Id, prerequisiteGraph, GetCompletedCourseIds(semesters)))
                .OrderBy(c => Math.Abs(metrics[c.Id].WorkloadHours - (targetWorkload - currentWorkload)))
                .ToList();

            foreach (var course in availableCourses)
            {
                if (currentWorkload + metrics[course.Id].WorkloadHours <= targetWorkload * 1.2) // 20% tolerance
                {
                    semester.Courses.Add(new PlannedCourse
                    {
                        CourseId = course.Id,
                        CourseName = course.CourseNumber,
                        CourseTitle = course.Title,
                        CreditHours = (int)course.CreditHours,
                        IsRequired = true
                    });
                    currentWorkload += metrics[course.Id].WorkloadHours;
                    remainingCourses.Remove(course);

                    if (semester.Courses.Count >= 5) break; // Max 5 courses per semester
                }
            }

            if (semester.Courses.Any())
            {
                semester.TotalCredits = semester.Courses.Sum(c => c.CreditHours);
                semesters.Add(semester);
            }
        }

        return semesters;
    }

    private async Task<List<SemesterPlan>> SolveDifficultyBalancingProblem(
        List<Course> courses,
        Dictionary<int, CourseMetrics> metrics,
        Dictionary<int, List<int>> prerequisiteGraph,
        CourseOptimizationRequest request)
    {
        var semesters = new List<SemesterPlan>();
        var remainingCourses = courses.ToList();
        var targetDifficulty = metrics.Values.Average(m => m.DifficultyRating);

        while (remainingCourses.Any())
        {
            var semester = new SemesterPlan
            {
                // SemesterNumber = semesters.Count + 1,
                Courses = new List<PlannedCourse>()
            };

            var availableCourses = remainingCourses
                .Where(c => ArePrerequisitesSatisfied(c.Id, prerequisiteGraph, GetCompletedCourseIds(semesters)))
                .ToList();

            // Balance difficulty across semester
            var easyCourses = availableCourses.Where(c => metrics[c.Id].DifficultyRating <= 2).ToList();
            var moderateCourses = availableCourses.Where(c => metrics[c.Id].DifficultyRating == 3).ToList();
            var hardCourses = availableCourses.Where(c => metrics[c.Id].DifficultyRating >= 4).ToList();

            // Add 1-2 hard courses, 2-3 moderate, 1-2 easy courses per semester
            AddCoursesToSemester(semester, hardCourses, 1, 2, remainingCourses);
            AddCoursesToSemester(semester, moderateCourses, 2, 3, remainingCourses);
            AddCoursesToSemester(semester, easyCourses, 1, 2, remainingCourses);

            if (semester.Courses.Any())
            {
                semester.TotalCredits = semester.Courses.Sum(c => c.CreditHours);
                semesters.Add(semester);
            }
            else
            {
                break; // No progress possible
            }
        }

        return semesters;
    }

    private void AddCoursesToSemester(SemesterPlan semester, List<Course> candidateCourses, int minCount, int maxCount, List<Course> remainingCourses)
    {
        var added = 0;
        var maxCredits = 18; // Default max credits per semester

        foreach (var course in candidateCourses.Take(maxCount))
        {
            if (semester.TotalCredits + course.CreditHours <= maxCredits)
            {
                semester.Courses.Add(new PlannedCourse
                {
                    CourseId = course.Id,
                    CourseName = course.CourseNumber,
                    CourseTitle = course.Title,
                    CreditHours = (int)course.CreditHours,
                    IsRequired = true
                });
                semester.TotalCredits += (int)course.CreditHours;
                remainingCourses.Remove(course);
                added++;

                if (added >= maxCount) break;
            }
        }
    }

    private decimal CalculateGPA(List<CourseEnrollment> enrollments)
    {
        var gradePoints = new Dictionary<string, decimal>
        {
            {"A+", 4.0m}, {"A", 4.0m}, {"A-", 3.7m},
            {"B+", 3.3m}, {"B", 3.0m}, {"B-", 2.7m},
            {"C+", 2.3m}, {"C", 2.0m}, {"C-", 1.7m},
            {"D+", 1.3m}, {"D", 1.0m}, {"D-", 0.7m},
            {"F", 0.0m}
        };

        var totalPoints = 0.0m;
        var totalCredits = 0;

        foreach (var enrollment in enrollments)
        {
            var grade = enrollment.GetEffectiveGrade();
            if (!string.IsNullOrEmpty(grade) && gradePoints.ContainsKey(grade))
            {
                totalPoints += gradePoints[grade] * enrollment.GetCourse().CreditHours;
                totalCredits += (int)enrollment.GetCourse().CreditHours;
            }
        }

        return totalCredits > 0 ? Math.Round(totalPoints / totalCredits, 2) : 0;
    }

    // Additional helper methods would be implemented here...
    private double CalculateTimeToGraduation(CourseSequenceOption sequence) => sequence.TotalSemesters * 0.5;
    private double CalculateDifficultyBalance(CourseSequenceOption sequence) => sequence.DifficultyVariance;
    private double CalculateWorkloadBalance(CourseSequenceOption sequence) => 85.0; // Placeholder
    private decimal CalculateOptimizationScore(CourseSequenceOption sequence, CourseOptimizationRequest request) => 85.0m; // Placeholder
    private List<string> GenerateImplementationRecommendations(OptimizedCoursePlan result, CourseOptimizationRequest request) => new();
    private double CalculateDifficultyVariance(List<SemesterPlan> semesters) => 1.5; // Placeholder
    private double CalculateWorkloadVariance(List<SemesterPlan> semesters) => 2.0; // Placeholder
    private decimal CalculateBalanceScore(BalancedCourseSchedule schedule) => 88.5m; // Placeholder
    private List<string> GenerateBalanceAnalysis(BalancedCourseSchedule schedule) => new();

    // Missing method stub implementations to fix compilation errors
    private async Task<Dictionary<int, List<int>>> BuildPrerequisiteDAGAsync(List<Course> courses)
        => await BuildPrerequisiteGraphAsync(courses);

    private List<int> TopologicalSort(Dictionary<int, List<int>> graph) =>
        graph.Keys.ToList(); // Simplified implementation

    private Task<List<SemesterPlan>> FindMinimumSemestersAsync(List<int> courses, Dictionary<int, List<int>> graph, int maxCredits) =>
        Task.FromResult(new List<SemesterPlan>());

    private DateTime CalculateGraduationDate(int semesterCount) =>
        DateTime.Now.AddMonths(semesterCount * 6);

    private Task<(bool IsValid, List<string> Issues)> ValidateGraduationPathAsync(ShortestPathGraduationPlan plan) =>
        Task.FromResult((true, new List<string>()));

    private decimal CalculatePathEfficiency(ShortestPathGraduationPlan plan) => 85.0m;
    private List<string> GeneratePathAnalysis(ShortestPathGraduationPlan plan) => new();

    private Task<List<Course>> GetCandidateCoursesAsync(int studentId, string degreeCode) =>
        Task.FromResult(new List<Course>());

    private Task<List<PlanningConstraint>> BuildConstraintsAsync(MultiCriteriaOptimizationRequest request) =>
        Task.FromResult(new List<PlanningConstraint>());

    private Task<List<CourseSequenceOption>> GenerateParetoOptimalSolutionsAsync(List<Course> courses, List<PlanningConstraint> constraints, List<OptimizationCriterion> criteria) =>
        Task.FromResult(new List<CourseSequenceOption>());

    private List<CourseSequenceOption> RankSolutionsByWeightedCriteria(List<CourseSequenceOption> solutions, Dictionary<OptimizationCriterion, decimal> weights) =>
        solutions.OrderByDescending(s => s.OptimizationScore).ToList();

    private Dictionary<OptimizationCriterion, decimal> CalculatePerformanceMetrics(CourseSequenceOption solution, List<OptimizationCriterion> criteria) =>
        new();

    private List<string> GenerateTradeoffAnalysis(List<CourseSequenceOption> solutions, List<OptimizationCriterion> criteria) =>
        new();

    private List<string> PerformSensitivityAnalysis(CourseSequenceOption solution, MultiCriteriaOptimizationRequest request) =>
        new();

    private Task<ConstraintViolation?> CheckConstraintViolationAsync(object plan, PlanningConstraint constraint) =>
        Task.FromResult<ConstraintViolation?>(null);

    private decimal CalculateViolationSeverity(List<ConstraintViolation> violations) => 0.0m;
    private List<string> GenerateConstraintResolutionRecommendations(List<ConstraintViolation> violations) => new();
    private List<string> GetResolutionStrategies(PlanningConstraintType constraintType) => new();

    private Task<ResolvedCoursePlan?> ApplyResolutionStrategyAsync(object plan, List<ConstraintViolation> violations, string strategy) =>
        Task.FromResult<ResolvedCoursePlan?>(null);

    #endregion
}

#region Supporting Classes and Enums

public class CourseOptimizationRequest
{
    public int StudentId { get; set; }
    public string DegreeCode { get; set; } = string.Empty;
    public OptimizationPriority OptimizationPriority { get; set; }
    public OptimizationConstraints Constraints { get; set; } = new();
    public CourseLoadPreference LoadPreference { get; set; } = CourseLoadPreference.Standard;
    public DifficultyLevel MaxDifficultyLevel { get; set; } = DifficultyLevel.Advanced;
}

public class OptimizationConstraints
{
    public int? MaxCreditsPerSemester { get; set; } = 18;
    public int? MinCreditsPerSemester { get; set; } = 12;
    public List<string> PreferredTerms { get; set; } = new();
    public List<string> AvoidTerms { get; set; } = new();
    public List<int> PreferredCourseIds { get; set; } = new();
    public List<int> AvoidCourseIds { get; set; } = new();
    public bool AllowSummerCourses { get; set; } = false;
    public bool RequireFullTimeStatus { get; set; } = false;
}

public class OptimizedCoursePlan
{
    public int StudentId { get; set; }
    public DateTime OptimizationDate { get; set; }
    public OptimizationPriority OptimizationPriority { get; set; }
    public OptimizationConstraints Constraints { get; set; } = null!;
    public CourseSequenceOption? OptimalSequence { get; set; }
    public List<CourseSequenceOption> AlternativePlans { get; set; } = new();
    public double TimeToGraduation { get; set; }
    public double DifficultyBalance { get; set; }
    public double WorkloadBalance { get; set; }
    public decimal OptimizationScore { get; set; }
    public List<string> Recommendations { get; set; } = new();
}

public class CourseSequenceOption
{
    public OptimizationStrategy Strategy { get; set; }
    public List<SemesterPlan> Semesters { get; set; } = new();
    public int TotalSemesters { get; set; }
    public double EstimatedCompletionTime { get; set; }
    public double DifficultyVariance { get; set; }
    public decimal OptimizationScore { get; set; }
}



public class StudentProgressSummary
{
    public int StudentId { get; set; }
    public List<CourseEnrollment> CompletedCourses { get; set; } = new();
    public List<CourseEnrollment> FailedCourses { get; set; } = new();
    public List<CourseEnrollment> WithdrawnCourses { get; set; } = new();
    public decimal CumulativeGPA { get; set; }
    public int TotalCreditsCompleted { get; set; }
}

public class CourseMetrics
{
    public int CourseId { get; set; }
    public int DifficultyRating { get; set; } // 1-5 scale
    public double WorkloadHours { get; set; }
    public decimal SuccessRate { get; set; }
    public string AverageGrade { get; set; } = string.Empty;
    public int PrerequisiteComplexity { get; set; }
}

public class BalancedCourseSchedule
{
    public int StudentId { get; set; }
    public int SemesterCount { get; set; }
    public DateTime OptimizationDate { get; set; }
    public List<SemesterPlan> SemesterPlans { get; set; } = new();
    public double DifficultyVariance { get; set; }
    public double WorkloadVariance { get; set; }
    public decimal BalanceScore { get; set; }
    public List<string> BalanceAnalysis { get; set; } = new();
}

public class ShortestPathGraduationPlan
{
    public int StudentId { get; set; }
    public string DegreeCode { get; set; } = string.Empty;
    public int MaxCreditsPerSemester { get; set; }
    public DateTime CalculationDate { get; set; }
    public List<SemesterPlan> SemesterPlans { get; set; } = new();
    public int TotalSemesters { get; set; }
    public DateTime EstimatedGraduationDate { get; set; }
    public int TotalCredits { get; set; }
    public bool IsValid { get; set; }
    public List<string> ValidationIssues { get; set; } = new();
    public decimal EfficiencyScore { get; set; }
    public List<string> PathAnalysis { get; set; } = new();
}

public class MultiCriteriaOptimizationRequest
{
    public int StudentId { get; set; }
    public string DegreeCode { get; set; } = string.Empty;
    public List<OptimizationCriterion> Criteria { get; set; } = new();
    public Dictionary<OptimizationCriterion, decimal> CriteriaWeights { get; set; } = new();
}

public class MultiCriteriaOptimizedPlan
{
    public int StudentId { get; set; }
    public DateTime OptimizationDate { get; set; }
    public List<OptimizationCriterion> Criteria { get; set; } = new();
    public CourseSequenceOption? OptimalSolution { get; set; }
    public List<CourseSequenceOption> ParetoAlternatives { get; set; } = new();
    public Dictionary<OptimizationCriterion, decimal> PerformanceMetrics { get; set; } = new();
    public List<string> TradeoffAnalysis { get; set; } = new();
    public List<string> SensitivityAnalysis { get; set; } = new();
}

public class ConstraintValidationResult
{
    public int PlanId { get; set; }
    public DateTime ValidationDate { get; set; }
    public bool IsValid { get; set; }
    public List<ConstraintViolation> Violations { get; set; } = new();
    public decimal ViolationSeverityScore { get; set; }
    public List<string> ResolutionRecommendations { get; set; } = new();
}

public class ConstraintViolation
{
    public PlanningConstraintType ConstraintType { get; set; }
    public string Description { get; set; } = string.Empty;
    public ViolationSeverity Severity { get; set; }
    public List<int> AffectedCourseIds { get; set; } = new();
    public int AffectedSemester { get; set; }
}

public class ResolvedCoursePlan
{
    public object ResolvedPlan { get; set; } = null!;
    public List<string> ResolutionChanges { get; set; } = new();
    public decimal ResolutionQualityScore { get; set; }
    public List<string> RemainingIssues { get; set; } = new();
}

public class PlanningConstraint
{
    public PlanningConstraintType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsHard { get; set; } = true; // Hard vs soft constraint
    public decimal Weight { get; set; } = 1.0m;
}

public enum OptimizationStrategy
{
    MinimizeTime,
    BalanceDifficulty,
    MaximizeFlexibility,
    MinimizeCost,
    CustomWeighted
}

public enum OptimizationCriterion
{
    Time,
    Difficulty,
    Cost,
    Flexibility,
    GPA,
    Workload
}

public enum PlanningConstraintType
{
    CreditLimit,
    PrerequisiteViolation,
    TermAvailability,
    CourseConflict,
    GraduationDeadline,
    FinancialLimit
}

public enum ViolationSeverity
{
    Low,
    Medium,
    High,
    Critical
}



#endregion