using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zeus.Academia.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Prompt6Task3_PrerequisiteRestrictionManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CircularDependencyResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    DetectionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HasCircularDependency = table.Column<bool>(type: "bit", nullable: false),
                    DependencyPath = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    InvolvedCourses = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Severity = table.Column<int>(type: "int", nullable: true),
                    ResolutionRecommendations = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsResolved = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ResolutionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DetectionDetails = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CircularDependencyResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CircularDependencyResults_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CorequisiteRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    RuleName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    EnforcementType = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorequisiteRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CorequisiteRules_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EnrollmentRestrictions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    RestrictionType = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Priority = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    EnforcementLevel = table.Column<int>(type: "int", nullable: false),
                    RestrictionName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ViolationMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnrollmentRestrictions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnrollmentRestrictions_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PrerequisiteOverrides",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    AcademicTermId = table.Column<int>(type: "int", nullable: false),
                    OverrideType = table.Column<int>(type: "int", nullable: false),
                    OverrideScope = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    OverrideReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    RequestedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RequestedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApproverAuthority = table.Column<int>(type: "int", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Conditions = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    RequiresPeriodicReview = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ReviewFrequencyDays = table.Column<int>(type: "int", nullable: true),
                    LastReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    OverrideMetadata = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrerequisiteOverrides", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrerequisiteOverrides_AcademicTerms_AcademicTermId",
                        column: x => x.AcademicTermId,
                        principalTable: "AcademicTerms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrerequisiteOverrides_Academics_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrerequisiteOverrides_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PrerequisiteRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    RuleName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    LogicOperator = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Priority = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RuleMetadata = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ParentRuleId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrerequisiteRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrerequisiteRules_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrerequisiteRules_PrerequisiteRules_ParentRuleId",
                        column: x => x.ParentRuleId,
                        principalTable: "PrerequisiteRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PrerequisiteValidationResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    CourseOfferingId = table.Column<int>(type: "int", nullable: true),
                    AcademicTermId = table.Column<int>(type: "int", nullable: false),
                    ValidationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OverallStatus = table.Column<int>(type: "int", nullable: false),
                    CanEnroll = table.Column<bool>(type: "bit", nullable: false),
                    FailureReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ValidationNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ValidationEngineVersion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProcessingTimeMs = table.Column<int>(type: "int", nullable: true),
                    ValidationMetadata = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrerequisiteValidationResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrerequisiteValidationResults_AcademicTerms_AcademicTermId",
                        column: x => x.AcademicTermId,
                        principalTable: "AcademicTerms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrerequisiteValidationResults_Academics_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrerequisiteValidationResults_CourseOfferings_CourseOfferingId",
                        column: x => x.CourseOfferingId,
                        principalTable: "CourseOfferings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrerequisiteValidationResults_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PrerequisiteWaivers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    WaiverType = table.Column<int>(type: "int", nullable: false),
                    WaiverScope = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    WaiverReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Justification = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    RequestedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RequestedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsPermanent = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Conditions = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AcademicConsequences = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    StudentAcknowledged = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    AcknowledgmentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    WaiverMetadata = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrerequisiteWaivers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrerequisiteWaivers_Academics_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrerequisiteWaivers_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CorequisiteRequirements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CorequisiteRuleId = table.Column<int>(type: "int", nullable: false),
                    RequiredCourseId = table.Column<int>(type: "int", nullable: false),
                    EnrollmentRelationship = table.Column<int>(type: "int", nullable: false),
                    IsWaivable = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    WaiverRequiredPermission = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FailureAction = table.Column<int>(type: "int", nullable: false),
                    RequirementNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorequisiteRequirements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CorequisiteRequirements_CorequisiteRules_CorequisiteRuleId",
                        column: x => x.CorequisiteRuleId,
                        principalTable: "CorequisiteRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CorequisiteRequirements_Courses_RequiredCourseId",
                        column: x => x.RequiredCourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClassStandingRestrictions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnrollmentRestrictionId = table.Column<int>(type: "int", nullable: false),
                    RequiredClassStanding = table.Column<int>(type: "int", nullable: false),
                    IsIncluded = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    MinimumCreditHours = table.Column<int>(type: "int", nullable: true),
                    MaximumCreditHours = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassStandingRestrictions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassStandingRestrictions_EnrollmentRestrictions_EnrollmentRestrictionId",
                        column: x => x.EnrollmentRestrictionId,
                        principalTable: "EnrollmentRestrictions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MajorRestrictions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnrollmentRestrictionId = table.Column<int>(type: "int", nullable: false),
                    RequiredMajorCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    MajorType = table.Column<int>(type: "int", nullable: false),
                    IsIncluded = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    MinimumMajorProgress = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MajorRestrictions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MajorRestrictions_EnrollmentRestrictions_EnrollmentRestrictionId",
                        column: x => x.EnrollmentRestrictionId,
                        principalTable: "EnrollmentRestrictions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PermissionRestrictions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnrollmentRestrictionId = table.Column<int>(type: "int", nullable: false),
                    RequiredPermission = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PermissionLevel = table.Column<int>(type: "int", nullable: false),
                    RequiresDocumentation = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DocumentationRequirements = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PermissionValidityDays = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionRestrictions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PermissionRestrictions_EnrollmentRestrictions_EnrollmentRestrictionId",
                        column: x => x.EnrollmentRestrictionId,
                        principalTable: "EnrollmentRestrictions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OverrideApprovalSteps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrerequisiteOverrideId = table.Column<int>(type: "int", nullable: false),
                    StepNumber = table.Column<int>(type: "int", nullable: false),
                    StepName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RequiredAuthority = table.Column<int>(type: "int", nullable: false),
                    AssignedTo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApproverComments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CanDelegate = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DelegatedTo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OverrideApprovalSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OverrideApprovalSteps_PrerequisiteOverrides_PrerequisiteOverrideId",
                        column: x => x.PrerequisiteOverrideId,
                        principalTable: "PrerequisiteOverrides",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OverrideAuditEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrerequisiteOverrideId = table.Column<int>(type: "int", nullable: false),
                    AuditDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PerformedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ActionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ActionDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    OldValues = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    NewValues = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OverrideAuditEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OverrideAuditEntries_PrerequisiteOverrides_PrerequisiteOverrideId",
                        column: x => x.PrerequisiteOverrideId,
                        principalTable: "PrerequisiteOverrides",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OverrideDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrerequisiteOverrideId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    StoragePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UploadedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UploadedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OverrideDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OverrideDocuments_PrerequisiteOverrides_PrerequisiteOverrideId",
                        column: x => x.PrerequisiteOverrideId,
                        principalTable: "PrerequisiteOverrides",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OverrideRuleMappings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrerequisiteOverrideId = table.Column<int>(type: "int", nullable: false),
                    PrerequisiteRuleId = table.Column<int>(type: "int", nullable: true),
                    CorequisiteRuleId = table.Column<int>(type: "int", nullable: true),
                    EnrollmentRestrictionId = table.Column<int>(type: "int", nullable: true),
                    IsCompleteOverride = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    PartialOverrideConditions = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OverrideRuleMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OverrideRuleMappings_CorequisiteRules_CorequisiteRuleId",
                        column: x => x.CorequisiteRuleId,
                        principalTable: "CorequisiteRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OverrideRuleMappings_EnrollmentRestrictions_EnrollmentRestrictionId",
                        column: x => x.EnrollmentRestrictionId,
                        principalTable: "EnrollmentRestrictions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OverrideRuleMappings_PrerequisiteOverrides_PrerequisiteOverrideId",
                        column: x => x.PrerequisiteOverrideId,
                        principalTable: "PrerequisiteOverrides",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OverrideRuleMappings_PrerequisiteRules_PrerequisiteRuleId",
                        column: x => x.PrerequisiteRuleId,
                        principalTable: "PrerequisiteRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PrerequisiteRequirements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrerequisiteRuleId = table.Column<int>(type: "int", nullable: false),
                    RequirementType = table.Column<int>(type: "int", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    SequenceOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    RequiredCourseId = table.Column<int>(type: "int", nullable: true),
                    MinimumGrade = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    MustBeCompleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    MinimumCreditHours = table.Column<int>(type: "int", nullable: true),
                    CreditHoursSubjectArea = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    RequiredClassStanding = table.Column<int>(type: "int", nullable: true),
                    MinimumGPA = table.Column<decimal>(type: "decimal(4,3)", nullable: true),
                    GPAScope = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RequiredPermission = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PermissionLevel = table.Column<int>(type: "int", nullable: true),
                    TestName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MinimumTestScore = table.Column<int>(type: "int", nullable: true),
                    TestScoreValidityMonths = table.Column<int>(type: "int", nullable: true),
                    AlternativeSatisfactionMethods = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RequirementNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrerequisiteRequirements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrerequisiteRequirements_Courses_RequiredCourseId",
                        column: x => x.RequiredCourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrerequisiteRequirements_PrerequisiteRules_PrerequisiteRuleId",
                        column: x => x.PrerequisiteRuleId,
                        principalTable: "PrerequisiteRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RuleValidationHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrerequisiteRuleId = table.Column<int>(type: "int", nullable: false),
                    ValidationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsValid = table.Column<bool>(type: "bit", nullable: false),
                    ValidationErrors = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ValidationDetails = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RuleValidationHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RuleValidationHistory_PrerequisiteRules_PrerequisiteRuleId",
                        column: x => x.PrerequisiteRuleId,
                        principalTable: "PrerequisiteRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CorequisiteCheckResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ValidationResultId = table.Column<int>(type: "int", nullable: false),
                    CorequisiteRuleId = table.Column<int>(type: "int", nullable: false),
                    CheckStatus = table.Column<int>(type: "int", nullable: false),
                    IsSatisfied = table.Column<bool>(type: "bit", nullable: false),
                    FailureReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EnforcementAction = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RequiredCorequisites = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EnrolledCorequisites = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CheckDetails = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorequisiteCheckResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CorequisiteCheckResults_CorequisiteRules_CorequisiteRuleId",
                        column: x => x.CorequisiteRuleId,
                        principalTable: "CorequisiteRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CorequisiteCheckResults_PrerequisiteValidationResults_ValidationResultId",
                        column: x => x.ValidationResultId,
                        principalTable: "PrerequisiteValidationResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrerequisiteCheckResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ValidationResultId = table.Column<int>(type: "int", nullable: false),
                    PrerequisiteRuleId = table.Column<int>(type: "int", nullable: false),
                    CheckStatus = table.Column<int>(type: "int", nullable: false),
                    IsSatisfied = table.Column<bool>(type: "bit", nullable: false),
                    FailureReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SatisfactionMethod = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SatisfactionPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    SatisfyingCourses = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SatisfyingGrades = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SatisfactionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CheckDetails = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AlternativeMethods = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrerequisiteCheckResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrerequisiteCheckResults_PrerequisiteRules_PrerequisiteRuleId",
                        column: x => x.PrerequisiteRuleId,
                        principalTable: "PrerequisiteRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrerequisiteCheckResults_PrerequisiteValidationResults_ValidationResultId",
                        column: x => x.ValidationResultId,
                        principalTable: "PrerequisiteValidationResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RestrictionCheckResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ValidationResultId = table.Column<int>(type: "int", nullable: false),
                    EnrollmentRestrictionId = table.Column<int>(type: "int", nullable: false),
                    CheckStatus = table.Column<int>(type: "int", nullable: false),
                    IsViolated = table.Column<bool>(type: "bit", nullable: false),
                    ViolationReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EnforcementAction = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ViolationSeverity = table.Column<int>(type: "int", nullable: false),
                    CanBeOverridden = table.Column<bool>(type: "bit", nullable: false),
                    OverridePermissionRequired = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CheckDetails = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestrictionCheckResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RestrictionCheckResults_EnrollmentRestrictions_EnrollmentRestrictionId",
                        column: x => x.EnrollmentRestrictionId,
                        principalTable: "EnrollmentRestrictions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RestrictionCheckResults_PrerequisiteValidationResults_ValidationResultId",
                        column: x => x.ValidationResultId,
                        principalTable: "PrerequisiteValidationResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ValidationResultOverrides",
                columns: table => new
                {
                    AppliedOverridesId = table.Column<int>(type: "int", nullable: false),
                    PrerequisiteValidationResultId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValidationResultOverrides", x => new { x.AppliedOverridesId, x.PrerequisiteValidationResultId });
                    table.ForeignKey(
                        name: "FK_ValidationResultOverrides_PrerequisiteOverrides_AppliedOverridesId",
                        column: x => x.AppliedOverridesId,
                        principalTable: "PrerequisiteOverrides",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ValidationResultOverrides_PrerequisiteValidationResults_PrerequisiteValidationResultId",
                        column: x => x.PrerequisiteValidationResultId,
                        principalTable: "PrerequisiteValidationResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ValidationResultWaivers",
                columns: table => new
                {
                    AppliedWaiversId = table.Column<int>(type: "int", nullable: false),
                    PrerequisiteValidationResultId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValidationResultWaivers", x => new { x.AppliedWaiversId, x.PrerequisiteValidationResultId });
                    table.ForeignKey(
                        name: "FK_ValidationResultWaivers_PrerequisiteValidationResults_PrerequisiteValidationResultId",
                        column: x => x.PrerequisiteValidationResultId,
                        principalTable: "PrerequisiteValidationResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ValidationResultWaivers_PrerequisiteWaivers_AppliedWaiversId",
                        column: x => x.AppliedWaiversId,
                        principalTable: "PrerequisiteWaivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WaiverDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrerequisiteWaiverId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    StoragePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UploadedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UploadedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaiverDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WaiverDocuments_PrerequisiteWaivers_PrerequisiteWaiverId",
                        column: x => x.PrerequisiteWaiverId,
                        principalTable: "PrerequisiteWaivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WaiverRuleMappings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrerequisiteWaiverId = table.Column<int>(type: "int", nullable: false),
                    PrerequisiteRuleId = table.Column<int>(type: "int", nullable: true),
                    CorequisiteRuleId = table.Column<int>(type: "int", nullable: true),
                    IsCompleteWaiver = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    PartialWaiverConditions = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaiverRuleMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WaiverRuleMappings_CorequisiteRules_CorequisiteRuleId",
                        column: x => x.CorequisiteRuleId,
                        principalTable: "CorequisiteRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WaiverRuleMappings_PrerequisiteRules_PrerequisiteRuleId",
                        column: x => x.PrerequisiteRuleId,
                        principalTable: "PrerequisiteRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WaiverRuleMappings_PrerequisiteWaivers_PrerequisiteWaiverId",
                        column: x => x.PrerequisiteWaiverId,
                        principalTable: "PrerequisiteWaivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequirementCheckResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrerequisiteCheckResultId = table.Column<int>(type: "int", nullable: false),
                    PrerequisiteRequirementId = table.Column<int>(type: "int", nullable: false),
                    IsSatisfied = table.Column<bool>(type: "bit", nullable: false),
                    FailureReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ActualValue = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RequiredValue = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CheckDetails = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SatisfactionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequirementCheckResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequirementCheckResults_PrerequisiteCheckResults_PrerequisiteCheckResultId",
                        column: x => x.PrerequisiteCheckResultId,
                        principalTable: "PrerequisiteCheckResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequirementCheckResults_PrerequisiteRequirements_PrerequisiteRequirementId",
                        column: x => x.PrerequisiteRequirementId,
                        principalTable: "PrerequisiteRequirements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CircularDependencyResults_Course_Dependency_Resolved",
                table: "CircularDependencyResults",
                columns: new[] { "CourseId", "HasCircularDependency", "IsResolved" });

            migrationBuilder.CreateIndex(
                name: "IX_CircularDependencyResults_CourseId",
                table: "CircularDependencyResults",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CircularDependencyResults_DetectionDate",
                table: "CircularDependencyResults",
                column: "DetectionDate");

            migrationBuilder.CreateIndex(
                name: "IX_CircularDependencyResults_HasDependency",
                table: "CircularDependencyResults",
                column: "HasCircularDependency");

            migrationBuilder.CreateIndex(
                name: "IX_CircularDependencyResults_IsResolved",
                table: "CircularDependencyResults",
                column: "IsResolved");

            migrationBuilder.CreateIndex(
                name: "IX_ClassStandingRestrictions_ClassStanding",
                table: "ClassStandingRestrictions",
                column: "RequiredClassStanding");

            migrationBuilder.CreateIndex(
                name: "IX_ClassStandingRestrictions_RestrictionId",
                table: "ClassStandingRestrictions",
                column: "EnrollmentRestrictionId");

            migrationBuilder.CreateIndex(
                name: "IX_CorequisiteCheckResults_IsSatisfied",
                table: "CorequisiteCheckResults",
                column: "IsSatisfied");

            migrationBuilder.CreateIndex(
                name: "IX_CorequisiteCheckResults_RuleId",
                table: "CorequisiteCheckResults",
                column: "CorequisiteRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_CorequisiteCheckResults_Status",
                table: "CorequisiteCheckResults",
                column: "CheckStatus");

            migrationBuilder.CreateIndex(
                name: "IX_CorequisiteCheckResults_ValidationResultId",
                table: "CorequisiteCheckResults",
                column: "ValidationResultId");

            migrationBuilder.CreateIndex(
                name: "IX_CorequisiteRequirements_RequiredCourseId",
                table: "CorequisiteRequirements",
                column: "RequiredCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CorequisiteRequirements_Rule_Course",
                table: "CorequisiteRequirements",
                columns: new[] { "CorequisiteRuleId", "RequiredCourseId" });

            migrationBuilder.CreateIndex(
                name: "IX_CorequisiteRequirements_RuleId",
                table: "CorequisiteRequirements",
                column: "CorequisiteRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_CorequisiteRules_Course_Active",
                table: "CorequisiteRules",
                columns: new[] { "CourseId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_CorequisiteRules_CourseId",
                table: "CorequisiteRules",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CorequisiteRules_IsActive",
                table: "CorequisiteRules",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_EnrollmentRestrictions_Course_Active_Priority",
                table: "EnrollmentRestrictions",
                columns: new[] { "CourseId", "IsActive", "Priority" });

            migrationBuilder.CreateIndex(
                name: "IX_EnrollmentRestrictions_CourseId",
                table: "EnrollmentRestrictions",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_EnrollmentRestrictions_IsActive",
                table: "EnrollmentRestrictions",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_EnrollmentRestrictions_Type",
                table: "EnrollmentRestrictions",
                column: "RestrictionType");

            migrationBuilder.CreateIndex(
                name: "IX_MajorRestrictions_MajorCode",
                table: "MajorRestrictions",
                column: "RequiredMajorCode");

            migrationBuilder.CreateIndex(
                name: "IX_MajorRestrictions_Restriction_Major",
                table: "MajorRestrictions",
                columns: new[] { "EnrollmentRestrictionId", "RequiredMajorCode" });

            migrationBuilder.CreateIndex(
                name: "IX_MajorRestrictions_RestrictionId",
                table: "MajorRestrictions",
                column: "EnrollmentRestrictionId");

            migrationBuilder.CreateIndex(
                name: "IX_OverrideApprovalSteps_DueDate",
                table: "OverrideApprovalSteps",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_OverrideApprovalSteps_Override_Step",
                table: "OverrideApprovalSteps",
                columns: new[] { "PrerequisiteOverrideId", "StepNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_OverrideApprovalSteps_OverrideId",
                table: "OverrideApprovalSteps",
                column: "PrerequisiteOverrideId");

            migrationBuilder.CreateIndex(
                name: "IX_OverrideApprovalSteps_Status",
                table: "OverrideApprovalSteps",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_OverrideAuditEntries_ActionType",
                table: "OverrideAuditEntries",
                column: "ActionType");

            migrationBuilder.CreateIndex(
                name: "IX_OverrideAuditEntries_AuditDate",
                table: "OverrideAuditEntries",
                column: "AuditDate");

            migrationBuilder.CreateIndex(
                name: "IX_OverrideAuditEntries_OverrideId",
                table: "OverrideAuditEntries",
                column: "PrerequisiteOverrideId");

            migrationBuilder.CreateIndex(
                name: "IX_OverrideAuditEntries_PerformedBy",
                table: "OverrideAuditEntries",
                column: "PerformedBy");

            migrationBuilder.CreateIndex(
                name: "IX_OverrideDocuments_OverrideId",
                table: "OverrideDocuments",
                column: "PrerequisiteOverrideId");

            migrationBuilder.CreateIndex(
                name: "IX_OverrideDocuments_UploadedBy",
                table: "OverrideDocuments",
                column: "UploadedBy");

            migrationBuilder.CreateIndex(
                name: "IX_OverrideDocuments_UploadedDate",
                table: "OverrideDocuments",
                column: "UploadedDate");

            migrationBuilder.CreateIndex(
                name: "IX_OverrideRuleMappings_CorequisiteRuleId",
                table: "OverrideRuleMappings",
                column: "CorequisiteRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_OverrideRuleMappings_OverrideId",
                table: "OverrideRuleMappings",
                column: "PrerequisiteOverrideId");

            migrationBuilder.CreateIndex(
                name: "IX_OverrideRuleMappings_PrerequisiteRuleId",
                table: "OverrideRuleMappings",
                column: "PrerequisiteRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_OverrideRuleMappings_RestrictionId",
                table: "OverrideRuleMappings",
                column: "EnrollmentRestrictionId");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionRestrictions_Permission",
                table: "PermissionRestrictions",
                column: "RequiredPermission");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionRestrictions_RestrictionId",
                table: "PermissionRestrictions",
                column: "EnrollmentRestrictionId");

            migrationBuilder.CreateIndex(
                name: "IX_PrerequisiteCheckResults_IsSatisfied",
                table: "PrerequisiteCheckResults",
                column: "IsSatisfied");

            migrationBuilder.CreateIndex(
                name: "IX_PrerequisiteCheckResults_RuleId",
                table: "PrerequisiteCheckResults",
                column: "PrerequisiteRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_PrerequisiteCheckResults_Status",
                table: "PrerequisiteCheckResults",
                column: "CheckStatus");

            migrationBuilder.CreateIndex(
                name: "IX_PrerequisiteCheckResults_ValidationResultId",
                table: "PrerequisiteCheckResults",
                column: "ValidationResultId");

            migrationBuilder.CreateIndex(
                name: "IX_PrerequisiteOverrides_AcademicTermId",
                table: "PrerequisiteOverrides",
                column: "AcademicTermId");

            migrationBuilder.CreateIndex(
                name: "IX_PrerequisiteOverrides_CourseId",
                table: "PrerequisiteOverrides",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_PrerequisiteOverrides_ExpirationDate",
                table: "PrerequisiteOverrides",
                column: "ExpirationDate");

            migrationBuilder.CreateIndex(
                name: "IX_PrerequisiteOverrides_IsActive",
                table: "PrerequisiteOverrides",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_PrerequisiteOverrides_RequestedDate",
                table: "PrerequisiteOverrides",
                column: "RequestedDate");

            migrationBuilder.CreateIndex(
                name: "IX_PrerequisiteOverrides_Status",
                table: "PrerequisiteOverrides",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PrerequisiteOverrides_Student_Active",
                table: "PrerequisiteOverrides",
                columns: new[] { "StudentId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_PrerequisiteOverrides_Student_Course_Term",
                table: "PrerequisiteOverrides",
                columns: new[] { "StudentId", "CourseId", "AcademicTermId" });

            migrationBuilder.CreateIndex(
                name: "IX_PrerequisiteOverrides_StudentId",
                table: "PrerequisiteOverrides",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_PrerequisiteOverrides_Type",
                table: "PrerequisiteOverrides",
                column: "OverrideType");

            migrationBuilder.CreateIndex(
                name: "IX_PrerequisiteRequirements_RequiredCourseId",
                table: "PrerequisiteRequirements",
                column: "RequiredCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_PrerequisiteRequirements_Rule_Sequence",
                table: "PrerequisiteRequirements",
                columns: new[] { "PrerequisiteRuleId", "SequenceOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_PrerequisiteRequirements_RuleId",
                table: "PrerequisiteRequirements",
                column: "PrerequisiteRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_PrerequisiteRequirements_Type",
                table: "PrerequisiteRequirements",
                column: "RequirementType");

            migrationBuilder.CreateIndex(
                name: "IX_PrerequisiteRules_Course_Active_Priority",
                table: "PrerequisiteRules",
                columns: new[] { "CourseId", "IsActive", "Priority" });

            migrationBuilder.CreateIndex(
                name: "IX_PrerequisiteRules_CourseId",
                table: "PrerequisiteRules",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_PrerequisiteRules_IsActive",
                table: "PrerequisiteRules",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_PrerequisiteRules_ParentRuleId",
                table: "PrerequisiteRules",
                column: "ParentRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_PrerequisiteRules_Priority",
                table: "PrerequisiteRules",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_PrerequisiteValidationResults_AcademicTermId",
                table: "PrerequisiteValidationResults",
                column: "AcademicTermId");

            migrationBuilder.CreateIndex(
                name: "IX_PrerequisiteValidationResults_CourseId",
                table: "PrerequisiteValidationResults",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_PrerequisiteValidationResults_CourseOfferingId",
                table: "PrerequisiteValidationResults",
                column: "CourseOfferingId");

            migrationBuilder.CreateIndex(
                name: "IX_PrerequisiteValidationResults_IsCurrent",
                table: "PrerequisiteValidationResults",
                column: "IsCurrent");

            migrationBuilder.CreateIndex(
                name: "IX_PrerequisiteValidationResults_Status",
                table: "PrerequisiteValidationResults",
                column: "OverallStatus");

            migrationBuilder.CreateIndex(
                name: "IX_PrerequisiteValidationResults_Student_Course_Term",
                table: "PrerequisiteValidationResults",
                columns: new[] { "StudentId", "CourseId", "AcademicTermId" });

            migrationBuilder.CreateIndex(
                name: "IX_PrerequisiteValidationResults_Student_Current",
                table: "PrerequisiteValidationResults",
                columns: new[] { "StudentId", "IsCurrent" });

            migrationBuilder.CreateIndex(
                name: "IX_PrerequisiteValidationResults_StudentId",
                table: "PrerequisiteValidationResults",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_PrerequisiteValidationResults_ValidationDate",
                table: "PrerequisiteValidationResults",
                column: "ValidationDate");

            migrationBuilder.CreateIndex(
                name: "IX_PrerequisiteWaivers_CourseId",
                table: "PrerequisiteWaivers",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_PrerequisiteWaivers_ExpirationDate",
                table: "PrerequisiteWaivers",
                column: "ExpirationDate");

            migrationBuilder.CreateIndex(
                name: "IX_PrerequisiteWaivers_IsActive",
                table: "PrerequisiteWaivers",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_PrerequisiteWaivers_IsPermanent",
                table: "PrerequisiteWaivers",
                column: "IsPermanent");

            migrationBuilder.CreateIndex(
                name: "IX_PrerequisiteWaivers_RequestedDate",
                table: "PrerequisiteWaivers",
                column: "RequestedDate");

            migrationBuilder.CreateIndex(
                name: "IX_PrerequisiteWaivers_Status",
                table: "PrerequisiteWaivers",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PrerequisiteWaivers_Student_Active",
                table: "PrerequisiteWaivers",
                columns: new[] { "StudentId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_PrerequisiteWaivers_Student_Course",
                table: "PrerequisiteWaivers",
                columns: new[] { "StudentId", "CourseId" });

            migrationBuilder.CreateIndex(
                name: "IX_PrerequisiteWaivers_StudentId",
                table: "PrerequisiteWaivers",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_PrerequisiteWaivers_Type",
                table: "PrerequisiteWaivers",
                column: "WaiverType");

            migrationBuilder.CreateIndex(
                name: "IX_RequirementCheckResults_CheckResultId",
                table: "RequirementCheckResults",
                column: "PrerequisiteCheckResultId");

            migrationBuilder.CreateIndex(
                name: "IX_RequirementCheckResults_IsSatisfied",
                table: "RequirementCheckResults",
                column: "IsSatisfied");

            migrationBuilder.CreateIndex(
                name: "IX_RequirementCheckResults_RequirementId",
                table: "RequirementCheckResults",
                column: "PrerequisiteRequirementId");

            migrationBuilder.CreateIndex(
                name: "IX_RestrictionCheckResults_IsViolated",
                table: "RestrictionCheckResults",
                column: "IsViolated");

            migrationBuilder.CreateIndex(
                name: "IX_RestrictionCheckResults_RestrictionId",
                table: "RestrictionCheckResults",
                column: "EnrollmentRestrictionId");

            migrationBuilder.CreateIndex(
                name: "IX_RestrictionCheckResults_Severity",
                table: "RestrictionCheckResults",
                column: "ViolationSeverity");

            migrationBuilder.CreateIndex(
                name: "IX_RestrictionCheckResults_Status",
                table: "RestrictionCheckResults",
                column: "CheckStatus");

            migrationBuilder.CreateIndex(
                name: "IX_RestrictionCheckResults_ValidationResultId",
                table: "RestrictionCheckResults",
                column: "ValidationResultId");

            migrationBuilder.CreateIndex(
                name: "IX_RuleValidationHistory_Rule_Date",
                table: "RuleValidationHistory",
                columns: new[] { "PrerequisiteRuleId", "ValidationDate" });

            migrationBuilder.CreateIndex(
                name: "IX_RuleValidationHistory_RuleId",
                table: "RuleValidationHistory",
                column: "PrerequisiteRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_RuleValidationHistory_ValidationDate",
                table: "RuleValidationHistory",
                column: "ValidationDate");

            migrationBuilder.CreateIndex(
                name: "IX_ValidationResultOverrides_PrerequisiteValidationResultId",
                table: "ValidationResultOverrides",
                column: "PrerequisiteValidationResultId");

            migrationBuilder.CreateIndex(
                name: "IX_ValidationResultWaivers_PrerequisiteValidationResultId",
                table: "ValidationResultWaivers",
                column: "PrerequisiteValidationResultId");

            migrationBuilder.CreateIndex(
                name: "IX_WaiverDocuments_UploadedBy",
                table: "WaiverDocuments",
                column: "UploadedBy");

            migrationBuilder.CreateIndex(
                name: "IX_WaiverDocuments_UploadedDate",
                table: "WaiverDocuments",
                column: "UploadedDate");

            migrationBuilder.CreateIndex(
                name: "IX_WaiverDocuments_WaiverId",
                table: "WaiverDocuments",
                column: "PrerequisiteWaiverId");

            migrationBuilder.CreateIndex(
                name: "IX_WaiverRuleMappings_CorequisiteRuleId",
                table: "WaiverRuleMappings",
                column: "CorequisiteRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_WaiverRuleMappings_PrerequisiteRuleId",
                table: "WaiverRuleMappings",
                column: "PrerequisiteRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_WaiverRuleMappings_WaiverId",
                table: "WaiverRuleMappings",
                column: "PrerequisiteWaiverId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CircularDependencyResults");

            migrationBuilder.DropTable(
                name: "ClassStandingRestrictions");

            migrationBuilder.DropTable(
                name: "CorequisiteCheckResults");

            migrationBuilder.DropTable(
                name: "CorequisiteRequirements");

            migrationBuilder.DropTable(
                name: "MajorRestrictions");

            migrationBuilder.DropTable(
                name: "OverrideApprovalSteps");

            migrationBuilder.DropTable(
                name: "OverrideAuditEntries");

            migrationBuilder.DropTable(
                name: "OverrideDocuments");

            migrationBuilder.DropTable(
                name: "OverrideRuleMappings");

            migrationBuilder.DropTable(
                name: "PermissionRestrictions");

            migrationBuilder.DropTable(
                name: "RequirementCheckResults");

            migrationBuilder.DropTable(
                name: "RestrictionCheckResults");

            migrationBuilder.DropTable(
                name: "RuleValidationHistory");

            migrationBuilder.DropTable(
                name: "ValidationResultOverrides");

            migrationBuilder.DropTable(
                name: "ValidationResultWaivers");

            migrationBuilder.DropTable(
                name: "WaiverDocuments");

            migrationBuilder.DropTable(
                name: "WaiverRuleMappings");

            migrationBuilder.DropTable(
                name: "PrerequisiteCheckResults");

            migrationBuilder.DropTable(
                name: "PrerequisiteRequirements");

            migrationBuilder.DropTable(
                name: "EnrollmentRestrictions");

            migrationBuilder.DropTable(
                name: "PrerequisiteOverrides");

            migrationBuilder.DropTable(
                name: "CorequisiteRules");

            migrationBuilder.DropTable(
                name: "PrerequisiteWaivers");

            migrationBuilder.DropTable(
                name: "PrerequisiteValidationResults");

            migrationBuilder.DropTable(
                name: "PrerequisiteRules");
        }
    }
}
