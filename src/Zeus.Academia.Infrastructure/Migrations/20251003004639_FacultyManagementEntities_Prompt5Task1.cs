using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zeus.Academia.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FacultyManagementEntities_Prompt5Task1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Users",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "ModifiedBy",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedEmail",
                table: "Users",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedUserName",
                table: "Users",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PhoneNumberConfirmed",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SecurityStamp",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TwoFactorEnabled",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "ModifiedBy",
                table: "UserRoles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "UserRoles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<int>(
                name: "AcademicTermId",
                table: "StudentEnrollments",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Roles",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "ModifiedBy",
                table: "Roles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Roles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "Roles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AcademiaUserId",
                table: "RefreshTokens",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "RefreshTokens",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "RefreshTokens",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DeviceId",
                table: "RefreshTokens",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUsedDate",
                table: "RefreshTokens",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "RefreshTokens",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "RefreshTokens",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "AcademicAdvisor",
                table: "Academics",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AcademicStanding",
                table: "Academics",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ActualGraduationDate",
                table: "Academics",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AdmissionDate",
                table: "Academics",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CareerGoals",
                table: "Academics",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CitizenshipStatus",
                table: "Academics",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Academics",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Academics",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreditHoursRequired",
                table: "Academics",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CumulativeGPA",
                table: "Academics",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentTerm",
                table: "Academics",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "Academics",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DietaryRestrictions",
                table: "Academics",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyPhone",
                table: "Academics",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EnrollmentStatus",
                table: "Academics",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EnrollmentStatusDate",
                table: "Academics",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ethnicity",
                table: "Academics",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "Academics",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasDietaryRestrictions",
                table: "Academics",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFullTime",
                table: "Academics",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastAcademicReviewDate",
                table: "Academics",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Academics",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PersonalEmail",
                table: "Academics",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PersonalInterests",
                table: "Academics",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "Academics",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PreferredContactMethod",
                table: "Academics",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreferredName",
                table: "Academics",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryAddress",
                table: "Academics",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ProfileCompletionPercentage",
                table: "Academics",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProfileLastUpdated",
                table: "Academics",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePhotoPath",
                table: "Academics",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresAccommodations",
                table: "Academics",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SemesterGPA",
                table: "Academics",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Academics",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalCreditHoursAttempted",
                table: "Academics",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalCreditHoursEarned",
                table: "Academics",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AcademicAdvisors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacultyEmpNr = table.Column<int>(type: "int", nullable: true),
                    AdvisorName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    OfficeLocation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DepartmentName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Specializations = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MaxStudentLoad = table.Column<int>(type: "int", nullable: true),
                    CurrentStudentCount = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsAcceptingNewStudents = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    PreferredContactMethod = table.Column<int>(type: "int", nullable: false),
                    OfficeHours = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicAdvisors", x => x.Id);
                    table.CheckConstraint("CK_AcademicAdvisors_Capacity", "MaxStudentLoad >= 1 OR MaxStudentLoad IS NULL");
                    table.CheckConstraint("CK_AcademicAdvisors_CurrentLoad", "CurrentStudentLoad >= 0");
                    table.ForeignKey(
                        name: "FK_AcademicAdvisors_Academics_FacultyEmpNr",
                        column: x => x.FacultyEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AcademicAdvisors_Departments_DepartmentName",
                        column: x => x.DepartmentName,
                        principalTable: "Departments",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AcademicTerms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TermCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TermName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TermType = table.Column<int>(type: "int", nullable: false),
                    AcademicYear = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApplicationDeadline = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EarlyApplicationDeadline = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EnrollmentStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EnrollmentDeadline = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LateEnrollmentDeadline = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DropDeadline = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WithdrawDeadline = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    ApplicationsOpen = table.Column<bool>(type: "bit", nullable: false),
                    EnrollmentOpen = table.Column<bool>(type: "bit", nullable: false),
                    MaxCreditsPerStudent = table.Column<int>(type: "int", nullable: true),
                    MinCreditsFullTime = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    TuitionAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    LateEnrollmentFee = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicTerms", x => x.Id);
                    table.CheckConstraint("CK_AcademicTerms_ApplicationDates", "ApplicationDeadline IS NULL OR ApplicationDeadline <= StartDate");
                    table.CheckConstraint("CK_AcademicTerms_Dates", "EndDate > StartDate");
                    table.CheckConstraint("CK_AcademicTerms_EarlyApplication", "EarlyApplicationDeadline IS NULL OR ApplicationDeadline IS NULL OR EarlyApplicationDeadline <= ApplicationDeadline");
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommitteeLeadership",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommitteeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AcademicEmpNr = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    ChangeReason = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AppointedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AppointmentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommitteeLeadership", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommitteeLeadership_Academics_AcademicEmpNr",
                        column: x => x.AcademicEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommitteeLeadership_Committees_CommitteeName",
                        column: x => x.CommitteeName,
                        principalTable: "Committees",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DegreeProgresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentEmpNr = table.Column<int>(type: "int", nullable: false),
                    DegreeCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RequiredCreditHours = table.Column<int>(type: "int", nullable: false),
                    CompletedCreditHours = table.Column<decimal>(type: "decimal(6,1)", precision: 6, scale: 1, nullable: false),
                    RemainingCreditHours = table.Column<decimal>(type: "decimal(6,1)", precision: 6, scale: 1, nullable: false),
                    CompletionPercentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    CumulativeGPA = table.Column<decimal>(type: "decimal(3,2)", precision: 3, scale: 2, nullable: false),
                    MajorGPA = table.Column<decimal>(type: "decimal(3,2)", precision: 3, scale: 2, nullable: true),
                    RequiredGPA = table.Column<decimal>(type: "decimal(3,2)", precision: 3, scale: 2, nullable: false, defaultValue: 2.0m),
                    MeetsGPARequirement = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ExpectedGraduationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProjectedGraduationTerm = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CoreRequirementsCompleted = table.Column<int>(type: "int", nullable: false),
                    TotalCoreRequirements = table.Column<int>(type: "int", nullable: false),
                    ElectiveRequirementsCompleted = table.Column<int>(type: "int", nullable: false),
                    TotalElectiveRequirements = table.Column<int>(type: "int", nullable: false),
                    PrerequisitesMet = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CapstoneCompleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ThesisCompleted = table.Column<bool>(type: "bit", nullable: true),
                    InternshipCompleted = table.Column<bool>(type: "bit", nullable: true),
                    AdditionalRequirements = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DegreeProgresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DegreeProgresses_Academics_StudentEmpNr",
                        column: x => x.StudentEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DegreeProgresses_Degrees_DegreeCode",
                        column: x => x.DegreeCode,
                        principalTable: "Degrees",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmergencyContacts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentEmpNr = table.Column<int>(type: "int", nullable: false),
                    ContactName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Relationship = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PrimaryPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SecondaryPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, defaultValue: "United States"),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    NotifyInEmergency = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    NotifyForAcademicIssues = table.Column<bool>(type: "bit", nullable: false),
                    NotifyForFinancialMatters = table.Column<bool>(type: "bit", nullable: false),
                    FerpaAuthorized = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    PreferredContactMethod = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    LastVerifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmergencyContacts", x => x.Id);
                    table.CheckConstraint("CK_EmergencyContacts_Priority", "Priority >= 1 AND Priority <= 10");
                    table.ForeignKey(
                        name: "FK_EmergencyContacts_Academics_StudentEmpNr",
                        column: x => x.StudentEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FacultyEmploymentHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcademicEmpNr = table.Column<int>(type: "int", nullable: false),
                    PositionTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DepartmentName = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    EmploymentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ContractType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsCurrentPosition = table.Column<bool>(type: "bit", nullable: false),
                    AnnualSalary = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    FtePercentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    TeachingLoadPercentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    ResearchExpectationPercentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    ServiceExpectationPercentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    ChangeReason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacultyEmploymentHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacultyEmploymentHistory_Academics_AcademicEmpNr",
                        column: x => x.AcademicEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FacultyEmploymentHistory_Departments_DepartmentName",
                        column: x => x.DepartmentName,
                        principalTable: "Departments",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "FacultyPromotions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcademicEmpNr = table.Column<int>(type: "int", nullable: false),
                    PromotionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FromRankCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    ToRankCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    FromTenureStatus = table.Column<bool>(type: "bit", nullable: true),
                    ToTenureStatus = table.Column<bool>(type: "bit", nullable: true),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApplicationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DepartmentRecommendation = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CollegeRecommendation = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UniversityRecommendation = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FinalDecision = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DecisionMadeBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DecisionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Justification = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    SupportingDocuments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacultyPromotions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacultyPromotions_Academics_AcademicEmpNr",
                        column: x => x.AcademicEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FacultyPromotions_Ranks_FromRankCode",
                        column: x => x.FromRankCode,
                        principalTable: "Ranks",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_FacultyPromotions_Ranks_ToRankCode",
                        column: x => x.ToRankCode,
                        principalTable: "Ranks",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "FacultyServiceRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcademicEmpNr = table.Column<int>(type: "int", nullable: false),
                    ServiceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ServiceTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Organization = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ServiceLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LeadershipRole = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    EstimatedHoursPerYear = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: true),
                    ServiceWeight = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    IsMajorService = table.Column<bool>(type: "bit", nullable: false),
                    IsExternalService = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Recognition = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacultyServiceRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacultyServiceRecords_Academics_AcademicEmpNr",
                        column: x => x.AcademicEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResearchAreas",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ParentAreaCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PrimaryDiscipline = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: true),
                    Id = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResearchAreas", x => x.Code);
                    table.ForeignKey(
                        name: "FK_ResearchAreas_ResearchAreas_ParentAreaCode",
                        column: x => x.ParentAreaCode,
                        principalTable: "ResearchAreas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "StudentDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentEmpNr = table.Column<int>(type: "int", nullable: false),
                    DocumentType = table.Column<int>(type: "int", nullable: false),
                    OriginalFileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    StoredFileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MimeType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    FileHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UploadedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UploadDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    VerifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    VerificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    AccessLevel = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentDocuments", x => x.Id);
                    table.CheckConstraint("CK_StudentDocuments_FileSize", "FileSizeBytes >= 0");
                    table.ForeignKey(
                        name: "FK_StudentDocuments_Academics_StudentEmpNr",
                        column: x => x.StudentEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentAdvisorAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentEmpNr = table.Column<int>(type: "int", nullable: false),
                    AdvisorId = table.Column<int>(type: "int", nullable: false),
                    AssignmentDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    AdvisorType = table.Column<int>(type: "int", nullable: false),
                    AssignmentReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EndReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AssignedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAdvisorAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentAdvisorAssignments_AcademicAdvisors_AdvisorId",
                        column: x => x.AdvisorId,
                        principalTable: "AcademicAdvisors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentAdvisorAssignments_Academics_StudentEmpNr",
                        column: x => x.StudentEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AcademicHonors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentEmpNr = table.Column<int>(type: "int", nullable: false),
                    HonorType = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AcademicTermId = table.Column<int>(type: "int", nullable: true),
                    Semester = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    AcademicYear = table.Column<int>(type: "int", nullable: false),
                    AwardDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequiredGPA = table.Column<decimal>(type: "decimal(3,2)", precision: 3, scale: 2, nullable: true),
                    StudentGPA = table.Column<decimal>(type: "decimal(3,2)", precision: 3, scale: 2, nullable: true),
                    MinimumCreditHours = table.Column<int>(type: "int", nullable: true),
                    AppearsOnTranscript = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    AwardingOrganization = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicHonors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcademicHonors_AcademicTerms_AcademicTermId",
                        column: x => x.AcademicTermId,
                        principalTable: "AcademicTerms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AcademicHonors_Academics_StudentEmpNr",
                        column: x => x.StudentEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Awards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentEmpNr = table.Column<int>(type: "int", nullable: false),
                    AwardType = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    MonetaryValue = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    AwardDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AcademicTermId = table.Column<int>(type: "int", nullable: true),
                    AcademicYear = table.Column<int>(type: "int", nullable: false),
                    AwardingOrganization = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Criteria = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AppearsOnTranscript = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsRecurring = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    RecurrenceFrequency = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CertificateNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Awards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Awards_AcademicTerms_AcademicTermId",
                        column: x => x.AcademicTermId,
                        principalTable: "AcademicTerms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Awards_Academics_StudentEmpNr",
                        column: x => x.StudentEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseEnrollments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentEmpNr = table.Column<int>(type: "int", nullable: false),
                    SubjectCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SectionId = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    AcademicTermId = table.Column<int>(type: "int", nullable: true),
                    Semester = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    AcademicYear = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    EnrollmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DropDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WithdrawalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreditHours = table.Column<decimal>(type: "decimal(4,1)", precision: 4, scale: 1, nullable: false),
                    IsAudit = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CountsTowardDegree = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseEnrollments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseEnrollments_AcademicTerms_AcademicTermId",
                        column: x => x.AcademicTermId,
                        principalTable: "AcademicTerms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CourseEnrollments_Academics_StudentEmpNr",
                        column: x => x.StudentEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseEnrollments_Subjects_SubjectCode",
                        column: x => x.SubjectCode,
                        principalTable: "Subjects",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EnrollmentApplications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicantEmpNr = table.Column<int>(type: "int", nullable: true),
                    ApplicantName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Program = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DepartmentName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AcademicTerm = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    AcademicYear = table.Column<int>(type: "int", nullable: true),
                    ApplicationDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Decision = table.Column<int>(type: "int", nullable: true),
                    DecisionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DecisionReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DecisionMadeBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    ExpectedEnrollmentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DocumentsComplete = table.Column<bool>(type: "bit", nullable: false),
                    SubmittedDocuments = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PreviousGPA = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PreviousInstitution = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreviousGraduationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequiresFinancialAid = table.Column<bool>(type: "bit", nullable: false),
                    IsInternationalStudent = table.Column<bool>(type: "bit", nullable: false),
                    AcademicTermId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnrollmentApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnrollmentApplications_AcademicTerms_AcademicTermId",
                        column: x => x.AcademicTermId,
                        principalTable: "AcademicTerms",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EnrollmentApplications_Academics_ApplicantEmpNr",
                        column: x => x.ApplicantEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EnrollmentApplications_Departments_DepartmentName",
                        column: x => x.DepartmentName,
                        principalTable: "Departments",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FacultyExpertise",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcademicEmpNr = table.Column<int>(type: "int", nullable: false),
                    ResearchAreaCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ExpertiseLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsPrimaryExpertise = table.Column<bool>(type: "bit", nullable: false),
                    YearsOfExperience = table.Column<int>(type: "int", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Certifications = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PublicationCount = table.Column<int>(type: "int", nullable: true),
                    GrantCount = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacultyExpertise", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacultyExpertise_Academics_AcademicEmpNr",
                        column: x => x.AcademicEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FacultyExpertise_ResearchAreas_ResearchAreaCode",
                        column: x => x.ResearchAreaCode,
                        principalTable: "ResearchAreas",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Grades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseEnrollmentId = table.Column<int>(type: "int", nullable: false),
                    GradeType = table.Column<int>(type: "int", nullable: false),
                    LetterGrade = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    NumericGrade = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    GradePoints = table.Column<decimal>(type: "decimal(3,2)", precision: 3, scale: 2, nullable: true),
                    CreditHours = table.Column<decimal>(type: "decimal(4,1)", precision: 4, scale: 1, nullable: false),
                    QualityPoints = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsFinal = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    GradeDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PostedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GradedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsMakeup = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsReplacement = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ReplacedGradeId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Grades_CourseEnrollments_CourseEnrollmentId",
                        column: x => x.CourseEnrollmentId,
                        principalTable: "CourseEnrollments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Grades_Grades_ReplacedGradeId",
                        column: x => x.ReplacedGradeId,
                        principalTable: "Grades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    MimeType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UploadDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    VerificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationDocuments_EnrollmentApplications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "EnrollmentApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EnrollmentHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentEmpNr = table.Column<int>(type: "int", nullable: false),
                    ApplicationId = table.Column<int>(type: "int", nullable: true),
                    EventType = table.Column<int>(type: "int", nullable: false),
                    PreviousStatus = table.Column<int>(type: "int", nullable: true),
                    NewStatus = table.Column<int>(type: "int", nullable: false),
                    EventDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    AcademicTerm = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    AcademicYear = table.Column<int>(type: "int", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ProcessedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DepartmentName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Program = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsSystemGenerated = table.Column<bool>(type: "bit", nullable: false),
                    NotificationSent = table.Column<bool>(type: "bit", nullable: false),
                    NotificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Metadata = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    AcademicTermId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnrollmentHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnrollmentHistory_AcademicTerms_AcademicTermId",
                        column: x => x.AcademicTermId,
                        principalTable: "AcademicTerms",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EnrollmentHistory_Academics_StudentEmpNr",
                        column: x => x.StudentEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EnrollmentHistory_Departments_DepartmentName",
                        column: x => x.DepartmentName,
                        principalTable: "Departments",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EnrollmentHistory_EnrollmentApplications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "EnrollmentApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Users",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_StudentEnrollments_AcademicTermId",
                table: "StudentEnrollments",
                column: "AcademicTermId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_AcademiaUserId",
                table: "RefreshTokens",
                column: "AcademiaUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicAdvisors_AcceptingNewStudents",
                table: "AcademicAdvisors",
                column: "IsAcceptingNewStudents");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicAdvisors_DepartmentName",
                table: "AcademicAdvisors",
                column: "DepartmentName");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicAdvisors_FacultyEmpNr",
                table: "AcademicAdvisors",
                column: "FacultyEmpNr",
                unique: true,
                filter: "[FacultyEmpNr] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicHonors_AcademicTermId",
                table: "AcademicHonors",
                column: "AcademicTermId");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicHonors_Student_Year",
                table: "AcademicHonors",
                columns: new[] { "StudentEmpNr", "AcademicYear" });

            migrationBuilder.CreateIndex(
                name: "IX_AcademicHonors_StudentEmpNr",
                table: "AcademicHonors",
                column: "StudentEmpNr");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicTerms_AcademicYear",
                table: "AcademicTerms",
                column: "AcademicYear");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicTerms_Active_Current",
                table: "AcademicTerms",
                columns: new[] { "IsActive", "IsCurrent" });

            migrationBuilder.CreateIndex(
                name: "IX_AcademicTerms_ApplicationsOpen",
                table: "AcademicTerms",
                column: "ApplicationsOpen");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicTerms_EnrollmentOpen",
                table: "AcademicTerms",
                column: "EnrollmentOpen");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicTerms_TermCode",
                table: "AcademicTerms",
                column: "TermCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AcademicTerms_TermType",
                table: "AcademicTerms",
                column: "TermType");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationDocuments_ApplicationId",
                table: "ApplicationDocuments",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationDocuments_DocumentType",
                table: "ApplicationDocuments",
                column: "DocumentType");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationDocuments_IsRequired",
                table: "ApplicationDocuments",
                column: "IsRequired");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Awards_AcademicTermId",
                table: "Awards",
                column: "AcademicTermId");

            migrationBuilder.CreateIndex(
                name: "IX_Awards_Student_Year",
                table: "Awards",
                columns: new[] { "StudentEmpNr", "AcademicYear" });

            migrationBuilder.CreateIndex(
                name: "IX_Awards_StudentEmpNr",
                table: "Awards",
                column: "StudentEmpNr");

            migrationBuilder.CreateIndex(
                name: "IX_CommitteeLeadership_Academic",
                table: "CommitteeLeadership",
                column: "AcademicEmpNr");

            migrationBuilder.CreateIndex(
                name: "IX_CommitteeLeadership_Committee",
                table: "CommitteeLeadership",
                column: "CommitteeName");

            migrationBuilder.CreateIndex(
                name: "IX_CommitteeLeadership_Committee_Position_Current",
                table: "CommitteeLeadership",
                columns: new[] { "CommitteeName", "Position", "IsCurrent" });

            migrationBuilder.CreateIndex(
                name: "IX_CommitteeLeadership_Current",
                table: "CommitteeLeadership",
                column: "IsCurrent");

            migrationBuilder.CreateIndex(
                name: "IX_CourseEnrollments_AcademicTermId",
                table: "CourseEnrollments",
                column: "AcademicTermId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseEnrollments_Student_Term",
                table: "CourseEnrollments",
                columns: new[] { "StudentEmpNr", "AcademicYear", "Semester" });

            migrationBuilder.CreateIndex(
                name: "IX_CourseEnrollments_StudentEmpNr",
                table: "CourseEnrollments",
                column: "StudentEmpNr");

            migrationBuilder.CreateIndex(
                name: "IX_CourseEnrollments_SubjectCode",
                table: "CourseEnrollments",
                column: "SubjectCode");

            migrationBuilder.CreateIndex(
                name: "IX_DegreeProgress_DegreeCode",
                table: "DegreeProgresses",
                column: "DegreeCode");

            migrationBuilder.CreateIndex(
                name: "IX_DegreeProgress_ExpectedGraduation",
                table: "DegreeProgresses",
                column: "ExpectedGraduationDate");

            migrationBuilder.CreateIndex(
                name: "IX_DegreeProgress_StudentEmpNr",
                table: "DegreeProgresses",
                column: "StudentEmpNr",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyContacts_Student_Priority",
                table: "EmergencyContacts",
                columns: new[] { "StudentEmpNr", "Priority" });

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyContacts_StudentEmpNr",
                table: "EmergencyContacts",
                column: "StudentEmpNr");

            migrationBuilder.CreateIndex(
                name: "IX_EnrollmentApplications_AcademicTermId",
                table: "EnrollmentApplications",
                column: "AcademicTermId");

            migrationBuilder.CreateIndex(
                name: "IX_EnrollmentApplications_ApplicantEmpNr",
                table: "EnrollmentApplications",
                column: "ApplicantEmpNr");

            migrationBuilder.CreateIndex(
                name: "IX_EnrollmentApplications_ApplicationDate",
                table: "EnrollmentApplications",
                column: "ApplicationDate");

            migrationBuilder.CreateIndex(
                name: "IX_EnrollmentApplications_Department_Status",
                table: "EnrollmentApplications",
                columns: new[] { "DepartmentName", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_EnrollmentApplications_Email",
                table: "EnrollmentApplications",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_EnrollmentApplications_Status",
                table: "EnrollmentApplications",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_EnrollmentHistory_AcademicTermId",
                table: "EnrollmentHistory",
                column: "AcademicTermId");

            migrationBuilder.CreateIndex(
                name: "IX_EnrollmentHistory_ApplicationId",
                table: "EnrollmentHistory",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_EnrollmentHistory_DepartmentName",
                table: "EnrollmentHistory",
                column: "DepartmentName");

            migrationBuilder.CreateIndex(
                name: "IX_EnrollmentHistory_EventDate",
                table: "EnrollmentHistory",
                column: "EventDate");

            migrationBuilder.CreateIndex(
                name: "IX_EnrollmentHistory_EventType",
                table: "EnrollmentHistory",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_EnrollmentHistory_Student_Date",
                table: "EnrollmentHistory",
                columns: new[] { "StudentEmpNr", "EventDate" });

            migrationBuilder.CreateIndex(
                name: "IX_EnrollmentHistory_StudentEmpNr",
                table: "EnrollmentHistory",
                column: "StudentEmpNr");

            migrationBuilder.CreateIndex(
                name: "IX_FacultyEmploymentHistory_Academic",
                table: "FacultyEmploymentHistory",
                column: "AcademicEmpNr");

            migrationBuilder.CreateIndex(
                name: "IX_FacultyEmploymentHistory_Academic_StartDate",
                table: "FacultyEmploymentHistory",
                columns: new[] { "AcademicEmpNr", "StartDate" });

            migrationBuilder.CreateIndex(
                name: "IX_FacultyEmploymentHistory_Current",
                table: "FacultyEmploymentHistory",
                column: "IsCurrentPosition");

            migrationBuilder.CreateIndex(
                name: "IX_FacultyEmploymentHistory_DepartmentName",
                table: "FacultyEmploymentHistory",
                column: "DepartmentName");

            migrationBuilder.CreateIndex(
                name: "IX_FacultyExpertise_Academic",
                table: "FacultyExpertise",
                column: "AcademicEmpNr");

            migrationBuilder.CreateIndex(
                name: "IX_FacultyExpertise_Academic_ResearchArea",
                table: "FacultyExpertise",
                columns: new[] { "AcademicEmpNr", "ResearchAreaCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FacultyExpertise_Level",
                table: "FacultyExpertise",
                column: "ExpertiseLevel");

            migrationBuilder.CreateIndex(
                name: "IX_FacultyExpertise_Primary",
                table: "FacultyExpertise",
                column: "IsPrimaryExpertise");

            migrationBuilder.CreateIndex(
                name: "IX_FacultyExpertise_ResearchAreaCode",
                table: "FacultyExpertise",
                column: "ResearchAreaCode");

            migrationBuilder.CreateIndex(
                name: "IX_FacultyPromotion_Academic",
                table: "FacultyPromotions",
                column: "AcademicEmpNr");

            migrationBuilder.CreateIndex(
                name: "IX_FacultyPromotion_EffectiveDate",
                table: "FacultyPromotions",
                column: "EffectiveDate");

            migrationBuilder.CreateIndex(
                name: "IX_FacultyPromotion_Status",
                table: "FacultyPromotions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_FacultyPromotions_FromRankCode",
                table: "FacultyPromotions",
                column: "FromRankCode");

            migrationBuilder.CreateIndex(
                name: "IX_FacultyPromotions_ToRankCode",
                table: "FacultyPromotions",
                column: "ToRankCode");

            migrationBuilder.CreateIndex(
                name: "IX_FacultyServiceRecord_Academic",
                table: "FacultyServiceRecords",
                column: "AcademicEmpNr");

            migrationBuilder.CreateIndex(
                name: "IX_FacultyServiceRecord_Active",
                table: "FacultyServiceRecords",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_FacultyServiceRecord_Level",
                table: "FacultyServiceRecords",
                column: "ServiceLevel");

            migrationBuilder.CreateIndex(
                name: "IX_FacultyServiceRecord_Major",
                table: "FacultyServiceRecords",
                column: "IsMajorService");

            migrationBuilder.CreateIndex(
                name: "IX_Grades_CourseEnrollmentId",
                table: "Grades",
                column: "CourseEnrollmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Grades_Enrollment_Final",
                table: "Grades",
                columns: new[] { "CourseEnrollmentId", "IsFinal" });

            migrationBuilder.CreateIndex(
                name: "IX_Grades_ReplacedGradeId",
                table: "Grades",
                column: "ReplacedGradeId");

            migrationBuilder.CreateIndex(
                name: "IX_ResearchArea_Discipline",
                table: "ResearchAreas",
                column: "PrimaryDiscipline");

            migrationBuilder.CreateIndex(
                name: "IX_ResearchArea_IsActive",
                table: "ResearchAreas",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ResearchArea_Parent",
                table: "ResearchAreas",
                column: "ParentAreaCode");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAdvisorAssignments_Advisor_Active",
                table: "StudentAdvisorAssignments",
                columns: new[] { "AdvisorId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentAdvisorAssignments_AdvisorId",
                table: "StudentAdvisorAssignments",
                column: "AdvisorId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAdvisorAssignments_Student_Primary_Active",
                table: "StudentAdvisorAssignments",
                columns: new[] { "StudentEmpNr", "IsPrimary", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentAdvisorAssignments_StudentEmpNr",
                table: "StudentAdvisorAssignments",
                column: "StudentEmpNr");

            migrationBuilder.CreateIndex(
                name: "IX_StudentDocuments_DocumentType",
                table: "StudentDocuments",
                column: "DocumentType");

            migrationBuilder.CreateIndex(
                name: "IX_StudentDocuments_IsVerified",
                table: "StudentDocuments",
                column: "IsVerified");

            migrationBuilder.CreateIndex(
                name: "IX_StudentDocuments_Student_Type",
                table: "StudentDocuments",
                columns: new[] { "StudentEmpNr", "DocumentType" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentDocuments_StudentEmpNr",
                table: "StudentDocuments",
                column: "StudentEmpNr");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_Users_AcademiaUserId",
                table: "RefreshTokens",
                column: "AcademiaUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentEnrollments_AcademicTerms_AcademicTermId",
                table: "StudentEnrollments",
                column: "AcademicTermId",
                principalTable: "AcademicTerms",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_Users_AcademiaUserId",
                table: "RefreshTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentEnrollments_AcademicTerms_AcademicTermId",
                table: "StudentEnrollments");

            migrationBuilder.DropTable(
                name: "AcademicHonors");

            migrationBuilder.DropTable(
                name: "ApplicationDocuments");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Awards");

            migrationBuilder.DropTable(
                name: "CommitteeLeadership");

            migrationBuilder.DropTable(
                name: "DegreeProgresses");

            migrationBuilder.DropTable(
                name: "EmergencyContacts");

            migrationBuilder.DropTable(
                name: "EnrollmentHistory");

            migrationBuilder.DropTable(
                name: "FacultyEmploymentHistory");

            migrationBuilder.DropTable(
                name: "FacultyExpertise");

            migrationBuilder.DropTable(
                name: "FacultyPromotions");

            migrationBuilder.DropTable(
                name: "FacultyServiceRecords");

            migrationBuilder.DropTable(
                name: "Grades");

            migrationBuilder.DropTable(
                name: "StudentAdvisorAssignments");

            migrationBuilder.DropTable(
                name: "StudentDocuments");

            migrationBuilder.DropTable(
                name: "EnrollmentApplications");

            migrationBuilder.DropTable(
                name: "ResearchAreas");

            migrationBuilder.DropTable(
                name: "CourseEnrollments");

            migrationBuilder.DropTable(
                name: "AcademicAdvisors");

            migrationBuilder.DropTable(
                name: "AcademicTerms");

            migrationBuilder.DropIndex(
                name: "EmailIndex",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_StudentEnrollments_AcademicTermId",
                table: "StudentEnrollments");

            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_AcademiaUserId",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NormalizedEmail",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NormalizedUserName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PhoneNumberConfirmed",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SecurityStamp",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TwoFactorEnabled",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AcademicTermId",
                table: "StudentEnrollments");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "AcademiaUserId",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "LastUsedDate",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "AcademicAdvisor",
                table: "Academics");

            migrationBuilder.DropColumn(
                name: "AcademicStanding",
                table: "Academics");

            migrationBuilder.DropColumn(
                name: "ActualGraduationDate",
                table: "Academics");

            migrationBuilder.DropColumn(
                name: "AdmissionDate",
                table: "Academics");

            migrationBuilder.DropColumn(
                name: "CareerGoals",
                table: "Academics");

            migrationBuilder.DropColumn(
                name: "CitizenshipStatus",
                table: "Academics");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Academics");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Academics");

            migrationBuilder.DropColumn(
                name: "CreditHoursRequired",
                table: "Academics");

            migrationBuilder.DropColumn(
                name: "CumulativeGPA",
                table: "Academics");

            migrationBuilder.DropColumn(
                name: "CurrentTerm",
                table: "Academics");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "Academics");

            migrationBuilder.DropColumn(
                name: "DietaryRestrictions",
                table: "Academics");

            migrationBuilder.DropColumn(
                name: "EmergencyPhone",
                table: "Academics");

            migrationBuilder.DropColumn(
                name: "EnrollmentStatus",
                table: "Academics");

            migrationBuilder.DropColumn(
                name: "EnrollmentStatusDate",
                table: "Academics");

            migrationBuilder.DropColumn(
                name: "Ethnicity",
                table: "Academics");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Academics");

            migrationBuilder.DropColumn(
                name: "HasDietaryRestrictions",
                table: "Academics");

            migrationBuilder.DropColumn(
                name: "IsFullTime",
                table: "Academics");

            migrationBuilder.DropColumn(
                name: "LastAcademicReviewDate",
                table: "Academics");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Academics");

            migrationBuilder.DropColumn(
                name: "PersonalEmail",
                table: "Academics");

            migrationBuilder.DropColumn(
                name: "PersonalInterests",
                table: "Academics");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "Academics");

            migrationBuilder.DropColumn(
                name: "PreferredContactMethod",
                table: "Academics");

            migrationBuilder.DropColumn(
                name: "PreferredName",
                table: "Academics");

            migrationBuilder.DropColumn(
                name: "PrimaryAddress",
                table: "Academics");

            migrationBuilder.DropColumn(
                name: "ProfileCompletionPercentage",
                table: "Academics");

            migrationBuilder.DropColumn(
                name: "ProfileLastUpdated",
                table: "Academics");

            migrationBuilder.DropColumn(
                name: "ProfilePhotoPath",
                table: "Academics");

            migrationBuilder.DropColumn(
                name: "RequiresAccommodations",
                table: "Academics");

            migrationBuilder.DropColumn(
                name: "SemesterGPA",
                table: "Academics");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Academics");

            migrationBuilder.DropColumn(
                name: "TotalCreditHoursAttempted",
                table: "Academics");

            migrationBuilder.DropColumn(
                name: "TotalCreditHoursEarned",
                table: "Academics");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ModifiedBy",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ModifiedBy",
                table: "UserRoles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "UserRoles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Roles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ModifiedBy",
                table: "Roles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Roles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
