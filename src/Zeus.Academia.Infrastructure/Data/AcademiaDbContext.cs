using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Zeus.Academia.Infrastructure.Configurations;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Enums;
using Zeus.Academia.Infrastructure.Identity;

namespace Zeus.Academia.Infrastructure.Data;

/// <summary>
/// Entity Framework DbContext for the Zeus Academia System with Identity integration
/// </summary>
public class AcademiaDbContext : IdentityDbContext<AcademiaUser, AcademiaRole, int, IdentityUserClaim<int>, AcademiaUserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
{
    private readonly IConfiguration _configuration;

    public AcademiaDbContext(DbContextOptions<AcademiaDbContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    // Core Academic Entities
    public DbSet<Academic> Academics { get; set; } = null!;
    public DbSet<Professor> Professors { get; set; } = null!;
    public DbSet<Teacher> Teachers { get; set; } = null!;
    public DbSet<TeachingProf> TeachingProfs { get; set; } = null!;
    public DbSet<Student> Students { get; set; } = null!;

    // Reference Entities
    public DbSet<Department> Departments { get; set; } = null!;
    public DbSet<Subject> Subjects { get; set; } = null!;
    public DbSet<Degree> Degrees { get; set; } = null!;

    // Course Catalog and Subject Management Entities (Prompt 6 Task 1)
    public DbSet<Course> Courses { get; set; } = null!;
    public DbSet<CoursePrerequisite> CoursePrerequisites { get; set; } = null!;
    public DbSet<CourseCorequisite> CourseCorequisites { get; set; } = null!;
    public DbSet<CourseRestriction> CourseRestrictions { get; set; } = null!;
    public DbSet<CreditType> CreditTypes { get; set; } = null!;
    public DbSet<CourseStatusHistory> CourseStatusHistory { get; set; } = null!;
    public DbSet<CourseOffering> CourseOfferings { get; set; } = null!;
    public DbSet<University> Universities { get; set; } = null!;
    public DbSet<Rank> Ranks { get; set; } = null!;
    public DbSet<Chair> Chairs { get; set; } = null!;
    public DbSet<Committee> Committees { get; set; } = null!;

    // Relationship Entities
    public DbSet<AcademicDegree> AcademicDegrees { get; set; } = null!;
    public DbSet<Teaching> Teachings { get; set; } = null!;
    public DbSet<CommitteeMember> CommitteeMembers { get; set; } = null!;
    public DbSet<TeacherRating> TeacherRatings { get; set; } = null!;

    // Infrastructure Entities (Task 4)
    public DbSet<Building> Buildings { get; set; } = null!;
    public DbSet<Room> Rooms { get; set; } = null!;
    public DbSet<Extension> Extensions { get; set; } = null!;
    public DbSet<AccessLevel> AccessLevels { get; set; } = null!;
    public DbSet<StudentEnrollment> StudentEnrollments { get; set; } = null!;

    // Enrollment Management Entities (Prompt 4 Task 2)
    public DbSet<EnrollmentApplication> EnrollmentApplications { get; set; } = null!;
    public DbSet<ApplicationDocument> ApplicationDocuments { get; set; } = null!;
    public DbSet<EnrollmentHistory> EnrollmentHistory { get; set; } = null!;
    public DbSet<AcademicTerm> AcademicTerms { get; set; } = null!;

    // Student Profile Management Entities (Prompt 4 Task 3)
    public DbSet<EmergencyContact> EmergencyContacts { get; set; } = null!;
    public DbSet<StudentDocument> StudentDocuments { get; set; } = null!;
    public DbSet<AcademicAdvisor> AcademicAdvisors { get; set; } = null!;
    public DbSet<StudentAdvisorAssignment> StudentAdvisorAssignments { get; set; } = null!;

    // Academic Record Management Entities (Prompt 4 Task 4)
    public DbSet<CourseEnrollment> CourseEnrollments { get; set; } = null!;
    public DbSet<Grade> Grades { get; set; } = null!;
    public DbSet<AcademicHonor> AcademicHonors { get; set; } = null!;
    public DbSet<Award> Awards { get; set; } = null!;
    public DbSet<DegreeProgress> DegreeProgresses { get; set; } = null!;

    // Faculty Management Entities (Prompt 5 Task 1)
    public DbSet<FacultyEmploymentHistory> FacultyEmploymentHistory { get; set; } = null!;
    public DbSet<FacultyPromotion> FacultyPromotions { get; set; } = null!;
    public DbSet<ResearchArea> ResearchAreas { get; set; } = null!;
    public DbSet<FacultyExpertise> FacultyExpertise { get; set; } = null!;
    public DbSet<FacultyServiceRecord> FacultyServiceRecords { get; set; } = null!;
    public DbSet<CommitteeLeadership> CommitteeLeadership { get; set; } = null!;

    // Faculty Profile Management Entities (Prompt 5 Task 2)
    public DbSet<FacultyProfile> FacultyProfiles { get; set; } = null!;
    public DbSet<FacultyDocument> FacultyDocuments { get; set; } = null!;
    public DbSet<FacultyPublication> FacultyPublications { get; set; } = null!;
    public DbSet<OfficeAssignment> OfficeAssignments { get; set; } = null!;

    // Academic Rank and Promotion System Entities (Prompt 5 Task 3)
    public DbSet<AcademicRank> AcademicRanks { get; set; } = null!;
    public DbSet<PromotionApplication> PromotionApplications { get; set; } = null!;
    public DbSet<TenureTrack> TenureTracks { get; set; } = null!;
    public DbSet<PromotionCommittee> PromotionCommittees { get; set; } = null!;
    public DbSet<PromotionCommitteeMember> PromotionCommitteeMembers { get; set; } = null!;
    public DbSet<PromotionWorkflowStep> PromotionWorkflowSteps { get; set; } = null!;
    public DbSet<TenureMilestone> TenureMilestones { get; set; } = null!;
    public DbSet<PromotionVote> PromotionVotes { get; set; } = null!;

    // Department Assignment and Administration Entities (Prompt 5 Task 4)
    public DbSet<DepartmentChair> DepartmentChairs { get; set; } = null!;
    public DbSet<CommitteeChair> CommitteeChairs { get; set; } = null!;
    public DbSet<CommitteeMemberAssignment> CommitteeMemberAssignments { get; set; } = null!;
    public DbSet<AdministrativeRole> AdministrativeRoles { get; set; } = null!;
    public DbSet<AdministrativeAssignment> AdministrativeAssignments { get; set; } = null!;
    public DbSet<FacultySearchCommittee> FacultySearchCommittees { get; set; } = null!;
    public DbSet<FacultySearchCommitteeMember> FacultySearchCommitteeMembers { get; set; } = null!;
    public DbSet<DepartmentalService> DepartmentalServices { get; set; } = null!;
    public DbSet<ServiceLoadSummary> ServiceLoadSummaries { get; set; } = null!;

    // Course Catalog Management Entities (Prompt 6 Task 2)
    public DbSet<CourseCatalog> CourseCatalogs { get; set; } = null!;
    public DbSet<CourseApprovalWorkflow> CourseApprovalWorkflows { get; set; } = null!;
    public DbSet<ApprovalStep> ApprovalSteps { get; set; } = null!;
    public DbSet<ApprovalAttachment> ApprovalAttachments { get; set; } = null!;
    public DbSet<CatalogApproval> CatalogApprovals { get; set; } = null!;
    public DbSet<LearningOutcome> LearningOutcomes { get; set; } = null!;
    public DbSet<OutcomeAssessment> OutcomeAssessments { get; set; } = null!;
    public DbSet<AssessmentResult> AssessmentResults { get; set; } = null!;
    public DbSet<CatalogPublication> CatalogPublications { get; set; } = null!;
    public DbSet<PublicationDistribution> PublicationDistributions { get; set; } = null!;
    public DbSet<PublicationAccessLog> PublicationAccessLogs { get; set; } = null!;
    public DbSet<CatalogVersion> CatalogVersions { get; set; } = null!;
    public DbSet<VersionChange> VersionChanges { get; set; } = null!;
    public DbSet<VersionComparison> VersionComparisons { get; set; } = null!;
    public DbSet<ComparisonDetail> ComparisonDetails { get; set; } = null!;

    // Additional Identity Entities (beyond the inherited ones)
    // AcademiaUserRole is accessed through inherited UserRoles property
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (!string.IsNullOrEmpty(connectionString))
            {
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Academic inheritance hierarchy (Table Per Hierarchy strategy)
        modelBuilder.Entity<Academic>()
            .HasDiscriminator<string>("AcademicType")
            .HasValue<Professor>("Professor")
            .HasValue<Teacher>("Teacher")
            .HasValue<TeachingProf>("TeachingProf")
            .HasValue<Student>("Student");

        // Configure Academic relationships
        modelBuilder.Entity<Academic>()
            .HasMany(a => a.AcademicDegrees)
            .WithOne(ad => ad.Academic)
            .HasForeignKey(ad => ad.AcademicEmpNr)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure AcademicDegree relationships
        modelBuilder.Entity<AcademicDegree>()
            .HasKey(ad => new { ad.AcademicEmpNr, ad.DegreeCode, ad.UniversityCode });

        modelBuilder.Entity<AcademicDegree>()
            .HasOne(ad => ad.Degree)
            .WithMany(d => d.AcademicDegrees)
            .HasForeignKey(ad => ad.DegreeCode)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AcademicDegree>()
            .HasOne(ad => ad.University)
            .WithMany(u => u.AcademicDegrees)
            .HasForeignKey(ad => ad.UniversityCode)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Professor relationships
        modelBuilder.Entity<Professor>()
            .HasOne(p => p.Department)
            .WithMany(d => d.Professors)
            .HasForeignKey(p => p.DepartmentName)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Professor>()
            .HasOne(p => p.Rank)
            .WithMany(r => r.Professors)
            .HasForeignKey(p => p.RankCode)
            .OnDelete(DeleteBehavior.SetNull);

        // Configure Teacher relationships
        modelBuilder.Entity<Teacher>()
            .HasOne(t => t.Department)
            .WithMany(d => d.Teachers)
            .HasForeignKey(t => t.DepartmentName)
            .OnDelete(DeleteBehavior.NoAction);

        // Configure TeachingProf relationships
        modelBuilder.Entity<TeachingProf>()
            .HasOne(tp => tp.Department)
            .WithMany(d => d.TeachingProfs)
            .HasForeignKey(tp => tp.DepartmentName)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<TeachingProf>()
            .HasOne(tp => tp.Rank)
            .WithMany(r => r.TeachingProfs)
            .HasForeignKey(tp => tp.RankCode)
            .OnDelete(DeleteBehavior.NoAction);

        // Configure Student relationships
        modelBuilder.Entity<Student>()
            .HasOne(s => s.Department)
            .WithMany(d => d.Students)
            .HasForeignKey(s => s.DepartmentName)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Student>()
            .HasOne(s => s.Degree)
            .WithMany(d => d.Students)
            .HasForeignKey(s => s.DegreeCode)
            .OnDelete(DeleteBehavior.SetNull);

        // Configure Chair relationships
        modelBuilder.Entity<Chair>()
            .HasOne(c => c.Academic)
            .WithMany()
            .HasForeignKey(c => c.AcademicEmpNr)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Chair>()
            .HasOne(c => c.Department)
            .WithMany(d => d.Chairs)
            .HasForeignKey(c => c.DepartmentName)
            .OnDelete(DeleteBehavior.SetNull);

        // Configure Department relationships
        modelBuilder.Entity<Department>()
            .HasOne(d => d.Head)
            .WithMany()
            .HasForeignKey(d => d.HeadEmpNr)
            .OnDelete(DeleteBehavior.SetNull);

        // Configure Subject relationships
        modelBuilder.Entity<Subject>()
            .HasOne(s => s.Department)
            .WithMany(d => d.Subjects)
            .HasForeignKey(s => s.DepartmentName)
            .OnDelete(DeleteBehavior.SetNull);

        // Configure Subject hierarchical relationships (Task 1)
        modelBuilder.Entity<Subject>()
            .HasOne(s => s.ParentSubject)
            .WithMany(s => s.ChildSubjects)
            .HasForeignKey(s => s.ParentSubjectCode)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Course relationships (Task 1)
        modelBuilder.Entity<Course>()
            .HasOne(c => c.Subject)
            .WithMany(s => s.Courses)
            .HasForeignKey(c => c.SubjectCode)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure CoursePrerequisite relationships
        modelBuilder.Entity<CoursePrerequisite>()
            .HasOne(cp => cp.Course)
            .WithMany(c => c.Prerequisites)
            .HasForeignKey(cp => cp.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure CourseCorequisite relationships
        modelBuilder.Entity<CourseCorequisite>()
            .HasOne(cc => cc.Course)
            .WithMany(c => c.Corequisites)
            .HasForeignKey(cc => cc.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure CourseRestriction relationships
        modelBuilder.Entity<CourseRestriction>()
            .HasOne(cr => cr.Course)
            .WithMany(c => c.Restrictions)
            .HasForeignKey(cr => cr.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure CreditType relationships
        modelBuilder.Entity<CreditType>()
            .HasOne(ct => ct.Course)
            .WithMany(c => c.CreditBreakdown)
            .HasForeignKey(ct => ct.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure CourseStatusHistory relationships
        modelBuilder.Entity<CourseStatusHistory>()
            .HasOne(csh => csh.Course)
            .WithMany(c => c.StatusHistory)
            .HasForeignKey(csh => csh.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure CourseOffering relationships
        modelBuilder.Entity<CourseOffering>()
            .HasOne(co => co.Course)
            .WithMany(c => c.Offerings)
            .HasForeignKey(co => co.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CourseOffering>()
            .HasOne(co => co.Instructor)
            .WithMany()
            .HasForeignKey(co => co.InstructorEmpNr)
            .OnDelete(DeleteBehavior.SetNull);

        // Configure Teaching relationships
        modelBuilder.Entity<Teaching>()
            .HasOne(t => t.Academic)
            .WithMany()
            .HasForeignKey(t => t.AcademicEmpNr)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Teaching>()
            .HasOne(t => t.Subject)
            .WithMany(s => s.Teachings)
            .HasForeignKey(t => t.SubjectCode)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure CommitteeMember relationships
        modelBuilder.Entity<CommitteeMember>()
            .HasOne(cm => cm.Committee)
            .WithMany(c => c.Members)
            .HasForeignKey(cm => cm.CommitteeName)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CommitteeMember>()
            .HasOne(cm => cm.Academic)
            .WithMany()
            .HasForeignKey(cm => cm.AcademicEmpNr)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure TeacherRating relationships
        modelBuilder.Entity<TeacherRating>()
            .HasOne(tr => tr.Academic)
            .WithMany()
            .HasForeignKey(tr => tr.AcademicEmpNr)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TeacherRating>()
            .HasOne(tr => tr.Subject)
            .WithMany()
            .HasForeignKey(tr => tr.SubjectCode)
            .OnDelete(DeleteBehavior.SetNull);

        // Configure StudentEnrollment relationships
        modelBuilder.Entity<StudentEnrollment>()
            .HasOne(se => se.Student)
            .WithMany(s => s.Enrollments)
            .HasForeignKey(se => se.StudentEmpNr)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<StudentEnrollment>()
            .HasOne(se => se.Subject)
            .WithMany()
            .HasForeignKey(se => se.SubjectCode)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure indexes for performance
        modelBuilder.Entity<Academic>()
            .HasIndex(a => a.Name);

        modelBuilder.Entity<Academic>()
            .HasIndex(a => a.PhoneNumber);

        modelBuilder.Entity<Teaching>()
            .HasIndex(t => new { t.AcademicEmpNr, t.SubjectCode, t.Semester, t.AcademicYear });

        modelBuilder.Entity<StudentEnrollment>()
            .HasIndex(se => new { se.StudentEmpNr, se.SubjectCode, se.Semester, se.AcademicYear });

        // Enhanced indexes for Task 3: Academic Structure Entities
        modelBuilder.Entity<Department>()
            .HasIndex(d => d.Name)
            .IsUnique();

        modelBuilder.Entity<Department>()
            .HasIndex(d => d.IsActive);

        modelBuilder.Entity<Department>()
            .HasIndex(d => d.EstablishedDate);

        modelBuilder.Entity<Subject>()
            .HasIndex(s => s.Title);

        modelBuilder.Entity<Subject>()
            .HasIndex(s => s.Level);

        modelBuilder.Entity<Subject>()
            .HasIndex(s => s.IsActive);

        modelBuilder.Entity<Subject>()
            .HasIndex(s => new { s.DepartmentName, s.Level });

        modelBuilder.Entity<Degree>()
            .HasIndex(d => d.Level);

        modelBuilder.Entity<Degree>()
            .HasIndex(d => d.IsActive);

        modelBuilder.Entity<Degree>()
            .HasIndex(d => new { d.Level, d.PrimaryDepartment });

        modelBuilder.Entity<University>()
            .HasIndex(u => u.Name);

        modelBuilder.Entity<University>()
            .HasIndex(u => u.Country);

        modelBuilder.Entity<University>()
            .HasIndex(u => u.IsActive);

        modelBuilder.Entity<University>()
            .HasIndex(u => new { u.Country, u.StateProvince, u.City });

        modelBuilder.Entity<Rank>()
            .HasIndex(r => r.Level);

        modelBuilder.Entity<Rank>()
            .HasIndex(r => r.Category);

        modelBuilder.Entity<Rank>()
            .HasIndex(r => r.IsActive);

        modelBuilder.Entity<Rank>()
            .HasIndex(r => new { r.Category, r.Level });

        // Enhanced constraints for Task 3: Academic Structure Entities
        modelBuilder.Entity<Rank>()
            .ToTable(t => t.HasCheckConstraint("CK_Rank_SalaryRange", "MinSalary <= MaxSalary OR MinSalary IS NULL OR MaxSalary IS NULL"));

        modelBuilder.Entity<Degree>()
            .ToTable(t => t.HasCheckConstraint("CK_Degree_MinimumGPA", "MinimumGPA >= 1.0 AND MinimumGPA <= 4.0 OR MinimumGPA IS NULL"));

        modelBuilder.Entity<Subject>()
            .ToTable(t => t.HasCheckConstraint("CK_Subject_CreditHours", "CreditHours >= 1 AND CreditHours <= 12 OR CreditHours IS NULL"));

        modelBuilder.Entity<University>()
            .ToTable(t => t.HasCheckConstraint("CK_University_StudentEnrollment", "StudentEnrollment >= 0 OR StudentEnrollment IS NULL"));

        // ========== Task 4: Infrastructure Entity Configurations ==========

        // Configure Room composite key and relationships
        modelBuilder.Entity<Room>()
            .HasKey(r => new { r.Number, r.BuildingCode });

        modelBuilder.Entity<Room>()
            .HasOne(r => r.Building)
            .WithMany(b => b.Rooms)
            .HasForeignKey(r => r.BuildingCode)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Extension relationships
        modelBuilder.Entity<Extension>()
            .HasOne(e => e.ResponsibleEmployee)
            .WithMany()
            .HasForeignKey(e => e.ResponsibleEmployeeNr)
            .OnDelete(DeleteBehavior.SetNull);

        // Configure Infrastructure entity indexes for performance
        modelBuilder.Entity<Building>()
            .HasIndex(b => b.Name);

        modelBuilder.Entity<Building>()
            .HasIndex(b => b.BuildingType);

        modelBuilder.Entity<Building>()
            .HasIndex(b => b.IsActive);

        modelBuilder.Entity<Room>()
            .HasIndex(r => r.Type);

        modelBuilder.Entity<Room>()
            .HasIndex(r => r.IsActive);

        modelBuilder.Entity<Room>()
            .HasIndex(r => r.FloorNumber);

        modelBuilder.Entity<Room>()
            .HasIndex(r => new { r.BuildingCode, r.Type });

        modelBuilder.Entity<Extension>()
            .HasIndex(e => e.Type);

        modelBuilder.Entity<Extension>()
            .HasIndex(e => e.Department);

        modelBuilder.Entity<Extension>()
            .HasIndex(e => e.IsActive);

        modelBuilder.Entity<AccessLevel>()
            .HasIndex(a => a.Category);

        modelBuilder.Entity<AccessLevel>()
            .HasIndex(a => a.Level);

        modelBuilder.Entity<AccessLevel>()
            .HasIndex(a => a.IsActive);

        modelBuilder.Entity<AccessLevel>()
            .HasIndex(a => new { a.Category, a.Level });

        // Configure Infrastructure entity constraints
        modelBuilder.Entity<Room>()
            .ToTable(t => t.HasCheckConstraint("CK_Room_Capacity", "Capacity >= 1 OR Capacity IS NULL"));

        modelBuilder.Entity<Building>()
            .ToTable(t => t.HasCheckConstraint("CK_Building_Floors", "NumberOfFloors >= 1 OR NumberOfFloors IS NULL"));

        modelBuilder.Entity<Building>()
            .ToTable(t => t.HasCheckConstraint("CK_Building_Area", "TotalAreaSqFt >= 0 OR TotalAreaSqFt IS NULL"));

        modelBuilder.Entity<AccessLevel>()
            .ToTable(t => t.HasCheckConstraint("CK_AccessLevel_Level", "Level >= 1 AND Level <= 100 OR Level IS NULL"));

        modelBuilder.Entity<AccessLevel>()
            .ToTable(t => t.HasCheckConstraint("CK_AccessLevel_Sessions", "MaxConcurrentSessions >= 1 OR MaxConcurrentSessions IS NULL"));

        // Configure Identity entities
        ConfigureIdentityEntities(modelBuilder);

        // Apply all entity configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AcademiaDbContext).Assembly);
    }

    private void ConfigureIdentityEntities(ModelBuilder modelBuilder)
    {
        // Configure AcademiaUser entity
        modelBuilder.Entity<AcademiaUser>(entity =>
        {
            entity.ToTable("Users");

            // Primary key configuration
            entity.HasKey(u => u.Id);

            // Username configuration
            entity.Property(u => u.UserName)
                .HasMaxLength(256);

            entity.HasIndex(u => u.UserName)
                .IsUnique()
                .HasDatabaseName("IX_Users_UserName")
                .HasFilter("UserName IS NOT NULL");

            // Email configuration
            entity.Property(u => u.Email)
                .HasMaxLength(256);

            entity.HasIndex(u => u.Email)
                .IsUnique()
                .HasDatabaseName("IX_Users_Email")
                .HasFilter("Email IS NOT NULL");

            // Password configuration
            entity.Property(u => u.PasswordHash)
                .HasMaxLength(255);

            // Academic relationship
            entity.HasOne(u => u.Academic)
                .WithMany()
                .HasForeignKey(u => u.AcademicId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Users_Academic_AcademicId");

            entity.HasIndex(u => u.AcademicId)
                .HasDatabaseName("IX_Users_AcademicId");

            // Personal information
            entity.Property(u => u.FirstName)
                .HasMaxLength(100);

            entity.Property(u => u.LastName)
                .HasMaxLength(100);

            entity.Property(u => u.DisplayName)
                .HasMaxLength(200);

            // Login tracking
            entity.Property(u => u.LastLoginDate)
                .HasColumnType("datetime2");

            entity.Property(u => u.LastLoginIpAddress)
                .HasMaxLength(45); // IPv6 max length

            // Lockout configuration
            entity.Property(u => u.LockoutEnd)
                .HasColumnType("datetimeoffset");

            // Access failure tracking
            entity.Property(u => u.AccessFailedCount)
                .HasDefaultValue(0);

            // Boolean properties with defaults
            entity.Property(u => u.EmailConfirmed)
                .HasDefaultValue(false);

            entity.Property(u => u.IsActive)
                .HasDefaultValue(true);

            entity.Property(u => u.LockoutEnabled)
                .HasDefaultValue(true);

            // Add check constraint for valid email format
            entity.ToTable(t => t.HasCheckConstraint("CK_Users_Email",
                "Email IS NULL OR Email LIKE '%_@_%.__%'"));

            // Add check constraint for access failed count
            entity.ToTable(t => t.HasCheckConstraint("CK_Users_AccessFailedCount",
                "AccessFailedCount >= 0"));
        });

        // Configure AcademiaRole entity
        modelBuilder.Entity<AcademiaRole>(entity =>
        {
            entity.ToTable("Roles");

            // Primary key configuration
            entity.HasKey(r => r.Id);

            // Role type configuration
            entity.Property(r => r.RoleType)
                .IsRequired()
                .HasConversion<int>();

            // Name configuration
            entity.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(r => r.NormalizedName)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(r => r.NormalizedName)
                .IsUnique()
                .HasDatabaseName("IX_Roles_NormalizedName");

            // Description
            entity.Property(r => r.Description)
                .HasMaxLength(500);

            // Priority configuration
            entity.Property(r => r.Priority)
                .HasDefaultValue(1);

            // Boolean properties with defaults
            entity.Property(r => r.IsActive)
                .HasDefaultValue(true);

            entity.Property(r => r.IsSystemRole)
                .HasDefaultValue(false);

            // Department relationship (optional)
            entity.HasOne(r => r.Department)
                .WithMany()
                .HasForeignKey(r => r.DepartmentName)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Roles_Department_DepartmentName");

            entity.HasIndex(r => r.DepartmentName)
                .HasDatabaseName("IX_Roles_DepartmentName");

            // Additional permissions (JSON)
            entity.Property(r => r.AdditionalPermissions)
                .HasColumnType("nvarchar(max)");

            // Composite index for role type and department
            entity.HasIndex(r => new { r.RoleType, r.DepartmentName })
                .HasDatabaseName("IX_Roles_RoleType_Department");

            // Check constraint for priority
            entity.ToTable(t => t.HasCheckConstraint("CK_Roles_Priority",
                "Priority >= 1 AND Priority <= 10"));
        });

        // Configure AcademiaUserRole entity
        modelBuilder.Entity<AcademiaUserRole>(entity =>
        {
            entity.ToTable("UserRoles");

            // Primary key configuration
            entity.HasKey(ur => ur.Id);

            // User relationship
            entity.HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_UserRoles_User_UserId");

            // Role relationship
            entity.HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_UserRoles_Role_RoleId");

            // Department context relationship (optional)
            entity.HasOne(ur => ur.DepartmentContext)
                .WithMany()
                .HasForeignKey(ur => ur.DepartmentContextName)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_UserRoles_DepartmentContext_DepartmentContextName");

            // Indexes
            entity.HasIndex(ur => ur.UserId)
                .HasDatabaseName("IX_UserRoles_UserId");

            entity.HasIndex(ur => ur.RoleId)
                .HasDatabaseName("IX_UserRoles_RoleId");

            entity.HasIndex(ur => ur.DepartmentContextName)
                .HasDatabaseName("IX_UserRoles_DepartmentContextName");

            // Composite index for user-role uniqueness (within same department context)
            entity.HasIndex(ur => new { ur.UserId, ur.RoleId, ur.DepartmentContextName })
                .IsUnique()
                .HasDatabaseName("IX_UserRoles_User_Role_Department")
                .HasFilter("DepartmentContextName IS NOT NULL");

            // Alternative composite index for global roles (no department context)
            entity.HasIndex(ur => new { ur.UserId, ur.RoleId })
                .IsUnique()
                .HasDatabaseName("IX_UserRoles_User_Role_Global")
                .HasFilter("DepartmentContextName IS NULL");

            // Date configuration
            entity.Property(ur => ur.EffectiveDate)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(ur => ur.ExpirationDate)
                .HasColumnType("datetime2");

            // String properties
            entity.Property(ur => ur.AssignmentReason)
                .HasMaxLength(500);

            entity.Property(ur => ur.AssignedBy)
                .HasMaxLength(100);

            // Assignment context (JSON)
            entity.Property(ur => ur.AssignmentContext)
                .HasColumnType("nvarchar(max)");

            // Boolean properties with defaults
            entity.Property(ur => ur.IsActive)
                .HasDefaultValue(true);

            entity.Property(ur => ur.IsPrimary)
                .HasDefaultValue(false);

            // Check constraint for effective dates
            entity.ToTable(t => t.HasCheckConstraint("CK_UserRoles_EffectiveDates",
                "ExpirationDate IS NULL OR ExpirationDate > EffectiveDate"));
        });

        // Configure RefreshToken entity
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("RefreshTokens");

            // Primary key configuration
            entity.HasKey(rt => rt.Id);

            // Token configuration
            entity.Property(rt => rt.Token)
                .IsRequired()
                .HasMaxLength(500);

            entity.HasIndex(rt => rt.Token)
                .IsUnique()
                .HasDatabaseName("IX_RefreshTokens_Token");

            // User relationship
            entity.HasOne(rt => rt.User)
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_RefreshTokens_User_UserId");

            entity.HasIndex(rt => rt.UserId)
                .HasDatabaseName("IX_RefreshTokens_UserId");

            // Date configuration
            entity.Property(rt => rt.CreatedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(rt => rt.ExpiresAt)
                .HasColumnType("datetime2")
                .IsRequired();

            entity.Property(rt => rt.RevokedAt)
                .HasColumnType("datetime2");

            // String properties
            entity.Property(rt => rt.RevocationReason)
                .HasMaxLength(200);

            entity.Property(rt => rt.JwtId)
                .HasMaxLength(100);

            entity.Property(rt => rt.CreatedByIp)
                .HasMaxLength(50);

            entity.Property(rt => rt.RevokedByIp)
                .HasMaxLength(50);

            // Boolean properties with defaults
            entity.Property(rt => rt.IsRevoked)
                .HasDefaultValue(false);

            // Indexes for performance
            entity.HasIndex(rt => rt.ExpiresAt)
                .HasDatabaseName("IX_RefreshTokens_ExpiresAt");

            entity.HasIndex(rt => rt.IsRevoked)
                .HasDatabaseName("IX_RefreshTokens_IsRevoked");

            entity.HasIndex(rt => new { rt.UserId, rt.IsRevoked, rt.ExpiresAt })
                .HasDatabaseName("IX_RefreshTokens_User_Status_Expiry");

            // Check constraint for expiration
            entity.ToTable(t => t.HasCheckConstraint("CK_RefreshTokens_Dates",
                "ExpiresAt > CreatedAt"));

            // Check constraint for revocation logic
            entity.ToTable(t => t.HasCheckConstraint("CK_RefreshTokens_Revocation",
                "(IsRevoked = 0 AND RevokedAt IS NULL) OR (IsRevoked = 1 AND RevokedAt IS NOT NULL)"));
        });

        // Configure Enrollment Management Entities (Prompt 4 Task 2)

        // Configure EnrollmentApplication entity
        modelBuilder.Entity<EnrollmentApplication>(entity =>
        {
            entity.ToTable("EnrollmentApplications");

            entity.HasKey(ea => ea.Id);

            // Required fields
            entity.Property(ea => ea.ApplicantName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(ea => ea.Email)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(ea => ea.Program)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(ea => ea.DepartmentName)
                .IsRequired()
                .HasMaxLength(100);

            // Enum configurations
            entity.Property(ea => ea.Status)
                .HasConversion<int>()
                .IsRequired();

            entity.Property(ea => ea.Decision)
                .HasConversion<int>();

            entity.Property(ea => ea.Priority)
                .HasConversion<int>()
                .HasDefaultValue(ApplicationPriority.Normal); // Normal priority

            // Date configurations
            entity.Property(ea => ea.ApplicationDate)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(ea => ea.DecisionDate)
                .HasColumnType("datetime2");

            entity.Property(ea => ea.ExpectedEnrollmentDate)
                .HasColumnType("datetime2");

            entity.Property(ea => ea.DateOfBirth)
                .HasColumnType("datetime2");

            entity.Property(ea => ea.PreviousGraduationDate)
                .HasColumnType("datetime2");

            // Relationships
            entity.HasOne(ea => ea.Applicant)
                .WithMany()
                .HasForeignKey(ea => ea.ApplicantEmpNr)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(ea => ea.Department)
                .WithMany()
                .HasForeignKey(ea => ea.DepartmentName)
                .HasPrincipalKey(d => d.Name)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            entity.HasIndex(ea => ea.Email)
                .HasDatabaseName("IX_EnrollmentApplications_Email");

            entity.HasIndex(ea => ea.Status)
                .HasDatabaseName("IX_EnrollmentApplications_Status");

            entity.HasIndex(ea => ea.ApplicationDate)
                .HasDatabaseName("IX_EnrollmentApplications_ApplicationDate");

            entity.HasIndex(ea => new { ea.DepartmentName, ea.Status })
                .HasDatabaseName("IX_EnrollmentApplications_Department_Status");
        });

        // Configure ApplicationDocument entity
        modelBuilder.Entity<ApplicationDocument>(entity =>
        {
            entity.ToTable("ApplicationDocuments");

            entity.HasKey(ad => ad.Id);

            // Required fields
            entity.Property(ad => ad.DocumentType)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(ad => ad.FileName)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(ad => ad.FilePath)
                .IsRequired()
                .HasMaxLength(500);

            // Date configurations
            entity.Property(ad => ad.UploadDate)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(ad => ad.VerificationDate)
                .HasColumnType("datetime2");

            // Relationships
            entity.HasOne(ad => ad.Application)
                .WithMany(ea => ea.Documents)
                .HasForeignKey(ad => ad.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            entity.HasIndex(ad => ad.ApplicationId)
                .HasDatabaseName("IX_ApplicationDocuments_ApplicationId");

            entity.HasIndex(ad => ad.DocumentType)
                .HasDatabaseName("IX_ApplicationDocuments_DocumentType");

            entity.HasIndex(ad => ad.IsRequired)
                .HasDatabaseName("IX_ApplicationDocuments_IsRequired");
        });

        // Configure EnrollmentHistory entity
        modelBuilder.Entity<EnrollmentHistory>(entity =>
        {
            entity.ToTable("EnrollmentHistory");

            entity.HasKey(eh => eh.Id);

            // Enum configurations
            entity.Property(eh => eh.EventType)
                .HasConversion<int>()
                .IsRequired();

            entity.Property(eh => eh.PreviousStatus)
                .HasConversion<int>();

            entity.Property(eh => eh.NewStatus)
                .HasConversion<int>()
                .IsRequired();

            // Date configurations
            entity.Property(eh => eh.EventDate)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(eh => eh.EffectiveDate)
                .HasColumnType("datetime2");

            entity.Property(eh => eh.NotificationDate)
                .HasColumnType("datetime2");

            // Relationships
            entity.HasOne(eh => eh.Student)
                .WithMany()
                .HasForeignKey(eh => eh.StudentEmpNr)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(eh => eh.Application)
                .WithMany(ea => ea.EnrollmentHistory)
                .HasForeignKey(eh => eh.ApplicationId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(eh => eh.Department)
                .WithMany()
                .HasForeignKey(eh => eh.DepartmentName)
                .HasPrincipalKey(d => d.Name)
                .OnDelete(DeleteBehavior.SetNull);

            // Indexes
            entity.HasIndex(eh => eh.StudentEmpNr)
                .HasDatabaseName("IX_EnrollmentHistory_StudentEmpNr");

            entity.HasIndex(eh => eh.EventType)
                .HasDatabaseName("IX_EnrollmentHistory_EventType");

            entity.HasIndex(eh => eh.EventDate)
                .HasDatabaseName("IX_EnrollmentHistory_EventDate");

            entity.HasIndex(eh => new { eh.StudentEmpNr, eh.EventDate })
                .HasDatabaseName("IX_EnrollmentHistory_Student_Date");
        });

        // Configure AcademicTerm entity
        modelBuilder.Entity<AcademicTerm>(entity =>
        {
            entity.ToTable("AcademicTerms");

            entity.HasKey(at => at.Id);

            // Required fields
            entity.Property(at => at.TermCode)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(at => at.TermName)
                .IsRequired()
                .HasMaxLength(50);

            // Enum configuration
            entity.Property(at => at.TermType)
                .HasConversion<int>()
                .IsRequired();

            // Date configurations
            entity.Property(at => at.StartDate)
                .HasColumnType("datetime2")
                .IsRequired();

            entity.Property(at => at.EndDate)
                .HasColumnType("datetime2")
                .IsRequired();

            entity.Property(at => at.ApplicationDeadline)
                .HasColumnType("datetime2");

            entity.Property(at => at.EarlyApplicationDeadline)
                .HasColumnType("datetime2");

            entity.Property(at => at.EnrollmentStartDate)
                .HasColumnType("datetime2");

            entity.Property(at => at.EnrollmentDeadline)
                .HasColumnType("datetime2");

            entity.Property(at => at.LateEnrollmentDeadline)
                .HasColumnType("datetime2");

            entity.Property(at => at.DropDeadline)
                .HasColumnType("datetime2");

            entity.Property(at => at.WithdrawDeadline)
                .HasColumnType("datetime2");

            // Decimal configurations
            entity.Property(at => at.TuitionAmount)
                .HasColumnType("decimal(10,2)");

            entity.Property(at => at.LateEnrollmentFee)
                .HasColumnType("decimal(8,2)");

            // Unique constraints
            entity.HasIndex(at => at.TermCode)
                .IsUnique()
                .HasDatabaseName("IX_AcademicTerms_TermCode");

            // Indexes
            entity.HasIndex(at => at.AcademicYear)
                .HasDatabaseName("IX_AcademicTerms_AcademicYear");

            entity.HasIndex(at => at.TermType)
                .HasDatabaseName("IX_AcademicTerms_TermType");

            entity.HasIndex(at => new { at.IsActive, at.IsCurrent })
                .HasDatabaseName("IX_AcademicTerms_Active_Current");

            entity.HasIndex(at => at.ApplicationsOpen)
                .HasDatabaseName("IX_AcademicTerms_ApplicationsOpen");

            entity.HasIndex(at => at.EnrollmentOpen)
                .HasDatabaseName("IX_AcademicTerms_EnrollmentOpen");

            // Check constraints
            entity.ToTable(t => t.HasCheckConstraint("CK_AcademicTerms_Dates",
                "EndDate > StartDate"));

            entity.ToTable(t => t.HasCheckConstraint("CK_AcademicTerms_ApplicationDates",
                "ApplicationDeadline IS NULL OR ApplicationDeadline <= StartDate"));

            entity.ToTable(t => t.HasCheckConstraint("CK_AcademicTerms_EarlyApplication",
                "EarlyApplicationDeadline IS NULL OR ApplicationDeadline IS NULL OR EarlyApplicationDeadline <= ApplicationDeadline"));
        });

        // ========== Prompt 4 Task 3: Student Profile Management Entity Configurations ==========

        // Configure EmergencyContact relationships and constraints
        modelBuilder.Entity<EmergencyContact>(entity =>
        {
            entity.HasKey(ec => ec.Id);

            entity.HasOne(ec => ec.Student)
                .WithMany(s => s.EmergencyContacts)
                .HasForeignKey(ec => ec.StudentEmpNr)
                .OnDelete(DeleteBehavior.Cascade);

            // String length constraints
            entity.Property(ec => ec.ContactName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(ec => ec.Relationship)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(ec => ec.PrimaryPhone)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(ec => ec.SecondaryPhone)
                .HasMaxLength(20);

            entity.Property(ec => ec.Email)
                .HasMaxLength(255);

            entity.Property(ec => ec.Address)
                .HasMaxLength(255);

            entity.Property(ec => ec.City)
                .HasMaxLength(100);

            entity.Property(ec => ec.State)
                .HasMaxLength(50);

            entity.Property(ec => ec.PostalCode)
                .HasMaxLength(20);

            entity.Property(ec => ec.Country)
                .HasMaxLength(100)
                .HasDefaultValue("United States");

            entity.Property(ec => ec.Notes)
                .HasMaxLength(1000);

            // Enum conversions
            entity.Property(ec => ec.PreferredContactMethod)
                .HasConversion<int>();

            // Boolean defaults
            entity.Property(ec => ec.NotifyInEmergency)
                .HasDefaultValue(true);

            entity.Property(ec => ec.IsActive)
                .HasDefaultValue(true);

            // Check constraints
            entity.ToTable(t => t.HasCheckConstraint("CK_EmergencyContacts_Priority",
                "Priority >= 1 AND Priority <= 10"));

            // Indexes
            entity.HasIndex(ec => ec.StudentEmpNr)
                .HasDatabaseName("IX_EmergencyContacts_StudentEmpNr");

            entity.HasIndex(ec => new { ec.StudentEmpNr, ec.Priority })
                .HasDatabaseName("IX_EmergencyContacts_Student_Priority");
        });

        // Configure StudentDocument relationships and constraints
        modelBuilder.Entity<StudentDocument>(entity =>
        {
            entity.HasKey(sd => sd.Id);

            entity.HasOne(sd => sd.Student)
                .WithMany(s => s.Documents)
                .HasForeignKey(sd => sd.StudentEmpNr)
                .OnDelete(DeleteBehavior.Cascade);

            // String length constraints
            entity.Property(sd => sd.OriginalFileName)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(sd => sd.StoredFileName)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(sd => sd.FilePath)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(sd => sd.MimeType)
                .HasMaxLength(100);

            entity.Property(sd => sd.Description)
                .HasMaxLength(1000);

            entity.Property(sd => sd.VerifiedBy)
                .HasMaxLength(100);

            entity.Property(sd => sd.Notes)
                .HasMaxLength(1000);

            // Enum conversions
            entity.Property(sd => sd.DocumentType)
                .IsRequired()
                .HasConversion<int>();

            entity.Property(sd => sd.AccessLevel)
                .HasConversion<int>()
                .HasDefaultValue(DocumentAccessLevel.StudentOnly);

            // Decimal precision
            entity.Property(sd => sd.FileSizeBytes)
                .HasColumnType("bigint");

            // Boolean defaults
            entity.Property(sd => sd.IsActive)
                .HasDefaultValue(true);

            entity.Property(sd => sd.IsVerified)
                .HasDefaultValue(false);

            entity.Property(sd => sd.IsRequired)
                .HasDefaultValue(false);

            // DateTime configurations
            entity.Property(sd => sd.UploadDate)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(sd => sd.VerificationDate)
                .HasColumnType("datetime2");

            entity.Property(sd => sd.ExpirationDate)
                .HasColumnType("datetime2");

            // Check constraints
            entity.ToTable(t => t.HasCheckConstraint("CK_StudentDocuments_FileSize",
                "FileSizeBytes >= 0"));

            // Indexes
            entity.HasIndex(sd => sd.StudentEmpNr)
                .HasDatabaseName("IX_StudentDocuments_StudentEmpNr");

            entity.HasIndex(sd => sd.DocumentType)
                .HasDatabaseName("IX_StudentDocuments_DocumentType");

            entity.HasIndex(sd => sd.IsVerified)
                .HasDatabaseName("IX_StudentDocuments_IsVerified");

            entity.HasIndex(sd => new { sd.StudentEmpNr, sd.DocumentType })
                .HasDatabaseName("IX_StudentDocuments_Student_Type");
        });

        // Configure AcademicAdvisor relationships and constraints
        modelBuilder.Entity<AcademicAdvisor>(entity =>
        {
            entity.HasKey(aa => aa.Id);

            // Configure relationship to Academic (faculty member)
            entity.HasOne(aa => aa.FacultyMember)
                .WithMany()
                .HasForeignKey(aa => aa.FacultyEmpNr)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(aa => aa.Department)
                .WithMany()
                .HasForeignKey(aa => aa.DepartmentName)
                .OnDelete(DeleteBehavior.Restrict);

            // String length constraints
            entity.Property(aa => aa.AdvisorName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(aa => aa.Title)
                .HasMaxLength(100);

            entity.Property(aa => aa.Email)
                .HasMaxLength(255);

            entity.Property(aa => aa.PhoneNumber)
                .HasMaxLength(20);

            entity.Property(aa => aa.OfficeLocation)
                .HasMaxLength(100);

            entity.Property(aa => aa.Specializations)
                .HasMaxLength(500);

            entity.Property(aa => aa.OfficeHours)
                .HasMaxLength(200);

            entity.Property(aa => aa.Notes)
                .HasMaxLength(1000);

            // Enum conversions
            entity.Property(aa => aa.PreferredContactMethod)
                .HasConversion<int>();

            // Boolean defaults
            entity.Property(aa => aa.IsActive)
                .HasDefaultValue(true);

            entity.Property(aa => aa.IsAcceptingNewStudents)
                .HasDefaultValue(true);

            // Check constraints
            entity.ToTable(t => t.HasCheckConstraint("CK_AcademicAdvisors_Capacity",
                "MaxStudentLoad >= 1 OR MaxStudentLoad IS NULL"));

            entity.ToTable(t => t.HasCheckConstraint("CK_AcademicAdvisors_CurrentLoad",
                "CurrentStudentCount >= 0"));

            // Indexes
            entity.HasIndex(aa => aa.FacultyEmpNr)
                .IsUnique()
                .HasDatabaseName("IX_AcademicAdvisors_FacultyEmpNr");

            entity.HasIndex(aa => aa.DepartmentName)
                .HasDatabaseName("IX_AcademicAdvisors_DepartmentName");

            entity.HasIndex(aa => aa.IsAcceptingNewStudents)
                .HasDatabaseName("IX_AcademicAdvisors_AcceptingNewStudents");
        });

        // Configure StudentAdvisorAssignment relationships and constraints
        modelBuilder.Entity<StudentAdvisorAssignment>(entity =>
        {
            entity.HasKey(saa => saa.Id);

            entity.HasOne(saa => saa.Student)
                .WithMany(s => s.AdvisorAssignments)
                .HasForeignKey(saa => saa.StudentEmpNr)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(saa => saa.Advisor)
                .WithMany(aa => aa.StudentAssignments)
                .HasForeignKey(saa => saa.AdvisorId)
                .OnDelete(DeleteBehavior.Restrict);

            // String length constraints
            entity.Property(saa => saa.AssignmentReason)
                .HasMaxLength(500);

            entity.Property(saa => saa.EndReason)
                .HasMaxLength(500);

            entity.Property(saa => saa.AssignedBy)
                .HasMaxLength(100);

            entity.Property(saa => saa.Notes)
                .HasMaxLength(1000);

            // Enum conversions
            entity.Property(saa => saa.AdvisorType)
                .IsRequired()
                .HasConversion<int>();

            // Boolean defaults
            entity.Property(saa => saa.IsActive)
                .HasDefaultValue(true);

            entity.Property(saa => saa.IsPrimary)
                .HasDefaultValue(false);

            // DateTime configurations
            entity.Property(saa => saa.AssignmentDate)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(saa => saa.EndDate)
                .HasColumnType("datetime2");

            // Indexes
            entity.HasIndex(saa => saa.StudentEmpNr)
                .HasDatabaseName("IX_StudentAdvisorAssignments_StudentEmpNr");

            entity.HasIndex(saa => saa.AdvisorId)
                .HasDatabaseName("IX_StudentAdvisorAssignments_AdvisorId");

            entity.HasIndex(saa => new { saa.StudentEmpNr, saa.IsPrimary, saa.IsActive })
                .HasDatabaseName("IX_StudentAdvisorAssignments_Student_Primary_Active");

            entity.HasIndex(saa => new { saa.AdvisorId, saa.IsActive })
                .HasDatabaseName("IX_StudentAdvisorAssignments_Advisor_Active");
        });

        // Configure CourseEnrollment relationships and constraints (Task 4)
        modelBuilder.Entity<CourseEnrollment>(entity =>
        {
            entity.HasKey(ce => ce.Id);

            entity.HasOne(ce => ce.Student)
                .WithMany()
                .HasForeignKey(ce => ce.StudentEmpNr)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ce => ce.Subject)
                .WithMany()
                .HasForeignKey(ce => ce.SubjectCode)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(ce => ce.AcademicTerm)
                .WithMany()
                .HasForeignKey(ce => ce.AcademicTermId)
                .OnDelete(DeleteBehavior.SetNull);

            // String length constraints
            entity.Property(ce => ce.SubjectCode)
                .IsRequired()
                .HasMaxLength(10);

            entity.Property(ce => ce.SectionId)
                .HasMaxLength(10);

            entity.Property(ce => ce.Semester)
                .HasMaxLength(20);

            entity.Property(ce => ce.Notes)
                .HasMaxLength(500);

            // Enum conversions
            entity.Property(ce => ce.Status)
                .HasConversion<int>();

            // Decimal precision
            entity.Property(ce => ce.CreditHours)
                .HasPrecision(4, 1);

            // Boolean defaults
            entity.Property(ce => ce.IsAudit)
                .HasDefaultValue(false);

            entity.Property(ce => ce.CountsTowardDegree)
                .HasDefaultValue(true);

            // DateTime configurations
            entity.Property(ce => ce.EnrollmentDate)
                .HasColumnType("datetime2");

            entity.Property(ce => ce.DropDate)
                .HasColumnType("datetime2");

            entity.Property(ce => ce.WithdrawalDate)
                .HasColumnType("datetime2");

            // Indexes
            entity.HasIndex(ce => ce.StudentEmpNr)
                .HasDatabaseName("IX_CourseEnrollments_StudentEmpNr");

            entity.HasIndex(ce => ce.SubjectCode)
                .HasDatabaseName("IX_CourseEnrollments_SubjectCode");

            entity.HasIndex(ce => new { ce.StudentEmpNr, ce.AcademicYear, ce.Semester })
                .HasDatabaseName("IX_CourseEnrollments_Student_Term");
        });

        // Configure Grade relationships and constraints (Task 4)
        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(g => g.Id);

            entity.HasOne(g => g.CourseEnrollment)
                .WithMany(ce => ce.Grades)
                .HasForeignKey(g => g.CourseEnrollmentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(g => g.ReplacedGrade)
                .WithMany()
                .HasForeignKey(g => g.ReplacedGradeId)
                .OnDelete(DeleteBehavior.SetNull);

            // String length constraints
            entity.Property(g => g.LetterGrade)
                .HasMaxLength(5);

            entity.Property(g => g.GradedBy)
                .HasMaxLength(100);

            entity.Property(g => g.Comments)
                .HasMaxLength(500);

            // Enum conversions
            entity.Property(g => g.GradeType)
                .HasConversion<int>();

            entity.Property(g => g.Status)
                .HasConversion<int>();

            // Decimal precision
            entity.Property(g => g.NumericGrade)
                .HasPrecision(5, 2);

            entity.Property(g => g.GradePoints)
                .HasPrecision(3, 2);

            entity.Property(g => g.CreditHours)
                .HasPrecision(4, 1);

            entity.Property(g => g.QualityPoints)
                .HasPrecision(6, 2);

            // Boolean defaults
            entity.Property(g => g.IsFinal)
                .HasDefaultValue(false);

            entity.Property(g => g.IsMakeup)
                .HasDefaultValue(false);

            entity.Property(g => g.IsReplacement)
                .HasDefaultValue(false);

            // DateTime configurations
            entity.Property(g => g.GradeDate)
                .HasColumnType("datetime2");

            entity.Property(g => g.PostedDate)
                .HasColumnType("datetime2");

            // Indexes
            entity.HasIndex(g => g.CourseEnrollmentId)
                .HasDatabaseName("IX_Grades_CourseEnrollmentId");

            entity.HasIndex(g => new { g.CourseEnrollmentId, g.IsFinal })
                .HasDatabaseName("IX_Grades_Enrollment_Final");
        });

        // Configure AcademicHonor relationships and constraints (Task 4)
        modelBuilder.Entity<AcademicHonor>(entity =>
        {
            entity.HasKey(ah => ah.Id);

            entity.HasOne(ah => ah.Student)
                .WithMany()
                .HasForeignKey(ah => ah.StudentEmpNr)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ah => ah.AcademicTerm)
                .WithMany()
                .HasForeignKey(ah => ah.AcademicTermId)
                .OnDelete(DeleteBehavior.SetNull);

            // String length constraints
            entity.Property(ah => ah.Title)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(ah => ah.Description)
                .HasMaxLength(500);

            entity.Property(ah => ah.Semester)
                .HasMaxLength(20);

            entity.Property(ah => ah.AwardingOrganization)
                .HasMaxLength(100);

            entity.Property(ah => ah.Notes)
                .HasMaxLength(500);

            // Enum conversions
            entity.Property(ah => ah.HonorType)
                .HasConversion<int>();

            // Decimal precision
            entity.Property(ah => ah.RequiredGPA)
                .HasPrecision(3, 2);

            entity.Property(ah => ah.StudentGPA)
                .HasPrecision(3, 2);

            // Boolean defaults
            entity.Property(ah => ah.AppearsOnTranscript)
                .HasDefaultValue(true);

            entity.Property(ah => ah.IsActive)
                .HasDefaultValue(true);

            // DateTime configurations
            entity.Property(ah => ah.AwardDate)
                .HasColumnType("datetime2");

            // Indexes
            entity.HasIndex(ah => ah.StudentEmpNr)
                .HasDatabaseName("IX_AcademicHonors_StudentEmpNr");

            entity.HasIndex(ah => new { ah.StudentEmpNr, ah.AcademicYear })
                .HasDatabaseName("IX_AcademicHonors_Student_Year");
        });

        // Configure Award relationships and constraints (Task 4)
        modelBuilder.Entity<Award>(entity =>
        {
            entity.HasKey(a => a.Id);

            entity.HasOne(a => a.Student)
                .WithMany()
                .HasForeignKey(a => a.StudentEmpNr)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(a => a.AcademicTerm)
                .WithMany()
                .HasForeignKey(a => a.AcademicTermId)
                .OnDelete(DeleteBehavior.SetNull);

            // String length constraints
            entity.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(a => a.Description)
                .HasMaxLength(1000);

            entity.Property(a => a.Currency)
                .HasMaxLength(3)
                .HasDefaultValue("USD");

            entity.Property(a => a.AwardingOrganization)
                .HasMaxLength(150);

            entity.Property(a => a.Criteria)
                .HasMaxLength(500);

            entity.Property(a => a.RecurrenceFrequency)
                .HasMaxLength(50);

            entity.Property(a => a.CertificateNumber)
                .HasMaxLength(50);

            entity.Property(a => a.Notes)
                .HasMaxLength(500);

            // Enum conversions
            entity.Property(a => a.AwardType)
                .HasConversion<int>();

            // Decimal precision
            entity.Property(a => a.MonetaryValue)
                .HasPrecision(10, 2);

            // Boolean defaults
            entity.Property(a => a.AppearsOnTranscript)
                .HasDefaultValue(true);

            entity.Property(a => a.IsRecurring)
                .HasDefaultValue(false);

            entity.Property(a => a.IsActive)
                .HasDefaultValue(true);

            // DateTime configurations
            entity.Property(a => a.AwardDate)
                .HasColumnType("datetime2");

            // Indexes
            entity.HasIndex(a => a.StudentEmpNr)
                .HasDatabaseName("IX_Awards_StudentEmpNr");

            entity.HasIndex(a => new { a.StudentEmpNr, a.AcademicYear })
                .HasDatabaseName("IX_Awards_Student_Year");
        });

        // Configure DegreeProgress relationships and constraints (Task 4)
        modelBuilder.Entity<DegreeProgress>(entity =>
        {
            entity.HasKey(dp => dp.Id);

            entity.HasOne(dp => dp.Student)
                .WithMany()
                .HasForeignKey(dp => dp.StudentEmpNr)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(dp => dp.Degree)
                .WithMany()
                .HasForeignKey(dp => dp.DegreeCode)
                .OnDelete(DeleteBehavior.Restrict);

            // String length constraints
            entity.Property(dp => dp.ProjectedGraduationTerm)
                .HasMaxLength(20);

            entity.Property(dp => dp.AdditionalRequirements)
                .HasMaxLength(1000);

            entity.Property(dp => dp.UpdatedBy)
                .HasMaxLength(100);

            entity.Property(dp => dp.Notes)
                .HasMaxLength(1000);

            // Decimal precision
            entity.Property(dp => dp.CompletedCreditHours)
                .HasPrecision(6, 1);

            entity.Property(dp => dp.RemainingCreditHours)
                .HasPrecision(6, 1);

            entity.Property(dp => dp.CompletionPercentage)
                .HasPrecision(5, 2);

            entity.Property(dp => dp.CumulativeGPA)
                .HasPrecision(3, 2);

            entity.Property(dp => dp.MajorGPA)
                .HasPrecision(3, 2);

            entity.Property(dp => dp.RequiredGPA)
                .HasPrecision(3, 2)
                .HasDefaultValue(2.0m);

            // Boolean defaults
            entity.Property(dp => dp.MeetsGPARequirement)
                .HasDefaultValue(false);

            entity.Property(dp => dp.PrerequisitesMet)
                .HasDefaultValue(false);

            entity.Property(dp => dp.CapstoneCompleted)
                .HasDefaultValue(false);

            // DateTime configurations
            entity.Property(dp => dp.ExpectedGraduationDate)
                .HasColumnType("datetime2");

            entity.Property(dp => dp.LastUpdated)
                .HasColumnType("datetime2");

            // Indexes
            entity.HasIndex(dp => dp.StudentEmpNr)
                .IsUnique()
                .HasDatabaseName("IX_DegreeProgress_StudentEmpNr");

            entity.HasIndex(dp => dp.DegreeCode)
                .HasDatabaseName("IX_DegreeProgress_DegreeCode");

            entity.HasIndex(dp => dp.ExpectedGraduationDate)
                .HasDatabaseName("IX_DegreeProgress_ExpectedGraduation");
        });

        // Configure Faculty Management entities (Prompt 5 Task 1)
        ConfigureFacultyManagementEntities(modelBuilder);
    }

    /// <summary>
    /// Configures Faculty Management entities and their relationships (Prompt 5 Task 1).
    /// </summary>
    /// <param name="modelBuilder">The model builder instance.</param>
    private void ConfigureFacultyManagementEntities(ModelBuilder modelBuilder)
    {
        // Configure FacultyEmploymentHistory
        modelBuilder.Entity<FacultyEmploymentHistory>(entity =>
        {
            entity.HasKey(feh => feh.Id);

            // Configure relationships
            entity.HasOne(feh => feh.Academic)
                .WithMany(a => a.EmploymentHistory)
                .HasForeignKey(feh => feh.AcademicEmpNr)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(feh => feh.Department)
                .WithMany()
                .HasForeignKey(feh => feh.DepartmentName)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure decimal precision
            entity.Property(feh => feh.AnnualSalary)
                .HasPrecision(10, 2);

            entity.Property(feh => feh.FtePercentage)
                .HasPrecision(5, 2);

            entity.Property(feh => feh.TeachingLoadPercentage)
                .HasPrecision(5, 2);

            entity.Property(feh => feh.ResearchExpectationPercentage)
                .HasPrecision(5, 2);

            entity.Property(feh => feh.ServiceExpectationPercentage)
                .HasPrecision(5, 2);

            // Configure indexes
            entity.HasIndex(feh => feh.AcademicEmpNr)
                .HasDatabaseName("IX_FacultyEmploymentHistory_Academic");

            entity.HasIndex(feh => feh.IsCurrentPosition)
                .HasDatabaseName("IX_FacultyEmploymentHistory_Current");

            entity.HasIndex(feh => new { feh.AcademicEmpNr, feh.StartDate })
                .HasDatabaseName("IX_FacultyEmploymentHistory_Academic_StartDate");
        });

        // Configure FacultyPromotion
        modelBuilder.Entity<FacultyPromotion>(entity =>
        {
            entity.HasKey(fp => fp.Id);

            // Configure relationships
            entity.HasOne(fp => fp.Academic)
                .WithMany(a => a.Promotions)
                .HasForeignKey(fp => fp.AcademicEmpNr)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(fp => fp.FromRank)
                .WithMany()
                .HasForeignKey(fp => fp.FromRankCode)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(fp => fp.ToRank)
                .WithMany()
                .HasForeignKey(fp => fp.ToRankCode)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure indexes
            entity.HasIndex(fp => fp.AcademicEmpNr)
                .HasDatabaseName("IX_FacultyPromotion_Academic");

            entity.HasIndex(fp => fp.Status)
                .HasDatabaseName("IX_FacultyPromotion_Status");

            entity.HasIndex(fp => fp.EffectiveDate)
                .HasDatabaseName("IX_FacultyPromotion_EffectiveDate");
        });

        // Configure ResearchArea
        modelBuilder.Entity<ResearchArea>(entity =>
        {
            entity.HasKey(ra => ra.Code);

            // Configure self-referencing relationship
            entity.HasOne(ra => ra.ParentArea)
                .WithMany(ra => ra.ChildAreas)
                .HasForeignKey(ra => ra.ParentAreaCode)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure indexes
            entity.HasIndex(ra => ra.IsActive)
                .HasDatabaseName("IX_ResearchArea_IsActive");

            entity.HasIndex(ra => ra.PrimaryDiscipline)
                .HasDatabaseName("IX_ResearchArea_Discipline");

            entity.HasIndex(ra => ra.ParentAreaCode)
                .HasDatabaseName("IX_ResearchArea_Parent");
        });

        // Configure FacultyExpertise
        modelBuilder.Entity<FacultyExpertise>(entity =>
        {
            entity.HasKey(fe => fe.Id);

            // Configure composite unique constraint
            entity.HasIndex(fe => new { fe.AcademicEmpNr, fe.ResearchAreaCode })
                .IsUnique()
                .HasDatabaseName("IX_FacultyExpertise_Academic_ResearchArea");

            // Configure relationships
            entity.HasOne(fe => fe.Academic)
                .WithMany(a => a.ResearchExpertise)
                .HasForeignKey(fe => fe.AcademicEmpNr)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(fe => fe.ResearchArea)
                .WithMany(ra => ra.FacultyExpertise)
                .HasForeignKey(fe => fe.ResearchAreaCode)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure indexes
            entity.HasIndex(fe => fe.AcademicEmpNr)
                .HasDatabaseName("IX_FacultyExpertise_Academic");

            entity.HasIndex(fe => fe.IsPrimaryExpertise)
                .HasDatabaseName("IX_FacultyExpertise_Primary");

            entity.HasIndex(fe => fe.ExpertiseLevel)
                .HasDatabaseName("IX_FacultyExpertise_Level");
        });

        // Configure FacultyServiceRecord
        modelBuilder.Entity<FacultyServiceRecord>(entity =>
        {
            entity.HasKey(fsr => fsr.Id);

            // Configure relationships
            entity.HasOne(fsr => fsr.Academic)
                .WithMany(a => a.ServiceRecords)
                .HasForeignKey(fsr => fsr.AcademicEmpNr)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure decimal precision
            entity.Property(fsr => fsr.EstimatedHoursPerYear)
                .HasPrecision(8, 2);

            entity.Property(fsr => fsr.ServiceWeight)
                .HasPrecision(5, 2);

            // Configure indexes
            entity.HasIndex(fsr => fsr.AcademicEmpNr)
                .HasDatabaseName("IX_FacultyServiceRecord_Academic");

            entity.HasIndex(fsr => fsr.IsActive)
                .HasDatabaseName("IX_FacultyServiceRecord_Active");

            entity.HasIndex(fsr => fsr.ServiceLevel)
                .HasDatabaseName("IX_FacultyServiceRecord_Level");

            entity.HasIndex(fsr => fsr.IsMajorService)
                .HasDatabaseName("IX_FacultyServiceRecord_Major");
        });

        // Configure CommitteeLeadership
        modelBuilder.Entity<CommitteeLeadership>(entity =>
        {
            entity.HasKey(cl => cl.Id);

            // Configure relationships
            entity.HasOne(cl => cl.Committee)
                .WithMany()
                .HasForeignKey(cl => cl.CommitteeName)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(cl => cl.Academic)
                .WithMany(a => a.CommitteeLeaderships)
                .HasForeignKey(cl => cl.AcademicEmpNr)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure indexes
            entity.HasIndex(cl => cl.CommitteeName)
                .HasDatabaseName("IX_CommitteeLeadership_Committee");

            entity.HasIndex(cl => cl.AcademicEmpNr)
                .HasDatabaseName("IX_CommitteeLeadership_Academic");

            entity.HasIndex(cl => cl.IsCurrent)
                .HasDatabaseName("IX_CommitteeLeadership_Current");

            entity.HasIndex(cl => new { cl.CommitteeName, cl.Position, cl.IsCurrent })
                .HasDatabaseName("IX_CommitteeLeadership_Committee_Position_Current");
        });

        // Configure FacultyProfile
        modelBuilder.Entity<FacultyProfile>(entity =>
        {
            entity.HasKey(fp => fp.Id);

            // Foreign key to Academic
            entity.HasOne(fp => fp.Academic)
                .WithOne()
                .HasForeignKey<FacultyProfile>(fp => fp.AcademicEmpNr)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure indexes
            entity.HasIndex(fp => fp.AcademicEmpNr)
                .IsUnique()
                .HasDatabaseName("IX_FacultyProfile_Academic");

            entity.HasIndex(fp => fp.IsPublicProfile)
                .HasDatabaseName("IX_FacultyProfile_Public");

            entity.HasIndex(fp => new { fp.IsPublicProfile, fp.ModifiedDate })
                .HasDatabaseName("IX_FacultyProfile_Public_LastModified");
        });

        // Configure FacultyDocument
        modelBuilder.Entity<FacultyDocument>(entity =>
        {
            entity.HasKey(fd => fd.Id);

            // Foreign key to Academic
            entity.HasOne(fd => fd.Academic)
                .WithMany()
                .HasForeignKey(fd => fd.AcademicEmpNr)
                .OnDelete(DeleteBehavior.Cascade);

            // Foreign key to FacultyProfile (optional - via Academic relationship)
            entity.HasOne(fd => fd.FacultyProfile)
                .WithMany()
                .HasForeignKey(fd => fd.AcademicEmpNr)
                .HasPrincipalKey(fp => fp.AcademicEmpNr)
                .OnDelete(DeleteBehavior.NoAction);

            // No decimal precision needed for FileSizeBytes (long)

            // Configure indexes
            entity.HasIndex(fd => fd.AcademicEmpNr)
                .HasDatabaseName("IX_FacultyDocument_Academic");

            entity.HasIndex(fd => fd.DocumentType)
                .HasDatabaseName("IX_FacultyDocument_Type");

            entity.HasIndex(fd => fd.IsCurrentVersion)
                .HasDatabaseName("IX_FacultyDocument_Current");

            entity.HasIndex(fd => new { fd.AcademicEmpNr, fd.DocumentType, fd.IsCurrentVersion })
                .HasDatabaseName("IX_FacultyDocument_Academic_Type_Current");
        });

        // Configure FacultyPublication
        modelBuilder.Entity<FacultyPublication>(entity =>
        {
            entity.HasKey(fp => fp.PublicationId);

            // Foreign key to Academic
            entity.HasOne(fp => fp.Academic)
                .WithMany()
                .HasForeignKey(fp => fp.AcademicId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure indexes
            entity.HasIndex(fp => fp.AcademicId)
                .HasDatabaseName("IX_FacultyPublication_Academic");

            entity.HasIndex(fp => fp.PublicationType)
                .HasDatabaseName("IX_FacultyPublication_Type");

            entity.HasIndex(fp => fp.PublicationYear)
                .HasDatabaseName("IX_FacultyPublication_Year");

            entity.HasIndex(fp => fp.DOI)
                .HasDatabaseName("IX_FacultyPublication_DOI");

            entity.HasIndex(fp => new { fp.AcademicId, fp.PublicationYear })
                .HasDatabaseName("IX_FacultyPublication_Academic_Year");
        });

        // Configure OfficeAssignment
        modelBuilder.Entity<OfficeAssignment>(entity =>
        {
            entity.HasKey(oa => oa.OfficeAssignmentId);

            // Foreign key to Academic
            entity.HasOne(oa => oa.Academic)
                .WithMany()
                .HasForeignKey(oa => oa.AcademicId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure decimal precision
            entity.Property(oa => oa.Latitude)
                .HasPrecision(10, 8);

            entity.Property(oa => oa.Longitude)
                .HasPrecision(11, 8);

            entity.Property(oa => oa.OfficeSize)
                .HasPrecision(8, 2);

            // Configure indexes
            entity.HasIndex(oa => oa.AcademicId)
                .HasDatabaseName("IX_OfficeAssignment_Academic");

            entity.HasIndex(oa => oa.AssignmentStatus)
                .HasDatabaseName("IX_OfficeAssignment_Status");

            entity.HasIndex(oa => new { oa.BuildingName, oa.RoomNumber })
                .HasDatabaseName("IX_OfficeAssignment_Building_Room");

            entity.HasIndex(oa => new { oa.AcademicId, oa.AssignmentStatus })
                .HasDatabaseName("IX_OfficeAssignment_Academic_Status");
        });

        // Configure Academic Rank and Promotion System Entities (Prompt 5 Task 3)

        // Configure AcademicRank
        modelBuilder.Entity<AcademicRank>(entity =>
        {
            entity.HasKey(ar => ar.Id);

            // Foreign key to Academic
            entity.HasOne(ar => ar.Academic)
                .WithMany(a => a.AcademicRanks)
                .HasForeignKey(ar => ar.AcademicEmpNr)
                .OnDelete(DeleteBehavior.Cascade);

            // Foreign key to PromotionCommittee (optional)
            entity.HasOne(ar => ar.PromotionCommittee)
                .WithMany(pc => pc.ApprovedRanks)
                .HasForeignKey(ar => ar.PromotionCommitteeId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure indexes
            entity.HasIndex(ar => ar.AcademicEmpNr)
                .HasDatabaseName("IX_AcademicRank_Academic");

            entity.HasIndex(ar => ar.RankLevel)
                .HasDatabaseName("IX_AcademicRank_RankLevel");

            entity.HasIndex(ar => ar.TenureStatus)
                .HasDatabaseName("IX_AcademicRank_TenureStatus");

            entity.HasIndex(ar => new { ar.AcademicEmpNr, ar.IsCurrentRank })
                .HasDatabaseName("IX_AcademicRank_Academic_Current");
        });

        // Configure PromotionApplication
        modelBuilder.Entity<PromotionApplication>(entity =>
        {
            entity.HasKey(pa => pa.Id);

            // Foreign key to Academic
            entity.HasOne(pa => pa.Academic)
                .WithMany(a => a.PromotionApplications)
                .HasForeignKey(pa => pa.AcademicEmpNr)
                .OnDelete(DeleteBehavior.Cascade);

            // Foreign key to PromotionCommittee (optional)
            entity.HasOne(pa => pa.PromotionCommittee)
                .WithMany(pc => pc.PromotionApplications)
                .HasForeignKey(pa => pa.PromotionCommitteeId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure indexes
            entity.HasIndex(pa => pa.AcademicEmpNr)
                .HasDatabaseName("IX_PromotionApplication_Academic");

            entity.HasIndex(pa => pa.Status)
                .HasDatabaseName("IX_PromotionApplication_Status");

            entity.HasIndex(pa => pa.RequestedRank)
                .HasDatabaseName("IX_PromotionApplication_RequestedRank");

            entity.HasIndex(pa => new { pa.AcademicYear, pa.Status })
                .HasDatabaseName("IX_PromotionApplication_Year_Status");
        });

        // Configure TenureTrack
        modelBuilder.Entity<TenureTrack>(entity =>
        {
            entity.HasKey(tt => tt.Id);

            // Foreign key to Academic
            entity.HasOne(tt => tt.Academic)
                .WithMany(a => a.TenureTracks)
                .HasForeignKey(tt => tt.AcademicEmpNr)
                .OnDelete(DeleteBehavior.Cascade);

            // Note: ProgressPercentage is a computed property and doesn't need configuration

            // Configure indexes
            entity.HasIndex(tt => tt.AcademicEmpNr)
                .HasDatabaseName("IX_TenureTrack_Academic");

            entity.HasIndex(tt => tt.TenureStatus)
                .HasDatabaseName("IX_TenureTrack_Status");

            entity.HasIndex(tt => tt.IsActive)
                .HasDatabaseName("IX_TenureTrack_Active");

            entity.HasIndex(tt => new { tt.AcademicEmpNr, tt.IsActive })
                .HasDatabaseName("IX_TenureTrack_Academic_Active");
        });

        // Configure PromotionCommittee
        modelBuilder.Entity<PromotionCommittee>(entity =>
        {
            entity.HasKey(pc => pc.Id);

            // Foreign key to Academic (Chair)
            entity.HasOne(pc => pc.Chair)
                .WithMany(a => a.ChairedPromotionCommittees)
                .HasForeignKey(pc => pc.ChairEmpNr)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure decimal precision
            entity.Property(pc => pc.VotingThreshold)
                .HasPrecision(5, 2);

            // Configure indexes
            entity.HasIndex(pc => pc.ChairEmpNr)
                .HasDatabaseName("IX_PromotionCommittee_Chair");

            entity.HasIndex(pc => pc.CommitteeType)
                .HasDatabaseName("IX_PromotionCommittee_Type");

            entity.HasIndex(pc => pc.IsActive)
                .HasDatabaseName("IX_PromotionCommittee_Active");

            entity.HasIndex(pc => new { pc.AcademicYear, pc.IsActive })
                .HasDatabaseName("IX_PromotionCommittee_Year_Active");
        });

        // Configure PromotionCommitteeMember
        modelBuilder.Entity<PromotionCommitteeMember>(entity =>
        {
            entity.HasKey(pcm => pcm.Id);

            // Foreign key to PromotionCommittee
            entity.HasOne(pcm => pcm.PromotionCommittee)
                .WithMany(pc => pc.Members)
                .HasForeignKey(pcm => pcm.PromotionCommitteeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Foreign key to Academic
            entity.HasOne(pcm => pcm.Academic)
                .WithMany(a => a.PromotionCommitteeMemberships)
                .HasForeignKey(pcm => pcm.EmpNr)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure decimal precision
            entity.Property(pcm => pcm.AttendanceRate)
                .HasPrecision(5, 2);

            // Configure indexes
            entity.HasIndex(pcm => pcm.PromotionCommitteeId)
                .HasDatabaseName("IX_PromotionCommitteeMember_Committee");

            entity.HasIndex(pcm => pcm.EmpNr)
                .HasDatabaseName("IX_PromotionCommitteeMember_Academic");

            entity.HasIndex(pcm => pcm.IsActive)
                .HasDatabaseName("IX_PromotionCommitteeMember_Active");

            entity.HasIndex(pcm => new { pcm.PromotionCommitteeId, pcm.IsActive })
                .HasDatabaseName("IX_PromotionCommitteeMember_Committee_Active");
        });

        // Configure PromotionWorkflowStep
        modelBuilder.Entity<PromotionWorkflowStep>(entity =>
        {
            entity.HasKey(pws => pws.Id);

            // Foreign key to PromotionApplication
            entity.HasOne(pws => pws.PromotionApplication)
                .WithMany(pa => pa.WorkflowSteps)
                .HasForeignKey(pws => pws.PromotionApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Foreign key to PromotionCommittee (optional)
            entity.HasOne(pws => pws.PromotionCommittee)
                .WithMany()
                .HasForeignKey(pws => pws.PromotionCommitteeId)
                .OnDelete(DeleteBehavior.SetNull);

            // Foreign key to Academic (AssignedReviewer, optional)
            entity.HasOne(pws => pws.AssignedReviewer)
                .WithMany(a => a.AssignedPromotionSteps)
                .HasForeignKey(pws => pws.AssignedReviewerEmpNr)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure decimal precision
            entity.Property(pws => pws.CompletionPercentage)
                .HasPrecision(5, 2);

            // Configure indexes
            entity.HasIndex(pws => pws.PromotionApplicationId)
                .HasDatabaseName("IX_PromotionWorkflowStep_Application");

            entity.HasIndex(pws => pws.Status)
                .HasDatabaseName("IX_PromotionWorkflowStep_Status");

            entity.HasIndex(pws => pws.StepOrder)
                .HasDatabaseName("IX_PromotionWorkflowStep_Order");

            entity.HasIndex(pws => new { pws.PromotionApplicationId, pws.StepOrder })
                .HasDatabaseName("IX_PromotionWorkflowStep_Application_Order");
        });

        // Configure TenureMilestone
        modelBuilder.Entity<TenureMilestone>(entity =>
        {
            entity.HasKey(tm => tm.Id);

            // Foreign key to TenureTrack
            entity.HasOne(tm => tm.TenureTrack)
                .WithMany(tt => tt.Milestones)
                .HasForeignKey(tm => tm.TenureTrackId)
                .OnDelete(DeleteBehavior.Cascade);

            // Foreign key to Academic (Reviewer, optional)
            entity.HasOne(tm => tm.Reviewer)
                .WithMany(a => a.ReviewedTenureMilestones)
                .HasForeignKey(tm => tm.ReviewerEmpNr)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure indexes
            entity.HasIndex(tm => tm.TenureTrackId)
                .HasDatabaseName("IX_TenureMilestone_TenureTrack");

            entity.HasIndex(tm => tm.Status)
                .HasDatabaseName("IX_TenureMilestone_Status");

            entity.HasIndex(tm => tm.TenureYear)
                .HasDatabaseName("IX_TenureMilestone_Year");

            entity.HasIndex(tm => new { tm.TenureTrackId, tm.MilestoneOrder })
                .HasDatabaseName("IX_TenureMilestone_Track_Order");
        });

        // Configure PromotionVote
        modelBuilder.Entity<PromotionVote>(entity =>
        {
            entity.HasKey(pv => pv.Id);

            // Foreign key to PromotionApplication
            entity.HasOne(pv => pv.PromotionApplication)
                .WithMany(pa => pa.Votes)
                .HasForeignKey(pv => pv.PromotionApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Foreign key to PromotionCommitteeMember
            entity.HasOne(pv => pv.PromotionCommitteeMember)
                .WithMany(pcm => pcm.Votes)
                .HasForeignKey(pv => pv.PromotionCommitteeMemberId)
                .OnDelete(DeleteBehavior.Cascade);

            // Foreign key to Academic (Voter)
            entity.HasOne(pv => pv.Voter)
                .WithMany(a => a.PromotionVotes)
                .HasForeignKey(pv => pv.VoterEmpNr)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure decimal precision
            entity.Property(pv => pv.VoteWeight)
                .HasPrecision(3, 2);

            entity.Property(pv => pv.OverallScore)
                .HasPrecision(4, 2);

            entity.Property(pv => pv.ReviewTimeHours)
                .HasPrecision(5, 2);

            // Configure indexes
            entity.HasIndex(pv => pv.PromotionApplicationId)
                .HasDatabaseName("IX_PromotionVote_Application");

            entity.HasIndex(pv => pv.VoterEmpNr)
                .HasDatabaseName("IX_PromotionVote_Voter");

            entity.HasIndex(pv => pv.Vote)
                .HasDatabaseName("IX_PromotionVote_Vote");

            entity.HasIndex(pv => new { pv.PromotionApplicationId, pv.Vote })
                .HasDatabaseName("IX_PromotionVote_Application_Vote");
        });

        // ========== Prompt 5 Task 4: Department Assignment and Administration Entity Configurations ==========

        // Configure DepartmentChair relationships
        modelBuilder.Entity<DepartmentChair>()
            .HasOne(dc => dc.Department)
            .WithMany(d => d.DepartmentChairs)
            .HasForeignKey(dc => dc.DepartmentName)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DepartmentChair>()
            .HasOne(dc => dc.Faculty)
            .WithMany(a => a.DepartmentChairAssignments)
            .HasForeignKey(dc => dc.FacultyEmpNr)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure CommitteeChair relationships  
        modelBuilder.Entity<CommitteeChair>()
            .HasOne(cc => cc.Committee)
            .WithMany(c => c.CommitteeChairs)
            .HasForeignKey(cc => cc.CommitteeName)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CommitteeChair>()
            .HasOne(cc => cc.Chair)
            .WithMany(a => a.CommitteeChairAssignments)
            .HasForeignKey(cc => cc.ChairEmpNr)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure CommitteeMemberAssignment relationships
        modelBuilder.Entity<CommitteeMemberAssignment>()
            .HasOne(cma => cma.Committee)
            .WithMany(c => c.CommitteeMemberAssignments)
            .HasForeignKey(cma => cma.CommitteeName)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CommitteeMemberAssignment>()
            .HasOne(cma => cma.Member)
            .WithMany(a => a.CommitteeMemberAssignments)
            .HasForeignKey(cma => cma.MemberEmpNr)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure AdministrativeAssignment relationships
        modelBuilder.Entity<AdministrativeAssignment>()
            .HasOne(aa => aa.Role)
            .WithMany(ar => ar.Assignments)
            .HasForeignKey(aa => aa.RoleCode)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AdministrativeAssignment>()
            .HasOne(aa => aa.Assignee)
            .WithMany(a => a.AdministrativeAssignments)
            .HasForeignKey(aa => aa.AssigneeEmpNr)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure FacultySearchCommittee relationships
        modelBuilder.Entity<FacultySearchCommittee>()
            .HasOne(fsc => fsc.Department)
            .WithMany(d => d.FacultySearchCommittees)
            .HasForeignKey(fsc => fsc.DepartmentName)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FacultySearchCommittee>()
            .HasOne(fsc => fsc.Chair)
            .WithMany(a => a.FacultySearchCommitteesAsChair)
            .HasForeignKey(fsc => fsc.ChairEmpNr)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure FacultySearchCommitteeMember relationships
        modelBuilder.Entity<FacultySearchCommitteeMember>()
            .HasOne(fscm => fscm.SearchCommittee)
            .WithMany(fsc => fsc.Members)
            .HasForeignKey(fscm => fscm.SearchCommitteeCode)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FacultySearchCommitteeMember>()
            .HasOne(fscm => fscm.Member)
            .WithMany(a => a.FacultySearchCommitteeMemberships)
            .HasForeignKey(fscm => fscm.MemberEmpNr)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure DepartmentalService relationships
        modelBuilder.Entity<DepartmentalService>()
            .HasOne(ds => ds.Faculty)
            .WithMany(a => a.DepartmentalServices)
            .HasForeignKey(ds => ds.FacultyEmpNr)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<DepartmentalService>()
            .HasOne(ds => ds.Department)
            .WithMany(d => d.DepartmentalServices)
            .HasForeignKey(ds => ds.DepartmentName)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure ServiceLoadSummary relationships
        modelBuilder.Entity<ServiceLoadSummary>()
            .HasOne(sls => sls.Faculty)
            .WithMany(a => a.ServiceLoadSummaries)
            .HasForeignKey(sls => sls.FacultyEmpNr)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ServiceLoadSummary>()
            .HasOne(sls => sls.Department)
            .WithMany(d => d.ServiceLoadSummaries)
            .HasForeignKey(sls => sls.DepartmentName)
            .OnDelete(DeleteBehavior.SetNull);

        // Apply Course Catalog Management configurations (Prompt 6 Task 2)
        modelBuilder.ApplyConfiguration(new CourseCatalogConfiguration());
        modelBuilder.ApplyConfiguration(new CourseApprovalWorkflowConfiguration());
        modelBuilder.ApplyConfiguration(new ApprovalStepConfiguration());
        modelBuilder.ApplyConfiguration(new ApprovalAttachmentConfiguration());
        modelBuilder.ApplyConfiguration(new CatalogApprovalConfiguration());
        modelBuilder.ApplyConfiguration(new LearningOutcomeConfiguration());
        modelBuilder.ApplyConfiguration(new OutcomeAssessmentConfiguration());
        modelBuilder.ApplyConfiguration(new AssessmentResultConfiguration());
        modelBuilder.ApplyConfiguration(new CatalogPublicationConfiguration());
        modelBuilder.ApplyConfiguration(new PublicationDistributionConfiguration());
        modelBuilder.ApplyConfiguration(new PublicationAccessLogConfiguration());
        modelBuilder.ApplyConfiguration(new CatalogVersionConfiguration());
        modelBuilder.ApplyConfiguration(new VersionChangeConfiguration());
        modelBuilder.ApplyConfiguration(new VersionComparisonConfiguration());
        modelBuilder.ApplyConfiguration(new ComparisonDetailConfiguration());
    }
}