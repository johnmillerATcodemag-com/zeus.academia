using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Services;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Enums;
using System.ComponentModel.DataAnnotations;

namespace Zeus.Academia.Api.UnitTests.Services;

/// <summary>
/// Comprehensive tests for Course Planning and Degree Requirements (Task 5).
/// Tests degree requirement templates, course sequence planning, degree audit, graduation requirements, and course plan optimization.
/// </summary>
public class CoursePlanningAndDegreeRequirementsTests
{
    private readonly Mock<AcademiaDbContext> _mockContext;
    private readonly Mock<ILogger<DegreeRequirementService>> _mockDegreeLogger;
    private readonly Mock<ILogger<CourseSequencePlanningService>> _mockSequenceLogger;
    private readonly Mock<ILogger<DegreeAuditService>> _mockAuditLogger;
    private readonly Mock<ILogger<GraduationRequirementService>> _mockGradLogger;
    private readonly Mock<ILogger<CoursePlanOptimizationService>> _mockOptimizationLogger;

    public CoursePlanningAndDegreeRequirementsTests()
    {
        _mockContext = new Mock<AcademiaDbContext>();
        _mockDegreeLogger = new Mock<ILogger<DegreeRequirementService>>();
        _mockSequenceLogger = new Mock<ILogger<CourseSequencePlanningService>>();
        _mockAuditLogger = new Mock<ILogger<DegreeAuditService>>();
        _mockGradLogger = new Mock<ILogger<GraduationRequirementService>>();
        _mockOptimizationLogger = new Mock<ILogger<CoursePlanOptimizationService>>();
    }

    #region Degree Requirement Template Tests

    [Fact]
    public async Task DegreeRequirementTemplate_Should_Create_Complete_Template()
    {
        // Arrange
        var template = new DegreeRequirementTemplate
        {
            DegreeCode = "BS_CS",
            DegreeName = "Bachelor of Science in Computer Science",
            TotalCreditsRequired = 120,
            ResidencyCreditsRequired = 30,
            MinimumGPA = 2.0m,
            MaxTimeToComplete = 6, // years
            EffectiveDate = new DateTime(2024, 8, 1),
            Categories = new List<RequirementCategory>
            {
                new RequirementCategory
                {
                    Name = "General Education",
                    CreditsRequired = 42,
                    Description = "Core liberal arts and sciences requirements",
                    Requirements = new List<DegreeRequirement>
                    {
                        new DegreeRequirement
                        {
                            Type = RequirementType.SpecificCourse,
                            Description = "English Composition I",
                            CourseIds = new List<int> { 101 },
                            CreditsRequired = 3,
                            IsRequired = true
                        },
                        new DegreeRequirement
                        {
                            Type = RequirementType.CourseGroup,
                            Description = "Mathematics",
                            SubjectCodes = new List<string> { "MATH" },
                            MinimumCourseLevel = 200,
                            CreditsRequired = 12,
                            IsRequired = true
                        }
                    }
                },
                new RequirementCategory
                {
                    Name = "Major Requirements",
                    CreditsRequired = 54,
                    Description = "Computer Science core courses",
                    Requirements = new List<DegreeRequirement>
                    {
                        new DegreeRequirement
                        {
                            Type = RequirementType.SpecificCourse,
                            Description = "Programming Fundamentals",
                            CourseIds = new List<int> { 201, 202 },
                            CreditsRequired = 6,
                            IsRequired = true
                        }
                    }
                }
            }
        };

        var service = new DegreeRequirementService(_mockContext.Object, _mockDegreeLogger.Object);

        // Act
        var result = await service.CreateDegreeTemplateAsync(template);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("BS_CS", result.DegreeCode);
        Assert.Equal(120, result.TotalCreditsRequired);
        Assert.Equal(2, result.Categories.Count);
        Assert.All(result.Categories, category =>
        {
            Assert.True(category.CreditsRequired > 0);
            Assert.True(category.Requirements.Count > 0);
        });
    }

    [Fact]
    public async Task DegreeRequirementTemplate_Should_Support_Complex_Requirements()
    {
        // Arrange
        var complexRequirement = new DegreeRequirement
        {
            Type = RequirementType.ConditionalGroup,
            Description = "Advanced CS Electives: Choose 3 from 400-level CS courses OR 2 from 500-level",
            LogicType = RequirementLogicType.Either,
            ConditionalRequirements = new List<ConditionalRequirement>
            {
                new ConditionalRequirement
                {
                    Condition = "Choose 3 courses",
                    SubjectCodes = new List<string> { "CS" },
                    MinimumCourseLevel = 400,
                    MaximumCourseLevel = 499,
                    CoursesRequired = 3,
                    CreditsRequired = 9
                },
                new ConditionalRequirement
                {
                    Condition = "Choose 2 courses",
                    SubjectCodes = new List<string> { "CS" },
                    MinimumCourseLevel = 500,
                    MaximumCourseLevel = 599,
                    CoursesRequired = 2,
                    CreditsRequired = 6
                }
            }
        };

        var service = new DegreeRequirementService(_mockContext.Object, _mockDegreeLogger.Object);

        // Act
        var isValid = await service.ValidateRequirementAsync(complexRequirement);

        // Assert
        Assert.True(isValid);
        Assert.Equal(RequirementType.ConditionalGroup, complexRequirement.Type);
        Assert.Equal(RequirementLogicType.Either, complexRequirement.LogicType);
        Assert.Equal(2, complexRequirement.ConditionalRequirements.Count);
    }

    [Fact]
    public async Task DegreeRequirementTemplate_Should_Handle_Prerequisites_In_Requirements()
    {
        // Arrange
        var requirementWithPrereqs = new DegreeRequirement
        {
            Type = RequirementType.SequencedCourses,
            Description = "Calculus Sequence",
            CourseIds = new List<int> { 301, 302, 303 }, // Calc I, II, III
            CreditsRequired = 12,
            IsRequired = true,
            SequenceRequired = true,
            PrerequisiteChain = new List<PrerequisiteLink>
            {
                new PrerequisiteLink { CourseId = 301, PrerequisiteCourseId = null }, // Calc I (no prereq)
                new PrerequisiteLink { CourseId = 302, PrerequisiteCourseId = 301 }, // Calc II needs Calc I
                new PrerequisiteLink { CourseId = 303, PrerequisiteCourseId = 302 }  // Calc III needs Calc II
            }
        };

        var service = new DegreeRequirementService(_mockContext.Object, _mockDegreeLogger.Object);

        // Act
        var sequenceValidation = await service.ValidateSequenceAsync(requirementWithPrereqs);

        // Assert
        Assert.True(sequenceValidation.IsValid);
        Assert.False(sequenceValidation.HasCircularDependency);
        Assert.Equal(3, sequenceValidation.SequenceLength);
        Assert.NotNull(sequenceValidation.RecommendedSemesterMapping);
    }

    #endregion

    #region Course Sequence Planning Tests

    [Fact]
    public async Task CourseSequencePlanning_Should_Create_Optimal_Semester_Plan()
    {
        // Arrange
        var studentProfile = new StudentAcademicProfile
        {
            StudentId = 12345,
            DegreeCode = "BS_CS",
            CurrentSemester = "Fall 2024",
            CompletedCourses = new List<int> { 101, 102, 201 },
            CurrentGPA = 3.25m,
            ExpectedGraduationDate = new DateTime(2026, 5, 15),
            CourseLoadPreference = CourseLoadPreference.Standard, // 15-16 credits per semester
            SummerAvailability = true
        };

        var degreeTemplate = CreateSampleDegreeTemplate();
        var service = new CourseSequencePlanningService(_mockContext.Object, _mockSequenceLogger.Object);

        // Act
        var sequencePlan = await service.CreateSequencePlanAsync(studentProfile, degreeTemplate);

        // Assert
        Assert.NotNull(sequencePlan);
        Assert.True(sequencePlan.TotalSemesters > 0);
        Assert.True(sequencePlan.TotalCredits >= degreeTemplate.TotalCreditsRequired);
        Assert.All(sequencePlan.SemesterPlans, semester =>
        {
            Assert.True(semester.TotalCredits >= 12 && semester.TotalCredits <= 18);
            Assert.True(semester.Courses.All(c => c.PrerequisitesSatisfied));
        });
        Assert.Equal(studentProfile.ExpectedGraduationDate.Value.ToString("yyyy-MM"),
                    sequencePlan.SemesterPlans.Last().Semester.ToString("yyyy-MM"));
    }

    [Fact]
    public async Task CourseSequencePlanning_Should_Handle_Complex_Prerequisites()
    {
        // Arrange
        var courses = new List<Course>
        {
            new Course { Id = 201, CourseNumber = "CS201", Title = "Programming I", Prerequisites = new List<Prerequisite>() },
            new Course { Id = 202, CourseNumber = "CS202", Title = "Programming II", Prerequisites = new List<Prerequisite>
                { new Prerequisite { RequiredCourseId = 201 } }},
            new Course { Id = 301, CourseNumber = "CS301", Title = "Data Structures", Prerequisites = new List<Prerequisite>
                { new Prerequisite { RequiredCourseId = 202 } }},
            new Course { Id = 302, CourseNumber = "CS302", Title = "Algorithms", Prerequisites = new List<Prerequisite>
                { new Prerequisite { RequiredCourseId = 301 } }},
            new Course { Id = 401, CourseNumber = "CS401", Title = "Software Engineering", Prerequisites = new List<Prerequisite>
                {
                    new Prerequisite { RequiredCourseId = 301, LogicType = PrerequisiteLogicType.And },
                    new Prerequisite { RequiredCourseId = 302, LogicType = PrerequisiteLogicType.And }
                }}
        };

        var service = new CourseSequencePlanningService(_mockContext.Object, _mockSequenceLogger.Object);

        // Act
        var prerequisiteChain = await service.BuildPrerequisiteChainAsync(courses);

        // Assert
        Assert.NotNull(prerequisiteChain);
        Assert.False(prerequisiteChain.HasCircularDependency);
        Assert.Equal(5, prerequisiteChain.Levels.Count);

        // Verify sequence levels
        Assert.Contains(201, prerequisiteChain.Levels[0]); // No prerequisites
        Assert.Contains(202, prerequisiteChain.Levels[1]); // Needs CS201
        Assert.Contains(301, prerequisiteChain.Levels[2]); // Needs CS202
        Assert.Contains(302, prerequisiteChain.Levels[3]); // Needs CS301
        Assert.Contains(401, prerequisiteChain.Levels[4]); // Needs CS301 AND CS302
    }

    [Fact]
    public async Task CourseSequencePlanning_Should_Optimize_For_Course_Availability()
    {
        // Arrange
        var courseOfferings = new List<CourseOffering>
        {
            new CourseOffering { CourseId = 201, Term = "Fall", IsRegularlyOffered = true },
            new CourseOffering { CourseId = 201, Term = "Spring", IsRegularlyOffered = true },
            new CourseOffering { CourseId = 302, Term = "Fall", IsRegularlyOffered = true },
            new CourseOffering { CourseId = 302, Term = "Spring", IsRegularlyOffered = false }, // Only offered in Fall
            new CourseOffering { CourseId = 401, Term = "Spring", IsRegularlyOffered = true }
        };

        var studentProfile = new StudentAcademicProfile
        {
            StudentId = 12345,
            StartingSemester = "Fall 2024"
        };

        var service = new CourseSequencePlanningService(_mockContext.Object, _mockSequenceLogger.Object);

        // Act
        var optimizedPlan = await service.OptimizeForAvailabilityAsync(studentProfile, courseOfferings);

        // Assert
        Assert.NotNull(optimizedPlan);

        // Verify that courses are scheduled in semesters when they're offered
        var fallPlan = optimizedPlan.SemesterPlans.FirstOrDefault(p => p.Semester.ToString().Contains("Fall"));
        var springPlan = optimizedPlan.SemesterPlans.FirstOrDefault(p => p.Semester.ToString().Contains("Spring"));

        if (fallPlan != null)
        {
            Assert.Contains(fallPlan.Courses, c => c.CourseId == 302); // CS302 should be in Fall
        }

        if (springPlan != null)
        {
            Assert.DoesNotContain(springPlan.Courses, c => c.CourseId == 302); // CS302 should NOT be in Spring
        }
    }

    #endregion

    #region Degree Audit Tests

    [Fact]
    public async Task DegreeAudit_Should_Calculate_Accurate_Progress()
    {
        // Arrange
        var studentRecord = new StudentAcademicRecord
        {
            StudentId = 12345,
            DegreeCode = "BS_CS",
            CompletedCourses = new List<CompletedCourse>
            {
                new CompletedCourse { CourseId = 101, Grade = "A", CreditHours = 3, Semester = "Fall 2023" },
                new CompletedCourse { CourseId = 102, Grade = "B+", CreditHours = 3, Semester = "Spring 2024" },
                new CompletedCourse { CourseId = 201, Grade = "A-", CreditHours = 4, Semester = "Fall 2024" },
                new CompletedCourse { CourseId = 301, Grade = "B", CreditHours = 3, Semester = "Spring 2024" }
            },
            TransferCredits = new List<TransferCredit>
            {
                new TransferCredit { CourseEquivalentId = 105, CreditHours = 3, Grade = "B" }
            }
        };

        var degreeTemplate = CreateSampleDegreeTemplate();
        var service = new DegreeAuditService(_mockContext.Object, _mockAuditLogger.Object);

        // Act
        var auditResult = await service.PerformDegreeAuditAsync(studentRecord, degreeTemplate);

        // Assert
        Assert.NotNull(auditResult);
        Assert.Equal(16, auditResult.TotalCreditsCompleted); // 13 + 3 transfer
        Assert.Equal(120, auditResult.TotalCreditsRequired);
        Assert.Equal(104, auditResult.RemainingCreditsNeeded);
        Assert.True(auditResult.CompletionPercentage > 0);
        Assert.True(auditResult.CompletionPercentage < 100);

        // Verify category-specific progress
        Assert.NotNull(auditResult.CategoryProgress);
        Assert.All(auditResult.CategoryProgress, progress =>
        {
            Assert.True(progress.CreditsCompleted >= 0);
            Assert.True(progress.CreditsRequired > 0);
            Assert.True(progress.CompletionPercentage >= 0 && progress.CompletionPercentage <= 100);
        });
    }

    [Fact]
    public async Task DegreeAudit_Should_Handle_Course_Substitutions()
    {
        // Arrange
        var studentRecord = new StudentAcademicRecord
        {
            StudentId = 12345,
            CompletedCourses = new List<CompletedCourse>
            {
                new CompletedCourse { CourseId = 299, Grade = "A", CreditHours = 3 } // Non-standard course
            },
            ApprovedSubstitutions = new List<CourseSubstitution>
            {
                new CourseSubstitution
                {
                    OriginalCourseId = 301, // Required course
                    SubstituteCourseId = 299, // What student actually took
                    ApprovedBy = "Department Chair",
                    ApprovalDate = DateTime.Now.AddDays(-30),
                    Reason = "Equivalent content confirmed"
                }
            }
        };

        var service = new DegreeAuditService(_mockContext.Object, _mockAuditLogger.Object);

        // Act
        var auditResult = await service.PerformDegreeAuditAsync(studentRecord, CreateSampleDegreeTemplate());

        // Assert
        Assert.NotNull(auditResult);
        Assert.Contains(auditResult.SatisfiedRequirements, req =>
            req.RequirementDescription.Contains("301") && req.SatisfiedBy.Contains("299"));
        Assert.Equal(1, auditResult.ApprovedSubstitutions.Count);
    }

    [Fact]
    public async Task DegreeAudit_Should_Identify_Outstanding_Requirements()
    {
        // Arrange
        var studentRecord = new StudentAcademicRecord
        {
            StudentId = 12345,
            DegreeCode = "BS_CS",
            CompletedCourses = new List<CompletedCourse>
            {
                new CompletedCourse { CourseId = 101, Grade = "A", CreditHours = 3 },
                new CompletedCourse { CourseId = 201, Grade = "B", CreditHours = 4 }
            }
        };

        var service = new DegreeAuditService(_mockContext.Object, _mockAuditLogger.Object);

        // Act
        var auditResult = await service.PerformDegreeAuditAsync(studentRecord, CreateSampleDegreeTemplate());

        // Assert
        Assert.NotNull(auditResult);
        Assert.NotNull(auditResult.OutstandingRequirements);
        Assert.True(auditResult.OutstandingRequirements.Count > 0);

        Assert.All(auditResult.OutstandingRequirements, requirement =>
        {
            Assert.NotNull(requirement.RequirementDescription);
            Assert.True(requirement.CreditsNeeded > 0);
            Assert.NotNull(requirement.SuggestedCourses);
        });
    }

    #endregion

    #region Graduation Requirements Tests

    [Fact]
    public async Task GraduationRequirements_Should_Validate_All_Criteria()
    {
        // Arrange
        var graduationCriteria = new GraduationRequirements
        {
            MinimumCredits = 120,
            MinimumGPA = 2.0m,
            ResidencyCredits = 30,
            MaxTimeLimit = 6, // years
            RequiredCourseCompletion = 100, // percentage
            MinimumGradeInMajor = "C",
            MaxFailedCourseRetakes = 2,
            MustCompleteCapstone = true,
            RequiredGraduationApplication = true,
            ApplicationDeadlineMonths = 2 // before graduation
        };

        var studentRecord = new StudentAcademicRecord
        {
            StudentId = 12345,
            TotalCredits = 125,
            CumulativeGPA = 3.15m,
            ResidencyCredits = 35,
            StartDate = DateTime.Now.AddYears(-4), // 4 years ago
            CompletedRequiredCourses = 95, // percentage
            LowestMajorGrade = "C+",
            FailedCourseRetakes = 1,
            CapstoneCompleted = true,
            GraduationApplicationSubmitted = true,
            ApplicationSubmissionDate = DateTime.Now.AddMonths(-3)
        };

        var service = new GraduationRequirementService(_mockContext.Object, _mockGradLogger.Object);

        // Act
        var validationResult = await service.ValidateGraduationEligibilityAsync(studentRecord, graduationCriteria);

        // Assert
        Assert.NotNull(validationResult);
        Assert.True(validationResult.IsEligible);
        Assert.True(validationResult.CreditRequirementMet);
        Assert.True(validationResult.GPARequirementMet);
        Assert.True(validationResult.ResidencyRequirementMet);
        Assert.True(validationResult.TimeLimitMet);
        Assert.False(validationResult.CourseCompletionRequirementMet); // 95% < 100%
        Assert.True(validationResult.GradeRequirementMet);
        Assert.True(validationResult.CapstoneRequirementMet);
        Assert.True(validationResult.ApplicationRequirementMet);

        // Should have warnings but still be eligible
        Assert.NotNull(validationResult.Warnings);
        Assert.Contains("Course completion", string.Join(" ", validationResult.Warnings));
    }

    [Fact]
    public async Task GraduationRequirements_Should_Block_Ineligible_Students()
    {
        // Arrange
        var graduationCriteria = new GraduationRequirements
        {
            MinimumCredits = 120,
            MinimumGPA = 2.0m,
            ResidencyCredits = 30
        };

        var ineligibleStudent = new StudentAcademicRecord
        {
            StudentId = 12345,
            TotalCredits = 110, // Below minimum
            CumulativeGPA = 1.85m, // Below minimum
            ResidencyCredits = 25 // Below minimum
        };

        var service = new GraduationRequirementService(_mockContext.Object, _mockGradLogger.Object);

        // Act
        var validationResult = await service.ValidateGraduationEligibilityAsync(ineligibleStudent, graduationCriteria);

        // Assert
        Assert.NotNull(validationResult);
        Assert.False(validationResult.IsEligible);
        Assert.False(validationResult.CreditRequirementMet);
        Assert.False(validationResult.GPARequirementMet);
        Assert.False(validationResult.ResidencyRequirementMet);

        Assert.NotNull(validationResult.BlockingIssues);
        Assert.Contains("credits", string.Join(" ", validationResult.BlockingIssues).ToLower());
        Assert.Contains("gpa", string.Join(" ", validationResult.BlockingIssues).ToLower());
        Assert.Contains("residency", string.Join(" ", validationResult.BlockingIssues).ToLower());
    }

    [Fact]
    public async Task GraduationRequirements_Should_Calculate_Time_To_Completion()
    {
        // Arrange
        var studentRecord = new StudentAcademicRecord
        {
            StudentId = 12345,
            TotalCredits = 90,
            CurrentSemesterLoad = 15,
            SummerCoursesTaken = true
        };

        var service = new GraduationRequirementService(_mockContext.Object, _mockGradLogger.Object);

        // Act
        var timeToCompletion = await service.EstimateTimeToGraduationAsync(studentRecord, 120);

        // Assert
        Assert.NotNull(timeToCompletion);
        Assert.True(timeToCompletion.SemestersRemaining > 0);
        Assert.True(timeToCompletion.SemestersRemaining <= 4); // 30 credits / 15 per semester = 2 semesters
        Assert.NotNull(timeToCompletion.EstimatedGraduationDate);
        Assert.True(timeToCompletion.EstimatedGraduationDate > DateTime.Now);
    }

    #endregion

    #region Course Plan Optimization Tests

    [Fact]
    public async Task CoursePlanOptimization_Should_Minimize_Time_To_Graduation()
    {
        // Arrange
        var studentProfile = new StudentAcademicProfile
        {
            StudentId = 12345,
            CompletedCourses = new List<int> { 101, 102, 201 },
            PreferredCourseLoad = 15,
            SummerAvailability = true,
            WorkScheduleConstraints = new List<string> { "No classes before 10 AM" }
        };

        var remainingRequirements = new List<DegreeRequirement>
        {
            new DegreeRequirement { CourseIds = new List<int> { 301, 302, 303 }, CreditsRequired = 12 },
            new DegreeRequirement { CourseIds = new List<int> { 401, 402 }, CreditsRequired = 8 },
            new DegreeRequirement { CourseIds = new List<int> { 450 }, CreditsRequired = 3 } // Capstone
        };

        var service = new CoursePlanOptimizationService(_mockContext.Object, _mockOptimizationLogger.Object);

        // Act
        var optimizedPlan = await service.OptimizeForMinimumTimeAsync(studentProfile, remainingRequirements);

        // Assert
        Assert.NotNull(optimizedPlan);
        Assert.True(optimizedPlan.TotalSemesters > 0);
        Assert.True(optimizedPlan.IncludesSummerSessions); // Should use summer to minimize time

        // Verify prerequisites are respected
        Assert.All(optimizedPlan.SemesterPlans, semester =>
        {
            Assert.All(semester.Courses, course =>
                Assert.True(course.PrerequisitesSatisfied));
        });

        // Verify course load constraints
        Assert.All(optimizedPlan.SemesterPlans, semester =>
        {
            Assert.True(semester.TotalCredits <= studentProfile.PreferredCourseLoad + 3); // Allow some flexibility
        });
    }

    [Fact]
    public async Task CoursePlanOptimization_Should_Consider_Course_Difficulty()
    {
        // Arrange
        var courses = new List<Course>
        {
            new Course { Id = 301, DifficultyRating = DifficultyLevel.High, CreditHours = 4 },
            new Course { Id = 302, DifficultyRating = DifficultyLevel.Medium, CreditHours = 3 },
            new Course { Id = 303, DifficultyRating = DifficultyLevel.Low, CreditHours = 3 },
            new Course { Id = 401, DifficultyRating = DifficultyLevel.High, CreditHours = 4 }
        };

        var studentProfile = new StudentAcademicProfile
        {
            StudentId = 12345,
            PreferredCourseLoad = 15,
            MaxDifficultCoursesPerSemester = 2
        };

        var service = new CoursePlanOptimizationService(_mockContext.Object, _mockOptimizationLogger.Object);

        // Act
        var balancedPlan = await service.OptimizeForDifficultyBalanceAsync(studentProfile, courses);

        // Assert
        Assert.NotNull(balancedPlan);

        // Verify no semester has too many difficult courses
        Assert.All(balancedPlan.SemesterPlans, semester =>
        {
            var difficultCourses = semester.Courses.Count(c => c.DifficultyRating == DifficultyLevel.High);
            Assert.True(difficultCourses <= studentProfile.MaxDifficultCoursesPerSemester);
        });
    }

    [Fact]
    public async Task CoursePlanOptimization_Should_Handle_Multiple_Optimization_Criteria()
    {
        // Arrange
        var optimizationCriteria = new OptimizationCriteria
        {
            PriorityWeights = new Dictionary<OptimizationPriority, float>
            {
                { OptimizationPriority.MinimizeTime, 0.4f },
                { OptimizationPriority.BalanceDifficulty, 0.3f },
                { OptimizationPriority.MaximizeScheduleFlexibility, 0.2f },
                { OptimizationPriority.MinimizeCost, 0.1f }
            },
            Constraints = new List<OptimizationConstraint>
            {
                new OptimizationConstraint { Type = "MaxCoursesPerSemester", Value = "5" },
                new OptimizationConstraint { Type = "NoFridayClasses", Value = "true" },
                new OptimizationConstraint { Type = "PreferOnlineCourses", Value = "true" }
            }
        };

        var studentProfile = new StudentAcademicProfile { StudentId = 12345 };
        var service = new CoursePlanOptimizationService(_mockContext.Object, _mockOptimizationLogger.Object);

        // Act
        var multiCriteriaOptimization = await service.OptimizeWithMultipleCriteriaAsync(
            studentProfile, new List<DegreeRequirement>(), optimizationCriteria);

        // Assert
        Assert.NotNull(multiCriteriaOptimization);
        Assert.True(multiCriteriaOptimization.OptimizationScore > 0);
        Assert.NotNull(multiCriteriaOptimization.AlternativePlans);

        // Verify constraints are respected
        Assert.All(multiCriteriaOptimization.OptimalPlan.SemesterPlans, semester =>
        {
            Assert.True(semester.Courses.Count <= 5); // MaxCoursesPerSemester constraint
        });
    }

    #endregion

    #region Integration Tests

    [Fact]
    public async Task FullDegreePlanningWorkflow_Should_Work_End_To_End()
    {
        // Arrange - Complete student starting their degree
        var newStudent = new StudentAcademicProfile
        {
            StudentId = 12345,
            DegreeCode = "BS_CS",
            StartingSemester = "Fall 2024",
            ExpectedGraduationDate = new DateTime(2028, 5, 15),
            CompletedCourses = new List<int>(), // New student
            PreferredCourseLoad = 15,
            SummerAvailability = true
        };

        var degreeTemplate = CreateSampleDegreeTemplate();

        // Services
        var sequencePlanningService = new CourseSequencePlanningService(_mockContext.Object, _mockSequenceLogger.Object);
        var auditService = new DegreeAuditService(_mockContext.Object, _mockAuditLogger.Object);
        var optimizationService = new CoursePlanOptimizationService(_mockContext.Object, _mockOptimizationLogger.Object);
        var graduationService = new GraduationRequirementService(_mockContext.Object, _mockGradLogger.Object);

        // Act - Complete workflow

        // Step 1: Create initial degree plan
        var initialPlan = await sequencePlanningService.CreateSequencePlanAsync(newStudent, degreeTemplate);

        // Step 2: Optimize the plan
        var optimizedPlan = await optimizationService.OptimizeForMinimumTimeAsync(
            newStudent, degreeTemplate.GetAllRequirements());

        // Step 3: Perform initial audit (should show 0% completion)
        var initialRecord = new StudentAcademicRecord { StudentId = newStudent.StudentId, CompletedCourses = new List<CompletedCourse>() };
        var initialAudit = await auditService.PerformDegreeAuditAsync(initialRecord, degreeTemplate);

        // Step 4: Simulate progress and re-audit
        var progressRecord = new StudentAcademicRecord
        {
            StudentId = newStudent.StudentId,
            CompletedCourses = new List<CompletedCourse>
            {
                new CompletedCourse { CourseId = 101, Grade = "A", CreditHours = 3 },
                new CompletedCourse { CourseId = 201, Grade = "B+", CreditHours = 4 }
            }
        };
        var progressAudit = await auditService.PerformDegreeAuditAsync(progressRecord, degreeTemplate);

        // Act - Graduation eligibility check
        var graduationCheck = await graduationService.ValidateGraduationEligibilityAsync(
            progressRecord, new GraduationRequirements { MinimumCredits = 120, MinimumGPA = 2.0m });

        // Assert - Verify complete workflow

        // Initial plan assertions
        Assert.NotNull(initialPlan);
        Assert.True(initialPlan.TotalSemesters <= 8); // Should complete in 4 years
        Assert.Equal(degreeTemplate.TotalCreditsRequired, initialPlan.TotalCredits);

        // Optimization assertions
        Assert.NotNull(optimizedPlan);
        Assert.True(optimizedPlan.TotalSemesters <= initialPlan.TotalSemesters); // Should be same or better

        // Initial audit assertions
        Assert.NotNull(initialAudit);
        Assert.Equal(0, initialAudit.CompletionPercentage);
        Assert.Equal(degreeTemplate.TotalCreditsRequired, initialAudit.RemainingCreditsNeeded);

        // Progress audit assertions
        Assert.NotNull(progressAudit);
        Assert.True(progressAudit.CompletionPercentage > 0);
        Assert.True(progressAudit.CompletionPercentage < 100);
        Assert.Equal(7, progressAudit.TotalCreditsCompleted);

        // Graduation check assertions
        Assert.NotNull(graduationCheck);
        Assert.False(graduationCheck.IsEligible); // Student just started
        Assert.Contains("credits", string.Join(" ", graduationCheck.BlockingIssues).ToLower());

        // Verify plan consistency
        Assert.True(initialPlan.SemesterPlans.SelectMany(s => s.Courses).All(c => c.PrerequisitesSatisfied));
        Assert.True(optimizedPlan.SemesterPlans.SelectMany(s => s.Courses).All(c => c.PrerequisitesSatisfied));
    }

    #endregion

    #region Helper Methods

    private DegreeRequirementTemplate CreateSampleDegreeTemplate()
    {
        return new DegreeRequirementTemplate
        {
            DegreeCode = "BS_CS",
            DegreeName = "Bachelor of Science in Computer Science",
            TotalCreditsRequired = 120,
            ResidencyCreditsRequired = 30,
            MinimumGPA = 2.0m,
            Categories = new List<RequirementCategory>
            {
                new RequirementCategory
                {
                    Name = "General Education",
                    CreditsRequired = 42,
                    Requirements = new List<DegreeRequirement>
                    {
                        new DegreeRequirement
                        {
                            Type = RequirementType.SpecificCourse,
                            CourseIds = new List<int> { 101, 102 },
                            CreditsRequired = 6,
                            IsRequired = true
                        }
                    }
                },
                new RequirementCategory
                {
                    Name = "Major Requirements",
                    CreditsRequired = 54,
                    Requirements = new List<DegreeRequirement>
                    {
                        new DegreeRequirement
                        {
                            Type = RequirementType.SpecificCourse,
                            CourseIds = new List<int> { 201, 301, 401 },
                            CreditsRequired = 12,
                            IsRequired = true
                        }
                    }
                }
            }
        };
    }

    #endregion
}

#region Supporting Classes and Enums for Tests

public enum RequirementType
{
    SpecificCourse,
    CourseGroup,
    ConditionalGroup,
    SequencedCourses,
    CreditHours
}

public enum RequirementLogicType
{
    And,
    Or,
    Either,
    All,
    MinimumOf
}

public enum PrerequisiteLogicType
{
    And,
    Or
}

public enum CourseLoadPreference
{
    Light,      // 12-13 credits
    Standard,   // 15-16 credits
    Heavy,      // 17-18 credits
    Maximum     // 19+ credits
}

public enum DifficultyLevel
{
    Low,
    Medium,
    High,
    Extreme
}

public enum OptimizationPriority
{
    MinimizeTime,
    BalanceDifficulty,
    MaximizeScheduleFlexibility,
    MinimizeCost,
    MaximizeGPA
}

// Supporting classes would be defined in the main implementation
public class DegreeRequirementTemplate
{
    public string DegreeCode { get; set; } = string.Empty;
    public string DegreeName { get; set; } = string.Empty;
    public int TotalCreditsRequired { get; set; }
    public int ResidencyCreditsRequired { get; set; }
    public decimal MinimumGPA { get; set; }
    public int MaxTimeToComplete { get; set; }
    public DateTime EffectiveDate { get; set; }
    public List<RequirementCategory> Categories { get; set; } = new();

    public List<DegreeRequirement> GetAllRequirements()
    {
        return Categories.SelectMany(c => c.Requirements).ToList();
    }
}

public class RequirementCategory
{
    public string Name { get; set; } = string.Empty;
    public int CreditsRequired { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<DegreeRequirement> Requirements { get; set; } = new();
}

public class DegreeRequirement
{
    public RequirementType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<int> CourseIds { get; set; } = new();
    public List<string> SubjectCodes { get; set; } = new();
    public int MinimumCourseLevel { get; set; }
    public int MaximumCourseLevel { get; set; }
    public int CreditsRequired { get; set; }
    public int CoursesRequired { get; set; }
    public bool IsRequired { get; set; }
    public bool SequenceRequired { get; set; }
    public RequirementLogicType LogicType { get; set; }
    public List<ConditionalRequirement> ConditionalRequirements { get; set; } = new();
    public List<PrerequisiteLink> PrerequisiteChain { get; set; } = new();
}

public class ConditionalRequirement
{
    public string Condition { get; set; } = string.Empty;
    public List<string> SubjectCodes { get; set; } = new();
    public int MinimumCourseLevel { get; set; }
    public int MaximumCourseLevel { get; set; }
    public int CoursesRequired { get; set; }
    public int CreditsRequired { get; set; }
}

public class PrerequisiteLink
{
    public int CourseId { get; set; }
    public int? PrerequisiteCourseId { get; set; }
}

public class StudentAcademicProfile
{
    public int StudentId { get; set; }
    public string DegreeCode { get; set; } = string.Empty;
    public string CurrentSemester { get; set; } = string.Empty;
    public string StartingSemester { get; set; } = string.Empty;
    public List<int> CompletedCourses { get; set; } = new();
    public decimal CurrentGPA { get; set; }
    public DateTime? ExpectedGraduationDate { get; set; }
    public CourseLoadPreference CourseLoadPreference { get; set; }
    public int PreferredCourseLoad { get; set; }
    public bool SummerAvailability { get; set; }
    public List<string> WorkScheduleConstraints { get; set; } = new();
    public int MaxDifficultCoursesPerSemester { get; set; }
    public int CurrentSemesterLoad { get; set; }
    public bool SummerCoursesTaken { get; set; }
}

public class StudentAcademicRecord
{
    public int StudentId { get; set; }
    public string DegreeCode { get; set; } = string.Empty;
    public List<CompletedCourse> CompletedCourses { get; set; } = new();
    public List<TransferCredit> TransferCredits { get; set; } = new();
    public List<CourseSubstitution> ApprovedSubstitutions { get; set; } = new();
    public int TotalCredits { get; set; }
    public decimal CumulativeGPA { get; set; }
    public int ResidencyCredits { get; set; }
    public DateTime StartDate { get; set; }
    public int CompletedRequiredCourses { get; set; }
    public string LowestMajorGrade { get; set; } = string.Empty;
    public int FailedCourseRetakes { get; set; }
    public bool CapstoneCompleted { get; set; }
    public bool GraduationApplicationSubmitted { get; set; }
    public DateTime? ApplicationSubmissionDate { get; set; }
}

// Additional supporting classes would be defined similarly...

#endregion