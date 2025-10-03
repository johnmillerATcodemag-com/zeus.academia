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
using Zeus.Academia.Domain.Services.Interfaces;
using Zeus.Academia.Infrastructure;
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

        _mockStudentService.Setup(x => x.GetAllStudentsAsync(1, 10))
            .ReturnsAsync(pagedStudents);

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
        _mockStudentService.Setup(x => x.GetStudentByEmpNrAsync(1))
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
        _mockStudentService.Setup(x => x.GetStudentByEmpNrAsync(999))
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
            DepartmentId = 1,
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
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            Nationality = request.Nationality,
            Address = request.Address,
            DegreeCode = request.DegreeCode,
            Program = request.Program,
            AcademicYear = request.AcademicYear,
            ExpectedGraduationDate = request.ExpectedGraduationDate,
            EnrollmentStatus = EnrollmentStatus.Pending,
            AcademicStanding = AcademicStanding.Good,
            IsActive = true,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow,
            CreatedBy = "System",
            ModifiedBy = "System"
        };

        _mockStudentService.Setup(x => x.GetStudentByStudentIdAsync(request.StudentId))
            .ReturnsAsync((Student?)null);

        _mockStudentService.Setup(x => x.CreateStudentAsync(
                request.StudentId,
                request.Name,
                request.Email,
                request.PhoneNumber,
                request.DateOfBirth,
                request.Gender,
                request.Nationality,
                request.Address,
                request.DepartmentId,
                request.DegreeCode,
                request.Program,
                request.AcademicYear,
                request.ExpectedGraduationDate))
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
        _mockStudentService.Setup(x => x.GetStudentByStudentIdAsync("STU001"))
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
        updatedStudent.Email = request.Email;
        updatedStudent.PhoneNumber = request.PhoneNumber;

        _mockStudentService.Setup(x => x.GetStudentByEmpNrAsync(1))
            .ReturnsAsync(existingStudent);

        _mockStudentService.Setup(x => x.UpdateStudentAsync(
                1,
                request.Name,
                request.Email,
                request.PhoneNumber,
                request.DateOfBirth,
                request.Gender,
                request.Nationality,
                request.Address,
                request.DepartmentId,
                request.DegreeCode,
                request.Program,
                request.ExpectedGraduationDate))
            .ReturnsAsync(updatedStudent);

        // Act
        var result = await _controller.UpdateStudent(1, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<StudentDetailsResponse>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal("John Updated", response.Data?.Name);
        Assert.Equal("john.updated@academia.com", response.Data?.Email);
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

        _mockStudentService.Setup(x => x.GetStudentByEmpNrAsync(999))
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
        _mockStudentService.Setup(x => x.GetStudentByEmpNrAsync(1))
            .ReturnsAsync(existingStudent);

        _mockStudentService.Setup(x => x.DeactivateStudentAsync(1))
            .Returns(Task.CompletedTask);

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
                request.DepartmentId,
                request.Program,
                request.EnrollmentStatus,
                request.AcademicStanding,
                request.MinGPA,
                request.MaxGPA,
                request.AcademicYear,
                request.IsActive,
                request.PageNumber,
                request.PageSize))
            .ReturnsAsync(pagedStudents);

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
        _mockStudentService.Setup(x => x.GetStudentByEmpNrAsync(1))
            .ReturnsAsync(existingStudent);

        _mockStudentService.Setup(x => x.UpdateEnrollmentStatusAsync(
                1,
                request.NewStatus,
                request.Reason,
                request.Notes,
                request.EffectiveDate))
            .Returns(Task.CompletedTask);

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
        _mockStudentService.Setup(x => x.GetStudentByEmpNrAsync(1))
            .ReturnsAsync(existingStudent);

        _mockStudentService.Setup(x => x.UpdateAcademicStandingAsync(
                1,
                request.NewStanding,
                request.Reason,
                request.Notes,
                request.ReviewDate))
            .Returns(Task.CompletedTask);

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
            ApplicantEmpNr = 1,
            Program = "Computer Science",
            DepartmentId = 1,
            ApplicationDate = DateTime.UtcNow,
            Priority = ApplicationPriority.Normal,
            PersonalStatement = "I am passionate about computer science..."
        };

        var application = new EnrollmentApplication
        {
            Id = 1,
            ApplicantEmpNr = request.ApplicantEmpNr,
            Program = request.Program,
            DepartmentId = request.DepartmentId,
            ApplicationDate = request.ApplicationDate,
            Priority = request.Priority,
            Status = ApplicationStatus.Submitted,
            Email = "john.doe@academia.com"
        };

        _mockStudentService.Setup(x => x.SubmitEnrollmentApplicationAsync(
                request.ApplicantEmpNr,
                request.Program,
                request.DepartmentId,
                request.ApplicationDate,
                request.Priority,
                request.PersonalStatement,
                request.AcademicHistory,
                request.References,
                request.AdditionalDocuments))
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
        var history = new List<EnrollmentHistory>
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

        _mockStudentService.Setup(x => x.GetStudentByEmpNrAsync(1))
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
            Email = "john.doe@academia.com",
            PhoneNumber = "123-456-7890",
            DateOfBirth = new DateTime(1995, 1, 15),
            Gender = "Male",
            Nationality = "American",
            Address = "123 Main St",
            DegreeCode = "CS",
            Program = "Computer Science",
            EnrollmentStatus = EnrollmentStatus.Enrolled,
            AcademicStanding = AcademicStanding.Good,
            CumulativeGPA = 3.5m,
            CreditHoursCompleted = 60,
            AcademicYear = 2024,
            ExpectedGraduationDate = new DateTime(2026, 5, 15),
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
                Email = "jane.smith@academia.com",
                PhoneNumber = "987-654-3210",
                DateOfBirth = new DateTime(1996, 3, 20),
                Gender = "Female",
                Nationality = "Canadian",
                Address = "456 Oak Ave",
                DegreeCode = "EE",
                Program = "Electrical Engineering",
                EnrollmentStatus = EnrollmentStatus.Enrolled,
                AcademicStanding = AcademicStanding.Good,
                CumulativeGPA = 3.8m,
                CreditHoursCompleted = 45,
                AcademicYear = 2024,
                ExpectedGraduationDate = new DateTime(2027, 5, 15),
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