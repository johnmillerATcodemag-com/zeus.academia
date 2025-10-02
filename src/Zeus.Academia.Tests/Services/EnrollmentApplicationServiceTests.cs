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
/// Unit tests for EnrollmentApplicationService
/// </summary>
public class EnrollmentApplicationServiceTests : IDisposable
{
    private readonly AcademiaDbContext _context;
    private readonly Mock<ILogger<EnrollmentApplicationService>> _mockLogger;
    private readonly EnrollmentApplicationService _applicationService;

    public EnrollmentApplicationServiceTests()
    {
        var options = new DbContextOptionsBuilder<AcademiaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var mockConfiguration = new Mock<IConfiguration>();
        _context = new AcademiaDbContext(options, mockConfiguration.Object);

        _mockLogger = new Mock<ILogger<EnrollmentApplicationService>>();
        _applicationService = new EnrollmentApplicationService(_context, _mockLogger.Object);

        SeedTestData();
    }

    private void SeedTestData()
    {
        var csDepartment = new Department
        {
            Id = 1,
            Name = "Computer Science",
            CreatedBy = "Test",
            ModifiedBy = "Test"
        };

        var eeDepartment = new Department
        {
            Id = 2,
            Name = "Electrical Engineering",
            CreatedBy = "Test",
            ModifiedBy = "Test"
        };

        _context.Departments.AddRange(csDepartment, eeDepartment);

        var applications = new[]
        {
            new EnrollmentApplication
            {
                Id = 1,
                ApplicantEmpNr = 1,
                ApplicantName = "John Doe",
                Email = "john.doe@test.com",
                Program = "CS-BS",
                DepartmentName = "Computer Science",
                ApplicationDate = DateTime.UtcNow.AddDays(-5),
                Status = ApplicationStatus.Submitted,
                Priority = ApplicationPriority.Normal,
                CreatedBy = "Test",
                ModifiedBy = "Test"
            },
            new EnrollmentApplication
            {
                Id = 2,
                ApplicantEmpNr = 2,
                ApplicantName = "Jane Smith",
                Email = "jane.smith@test.com",
                Program = "CS-MS",
                DepartmentName = "Computer Science",
                ApplicationDate = DateTime.UtcNow.AddDays(-10),
                Status = ApplicationStatus.UnderReview,
                Priority = ApplicationPriority.High,
                CreatedBy = "Test",
                ModifiedBy = "Test"
            },
            new EnrollmentApplication
            {
                Id = 3,
                ApplicantEmpNr = 3,
                ApplicantName = "Bob Johnson",
                Email = "bob.johnson@test.com",
                Program = "EE-BS",
                DepartmentName = "Electrical Engineering",
                ApplicationDate = DateTime.UtcNow.AddDays(-3),
                Status = ApplicationStatus.Approved,
                Decision = AdmissionDecision.Admitted,
                DecisionDate = DateTime.UtcNow.AddDays(-1),
                Priority = ApplicationPriority.Normal,
                CreatedBy = "Test",
                ModifiedBy = "Test"
            }
        };

        _context.EnrollmentApplications.AddRange(applications);
        _context.SaveChanges();
    }

    [Fact]
    public async Task SubmitApplicationAsync_ValidApplication_ShouldCreateApplication()
    {
        // Arrange
        var application = new EnrollmentApplication
        {
            ApplicantEmpNr = 4,
            ApplicantName = "Alice Brown",
            Email = "alice.brown@test.com",
            Program = "Math-BS",
            DepartmentName = "Mathematics",
            ApplicationDate = DateTime.UtcNow,
            Status = ApplicationStatus.Submitted,
            Priority = ApplicationPriority.Normal
        };

        // Act
        var result = await _applicationService.SubmitApplicationAsync(application);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("Alice Brown", result.ApplicantName);
        Assert.Equal(ApplicationStatus.Submitted, result.Status);

        // Verify it was saved to database
        var savedApplication = await _context.EnrollmentApplications.FindAsync(result.Id);
        Assert.NotNull(savedApplication);
        Assert.Equal("Alice Brown", savedApplication.ApplicantName);
    }

    [Fact]
    public async Task SubmitApplicationAsync_NullApplication_ShouldThrowException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => _applicationService.SubmitApplicationAsync(null!));
    }

    [Fact]
    public async Task SubmitApplicationAsync_InvalidApplication_ShouldThrowException()
    {
        // Arrange
        var application = new EnrollmentApplication
        {
            // Missing required fields
            ApplicantName = "",
            Email = "",
            Program = "",
            DepartmentName = ""
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _applicationService.SubmitApplicationAsync(application));

        Assert.Contains("Applicant name is required", exception.Message);
    }

    [Fact]
    public async Task GetApplicationByIdAsync_ExistingApplication_ShouldReturnApplication()
    {
        // Arrange
        var applicationId = 1;

        // Act
        var result = await _applicationService.GetApplicationByIdAsync(applicationId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(applicationId, result.Id);
        Assert.Equal("John Doe", result.ApplicantName);
    }

    [Fact]
    public async Task GetApplicationByIdAsync_NonExistentApplication_ShouldReturnNull()
    {
        // Arrange
        var applicationId = 999;

        // Act
        var result = await _applicationService.GetApplicationByIdAsync(applicationId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetApplicationsAsync_ShouldReturnPaginatedResults()
    {
        // Arrange
        var pageNumber = 1;
        var pageSize = 2;

        // Act
        var result = await _applicationService.GetApplicationsAsync(pageNumber, pageSize);

        // Assert
        Assert.NotNull(result.Applications);
        Assert.Equal(3, result.TotalCount); // Total from seed data
        Assert.Equal(2, result.Applications.Count()); // Page size
    }

    [Fact]
    public async Task SearchApplicationsAsync_WithStatusFilter_ShouldReturnFilteredResults()
    {
        // Arrange
        var status = ApplicationStatus.Submitted;

        // Act
        var result = await _applicationService.SearchApplicationsAsync(status: status);

        // Assert
        Assert.NotNull(result.Applications);
        Assert.All(result.Applications, app => Assert.Equal(status, app.Status));
    }

    [Fact]
    public async Task SearchApplicationsAsync_WithDepartmentFilter_ShouldReturnFilteredResults()
    {
        // Arrange
        var departmentName = "Computer Science";

        // Act
        var result = await _applicationService.SearchApplicationsAsync(departmentName: departmentName);

        // Assert
        Assert.NotNull(result.Applications);
        Assert.All(result.Applications, app => Assert.Contains(departmentName, app.DepartmentName));
    }

    [Fact]
    public async Task SearchApplicationsAsync_WithSearchTerm_ShouldReturnMatchingResults()
    {
        // Arrange
        var searchTerm = "john";

        // Act
        var result = await _applicationService.SearchApplicationsAsync(searchTerm: searchTerm);

        // Assert
        Assert.NotNull(result.Applications);
        Assert.All(result.Applications, app =>
            Assert.True(app.ApplicantName.ToLower().Contains(searchTerm) ||
                       app.Email.ToLower().Contains(searchTerm)));
    }

    [Fact]
    public async Task UpdateApplicationAsync_ValidApplication_ShouldUpdateFields()
    {
        // Arrange
        var applicationId = 1;
        var originalApplication = await _applicationService.GetApplicationByIdAsync(applicationId);
        Assert.NotNull(originalApplication);

        var updatedApplication = new EnrollmentApplication
        {
            Id = applicationId,
            ApplicantName = "John Doe Updated",
            Email = "john.doe.updated@test.com",
            Program = originalApplication.Program,
            DepartmentName = originalApplication.DepartmentName,
            PhoneNumber = "123-456-7890"
        };

        // Act
        var result = await _applicationService.UpdateApplicationAsync(updatedApplication);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John Doe Updated", result.ApplicantName);
        Assert.Equal("john.doe.updated@test.com", result.Email);
        Assert.Equal("123-456-7890", result.PhoneNumber);
    }

    [Fact]
    public async Task UpdateApplicationAsync_NonExistentApplication_ShouldThrowException()
    {
        // Arrange
        var nonExistentApplication = new EnrollmentApplication
        {
            Id = 999,
            ApplicantName = "Non Existent",
            Email = "test@test.com",
            Program = "CS-BS",
            DepartmentName = "Computer Science"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _applicationService.UpdateApplicationAsync(nonExistentApplication));

        Assert.Contains("not found", exception.Message);
    }

    [Fact]
    public async Task ProcessAdmissionDecisionAsync_ValidApplication_ShouldUpdateDecision()
    {
        // Arrange
        var applicationId = 1;
        var decision = AdmissionDecision.Admitted;
        var reason = "Strong academic background";
        var decisionMadeBy = "Dr. Smith";

        // Act
        var result = await _applicationService.ProcessAdmissionDecisionAsync(
            applicationId, decision, reason, decisionMadeBy);

        // Assert
        Assert.True(result);

        var updatedApplication = await _applicationService.GetApplicationByIdAsync(applicationId);
        Assert.NotNull(updatedApplication);
        Assert.Equal(decision, updatedApplication.Decision);
        Assert.Equal(reason, updatedApplication.DecisionReason);
        Assert.Equal(decisionMadeBy, updatedApplication.DecisionMadeBy);
        Assert.Equal(ApplicationStatus.Approved, updatedApplication.Status);
        Assert.NotNull(updatedApplication.DecisionDate);
    }

    [Fact]
    public async Task ProcessAdmissionDecisionAsync_NonExistentApplication_ShouldReturnFalse()
    {
        // Arrange
        var applicationId = 999;
        var decision = AdmissionDecision.Admitted;

        // Act
        var result = await _applicationService.ProcessAdmissionDecisionAsync(applicationId, decision);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateApplicationStatusAsync_ValidApplication_ShouldUpdateStatus()
    {
        // Arrange
        var applicationId = 1;
        var newStatus = ApplicationStatus.UnderReview;
        var notes = "Review initiated";

        // Act
        var result = await _applicationService.UpdateApplicationStatusAsync(applicationId, newStatus, notes);

        // Assert
        Assert.True(result);

        var updatedApplication = await _applicationService.GetApplicationByIdAsync(applicationId);
        Assert.NotNull(updatedApplication);
        Assert.Equal(newStatus, updatedApplication.Status);
        Assert.Contains(notes, updatedApplication.Notes ?? "");
    }

    [Fact]
    public async Task GetApplicationsByStatusAsync_ShouldReturnCorrectApplications()
    {
        // Arrange
        var status = ApplicationStatus.Approved;

        // Act
        var result = await _applicationService.GetApplicationsByStatusAsync(status);

        // Assert
        Assert.NotNull(result.Applications);
        Assert.All(result.Applications, app => Assert.Equal(status, app.Status));
    }

    [Fact]
    public async Task GetApplicationsByStudentAsync_ValidStudent_ShouldReturnStudentApplications()
    {
        // Arrange
        var studentEmpNr = 1;

        // Act
        var result = await _applicationService.GetApplicationsByStudentAsync(studentEmpNr);

        // Assert
        Assert.NotNull(result);
        Assert.All(result, app => Assert.Equal(studentEmpNr, app.ApplicantEmpNr));
    }

    [Fact]
    public async Task GetApplicationsByStudentAsync_WithStatusFilter_ShouldReturnFilteredResults()
    {
        // Arrange
        var studentEmpNr = 1;
        var status = ApplicationStatus.Submitted;

        // Act
        var result = await _applicationService.GetApplicationsByStudentAsync(studentEmpNr, status);

        // Assert
        Assert.NotNull(result);
        Assert.All(result, app =>
        {
            Assert.Equal(studentEmpNr, app.ApplicantEmpNr);
            Assert.Equal(status, app.Status);
        });
    }

    [Fact]
    public async Task GetApplicationsByDepartmentAsync_ShouldReturnDepartmentApplications()
    {
        // Arrange
        var departmentName = "Computer Science";

        // Act
        var result = await _applicationService.GetApplicationsByDepartmentAsync(departmentName);

        // Assert
        Assert.NotNull(result.Applications);
        Assert.All(result.Applications, app => Assert.Equal(departmentName, app.DepartmentName));
    }

    [Fact]
    public async Task GetApplicationsByPriorityAsync_ShouldReturnPriorityApplications()
    {
        // Arrange
        var priority = ApplicationPriority.High;

        // Act
        var result = await _applicationService.GetApplicationsByPriorityAsync(priority);

        // Assert
        Assert.NotNull(result.Applications);
        Assert.All(result.Applications, app => Assert.Equal(priority, app.Priority));
    }

    [Fact]
    public async Task WithdrawApplicationAsync_ValidApplication_ShouldWithdrawApplication()
    {
        // Arrange
        var applicationId = 1;
        var reason = "Changed mind";

        // Act
        var result = await _applicationService.WithdrawApplicationAsync(applicationId, reason);

        // Assert
        Assert.True(result);

        var withdrawnApplication = await _applicationService.GetApplicationByIdAsync(applicationId);
        Assert.NotNull(withdrawnApplication);
        Assert.Equal(ApplicationStatus.Withdrawn, withdrawnApplication.Status);
        Assert.Contains(reason, withdrawnApplication.Notes ?? "");
    }

    [Fact]
    public async Task WithdrawApplicationAsync_AlreadyProcessedApplication_ShouldThrowException()
    {
        // Arrange
        var applicationId = 3; // This application is already approved
        var reason = "Changed mind";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _applicationService.WithdrawApplicationAsync(applicationId, reason));

        Assert.Contains("Cannot withdraw an application that has already been processed", exception.Message);
    }

    [Fact]
    public async Task ApplicationExistsAsync_ExistingApplication_ShouldReturnTrue()
    {
        // Arrange
        var applicationId = 1;

        // Act
        var result = await _applicationService.ApplicationExistsAsync(applicationId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ApplicationExistsAsync_NonExistentApplication_ShouldReturnFalse()
    {
        // Arrange
        var applicationId = 999;

        // Act
        var result = await _applicationService.ApplicationExistsAsync(applicationId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetTotalApplicationCountAsync_ShouldReturnCorrectCount()
    {
        // Act
        var result = await _applicationService.GetTotalApplicationCountAsync();

        // Assert
        Assert.Equal(3, result); // Based on seed data
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}