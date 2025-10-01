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

        // Apply all entity configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AcademiaDbContext).Assembly);
    }
}