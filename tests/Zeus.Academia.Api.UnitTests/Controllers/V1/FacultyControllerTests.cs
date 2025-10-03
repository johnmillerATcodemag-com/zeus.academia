using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Zeus.Academia.Api.Controllers.V1;
using Zeus.Academia.Api.Mapping;
using Zeus.Academia.Api.Models.Common;
using Zeus.Academia.Api.Models.Requests;
using Zeus.Academia.Api.Models.Responses;
using Zeus.Academia.Infrastructure.Services.Interfaces;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Enums;

namespace Zeus.Academia.Api.UnitTests.Controllers.V1;

/// <summary>
/// Unit tests for the FacultyController.
/// </summary>
public class FacultyControllerTests
{
    private readonly Mock<IFacultyService> _mockFacultyService;
    private readonly Mock<ILogger<FacultyController>> _mockLogger;
    private readonly IMapper _mapper;
    private readonly FacultyController _controller;

    public FacultyControllerTests()
    {
        _mockFacultyService = new Mock<IFacultyService>();
        _mockLogger = new Mock<ILogger<FacultyController>>();

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<FacultyMappingProfile>();
        });
        _mapper = mapperConfig.CreateMapper();

        _controller = new FacultyController(
            _mockFacultyService.Object,
            _mapper,
            _mockLogger.Object);
    }

    #region Test Data Helpers

    private Professor CreateTestProfessor()
    {
        return new Professor
        {
            EmpNr = 1,
            Name = "Dr. John Smith",
            PhoneNumber = "123-456-7890",
            Salary = 85000m,
            DepartmentName = "Computer Science",
            RankCode = "PROF",
            HasTenure = true,
            ResearchArea = "Artificial Intelligence, Machine Learning"
        };
    }

    private TeachingProf CreateTestTeachingProf()
    {
        return new TeachingProf
        {
            EmpNr = 2,
            Name = "Prof. Jane Doe",
            PhoneNumber = "098-765-4321",
            Salary = 65000m,
            DepartmentName = "Mathematics",
            RankCode = "ASTP",
            HasTenure = false,
            ResearchArea = "Applied Mathematics"
        };
    }

    private IEnumerable<Academic> CreateTestFaculty()
    {
        return new List<Academic>
        {
            CreateTestProfessor(),
            CreateTestTeachingProf()
        };
    }

    #endregion

    #region GetFaculty Tests

    [Fact]
    public async Task GetFaculty_ValidRequest_ReturnsPagedResponse()
    {
        // Arrange
        var faculty = CreateTestFaculty();
        var totalCount = 2;

        _mockFacultyService.Setup(x => x.GetFacultyAsync(1, 10))
            .ReturnsAsync((faculty, totalCount));

        // Act
        var result = await _controller.GetFaculty(1, 10);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<PagedResponse<FacultySummaryResponse>>(okResult.Value);
        Assert.Equal(2, response.Data.Count());
        Assert.Equal(1, response.PageNumber);
        Assert.Equal(10, response.PageSize);
        Assert.Equal(2, response.TotalCount);
    }

    [Fact]
    public async Task GetFaculty_InvalidPageNumber_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.GetFaculty(0, 10);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(badRequestResult.Value);
        Assert.False(response.Success);
        Assert.Contains("Page number must be greater than 0", response.Message);
    }

    [Fact]
    public async Task GetFaculty_InvalidPageSize_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.GetFaculty(1, 0);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(badRequestResult.Value);
        Assert.False(response.Success);
        Assert.Contains("Page size must be between 1 and 100", response.Message);
    }

    #endregion

    #region GetFacultyByEmpNr Tests

    [Fact]
    public async Task GetFacultyByEmpNr_ExistingFaculty_ReturnsFacultyDetails()
    {
        // Arrange
        var faculty = CreateTestProfessor();
        _mockFacultyService.Setup(x => x.GetFacultyByEmpNrAsync(1))
            .ReturnsAsync(faculty);

        // Act
        var result = await _controller.GetFaculty(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<FacultyDetailsResponse>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal("Dr. John Smith", response.Data?.Name);
    }

    [Fact]
    public async Task GetFacultyByEmpNr_NonExistentFaculty_ReturnsNotFound()
    {
        // Arrange
        _mockFacultyService.Setup(x => x.GetFacultyByEmpNrAsync(999))
            .ReturnsAsync((Academic?)null);

        // Act
        var result = await _controller.GetFaculty(999);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(notFoundResult.Value);
        Assert.False(response.Success);
        Assert.Contains("Faculty member with employee number 999 not found", response.Message);
    }

    #endregion

    #region SearchFaculty Tests

    [Fact]
    public async Task SearchFaculty_ValidRequest_ReturnsFilteredResults()
    {
        // Arrange
        var request = new FacultySearchRequest
        {
            SearchTerm = "John",
            DepartmentName = "Computer Science",
            Page = 1,
            PageSize = 10
        };

        var faculty = new List<Academic> { CreateTestProfessor() };
        var totalCount = 1;

        _mockFacultyService.Setup(x => x.SearchFacultyAsync(
            request.SearchTerm,
            request.DepartmentName,
            request.RankCode,
            request.HasTenure,
            request.ResearchArea,
            request.FacultyType,
            request.Page,
            request.PageSize))
            .ReturnsAsync((faculty, totalCount));

        // Act
        var result = await _controller.SearchFaculty(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<PagedResponse<FacultySummaryResponse>>(okResult.Value);
        Assert.Single(response.Data);
        Assert.Equal(1, response.TotalCount);
    }

    [Fact]
    public async Task SearchFaculty_EmptyRequest_ReturnsAllFaculty()
    {
        // Arrange
        var request = new FacultySearchRequest();
        var faculty = CreateTestFaculty();
        var totalCount = 2;

        _mockFacultyService.Setup(x => x.SearchFacultyAsync(
            null, null, null, null, null, null, 1, 10))
            .ReturnsAsync((faculty, totalCount));

        // Act
        var result = await _controller.SearchFaculty(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<PagedResponse<FacultySummaryResponse>>(okResult.Value);
        Assert.Equal(2, response.Data.Count());
        Assert.Equal(2, response.TotalCount);
    }

    #endregion

    #region CreateFaculty Tests

    [Fact]
    public async Task CreateFaculty_ValidProfessorRequest_ReturnsCreatedFaculty()
    {
        // Arrange
        var request = new CreateFacultyRequest
        {
            Name = "Dr. Alice Johnson",
            EmpNr = 3,
            PhoneNumber = "555-123-4567",
            Salary = 75000m,
            DepartmentName = "Physics",
            RankCode = "ASPR",
            FacultyType = FacultyType.Professor,
            ResearchArea = "Quantum Physics, Particle Physics",
            HasTenure = false
        };

        var createdFaculty = new Professor
        {
            EmpNr = request.EmpNr,
            Name = request.Name,
            PhoneNumber = request.PhoneNumber,
            Salary = request.Salary,
            DepartmentName = request.DepartmentName,
            RankCode = request.RankCode,
            ResearchArea = request.ResearchArea,
            HasTenure = request.HasTenure ?? false
        };

        _mockFacultyService.Setup(x => x.CreateFacultyAsync(It.IsAny<Academic>()))
            .ReturnsAsync(createdFaculty);

        // Act
        var result = await _controller.CreateFaculty(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsType<ApiResponse<FacultyDetailsResponse>>(createdResult.Value);
        Assert.True(response.Success);
        Assert.Equal("Dr. Alice Johnson", response.Data?.Name);
    }

    [Fact]
    public async Task CreateFaculty_ValidTeachingProfRequest_ReturnsCreatedFaculty()
    {
        // Arrange
        var request = new CreateFacultyRequest
        {
            Name = "Prof. Bob Wilson",
            EmpNr = 4,
            PhoneNumber = "555-987-6543",
            Salary = 68000m,
            DepartmentName = "English",
            RankCode = "ASTP",
            FacultyType = FacultyType.TeachingProf,
            HasTenure = false
        };

        var createdFaculty = new TeachingProf
        {
            EmpNr = request.EmpNr,
            Name = request.Name,
            PhoneNumber = request.PhoneNumber,
            Salary = request.Salary,
            DepartmentName = request.DepartmentName,
            RankCode = request.RankCode,
            HasTenure = request.HasTenure ?? false
        };

        _mockFacultyService.Setup(x => x.CreateFacultyAsync(It.IsAny<Academic>()))
            .ReturnsAsync(createdFaculty);

        // Act
        var result = await _controller.CreateFaculty(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsType<ApiResponse<FacultyDetailsResponse>>(createdResult.Value);
        Assert.True(response.Success);
        Assert.Equal("Prof. Bob Wilson", response.Data?.Name);
    }

    #endregion

    #region UpdateFaculty Tests

    [Fact]
    public async Task UpdateFaculty_ValidRequest_ReturnsUpdatedFaculty()
    {
        // Arrange
        var empNr = 1;
        var request = new UpdateFacultyRequest
        {
            Name = "Dr. John Smith Updated",
            PhoneNumber = "123-456-7890",
            Salary = 90000m,
            ResearchArea = "Updated Research Area"
        };

        var existingFaculty = CreateTestProfessor();
        var updatedFaculty = CreateTestProfessor();
        updatedFaculty.Name = request.Name;
        updatedFaculty.PhoneNumber = request.PhoneNumber;
        updatedFaculty.Salary = request.Salary;
        updatedFaculty.ResearchArea = request.ResearchArea;

        _mockFacultyService.Setup(x => x.GetFacultyByEmpNrAsync(empNr))
            .ReturnsAsync(existingFaculty);
        _mockFacultyService.Setup(x => x.UpdateFacultyAsync(It.IsAny<Academic>()))
            .ReturnsAsync(updatedFaculty);

        // Act
        var result = await _controller.UpdateFaculty(empNr, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<FacultyDetailsResponse>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal("Dr. John Smith Updated", response.Data?.Name);
    }

    [Fact]
    public async Task UpdateFaculty_NonExistentFaculty_ReturnsNotFound()
    {
        // Arrange
        var empNr = 999;
        var request = new UpdateFacultyRequest
        {
            Name = "Updated Name"
        };

        _mockFacultyService.Setup(x => x.GetFacultyByEmpNrAsync(empNr))
            .ReturnsAsync((Academic?)null);

        // Act
        var result = await _controller.UpdateFaculty(empNr, request);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(notFoundResult.Value);
        Assert.False(response.Success);
        Assert.Contains("Faculty member with employee number 999 not found", response.Message);
    }

    #endregion

    #region DeleteFaculty Tests

    [Fact]
    public async Task DeleteFaculty_ExistingFaculty_ReturnsNoContent()
    {
        // Arrange
        var empNr = 1;
        _mockFacultyService.Setup(x => x.DeleteFacultyAsync(empNr))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteFaculty(empNr);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteFaculty_NonExistentFaculty_ReturnsNotFound()
    {
        // Arrange
        var empNr = 999;
        _mockFacultyService.Setup(x => x.DeleteFacultyAsync(empNr))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteFaculty(empNr);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        var response = Assert.IsType<ApiResponse>(notFoundResult.Value);
        Assert.False(response.Success);
        Assert.Contains("Faculty member with employee number 999 not found", response.Message);
    }

    #endregion

    #region Tenure Management Tests

    [Fact]
    public async Task UpdateTenureStatus_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var empNr = 1;
        var request = new UpdateTenureStatusRequest
        {
            HasTenure = true,
            Notes = "Granted tenure after successful review"
        };

        var updatedFaculty = CreateTestProfessor();
        updatedFaculty.HasTenure = true;

        _mockFacultyService.Setup(x => x.UpdateTenureStatusAsync(empNr, request.HasTenure, request.Notes))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.UpdateTenureStatus(empNr, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<FacultyDetailsResponse>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Contains("Tenure status updated successfully", response.Message);
    }

    [Fact]
    public async Task UpdateTenureStatus_NonExistentFaculty_ReturnsNotFound()
    {
        // Arrange
        var empNr = 999;
        var request = new UpdateTenureStatusRequest
        {
            HasTenure = true
        };

        _mockFacultyService.Setup(x => x.UpdateTenureStatusAsync(empNr, request.HasTenure, request.Notes))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.UpdateTenureStatus(empNr, request);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(notFoundResult.Value);
        Assert.False(response.Success);
        Assert.Contains("Faculty member with employee number 999 not found", response.Message);
    }

    #endregion

    #region Rank Management Tests

    [Fact]
    public async Task UpdateRank_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var empNr = 1;
        var request = new UpdateRankRequest
        {
            RankCode = "PROF",
            EffectiveDate = new DateTime(2024, 7, 1),
            Notes = "Promoted to full professor"
        };

        var updatedFaculty = CreateTestProfessor();
        updatedFaculty.RankCode = "PROF";

        _mockFacultyService.Setup(x => x.UpdateRankAsync(empNr, request.RankCode, request.EffectiveDate, request.Notes))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.UpdateRank(empNr, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<FacultyDetailsResponse>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Contains("Rank updated successfully", response.Message);
    }

    [Fact]
    public async Task UpdateRank_NonExistentFaculty_ReturnsNotFound()
    {
        // Arrange
        var empNr = 999;
        var request = new UpdateRankRequest
        {
            RankCode = "PROF",
            EffectiveDate = new DateTime(2024, 7, 1)
        };

        _mockFacultyService.Setup(x => x.UpdateRankAsync(empNr, request.RankCode, request.EffectiveDate, request.Notes))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.UpdateRank(empNr, request);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(notFoundResult.Value);
        Assert.False(response.Success);
        Assert.Contains("Faculty member with employee number 999 not found", response.Message);
    }

    #endregion

    #region Statistics Tests

    [Fact]
    public async Task GetFacultyStatistics_ValidRequest_ReturnsStatistics()
    {
        // Arrange
        var expectedStats = new
        {
            TotalFaculty = 100,
            TotalProfessors = 60,
            TotalTeachingProfs = 40,
            FacultyWithTenure = 75,
            FacultyByDepartment = new Dictionary<string, int>
            {
                ["Computer Science"] = 25,
                ["Mathematics"] = 20,
                ["Physics"] = 15
            },
            FacultyByRank = new Dictionary<string, int>
            {
                ["PROF"] = 30,
                ["ASPR"] = 35,
                ["ASTP"] = 35
            }
        };

        _mockFacultyService.Setup(x => x.GetFacultyStatisticsAsync())
            .ReturnsAsync(expectedStats);

        // Act
        var result = await _controller.GetFacultyStatistics();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<object>>(okResult.Value);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
    }

    #endregion

    #region Exception Handling Tests

    [Fact]
    public async Task GetFaculty_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        _mockFacultyService.Setup(x => x.GetFacultyAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ThrowsAsync(new Exception("Database connection failed"));

        // Act
        var result = await _controller.GetFaculty(1, 10);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status500InternalServerError, statusResult.StatusCode);
        var response = Assert.IsType<ApiResponse>(statusResult.Value);
        Assert.False(response.Success);
        Assert.Contains("An error occurred while processing the request", response.Message);
    }

    #endregion
}