using Xunit;
using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.Tests.Infrastructure.Entities;

/// <summary>
/// Unit tests for the DepartmentChair entity.
/// </summary>
public class DepartmentChairTests
{
    [Fact]
    public void DepartmentChair_Should_Initialize_With_Default_Values()
    {
        // Act
        var departmentChair = new DepartmentChair();

        // Assert
        Assert.Equal(string.Empty, departmentChair.DepartmentName);
        Assert.Equal(0, departmentChair.FacultyEmpNr);
        Assert.True(departmentChair.IsCurrent);
        Assert.Equal(3, departmentChair.TermLengthYears);
        Assert.True(departmentChair.IsEligibleForRenewal);
        Assert.Equal(1, departmentChair.TermNumber);
        Assert.Equal("Initial", departmentChair.AppointmentType);
    }

    [Fact]
    public void DepartmentChair_Should_Calculate_Expected_End_Date()
    {
        // Arrange
        var startDate = new DateTime(2023, 1, 1);
        var departmentChair = new DepartmentChair
        {
            AppointmentStartDate = startDate,
            TermLengthYears = 3
        };

        // Act
        var expectedEndDate = departmentChair.ExpectedEndDate;

        // Assert
        Assert.Equal(new DateTime(2026, 1, 1), expectedEndDate);
    }

    [Fact]
    public void DepartmentChair_Should_Calculate_Days_Remaining_In_Term()
    {
        // Arrange
        var startDate = DateTime.Today.AddDays(-100);
        var departmentChair = new DepartmentChair
        {
            AppointmentStartDate = startDate,
            TermLengthYears = 3
        };

        // Act
        var daysRemaining = departmentChair.DaysRemainingInTerm;

        // Assert
        Assert.True(daysRemaining > 900); // Should have most of 3 years left
    }

    [Fact]
    public void DepartmentChair_Should_Determine_If_Active()
    {
        // Arrange
        var departmentChair = new DepartmentChair
        {
            IsCurrent = true,
            AppointmentStartDate = DateTime.Today.AddYears(-1),
            AppointmentEndDate = null // No end date
        };

        // Act
        var isActive = departmentChair.IsActive;

        // Assert
        Assert.True(isActive);
    }

    [Fact]
    public void DepartmentChair_Should_Determine_If_Not_Active_Due_To_End_Date()
    {
        // Arrange
        var departmentChair = new DepartmentChair
        {
            IsCurrent = true,
            AppointmentStartDate = DateTime.Today.AddYears(-2),
            AppointmentEndDate = DateTime.Today.AddDays(-1) // Ended yesterday
        };

        // Act
        var isActive = departmentChair.IsActive;

        // Assert
        Assert.False(isActive);
    }

    [Fact]
    public void DepartmentChair_Should_Determine_If_Nearing_Expiration()
    {
        // Arrange
        var departmentChair = new DepartmentChair
        {
            AppointmentStartDate = DateTime.Today.AddDays(-90), // Started 90 days ago
            TermLengthYears = 1, // 1 year term
            AppointmentEndDate = DateTime.Today.AddDays(90) // Ends in 90 days (within 180 day threshold)
        };

        // Act
        var isNearingExpiration = departmentChair.IsNearingExpiration;

        // Assert
        Assert.True(isNearingExpiration);
    }

    [Fact]
    public void DepartmentChair_Should_Identify_Interim_Appointment()
    {
        // Arrange
        var departmentChair = new DepartmentChair
        {
            AppointmentType = "Interim"
        };

        // Act
        var isInterim = departmentChair.IsInterimAppointment;

        // Assert
        Assert.True(isInterim);
    }

    [Fact]
    public void DepartmentChair_Should_Calculate_Total_Years_As_Chair()
    {
        // Arrange
        var startDate = DateTime.Today.AddYears(-2).AddDays(-100);
        var departmentChair = new DepartmentChair
        {
            AppointmentStartDate = startDate,
            AppointmentEndDate = null
        };

        // Act
        var totalYears = departmentChair.TotalYearsAsChair;

        // Assert
        Assert.True(totalYears >= 2);
    }

    [Theory]
    [InlineData("Initial")]
    [InlineData("Reappointment")]
    [InlineData("Interim")]
    [InlineData("Acting")]
    public void DepartmentChair_Should_Accept_Valid_Appointment_Types(string appointmentType)
    {
        // Arrange & Act
        var departmentChair = new DepartmentChair
        {
            AppointmentType = appointmentType
        };

        // Assert
        Assert.Equal(appointmentType, departmentChair.AppointmentType);
    }

    [Fact]
    public void DepartmentChair_Should_Handle_Chair_Stipend()
    {
        // Arrange
        var departmentChair = new DepartmentChair
        {
            ChairStipend = 15000.00m
        };

        // Act & Assert
        Assert.Equal(15000.00m, departmentChair.ChairStipend);
    }

    [Fact]
    public void DepartmentChair_Should_Handle_Course_Release()
    {
        // Arrange
        var departmentChair = new DepartmentChair
        {
            ReceivesCourseRelease = true,
            CourseReleaseCount = 2
        };

        // Act & Assert
        Assert.True(departmentChair.ReceivesCourseRelease);
        Assert.Equal(2, departmentChair.CourseReleaseCount);
    }

    [Fact]
    public void DepartmentChair_Should_Track_Performance_Notes()
    {
        // Arrange
        var notes = "Excellent leadership and departmental management";
        var departmentChair = new DepartmentChair
        {
            PerformanceNotes = notes
        };

        // Act & Assert
        Assert.Equal(notes, departmentChair.PerformanceNotes);
    }
}