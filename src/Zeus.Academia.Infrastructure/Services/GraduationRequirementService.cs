using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Enums;
using Zeus.Academia.Infrastructure.Extensions;
using Zeus.Academia.Infrastructure.Models;

namespace Zeus.Academia.Infrastructure.Services;

/// <summary>
/// Service for validating graduation requirements including credit hours, GPA, 
/// residency requirements, course distribution, and time limits.
/// </summary>
public class GraduationRequirementService
{
    private readonly AcademiaDbContext _context;
    private readonly ILogger<GraduationRequirementService> _logger;

    public GraduationRequirementService(AcademiaDbContext context, ILogger<GraduationRequirementService> logger)
    {
        _context = context;
        _logger = logger;
    }

    #region Graduation Eligibility Validation

    /// <summary>
    /// Validates a student's eligibility for graduation against all criteria.
    /// </summary>
    /// <param name="studentRecord">Student's academic record</param>
    /// <param name="graduationCriteria">Graduation requirements</param>
    /// <returns>Comprehensive validation result</returns>
    public async Task<GraduationValidationResult> ValidateGraduationEligibilityAsync(
        Zeus.Academia.Infrastructure.Models.StudentAcademicRecord studentRecord,
        GraduationRequirements graduationCriteria)
    {
        try
        {
            _logger.LogInformation("Validating graduation eligibility for student {StudentId}", studentRecord.StudentId);

            var result = new GraduationValidationResult
            {
                StudentId = studentRecord.StudentId,
                ValidationDate = DateTime.UtcNow,
                Criteria = graduationCriteria
            };

            // Validate each graduation requirement
            await ValidateCreditRequirementsAsync(studentRecord, graduationCriteria, result);
            await ValidateGPARequirementsAsync(studentRecord, graduationCriteria, result);
            await ValidateResidencyRequirementsAsync(studentRecord, graduationCriteria, result);
            await ValidateTimeLimitsAsync(studentRecord, graduationCriteria, result);
            await ValidateCourseCompletionRequirementsAsync(studentRecord, graduationCriteria, result);
            await ValidateGradeRequirementsAsync(studentRecord, graduationCriteria, result);
            await ValidateSpecialRequirementsAsync(studentRecord, graduationCriteria, result);
            await ValidateApplicationRequirementsAsync(studentRecord, graduationCriteria, result);

            // Determine overall eligibility
            result.IsEligible = !result.BlockingIssues.Any();

            // Calculate graduation readiness score
            result.ReadinessScore = CalculateGraduationReadinessScore(result);

            // Generate recommendations
            result.Recommendations = GenerateGraduationRecommendations(result);

            _logger.LogInformation("Graduation validation completed: Eligible={IsEligible}, Score={Score}",
                result.IsEligible, result.ReadinessScore);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating graduation eligibility for student {StudentId}", studentRecord.StudentId);
            throw;
        }
    }

    /// <summary>
    /// Estimates time to graduation based on current progress and course load.
    /// </summary>
    /// <param name="studentRecord">Student's current record</param>
    /// <param name="totalCreditsRequired">Total credits needed for degree</param>
    /// <returns>Time estimation result</returns>
    public async Task<TimeToGraduationResult> EstimateTimeToGraduationAsync(
        Zeus.Academia.Infrastructure.Models.StudentAcademicRecord studentRecord,
        int totalCreditsRequired)
    {
        try
        {
            _logger.LogInformation("Estimating time to graduation for student {StudentId}", studentRecord.StudentId);

            var result = new TimeToGraduationResult
            {
                StudentId = studentRecord.StudentId,
                EstimationDate = DateTime.UtcNow,
                CurrentCredits = studentRecord.TotalCredits(),
                TotalCreditsRequired = totalCreditsRequired
            };

            // Calculate remaining credits
            result.RemainingCredits = Math.Max(0, totalCreditsRequired - studentRecord.TotalCredits());

            // Estimate based on current course load pattern
            var averageCourseLoad = await CalculateAverageCourseLoadAsync(studentRecord);

            // Account for summer sessions if applicable
            var effectiveCoursesPerYear = studentRecord.SummerCoursesTaken() ? 3 : 2; // Fall, Spring, (Summer)
            var creditsPerYear = averageCourseLoad * effectiveCoursesPerYear;

            if (creditsPerYear > 0)
            {
                var yearsRemaining = (double)result.RemainingCredits / creditsPerYear;
                result.EstimatedYearsRemaining = Math.Round(yearsRemaining, 1);

                // Convert to semesters (assuming Fall/Spring pattern)
                result.SemestersRemaining = (int)Math.Ceiling(yearsRemaining * 2);

                // Calculate estimated graduation date
                var currentDate = DateTime.UtcNow;
                var graduationMonth = DetermineGraduationMonth(result.SemestersRemaining);
                result.EstimatedGraduationDate = new DateTime(
                    currentDate.Year + (int)Math.Ceiling(yearsRemaining),
                    graduationMonth,
                    15);
            }

            // Account for prerequisite chains and course availability
            await AdjustForPrerequisiteConstraintsAsync(result, studentRecord);

            // Account for part-time vs full-time status
            await AdjustForEnrollmentStatusAsync(result, studentRecord);

            // Generate timeline milestones
            result.Milestones = GenerateGraduationMilestones(result, studentRecord);

            _logger.LogInformation("Time to graduation estimated: {Years} years, {Semesters} semesters",
                result.EstimatedYearsRemaining, result.SemestersRemaining);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error estimating time to graduation for student {StudentId}", studentRecord.StudentId);
            throw;
        }
    }

    /// <summary>
    /// Validates degree completion requirements for a specific degree program.
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <param name="degreeCode">Degree program code</param>
    /// <returns>Degree completion validation result</returns>
    public async Task<DegreeCompletionValidationResult> ValidateDegreeCompletionAsync(int studentId, string degreeCode)
    {
        try
        {
            _logger.LogInformation("Validating degree completion for student {StudentId} in degree {DegreeCode}",
                studentId, degreeCode);

            var result = new DegreeCompletionValidationResult
            {
                StudentId = studentId,
                DegreeCode = degreeCode,
                ValidationDate = DateTime.UtcNow
            };

            // Get degree template
            var degreeTemplate = await _context.DegreeRequirementTemplates
                .Include(t => t.Categories)
                    .ThenInclude(c => c.Requirements)
                .FirstOrDefaultAsync(t => t.DegreeCode == degreeCode && t.IsActive);

            if (degreeTemplate == null)
            {
                result.ValidationErrors.Add($"Active degree template not found for {degreeCode}");
                return result;
            }

            // Get student record
            var studentRecord = await GetStudentAcademicRecordAsync(studentId, degreeCode);

            // Validate each category completion
            foreach (var category in degreeTemplate.Categories)
            {
                var categoryValidation = await ValidateCategoryCompletionAsync(category, studentRecord);
                result.CategoryValidations.Add(categoryValidation);
            }

            // Overall validation
            result.IsComplete = result.CategoryValidations.All(cv => cv.IsComplete);
            result.CompletionPercentage = CalculateOverallCompletionPercentage(result.CategoryValidations);
            result.TotalCreditsCompleted = studentRecord.TotalCredits();
            result.TotalCreditsRequired = degreeTemplate.TotalCreditsRequired;

            // Check special requirements
            await ValidateSpecialDegreeRequirementsAsync(result, studentRecord, degreeTemplate);

            _logger.LogInformation("Degree completion validation: {CompletionPercentage}% complete",
                result.CompletionPercentage);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating degree completion for student {StudentId}", studentId);
            throw;
        }
    }

    #endregion

    #region Specific Requirement Validations

    /// <summary>
    /// Validates residency requirements for graduation.
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <param name="minimumResidencyCredits">Minimum residency credits required</param>
    /// <returns>Residency validation result</returns>
    public async Task<ResidencyValidationResult> ValidateResidencyRequirementsAsync(
        int studentId,
        int minimumResidencyCredits)
    {
        try
        {
            _logger.LogDebug("Validating residency requirements for student {StudentId}", studentId);

            var result = new ResidencyValidationResult
            {
                StudentId = studentId,
                MinimumResidencyCredits = minimumResidencyCredits,
                ValidationDate = DateTime.UtcNow
            };

            // Get student's enrollments at this institution
            var institutionEnrollments = await _context.CourseEnrollments
                .Where(e => e.StudentEmpNr == studentId &&
                           e.CountsTowardDegree &&
                           e.HasPassingGrade())
                .ToListAsync();

            result.ResidencyCreditsEarned = (int)institutionEnrollments.Sum(e => e.CreditHours);
            result.ResidencyCreditsRemaining = Math.Max(0, minimumResidencyCredits - result.ResidencyCreditsEarned);
            result.IsSatisfied = result.ResidencyCreditsEarned >= minimumResidencyCredits;

            // Calculate percentage completion
            result.CompletionPercentage = minimumResidencyCredits > 0
                ? Math.Round((decimal)result.ResidencyCreditsEarned / minimumResidencyCredits * 100, 1)
                : 100;

            // Identify residency-eligible courses
            result.ResidencyEligibleCourses = institutionEnrollments
                .Select(e => new ResidencyEligibleCourse
                {
                    CourseId = e.Id, // Use enrollment ID as proxy
                    CourseName = e.SubjectCode, // Use SubjectCode as course name proxy
                    CreditHours = (int)e.CreditHours
                }).ToList();

            _logger.LogDebug("Residency validation: {EarnedCredits}/{RequiredCredits} credits",
                result.ResidencyCreditsEarned, minimumResidencyCredits);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating residency requirements for student {StudentId}", studentId);
            throw;
        }
    }

    /// <summary>
    /// Validates GPA requirements including overall and major GPA.
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <param name="gpaRequirements">GPA requirements to validate</param>
    /// <returns>GPA validation result</returns>
    public async Task<GPAValidationResult> ValidateGPARequirementsAsync(
        int studentId,
        GPARequirements gpaRequirements)
    {
        try
        {
            _logger.LogDebug("Validating GPA requirements for student {StudentId}", studentId);

            var result = new GPAValidationResult
            {
                StudentId = studentId,
                Requirements = gpaRequirements,
                ValidationDate = DateTime.UtcNow
            };

            // Get all student enrollments with grades
            var enrollments = await _context.CourseEnrollments
                .Include(e => e.Subject)
                .Where(e => e.StudentEmpNr == studentId && e.HasPassingGrade())
                .ToListAsync();

            // Calculate overall GPA
            result.OverallGPA = CalculateGPA(enrollments);
            result.OverallGPAMet = result.OverallGPA >= gpaRequirements.MinimumOverallGPA;

            // Calculate major GPA if required
            if (gpaRequirements.MinimumMajorGPA.HasValue)
            {
                var student = await _context.Students.FindAsync(studentId);
                if (student?.GetMajorCode() != null)
                {
                    var majorSubjectCode = ExtractSubjectCodeFromMajor(student.GetMajorCode());
                    var majorEnrollments = enrollments
                        .Where(e => e.Subject.Code == majorSubjectCode)
                        .ToList();

                    result.MajorGPA = CalculateGPA(majorEnrollments);
                    result.MajorGPAMet = result.MajorGPA >= gpaRequirements.MinimumMajorGPA.Value;
                }
            }

            // Calculate upper-division GPA if required (using SubjectCode as proxy for course level)
            if (gpaRequirements.MinimumUpperDivisionGPA.HasValue)
            {
                var upperDivisionEnrollments = enrollments
                    .Where(e => e.SubjectCode.Length >= 4 && char.IsDigit(e.SubjectCode[3]) && int.Parse(e.SubjectCode[3].ToString()) >= 3)
                    .ToList();

                result.UpperDivisionGPA = CalculateGPA(upperDivisionEnrollments);
                result.UpperDivisionGPAMet = result.UpperDivisionGPA >= gpaRequirements.MinimumUpperDivisionGPA.Value;
            }

            // Overall GPA requirement satisfaction
            result.AllGPARequirementsMet = result.OverallGPAMet &&
                                         (!gpaRequirements.MinimumMajorGPA.HasValue || result.MajorGPAMet) &&
                                         (!gpaRequirements.MinimumUpperDivisionGPA.HasValue || result.UpperDivisionGPAMet);

            // Calculate GPA deficiencies
            result.OverallGPADeficiency = Math.Max(0, gpaRequirements.MinimumOverallGPA - result.OverallGPA);
            if (gpaRequirements.MinimumMajorGPA.HasValue && result.MajorGPA.HasValue)
            {
                result.MajorGPADeficiency = Math.Max(0, gpaRequirements.MinimumMajorGPA.Value - result.MajorGPA.Value);
            }

            _logger.LogDebug("GPA validation: Overall={OverallGPA:F2}, Major={MajorGPA:F2}, Met={AllMet}",
                result.OverallGPA, result.MajorGPA, result.AllGPARequirementsMet);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating GPA requirements for student {StudentId}", studentId);
            throw;
        }
    }

    /// <summary>
    /// Validates course distribution requirements across different areas.
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <param name="distributionRequirements">Distribution requirements to validate</param>
    /// <returns>Distribution validation result</returns>
    public async Task<DistributionValidationResult> ValidateDistributionRequirementsAsync(
        int studentId,
        List<DistributionRequirement> distributionRequirements)
    {
        try
        {
            _logger.LogDebug("Validating distribution requirements for student {StudentId}", studentId);

            var result = new DistributionValidationResult
            {
                StudentId = studentId,
                ValidationDate = DateTime.UtcNow
            };

            // Get student's completed courses
            var completedCourses = await GetCompletedCoursesAsync(studentId);

            foreach (var requirement in distributionRequirements)
            {
                var validation = new DistributionAreaValidation
                {
                    RequirementId = requirement.Id,
                    AreaName = requirement.AreaName,
                    CreditsRequired = requirement.MinimumCredits
                };

                // Find courses that satisfy this distribution area
                var satisfyingCourses = completedCourses
                    .Where(c => DoesCourseSatisfyDistribution(c, requirement))
                    .ToList();

                validation.CreditsEarned = (int)satisfyingCourses.Sum(c => c.CreditHours);
                validation.CreditsRemaining = Math.Max(0, requirement.MinimumCredits - validation.CreditsEarned);
                validation.IsSatisfied = validation.CreditsEarned >= requirement.MinimumCredits;
                validation.CompletionPercentage = requirement.MinimumCredits > 0
                    ? Math.Round((decimal)validation.CreditsEarned / requirement.MinimumCredits * 100, 1)
                    : 100;

                validation.SatisfyingCourses = satisfyingCourses
                    .Select(c => new DistributionCourse
                    {
                        CourseId = c.Id,
                        CourseName = $"{c.Subject.Code} {c.CourseNumber}",
                        CourseTitle = c.Title,
                        CreditHours = (int)c.CreditHours
                    }).ToList();

                result.AreaValidations.Add(validation);
            }

            result.AllDistributionRequirementsMet = result.AreaValidations.All(av => av.IsSatisfied);
            result.OverallCompletionPercentage = result.AreaValidations.Any()
                ? Math.Round(result.AreaValidations.Average(av => av.CompletionPercentage), 1)
                : 100;

            _logger.LogDebug("Distribution validation: {SatisfiedAreas}/{TotalAreas} areas satisfied",
                result.AreaValidations.Count(av => av.IsSatisfied), result.AreaValidations.Count);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating distribution requirements for student {StudentId}", studentId);
            throw;
        }
    }

    #endregion

    #region Private Helper Methods

    private Task ValidateCreditRequirementsAsync(Zeus.Academia.Infrastructure.Models.StudentAcademicRecord studentRecord, GraduationRequirements criteria, GraduationValidationResult result)
    {
        result.CreditRequirementMet = studentRecord.TotalCredits() >= criteria.MinimumCredits;

        if (!result.CreditRequirementMet)
        {
            var deficit = criteria.MinimumCredits - studentRecord.TotalCredits();
            result.BlockingIssues.Add($"Credit requirement not met: {deficit} credits short of {criteria.MinimumCredits} required");
        }

        return Task.CompletedTask;
    }

    private Task ValidateGPARequirementsAsync(Zeus.Academia.Infrastructure.Models.StudentAcademicRecord studentRecord, GraduationRequirements criteria, GraduationValidationResult result)
    {
        result.GPARequirementMet = studentRecord.CumulativeGPA() >= criteria.MinimumGPA;

        if (!result.GPARequirementMet)
        {
            var deficit = criteria.MinimumGPA - studentRecord.CumulativeGPA();
            result.BlockingIssues.Add($"GPA requirement not met: {deficit:F2} points below required {criteria.MinimumGPA:F2}");
        }

        return Task.CompletedTask;
    }

    private Task ValidateResidencyRequirementsAsync(Zeus.Academia.Infrastructure.Models.StudentAcademicRecord studentRecord, GraduationRequirements criteria, GraduationValidationResult result)
    {
        result.ResidencyRequirementMet = studentRecord.ResidencyCredits >= criteria.ResidencyCredits;

        if (!result.ResidencyRequirementMet)
        {
            var deficit = criteria.ResidencyCredits - studentRecord.ResidencyCredits;
            result.BlockingIssues.Add($"Residency requirement not met: {deficit} credits short of {criteria.ResidencyCredits} required");
        }

        return Task.CompletedTask;
    }

    private Task ValidateTimeLimitsAsync(Zeus.Academia.Infrastructure.Models.StudentAcademicRecord studentRecord, GraduationRequirements criteria, GraduationValidationResult result)
    {
        var timeInProgram = DateTime.UtcNow - studentRecord.StartDate;
        var yearsInProgram = timeInProgram.TotalDays / 365.25;

        result.TimeLimitMet = yearsInProgram <= criteria.MaxTimeLimit;

        if (!result.TimeLimitMet)
        {
            var excess = yearsInProgram - criteria.MaxTimeLimit;
            result.Warnings.Add($"Time limit exceeded: {excess:F1} years over the {criteria.MaxTimeLimit} year limit");
        }

        return Task.CompletedTask;
    }

    private Task ValidateCourseCompletionRequirementsAsync(Zeus.Academia.Infrastructure.Models.StudentAcademicRecord studentRecord, GraduationRequirements criteria, GraduationValidationResult result)
    {
        result.CourseCompletionRequirementMet = studentRecord.CompletedRequiredCourses() >= criteria.RequiredCourseCompletion;

        if (!result.CourseCompletionRequirementMet)
        {
            var deficit = criteria.RequiredCourseCompletion - studentRecord.CompletedRequiredCourses();
            result.Warnings.Add($"Course completion below target: {deficit}% short of {criteria.RequiredCourseCompletion}% required");
        }

        return Task.CompletedTask;
    }

    private Task ValidateGradeRequirementsAsync(Zeus.Academia.Infrastructure.Models.StudentAcademicRecord studentRecord, GraduationRequirements criteria, GraduationValidationResult result)
    {
        var gradeValue = ConvertGradeToValue(studentRecord.LowestMajorGrade);
        var requiredGradeValue = ConvertGradeToValue(criteria.MinimumGradeInMajor);

        result.GradeRequirementMet = gradeValue >= requiredGradeValue;

        if (!result.GradeRequirementMet)
        {
            result.BlockingIssues.Add($"Grade requirement not met: lowest major grade {studentRecord.LowestMajorGrade} below required {criteria.MinimumGradeInMajor}");
        }

        return Task.CompletedTask;
    }

    private Task ValidateSpecialRequirementsAsync(Zeus.Academia.Infrastructure.Models.StudentAcademicRecord studentRecord, GraduationRequirements criteria, GraduationValidationResult result)
    {
        result.CapstoneRequirementMet = !criteria.MustCompleteCapstone || studentRecord.CapstoneCompleted;

        if (!result.CapstoneRequirementMet)
        {
            result.BlockingIssues.Add("Capstone requirement not completed");
        }

        // Validate retake limits
        var retakeViolation = studentRecord.FailedCourseRetakes() > criteria.MaxFailedCourseRetakes;
        if (retakeViolation)
        {
            result.BlockingIssues.Add($"Too many course retakes: {studentRecord.FailedCourseRetakes()} exceeds maximum of {criteria.MaxFailedCourseRetakes}");
        }

        return Task.CompletedTask;
    }

    private Task ValidateApplicationRequirementsAsync(Zeus.Academia.Infrastructure.Models.StudentAcademicRecord studentRecord, GraduationRequirements criteria, GraduationValidationResult result)
    {
        result.ApplicationRequirementMet = !criteria.RequiredGraduationApplication || studentRecord.GraduationApplicationSubmitted;

        if (!result.ApplicationRequirementMet)
        {
            result.BlockingIssues.Add("Graduation application not submitted");
        }
        else if (criteria.RequiredGraduationApplication && studentRecord.ApplicationSubmissionDate.HasValue)
        {
            var leadTime = DateTime.UtcNow - studentRecord.ApplicationSubmissionDate.Value;
            var requiredLeadTimeMonths = criteria.ApplicationDeadlineMonths;

            if (leadTime.TotalDays < requiredLeadTimeMonths * 30)
            {
                result.Warnings.Add($"Graduation application submitted less than {requiredLeadTimeMonths} months in advance");
            }
        }

        return Task.CompletedTask;
    }

    private decimal CalculateGraduationReadinessScore(GraduationValidationResult result)
    {
        var totalCriteria = 8; // Total number of criteria checked
        var metCriteria = 0;

        if (result.CreditRequirementMet) metCriteria++;
        if (result.GPARequirementMet) metCriteria++;
        if (result.ResidencyRequirementMet) metCriteria++;
        if (result.TimeLimitMet) metCriteria++;
        if (result.CourseCompletionRequirementMet) metCriteria++;
        if (result.GradeRequirementMet) metCriteria++;
        if (result.CapstoneRequirementMet) metCriteria++;
        if (result.ApplicationRequirementMet) metCriteria++;

        return Math.Round((decimal)metCriteria / totalCriteria * 100, 1);
    }

    private List<string> GenerateGraduationRecommendations(GraduationValidationResult result)
    {
        var recommendations = new List<string>();

        if (!result.IsEligible)
        {
            recommendations.Add("Address blocking issues before applying for graduation:");
            recommendations.AddRange(result.BlockingIssues.Select(issue => $"- {issue}"));
        }

        if (result.Warnings.Any())
        {
            recommendations.Add("Consider addressing the following warnings:");
            recommendations.AddRange(result.Warnings.Select(warning => $"- {warning}"));
        }

        if (result.ReadinessScore >= 90)
        {
            recommendations.Add("You are very close to graduation eligibility. Review any remaining requirements carefully.");
        }
        else if (result.ReadinessScore >= 75)
        {
            recommendations.Add("You are making good progress toward graduation. Focus on completing outstanding requirements.");
        }
        else
        {
            recommendations.Add("Significant work remains to meet graduation requirements. Consider meeting with an academic advisor.");
        }

        return recommendations;
    }

    private async Task<int> CalculateAverageCourseLoadAsync(Zeus.Academia.Infrastructure.Models.StudentAcademicRecord studentRecord)
    {
        // Get recent enrollments to calculate average course load
        var recentEnrollments = await _context.CourseEnrollments
            .Where(e => e.StudentEmpNr == studentRecord.StudentId)
            .Where(e => e.AcademicYear >= DateTime.UtcNow.Year - 2) // Last 2 years
            .GroupBy(e => new { e.Semester, e.AcademicYear })
            .Select(g => g.Sum(e => e.CreditHours))
            .ToListAsync();

        return recentEnrollments.Any() ? (int)recentEnrollments.Average() : 15; // Default to 15 credits
    }

    private int DetermineGraduationMonth(int semestersRemaining)
    {
        // Determine likely graduation month based on semesters remaining
        var isEvenSemesters = semestersRemaining % 2 == 0;
        return isEvenSemesters ? 5 : 12; // May for even, December for odd
    }

    private Task AdjustForPrerequisiteConstraintsAsync(TimeToGraduationResult result, Zeus.Academia.Infrastructure.Models.StudentAcademicRecord studentRecord)
    {
        // This would analyze prerequisite chains and course availability to adjust timeline
        // For now, add a semester buffer if there are many remaining credits
        if (result.RemainingCredits > 30)
        {
            result.SemestersRemaining += 1;
            result.EstimatedYearsRemaining += 0.5;
            result.AdjustmentReasons.Add("Additional time added for prerequisite sequencing");
        }

        return Task.CompletedTask;
    }

    private Task AdjustForEnrollmentStatusAsync(TimeToGraduationResult result, Zeus.Academia.Infrastructure.Models.StudentAcademicRecord studentRecord)
    {
        // Adjust timeline based on part-time vs full-time status
        if (studentRecord.CurrentSemesterLoad < 12) // Part-time
        {
            result.EstimatedYearsRemaining *= 1.5; // 50% longer for part-time
            result.SemestersRemaining = (int)Math.Ceiling(result.EstimatedYearsRemaining * 2);
            result.AdjustmentReasons.Add("Timeline adjusted for part-time enrollment status");
        }

        return Task.CompletedTask;
    }

    private List<GraduationMilestone> GenerateGraduationMilestones(TimeToGraduationResult result, Zeus.Academia.Infrastructure.Models.StudentAcademicRecord studentRecord)
    {
        var milestones = new List<GraduationMilestone>();
        var currentDate = DateTime.UtcNow;

        // Add milestone for each year
        for (int year = 1; year <= Math.Ceiling(result.EstimatedYearsRemaining); year++)
        {
            var milestoneDate = currentDate.AddYears(year);
            var expectedCredits = studentRecord.TotalCredits() + (year * 30); // Assume 30 credits per year

            milestones.Add(new GraduationMilestone
            {
                Year = year,
                Date = milestoneDate,
                Description = $"Complete approximately {expectedCredits} total credits",
                IsRequired = false
            });
        }

        // Add specific milestones
        if (result.SemestersRemaining > 4)
        {
            milestones.Add(new GraduationMilestone
            {
                Year = 2,
                Date = currentDate.AddYears(2),
                Description = "Meet with academic advisor to review degree progress",
                IsRequired = true
            });
        }

        if (result.SemestersRemaining > 2)
        {
            milestones.Add(new GraduationMilestone
            {
                Year = (int)Math.Max(1, result.EstimatedYearsRemaining - 1),
                Date = result.EstimatedGraduationDate.AddYears(-1),
                Description = "Submit graduation application",
                IsRequired = true
            });
        }

        return milestones.OrderBy(m => m.Date).ToList();
    }

    private async Task<Zeus.Academia.Infrastructure.Models.StudentAcademicRecord> GetStudentAcademicRecordAsync(int studentId, string degreeCode)
    {
        // Implementation would build comprehensive academic record
        // This is a simplified version
        var enrollments = await _context.CourseEnrollments
            .Where(e => e.StudentEmpNr == studentId && e.Grades.Any())
            .ToListAsync();

        var student = await _context.Students.FindAsync(studentId);

        return new Zeus.Academia.Infrastructure.Models.StudentAcademicRecord
        {
            StudentId = studentId,
            TotalCreditsEarned = enrollments.Sum(e => e.CreditHours),
            OverallGPA = CalculateGPA(enrollments),
            StartDate = student?.EnrollmentDate ?? DateTime.UtcNow.AddYears(-2),
            SummerCoursesTaken = enrollments.Count(e => e.Semester?.ToLower().Contains("summer") == true),
            ResidencyCredits = enrollments.Where(e => e.CountsTowardDegree).Sum(e => e.CreditHours),
            CompletedRequiredCourses = enrollments.Where(e => e.HasPassingGrade()).Select(e => e.Id).ToList(),
            LowestMajorGrade = enrollments.Where(e => e.HasPassingGrade()).Min(e => e.GetEffectiveGrade()) ?? "A",
            CapstoneCompleted = enrollments.Any(e => e.SubjectCode.Contains("CAPS")),
            GraduationApplicationSubmitted = false, // This would need to be determined from another source
            CurrentSemesterLoad = 15 // Default assumption
        };
    }

    private Task<CategoryCompletionValidation> ValidateCategoryCompletionAsync(RequirementCategory category, Zeus.Academia.Infrastructure.Models.StudentAcademicRecord studentRecord)
    {
        var validation = new CategoryCompletionValidation
        {
            CategoryId = category.Id,
            CategoryName = category.Name,
            CreditsRequired = category.CreditsRequired
        };

        // This would involve complex logic to determine category completion
        // Since Models.StudentAcademicRecord doesn't have CompletedCourses, we'll use a simplified approach
        // In a real implementation, this would query the CourseEnrollments directly
        var relevantCredits = 0; // Placeholder - would need actual implementation based on category requirements

        validation.CreditsEarned = Math.Min(relevantCredits, category.CreditsRequired);
        validation.CreditsRemaining = Math.Max(0, category.CreditsRequired - validation.CreditsEarned);
        validation.IsComplete = validation.CreditsEarned >= category.CreditsRequired;
        validation.CompletionPercentage = category.CreditsRequired > 0
            ? Math.Round((decimal)validation.CreditsEarned / category.CreditsRequired * 100, 1)
            : 100;

        return Task.FromResult(validation);
    }

    private bool IsCourseRelevantToCategory(CompletedCourse course, RequirementCategory category)
    {
        // Simplified logic - would need more sophisticated mapping
        return true;
    }

    private async Task ValidateSpecialDegreeRequirementsAsync(DegreeCompletionValidationResult result, Zeus.Academia.Infrastructure.Models.StudentAcademicRecord studentRecord, DegreeRequirementTemplate degreeTemplate)
    {
        // Validate residency requirements
        if (degreeTemplate.ResidencyCreditsRequired > 0)
        {
            var residencyValidation = await ValidateResidencyRequirementsAsync(studentRecord.StudentId, degreeTemplate.ResidencyCreditsRequired);
            if (!residencyValidation.IsSatisfied)
            {
                result.ValidationErrors.Add($"Residency requirement not met: {residencyValidation.ResidencyCreditsRemaining} credits short");
            }
        }

        // Validate GPA requirements
        if (degreeTemplate.MinimumGPA > 0)
        {
            if (studentRecord.CumulativeGPA() < degreeTemplate.MinimumGPA)
            {
                result.ValidationErrors.Add($"GPA requirement not met: {studentRecord.CumulativeGPA:F2} below required {degreeTemplate.MinimumGPA:F2}");
            }
        }
    }

    private decimal CalculateOverallCompletionPercentage(List<CategoryCompletionValidation> categoryValidations)
    {
        if (!categoryValidations.Any()) return 100;

        var totalRequired = categoryValidations.Sum(cv => cv.CreditsRequired);
        var totalEarned = categoryValidations.Sum(cv => cv.CreditsEarned);

        return totalRequired > 0 ? Math.Round((decimal)totalEarned / totalRequired * 100, 1) : 100;
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
                totalPoints += gradePoints[grade] * enrollment.CreditHours;
                totalCredits += (int)enrollment.CreditHours;
            }
        }

        return totalCredits > 0 ? Math.Round(totalPoints / totalCredits, 2) : 0;
    }

    private string ExtractSubjectCodeFromMajor(string majorCode)
    {
        // Simple extraction - would need more sophisticated mapping
        return majorCode.Length >= 4 ? majorCode.Substring(0, 4) : majorCode;
    }

    private decimal ConvertGradeToValue(string grade)
    {
        var gradeValues = new Dictionary<string, decimal>
        {
            {"A+", 4.0m}, {"A", 4.0m}, {"A-", 3.7m},
            {"B+", 3.3m}, {"B", 3.0m}, {"B-", 2.7m},
            {"C+", 2.3m}, {"C", 2.0m}, {"C-", 1.7m},
            {"D+", 1.3m}, {"D", 1.0m}, {"D-", 0.7m},
            {"F", 0.0m}
        };

        return gradeValues.GetValueOrDefault(grade, 0.0m);
    }

    private async Task<List<Course>> GetCompletedCoursesAsync(int studentId)
    {
        var enrollments = await _context.CourseEnrollments
            .Where(e => e.StudentEmpNr == studentId && e.Grades.Any(g => g.LetterGrade != "W"))
            .ToListAsync();

        // Since CourseEnrollment doesn't have direct Course navigation, we need to query differently
        // For now, return empty list - this would need proper implementation
        return new List<Course>();
    }

    private bool DoesCourseSatisfyDistribution(Course course, DistributionRequirement requirement)
    {
        // This would contain logic to determine if a course satisfies a distribution requirement
        // Could be based on subject codes, course attributes, or other criteria
        return requirement.EligibleSubjectCodes?.Contains(course.Subject.Code) == true ||
               requirement.EligibleCourseAttributes?.Any(attr => course.Title.Contains(attr)) == true;
    }

    #endregion
}

#region Supporting Classes

public class GraduationRequirements
{
    public int MinimumCredits { get; set; } = 120;
    public decimal MinimumGPA { get; set; } = 2.0m;
    public int ResidencyCredits { get; set; } = 30;
    public int MaxTimeLimit { get; set; } = 6; // years
    public int RequiredCourseCompletion { get; set; } = 100; // percentage
    public string MinimumGradeInMajor { get; set; } = "C";
    public int MaxFailedCourseRetakes { get; set; } = 2;
    public bool MustCompleteCapstone { get; set; } = false;
    public bool RequiredGraduationApplication { get; set; } = true;
    public int ApplicationDeadlineMonths { get; set; } = 2; // months before graduation
}

public class GraduationValidationResult
{
    public int StudentId { get; set; }
    public DateTime ValidationDate { get; set; }
    public GraduationRequirements Criteria { get; set; } = null!;
    public bool IsEligible { get; set; }
    public decimal ReadinessScore { get; set; }

    // Individual requirement validations
    public bool CreditRequirementMet { get; set; }
    public bool GPARequirementMet { get; set; }
    public bool ResidencyRequirementMet { get; set; }
    public bool TimeLimitMet { get; set; }
    public bool CourseCompletionRequirementMet { get; set; }
    public bool GradeRequirementMet { get; set; }
    public bool CapstoneRequirementMet { get; set; }
    public bool ApplicationRequirementMet { get; set; }

    public List<string> BlockingIssues { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
}

public class TimeToGraduationResult
{
    public int StudentId { get; set; }
    public DateTime EstimationDate { get; set; }
    public int CurrentCredits { get; set; }
    public int TotalCreditsRequired { get; set; }
    public int RemainingCredits { get; set; }
    public double EstimatedYearsRemaining { get; set; }
    public int SemestersRemaining { get; set; }
    public DateTime EstimatedGraduationDate { get; set; }
    public List<string> AdjustmentReasons { get; set; } = new();
    public List<GraduationMilestone> Milestones { get; set; } = new();
}

public class GraduationMilestone
{
    public int Year { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
}

public class DegreeCompletionValidationResult
{
    public int StudentId { get; set; }
    public string DegreeCode { get; set; } = string.Empty;
    public DateTime ValidationDate { get; set; }
    public bool IsComplete { get; set; }
    public decimal CompletionPercentage { get; set; }
    public int TotalCreditsCompleted { get; set; }
    public int TotalCreditsRequired { get; set; }
    public List<CategoryCompletionValidation> CategoryValidations { get; set; } = new();
    public List<string> ValidationErrors { get; set; } = new();
}

public class CategoryCompletionValidation
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int CreditsEarned { get; set; }
    public int CreditsRequired { get; set; }
    public int CreditsRemaining { get; set; }
    public bool IsComplete { get; set; }
    public decimal CompletionPercentage { get; set; }
}

public class ResidencyValidationResult
{
    public int StudentId { get; set; }
    public DateTime ValidationDate { get; set; }
    public int MinimumResidencyCredits { get; set; }
    public int ResidencyCreditsEarned { get; set; }
    public int ResidencyCreditsRemaining { get; set; }
    public bool IsSatisfied { get; set; }
    public decimal CompletionPercentage { get; set; }
    public List<ResidencyEligibleCourse> ResidencyEligibleCourses { get; set; } = new();
}

public class ResidencyEligibleCourse
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int CreditHours { get; set; }
    public string Term { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Grade { get; set; } = string.Empty;
}

public class GPARequirements
{
    public decimal MinimumOverallGPA { get; set; } = 2.0m;
    public decimal? MinimumMajorGPA { get; set; }
    public decimal? MinimumUpperDivisionGPA { get; set; }
}

public class GPAValidationResult
{
    public int StudentId { get; set; }
    public DateTime ValidationDate { get; set; }
    public GPARequirements Requirements { get; set; } = null!;

    public decimal OverallGPA { get; set; }
    public bool OverallGPAMet { get; set; }
    public decimal OverallGPADeficiency { get; set; }

    public decimal? MajorGPA { get; set; }
    public bool MajorGPAMet { get; set; }
    public decimal MajorGPADeficiency { get; set; }

    public decimal? UpperDivisionGPA { get; set; }
    public bool UpperDivisionGPAMet { get; set; }

    public bool AllGPARequirementsMet { get; set; }
}

public class DistributionRequirement
{
    public int Id { get; set; }
    public string AreaName { get; set; } = string.Empty;
    public int MinimumCredits { get; set; }
    public List<string>? EligibleSubjectCodes { get; set; }
    public List<string>? EligibleCourseAttributes { get; set; }
}

public class DistributionValidationResult
{
    public int StudentId { get; set; }
    public DateTime ValidationDate { get; set; }
    public bool AllDistributionRequirementsMet { get; set; }
    public decimal OverallCompletionPercentage { get; set; }
    public List<DistributionAreaValidation> AreaValidations { get; set; } = new();
}

public class DistributionAreaValidation
{
    public int RequirementId { get; set; }
    public string AreaName { get; set; } = string.Empty;
    public int CreditsEarned { get; set; }
    public int CreditsRequired { get; set; }
    public int CreditsRemaining { get; set; }
    public bool IsSatisfied { get; set; }
    public decimal CompletionPercentage { get; set; }
    public List<DistributionCourse> SatisfyingCourses { get; set; } = new();
}

public class DistributionCourse
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string CourseTitle { get; set; } = string.Empty;
    public int CreditHours { get; set; }
}

#endregion