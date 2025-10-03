using Xunit;
using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.Tests.Infrastructure.Entities;

/// <summary>
/// Unit tests for the TenureTrack entity.
/// Tests tenure tracking, milestone management, and evaluation processes.
/// </summary>
public class TenureTrackTests
{
    [Fact]
    public void TenureTrack_Constructor_SetsDefaultValues()
    {
        // Act
        var tenureTrack = new TenureTrack();

        // Assert
        Assert.Equal("On Track", tenureTrack.TenureStatus);
        Assert.Equal("Running", tenureTrack.ClockStatus);
        Assert.Equal(DateTime.Today, tenureTrack.StartDate.Date);
        Assert.Equal(6.0m, tenureTrack.MaxYearsAllowed);
        Assert.True(tenureTrack.IsActive);
        Assert.False(tenureTrack.IsEligibleForEarlyTenure);
        Assert.NotNull(tenureTrack.Milestones);
        Assert.Empty(tenureTrack.Milestones);
    }

    [Fact]
    public void YearsOnTrack_CalculatesCorrectly()
    {
        // Arrange
        var tenureTrack = new TenureTrack
        {
            StartDate = DateTime.Today.AddYears(-3).AddMonths(-6)
        };

        // Act
        var yearsOnTrack = tenureTrack.YearsOnTrack;

        // Assert
        Assert.True(yearsOnTrack >= 3.4m && yearsOnTrack <= 3.6m); // Approximately 3.5 years
    }

    [Fact]
    public void RemainingYears_CalculatesCorrectly()
    {
        // Arrange
        var tenureTrack = new TenureTrack
        {
            StartDate = DateTime.Today.AddYears(-2),
            MaxYearsAllowed = 6.0m
        };

        // Act
        var remainingYears = tenureTrack.RemainingYears;

        // Assert
        Assert.Equal(4.0m, remainingYears, 1); // Within 1 decimal place due to date calculations
    }

    [Fact]
    public void RemainingYears_ExceededLimit_ReturnsZero()
    {
        // Arrange
        var tenureTrack = new TenureTrack
        {
            StartDate = DateTime.Today.AddYears(-7),
            MaxYearsAllowed = 6.0m
        };

        // Act
        var remainingYears = tenureTrack.RemainingYears;

        // Assert
        Assert.True(remainingYears <= 0);
    }

    [Fact]
    public void IsClockRunning_RunningStatus_ReturnsTrue()
    {
        // Arrange
        var tenureTrack = new TenureTrack
        {
            ClockStatus = "Running"
        };

        // Act & Assert
        Assert.True(tenureTrack.IsClockRunning);
    }

    [Fact]
    public void IsClockRunning_StoppedStatus_ReturnsFalse()
    {
        // Arrange
        var tenureTrack = new TenureTrack
        {
            ClockStatus = "Stopped"
        };

        // Act & Assert
        Assert.False(tenureTrack.IsClockRunning);
    }

    [Fact]
    public void IsApproachingTenureReview_OneYearRemaining_ReturnsTrue()
    {
        // Arrange
        var tenureTrack = new TenureTrack
        {
            StartDate = DateTime.Today.AddYears(-5),
            MaxYearsAllowed = 6.0m,
            TenureStatus = "On Track"
        };

        // Act & Assert
        Assert.True(tenureTrack.IsApproachingTenureReview);
    }

    [Fact]
    public void IsApproachingTenureReview_ManyYearsRemaining_ReturnsFalse()
    {
        // Arrange
        var tenureTrack = new TenureTrack
        {
            StartDate = DateTime.Today.AddYears(-2),
            MaxYearsAllowed = 6.0m,
            TenureStatus = "On Track"
        };

        // Act & Assert
        Assert.False(tenureTrack.IsApproachingTenureReview);
    }

    [Fact]
    public void IsApproachingTenureReview_NotOnTrack_ReturnsFalse()
    {
        // Arrange
        var tenureTrack = new TenureTrack
        {
            StartDate = DateTime.Today.AddYears(-5),
            MaxYearsAllowed = 6.0m,
            TenureStatus = "Under Review"
        };

        // Act & Assert
        Assert.False(tenureTrack.IsApproachingTenureReview);
    }

    [Fact]
    public void IsExpired_ExceededTimeLimit_ReturnsTrue()
    {
        // Arrange
        var tenureTrack = new TenureTrack
        {
            StartDate = DateTime.Today.AddYears(-7),
            MaxYearsAllowed = 6.0m,
            FinalTenureDecision = null
        };

        // Act & Assert
        Assert.True(tenureTrack.IsExpired);
    }

    [Fact]
    public void IsExpired_WithinTimeLimit_ReturnsFalse()
    {
        // Arrange
        var tenureTrack = new TenureTrack
        {
            StartDate = DateTime.Today.AddYears(-3),
            MaxYearsAllowed = 6.0m
        };

        // Act & Assert
        Assert.False(tenureTrack.IsExpired);
    }

    [Fact]
    public void IsExpired_ExceededButDecisionMade_ReturnsFalse()
    {
        // Arrange
        var tenureTrack = new TenureTrack
        {
            StartDate = DateTime.Today.AddYears(-7),
            MaxYearsAllowed = 6.0m,
            FinalTenureDecision = "Granted"
        };

        // Act & Assert
        Assert.False(tenureTrack.IsExpired);
    }

    [Fact]
    public void HasTenure_GrantedDecision_ReturnsTrue()
    {
        // Arrange
        var tenureTrack = new TenureTrack
        {
            FinalTenureDecision = "Granted"
        };

        // Act & Assert
        Assert.True(tenureTrack.HasTenure);
    }

    [Fact]
    public void HasTenure_DeniedDecision_ReturnsFalse()
    {
        // Arrange
        var tenureTrack = new TenureTrack
        {
            FinalTenureDecision = "Denied"
        };

        // Act & Assert
        Assert.False(tenureTrack.HasTenure);
    }

    [Fact]
    public void HasTenure_NoDecision_ReturnsFalse()
    {
        // Arrange
        var tenureTrack = new TenureTrack
        {
            FinalTenureDecision = null
        };

        // Act & Assert
        Assert.False(tenureTrack.HasTenure);
    }

    [Fact]
    public void ProgressPercentage_CalculatesCorrectly()
    {
        // Arrange
        var tenureTrack = new TenureTrack
        {
            StartDate = DateTime.Today.AddYears(-3),
            MaxYearsAllowed = 6.0m
        };

        // Act
        var progress = tenureTrack.ProgressPercentage;

        // Assert
        Assert.True(progress >= 45 && progress <= 55); // Approximately 50%
    }

    [Fact]
    public void ProgressPercentage_ExceedsMaximum_ReturnsHundred()
    {
        // Arrange
        var tenureTrack = new TenureTrack
        {
            StartDate = DateTime.Today.AddYears(-8),
            MaxYearsAllowed = 6.0m
        };

        // Act
        var progress = tenureTrack.ProgressPercentage;

        // Assert
        Assert.Equal(100, progress);
    }

    [Fact]
    public void ExpectedTenureReviewDate_CalculatesCorrectly()
    {
        // Arrange
        var startDate = new DateTime(2020, 8, 15);
        var tenureTrack = new TenureTrack
        {
            StartDate = startDate,
            MaxYearsAllowed = 6.0m
        };

        // Act
        var expectedDate = tenureTrack.ExpectedTenureReviewDate;

        // Assert
        var expectedReviewDate = startDate.AddYears(6);
        Assert.Equal(expectedReviewDate.Date, expectedDate.Date);
    }

    [Fact]
    public void TenureClockSummary_FormatsCorrectly()
    {
        // Arrange
        var tenureTrack = new TenureTrack
        {
            StartDate = DateTime.Today.AddYears(-2),
            MaxYearsAllowed = 6.0m,
            ClockStatus = "Running"
        };

        // Act
        var summary = tenureTrack.TenureClockSummary;

        // Assert
        Assert.Contains("Running", summary);
        Assert.Contains("years", summary);
    }

    [Fact]
    public void IsInGoodStanding_OnTrackWithPositiveReviews_ReturnsTrue()
    {
        // Arrange
        var tenureTrack = new TenureTrack
        {
            TenureStatus = "On Track",
            ClockStatus = "Running",
            DepartmentTenureRecommendation = "Positive"
        };

        // Act & Assert
        Assert.True(tenureTrack.IsInGoodStanding);
    }

    [Fact]
    public void IsInGoodStanding_NotOnTrack_ReturnsFalse()
    {
        // Arrange
        var tenureTrack = new TenureTrack
        {
            TenureStatus = "Under Review",
            ClockStatus = "Running"
        };

        // Act & Assert
        Assert.False(tenureTrack.IsInGoodStanding);
    }

    [Fact]
    public void IsInGoodStanding_ClockStopped_ReturnsFalse()
    {
        // Arrange
        var tenureTrack = new TenureTrack
        {
            TenureStatus = "On Track",
            ClockStatus = "Stopped"
        };

        // Act & Assert
        Assert.False(tenureTrack.IsInGoodStanding);
    }

    [Theory]
    [InlineData("Granted", true)]
    [InlineData("Approved", true)]
    [InlineData("Denied", false)]
    [InlineData("Rejected", false)]
    [InlineData("Pending", false)]
    [InlineData(null, false)]
    public void HasTenure_VariousDecisions_ReturnsExpectedResult(string decision, bool expected)
    {
        // Arrange
        var tenureTrack = new TenureTrack
        {
            FinalTenureDecision = decision
        };

        // Act & Assert
        Assert.Equal(expected, tenureTrack.HasTenure);
    }

    [Theory]
    [InlineData("Running", true)]
    [InlineData("Stopped", false)]
    [InlineData("Extended", false)]
    [InlineData("Reset", false)]
    public void IsClockRunning_VariousStatuses_ReturnsExpectedResult(string clockStatus, bool expected)
    {
        // Arrange
        var tenureTrack = new TenureTrack
        {
            ClockStatus = clockStatus
        };

        // Act & Assert
        Assert.Equal(expected, tenureTrack.IsClockRunning);
    }

    [Fact]
    public void Milestones_Collection_InitializedProperly()
    {
        // Act
        var tenureTrack = new TenureTrack();

        // Assert
        Assert.NotNull(tenureTrack.Milestones);
        Assert.Empty(tenureTrack.Milestones);
    }

    [Fact]
    public void Academic_NavigationProperty_CanBeSet()
    {
        // Arrange
        var tenureTrack = new TenureTrack();
        var academic = new Professor { EmpNr = 123, Name = "Test Professor" };

        // Act
        tenureTrack.Academic = academic;

        // Assert
        Assert.Equal(academic, tenureTrack.Academic);
        Assert.Equal(123, tenureTrack.AcademicEmpNr);
    }
}