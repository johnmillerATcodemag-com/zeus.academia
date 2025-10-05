using Serilog;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

// Minimal API for smoke testing - includes essential endpoints
var builder = WebApplication.CreateBuilder(args);

// Configure Serilog for logging
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("=== ZEUS ACADEMIA API - MINIMAL VERSION FOR TESTING ===");
    Log.Information("Starting minimal API with essential endpoints for smoke testing...");

    // Clear existing logging providers since we're using Serilog
    builder.Logging.ClearProviders();

    // Add CORS for frontend integration - more permissive for testing
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.SetIsOriginAllowed(origin => true)
                  .WithOrigins("http://localhost:3000", "http://localhost:5173", "http://localhost:5174", "http://127.0.0.1:3000")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
    });

    // Add controllers and JSON serialization
    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        });

    // Add basic API versioning support
    builder.Services.AddApiVersioning(options =>
    {
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.DefaultApiVersion = new ApiVersion(1, 0);
    });

    var app = builder.Build();

    Log.Information("App built successfully, configuring pipeline...");

    // Enable CORS
    app.UseCors("AllowAll");

    // Add routing
    app.UseRouting();

    // Map essential endpoints for smoke testing
    app.MapGet("/", () => new
    {
        Message = "Zeus Academia API - Minimal Version",
        Status = "Running",
        Version = "1.0-minimal",
        Timestamp = DateTime.UtcNow
    });

    app.MapGet("/health", () => new
    {
        Status = "Healthy",
        Service = "Zeus Academia API",
        Version = "1.0-minimal",
        Timestamp = DateTime.UtcNow,
        Uptime = TimeSpan.FromMilliseconds(Environment.TickCount64).ToString()
    });

    // Authentication endpoints (mock responses for testing)
    app.MapPost("/api/auth/login", ([FromBody] dynamic loginData) => new
    {
        user = new
        {
            id = "1",
            email = "prof@university.edu",
            firstName = "Jane",
            lastName = "Smith",
            role = "professor",
            department = "Computer Science",
            title = "Professor",
            permissions = new[] { "view_courses", "manage_course_content", "manage_grades" },
            officeLocation = "Engineering 301"
        },
        token = "mock-jwt-token-for-testing",
        refreshToken = "mock-refresh-token-for-testing",
        expiresIn = 3600
    });

    app.MapPost("/api/auth/register", ([FromBody] dynamic registerData) => new
    {
        Success = true,
        Message = "Mock registration successful",
        UserId = 123,
        Timestamp = DateTime.UtcNow
    });

    // Course endpoints (mock responses for testing)
    app.MapGet("/api/courses", ([FromQuery] int page = 1, [FromQuery] int size = 10) => Results.Ok(new
    {
        Data = new[]
        {
            new { Id = 1, Code = "CS101", Title = "Introduction to Computer Science", Credits = 3, Department = "Computer Science" },
            new { Id = 2, Code = "MATH201", Title = "Calculus I", Credits = 4, Department = "Mathematics" },
            new { Id = 3, Code = "ENG101", Title = "English Composition", Credits = 3, Department = "English" }
        },
        Page = page,
        Size = size,
        Total = 3,
        TotalPages = 1
    }));

    app.MapGet("/api/courses/paginated", ([FromQuery] int page = 1, [FromQuery] int size = 10) => new
    {
        Items = new[]
        {
            new { Id = 1, Code = "CS101", Title = "Introduction to Computer Science", Credits = 3 },
            new { Id = 2, Code = "MATH201", Title = "Calculus I", Credits = 4 }
        },
        PageNumber = page,
        PageSize = size,
        TotalCount = 2,
        TotalPages = 1,
        HasNextPage = false,
        HasPreviousPage = false
    });

    app.MapGet("/api/courses/search", ([FromQuery] string? q = "", [FromQuery] string? department = null) =>
    {
        var allCourses = new[]
        {
            new { Id = 1, Code = "CS101", Title = "Introduction to Computer Science", Credits = 3, Department = "Computer Science", Relevance = 0.95 },
            new { Id = 2, Code = "MATH201", Title = "Calculus I", Credits = 4, Department = "Mathematics", Relevance = 0.90 },
            new { Id = 3, Code = "ENG101", Title = "English Composition", Credits = 3, Department = "English", Relevance = 0.85 },
            new { Id = 4, Code = "PHYS201", Title = "Physics I", Credits = 4, Department = "Physics", Relevance = 0.80 }
        };

        var results = new List<dynamic>();

        foreach (var course in allCourses)
        {
            bool matchesQuery = string.IsNullOrEmpty(q) ||
                               course.Title.ToLower().Contains(q.ToLower()) ||
                               course.Code.ToLower().Contains(q.ToLower());

            bool matchesDepartment = string.IsNullOrEmpty(department) ||
                                   course.Department.ToLower() == department.ToLower();

            if (matchesQuery && matchesDepartment)
            {
                results.Add(course);
            }
        }

        return Results.Ok(new
        {
            Query = q,
            Department = department,
            Results = results.ToArray(),
            Count = results.Count
        });
    });

    // Individual course details
    app.MapGet("/api/courses/{id}", ([FromRoute] int id) =>
    {
        if (id == 1)
        {
            return Results.Ok(new
            {
                Id = 1,
                Code = "CS101",
                Title = "Introduction to Computer Science",
                Credits = 3,
                Description = "Comprehensive introduction to computer science fundamentals including programming concepts, problem-solving techniques, algorithm design, and data structures.",
                Prerequisites = new[] { "MATH100 - College Algebra" },
                Instructor = "Dr. Sarah Smith",
                InstructorEmail = "s.smith@zeus.edu",
                EnrollmentStatus = "available",
                MaxEnrollment = 30,
                EnrolledStudents = 25,
                Department = "Computer Science",
                Term = "Fall 2025",
                Schedule = new[] {
                    new { DayOfWeek = "Monday", StartTime = "09:00", EndTime = "10:30", Location = "CS Building 101" },
                    new { DayOfWeek = "Wednesday", StartTime = "09:00", EndTime = "10:30", Location = "CS Building 101" }
                }
            });
        }
        else if (id == 2)
        {
            return Results.Ok(new
            {
                Id = 2,
                Code = "MATH201",
                Title = "Calculus I",
                Credits = 4,
                Description = "Introduction to differential and integral calculus including limits, derivatives, applications of derivatives, and integrals.",
                Prerequisites = new[] { "MATH150 - Pre-Calculus" },
                Instructor = "Prof. Michael Johnson",
                InstructorEmail = "m.johnson@zeus.edu",
                EnrollmentStatus = "available",
                MaxEnrollment = 40,
                EnrolledStudents = 35,
                Department = "Mathematics",
                Term = "Fall 2025",
                Schedule = new[] {
                    new { DayOfWeek = "Tuesday", StartTime = "11:00", EndTime = "12:30", Location = "Math Building 201" },
                    new { DayOfWeek = "Thursday", StartTime = "11:00", EndTime = "12:30", Location = "Math Building 201" }
                }
            });
        }
        else
        {
            return Results.NotFound(new
            {
                Error = $"Course with ID {id} not found",
                CourseId = id,
                Message = "Please check the course ID and try again"
            });
        }
    });

    // Student Profile endpoints
    app.MapGet("/api/student/profile", () => new
    {
        Id = 1,
        StudentId = "STU-2024-001",
        FirstName = "John",
        LastName = "Doe",
        Email = "john.doe@zeus.edu",
        Phone = "(555) 123-4567",
        DateOfBirth = "1998-05-15",
        EnrollmentDate = "2022-08-15",
        Gpa = 3.75,
        Status = "Active",
        Address = new
        {
            Street = "123 College Ave",
            City = "University City",
            State = "CA",
            ZipCode = "90210",
            Country = "USA"
        },
        Major = "Computer Science",
        AcademicLevel = "Junior"
    });

    app.MapPut("/api/student/profile", ([FromBody] dynamic profileData) => new
    {
        Success = true,
        Message = "Profile updated successfully",
        UpdatedAt = DateTime.UtcNow
    });

    // Student Enrollments
    app.MapGet("/api/student/enrollments", () => new
    {
        Enrollments = new[] {
            new {
                Id = 1,
                CourseId = 1,
                Course = new { Code = "CS101", Title = "Introduction to Computer Science", Credits = 3 },
                EnrollmentDate = "2024-08-15",
                Status = "Enrolled",
                Grade = (string?)null,
                DropDeadline = "2024-09-15"
            },
            new {
                Id = 2,
                CourseId = 2,
                Course = new { Code = "MATH201", Title = "Calculus I", Credits = 4 },
                EnrollmentDate = "2024-08-15",
                Status = "Enrolled",
                Grade = (string?)null,
                DropDeadline = "2024-09-15"
            }
        },
        TotalCredits = 7,
        Semester = "Fall 2024"
    });

    // Enrollment actions
    app.MapPost("/api/student/enroll/{courseId}", ([FromRoute] string courseId) => new
    {
        Success = true,
        Message = $"Successfully enrolled in course {courseId}",
        EnrollmentId = Random.Shared.Next(1000, 9999),
        EnrollmentDate = DateTime.UtcNow.ToString("yyyy-MM-dd"),
        CourseId = courseId
    });

    app.MapDelete("/api/student/enroll/{courseId}", ([FromRoute] string courseId) => new
    {
        Success = true,
        Message = $"Successfully dropped course {courseId}",
        DropDate = DateTime.UtcNow.ToString("yyyy-MM-dd"),
        CourseId = courseId
    });

    // Faculty-specific course endpoints
    app.MapGet("/api/faculty/{facultyId}/courses", ([FromRoute] string facultyId) => Results.Ok(new
    {
        success = true,
        data = new[]
        {
            new {
                id = "1",
                name = "Introduction to Computer Science",
                title = "Introduction to Computer Science",
                code = "CS-101",
                section = "001",
                semester = "Fall 2025",
                year = 2025,
                credits = 3,
                description = "Fundamental concepts of computer science and programming.",
                enrollmentCount = 28,
                maxEnrollment = 30,
                status = "active",
                facultyId = facultyId,
                sections = new[] {
                    new {
                        id = "1-1",
                        courseId = "1",
                        sectionNumber = "001",
                        meetingTimes = new[] { "MWF 10:00-10:50" },
                        location = "Engineering 101",
                        capacity = 30,
                        enrolled = 28,
                        waitlist = 2,
                        status = "active"
                    }
                },
                assignments = new[] {
                    new {
                        id = "assign-1",
                        courseId = "1",
                        title = "Programming Fundamentals",
                        description = "Basic programming concepts and problem solving",
                        dueDate = DateTime.Now.AddDays(7).ToString("yyyy-MM-ddTHH:mm:ss"),
                        points = 100,
                        type = "programming",
                        status = "active"
                    }
                },
                students = new[] {
                    new {
                        id = "stud-1",
                        firstName = "John",
                        lastName = "Doe",
                        email = "john.doe@student.edu",
                        gpa = 3.5,
                        year = "Sophomore",
                        major = "Computer Science"
                    },
                    new {
                        id = "stud-2",
                        firstName = "Jane",
                        lastName = "Smith",
                        email = "jane.smith@student.edu",
                        gpa = 3.8,
                        year = "Junior",
                        major = "Computer Science"
                    }
                },
                materials = new[] {
                    new {
                        id = "mat-1",
                        courseId = "1",
                        title = "Course Syllabus",
                        type = "document",
                        url = "/content/1/syllabus.pdf",
                        uploadedAt = DateTime.Now.AddDays(-30).ToString("yyyy-MM-ddTHH:mm:ss")
                    }
                },
                announcements = new[] {
                    new {
                        id = "ann-1",
                        courseId = "1",
                        title = "Welcome to CS-101",
                        content = "Welcome to Introduction to Computer Science!",
                        priority = "normal",
                        publishedAt = DateTime.Now.AddDays(-5).ToString("yyyy-MM-ddTHH:mm:ss")
                    }
                },
                metrics = new {
                    totalEnrolled = 28,
                    totalCapacity = 30,
                    waitlistCount = 2,
                    enrollmentPercentage = 93.3,
                    averageGPA = 3.4,
                    completionRate = 94.0,
                    lastUpdated = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
                }
            },
            new {
                id = "2",
                name = "Data Structures and Algorithms",
                title = "Data Structures and Algorithms",
                code = "CS-201",
                section = "001",
                semester = "Fall 2025",
                year = 2025,
                credits = 4,
                description = "Advanced programming concepts, data structures, and algorithm design.",
                enrollmentCount = 24,
                maxEnrollment = 25,
                status = "active",
                facultyId = facultyId,
                sections = new[] {
                    new {
                        id = "2-1",
                        courseId = "2",
                        sectionNumber = "001",
                        meetingTimes = new[] { "TTH 14:00-15:30" },
                        location = "Engineering 102",
                        capacity = 25,
                        enrolled = 24,
                        waitlist = 1,
                        status = "active"
                    }
                },
                assignments = new[] {
                    new {
                        id = "assign-2",
                        courseId = "2",
                        title = "Binary Tree Implementation",
                        description = "Implement a binary search tree with insert, delete, and search operations",
                        dueDate = DateTime.Now.AddDays(10).ToString("yyyy-MM-ddTHH:mm:ss"),
                        points = 150,
                        type = "programming",
                        status = "active"
                    }
                },
                students = new[] {
                    new {
                        id = "stud-3",
                        firstName = "Mike",
                        lastName = "Johnson",
                        email = "mike.johnson@student.edu",
                        gpa = 3.2,
                        year = "Senior",
                        major = "Computer Science"
                    }
                },
                materials = new[] {
                    new {
                        id = "mat-2",
                        courseId = "2",
                        title = "Algorithm Analysis Notes",
                        type = "document",
                        url = "/content/2/algorithms.pdf",
                        uploadedAt = DateTime.Now.AddDays(-20).ToString("yyyy-MM-ddTHH:mm:ss")
                    }
                },
                announcements = new[] {
                    new {
                        id = "ann-2",
                        courseId = "2",
                        title = "Midterm Exam Schedule",
                        content = "Midterm exam will be held next Friday in the regular classroom.",
                        priority = "high",
                        publishedAt = DateTime.Now.AddDays(-2).ToString("yyyy-MM-ddTHH:mm:ss")
                    }
                },
                metrics = new {
                    totalEnrolled = 24,
                    totalCapacity = 25,
                    waitlistCount = 1,
                    enrollmentPercentage = 96.0,
                    averageGPA = 3.1,
                    completionRate = 88.0,
                    lastUpdated = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
                }
            },
            new {
                id = "3",
                name = "Software Engineering Principles",
                title = "Software Engineering Principles",
                code = "CS-301",
                section = "001",
                semester = "Fall 2025",
                year = 2025,
                credits = 3,
                description = "Software development methodologies, project management, and team collaboration.",
                enrollmentCount = 20,
                maxEnrollment = 22,
                status = "active",
                facultyId = facultyId,
                sections = new[] {
                    new {
                        id = "3-1",
                        courseId = "3",
                        sectionNumber = "001",
                        meetingTimes = new[] { "MW 16:00-17:30" },
                        location = "Engineering 201",
                        capacity = 22,
                        enrolled = 20,
                        waitlist = 0,
                        status = "active"
                    }
                },
                assignments = new[] {
                    new {
                        id = "assign-3",
                        courseId = "3",
                        title = "Team Project Proposal",
                        description = "Submit a proposal for your semester-long team project",
                        dueDate = DateTime.Now.AddDays(14).ToString("yyyy-MM-ddTHH:mm:ss"),
                        points = 75,
                        type = "project",
                        status = "active"
                    }
                },
                students = new[] {
                    new {
                        id = "stud-4",
                        firstName = "Sarah",
                        lastName = "Wilson",
                        email = "sarah.wilson@student.edu",
                        gpa = 3.9,
                        year = "Senior",
                        major = "Computer Science"
                    }
                },
                materials = new[] {
                    new {
                        id = "mat-3",
                        courseId = "3",
                        title = "Agile Development Guide",
                        type = "document",
                        url = "/content/3/agile-guide.pdf",
                        uploadedAt = DateTime.Now.AddDays(-15).ToString("yyyy-MM-ddTHH:mm:ss")
                    }
                },
                announcements = new[] {
                    new {
                        id = "ann-3",
                        courseId = "3",
                        title = "Guest Speaker Next Week",
                        content = "Industry professional will speak about software development practices.",
                        priority = "normal",
                        publishedAt = DateTime.Now.AddDays(-1).ToString("yyyy-MM-ddTHH:mm:ss")
                    }
                },
                metrics = new {
                    totalEnrolled = 20,
                    totalCapacity = 22,
                    waitlistCount = 0,
                    enrollmentPercentage = 90.9,
                    averageGPA = 3.6,
                    completionRate = 95.0,
                    lastUpdated = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
                }
            }
        }
    }));

    // Course metrics endpoint
    app.MapGet("/api/courses/{courseId}/metrics", ([FromRoute] string courseId) => Results.Ok(new
    {
        success = true,
        data = new
        {
            totalEnrolled = courseId == "1" ? 28 : courseId == "2" ? 24 : 20,
            totalCapacity = courseId == "1" ? 30 : courseId == "2" ? 25 : 22,
            waitlistCount = courseId == "1" ? 2 : courseId == "2" ? 1 : 0,
            enrollmentPercentage = courseId == "1" ? 93.3 : courseId == "2" ? 96.0 : 90.9,
            averageGPA = courseId == "1" ? 3.4 : courseId == "2" ? 3.1 : 3.6,
            completionRate = courseId == "1" ? 94.0 : courseId == "2" ? 88.0 : 95.0,
            lastUpdated = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
        }
    }));

    // Additional auth endpoint for token refresh
    app.MapPost("/api/auth/refresh", ([FromBody] dynamic refreshData) => new
    {
        Success = true,
        Token = "mock-refreshed-jwt-token",
        RefreshToken = "mock-new-refresh-token",
        ExpiresAt = DateTime.UtcNow.AddHours(1)
    });

    // API versioning test endpoints
    app.MapGet("/api/version", ([FromHeader(Name = "X-API-Version")] string? version = "1.0") => Results.Ok(new
    {
        RequestedVersion = version ?? "1.0",
        SupportedVersions = new[] { "1.0", "2.0" },
        CurrentVersion = "1.0",
        IsVersionSupported = (version == "1.0" || version == "2.0" || version == null)
    }));

    // Error handling test endpoints
    app.MapGet("/api/test/404", () => Results.NotFound(new
    {
        Error = "Resource not found",
        Code = 404,
        Timestamp = DateTime.UtcNow
    }));

    app.MapPost("/api/test/validation-error", ([FromBody] dynamic data) => Results.BadRequest(new
    {
        Error = "Validation failed",
        Code = 400,
        ValidationErrors = new[] { "Field 'name' is required", "Field 'email' must be valid" },
        Timestamp = DateTime.UtcNow
    }));

    // Performance test endpoint
    app.MapGet("/api/test/performance", async ([FromQuery] int delay = 0) =>
    {
        if (delay > 0) await Task.Delay(delay);
        return new
        {
            ProcessedAt = DateTime.UtcNow,
            DelayMs = delay,
            ProcessingTime = $"{delay}ms"
        };
    });

    // Map controllers if any exist
    app.MapControllers();

    Log.Information("All endpoints configured successfully");
    Log.Information("Starting Zeus Academia API on port 5000...");
    Log.Information("Available endpoints:");
    Log.Information("  GET  /              - API information");
    Log.Information("  GET  /health        - Health check");
    Log.Information("  POST /api/auth/login - Authentication");
    Log.Information("  GET  /api/courses   - Course listings");
    Log.Information("  GET  /api/version   - API versioning");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Zeus Academia API failed during startup: {Message}", ex.Message);
    Log.Fatal("Inner Exception: {InnerException}", ex.InnerException?.Message);
    Log.Fatal("Stack trace: {StackTrace}", ex.StackTrace);
    throw;
}
finally
{
    Log.CloseAndFlush();
}