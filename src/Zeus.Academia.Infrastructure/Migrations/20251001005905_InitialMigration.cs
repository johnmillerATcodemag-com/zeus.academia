using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zeus.Academia.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccessLevels",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Level = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CanRead = table.Column<bool>(type: "bit", nullable: false),
                    CanCreate = table.Column<bool>(type: "bit", nullable: false),
                    CanUpdate = table.Column<bool>(type: "bit", nullable: false),
                    CanDelete = table.Column<bool>(type: "bit", nullable: false),
                    CanExecute = table.Column<bool>(type: "bit", nullable: false),
                    CanModifySystem = table.Column<bool>(type: "bit", nullable: false),
                    CanAccessFinancial = table.Column<bool>(type: "bit", nullable: false),
                    CanAccessStudentRecords = table.Column<bool>(type: "bit", nullable: false),
                    CanAccessFacultyRecords = table.Column<bool>(type: "bit", nullable: false),
                    CanGenerateReports = table.Column<bool>(type: "bit", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaxConcurrentSessions = table.Column<int>(type: "int", nullable: true),
                    SessionTimeoutMinutes = table.Column<int>(type: "int", nullable: true),
                    RequiresTwoFactor = table.Column<bool>(type: "bit", nullable: false),
                    RequiresPasswordChange = table.Column<bool>(type: "bit", nullable: false),
                    PasswordChangeFrequencyDays = table.Column<int>(type: "int", nullable: true),
                    SpecialPermissions = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessLevels", x => x.Code);
                    table.CheckConstraint("CK_AccessLevel_Level", "Level >= 1 AND Level <= 100 OR Level IS NULL");
                    table.CheckConstraint("CK_AccessLevel_Sessions", "MaxConcurrentSessions >= 1 OR MaxConcurrentSessions IS NULL");
                });

            migrationBuilder.CreateTable(
                name: "Buildings",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NumberOfFloors = table.Column<int>(type: "int", nullable: true),
                    ConstructionYear = table.Column<int>(type: "int", nullable: true),
                    TotalAreaSqFt = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    HasElevator = table.Column<bool>(type: "bit", nullable: false),
                    IsAccessible = table.Column<bool>(type: "bit", nullable: false),
                    BuildingType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    BuildingManager = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EmergencyContact = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buildings", x => x.Code);
                    table.CheckConstraint("CK_Building_Area", "TotalAreaSqFt >= 0 OR TotalAreaSqFt IS NULL");
                    table.CheckConstraint("CK_Building_Floors", "NumberOfFloors >= 1 OR NumberOfFloors IS NULL");
                });

            migrationBuilder.CreateTable(
                name: "Committees",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Committees", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Degrees",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Level = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TotalCreditHours = table.Column<int>(type: "int", nullable: true),
                    DurationYears = table.Column<decimal>(type: "decimal(3,1)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    PrimaryDepartment = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Specialization = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MinimumGPA = table.Column<decimal>(type: "decimal(3,2)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Degrees", x => x.Code);
                    table.CheckConstraint("CK_Degree_MinimumGPA", "MinimumGPA >= 1.0 AND MinimumGPA <= 4.0 OR MinimumGPA IS NULL");
                });

            migrationBuilder.CreateTable(
                name: "Ranks",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Level = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MinSalary = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    MaxSalary = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RequiresTenure = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    MinExperienceYears = table.Column<int>(type: "int", nullable: true),
                    MinDegreeLevel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    AllowsTeaching = table.Column<bool>(type: "bit", nullable: false),
                    AllowsResearch = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ranks", x => x.Code);
                    table.CheckConstraint("CK_Rank_SalaryRange", "MinSalary <= MaxSalary OR MinSalary IS NULL OR MaxSalary IS NULL");
                });

            migrationBuilder.CreateTable(
                name: "Universities",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Website = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AccreditationStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EstablishedYear = table.Column<int>(type: "int", nullable: true),
                    UniversityType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StateProvince = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    City = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    StudentEnrollment = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Universities", x => x.Code);
                    table.CheckConstraint("CK_University_StudentEnrollment", "StudentEnrollment >= 0 OR StudentEnrollment IS NULL");
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Number = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    BuildingCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: true),
                    FloorNumber = table.Column<int>(type: "int", nullable: true),
                    AreaSqFt = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    HasAVEquipment = table.Column<bool>(type: "bit", nullable: false),
                    HasComputerAccess = table.Column<bool>(type: "bit", nullable: false),
                    HasProjector = table.Column<bool>(type: "bit", nullable: false),
                    HasWhiteboard = table.Column<bool>(type: "bit", nullable: false),
                    IsAccessible = table.Column<bool>(type: "bit", nullable: false),
                    SpecialEquipment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MaintenanceNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LastMaintenanceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BookingNotes = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => new { x.Number, x.BuildingCode });
                    table.CheckConstraint("CK_Room_Capacity", "Capacity >= 1 OR Capacity IS NULL");
                    table.ForeignKey(
                        name: "FK_Rooms_Buildings_BuildingCode",
                        column: x => x.BuildingCode,
                        principalTable: "Buildings",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AcademicDegrees",
                columns: table => new
                {
                    AcademicEmpNr = table.Column<int>(type: "int", nullable: false),
                    DegreeCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    UniversityCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DateObtained = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicDegrees", x => new { x.AcademicEmpNr, x.DegreeCode, x.UniversityCode });
                    table.ForeignKey(
                        name: "FK_AcademicDegrees_Degrees_DegreeCode",
                        column: x => x.DegreeCode,
                        principalTable: "Degrees",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AcademicDegrees_Universities_UniversityCode",
                        column: x => x.UniversityCode,
                        principalTable: "Universities",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Academics",
                columns: table => new
                {
                    EmpNr = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Salary = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AcademicType = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    RankCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    DepartmentName = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    HasTenure = table.Column<bool>(type: "bit", nullable: true),
                    ResearchArea = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StudentId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Program = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DegreeCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Student_DepartmentName = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    YearOfStudy = table.Column<int>(type: "int", nullable: true),
                    GPA = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EnrollmentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpectedGraduationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    Teacher_DepartmentName = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Specialization = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EmploymentType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    MaxCourseLoad = table.Column<int>(type: "int", nullable: true),
                    TeachingProf_RankCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    TeachingProf_DepartmentName = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    TeachingProf_HasTenure = table.Column<bool>(type: "bit", nullable: true),
                    TeachingProf_ResearchArea = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TeachingProf_Specialization = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TeachingProf_EmploymentType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TeachingProf_MaxCourseLoad = table.Column<int>(type: "int", nullable: true),
                    TeachingPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ResearchPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Academics", x => x.EmpNr);
                    table.ForeignKey(
                        name: "FK_Academics_Degrees_DegreeCode",
                        column: x => x.DegreeCode,
                        principalTable: "Degrees",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Academics_Ranks_RankCode",
                        column: x => x.RankCode,
                        principalTable: "Ranks",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Academics_Ranks_TeachingProf_RankCode",
                        column: x => x.TeachingProf_RankCode,
                        principalTable: "Ranks",
                        principalColumn: "Code");
                });

            migrationBuilder.CreateTable(
                name: "CommitteeMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommitteeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AcademicEmpNr = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ProfessorEmpNr = table.Column<int>(type: "int", nullable: true),
                    TeachingProfEmpNr = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommitteeMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommitteeMembers_Academics_AcademicEmpNr",
                        column: x => x.AcademicEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommitteeMembers_Academics_ProfessorEmpNr",
                        column: x => x.ProfessorEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr");
                    table.ForeignKey(
                        name: "FK_CommitteeMembers_Academics_TeachingProfEmpNr",
                        column: x => x.TeachingProfEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr");
                    table.ForeignKey(
                        name: "FK_CommitteeMembers_Committees_CommitteeName",
                        column: x => x.CommitteeName,
                        principalTable: "Committees",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    HeadEmpNr = table.Column<int>(type: "int", nullable: true),
                    Budget = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    EstablishedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Name);
                    table.ForeignKey(
                        name: "FK_Departments_Academics_HeadEmpNr",
                        column: x => x.HeadEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Extensions",
                columns: table => new
                {
                    Number = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    HasVoicemail = table.Column<bool>(type: "bit", nullable: false),
                    AllowsForwarding = table.Column<bool>(type: "bit", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: true),
                    PrimaryContact = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ResponsibleEmployeeNr = table.Column<int>(type: "int", nullable: true),
                    SpecialInstructions = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    OperatingHours = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InstallationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastServiceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Extensions", x => x.Number);
                    table.ForeignKey(
                        name: "FK_Extensions_Academics_ResponsibleEmployeeNr",
                        column: x => x.ResponsibleEmployeeNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Chairs",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DepartmentName = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    AcademicEmpNr = table.Column<int>(type: "int", nullable: true),
                    AppointmentStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AppointmentEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chairs", x => x.Name);
                    table.ForeignKey(
                        name: "FK_Chairs_Academics_AcademicEmpNr",
                        column: x => x.AcademicEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Chairs_Departments_DepartmentName",
                        column: x => x.DepartmentName,
                        principalTable: "Departments",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreditHours = table.Column<int>(type: "int", nullable: true),
                    DepartmentName = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Level = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Prerequisites = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    TypicalSemester = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    MaxEnrollment = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Code);
                    table.CheckConstraint("CK_Subject_CreditHours", "CreditHours >= 1 AND CreditHours <= 12 OR CreditHours IS NULL");
                    table.ForeignKey(
                        name: "FK_Subjects_Departments_DepartmentName",
                        column: x => x.DepartmentName,
                        principalTable: "Departments",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "StudentEnrollments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentEmpNr = table.Column<int>(type: "int", nullable: false),
                    SubjectCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Semester = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    AcademicYear = table.Column<int>(type: "int", nullable: true),
                    Grade = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    EnrollmentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentEnrollments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentEnrollments_Academics_StudentEmpNr",
                        column: x => x.StudentEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentEnrollments_Subjects_SubjectCode",
                        column: x => x.SubjectCode,
                        principalTable: "Subjects",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeacherRatings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcademicEmpNr = table.Column<int>(type: "int", nullable: false),
                    RatingValue = table.Column<int>(type: "int", nullable: false),
                    SubjectCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Semester = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    AcademicYear = table.Column<int>(type: "int", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RatingSource = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    TeacherEmpNr = table.Column<int>(type: "int", nullable: true),
                    TeachingProfEmpNr = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherRatings_Academics_AcademicEmpNr",
                        column: x => x.AcademicEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeacherRatings_Academics_TeacherEmpNr",
                        column: x => x.TeacherEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr");
                    table.ForeignKey(
                        name: "FK_TeacherRatings_Academics_TeachingProfEmpNr",
                        column: x => x.TeachingProfEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr");
                    table.ForeignKey(
                        name: "FK_TeacherRatings_Subjects_SubjectCode",
                        column: x => x.SubjectCode,
                        principalTable: "Subjects",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Teachings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcademicEmpNr = table.Column<int>(type: "int", nullable: false),
                    SubjectCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Semester = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    AcademicYear = table.Column<int>(type: "int", nullable: true),
                    EnrollmentCount = table.Column<int>(type: "int", nullable: true),
                    ProfessorEmpNr = table.Column<int>(type: "int", nullable: true),
                    TeacherEmpNr = table.Column<int>(type: "int", nullable: true),
                    TeachingProfEmpNr = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teachings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teachings_Academics_AcademicEmpNr",
                        column: x => x.AcademicEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Teachings_Academics_ProfessorEmpNr",
                        column: x => x.ProfessorEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr");
                    table.ForeignKey(
                        name: "FK_Teachings_Academics_TeacherEmpNr",
                        column: x => x.TeacherEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr");
                    table.ForeignKey(
                        name: "FK_Teachings_Academics_TeachingProfEmpNr",
                        column: x => x.TeachingProfEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr");
                    table.ForeignKey(
                        name: "FK_Teachings_Subjects_SubjectCode",
                        column: x => x.SubjectCode,
                        principalTable: "Subjects",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcademicDegrees_DegreeCode",
                table: "AcademicDegrees",
                column: "DegreeCode");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicDegrees_UniversityCode",
                table: "AcademicDegrees",
                column: "UniversityCode");

            migrationBuilder.CreateIndex(
                name: "IX_Academics_DegreeCode",
                table: "Academics",
                column: "DegreeCode");

            migrationBuilder.CreateIndex(
                name: "IX_Academics_DepartmentName",
                table: "Academics",
                column: "DepartmentName");

            migrationBuilder.CreateIndex(
                name: "IX_Academics_Name",
                table: "Academics",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Academics_PhoneNumber",
                table: "Academics",
                column: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Academics_RankCode",
                table: "Academics",
                column: "RankCode");

            migrationBuilder.CreateIndex(
                name: "IX_Academics_Student_DepartmentName",
                table: "Academics",
                column: "Student_DepartmentName");

            migrationBuilder.CreateIndex(
                name: "IX_Academics_Teacher_DepartmentName",
                table: "Academics",
                column: "Teacher_DepartmentName");

            migrationBuilder.CreateIndex(
                name: "IX_Academics_TeachingProf_DepartmentName",
                table: "Academics",
                column: "TeachingProf_DepartmentName");

            migrationBuilder.CreateIndex(
                name: "IX_Academics_TeachingProf_RankCode",
                table: "Academics",
                column: "TeachingProf_RankCode");

            migrationBuilder.CreateIndex(
                name: "IX_AccessLevels_Category",
                table: "AccessLevels",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_AccessLevels_Category_Level",
                table: "AccessLevels",
                columns: new[] { "Category", "Level" });

            migrationBuilder.CreateIndex(
                name: "IX_AccessLevels_IsActive",
                table: "AccessLevels",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_AccessLevels_Level",
                table: "AccessLevels",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_BuildingType",
                table: "Buildings",
                column: "BuildingType");

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_IsActive",
                table: "Buildings",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_Name",
                table: "Buildings",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Chairs_AcademicEmpNr",
                table: "Chairs",
                column: "AcademicEmpNr");

            migrationBuilder.CreateIndex(
                name: "IX_Chairs_DepartmentName",
                table: "Chairs",
                column: "DepartmentName");

            migrationBuilder.CreateIndex(
                name: "IX_CommitteeMembers_AcademicEmpNr",
                table: "CommitteeMembers",
                column: "AcademicEmpNr");

            migrationBuilder.CreateIndex(
                name: "IX_CommitteeMembers_CommitteeName",
                table: "CommitteeMembers",
                column: "CommitteeName");

            migrationBuilder.CreateIndex(
                name: "IX_CommitteeMembers_ProfessorEmpNr",
                table: "CommitteeMembers",
                column: "ProfessorEmpNr");

            migrationBuilder.CreateIndex(
                name: "IX_CommitteeMembers_TeachingProfEmpNr",
                table: "CommitteeMembers",
                column: "TeachingProfEmpNr");

            migrationBuilder.CreateIndex(
                name: "IX_Degrees_IsActive",
                table: "Degrees",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Degrees_Level",
                table: "Degrees",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_Degrees_Level_PrimaryDepartment",
                table: "Degrees",
                columns: new[] { "Level", "PrimaryDepartment" });

            migrationBuilder.CreateIndex(
                name: "IX_Departments_EstablishedDate",
                table: "Departments",
                column: "EstablishedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_HeadEmpNr",
                table: "Departments",
                column: "HeadEmpNr");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_IsActive",
                table: "Departments",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_Name",
                table: "Departments",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Extensions_Department",
                table: "Extensions",
                column: "Department");

            migrationBuilder.CreateIndex(
                name: "IX_Extensions_IsActive",
                table: "Extensions",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Extensions_ResponsibleEmployeeNr",
                table: "Extensions",
                column: "ResponsibleEmployeeNr");

            migrationBuilder.CreateIndex(
                name: "IX_Extensions_Type",
                table: "Extensions",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Ranks_Category",
                table: "Ranks",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Ranks_Category_Level",
                table: "Ranks",
                columns: new[] { "Category", "Level" });

            migrationBuilder.CreateIndex(
                name: "IX_Ranks_IsActive",
                table: "Ranks",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Ranks_Level",
                table: "Ranks",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_BuildingCode_Type",
                table: "Rooms",
                columns: new[] { "BuildingCode", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_FloorNumber",
                table: "Rooms",
                column: "FloorNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_IsActive",
                table: "Rooms",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_Type",
                table: "Rooms",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_StudentEnrollments_StudentEmpNr_SubjectCode_Semester_AcademicYear",
                table: "StudentEnrollments",
                columns: new[] { "StudentEmpNr", "SubjectCode", "Semester", "AcademicYear" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentEnrollments_SubjectCode",
                table: "StudentEnrollments",
                column: "SubjectCode");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_DepartmentName_Level",
                table: "Subjects",
                columns: new[] { "DepartmentName", "Level" });

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_IsActive",
                table: "Subjects",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_Level",
                table: "Subjects",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_Title",
                table: "Subjects",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherRatings_AcademicEmpNr",
                table: "TeacherRatings",
                column: "AcademicEmpNr");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherRatings_SubjectCode",
                table: "TeacherRatings",
                column: "SubjectCode");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherRatings_TeacherEmpNr",
                table: "TeacherRatings",
                column: "TeacherEmpNr");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherRatings_TeachingProfEmpNr",
                table: "TeacherRatings",
                column: "TeachingProfEmpNr");

            migrationBuilder.CreateIndex(
                name: "IX_Teachings_AcademicEmpNr_SubjectCode_Semester_AcademicYear",
                table: "Teachings",
                columns: new[] { "AcademicEmpNr", "SubjectCode", "Semester", "AcademicYear" });

            migrationBuilder.CreateIndex(
                name: "IX_Teachings_ProfessorEmpNr",
                table: "Teachings",
                column: "ProfessorEmpNr");

            migrationBuilder.CreateIndex(
                name: "IX_Teachings_SubjectCode",
                table: "Teachings",
                column: "SubjectCode");

            migrationBuilder.CreateIndex(
                name: "IX_Teachings_TeacherEmpNr",
                table: "Teachings",
                column: "TeacherEmpNr");

            migrationBuilder.CreateIndex(
                name: "IX_Teachings_TeachingProfEmpNr",
                table: "Teachings",
                column: "TeachingProfEmpNr");

            migrationBuilder.CreateIndex(
                name: "IX_Universities_Country",
                table: "Universities",
                column: "Country");

            migrationBuilder.CreateIndex(
                name: "IX_Universities_Country_StateProvince_City",
                table: "Universities",
                columns: new[] { "Country", "StateProvince", "City" });

            migrationBuilder.CreateIndex(
                name: "IX_Universities_IsActive",
                table: "Universities",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Universities_Name",
                table: "Universities",
                column: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_AcademicDegrees_Academics_AcademicEmpNr",
                table: "AcademicDegrees",
                column: "AcademicEmpNr",
                principalTable: "Academics",
                principalColumn: "EmpNr",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Academics_Departments_DepartmentName",
                table: "Academics",
                column: "DepartmentName",
                principalTable: "Departments",
                principalColumn: "Name",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Academics_Departments_Student_DepartmentName",
                table: "Academics",
                column: "Student_DepartmentName",
                principalTable: "Departments",
                principalColumn: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_Academics_Departments_Teacher_DepartmentName",
                table: "Academics",
                column: "Teacher_DepartmentName",
                principalTable: "Departments",
                principalColumn: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_Academics_Departments_TeachingProf_DepartmentName",
                table: "Academics",
                column: "TeachingProf_DepartmentName",
                principalTable: "Departments",
                principalColumn: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Academics_HeadEmpNr",
                table: "Departments");

            migrationBuilder.DropTable(
                name: "AcademicDegrees");

            migrationBuilder.DropTable(
                name: "AccessLevels");

            migrationBuilder.DropTable(
                name: "Chairs");

            migrationBuilder.DropTable(
                name: "CommitteeMembers");

            migrationBuilder.DropTable(
                name: "Extensions");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "StudentEnrollments");

            migrationBuilder.DropTable(
                name: "TeacherRatings");

            migrationBuilder.DropTable(
                name: "Teachings");

            migrationBuilder.DropTable(
                name: "Universities");

            migrationBuilder.DropTable(
                name: "Committees");

            migrationBuilder.DropTable(
                name: "Buildings");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropTable(
                name: "Academics");

            migrationBuilder.DropTable(
                name: "Degrees");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Ranks");
        }
    }
}
