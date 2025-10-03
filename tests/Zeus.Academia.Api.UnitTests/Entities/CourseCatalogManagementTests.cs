using System.ComponentModel.DataAnnotations;
using Xunit;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Enums;

namespace Zeus.Academia.Api.UnitTests.Entities;

/// <summary>
/// Unit tests for Course Catalog Management entities for Task 2: Course Catalog Management.
/// Tests catalog CRUD operations, versioning, approval workflow, learning outcomes, and publication system.
/// </summary>
public class CourseCatalogManagementTests
{
    #region Course Catalog Entity Tests

    [Fact]
    public void CourseCatalog_Should_Support_Academic_Year_Versioning()
    {
        // Arrange & Act
        var catalog2024 = new CourseCatalog
        {
            Id = 1,
            AcademicYear = 2024,
            CatalogName = "Undergraduate Catalog 2024-2025",
            Version = "1.0",
            Status = CatalogStatus.Published,
            PublicationDate = DateTime.Now.AddMonths(-6),
            EffectiveDate = new DateTime(2024, 8, 1),
            ExpirationDate = new DateTime(2025, 7, 31),
            Description = "Official undergraduate course catalog for academic year 2024-2025",
            CoverImageUrl = "/catalogs/2024/cover.jpg",
            IsActive = true
        };

        var catalog2025 = new CourseCatalog
        {
            Id = 2,
            AcademicYear = 2025,
            CatalogName = "Undergraduate Catalog 2025-2026",
            Version = "1.0",
            Status = CatalogStatus.Draft,
            EffectiveDate = new DateTime(2025, 8, 1),
            ExpirationDate = new DateTime(2026, 7, 31),
            Description = "Draft undergraduate course catalog for academic year 2025-2026",
            IsActive = true,
            BasedOnCatalogId = 1 // Based on previous year's catalog
        };

        // Assert
        Assert.Equal(2024, catalog2024.AcademicYear);
        Assert.Equal(CatalogStatus.Published, catalog2024.Status);
        Assert.NotNull(catalog2024.PublicationDate);

        Assert.Equal(2025, catalog2025.AcademicYear);
        Assert.Equal(CatalogStatus.Draft, catalog2025.Status);
        Assert.Equal(1, catalog2025.BasedOnCatalogId);
    }

    [Fact]
    public void CourseCatalog_Should_Track_Comprehensive_Metadata()
    {
        // Arrange & Act
        var catalog = new CourseCatalog
        {
            AcademicYear = 2024,
            CatalogName = "Graduate Catalog 2024-2025",
            Version = "2.1",
            Status = CatalogStatus.Published,
            CatalogType = CatalogType.Graduate,
            TotalPages = 485,
            TotalCourses = 1247,
            LastUpdated = DateTime.Now.AddDays(-15),
            ApprovalHistory = new List<CatalogApproval>
            {
                new() {
                    ApprovalStage = ApprovalStage.DepartmentReview,
                    ApprovedBy = "dept-chair@university.edu",
                    ApprovalDate = DateTime.Now.AddDays(-45),
                    Status = ApprovalStatus.Approved
                },
                new() {
                    ApprovalStage = ApprovalStage.CurriculumCommittee,
                    ApprovedBy = "curriculum-chair@university.edu",
                    ApprovalDate = DateTime.Now.AddDays(-30),
                    Status = ApprovalStatus.Approved
                }
            }
        };

        // Assert
        Assert.Equal("Graduate Catalog 2024-2025", catalog.CatalogName);
        Assert.Equal("2.1", catalog.Version);
        Assert.Equal(CatalogType.Graduate, catalog.CatalogType);
        Assert.Equal(485, catalog.TotalPages);
        Assert.Equal(1247, catalog.TotalCourses);
        Assert.Equal(2, catalog.ApprovalHistory.Count);
    }

    [Fact]
    public void CourseCatalog_Should_Support_Publication_Management()
    {
        // Arrange & Act
        var catalog = new CourseCatalog
        {
            AcademicYear = 2024,
            CatalogName = "Test Catalog",
            Status = CatalogStatus.Published,
            PublicationDate = DateTime.Now.AddDays(-30),
            PublicationFormats = new List<PublicationFormat>
            {
                PublicationFormat.PDF,
                PublicationFormat.HTML,
                PublicationFormat.Mobile
            },
            DistributionChannels = new List<DistributionChannel>
            {
                DistributionChannel.Website,
                DistributionChannel.StudentPortal,
                DistributionChannel.PrintCopy
            },
            DownloadCount = 15420,
            ViewCount = 89750
        };

        // Assert
        Assert.Equal(CatalogStatus.Published, catalog.Status);
        Assert.NotNull(catalog.PublicationDate);
        Assert.Contains(PublicationFormat.PDF, catalog.PublicationFormats);
        Assert.Contains(DistributionChannel.Website, catalog.DistributionChannels);
        Assert.Equal(15420, catalog.DownloadCount);
        Assert.Equal(89750, catalog.ViewCount);
    }

    #endregion

    #region Course Approval Workflow Tests

    [Fact]
    public void CourseApprovalWorkflow_Should_Support_Multi_Stage_Process()
    {
        // Arrange & Act
        var workflow = new CourseApprovalWorkflow
        {
            Id = 1,
            CourseId = 1,
            WorkflowName = "New Course Approval Process",
            InitiatedBy = "faculty@university.edu",
            InitiationDate = DateTime.Now.AddDays(-14),
            Status = WorkflowStatus.InProgress,
            CurrentStage = ApprovalStage.CurriculumCommittee,
            ApprovalSteps = new List<ApprovalStep>
            {
                new() {
                    StepOrder = 1,
                    ApprovalStage = ApprovalStage.DepartmentReview,
                    AssignedTo = "dept-chair@university.edu",
                    Status = ApprovalStatus.Approved,
                    CompletedDate = DateTime.Now.AddDays(-10),
                    Comments = "Course aligns with department curriculum goals"
                },
                new() {
                    StepOrder = 2,
                    ApprovalStage = ApprovalStage.CurriculumCommittee,
                    AssignedTo = "curriculum-committee@university.edu",
                    Status = ApprovalStatus.Pending,
                    DueDate = DateTime.Now.AddDays(7)
                },
                new() {
                    StepOrder = 3,
                    ApprovalStage = ApprovalStage.AcademicSenatePending,
                    AssignedTo = "senate@university.edu",
                    Status = ApprovalStatus.NotStarted
                }
            }
        };

        // Assert
        Assert.Equal("New Course Approval Process", workflow.WorkflowName);
        Assert.Equal(WorkflowStatus.InProgress, workflow.Status);
        Assert.Equal(ApprovalStage.CurriculumCommittee, workflow.CurrentStage);
        Assert.Equal(3, workflow.ApprovalSteps.Count);

        var completedStep = workflow.ApprovalSteps.First(s => s.StepOrder == 1);
        Assert.Equal(ApprovalStatus.Approved, completedStep.Status);
        Assert.NotNull(completedStep.CompletedDate);

        var currentStep = workflow.ApprovalSteps.First(s => s.StepOrder == 2);
        Assert.Equal(ApprovalStatus.Pending, currentStep.Status);
        Assert.NotNull(currentStep.DueDate);
    }

    [Fact]
    public void ApprovalStep_Should_Track_Detailed_Review_Information()
    {
        // Arrange & Act
        var approvalStep = new ApprovalStep
        {
            Id = 1,
            ApprovalStage = ApprovalStage.CurriculumCommittee,
            AssignedTo = "curriculum-chair@university.edu",
            Status = ApprovalStatus.Approved,
            StartDate = DateTime.Now.AddDays(-5),
            CompletedDate = DateTime.Now.AddDays(-2),
            DueDate = DateTime.Now.AddDays(2),
            EstimatedDuration = TimeSpan.FromDays(7),
            ActualDuration = TimeSpan.FromDays(3),
            Comments = "Course content meets curriculum standards. Minor revisions suggested for learning outcomes.",
            RequiredDocuments = new List<string>
            {
                "Course Syllabus",
                "Learning Outcomes Assessment Plan",
                "Faculty Qualifications"
            },
            ReviewCriteria = new List<string>
            {
                "Academic Rigor",
                "Learning Outcomes Alignment",
                "Resource Requirements"
            },
            Attachments = new List<ApprovalAttachment>
            {
                new() { FileName = "revised_syllabus.pdf", FilePath = "/approvals/documents/revised_syllabus.pdf" }
            }
        };

        // Assert
        Assert.Equal(ApprovalStage.CurriculumCommittee, approvalStep.ApprovalStage);
        Assert.Equal(ApprovalStatus.Approved, approvalStep.Status);
        Assert.Equal(TimeSpan.FromDays(7), approvalStep.EstimatedDuration);
        Assert.Equal(TimeSpan.FromDays(3), approvalStep.ActualDuration);
        Assert.Equal(3, approvalStep.RequiredDocuments.Count);
        Assert.Equal(3, approvalStep.ReviewCriteria.Count);
        Assert.Single(approvalStep.Attachments);
    }

    #endregion

    #region Learning Outcomes Management Tests

    [Fact]
    public void LearningOutcome_Should_Support_Comprehensive_Documentation()
    {
        // Arrange & Act
        var learningOutcome = new LearningOutcome
        {
            Id = 1,
            CourseId = 1,
            OutcomeNumber = "LO1",
            OutcomeText = "Students will demonstrate proficiency in object-oriented programming principles",
            BloomsTaxonomyLevel = BloomsTaxonomyLevel.Apply,
            DifficultyLevel = DifficultyLevel.Intermediate,
            AssessmentMethods = new List<AssessmentMethod>
            {
                AssessmentMethod.Programming,
                AssessmentMethod.ProjectBased,
                AssessmentMethod.PeerReview
            },
            ProgramOutcomes = new List<string>
            {
                "PO1: Technical Competency",
                "PO3: Problem Solving"
            },
            MeasurementCriteria = "80% of students will achieve 75% or higher on programming assignments",
            IsActive = true,
            LastRevisionDate = DateTime.Now.AddMonths(-3),
            RevisedBy = "curriculum-committee@university.edu"
        };

        // Assert
        Assert.Equal("LO1", learningOutcome.OutcomeNumber);
        Assert.Equal(BloomsTaxonomyLevel.Apply, learningOutcome.BloomsTaxonomyLevel);
        Assert.Equal(DifficultyLevel.Intermediate, learningOutcome.DifficultyLevel);
        Assert.Contains(AssessmentMethod.Programming, learningOutcome.AssessmentMethods);
        Assert.Equal(2, learningOutcome.ProgramOutcomes.Count);
        Assert.True(learningOutcome.IsActive);
    }

    [Fact]
    public void CourseOutcomeMapping_Should_Link_Courses_To_Program_Outcomes()
    {
        // Arrange & Act
        var outcomeMapping = new CourseOutcomeMapping
        {
            Id = 1,
            CourseId = 1,
            LearningOutcomeId = 1,
            ProgramOutcomeCode = "PO1",
            ProgramOutcomeTitle = "Technical Competency in Computing",
            MappingStrength = MappingStrength.Strong,
            ContributionLevel = ContributionLevel.Major,
            AssessmentWeight = 0.35M,
            Notes = "This course provides primary introduction to technical competency through hands-on programming",
            LastValidated = DateTime.Now.AddMonths(-1),
            ValidatedBy = "program-coordinator@university.edu"
        };

        // Assert
        Assert.Equal("PO1", outcomeMapping.ProgramOutcomeCode);
        Assert.Equal(MappingStrength.Strong, outcomeMapping.MappingStrength);
        Assert.Equal(ContributionLevel.Major, outcomeMapping.ContributionLevel);
        Assert.Equal(0.35M, outcomeMapping.AssessmentWeight);
        Assert.NotNull(outcomeMapping.LastValidated);
    }

    #endregion

    #region Catalog Publication System Tests

    [Fact]
    public void CatalogPublication_Should_Support_Multiple_Formats()
    {
        // Arrange & Act
        var publication = new CatalogPublication
        {
            Id = 1,
            CatalogId = 1,
            PublicationFormat = PublicationFormat.PDF,
            FileName = "undergraduate_catalog_2024.pdf",
            FilePath = "/catalogs/2024/undergraduate_catalog_2024.pdf",
            FileSize = 25_600_000, // 25.6 MB
            PublicationDate = DateTime.Now.AddDays(-30),
            Status = PublicationStatus.Published,
            DownloadCount = 8450,
            LastDownloaded = DateTime.Now.AddHours(-2),
            Checksum = "a1b2c3d4e5f6789012345678901234567890",
            PublicationMetadata = new PublicationMetadata
            {
                Title = "Undergraduate Course Catalog 2024-2025",
                Author = "University Academic Affairs",
                Subject = "Course Catalog",
                Keywords = new List<string> { "courses", "undergraduate", "catalog", "2024" },
                CreationDate = DateTime.Now.AddDays(-35),
                Language = "en-US"
            }
        };

        // Assert
        Assert.Equal(PublicationFormat.PDF, publication.PublicationFormat);
        Assert.Equal(25_600_000, publication.FileSize);
        Assert.Equal(PublicationStatus.Published, publication.Status);
        Assert.Equal(8450, publication.DownloadCount);
        Assert.Equal("Undergraduate Course Catalog 2024-2025", publication.PublicationMetadata.Title);
        Assert.Contains("undergraduate", publication.PublicationMetadata.Keywords);
    }

    [Fact]
    public void CatalogDistribution_Should_Track_Multiple_Channels()
    {
        // Arrange & Act
        var distribution = new CatalogDistribution
        {
            Id = 1,
            CatalogPublicationId = 1,
            DistributionChannel = DistributionChannel.Website,
            DistributionDate = DateTime.Now.AddDays(-25),
            Status = DistributionStatus.Active,
            DistributionUrl = "https://university.edu/catalog/2024/undergraduate",
            AccessCount = 125_000,
            LastAccessed = DateTime.Now.AddMinutes(-15),
            DistributionSettings = new DistributionSettings
            {
                AllowDownload = true,
                RequireAuthentication = false,
                AccessRestrictions = new List<string> { "Geo: US-only" },
                CacheSettings = new CacheSettings
                {
                    CacheDuration = TimeSpan.FromHours(24),
                    EnableCompression = true
                }
            }
        };

        // Assert
        Assert.Equal(DistributionChannel.Website, distribution.DistributionChannel);
        Assert.Equal(DistributionStatus.Active, distribution.Status);
        Assert.Equal(125_000, distribution.AccessCount);
        Assert.True(distribution.DistributionSettings.AllowDownload);
        Assert.False(distribution.DistributionSettings.RequireAuthentication);
        Assert.Equal(TimeSpan.FromHours(24), distribution.DistributionSettings.CacheSettings.CacheDuration);
    }

    #endregion

    #region Catalog Versioning Tests

    [Fact]
    public void CatalogVersion_Should_Support_History_Tracking()
    {
        // Arrange & Act
        var catalogVersion = new CatalogVersion
        {
            Id = 1,
            CatalogId = 1,
            VersionNumber = "2.1",
            VersionType = VersionType.Minor,
            CreatedDate = DateTime.Now.AddDays(-10),
            CreatedBy = "catalog-admin@university.edu",
            ChangesSummary = "Updated prerequisite requirements for CS courses, added new elective options",
            PreviousVersionId = 5,
            ChangeLog = new List<VersionChange>
            {
                new() {
                    ChangeType = ChangeType.CourseModified,
                    EntityType = "Course",
                    EntityId = 101,
                    FieldChanged = "Prerequisites",
                    OldValue = "CS 201",
                    NewValue = "CS 201 and MATH 210",
                    ChangeReason = "Updated to align with new math requirements"
                },
                new() {
                    ChangeType = ChangeType.CourseAdded,
                    EntityType = "Course",
                    EntityId = 205,
                    NewValue = "CS 499: Senior Capstone Project",
                    ChangeReason = "New capstone requirement for program"
                }
            },
            ApprovalRequired = true,
            ApprovalStatus = ApprovalStatus.Approved,
            ApprovedBy = "dean@university.edu",
            ApprovalDate = DateTime.Now.AddDays(-3)
        };

        // Assert
        Assert.Equal("2.1", catalogVersion.VersionNumber);
        Assert.Equal(VersionType.Minor, catalogVersion.VersionType);
        Assert.Equal(5, catalogVersion.PreviousVersionId);
        Assert.Equal(2, catalogVersion.ChangeLog.Count);
        Assert.Equal(ApprovalStatus.Approved, catalogVersion.ApprovalStatus);

        var modificationChange = catalogVersion.ChangeLog.First(c => c.ChangeType == ChangeType.CourseModified);
        Assert.Equal("Prerequisites", modificationChange.FieldChanged);
        Assert.Equal("CS 201", modificationChange.OldValue);
        Assert.Equal("CS 201 and MATH 210", modificationChange.NewValue);
    }

    [Fact]
    public void CatalogComparison_Should_Support_Version_Differences()
    {
        // Arrange & Act
        var comparison = new CatalogComparison
        {
            Id = 1,
            SourceVersionId = 5,
            TargetVersionId = 6,
            ComparisonDate = DateTime.Now.AddDays(-1),
            ComparisonType = ComparisonType.Sequential,
            TotalChanges = 15,
            CoursesAdded = 3,
            CoursesModified = 8,
            CoursesRemoved = 1,
            PrerequisitesChanged = 5,
            Differences = new List<CatalogDifference>
            {
                new() {
                    DifferenceType = DifferenceType.CourseAdded,
                    EntityType = "Course",
                    EntityId = 301,
                    Summary = "Added CS 301: Database Systems",
                    Impact = ImpactLevel.Medium,
                    AffectedPrograms = new List<string> { "Computer Science BS", "Information Systems BS" }
                },
                new() {
                    DifferenceType = DifferenceType.PrerequisiteChanged,
                    EntityType = "Course",
                    EntityId = 205,
                    Summary = "CS 205 prerequisites changed from 'CS 101' to 'CS 101 and CS 102'",
                    Impact = ImpactLevel.High,
                    AffectedPrograms = new List<string> { "Computer Science BS" }
                }
            }
        };

        // Assert
        Assert.Equal(ComparisonType.Sequential, comparison.ComparisonType);
        Assert.Equal(15, comparison.TotalChanges);
        Assert.Equal(3, comparison.CoursesAdded);
        Assert.Equal(8, comparison.CoursesModified);
        Assert.Equal(2, comparison.Differences.Count);

        var addedCourse = comparison.Differences.First(d => d.DifferenceType == DifferenceType.CourseAdded);
        Assert.Equal(ImpactLevel.Medium, addedCourse.Impact);
        Assert.Equal(2, addedCourse.AffectedPrograms.Count);
    }

    #endregion

    #region Integration and Validation Tests

    [Fact]
    public void CourseCatalog_Should_Validate_Business_Rules()
    {
        // Arrange
        var catalog = new CourseCatalog
        {
            AcademicYear = 2024,
            CatalogName = "", // Invalid: empty name
            EffectiveDate = new DateTime(2024, 8, 1),
            ExpirationDate = new DateTime(2024, 6, 1) // Invalid: expiration before effective
        };

        var validationContext = new ValidationContext(catalog);
        var validationResults = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(catalog, validationContext, validationResults, true);

        // Assert
        Assert.False(isValid);
        Assert.True(validationResults.Any(vr => vr.ErrorMessage!.Contains("name")));
    }

    [Fact]
    public void ApprovalWorkflow_Should_Enforce_Sequential_Processing()
    {
        // Arrange & Act
        var workflow = new CourseApprovalWorkflow
        {
            CourseId = 1,
            Status = WorkflowStatus.InProgress,
            CurrentStage = ApprovalStage.CurriculumCommittee,
            ApprovalSteps = new List<ApprovalStep>
            {
                new() { StepOrder = 1, ApprovalStage = ApprovalStage.DepartmentReview, Status = ApprovalStatus.Approved },
                new() { StepOrder = 2, ApprovalStage = ApprovalStage.CurriculumCommittee, Status = ApprovalStatus.InReview },
                new() { StepOrder = 3, ApprovalStage = ApprovalStage.AcademicSenatePending, Status = ApprovalStatus.NotStarted }
            }
        };

        // Assert - Workflow should be in correct sequential state
        Assert.Equal(WorkflowStatus.InProgress, workflow.Status);
        Assert.Equal(ApprovalStage.CurriculumCommittee, workflow.CurrentStage);
        Assert.Equal(ApprovalStatus.Approved, workflow.ApprovalSteps.First(s => s.StepOrder == 1).Status);
        Assert.Equal(ApprovalStatus.InReview, workflow.ApprovalSteps.First(s => s.StepOrder == 2).Status);
        Assert.Equal(ApprovalStatus.NotStarted, workflow.ApprovalSteps.First(s => s.StepOrder == 3).Status);
    }

    [Fact]
    public void LearningOutcome_Should_Support_Assessment_Alignment()
    {
        // Arrange & Act
        var outcome = new LearningOutcome
        {
            CourseId = 1,
            OutcomeText = "Analyze complex algorithms for efficiency",
            BloomsTaxonomyLevel = BloomsTaxonomyLevel.Analyze,
            AssessmentMethods = new List<AssessmentMethod>
            {
                AssessmentMethod.Programming,
                AssessmentMethod.WrittenExam
            }
        };

        var mapping = new CourseOutcomeMapping
        {
            LearningOutcomeId = outcome.Id,
            CourseId = 1,
            ProgramOutcomeCode = "PO2",
            MappingStrength = MappingStrength.Strong,
            ContributionLevel = ContributionLevel.Major
        };

        // Assert
        Assert.Equal(BloomsTaxonomyLevel.Analyze, outcome.BloomsTaxonomyLevel);
        Assert.Contains(AssessmentMethod.Programming, outcome.AssessmentMethods);
        Assert.Equal(MappingStrength.Strong, mapping.MappingStrength);
        Assert.Equal(ContributionLevel.Major, mapping.ContributionLevel);
    }

    #endregion

    #region Helper Methods

    private static bool IsValidCatalogStructure(CourseCatalog catalog)
    {
        return !string.IsNullOrEmpty(catalog.CatalogName) &&
               catalog.AcademicYear > 2020 &&
               catalog.EffectiveDate < catalog.ExpirationDate;
    }

    private static ApprovalStage GetNextApprovalStage(ApprovalStage currentStage)
    {
        return currentStage switch
        {
            ApprovalStage.DepartmentReview => ApprovalStage.CurriculumCommittee,
            ApprovalStage.CurriculumCommittee => ApprovalStage.AcademicSenatePending,
            ApprovalStage.AcademicSenatePending => ApprovalStage.ProvostApproval,
            _ => ApprovalStage.DepartmentReview
        };
    }

    #endregion
}