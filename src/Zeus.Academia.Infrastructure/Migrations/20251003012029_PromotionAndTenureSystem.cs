using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zeus.Academia.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PromotionAndTenureSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PromotionCommittees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommitteeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CommitteeType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AcademicYear = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    DepartmentCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CollegeCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    ChairEmpNr = table.Column<int>(type: "int", nullable: false),
                    FormationDate = table.Column<DateTime>(type: "date", nullable: false),
                    DissolutionDate = table.Column<DateTime>(type: "date", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    MinimumMembers = table.Column<int>(type: "int", nullable: false),
                    MaximumMembers = table.Column<int>(type: "int", nullable: false),
                    QuorumRequirement = table.Column<int>(type: "int", nullable: false),
                    VotingThreshold = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    CommitteeScope = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MembershipRequirements = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MeetingSchedule = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NextMeetingDate = table.Column<DateTime>(type: "date", nullable: true),
                    ConfidentialityGuidelines = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CurrentWorkload = table.Column<int>(type: "int", nullable: false),
                    MaxWorkloadCapacity = table.Column<int>(type: "int", nullable: false),
                    ContactInformation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionCommittees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromotionCommittees_Academics_ChairEmpNr",
                        column: x => x.ChairEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AcademicRanks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcademicEmpNr = table.Column<int>(type: "int", nullable: false),
                    RankLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RankCategory = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TenureStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "date", nullable: false),
                    EndDate = table.Column<DateTime>(type: "date", nullable: true),
                    IsCurrentRank = table.Column<bool>(type: "bit", nullable: false),
                    AssignmentReason = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PromotionCommitteeId = table.Column<int>(type: "int", nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "date", nullable: true),
                    MinimumYearsInRank = table.Column<int>(type: "int", nullable: true),
                    AnnualSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SalaryGrade = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DepartmentCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CollegeCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    AppointmentPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    IsSabbaticalEligible = table.Column<bool>(type: "bit", nullable: false),
                    VotingRights = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    RankQualifications = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicRanks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcademicRanks_Academics_AcademicEmpNr",
                        column: x => x.AcademicEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AcademicRanks_PromotionCommittees_PromotionCommitteeId",
                        column: x => x.PromotionCommitteeId,
                        principalTable: "PromotionCommittees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PromotionApplications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcademicEmpNr = table.Column<int>(type: "int", nullable: false),
                    CurrentRank = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RequestedRank = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ApplicationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AcademicYear = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    ApplicationDate = table.Column<DateTime>(type: "date", nullable: false),
                    ApplicationDeadline = table.Column<DateTime>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    WorkflowStage = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PromotionCommitteeId = table.Column<int>(type: "int", nullable: true),
                    ExpectedDecisionDate = table.Column<DateTime>(type: "date", nullable: true),
                    DecisionDate = table.Column<DateTime>(type: "date", nullable: true),
                    FinalDecision = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    DecisionRationale = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CandidateStatement = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    TeachingPortfolio = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ResearchPortfolio = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ServicePortfolio = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ExternalReferees = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    YearsInCurrentRank = table.Column<int>(type: "int", nullable: false),
                    PublicationsCount = table.Column<int>(type: "int", nullable: false),
                    TeachingEvaluationAverage = table.Column<decimal>(type: "decimal(4,2)", nullable: true),
                    GrantFundingAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsSeekingTenure = table.Column<bool>(type: "bit", nullable: false),
                    MentorEmpNr = table.Column<int>(type: "int", nullable: true),
                    DepartmentRecommendation = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    CollegeRecommendation = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    UniversityRecommendation = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    NotificationPreferences = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AdditionalNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromotionApplications_Academics_AcademicEmpNr",
                        column: x => x.AcademicEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PromotionApplications_Academics_MentorEmpNr",
                        column: x => x.MentorEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr");
                    table.ForeignKey(
                        name: "FK_PromotionApplications_PromotionCommittees_PromotionCommitteeId",
                        column: x => x.PromotionCommitteeId,
                        principalTable: "PromotionCommittees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PromotionCommitteeMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PromotionCommitteeId = table.Column<int>(type: "int", nullable: false),
                    EmpNr = table.Column<int>(type: "int", nullable: false),
                    MemberRole = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AppointmentDate = table.Column<DateTime>(type: "date", nullable: false),
                    TermEndDate = table.Column<DateTime>(type: "date", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    HasVotingPrivileges = table.Column<bool>(type: "bit", nullable: false),
                    ExpertiseAreas = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DepartmentCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CollegeCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    IsExternalMember = table.Column<bool>(type: "bit", nullable: false),
                    ExternalInstitution = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MemberRank = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ConflictOfInterestDeclaration = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TrainingCompletionDate = table.Column<DateTime>(type: "date", nullable: true),
                    ConfidentialityAgreementSigned = table.Column<bool>(type: "bit", nullable: false),
                    AttendanceRate = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    MeetingsAttended = table.Column<int>(type: "int", nullable: false),
                    TotalMeetingsScheduled = table.Column<int>(type: "int", nullable: false),
                    AssignedCases = table.Column<int>(type: "int", nullable: false),
                    CompletedCases = table.Column<int>(type: "int", nullable: false),
                    PreferredContactMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EmergencyContact = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ServiceNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionCommitteeMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromotionCommitteeMembers_Academics_EmpNr",
                        column: x => x.EmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PromotionCommitteeMembers_PromotionCommittees_PromotionCommitteeId",
                        column: x => x.PromotionCommitteeId,
                        principalTable: "PromotionCommittees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PromotionWorkflowSteps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PromotionApplicationId = table.Column<int>(type: "int", nullable: false),
                    StepOrder = table.Column<int>(type: "int", nullable: false),
                    StepName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StepDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StepType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StartDate = table.Column<DateTime>(type: "date", nullable: true),
                    CompletionDate = table.Column<DateTime>(type: "date", nullable: true),
                    DueDate = table.Column<DateTime>(type: "date", nullable: true),
                    EstimatedDurationDays = table.Column<int>(type: "int", nullable: false),
                    ActualDurationDays = table.Column<int>(type: "int", nullable: true),
                    ResponsibleParty = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PromotionCommitteeId = table.Column<int>(type: "int", nullable: true),
                    AssignedReviewerEmpNr = table.Column<int>(type: "int", nullable: true),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    CanRunInParallel = table.Column<bool>(type: "bit", nullable: false),
                    PrerequisiteSteps = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RequiredDocuments = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ExpectedDeliverables = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ApprovalCriteria = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Outcome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OutcomeDate = table.Column<DateTime>(type: "date", nullable: true),
                    VoteResult = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Feedback = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Recommendations = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RequiresExternalReview = table.Column<bool>(type: "bit", nullable: false),
                    ExternalReviewerInfo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NotificationRequirements = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EscalationPath = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CompletionPercentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    StepMetadata = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionWorkflowSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromotionWorkflowSteps_Academics_AssignedReviewerEmpNr",
                        column: x => x.AssignedReviewerEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PromotionWorkflowSteps_PromotionApplications_PromotionApplicationId",
                        column: x => x.PromotionApplicationId,
                        principalTable: "PromotionApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PromotionWorkflowSteps_PromotionCommittees_PromotionCommitteeId",
                        column: x => x.PromotionCommitteeId,
                        principalTable: "PromotionCommittees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "TenureTracks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcademicEmpNr = table.Column<int>(type: "int", nullable: false),
                    TenureTrackStartDate = table.Column<DateTime>(type: "date", nullable: false),
                    ExpectedTenureDate = table.Column<DateTime>(type: "date", nullable: false),
                    ActualTenureDate = table.Column<DateTime>(type: "date", nullable: true),
                    TenureStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ClockStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    YearsOnTrack = table.Column<decimal>(type: "decimal(4,2)", nullable: false),
                    MaxYearsAllowed = table.Column<decimal>(type: "decimal(4,2)", nullable: false),
                    TenureApplicationId = table.Column<int>(type: "int", nullable: true),
                    FirstYearReviewDate = table.Column<DateTime>(type: "date", nullable: true),
                    FirstYearReviewOutcome = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    ThirdYearReviewDate = table.Column<DateTime>(type: "date", nullable: true),
                    ThirdYearReviewOutcome = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    SixthYearReviewDate = table.Column<DateTime>(type: "date", nullable: true),
                    SixthYearReviewOutcome = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    MentorEmpNr = table.Column<int>(type: "int", nullable: true),
                    ClockExtensionYears = table.Column<decimal>(type: "decimal(4,2)", nullable: true),
                    ExtensionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ProbationaryEndDate = table.Column<DateTime>(type: "date", nullable: true),
                    IsEligibleForEarlyTenure = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DepartmentTenureRecommendation = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    CollegeTenureRecommendation = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    UniversityTenureRecommendation = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    FinalTenureDecision = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    TenureDecisionRationale = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    TenureNotificationDate = table.Column<DateTime>(type: "date", nullable: true),
                    SpecialConditions = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenureTracks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenureTracks_Academics_AcademicEmpNr",
                        column: x => x.AcademicEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenureTracks_Academics_MentorEmpNr",
                        column: x => x.MentorEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr");
                    table.ForeignKey(
                        name: "FK_TenureTracks_PromotionApplications_TenureApplicationId",
                        column: x => x.TenureApplicationId,
                        principalTable: "PromotionApplications",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PromotionVotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PromotionApplicationId = table.Column<int>(type: "int", nullable: false),
                    PromotionCommitteeMemberId = table.Column<int>(type: "int", nullable: false),
                    VoterEmpNr = table.Column<int>(type: "int", nullable: false),
                    VotingSessionId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Vote = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    VoteDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VotingMethod = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsConfidential = table.Column<bool>(type: "bit", nullable: false),
                    VoteWeight = table.Column<decimal>(type: "decimal(3,2)", precision: 3, scale: 2, nullable: false),
                    ConfidenceLevel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PrimaryReason = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IdentifiedStrengths = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IdentifiedConcerns = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Recommendations = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TeachingEvaluation = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ResearchEvaluation = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ServiceEvaluation = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    OverallScore = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: true),
                    HasConflictOfInterest = table.Column<bool>(type: "bit", nullable: false),
                    ConflictOfInterestDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsRecused = table.Column<bool>(type: "bit", nullable: false),
                    RecusalReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ExternalConsultationSought = table.Column<bool>(type: "bit", nullable: false),
                    ExternalConsultationDetails = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AdditionalEvidenceNeeded = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SuggestedImprovements = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ReviewTimeHours = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    DocumentsReviewed = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CanChangeVote = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VoteChangeCount = table.Column<int>(type: "int", nullable: false),
                    SubmissionMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    VoterIPAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    VerificationCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    VoteMetadata = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromotionVotes_Academics_VoterEmpNr",
                        column: x => x.VoterEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PromotionVotes_PromotionApplications_PromotionApplicationId",
                        column: x => x.PromotionApplicationId,
                        principalTable: "PromotionApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PromotionVotes_PromotionCommitteeMembers_PromotionCommitteeMemberId",
                        column: x => x.PromotionCommitteeMemberId,
                        principalTable: "PromotionCommitteeMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenureMilestones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenureTrackId = table.Column<int>(type: "int", nullable: false),
                    MilestoneOrder = table.Column<int>(type: "int", nullable: false),
                    MilestoneName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MilestoneDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MilestoneType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AcademicYear = table.Column<int>(type: "int", nullable: false),
                    TenureYear = table.Column<int>(type: "int", nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "date", nullable: false),
                    CompletionDate = table.Column<DateTime>(type: "date", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    ImportanceLevel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PreparationDays = table.Column<int>(type: "int", nullable: false),
                    ExpectedDurationDays = table.Column<int>(type: "int", nullable: false),
                    ResponsibleParty = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CommitteeId = table.Column<int>(type: "int", nullable: true),
                    ReviewerEmpNr = table.Column<int>(type: "int", nullable: true),
                    RequiredDocuments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RequiredEvidence = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    EvaluationCriteria = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    SuccessCriteria = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Outcome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OutcomeDate = table.Column<DateTime>(type: "date", nullable: true),
                    Feedback = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Recommendations = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DevelopmentGoals = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ImpactsTenureEligibility = table.Column<bool>(type: "bit", nullable: false),
                    RemediationRequired = table.Column<bool>(type: "bit", nullable: false),
                    RemediationPlan = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RemediationDeadline = table.Column<DateTime>(type: "date", nullable: true),
                    RequiresExternalReview = table.Column<bool>(type: "bit", nullable: false),
                    ExternalReviewerInfo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NotificationRequirements = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FollowUpActions = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    NextMilestoneDependencies = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Metadata = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenureMilestones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenureMilestones_Academics_ReviewerEmpNr",
                        column: x => x.ReviewerEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TenureMilestones_TenureTracks_TenureTrackId",
                        column: x => x.TenureTrackId,
                        principalTable: "TenureTracks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcademicRank_Academic",
                table: "AcademicRanks",
                column: "AcademicEmpNr");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicRank_Academic_Current",
                table: "AcademicRanks",
                columns: new[] { "AcademicEmpNr", "IsCurrentRank" });

            migrationBuilder.CreateIndex(
                name: "IX_AcademicRank_RankLevel",
                table: "AcademicRanks",
                column: "RankLevel");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicRank_TenureStatus",
                table: "AcademicRanks",
                column: "TenureStatus");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicRanks_PromotionCommitteeId",
                table: "AcademicRanks",
                column: "PromotionCommitteeId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionApplication_Academic",
                table: "PromotionApplications",
                column: "AcademicEmpNr");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionApplication_RequestedRank",
                table: "PromotionApplications",
                column: "RequestedRank");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionApplication_Status",
                table: "PromotionApplications",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionApplication_Year_Status",
                table: "PromotionApplications",
                columns: new[] { "AcademicYear", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_PromotionApplications_MentorEmpNr",
                table: "PromotionApplications",
                column: "MentorEmpNr");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionApplications_PromotionCommitteeId",
                table: "PromotionApplications",
                column: "PromotionCommitteeId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionCommitteeMember_Academic",
                table: "PromotionCommitteeMembers",
                column: "EmpNr");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionCommitteeMember_Active",
                table: "PromotionCommitteeMembers",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionCommitteeMember_Committee",
                table: "PromotionCommitteeMembers",
                column: "PromotionCommitteeId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionCommitteeMember_Committee_Active",
                table: "PromotionCommitteeMembers",
                columns: new[] { "PromotionCommitteeId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_PromotionCommittee_Active",
                table: "PromotionCommittees",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionCommittee_Chair",
                table: "PromotionCommittees",
                column: "ChairEmpNr");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionCommittee_Type",
                table: "PromotionCommittees",
                column: "CommitteeType");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionCommittee_Year_Active",
                table: "PromotionCommittees",
                columns: new[] { "AcademicYear", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_PromotionVote_Application",
                table: "PromotionVotes",
                column: "PromotionApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionVote_Application_Vote",
                table: "PromotionVotes",
                columns: new[] { "PromotionApplicationId", "Vote" });

            migrationBuilder.CreateIndex(
                name: "IX_PromotionVote_Vote",
                table: "PromotionVotes",
                column: "Vote");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionVote_Voter",
                table: "PromotionVotes",
                column: "VoterEmpNr");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionVotes_PromotionCommitteeMemberId",
                table: "PromotionVotes",
                column: "PromotionCommitteeMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionWorkflowStep_Application",
                table: "PromotionWorkflowSteps",
                column: "PromotionApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionWorkflowStep_Application_Order",
                table: "PromotionWorkflowSteps",
                columns: new[] { "PromotionApplicationId", "StepOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_PromotionWorkflowStep_Order",
                table: "PromotionWorkflowSteps",
                column: "StepOrder");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionWorkflowStep_Status",
                table: "PromotionWorkflowSteps",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionWorkflowSteps_AssignedReviewerEmpNr",
                table: "PromotionWorkflowSteps",
                column: "AssignedReviewerEmpNr");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionWorkflowSteps_PromotionCommitteeId",
                table: "PromotionWorkflowSteps",
                column: "PromotionCommitteeId");

            migrationBuilder.CreateIndex(
                name: "IX_TenureMilestone_Status",
                table: "TenureMilestones",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_TenureMilestone_TenureTrack",
                table: "TenureMilestones",
                column: "TenureTrackId");

            migrationBuilder.CreateIndex(
                name: "IX_TenureMilestone_Track_Order",
                table: "TenureMilestones",
                columns: new[] { "TenureTrackId", "MilestoneOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_TenureMilestone_Year",
                table: "TenureMilestones",
                column: "TenureYear");

            migrationBuilder.CreateIndex(
                name: "IX_TenureMilestones_ReviewerEmpNr",
                table: "TenureMilestones",
                column: "ReviewerEmpNr");

            migrationBuilder.CreateIndex(
                name: "IX_TenureTrack_Academic",
                table: "TenureTracks",
                column: "AcademicEmpNr");

            migrationBuilder.CreateIndex(
                name: "IX_TenureTrack_Academic_Active",
                table: "TenureTracks",
                columns: new[] { "AcademicEmpNr", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_TenureTrack_Active",
                table: "TenureTracks",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_TenureTrack_Status",
                table: "TenureTracks",
                column: "TenureStatus");

            migrationBuilder.CreateIndex(
                name: "IX_TenureTracks_MentorEmpNr",
                table: "TenureTracks",
                column: "MentorEmpNr");

            migrationBuilder.CreateIndex(
                name: "IX_TenureTracks_TenureApplicationId",
                table: "TenureTracks",
                column: "TenureApplicationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcademicRanks");

            migrationBuilder.DropTable(
                name: "PromotionVotes");

            migrationBuilder.DropTable(
                name: "PromotionWorkflowSteps");

            migrationBuilder.DropTable(
                name: "TenureMilestones");

            migrationBuilder.DropTable(
                name: "PromotionCommitteeMembers");

            migrationBuilder.DropTable(
                name: "TenureTracks");

            migrationBuilder.DropTable(
                name: "PromotionApplications");

            migrationBuilder.DropTable(
                name: "PromotionCommittees");
        }
    }
}
