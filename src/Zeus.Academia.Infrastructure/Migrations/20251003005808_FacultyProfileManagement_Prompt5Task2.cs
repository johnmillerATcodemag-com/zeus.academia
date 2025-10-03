using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zeus.Academia.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FacultyProfileManagement_Prompt5Task2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FacultyProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcademicEmpNr = table.Column<int>(type: "int", nullable: false),
                    ProfessionalTitle = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreferredName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ProfessionalEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OfficePhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    MobilePhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    WebsiteUrl = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    OrcidId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GoogleScholarUrl = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LinkedInUrl = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ResearchGateUrl = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Biography = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ResearchInterests = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TeachingPhilosophy = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    EducationSummary = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AwardsHonors = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ProfessionalMemberships = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CurrentResearchProjects = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ConsultationAvailability = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OfficeHours = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsPublicProfile = table.Column<bool>(type: "bit", nullable: false),
                    IsMediaContactAvailable = table.Column<bool>(type: "bit", nullable: false),
                    IsAcceptingGraduateStudents = table.Column<bool>(type: "bit", nullable: false),
                    LastProfileUpdate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProfilePhotoPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EmergencyContact = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    InternalNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacultyProfiles", x => x.Id);
                    table.UniqueConstraint("AK_FacultyProfiles_AcademicEmpNr", x => x.AcademicEmpNr);
                    table.ForeignKey(
                        name: "FK_FacultyProfiles_Academics_AcademicEmpNr",
                        column: x => x.AcademicEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FacultyDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcademicEmpNr = table.Column<int>(type: "int", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    OriginalFileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MimeType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    IsCurrentVersion = table.Column<bool>(type: "bit", nullable: false),
                    VersionNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    ApprovedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UploadDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AccessLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Metadata = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DownloadCount = table.Column<int>(type: "int", nullable: false),
                    LastDownloadDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsArchived = table.Column<bool>(type: "bit", nullable: false),
                    ArchiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ArchiveReason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FacultyProfileId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacultyDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacultyDocuments_Academics_AcademicEmpNr",
                        column: x => x.AcademicEmpNr,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FacultyDocuments_FacultyProfiles_AcademicEmpNr",
                        column: x => x.AcademicEmpNr,
                        principalTable: "FacultyProfiles",
                        principalColumn: "AcademicEmpNr");
                    table.ForeignKey(
                        name: "FK_FacultyDocuments_FacultyProfiles_FacultyProfileId",
                        column: x => x.FacultyProfileId,
                        principalTable: "FacultyProfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FacultyPublications",
                columns: table => new
                {
                    PublicationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcademicId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PublicationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Journal = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Publisher = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ConferenceName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Volume = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Issue = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Pages = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PublicationDate = table.Column<DateTime>(type: "date", nullable: true),
                    PublicationYear = table.Column<int>(type: "int", nullable: false),
                    DOI = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ISBN = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ISSN = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Abstract = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Keywords = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CoAuthors = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsPeerReviewed = table.Column<bool>(type: "bit", nullable: false),
                    IsOpenAccess = table.Column<bool>(type: "bit", nullable: false),
                    ExternalUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CitationCount = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ResearchArea = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FundingSource = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FacultyProfileId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacultyPublications", x => x.PublicationId);
                    table.ForeignKey(
                        name: "FK_FacultyPublications_Academics_AcademicId",
                        column: x => x.AcademicId,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FacultyPublications_FacultyProfiles_FacultyProfileId",
                        column: x => x.FacultyProfileId,
                        principalTable: "FacultyProfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OfficeAssignments",
                columns: table => new
                {
                    OfficeAssignmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcademicId = table.Column<int>(type: "int", nullable: false),
                    BuildingName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RoomNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Floor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Wing = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PhoneExtension = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DirectPhoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(10,8)", precision: 10, scale: 8, nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(11,8)", precision: 11, scale: 8, nullable: true),
                    OfficeSize = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: true),
                    MaxOccupancy = table.Column<int>(type: "int", nullable: true),
                    OfficeType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OfficeFeatures = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AccessibilityFeatures = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ParkingSpot = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    KeyCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SecurityAccess = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    HasWindowView = table.Column<bool>(type: "bit", nullable: false),
                    IsSharedOffice = table.Column<bool>(type: "bit", nullable: false),
                    HasConferenceCapability = table.Column<bool>(type: "bit", nullable: false),
                    IsAccessible = table.Column<bool>(type: "bit", nullable: false),
                    AssignmentStartDate = table.Column<DateTime>(type: "date", nullable: false),
                    AssignmentEndDate = table.Column<DateTime>(type: "date", nullable: true),
                    AssignmentStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AssignmentReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EmergencyContactInstructions = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DepartmentCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CostCenter = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FacultyProfileId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfficeAssignments", x => x.OfficeAssignmentId);
                    table.ForeignKey(
                        name: "FK_OfficeAssignments_Academics_AcademicId",
                        column: x => x.AcademicId,
                        principalTable: "Academics",
                        principalColumn: "EmpNr",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OfficeAssignments_FacultyProfiles_FacultyProfileId",
                        column: x => x.FacultyProfileId,
                        principalTable: "FacultyProfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FacultyDocument_Academic",
                table: "FacultyDocuments",
                column: "AcademicEmpNr");

            migrationBuilder.CreateIndex(
                name: "IX_FacultyDocument_Academic_Type_Current",
                table: "FacultyDocuments",
                columns: new[] { "AcademicEmpNr", "DocumentType", "IsCurrentVersion" });

            migrationBuilder.CreateIndex(
                name: "IX_FacultyDocument_Current",
                table: "FacultyDocuments",
                column: "IsCurrentVersion");

            migrationBuilder.CreateIndex(
                name: "IX_FacultyDocument_Type",
                table: "FacultyDocuments",
                column: "DocumentType");

            migrationBuilder.CreateIndex(
                name: "IX_FacultyDocuments_FacultyProfileId",
                table: "FacultyDocuments",
                column: "FacultyProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_FacultyProfile_Academic",
                table: "FacultyProfiles",
                column: "AcademicEmpNr",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FacultyProfile_Public",
                table: "FacultyProfiles",
                column: "IsPublicProfile");

            migrationBuilder.CreateIndex(
                name: "IX_FacultyProfile_Public_LastModified",
                table: "FacultyProfiles",
                columns: new[] { "IsPublicProfile", "ModifiedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_FacultyPublication_Academic",
                table: "FacultyPublications",
                column: "AcademicId");

            migrationBuilder.CreateIndex(
                name: "IX_FacultyPublication_Academic_Year",
                table: "FacultyPublications",
                columns: new[] { "AcademicId", "PublicationYear" });

            migrationBuilder.CreateIndex(
                name: "IX_FacultyPublication_DOI",
                table: "FacultyPublications",
                column: "DOI");

            migrationBuilder.CreateIndex(
                name: "IX_FacultyPublication_Type",
                table: "FacultyPublications",
                column: "PublicationType");

            migrationBuilder.CreateIndex(
                name: "IX_FacultyPublication_Year",
                table: "FacultyPublications",
                column: "PublicationYear");

            migrationBuilder.CreateIndex(
                name: "IX_FacultyPublications_FacultyProfileId",
                table: "FacultyPublications",
                column: "FacultyProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_OfficeAssignment_Academic",
                table: "OfficeAssignments",
                column: "AcademicId");

            migrationBuilder.CreateIndex(
                name: "IX_OfficeAssignment_Academic_Status",
                table: "OfficeAssignments",
                columns: new[] { "AcademicId", "AssignmentStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_OfficeAssignment_Building_Room",
                table: "OfficeAssignments",
                columns: new[] { "BuildingName", "RoomNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_OfficeAssignment_Status",
                table: "OfficeAssignments",
                column: "AssignmentStatus");

            migrationBuilder.CreateIndex(
                name: "IX_OfficeAssignments_FacultyProfileId",
                table: "OfficeAssignments",
                column: "FacultyProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FacultyDocuments");

            migrationBuilder.DropTable(
                name: "FacultyPublications");

            migrationBuilder.DropTable(
                name: "OfficeAssignments");

            migrationBuilder.DropTable(
                name: "FacultyProfiles");
        }
    }
}
