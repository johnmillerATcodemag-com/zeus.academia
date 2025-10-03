using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Enums;
using Zeus.Academia.Infrastructure.Extensions;

namespace Zeus.Academia.Infrastructure.Services;

/// <summary>
/// Service for performing degree audits, tracking progress toward requirements,
/// completion calculations, and handling course substitutions.
/// </summary>
public class DegreeAuditService
{
    private readonly AcademiaDbContext _context;
    private readonly ILogger<DegreeAuditService> _logger;

    public DegreeAuditService(AcademiaDbContext context, ILogger<DegreeAuditService> logger)
    {
        _context = context;
        _logger = logger;
    }

    #region Degree Audit Operations

    /// <summary>
    /// Performs a comprehensive degree audit for a student against their degree requirements.
    /// </summary>
    /// <param name="studentRecord">Student's academic record</param>
    /// <param name="degreeTemplate">Degree requirements template</param>
    /// <returns>Complete audit result with progress and recommendations</returns>
    public async Task<DegreeAuditResult> PerformDegreeAuditAsync(StudentAcademicRecord studentRecord, DegreeRequirementTemplate degreeTemplate)
    {
        try
        {
            _logger.LogInformation("Performing degree audit for student {StudentId} in degree {DegreeCode}",
                studentRecord.StudentId, degreeTemplate.DegreeCode);

            var auditResult = new DegreeAuditResult
            {
                StudentId = studentRecord.StudentId,
                DegreeCode = degreeTemplate.DegreeCode,
                AuditDate = DateTime.UtcNow,
                TotalCreditsRequired = degreeTemplate.TotalCreditsRequired
            };

            // Calculate total credits completed (including transfer credits)
            var completedCredits = studentRecord.CompletedCourses.Sum(c => c.CreditHours);
            var transferCredits = studentRecord.TransferCredits?.Sum(t => t.CreditHours) ?? 0;
            auditResult.TotalCreditsCompleted = completedCredits + transferCredits;
            auditResult.RemainingCreditsNeeded = Math.Max(0, auditResult.TotalCreditsRequired - auditResult.TotalCreditsCompleted);

            // Calculate completion percentage
            auditResult.UpdateCompletionPercentage();

            // Get all completed courses including substitutions
            var allCompletedCourses = await GetAllCompletedCoursesAsync(studentRecord);

            // Audit each category
            auditResult.CategoryProgress = new List<CategoryProgressResult>();
            auditResult.SatisfiedRequirements = new List<SatisfiedRequirement>();
            auditResult.OutstandingRequirements = new List<OutstandingRequirement>();

            foreach (var category in degreeTemplate.Categories)
            {
                var categoryProgress = await AuditCategoryAsync(category, allCompletedCourses, studentRecord);
                auditResult.CategoryProgress.Add(categoryProgress);

                // Add satisfied and outstanding requirements
                auditResult.SatisfiedRequirements.AddRange(categoryProgress.SatisfiedRequirements);
                auditResult.OutstandingRequirements.AddRange(categoryProgress.OutstandingRequirements);
            }

            // Process approved substitutions
            auditResult.ApprovedSubstitutions = await ProcessCourseSubstitutionsAsync(studentRecord);

            // Calculate GPA requirements
            await CalculateGPARequirementsAsync(auditResult, studentRecord, degreeTemplate);

            // Generate recommendations
            auditResult.Recommendations = await GenerateAuditRecommendationsAsync(auditResult, degreeTemplate);

            // Check graduation eligibility
            auditResult.IsEligibleForGraduation = await CheckGraduationEligibilityAsync(auditResult, degreeTemplate, studentRecord);

            _logger.LogInformation("Degree audit completed: {CompletionPercentage}% complete, {RemainingCredits} credits remaining",
                auditResult.CompletionPercentage, auditResult.RemainingCreditsNeeded);

            return auditResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing degree audit for student {StudentId}", studentRecord.StudentId);
            throw;
        }
    }

    /// <summary>
    /// Creates or updates a student's degree audit record in the database.
    /// </summary>
    /// <param name="auditResult">Audit result to persist</param>
    /// <returns>Persisted audit record</returns>
    public async Task<StudentDegreeAudit> SaveDegreeAuditAsync(DegreeAuditResult auditResult)
    {
        try
        {
            _logger.LogInformation("Saving degree audit for student {StudentId}", auditResult.StudentId);

            // Get degree template
            var degreeTemplate = await _context.DegreeRequirementTemplates
                .FirstOrDefaultAsync(t => t.DegreeCode == auditResult.DegreeCode && t.IsActive);

            if (degreeTemplate == null)
            {
                throw new InvalidOperationException($"Active degree template not found for {auditResult.DegreeCode}");
            }

            // Check for existing audit
            var existingAudit = await _context.StudentDegreeAudits
                .Include(a => a.CategoryProgress)
                .Include(a => a.RequirementFulfillments)
                .FirstOrDefaultAsync(a => a.StudentId == auditResult.StudentId && a.DegreeTemplateId == degreeTemplate.Id);

            StudentDegreeAudit auditRecord;

            if (existingAudit != null)
            {
                // Update existing audit
                auditRecord = existingAudit;
                UpdateAuditRecord(auditRecord, auditResult);
            }
            else
            {
                // Create new audit
                auditRecord = CreateAuditRecord(auditResult, degreeTemplate.Id);
                _context.StudentDegreeAudits.Add(auditRecord);
            }

            // Update category progress
            await UpdateCategoryProgressAsync(auditRecord, auditResult);

            // Update requirement fulfillments
            await UpdateRequirementFulfillmentsAsync(auditRecord, auditResult);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Degree audit saved with ID {AuditId}", auditRecord.Id);

            return auditRecord;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving degree audit for student {StudentId}", auditResult.StudentId);
            throw;
        }
    }

    /// <summary>
    /// Retrieves the most recent degree audit for a student.
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <param name="degreeCode">Degree code (optional)</param>
    /// <returns>Most recent audit result</returns>
    public async Task<DegreeAuditResult?> GetMostRecentAuditAsync(int studentId, string? degreeCode = null)
    {
        try
        {
            var query = _context.StudentDegreeAudits
                .Include(a => a.DegreeTemplate)
                .Include(a => a.CategoryProgress)
                    .ThenInclude(cp => cp.Category)
                .Include(a => a.RequirementFulfillments)
                    .ThenInclude(rf => rf.Requirement)
                .Where(a => a.StudentId == studentId);

            if (!string.IsNullOrEmpty(degreeCode))
            {
                query = query.Where(a => a.DegreeTemplate.DegreeCode == degreeCode);
            }

            var auditRecord = await query
                .OrderByDescending(a => a.AuditDate)
                .FirstOrDefaultAsync();

            if (auditRecord == null)
            {
                return null;
            }

            return await ConvertAuditRecordToResultAsync(auditRecord);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving most recent audit for student {StudentId}", studentId);
            throw;
        }
    }

    #endregion

    #region Progress Tracking

    /// <summary>
    /// Tracks progress for a specific requirement category.
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <param name="categoryId">Category ID</param>
    /// <returns>Detailed category progress</returns>
    public async Task<CategoryProgressResult> TrackCategoryProgressAsync(int studentId, int categoryId)
    {
        try
        {
            _logger.LogDebug("Tracking category progress for student {StudentId}, category {CategoryId}", studentId, categoryId);

            var category = await _context.RequirementCategories
                .Include(c => c.Requirements)
                    .ThenInclude(r => r.RequiredCourses)
                        .ThenInclude(rc => rc.Course)
                .Include(c => c.Requirements)
                    .ThenInclude(r => r.RequiredSubjects)
                .Include(c => c.Requirements)
                    .ThenInclude(r => r.ConditionalRequirements)
                .FirstOrDefaultAsync(c => c.Id == categoryId);

            if (category == null)
            {
                throw new ArgumentException($"Category not found: {categoryId}");
            }

            // Get student's completed courses
            var studentRecord = await GetStudentAcademicRecordAsync(studentId);
            var completedCourses = await GetAllCompletedCoursesAsync(studentRecord);

            var categoryProgress = await AuditCategoryAsync(category, completedCourses, studentRecord);

            _logger.LogDebug("Category progress calculated: {CompletionPercentage}% complete", categoryProgress.CompletionPercentage);

            return categoryProgress;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error tracking category progress for student {StudentId}, category {CategoryId}", studentId, categoryId);
            throw;
        }
    }

    /// <summary>
    /// Calculates what-if scenarios for course selection.
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <param name="prospectiveCourses">Courses being considered</param>
    /// <returns>Impact analysis of taking these courses</returns>
    public async Task<WhatIfAnalysisResult> PerformWhatIfAnalysisAsync(int studentId, List<int> prospectiveCourses)
    {
        try
        {
            _logger.LogInformation("Performing what-if analysis for student {StudentId} with {CourseCount} prospective courses",
                studentId, prospectiveCourses.Count);

            var result = new WhatIfAnalysisResult
            {
                StudentId = studentId,
                ProspectiveCourses = prospectiveCourses,
                AnalysisDate = DateTime.UtcNow
            };

            // Get current student record
            var studentRecord = await GetStudentAcademicRecordAsync(studentId);

            // Get degree template
            var degreeTemplate = await _context.DegreeRequirementTemplates
                .Include(t => t.Categories)
                    .ThenInclude(c => c.Requirements)
                .FirstOrDefaultAsync(t => t.DegreeCode == studentRecord.DegreeCode && t.IsActive);

            if (degreeTemplate == null)
            {
                throw new InvalidOperationException($"Degree template not found for {studentRecord.DegreeCode}");
            }

            // Perform current audit
            var currentAudit = await PerformDegreeAuditAsync(studentRecord, degreeTemplate);
            result.CurrentProgress = currentAudit;

            // Create hypothetical record with prospective courses
            var hypotheticalRecord = CreateHypotheticalRecord(studentRecord, prospectiveCourses);
            var hypotheticalAudit = await PerformDegreeAuditAsync(hypotheticalRecord, degreeTemplate);
            result.ProjectedProgress = hypotheticalAudit;

            // Calculate impact
            result.CreditImpact = hypotheticalAudit.TotalCreditsCompleted - currentAudit.TotalCreditsCompleted;
            result.ProgressImpact = hypotheticalAudit.CompletionPercentage - currentAudit.CompletionPercentage;
            result.RequirementsSatisfied = hypotheticalAudit.SatisfiedRequirements.Count - currentAudit.SatisfiedRequirements.Count;

            // Analyze category impacts
            result.CategoryImpacts = new List<CategoryImpact>();
            for (int i = 0; i < currentAudit.CategoryProgress.Count; i++)
            {
                var currentCategory = currentAudit.CategoryProgress[i];
                var hypotheticalCategory = hypotheticalAudit.CategoryProgress[i];

                result.CategoryImpacts.Add(new CategoryImpact
                {
                    CategoryName = currentCategory.CategoryName,
                    CurrentProgress = currentCategory.CompletionPercentage,
                    ProjectedProgress = hypotheticalCategory.CompletionPercentage,
                    ProgressGain = hypotheticalCategory.CompletionPercentage - currentCategory.CompletionPercentage,
                    AdditionalRequirementsSatisfied = hypotheticalCategory.SatisfiedRequirements.Count - currentCategory.SatisfiedRequirements.Count
                });
            }

            // Generate recommendations based on analysis
            result.Recommendations = GenerateWhatIfRecommendations(result);

            _logger.LogInformation("What-if analysis completed: {CreditImpact} credit impact, {ProgressImpact}% progress gain",
                result.CreditImpact, result.ProgressImpact);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing what-if analysis for student {StudentId}", studentId);
            throw;
        }
    }

    #endregion

    #region Course Substitutions

    /// <summary>
    /// Processes and validates course substitutions for degree requirements.
    /// </summary>
    /// <param name="studentRecord">Student academic record</param>
    /// <returns>List of processed substitutions</returns>
    public async Task<List<ProcessedSubstitution>> ProcessCourseSubstitutionsAsync(StudentAcademicRecord studentRecord)
    {
        try
        {
            _logger.LogDebug("Processing course substitutions for student {StudentId}", studentRecord.StudentId);

            var processedSubstitutions = new List<ProcessedSubstitution>();

            if (studentRecord.ApprovedSubstitutions?.Any() != true)
            {
                return processedSubstitutions;
            }

            foreach (var substitution in studentRecord.ApprovedSubstitutions)
            {
                if (!substitution.IsCurrentlyValid())
                {
                    continue;
                }

                var originalCourse = await _context.Courses
                    .Include(c => c.Subject)
                    .FirstOrDefaultAsync(c => c.Id == substitution.OriginalCourseId);

                var substituteCourse = await _context.Courses
                    .Include(c => c.Subject)
                    .FirstOrDefaultAsync(c => c.Id == substitution.SubstituteCourseId);

                if (originalCourse != null && substituteCourse != null)
                {
                    var processed = new ProcessedSubstitution
                    {
                        SubstitutionId = substitution.Id,
                        OriginalCourseId = substitution.OriginalCourseId,
                        OriginalCourseName = $"{originalCourse.Subject.Code} {originalCourse.CourseNumber}",
                        OriginalCourseTitle = originalCourse.Title,
                        SubstituteCourseId = substitution.SubstituteCourseId,
                        SubstituteCourseName = $"{substituteCourse.Subject.Code} {substituteCourse.CourseNumber}",
                        SubstituteCourseTitle = substituteCourse.Title,
                        Reason = substitution.Reason,
                        ApprovedBy = substitution.ApprovedBy,
                        ApprovalDate = substitution.ApprovalDate,
                        CreditHoursDifference = (int)(substituteCourse.CreditHours - originalCourse.CreditHours)
                    };

                    // Validate substitution effectiveness
                    processed.IsEffective = await ValidateSubstitutionEffectivenessAsync(substitution, studentRecord);

                    processedSubstitutions.Add(processed);
                }
            }

            _logger.LogDebug("Processed {Count} course substitutions", processedSubstitutions.Count);

            return processedSubstitutions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing course substitutions for student {StudentId}", studentRecord.StudentId);
            throw;
        }
    }

    /// <summary>
    /// Requests a new course substitution for approval.
    /// </summary>
    /// <param name="substitutionRequest">Substitution request details</param>
    /// <returns>Created substitution request</returns>
    public async Task<CourseSubstitution> RequestCourseSubstitutionAsync(SubstitutionRequest substitutionRequest)
    {
        try
        {
            _logger.LogInformation("Requesting course substitution for student {StudentId}: {OriginalCourse} -> {SubstituteCourse}",
                substitutionRequest.StudentId, substitutionRequest.OriginalCourseId, substitutionRequest.SubstituteCourseId);

            // Validate the request
            await ValidateSubstitutionRequestAsync(substitutionRequest);

            var substitution = new CourseSubstitution
            {
                StudentId = substitutionRequest.StudentId,
                OriginalCourseId = substitutionRequest.OriginalCourseId,
                SubstituteCourseId = substitutionRequest.SubstituteCourseId,
                Reason = substitutionRequest.Reason,
                ApprovedBy = substitutionRequest.RequestedBy,
                ApprovalDate = DateTime.UtcNow,
                IsActive = false, // Requires approval
                AdditionalConditions = substitutionRequest.AdditionalConditions,
                Notes = substitutionRequest.Notes
            };

            _context.CourseSubstitutions.Add(substitution);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Course substitution request created with ID {SubstitutionId}", substitution.Id);

            return substitution;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error requesting course substitution for student {StudentId}", substitutionRequest.StudentId);
            throw;
        }
    }

    #endregion

    #region Private Helper Methods

    private async Task<List<Course>> GetAllCompletedCoursesAsync(StudentAcademicRecord studentRecord)
    {
        var completedCourseIds = studentRecord.CompletedCourses.Select(c => c.CourseId).ToList();

        var courses = await _context.Courses
            .Include(c => c.Subject)
            .Where(c => completedCourseIds.Contains(c.Id))
            .ToListAsync();

        // Apply course substitutions
        if (studentRecord.ApprovedSubstitutions?.Any() == true)
        {
            foreach (var substitution in studentRecord.ApprovedSubstitutions.Where(s => s.IsCurrentlyValid()))
            {
                // Remove original course and add substitute course if not already present
                courses = courses.Where(c => c.Id != substitution.OriginalCourseId).ToList();

                var substituteCourse = await _context.Courses
                    .Include(c => c.Subject)
                    .FirstOrDefaultAsync(c => c.Id == substitution.SubstituteCourseId);

                if (substituteCourse != null && !courses.Any(c => c.Id == substituteCourse.Id))
                {
                    courses.Add(substituteCourse);
                }
            }
        }

        return courses;
    }

    private async Task<CategoryProgressResult> AuditCategoryAsync(RequirementCategory category, List<Course> completedCourses, StudentAcademicRecord studentRecord)
    {
        var result = new CategoryProgressResult
        {
            CategoryId = category.Id,
            CategoryName = category.Name,
            CreditsRequired = category.CreditsRequired,
            SatisfiedRequirements = new List<SatisfiedRequirement>(),
            OutstandingRequirements = new List<OutstandingRequirement>()
        };

        var totalCreditsSatisfied = 0;

        foreach (var requirement in category.Requirements)
        {
            var satisfactionResult = await CheckRequirementSatisfactionAsync(requirement, completedCourses, studentRecord.CumulativeGPA);

            if (satisfactionResult.IsSatisfied)
            {
                result.SatisfiedRequirements.Add(new SatisfiedRequirement
                {
                    RequirementId = requirement.Id,
                    RequirementDescription = requirement.Description,
                    SatisfiedBy = string.Join(", ", satisfactionResult.SatisfyingCourses.Select(id =>
                        completedCourses.FirstOrDefault(c => c.Id == id)?.Title ?? $"Course {id}")),
                    CreditsSatisfied = satisfactionResult.CreditsSatisfied
                });

                totalCreditsSatisfied += satisfactionResult.CreditsSatisfied;
            }
            else
            {
                var outstandingRequirement = new OutstandingRequirement
                {
                    RequirementId = requirement.Id,
                    RequirementDescription = requirement.Description,
                    CreditsNeeded = requirement.CreditsRequired - satisfactionResult.CreditsSatisfied,
                    SuggestedCourses = await GetSuggestedCoursesAsync(requirement)
                };

                result.OutstandingRequirements.Add(outstandingRequirement);
            }
        }

        result.CreditsCompleted = Math.Min(totalCreditsSatisfied, category.CreditsRequired);
        result.CreditsRemaining = Math.Max(0, category.CreditsRequired - result.CreditsCompleted);
        result.CompletionPercentage = category.CreditsRequired > 0
            ? Math.Round((decimal)result.CreditsCompleted / category.CreditsRequired * 100, 2)
            : 0;
        result.IsComplete = result.CreditsRemaining == 0;

        return result;
    }

    private async Task<RequirementSatisfactionResult> CheckRequirementSatisfactionAsync(DegreeRequirement requirement, List<Course> completedCourses, decimal? studentGPA)
    {
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

        return result;
    }

    private void CheckSpecificCourseRequirement(DegreeRequirement requirement, List<Course> completedCourses, RequirementSatisfactionResult result)
    {
        var requiredCourseIds = requirement.GetCourseIds();
        var completedRequiredCourses = completedCourses.Where(c => requiredCourseIds.Contains(c.Id)).ToList();

        var completedCredits = completedRequiredCourses.Sum(c => c.CreditHours);
        var requiredCredits = requirement.CreditsRequired;

        result.IsSatisfied = completedCredits >= requiredCredits;
        result.CreditsSatisfied = (int)completedCredits;
        result.CreditsRequired = (int)requiredCredits;
        result.ProgressPercentage = requiredCredits > 0 ? (int)Math.Min(100, (completedCredits * 100) / requiredCredits) : 0;
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
        result.CreditsRequired = (int)requiredCredits;
        result.ProgressPercentage = requiredCredits > 0 ? (int)Math.Min(100, (completedCredits * 100) / requiredCredits) : 0;
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

        result.IsSatisfied = satisfiedConditions.Any();
        result.ProgressPercentage = result.IsSatisfied ? 100 : 0;
        result.SatisfiedConditionalIds = satisfiedConditions;
    }

    private void CheckSequencedCoursesRequirement(DegreeRequirement requirement, List<Course> completedCourses, RequirementSatisfactionResult result)
    {
        var requiredCourseIds = requirement.GetCourseIds();
        var completedRequiredCourses = completedCourses.Where(c => requiredCourseIds.Contains(c.Id)).ToList();

        var totalCredits = completedRequiredCourses.Sum(c => c.CreditHours);
        var requiredCredits = requirement.CreditsRequired;

        result.IsSatisfied = totalCredits >= requiredCredits;
        result.CreditsSatisfied = (int)totalCredits;
        result.CreditsRequired = requiredCredits;
        result.ProgressPercentage = requiredCredits > 0 ? Math.Min(100, (int)((totalCredits * 100) / requiredCredits)) : 0;
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

    private async Task<List<string>> GetSuggestedCoursesAsync(DegreeRequirement requirement)
    {
        var suggestions = new List<string>();

        if (requirement.Type == RequirementType.SpecificCourse)
        {
            var courseIds = requirement.GetCourseIds();
            var courses = await _context.Courses
                .Include(c => c.Subject)
                .Where(c => courseIds.Contains(c.Id))
                .ToListAsync();

            suggestions.AddRange(courses.Select(c => $"{c.Subject.Code} {c.CourseNumber} - {c.Title}"));
        }
        else if (requirement.Type == RequirementType.CourseGroup)
        {
            var subjectCodes = requirement.GetSubjectCodes();
            var courses = await _context.Courses
                .Include(c => c.Subject)
                .Where(c => subjectCodes.Contains(c.Subject.Code) &&
                           c.GetCourseLevel() >= requirement.MinimumCourseLevel &&
                           c.GetCourseLevel() <= requirement.MaximumCourseLevel &&
                           c.IsActive())
                .Take(5) // Limit suggestions
                .ToListAsync();

            suggestions.AddRange(courses.Select(c => $"{c.Subject.Code} {c.CourseNumber} - {c.Title}"));
        }

        return suggestions;
    }

    private async Task<StudentAcademicRecord> GetStudentAcademicRecordAsync(int studentId)
    {
        // This would typically retrieve from database, but for now create from enrollments
        var enrollments = await _context.CourseEnrollments
            .Include(e => e.Grades)
            .Include(e => e.Subject)
            .ThenInclude(s => s.Courses)
            .Where(e => e.StudentEmpNr == studentId && e.GetEffectiveGrade() != null)
            .ToListAsync();

        var student = await _context.Students.FindAsync(studentId);

        return new StudentAcademicRecord
        {
            StudentId = studentId,
            DegreeCode = student?.GetMajorCode() ?? "UNKNOWN",
            CompletedCourses = enrollments.Select(e => new CompletedCourse
            {
                CourseId = e.GetCourseId() ?? 0,
                Grade = e.GetFinalGrade() ?? "P",
                CreditHours = 3, // Default credit hours since Course navigation not available
                Semester = e.GetTerm()
            }).ToList(),
            CumulativeGPA = CalculateGPA(enrollments),
            TotalCredits = enrollments.Count * 3 // Estimated total credits
        };
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
            if (!string.IsNullOrEmpty(enrollment.GetEffectiveGrade()) && gradePoints.ContainsKey(enrollment.GetEffectiveGrade()))
            {
                totalPoints += gradePoints[enrollment.GetEffectiveGrade()!] * enrollment.GetCourse()!.CreditHours;
                totalCredits += (int)enrollment.GetCourse()!.CreditHours;
            }
        }

        return totalCredits > 0 ? Math.Round(totalPoints / totalCredits, 2) : 0;
    }

    private async Task CalculateGPARequirementsAsync(DegreeAuditResult auditResult, StudentAcademicRecord studentRecord, DegreeRequirementTemplate degreeTemplate)
    {
        auditResult.CurrentGPA = studentRecord.CumulativeGPA;
        auditResult.RequiredGPA = degreeTemplate.MinimumGPA;
        auditResult.GPADeficiency = Math.Max(0, degreeTemplate.MinimumGPA - studentRecord.CumulativeGPA);

        // Calculate major GPA if applicable
        var majorCourses = await _context.Courses
            .Where(c => studentRecord.CompletedCourses.Select(cc => cc.CourseId).Contains(c.Id))
            .Where(c => c.Subject.Code == studentRecord.DegreeCode.Substring(0, Math.Min(4, studentRecord.DegreeCode.Length)))
            .ToListAsync();

        if (majorCourses.Any())
        {
            // This would need more sophisticated major GPA calculation
            auditResult.MajorGPA = studentRecord.CumulativeGPA; // Simplified
        }
    }

    private async Task<List<string>> GenerateAuditRecommendationsAsync(DegreeAuditResult auditResult, DegreeRequirementTemplate degreeTemplate)
    {
        var recommendations = new List<string>();

        // Progress-based recommendations
        if (auditResult.CompletionPercentage < 25)
        {
            recommendations.Add("Focus on completing general education requirements early in your academic career.");
        }
        else if (auditResult.CompletionPercentage < 50)
        {
            recommendations.Add("Begin taking major-specific courses while completing remaining general education.");
        }
        else if (auditResult.CompletionPercentage < 75)
        {
            recommendations.Add("Focus on upper-division major requirements and electives.");
        }
        else
        {
            recommendations.Add("Complete remaining requirements and consider applying for graduation.");
        }

        // GPA-based recommendations
        if (auditResult.GPADeficiency > 0)
        {
            recommendations.Add($"Current GPA ({auditResult.CurrentGPA:F2}) is below the minimum required ({auditResult.RequiredGPA:F2}). Consider academic support resources.");
        }

        // Category-specific recommendations
        foreach (var category in auditResult.CategoryProgress)
        {
            if (category.CompletionPercentage < 50 && category.IsComplete == false)
            {
                recommendations.Add($"Prioritize completing {category.CategoryName} requirements ({category.CreditsRemaining} credits remaining).");
            }
        }

        return recommendations;
    }

    private async Task<bool> CheckGraduationEligibilityAsync(DegreeAuditResult auditResult, DegreeRequirementTemplate degreeTemplate, StudentAcademicRecord studentRecord)
    {
        // Basic eligibility checks
        var hasRequiredCredits = auditResult.TotalCreditsCompleted >= degreeTemplate.TotalCreditsRequired;
        var hasRequiredGPA = auditResult.CurrentGPA >= degreeTemplate.MinimumGPA;
        var hasCompletedAllRequirements = auditResult.OutstandingRequirements.Count == 0;

        return hasRequiredCredits && hasRequiredGPA && hasCompletedAllRequirements;
    }

    private StudentDegreeAudit CreateAuditRecord(DegreeAuditResult auditResult, int degreeTemplateId)
    {
        return new StudentDegreeAudit
        {
            StudentId = auditResult.StudentId,
            DegreeTemplateId = degreeTemplateId,
            AuditDate = auditResult.AuditDate,
            TotalCreditsCompleted = auditResult.TotalCreditsCompleted,
            TotalCreditsRequired = auditResult.TotalCreditsRequired,
            RemainingCreditsNeeded = auditResult.RemainingCreditsNeeded,
            CompletionPercentage = auditResult.CompletionPercentage,
            CurrentGPA = auditResult.CurrentGPA,
            MajorGPA = auditResult.MajorGPA,
            IsEligibleForGraduation = auditResult.IsEligibleForGraduation,
            AuditedBy = "System"
        };
    }

    private void UpdateAuditRecord(StudentDegreeAudit auditRecord, DegreeAuditResult auditResult)
    {
        auditRecord.AuditDate = auditResult.AuditDate;
        auditRecord.TotalCreditsCompleted = auditResult.TotalCreditsCompleted;
        auditRecord.TotalCreditsRequired = auditResult.TotalCreditsRequired;
        auditRecord.RemainingCreditsNeeded = auditResult.RemainingCreditsNeeded;
        auditRecord.CompletionPercentage = auditResult.CompletionPercentage;
        auditRecord.CurrentGPA = auditResult.CurrentGPA;
        auditRecord.MajorGPA = auditResult.MajorGPA;
        auditRecord.IsEligibleForGraduation = auditResult.IsEligibleForGraduation;
        auditRecord.AuditedBy = "System";
    }

    private async Task UpdateCategoryProgressAsync(StudentDegreeAudit auditRecord, DegreeAuditResult auditResult)
    {
        // Clear existing category progress
        var existingProgress = auditRecord.CategoryProgress.ToList();
        foreach (var progress in existingProgress)
        {
            _context.CategoryProgress.Remove(progress);
        }

        // Add new category progress
        foreach (var categoryResult in auditResult.CategoryProgress)
        {
            var categoryProgress = new CategoryProgress
            {
                AuditId = auditRecord.Id,
                CategoryId = categoryResult.CategoryId,
                CreditsCompleted = categoryResult.CreditsCompleted,
                CreditsRequired = categoryResult.CreditsRequired,
                CreditsRemaining = categoryResult.CreditsRemaining,
                CompletionPercentage = categoryResult.CompletionPercentage,
                IsComplete = categoryResult.IsComplete
            };

            auditRecord.CategoryProgress.Add(categoryProgress);
        }
    }

    private async Task UpdateRequirementFulfillmentsAsync(StudentDegreeAudit auditRecord, DegreeAuditResult auditResult)
    {
        // Clear existing fulfillments
        var existingFulfillments = auditRecord.RequirementFulfillments.ToList();
        foreach (var fulfillment in existingFulfillments)
        {
            _context.RequirementFulfillments.Remove(fulfillment);
        }

        // Add satisfied requirements
        foreach (var satisfied in auditResult.SatisfiedRequirements)
        {
            var fulfillment = new RequirementFulfillment
            {
                AuditId = auditRecord.Id,
                RequirementId = satisfied.RequirementId,
                IsFulfilled = true,
                CreditsFulfilled = satisfied.CreditsSatisfied,
                FulfillmentMethod = satisfied.SatisfiedBy
            };

            auditRecord.RequirementFulfillments.Add(fulfillment);
        }
    }

    private async Task<DegreeAuditResult> ConvertAuditRecordToResultAsync(StudentDegreeAudit auditRecord)
    {
        var result = new DegreeAuditResult
        {
            StudentId = auditRecord.StudentId,
            DegreeCode = auditRecord.DegreeTemplate.DegreeCode,
            AuditDate = auditRecord.AuditDate,
            TotalCreditsCompleted = auditRecord.TotalCreditsCompleted,
            TotalCreditsRequired = auditRecord.TotalCreditsRequired,
            RemainingCreditsNeeded = auditRecord.RemainingCreditsNeeded,
            CompletionPercentage = auditRecord.CompletionPercentage,
            CurrentGPA = auditRecord.CurrentGPA ?? 0.0m,
            RequiredGPA = auditRecord.DegreeTemplate.MinimumGPA,
            MajorGPA = auditRecord.MajorGPA,
            IsEligibleForGraduation = auditRecord.IsEligibleForGraduation
        };

        // Convert category progress
        result.CategoryProgress = auditRecord.CategoryProgress.Select(cp => new CategoryProgressResult
        {
            CategoryId = cp.CategoryId,
            CategoryName = cp.Category.Name,
            CreditsCompleted = cp.CreditsCompleted,
            CreditsRequired = cp.CreditsRequired,
            CreditsRemaining = cp.CreditsRemaining,
            CompletionPercentage = cp.CompletionPercentage,
            IsComplete = cp.IsComplete
        }).ToList();

        // Convert satisfied requirements
        result.SatisfiedRequirements = auditRecord.RequirementFulfillments
            .Where(rf => rf.IsFulfilled)
            .Select(rf => new SatisfiedRequirement
            {
                RequirementId = rf.RequirementId,
                RequirementDescription = rf.Requirement.Description,
                SatisfiedBy = rf.FulfillmentMethod ?? "Unknown",
                CreditsSatisfied = rf.CreditsFulfilled
            }).ToList();

        return result;
    }

    private StudentAcademicRecord CreateHypotheticalRecord(StudentAcademicRecord baseRecord, List<int> prospectiveCourses)
    {
        var hypotheticalRecord = new StudentAcademicRecord
        {
            StudentId = baseRecord.StudentId,
            DegreeCode = baseRecord.DegreeCode,
            CompletedCourses = new List<CompletedCourse>(baseRecord.CompletedCourses),
            TransferCredits = baseRecord.TransferCredits,
            ApprovedSubstitutions = baseRecord.ApprovedSubstitutions,
            CumulativeGPA = baseRecord.CumulativeGPA
        };

        // Add prospective courses as completed (for analysis purposes)
        foreach (var courseId in prospectiveCourses)
        {
            hypotheticalRecord.CompletedCourses.Add(new CompletedCourse
            {
                CourseId = courseId,
                Grade = "B", // Assume average grade for projection
                CreditHours = 3, // Would need to look up actual credit hours
                Semester = "Hypothetical"
            });
        }

        return hypotheticalRecord;
    }

    private List<string> GenerateWhatIfRecommendations(WhatIfAnalysisResult result)
    {
        var recommendations = new List<string>();

        if (result.ProgressImpact > 10)
        {
            recommendations.Add("These courses would significantly advance your degree progress.");
        }

        if (result.RequirementsSatisfied > 2)
        {
            recommendations.Add("Taking these courses would satisfy multiple degree requirements efficiently.");
        }

        var highImpactCategories = result.CategoryImpacts.Where(ci => ci.ProgressGain > 20).ToList();
        if (highImpactCategories.Any())
        {
            recommendations.Add($"These courses would significantly impact: {string.Join(", ", highImpactCategories.Select(c => c.CategoryName))}");
        }

        return recommendations;
    }

    private async Task ValidateSubstitutionRequestAsync(SubstitutionRequest request)
    {
        if (request.StudentId <= 0)
            throw new ArgumentException("Valid student ID is required");

        if (request.OriginalCourseId <= 0)
            throw new ArgumentException("Valid original course ID is required");

        if (request.SubstituteCourseId <= 0)
            throw new ArgumentException("Valid substitute course ID is required");

        if (string.IsNullOrWhiteSpace(request.Reason))
            throw new ArgumentException("Substitution reason is required");

        // Verify courses exist
        var originalExists = await _context.Courses.AnyAsync(c => c.Id == request.OriginalCourseId);
        var substituteExists = await _context.Courses.AnyAsync(c => c.Id == request.SubstituteCourseId);

        if (!originalExists)
            throw new ArgumentException($"Original course {request.OriginalCourseId} not found");

        if (!substituteExists)
            throw new ArgumentException($"Substitute course {request.SubstituteCourseId} not found");
    }

    private async Task<bool> ValidateSubstitutionEffectivenessAsync(CourseSubstitution substitution, StudentAcademicRecord studentRecord)
    {
        // Check if the substitution actually helps satisfy degree requirements
        // This would involve checking if the substitute course can fulfill the same requirements
        // as the original course. For now, return true if the substitution is active.
        return substitution.IsCurrentlyValid();
    }

    #endregion
}

#region Supporting Classes

public class StudentAcademicRecord
{
    public int StudentId { get; set; }
    public string DegreeCode { get; set; } = string.Empty;
    public List<CompletedCourse> CompletedCourses { get; set; } = new();
    public List<TransferCredit>? TransferCredits { get; set; }
    public List<CourseSubstitution>? ApprovedSubstitutions { get; set; }
    public decimal CumulativeGPA { get; set; }
    public int TotalCredits { get; set; }
}

public class CompletedCourse
{
    public int CourseId { get; set; }
    public string Grade { get; set; } = string.Empty;
    public int CreditHours { get; set; }
    public string Semester { get; set; } = string.Empty;
}

public class TransferCredit
{
    public int? CourseEquivalentId { get; set; }
    public int CreditHours { get; set; }
    public string Grade { get; set; } = string.Empty;
    public string Institution { get; set; } = string.Empty;
}

public class DegreeAuditResult
{
    public int StudentId { get; set; }
    public string DegreeCode { get; set; } = string.Empty;
    public DateTime AuditDate { get; set; }
    public int TotalCreditsCompleted { get; set; }
    public int TotalCreditsRequired { get; set; }
    public int RemainingCreditsNeeded { get; set; }
    public decimal CompletionPercentage { get; set; }
    public decimal CurrentGPA { get; set; }
    public decimal RequiredGPA { get; set; }
    public decimal GPADeficiency { get; set; }
    public decimal? MajorGPA { get; set; }
    public bool IsEligibleForGraduation { get; set; }
    public List<CategoryProgressResult> CategoryProgress { get; set; } = new();
    public List<SatisfiedRequirement> SatisfiedRequirements { get; set; } = new();
    public List<OutstandingRequirement> OutstandingRequirements { get; set; } = new();
    public List<ProcessedSubstitution> ApprovedSubstitutions { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();

    public void UpdateCompletionPercentage()
    {
        if (TotalCreditsRequired > 0)
        {
            CompletionPercentage = Math.Round((decimal)TotalCreditsCompleted / TotalCreditsRequired * 100, 2);
        }
        else
        {
            CompletionPercentage = 0;
        }
    }
}

public class CategoryProgressResult
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int CreditsCompleted { get; set; }
    public int CreditsRequired { get; set; }
    public int CreditsRemaining { get; set; }
    public decimal CompletionPercentage { get; set; }
    public bool IsComplete { get; set; }
    public List<SatisfiedRequirement> SatisfiedRequirements { get; set; } = new();
    public List<OutstandingRequirement> OutstandingRequirements { get; set; } = new();
}

public class SatisfiedRequirement
{
    public int RequirementId { get; set; }
    public string RequirementDescription { get; set; } = string.Empty;
    public string SatisfiedBy { get; set; } = string.Empty;
    public int CreditsSatisfied { get; set; }
}

public class OutstandingRequirement
{
    public int RequirementId { get; set; }
    public string RequirementDescription { get; set; } = string.Empty;
    public int CreditsNeeded { get; set; }
    public List<string> SuggestedCourses { get; set; } = new();
}

public class ProcessedSubstitution
{
    public int SubstitutionId { get; set; }
    public int OriginalCourseId { get; set; }
    public string OriginalCourseName { get; set; } = string.Empty;
    public string OriginalCourseTitle { get; set; } = string.Empty;
    public int SubstituteCourseId { get; set; }
    public string SubstituteCourseName { get; set; } = string.Empty;
    public string SubstituteCourseTitle { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string ApprovedBy { get; set; } = string.Empty;
    public DateTime ApprovalDate { get; set; }
    public int CreditHoursDifference { get; set; }
    public bool IsEffective { get; set; }
}

public class WhatIfAnalysisResult
{
    public int StudentId { get; set; }
    public List<int> ProspectiveCourses { get; set; } = new();
    public DateTime AnalysisDate { get; set; }
    public DegreeAuditResult CurrentProgress { get; set; } = null!;
    public DegreeAuditResult ProjectedProgress { get; set; } = null!;
    public int CreditImpact { get; set; }
    public decimal ProgressImpact { get; set; }
    public int RequirementsSatisfied { get; set; }
    public List<CategoryImpact> CategoryImpacts { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
}

public class CategoryImpact
{
    public string CategoryName { get; set; } = string.Empty;
    public decimal CurrentProgress { get; set; }
    public decimal ProjectedProgress { get; set; }
    public decimal ProgressGain { get; set; }
    public int AdditionalRequirementsSatisfied { get; set; }
}

public class SubstitutionRequest
{
    public int StudentId { get; set; }
    public int OriginalCourseId { get; set; }
    public int SubstituteCourseId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string RequestedBy { get; set; } = string.Empty;
    public string? AdditionalConditions { get; set; }
    public string? Notes { get; set; }
}

public class RequirementSatisfactionResult
{
    public int RequirementId { get; set; }
    public string RequirementDescription { get; set; } = string.Empty;
    public bool IsSatisfied { get; set; }
    public int CreditsSatisfied { get; set; }
    public int CreditsRequired { get; set; }
    public int ProgressPercentage { get; set; }
    public List<int> SatisfyingCourses { get; set; } = new();
    public List<int> SatisfiedConditionalIds { get; set; } = new();
}

#endregion