using System.ComponentModel.DataAnnotations;
using Xunit;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Enums;

namespace Zeus.Academia.Api.UnitTests.Entities;

/// <summary>
/// Unit tests for Prerequisite and Restriction Management entities for Task 3: Prerequisite and Restriction Management.
/// Tests complex prerequisite chains with AND/OR logic, corequisite requirements, enrollment restrictions, 
/// prerequisite validation service, and override system.
/// </summary>
public class PrerequisiteRestrictionManagementTests
{
    #region Prerequisite Management with AND/OR Logic Tests

    [Fact]
    public void PrerequisiteRule_Should_Support_Complex_AND_Logic()
    {
        // Arrange & Act
        var prerequisiteRule = new PrerequisiteRule
        {
            Id = 1,
            CourseId = 301, // CS 301: Data Structures
            RuleName = "CS 301 Prerequisites",
            LogicOperator = PrerequisiteLogicOperator.AND,
            IsActive = true,
            Requirements = new List<PrerequisiteRequirement>
            {
                new() {
                    RequirementType = PrerequisiteType.Course,
                    RequiredCourseId = 101, // CS 101: Intro to Programming
                    MinimumGrade = "C",
                    IsRequired = true
                },
                new() {
                    RequirementType = PrerequisiteType.Course,
                    RequiredCourseId = 201, // CS 201: Object-Oriented Programming
                    MinimumGrade = "C",
                    IsRequired = true
                },
                new() {
                    RequirementType = PrerequisiteType.Course,
                    RequiredCourseId = 210, // MATH 210: Discrete Mathematics
                    MinimumGrade = "C",
                    IsRequired = true
                }
            }
        };

        // Assert
        Assert.Equal(PrerequisiteLogicOperator.AND, prerequisiteRule.LogicOperator);
        Assert.Equal(3, prerequisiteRule.Requirements.Count);
        Assert.True(prerequisiteRule.Requirements.All(r => r.IsRequired));
        Assert.True(prerequisiteRule.Requirements.All(r => r.MinimumGrade == "C"));
    }

    [Fact]
    public void PrerequisiteRule_Should_Support_Complex_OR_Logic()
    {
        // Arrange & Act
        var prerequisiteRule = new PrerequisiteRule
        {
            Id = 2,
            CourseId = 450, // CS 450: Advanced Topics
            RuleName = "CS 450 Flexible Prerequisites",
            LogicOperator = PrerequisiteLogicOperator.OR,
            IsActive = true,
            Requirements = new List<PrerequisiteRequirement>
            {
                new() {
                    RequirementType = PrerequisiteType.Course,
                    RequiredCourseId = 301, // CS 301: Data Structures
                    MinimumGrade = "B",
                    IsRequired = false
                },
                new() {
                    RequirementType = PrerequisiteType.Course,
                    RequiredCourseId = 310, // CS 310: Algorithms
                    MinimumGrade = "B",
                    IsRequired = false
                },
                new() {
                    RequirementType = PrerequisiteType.PermissionRequired,
                    RequiredPermission = "INSTRUCTOR_PERMISSION",
                    IsRequired = false
                }
            }
        };

        // Assert
        Assert.Equal(PrerequisiteLogicOperator.OR, prerequisiteRule.LogicOperator);
        Assert.Equal(3, prerequisiteRule.Requirements.Count);
        Assert.True(prerequisiteRule.Requirements.All(r => !r.IsRequired)); // OR logic means individual items not required
        Assert.Contains(prerequisiteRule.Requirements, r => r.RequirementType == PrerequisiteType.PermissionRequired);
    }

    [Fact]
    public void PrerequisiteRule_Should_Support_Nested_Complex_Logic()
    {
        // Arrange & Act
        var complexRule = new PrerequisiteRule
        {
            Id = 3,
            CourseId = 485, // CS 485: Senior Capstone
            RuleName = "CS 485 Complex Prerequisites",
            LogicOperator = PrerequisiteLogicOperator.AND,
            IsActive = true,
            Requirements = new List<PrerequisiteRequirement>
            {
                // Core requirement
                new() {
                    RequirementType = PrerequisiteType.CreditHours,
                    MinimumCreditHours = 90,
                    IsRequired = true
                },
                // Standing requirement
                new() {
                    RequirementType = PrerequisiteType.ClassStanding,
                    RequiredClassStanding = ClassStanding.Senior,
                    IsRequired = true
                }
            },
            NestedRules = new List<PrerequisiteRule>
            {
                // Nested OR rule for advanced courses
                new() {
                    RuleName = "Advanced Course Requirement",
                    LogicOperator = PrerequisiteLogicOperator.OR,
                    Requirements = new List<PrerequisiteRequirement>
                    {
                        new() { RequirementType = PrerequisiteType.Course, RequiredCourseId = 401 },
                        new() { RequirementType = PrerequisiteType.Course, RequiredCourseId = 402 },
                        new() { RequirementType = PrerequisiteType.Course, RequiredCourseId = 403 }
                    }
                }
            }
        };

        // Assert
        Assert.Equal(PrerequisiteLogicOperator.AND, complexRule.LogicOperator);
        Assert.Equal(2, complexRule.Requirements.Count);
        Assert.Single(complexRule.NestedRules);
        Assert.Equal(PrerequisiteLogicOperator.OR, complexRule.NestedRules.First().LogicOperator);
        Assert.Equal(3, complexRule.NestedRules.First().Requirements.Count);
    }

    #endregion

    #region Corequisite Requirements Tests

    [Fact]
    public void CorequisiteRequirement_Should_Enforce_Concurrent_Enrollment()
    {
        // Arrange & Act
        var corequisiteRule = new CorequisiteRule
        {
            Id = 1,
            CourseId = 250, // PHYS 250: Physics I
            RuleName = "Physics Lab Corequisite",
            IsActive = true,
            EnforcementType = CorequisiteEnforcementType.MustTakeSimultaneously,
            CorequisiteRequirements = new List<CorequisiteRequirement>
            {
                new() {
                    RequiredCourseId = 251, // PHYS 251: Physics I Lab
                    EnrollmentRelationship = CorequisiteRelationship.MustEnrollSimultaneously,
                    IsWaivable = false,
                    FailureAction = CorequisiteFailureAction.BlockEnrollment
                }
            }
        };

        // Assert
        Assert.Equal(CorequisiteEnforcementType.MustTakeSimultaneously, corequisiteRule.EnforcementType);
        Assert.Single(corequisiteRule.CorequisiteRequirements);
        Assert.Equal(CorequisiteRelationship.MustEnrollSimultaneously, 
            corequisiteRule.CorequisiteRequirements.First().EnrollmentRelationship);
        Assert.False(corequisiteRule.CorequisiteRequirements.First().IsWaivable);
    }

    [Fact]
    public void CorequisiteRequirement_Should_Support_Flexible_Timing()
    {
        // Arrange & Act
        var corequisiteRule = new CorequisiteRule
        {
            Id = 2,
            CourseId = 301, // CS 301: Data Structures
            RuleName = "Concurrent or Prior Math Requirement",
            IsActive = true,
            EnforcementType = CorequisiteEnforcementType.MustTakeBeforeOrWith,
            CorequisiteRequirements = new List<CorequisiteRequirement>
            {
                new() {
                    RequiredCourseId = 210, // MATH 210: Discrete Math
                    EnrollmentRelationship = CorequisiteRelationship.MustCompleteBeforeOrWith,
                    IsWaivable = true,
                    WaiverRequiredPermission = "MATH_DEPARTMENT_PERMISSION",
                    FailureAction = CorequisiteFailureAction.RequireAdvisorApproval
                }
            }
        };

        // Assert
        Assert.Equal(CorequisiteEnforcementType.MustTakeBeforeOrWith, corequisiteRule.EnforcementType);
        Assert.Equal(CorequisiteRelationship.MustCompleteBeforeOrWith, 
            corequisiteRule.CorequisiteRequirements.First().EnrollmentRelationship);
        Assert.True(corequisiteRule.CorequisiteRequirements.First().IsWaivable);
        Assert.Equal("MATH_DEPARTMENT_PERMISSION", 
            corequisiteRule.CorequisiteRequirements.First().WaiverRequiredPermission);
    }

    #endregion

    #region Enrollment Restrictions Tests

    [Fact]
    public void EnrollmentRestriction_Should_Support_Major_Requirements()
    {
        // Arrange & Act
        var majorRestriction = new EnrollmentRestriction
        {
            Id = 1,
            CourseId = 401, // CS 401: Senior Seminar
            RestrictionType = RestrictionType.MajorRequired,
            IsActive = true,
            Priority = 1,
            EnforcementLevel = RestrictionEnforcementLevel.Hard,
            MajorRestrictions = new List<MajorRestriction>
            {
                new() {
                    RequiredMajorCode = "CSCI",
                    MajorType = MajorType.Primary,
                    IsIncluded = true
                },
                new() {
                    RequiredMajorCode = "CSIS",
                    MajorType = MajorType.Primary,
                    IsIncluded = true
                },
                new() {
                    RequiredMajorCode = "SWEN",
                    MajorType = MajorType.Primary,
                    IsIncluded = true
                }
            }
        };

        // Assert
        Assert.Equal(RestrictionType.MajorRequired, majorRestriction.RestrictionType);
        Assert.Equal(RestrictionEnforcementLevel.Hard, majorRestriction.EnforcementLevel);
        Assert.Equal(3, majorRestriction.MajorRestrictions.Count);
        Assert.True(majorRestriction.MajorRestrictions.All(mr => mr.IsIncluded));
        Assert.True(majorRestriction.MajorRestrictions.All(mr => mr.MajorType == MajorType.Primary));
    }

    [Fact]
    public void EnrollmentRestriction_Should_Support_Class_Level_Requirements()
    {
        // Arrange & Act
        var classLevelRestriction = new EnrollmentRestriction
        {
            Id = 2,
            CourseId = 100, // GEN 100: First Year Seminar
            RestrictionType = RestrictionType.ClassStandingRequired,
            IsActive = true,
            Priority = 1,
            EnforcementLevel = RestrictionEnforcementLevel.Hard,
            ClassStandingRestrictions = new List<ClassStandingRestriction>
            {
                new() {
                    RequiredClassStanding = ClassStanding.Freshman,
                    IsIncluded = true,
                    MinimumCreditHours = 0,
                    MaximumCreditHours = 29
                }
            }
        };

        // Assert
        Assert.Equal(RestrictionType.ClassStandingRequired, classLevelRestriction.RestrictionType);
        Assert.Single(classLevelRestriction.ClassStandingRestrictions);
        Assert.Equal(ClassStanding.Freshman, 
            classLevelRestriction.ClassStandingRestrictions.First().RequiredClassStanding);
        Assert.Equal(29, classLevelRestriction.ClassStandingRestrictions.First().MaximumCreditHours);
    }

    [Fact]
    public void EnrollmentRestriction_Should_Support_Permission_Requirements()
    {
        // Arrange & Act
        var permissionRestriction = new EnrollmentRestriction
        {
            Id = 3,
            CourseId = 495, // CS 495: Independent Study
            RestrictionType = RestrictionType.PermissionRequired,
            IsActive = true,
            Priority = 1,
            EnforcementLevel = RestrictionEnforcementLevel.Hard,
            PermissionRestrictions = new List<PermissionRestriction>
            {
                new() {
                    RequiredPermission = "INSTRUCTOR_PERMISSION",
                    PermissionLevel = PermissionLevel.Instructor,
                    RequiresDocumentation = true,
                    DocumentationRequirements = "Research proposal and faculty advisor approval"
                },
                new() {
                    RequiredPermission = "DEPARTMENT_PERMISSION",
                    PermissionLevel = PermissionLevel.Department,
                    RequiresDocumentation = true,
                    DocumentationRequirements = "Completed application with project outline"
                }
            }
        };

        // Assert
        Assert.Equal(RestrictionType.PermissionRequired, permissionRestriction.RestrictionType);
        Assert.Equal(2, permissionRestriction.PermissionRestrictions.Count);
        Assert.True(permissionRestriction.PermissionRestrictions.All(pr => pr.RequiresDocumentation));
        Assert.Contains(permissionRestriction.PermissionRestrictions, 
            pr => pr.PermissionLevel == PermissionLevel.Instructor);
    }

    #endregion

    #region Prerequisite Validation Service Tests

    [Fact]
    public void PrerequisiteValidationResult_Should_Provide_Detailed_Feedback()
    {
        // Arrange & Act
        var validationResult = new PrerequisiteValidationResult
        {
            CourseId = 301,
            StudentId = 12345,
            IsValid = false,
            ValidationDate = DateTime.Now,
            OverallStatus = PrerequisiteValidationStatus.Failed,
            RequirementResults = new List<RequirementValidationResult>
            {
                new() {
                    RequirementId = 1,
                    RequirementType = PrerequisiteType.Course,
                    RequiredCourseId = 101,
                    Status = RequirementStatus.Satisfied,
                    CompletedGrade = "A",
                    CompletionDate = DateTime.Now.AddYears(-1)
                },
                new() {
                    RequirementId = 2,
                    RequirementType = PrerequisiteType.Course,
                    RequiredCourseId = 201,
                    Status = RequirementStatus.NotSatisfied,
                    FailureReason = "Course not completed",
                    SuggestedActions = new List<string> 
                    { 
                        "Complete CS 201 with grade C or better",
                        "Consider taking CS 201 in current semester if available"
                    }
                }
            },
            MissingRequirements = new List<MissingRequirement>
            {
                new() {
                    RequirementType = PrerequisiteType.Course,
                    RequiredCourseId = 201,
                    CourseName = "Object-Oriented Programming",
                    MinimumGrade = "C",
                    Priority = RequirementPriority.Critical
                }
            }
        };

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Equal(PrerequisiteValidationStatus.Failed, validationResult.OverallStatus);
        Assert.Equal(2, validationResult.RequirementResults.Count);
        Assert.Single(validationResult.MissingRequirements);
        
        var satisfiedRequirement = validationResult.RequirementResults.First(r => r.Status == RequirementStatus.Satisfied);
        Assert.Equal("A", satisfiedRequirement.CompletedGrade);
        
        var unsatisfiedRequirement = validationResult.RequirementResults.First(r => r.Status == RequirementStatus.NotSatisfied);
        Assert.Equal(2, unsatisfiedRequirement.SuggestedActions.Count);
    }

    [Fact]
    public void PrerequisiteValidationResult_Should_Support_Complex_Logic_Evaluation()
    {
        // Arrange & Act
        var complexValidationResult = new PrerequisiteValidationResult
        {
            CourseId = 485,
            StudentId = 12345,
            IsValid = true,
            ValidationDate = DateTime.Now,
            OverallStatus = PrerequisiteValidationStatus.Satisfied,
            LogicEvaluationResults = new List<LogicEvaluationResult>
            {
                new() {
                    RuleId = 1,
                    LogicOperator = PrerequisiteLogicOperator.AND,
                    IsEvaluated = true,
                    IsSatisfied = true,
                    ChildResults = new List<LogicEvaluationResult>
                    {
                        new() {
                            RuleId = 2,
                            LogicOperator = PrerequisiteLogicOperator.OR,
                            IsEvaluated = true,
                            IsSatisfied = true,
                            SatisfiedByRequirements = new List<int> { 401, 402 } // Student satisfied through multiple courses
                        }
                    }
                }
            }
        };

        // Assert
        Assert.True(complexValidationResult.IsValid);
        Assert.Equal(PrerequisiteValidationStatus.Satisfied, complexValidationResult.OverallStatus);
        Assert.Single(complexValidationResult.LogicEvaluationResults);
        Assert.True(complexValidationResult.LogicEvaluationResults.First().IsSatisfied);
        Assert.Single(complexValidationResult.LogicEvaluationResults.First().ChildResults);
        Assert.Equal(2, complexValidationResult.LogicEvaluationResults.First().ChildResults.First().SatisfiedByRequirements.Count);
    }

    #endregion

    #region Override and Waiver System Tests

    [Fact]
    public void PrerequisiteOverride_Should_Support_Administrative_Bypass()
    {
        // Arrange & Act
        var prerequisiteOverride = new PrerequisiteOverride
        {
            Id = 1,
            StudentId = 12345,
            CourseId = 401,
            OverrideType = OverrideType.AdministrativeOverride,
            Status = OverrideStatus.Approved,
            RequestedBy = "advisor@university.edu",
            RequestDate = DateTime.Now.AddDays(-5),
            ReviewedBy = "department-chair@university.edu",
            ReviewDate = DateTime.Now.AddDays(-2),
            ApprovalDate = DateTime.Now.AddDays(-1),
            Justification = "Student demonstrates exceptional preparation through work experience",
            Conditions = "Must meet with instructor during first week of class",
            ExpirationDate = DateTime.Now.AddMonths(6),
            IsActive = true,
            OverriddenRequirements = new List<OverriddenRequirement>
            {
                new() {
                    RequirementType = PrerequisiteType.Course,
                    RequiredCourseId = 301,
                    OriginalMinimumGrade = "C",
                    WaiverReason = "Industry experience equivalent to course content",
                    EquivalentExperience = "3 years software development experience"
                }
            }
        };

        // Assert
        Assert.Equal(OverrideType.AdministrativeOverride, prerequisiteOverride.OverrideType);
        Assert.Equal(OverrideStatus.Approved, prerequisiteOverride.Status);
        Assert.NotNull(prerequisiteOverride.Justification);
        Assert.NotNull(prerequisiteOverride.Conditions);
        Assert.Single(prerequisiteOverride.OverriddenRequirements);
        Assert.Equal("Industry experience equivalent to course content", 
            prerequisiteOverride.OverriddenRequirements.First().WaiverReason);
    }

    [Fact]
    public void PrerequisiteWaiver_Should_Support_Academic_Exceptions()
    {
        // Arrange & Act
        var academicWaiver = new PrerequisiteWaiver
        {
            Id = 1,
            StudentId = 12345,
            CourseId = 485,
            WaiverType = WaiverType.AcademicException,
            Status = WaiverStatus.Pending,
            RequestDate = DateTime.Now.AddDays(-3),
            AcademicJustification = "Transfer student with equivalent coursework from accredited institution",
            SupportingDocumentation = new List<WaiverDocument>
            {
                new() {
                    DocumentType = WaiverDocumentType.Transcript,
                    DocumentName = "Official Transcript - Previous University",
                    FilePath = "/documents/waivers/transcript_12345.pdf",
                    UploadDate = DateTime.Now.AddDays(-3)
                },
                new() {
                    DocumentType = WaiverDocumentType.CourseEquivalency,
                    DocumentName = "Course Equivalency Analysis",
                    FilePath = "/documents/waivers/equivalency_12345.pdf", 
                    UploadDate = DateTime.Now.AddDays(-2)
                }
            },
            RequiresCommitteeReview = true,
            ReviewCommittee = "Undergraduate Curriculum Committee",
            WaivedRequirements = new List<WaivedRequirement>
            {
                new() {
                    RequirementType = PrerequisiteType.Course,
                    RequiredCourseId = 301,
                    WaiverJustification = "Completed equivalent CS-301 at previous institution",
                    EquivalentCourse = "COMP 310 - Data Structures and Algorithms"
                }
            }
        };

        // Assert
        Assert.Equal(WaiverType.AcademicException, academicWaiver.WaiverType);
        Assert.Equal(WaiverStatus.Pending, academicWaiver.Status);
        Assert.Equal(2, academicWaiver.SupportingDocumentation.Count);
        Assert.True(academicWaiver.RequiresCommitteeReview);
        Assert.Equal("Undergraduate Curriculum Committee", academicWaiver.ReviewCommittee);
        Assert.Single(academicWaiver.WaivedRequirements);
    }

    #endregion

    #region Integration and Business Rule Tests

    [Fact]
    public void PrerequisiteSystem_Should_Prevent_Circular_Dependencies()
    {
        // Arrange & Act
        var circularDependencyCheck = new CircularDependencyValidator();
        var courseA = 301;
        var courseB = 302;
        var courseC = 303;

        var prerequisites = new List<(int CourseId, int PrerequisiteCourseId)>
        {
            (courseA, courseB), // A requires B
            (courseB, courseC), // B requires C
            (courseC, courseA)  // C requires A - creates circular dependency
        };

        var validationResult = circularDependencyCheck.ValidatePrerequisiteChain(prerequisites);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains("Circular dependency detected", validationResult.ErrorMessage);
        Assert.Equal(3, validationResult.CircularPath.Count);
        Assert.Contains(courseA, validationResult.CircularPath);
        Assert.Contains(courseB, validationResult.CircularPath);
        Assert.Contains(courseC, validationResult.CircularPath);
    }

    [Fact]
    public void EnrollmentValidation_Should_Check_All_Restrictions()
    {
        // Arrange & Act
        var enrollmentValidator = new EnrollmentValidator();
        var enrollmentRequest = new EnrollmentValidationRequest
        {
            StudentId = 12345,
            CourseId = 401,
            SemesterId = "FALL2024",
            Student = new StudentProfile
            {
                StudentId = 12345,
                PrimaryMajor = "CSCI",
                ClassStanding = ClassStanding.Senior,
                CompletedCreditHours = 95,
                CompletedCourses = new List<CompletedCourse>
                {
                    new() { CourseId = 101, Grade = "A", CompletionDate = DateTime.Now.AddYears(-2) },
                    new() { CourseId = 201, Grade = "B", CompletionDate = DateTime.Now.AddYears(-1) },
                    new() { CourseId = 301, Grade = "B+", CompletionDate = DateTime.Now.AddMonths(-6) }
                }
            }
        };

        var validationResult = enrollmentValidator.ValidateEnrollment(enrollmentRequest);

        // Assert
        Assert.True(validationResult.IsValid);
        Assert.Equal(EnrollmentValidationStatus.Approved, validationResult.Status);
        Assert.True(validationResult.PrerequisiteValidation.IsValid);
        Assert.True(validationResult.RestrictionValidation.IsValid);
        Assert.Empty(validationResult.ValidationErrors);
    }

    [Fact]
    public void PrerequisiteRule_Should_Validate_Business_Rules()
    {
        // Arrange
        var invalidRule = new PrerequisiteRule
        {
            CourseId = 301,
            RuleName = "", // Invalid: empty name
            LogicOperator = PrerequisiteLogicOperator.AND,
            Requirements = new List<PrerequisiteRequirement>() // Invalid: no requirements
        };

        var validationContext = new ValidationContext(invalidRule);
        var validationResults = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(invalidRule, validationContext, validationResults, true);

        // Assert
        Assert.False(isValid);
        Assert.True(validationResults.Any(vr => vr.ErrorMessage!.Contains("RuleName")));
        Assert.True(validationResults.Any(vr => vr.ErrorMessage!.Contains("Requirements")));
    }

    #endregion

    #region Helper Classes and Mock Data

    /// <summary>
    /// Mock validator for testing circular dependency detection
    /// </summary>
    public class CircularDependencyValidator
    {
        public CircularDependencyValidationResult ValidatePrerequisiteChain(
            List<(int CourseId, int PrerequisiteCourseId)> prerequisites)
        {
            var graph = BuildDependencyGraph(prerequisites);
            var visited = new HashSet<int>();
            var recursionStack = new HashSet<int>();
            var path = new List<int>();

            foreach (var courseId in graph.Keys)
            {
                if (!visited.Contains(courseId))
                {
                    if (HasCycle(graph, courseId, visited, recursionStack, path))
                    {
                        return new CircularDependencyValidationResult
                        {
                            IsValid = false,
                            ErrorMessage = "Circular dependency detected in prerequisite chain",
                            CircularPath = new List<int>(path)
                        };
                    }
                }
            }

            return new CircularDependencyValidationResult { IsValid = true };
        }

        private Dictionary<int, List<int>> BuildDependencyGraph(
            List<(int CourseId, int PrerequisiteCourseId)> prerequisites)
        {
            var graph = new Dictionary<int, List<int>>();
            
            foreach (var (courseId, prerequisiteId) in prerequisites)
            {
                if (!graph.ContainsKey(courseId))
                    graph[courseId] = new List<int>();
                if (!graph.ContainsKey(prerequisiteId))
                    graph[prerequisiteId] = new List<int>();
                    
                graph[courseId].Add(prerequisiteId);
            }
            
            return graph;
        }

        private bool HasCycle(Dictionary<int, List<int>> graph, int courseId, 
            HashSet<int> visited, HashSet<int> recursionStack, List<int> path)
        {
            visited.Add(courseId);
            recursionStack.Add(courseId);
            path.Add(courseId);

            foreach (var prerequisite in graph[courseId])
            {
                if (!visited.Contains(prerequisite))
                {
                    if (HasCycle(graph, prerequisite, visited, recursionStack, path))
                        return true;
                }
                else if (recursionStack.Contains(prerequisite))
                {
                    return true;
                }
            }

            recursionStack.Remove(courseId);
            path.RemoveAt(path.Count - 1);
            return false;
        }
    }

    public class CircularDependencyValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public List<int> CircularPath { get; set; } = new();
    }

    public class EnrollmentValidator
    {
        public EnrollmentValidationResult ValidateEnrollment(EnrollmentValidationRequest request)
        {
            // Mock validation logic - in real implementation would check database
            return new EnrollmentValidationResult
            {
                IsValid = true,
                Status = EnrollmentValidationStatus.Approved,
                PrerequisiteValidation = new PrerequisiteValidationResult { IsValid = true },
                RestrictionValidation = new RestrictionValidationResult { IsValid = true },
                ValidationErrors = new List<string>()
            };
        }
    }

    public class EnrollmentValidationRequest
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public string SemesterId { get; set; } = string.Empty;
        public StudentProfile Student { get; set; } = null!;
    }

    public class EnrollmentValidationResult
    {
        public bool IsValid { get; set; }
        public EnrollmentValidationStatus Status { get; set; }
        public PrerequisiteValidationResult PrerequisiteValidation { get; set; } = null!;
        public RestrictionValidationResult RestrictionValidation { get; set; } = null!;
        public List<string> ValidationErrors { get; set; } = new();
    }

    public class RestrictionValidationResult
    {
        public bool IsValid { get; set; }
    }

    public class StudentProfile
    {
        public int StudentId { get; set; }
        public string PrimaryMajor { get; set; } = string.Empty;
        public ClassStanding ClassStanding { get; set; }
        public int CompletedCreditHours { get; set; }
        public List<CompletedCourse> CompletedCourses { get; set; } = new();
    }

    public class CompletedCourse
    {
        public int CourseId { get; set; }
        public string Grade { get; set; } = string.Empty;
        public DateTime CompletionDate { get; set; }
    }

    #endregion
}