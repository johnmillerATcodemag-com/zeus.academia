using Xunit;
using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.Tests.Infrastructure.Entities;

/// <summary>
/// Unit tests for Task 4 entities - Faculty Search Committee.
/// </summary>
public class FacultySearchCommitteeTests_New
{
    [Fact]
    public void FacultySearchCommittee_Should_Initialize_With_Default_Values()
    {
        // Act
        var searchCommittee = new FacultySearchCommittee();

        // Assert
        Assert.Equal(string.Empty, searchCommittee.SearchCommitteeName);
        Assert.Equal(string.Empty, searchCommittee.DepartmentName);
        Assert.Equal(string.Empty, searchCommittee.PositionTitle);
        Assert.Equal("Active", searchCommittee.SearchStatus);
        Assert.Equal("Full-Time", searchCommittee.EmploymentType);
        Assert.Equal(5, searchCommittee.RequiredMemberCount);
    }

    [Fact]
    public void FacultySearchCommittee_Should_Track_Search_Information()
    {
        // Arrange
        var searchCommittee = new FacultySearchCommittee
        {
            SearchCommitteeName = "CS Faculty Search 2024",
            DepartmentName = "Computer Science",
            PositionTitle = "Assistant Professor",
            PositionRank = "Assistant Professor",
            PositionType = "Tenure-Track",
            AcademicYear = "2024-2025",
            SearchStatus = "Active",
            ChairEmpNr = 12345
        };

        // Act & Assert
        Assert.Equal("CS Faculty Search 2024", searchCommittee.SearchCommitteeName);
        Assert.Equal("Computer Science", searchCommittee.DepartmentName);
        Assert.Equal("Assistant Professor", searchCommittee.PositionTitle);
        Assert.Equal("Assistant Professor", searchCommittee.PositionRank);
        Assert.Equal("Tenure-Track", searchCommittee.PositionType);
        Assert.Equal("2024-2025", searchCommittee.AcademicYear);
        Assert.Equal("Active", searchCommittee.SearchStatus);
    }

    [Fact]
    public void FacultySearchCommitteeMember_Should_Initialize_With_Default_Values()
    {
        // Act
        var member = new FacultySearchCommitteeMember();

        // Assert
        Assert.Equal(string.Empty, member.SearchCommitteeCode);
        Assert.Equal(0, member.MemberEmpNr);
        Assert.True(member.IsActive);
        Assert.Equal("Member", member.MemberRole);
        Assert.Equal("Faculty", member.RepresentationCategory);
        Assert.False(member.IsExternalMember);
    }

    [Fact]
    public void FacultySearchCommitteeMember_Should_Track_Member_Information()
    {
        // Arrange
        var member = new FacultySearchCommitteeMember
        {
            SearchCommitteeCode = "CS-SEARCH-2024",
            MemberEmpNr = 12345,
            MemberRole = "Chair",
            ExpertiseArea = "Machine Learning",
            RepresentationCategory = "Faculty",
            IsExternalMember = true,
            ExternalAffiliation = "Mathematics Department"
        };

        // Act & Assert
        Assert.Equal("CS-SEARCH-2024", member.SearchCommitteeCode);
        Assert.Equal(12345, member.MemberEmpNr);
        Assert.Equal("Chair", member.MemberRole);
        Assert.Equal("Machine Learning", member.ExpertiseArea);
        Assert.Equal("Faculty", member.RepresentationCategory);
        Assert.True(member.IsExternalMember);
        Assert.Equal("Mathematics Department", member.ExternalAffiliation);
    }
}