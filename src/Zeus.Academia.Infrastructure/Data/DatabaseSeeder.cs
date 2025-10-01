using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.Infrastructure.Data;

/// <summary>
/// Database seed data configuration for reference tables.
/// Task 5: Database Migrations - Seed data population for lookup and reference tables.
/// </summary>
public static class DatabaseSeeder
{
    /// <summary>
    /// Seeds the database with initial reference data.
    /// </summary>
    /// <param name="context">The database context.</param>
    public static async Task SeedAsync(AcademiaDbContext context)
    {
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Seed Access Levels
        if (!await context.AccessLevels.AnyAsync())
        {
            await SeedAccessLevelsAsync(context);
        }

        // Seed Universities
        if (!await context.Universities.AnyAsync())
        {
            await SeedUniversitiesAsync(context);
        }

        // Seed Degrees
        if (!await context.Degrees.AnyAsync())
        {
            await SeedDegreesAsync(context);
        }

        // Seed Ranks
        if (!await context.Ranks.AnyAsync())
        {
            await SeedRanksAsync(context);
        }

        // Seed Subjects
        if (!await context.Subjects.AnyAsync())
        {
            await SeedSubjectsAsync(context);
        }

        // Seed Buildings
        if (!await context.Buildings.AnyAsync())
        {
            await SeedBuildingsAsync(context);
        }

        // Save all changes
        await context.SaveChangesAsync();
    }

    private static async Task SeedAccessLevelsAsync(AcademiaDbContext context)
    {
        var accessLevels = new[]
        {
            new AccessLevel
            {
                Code = "ADMIN",
                Name = "System Administrator",
                Description = "Full system access with all administrative privileges",
                Level = 1,
                IsActive = true,
                CanRead = true,
                CanCreate = true,
                CanUpdate = true,
                CanDelete = true,
                CanExecute = true,
                CanModifySystem = true,
                CanAccessFinancial = true,
                CanAccessStudentRecords = true,
                CanAccessFacultyRecords = true,
                CanGenerateReports = true,
                Category = "System",
                MaxConcurrentSessions = 5,
                SessionTimeoutMinutes = 480,
                RequiresTwoFactor = true,
                RequiresPasswordChange = true,
                PasswordChangeFrequencyDays = 90,
                CreatedBy = "System",
                ModifiedBy = "System"
            },
            new AccessLevel
            {
                Code = "FACULT1",
                Name = "Faculty Level 1",
                Description = "Standard faculty access with course and grade management",
                Level = 10,
                IsActive = true,
                CanRead = true,
                CanCreate = true,
                CanUpdate = true,
                CanDelete = false,
                CanExecute = false,
                CanModifySystem = false,
                CanAccessFinancial = false,
                CanAccessStudentRecords = true,
                CanAccessFacultyRecords = false,
                CanGenerateReports = true,
                Category = "Faculty",
                MaxConcurrentSessions = 3,
                SessionTimeoutMinutes = 240,
                RequiresTwoFactor = false,
                RequiresPasswordChange = true,
                PasswordChangeFrequencyDays = 120,
                CreatedBy = "System",
                ModifiedBy = "System"
            },
            new AccessLevel
            {
                Code = "STAFF",
                Name = "Administrative Staff",
                Description = "Staff access for administrative functions",
                Level = 20,
                IsActive = true,
                CanRead = true,
                CanCreate = true,
                CanUpdate = true,
                CanDelete = false,
                CanExecute = false,
                CanModifySystem = false,
                CanAccessFinancial = false,
                CanAccessStudentRecords = true,
                CanAccessFacultyRecords = false,
                CanGenerateReports = false,
                Category = "Staff",
                MaxConcurrentSessions = 2,
                SessionTimeoutMinutes = 120,
                RequiresTwoFactor = false,
                RequiresPasswordChange = true,
                PasswordChangeFrequencyDays = 90,
                CreatedBy = "System",
                ModifiedBy = "System"
            },
            new AccessLevel
            {
                Code = "STUDENT",
                Name = "Student",
                Description = "Student access for academic information and enrollment",
                Level = 50,
                IsActive = true,
                CanRead = true,
                CanCreate = false,
                CanUpdate = false,
                CanDelete = false,
                CanExecute = false,
                CanModifySystem = false,
                CanAccessFinancial = false,
                CanAccessStudentRecords = false,
                CanAccessFacultyRecords = false,
                CanGenerateReports = false,
                Category = "Student",
                MaxConcurrentSessions = 2,
                SessionTimeoutMinutes = 60,
                RequiresTwoFactor = false,
                RequiresPasswordChange = false,
                PasswordChangeFrequencyDays = 180,
                CreatedBy = "System",
                ModifiedBy = "System"
            },
            new AccessLevel
            {
                Code = "GUEST",
                Name = "Guest Access",
                Description = "Limited guest access for public information",
                Level = 99,
                IsActive = true,
                CanRead = true,
                CanCreate = false,
                CanUpdate = false,
                CanDelete = false,
                CanExecute = false,
                CanModifySystem = false,
                CanAccessFinancial = false,
                CanAccessStudentRecords = false,
                CanAccessFacultyRecords = false,
                CanGenerateReports = false,
                Category = "Guest",
                MaxConcurrentSessions = 1,
                SessionTimeoutMinutes = 30,
                RequiresTwoFactor = false,
                RequiresPasswordChange = false,
                CreatedBy = "System",
                ModifiedBy = "System"
            }
        };

        await context.AccessLevels.AddRangeAsync(accessLevels);
    }

    private static async Task SeedUniversitiesAsync(AcademiaDbContext context)
    {
        var universities = new[]
        {
            new University
            {
                Code = "ZAU",
                Name = "Zeus Academia University",
                Location = "1234 Education Boulevard",
                City = "Academic City",
                StateProvince = "Knowledge State",
                Country = "United States",
                PostalCode = "12345",
                PhoneNumber = "555-ACADEMIA",
                Website = "https://www.zeusacademia.edu",
                EstablishedYear = 1965,
                AccreditationStatus = "Fully Accredited",
                StudentEnrollment = 15000,
                IsActive = true,
                CreatedBy = "System",
                ModifiedBy = "System"
            }
        };

        await context.Universities.AddRangeAsync(universities);
    }

    private static async Task SeedDegreesAsync(AcademiaDbContext context)
    {
        var degrees = new[]
        {
            new Degree
            {
                Code = "BS-CS",
                Title = "Bachelor of Science in Computer Science",
                Level = "Bachelor",
                Description = "Comprehensive undergraduate program in computer science and software engineering",
                TotalCreditHours = 120,
                DurationYears = 4.0m,
                IsActive = true,
                PrimaryDepartment = "COMP-SCI",
                Specialization = "General Computer Science",
                MinimumGPA = 2.0m,
                CreatedBy = "System",
                ModifiedBy = "System"
            },
            new Degree
            {
                Code = "MS-CS",
                Title = "Master of Science in Computer Science",
                Level = "Master",
                Description = "Advanced graduate program in computer science with research focus",
                TotalCreditHours = 36,
                DurationYears = 2.0m,
                IsActive = true,
                PrimaryDepartment = "COMP-SCI",
                Specialization = "Advanced Computer Science",
                MinimumGPA = 3.0m,
                CreatedBy = "System",
                ModifiedBy = "System"
            },
            new Degree
            {
                Code = "PHD-CS",
                Title = "Doctor of Philosophy in Computer Science",
                Level = "Doctoral",
                Description = "Research-intensive doctoral program in computer science",
                TotalCreditHours = 72,
                DurationYears = 5.0m,
                IsActive = true,
                PrimaryDepartment = "COMP-SCI",
                Specialization = "Computer Science Research",
                MinimumGPA = 3.5m,
                CreatedBy = "System",
                ModifiedBy = "System"
            },
            new Degree
            {
                Code = "BS-MATH",
                Title = "Bachelor of Science in Mathematics",
                Level = "Bachelor",
                Description = "Comprehensive undergraduate program in mathematics",
                TotalCreditHours = 120,
                DurationYears = 4.0m,
                IsActive = true,
                PrimaryDepartment = "MATH",
                Specialization = "General Mathematics",
                MinimumGPA = 2.0m,
                CreatedBy = "System",
                ModifiedBy = "System"
            }
        };

        await context.Degrees.AddRangeAsync(degrees);
    }

    private static async Task SeedRanksAsync(AcademiaDbContext context)
    {
        var ranks = new[]
        {
            new Rank
            {
                Code = "PROF",
                Title = "Professor",
                Level = 1,
                Description = "Senior faculty member with tenure eligibility",
                MinSalary = 80000m,
                MaxSalary = 150000m,
                Category = "Faculty",
                RequiresTenure = true,
                IsActive = true,
                MinExperienceYears = 10,
                MinDegreeLevel = "Doctoral",
                AllowsTeaching = true,
                AllowsResearch = true,
                CreatedBy = "System",
                ModifiedBy = "System"
            },
            new Rank
            {
                Code = "ASSOCPROF",
                Title = "Associate Professor",
                Level = 2,
                Description = "Mid-level faculty member with tenure eligibility",
                MinSalary = 65000m,
                MaxSalary = 90000m,
                Category = "Faculty",
                RequiresTenure = true,
                IsActive = true,
                MinExperienceYears = 5,
                MinDegreeLevel = "Doctoral",
                AllowsTeaching = true,
                AllowsResearch = true,
                CreatedBy = "System",
                ModifiedBy = "System"
            },
            new Rank
            {
                Code = "ASSTPROF",
                Title = "Assistant Professor",
                Level = 3,
                Description = "Entry-level tenure-track faculty member",
                MinSalary = 55000m,
                MaxSalary = 75000m,
                Category = "Faculty",
                RequiresTenure = false,
                IsActive = true,
                MinExperienceYears = 0,
                MinDegreeLevel = "Doctoral",
                AllowsTeaching = true,
                AllowsResearch = true,
                CreatedBy = "System",
                ModifiedBy = "System"
            },
            new Rank
            {
                Code = "LECTURER",
                Title = "Lecturer",
                Level = 4,
                Description = "Teaching-focused faculty member",
                MinSalary = 45000m,
                MaxSalary = 65000m,
                Category = "Faculty",
                RequiresTenure = false,
                IsActive = true,
                MinExperienceYears = 0,
                MinDegreeLevel = "Master",
                AllowsTeaching = true,
                AllowsResearch = false,
                CreatedBy = "System",
                ModifiedBy = "System"
            }
        };

        await context.Ranks.AddRangeAsync(ranks);
    }

    private static async Task SeedSubjectsAsync(AcademiaDbContext context)
    {
        var subjects = new[]
        {
            new Subject
            {
                Code = "CS101",
                Title = "Introduction to Computer Science",
                Description = "Fundamentals of computer science and programming",
                CreditHours = 3,
                Level = "Undergraduate",
                IsActive = true,
                DepartmentName = "COMP-SCI",
                Prerequisites = null,
                TypicalSemester = "Fall",
                MaxEnrollment = 150,
                CreatedBy = "System",
                ModifiedBy = "System"
            },
            new Subject
            {
                Code = "CS201",
                Title = "Data Structures and Algorithms",
                Description = "Advanced data structures and algorithm analysis",
                CreditHours = 3,
                Level = "Undergraduate",
                IsActive = true,
                DepartmentName = "COMP-SCI", 
                Prerequisites = "CS101",
                TypicalSemester = "Spring",
                MaxEnrollment = 100,
                CreatedBy = "System",
                ModifiedBy = "System"
            },
            new Subject
            {
                Code = "MATH101",
                Title = "Calculus I",
                Description = "Introduction to differential calculus",
                CreditHours = 4,
                Level = "Undergraduate",
                IsActive = true,
                DepartmentName = "MATH",
                Prerequisites = null,
                TypicalSemester = "Fall",
                MaxEnrollment = 200,
                CreatedBy = "System",
                ModifiedBy = "System"
            },
            new Subject
            {
                Code = "MATH201",
                Title = "Calculus II",
                Description = "Integral calculus and series",
                CreditHours = 4,
                Level = "Undergraduate",
                IsActive = true,
                DepartmentName = "MATH",
                Prerequisites = "MATH101",
                TypicalSemester = "Spring",
                MaxEnrollment = 150,
                CreatedBy = "System",
                ModifiedBy = "System"
            }
        };

        await context.Subjects.AddRangeAsync(subjects);
    }

    private static async Task SeedBuildingsAsync(AcademiaDbContext context)
    {
        var buildings = new[]
        {
            new Building
            {
                Code = "ADMIN",
                Name = "Administration Building",
                Description = "Main administrative offices and student services",
                Address = "100 Campus Drive",
                NumberOfFloors = 3,
                ConstructionYear = 1985,
                TotalAreaSqFt = 45000m,
                IsActive = true,
                HasElevator = true,
                IsAccessible = true,
                BuildingType = "Administrative",
                PhoneNumber = "555-0100",
                BuildingManager = "John Smith",
                EmergencyContact = "Campus Security: 555-0911",
                CreatedBy = "System",
                ModifiedBy = "System"
            },
            new Building
            {
                Code = "SCI",
                Name = "Science Building",
                Description = "Laboratories and classrooms for science programs",
                Address = "200 Research Lane",
                NumberOfFloors = 4,
                ConstructionYear = 1992,
                TotalAreaSqFt = 75000m,
                IsActive = true,
                HasElevator = true,
                IsAccessible = true,
                BuildingType = "Academic",
                PhoneNumber = "555-0200",
                BuildingManager = "Dr. Sarah Wilson",
                EmergencyContact = "Campus Security: 555-0911",
                CreatedBy = "System",
                ModifiedBy = "System"
            },
            new Building
            {
                Code = "LIB",
                Name = "University Library",
                Description = "Main library and study facility",
                Address = "300 Knowledge Ave",
                NumberOfFloors = 5,
                ConstructionYear = 1978,
                TotalAreaSqFt = 85000m,
                IsActive = true,
                HasElevator = true,
                IsAccessible = true,
                BuildingType = "Library",
                PhoneNumber = "555-0300",
                BuildingManager = "Margaret Brown",
                EmergencyContact = "Campus Security: 555-0911",
                CreatedBy = "System",
                ModifiedBy = "System"
            }
        };

        await context.Buildings.AddRangeAsync(buildings);
    }
}