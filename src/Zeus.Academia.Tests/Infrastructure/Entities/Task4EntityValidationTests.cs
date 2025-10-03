using Xunit;
using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.Tests.Infrastructure.Entities;

/// <summary>
/// Simple validation tests for Task 4 entities.
/// </summary>
public class Task4EntityValidationTests
{
    [Fact]
    public void DepartmentChair_Should_Create_Successfully()
    {
        // Act
        var chair = new DepartmentChair
        {
            DepartmentName = "CS",
            FacultyEmpNr = 12345,
            AppointmentStartDate = DateTime.Today.AddYears(-1),
            TermLengthYears = 3
        };

        // Assert
        Assert.NotNull(chair);
        Assert.Equal("CS", chair.DepartmentName);
        Assert.Equal(12345, chair.FacultyEmpNr);
        Assert.True(chair.IsActive);
    }

    [Fact]
    public void CommitteeChair_Should_Create_Successfully()
    {
        // Act
        var chair = new CommitteeChair
        {
            CommitteeName = "Graduate Committee",
            ChairEmpNr = 12345,
            AppointmentStartDate = DateTime.Today.AddMonths(-6)
        };

        // Assert
        Assert.NotNull(chair);
        Assert.Equal("Graduate Committee", chair.CommitteeName);
        Assert.Equal(12345, chair.ChairEmpNr);
        Assert.Equal("Elected", chair.AppointmentType);
    }

    [Fact]
    public void CommitteeMemberAssignment_Should_Create_Successfully()
    {
        // Act
        var assignment = new CommitteeMemberAssignment
        {
            CommitteeName = "Faculty Senate",
            MemberEmpNr = 54321,
            AppointmentStartDate = DateTime.Today.AddMonths(-3)
        };

        // Assert
        Assert.NotNull(assignment);
        Assert.Equal("Faculty Senate", assignment.CommitteeName);
        Assert.Equal(54321, assignment.MemberEmpNr);
        Assert.True(assignment.IsCurrent);
    }

    [Fact]
    public void AdministrativeRole_Should_Create_Successfully()
    {
        // Act
        var role = new AdministrativeRole
        {
            RoleTitle = "Associate Dean",
            RoleCode = "ASSOC-DEAN",
            RoleCategory = "Academic",
            AuthorityScope = "College"
        };

        // Assert
        Assert.NotNull(role);
        Assert.Equal("Associate Dean", role.RoleTitle);
        Assert.Equal("ASSOC-DEAN", role.RoleCode);
        Assert.True(role.IsActive);
    }

    [Fact]
    public void AdministrativeRoleAssignment_Should_Create_Successfully()
    {
        // Act
        var assignment = new AdministrativeAssignment
        {
            AssigneeEmpNr = 11111,
            RoleCode = "DEAN",
            AssignmentStartDate = DateTime.Today.AddYears(-1)
        };

        // Assert
        Assert.NotNull(assignment);
        Assert.Equal(11111, assignment.AssigneeEmpNr);
        Assert.Equal("DEAN", assignment.RoleCode);
        Assert.True(assignment.IsCurrent);
    }

    [Fact]
    public void FacultySearchCommittee_Should_Create_Successfully()
    {
        // Act
        var committee = new FacultySearchCommittee
        {
            SearchCommitteeName = "CS Faculty Search 2024",
            DepartmentName = "Computer Science",
            PositionTitle = "Assistant Professor",
            SearchStartDate = DateTime.Today.AddMonths(-3)
        };

        // Assert
        Assert.NotNull(committee);
        Assert.Equal("CS Faculty Search 2024", committee.SearchCommitteeName);
        Assert.Equal("Computer Science", committee.DepartmentName);
        Assert.Equal("Active", committee.SearchStatus);
    }

    [Fact]
    public void FacultySearchCommitteeMember_Should_Create_Successfully()
    {
        // Act
        var member = new FacultySearchCommitteeMember
        {
            SearchCommitteeCode = "CS-SEARCH-2024",
            MemberEmpNr = 98765,
            AppointmentDate = DateTime.Today.AddMonths(-2)
        };

        // Assert
        Assert.NotNull(member);
        Assert.Equal("CS-SEARCH-2024", member.SearchCommitteeCode);
        Assert.Equal(98765, member.MemberEmpNr);
        Assert.Equal("Member", member.MemberRole);
    }

    [Fact]
    public void DepartmentalService_Should_Create_Successfully()
    {
        // Act
        var service = new DepartmentalService
        {
            ServiceTitle = "Graduate Committee",
            ServiceType = "Committee",
            ServiceLevel = "Department",
            ServiceStartDate = DateTime.Today.AddMonths(-6),
            AcademicYear = "2024-2025",
            DepartmentName = "Computer Science"
        };

        // Assert
        Assert.NotNull(service);
        Assert.Equal("Graduate Committee", service.ServiceTitle);
        Assert.Equal("Committee", service.ServiceType);
        Assert.Equal("Department", service.ServiceLevel);
    }

    [Fact]
    public void DepartmentalServiceLoadSummary_Should_Create_Successfully()
    {
        // Act
        var summary = new ServiceLoadSummary
        {
            FacultyEmpNr = 12345,
            DepartmentName = "Computer Science",
            AcademicYear = "2024-2025"
        };

        // Assert
        Assert.NotNull(summary);
        Assert.Equal(12345, summary.FacultyEmpNr);
        Assert.Equal("Computer Science", summary.DepartmentName);
        Assert.Equal("Normal", summary.ServiceLoadCategory);
    }
}