using AutoMapper;
using Zeus.Academia.Api.Models.Requests;
using Zeus.Academia.Api.Models.Responses;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Services.Interfaces;

namespace Zeus.Academia.Api.Mapping;

/// <summary>
/// AutoMapper profile for Faculty entity mappings.
/// </summary>
public class FacultyMappingProfile : Profile
{
    /// <summary>
    /// Initializes the faculty mapping configuration.
    /// </summary>
    public FacultyMappingProfile()
    {
        // Faculty to Response mappings
        CreateMap<Academic, FacultyDetailsResponse>()
            .ForMember(dest => dest.FacultyType, opt => opt.MapFrom(src => GetFacultyType(src)))
            .ForMember(dest => dest.RankCode, opt => opt.MapFrom(src => GetRankCode(src)))
            .ForMember(dest => dest.RankName, opt => opt.MapFrom(src => GetRankName(src)))
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => GetDepartmentName(src)))
            .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => GetDepartmentId(src)))
            .ForMember(dest => dest.HasTenure, opt => opt.MapFrom(src => GetHasTenure(src)))
            .ForMember(dest => dest.ResearchArea, opt => opt.MapFrom(src => GetResearchArea(src)))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy ?? string.Empty))
            .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.ModifiedBy ?? string.Empty))
            .ForMember(dest => dest.Profile, opt => opt.Ignore())
            .ForMember(dest => dest.TenureTrack, opt => opt.Ignore())
            .ForMember(dest => dest.ResearchExpertise, opt => opt.Ignore())
            .ForMember(dest => dest.RecentPublications, opt => opt.Ignore())
            .ForMember(dest => dest.OfficeAssignments, opt => opt.Ignore())
            .ForMember(dest => dest.CommitteeAssignments, opt => opt.Ignore())
            .ForMember(dest => dest.AdministrativeAssignments, opt => opt.Ignore());

        CreateMap<Academic, FacultySummaryResponse>()
            .ForMember(dest => dest.FacultyType, opt => opt.MapFrom(src => GetFacultyType(src)))
            .ForMember(dest => dest.RankCode, opt => opt.MapFrom(src => GetRankCode(src)))
            .ForMember(dest => dest.RankName, opt => opt.MapFrom(src => GetRankName(src)))
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => GetDepartmentName(src)))
            .ForMember(dest => dest.HasTenure, opt => opt.MapFrom(src => GetHasTenure(src)))
            .ForMember(dest => dest.ResearchArea, opt => opt.MapFrom(src => GetResearchArea(src)))
            .ForMember(dest => dest.ProfessionalEmail, opt => opt.Ignore());

        CreateMap<Professor, FacultyDetailsResponse>()
            .IncludeBase<Academic, FacultyDetailsResponse>()
            .ForMember(dest => dest.FacultyType, opt => opt.MapFrom(src => FacultyType.Professor))
            .ForMember(dest => dest.RankCode, opt => opt.MapFrom(src => src.RankCode))
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.DepartmentName))
            .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.Department != null ? src.Department.Id : (int?)null))
            .ForMember(dest => dest.HasTenure, opt => opt.MapFrom(src => src.HasTenure))
            .ForMember(dest => dest.ResearchArea, opt => opt.MapFrom(src => src.ResearchArea));

        CreateMap<Professor, FacultySummaryResponse>()
            .IncludeBase<Academic, FacultySummaryResponse>()
            .ForMember(dest => dest.FacultyType, opt => opt.MapFrom(src => FacultyType.Professor))
            .ForMember(dest => dest.RankCode, opt => opt.MapFrom(src => src.RankCode))
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.DepartmentName))
            .ForMember(dest => dest.HasTenure, opt => opt.MapFrom(src => src.HasTenure))
            .ForMember(dest => dest.ResearchArea, opt => opt.MapFrom(src => src.ResearchArea));

        CreateMap<TeachingProf, FacultyDetailsResponse>()
            .IncludeBase<Academic, FacultyDetailsResponse>()
            .ForMember(dest => dest.FacultyType, opt => opt.MapFrom(src => FacultyType.TeachingProf))
            .ForMember(dest => dest.RankCode, opt => opt.MapFrom(src => src.RankCode))
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.DepartmentName))
            .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.Department != null ? src.Department.Id : (int?)null))
            .ForMember(dest => dest.HasTenure, opt => opt.MapFrom(src => src.HasTenure))
            .ForMember(dest => dest.ResearchArea, opt => opt.MapFrom(src => src.ResearchArea));

        CreateMap<TeachingProf, FacultySummaryResponse>()
            .IncludeBase<Academic, FacultySummaryResponse>()
            .ForMember(dest => dest.FacultyType, opt => opt.MapFrom(src => FacultyType.TeachingProf))
            .ForMember(dest => dest.RankCode, opt => opt.MapFrom(src => src.RankCode))
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.DepartmentName))
            .ForMember(dest => dest.HasTenure, opt => opt.MapFrom(src => src.HasTenure))
            .ForMember(dest => dest.ResearchArea, opt => opt.MapFrom(src => src.ResearchArea));

        // Faculty profile mappings
        CreateMap<FacultyProfile, FacultyProfileResponse>();

        // Tenure track mappings
        CreateMap<TenureTrack, TenureTrackResponse>();

        // Research expertise mappings
        CreateMap<FacultyExpertise, ResearchExpertiseResponse>()
            .ForMember(dest => dest.ResearchAreaName, opt => opt.MapFrom(src => src.ResearchArea != null ? src.ResearchArea.Name : string.Empty));

        // Publication mappings
        CreateMap<FacultyPublication, PublicationResponse>();

        // Office assignment mappings
        CreateMap<OfficeAssignment, OfficeAssignmentResponse>()
            .ForMember(dest => dest.OfficeLocation, opt => opt.MapFrom(src =>
                !string.IsNullOrEmpty(src.BuildingName) && !string.IsNullOrEmpty(src.RoomNumber)
                    ? $"{src.BuildingName} {src.RoomNumber}"
                    : src.BuildingName ?? src.RoomNumber));

        // Committee assignment mappings
        CreateMap<CommitteeMemberAssignment, CommitteeAssignmentResponse>()
            .ForMember(dest => dest.CommitteeName, opt => opt.Ignore())
            .ForMember(dest => dest.CommitteeRole, opt => opt.Ignore());

        // Administrative assignment mappings
        CreateMap<AdministrativeAssignment, AdministrativeAssignmentResponse>()
            .ForMember(dest => dest.Department, opt => opt.Ignore());

        // Promotion history mappings
        CreateMap<FacultyPromotion, PromotionHistoryResponse>()
            .ForMember(dest => dest.FromRankName, opt => opt.Ignore())
            .ForMember(dest => dest.ToRankName, opt => opt.Ignore());

        // Statistics mappings
        CreateMap<FacultyStatistics, FacultyStatisticsResponse>();

        // Request to Entity mappings
        CreateMap<CreateFacultyRequest, Professor>()
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Department, opt => opt.Ignore())
            .ForMember(dest => dest.Rank, opt => opt.Ignore())
            .ForMember(dest => dest.DepartmentId, opt => opt.Ignore());

        CreateMap<CreateFacultyRequest, TeachingProf>()
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Department, opt => opt.Ignore())
            .ForMember(dest => dest.Rank, opt => opt.Ignore())
            .ForMember(dest => dest.DepartmentId, opt => opt.Ignore());

        CreateMap<UpdateFacultyRequest, Professor>()
            .ForMember(dest => dest.EmpNr, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Department, opt => opt.Ignore())
            .ForMember(dest => dest.Rank, opt => opt.Ignore())
            .ForMember(dest => dest.DepartmentId, opt => opt.Ignore());

        CreateMap<UpdateFacultyRequest, TeachingProf>()
            .ForMember(dest => dest.EmpNr, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Department, opt => opt.Ignore())
            .ForMember(dest => dest.Rank, opt => opt.Ignore())
            .ForMember(dest => dest.DepartmentId, opt => opt.Ignore());

        CreateMap<SaveFacultyProfileRequest, FacultyProfile>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.AcademicEmpNr, opt => opt.Ignore())
            .ForMember(dest => dest.Academic, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Documents, opt => opt.Ignore())
            .ForMember(dest => dest.Publications, opt => opt.Ignore())
            .ForMember(dest => dest.OfficeAssignments, opt => opt.Ignore());
    }

    private static FacultyType GetFacultyType(Academic academic)
    {
        return academic switch
        {
            Professor => FacultyType.Professor,
            TeachingProf => FacultyType.TeachingProf,
            _ => FacultyType.Professor
        };
    }

    private static string? GetRankCode(Academic academic)
    {
        return academic switch
        {
            Professor professor => professor.RankCode,
            TeachingProf teachingProf => teachingProf.RankCode,
            _ => null
        };
    }

    private static string? GetRankName(Academic academic)
    {
        // This would typically involve looking up the rank name from the rank code
        // For now, return the rank code as a placeholder
        return GetRankCode(academic);
    }

    private static string? GetDepartmentName(Academic academic)
    {
        return academic switch
        {
            Professor professor => professor.DepartmentName,
            TeachingProf teachingProf => teachingProf.DepartmentName,
            _ => null
        };
    }

    private static int? GetDepartmentId(Academic academic)
    {
        return academic switch
        {
            Professor professor => professor.Department?.Id,
            TeachingProf teachingProf => teachingProf.Department?.Id,
            _ => null
        };
    }

    private static bool? GetHasTenure(Academic academic)
    {
        return academic switch
        {
            Professor professor => professor.HasTenure,
            TeachingProf teachingProf => teachingProf.HasTenure,
            _ => null
        };
    }

    private static string? GetResearchArea(Academic academic)
    {
        return academic switch
        {
            Professor professor => professor.ResearchArea,
            TeachingProf teachingProf => teachingProf.ResearchArea,
            _ => null
        };
    }
}

/// <summary>
/// Represents faculty statistics.
/// </summary>
public class FacultyStatistics
{
    public int TotalFaculty { get; set; }
    public int TotalProfessors { get; set; }
    public int TotalTeachingProfs { get; set; }
    public int TotalTeachers { get; set; }
    public int TenuredCount { get; set; }
    public int NonTenuredCount { get; set; }
    public Dictionary<string, int> FacultyByDepartment { get; set; } = new();
    public Dictionary<string, int> FacultyByRank { get; set; } = new();
    public Dictionary<string, int> FacultyByTenureStatus { get; set; } = new();
}