using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Enums;
using Zeus.Academia.Infrastructure.Services;

namespace Zeus.Academia.Tests.Services;

/// <summary>
/// Comprehensive unit tests for StudentService business logic
/// Tests the specific methods mentioned in Prompt 4 Task 1
/// </summary>
public class StudentServiceTests : IDisposable
{
    private readonly AcademiaDbContext _context;
    private readonly Mock<ILogger<StudentService>> _mockLogger;
    private readonly StudentService _studentService;

    public StudentServiceTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<AcademiaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var mockConfiguration = new Mock<IConfiguration>();
        _context = new AcademiaDbContext(options, mockConfiguration.Object);

        // Setup mock logger
        _mockLogger = new Mock<ILogger<StudentService>>();

        // Create service instance
        _studentService = new StudentService(_context, _mockLogger.Object);

        // Seed test data
        SeedTestData().Wait();
    }

    private async Task SeedTestData()
    {
        // Add Department for foreign key requirements
        var department = new Department
        {
            Name = "Computer Science",
            FullName = "Computer Science Department",
            CreatedBy = "System",
            ModifiedBy = "System"
        };
        _context.Departments.Add(department);

        // Add University for foreign key requirements
        var university = new University
        {
            Name = "Test University",
            CreatedBy = "System",
            ModifiedBy = "System"
        };
        _context.Universities.Add(university);

        // Add Subjects for enrollment testing
        var subjects = new List<Subject>
        {
            new Subject
            {
                Code = "CS101",
                Title = "Introduction to Computer Science",
                DepartmentName = "Computer Science",
                CreatedBy = "System",
                ModifiedBy = "System"
            },
            new Subject
            {
                Code = "CS102",
                Title = "Data Structures",
                DepartmentName = "Computer Science",
                CreatedBy = "System",
                ModifiedBy = "System"
            }
        };
        _context.Subjects.AddRange(subjects);

        await _context.SaveChangesAsync();
    }

    [Fact]
    public async Task CreateStudent_Should_Create_Valid_Student()
    {
        // Arrange
        var student = new Student
        {
            EmpNr = 12345,
            Name = "John Doe",
            StudentId = "STU001",
            Program = "Computer Science",
            DepartmentName = "Computer Science",
            EnrollmentDate = DateTime.UtcNow,
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };

        // Act
        var result = await _studentService.CreateStudentAsync(student);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John Doe", result.Name);
        Assert.Equal("STU001", result.StudentId);
        Assert.Equal("Computer Science", result.Program);
        Assert.Equal(EnrollmentStatus.Applied, result.EnrollmentStatus);
        Assert.Equal(AcademicStanding.NewStudent, result.AcademicStanding);
        Assert.NotNull(result.EnrollmentDate);
        Assert.Equal("System", result.CreatedBy); // Service sets this
        Assert.Equal("System", result.ModifiedBy); // Service sets this

        // Verify student was saved to database
        var savedStudent = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == "STU001");
        Assert.NotNull(savedStudent);
        Assert.Equal("John Doe", savedStudent.Name);

        // Verify logging occurred
        _mockLogger.Verify(
            logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Creating new student: John Doe")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateStudent_Should_Throw_Exception_For_Duplicate_StudentId()
    {
        // Arrange
        var existingStudent = new Student
        {
            EmpNr = 11111,
            Name = "Existing Student",
            StudentId = "STU999",
            CreatedBy = "System",
            ModifiedBy = "System"
        };
        _context.Students.Add(existingStudent);
        await _context.SaveChangesAsync();

        var duplicateStudent = new Student
        {
            EmpNr = 22222,
            Name = "Duplicate Student",
            StudentId = "STU999", // Same as existing
            CreatedBy = "System",
            ModifiedBy = "System"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _studentService.CreateStudentAsync(duplicateStudent));

        Assert.Contains("Student with ID STU999 already exists", exception.Message);
    }

    [Fact]
    public async Task CreateStudent_Should_Validate_Required_Fields()
    {
        // Arrange
        var invalidStudent = new Student
        {
            EmpNr = 33333,
            Name = "", // Invalid empty name
            StudentId = "STU002",
            CreatedBy = "System",
            ModifiedBy = "System"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _studentService.CreateStudentAsync(invalidStudent));

        Assert.Contains("Name is required", exception.Message);
    }

    [Fact]
    public async Task CreateStudent_Should_Validate_GPA_Range()
    {
        // Arrange
        var invalidStudent = new Student
        {
            EmpNr = 44444,
            Name = "Test Student",
            StudentId = "STU003",
            CumulativeGPA = 5.0m, // Invalid GPA > 4.0
            CreatedBy = "System",
            ModifiedBy = "System"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _studentService.CreateStudentAsync(invalidStudent));

        Assert.Contains("Cumulative GPA must be between 0.0 and 4.0", exception.Message);
    }

    [Fact]
    public async Task EnrollStudent_Should_Update_Status_And_Send_Notification()
    {
        // Arrange - Create a student first
        var student = new Student
        {
            EmpNr = 55555,
            Name = "Jane Smith",
            StudentId = "STU004",
            EnrollmentStatus = EnrollmentStatus.Admitted,
            CreatedBy = "System",
            ModifiedBy = "System"
        };
        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        // Act - Update enrollment status to Enrolled
        var result = await _studentService.UpdateEnrollmentStatusAsync(
            55555,
            EnrollmentStatus.Enrolled,
            "Student has completed enrollment process");

        // Assert
        Assert.True(result);

        // Verify student status was updated
        var updatedStudent = await _context.Students.FirstOrDefaultAsync(s => s.EmpNr == 55555);
        Assert.NotNull(updatedStudent);
        Assert.Equal(EnrollmentStatus.Enrolled, updatedStudent.EnrollmentStatus);
        Assert.Equal("System", updatedStudent.ModifiedBy);

        // Verify logging occurred (simulating notification sending)
        _mockLogger.Verify(
            logger => logger.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task EnrollStudent_Should_Reject_Invalid_Status_Transition()
    {
        // Arrange - Create a graduated student
        var student = new Student
        {
            EmpNr = 66666,
            Name = "Graduate Student",
            StudentId = "STU005",
            EnrollmentStatus = EnrollmentStatus.Graduated, // Final status
            CreatedBy = "System",
            ModifiedBy = "System"
        };
        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        // Act & Assert - Try to enroll a graduated student (invalid transition)
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _studentService.UpdateEnrollmentStatusAsync(66666, EnrollmentStatus.Enrolled));

        Assert.Contains("Invalid status transition from Graduated to Enrolled", exception.Message);
    }

    [Fact]
    public async Task EnrollStudent_Should_Set_Graduation_Date_When_Graduated()
    {
        // Arrange
        var student = new Student
        {
            EmpNr = 77777,
            Name = "Soon To Graduate",
            StudentId = "STU006",
            EnrollmentStatus = EnrollmentStatus.Enrolled,
            CreatedBy = "System",
            ModifiedBy = "System"
        };
        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        // Act - Graduate the student
        var result = await _studentService.UpdateEnrollmentStatusAsync(77777, EnrollmentStatus.Graduated);

        // Assert
        Assert.True(result);

        var graduatedStudent = await _context.Students.FirstOrDefaultAsync(s => s.EmpNr == 77777);
        Assert.NotNull(graduatedStudent);
        Assert.Equal(EnrollmentStatus.Graduated, graduatedStudent.EnrollmentStatus);
        Assert.NotNull(graduatedStudent.ActualGraduationDate);
        Assert.True(graduatedStudent.ActualGraduationDate <= DateTime.UtcNow);
    }

    [Fact]
    public async Task CalculateGPA_Should_Return_Correct_Value()
    {
        // Arrange - Create a student with course enrollments
        var student = new Student
        {
            EmpNr = 88888,
            Name = "GPA Test Student",
            StudentId = "STU007",
            CreatedBy = "System",
            ModifiedBy = "System"
        };
        _context.Students.Add(student);

        // Add student enrollments with grades
        var enrollments = new List<StudentEnrollment>
        {
            new StudentEnrollment
            {
                StudentEmpNr = 88888,
                SubjectCode = "CS101",
                Grade = "A", // 4.0 points
                Status = "completed",
                CreatedBy = "System",
                ModifiedBy = "System"
            },
            new StudentEnrollment
            {
                StudentEmpNr = 88888,
                SubjectCode = "CS102",
                Grade = "B", // 3.0 points
                Status = "completed",
                CreatedBy = "System",
                ModifiedBy = "System"
            }
        };
        _context.StudentEnrollments.AddRange(enrollments);
        await _context.SaveChangesAsync();

        // Act - Calculate GPA
        var calculatedGPA = await _studentService.RecalculateGPAAsync(88888);

        // Assert
        Assert.NotNull(calculatedGPA);
        Assert.Equal(3.5m, calculatedGPA); // (4.0 + 3.0) / 2 = 3.5

        // Verify GPA was saved to student record
        var updatedStudent = await _context.Students.FirstOrDefaultAsync(s => s.EmpNr == 88888);
        Assert.NotNull(updatedStudent);
        Assert.Equal(3.5m, updatedStudent.CumulativeGPA);
        Assert.Equal("System", updatedStudent.ModifiedBy);
    }

    [Fact]
    public async Task CalculateGPA_Should_Ignore_Withdrawn_Grades()
    {
        // Arrange
        var student = new Student
        {
            EmpNr = 99999,
            Name = "Withdrawal Test Student",
            StudentId = "STU008",
            CreatedBy = "System",
            ModifiedBy = "System"
        };
        _context.Students.Add(student);

        var enrollments = new List<StudentEnrollment>
        {
            new StudentEnrollment
            {
                StudentEmpNr = 99999,
                SubjectCode = "CS101",
                Grade = "A", // 4.0 points - should count
                Status = "completed",
                CreatedBy = "System",
                ModifiedBy = "System"
            },
            new StudentEnrollment
            {
                StudentEmpNr = 99999,
                SubjectCode = "CS102",
                Grade = "W", // Withdrawn - should not count
                Status = "withdrawn",
                CreatedBy = "System",
                ModifiedBy = "System"
            }
        };
        _context.StudentEnrollments.AddRange(enrollments);
        await _context.SaveChangesAsync();

        // Act
        var calculatedGPA = await _studentService.RecalculateGPAAsync(99999);

        // Assert - GPA should be 4.0 (only the A grade counts)
        Assert.NotNull(calculatedGPA);
        Assert.Equal(4.0m, calculatedGPA);
    }

    [Fact]
    public async Task CalculateGPA_Should_Return_Null_For_Nonexistent_Student()
    {
        // Act
        var result = await _studentService.RecalculateGPAAsync(999999); // Non-existent student

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CalculateGPA_Should_Return_Null_For_Student_With_No_Completed_Courses()
    {
        // Arrange
        var student = new Student
        {
            EmpNr = 111111,
            Name = "No Courses Student",
            StudentId = "STU009",
            CreatedBy = "System",
            ModifiedBy = "System"
        };
        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        // Act
        var result = await _studentService.RecalculateGPAAsync(111111);

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData("A", 4.0)]
    [InlineData("A-", 3.7)]
    [InlineData("B+", 3.3)]
    [InlineData("B", 3.0)]
    [InlineData("B-", 2.7)]
    [InlineData("C+", 2.3)]
    [InlineData("C", 2.0)]
    [InlineData("C-", 1.7)]
    [InlineData("D+", 1.3)]
    [InlineData("D", 1.0)]
    [InlineData("F", 0.0)]
    public async Task CalculateGPA_Should_Handle_All_Grade_Types_Correctly(string grade, decimal expectedPoints)
    {
        // Arrange
        var studentId = 222222 + expectedPoints; // Unique ID for each test case
        var student = new Student
        {
            EmpNr = (int)studentId,
            Name = $"Grade Test Student {grade}",
            StudentId = $"STU{grade}",
            CreatedBy = "System",
            ModifiedBy = "System"
        };
        _context.Students.Add(student);

        var enrollment = new StudentEnrollment
        {
            StudentEmpNr = (int)studentId,
            SubjectCode = "CS101",
            Grade = grade,
            Status = "completed",
            CreatedBy = "System",
            ModifiedBy = "System"
        };
        _context.StudentEnrollments.Add(enrollment);
        await _context.SaveChangesAsync();

        // Act
        var calculatedGPA = await _studentService.RecalculateGPAAsync((int)studentId);

        // Assert
        Assert.NotNull(calculatedGPA);
        Assert.Equal(expectedPoints, calculatedGPA);
    }

    [Fact]
    public async Task UpdateAcademicStanding_Should_Validate_GPA_Requirements()
    {
        // Arrange - Create student with high GPA
        var student = new Student
        {
            EmpNr = 333333,
            Name = "Honor Roll Student",
            StudentId = "STU010",
            CumulativeGPA = 3.9m,
            CreatedBy = "System",
            ModifiedBy = "System"
        };
        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        // Act - Update to President's List (requires 3.9+ GPA)
        var result = await _studentService.UpdateAcademicStandingAsync(
            333333,
            AcademicStanding.PresidentsListqualification);

        // Assert
        Assert.True(result);

        var updatedStudent = await _context.Students.FirstOrDefaultAsync(s => s.EmpNr == 333333);
        Assert.NotNull(updatedStudent);
        Assert.Equal(AcademicStanding.PresidentsListqualification, updatedStudent.AcademicStanding);
        Assert.NotNull(updatedStudent.LastAcademicReviewDate);
    }

    [Fact]
    public async Task UpdateAcademicStanding_Should_Reject_Invalid_GPA_For_Standing()
    {
        // Arrange - Create student with low GPA
        var student = new Student
        {
            EmpNr = 444444,
            Name = "Low GPA Student",
            StudentId = "STU011",
            CumulativeGPA = 2.0m, // Too low for Dean's List
            CreatedBy = "System",
            ModifiedBy = "System"
        };
        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        // Act & Assert - Try to set Dean's List (requires 3.5+ GPA)
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _studentService.UpdateAcademicStandingAsync(444444, AcademicStanding.DeansListqualification));

        Assert.Contains("Invalid academic standing DeansListqualification for GPA 2", exception.Message);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}