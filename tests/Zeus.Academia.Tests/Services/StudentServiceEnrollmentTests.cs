using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Enums;
using Zeus.Academia.Infrastructure.Services;
using Zeus.Academia.Infrastructure.Services.Interfaces;

namespace Zeus.Academia.Tests.Services;

/// <summary>
/// Unit tests for enrollment management functionality in StudentService
/// </summary>
public class StudentServiceEnrollmentTests : IDisposable
{
    private readonly AcademiaDbContext _context;
    private readonly Mock<ILogger<StudentService>> _mockLogger;
    private readonly Mock<IEnrollmentApplicationService> _mockApplicationService;
    private readonly Mock<IEnrollmentHistoryService> _mockHistoryService;
    private readonly StudentService _studentService;

    public StudentServiceEnrollmentTests()
    {
        var options = new DbContextOptionsBuilder<AcademiaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AcademiaDbContext(options);

        _mockLogger = new Mock<ILogger<StudentService>>();
        _mockApplicationService = new Mock<IEnrollmentApplicationService>();
        _mockHistoryService = new Mock<IEnrollmentHistoryService>();

        _studentService = new StudentService(
            _context,
            _mockLogger.Object,
            _mockApplicationService.Object,
            _mockHistoryService.Object);

        SeedTestData();
    }

    private void SeedTestData()
    {
        // Create test department
        var department = new Department
        {
            Id = 1,
            Name = "Computer Science",
            CreatedBy = "Test",
            ModifiedBy = "Test"
        };
        _context.Departments.Add(department);

        // Create test students
        var student1 = new Student
        {
            EmpNr = 1,
            Name = "John Doe",
            StudentId = "STU001",
            DepartmentId = 1,
            Department = department,
            EnrollmentStatus = EnrollmentStatus.Applied,
            AcademicStanding = AcademicStanding.NewStudent,
            CreatedBy = "Test",
            ModifiedBy = "Test"
        };

        var student2 = new Student
        {
            EmpNr = 2,
            Name = "Jane Smith",
            StudentId = "STU002",
            DepartmentId = 1,
            Department = department,
            EnrollmentStatus = EnrollmentStatus.Enrolled,
            AcademicStanding = AcademicStanding.Good,
            CreatedBy = "Test",
            ModifiedBy = "Test"
        };

        var student3 = new Student
        {
            EmpNr = 3,
            Name = "Bob Johnson",
            StudentId = "STU003",
            DepartmentId = 1,
            Department = department,
            EnrollmentStatus = EnrollmentStatus.Admitted,
            AcademicStanding = AcademicStanding.Good,
            CreatedBy = "Test",
            ModifiedBy = "Test"
        };

        _context.Students.AddRange(student1, student2, student3);
        _context.SaveChanges();
    }

    [Fact]
    public async Task SubmitApplicationAsync_ValidStudent_ShouldCreateApplication()
    {
        // Arrange
        var studentId = 1;
        var programCode = "CS-BS";
        var preferredStartDate = DateTime.Now.AddMonths(6);

        var expectedApplication = new EnrollmentApplication
        {
            Id = 1,
            ApplicantEmpNr = studentId,
            ApplicantName = "John Doe",
            Email = "john.doe@academia.edu",
            Program = programCode,
            DepartmentName = "Computer Science",
            Status = ApplicationStatus.Submitted,
            Priority = ApplicationPriority.Normal
        };

        _mockApplicationService
            .Setup(x => x.SearchApplicationsAsync(ApplicationStatus.Submitted, null, null, null, null, "John Doe", 1, 10))
            .ReturnsAsync((new List<EnrollmentApplication>(), 0));

        _mockApplicationService
            .Setup(x => x.SubmitApplicationAsync(It.IsAny<EnrollmentApplication>()))
            .ReturnsAsync(expectedApplication);

        _mockHistoryService
            .Setup(x => x.RecordEnrollmentEventAsync(studentId, EnrollmentEventType.ApplicationSubmitted,
                EnrollmentStatus.Applied, null, It.IsAny<string>(), It.IsAny<string>(), null, 1, null, null, null, null))
            .Returns(Task.FromResult(new EnrollmentHistory()));

        // Act
        var result = await _studentService.SubmitApplicationAsync(studentId, programCode, preferredStartDate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(studentId, result.ApplicantEmpNr);
        Assert.Equal(programCode, result.Program);
        Assert.Equal(ApplicationStatus.Submitted, result.Status);

        _mockApplicationService.Verify(x => x.SubmitApplicationAsync(It.IsAny<EnrollmentApplication>()), Times.Once);
        _mockHistoryService.Verify(x => x.RecordEnrollmentEventAsync(
            studentId, EnrollmentEventType.ApplicationSubmitted,
            EnrollmentStatus.Applied, null, It.IsAny<string>(), It.IsAny<string>(), null, 1, null, null, null, null), Times.Once);
    }

    [Fact]
    public async Task SubmitApplicationAsync_StudentNotFound_ShouldThrowException()
    {
        // Arrange
        var nonExistentStudentId = 999;
        var programCode = "CS-BS";
        var preferredStartDate = DateTime.Now.AddMonths(6);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _studentService.SubmitApplicationAsync(nonExistentStudentId, programCode, preferredStartDate));

        Assert.Contains("not found", exception.Message);
    }

    [Fact]
    public async Task SubmitApplicationAsync_StudentAlreadyEnrolled_ShouldThrowException()
    {
        // Arrange
        var enrolledStudentId = 2; // This student is already enrolled
        var programCode = "CS-BS";
        var preferredStartDate = DateTime.Now.AddMonths(6);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _studentService.SubmitApplicationAsync(enrolledStudentId, programCode, preferredStartDate));

        Assert.Contains("already enrolled", exception.Message);
    }

    [Fact]
    public async Task SubmitApplicationAsync_StudentWithPendingApplication_ShouldThrowException()
    {
        // Arrange
        var studentId = 1;
        var programCode = "CS-BS";
        var preferredStartDate = DateTime.Now.AddMonths(6);

        var existingApplication = new EnrollmentApplication
        {
            Id = 1,
            ApplicantEmpNr = studentId,
            Status = ApplicationStatus.Submitted
        };

        _mockApplicationService
            .Setup(x => x.SearchApplicationsAsync(ApplicationStatus.Submitted, null, null, null, null, "John Doe", 1, 10))
            .ReturnsAsync((new List<EnrollmentApplication> { existingApplication }, 1));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _studentService.SubmitApplicationAsync(studentId, programCode, preferredStartDate));

        Assert.Contains("pending application", exception.Message);
    }

    [Fact]
    public async Task ProcessAdmissionDecisionAsync_ValidApplication_ShouldProcessDecision()
    {
        // Arrange
        var applicationId = 1;
        var decision = AdmissionDecision.Admitted;
        var decisionReason = "Strong academic background";
        var decisionMadeBy = "Dr. Smith";

        var application = new EnrollmentApplication
        {
            Id = applicationId,
            ApplicantEmpNr = 1,
            Status = ApplicationStatus.UnderReview
        };

        _mockApplicationService
            .Setup(x => x.GetApplicationByIdAsync(applicationId))
            .ReturnsAsync(application);

        _mockApplicationService
            .Setup(x => x.ProcessAdmissionDecisionAsync(applicationId, decision, decisionReason, decisionMadeBy))
            .ReturnsAsync(true);

        _mockHistoryService
            .Setup(x => x.RecordEnrollmentEventAsync(1, EnrollmentEventType.AdmissionDecision,
                EnrollmentStatus.Admitted, EnrollmentStatus.Applied, It.IsAny<string>(), It.IsAny<string>(), decisionMadeBy, applicationId, null, null, null, null))
            .Returns(Task.FromResult(new EnrollmentHistory()));

        // Act
        var result = await _studentService.ProcessAdmissionDecisionAsync(
            applicationId, decision, decisionReason, decisionMadeBy);

        // Assert
        Assert.True(result);
        _mockApplicationService.Verify(x => x.ProcessAdmissionDecisionAsync(applicationId, decision, decisionReason, decisionMadeBy), Times.Once);
        _mockHistoryService.Verify(x => x.RecordEnrollmentEventAsync(
            1, EnrollmentEventType.AdmissionDecision, EnrollmentStatus.Admitted, EnrollmentStatus.Applied,
            It.IsAny<string>(), It.IsAny<string>(), decisionMadeBy, applicationId, null, null, null, null), Times.Once);
    }

    [Fact]
    public async Task ProcessAdmissionDecisionAsync_ApplicationNotFound_ShouldThrowException()
    {
        // Arrange
        var applicationId = 999;
        var decision = AdmissionDecision.Admitted;
        var decisionReason = "Strong academic background";
        var decisionMadeBy = "Dr. Smith";

        _mockApplicationService
            .Setup(x => x.GetApplicationByIdAsync(applicationId))
            .ReturnsAsync((EnrollmentApplication?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _studentService.ProcessAdmissionDecisionAsync(applicationId, decision, decisionReason, decisionMadeBy));

        Assert.Contains("not found", exception.Message);
    }

    [Fact]
    public async Task ProcessEnrollmentAsync_ValidApplication_ShouldEnrollStudent()
    {
        // Arrange
        var applicationId = 1;
        var enrollmentDate = DateTime.Now;
        var academicTermId = 1;
        var notes = "Enrollment processed successfully";

        var application = new EnrollmentApplication
        {
            Id = applicationId,
            ApplicantEmpNr = 3, // Student with Admitted status
            Program = "CS-BS",
            Decision = AdmissionDecision.Admitted
        };

        _mockApplicationService
            .Setup(x => x.GetApplicationByIdAsync(applicationId))
            .ReturnsAsync(application);

        _mockApplicationService
            .Setup(x => x.UpdateApplicationStatusAsync(applicationId, ApplicationStatus.Approved, It.IsAny<string>()))
            .ReturnsAsync(true);

        _mockHistoryService
            .Setup(x => x.RecordEnrollmentEventAsync(3, EnrollmentEventType.Enrolled,
                EnrollmentStatus.Enrolled, EnrollmentStatus.Admitted, It.IsAny<string>(), It.IsAny<string>(), "System", applicationId, null, null, null, null))
            .Returns(Task.FromResult(new EnrollmentHistory()));

        // Act
        var result = await _studentService.ProcessEnrollmentAsync(applicationId, enrollmentDate, academicTermId, notes);

        // Assert
        Assert.True(result);

        var student = await _context.Students.FindAsync(3);
        Assert.NotNull(student);
        Assert.Equal(EnrollmentStatus.Enrolled, student.EnrollmentStatus);
        Assert.Equal("CS-BS", student.Program);

        _mockApplicationService.Verify(x => x.UpdateApplicationStatusAsync(applicationId, ApplicationStatus.Approved, It.IsAny<string>()), Times.Once);
        _mockHistoryService.Verify(x => x.RecordEnrollmentEventAsync(
            3, EnrollmentEventType.Enrolled, EnrollmentStatus.Enrolled, EnrollmentStatus.Admitted,
            It.IsAny<string>(), It.IsAny<string>(), "System", applicationId, null, null, null, null), Times.Once);
    }

    [Fact]
    public async Task ProcessEnrollmentAsync_ApplicationWithoutAcceptedDecision_ShouldThrowException()
    {
        // Arrange
        var applicationId = 1;
        var enrollmentDate = DateTime.Now;
        var academicTermId = 1;

        var application = new EnrollmentApplication
        {
            Id = applicationId,
            ApplicantEmpNr = 1,
            Decision = AdmissionDecision.Rejected // Not accepted
        };

        _mockApplicationService
            .Setup(x => x.GetApplicationByIdAsync(applicationId))
            .ReturnsAsync(application);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _studentService.ProcessEnrollmentAsync(applicationId, enrollmentDate, academicTermId));

        Assert.Contains("without accepted admission", exception.Message);
    }

    [Fact]
    public async Task ProcessEnrollmentAsync_StudentNotInAdmittedStatus_ShouldThrowException()
    {
        // Arrange
        var applicationId = 1;
        var enrollmentDate = DateTime.Now;
        var academicTermId = 1;

        var application = new EnrollmentApplication
        {
            Id = applicationId,
            ApplicantEmpNr = 1, // Student with Applied status, not Admitted
            Decision = AdmissionDecision.Admitted
        };

        _mockApplicationService
            .Setup(x => x.GetApplicationByIdAsync(applicationId))
            .ReturnsAsync(application);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _studentService.ProcessEnrollmentAsync(applicationId, enrollmentDate, academicTermId));

        Assert.Contains("must be in Admitted status", exception.Message);
    }

    [Fact]
    public async Task GetStudentApplicationsAsync_ValidStudent_ShouldReturnApplications()
    {
        // Arrange
        var studentId = 1;
        var expectedApplications = new List<EnrollmentApplication>
        {
            new() { Id = 1, ApplicantEmpNr = studentId, Status = ApplicationStatus.Submitted },
            new() { Id = 2, ApplicantEmpNr = studentId, Status = ApplicationStatus.Approved }
        };

        _mockApplicationService
            .Setup(x => x.GetApplicationsByStudentAsync(studentId, null))
            .ReturnsAsync(expectedApplications);

        // Act
        var result = await _studentService.GetStudentApplicationsAsync(studentId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockApplicationService.Verify(x => x.GetApplicationsByStudentAsync(studentId, null), Times.Once);
    }

    [Fact]
    public async Task GetStudentApplicationsAsync_WithStatusFilter_ShouldReturnFilteredApplications()
    {
        // Arrange
        var studentId = 1;
        var status = ApplicationStatus.Submitted;
        var expectedApplications = new List<EnrollmentApplication>
        {
            new() { Id = 1, ApplicantEmpNr = studentId, Status = ApplicationStatus.Submitted }
        };

        _mockApplicationService
            .Setup(x => x.GetApplicationsByStudentAsync(studentId, status))
            .ReturnsAsync(expectedApplications);

        // Act
        var result = await _studentService.GetStudentApplicationsAsync(studentId, status);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.All(result, app => Assert.Equal(status, app.Status));
        _mockApplicationService.Verify(x => x.GetApplicationsByStudentAsync(studentId, status), Times.Once);
    }

    [Fact]
    public async Task GetStudentEnrollmentHistoryAsync_ValidStudent_ShouldReturnHistory()
    {
        // Arrange
        var studentId = 1;
        var expectedHistory = new List<EnrollmentHistory>
        {
            new() { StudentEmpNr = studentId, EventType = EnrollmentEventType.ApplicationSubmitted },
            new() { StudentEmpNr = studentId, EventType = EnrollmentEventType.AdmissionDecision }
        };

        _mockHistoryService
            .Setup(x => x.GetStudentEnrollmentHistoryAsync(studentId, 1, 10))
            .ReturnsAsync((expectedHistory, 2));

        // Act
        var result = await _studentService.GetStudentEnrollmentHistoryAsync(studentId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockHistoryService.Verify(x => x.GetStudentEnrollmentHistoryAsync(studentId, 1, 10), Times.Once);
    }

    [Fact]
    public async Task GetPendingApplicationsAsync_ShouldReturnPendingApplications()
    {
        // Arrange
        var pageNumber = 1;
        var pageSize = 10;
        var expectedApplications = new List<EnrollmentApplication>
        {
            new() { Id = 1, Status = ApplicationStatus.Submitted },
            new() { Id = 2, Status = ApplicationStatus.Submitted }
        };

        _mockApplicationService
            .Setup(x => x.GetApplicationsByStatusAsync(ApplicationStatus.Submitted, pageNumber, pageSize))
            .ReturnsAsync((expectedApplications, 2));

        // Act
        var result = await _studentService.GetPendingApplicationsAsync(pageNumber, pageSize);

        // Assert
        Assert.NotNull(result.Applications);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Applications.Count());
        _mockApplicationService.Verify(x => x.GetApplicationsByStatusAsync(ApplicationStatus.Submitted, pageNumber, pageSize), Times.Once);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}