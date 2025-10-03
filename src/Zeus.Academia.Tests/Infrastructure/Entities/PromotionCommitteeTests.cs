using Xunit;
using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.Tests.Infrastructure.Entities;

/// <summary>
/// Unit tests for the PromotionCommittee entity.
/// Tests committee management, membership tracking, and workload monitoring.
/// </summary>
public class PromotionCommitteeTests
{
    [Fact]
    public void PromotionCommittee_Constructor_SetsDefaultValues()
    {
        // Act
        var committee = new PromotionCommittee();

        // Assert
        Assert.NotNull(committee.CommitteeName);
        Assert.Empty(committee.CommitteeName);
        Assert.NotNull(committee.CommitteeType);
        Assert.Empty(committee.CommitteeType);
        Assert.NotNull(committee.AcademicYear);
        Assert.Empty(committee.AcademicYear);
        Assert.True(committee.IsActive);
        Assert.Equal(3, committee.MinimumMembers);
        Assert.Equal(7, committee.MaximumMembers);
        Assert.Equal(3, committee.QuorumRequirement);
        Assert.Equal(50.0m, committee.VotingThreshold);
        Assert.Equal(0, committee.CurrentWorkload);
        Assert.Equal(10, committee.MaxWorkloadCapacity);
        Assert.NotNull(committee.Members);
        Assert.NotNull(committee.PromotionApplications);
        Assert.NotNull(committee.ApprovedRanks);
    }

    [Fact]
    public void CurrentMemberCount_WithNoMembers_ReturnsZero()
    {
        // Arrange
        var committee = new PromotionCommittee();

        // Act & Assert
        Assert.Equal(0, committee.CurrentMemberCount);
    }

    [Fact]
    public void CurrentMemberCount_WithActiveMembers_ReturnsCorrectCount()
    {
        // Arrange
        var committee = new PromotionCommittee();
        committee.Members.Add(new PromotionCommitteeMember { IsActive = true });
        committee.Members.Add(new PromotionCommitteeMember { IsActive = true });
        committee.Members.Add(new PromotionCommitteeMember { IsActive = false });

        // Act & Assert
        Assert.Equal(2, committee.CurrentMemberCount);
    }

    [Fact]
    public void HasSufficientMembers_WithEnoughMembers_ReturnsTrue()
    {
        // Arrange
        var committee = new PromotionCommittee
        {
            MinimumMembers = 3
        };
        committee.Members.Add(new PromotionCommitteeMember { IsActive = true });
        committee.Members.Add(new PromotionCommitteeMember { IsActive = true });
        committee.Members.Add(new PromotionCommitteeMember { IsActive = true });

        // Act & Assert
        Assert.True(committee.HasSufficientMembers);
    }

    [Fact]
    public void HasSufficientMembers_WithInsufficientMembers_ReturnsFalse()
    {
        // Arrange
        var committee = new PromotionCommittee
        {
            MinimumMembers = 3
        };
        committee.Members.Add(new PromotionCommitteeMember { IsActive = true });
        committee.Members.Add(new PromotionCommitteeMember { IsActive = true });

        // Act & Assert
        Assert.False(committee.HasSufficientMembers);
    }

    [Fact]
    public void IsAtCapacity_WithMaximumMembers_ReturnsTrue()
    {
        // Arrange
        var committee = new PromotionCommittee
        {
            MaximumMembers = 2
        };
        committee.Members.Add(new PromotionCommitteeMember { IsActive = true });
        committee.Members.Add(new PromotionCommitteeMember { IsActive = true });

        // Act & Assert
        Assert.True(committee.IsAtCapacity);
    }

    [Fact]
    public void IsAtCapacity_BelowMaximum_ReturnsFalse()
    {
        // Arrange
        var committee = new PromotionCommittee
        {
            MaximumMembers = 3
        };
        committee.Members.Add(new PromotionCommitteeMember { IsActive = true });
        committee.Members.Add(new PromotionCommitteeMember { IsActive = true });

        // Act & Assert
        Assert.False(committee.IsAtCapacity);
    }

    [Fact]
    public void WorkloadUtilization_CalculatesCorrectly()
    {
        // Arrange
        var committee = new PromotionCommittee
        {
            CurrentWorkload = 7,
            MaxWorkloadCapacity = 10
        };

        // Act & Assert
        Assert.Equal(70.0m, committee.WorkloadUtilization);
    }

    [Fact]
    public void WorkloadUtilization_ZeroCapacity_ReturnsZero()
    {
        // Arrange
        var committee = new PromotionCommittee
        {
            CurrentWorkload = 5,
            MaxWorkloadCapacity = 0
        };

        // Act & Assert
        Assert.Equal(0, committee.WorkloadUtilization);
    }

    [Fact]
    public void WorkloadUtilization_ExceedsCapacity_ReturnsMaximum()
    {
        // Arrange
        var committee = new PromotionCommittee
        {
            CurrentWorkload = 15,
            MaxWorkloadCapacity = 10
        };

        // Act & Assert
        Assert.Equal(100.0m, committee.WorkloadUtilization);
    }

    [Fact]
    public void IsOverloaded_ExceedsCapacity_ReturnsTrue()
    {
        // Arrange
        var committee = new PromotionCommittee
        {
            CurrentWorkload = 12,
            MaxWorkloadCapacity = 10
        };

        // Act & Assert
        Assert.True(committee.IsOverloaded);
    }

    [Fact]
    public void IsOverloaded_WithinCapacity_ReturnsFalse()
    {
        // Arrange
        var committee = new PromotionCommittee
        {
            CurrentWorkload = 8,
            MaxWorkloadCapacity = 10
        };

        // Act & Assert
        Assert.False(committee.IsOverloaded);
    }

    [Fact]
    public void CanAcceptNewCases_OptimalConditions_ReturnsTrue()
    {
        // Arrange
        var committee = new PromotionCommittee
        {
            IsActive = true,
            CurrentWorkload = 5,
            MaxWorkloadCapacity = 10,
            MinimumMembers = 3
        };
        committee.Members.Add(new PromotionCommitteeMember { IsActive = true });
        committee.Members.Add(new PromotionCommitteeMember { IsActive = true });
        committee.Members.Add(new PromotionCommitteeMember { IsActive = true });

        // Act & Assert
        Assert.True(committee.CanAcceptNewCases);
    }

    [Fact]
    public void CanAcceptNewCases_InactiveCommittee_ReturnsFalse()
    {
        // Arrange
        var committee = new PromotionCommittee
        {
            IsActive = false,
            CurrentWorkload = 5,
            MaxWorkloadCapacity = 10
        };

        // Act & Assert
        Assert.False(committee.CanAcceptNewCases);
    }

    [Fact]
    public void CanAcceptNewCases_InsufficientMembers_ReturnsFalse()
    {
        // Arrange
        var committee = new PromotionCommittee
        {
            IsActive = true,
            CurrentWorkload = 5,
            MaxWorkloadCapacity = 10,
            MinimumMembers = 3
        };
        committee.Members.Add(new PromotionCommitteeMember { IsActive = true });
        committee.Members.Add(new PromotionCommitteeMember { IsActive = true });

        // Act & Assert
        Assert.False(committee.CanAcceptNewCases);
    }

    [Fact]
    public void CanAcceptNewCases_AtCapacity_ReturnsFalse()
    {
        // Arrange
        var committee = new PromotionCommittee
        {
            IsActive = true,
            CurrentWorkload = 10,
            MaxWorkloadCapacity = 10,
            MinimumMembers = 3
        };
        committee.Members.Add(new PromotionCommitteeMember { IsActive = true });
        committee.Members.Add(new PromotionCommitteeMember { IsActive = true });
        committee.Members.Add(new PromotionCommitteeMember { IsActive = true });

        // Act & Assert
        Assert.False(committee.CanAcceptNewCases);
    }

    [Theory]
    [InlineData(false, false, false, "Inactive")]
    [InlineData(true, false, false, "Understaffed")]
    [InlineData(true, true, true, "Overloaded")]
    [InlineData(true, true, false, "Available")]
    public void EffectivenessStatus_VariousConditions_ReturnsExpectedStatus(
        bool isActive, bool hasSufficientMembers, bool isOverloaded, string expectedStatus)
    {
        // Arrange
        var committee = new PromotionCommittee
        {
            IsActive = isActive,
            MinimumMembers = 3,
            CurrentWorkload = isOverloaded ? 15 : 5,
            MaxWorkloadCapacity = 10
        };

        if (hasSufficientMembers)
        {
            committee.Members.Add(new PromotionCommitteeMember { IsActive = true });
            committee.Members.Add(new PromotionCommitteeMember { IsActive = true });
            committee.Members.Add(new PromotionCommitteeMember { IsActive = true });
        }

        // Act & Assert
        Assert.Equal(expectedStatus, committee.EffectivenessStatus);
    }

    [Fact]
    public void EffectivenessStatus_BusyWorkload_ReturnsBusy()
    {
        // Arrange
        var committee = new PromotionCommittee
        {
            IsActive = true,
            MinimumMembers = 3,
            CurrentWorkload = 9,
            MaxWorkloadCapacity = 10
        };
        committee.Members.Add(new PromotionCommitteeMember { IsActive = true });
        committee.Members.Add(new PromotionCommitteeMember { IsActive = true });
        committee.Members.Add(new PromotionCommitteeMember { IsActive = true });

        // Act & Assert
        Assert.Equal("Busy", committee.EffectivenessStatus);
    }

    [Fact]
    public void FullDescription_WithDepartmentCode_FormatsCorrectly()
    {
        // Arrange
        var committee = new PromotionCommittee
        {
            CommitteeName = "Computer Science Promotion Committee",
            DepartmentCode = "CS",
            AcademicYear = "2024-2025"
        };

        // Act & Assert
        Assert.Equal("Computer Science Promotion Committee (CS - 2024-2025)", committee.FullDescription);
    }

    [Fact]
    public void FullDescription_WithoutDepartmentCode_FormatsCorrectly()
    {
        // Arrange
        var committee = new PromotionCommittee
        {
            CommitteeName = "University Promotion Committee",
            DepartmentCode = null,
            AcademicYear = "2024-2025"
        };

        // Act & Assert
        Assert.Equal("University Promotion Committee (2024-2025)", committee.FullDescription);
    }

    [Fact]
    public void CanAchieveQuorum_WithSufficientMembers_ReturnsTrue()
    {
        // Arrange
        var committee = new PromotionCommittee
        {
            QuorumRequirement = 3
        };
        committee.Members.Add(new PromotionCommitteeMember { IsActive = true });
        committee.Members.Add(new PromotionCommitteeMember { IsActive = true });
        committee.Members.Add(new PromotionCommitteeMember { IsActive = true });

        // Act & Assert
        Assert.True(committee.CanAchieveQuorum);
    }

    [Fact]
    public void CanAchieveQuorum_WithInsufficientMembers_ReturnsFalse()
    {
        // Arrange
        var committee = new PromotionCommittee
        {
            QuorumRequirement = 3
        };
        committee.Members.Add(new PromotionCommitteeMember { IsActive = true });
        committee.Members.Add(new PromotionCommitteeMember { IsActive = true });

        // Act & Assert
        Assert.False(committee.CanAchieveQuorum);
    }

    [Fact]
    public void Collections_InitializedProperly()
    {
        // Act
        var committee = new PromotionCommittee();

        // Assert
        Assert.NotNull(committee.Members);
        Assert.Empty(committee.Members);
        Assert.NotNull(committee.PromotionApplications);
        Assert.Empty(committee.PromotionApplications);
        Assert.NotNull(committee.ApprovedRanks);
        Assert.Empty(committee.ApprovedRanks);
    }

    [Fact]
    public void Chair_NavigationProperty_CanBeSet()
    {
        // Arrange
        var committee = new PromotionCommittee();
        var chair = new Professor { EmpNr = 123, Name = "Committee Chair" };

        // Act
        committee.Chair = chair;
        committee.ChairEmpNr = chair.EmpNr; // ChairEmpNr is not automatically set

        // Assert
        Assert.Equal(chair, committee.Chair);
        Assert.Equal(123, committee.ChairEmpNr);
    }
}