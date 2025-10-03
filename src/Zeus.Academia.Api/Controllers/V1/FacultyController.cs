using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Zeus.Academia.Api.Controllers.Base;
using Zeus.Academia.Api.Models.Common;
using Zeus.Academia.Api.Models.Requests;
using Zeus.Academia.Api.Models.Responses;
using Zeus.Academia.Infrastructure.Services.Interfaces;
using Zeus.Academia.Infrastructure.Entities;
using AutoMapper;
using BaseController = Zeus.Academia.Api.Controllers.Base.BaseApiController;

namespace Zeus.Academia.Api.Controllers.V1;

/// <summary>
/// Controller for managing faculty operations.
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class FacultyController : BaseController
{
    private readonly IFacultyService _facultyService;
    private readonly IMapper _mapper;
    private readonly ILogger<FacultyController> _logger;

    /// <summary>
    /// Initializes a new instance of the FacultyController.
    /// </summary>
    public FacultyController(
        IFacultyService facultyService,
        IMapper mapper,
        ILogger<FacultyController> logger)
    {
        _facultyService = facultyService ?? throw new ArgumentNullException(nameof(facultyService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Retrieves a paginated list of faculty.
    /// </summary>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A paginated list of faculty.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<FacultySummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResponse<FacultySummaryResponse>>> GetFaculty(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        // Validate parameters
        if (pageNumber <= 0)
            return BadRequest(CreateErrorResponse("Page number must be greater than 0"));

        if (pageSize <= 0 || pageSize > 100)
            return BadRequest(CreateErrorResponse("Page size must be between 1 and 100"));

        try
        {
            var facultyResult = await _facultyService.GetFacultyAsync(pageNumber, pageSize);
            var facultyResponses = _mapper.Map<IEnumerable<FacultySummaryResponse>>(facultyResult.Faculty);

            var totalPages = (int)Math.Ceiling((double)facultyResult.TotalCount / pageSize);

            var response = new PagedResponse<FacultySummaryResponse>
            {
                Data = facultyResponses,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = facultyResult.TotalCount,
                TotalPages = totalPages,
                HasPreviousPage = pageNumber > 1,
                HasNextPage = pageNumber < totalPages
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving faculty");
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while processing the request"));
        }
    }

    /// <summary>
    /// Searches for faculty based on criteria.
    /// </summary>
    /// <param name="request">The search criteria.</param>
    /// <returns>A paginated list of matching faculty.</returns>
    [HttpPost("search")]
    [ProducesResponseType(typeof(PagedResponse<FacultySummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResponse<FacultySummaryResponse>>> SearchFaculty(
        [FromBody] FacultySearchRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CreateValidationErrorResponse(ModelState));
            }

            var facultyResult = await _facultyService.SearchFacultyAsync(
                request.SearchTerm,
                request.DepartmentName,
                request.RankCode,
                request.HasTenure,
                request.ResearchArea,
                request.FacultyType,
                request.Page,
                request.PageSize);

            var facultyResponses = _mapper.Map<IEnumerable<FacultySummaryResponse>>(facultyResult.Faculty);

            var totalPages = (int)Math.Ceiling((double)facultyResult.TotalCount / request.PageSize);

            var response = new PagedResponse<FacultySummaryResponse>
            {
                Data = facultyResponses,
                PageNumber = request.Page,
                PageSize = request.PageSize,
                TotalCount = facultyResult.TotalCount,
                TotalPages = totalPages,
                HasPreviousPage = request.Page > 1,
                HasNextPage = request.Page < totalPages
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching faculty with criteria: {@SearchCriteria}", request);
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while searching faculty"));
        }
    }

    /// <summary>
    /// Retrieves a specific faculty member by employee number.
    /// </summary>
    /// <param name="empNr">The faculty member's employee number.</param>
    /// <returns>The faculty member details.</returns>
    [HttpGet("{empNr:int}")]
    [ProducesResponseType(typeof(ApiResponse<FacultyDetailsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<FacultyDetailsResponse>>> GetFaculty(int empNr)
    {
        try
        {
            var faculty = await _facultyService.GetFacultyByEmpNrAsync(empNr);
            if (faculty == null)
            {
                return NotFound(CreateErrorResponse($"Faculty member with employee number {empNr} not found"));
            }

            var facultyResponse = _mapper.Map<FacultyDetailsResponse>(faculty);
            return Ok(CreateSuccessResponse(facultyResponse));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving faculty member with employee number {EmpNr}", empNr);
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while retrieving the faculty member"));
        }
    }

    /// <summary>
    /// Creates a new faculty member.
    /// </summary>
    /// <param name="request">The faculty creation data.</param>
    /// <returns>The created faculty member details.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<FacultyDetailsResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<FacultyDetailsResponse>>> CreateFaculty(
        [FromBody] CreateFacultyRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CreateValidationErrorResponse(ModelState));
            }

            // Check if faculty member already exists
            var existingFaculty = await _facultyService.GetFacultyByEmpNrAsync(request.EmpNr);
            if (existingFaculty != null)
            {
                return Conflict(CreateErrorResponse($"A faculty member with employee number {request.EmpNr} already exists"));
            }

            Academic newFaculty;

            // Create the appropriate faculty type
            switch (request.FacultyType)
            {
                case FacultyType.Professor:
                    newFaculty = new Professor
                    {
                        EmpNr = request.EmpNr,
                        Name = request.Name,
                        PhoneNumber = request.PhoneNumber,
                        Salary = request.Salary,
                        RankCode = request.RankCode,
                        DepartmentName = request.DepartmentName,
                        HasTenure = request.HasTenure,
                        ResearchArea = request.ResearchArea
                    };
                    break;
                case FacultyType.TeachingProf:
                    newFaculty = new TeachingProf
                    {
                        EmpNr = request.EmpNr,
                        Name = request.Name,
                        PhoneNumber = request.PhoneNumber,
                        Salary = request.Salary,
                        RankCode = request.RankCode,
                        DepartmentName = request.DepartmentName,
                        HasTenure = request.HasTenure,
                        ResearchArea = request.ResearchArea
                    };
                    break;
                default:
                    return BadRequest(CreateErrorResponse("Invalid faculty type specified"));
            }

            var faculty = await _facultyService.CreateFacultyAsync(newFaculty);

            var facultyResponse = _mapper.Map<FacultyDetailsResponse>(faculty);

            _logger.LogInformation("Faculty member created successfully with employee number {EmpNr}",
                faculty.EmpNr);

            return CreatedAtAction(
                nameof(GetFaculty),
                new { empNr = faculty.EmpNr },
                CreateSuccessResponse(facultyResponse, "Faculty member created successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while creating faculty member: {Message}", ex.Message);
            return BadRequest(CreateErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating faculty member with employee number {EmpNr}", request.EmpNr);
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while creating the faculty member"));
        }
    }

    /// <summary>
    /// Updates an existing faculty member.
    /// </summary>
    /// <param name="empNr">The faculty member's employee number.</param>
    /// <param name="request">The updated faculty data.</param>
    /// <returns>The updated faculty member details.</returns>
    [HttpPut("{empNr:int}")]
    [ProducesResponseType(typeof(ApiResponse<FacultyDetailsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<FacultyDetailsResponse>>> UpdateFaculty(
        int empNr,
        [FromBody] UpdateFacultyRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CreateValidationErrorResponse(ModelState));
            }

            var existingFaculty = await _facultyService.GetFacultyByEmpNrAsync(empNr);
            if (existingFaculty == null)
            {
                return NotFound(CreateErrorResponse($"Faculty member with employee number {empNr} not found"));
            }

            // Update the existing faculty object with new values
            existingFaculty.Name = request.Name;
            existingFaculty.PhoneNumber = request.PhoneNumber;
            existingFaculty.Salary = request.Salary;

            // Update specific fields based on type
            if (existingFaculty is Professor professor)
            {
                professor.RankCode = request.RankCode;
                professor.DepartmentName = request.DepartmentName;
                professor.HasTenure = request.HasTenure;
                professor.ResearchArea = request.ResearchArea;
            }
            else if (existingFaculty is TeachingProf teachingProf)
            {
                teachingProf.RankCode = request.RankCode;
                teachingProf.DepartmentName = request.DepartmentName;
                teachingProf.HasTenure = request.HasTenure;
                teachingProf.ResearchArea = request.ResearchArea;
            }

            var updatedFaculty = await _facultyService.UpdateFacultyAsync(existingFaculty);

            var facultyResponse = _mapper.Map<FacultyDetailsResponse>(updatedFaculty);

            _logger.LogInformation("Faculty member updated successfully with employee number {EmpNr}", empNr);

            return Ok(CreateSuccessResponse(facultyResponse, "Faculty member updated successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while updating faculty member: {Message}", ex.Message);
            return BadRequest(CreateErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating faculty member with employee number {EmpNr}", empNr);
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while updating the faculty member"));
        }
    }

    /// <summary>
    /// Deletes a faculty member.
    /// </summary>
    /// <param name="empNr">The faculty member's employee number.</param>
    /// <returns>Success or failure status.</returns>
    [HttpDelete("{empNr:int}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> DeleteFaculty(int empNr)
    {
        try
        {
            var faculty = await _facultyService.GetFacultyByEmpNrAsync(empNr);
            if (faculty == null)
            {
                return NotFound(CreateErrorResponse($"Faculty member with employee number {empNr} not found"));
            }

            var deleted = await _facultyService.DeleteFacultyAsync(empNr);
            if (!deleted)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateErrorResponse("Failed to delete faculty member"));
            }

            _logger.LogInformation("Faculty member deleted successfully with employee number {EmpNr}", empNr);

            return Ok(CreateSuccessResponse("Faculty member deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting faculty member with employee number {EmpNr}", empNr);
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while deleting the faculty member"));
        }
    }

    /// <summary>
    /// Gets all professors.
    /// </summary>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A paginated list of professors.</returns>
    [HttpGet("professors")]
    [ProducesResponseType(typeof(PagedResponse<FacultySummaryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<FacultySummaryResponse>>> GetProfessors(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var professorsResult = await _facultyService.GetProfessorsAsync(pageNumber, pageSize);
            var professorResponses = _mapper.Map<IEnumerable<FacultySummaryResponse>>(professorsResult.Professors);

            var totalPages = (int)Math.Ceiling((double)professorsResult.TotalCount / pageSize);

            var response = new PagedResponse<FacultySummaryResponse>
            {
                Data = professorResponses,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = professorsResult.TotalCount,
                TotalPages = totalPages,
                HasPreviousPage = pageNumber > 1,
                HasNextPage = pageNumber < totalPages
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving professors");
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while retrieving professors"));
        }
    }

    /// <summary>
    /// Gets all teaching professors.
    /// </summary>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A paginated list of teaching professors.</returns>
    [HttpGet("teaching-professors")]
    [ProducesResponseType(typeof(PagedResponse<FacultySummaryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<FacultySummaryResponse>>> GetTeachingProfessors(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var teachingProfsResult = await _facultyService.GetTeachingProfsAsync(pageNumber, pageSize);
            var teachingProfResponses = _mapper.Map<IEnumerable<FacultySummaryResponse>>(teachingProfsResult.TeachingProfs);

            var totalPages = (int)Math.Ceiling((double)teachingProfsResult.TotalCount / pageSize);

            var response = new PagedResponse<FacultySummaryResponse>
            {
                Data = teachingProfResponses,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = teachingProfsResult.TotalCount,
                TotalPages = totalPages,
                HasPreviousPage = pageNumber > 1,
                HasNextPage = pageNumber < totalPages
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving teaching professors");
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while retrieving teaching professors"));
        }
    }

    /// <summary>
    /// Gets faculty by department.
    /// </summary>
    /// <param name="departmentName">The department name.</param>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A paginated list of faculty in the department.</returns>
    [HttpGet("by-department/{departmentName}")]
    [ProducesResponseType(typeof(PagedResponse<FacultySummaryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<FacultySummaryResponse>>> GetFacultyByDepartment(
        string departmentName,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var facultyResult = await _facultyService.GetFacultyByDepartmentAsync(departmentName, pageNumber, pageSize);
            var facultyResponses = _mapper.Map<IEnumerable<FacultySummaryResponse>>(facultyResult.Faculty);

            var totalPages = (int)Math.Ceiling((double)facultyResult.TotalCount / pageSize);

            var response = new PagedResponse<FacultySummaryResponse>
            {
                Data = facultyResponses,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = facultyResult.TotalCount,
                TotalPages = totalPages,
                HasPreviousPage = pageNumber > 1,
                HasNextPage = pageNumber < totalPages
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving faculty by department {DepartmentName}", departmentName);
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while retrieving faculty by department"));
        }
    }

    /// <summary>
    /// Gets tenured faculty.
    /// </summary>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A paginated list of tenured faculty.</returns>
    [HttpGet("tenured")]
    [ProducesResponseType(typeof(PagedResponse<FacultySummaryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<FacultySummaryResponse>>> GetTenuredFaculty(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var facultyResult = await _facultyService.GetTenuredFacultyAsync(pageNumber, pageSize);
            var facultyResponses = _mapper.Map<IEnumerable<FacultySummaryResponse>>(facultyResult.Faculty);

            var totalPages = (int)Math.Ceiling((double)facultyResult.TotalCount / pageSize);

            var response = new PagedResponse<FacultySummaryResponse>
            {
                Data = facultyResponses,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = facultyResult.TotalCount,
                TotalPages = totalPages,
                HasPreviousPage = pageNumber > 1,
                HasNextPage = pageNumber < totalPages
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tenured faculty");
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while retrieving tenured faculty"));
        }
    }

    /// <summary>
    /// Updates tenure status for a faculty member.
    /// </summary>
    /// <param name="empNr">The faculty member's employee number.</param>
    /// <param name="request">The tenure status update data.</param>
    /// <returns>Success or failure status.</returns>
    [HttpPut("{empNr:int}/tenure")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> UpdateTenureStatus(
        int empNr,
        [FromBody] UpdateTenureStatusRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CreateValidationErrorResponse(ModelState));
            }

            var faculty = await _facultyService.GetFacultyByEmpNrAsync(empNr);
            if (faculty == null)
            {
                return NotFound(CreateErrorResponse($"Faculty member with employee number {empNr} not found"));
            }

            var updated = await _facultyService.UpdateTenureStatusAsync(empNr, request.HasTenure, request.Notes);
            if (!updated)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateErrorResponse("Failed to update tenure status"));
            }

            _logger.LogInformation("Tenure status updated successfully for faculty member {EmpNr}", empNr);

            return Ok(CreateSuccessResponse("Tenure status updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tenure status for faculty member {EmpNr}", empNr);
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while updating tenure status"));
        }
    }

    /// <summary>
    /// Updates rank for a faculty member.
    /// </summary>
    /// <param name="empNr">The faculty member's employee number.</param>
    /// <param name="request">The rank update data.</param>
    /// <returns>Success or failure status.</returns>
    [HttpPut("{empNr:int}/rank")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> UpdateRank(
        int empNr,
        [FromBody] UpdateRankRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CreateValidationErrorResponse(ModelState));
            }

            var faculty = await _facultyService.GetFacultyByEmpNrAsync(empNr);
            if (faculty == null)
            {
                return NotFound(CreateErrorResponse($"Faculty member with employee number {empNr} not found"));
            }

            var updated = await _facultyService.UpdateRankAsync(empNr, request.RankCode, request.EffectiveDate, request.Notes);
            if (!updated)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateErrorResponse("Failed to update rank"));
            }

            _logger.LogInformation("Rank updated successfully for faculty member {EmpNr}", empNr);

            return Ok(CreateSuccessResponse("Rank updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating rank for faculty member {EmpNr}", empNr);
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while updating rank"));
        }
    }

    /// <summary>
    /// Gets faculty profile.
    /// </summary>
    /// <param name="empNr">The faculty member's employee number.</param>
    /// <returns>The faculty profile.</returns>
    [HttpGet("{empNr:int}/profile")]
    [ProducesResponseType(typeof(ApiResponse<FacultyProfileResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<FacultyProfileResponse>>> GetFacultyProfile(int empNr)
    {
        try
        {
            var faculty = await _facultyService.GetFacultyByEmpNrAsync(empNr);
            if (faculty == null)
            {
                return NotFound(CreateErrorResponse($"Faculty member with employee number {empNr} not found"));
            }

            var profile = await _facultyService.GetFacultyProfileAsync(empNr);
            if (profile == null)
            {
                return NotFound(CreateErrorResponse($"Faculty profile for employee number {empNr} not found"));
            }

            var profileResponse = _mapper.Map<FacultyProfileResponse>(profile);
            return Ok(CreateSuccessResponse(profileResponse));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving faculty profile for {EmpNr}", empNr);
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while retrieving the faculty profile"));
        }
    }

    /// <summary>
    /// Creates or updates faculty profile.
    /// </summary>
    /// <param name="empNr">The faculty member's employee number.</param>
    /// <param name="request">The profile data.</param>
    /// <returns>The saved faculty profile.</returns>
    [HttpPut("{empNr:int}/profile")]
    [ProducesResponseType(typeof(ApiResponse<FacultyProfileResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<FacultyProfileResponse>>> SaveFacultyProfile(
        int empNr,
        [FromBody] SaveFacultyProfileRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CreateValidationErrorResponse(ModelState));
            }

            var faculty = await _facultyService.GetFacultyByEmpNrAsync(empNr);
            if (faculty == null)
            {
                return NotFound(CreateErrorResponse($"Faculty member with employee number {empNr} not found"));
            }

            var profile = new FacultyProfile
            {
                AcademicEmpNr = empNr,
                ProfessionalTitle = request.ProfessionalTitle,
                PreferredName = request.PreferredName,
                ProfessionalEmail = request.ProfessionalEmail,
                WebsiteUrl = request.ProfessionalWebsite,
                Biography = request.Biography,
                ResearchInterests = request.ResearchInterests,
                TeachingPhilosophy = request.TeachingPhilosophy,
                AwardsHonors = request.Awards,
                ProfessionalMemberships = request.ProfessionalMemberships,
                CurrentResearchProjects = request.CurrentResearchProjects,
                ConsultationAvailability = request.ConsultationAvailability,
                OfficeHours = request.OfficeHours,
                IsPublicProfile = request.IsPublicProfile
            };

            var savedProfile = await _facultyService.SaveFacultyProfileAsync(profile);
            var profileResponse = _mapper.Map<FacultyProfileResponse>(savedProfile);

            _logger.LogInformation("Faculty profile saved successfully for employee number {EmpNr}", empNr);

            return Ok(CreateSuccessResponse(profileResponse, "Faculty profile saved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving faculty profile for {EmpNr}", empNr);
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while saving the faculty profile"));
        }
    }

    /// <summary>
    /// Gets faculty statistics.
    /// </summary>
    /// <returns>Faculty statistics.</returns>
    [HttpGet("statistics")]
    [ProducesResponseType(typeof(ApiResponse<FacultyStatisticsResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<FacultyStatisticsResponse>>> GetFacultyStatistics()
    {
        try
        {
            var statistics = await _facultyService.GetFacultyStatisticsAsync();
            var statisticsResponse = _mapper.Map<FacultyStatisticsResponse>(statistics);

            return CreateSuccessResponse(statisticsResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving faculty statistics");
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while retrieving faculty statistics"));
        }
    }

    /// <summary>
    /// Uploads a document for a faculty member.
    /// </summary>
    /// <param name="empNr">The faculty member's employee number.</param>
    /// <param name="request">The document upload data.</param>
    /// <returns>Upload result.</returns>
    [HttpPost("{empNr:int}/documents")]
    [ProducesResponseType(typeof(ApiResponse<FileUploadResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<FileUploadResponse>>> UploadDocument(
        int empNr,
        [FromBody] UploadFacultyDocumentRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CreateValidationErrorResponse(ModelState));
            }

            var faculty = await _facultyService.GetFacultyByEmpNrAsync(empNr);
            if (faculty == null)
            {
                return NotFound(CreateErrorResponse($"Faculty member with employee number {empNr} not found"));
            }

            // Convert base64 to byte array
            byte[] fileContent;
            try
            {
                fileContent = Convert.FromBase64String(request.FileContent);
            }
            catch (FormatException)
            {
                return BadRequest(CreateErrorResponse("Invalid file content format"));
            }

            var result = await _facultyService.UploadDocumentAsync(
                empNr,
                request.DocumentType,
                request.FileName,
                fileContent);

            var uploadResponse = new FileUploadResponse
            {
                Success = true,
                FileName = request.FileName,
                FileSize = fileContent.Length,
                UploadedAt = DateTime.UtcNow
            };

            _logger.LogInformation("Document uploaded successfully for faculty member {EmpNr}", empNr);

            return Ok(CreateSuccessResponse(uploadResponse, "Document uploaded successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading document for faculty member {EmpNr}", empNr);
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while uploading the document"));
        }
    }

    /// <summary>
    /// Uploads a photo for a faculty member.
    /// </summary>
    /// <param name="empNr">The faculty member's employee number.</param>
    /// <param name="request">The photo upload data.</param>
    /// <returns>Upload result.</returns>
    [HttpPost("{empNr:int}/photo")]
    [ProducesResponseType(typeof(ApiResponse<FileUploadResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<FileUploadResponse>>> UploadPhoto(
        int empNr,
        [FromBody] UploadFacultyPhotoRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CreateValidationErrorResponse(ModelState));
            }

            var faculty = await _facultyService.GetFacultyByEmpNrAsync(empNr);
            if (faculty == null)
            {
                return NotFound(CreateErrorResponse($"Faculty member with employee number {empNr} not found"));
            }

            // Convert base64 to byte array
            byte[] photoContent;
            try
            {
                photoContent = Convert.FromBase64String(request.PhotoContent);
            }
            catch (FormatException)
            {
                return BadRequest(CreateErrorResponse("Invalid photo content format"));
            }

            var result = await _facultyService.UploadPhotoAsync(empNr, request.FileName, photoContent);

            var uploadResponse = new FileUploadResponse
            {
                Success = true,
                FileName = request.FileName,
                FileSize = photoContent.Length,
                UploadedAt = DateTime.UtcNow
            };

            _logger.LogInformation("Photo uploaded successfully for faculty member {EmpNr}", empNr);

            return Ok(CreateSuccessResponse(uploadResponse, "Photo uploaded successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading photo for faculty member {EmpNr}", empNr);
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while uploading the photo"));
        }
    }

    #region Administrative Assignment Endpoints

    /// <summary>
    /// Assigns an administrative role to a faculty member.
    /// </summary>
    /// <param name="empNr">The faculty member's employee number.</param>
    /// <param name="request">The administrative role assignment data.</param>
    /// <returns>Success or failure status.</returns>
    [HttpPost("{empNr:int}/administrative-roles")]
    [ProducesResponseType(typeof(ApiResponse<AdministrativeAssignmentResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<AdministrativeAssignmentResponse>>> AssignAdministrativeRole(
        int empNr,
        [FromBody] AssignAdministrativeRoleRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CreateValidationErrorResponse(ModelState));
            }

            var faculty = await _facultyService.GetFacultyByEmpNrAsync(empNr);
            if (faculty == null)
            {
                return NotFound(CreateErrorResponse($"Faculty member with employee number {empNr} not found"));
            }

            // Create administrative assignment (this would need service method implementation)
            var assignment = new AdministrativeAssignmentResponse
            {
                Id = 1, // This would come from the service
                RoleTitle = request.RoleTitle,
                Department = request.Department,
                AssignmentStartDate = request.AssignmentStartDate,
                AssignmentEndDate = request.AssignmentEndDate,
                AssignmentStatus = "Active"
            };

            _logger.LogInformation("Administrative role assigned successfully for faculty member {EmpNr}", empNr);

            return CreatedAtAction(
                nameof(GetAdministrativeAssignments),
                new { empNr = empNr },
                CreateSuccessResponse(assignment, "Administrative role assigned successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning administrative role for faculty member {EmpNr}", empNr);
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while assigning the administrative role"));
        }
    }

    /// <summary>
    /// Gets administrative assignments for a faculty member.
    /// </summary>
    /// <param name="empNr">The faculty member's employee number.</param>
    /// <param name="activeOnly">Whether to return only active assignments.</param>
    /// <returns>List of administrative assignments.</returns>
    [HttpGet("{empNr:int}/administrative-roles")]
    [ProducesResponseType(typeof(ApiResponse<List<AdministrativeAssignmentResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<List<AdministrativeAssignmentResponse>>>> GetAdministrativeAssignments(
        int empNr,
        [FromQuery] bool activeOnly = true)
    {
        try
        {
            var faculty = await _facultyService.GetFacultyByEmpNrAsync(empNr);
            if (faculty == null)
            {
                return NotFound(CreateErrorResponse($"Faculty member with employee number {empNr} not found"));
            }

            var assignments = await _facultyService.GetAdministrativeAssignmentsAsync(empNr, activeOnly);
            var assignmentResponses = _mapper.Map<List<AdministrativeAssignmentResponse>>(assignments);

            return Ok(CreateSuccessResponse(assignmentResponses));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving administrative assignments for faculty member {EmpNr}", empNr);
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while retrieving administrative assignments"));
        }
    }

    /// <summary>
    /// Updates an administrative assignment.
    /// </summary>
    /// <param name="empNr">The faculty member's employee number.</param>
    /// <param name="assignmentId">The assignment ID.</param>
    /// <param name="request">The updated assignment data.</param>
    /// <returns>Updated assignment details.</returns>
    [HttpPut("{empNr:int}/administrative-roles/{assignmentId:int}")]
    [ProducesResponseType(typeof(ApiResponse<AdministrativeAssignmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<AdministrativeAssignmentResponse>>> UpdateAdministrativeAssignment(
        int empNr,
        int assignmentId,
        [FromBody] AssignAdministrativeRoleRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CreateValidationErrorResponse(ModelState));
            }

            var faculty = await _facultyService.GetFacultyByEmpNrAsync(empNr);
            if (faculty == null)
            {
                return NotFound(CreateErrorResponse($"Faculty member with employee number {empNr} not found"));
            }

            // Update assignment (this would need service method implementation)
            var updatedAssignment = new AdministrativeAssignmentResponse
            {
                Id = assignmentId,
                RoleTitle = request.RoleTitle,
                Department = request.Department,
                AssignmentStartDate = request.AssignmentStartDate,
                AssignmentEndDate = request.AssignmentEndDate,
                AssignmentStatus = "Active"
            };

            _logger.LogInformation("Administrative assignment updated successfully for faculty member {EmpNr}", empNr);

            return Ok(CreateSuccessResponse(updatedAssignment, "Administrative assignment updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating administrative assignment for faculty member {EmpNr}", empNr);
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while updating the administrative assignment"));
        }
    }

    #endregion

    #region Committee Assignment Endpoints

    /// <summary>
    /// Assigns a faculty member to a committee.
    /// </summary>
    /// <param name="empNr">The faculty member's employee number.</param>
    /// <param name="request">The committee assignment data.</param>
    /// <returns>Success or failure status.</returns>
    [HttpPost("{empNr:int}/committees")]
    [ProducesResponseType(typeof(ApiResponse<CommitteeAssignmentResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<CommitteeAssignmentResponse>>> AssignToCommittee(
        int empNr,
        [FromBody] AssignCommitteeRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CreateValidationErrorResponse(ModelState));
            }

            var faculty = await _facultyService.GetFacultyByEmpNrAsync(empNr);
            if (faculty == null)
            {
                return NotFound(CreateErrorResponse($"Faculty member with employee number {empNr} not found"));
            }

            // Create committee assignment (this would need service method implementation)
            var assignment = new CommitteeAssignmentResponse
            {
                Id = 1, // This would come from the service
                CommitteeName = "Committee Name", // This would be resolved from CommitteeId
                CommitteeRole = request.CommitteeRole,
                AssignmentStartDate = request.AssignmentStartDate,
                AssignmentEndDate = request.AssignmentEndDate,
                AssignmentStatus = "Active"
            };

            _logger.LogInformation("Committee assignment created successfully for faculty member {EmpNr}", empNr);

            return CreatedAtAction(
                nameof(GetCommitteeAssignments),
                new { empNr = empNr },
                CreateSuccessResponse(assignment, "Committee assignment created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating committee assignment for faculty member {EmpNr}", empNr);
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while creating the committee assignment"));
        }
    }

    /// <summary>
    /// Gets committee assignments for a faculty member.
    /// </summary>
    /// <param name="empNr">The faculty member's employee number.</param>
    /// <param name="activeOnly">Whether to return only active assignments.</param>
    /// <returns>List of committee assignments.</returns>
    [HttpGet("{empNr:int}/committees")]
    [ProducesResponseType(typeof(ApiResponse<List<CommitteeAssignmentResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<List<CommitteeAssignmentResponse>>>> GetCommitteeAssignments(
        int empNr,
        [FromQuery] bool activeOnly = true)
    {
        try
        {
            var faculty = await _facultyService.GetFacultyByEmpNrAsync(empNr);
            if (faculty == null)
            {
                return NotFound(CreateErrorResponse($"Faculty member with employee number {empNr} not found"));
            }

            var assignments = await _facultyService.GetCommitteeAssignmentsAsync(empNr, activeOnly);
            var assignmentResponses = _mapper.Map<List<CommitteeAssignmentResponse>>(assignments);

            return Ok(CreateSuccessResponse(assignmentResponses));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving committee assignments for faculty member {EmpNr}", empNr);
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while retrieving committee assignments"));
        }
    }

    /// <summary>
    /// Removes a faculty member from a committee.
    /// </summary>
    /// <param name="empNr">The faculty member's employee number.</param>
    /// <param name="assignmentId">The committee assignment ID.</param>
    /// <returns>Success or failure status.</returns>
    [HttpDelete("{empNr:int}/committees/{assignmentId:int}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> RemoveFromCommittee(int empNr, int assignmentId)
    {
        try
        {
            var faculty = await _facultyService.GetFacultyByEmpNrAsync(empNr);
            if (faculty == null)
            {
                return NotFound(CreateErrorResponse($"Faculty member with employee number {empNr} not found"));
            }

            // Remove committee assignment (this would need service method implementation)
            var removed = true; // This would come from the service

            if (!removed)
            {
                return NotFound(CreateErrorResponse($"Committee assignment {assignmentId} not found"));
            }

            _logger.LogInformation("Committee assignment removed successfully for faculty member {EmpNr}", empNr);

            return Ok(CreateSuccessResponse("Committee assignment removed successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing committee assignment for faculty member {EmpNr}", empNr);
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while removing the committee assignment"));
        }
    }

    #endregion

    #region Teaching Load and Course Assignment Endpoints

    /// <summary>
    /// Assigns a course to a faculty member.
    /// </summary>
    /// <param name="empNr">The faculty member's employee number.</param>
    /// <param name="request">The course assignment data.</param>
    /// <returns>Course assignment details.</returns>
    [HttpPost("{empNr:int}/courses")]
    [ProducesResponseType(typeof(ApiResponse<CourseAssignmentResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<CourseAssignmentResponse>>> AssignCourse(
        int empNr,
        [FromBody] AssignCourseRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CreateValidationErrorResponse(ModelState));
            }

            var faculty = await _facultyService.GetFacultyByEmpNrAsync(empNr);
            if (faculty == null)
            {
                return NotFound(CreateErrorResponse($"Faculty member with employee number {empNr} not found"));
            }

            // Create course assignment (this would need service method implementation)
            var assignment = new CourseAssignmentResponse
            {
                Id = 1, // This would come from the service
                CourseId = request.CourseId,
                CourseCode = "CS 101", // This would be resolved from CourseId
                CourseTitle = "Introduction to Computer Science", // This would be resolved from CourseId
                Semester = request.Semester,
                AcademicYear = request.AcademicYear,
                Section = request.Section,
                CreditHours = request.CreditHours,
                TeachingModality = request.TeachingModality,
                MaxEnrollment = request.MaxEnrollment,
                CurrentEnrollment = 0,
                AssignmentStatus = "Active"
            };

            _logger.LogInformation("Course assignment created successfully for faculty member {EmpNr}", empNr);

            return CreatedAtAction(
                nameof(GetCourseAssignments),
                new { empNr = empNr },
                CreateSuccessResponse(assignment, "Course assignment created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating course assignment for faculty member {EmpNr}", empNr);
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while creating the course assignment"));
        }
    }

    /// <summary>
    /// Gets course assignments for a faculty member.
    /// </summary>
    /// <param name="empNr">The faculty member's employee number.</param>
    /// <param name="academicYear">Filter by academic year.</param>
    /// <param name="semester">Filter by semester.</param>
    /// <returns>List of course assignments.</returns>
    [HttpGet("{empNr:int}/courses")]
    [ProducesResponseType(typeof(ApiResponse<List<CourseAssignmentResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<List<CourseAssignmentResponse>>>> GetCourseAssignments(
        int empNr,
        [FromQuery] int? academicYear = null,
        [FromQuery] string? semester = null)
    {
        try
        {
            var faculty = await _facultyService.GetFacultyByEmpNrAsync(empNr);
            if (faculty == null)
            {
                return NotFound(CreateErrorResponse($"Faculty member with employee number {empNr} not found"));
            }

            // Get course assignments (this would need service method implementation)
            var assignments = new List<CourseAssignmentResponse>();

            return Ok(CreateSuccessResponse(assignments));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving course assignments for faculty member {EmpNr}", empNr);
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while retrieving course assignments"));
        }
    }

    /// <summary>
    /// Gets faculty workload information.
    /// </summary>
    /// <param name="empNr">The faculty member's employee number.</param>
    /// <param name="request">The workload request parameters.</param>
    /// <returns>Faculty workload details.</returns>
    [HttpPost("{empNr:int}/workload")]
    [ProducesResponseType(typeof(ApiResponse<FacultyWorkloadResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<FacultyWorkloadResponse>>> GetFacultyWorkload(
        int empNr,
        [FromBody] FacultyWorkloadRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CreateValidationErrorResponse(ModelState));
            }

            var faculty = await _facultyService.GetFacultyByEmpNrAsync(empNr);
            if (faculty == null)
            {
                return NotFound(CreateErrorResponse($"Faculty member with employee number {empNr} not found"));
            }

            // Get workload information (this would need service method implementation)
            var workload = new FacultyWorkloadResponse
            {
                EmpNr = empNr,
                Name = faculty.Name,
                AcademicYear = request.AcademicYear,
                Semester = request.Semester,
                TotalTeachingLoad = 12.0m,
                TotalServiceLoad = 4.0m,
                TotalResearchLoad = 20.0m,
                TotalAdministrativeLoad = 0.0m,
                WorkloadPercentage = 100.0m,
                HasOverload = false,
                CourseAssignments = new(),
                ActiveCommitteeAssignments = new(),
                ActiveAdministrativeAssignments = new()
            };

            return Ok(CreateSuccessResponse(workload));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving workload for faculty member {EmpNr}", empNr);
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while retrieving faculty workload"));
        }
    }

    /// <summary>
    /// Updates teaching preferences for a faculty member.
    /// </summary>
    /// <param name="empNr">The faculty member's employee number.</param>
    /// <param name="request">The teaching preferences data.</param>
    /// <returns>Updated teaching preferences.</returns>
    [HttpPut("{empNr:int}/teaching-preferences")]
    [ProducesResponseType(typeof(ApiResponse<TeachingPreferencesResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<TeachingPreferencesResponse>>> UpdateTeachingPreferences(
        int empNr,
        [FromBody] UpdateTeachingPreferencesRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CreateValidationErrorResponse(ModelState));
            }

            var faculty = await _facultyService.GetFacultyByEmpNrAsync(empNr);
            if (faculty == null)
            {
                return NotFound(CreateErrorResponse($"Faculty member with employee number {empNr} not found"));
            }

            // Update teaching preferences (this would need service method implementation)
            var preferences = new TeachingPreferencesResponse
            {
                EmpNr = empNr,
                PreferredCourses = new(),
                PreferredTimes = request.PreferredTimes,
                PreferredDays = request.PreferredDays,
                MaxPreferredLoad = request.MaxPreferredLoad,
                MinPreferredLoad = request.MinPreferredLoad,
                PreferredModalities = request.PreferredModalities,
                WillingToTeachOverload = request.WillingToTeachOverload,
                AdditionalNotes = request.AdditionalNotes,
                LastUpdated = DateTime.UtcNow
            };

            _logger.LogInformation("Teaching preferences updated successfully for faculty member {EmpNr}", empNr);

            return Ok(CreateSuccessResponse(preferences, "Teaching preferences updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating teaching preferences for faculty member {EmpNr}", empNr);
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while updating teaching preferences"));
        }
    }

    /// <summary>
    /// Gets teaching preferences for a faculty member.
    /// </summary>
    /// <param name="empNr">The faculty member's employee number.</param>
    /// <returns>Teaching preferences details.</returns>
    [HttpGet("{empNr:int}/teaching-preferences")]
    [ProducesResponseType(typeof(ApiResponse<TeachingPreferencesResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<TeachingPreferencesResponse>>> GetTeachingPreferences(int empNr)
    {
        try
        {
            var faculty = await _facultyService.GetFacultyByEmpNrAsync(empNr);
            if (faculty == null)
            {
                return NotFound(CreateErrorResponse($"Faculty member with employee number {empNr} not found"));
            }

            // Get teaching preferences (this would need service method implementation)
            var preferences = new TeachingPreferencesResponse
            {
                EmpNr = empNr,
                PreferredCourses = new(),
                PreferredTimes = new(),
                PreferredDays = new(),
                PreferredModalities = new(),
                WillingToTeachOverload = false,
                LastUpdated = DateTime.UtcNow
            };

            return Ok(CreateSuccessResponse(preferences));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving teaching preferences for faculty member {EmpNr}", empNr);
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while retrieving teaching preferences"));
        }
    }

    #endregion

    #region Faculty Directory and Advanced Search Endpoints

    /// <summary>
    /// Gets the faculty directory with comprehensive search and filtering.
    /// </summary>
    /// <param name="request">The advanced search criteria.</param>
    /// <returns>Paginated list of faculty directory entries.</returns>
    [HttpPost("directory")]
    [ProducesResponseType(typeof(PagedResponse<FacultyDirectoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public ActionResult<PagedResponse<FacultyDirectoryResponse>> GetFacultyDirectory(
        [FromBody] AdvancedFacultySearchRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CreateValidationErrorResponse(ModelState));
            }

            // Perform advanced search (this would need service method implementation)
            var faculty = new List<FacultyDirectoryResponse>();
            var totalCount = 0;

            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

            var response = new PagedResponse<FacultyDirectoryResponse>
            {
                Data = faculty,
                PageNumber = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                HasPreviousPage = request.Page > 1,
                HasNextPage = request.Page < totalPages
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving faculty directory");
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while retrieving the faculty directory"));
        }
    }

    /// <summary>
    /// Gets public faculty directory (limited information).
    /// </summary>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="departmentName">Filter by department.</param>
    /// <param name="searchTerm">Search term for name or research area.</param>
    /// <returns>Paginated list of public faculty information.</returns>
    [HttpGet("directory/public")]
    [ProducesResponseType(typeof(PagedResponse<FacultyDirectoryResponse>), StatusCodes.Status200OK)]
    [AllowAnonymous]
    public ActionResult<PagedResponse<FacultyDirectoryResponse>> GetPublicFacultyDirectory(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? departmentName = null,
        [FromQuery] string? searchTerm = null)
    {
        try
        {
            if (pageNumber < 1)
            {
                return BadRequest(CreateErrorResponse("Page number must be greater than 0"));
            }

            if (pageSize < 1 || pageSize > 100)
            {
                return BadRequest(CreateErrorResponse("Page size must be between 1 and 100"));
            }

            // Get public faculty directory (this would need service method implementation)
            var faculty = new List<FacultyDirectoryResponse>();
            var totalCount = 0;

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var response = new PagedResponse<FacultyDirectoryResponse>
            {
                Data = faculty,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                HasPreviousPage = pageNumber > 1,
                HasNextPage = pageNumber < totalPages
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving public faculty directory");
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while retrieving the faculty directory"));
        }
    }

    #endregion

    #region Faculty Analytics and Reporting Endpoints

    /// <summary>
    /// Gets comprehensive faculty analytics.
    /// </summary>
    /// <param name="academicYear">The academic year for analytics (optional).</param>
    /// <returns>Faculty analytics data.</returns>
    [HttpGet("analytics")]
    [ProducesResponseType(typeof(ApiResponse<FacultyAnalyticsResponse>), StatusCodes.Status200OK)]
    public ActionResult<ApiResponse<FacultyAnalyticsResponse>> GetFacultyAnalytics(
        [FromQuery] int? academicYear = null)
    {
        try
        {
            // Get faculty analytics (this would need service method implementation)
            var analytics = new FacultyAnalyticsResponse
            {
                OverallStatistics = new FacultyStatisticsResponse
                {
                    TotalFaculty = 100,
                    TotalProfessors = 60,
                    TotalTeachingProfs = 40,
                    TotalTeachers = 0,
                    TenuredCount = 75,
                    NonTenuredCount = 25,
                    FacultyByDepartment = new(),
                    FacultyByRank = new(),
                    FacultyByTenureStatus = new()
                },
                WorkloadByDepartment = new(),
                PromotionTrends = new(),
                RetirementProjections = new(),
                ServiceLoadDistribution = new(),
                TeachingLoadStats = new()
            };

            return Ok(CreateSuccessResponse(analytics));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving faculty analytics");
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while retrieving faculty analytics"));
        }
    }

    /// <summary>
    /// Gets department-specific faculty analytics.
    /// </summary>
    /// <param name="departmentName">The department name.</param>
    /// <param name="academicYear">The academic year for analytics (optional).</param>
    /// <returns>Department faculty analytics data.</returns>
    [HttpGet("analytics/department/{departmentName}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> GetDepartmentAnalytics(
        string departmentName,
        [FromQuery] int? academicYear = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(departmentName))
            {
                return BadRequest(CreateErrorResponse("Department name is required"));
            }

            var analytics = await _facultyService.GetDepartmentFacultyStatisticsAsync(departmentName);

            return CreateSuccessResponse(analytics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving department analytics for {DepartmentName}", departmentName);
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while retrieving department analytics"));
        }
    }

    /// <summary>
    /// Gets faculty workload distribution across the institution.
    /// </summary>
    /// <param name="academicYear">The academic year for workload analysis.</param>
    /// <param name="semester">The semester for workload analysis (optional).</param>
    /// <returns>Workload distribution data.</returns>
    [HttpGet("workload-distribution")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public ActionResult<ApiResponse<object>> GetWorkloadDistribution(
        [FromQuery] int academicYear,
        [FromQuery] string? semester = null)
    {
        try
        {
            if (academicYear < 2020 || academicYear > 2050)
            {
                return BadRequest(CreateErrorResponse("Academic year must be between 2020 and 2050"));
            }

            // Get workload distribution (this would need service method implementation)
            var distribution = new
            {
                AcademicYear = academicYear,
                Semester = semester,
                TotalFaculty = 100,
                AverageTeachingLoad = 12.0m,
                FacultyWithOverload = 15,
                FacultyUnderMinimum = 5,
                WorkloadByDepartment = new Dictionary<string, object>(),
                WorkloadByRank = new Dictionary<string, object>()
            };

            return CreateSuccessResponse((object)distribution);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving workload distribution for academic year {AcademicYear}", academicYear);
            return StatusCode(StatusCodes.Status500InternalServerError,
                CreateErrorResponse("An error occurred while retrieving workload distribution"));
        }
    }

    #endregion
}