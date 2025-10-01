using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.Infrastructure.Data;

/// <summary>
/// Entity Framework DbContext for the Zeus Academia System
/// </summary>
public class AcademiaDbContext : DbContext
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
            .OnDelete(DeleteBehavior.SetNull);

        // Configure TeachingProf relationships
        modelBuilder.Entity<TeachingProf>()
            .HasOne(tp => tp.Department)
            .WithMany(d => d.TeachingProfs)
            .HasForeignKey(tp => tp.DepartmentName)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<TeachingProf>()
            .HasOne(tp => tp.Rank)
            .WithMany(r => r.TeachingProfs)
            .HasForeignKey(tp => tp.RankCode)
            .OnDelete(DeleteBehavior.SetNull);

        // Configure Student relationships
        modelBuilder.Entity<Student>()
            .HasOne(s => s.Department)
            .WithMany(d => d.Students)
            .HasForeignKey(s => s.DepartmentName)
            .OnDelete(DeleteBehavior.SetNull);

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

        // Apply all entity configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AcademiaDbContext).Assembly);
    }
}