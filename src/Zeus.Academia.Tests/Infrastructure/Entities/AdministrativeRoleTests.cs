using Xunit;
using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.Tests.Infrastructure.Entities;

/// <summary>
/// Unit tests for Task 4 entities - Administrative Roles.
/// </summary>
public class AdministrativeRoleTests_New
{
    [Fact]
    public void AdministrativeRole_Should_Initialize_With_Default_Values()
    {
        // Act
        var role = new AdministrativeRole();

        // Assert
        Assert.Equal(string.Empty, role.RoleTitle);
        Assert.Equal(string.Empty, role.RoleCode);
        Assert.Equal(5, role.HierarchyLevel);
        Assert.True(role.IsActive);
        Assert.Equal(string.Empty, role.RoleCategory);
        Assert.Equal(string.Empty, role.AuthorityScope);
    }

    [Fact]
    public void AdministrativeRole_Should_Track_Role_Information()
    {
        // Arrange
        var role = new AdministrativeRole
        {
            RoleTitle = "Associate Dean",
            RoleCode = "ASSOC-DEAN",
            HierarchyLevel = 3,
            RoleCategory = "Academic",
            AuthorityScope = "College",
            TypicalTermLength = 5
        };

        // Act & Assert
        Assert.Equal("Associate Dean", role.RoleTitle);
        Assert.Equal("ASSOC-DEAN", role.RoleCode);
        Assert.Equal(3, role.HierarchyLevel);
        Assert.Equal("Academic", role.RoleCategory);
        Assert.Equal("College", role.AuthorityScope);
    }

    [Fact]
    public void AdministrativeAssignment_Should_Initialize_With_Default_Values()
    {
        // Act
        var assignment = new AdministrativeAssignment();

        // Assert
        Assert.Equal(0, assignment.AssigneeEmpNr);
        Assert.Equal(string.Empty, assignment.RoleCode);
        Assert.True(assignment.IsCurrent);
        Assert.Equal("Permanent", assignment.AppointmentType);
        Assert.Equal("Active", assignment.AppointmentStatus);
        Assert.Equal(1, assignment.TermNumber);
    }

    [Fact]
    public void AdministrativeAssignment_Should_Track_Assignment_Information()
    {
        // Arrange
        var assignment = new AdministrativeAssignment
        {
            AssigneeEmpNr = 12345,
            RoleCode = "DEAN",
            AppointmentType = "Interim",
            TermLengthYears = 3,
            AppointedBy = "President"
        };

        // Act & Assert
        Assert.Equal(12345, assignment.AssigneeEmpNr);
        Assert.Equal("DEAN", assignment.RoleCode);
        Assert.Equal("Interim", assignment.AppointmentType);
        Assert.Equal(3, assignment.TermLengthYears);
        Assert.Equal("President", assignment.AppointedBy);
    }
}