using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.Tests.Entities;

/// <summary>
/// Unit tests for Faculty Management entities (Prompt 5 Task 1).
/// Tests entity validation, relationships, and business logic for faculty hierarchy management.
/// </summary>
public class FacultyManagementEntityTests : IDisposable
{
    private readonly AcademiaDbContext _context;

    public FacultyManagementEntityTests()
    {
        var options = new DbContextOptionsBuilder<AcademiaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var mockConfiguration = new Mock<IConfiguration>();
        _context = new AcademiaDbContext(options, mockConfiguration.Object);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    #region FacultyEmploymentHistory Tests

    [Fact]
    public void FacultyEmploymentHistory_Should_Initialize_With_Default_Values()
    {
        // Act
        var employment = new FacultyEmploymentHistory();

        // Assert
        Assert.NotNull(employment);
        Assert.Equal(string.Empty, employment.PositionTitle);
        Assert.Equal(string.Empty, employment.EmploymentType);
        Assert.Equal(string.Empty, employment.ContractType);
        Assert.False(employment.IsCurrentPosition);
        Assert.Equal(100m, employment.FtePercentage);
    }

    [Fact]
    public void FacultyEmploymentHistory_Should_Accept_Valid_Data()
    {
        // Arrange
        var startDate = DateTime.Now.AddYears(-3);
        var endDate = DateTime.Now.AddYears(-1);

        var employment = new FacultyEmploymentHistory
        {
            AcademicEmpNr = 12345,
            PositionTitle = "Assistant Professor",
            DepartmentName = "CS",
            EmploymentType = "Full-time",
            ContractType = "Tenure-track",
            StartDate = startDate,
            EndDate = endDate,
            IsCurrentPosition = false,
            AnnualSalary = 85000m,
            FtePercentage = 100m,
            TeachingLoadPercentage = 40m,
            ResearchExpectationPercentage = 40m,
            ServiceExpectationPercentage = 20m,
            ChangeReason = "Promotion",
            ApprovedBy = "Department Chair",
            ApprovalDate = startDate.AddDays(-30),
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };

        // Act & Assert - no exception should be thrown
        Assert.Equal(12345, employment.AcademicEmpNr);
        Assert.Equal("Assistant Professor", employment.PositionTitle);
        Assert.Equal("CS", employment.DepartmentName);
        Assert.Equal("Full-time", employment.EmploymentType);
        Assert.Equal("Tenure-track", employment.ContractType);
        Assert.Equal(startDate, employment.StartDate);
        Assert.Equal(endDate, employment.EndDate);
        Assert.False(employment.IsCurrentPosition);
        Assert.Equal(85000m, employment.AnnualSalary);
        Assert.Equal(100m, employment.FtePercentage);
        Assert.Equal(40m, employment.TeachingLoadPercentage);
        Assert.Equal(40m, employment.ResearchExpectationPercentage);
        Assert.Equal(20m, employment.ServiceExpectationPercentage);
        Assert.Equal("Promotion", employment.ChangeReason);
        Assert.Equal("Department Chair", employment.ApprovedBy);
    }

    [Fact]
    public async Task FacultyEmploymentHistory_Should_Be_Added_To_Database()
    {
        // Arrange
        var professor = new Professor
        {
            EmpNr = 12345,
            Name = "Dr. John Smith",
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };
        _context.Professors.Add(professor);

        var employment = new FacultyEmploymentHistory
        {
            AcademicEmpNr = 12345,
            PositionTitle = "Associate Professor",
            EmploymentType = "Full-time",
            ContractType = "Tenured",
            StartDate = DateTime.Now.AddYears(-2),
            IsCurrentPosition = true,
            AnnualSalary = 95000m,
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };

        // Act
        _context.FacultyEmploymentHistory.Add(employment);
        await _context.SaveChangesAsync();

        // Assert
        var savedEmployment = await _context.FacultyEmploymentHistory
            .FirstOrDefaultAsync(e => e.AcademicEmpNr == 12345);
        Assert.NotNull(savedEmployment);
        Assert.Equal("Associate Professor", savedEmployment.PositionTitle);
        Assert.True(savedEmployment.IsCurrentPosition);
        Assert.Equal(95000m, savedEmployment.AnnualSalary);
    }

    #endregion

    #region FacultyPromotion Tests

    [Fact]
    public void FacultyPromotion_Should_Initialize_With_Default_Values()
    {
        // Act
        var promotion = new FacultyPromotion();

        // Assert
        Assert.NotNull(promotion);
        Assert.Equal(string.Empty, promotion.PromotionType);
        Assert.Equal("Pending", promotion.Status);
    }

    [Fact]
    public void FacultyPromotion_Should_Accept_Valid_Data()
    {
        // Arrange
        var applicationDate = DateTime.Now.AddMonths(-6);
        var effectiveDate = DateTime.Now.AddMonths(6);
        var decisionDate = DateTime.Now.AddDays(-30);

        var promotion = new FacultyPromotion
        {
            AcademicEmpNr = 12345,
            PromotionType = "Rank",
            FromRankCode = "ASST",
            ToRankCode = "ASSOC",
            FromTenureStatus = false,
            ToTenureStatus = true,
            EffectiveDate = effectiveDate,
            ApplicationDate = applicationDate,
            Status = "Approved",
            DepartmentRecommendation = "Approved",
            CollegeRecommendation = "Approved",
            UniversityRecommendation = "Approved",
            FinalDecision = "Approved",
            DecisionMadeBy = "Provost",
            DecisionDate = decisionDate,
            Justification = "Excellent teaching, research, and service record",
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };

        // Act & Assert
        Assert.Equal(12345, promotion.AcademicEmpNr);
        Assert.Equal("Rank", promotion.PromotionType);
        Assert.Equal("ASST", promotion.FromRankCode);
        Assert.Equal("ASSOC", promotion.ToRankCode);
        Assert.False(promotion.FromTenureStatus);
        Assert.True(promotion.ToTenureStatus);
        Assert.Equal(effectiveDate, promotion.EffectiveDate);
        Assert.Equal("Approved", promotion.Status);
        Assert.Equal("Approved", promotion.FinalDecision);
        Assert.Equal("Provost", promotion.DecisionMadeBy);
    }

    [Fact]
    public async Task FacultyPromotion_Should_Be_Added_To_Database()
    {
        // Arrange
        var professor = new Professor
        {
            EmpNr = 12346,
            Name = "Dr. Jane Doe",
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };
        _context.Professors.Add(professor);

        var promotion = new FacultyPromotion
        {
            AcademicEmpNr = 12346,
            PromotionType = "Tenure",
            FromTenureStatus = false,
            ToTenureStatus = true,
            EffectiveDate = DateTime.Now.AddMonths(6),
            Status = "Under Review",
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };

        // Act
        _context.FacultyPromotions.Add(promotion);
        await _context.SaveChangesAsync();

        // Assert
        var savedPromotion = await _context.FacultyPromotions
            .FirstOrDefaultAsync(p => p.AcademicEmpNr == 12346);
        Assert.NotNull(savedPromotion);
        Assert.Equal("Tenure", savedPromotion.PromotionType);
        Assert.Equal("Under Review", savedPromotion.Status);
        Assert.True(savedPromotion.ToTenureStatus);
    }

    #endregion

    #region ResearchArea Tests

    [Fact]
    public void ResearchArea_Should_Initialize_With_Default_Values()
    {
        // Act
        var researchArea = new ResearchArea();

        // Assert
        Assert.NotNull(researchArea);
        Assert.Equal(string.Empty, researchArea.Code);
        Assert.Equal(string.Empty, researchArea.Name);
        Assert.True(researchArea.IsActive);
        Assert.NotNull(researchArea.ChildAreas);
        Assert.NotNull(researchArea.FacultyExpertise);
        Assert.Empty(researchArea.ChildAreas);
        Assert.Empty(researchArea.FacultyExpertise);
    }

    [Fact]
    public void ResearchArea_Should_Accept_Valid_Data()
    {
        // Arrange & Act
        var researchArea = new ResearchArea
        {
            Code = "CS-AI",
            Name = "Artificial Intelligence",
            Description = "Study of intelligent agents and machine learning",
            ParentAreaCode = "CS",
            PrimaryDiscipline = "Computer Science",
            IsActive = true,
            DisplayOrder = 1,
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };

        // Assert
        Assert.Equal("CS-AI", researchArea.Code);
        Assert.Equal("Artificial Intelligence", researchArea.Name);
        Assert.Equal("Study of intelligent agents and machine learning", researchArea.Description);
        Assert.Equal("CS", researchArea.ParentAreaCode);
        Assert.Equal("Computer Science", researchArea.PrimaryDiscipline);
        Assert.True(researchArea.IsActive);
        Assert.Equal(1, researchArea.DisplayOrder);
    }

    [Fact]
    public async Task ResearchArea_Should_Be_Added_To_Database()
    {
        // Arrange
        var researchArea = new ResearchArea
        {
            Code = "MATH-STAT",
            Name = "Statistics",
            Description = "Mathematical statistics and probability theory",
            PrimaryDiscipline = "Mathematics",
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };

        // Act
        _context.ResearchAreas.Add(researchArea);
        await _context.SaveChangesAsync();

        // Assert
        var savedArea = await _context.ResearchAreas
            .FirstOrDefaultAsync(ra => ra.Code == "MATH-STAT");
        Assert.NotNull(savedArea);
        Assert.Equal("Statistics", savedArea.Name);
        Assert.Equal("Mathematics", savedArea.PrimaryDiscipline);
    }

    #endregion

    #region FacultyExpertise Tests

    [Fact]
    public void FacultyExpertise_Should_Initialize_With_Default_Values()
    {
        // Act
        var expertise = new FacultyExpertise();

        // Assert
        Assert.NotNull(expertise);
        Assert.Equal(string.Empty, expertise.ResearchAreaCode);
        Assert.Equal(string.Empty, expertise.ExpertiseLevel);
        Assert.False(expertise.IsPrimaryExpertise);
    }

    [Fact]
    public void FacultyExpertise_Should_Accept_Valid_Data()
    {
        // Arrange
        var startDate = DateTime.Now.AddYears(-5);

        var expertise = new FacultyExpertise
        {
            AcademicEmpNr = 12345,
            ResearchAreaCode = "CS-AI",
            ExpertiseLevel = "Expert",
            IsPrimaryExpertise = true,
            YearsOfExperience = 8,
            StartDate = startDate,
            Notes = "Published numerous papers in machine learning",
            Certifications = "Google AI Certification",
            PublicationCount = 25,
            GrantCount = 3,
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };

        // Act & Assert
        Assert.Equal(12345, expertise.AcademicEmpNr);
        Assert.Equal("CS-AI", expertise.ResearchAreaCode);
        Assert.Equal("Expert", expertise.ExpertiseLevel);
        Assert.True(expertise.IsPrimaryExpertise);
        Assert.Equal(8, expertise.YearsOfExperience);
        Assert.Equal(startDate, expertise.StartDate);
        Assert.Equal("Published numerous papers in machine learning", expertise.Notes);
        Assert.Equal(25, expertise.PublicationCount);
        Assert.Equal(3, expertise.GrantCount);
    }

    [Fact]
    public async Task FacultyExpertise_Should_Be_Added_To_Database()
    {
        // Arrange
        var professor = new Professor
        {
            EmpNr = 12347,
            Name = "Dr. Bob Wilson",
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };
        _context.Professors.Add(professor);

        var researchArea = new ResearchArea
        {
            Code = "PHYS-QM",
            Name = "Quantum Mechanics",
            PrimaryDiscipline = "Physics",
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };
        _context.ResearchAreas.Add(researchArea);

        var expertise = new FacultyExpertise
        {
            AcademicEmpNr = 12347,
            ResearchAreaCode = "PHYS-QM",
            ExpertiseLevel = "Advanced",
            IsPrimaryExpertise = true,
            YearsOfExperience = 12,
            PublicationCount = 18,
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };

        // Act
        _context.FacultyExpertise.Add(expertise);
        await _context.SaveChangesAsync();

        // Assert
        var savedExpertise = await _context.FacultyExpertise
            .FirstOrDefaultAsync(e => e.AcademicEmpNr == 12347 && e.ResearchAreaCode == "PHYS-QM");
        Assert.NotNull(savedExpertise);
        Assert.Equal("Advanced", savedExpertise.ExpertiseLevel);
        Assert.True(savedExpertise.IsPrimaryExpertise);
        Assert.Equal(18, savedExpertise.PublicationCount);
    }

    #endregion

    #region FacultyServiceRecord Tests

    [Fact]
    public void FacultyServiceRecord_Should_Initialize_With_Default_Values()
    {
        // Act
        var serviceRecord = new FacultyServiceRecord();

        // Assert
        Assert.NotNull(serviceRecord);
        Assert.Equal(string.Empty, serviceRecord.ServiceType);
        Assert.Equal(string.Empty, serviceRecord.ServiceTitle);
        Assert.Equal(string.Empty, serviceRecord.ServiceLevel);
        Assert.True(serviceRecord.IsActive);
        Assert.Equal(1.0m, serviceRecord.ServiceWeight);
        Assert.False(serviceRecord.IsMajorService);
        Assert.False(serviceRecord.IsExternalService);
    }

    [Fact]
    public void FacultyServiceRecord_Should_Accept_Valid_Data()
    {
        // Arrange
        var startDate = DateTime.Now.AddYears(-2);

        var serviceRecord = new FacultyServiceRecord
        {
            AcademicEmpNr = 12345,
            ServiceType = "Committee",
            ServiceTitle = "Graduate Admissions Committee Chair",
            Organization = "Computer Science Department",
            ServiceLevel = "Department",
            LeadershipRole = "Chair",
            StartDate = startDate,
            EndDate = null,
            IsActive = true,
            EstimatedHoursPerYear = 120m,
            ServiceWeight = 2.0m,
            IsMajorService = true,
            IsExternalService = false,
            Notes = "Responsible for reviewing graduate applications",
            Recognition = "Department Service Award 2023",
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };

        // Act & Assert
        Assert.Equal(12345, serviceRecord.AcademicEmpNr);
        Assert.Equal("Committee", serviceRecord.ServiceType);
        Assert.Equal("Graduate Admissions Committee Chair", serviceRecord.ServiceTitle);
        Assert.Equal("Department", serviceRecord.ServiceLevel);
        Assert.Equal("Chair", serviceRecord.LeadershipRole);
        Assert.True(serviceRecord.IsActive);
        Assert.Equal(120m, serviceRecord.EstimatedHoursPerYear);
        Assert.Equal(2.0m, serviceRecord.ServiceWeight);
        Assert.True(serviceRecord.IsMajorService);
        Assert.False(serviceRecord.IsExternalService);
    }

    [Fact]
    public async Task FacultyServiceRecord_Should_Be_Added_To_Database()
    {
        // Arrange
        var professor = new Professor
        {
            EmpNr = 12348,
            Name = "Dr. Alice Brown",
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };
        _context.Professors.Add(professor);

        var serviceRecord = new FacultyServiceRecord
        {
            AcademicEmpNr = 12348,
            ServiceType = "Editorial",
            ServiceTitle = "Associate Editor",
            Organization = "Journal of Computer Science",
            ServiceLevel = "Professional",
            StartDate = DateTime.Now.AddYears(-1),
            IsActive = true,
            EstimatedHoursPerYear = 80m,
            IsExternalService = true,
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };

        // Act
        _context.FacultyServiceRecords.Add(serviceRecord);
        await _context.SaveChangesAsync();

        // Assert
        var savedRecord = await _context.FacultyServiceRecords
            .FirstOrDefaultAsync(sr => sr.AcademicEmpNr == 12348);
        Assert.NotNull(savedRecord);
        Assert.Equal("Editorial", savedRecord.ServiceType);
        Assert.Equal("Professional", savedRecord.ServiceLevel);
        Assert.True(savedRecord.IsExternalService);
    }

    #endregion

    #region CommitteeLeadership Tests

    [Fact]
    public void CommitteeLeadership_Should_Initialize_With_Default_Values()
    {
        // Act
        var leadership = new CommitteeLeadership();

        // Assert
        Assert.NotNull(leadership);
        Assert.Equal(string.Empty, leadership.CommitteeName);
        Assert.Equal(string.Empty, leadership.Position);
        Assert.True(leadership.IsCurrent);
    }

    [Fact]
    public void CommitteeLeadership_Should_Accept_Valid_Data()
    {
        // Arrange
        var startDate = DateTime.Now.AddMonths(-6);
        var appointmentDate = startDate.AddDays(-15);

        var leadership = new CommitteeLeadership
        {
            CommitteeName = "Faculty Senate",
            AcademicEmpNr = 12345,
            Position = "Chair",
            StartDate = startDate,
            EndDate = null,
            IsCurrent = true,
            ChangeReason = null,
            AppointedBy = "Dean",
            AppointmentDate = appointmentDate,
            Notes = "Elected by faculty vote",
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };

        // Act & Assert
        Assert.Equal("Faculty Senate", leadership.CommitteeName);
        Assert.Equal(12345, leadership.AcademicEmpNr);
        Assert.Equal("Chair", leadership.Position);
        Assert.Equal(startDate, leadership.StartDate);
        Assert.Null(leadership.EndDate);
        Assert.True(leadership.IsCurrent);
        Assert.Equal("Dean", leadership.AppointedBy);
        Assert.Equal(appointmentDate, leadership.AppointmentDate);
    }

    [Fact]
    public async Task CommitteeLeadership_Should_Be_Added_To_Database()
    {
        // Arrange
        var committee = new Committee
        {
            Name = "Curriculum Committee",
            Description = "Reviews curriculum changes",
            Type = "Academic",
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };
        _context.Committees.Add(committee);

        var professor = new Professor
        {
            EmpNr = 12349,
            Name = "Dr. Carol Davis",
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };
        _context.Professors.Add(professor);

        var leadership = new CommitteeLeadership
        {
            CommitteeName = "Curriculum Committee",
            AcademicEmpNr = 12349,
            Position = "Vice-Chair",
            StartDate = DateTime.Now.AddMonths(-3),
            IsCurrent = true,
            AppointedBy = "Committee Chair",
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };

        // Act
        _context.CommitteeLeadership.Add(leadership);
        await _context.SaveChangesAsync();

        // Assert
        var savedLeadership = await _context.CommitteeLeadership
            .FirstOrDefaultAsync(cl => cl.CommitteeName == "Curriculum Committee" && cl.AcademicEmpNr == 12349);
        Assert.NotNull(savedLeadership);
        Assert.Equal("Vice-Chair", savedLeadership.Position);
        Assert.True(savedLeadership.IsCurrent);
    }

    #endregion

    #region Navigation Property Tests

    [Fact]
    public async Task Academic_Should_Load_All_Faculty_Management_Navigation_Properties()
    {
        // Arrange
        var professor = new Professor
        {
            EmpNr = 12350,
            Name = "Dr. Full Test",
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };
        _context.Professors.Add(professor);

        var employment = new FacultyEmploymentHistory
        {
            AcademicEmpNr = 12350,
            PositionTitle = "Professor",
            EmploymentType = "Full-time",
            ContractType = "Tenured",
            StartDate = DateTime.Now.AddYears(-5),
            IsCurrentPosition = true,
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };
        _context.FacultyEmploymentHistory.Add(employment);

        var promotion = new FacultyPromotion
        {
            AcademicEmpNr = 12350,
            PromotionType = "Rank",
            EffectiveDate = DateTime.Now.AddYears(-1),
            Status = "Approved",
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };
        _context.FacultyPromotions.Add(promotion);

        await _context.SaveChangesAsync();

        // Act
        var loadedProfessor = await _context.Professors
            .Include(p => p.EmploymentHistory)
            .Include(p => p.Promotions)
            .Include(p => p.ResearchExpertise)
            .Include(p => p.ServiceRecords)
            .Include(p => p.CommitteeLeaderships)
            .FirstOrDefaultAsync(p => p.EmpNr == 12350);

        // Assert
        Assert.NotNull(loadedProfessor);
        Assert.NotNull(loadedProfessor.EmploymentHistory);
        Assert.NotNull(loadedProfessor.Promotions);
        Assert.NotNull(loadedProfessor.ResearchExpertise);
        Assert.NotNull(loadedProfessor.ServiceRecords);
        Assert.NotNull(loadedProfessor.CommitteeLeaderships);
        Assert.Single(loadedProfessor.EmploymentHistory);
        Assert.Single(loadedProfessor.Promotions);
    }

    #endregion
}