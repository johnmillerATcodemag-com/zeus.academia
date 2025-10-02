using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
                "CurrentStudentLoad >= 0"));

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
    }
}