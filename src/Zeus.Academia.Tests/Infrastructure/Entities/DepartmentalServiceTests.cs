using Xunit;
using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.Tests.Infrastructure.Entities;

/// <summary>
/// Unit tests for Task 4 entities - Departmental Service.
/// </summary>
public class DepartmentalServiceTests_New
{
    [Fact]
    public void DepartmentalService_Should_Initialize_With_Default_Values()
    {
        // Act
        var service = new DepartmentalService();

        // Assert
        Assert.Equal(string.Empty, service.ServiceType);
        Assert.Equal(string.Empty, service.ServiceTitle);
        Assert.Equal(string.Empty, service.ServiceLevel);
        Assert.True(service.IsActive);
        Assert.Equal(string.Empty, service.AcademicYear);
        Assert.Equal(string.Empty, service.DepartmentName);
    }

    [Fact]
    public void DepartmentalService_Should_Track_Service_Information()
    {
        // Arrange
        var service = new DepartmentalService
        {
            ServiceTitle = "Graduate Admissions Committee",
            ServiceType = "Committee",
            ServiceLevel = "Department",
            ServiceCategory = "Admissions",
            EstimatedHoursPerYear = 20.5m,
            AcademicYear = "2024-2025",
            DepartmentName = "Computer Science"
        };

        // Act & Assert
        Assert.Equal("Graduate Admissions Committee", service.ServiceTitle);
        Assert.Equal("Committee", service.ServiceType);
        Assert.Equal("Department", service.ServiceLevel);
        Assert.Equal("Admissions", service.ServiceCategory);
        Assert.Equal(20.5m, service.EstimatedHoursPerYear);
        Assert.Equal("2024-2025", service.AcademicYear);
    }

    [Fact]
    public void ServiceLoadSummary_Should_Initialize_With_Default_Values()
    {
        // Act
        var summary = new ServiceLoadSummary();

        // Assert
        Assert.Equal(0, summary.FacultyEmpNr);
        Assert.Equal(string.Empty, summary.DepartmentName);
        Assert.Equal(0, summary.TotalServiceAssignments);
        Assert.Equal(0, summary.TotalEstimatedHours);
        Assert.Equal(0, summary.LeadershipRoles);
        Assert.Equal("Normal", summary.ServiceLoadCategory);
    }

    [Fact]
    public void ServiceLoadSummary_Should_Track_Load_Information()
    {
        // Arrange
        var summary = new ServiceLoadSummary
        {
            FacultyEmpNr = 12345,
            DepartmentName = "Computer Science",
            AcademicYear = "2024-2025",
            TotalServiceAssignments = 4,
            TotalEstimatedHours = 60.5m,
            LeadershipRoles = 2,
            ServiceLoadCategory = "Heavy",
            CommitteeMemberships = 3
        };

        // Act & Assert
        Assert.Equal(12345, summary.FacultyEmpNr);
        Assert.Equal("Computer Science", summary.DepartmentName);
        Assert.Equal(4, summary.TotalServiceAssignments);
        Assert.Equal(60.5m, summary.TotalEstimatedHours);
        Assert.Equal(2, summary.LeadershipRoles);
        Assert.Equal("Heavy", summary.ServiceLoadCategory);
    }

    [Fact]
    public void ServiceLoadSummary_Should_Calculate_Service_Distribution()
    {
        // Arrange
        var summary = new ServiceLoadSummary
        {
            TotalServiceAssignments = 5,
            LeadershipRoles = 2,
            CommitteeMemberships = 3,
            ExternalServiceCommitments = 1
        };

        // Act & Assert
        Assert.Equal(5, summary.TotalServiceAssignments);
        Assert.Equal(2, summary.LeadershipRoles);
        Assert.Equal(3, summary.CommitteeMemberships);
        Assert.Equal(1, summary.ExternalServiceCommitments);
    }

    [Fact]
    public void ServiceLoadSummary_Should_Handle_Service_Weight_Tracking()
    {
        // Arrange
        var summary = new ServiceLoadSummary
        {
            TotalServiceWeight = 8.5m,
            TotalEstimatedHours = 120.0m,
            TotalActualHours = 115.5m,
            ServiceLoadCategory = "Heavy"
        };

        // Act & Assert
        Assert.Equal(8.5m, summary.TotalServiceWeight);
        Assert.Equal(120.0m, summary.TotalEstimatedHours);
        Assert.Equal(115.5m, summary.TotalActualHours);
        Assert.Equal("Heavy", summary.ServiceLoadCategory);
    }
}