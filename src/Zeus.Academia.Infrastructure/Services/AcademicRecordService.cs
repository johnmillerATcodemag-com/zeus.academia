using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Enums;
using Zeus.Academia.Infrastructure.Services.Interfaces;

namespace Zeus.Academia.Infrastructure.Services;

/// <summary>
/// Service for managing academic records including course enrollments, grades, and transcripts.
/// Implementation of Prompt 4 Task 4: Academic Record Management.
/// </summary>
public class AcademicRecordService : IAcademicRecordService
{
    private readonly AcademiaDbContext _context;
    private readonly ILogger<AcademicRecordService> _logger;

    public AcademicRecordService(AcademiaDbContext context, ILogger<AcademicRecordService> logger)
    {
        _context = context;
        _logger = logger;
    }

    #region Course Enrollment Management

    public async Task<CourseEnrollment?> EnrollStudentInCourseAsync(int studentEmpNr, string subjectCode, string? sectionId = null, int? academicTermId = null, decimal creditHours = 3.0m, bool isAudit = false)
    {
        try
        {
            _logger.LogInformation("Enrolling student {StudentEmpNr} in course {SubjectCode}", studentEmpNr, subjectCode);

            // Verify student exists
            var student = await _context.Students.FindAsync(studentEmpNr);
            if (student == null)
            {
                _logger.LogWarning("Student {StudentEmpNr} not found", studentEmpNr);
                return null;
            }

            // Verify subject exists
            var subject = await _context.Subjects.FindAsync(subjectCode);
            if (subject == null)
            {
                _logger.LogWarning("Subject {SubjectCode} not found", subjectCode);
                return null;
            }

            // Check for duplicate enrollment
            var existingEnrollment = await _context.CourseEnrollments
                .Where(ce => ce.StudentEmpNr == studentEmpNr &&
                           ce.SubjectCode == subjectCode &&
                           ce.Status == CourseEnrollmentStatus.Enrolled)
                .FirstOrDefaultAsync();

            if (existingEnrollment != null)
            {
                _logger.LogWarning("Student {StudentEmpNr} is already enrolled in {SubjectCode}", studentEmpNr, subjectCode);
                return null;
            }

            // Get current academic term if not provided
            if (!academicTermId.HasValue)
            {
                var currentTerm = await _context.AcademicTerms
                    .Where(at => at.IsActive && DateTime.Now >= at.StartDate && DateTime.Now <= at.EndDate)
                    .FirstOrDefaultAsync();
                academicTermId = currentTerm?.Id;
            }

            var enrollment = new CourseEnrollment
            {
                StudentEmpNr = studentEmpNr,
                SubjectCode = subjectCode,
                SectionId = sectionId,
                AcademicTermId = academicTermId,
                AcademicYear = DateTime.Now.Year,
                Semester = GetCurrentSemester(),
                Status = CourseEnrollmentStatus.Enrolled,
                EnrollmentDate = DateTime.UtcNow,
                CreditHours = subject.CreditHours ?? creditHours, // Use credit hours from subject, fallback to parameter
                IsAudit = isAudit,
                CountsTowardDegree = !isAudit,
                CreatedBy = "System",
                ModifiedBy = "System"
            };

            _context.CourseEnrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully enrolled student {StudentEmpNr} in course {SubjectCode}", studentEmpNr, subjectCode);
            return enrollment;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enrolling student {StudentEmpNr} in course {SubjectCode}", studentEmpNr, subjectCode);
            return null;
        }
    }

    public async Task<bool> DropStudentFromCourseAsync(int enrollmentId, string? dropReason = null)
    {
        try
        {
            _logger.LogInformation("Dropping student from course enrollment {EnrollmentId}", enrollmentId);

            var enrollment = await _context.CourseEnrollments.FindAsync(enrollmentId);
            if (enrollment == null)
            {
                _logger.LogWarning("Course enrollment {EnrollmentId} not found", enrollmentId);
                return false;
            }

            if (enrollment.Status != CourseEnrollmentStatus.Enrolled)
            {
                _logger.LogWarning("Cannot drop from course enrollment {EnrollmentId} - current status: {Status}", enrollmentId, enrollment.Status);
                return false;
            }

            enrollment.Status = CourseEnrollmentStatus.Dropped;
            enrollment.DropDate = DateTime.UtcNow;
            enrollment.Notes = string.IsNullOrEmpty(enrollment.Notes)
                ? $"Dropped: {dropReason ?? "No reason provided"}"
                : $"{enrollment.Notes}; Dropped: {dropReason ?? "No reason provided"}";
            enrollment.ModifiedBy = "System";

            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully dropped student from course enrollment {EnrollmentId}", enrollmentId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error dropping student from course enrollment {EnrollmentId}", enrollmentId);
            return false;
        }
    }

    public async Task<bool> WithdrawStudentFromCourseAsync(int enrollmentId, string? withdrawalReason = null)
    {
        try
        {
            _logger.LogInformation("Withdrawing student from course enrollment {EnrollmentId}", enrollmentId);

            var enrollment = await _context.CourseEnrollments.FindAsync(enrollmentId);
            if (enrollment == null)
            {
                _logger.LogWarning("Course enrollment {EnrollmentId} not found", enrollmentId);
                return false;
            }

            if (enrollment.Status != CourseEnrollmentStatus.Enrolled)
            {
                _logger.LogWarning("Cannot withdraw from course enrollment {EnrollmentId} - current status: {Status}", enrollmentId, enrollment.Status);
                return false;
            }

            enrollment.Status = CourseEnrollmentStatus.Withdrawn;
            enrollment.WithdrawalDate = DateTime.UtcNow;
            enrollment.Notes = string.IsNullOrEmpty(enrollment.Notes)
                ? $"Withdrawn: {withdrawalReason ?? "No reason provided"}"
                : $"{enrollment.Notes}; Withdrawn: {withdrawalReason ?? "No reason provided"}";
            enrollment.ModifiedBy = "System";

            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully withdrew student from course enrollment {EnrollmentId}", enrollmentId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error withdrawing student from course enrollment {EnrollmentId}", enrollmentId);
            return false;
        }
    }

    public async Task<IEnumerable<CourseEnrollment>> GetStudentEnrollmentsAsync(int studentEmpNr, int? academicYear = null, string? semester = null)
    {
        try
        {
            var query = _context.CourseEnrollments
                .Include(ce => ce.Subject)
                .Include(ce => ce.AcademicTerm)
                .Include(ce => ce.Grades)
                .Where(ce => ce.StudentEmpNr == studentEmpNr);

            if (academicYear.HasValue)
                query = query.Where(ce => ce.AcademicYear == academicYear.Value);

            if (!string.IsNullOrEmpty(semester))
                query = query.Where(ce => ce.Semester == semester);

            return await query
                .OrderByDescending(ce => ce.AcademicYear)
                .ThenByDescending(ce => ce.Semester)
                .ThenBy(ce => ce.SubjectCode)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting enrollments for student {StudentEmpNr}", studentEmpNr);
            return new List<CourseEnrollment>();
        }
    }

    public async Task<IEnumerable<CourseEnrollment>> GetStudentEnrollmentHistoryAsync(int studentEmpNr)
    {
        try
        {
            return await _context.CourseEnrollments
                .Include(ce => ce.Subject)
                .Include(ce => ce.AcademicTerm)
                .Include(ce => ce.Grades)
                .Where(ce => ce.StudentEmpNr == studentEmpNr)
                .OrderByDescending(ce => ce.AcademicYear)
                .ThenByDescending(ce => ce.Semester)
                .ThenBy(ce => ce.SubjectCode)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting enrollment history for student {StudentEmpNr}", studentEmpNr);
            return new List<CourseEnrollment>();
        }
    }

    #endregion

    #region Grade Management

    public async Task<Grade?> RecordGradeAsync(int courseEnrollmentId, GradeType gradeType, string? letterGrade = null, decimal? numericGrade = null, string? gradedBy = null, string? comments = null)
    {
        try
        {
            _logger.LogInformation("Recording grade for course enrollment {CourseEnrollmentId}", courseEnrollmentId);

            var enrollment = await _context.CourseEnrollments.FindAsync(courseEnrollmentId);
            if (enrollment == null)
            {
                _logger.LogWarning("Course enrollment {CourseEnrollmentId} not found", courseEnrollmentId);
                return null;
            }

            // Convert grades if needed
            if (!string.IsNullOrEmpty(letterGrade) && !numericGrade.HasValue)
            {
                numericGrade = ConvertLetterGradeToNumeric(letterGrade);
            }
            else if (numericGrade.HasValue && string.IsNullOrEmpty(letterGrade))
            {
                letterGrade = ConvertNumericGradeToLetter(numericGrade.Value);
            }

            var gradePoints = !string.IsNullOrEmpty(letterGrade) ? ConvertLetterGradeToPoints(letterGrade) : 0m;
            var qualityPoints = gradePoints * enrollment.CreditHours;

            var grade = new Grade
            {
                CourseEnrollmentId = courseEnrollmentId,
                GradeType = gradeType,
                LetterGrade = letterGrade,
                NumericGrade = numericGrade,
                GradePoints = gradePoints,
                CreditHours = enrollment.CreditHours,
                QualityPoints = qualityPoints,
                Status = GradeStatus.Posted,
                IsFinal = gradeType == GradeType.Final,
                GradeDate = DateTime.UtcNow,
                PostedDate = DateTime.UtcNow,
                GradedBy = gradedBy,
                Comments = comments,
                CreatedBy = gradedBy ?? "System",
                ModifiedBy = gradedBy ?? "System"
            };

            _context.Grades.Add(grade);

            // Update enrollment status if final grade
            if (gradeType == GradeType.Final)
            {
                enrollment.Status = CourseEnrollmentStatus.Completed;
                enrollment.ModifiedBy = gradedBy ?? "System";
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully recorded grade for course enrollment {CourseEnrollmentId}", courseEnrollmentId);
            return grade;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording grade for course enrollment {CourseEnrollmentId}", courseEnrollmentId);
            return null;
        }
    }

    public async Task<bool> UpdateGradeAsync(int gradeId, string? letterGrade = null, decimal? numericGrade = null, string? comments = null, string? gradedBy = null)
    {
        try
        {
            _logger.LogInformation("Updating grade {GradeId}", gradeId);

            var grade = await _context.Grades.FindAsync(gradeId);
            if (grade == null)
            {
                _logger.LogWarning("Grade {GradeId} not found", gradeId);
                return false;
            }

            var enrollment = await _context.CourseEnrollments.FindAsync(grade.CourseEnrollmentId);
            if (enrollment == null)
            {
                _logger.LogWarning("Course enrollment {CourseEnrollmentId} not found for grade {GradeId}", grade.CourseEnrollmentId, gradeId);
                return false;
            }

            // Update grade information
            if (!string.IsNullOrEmpty(letterGrade))
            {
                grade.LetterGrade = letterGrade;
                grade.GradePoints = ConvertLetterGradeToPoints(letterGrade);
                grade.QualityPoints = (grade.GradePoints ?? 0m) * grade.CreditHours;
            }

            if (numericGrade.HasValue)
            {
                grade.NumericGrade = numericGrade.Value;
                if (string.IsNullOrEmpty(letterGrade))
                {
                    grade.LetterGrade = ConvertNumericGradeToLetter(numericGrade.Value);
                    grade.GradePoints = ConvertLetterGradeToPoints(grade.LetterGrade);
                    grade.QualityPoints = (grade.GradePoints ?? 0m) * grade.CreditHours;
                }
            }

            if (!string.IsNullOrEmpty(comments))
                grade.Comments = comments;

            grade.Status = GradeStatus.Changed;
            grade.ModifiedBy = gradedBy ?? grade.ModifiedBy;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully updated grade {GradeId}", gradeId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating grade {GradeId}", gradeId);
            return false;
        }
    }

    public async Task<IEnumerable<Grade>> GetStudentGradesAsync(int studentEmpNr, int? academicYear = null, string? semester = null)
    {
        try
        {
            var query = _context.Grades
                .Include(g => g.CourseEnrollment)
                    .ThenInclude(ce => ce.Subject)
                .Where(g => g.CourseEnrollment.StudentEmpNr == studentEmpNr);

            if (academicYear.HasValue)
                query = query.Where(g => g.CourseEnrollment.AcademicYear == academicYear.Value);

            if (!string.IsNullOrEmpty(semester))
                query = query.Where(g => g.CourseEnrollment.Semester == semester);

            return await query
                .OrderByDescending(g => g.CourseEnrollment.AcademicYear)
                .ThenByDescending(g => g.CourseEnrollment.Semester)
                .ThenBy(g => g.CourseEnrollment.SubjectCode)
                .ThenBy(g => g.GradeType)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting grades for student {StudentEmpNr}", studentEmpNr);
            return new List<Grade>();
        }
    }

    public async Task<IEnumerable<Grade>> GetCourseEnrollmentGradesAsync(int courseEnrollmentId)
    {
        try
        {
            return await _context.Grades
                .Where(g => g.CourseEnrollmentId == courseEnrollmentId)
                .OrderBy(g => g.GradeType)
                .ThenBy(g => g.GradeDate)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting grades for course enrollment {CourseEnrollmentId}", courseEnrollmentId);
            return new List<Grade>();
        }
    }

    public async Task<Grade?> GetFinalGradeAsync(int courseEnrollmentId)
    {
        try
        {
            return await _context.Grades
                .Where(g => g.CourseEnrollmentId == courseEnrollmentId && g.IsFinal)
                .OrderByDescending(g => g.GradeDate)
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting final grade for course enrollment {CourseEnrollmentId}", courseEnrollmentId);
            return null;
        }
    }

    #endregion

    #region GPA Calculation

    public async Task<decimal> CalculateCumulativeGPAAsync(int studentEmpNr)
    {
        try
        {
            var grades = await _context.Grades
                .Include(g => g.CourseEnrollment)
                .Where(g => g.CourseEnrollment.StudentEmpNr == studentEmpNr &&
                           g.IsFinal &&
                           g.CourseEnrollment.CountsTowardDegree &&
                           !g.CourseEnrollment.IsAudit)
                .ToListAsync();

            if (!grades.Any())
                return 0m;

            var totalQualityPoints = grades.Sum(g => g.QualityPoints);
            var totalCreditHours = grades.Sum(g => g.CreditHours);

            return totalCreditHours > 0 ? Math.Round(totalQualityPoints / totalCreditHours, 2) : 0m;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating cumulative GPA for student {StudentEmpNr}", studentEmpNr);
            return 0m;
        }
    }

    public async Task<decimal> CalculateTermGPAAsync(int studentEmpNr, int academicYear, string semester)
    {
        try
        {
            var grades = await _context.Grades
                .Include(g => g.CourseEnrollment)
                .Where(g => g.CourseEnrollment.StudentEmpNr == studentEmpNr &&
                           g.CourseEnrollment.AcademicYear == academicYear &&
                           g.CourseEnrollment.Semester == semester &&
                           g.IsFinal &&
                           g.CourseEnrollment.CountsTowardDegree &&
                           !g.CourseEnrollment.IsAudit)
                .ToListAsync();

            if (!grades.Any())
                return 0m;

            var totalQualityPoints = grades.Sum(g => g.QualityPoints);
            var totalCreditHours = grades.Sum(g => g.CreditHours);

            return totalCreditHours > 0 ? Math.Round(totalQualityPoints / totalCreditHours, 2) : 0m;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating term GPA for student {StudentEmpNr} for {Semester} {AcademicYear}", studentEmpNr, semester, academicYear);
            return 0m;
        }
    }

    public async Task<decimal> CalculateMajorGPAAsync(int studentEmpNr, string majorDepartment)
    {
        try
        {
            var grades = await _context.Grades
                .Include(g => g.CourseEnrollment)
                    .ThenInclude(ce => ce.Subject)
                .Where(g => g.CourseEnrollment.StudentEmpNr == studentEmpNr &&
                           g.CourseEnrollment.Subject.DepartmentName == majorDepartment &&
                           g.IsFinal &&
                           g.CourseEnrollment.CountsTowardDegree &&
                           !g.CourseEnrollment.IsAudit)
                .ToListAsync();

            if (!grades.Any())
                return 0m;

            var totalQualityPoints = grades.Sum(g => g.QualityPoints);
            var totalCreditHours = grades.Sum(g => g.CreditHours);

            return totalCreditHours > 0 ? Math.Round(totalQualityPoints / totalCreditHours, 2) : 0m;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating major GPA for student {StudentEmpNr} in department {Department}", studentEmpNr, majorDepartment);
            return 0m;
        }
    }

    public async Task<IEnumerable<(int AcademicYear, string Semester, decimal GPA, decimal CumulativeGPA)>> GetGPAHistoryAsync(int studentEmpNr)
    {
        try
        {
            var enrollments = await _context.CourseEnrollments
                .Include(ce => ce.Grades.Where(g => g.IsFinal))
                .Where(ce => ce.StudentEmpNr == studentEmpNr && ce.CountsTowardDegree && !ce.IsAudit)
                .OrderBy(ce => ce.AcademicYear)
                .ThenBy(ce => ce.Semester)
                .ToListAsync();

            var gpaHistory = new List<(int AcademicYear, string Semester, decimal GPA, decimal CumulativeGPA)>();
            var cumulativeQualityPoints = 0m;
            var cumulativeCreditHours = 0m;

            var termGroups = enrollments
                .GroupBy(ce => new { ce.AcademicYear, ce.Semester })
                .OrderBy(g => g.Key.AcademicYear)
                .ThenBy(g => g.Key.Semester);

            foreach (var termGroup in termGroups)
            {
                var termEnrollments = termGroup.ToList();
                var termGrades = termEnrollments.SelectMany(ce => ce.Grades.Where(g => g.IsFinal)).ToList();

                if (termGrades.Any())
                {
                    var termQualityPoints = termGrades.Sum(g => g.QualityPoints);
                    var termCreditHours = termGrades.Sum(g => g.CreditHours);
                    var termGPA = termCreditHours > 0 ? Math.Round(termQualityPoints / termCreditHours, 2) : 0m;

                    cumulativeQualityPoints += termQualityPoints;
                    cumulativeCreditHours += termCreditHours;
                    var cumulativeGPA = cumulativeCreditHours > 0 ? Math.Round(cumulativeQualityPoints / cumulativeCreditHours, 2) : 0m;

                    gpaHistory.Add((termGroup.Key.AcademicYear, termGroup.Key.Semester ?? "Unknown", termGPA, cumulativeGPA));
                }
            }

            return gpaHistory;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting GPA history for student {StudentEmpNr}", studentEmpNr);
            return new List<(int, string, decimal, decimal)>();
        }
    }

    #endregion

    #region Transcript Generation

    public async Task<TranscriptData> GenerateTranscriptAsync(int studentEmpNr, bool includeInProgress = false)
    {
        try
        {
            _logger.LogInformation("Generating official transcript for student {StudentEmpNr}", studentEmpNr);

            var student = await _context.Students
                .Include(s => s.Department)
                .Include(s => s.Degree)
                .FirstOrDefaultAsync(s => s.EmpNr == studentEmpNr);

            if (student == null)
            {
                throw new ArgumentException($"Student {studentEmpNr} not found");
            }

            var enrollmentQuery = _context.CourseEnrollments
                .Include(ce => ce.Subject)
                .Include(ce => ce.Grades)
                .Where(ce => ce.StudentEmpNr == studentEmpNr);

            if (!includeInProgress)
            {
                enrollmentQuery = enrollmentQuery.Where(ce => ce.Status == CourseEnrollmentStatus.Completed);
            }

            var enrollments = await enrollmentQuery
                .OrderBy(ce => ce.AcademicYear)
                .ThenBy(ce => ce.Semester)
                .ThenBy(ce => ce.SubjectCode)
                .ToListAsync();

            var grades = enrollments.SelectMany(ce => ce.Grades.Where(g => g.IsFinal)).ToList();

            var honors = await _context.AcademicHonors
                .Where(ah => ah.StudentEmpNr == studentEmpNr && ah.AppearsOnTranscript && ah.IsActive)
                .OrderBy(ah => ah.AcademicYear)
                .ThenBy(ah => ah.Semester)
                .ToListAsync();

            var awards = await _context.Awards
                .Where(a => a.StudentEmpNr == studentEmpNr && a.AppearsOnTranscript && a.IsActive)
                .OrderBy(a => a.AcademicYear)
                .ToListAsync();

            var cumulativeGPA = await CalculateCumulativeGPAAsync(studentEmpNr);
            var totalCreditHours = grades.Sum(g => g.CreditHours);
            var qualityPoints = grades.Sum(g => g.QualityPoints);
            var academicStanding = DetermineAcademicStanding(cumulativeGPA, totalCreditHours);

            return new TranscriptData
            {
                Student = student,
                Enrollments = enrollments,
                Grades = grades,
                Honors = honors,
                Awards = awards,
                CumulativeGPA = cumulativeGPA,
                TotalCreditHours = totalCreditHours,
                QualityPoints = qualityPoints,
                AcademicStanding = academicStanding,
                GeneratedDate = DateTime.UtcNow,
                IsOfficial = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating transcript for student {StudentEmpNr}", studentEmpNr);
            throw;
        }
    }

    public async Task<TranscriptData> GenerateUnofficialTranscriptAsync(int studentEmpNr)
    {
        var transcript = await GenerateTranscriptAsync(studentEmpNr, true);
        transcript.IsOfficial = false;
        return transcript;
    }

    public async Task<TranscriptSummary> GetTranscriptSummaryAsync(int studentEmpNr)
    {
        try
        {
            var enrollments = await _context.CourseEnrollments
                .Include(ce => ce.Grades)
                .Where(ce => ce.StudentEmpNr == studentEmpNr)
                .ToListAsync();

            var completedCourses = enrollments.Count(ce => ce.Status == CourseEnrollmentStatus.Completed);
            var inProgressCourses = enrollments.Count(ce => ce.Status == CourseEnrollmentStatus.Enrolled);
            var totalCourses = enrollments.Count;

            var grades = enrollments.SelectMany(ce => ce.Grades.Where(g => g.IsFinal)).ToList();
            var totalCreditHours = grades.Sum(g => g.CreditHours);
            var qualityPoints = grades.Sum(g => g.QualityPoints);
            var cumulativeGPA = totalCreditHours > 0 ? Math.Round(qualityPoints / totalCreditHours, 2) : 0m;
            var academicStanding = DetermineAcademicStanding(cumulativeGPA, totalCreditHours);

            var gpaHistory = await GetGPAHistoryAsync(studentEmpNr);
            var termGPAs = gpaHistory.Select(gh => (gh.AcademicYear, gh.Semester, gh.GPA));

            return new TranscriptSummary
            {
                StudentEmpNr = studentEmpNr,
                CumulativeGPA = cumulativeGPA,
                TotalCreditHours = totalCreditHours,
                QualityPoints = qualityPoints,
                AcademicStanding = academicStanding,
                TotalCourses = totalCourses,
                CompletedCourses = completedCourses,
                InProgressCourses = inProgressCourses,
                TermGPAs = termGPAs
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting transcript summary for student {StudentEmpNr}", studentEmpNr);
            throw;
        }
    }

    #endregion

    #region Academic Honors and Awards

    public async Task<AcademicHonor?> AwardAcademicHonorAsync(int studentEmpNr, HonorType honorType, string title, string? description = null, int? academicYear = null, string? semester = null, decimal? requiredGPA = null)
    {
        try
        {
            _logger.LogInformation("Awarding academic honor {Title} to student {StudentEmpNr}", title, studentEmpNr);

            var student = await _context.Students.FindAsync(studentEmpNr);
            if (student == null)
            {
                _logger.LogWarning("Student {StudentEmpNr} not found", studentEmpNr);
                return null;
            }

            var currentGPA = await CalculateCumulativeGPAAsync(studentEmpNr);
            var currentYear = academicYear ?? DateTime.Now.Year;
            var currentSemester = semester ?? GetCurrentSemester();

            var honor = new AcademicHonor
            {
                StudentEmpNr = studentEmpNr,
                HonorType = honorType,
                Title = title,
                Description = description,
                AcademicYear = currentYear,
                Semester = currentSemester,
                AwardDate = DateTime.UtcNow,
                RequiredGPA = requiredGPA,
                StudentGPA = currentGPA,
                AppearsOnTranscript = true,
                IsActive = true,
                CreatedBy = "System",
                ModifiedBy = "System"
            };

            _context.AcademicHonors.Add(honor);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully awarded academic honor {Title} to student {StudentEmpNr}", title, studentEmpNr);
            return honor;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error awarding academic honor {Title} to student {StudentEmpNr}", title, studentEmpNr);
            return null;
        }
    }

    public async Task<Award?> GiveAwardAsync(int studentEmpNr, AwardType awardType, string name, string? description = null, decimal? monetaryValue = null, string? awardingOrganization = null)
    {
        try
        {
            _logger.LogInformation("Giving award {Name} to student {StudentEmpNr}", name, studentEmpNr);

            var student = await _context.Students.FindAsync(studentEmpNr);
            if (student == null)
            {
                _logger.LogWarning("Student {StudentEmpNr} not found", studentEmpNr);
                return null;
            }

            var award = new Award
            {
                StudentEmpNr = studentEmpNr,
                AwardType = awardType,
                Name = name,
                Description = description,
                MonetaryValue = monetaryValue,
                Currency = "USD",
                AwardDate = DateTime.UtcNow,
                AcademicYear = DateTime.Now.Year,
                AwardingOrganization = awardingOrganization,
                AppearsOnTranscript = true,
                IsRecurring = false,
                IsActive = true,
                CreatedBy = "System",
                ModifiedBy = "System"
            };

            _context.Awards.Add(award);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully gave award {Name} to student {StudentEmpNr}", name, studentEmpNr);
            return award;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error giving award {Name} to student {StudentEmpNr}", name, studentEmpNr);
            return null;
        }
    }

    public async Task<IEnumerable<AcademicHonor>> GetStudentAcademicHonorsAsync(int studentEmpNr)
    {
        try
        {
            return await _context.AcademicHonors
                .Where(ah => ah.StudentEmpNr == studentEmpNr && ah.IsActive)
                .OrderByDescending(ah => ah.AcademicYear)
                .ThenByDescending(ah => ah.Semester)
                .ThenBy(ah => ah.Title)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting academic honors for student {StudentEmpNr}", studentEmpNr);
            return new List<AcademicHonor>();
        }
    }

    public async Task<IEnumerable<Award>> GetStudentAwardsAsync(int studentEmpNr)
    {
        try
        {
            return await _context.Awards
                .Where(a => a.StudentEmpNr == studentEmpNr && a.IsActive)
                .OrderByDescending(a => a.AwardDate)
                .ThenBy(a => a.Name)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting awards for student {StudentEmpNr}", studentEmpNr);
            return new List<Award>();
        }
    }

    #endregion

    #region Degree Progress Tracking

    public async Task<DegreeProgress?> UpdateDegreeProgressAsync(int studentEmpNr, string degreeCode)
    {
        try
        {
            _logger.LogInformation("Updating degree progress for student {StudentEmpNr}, degree {DegreeCode}", studentEmpNr, degreeCode);

            var student = await _context.Students.FindAsync(studentEmpNr);
            if (student == null)
            {
                _logger.LogWarning("Student {StudentEmpNr} not found", studentEmpNr);
                return null;
            }

            var degree = await _context.Degrees.FindAsync(degreeCode);
            if (degree == null)
            {
                _logger.LogWarning("Degree {DegreeCode} not found", degreeCode);
                return null;
            }

            var existingProgress = await _context.DegreeProgresses
                .FirstOrDefaultAsync(dp => dp.StudentEmpNr == studentEmpNr);

            var completedGrades = await _context.Grades
                .Include(g => g.CourseEnrollment)
                .Where(g => g.CourseEnrollment.StudentEmpNr == studentEmpNr &&
                           g.IsFinal &&
                           g.CourseEnrollment.CountsTowardDegree &&
                           !g.CourseEnrollment.IsAudit)
                .ToListAsync();

            var completedCreditHours = completedGrades.Sum(g => g.CreditHours);
            var requiredCreditHours = degree.TotalCreditHours ?? 120; // Default to 120 if not specified
            var remainingCreditHours = requiredCreditHours - completedCreditHours;
            var completionPercentage = requiredCreditHours > 0 ? Math.Round((completedCreditHours / requiredCreditHours) * 100, 2) : 0m;

            var cumulativeGPA = await CalculateCumulativeGPAAsync(studentEmpNr);
            var requiredGPA = 2.0m; // Standard requirement
            var meetsGPARequirement = cumulativeGPA >= requiredGPA;

            if (existingProgress == null)
            {
                existingProgress = new DegreeProgress
                {
                    StudentEmpNr = studentEmpNr,
                    DegreeCode = degreeCode,
                    RequiredCreditHours = requiredCreditHours,
                    RequiredGPA = requiredGPA,
                    CreatedBy = "System",
                    ModifiedBy = "System"
                };
                _context.DegreeProgresses.Add(existingProgress);
            }

            existingProgress.CompletedCreditHours = completedCreditHours;
            existingProgress.RemainingCreditHours = Math.Max(0, remainingCreditHours);
            existingProgress.CompletionPercentage = Math.Min(100, completionPercentage);
            existingProgress.CumulativeGPA = cumulativeGPA;
            existingProgress.MeetsGPARequirement = meetsGPARequirement;
            existingProgress.LastUpdated = DateTime.UtcNow;
            existingProgress.UpdatedBy = "System";
            existingProgress.ModifiedBy = "System";

            // Calculate expected graduation date (rough estimate)
            if (remainingCreditHours > 0)
            {
                var averageCreditsPerTerm = 15m; // Assume 15 credits per semester
                var remainingTerms = Math.Ceiling(remainingCreditHours / averageCreditsPerTerm);
                var monthsToAdd = (int)(remainingTerms * 4); // Assume 4 months per term
                existingProgress.ExpectedGraduationDate = DateTime.Now.AddMonths(monthsToAdd);
                existingProgress.ProjectedGraduationTerm = GetProjectedGraduationTerm(existingProgress.ExpectedGraduationDate.Value);
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully updated degree progress for student {StudentEmpNr}", studentEmpNr);
            return existingProgress;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating degree progress for student {StudentEmpNr}, degree {DegreeCode}", studentEmpNr, degreeCode);
            return null;
        }
    }

    public async Task<DegreeProgress?> GetDegreeProgressAsync(int studentEmpNr)
    {
        try
        {
            return await _context.DegreeProgresses
                .Include(dp => dp.Student)
                .Include(dp => dp.Degree)
                .FirstOrDefaultAsync(dp => dp.StudentEmpNr == studentEmpNr);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting degree progress for student {StudentEmpNr}", studentEmpNr);
            return null;
        }
    }

    public async Task<DegreeRequirements> GetRemainingRequirementsAsync(int studentEmpNr)
    {
        try
        {
            var progress = await GetDegreeProgressAsync(studentEmpNr);
            if (progress == null)
            {
                return new DegreeRequirements
                {
                    StudentEmpNr = studentEmpNr,
                    DegreeCode = string.Empty,
                    RemainingCreditHours = 0,
                    RequiredGPA = 2.0m,
                    CurrentGPA = 0m
                };
            }

            // This is a simplified implementation
            // In a real system, you would have detailed requirement tracking
            var remainingCore = new List<string>();
            var remainingElectives = new List<string>();

            if (progress.RemainingCreditHours > 0)
            {
                remainingCore.Add($"{progress.RemainingCreditHours} credit hours remaining");
            }

            return new DegreeRequirements
            {
                StudentEmpNr = studentEmpNr,
                DegreeCode = progress.DegreeCode,
                RemainingCreditHours = progress.RemainingCreditHours,
                RemainingCoreRequirements = remainingCore,
                RemainingElectiveRequirements = remainingElectives,
                RequiredGPA = progress.RequiredGPA,
                CurrentGPA = progress.CumulativeGPA
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting remaining requirements for student {StudentEmpNr}", studentEmpNr);
            throw;
        }
    }

    public async Task<GraduationEligibility> CheckGraduationEligibilityAsync(int studentEmpNr)
    {
        try
        {
            var progress = await GetDegreeProgressAsync(studentEmpNr);
            if (progress == null)
            {
                return new GraduationEligibility
                {
                    StudentEmpNr = studentEmpNr,
                    IsEligible = false,
                    UnmetRequirements = new[] { "No degree progress record found" }
                };
            }

            var unmetRequirements = new List<string>();

            if (progress.RemainingCreditHours > 0)
            {
                unmetRequirements.Add($"{progress.RemainingCreditHours} credit hours remaining");
            }

            if (!progress.MeetsGPARequirement)
            {
                unmetRequirements.Add($"GPA requirement not met (current: {progress.CumulativeGPA:F2}, required: {progress.RequiredGPA:F2})");
            }

            if (!progress.CapstoneCompleted)
            {
                unmetRequirements.Add("Capstone requirement not completed");
            }

            return new GraduationEligibility
            {
                StudentEmpNr = studentEmpNr,
                IsEligible = !unmetRequirements.Any(),
                UnmetRequirements = unmetRequirements,
                RemainingCreditHours = progress.RemainingCreditHours,
                MeetsGPARequirement = progress.MeetsGPARequirement,
                MeetsResidencyRequirement = true, // Simplified - assume always met
                ProjectedGraduationDate = progress.ExpectedGraduationDate
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking graduation eligibility for student {StudentEmpNr}", studentEmpNr);
            throw;
        }
    }

    #endregion

    #region Utility Methods

    public decimal ConvertLetterGradeToPoints(string letterGrade)
    {
        return letterGrade?.ToUpper() switch
        {
            "A+" => 4.0m,
            "A" => 4.0m,
            "A-" => 3.7m,
            "B+" => 3.3m,
            "B" => 3.0m,
            "B-" => 2.7m,
            "C+" => 2.3m,
            "C" => 2.0m,
            "C-" => 1.7m,
            "D+" => 1.3m,
            "D" => 1.0m,
            "D-" => 0.7m,
            "F" => 0.0m,
            "I" => 0.0m, // Incomplete
            "W" => 0.0m, // Withdrawal
            "P" => 0.0m, // Pass (no grade points)
            _ => 0.0m
        };
    }

    public string ConvertNumericGradeToLetter(decimal numericGrade)
    {
        return numericGrade switch
        {
            >= 97 => "A+",
            >= 93 => "A",
            >= 90 => "A-",
            >= 87 => "B+",
            >= 83 => "B",
            >= 80 => "B-",
            >= 77 => "C+",
            >= 73 => "C",
            >= 70 => "C-",
            >= 67 => "D+",
            >= 63 => "D",
            >= 60 => "D-",
            _ => "F"
        };
    }

    public AcademicStanding DetermineAcademicStanding(decimal gpa, decimal creditHours)
    {
        if (gpa >= 3.8m && creditHours >= 12)
            return AcademicStanding.DeansListqualification;
        if (gpa >= 3.5m)
            return AcademicStanding.Good;
        if (gpa >= 2.0m)
            return AcademicStanding.Good;
        if (gpa >= 1.5m)
            return AcademicStanding.Warning;
        if (gpa >= 1.0m)
            return AcademicStanding.Probation;

        return AcademicStanding.AcademicSuspension;
    }

    #endregion

    #region Private Helper Methods

    private string GetCurrentSemester()
    {
        var month = DateTime.Now.Month;
        return month switch
        {
            >= 1 and <= 5 => "Spring",
            >= 6 and <= 8 => "Summer",
            >= 9 and <= 12 => "Fall",
            _ => "Unknown"
        };
    }

    private decimal ConvertLetterGradeToNumeric(string letterGrade)
    {
        return letterGrade?.ToUpper() switch
        {
            "A+" => 97m,
            "A" => 94m,
            "A-" => 90m,
            "B+" => 87m,
            "B" => 84m,
            "B-" => 80m,
            "C+" => 77m,
            "C" => 74m,
            "C-" => 70m,
            "D+" => 67m,
            "D" => 64m,
            "D-" => 60m,
            "F" => 50m,
            _ => 0m
        };
    }

    private string GetProjectedGraduationTerm(DateTime graduationDate)
    {
        var month = graduationDate.Month;
        var year = graduationDate.Year;
        var semester = month switch
        {
            >= 1 and <= 5 => "Spring",
            >= 6 and <= 8 => "Summer",
            >= 9 and <= 12 => "Fall",
            _ => "Unknown"
        };
        return $"{semester} {year}";
    }

    #endregion
}