using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zeus.Academia.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseCatalogManagementEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CourseCatalogId",
                table: "Courses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CourseCatalogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcademicYear = table.Column<int>(type: "int", nullable: false),
                    CatalogName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Version = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CatalogType = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    EffectiveDate = table.Column<DateTime>(type: "date", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "date", nullable: false),
                    PublicationDate = table.Column<DateTime>(type: "date", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CoverImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TotalPages = table.Column<int>(type: "int", nullable: true),
                    TotalCourses = table.Column<int>(type: "int", nullable: true),
                    DownloadCount = table.Column<int>(type: "int", nullable: false),
                    ViewCount = table.Column<int>(type: "int", nullable: false),
                    BasedOnCatalogId = table.Column<int>(type: "int", nullable: true),
                    PublicationFormats = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DistributionChannels = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseCatalogs", x => x.Id);
                    table.CheckConstraint("CK_CourseCatalogs_AcademicYear", "[AcademicYear] >= 2020 AND [AcademicYear] <= 2050");
                    table.CheckConstraint("CK_CourseCatalogs_Dates", "[EffectiveDate] <= [ExpirationDate]");
                    table.ForeignKey(
                        name: "FK_CourseCatalogs_CourseCatalogs_BasedOnCatalogId",
                        column: x => x.BasedOnCatalogId,
                        principalTable: "CourseCatalogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LearningOutcomes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: true),
                    SubjectCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    OutcomeText = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BloomsTaxonomyLevel = table.Column<int>(type: "int", nullable: false),
                    DifficultyLevel = table.Column<int>(type: "int", nullable: false),
                    ExpectedMasteryLevel = table.Column<int>(type: "int", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(5,4)", nullable: false),
                    IsMeasurable = table.Column<bool>(type: "bit", nullable: false),
                    IsObservable = table.Column<bool>(type: "bit", nullable: false),
                    AssessmentMethods = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProgramOutcomeAlignment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrerequisiteOutcomeIds = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Version = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    LastReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearningOutcomes", x => x.Id);
                    table.CheckConstraint("CK_LearningOutcomes_ExpectedMasteryLevel", "[ExpectedMasteryLevel] >= 0 AND [ExpectedMasteryLevel] <= 100");
                    table.CheckConstraint("CK_LearningOutcomes_Weight", "[Weight] >= 0 AND [Weight] <= 1");
                    table.ForeignKey(
                        name: "FK_LearningOutcomes_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LearningOutcomes_Subjects_SubjectCode",
                        column: x => x.SubjectCode,
                        principalTable: "Subjects",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CatalogApprovals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CatalogId = table.Column<int>(type: "int", nullable: false),
                    ApprovalStage = table.Column<int>(type: "int", nullable: false),
                    ApprovedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogApprovals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatalogApprovals_CourseCatalogs_CatalogId",
                        column: x => x.CatalogId,
                        principalTable: "CourseCatalogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CatalogPublications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CatalogId = table.Column<int>(type: "int", nullable: false),
                    Format = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AccessUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: true),
                    PageCount = table.Column<int>(type: "int", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PublicationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PublishedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DownloadCount = table.Column<int>(type: "int", nullable: false),
                    ViewCount = table.Column<int>(type: "int", nullable: false),
                    DistributionChannels = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SecuritySettings = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PublicationSettings = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Checksum = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Version = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogPublications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatalogPublications_CourseCatalogs_CatalogId",
                        column: x => x.CatalogId,
                        principalTable: "CourseCatalogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CatalogVersions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CatalogId = table.Column<int>(type: "int", nullable: false),
                    VersionNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    VersionType = table.Column<int>(type: "int", nullable: false),
                    VersionName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    PublishedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PublishedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PreviousVersionId = table.Column<int>(type: "int", nullable: true),
                    SnapshotData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SnapshotSize = table.Column<long>(type: "bigint", nullable: true),
                    ChangeSummary = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ChangeLog = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReleaseNotes = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApprovalStatus = table.Column<int>(type: "int", nullable: false),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsArchived = table.Column<bool>(type: "bit", nullable: false),
                    ArchivedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatalogVersions_CatalogVersions_PreviousVersionId",
                        column: x => x.PreviousVersionId,
                        principalTable: "CatalogVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CatalogVersions_CourseCatalogs_CatalogId",
                        column: x => x.CatalogId,
                        principalTable: "CourseCatalogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseApprovalWorkflows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    CatalogId = table.Column<int>(type: "int", nullable: true),
                    WorkflowName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    InitiatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    InitiationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CurrentStage = table.Column<int>(type: "int", nullable: false),
                    CompletionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ExpectedCompletionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Priority = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseApprovalWorkflows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseApprovalWorkflows_CourseCatalogs_CatalogId",
                        column: x => x.CatalogId,
                        principalTable: "CourseCatalogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CourseApprovalWorkflows_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LearningOutcomePrerequisites",
                columns: table => new
                {
                    PrerequisiteId = table.Column<int>(type: "int", nullable: false),
                    DependentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearningOutcomePrerequisites", x => new { x.PrerequisiteId, x.DependentId });
                    table.ForeignKey(
                        name: "FK_LearningOutcomePrerequisites_LearningOutcomes_DependentId",
                        column: x => x.DependentId,
                        principalTable: "LearningOutcomes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LearningOutcomePrerequisites_LearningOutcomes_PrerequisiteId",
                        column: x => x.PrerequisiteId,
                        principalTable: "LearningOutcomes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OutcomeAssessments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LearningOutcomeId = table.Column<int>(type: "int", nullable: false),
                    AssessmentName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AssessmentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AssessmentMethod = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(5,4)", nullable: false),
                    RubricCriteria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ScoringScale = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MinimumPassingScore = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    TargetScore = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    AssessmentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutcomeAssessments", x => x.Id);
                    table.CheckConstraint("CK_OutcomeAssessments_Weight", "[Weight] >= 0 AND [Weight] <= 1");
                    table.ForeignKey(
                        name: "FK_OutcomeAssessments_LearningOutcomes_LearningOutcomeId",
                        column: x => x.LearningOutcomeId,
                        principalTable: "LearningOutcomes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PublicationAccessLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CatalogPublicationId = table.Column<int>(type: "int", nullable: false),
                    AccessDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AccessType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AccessedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Referrer = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SessionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AccessDurationSeconds = table.Column<int>(type: "int", nullable: true),
                    IsSuccessful = table.Column<bool>(type: "bit", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicationAccessLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PublicationAccessLogs_CatalogPublications_CatalogPublicationId",
                        column: x => x.CatalogPublicationId,
                        principalTable: "CatalogPublications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PublicationDistributions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CatalogPublicationId = table.Column<int>(type: "int", nullable: false),
                    Channel = table.Column<int>(type: "int", nullable: false),
                    DistributionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DistributedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TargetAudience = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RecipientCount = table.Column<int>(type: "int", nullable: true),
                    DeliveryConfirmationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RetryCount = table.Column<int>(type: "int", nullable: false),
                    NextRetryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Metrics = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicationDistributions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PublicationDistributions_CatalogPublications_CatalogPublicationId",
                        column: x => x.CatalogPublicationId,
                        principalTable: "CatalogPublications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VersionChanges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CatalogVersionId = table.Column<int>(type: "int", nullable: false),
                    ChangeType = table.Column<int>(type: "int", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    PropertyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ChangeDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ImpactLevel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RequiresApproval = table.Column<bool>(type: "bit", nullable: false),
                    ApprovalStatus = table.Column<int>(type: "int", nullable: false),
                    ChangeMetadata = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VersionChanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VersionChanges_CatalogVersions_CatalogVersionId",
                        column: x => x.CatalogVersionId,
                        principalTable: "CatalogVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VersionComparisons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceVersionId = table.Column<int>(type: "int", nullable: false),
                    TargetVersionId = table.Column<int>(type: "int", nullable: false),
                    ComparisonType = table.Column<int>(type: "int", nullable: false),
                    ComparisonDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ComparedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ComparisonResults = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DifferencesSummary = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    AdditionsCount = table.Column<int>(type: "int", nullable: false),
                    ModificationsCount = table.Column<int>(type: "int", nullable: false),
                    DeletionsCount = table.Column<int>(type: "int", nullable: false),
                    ComparisonMetrics = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SimilarityPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    IsArchived = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VersionComparisons", x => x.Id);
                    table.CheckConstraint("CK_VersionComparisons_SimilarityPercentage", "[SimilarityPercentage] >= 0 AND [SimilarityPercentage] <= 100");
                    table.CheckConstraint("CK_VersionComparisons_SourceTarget", "[SourceVersionId] <> [TargetVersionId]");
                    table.ForeignKey(
                        name: "FK_VersionComparisons_CatalogVersions_SourceVersionId",
                        column: x => x.SourceVersionId,
                        principalTable: "CatalogVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VersionComparisons_CatalogVersions_TargetVersionId",
                        column: x => x.TargetVersionId,
                        principalTable: "CatalogVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalSteps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkflowId = table.Column<int>(type: "int", nullable: false),
                    StepOrder = table.Column<int>(type: "int", nullable: false),
                    ApprovalStage = table.Column<int>(type: "int", nullable: false),
                    AssignedTo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EstimatedDuration = table.Column<TimeSpan>(type: "time", nullable: false),
                    ActualDuration = table.Column<TimeSpan>(type: "time", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    RequiredDocuments = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReviewCriteria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalSteps_CourseApprovalWorkflows_WorkflowId",
                        column: x => x.WorkflowId,
                        principalTable: "CourseApprovalWorkflows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssessmentResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OutcomeAssessmentId = table.Column<int>(type: "int", nullable: false),
                    AssessedEntity = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Score = table.Column<decimal>(type: "decimal(10,4)", nullable: false),
                    MaxScore = table.Column<decimal>(type: "decimal(10,4)", nullable: false),
                    PercentageScore = table.Column<decimal>(type: "decimal(7,4)", nullable: false),
                    PassesCriteria = table.Column<bool>(type: "bit", nullable: false),
                    MeetsTarget = table.Column<bool>(type: "bit", nullable: false),
                    AssessmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AssessedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DetailedScores = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentResults", x => x.Id);
                    table.CheckConstraint("CK_AssessmentResults_MaxScore", "[MaxScore] > 0");
                    table.CheckConstraint("CK_AssessmentResults_Score", "[Score] >= 0");
                    table.ForeignKey(
                        name: "FK_AssessmentResults_OutcomeAssessments_OutcomeAssessmentId",
                        column: x => x.OutcomeAssessmentId,
                        principalTable: "OutcomeAssessments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComparisonDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VersionComparisonId = table.Column<int>(type: "int", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    PropertyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ChangeType = table.Column<int>(type: "int", nullable: false),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Significance = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComparisonDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComparisonDetails_VersionComparisons_VersionComparisonId",
                        column: x => x.VersionComparisonId,
                        principalTable: "VersionComparisons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApprovalStepId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    UploadDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UploadedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalAttachments_ApprovalSteps_ApprovalStepId",
                        column: x => x.ApprovalStepId,
                        principalTable: "ApprovalSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CourseCatalogId",
                table: "Courses",
                column: "CourseCatalogId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalAttachments_ApprovalStepId",
                table: "ApprovalAttachments",
                column: "ApprovalStepId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalSteps_Status",
                table: "ApprovalSteps",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalSteps_WorkflowId",
                table: "ApprovalSteps",
                column: "WorkflowId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalSteps_WorkflowId_StepOrder",
                table: "ApprovalSteps",
                columns: new[] { "WorkflowId", "StepOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentResults_AssessmentDate",
                table: "AssessmentResults",
                column: "AssessmentDate");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentResults_MeetsTarget",
                table: "AssessmentResults",
                column: "MeetsTarget");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentResults_OutcomeAssessmentId",
                table: "AssessmentResults",
                column: "OutcomeAssessmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentResults_PassesCriteria",
                table: "AssessmentResults",
                column: "PassesCriteria");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogApprovals_ApprovalStage",
                table: "CatalogApprovals",
                column: "ApprovalStage");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogApprovals_CatalogId",
                table: "CatalogApprovals",
                column: "CatalogId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogApprovals_Status",
                table: "CatalogApprovals",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogPublications_CatalogId",
                table: "CatalogPublications",
                column: "CatalogId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogPublications_CatalogId_Format_Unique",
                table: "CatalogPublications",
                columns: new[] { "CatalogId", "Format" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogPublications_Format",
                table: "CatalogPublications",
                column: "Format");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogPublications_IsActive",
                table: "CatalogPublications",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogPublications_PublicationDate",
                table: "CatalogPublications",
                column: "PublicationDate");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogPublications_Status",
                table: "CatalogPublications",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogVersions_ApprovalStatus",
                table: "CatalogVersions",
                column: "ApprovalStatus");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogVersions_CatalogId_IsCurrent_Unique",
                table: "CatalogVersions",
                column: "CatalogId",
                unique: true,
                filter: "[IsCurrent] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogVersions_CatalogId_VersionNumber_Unique",
                table: "CatalogVersions",
                columns: new[] { "CatalogId", "VersionNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CatalogVersions_CreatedDate",
                table: "CatalogVersions",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogVersions_IsCurrent",
                table: "CatalogVersions",
                column: "IsCurrent");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogVersions_IsPublished",
                table: "CatalogVersions",
                column: "IsPublished");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogVersions_PreviousVersionId",
                table: "CatalogVersions",
                column: "PreviousVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogVersions_VersionNumber",
                table: "CatalogVersions",
                column: "VersionNumber");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogVersions_VersionType",
                table: "CatalogVersions",
                column: "VersionType");

            migrationBuilder.CreateIndex(
                name: "IX_ComparisonDetails_ChangeType",
                table: "ComparisonDetails",
                column: "ChangeType");

            migrationBuilder.CreateIndex(
                name: "IX_ComparisonDetails_EntityId",
                table: "ComparisonDetails",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_ComparisonDetails_EntityType",
                table: "ComparisonDetails",
                column: "EntityType");

            migrationBuilder.CreateIndex(
                name: "IX_ComparisonDetails_Significance",
                table: "ComparisonDetails",
                column: "Significance");

            migrationBuilder.CreateIndex(
                name: "IX_ComparisonDetails_VersionComparisonId",
                table: "ComparisonDetails",
                column: "VersionComparisonId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseApprovalWorkflows_CatalogId",
                table: "CourseApprovalWorkflows",
                column: "CatalogId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseApprovalWorkflows_CourseId",
                table: "CourseApprovalWorkflows",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseApprovalWorkflows_CurrentStage",
                table: "CourseApprovalWorkflows",
                column: "CurrentStage");

            migrationBuilder.CreateIndex(
                name: "IX_CourseApprovalWorkflows_Status",
                table: "CourseApprovalWorkflows",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CourseCatalogs_AcademicYear",
                table: "CourseCatalogs",
                column: "AcademicYear");

            migrationBuilder.CreateIndex(
                name: "IX_CourseCatalogs_AcademicYear_CatalogType",
                table: "CourseCatalogs",
                columns: new[] { "AcademicYear", "CatalogType" });

            migrationBuilder.CreateIndex(
                name: "IX_CourseCatalogs_BasedOnCatalogId",
                table: "CourseCatalogs",
                column: "BasedOnCatalogId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseCatalogs_Status",
                table: "CourseCatalogs",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_LearningOutcomePrerequisites_DependentId",
                table: "LearningOutcomePrerequisites",
                column: "DependentId");

            migrationBuilder.CreateIndex(
                name: "IX_LearningOutcomes_BloomsTaxonomyLevel",
                table: "LearningOutcomes",
                column: "BloomsTaxonomyLevel");

            migrationBuilder.CreateIndex(
                name: "IX_LearningOutcomes_CourseId",
                table: "LearningOutcomes",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_LearningOutcomes_DifficultyLevel",
                table: "LearningOutcomes",
                column: "DifficultyLevel");

            migrationBuilder.CreateIndex(
                name: "IX_LearningOutcomes_IsActive",
                table: "LearningOutcomes",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_LearningOutcomes_SubjectCode",
                table: "LearningOutcomes",
                column: "SubjectCode");

            migrationBuilder.CreateIndex(
                name: "IX_OutcomeAssessments_AssessmentType",
                table: "OutcomeAssessments",
                column: "AssessmentType");

            migrationBuilder.CreateIndex(
                name: "IX_OutcomeAssessments_IsActive",
                table: "OutcomeAssessments",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_OutcomeAssessments_LearningOutcomeId",
                table: "OutcomeAssessments",
                column: "LearningOutcomeId");

            migrationBuilder.CreateIndex(
                name: "IX_PublicationAccessLogs_AccessDateTime",
                table: "PublicationAccessLogs",
                column: "AccessDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_PublicationAccessLogs_AccessedBy",
                table: "PublicationAccessLogs",
                column: "AccessedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PublicationAccessLogs_AccessType",
                table: "PublicationAccessLogs",
                column: "AccessType");

            migrationBuilder.CreateIndex(
                name: "IX_PublicationAccessLogs_CatalogPublicationId",
                table: "PublicationAccessLogs",
                column: "CatalogPublicationId");

            migrationBuilder.CreateIndex(
                name: "IX_PublicationAccessLogs_IsSuccessful",
                table: "PublicationAccessLogs",
                column: "IsSuccessful");

            migrationBuilder.CreateIndex(
                name: "IX_PublicationDistributions_CatalogPublicationId",
                table: "PublicationDistributions",
                column: "CatalogPublicationId");

            migrationBuilder.CreateIndex(
                name: "IX_PublicationDistributions_Channel",
                table: "PublicationDistributions",
                column: "Channel");

            migrationBuilder.CreateIndex(
                name: "IX_PublicationDistributions_DistributionDate",
                table: "PublicationDistributions",
                column: "DistributionDate");

            migrationBuilder.CreateIndex(
                name: "IX_PublicationDistributions_Status",
                table: "PublicationDistributions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_VersionChanges_CatalogVersionId",
                table: "VersionChanges",
                column: "CatalogVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_VersionChanges_ChangeDate",
                table: "VersionChanges",
                column: "ChangeDate");

            migrationBuilder.CreateIndex(
                name: "IX_VersionChanges_ChangeType",
                table: "VersionChanges",
                column: "ChangeType");

            migrationBuilder.CreateIndex(
                name: "IX_VersionChanges_EntityId",
                table: "VersionChanges",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_VersionChanges_EntityType",
                table: "VersionChanges",
                column: "EntityType");

            migrationBuilder.CreateIndex(
                name: "IX_VersionChanges_ImpactLevel",
                table: "VersionChanges",
                column: "ImpactLevel");

            migrationBuilder.CreateIndex(
                name: "IX_VersionChanges_RequiresApproval",
                table: "VersionChanges",
                column: "RequiresApproval");

            migrationBuilder.CreateIndex(
                name: "IX_VersionComparisons_ComparisonDate",
                table: "VersionComparisons",
                column: "ComparisonDate");

            migrationBuilder.CreateIndex(
                name: "IX_VersionComparisons_ComparisonType",
                table: "VersionComparisons",
                column: "ComparisonType");

            migrationBuilder.CreateIndex(
                name: "IX_VersionComparisons_IsArchived",
                table: "VersionComparisons",
                column: "IsArchived");

            migrationBuilder.CreateIndex(
                name: "IX_VersionComparisons_Source_Target_Type_Unique",
                table: "VersionComparisons",
                columns: new[] { "SourceVersionId", "TargetVersionId", "ComparisonType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VersionComparisons_SourceVersionId",
                table: "VersionComparisons",
                column: "SourceVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_VersionComparisons_TargetVersionId",
                table: "VersionComparisons",
                column: "TargetVersionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_CourseCatalogs_CourseCatalogId",
                table: "Courses",
                column: "CourseCatalogId",
                principalTable: "CourseCatalogs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_CourseCatalogs_CourseCatalogId",
                table: "Courses");

            migrationBuilder.DropTable(
                name: "ApprovalAttachments");

            migrationBuilder.DropTable(
                name: "AssessmentResults");

            migrationBuilder.DropTable(
                name: "CatalogApprovals");

            migrationBuilder.DropTable(
                name: "ComparisonDetails");

            migrationBuilder.DropTable(
                name: "LearningOutcomePrerequisites");

            migrationBuilder.DropTable(
                name: "PublicationAccessLogs");

            migrationBuilder.DropTable(
                name: "PublicationDistributions");

            migrationBuilder.DropTable(
                name: "VersionChanges");

            migrationBuilder.DropTable(
                name: "ApprovalSteps");

            migrationBuilder.DropTable(
                name: "OutcomeAssessments");

            migrationBuilder.DropTable(
                name: "VersionComparisons");

            migrationBuilder.DropTable(
                name: "CatalogPublications");

            migrationBuilder.DropTable(
                name: "CourseApprovalWorkflows");

            migrationBuilder.DropTable(
                name: "LearningOutcomes");

            migrationBuilder.DropTable(
                name: "CatalogVersions");

            migrationBuilder.DropTable(
                name: "CourseCatalogs");

            migrationBuilder.DropIndex(
                name: "IX_Courses_CourseCatalogId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "CourseCatalogId",
                table: "Courses");
        }
    }
}
