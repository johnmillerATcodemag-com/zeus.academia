using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zeus.Academia.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAcademicCalendarEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsValid",
                table: "PrerequisiteValidationResults",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LogicEvaluationResults",
                table: "PrerequisiteValidationResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MissingRequirements",
                table: "PrerequisiteValidationResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequirementResults",
                table: "PrerequisiteValidationResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AcademicCalendars",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CalendarName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AcademicYear = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicCalendars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CalendarEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CalendarId = table.Column<int>(type: "int", nullable: false),
                    EventName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EventType = table.Column<int>(type: "int", nullable: false),
                    IsAllDay = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarEvents", x => x.Id);
                    table.CheckConstraint("CK_CalendarEvents_EndDate", "[EndDate] IS NULL OR [EndDate] >= [StartDate]");
                    table.ForeignKey(
                        name: "FK_CalendarEvents_AcademicCalendars_CalendarId",
                        column: x => x.CalendarId,
                        principalTable: "AcademicCalendars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcademicCalendars_AcademicYear",
                table: "AcademicCalendars",
                column: "AcademicYear");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicCalendars_AcademicYear_IsPublished",
                table: "AcademicCalendars",
                columns: new[] { "AcademicYear", "IsPublished" });

            migrationBuilder.CreateIndex(
                name: "IX_AcademicCalendars_CalendarName_AcademicYear",
                table: "AcademicCalendars",
                columns: new[] { "CalendarName", "AcademicYear" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AcademicCalendars_IsPublished",
                table: "AcademicCalendars",
                column: "IsPublished");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_CalendarId",
                table: "CalendarEvents",
                column: "CalendarId");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_CalendarId_StartDate",
                table: "CalendarEvents",
                columns: new[] { "CalendarId", "StartDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_EventType",
                table: "CalendarEvents",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_StartDate",
                table: "CalendarEvents",
                column: "StartDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CalendarEvents");

            migrationBuilder.DropTable(
                name: "AcademicCalendars");

            migrationBuilder.DropColumn(
                name: "IsValid",
                table: "PrerequisiteValidationResults");

            migrationBuilder.DropColumn(
                name: "LogicEvaluationResults",
                table: "PrerequisiteValidationResults");

            migrationBuilder.DropColumn(
                name: "MissingRequirements",
                table: "PrerequisiteValidationResults");

            migrationBuilder.DropColumn(
                name: "RequirementResults",
                table: "PrerequisiteValidationResults");
        }
    }
}
