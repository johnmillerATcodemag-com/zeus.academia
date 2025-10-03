using AutoMapper;
using Zeus.Academia.Api.Models.Requests;
using Zeus.Academia.Api.Models.Responses;
using Zeus.Academia.Infrastructure;
using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.Api.Mapping;

/// <summary>
/// AutoMapper profile for Student entity mappings.
/// </summary>
public class StudentMappingProfile : Profile
{
    /// <summary>
    /// Initializes the student mapping configuration.
    /// </summary>
    public StudentMappingProfile()
    {
        // Student to Response mappings
        CreateMap<Student, StudentDetailsResponse>()
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : null))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy ?? string.Empty))
            .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.ModifiedBy ?? string.Empty));

        CreateMap<Student, StudentSummaryResponse>()
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : null));

        // Application-related mappings
        CreateMap<EnrollmentApplication, EnrollmentApplicationResponse>()
            .ForMember(dest => dest.ApplicantName, opt => opt.MapFrom(src => src.Applicant != null ? src.Applicant.Name : string.Empty))
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : string.Empty));

        CreateMap<EnrollmentHistory, EnrollmentHistoryResponse>()
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : null));

        // Request to Entity mappings (if needed for direct mapping)
        CreateMap<CreateStudentRequest, Student>()
            .ForMember(dest => dest.EmpNr, opt => opt.Ignore())
            .ForMember(dest => dest.Department, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.EnrollmentStatus, opt => opt.MapFrom(src => Infrastructure.Enums.EnrollmentStatus.Pending))
            .ForMember(dest => dest.AcademicStanding, opt => opt.MapFrom(src => Infrastructure.Enums.AcademicStanding.Good));

        CreateMap<UpdateStudentRequest, Student>()
            .ForMember(dest => dest.EmpNr, opt => opt.Ignore())
            .ForMember(dest => dest.StudentId, opt => opt.Ignore())
            .ForMember(dest => dest.Department, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
            .ForMember(dest => dest.EnrollmentStatus, opt => opt.Ignore())
            .ForMember(dest => dest.AcademicStanding, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore());

        // Statistics mapping
        CreateMap<StudentStatistics, StudentStatisticsResponse>();
    }
}

/// <summary>
/// Represents student statistics.
/// </summary>
public class StudentStatistics
{
    public int TotalStudents { get; set; }
    public int ActiveStudents { get; set; }
    public int EnrolledStudents { get; set; }
    public int GraduatedStudents { get; set; }
    public Dictionary<string, int> StudentsByDepartment { get; set; } = new();
    public Dictionary<Infrastructure.Enums.EnrollmentStatus, int> StudentsByEnrollmentStatus { get; set; } = new();
    public Dictionary<Infrastructure.Enums.AcademicStanding, int> StudentsByAcademicStanding { get; set; } = new();
}

/// <summary>
/// Represents an enrollment application.
/// </summary>
public class EnrollmentApplication
{
    public int Id { get; set; }
    public int? ApplicantEmpNr { get; set; }
    public Student? Applicant { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Program { get; set; } = string.Empty;
    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }
    public DateTime ApplicationDate { get; set; }
    public Infrastructure.Enums.ApplicationStatus Status { get; set; }
    public Infrastructure.Enums.ApplicationPriority Priority { get; set; }
    public Infrastructure.Enums.AdmissionDecision? Decision { get; set; }
    public DateTime? DecisionDate { get; set; }
    public string? DecisionReason { get; set; }
    public string? DecisionMadeBy { get; set; }
    public string? ConditionalRequirements { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Represents an enrollment history entry.
/// </summary>
public class EnrollmentHistory
{
    public int Id { get; set; }
    public int StudentEmpNr { get; set; }
    public Infrastructure.Enums.EnrollmentEventType EventType { get; set; }
    public Infrastructure.Enums.EnrollmentStatus NewStatus { get; set; }
    public Infrastructure.Enums.EnrollmentStatus? PreviousStatus { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
    public string? ProcessedBy { get; set; }
    public DateTime EventDate { get; set; }
    public int? ApplicationId { get; set; }
    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }
    public string? Program { get; set; }
    public string? AcademicTerm { get; set; }
    public int? AcademicYear { get; set; }
}