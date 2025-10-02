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
/// Unit tests for AdmissionWorkflowService
/// </summary>
public class AdmissionWorkflowServiceTests : IDisposable
{
    private readonly AcademiaDbContext _context;
    private readonly Mock<ILogger<AdmissionWorkflowService>> _mockLogger;
    private readonly Mock<IEnrollmentApplicationService> _mockApplicationService;
    private readonly Mock<IEnrollmentHistoryService> _mockHistoryService;
    private readonly AdmissionWorkflowService _workflowService;

    public AdmissionWorkflowServiceTests()
    {
        var options = new DbContextOptionsBuilder<AcademiaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AcademiaDbContext(options);

        _mockLogger = new Mock<ILogger<AdmissionWorkflowService>>();
        _mockApplicationService = new Mock<IEnrollmentApplicationService>();
        _mockHistoryService = new Mock<IEnrollmentHistoryService>();

        _workflowService = new AdmissionWorkflowService(
            _context,
            _mockLogger.Object,
            _mockApplicationService.Object,
            _mockHistoryService.Object);

        SeedTestData();
    }

    private void SeedTestData()
    {
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
                Priority = ApplicationPriority.Normal
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
                Priority = ApplicationPriority.High
            },
            new EnrollmentApplication
            {
                Id = 3,
                ApplicantEmpNr = 3,
                ApplicantName = "Bob Johnson",
                Email = "bob.johnson@test.com",
                Program = "EE-BS",
                DepartmentName = "Electrical Engineering",
                ApplicationDate = DateTime.UtcNow.AddDays(-15),
                Status = ApplicationStatus.OnHold,
                Priority = ApplicationPriority.Normal
            }
        };

        _context.EnrollmentApplications.AddRange(applications);
        _context.SaveChanges();
    }

    [Fact]
    public async Task InitiateReviewAsync_ValidSubmittedApplication_ShouldStartReview()
    {
        // Arrange
        var applicationId = 1;
        var reviewerName = "Dr. Smith";
        var reviewNotes = "Initial review started";

        var application = new EnrollmentApplication
        {
            Id = applicationId,
            ApplicantEmpNr = 1,
            Status = ApplicationStatus.Submitted
        };

        _mockApplicationService
            .Setup(x => x.GetApplicationByIdAsync(applicationId))
            .ReturnsAsync(application);

        _mockApplicationService
            .Setup(x => x.UpdateApplicationStatusAsync(applicationId, ApplicationStatus.UnderReview, It.IsAny<string>()))
            .ReturnsAsync(true);

        _mockHistoryService
            .Setup(x => x.RecordEnrollmentEventAsync(1, EnrollmentEventType.ApplicationReviewed,
                EnrollmentStatus.Applied, null, "Application review initiated", reviewNotes, reviewerName, applicationId, null, null, null, null))
            .Returns(Task.FromResult(new EnrollmentHistory()));

        // Act
        var result = await _workflowService.InitiateReviewAsync(applicationId, reviewerName, reviewNotes);

        // Assert
        Assert.True(result);
        _mockApplicationService.Verify(x => x.UpdateApplicationStatusAsync(applicationId, ApplicationStatus.UnderReview, It.IsAny<string>()), Times.Once);
        _mockHistoryService.Verify(x => x.RecordEnrollmentEventAsync(
            1, EnrollmentEventType.ApplicationReviewed, EnrollmentStatus.Applied, null,
            "Application review initiated", reviewNotes, reviewerName, applicationId, null, null, null, null), Times.Once);
    }

    [Fact]
    public async Task InitiateReviewAsync_ApplicationNotFound_ShouldReturnFalse()
    {
        // Arrange
        var applicationId = 999;
        var reviewerName = "Dr. Smith";

        _mockApplicationService
            .Setup(x => x.GetApplicationByIdAsync(applicationId))
            .ReturnsAsync((EnrollmentApplication?)null);

        // Act
        var result = await _workflowService.InitiateReviewAsync(applicationId, reviewerName);

        // Assert
        Assert.False(result);
        _mockApplicationService.Verify(x => x.UpdateApplicationStatusAsync(It.IsAny<int>(), It.IsAny<ApplicationStatus>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task InitiateReviewAsync_ApplicationNotInSubmittedStatus_ShouldReturnFalse()
    {
        // Arrange
        var applicationId = 1;
        var reviewerName = "Dr. Smith";

        var application = new EnrollmentApplication
        {
            Id = applicationId,
            Status = ApplicationStatus.UnderReview // Not submitted
        };

        _mockApplicationService
            .Setup(x => x.GetApplicationByIdAsync(applicationId))
            .ReturnsAsync(application);

        // Act
        var result = await _workflowService.InitiateReviewAsync(applicationId, reviewerName);

        // Assert
        Assert.False(result);
        _mockApplicationService.Verify(x => x.UpdateApplicationStatusAsync(It.IsAny<int>(), It.IsAny<ApplicationStatus>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task RequestAdditionalDocumentsAsync_ValidApplication_ShouldUpdateStatus()
    {
        // Arrange
        var applicationId = 1;
        var requiredDocuments = new List<string> { "Transcript", "Letter of Recommendation" };
        var reviewerName = "Dr. Smith";
        var notes = "Missing essential documents";

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
            .Setup(x => x.UpdateApplicationStatusAsync(applicationId, ApplicationStatus.IncompleteDocuments, It.IsAny<string>()))
            .ReturnsAsync(true);

        _mockHistoryService
            .Setup(x => x.RecordEnrollmentEventAsync(1, EnrollmentEventType.ApplicationReviewed,
                EnrollmentStatus.Applied, null, "Additional documents requested", It.IsAny<string>(), reviewerName, applicationId, null, null, null, null))
            .Returns(Task.FromResult(new EnrollmentHistory()));

        // Act
        var result = await _workflowService.RequestAdditionalDocumentsAsync(applicationId, requiredDocuments, reviewerName, notes);

        // Assert
        Assert.True(result);
        _mockApplicationService.Verify(x => x.UpdateApplicationStatusAsync(applicationId, ApplicationStatus.IncompleteDocuments, It.IsAny<string>()), Times.Once);
        _mockHistoryService.Verify(x => x.RecordEnrollmentEventAsync(
            1, EnrollmentEventType.ApplicationReviewed, EnrollmentStatus.Applied, null,
            "Additional documents requested", It.IsAny<string>(), reviewerName, applicationId, null, null, null, null), Times.Once);
    }

    [Fact]
    public async Task PlaceOnHoldAsync_ValidApplication_ShouldPlaceOnHold()
    {
        // Arrange
        var applicationId = 1;
        var holdReason = "Pending committee review";
        var reviewerName = "Dr. Smith";
        var expectedResolutionDate = DateTime.Now.AddDays(14);

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
            .Setup(x => x.UpdateApplicationStatusAsync(applicationId, ApplicationStatus.OnHold, It.IsAny<string>()))
            .ReturnsAsync(true);

        _mockHistoryService
            .Setup(x => x.RecordEnrollmentEventAsync(1, EnrollmentEventType.ApplicationReviewed,
                EnrollmentStatus.Applied, null, "Application placed on hold", It.IsAny<string>(), reviewerName, applicationId, null, null, null, null))
            .Returns(Task.FromResult(new EnrollmentHistory()));

        // Act
        var result = await _workflowService.PlaceOnHoldAsync(applicationId, holdReason, reviewerName, expectedResolutionDate);

        // Assert
        Assert.True(result);
        _mockApplicationService.Verify(x => x.UpdateApplicationStatusAsync(applicationId, ApplicationStatus.OnHold, It.IsAny<string>()), Times.Once);
        _mockHistoryService.Verify(x => x.RecordEnrollmentEventAsync(
            1, EnrollmentEventType.ApplicationReviewed, EnrollmentStatus.Applied, null,
            "Application placed on hold", It.IsAny<string>(), reviewerName, applicationId, null, null, null, null), Times.Once);
    }

    [Fact]
    public async Task RemoveHoldAsync_ApplicationOnHold_ShouldRemoveHold()
    {
        // Arrange
        var applicationId = 1;
        var resolutionNotes = "Committee review completed";
        var reviewerName = "Dr. Smith";

        var application = new EnrollmentApplication
        {
            Id = applicationId,
            ApplicantEmpNr = 1,
            Status = ApplicationStatus.OnHold
        };

        _mockApplicationService
            .Setup(x => x.GetApplicationByIdAsync(applicationId))
            .ReturnsAsync(application);

        _mockApplicationService
            .Setup(x => x.UpdateApplicationStatusAsync(applicationId, ApplicationStatus.UnderReview, It.IsAny<string>()))
            .ReturnsAsync(true);

        _mockHistoryService
            .Setup(x => x.RecordEnrollmentEventAsync(1, EnrollmentEventType.ApplicationReviewed,
                EnrollmentStatus.Applied, null, "Hold removed from application", It.IsAny<string>(), reviewerName, applicationId, null, null, null, null))
            .Returns(Task.FromResult(new EnrollmentHistory()));

        // Act
        var result = await _workflowService.RemoveHoldAsync(applicationId, resolutionNotes, reviewerName);

        // Assert
        Assert.True(result);
        _mockApplicationService.Verify(x => x.UpdateApplicationStatusAsync(applicationId, ApplicationStatus.UnderReview, It.IsAny<string>()), Times.Once);
        _mockHistoryService.Verify(x => x.RecordEnrollmentEventAsync(
            1, EnrollmentEventType.ApplicationReviewed, EnrollmentStatus.Applied, null,
            "Hold removed from application", It.IsAny<string>(), reviewerName, applicationId, null, null, null, null), Times.Once);
    }

    [Fact]
    public async Task RemoveHoldAsync_ApplicationNotOnHold_ShouldReturnFalse()
    {
        // Arrange
        var applicationId = 1;
        var resolutionNotes = "Committee review completed";
        var reviewerName = "Dr. Smith";

        var application = new EnrollmentApplication
        {
            Id = applicationId,
            ApplicantEmpNr = 1,
            Status = ApplicationStatus.UnderReview // Not on hold
        };

        _mockApplicationService
            .Setup(x => x.GetApplicationByIdAsync(applicationId))
            .ReturnsAsync(application);

        // Act
        var result = await _workflowService.RemoveHoldAsync(applicationId, resolutionNotes, reviewerName);

        // Assert
        Assert.False(result);
        _mockApplicationService.Verify(x => x.UpdateApplicationStatusAsync(It.IsAny<int>(), It.IsAny<ApplicationStatus>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ValidateReadyForDecisionAsync_ReadyApplication_ShouldReturnValid()
    {
        // Arrange
        var applicationId = 1;
        var application = new EnrollmentApplication
        {
            Id = applicationId,
            Status = ApplicationStatus.UnderReview,
            ApplicationDate = DateTime.UtcNow.AddDays(-2) // More than 24 hours ago
        };

        _mockApplicationService
            .Setup(x => x.GetApplicationByIdAsync(applicationId))
            .ReturnsAsync(application);

        _mockApplicationService
            .Setup(x => x.AreAllRequiredDocumentsSubmittedAsync(applicationId))
            .ReturnsAsync(true);

        // Act
        var result = await _workflowService.ValidateReadyForDecisionAsync(applicationId);

        // Assert
        Assert.True(result.IsReadyForDecision);
        Assert.Empty(result.Issues);
        Assert.True(result.AllDocumentsSubmitted);
    }

    [Fact]
    public async Task ValidateReadyForDecisionAsync_ApplicationNotReviewed_ShouldHaveIssues()
    {
        // Arrange
        var applicationId = 1;
        var application = new EnrollmentApplication
        {
            Id = applicationId,
            Status = ApplicationStatus.Submitted, // Not reviewed yet
            ApplicationDate = DateTime.UtcNow.AddDays(-2)
        };

        _mockApplicationService
            .Setup(x => x.GetApplicationByIdAsync(applicationId))
            .ReturnsAsync(application);

        _mockApplicationService
            .Setup(x => x.AreAllRequiredDocumentsSubmittedAsync(applicationId))
            .ReturnsAsync(true);

        // Act
        var result = await _workflowService.ValidateReadyForDecisionAsync(applicationId);

        // Assert
        Assert.False(result.IsReadyForDecision);
        Assert.Contains("has not been reviewed yet", result.Issues);
    }

    [Fact]
    public async Task ValidateReadyForDecisionAsync_IncompleteDocuments_ShouldHaveIssues()
    {
        // Arrange
        var applicationId = 1;
        var application = new EnrollmentApplication
        {
            Id = applicationId,
            Status = ApplicationStatus.UnderReview,
            ApplicationDate = DateTime.UtcNow.AddDays(-2)
        };

        _mockApplicationService
            .Setup(x => x.GetApplicationByIdAsync(applicationId))
            .ReturnsAsync(application);

        _mockApplicationService
            .Setup(x => x.AreAllRequiredDocumentsSubmittedAsync(applicationId))
            .ReturnsAsync(false);

        // Act
        var result = await _workflowService.ValidateReadyForDecisionAsync(applicationId);

        // Assert
        Assert.False(result.IsReadyForDecision);
        Assert.Contains("Not all required documents", result.Issues);
        Assert.False(result.AllDocumentsSubmitted);
    }

    [Fact]
    public async Task ExpediteApplicationAsync_ValidApplication_ShouldExpedite()
    {
        // Arrange
        var applicationId = 1;
        var expediteReason = "Scholarship deadline approaching";
        var requestedBy = "Dr. Johnson";

        var application = new EnrollmentApplication
        {
            Id = applicationId,
            ApplicantEmpNr = 1,
            Status = ApplicationStatus.UnderReview,
            Priority = ApplicationPriority.Normal
        };

        _mockApplicationService
            .Setup(x => x.GetApplicationByIdAsync(applicationId))
            .ReturnsAsync(application);

        _mockApplicationService
            .Setup(x => x.UpdateApplicationStatusAsync(applicationId, ApplicationStatus.UnderReview, It.IsAny<string>()))
            .ReturnsAsync(true);

        _mockHistoryService
            .Setup(x => x.RecordEnrollmentEventAsync(1, EnrollmentEventType.ApplicationReviewed,
                EnrollmentStatus.Applied, null, "Application expedited", It.IsAny<string>(), requestedBy, applicationId, null, null, null, null))
            .Returns(Task.FromResult(new EnrollmentHistory()));

        // Act
        var result = await _workflowService.ExpediteApplicationAsync(applicationId, expediteReason, requestedBy);

        // Assert
        Assert.True(result);
        _mockApplicationService.Verify(x => x.UpdateApplicationStatusAsync(applicationId, ApplicationStatus.UnderReview, It.IsAny<string>()), Times.Once);
        _mockHistoryService.Verify(x => x.RecordEnrollmentEventAsync(
            1, EnrollmentEventType.ApplicationReviewed, EnrollmentStatus.Applied, null,
            "Application expedited", It.IsAny<string>(), requestedBy, applicationId, null, null, null, null), Times.Once);
    }

    [Fact]
    public async Task GetApplicationsRequiringAttentionAsync_ShouldReturnOverdueApplications()
    {
        // Arrange
        var daysOverdue = 7;
        var pageNumber = 1;
        var pageSize = 10;

        // Act
        var result = await _workflowService.GetApplicationsRequiringAttentionAsync(daysOverdue, pageNumber, pageSize);

        // Assert
        Assert.NotNull(result.Applications);
        Assert.True(result.TotalCount >= 0);

        // Should include applications that are overdue or have specific statuses
        var applications = result.Applications.ToList();
        if (applications.Any())
        {
            Assert.All(applications, app =>
            {
                var isOverdue = app.ApplicationDate <= DateTime.UtcNow.AddDays(-daysOverdue);
                var requiresAttention = app.Status == ApplicationStatus.IncompleteDocuments ||
                                      app.Status == ApplicationStatus.OnHold ||
                                      (app.Status == ApplicationStatus.Submitted && isOverdue) ||
                                      (app.Status == ApplicationStatus.UnderReview && app.ApplicationDate <= DateTime.UtcNow.AddDays(-daysOverdue - 3));

                Assert.True(requiresAttention);
            });
        }
    }

    [Fact]
    public async Task GetAdmissionStatsAsync_ValidDateRange_ShouldReturnStatistics()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(-30);
        var endDate = DateTime.UtcNow;
        var departmentName = "Computer Science";

        // Act
        var result = await _workflowService.GetAdmissionStatsAsync(startDate, endDate, departmentName);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(departmentName, result.DepartmentName);
        Assert.Equal(startDate, result.StartDate);
        Assert.Equal(endDate, result.EndDate);
        Assert.True(result.TotalApplications >= 0);
        Assert.True(result.AdmissionRate >= 0 && result.AdmissionRate <= 100);
    }

    [Fact]
    public async Task GetApplicationTimelineAsync_ValidApplication_ShouldReturnTimeline()
    {
        // Arrange
        var applicationId = 1;
        var application = new EnrollmentApplication
        {
            Id = applicationId,
            ApplicantEmpNr = 1,
            ApplicationDate = DateTime.UtcNow.AddDays(-5),
            Program = "CS-BS",
            Decision = AdmissionDecision.Admitted,
            DecisionDate = DateTime.UtcNow.AddDays(-1),
            DecisionReason = "Strong academic record",
            DecisionMadeBy = "Dr. Smith"
        };

        var enrollmentHistory = new List<EnrollmentHistory>
        {
            new()
            {
                StudentEmpNr = 1,
                EventType = EnrollmentEventType.ApplicationReviewed,
                EventDate = DateTime.UtcNow.AddDays(-3),
                Reason = "Review initiated",
                ProcessedBy = "Dr. Jones",
                ApplicationId = applicationId
            }
        };

        _mockApplicationService
            .Setup(x => x.GetApplicationByIdAsync(applicationId))
            .ReturnsAsync(application);

        _mockHistoryService
            .Setup(x => x.GetStudentEnrollmentHistoryAsync(1, 1, 10))
            .ReturnsAsync((enrollmentHistory, 1));

        // Act
        var result = await _workflowService.GetApplicationTimelineAsync(applicationId);

        // Assert
        Assert.NotNull(result);
        var events = result.ToList();
        Assert.NotEmpty(events);

        // Should include application submission event
        Assert.Contains(events, e => e.EventType == "Application Submitted");

        // Should include decision event
        Assert.Contains(events, e => e.EventType == "Admission Decision");

        // Events should be ordered chronologically
        Assert.True(events.OrderBy(e => e.EventDate).SequenceEqual(events));
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}