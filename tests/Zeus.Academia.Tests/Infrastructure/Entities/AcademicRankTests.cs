using Xunit;
using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.Tests.Infrastructure.Entities;

/// <summary>
/// Unit tests for the AcademicRank entity.
/// Tests academic rank assignment, progression tracking, and tenure status management.
/// </summary>
public class AcademicRankTests
{
    [Fact]
    public void AcademicRank_Constructor_SetsDefaultValues()
    {
        // Act
        var academicRank = new AcademicRank();

        // Assert
        Assert.NotNull(academicRank.RankLevel);
        Assert.Empty(academicRank.RankLevel);
        Assert.True(academicRank.IsCurrentRank);
        Assert.Equal("Not Applicable", academicRank.TenureStatus);
        Assert.Equal(DateTime.Today, academicRank.EffectiveDate.Date);
        Assert.True(academicRank.IsSabbaticalEligible);
        Assert.NotNull(academicRank.Notes);
        Assert.Empty(academicRank.Notes);
    }

    [Fact]
    public void YearsInRank_CurrentRank_ReturnsCorrectValue()
    {
        // Arrange
        var academicRank = new AcademicRank
        {
            EffectiveDate = DateTime.Today.AddYears(-3),
            EndDate = null // Current rank
        };

        // Act
        var yearsInRank = academicRank.YearsInRank;

        // Assert
        Assert.Equal(3, yearsInRank);
    }

    [Fact]
    public void YearsInRank_PastRank_ReturnsCorrectValue()
    {
        // Arrange
        var academicRank = new AcademicRank
        {
            EffectiveDate = DateTime.Today.AddYears(-5),
            EndDate = DateTime.Today.AddYears(-2)
        };

        // Act
        var yearsInRank = academicRank.YearsInRank;

        // Assert
        Assert.Equal(3, yearsInRank);
    }

    [Fact]
    public void IsEligibleForPromotion_WithSufficientYears_ReturnsTrue()
    {
        // Arrange
        var academicRank = new AcademicRank
        {
            EffectiveDate = DateTime.Today.AddYears(-7),
            MinimumYearsInRank = 6,
            IsCurrentRank = true,
            TenureStatus = "Tenured"
        };

        // Act & Assert
        Assert.True(academicRank.IsEligibleForPromotion);
    }

    [Fact]
    public void IsEligibleForPromotion_WithInsufficientYears_ReturnsFalse()
    {
        // Arrange
        var academicRank = new AcademicRank
        {
            EffectiveDate = DateTime.Today.AddYears(-3),
            MinimumYearsInRank = 6,
            IsCurrentRank = true
        };

        // Act & Assert
        Assert.False(academicRank.IsEligibleForPromotion);
    }

    [Fact]
    public void IsEligibleForPromotion_NotCurrentRank_ReturnsFalse()
    {
        // Arrange
        var academicRank = new AcademicRank
        {
            EffectiveDate = DateTime.Today.AddYears(-7),
            MinimumYearsInRank = 6,
            IsCurrentRank = false
        };

        // Act & Assert
        Assert.False(academicRank.IsEligibleForPromotion);
    }

    [Fact]
    public void IsEligibleForTenure_TenureTrackAssistantProfessor_ReturnsTrue()
    {
        // Arrange
        var academicRank = new AcademicRank
        {
            RankLevel = "Assistant Professor",
            TenureStatus = "Tenure Track",
            EffectiveDate = DateTime.Today.AddYears(-5),
            IsCurrentRank = true
        };

        // Act & Assert
        Assert.True(academicRank.IsEligibleForTenure);
    }

    [Fact]
    public void IsEligibleForTenure_NonTenureTrack_ReturnsFalse()
    {
        // Arrange
        var academicRank = new AcademicRank
        {
            RankLevel = "Assistant Professor",
            TenureStatus = "Non-Tenure Track",
            EffectiveDate = DateTime.Today.AddYears(-5),
            IsCurrentRank = true
        };

        // Act & Assert
        Assert.False(academicRank.IsEligibleForTenure);
    }

    [Fact]
    public void IsEligibleForTenure_AlreadyTenured_ReturnsFalse()
    {
        // Arrange
        var academicRank = new AcademicRank
        {
            RankLevel = "Associate Professor",
            TenureStatus = "Tenured",
            EffectiveDate = DateTime.Today.AddYears(-5),
            IsCurrentRank = true
        };

        // Act & Assert
        Assert.False(academicRank.IsEligibleForTenure);
    }

    [Fact]
    public void HasTenure_TenuredStatus_ReturnsTrue()
    {
        // Arrange
        var academicRank = new AcademicRank
        {
            TenureStatus = "Tenured"
        };

        // Act & Assert
        Assert.True(academicRank.HasTenure);
    }

    [Fact]
    public void HasTenure_NonTenuredStatus_ReturnsFalse()
    {
        // Arrange
        var academicRank = new AcademicRank
        {
            TenureStatus = "Tenure Track"
        };

        // Act & Assert
        Assert.False(academicRank.HasTenure);
    }

    [Fact]
    public void NextPromotionRank_AssistantProfessor_ReturnsAssociate()
    {
        // Arrange
        var academicRank = new AcademicRank
        {
            RankLevel = "Assistant Professor"
        };

        // Act & Assert
        Assert.Equal("Associate Professor", academicRank.NextPromotionRank);
    }

    [Fact]
    public void NextPromotionRank_AssociateProfessor_ReturnsFull()
    {
        // Arrange
        var academicRank = new AcademicRank
        {
            RankLevel = "Associate Professor"
        };

        // Act & Assert
        Assert.Equal("Full Professor", academicRank.NextPromotionRank);
    }

    [Fact]
    public void NextPromotionRank_FullProfessor_ReturnsNull()
    {
        // Arrange
        var academicRank = new AcademicRank
        {
            RankLevel = "Full Professor"
        };

        // Act & Assert
        Assert.Null(academicRank.NextPromotionRank);
    }

    [Fact]
    public void RankLevel_AssistantProfessor_ReturnsLevel1()
    {
        // Arrange
        var academicRank = new AcademicRank
        {
            RankLevel = "Assistant Professor"
        };

        // Act & Assert
        Assert.Equal(1, academicRank.RankLevelNumber);
    }

    [Fact]
    public void RankLevel_AssociateProfessor_ReturnsLevel2()
    {
        // Arrange
        var academicRank = new AcademicRank
        {
            RankLevel = "Associate Professor"
        };

        // Act & Assert
        Assert.Equal(2, academicRank.RankLevelNumber);
    }

    [Fact]
    public void RankLevel_FullProfessor_ReturnsLevel3()
    {
        // Arrange
        var academicRank = new AcademicRank
        {
            RankLevel = "Full Professor"
        };

        // Act & Assert
        Assert.Equal(3, academicRank.RankLevelNumber);
    }

    [Fact]
    public void RankLevel_UnknownRank_ReturnsZero()
    {
        // Arrange
        var academicRank = new AcademicRank
        {
            RankLevel = "Unknown Rank"
        };

        // Act & Assert
        Assert.Equal(0, academicRank.RankLevelNumber);
    }

    [Fact]
    public void FullRankDescription_WithCategory_ReturnsFormattedDescription()
    {
        // Arrange
        var academicRank = new AcademicRank
        {
            RankLevel = "Associate Professor",
            RankCategory = "Research"
        };

        // Act & Assert
        Assert.Equal("Associate Professor (Research)", academicRank.FullRankDescription);
    }

    [Fact]
    public void FullRankDescription_WithoutCategory_ReturnsRankLevel()
    {
        // Arrange
        var academicRank = new AcademicRank
        {
            RankLevel = "Full Professor",
            RankCategory = null
        };

        // Act & Assert
        Assert.Equal("Full Professor", academicRank.FullRankDescription);
    }

    [Theory]
    [InlineData("Tenured", true)]
    [InlineData("Tenure Track", false)]
    [InlineData("Non-Tenure Track", false)]
    [InlineData("Not Applicable", false)]
    public void HasTenure_VariousStatuses_ReturnsExpectedResult(string tenureStatus, bool expected)
    {
        // Arrange
        var academicRank = new AcademicRank
        {
            TenureStatus = tenureStatus
        };

        // Act & Assert
        Assert.Equal(expected, academicRank.HasTenure);
    }

    [Theory]
    [InlineData("Assistant Professor", "Tenure Track", true, true)]
    [InlineData("Associate Professor", "Tenure Track", true, true)]
    [InlineData("Full Professor", "Tenured", true, false)]
    [InlineData("Assistant Professor", "Non-Tenure Track", true, false)]
    [InlineData("Assistant Professor", "Tenure Track", false, false)]
    public void IsEligibleForTenure_VariousScenarios_ReturnsExpectedResult(
        string rankLevel, string tenureStatus, bool isCurrentRank, bool expected)
    {
        // Arrange
        var academicRank = new AcademicRank
        {
            RankLevel = rankLevel,
            TenureStatus = tenureStatus,
            IsCurrentRank = isCurrentRank,
            EffectiveDate = DateTime.Today.AddYears(-5)
        };

        // Act & Assert
        Assert.Equal(expected, academicRank.IsEligibleForTenure);
    }

    [Fact]
    public void AcademicRank_CollectionProperties_InitializedProperly()
    {
        // Act
        var academicRank = new AcademicRank();

        // Assert - Navigation properties should be initialized
        Assert.NotNull(academicRank.Academic);
        Assert.NotNull(academicRank.PromotionCommittee);
    }
}