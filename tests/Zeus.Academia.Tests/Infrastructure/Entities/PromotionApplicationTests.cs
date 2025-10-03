using Xunit;
using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.Tests.Infrastructure.Entities;

/// <summary>
/// Unit tests for the PromotionApplication entity.
/// Tests promotion application workflow, status tracking, and approval processes.
/// </summary>
public class PromotionApplicationTests
{
    [Fact]
    public void PromotionApplication_Constructor_SetsDefaultValues()
    {
        // Act
        var application = new PromotionApplication();

        // Assert
        Assert.Equal("Draft", application.Status);
        Assert.Equal(DateTime.Today.Year, application.AcademicYear);
        Assert.Equal(DateTime.Today, application.ApplicationDate.Date);
        Assert.NotNull(application.RequestedRank);
        Assert.Empty(application.RequestedRank);
        Assert.NotNull(application.ApplicationReason);
        Assert.Empty(application.ApplicationReason);
        Assert.True(application.IsActive);
        Assert.NotNull(application.WorkflowSteps);
        Assert.NotNull(application.Votes);
    }

    [Fact]
    public void IsInProgress_VariousStatuses_ReturnsCorrectValue()
    {
        // Arrange & Act & Assert
        var draftApp = new PromotionApplication { Status = "Draft" };
        Assert.False(draftApp.IsInProgress);

        var submittedApp = new PromotionApplication { Status = "Submitted" };
        Assert.True(submittedApp.IsInProgress);

        var underReviewApp = new PromotionApplication { Status = "Under Review" };
        Assert.True(underReviewApp.IsInProgress);

        var approvedApp = new PromotionApplication { Status = "Approved" };
        Assert.False(approvedApp.IsInProgress);

        var rejectedApp = new PromotionApplication { Status = "Rejected" };
        Assert.False(rejectedApp.IsInProgress);
    }

    [Fact]
    public void IsCompleted_VariousStatuses_ReturnsCorrectValue()
    {
        // Arrange & Act & Assert
        var inProgressApp = new PromotionApplication { Status = "Under Review" };
        Assert.False(inProgressApp.IsCompleted);

        var approvedApp = new PromotionApplication { Status = "Approved" };
        Assert.True(approvedApp.IsCompleted);

        var rejectedApp = new PromotionApplication { Status = "Rejected" };
        Assert.True(rejectedApp.IsCompleted);

        var withdrawnApp = new PromotionApplication { Status = "Withdrawn" };
        Assert.True(withdrawnApp.IsCompleted);
    }

    [Fact]
    public void IsSuccessful_ApprovedStatus_ReturnsTrue()
    {
        // Arrange
        var application = new PromotionApplication
        {
            Status = "Approved"
        };

        // Act & Assert
        Assert.True(application.IsSuccessful);
    }

    [Fact]
    public void IsSuccessful_NonApprovedStatus_ReturnsFalse()
    {
        // Arrange
        var application = new PromotionApplication
        {
            Status = "Rejected"
        };

        // Act & Assert
        Assert.False(application.IsSuccessful);
    }

    [Fact]
    public void DaysInProcess_CalculatesCorrectly()
    {
        // Arrange
        var application = new PromotionApplication
        {
            ApplicationDate = DateTime.Today.AddDays(-30)
        };

        // Act & Assert
        Assert.Equal(30, application.DaysInProcess);
    }

    [Fact]
    public void IsOverdue_WithDueDate_ReturnsCorrectValue()
    {
        // Arrange
        var overdueApp = new PromotionApplication
        {
            ReviewDeadline = DateTime.Today.AddDays(-5),
            Status = "Under Review"
        };

        var onTimeApp = new PromotionApplication
        {
            ReviewDeadline = DateTime.Today.AddDays(5),
            Status = "Under Review"
        };

        var completedApp = new PromotionApplication
        {
            ReviewDeadline = DateTime.Today.AddDays(-5),
            Status = "Approved"
        };

        // Act & Assert
        Assert.True(overdueApp.IsOverdue);
        Assert.False(onTimeApp.IsOverdue);
        Assert.False(completedApp.IsOverdue);
    }

    [Fact]
    public void IsOverdue_NoDueDate_ReturnsFalse()
    {
        // Arrange
        var application = new PromotionApplication
        {
            ReviewDeadline = null,
            Status = "Under Review"
        };

        // Act & Assert
        Assert.False(application.IsOverdue);
    }

    [Fact]
    public void DaysUntilDeadline_CalculatesCorrectly()
    {
        // Arrange
        var application = new PromotionApplication
        {
            ReviewDeadline = DateTime.Today.AddDays(15)
        };

        // Act & Assert
        Assert.Equal(15, application.DaysUntilDeadline);
    }

    [Fact]
    public void DaysUntilDeadline_NoDueDate_ReturnsNull()
    {
        // Arrange
        var application = new PromotionApplication
        {
            ReviewDeadline = null
        };

        // Act & Assert
        Assert.Null(application.DaysUntilDeadline);
    }

    [Fact]
    public void StatusDescription_ReturnsCorrectDescriptions()
    {
        // Arrange & Act & Assert
        var draftApp = new PromotionApplication { Status = "Draft" };
        Assert.Equal("Application in draft status", draftApp.StatusDescription);

        var submittedApp = new PromotionApplication { Status = "Submitted" };
        Assert.Equal("Application submitted for review", submittedApp.StatusDescription);

        var approvedApp = new PromotionApplication { Status = "Approved" };
        Assert.Equal("Application approved", approvedApp.StatusDescription);

        var rejectedApp = new PromotionApplication { Status = "Rejected" };
        Assert.Equal("Application rejected", rejectedApp.StatusDescription);

        var unknownApp = new PromotionApplication { Status = "Unknown" };
        Assert.Equal("Unknown status", unknownApp.StatusDescription);
    }

    [Fact]
    public void ApplicationSummary_FormatsCorrectly()
    {
        // Arrange
        var application = new PromotionApplication
        {
            RequestedRank = "Associate Professor",
            AcademicYear = 2024,
            Status = "Under Review"
        };

        // Act
        var summary = application.ApplicationSummary;

        // Assert
        Assert.Equal("Associate Professor (2024) - Under Review", summary);
    }

    [Fact]
    public void CanEdit_DraftStatus_ReturnsTrue()
    {
        // Arrange
        var application = new PromotionApplication
        {
            Status = "Draft"
        };

        // Act & Assert
        Assert.True(application.CanEdit);
    }

    [Fact]
    public void CanEdit_SubmittedStatus_ReturnsFalse()
    {
        // Arrange
        var application = new PromotionApplication
        {
            Status = "Submitted"
        };

        // Act & Assert
        Assert.False(application.CanEdit);
    }

    [Fact]
    public void CanWithdraw_InProgressApplication_ReturnsTrue()
    {
        // Arrange
        var application = new PromotionApplication
        {
            Status = "Under Review"
        };

        // Act & Assert
        Assert.True(application.CanWithdraw);
    }

    [Fact]
    public void CanWithdraw_CompletedApplication_ReturnsFalse()
    {
        // Arrange
        var application = new PromotionApplication
        {
            Status = "Approved"
        };

        // Act & Assert
        Assert.False(application.CanWithdraw);
    }

    [Fact]
    public void RequiresImmediateAttention_OverdueApplication_ReturnsTrue()
    {
        // Arrange
        var application = new PromotionApplication
        {
            Status = "Under Review",
            ReviewDeadline = DateTime.Today.AddDays(-1)
        };

        // Act & Assert
        Assert.True(application.RequiresImmediateAttention);
    }

    [Fact]
    public void RequiresImmediateAttention_OnTimeApplication_ReturnsFalse()
    {
        // Arrange
        var application = new PromotionApplication
        {
            Status = "Under Review",
            ReviewDeadline = DateTime.Today.AddDays(10)
        };

        // Act & Assert
        Assert.False(application.RequiresImmediateAttention);
    }

    [Theory]
    [InlineData("Draft")]
    [InlineData("Submitted")]
    [InlineData("Under Review")]
    [InlineData("Committee Review")]
    public void IsInProgress_VariousInProgressStatuses_ReturnsTrue(string status)
    {
        // Arrange
        var application = new PromotionApplication
        {
            Status = status
        };

        // Act & Assert
        Assert.Equal(status == "Draft" ? false : true, application.IsInProgress);
    }

    [Theory]
    [InlineData("Approved")]
    [InlineData("Rejected")]
    [InlineData("Withdrawn")]
    [InlineData("Cancelled")]
    public void IsCompleted_VariousCompletedStatuses_ReturnsTrue(string status)
    {
        // Arrange
        var application = new PromotionApplication
        {
            Status = status
        };

        // Act & Assert
        Assert.True(application.IsCompleted);
    }

    [Fact]
    public void WorkflowSteps_Collection_InitializedProperly()
    {
        // Act
        var application = new PromotionApplication();

        // Assert
        Assert.NotNull(application.WorkflowSteps);
        Assert.Empty(application.WorkflowSteps);
    }

    [Fact]
    public void Votes_Collection_InitializedProperly()
    {
        // Act
        var application = new PromotionApplication();

        // Assert
        Assert.NotNull(application.Votes);
        Assert.Empty(application.Votes);
    }

    [Fact]
    public void AcademicYear_CurrentYear_SetsCorrectly()
    {
        // Arrange
        var expectedYear = DateTime.Today.Year;

        // Act
        var application = new PromotionApplication();

        // Assert
        Assert.Equal(expectedYear, application.AcademicYear);
    }

    [Fact]
    public void IsActive_DefaultValue_ReturnsTrue()
    {
        // Act
        var application = new PromotionApplication();

        // Assert
        Assert.True(application.IsActive);
    }
}