using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Zeus.Academia.Api.Controllers.Base;
using Zeus.Academia.Api.Models.Common;
using Zeus.Academia.Api.Models.Requests;
using Zeus.Academia.Api.Models.Responses;
using Zeus.Academia.Infrastructure.Services.Interfaces;
using Zeus.Academia.Infrastructure.Enums;
using Zeus.Academia.Infrastructure.Entities;
using AutoMapper;

namespace Zeus.Academia.Api.Controllers.V1;

/// <summary>
/// Controller for managing student operations.
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class StudentsController : BaseApiController
{
    private readonly IStudentService _studentService;
    private readonly IStudentProfileService _studentProfileService;
    private readonly IAcademicRecordService _academicRecordService;
    private readonly IMapper _mapper;
    private readonly ILogger<StudentsController> _logger;

    /// <summary>
    /// Initializes a new instance of the StudentsController.
    /// </summary>
    public StudentsController(
        IStudentService studentService,
        IStudentProfileService studentProfileService,
        IAcademicRecordService academicRecordService,
        IMapper mapper,
        ILogger<StudentsController> logger)
    {
        _studentService = studentService ?? throw new ArgumentNullException(nameof(studentService));
        _studentProfileService = studentProfileService ?? throw new ArgumentNullException(nameof(studentProfileService));
        _academicRecordService = academicRecordService ?? throw new ArgumentNullException(nameof(academicRecordService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Retrieves a paginated list of students.
    /// </summary>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A paginated list of students.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<StudentSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResponse<StudentSummaryResponse>>> GetStudents(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var studentsResult = await _studentService.GetStudentsAsync(pageNumber, pageSize);
            var studentResponses = _mapper.Map<IEnumerable<StudentSummaryResponse>>(studentsResult.Students);

            var totalPages = (int)Math.Ceiling((double)studentsResult.TotalCount / pageSize);

            var response = new PagedResponse<StudentSummaryResponse>
            {
                Data = studentResponses,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = studentsResult.TotalCount,
                TotalPages = totalPages,
                HasPreviousPage = pageNumber > 1,
                HasNextPage = pageNumber < totalPages
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving students");
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while retrieving students"));
        }
    }

    /// <summary>
    /// Searches for students based on criteria.
    /// </summary>
    /// <param name="request">The search criteria.</param>
    /// <returns>A paginated list of matching students.</returns>
    [HttpPost("search")]
    [ProducesResponseType(typeof(PagedResponse<StudentSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResponse<StudentSummaryResponse>>> SearchStudents(
        [FromBody] StudentSearchRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CreateValidationErrorResponse(ModelState));
            }

            var studentsResult = await _studentService.SearchStudentsAsync(
                request.SearchTerm,
                request.DepartmentId?.ToString(),
                request.EnrollmentStatus,
                request.AcademicStanding,
                request.Program,
                request.PageNumber,
                request.PageSize);

            var studentResponses = _mapper.Map<IEnumerable<StudentSummaryResponse>>(studentsResult.Students);

            var totalPages = (int)Math.Ceiling((double)studentsResult.TotalCount / request.PageSize);

            var response = new PagedResponse<StudentSummaryResponse>
            {
                Data = studentResponses,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = studentsResult.TotalCount,
                TotalPages = totalPages,
                HasPreviousPage = request.PageNumber > 1,
                HasNextPage = request.PageNumber < totalPages
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching students with criteria: {@SearchCriteria}", request);
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while searching students"));
        }
    }

    /// <summary>
    /// Retrieves a specific student by employee number.
    /// </summary>
    /// <param name="empNr">The student's employee number.</param>
    /// <returns>The student details.</returns>
    [HttpGet("{empNr:int}")]
    [ProducesResponseType(typeof(ApiResponse<StudentDetailsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<StudentDetailsResponse>>> GetStudent(int empNr)
    {
        try
        {
            var student = await _studentService.GetStudentByEmpNrAsync(empNr.ToString());
            if (student == null)
            {
                return NotFound(CreateErrorResponse($"Student with employee number {empNr} not found"));
            }

            var studentResponse = _mapper.Map<StudentDetailsResponse>(student);
            return Ok(CreateSuccessResponse(studentResponse));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving student with employee number {EmpNr}", empNr);
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while retrieving the student"));
        }
    }

    /// <summary>
    /// Retrieves a specific student by student ID.
    /// </summary>
    /// <param name="studentId">The student's unique ID.</param>
    /// <returns>The student details.</returns>
    [HttpGet("by-student-id/{studentId}")]
    [ProducesResponseType(typeof(ApiResponse<StudentDetailsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<StudentDetailsResponse>>> GetStudentByStudentId(string studentId)
    {
        try
        {
            var student = await _studentService.GetStudentByStudentIdAsync(studentId);
            if (student == null)
            {
                return NotFound(CreateErrorResponse($"Student with ID {studentId} not found"));
            }

            var studentResponse = _mapper.Map<StudentDetailsResponse>(student);
            return Ok(CreateSuccessResponse(studentResponse));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving student with ID {StudentId}", studentId);
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while retrieving the student"));
        }
    }

    /// <summary>
    /// Creates a new student.
    /// </summary>
    /// <param name="request">The student creation data.</param>
    /// <returns>The created student details.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<StudentDetailsResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<StudentDetailsResponse>>> CreateStudent(
        [FromBody] CreateStudentRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CreateValidationErrorResponse(ModelState));
            }

            // Check if student ID already exists
            var existingStudent = await _studentService.GetStudentByStudentIdAsync(request.StudentId);
            if (existingStudent != null)
            {
                return Conflict(CreateErrorResponse($"A student with ID {request.StudentId} already exists"));
            }

            var newStudent = new Student
            {
                StudentId = request.StudentId,
                Name = request.Name,
                PhoneNumber = request.PhoneNumber,
                DepartmentName = request.DepartmentName,
                DegreeCode = request.DegreeCode,
                Program = request.Program,
                ExpectedGraduationDate = request.ExpectedGraduationDate,
                EnrollmentStatus = request.EnrollmentStatus,
                AcademicStanding = request.AcademicStanding,
                EnrollmentDate = DateTime.UtcNow,
                IsActive = true
            };

            var student = await _studentService.CreateStudentAsync(newStudent);

            var studentResponse = _mapper.Map<StudentDetailsResponse>(student);

            _logger.LogInformation("Student created successfully with ID {StudentId} and EmpNr {EmpNr}",
                student.StudentId, student.EmpNr);

            return CreatedAtAction(
                nameof(GetStudent),
                new { empNr = student.EmpNr },
                CreateSuccessResponse(studentResponse, "Student created successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while creating student: {Message}", ex.Message);
            return BadRequest(CreateErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating student with ID {StudentId}", request.StudentId);
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while creating the student"));
        }
    }

    /// <summary>
    /// Updates an existing student.
    /// </summary>
    /// <param name="empNr">The student's employee number.</param>
    /// <param name="request">The updated student data.</param>
    /// <returns>The updated student details.</returns>
    [HttpPut("{empNr:int}")]
    [ProducesResponseType(typeof(ApiResponse<StudentDetailsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<StudentDetailsResponse>>> UpdateStudent(
        int empNr,
        [FromBody] UpdateStudentRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CreateValidationErrorResponse(ModelState));
            }

            var existingStudent = await _studentService.GetStudentByEmpNrAsync(empNr.ToString());
            if (existingStudent == null)
            {
                return NotFound(CreateErrorResponse($"Student with employee number {empNr} not found"));
            }

            // Update the existing student object with new values
            existingStudent.Name = request.Name;
            existingStudent.PhoneNumber = request.PhoneNumber;
            if (request.DepartmentName != null)
                existingStudent.DepartmentName = request.DepartmentName;
            if (request.DegreeCode != null)
                existingStudent.DegreeCode = request.DegreeCode;
            if (request.Program != null)
                existingStudent.Program = request.Program;

            var updatedStudent = await _studentService.UpdateStudentAsync(existingStudent);

            var studentResponse = _mapper.Map<StudentDetailsResponse>(updatedStudent);

            _logger.LogInformation("Student updated successfully with EmpNr {EmpNr}", empNr);

            return Ok(CreateSuccessResponse(studentResponse, "Student updated successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while updating student {EmpNr}: {Message}", empNr, ex.Message);
            return BadRequest(CreateErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating student with EmpNr {EmpNr}", empNr);
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while updating the student"));
        }
    }

    /// <summary>
    /// Deletes a student (soft delete by deactivating).
    /// </summary>
    /// <param name="empNr">The student's employee number.</param>
    /// <returns>A success response.</returns>
    [HttpDelete("{empNr:int}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> DeleteStudent(int empNr)
    {
        try
        {
            var existingStudent = await _studentService.GetStudentByEmpNrAsync(empNr.ToString());
            if (existingStudent == null)
            {
                return NotFound(CreateErrorResponse($"Student with employee number {empNr} not found"));
            }

            await _studentService.DeactivateStudentAsync(empNr);

            _logger.LogInformation("Student deactivated successfully with EmpNr {EmpNr}", empNr);

            return Ok(CreateSuccessResponse("Student deactivated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating student with EmpNr {EmpNr}", empNr);
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while deactivating the student"));
        }
    }

    /// <summary>
    /// Updates a student's enrollment status.
    /// </summary>
    /// <param name="empNr">The student's employee number.</param>
    /// <param name="request">The enrollment status update data.</param>
    /// <returns>A success response.</returns>
    [HttpPut("{empNr:int}/enrollment-status")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> UpdateEnrollmentStatus(
        int empNr,
        [FromBody] UpdateEnrollmentStatusRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CreateValidationErrorResponse(ModelState));
            }

            var existingStudent = await _studentService.GetStudentByEmpNrAsync(empNr.ToString());
            if (existingStudent == null)
            {
                return NotFound(CreateErrorResponse($"Student with employee number {empNr} not found"));
            }

            await _studentService.UpdateEnrollmentStatusAsync(
                empNr,
                request.NewStatus,
                request.Notes);

            _logger.LogInformation("Enrollment status updated for student {EmpNr} to {Status}",
                empNr, request.NewStatus);

            return Ok(CreateSuccessResponse("Enrollment status updated successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while updating enrollment status for student {EmpNr}: {Message}",
                empNr, ex.Message);
            return BadRequest(CreateErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating enrollment status for student {EmpNr}", empNr);
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while updating the enrollment status"));
        }
    }

    /// <summary>
    /// Updates a student's academic standing.
    /// </summary>
    /// <param name="empNr">The student's employee number.</param>
    /// <param name="request">The academic standing update data.</param>
    /// <returns>A success response.</returns>
    [HttpPut("{empNr:int}/academic-standing")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> UpdateAcademicStanding(
        int empNr,
        [FromBody] UpdateAcademicStandingRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CreateValidationErrorResponse(ModelState));
            }

            var existingStudent = await _studentService.GetStudentByEmpNrAsync(empNr.ToString());
            if (existingStudent == null)
            {
                return NotFound(CreateErrorResponse($"Student with employee number {empNr} not found"));
            }

            await _studentService.UpdateAcademicStandingAsync(
                empNr,
                request.NewStanding,
                request.Notes);

            _logger.LogInformation("Academic standing updated for student {EmpNr} to {Standing}",
                empNr, request.NewStanding);

            return Ok(CreateSuccessResponse("Academic standing updated successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while updating academic standing for student {EmpNr}: {Message}",
                empNr, ex.Message);
            return BadRequest(CreateErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating academic standing for student {EmpNr}", empNr);
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while updating the academic standing"));
        }
    }

    /// <summary>
    /// Submits an enrollment application for a student.
    /// </summary>
    /// <param name="request">The application submission data.</param>
    /// <returns>The submitted application details.</returns>
    [HttpPost("applications")]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentApplicationResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<EnrollmentApplicationResponse>>> SubmitApplication(
        [FromBody] SubmitApplicationRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CreateValidationErrorResponse(ModelState));
            }

            var enrollmentApplication = new EnrollmentApplication
            {
                ApplicantEmpNr = int.TryParse(request.ApplicantEmpNr, out var empNr) ? empNr : null,
                Program = request.Program,
                ApplicationDate = request.ApplicationDate ?? DateTime.UtcNow,
                Status = ApplicationStatus.Submitted
            };

            var application = await _studentService.SubmitEnrollmentApplicationAsync(enrollmentApplication);

            var applicationResponse = _mapper.Map<EnrollmentApplicationResponse>(application);

            _logger.LogInformation("Enrollment application submitted successfully with ID {ApplicationId}",
                application.Id);

            return CreatedAtAction(
                nameof(GetApplication),
                new { applicationId = application.Id },
                CreateSuccessResponse(applicationResponse, "Application submitted successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while submitting application: {Message}", ex.Message);
            return BadRequest(CreateErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting enrollment application");
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while submitting the application"));
        }
    }

    /// <summary>
    /// Retrieves an enrollment application by ID.
    /// </summary>
    /// <param name="applicationId">The application ID.</param>
    /// <returns>The application details.</returns>
    [HttpGet("applications/{applicationId:int}")]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentApplicationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<EnrollmentApplicationResponse>>> GetApplication(int applicationId)
    {
        try
        {
            var application = await _studentService.GetEnrollmentApplicationAsync(applicationId);
            if (application == null)
            {
                return NotFound(CreateErrorResponse($"Application with ID {applicationId} not found"));
            }

            var applicationResponse = _mapper.Map<EnrollmentApplicationResponse>(application);
            return Ok(CreateSuccessResponse(applicationResponse));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving application with ID {ApplicationId}", applicationId);
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while retrieving the application"));
        }
    }

    /// <summary>
    /// Processes an admission decision for an application.
    /// </summary>
    /// <param name="applicationId">The application ID.</param>
    /// <param name="request">The admission decision data.</param>
    /// <returns>A success response.</returns>
    [HttpPost("applications/{applicationId:int}/decision")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> ProcessAdmissionDecision(
        int applicationId,
        [FromBody] ProcessAdmissionDecisionRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CreateValidationErrorResponse(ModelState));
            }

            var application = await _studentService.GetEnrollmentApplicationAsync(applicationId);
            if (application == null)
            {
                return NotFound(CreateErrorResponse($"Application with ID {applicationId} not found"));
            }

            await _studentService.ProcessAdmissionDecisionAsync(
                applicationId,
                request.Decision,
                request.DecisionReason,
                "AdminUser", // TODO: Get from authenticated user context
                request.ConditionalRequirements);

            _logger.LogInformation("Admission decision processed for application {ApplicationId}: {Decision}",
                applicationId, request.Decision);

            return Ok(CreateSuccessResponse("Admission decision processed successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while processing admission decision for application {ApplicationId}: {Message}",
                applicationId, ex.Message);
            return BadRequest(CreateErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing admission decision for application {ApplicationId}", applicationId);
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while processing the admission decision"));
        }
    }

    /// <summary>
    /// Processes enrollment for an accepted application.
    /// </summary>
    /// <param name="applicationId">The application ID.</param>
    /// <param name="request">The enrollment processing data.</param>
    /// <returns>A success response.</returns>
    [HttpPost("applications/{applicationId:int}/enroll")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> ProcessEnrollment(
        int applicationId,
        [FromBody] ProcessEnrollmentRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CreateValidationErrorResponse(ModelState));
            }

            var application = await _studentService.GetEnrollmentApplicationAsync(applicationId);
            if (application == null)
            {
                return NotFound(CreateErrorResponse($"Application with ID {applicationId} not found"));
            }

            await _studentService.ProcessEnrollmentAsync(
                applicationId,
                request.EnrollmentDate,
                int.TryParse(request.AcademicTerm, out var termId) ? termId : 1,
                request.Notes);

            _logger.LogInformation("Enrollment processed successfully for application {ApplicationId}", applicationId);

            return Ok(CreateSuccessResponse("Enrollment processed successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while processing enrollment for application {ApplicationId}: {Message}",
                applicationId, ex.Message);
            return BadRequest(CreateErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing enrollment for application {ApplicationId}", applicationId);
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while processing the enrollment"));
        }
    }

    /// <summary>
    /// Retrieves enrollment history for a student.
    /// </summary>
    /// <param name="empNr">The student's employee number.</param>
    /// <returns>The enrollment history.</returns>
    [HttpGet("{empNr:int}/enrollment-history")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<EnrollmentHistoryResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<IEnumerable<EnrollmentHistoryResponse>>>> GetEnrollmentHistory(int empNr)
    {
        try
        {
            var existingStudent = await _studentService.GetStudentByEmpNrAsync(empNr.ToString());
            if (existingStudent == null)
            {
                return NotFound(CreateErrorResponse($"Student with employee number {empNr} not found"));
            }

            var history = await _studentService.GetEnrollmentHistoryAsync(empNr);
            var historyResponse = _mapper.Map<IEnumerable<EnrollmentHistoryResponse>>(history);

            return Ok(CreateSuccessResponse(historyResponse));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving enrollment history for student {EmpNr}", empNr);
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while retrieving the enrollment history"));
        }
    }

    /// <summary>
    /// Retrieves student statistics.
    /// </summary>
    /// <returns>Student statistics summary.</returns>
    [HttpGet("statistics")]
    [ProducesResponseType(typeof(ApiResponse<StudentStatisticsResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<StudentStatisticsResponse>>> GetStudentStatistics()
    {
        try
        {
            var statistics = await _studentService.GetStudentStatisticsAsync();
            var statisticsResponse = _mapper.Map<StudentStatisticsResponse>(statistics);

            return Ok(CreateSuccessResponse(statisticsResponse));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving student statistics");
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while retrieving student statistics"));
        }
    }

    /// <summary>
    /// Uploads a document for a student.
    /// </summary>
    /// <param name="empNr">The student's employee number.</param>
    /// <param name="file">The file to upload.</param>
    /// <param name="documentType">The type of document.</param>
    /// <param name="description">Optional description of the document.</param>
    /// <returns>File upload result.</returns>
    [HttpPost("{empNr:int}/documents")]
    [ProducesResponseType(typeof(ApiResponse<FileUploadResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<FileUploadResponse>>> UploadDocument(
        int empNr,
        [Required] IFormFile file,
        [FromForm] string documentType,
        [FromForm] string? description = null)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(CreateErrorResponse("No file provided"));
            }

            var existingStudent = await _studentService.GetStudentByEmpNrAsync(empNr.ToString());
            if (existingStudent == null)
            {
                return NotFound(CreateErrorResponse($"Student with employee number {empNr} not found"));
            }

            // Validate file type and size
            const long maxFileSize = 10 * 1024 * 1024; // 10MB
            if (file.Length > maxFileSize)
            {
                return BadRequest(CreateErrorResponse("File size exceeds the maximum allowed size of 10MB"));
            }

            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest(CreateErrorResponse("File type not allowed. Allowed types: PDF, DOC, DOCX, JPG, JPEG, PNG"));
            }

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var fileContent = memoryStream.ToArray();

            var uploadResult = await _studentService.UploadDocumentAsync(
                empNr,
                documentType,
                file.FileName,
                fileContent);

            var response = new FileUploadResponse
            {
                Success = true,
                FileName = file.FileName,
                FilePath = $"/uploads/documents/{empNr}_{file.FileName}",
                FileSize = file.Length,
                UploadedAt = DateTime.UtcNow
            };

            _logger.LogInformation("Document uploaded successfully for student {EmpNr}: {FileName}",
                empNr, file.FileName);

            return Ok(CreateSuccessResponse(response, "Document uploaded successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while uploading document for student {EmpNr}: {Message}",
                empNr, ex.Message);
            return BadRequest(CreateErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading document for student {EmpNr}", empNr);
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while uploading the document"));
        }
    }

    /// <summary>
    /// Uploads a profile photo for a student.
    /// </summary>
    /// <param name="empNr">The student's employee number.</param>
    /// <param name="file">The photo file to upload.</param>
    /// <returns>File upload result.</returns>
    [HttpPost("{empNr:int}/photo")]
    [ProducesResponseType(typeof(ApiResponse<FileUploadResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<FileUploadResponse>>> UploadPhoto(
        int empNr,
        [Required] IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(CreateErrorResponse("No file provided"));
            }

            var existingStudent = await _studentService.GetStudentByEmpNrAsync(empNr.ToString());
            if (existingStudent == null)
            {
                return NotFound(CreateErrorResponse($"Student with employee number {empNr} not found"));
            }

            // Validate image file
            const long maxFileSize = 5 * 1024 * 1024; // 5MB
            if (file.Length > maxFileSize)
            {
                return BadRequest(CreateErrorResponse("File size exceeds the maximum allowed size of 5MB"));
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest(CreateErrorResponse("File type not allowed. Allowed types: JPG, JPEG, PNG"));
            }

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var fileContent = memoryStream.ToArray();

            var uploadResult = await _studentService.UploadPhotoAsync(empNr, file.FileName, fileContent);

            var response = new FileUploadResponse
            {
                Success = true,
                FileName = file.FileName,
                FilePath = $"/uploads/photos/{empNr}_{file.FileName}",
                FileSize = file.Length,
                UploadedAt = DateTime.UtcNow
            };

            _logger.LogInformation("Photo uploaded successfully for student {EmpNr}: {FileName}",
                empNr, file.FileName);

            return Ok(CreateSuccessResponse(response, "Photo uploaded successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while uploading photo for student {EmpNr}: {Message}",
                empNr, ex.Message);
            return BadRequest(CreateErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading photo for student {EmpNr}", empNr);
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while uploading the photo"));
        }
    }
}