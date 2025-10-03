using Xunit;
using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.Tests.Infrastructure.Entities;

/// <summary>
/// Unit tests for Task 4 entities - Committee Management.
/// </summary>
public class CommitteeManagementTests
{
    [Fact]
    public void CommitteeChair_Should_Initialize_With_Default_Values()
    {
        // Act
        var committeeChair = new CommitteeChair();

        // Assert
        Assert.Equal(string.Empty, committeeChair.CommitteeName);
        Assert.Equal(0, committeeChair.ChairEmpNr);
        Assert.True(committeeChair.IsCurrent);
        Assert.Equal(2, committeeChair.TermLengthYears);
        Assert.Equal("Elected", committeeChair.AppointmentType);
        Assert.True(committeeChair.IsEligibleForRenewal);
        Assert.Equal(1, committeeChair.ConsecutiveTermNumber);
    }

    [Fact]
    public void CommitteeChair_Should_Calculate_Vote_Percentage()
    {
        // Arrange
        var committeeChair = new CommitteeChair
        {
            VotesReceived = 15,
            TotalVotesCast = 20
        };

        // Act
        var votePercentage = committeeChair.VotePercentage;

        // Assert
        Assert.Equal(75.0m, votePercentage);
    }

    [Fact]
    public void CommitteeMemberAssignment_Should_Initialize_With_Default_Values()
    {
        // Act
        var assignment = new CommitteeMemberAssignment();

        // Assert
        Assert.Equal(string.Empty, assignment.CommitteeName);
        Assert.Equal(0, assignment.MemberEmpNr);
        Assert.True(assignment.IsCurrent);
        Assert.Equal("Member", assignment.MemberRole);
        Assert.Equal(3, assignment.TermLengthYears);
        Assert.Equal(1, assignment.ConsecutiveTermNumber);
    }

    [Fact]
    public void CommitteeMemberAssignment_Should_Track_Service_Information()
    {
        // Arrange
        var assignment = new CommitteeMemberAssignment
        {
            CommitteeName = "Graduate Committee",
            MemberEmpNr = 12345,
            MemberRole = "Secretary",
            AttendancePercentage = 95.5m,
            MeetingsAttended = 19,
            TotalMeetingsHeld = 20
        };

        // Act & Assert
        Assert.Equal("Graduate Committee", assignment.CommitteeName);
        Assert.Equal(12345, assignment.MemberEmpNr);
        Assert.Equal("Secretary", assignment.MemberRole);
        Assert.Equal(95.5m, assignment.AttendancePercentage);
        Assert.Equal(19, assignment.MeetingsAttended);
        Assert.Equal(20, assignment.TotalMeetingsHeld);
    }
}