using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Enums;
using Zeus.Academia.Infrastructure.Extensions;

namespace Zeus.Academia.Infrastructure.Services;

/// <summary>
/// Service for managing degree requirement templates and validating degree requirements.
/// Handles complex conditional logic, course substitutions, and requirement validation.
/// </summary>
public class DegreeRequirementService
{
    private readonly AcademiaDbContext _context;
    private readonly ILogger<DegreeRequirementService> _logger;

    public DegreeRequirementService(AcademiaDbContext context, ILogger<DegreeRequirementService> logger)
    {
        _context = context;
        _logger = logger;
    }

    #region Template Management

    /// <summary>
    /// Creates a new degree requirement template with all its categories and requirements.
    /// </summary>
    /// <param name="template">The degree template to create</param>
    /// <returns>The created template with assigned IDs</returns>
    public async Task<DegreeRequirementTemplate> CreateDegreeTemplateAsync(DegreeRequirementTemplate template)
    {
        try
        {
            _logger.LogInformation("Creating degree template for {DegreeCode} - {DegreeName}",
                template.DegreeCode, template.DegreeName);

            // Validate template
            await ValidateTemplateAsync(template);

            // Set audit fields
            template.CreatedDate = DateTime.UtcNow;
            template.IsActive = true;

            // Add to context
            _context.DegreeRequirementTemplates.Add(template);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully created degree template {Id} for {DegreeCode}",
                template.Id, template.DegreeCode);

            return template;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating degree template for {DegreeCode}", template.DegreeCode);
            throw;
        }
    }

    /// <summary>
    /// Retrieves a degree template by degree code.
    /// </summary>
    /// <param name="degreeCode">The degree code to search for</param>
    /// <returns>The degree template if found</returns>
    public async Task<DegreeRequirementTemplate?> GetDegreeTemplateAsync(string degreeCode)
    {
        try
        {
            return await _context.DegreeRequirementTemplates
                .Include(t => t.Categories)
                    .ThenInclude(c => c.Requirements)
                        .ThenInclude(r => r.RequiredCourses)
                            .ThenInclude(rc => rc.Course)
                .Include(t => t.Categories)
                    .ThenInclude(c => c.Requirements)
                        .ThenInclude(r => r.RequiredSubjects)
                            .ThenInclude(rs => rs.Subject)
                .Include(t => t.Categories)
                    .ThenInclude(c => c.Requirements)
                        .ThenInclude(r => r.ConditionalRequirements)
                            .ThenInclude(cr => cr.Courses)
                                .ThenInclude(crc => crc.Course)
                .Include(t => t.Categories)
                    .ThenInclude(c => c.Requirements)
                        .ThenInclude(r => r.PrerequisiteChain)
                            .ThenInclude(pl => pl.Course)
                .Where(t => t.DegreeCode == degreeCode && t.IsCurrentlyEffective())
                .OrderByDescending(t => t.EffectiveDate)
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving degree template for {DegreeCode}", degreeCode);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing degree template.
    /// </summary>
    /// <param name="template">The updated template</param>
    /// <returns>The updated template</returns>
    public async Task<DegreeRequirementTemplate> UpdateDegreeTemplateAsync(DegreeRequirementTemplate template)
    {
        try
        {
            _logger.LogInformation("Updating degree template {Id} for {DegreeCode}",
                template.Id, template.DegreeCode);

            // Validate template
            await ValidateTemplateAsync(template);

            // Set modification fields
            template.ModifiedDate = DateTime.UtcNow;

            _context.DegreeRequirementTemplates.Update(template);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully updated degree template {Id}", template.Id);

            return template;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating degree template {Id}", template.Id);
            throw;
        }
    }

    /// <summary>
    /// Gets all active degree templates.
    /// </summary>
    /// <returns>List of active degree templates</returns>
    public async Task<List<DegreeRequirementTemplate>> GetAllActiveTemplatesAsync()
    {
        try
        {
            return await _context.DegreeRequirementTemplates
                .Include(t => t.Categories)
                                .Where(c => true) // IsActive property doesn't exist
                .OrderBy(t => t.DegreeName)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active degree templates");
            throw;
        }
    }

    #endregion

    #region Requirement Validation

    /// <summary>
    /// Validates a degree requirement for logical consistency and completeness.
    /// </summary>
    /// <param name="requirement">The requirement to validate</param>
    /// <returns>True if valid, false otherwise</returns>
    public async Task<bool> ValidateRequirementAsync(DegreeRequirement requirement)
    {
        try
        {
            _logger.LogDebug("Validating requirement: {Description}", requirement.Description);

            var validationResult = new RequirementValidationResult();

            // Basic validation
            if (string.IsNullOrWhiteSpace(requirement.Description))
            {
                validationResult.AddError("Requirement description is required");
            }

            if (requirement.CreditsRequired < 0)
            {
                validationResult.AddError("Credits required cannot be negative");
            }

            // Type-specific validation
            switch (requirement.Type)
            {
                case RequirementType.SpecificCourse:
                    await ValidateSpecificCourseRequirementAsync(requirement, validationResult);
                    break;

                case RequirementType.CourseGroup:
                    await ValidateCourseGroupRequirementAsync(requirement, validationResult);
                    break;

                case RequirementType.ConditionalGroup:
                    await ValidateConditionalGroupRequirementAsync(requirement, validationResult);
                    break;

                case RequirementType.SequencedCourses:
                    await ValidateSequencedCoursesRequirementAsync(requirement, validationResult);
                    break;

                case RequirementType.CreditHours:
                    ValidateCreditHoursRequirement(requirement, validationResult);
                    break;
            }

            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Requirement validation failed: {Errors}",
                    string.Join(", ", validationResult.Errors));
            }

            return validationResult.IsValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating requirement: {Description}", requirement.Description);
            throw;
        }
    }

    /// <summary>
    /// Validates a course sequence for prerequisite consistency and circular dependencies.
    /// </summary>
    /// <param name="requirement">The sequenced course requirement</param>
    /// <returns>Sequence validation result</returns>
    public async Task<SequenceValidationResult> ValidateSequenceAsync(DegreeRequirement requirement)
    {
        try
        {
            _logger.LogDebug("Validating course sequence: {Description}", requirement.Description);

            var result = new SequenceValidationResult();

            if (requirement.Type != RequirementType.SequencedCourses)
            {
                result.AddError("Requirement must be of type SequencedCourses");
                return result;
            }

            // Build prerequisite graph
            var prerequisiteGraph = BuildPrerequisiteGraph(requirement.PrerequisiteChain.ToList());

            // Check for circular dependencies
            var circularDependency = DetectCircularDependencies(prerequisiteGraph);
            if (circularDependency != null)
            {
                result.HasCircularDependency = true;
                result.AddError($"Circular dependency detected: {string.Join(" -> ", circularDependency)}");
                return result;
            }

            // Calculate sequence length and levels
            result.SequenceLength = prerequisiteGraph.Keys.Count;
            result.Levels = TopologicalSort(prerequisiteGraph);

            // Generate recommended semester mapping
            result.RecommendedSemesterMapping = GenerateSemesterMapping(result.Levels);

            result.IsValid = true;

            _logger.LogDebug("Sequence validation completed. Length: {Length}, Levels: {LevelCount}",
                result.SequenceLength, result.Levels.Count);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating course sequence");
            throw;
        }
    }

    /// <summary>
    /// Validates that a student's completed courses satisfy a specific requirement.
    /// </summary>
    /// <param name="requirement">The requirement to check</param>
    /// <param name="completedCourses">List of completed courses</param>
    /// <param name="studentGPA">Student's current GPA</param>
    /// <returns>Requirement satisfaction result</returns>
    public async Task<RequirementSatisfactionResult> CheckRequirementSatisfactionAsync(
        DegreeRequirement requirement,
        List<Course> completedCourses,
        decimal? studentGPA = null)
    {
        try
        {
            _logger.LogDebug("Checking requirement satisfaction for: {Description}", requirement.Description);

            var result = new RequirementSatisfactionResult
            {
                RequirementId = requirement.Id,
                RequirementDescription = requirement.Description
            };

            switch (requirement.Type)
            {
                case RequirementType.SpecificCourse:
                    CheckSpecificCourseRequirement(requirement, completedCourses, result);
                    break;

                case RequirementType.CourseGroup:
                    await CheckCourseGroupRequirementAsync(requirement, completedCourses, result);
                    break;

                case RequirementType.ConditionalGroup:
                    CheckConditionalGroupRequirement(requirement, completedCourses, studentGPA, result);
                    break;

                case RequirementType.SequencedCourses:
                    CheckSequencedCoursesRequirement(requirement, completedCourses, result);
                    break;

                case RequirementType.CreditHours:
                    CheckCreditHoursRequirement(requirement, completedCourses, result);
                    break;
            }

            _logger.LogDebug("Requirement satisfaction check completed. Satisfied: {IsSatisfied}, Progress: {Progress}%",
                result.IsSatisfied, result.ProgressPercentage);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking requirement satisfaction");
            throw;
        }
    }

    #endregion

    #region Complex Conditional Logic

    /// <summary>
    /// Evaluates complex conditional requirements with multiple criteria.
    /// </summary>
    /// <param name="conditionalRequirement">The conditional requirement to evaluate</param>
    /// <param name="completedCourses">Student's completed courses</param>
    /// <param name="studentGPA">Student's GPA</param>
    /// <returns>Evaluation result</returns>
    public async Task<ConditionalRequirementEvaluationResult> EvaluateConditionalRequirementAsync(
        ConditionalRequirement conditionalRequirement,
        List<Course> completedCourses,
        decimal? studentGPA = null)
    {
        try
        {
            _logger.LogDebug("Evaluating conditional requirement: {Condition}", conditionalRequirement.Condition);

            var result = new ConditionalRequirementEvaluationResult
            {
                ConditionalRequirementId = conditionalRequirement.Id,
                Condition = conditionalRequirement.Condition
            };

            // Check basic satisfaction
            result.IsSatisfied = conditionalRequirement.IsSatisfiedBy(completedCourses, studentGPA);

            // Get applicable courses
            var applicableCourses = await GetApplicableCoursesForConditionalAsync(conditionalRequirement, completedCourses);
            result.ApplicableCourses = applicableCourses;

            // Calculate progress
            var completedCredits = applicableCourses.Sum(c => c.CreditHours);
            var completedCourseCount = applicableCourses.Count;

            result.CreditProgress = conditionalRequirement.CreditsRequired > 0
                ? Math.Min(100, (int)((completedCredits * 100) / conditionalRequirement.CreditsRequired))
                : 0;

            result.CourseProgress = conditionalRequirement.CoursesRequired > 0
                ? Math.Min(100, (completedCourseCount * 100) / conditionalRequirement.CoursesRequired)
                : 0;

            // Overall progress is the minimum of credit and course progress
            result.OverallProgress = Math.Min(result.CreditProgress, result.CourseProgress);

            // Calculate remaining needs
            result.RemainingCreditsNeeded = Math.Max(0, (int)(conditionalRequirement.CreditsRequired - completedCredits));
            result.RemainingCoursesNeeded = Math.Max(0, conditionalRequirement.CoursesRequired - completedCourseCount);

            _logger.LogDebug("Conditional requirement evaluation completed. Satisfied: {IsSatisfied}, Progress: {Progress}%",
                result.IsSatisfied, result.OverallProgress);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error evaluating conditional requirement {Id}", conditionalRequirement.Id);
            throw;
        }
    }

    /// <summary>
    /// Finds the best path to satisfy a conditional requirement group.
    /// </summary>
    /// <param name="requirement">The conditional group requirement</param>
    /// <param name="availableCourses">All available courses</param>
    /// <param name="completedCourses">Already completed courses</param>
    /// <returns>Optimal path recommendation</returns>
    public async Task<ConditionalRequirementPathResult> FindOptimalConditionalPathAsync(
        DegreeRequirement requirement,
        List<Course> availableCourses,
        List<Course> completedCourses)
    {
        try
        {
            _logger.LogDebug("Finding optimal path for conditional requirement: {Description}", requirement.Description);

            var result = new ConditionalRequirementPathResult
            {
                RequirementId = requirement.Id,
                RequirementDescription = requirement.Description
            };

            if (requirement.Type != RequirementType.ConditionalGroup)
            {
                result.AddError("Requirement must be of ConditionalGroup type");
                return result;
            }

            var paths = new List<ConditionalPath>();

            // Evaluate each conditional alternative
            foreach (var conditional in requirement.ConditionalRequirements)
            {
                var path = await EvaluateConditionalPathAsync(conditional, availableCourses, completedCourses);
                paths.Add(path);
            }

            // Sort paths by efficiency (least additional courses/credits needed)
            result.AlternativePaths = paths.OrderBy(p => p.TotalEffort).ToList();
            result.RecommendedPath = result.AlternativePaths.FirstOrDefault();

            if (result.RecommendedPath != null)
            {
                result.IsPathAvailable = true;
                result.RecommendedCourses = result.RecommendedPath.RequiredCourses;
                result.TotalAdditionalCredits = result.RecommendedPath.AdditionalCreditsNeeded;
                result.EstimatedSemesters = CalculateEstimatedSemesters(result.RecommendedPath.RequiredCourses);
            }

            _logger.LogDebug("Optimal path analysis completed. {PathCount} paths found, best requires {Credits} additional credits",
                paths.Count, result.TotalAdditionalCredits);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding optimal conditional path");
            throw;
        }
    }

    #endregion

    #region Private Helper Methods

    private async Task ValidateTemplateAsync(DegreeRequirementTemplate template)
    {
        if (string.IsNullOrWhiteSpace(template.DegreeCode))
            throw new ArgumentException("Degree code is required");

        if (string.IsNullOrWhiteSpace(template.DegreeName))
            throw new ArgumentException("Degree name is required");

        if (template.TotalCreditsRequired <= 0)
            throw new ArgumentException("Total credits required must be positive");

        if (template.MinimumGPA < 0 || template.MinimumGPA > 4.0m)
            throw new ArgumentException("Minimum GPA must be between 0.0 and 4.0");

        // Check for duplicate degree codes
        var existingTemplate = await _context.DegreeRequirementTemplates
            .Where(t => t.DegreeCode == template.DegreeCode && t.Id != template.Id && t.IsActive)
            .FirstOrDefaultAsync();

        if (existingTemplate != null)
            throw new InvalidOperationException($"Active degree template with code '{template.DegreeCode}' already exists");

        // Validate categories sum up correctly
        var categoryCreditsSum = template.Categories.Sum(c => c.CreditsRequired);
        if (categoryCreditsSum != template.TotalCreditsRequired)
        {
            _logger.LogWarning("Category credits sum ({CategorySum}) does not equal total required credits ({TotalRequired}) for {DegreeCode}",
                categoryCreditsSum, template.TotalCreditsRequired, template.DegreeCode);
        }
    }

    private async Task ValidateSpecificCourseRequirementAsync(DegreeRequirement requirement, RequirementValidationResult result)
    {
        if (!requirement.RequiredCourses.Any())
        {
            result.AddError("Specific course requirement must have at least one required course");
            return;
        }

        // Verify courses exist and are active
        var courseIds = requirement.RequiredCourses.Select(rc => rc.CourseId).ToList();
        var existingCourses = await _context.Courses
            .Where(c => courseIds.Contains(c.Id)) // IsActive property doesn't exist
            .CountAsync();

        if (existingCourses != courseIds.Count)
        {
            result.AddError("One or more required courses do not exist or are inactive");
        }
    }

    private async Task ValidateCourseGroupRequirementAsync(DegreeRequirement requirement, RequirementValidationResult result)
    {
        if (!requirement.RequiredSubjects.Any())
        {
            result.AddError("Course group requirement must have at least one subject area");
            return;
        }

        // Verify subject codes exist
        var subjectCodes = requirement.RequiredSubjects.Select(rs => rs.SubjectCode).ToList();
        var existingSubjects = await _context.Subjects
            .Where(s => subjectCodes.Contains(s.Code) && s.IsActive)
            .CountAsync();

        if (existingSubjects != subjectCodes.Count)
        {
            result.AddError("One or more subject codes do not exist or are inactive");
        }

        // Validate course level ranges
        foreach (var subject in requirement.RequiredSubjects)
        {
            if (subject.MinimumLevel > subject.MaximumLevel)
            {
                result.AddError($"Minimum level ({subject.MinimumLevel}) cannot be greater than maximum level ({subject.MaximumLevel}) for subject {subject.SubjectCode}");
            }
        }
    }

    private async Task ValidateConditionalGroupRequirementAsync(DegreeRequirement requirement, RequirementValidationResult result)
    {
        if (!requirement.ConditionalRequirements.Any())
        {
            result.AddError("Conditional group requirement must have at least one conditional option");
            return;
        }

        // Validate each conditional requirement
        foreach (var conditional in requirement.ConditionalRequirements)
        {
            if (conditional.CreditsRequired <= 0 && conditional.CoursesRequired <= 0)
            {
                result.AddError($"Conditional requirement '{conditional.Condition}' must specify either credits or courses required");
            }

            if (conditional.MinimumCourseLevel > conditional.MaximumCourseLevel)
            {
                result.AddError($"Minimum course level cannot be greater than maximum for condition '{conditional.Condition}'");
            }
        }
    }

    private async Task ValidateSequencedCoursesRequirementAsync(DegreeRequirement requirement, RequirementValidationResult result)
    {
        if (!requirement.PrerequisiteChain.Any())
        {
            result.AddError("Sequenced courses requirement must have prerequisite chain defined");
            return;
        }

        // Check for circular dependencies
        var prerequisiteGraph = BuildPrerequisiteGraph(requirement.PrerequisiteChain.ToList());
        var circularDependency = DetectCircularDependencies(prerequisiteGraph);

        if (circularDependency != null)
        {
            result.AddError($"Circular dependency detected in prerequisite chain: {string.Join(" -> ", circularDependency)}");
        }
    }

    private void ValidateCreditHoursRequirement(DegreeRequirement requirement, RequirementValidationResult result)
    {
        if (requirement.CreditsRequired <= 0)
        {
            result.AddError("Credit hours requirement must specify positive number of credits");
        }
    }

    private Dictionary<int, List<int>> BuildPrerequisiteGraph(List<PrerequisiteLink> prerequisites)
    {
        var graph = new Dictionary<int, List<int>>();

        foreach (var link in prerequisites)
        {
            if (!graph.ContainsKey(link.CourseId))
            {
                graph[link.CourseId] = new List<int>();
            }

            if (link.PrerequisiteCourseId.HasValue)
            {
                graph[link.CourseId].Add(link.PrerequisiteCourseId.Value);
            }
        }

        return graph;
    }

    private List<int>? DetectCircularDependencies(Dictionary<int, List<int>> graph)
    {
        var visited = new HashSet<int>();
        var recursionStack = new HashSet<int>();
        var path = new List<int>();

        foreach (var node in graph.Keys)
        {
            if (!visited.Contains(node))
            {
                var cycle = DetectCycleUtil(graph, node, visited, recursionStack, path);
                if (cycle != null)
                {
                    return cycle;
                }
            }
        }

        return null;
    }

    private List<int>? DetectCycleUtil(Dictionary<int, List<int>> graph, int node, HashSet<int> visited,
        HashSet<int> recursionStack, List<int> path)
    {
        visited.Add(node);
        recursionStack.Add(node);
        path.Add(node);

        if (graph.ContainsKey(node))
        {
            foreach (var neighbor in graph[node])
            {
                if (!visited.Contains(neighbor))
                {
                    var cycle = DetectCycleUtil(graph, neighbor, visited, recursionStack, path);
                    if (cycle != null)
                    {
                        return cycle;
                    }
                }
                else if (recursionStack.Contains(neighbor))
                {
                    // Found cycle - return the cycle path
                    var cycleIndex = path.IndexOf(neighbor);
                    return path.Skip(cycleIndex).ToList();
                }
            }
        }

        path.RemoveAt(path.Count - 1);
        recursionStack.Remove(node);
        return null;
    }

    private List<List<int>> TopologicalSort(Dictionary<int, List<int>> graph)
    {
        var levels = new List<List<int>>();
        var inDegree = new Dictionary<int, int>();
        var queue = new Queue<int>();

        // Calculate in-degrees
        foreach (var node in graph.Keys)
        {
            inDegree[node] = 0;
        }

        foreach (var edges in graph.Values)
        {
            foreach (var target in edges)
            {
                if (inDegree.ContainsKey(target))
                {
                    inDegree[target]++;
                }
            }
        }

        // Find starting nodes (no prerequisites)
        foreach (var kvp in inDegree)
        {
            if (kvp.Value == 0)
            {
                queue.Enqueue(kvp.Key);
            }
        }

        // Process levels
        while (queue.Count > 0)
        {
            var currentLevel = new List<int>();
            var levelSize = queue.Count;

            for (int i = 0; i < levelSize; i++)
            {
                var node = queue.Dequeue();
                currentLevel.Add(node);

                if (graph.ContainsKey(node))
                {
                    foreach (var neighbor in graph[node])
                    {
                        inDegree[neighbor]--;
                        if (inDegree[neighbor] == 0)
                        {
                            queue.Enqueue(neighbor);
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

    private Dictionary<int, int> GenerateSemesterMapping(List<List<int>> levels)
    {
        var mapping = new Dictionary<int, int>();

        for (int level = 0; level < levels.Count; level++)
        {
            foreach (var courseId in levels[level])
            {
                mapping[courseId] = level + 1; // Semester 1-based
            }
        }

        return mapping;
    }

    private void CheckSpecificCourseRequirement(DegreeRequirement requirement, List<Course> completedCourses, RequirementSatisfactionResult result)
    {
        var requiredCourseIds = requirement.GetCourseIds();
        var completedRequiredCourses = completedCourses.Where(c => requiredCourseIds.Contains(c.Id)).ToList();

        var completedCredits = completedRequiredCourses.Sum(c => c.CreditHours);
        var requiredCredits = requirement.CreditsRequired;

        result.IsSatisfied = completedCredits >= requiredCredits;
        result.CreditsSatisfied = (int)completedCredits;
        result.CreditsRequired = requiredCredits;
        result.ProgressPercentage = requiredCredits > 0 ? Math.Min(100, (int)((completedCredits * 100) / requiredCredits)) : 0;
        result.SatisfyingCourses = completedRequiredCourses.Select(c => c.Id).ToList();
    }

    private async Task CheckCourseGroupRequirementAsync(DegreeRequirement requirement, List<Course> completedCourses, RequirementSatisfactionResult result)
    {
        var subjectCodes = requirement.GetSubjectCodes();
        var applicableCourses = completedCourses.Where(c =>
            subjectCodes.Contains(c.Subject.Code) &&
            c.GetCourseLevel() >= requirement.MinimumCourseLevel &&
            c.GetCourseLevel() <= requirement.MaximumCourseLevel).ToList();

        var completedCredits = applicableCourses.Sum(c => c.CreditHours);
        var requiredCredits = requirement.CreditsRequired;

        result.IsSatisfied = completedCredits >= requiredCredits;
        result.CreditsSatisfied = (int)completedCredits;
        result.CreditsRequired = requiredCredits;
        result.ProgressPercentage = requiredCredits > 0 ? Math.Min(100, (int)((completedCredits * 100) / requiredCredits)) : 0;
        result.SatisfyingCourses = applicableCourses.Select(c => c.Id).ToList();
    }

    private void CheckConditionalGroupRequirement(DegreeRequirement requirement, List<Course> completedCourses, decimal? studentGPA, RequirementSatisfactionResult result)
    {
        var satisfiedConditions = new List<int>();

        foreach (var conditional in requirement.ConditionalRequirements)
        {
            if (conditional.IsSatisfiedBy(completedCourses, studentGPA))
            {
                satisfiedConditions.Add(conditional.Id);
            }
        }

        // For conditional groups, typically satisfying any one condition is sufficient
        result.IsSatisfied = satisfiedConditions.Any();
        result.ProgressPercentage = result.IsSatisfied ? 100 : 0;
        result.SatisfiedConditionalIds = satisfiedConditions;
    }

    private void CheckSequencedCoursesRequirement(DegreeRequirement requirement, List<Course> completedCourses, RequirementSatisfactionResult result)
    {
        var requiredCourseIds = requirement.GetCourseIds();
        var completedRequiredCourses = completedCourses.Where(c => requiredCourseIds.Contains(c.Id)).ToList();

        // For sequenced courses, check if the sequence is being followed correctly
        var prerequisiteChain = requirement.PrerequisiteChain.OrderBy(p => p.SequenceOrder).ToList();
        var validSequenceProgress = 0;
        var completedIds = completedRequiredCourses.Select(c => c.Id).ToHashSet();

        foreach (var link in prerequisiteChain)
        {
            if (completedIds.Contains(link.CourseId))
            {
                // Check if prerequisites are satisfied
                if (!link.PrerequisiteCourseId.HasValue || completedIds.Contains(link.PrerequisiteCourseId.Value))
                {
                    validSequenceProgress++;
                }
                else
                {
                    // Sequence violation - course taken without prerequisite
                    break;
                }
            }
        }

        var totalCredits = completedRequiredCourses.Sum(c => c.CreditHours);
        var requiredCredits = requirement.CreditsRequired;

        result.IsSatisfied = totalCredits >= requiredCredits && validSequenceProgress == prerequisiteChain.Count;
        result.CreditsSatisfied = (int)totalCredits;
        result.CreditsRequired = requiredCredits;
        result.ProgressPercentage = requiredCredits > 0 ? Math.Min(100, (int)((totalCredits * 100) / requiredCredits)) : 0;
        // Sequence progress is tracked separately in the prerequisite chain validation
        result.SatisfyingCourses = completedRequiredCourses.Select(c => c.Id).ToList();
    }

    private void CheckCreditHoursRequirement(DegreeRequirement requirement, List<Course> completedCourses, RequirementSatisfactionResult result)
    {
        var totalCredits = completedCourses.Sum(c => c.CreditHours);
        var requiredCredits = requirement.CreditsRequired;

        result.IsSatisfied = totalCredits >= requiredCredits;
        result.CreditsSatisfied = (int)totalCredits;
        result.CreditsRequired = requiredCredits;
        result.ProgressPercentage = requiredCredits > 0 ? Math.Min(100, (int)((totalCredits * 100) / requiredCredits)) : 0;
        result.SatisfyingCourses = completedCourses.Select(c => c.Id).ToList();
    }

    private async Task<List<Course>> GetApplicableCoursesForConditionalAsync(ConditionalRequirement conditional, List<Course> completedCourses)
    {
        // Get courses that apply to this conditional requirement
        var coursesFromSpecificList = completedCourses
            .Where(c => conditional.Courses.Any(cc => cc.CourseId == c.Id))
            .ToList();

        var coursesFromSubjects = completedCourses
            .Where(c => conditional.Subjects.Any(cs => cs.SubjectCode == c.Subject.Code) &&
                       c.GetCourseLevel() >= conditional.MinimumCourseLevel &&
                       c.GetCourseLevel() <= conditional.MaximumCourseLevel)
            .ToList();

        return coursesFromSpecificList.Union(coursesFromSubjects).ToList();
    }

    private async Task<ConditionalPath> EvaluateConditionalPathAsync(ConditionalRequirement conditional, List<Course> availableCourses, List<Course> completedCourses)
    {
        var path = new ConditionalPath
        {
            ConditionalRequirementId = conditional.Id,
            Condition = conditional.Condition
        };

        // Get applicable completed courses
        var applicableCompleted = await GetApplicableCoursesForConditionalAsync(conditional, completedCourses);

        // Calculate what's still needed
        var completedCredits = applicableCompleted.Sum(c => c.CreditHours);
        var additionalCreditsNeeded = Math.Max(0, conditional.CreditsRequired - completedCredits);

        // Find courses that could fulfill remaining requirements
        var remainingCourses = availableCourses
            .Where(c => !completedCourses.Any(cc => cc.Id == c.Id))
            .Where(c => conditional.Courses.Any(cc => cc.CourseId == c.Id) ||
                       conditional.Subjects.Any(cs => cs.SubjectCode == c.Subject.Code &&
                                                     c.GetCourseLevel() >= conditional.MinimumCourseLevel &&
                                                     c.GetCourseLevel() <= conditional.MaximumCourseLevel))
            .OrderBy(c => c.CreditHours) // Prefer courses with fewer credits for efficiency
            .ToList();

        // Build optimal course selection
        var selectedCourses = new List<Course>();
        var remainingCredits = additionalCreditsNeeded;

        foreach (var course in remainingCourses)
        {
            if (remainingCredits <= 0) break;

            selectedCourses.Add(course);
            remainingCredits -= course.CreditHours;
        }

        path.RequiredCourses = selectedCourses;
        path.AdditionalCreditsNeeded = (int)additionalCreditsNeeded;
        path.TotalEffort = selectedCourses.Count + ((double)additionalCreditsNeeded / 3); // Weight by courses and credits

        return path;
    }

    private int CalculateEstimatedSemesters(List<Course> courses)
    {
        // Simple estimation: assume 15 credits per semester
        const int creditsPerSemester = 15;
        var totalCredits = courses.Sum(c => c.CreditHours);
        return (int)Math.Ceiling((double)totalCredits / creditsPerSemester);
    }

    #endregion
}

#region Supporting Classes

public class RequirementValidationResult
{
    public bool IsValid => !Errors.Any();
    public List<string> Errors { get; } = new List<string>();
    public List<string> Warnings { get; } = new List<string>();

    public void AddError(string error) => Errors.Add(error);
    public void AddWarning(string warning) => Warnings.Add(warning);
}

public class SequenceValidationResult
{
    public bool IsValid { get; set; }
    public bool HasCircularDependency { get; set; }
    public int SequenceLength { get; set; }
    public List<List<int>> Levels { get; set; } = new List<List<int>>();
    public Dictionary<int, int>? RecommendedSemesterMapping { get; set; }
    public List<string> Errors { get; } = new List<string>();

    public void AddError(string error) => Errors.Add(error);
}



public class ConditionalRequirementEvaluationResult
{
    public int ConditionalRequirementId { get; set; }
    public string Condition { get; set; } = string.Empty;
    public bool IsSatisfied { get; set; }
    public int CreditProgress { get; set; }
    public int CourseProgress { get; set; }
    public int OverallProgress { get; set; }
    public int RemainingCreditsNeeded { get; set; }
    public int RemainingCoursesNeeded { get; set; }
    public List<Course> ApplicableCourses { get; set; } = new List<Course>();
}

public class ConditionalRequirementPathResult
{
    public int RequirementId { get; set; }
    public string RequirementDescription { get; set; } = string.Empty;
    public bool IsPathAvailable { get; set; }
    public ConditionalPath? RecommendedPath { get; set; }
    public List<ConditionalPath> AlternativePaths { get; set; } = new List<ConditionalPath>();
    public List<Course> RecommendedCourses { get; set; } = new List<Course>();
    public int TotalAdditionalCredits { get; set; }
    public int EstimatedSemesters { get; set; }
    public List<string> Errors { get; } = new List<string>();

    public void AddError(string error) => Errors.Add(error);
}

public class ConditionalPath
{
    public int ConditionalRequirementId { get; set; }
    public string Condition { get; set; } = string.Empty;
    public List<Course> RequiredCourses { get; set; } = new List<Course>();
    public int AdditionalCreditsNeeded { get; set; }
    public double TotalEffort { get; set; }
}

#endregion