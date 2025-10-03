using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zeus.Academia.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Prompt5Task4_DepartmentAdministration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdministrativeRoles",
                columns: table => new
                {
                    RoleCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RoleTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RoleDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    HierarchyLevel = table.Column<int>(type: "int", nullable: false),
                    RoleCategory = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AuthorityScope = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RequiresFacultyStatus = table.Column<bool>(type: "bit", nullable: false),
                    MinimumRankRequired = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    MinimumYearsExperience = table.Column<int>(type: "int", nullable: true),
                    TypicalTermLength = table.Column<int>(type: "int", nullable: false),
                    MaxConsecutiveTerms = table.Column<int>(type: "int", nullable: true),
                    IncludesTenure = table.Column<bool>(type: "bit", nullable: false),
                    BaseSalaryRangeMin = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    BaseSalaryRangeMax = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    AdministrativeStipend = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    IncludesCourseRelease = table.Column<bool>(type: "bit", nullable: false),
                    CourseReleaseCount = table.Column<int>(type: "int", nullable: true),
                    IncludesAdminSupport = table.Column<bool>(type: "bit", nullable: false),
                    AdditionalBenefits = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    KeyResponsibilities = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ReportsTo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SupervisesRoles = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    BudgetAuthority = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    HiringAuthority = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    EstablishedDate = table.Column<DateTime>(type: "date", nullable: true),
                    SpecialQualifications = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Id = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdministrativeRoles", x => x.RoleCode);
                });

            migrationBuilder.CreateTable(
                name: "CommitteeChairs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommitteeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ChairEmpNr = table.Column<int>(type: "int", nullable: false),
                    AppointmentStartDate = table.Column<DateTime>(type: "date", nullable: false),
                    AppointmentEndDate = table.Column<DateTime>(type: "date", nullable: true),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    TermLengthYears = table.Column<int>(type: "int", nullable: false),
                    AppointmentType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AppointedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SelectionMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    VotesReceived = table.Column<int>(type: "int", nullable: true),
                    TotalVotesCast = table.Column<int>(type: "int", nullable: true),
                    IsEligibleForRenewal = table.Column<bool>(type: "bit", nullable: false),
                    MaxConsecutiveTerms = table.Column<int>(type: "int", nullable: true),
                    ConsecutiveTermNumber = table.Column<int>(type: "int", nullable: false),
                    ChangeReason = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SpecialResponsibilities = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    MeetingFrequency = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MeetingFormat = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    LeadershipApproach = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TermGoals = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    TransitionNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    PerformanceNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    LastReviewDate = table.Column<DateTime>(type: "date", nullable: true),
                    ChairStipend = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    ReceivesAdminSupport = table.Column<bool>(type: "bit", nullable: false),
                    ChairContactInfo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommitteeChairs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommitteeChairs_Academics_ChairEmpNr",
                        column: x => x.ChairEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommitteeChairs_Committees_CommitteeName",
                        column: x => x.CommitteeName,
                        principalTable: "Committees",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommitteeMemberAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommitteeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MemberEmpNr = table.Column<int>(type: "int", nullable: false),
                    MemberRole = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AppointmentStartDate = table.Column<DateTime>(type: "date", nullable: false),
                    AppointmentEndDate = table.Column<DateTime>(type: "date", nullable: true),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    TermLengthYears = table.Column<int>(type: "int", nullable: false),
                    SelectionMethod = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    AppointedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsEligibleForRenewal = table.Column<bool>(type: "bit", nullable: false),
                    ConsecutiveTermNumber = table.Column<int>(type: "int", nullable: false),
                    MaxConsecutiveTerms = table.Column<int>(type: "int", nullable: true),
                    ExpertiseArea = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RepresentingUnit = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AttendancePercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MeetingsAttended = table.Column<int>(type: "int", nullable: false),
                    TotalMeetingsHeld = table.Column<int>(type: "int", nullable: false),
                    SpecificResponsibilities = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PreferredContactMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AvailabilityConstraints = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PerformanceNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ChangeReason = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommitteeMemberAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommitteeMemberAssignments_Academics_MemberEmpNr",
                        column: x => x.MemberEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommitteeMemberAssignments_Committees_CommitteeName",
                        column: x => x.CommitteeName,
                        principalTable: "Committees",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DepartmentalServices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacultyEmpNr = table.Column<int>(type: "int", nullable: false),
                    DepartmentName = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    ServiceType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ServiceTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ServiceLevel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ServiceCategory = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Organization = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ServiceStartDate = table.Column<DateTime>(type: "date", nullable: false),
                    ServiceEndDate = table.Column<DateTime>(type: "date", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    AcademicYear = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    ServiceRole = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EstimatedHoursPerYear = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    ActualHoursLogged = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    ServiceWeight = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    AppointmentMethod = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    AppointedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ConfirmationDate = table.Column<DateTime>(type: "date", nullable: true),
                    IncludesLeadership = table.Column<bool>(type: "bit", nullable: false),
                    InvolvesBudgetOversight = table.Column<bool>(type: "bit", nullable: false),
                    BudgetOversight = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    InvolvestPersonnelDecisions = table.Column<bool>(type: "bit", nullable: false),
                    InvolvesStudentInteractions = table.Column<bool>(type: "bit", nullable: false),
                    InvolvesExternalStakeholders = table.Column<bool>(type: "bit", nullable: false),
                    MeetingFrequency = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MeetingsAttended = table.Column<int>(type: "int", nullable: false),
                    TotalMeetingsScheduled = table.Column<int>(type: "int", nullable: false),
                    ReceivesCompensation = table.Column<bool>(type: "bit", nullable: false),
                    CompensationAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    ReceivesCourseRelease = table.Column<bool>(type: "bit", nullable: false),
                    CourseReleaseCount = table.Column<int>(type: "int", nullable: false),
                    ReceivesAdminSupport = table.Column<bool>(type: "bit", nullable: false),
                    AdminSupportDetails = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    KeyResponsibilities = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MajorAccomplishments = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ChallengesFaced = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SkillsGained = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PerformanceEvaluation = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    LastReviewDate = table.Column<DateTime>(type: "date", nullable: true),
                    RecognitionReceived = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ServiceImpact = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CountsTowardPromotion = table.Column<bool>(type: "bit", nullable: false),
                    FulfillsTenureRequirements = table.Column<bool>(type: "bit", nullable: false),
                    AnnualReviewRating = table.Column<int>(type: "int", nullable: true),
                    WillServeAgain = table.Column<bool>(type: "bit", nullable: true),
                    FutureServiceRecommendations = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TerminationReason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TransitionNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ServiceNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentalServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DepartmentalServices_Academics_FacultyEmpNr",
                        column: x => x.FacultyEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DepartmentalServices_Departments_DepartmentName",
                        column: x => x.DepartmentName,
                        principalTable: "Departments",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DepartmentChairs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentName = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    FacultyEmpNr = table.Column<int>(type: "int", nullable: false),
                    AppointmentStartDate = table.Column<DateTime>(type: "date", nullable: false),
                    AppointmentEndDate = table.Column<DateTime>(type: "date", nullable: true),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    TermLengthYears = table.Column<int>(type: "int", nullable: false),
                    IsEligibleForRenewal = table.Column<bool>(type: "bit", nullable: false),
                    TermNumber = table.Column<int>(type: "int", nullable: false),
                    AppointmentType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AppointedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AppointmentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ChangeReason = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AnnouncementDate = table.Column<DateTime>(type: "date", nullable: true),
                    ChairStipend = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    ReceivesCourseRelease = table.Column<bool>(type: "bit", nullable: false),
                    CourseReleaseCount = table.Column<int>(type: "int", nullable: false),
                    AdditionalBenefits = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SpecialResponsibilities = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TransitionNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    PerformanceNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    LastReviewDate = table.Column<DateTime>(type: "date", nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "date", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentChairs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DepartmentChairs_Academics_FacultyEmpNr",
                        column: x => x.FacultyEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DepartmentChairs_Departments_DepartmentName",
                        column: x => x.DepartmentName,
                        principalTable: "Departments",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FacultySearchCommittees",
                columns: table => new
                {
                    SearchCommitteeCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SearchCommitteeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PositionTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DepartmentName = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    College = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PositionRank = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PositionType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    EmploymentType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AcademicYear = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    SearchStartDate = table.Column<DateTime>(type: "date", nullable: false),
                    PlannedCompletionDate = table.Column<DateTime>(type: "date", nullable: true),
                    ActualCompletionDate = table.Column<DateTime>(type: "date", nullable: true),
                    SearchStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ChairEmpNr = table.Column<int>(type: "int", nullable: false),
                    CommitteeFormationDate = table.Column<DateTime>(type: "date", nullable: false),
                    RequiredMemberCount = table.Column<int>(type: "int", nullable: false),
                    MinExternalMembers = table.Column<int>(type: "int", nullable: false),
                    RequiresDiversityCompliance = table.Column<bool>(type: "bit", nullable: false),
                    SearchBudget = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    SalaryRangeMin = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    SalaryRangeMax = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    JobDescription = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    RequiredQualifications = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    PreferredQualifications = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ResearchAreas = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TeachingResponsibilities = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ServiceExpectations = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ApplicationDeadline = table.Column<DateTime>(type: "date", nullable: true),
                    AcceptingApplications = table.Column<bool>(type: "bit", nullable: false),
                    ApplicationsReceived = table.Column<int>(type: "int", nullable: false),
                    InitialInterviewsScheduled = table.Column<int>(type: "int", nullable: false),
                    CampusVisitsScheduled = table.Column<int>(type: "int", nullable: false),
                    OffersExtended = table.Column<int>(type: "int", nullable: false),
                    OffersAccepted = table.Column<int>(type: "int", nullable: false),
                    PositionFilled = table.Column<bool>(type: "bit", nullable: true),
                    HiredCandidateEmpNr = table.Column<int>(type: "int", nullable: true),
                    HireDate = table.Column<DateTime>(type: "date", nullable: true),
                    StartingSalary = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    UnsuccessfulReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    WillReopenSearch = table.Column<bool>(type: "bit", nullable: true),
                    ComplianceNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    SearchTimeline = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    EvaluationCriteria = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    SearchNotes = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    ConfidentialNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Id = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacultySearchCommittees", x => x.SearchCommitteeCode);
                    table.ForeignKey(
                        name: "FK_FacultySearchCommittees_Academics_ChairEmpNr",
                        column: x => x.ChairEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FacultySearchCommittees_Academics_HiredCandidateEmpNr",
                        column: x => x.HiredCandidateEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr");
                    table.ForeignKey(
                        name: "FK_FacultySearchCommittees_Departments_DepartmentName",
                        column: x => x.DepartmentName,
                        principalTable: "Departments",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceLoadSummaries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacultyEmpNr = table.Column<int>(type: "int", nullable: false),
                    DepartmentName = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    AcademicYear = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    TotalServiceAssignments = table.Column<int>(type: "int", nullable: false),
                    LeadershipRoles = table.Column<int>(type: "int", nullable: false),
                    CommitteeMemberships = table.Column<int>(type: "int", nullable: false),
                    ExternalServiceCommitments = table.Column<int>(type: "int", nullable: false),
                    TotalEstimatedHours = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TotalActualHours = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TotalServiceWeight = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    TotalServiceCompensation = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    TotalCourseReleases = table.Column<int>(type: "int", nullable: false),
                    ServiceLoadCategory = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DepartmentServiceRanking = table.Column<int>(type: "int", nullable: true),
                    IsServiceLoadBalanced = table.Column<bool>(type: "bit", nullable: false),
                    ServiceLoadRecommendations = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AverageServiceRating = table.Column<decimal>(type: "decimal(3,2)", nullable: true),
                    ServiceRecognition = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SummaryCalculationDate = table.Column<DateTime>(type: "date", nullable: false),
                    ServiceContributionNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceLoadSummaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceLoadSummaries_Academics_FacultyEmpNr",
                        column: x => x.FacultyEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceLoadSummaries_Departments_DepartmentName",
                        column: x => x.DepartmentName,
                        principalTable: "Departments",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AdministrativeAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AssigneeEmpNr = table.Column<int>(type: "int", nullable: false),
                    AssignmentStartDate = table.Column<DateTime>(type: "date", nullable: false),
                    AssignmentEndDate = table.Column<DateTime>(type: "date", nullable: true),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    AppointmentType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AppointmentStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TermNumber = table.Column<int>(type: "int", nullable: false),
                    TermLengthYears = table.Column<int>(type: "int", nullable: false),
                    AppointedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AppointmentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AnnouncementDate = table.Column<DateTime>(type: "date", nullable: true),
                    ConfirmationDate = table.Column<DateTime>(type: "date", nullable: true),
                    ActualSalary = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    AdministrativeStipend = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    ActualCourseReleaseCount = table.Column<int>(type: "int", nullable: true),
                    ReportingUnit = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OfficeLocation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DirectPhoneLine = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    AdministrativeEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    HasExecutiveAssistant = table.Column<bool>(type: "bit", nullable: false),
                    ExecutiveAssistantName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AssignmentGoals = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    KeyPerformanceIndicators = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    LastPerformanceReview = table.Column<DateTime>(type: "date", nullable: true),
                    NextPerformanceReview = table.Column<DateTime>(type: "date", nullable: true),
                    PerformanceNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ChangeReason = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SuccessionPlan = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TransitionNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdministrativeAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdministrativeAssignments_Academics_AssigneeEmpNr",
                        column: x => x.AssigneeEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AdministrativeAssignments_AdministrativeRoles_RoleCode",
                        column: x => x.RoleCode,
                        principalTable: "AdministrativeRoles",
                        principalColumn: "RoleCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FacultySearchCommitteeMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SearchCommitteeCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MemberEmpNr = table.Column<int>(type: "int", nullable: false),
                    MemberRole = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AppointmentDate = table.Column<DateTime>(type: "date", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsExternalMember = table.Column<bool>(type: "bit", nullable: false),
                    ExternalAffiliation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ExpertiseArea = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RepresentationCategory = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MemberRank = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AppointedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SelectionReason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AttendedDiversityTraining = table.Column<bool>(type: "bit", nullable: false),
                    DiversityTrainingDate = table.Column<DateTime>(type: "date", nullable: true),
                    SignedConfidentialityAgreement = table.Column<bool>(type: "bit", nullable: false),
                    ConfidentialityAgreementDate = table.Column<DateTime>(type: "date", nullable: true),
                    ContactInformation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AvailabilityConstraints = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MeetingsAttended = table.Column<int>(type: "int", nullable: false),
                    TotalMeetingsHeld = table.Column<int>(type: "int", nullable: false),
                    InterviewsParticipated = table.Column<int>(type: "int", nullable: false),
                    ParticipatedInCampusVisits = table.Column<bool>(type: "bit", nullable: false),
                    ProcessEvaluation = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ConflictsOfInterest = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DepartureDate = table.Column<DateTime>(type: "date", nullable: true),
                    DepartureReason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PerformanceNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacultySearchCommitteeMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacultySearchCommitteeMembers_Academics_MemberEmpNr",
                        column: x => x.MemberEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FacultySearchCommitteeMembers_FacultySearchCommittees_SearchCommitteeCode",
                        column: x => x.SearchCommitteeCode,
                        principalTable: "FacultySearchCommittees",
                        principalColumn: "SearchCommitteeCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdministrativeAssignments_AssigneeEmpNr",
                table: "AdministrativeAssignments",
                column: "AssigneeEmpNr");

            migrationBuilder.CreateIndex(
                name: "IX_AdministrativeAssignments_RoleCode",
                table: "AdministrativeAssignments",
                column: "RoleCode");

            migrationBuilder.CreateIndex(
                name: "IX_CommitteeChairs_ChairEmpNr",
                table: "CommitteeChairs",
                column: "ChairEmpNr");

            migrationBuilder.CreateIndex(
                name: "IX_CommitteeChairs_CommitteeName",
                table: "CommitteeChairs",
                column: "CommitteeName");

            migrationBuilder.CreateIndex(
                name: "IX_CommitteeMemberAssignments_CommitteeName",
                table: "CommitteeMemberAssignments",
                column: "CommitteeName");

            migrationBuilder.CreateIndex(
                name: "IX_CommitteeMemberAssignments_MemberEmpNr",
                table: "CommitteeMemberAssignments",
                column: "MemberEmpNr");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentalServices_DepartmentName",
                table: "DepartmentalServices",
                column: "DepartmentName");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentalServices_FacultyEmpNr",
                table: "DepartmentalServices",
                column: "FacultyEmpNr");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentChairs_DepartmentName",
                table: "DepartmentChairs",
                column: "DepartmentName");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentChairs_FacultyEmpNr",
                table: "DepartmentChairs",
                column: "FacultyEmpNr");

            migrationBuilder.CreateIndex(
                name: "IX_FacultySearchCommitteeMembers_MemberEmpNr",
                table: "FacultySearchCommitteeMembers",
                column: "MemberEmpNr");

            migrationBuilder.CreateIndex(
                name: "IX_FacultySearchCommitteeMembers_SearchCommitteeCode",
                table: "FacultySearchCommitteeMembers",
                column: "SearchCommitteeCode");

            migrationBuilder.CreateIndex(
                name: "IX_FacultySearchCommittees_ChairEmpNr",
                table: "FacultySearchCommittees",
                column: "ChairEmpNr");

            migrationBuilder.CreateIndex(
                name: "IX_FacultySearchCommittees_DepartmentName",
                table: "FacultySearchCommittees",
                column: "DepartmentName");

            migrationBuilder.CreateIndex(
                name: "IX_FacultySearchCommittees_HiredCandidateEmpNr",
                table: "FacultySearchCommittees",
                column: "HiredCandidateEmpNr");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceLoadSummaries_DepartmentName",
                table: "ServiceLoadSummaries",
                column: "DepartmentName");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceLoadSummaries_FacultyEmpNr",
                table: "ServiceLoadSummaries",
                column: "FacultyEmpNr");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdministrativeAssignments");

            migrationBuilder.DropTable(
                name: "CommitteeChairs");

            migrationBuilder.DropTable(
                name: "CommitteeMemberAssignments");

            migrationBuilder.DropTable(
                name: "DepartmentalServices");

            migrationBuilder.DropTable(
                name: "DepartmentChairs");

            migrationBuilder.DropTable(
                name: "FacultySearchCommitteeMembers");

            migrationBuilder.DropTable(
                name: "ServiceLoadSummaries");

            migrationBuilder.DropTable(
                name: "AdministrativeRoles");

            migrationBuilder.DropTable(
                name: "FacultySearchCommittees");
        }
    }
}
