using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zeus.Academia.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Prompt6Task1_CourseAndSubjectExtensions_Fixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_AcademicAdvisors_CurrentLoad",
                table: "AcademicAdvisors");

            migrationBuilder.AddColumn<string>(
                name: "AssessmentMethods",
                table: "Subjects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<bool>(
                name: "CanRepeat",
                table: "Subjects",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "CatalogYear",
                table: "Subjects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Corequisites",
                table: "Subjects",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryMethods",
                table: "Subjects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "LearningOutcomes",
                table: "Subjects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<int>(
                name: "MaxRepeats",
                table: "Subjects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParentSubjectCode",
                table: "Subjects",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubjectType",
                table: "Subjects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "DepartmentName",
                table: "AcademicAdvisors",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    SubjectCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CreditHours = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ContactHours = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RetiredDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RetirementReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CatalogYear = table.Column<int>(type: "int", nullable: false),
                    MaxEnrollment = table.Column<int>(type: "int", nullable: true),
                    CanRepeat = table.Column<bool>(type: "bit", nullable: false),
                    MaxRepeats = table.Column<int>(type: "int", nullable: true),
                    RequiresApproval = table.Column<bool>(type: "bit", nullable: false),
                    LearningOutcomes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Topics = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeliveryMethods = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssessmentMethods = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Courses_Subjects_SubjectCode",
                        column: x => x.SubjectCode,
                        principalTable: "Subjects",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CourseCorequisites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    RequiredCourseNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CanBeTakenBefore = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseCorequisites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseCorequisites_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseOfferings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    SectionId = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Term = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    InstructorEmpNr = table.Column<int>(type: "int", nullable: true),
                    MaxEnrollment = table.Column<int>(type: "int", nullable: true),
                    CurrentEnrollment = table.Column<int>(type: "int", nullable: false),
                    DeliveryMethod = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseOfferings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseOfferings_Academics_InstructorEmpNr",
                        column: x => x.InstructorEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CourseOfferings_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CoursePrerequisites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    PrerequisiteType = table.Column<int>(type: "int", nullable: false),
                    RequiredCourseNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    MinimumGrade = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    LogicalOperator = table.Column<int>(type: "int", nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: true),
                    CanBeWaived = table.Column<bool>(type: "bit", nullable: false),
                    WaiverRequiresApproval = table.Column<bool>(type: "bit", nullable: false),
                    AlternativeOptions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoursePrerequisites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoursePrerequisites_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseRestrictions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    RestrictionType = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseRestrictions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseRestrictions_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseStatusHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseStatusHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseStatusHistory_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreditTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Hours = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditTypes_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_ParentSubjectCode",
                table: "Subjects",
                column: "ParentSubjectCode");

            migrationBuilder.AddCheckConstraint(
                name: "CK_AcademicAdvisors_CurrentLoad",
                table: "AcademicAdvisors",
                sql: "CurrentStudentCount >= 0");

            migrationBuilder.CreateIndex(
                name: "IX_CourseCorequisites_CourseId",
                table: "CourseCorequisites",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseOfferings_CourseId",
                table: "CourseOfferings",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseOfferings_InstructorEmpNr",
                table: "CourseOfferings",
                column: "InstructorEmpNr");

            migrationBuilder.CreateIndex(
                name: "IX_CoursePrerequisites_CourseId",
                table: "CoursePrerequisites",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseRestrictions_CourseId",
                table: "CourseRestrictions",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_SubjectCode",
                table: "Courses",
                column: "SubjectCode");

            migrationBuilder.CreateIndex(
                name: "IX_CourseStatusHistory_CourseId",
                table: "CourseStatusHistory",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditTypes_CourseId",
                table: "CreditTypes",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_Subjects_ParentSubjectCode",
                table: "Subjects",
                column: "ParentSubjectCode",
                principalTable: "Subjects",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_Subjects_ParentSubjectCode",
                table: "Subjects");

            migrationBuilder.DropTable(
                name: "CourseCorequisites");

            migrationBuilder.DropTable(
                name: "CourseOfferings");

            migrationBuilder.DropTable(
                name: "CoursePrerequisites");

            migrationBuilder.DropTable(
                name: "CourseRestrictions");

            migrationBuilder.DropTable(
                name: "CourseStatusHistory");

            migrationBuilder.DropTable(
                name: "CreditTypes");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Subjects_ParentSubjectCode",
                table: "Subjects");

            migrationBuilder.DropCheckConstraint(
                name: "CK_AcademicAdvisors_CurrentLoad",
                table: "AcademicAdvisors");

            migrationBuilder.DropColumn(
                name: "AssessmentMethods",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "CanRepeat",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "CatalogYear",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "Corequisites",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "DeliveryMethods",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "LearningOutcomes",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "MaxRepeats",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "ParentSubjectCode",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "SubjectType",
                table: "Subjects");

            migrationBuilder.AlterColumn<string>(
                name: "DepartmentName",
                table: "AcademicAdvisors",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15);

            migrationBuilder.AddCheckConstraint(
                name: "CK_AcademicAdvisors_CurrentLoad",
                table: "AcademicAdvisors",
                sql: "CurrentStudentLoad >= 0");
        }
    }
}
