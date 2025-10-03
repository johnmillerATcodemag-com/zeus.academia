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
using Zeus.Academia.Infrastructure;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Enums;

namespace Zeus.Academia.Api.UnitTests.Controllers.V1;

/// <summary>
/// Unit tests for the StudentsController.
/// </summary>
public class StudentsControllerTests
{
    private readonly Mock<IStudentService> _mockStudentService;
    private readonly Mock<IStudentProfileService> _mockStudentProfileService;
    private readonly Mock<IAcademicRecordService> _mockAcademicRecordService;
    private readonly Mock<ILogger<StudentsController>> _mockLogger;
    private readonly IMapper _mapper;
    private readonly StudentsController _controller;

    public StudentsControllerTests()
    {
        _mockStudentService = new Mock<IStudentService>();
        _mockStudentProfileService = new Mock<IStudentProfileService>();
        _mockAcademicRecordService = new Mock<IAcademicRecordService>();
        _mockLogger = new Mock<ILogger<StudentsController>>();

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<StudentMappingProfile>();
        });
        _mapper = mapperConfig.CreateMapper();

        _controller = new StudentsController(
            _mockStudentService.Object,
            _mockStudentProfileService.Object,
            _mockAcademicRecordService.Object,
            _mapper,
            _mockLogger.Object);
    }

    [Fact]
    public async Task GetStudents_ValidRequest_ReturnsPagedResponse()
    {
        // Arrange
        var students = CreateTestStudents();
        var pagedStudents = new PagedList<Student>(students, students.Count, 1, 10);

        _mockStudentService.Setup(x => x.GetStudentsAsync(1, 10))
            .ReturnsAsync((students, students.Count));

        // Act
        var result = await _controller.GetStudents(1, 10);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<PagedResponse<StudentSummaryResponse>>(okResult.Value);
        Assert.Equal(2, response.Data.Count());
        Assert.Equal(1, response.PageNumber);
        Assert.Equal(10, response.PageSize);
        Assert.Equal(2, response.TotalCount);
    }

    [Fact]
    public async Task GetStudent_ExistingStudent_ReturnsStudentDetails()
    {
        // Arrange
        var student = CreateTestStudent();
        _mockStudentService.Setup(x => x.GetStudentByIdAsync(1))
            .ReturnsAsync(student);

        // Act
        var result = await _controller.GetStudent(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<StudentDetailsResponse>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal("STU001", response.Data?.StudentId);
        Assert.Equal("John Doe", response.Data?.Name);
    }

    [Fact]
    public async Task GetStudent_NonExistentStudent_ReturnsNotFound()
    {
        // Arrange
        _mockStudentService.Setup(x => x.GetStudentByIdAsync(999))
            .ReturnsAsync((Student?)null);

        // Act
        var result = await _controller.GetStudent(999);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(notFoundResult.Value);
        Assert.False(response.Success);
        Assert.Contains("not found", response.Message);
    }

    [Fact]
    public async Task GetStudentByStudentId_ExistingStudent_ReturnsStudentDetails()
    {
        // Arrange
        var student = CreateTestStudent();
        _mockStudentService.Setup(x => x.GetStudentByStudentIdAsync("STU001"))
            .ReturnsAsync(student);

        // Act
        var result = await _controller.GetStudentByStudentId("STU001");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<StudentDetailsResponse>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal("STU001", response.Data?.StudentId);
    }

    [Fact]
    public async Task CreateStudent_ValidRequest_ReturnsCreatedStudent()
    {
        // Arrange
        var request = new CreateStudentRequest
        {
            StudentId = "STU003",
            Name = "Jane Smith",
            Email = "jane.smith@academia.com",
            PhoneNumber = "123-456-7890",
            DateOfBirth = new DateTime(1995, 5, 15),
            Gender = "Female",
            Nationality = "American",
            Address = "123 Main St",
            DepartmentName = "Computer Science",
            DegreeCode = "CS",
            Program = "Computer Science",
            AcademicYear = 2024,
            ExpectedGraduationDate = new DateTime(2028, 5, 15)
        };

        var createdStudent = new Student
        {
            EmpNr = 3,
            StudentId = request.StudentId,
            Name = request.Name,
            PhoneNumber = request.PhoneNumber,
            DegreeCode = request.DegreeCode,
            DepartmentName = request.DepartmentName,
            Program = request.Program,
            ExpectedGraduationDate = request.ExpectedGraduationDate,
            EnrollmentStatus = EnrollmentStatus.Pending,
            AcademicStanding = AcademicStanding.Good,
            IsActive = true,
            YearOfStudy = 1,
            GPA = 0.0m,
            EnrollmentDate = DateTime.UtcNow,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow,
            CreatedBy = "System",
            ModifiedBy = "System"
        };

        _mockStudentService.Setup(x => x.GetStudentByStudentNumberAsync(request.StudentId))
            .ReturnsAsync((Student?)null);

        _mockStudentService.Setup(x => x.CreateStudentAsync(It.IsAny<Student>()))
            .ReturnsAsync(createdStudent);

        // Act
        var result = await _controller.CreateStudent(request);

        // Assert
        var createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsType<ApiResponse<StudentDetailsResponse>>(createdAtResult.Value);
        Assert.True(response.Success);
        Assert.Equal("STU003", response.Data?.StudentId);
        Assert.Equal("Jane Smith", response.Data?.Name);
    }

    [Fact]
    public async Task CreateStudent_DuplicateStudentId_ReturnsConflict()
    {
        // Arrange
        var request = new CreateStudentRequest
        {
            StudentId = "STU001",
            Name = "Jane Smith",
            Email = "jane.smith@academia.com"
        };

        var existingStudent = CreateTestStudent();
        _mockStudentService.Setup(x => x.GetStudentByStudentNumberAsync("STU001"))
            .ReturnsAsync(existingStudent);

        // Act
        var result = await _controller.CreateStudent(request);

        // Assert
        var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(conflictResult.Value);
        Assert.False(response.Success);
        Assert.Contains("already exists", response.Message);
    }

    [Fact]
    public async Task UpdateStudent_ValidRequest_ReturnsUpdatedStudent()
    {
        // Arrange
        var request = new UpdateStudentRequest
        {
            Name = "John Updated",
            Email = "john.updated@academia.com",
            PhoneNumber = "987-654-3210"
        };

        var existingStudent = CreateTestStudent();
        var updatedStudent = CreateTestStudent();
        updatedStudent.Name = request.Name;
        updatedStudent.PhoneNumber = request.PhoneNumber;

        _mockStudentService.Setup(x => x.GetStudentByIdAsync(1))
            .ReturnsAsync(existingStudent);

        _mockStudentService.Setup(x => x.UpdateStudentAsync(It.IsAny<Student>()))
            .ReturnsAsync(updatedStudent);

        // Act
        var result = await _controller.UpdateStudent(1, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<StudentDetailsResponse>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal("John Updated", response.Data?.Name);
        Assert.Equal("987-654-3210", response.Data?.PhoneNumber);
    }

    [Fact]
    public async Task UpdateStudent_NonExistentStudent_ReturnsNotFound()
    {
        // Arrange
        var request = new UpdateStudentRequest
        {
            Name = "John Updated",
            Email = "john.updated@academia.com"
        };

        _mockStudentService.Setup(x => x.GetStudentByIdAsync(999))
            .ReturnsAsync((Student?)null);

        // Act
        var result = await _controller.UpdateStudent(999, request);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(notFoundResult.Value);
        Assert.False(response.Success);
        Assert.Contains("not found", response.Message);
    }

    [Fact]
    public async Task DeleteStudent_ExistingStudent_ReturnsSuccess()
    {
        // Arrange
        var existingStudent = CreateTestStudent();
        _mockStudentService.Setup(x => x.GetStudentByIdAsync(1))
            .ReturnsAsync(existingStudent);

        _mockStudentService.Setup(x => x.DeleteStudentAsync(1))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteStudent(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(okResult.Value);
        Assert.True(response.Success);
        Assert.Contains("deactivated successfully", response.Message);
    }

    [Fact]
    public async Task SearchStudents_ValidCriteria_ReturnsFilteredResults()
    {
        // Arrange
        var request = new StudentSearchRequest
        {
            SearchTerm = "John",
            DepartmentId = 1,
            EnrollmentStatus = EnrollmentStatus.Enrolled,
            PageNumber = 1,
            PageSize = 10
        };

        var students = new List<Student> { CreateTestStudent() };
        var pagedStudents = new PagedList<Student>(students, students.Count, 1, 10);

        _mockStudentService.Setup(x => x.SearchStudentsAsync(
                request.SearchTerm,
                null, // departmentName - request.DepartmentId is int, but service expects string
                request.EnrollmentStatus,
                request.AcademicStanding,
                request.Program,
                request.PageNumber,
                request.PageSize))
            .ReturnsAsync((students, students.Count));

        // Act
        var result = await _controller.SearchStudents(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<PagedResponse<StudentSummaryResponse>>(okResult.Value);
        Assert.Single(response.Data);
        Assert.Equal("John Doe", response.Data.First().Name);
    }

    [Fact]
    public async Task UpdateEnrollmentStatus_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var request = new UpdateEnrollmentStatusRequest
        {
            NewStatus = EnrollmentStatus.Enrolled,
            Reason = "Student met all requirements",
            EffectiveDate = DateTime.UtcNow
        };

        var existingStudent = CreateTestStudent();
        _mockStudentService.Setup(x => x.GetStudentByIdAsync(1))
            .ReturnsAsync(existingStudent);

        _mockStudentService.Setup(x => x.UpdateEnrollmentStatusAsync(
                1,
                request.NewStatus,
                request.Reason))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.UpdateEnrollmentStatus(1, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(okResult.Value);
        Assert.True(response.Success);
        Assert.Contains("updated successfully", response.Message);
    }

    [Fact]
    public async Task UpdateAcademicStanding_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var request = new UpdateAcademicStandingRequest
        {
            NewStanding = AcademicStanding.Warning,
            Reason = "GPA below threshold",
            ReviewDate = DateTime.UtcNow
        };

        var existingStudent = CreateTestStudent();
        _mockStudentService.Setup(x => x.GetStudentByIdAsync(1))
            .ReturnsAsync(existingStudent);

        _mockStudentService.Setup(x => x.UpdateAcademicStandingAsync(
                1,
                request.NewStanding,
                request.Reason))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.UpdateAcademicStanding(1, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse>(okResult.Value);
        Assert.True(response.Success);
        Assert.Contains("updated successfully", response.Message);
    }

    [Fact]
    public async Task SubmitApplication_ValidRequest_ReturnsCreatedApplication()
    {
        // Arrange
        var request = new SubmitApplicationRequest
        {
            ApplicantEmpNr = "1", // String as expected by SubmitApplicationRequest
            Program = "Computer Science",
            DepartmentId = 1,
            ApplicationDate = DateTime.UtcNow,
            Priority = "Normal", // String as expected by SubmitApplicationRequest
            PersonalStatement = "I am passionate about computer science..."
        };

        var application = new Zeus.Academia.Infrastructure.Entities.EnrollmentApplication
        {
            Id = 1,
            ApplicantEmpNr = int.TryParse(request.ApplicantEmpNr, out var empNr) ? empNr : null, // Convert string to int?
            ApplicantName = "John Doe",
            Program = request.Program,
            DepartmentName = "Computer Science", // Using DepartmentName instead of DepartmentId
            ApplicationDate = request.ApplicationDate ?? DateTime.UtcNow, // Handle nullable DateTime
            Priority = Enum.TryParse<ApplicationPriority>(request.Priority, out var priority) ? priority : ApplicationPriority.Normal, // Convert string to enum
            Status = ApplicationStatus.Submitted,
            Email = "john.doe@academia.com"
        };

        _mockStudentService.Setup(x => x.SubmitEnrollmentApplicationAsync(It.IsAny<Zeus.Academia.Infrastructure.Entities.EnrollmentApplication>()))
            .ReturnsAsync(application);

        // Act
        var result = await _controller.SubmitApplication(request);

        // Assert
        var createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsType<ApiResponse<EnrollmentApplicationResponse>>(createdAtResult.Value);
        Assert.True(response.Success);
        Assert.Equal(1, response.Data?.Id);
        Assert.Equal("Computer Science", response.Data?.Program);
    }

    [Fact]
    public async Task GetEnrollmentHistory_ExistingStudent_ReturnsHistory()
    {
        // Arrange
        var existingStudent = CreateTestStudent();
        var history = new List<Zeus.Academia.Infrastructure.Entities.EnrollmentHistory>
        {
            new()
            {
                Id = 1,
                StudentEmpNr = 1,
                EventType = EnrollmentEventType.Enrolled,
                NewStatus = EnrollmentStatus.Enrolled,
                EventDate = DateTime.UtcNow.AddDays(-30),
                Reason = "Successfully completed admission requirements"
            }
        };

        _mockStudentService.Setup(x => x.GetStudentByIdAsync(1))
            .ReturnsAsync(existingStudent);

        _mockStudentService.Setup(x => x.GetEnrollmentHistoryAsync(1))
            .ReturnsAsync(history);

        // Act
        var result = await _controller.GetEnrollmentHistory(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<IEnumerable<EnrollmentHistoryResponse>>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Single(response.Data!);
        Assert.Equal(EnrollmentEventType.Enrolled, response.Data!.First().EventType);
    }

    private static Student CreateTestStudent()
    {
        return new Student
        {
            EmpNr = 1,
            StudentId = "STU001",
            Name = "John Doe",
            PhoneNumber = "123-456-7890",
            DegreeCode = "CS",
            DepartmentName = "Computer Science",
            Program = "Computer Science",
            YearOfStudy = 2,
            GPA = 3.5m,
            EnrollmentDate = DateTime.UtcNow.AddMonths(-6),
            ExpectedGraduationDate = new DateTime(2026, 5, 15),
            EnrollmentStatus = EnrollmentStatus.Enrolled,
            AcademicStanding = AcademicStanding.Good,
            CumulativeGPA = 3.5m,
            TotalCreditHoursAttempted = 60,
            TotalCreditHoursEarned = 60,
            IsActive = true,
            CreatedDate = DateTime.UtcNow.AddMonths(-6),
            ModifiedDate = DateTime.UtcNow,
            CreatedBy = "System",
            ModifiedBy = "System"
        };
    }

    private static List<Student> CreateTestStudents()
    {
        return new List<Student>
        {
            CreateTestStudent(),
            new()
            {
                EmpNr = 2,
                StudentId = "STU002",
                Name = "Jane Smith",
                PhoneNumber = "987-654-3210",
                DegreeCode = "EE",
                DepartmentName = "Electrical Engineering",
                Program = "Electrical Engineering",
                YearOfStudy = 3,
                GPA = 3.8m,
                EnrollmentDate = DateTime.UtcNow.AddMonths(-4),
                ExpectedGraduationDate = new DateTime(2027, 5, 15),
                EnrollmentStatus = EnrollmentStatus.Enrolled,
                AcademicStanding = AcademicStanding.Good,
                CumulativeGPA = 3.8m,
                TotalCreditHoursAttempted = 45,
                TotalCreditHoursEarned = 45,
                IsActive = true,
                CreatedDate = DateTime.UtcNow.AddMonths(-4),
                ModifiedDate = DateTime.UtcNow,
                CreatedBy = "System",
                ModifiedBy = "System"
            }
        };
    }
}

/// <summary>
/// Helper class to simulate a paged list.
/// </summary>
public class PagedList<T> : List<T>
{
    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public int TotalPages { get; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
    public IEnumerable<T> Items => this;

    public PagedList(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
    {
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        AddRange(items);
    }
}