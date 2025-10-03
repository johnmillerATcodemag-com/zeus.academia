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
        Assert.Equal("Non-Tenure Track", academicRank.TenureStatus);
        Assert.Equal(DateTime.MinValue, academicRank.EffectiveDate);
        Assert.False(academicRank.IsSabbaticalEligible);
        Assert.Equal(100.00m, academicRank.AppointmentPercentage);
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
    public void IsEligibleForPromotion_TenureTrackAssistantProfessor_ReturnsTrue()
    {
        // Arrange
        var academicRank = new AcademicRank
        {
            RankLevel = "Assistant Professor",
            TenureStatus = "Tenure Track",
            EffectiveDate = DateTime.Today.AddYears(-5),
            IsCurrentRank = true,
            MinimumYearsInRank = 3
        };

        // Act & Assert
        Assert.True(academicRank.IsEligibleForPromotion);
    }

    [Fact]
    public void IsEligibleForPromotion_NonTenureTrack_ReturnsFalse()
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
        Assert.False(academicRank.IsEligibleForPromotion);
    }

    [Fact]
    public void IsEligibleForPromotion_AlreadyTenured_ReturnsFalse()
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
        Assert.False(academicRank.IsEligibleForPromotion);
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
    public void FullRankDescription_AssistantProfessor_ReturnsAssistant()
    {
        // Arrange
        var academicRank = new AcademicRank
        {
            RankLevel = "Assistant Professor"
        };

        // Act & Assert
        Assert.Equal("Assistant Professor", academicRank.FullRankDescription);
    }

    [Fact]
    public void FullRankDescription_AssociateProfessor_ReturnsAssociate()
    {
        // Arrange
        var academicRank = new AcademicRank
        {
            RankLevel = "Associate Professor"
        };

        // Act & Assert
        Assert.Equal("Associate Professor", academicRank.FullRankDescription);
    }

    [Fact]
    public void FullRankDescription_FullProfessor_ReturnsFull()
    {
        // Arrange
        var academicRank = new AcademicRank
        {
            RankLevel = "Full Professor"
        };

        // Act & Assert
        Assert.Equal("Full Professor", academicRank.FullRankDescription);
    }

    [Fact]
    public void YearsInRank_AssistantProfessor_OneYearAgo_ReturnsOne()
    {
        // Arrange
        var academicRank = new AcademicRank
        {
            RankLevel = "Assistant Professor",
            EffectiveDate = DateTime.Today.AddYears(-1),
            EndDate = null
        };

        // Act & Assert
        Assert.Equal(1, academicRank.YearsInRank);
    }

    [Fact]
    public void YearsInRank_AssociateProfessor_TwoYearsAgo_ReturnsTwo()
    {
        // Arrange
        var academicRank = new AcademicRank
        {
            RankLevel = "Associate Professor",
            EffectiveDate = DateTime.Today.AddYears(-2),
            EndDate = null
        };

        // Act & Assert
        Assert.Equal(2, academicRank.YearsInRank);
    }

    [Fact]
    public void YearsInRank_FullProfessor_ThreeYearsAgo_ReturnsThree()
    {
        // Arrange
        var academicRank = new AcademicRank
        {
            RankLevel = "Full Professor",
            EffectiveDate = DateTime.Today.AddYears(-3),
            EndDate = null
        };

        // Act & Assert
        Assert.Equal(3, academicRank.YearsInRank);
    }

    [Fact]
    public void YearsInRank_NewRank_ReturnsZero()
    {
        // Arrange
        var academicRank = new AcademicRank
        {
            RankLevel = "Unknown Rank",
            EffectiveDate = DateTime.Today,
            EndDate = null
        };

        // Act & Assert
        Assert.Equal(0, academicRank.YearsInRank);
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
    [InlineData("Assistant Professor", "Tenure Track", true, 3, true)]
    [InlineData("Associate Professor", "Tenure Track", true, 3, true)]
    [InlineData("Full Professor", "Tenured", true, null, false)]
    [InlineData("Assistant Professor", "Non-Tenure Track", true, 3, true)]
    [InlineData("Assistant Professor", "Tenure Track", false, 3, false)]
    public void IsEligibleForPromotion_VariousScenarios_ReturnsExpectedResult(
        string rankLevel, string tenureStatus, bool isCurrentRank, int? minimumYears, bool expected)
    {
        // Arrange
        var academicRank = new AcademicRank
        {
            RankLevel = rankLevel,
            TenureStatus = tenureStatus,
            IsCurrentRank = isCurrentRank,
            EffectiveDate = DateTime.Today.AddYears(-5),
            MinimumYearsInRank = minimumYears
        };

        // Act & Assert
        Assert.Equal(expected, academicRank.IsEligibleForPromotion);
    }

    [Fact]
    public void AcademicRank_NavigationProperties_CanBeSet()
    {
        // Arrange
        var academicRank = new AcademicRank();
        var academic = new Professor { EmpNr = 123, Name = "Test Professor" };

        // Act
        academicRank.Academic = academic;
        academicRank.AcademicEmpNr = academic.EmpNr;

        // Assert
        Assert.Equal(academic, academicRank.Academic);
        Assert.Equal(123, academicRank.AcademicEmpNr);
    }
}
