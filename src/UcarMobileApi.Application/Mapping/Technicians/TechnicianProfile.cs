using AutoMapper;
using AutoMapper.EquivalencyExpression;
using UcarMobileApi.Application.DTOs.Technicians;
using UcarMobileApi.Application.DTOs.Users;
using UcarMobileApi.Application.Mapping.Common;
using UcarMobileApi.Core.Entities.Technicians;
using UcarMobileApi.Core.Entities.Users;

namespace UcarMobileApi.Application.Mapping.Technicians;

/// <summary>
/// AutoMapper profile for mapping between Technician entity and TechnicianDto.
/// Uses AutoMapper.Collection for efficient collection synchronization.
/// </summary>
public class TechnicianProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TechnicianProfile"/> class.
    /// Configures mappings for Technician entity and related collections.
    /// </summary>
    public TechnicianProfile()
    {
        // Map Technician entity to TechnicianDto and vice versa
        CreateMap<Technician, TechnicianDto>()
                .IncludeBase<User, UserAccountDto>() // Inherit mapping from base User entity
                .ForMember(dest => dest.AuthProviderId, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(dest => dest.ProviderDisplayName, opt => opt.Ignore())
                .ForMember(dest => dest.ProviderPaymentsEnabled, opt => opt.Ignore())
                .EqualityComparison((dto, entity) => dto.Id == entity.Id); // smart update

        CreateMap<Technician, TechnicianBasicDto>()
            .IncludeBase<User, UserDto>();

        // Partial Update
        CreateMap<TechnicianUpdateDto, Technician>()
            .IncludeBase<UserUpdateDto, User>()
            .IgnoreNullValuesForPatch();

        // Map TechnicianServiceZone entity to TechnicianServiceZoneDto
        CreateMap<TechnicianSpecialityDto, TechnicianSpeciality>()
            .ForMember(dest => dest.ServiceCategoryId, opt => opt.MapFrom(src => src.ServiceCategory!.Id))
            .ForMember(dest => dest.ServiceCategory, opt => opt.Ignore()) // avoid duplicate insertion
            .EqualityComparison((dto, entity) => dto.ServiceCategory != null && dto.ServiceCategory.Id == entity.ServiceCategoryId) // smart update
            .ReverseMap();

        // Map TechnicianSpeciality entity to TechnicianSpecialityDto
        CreateMap<TechnicianServiceZoneDto, TechnicianServiceZone>()
            .ForMember(dest => dest.ServiceZoneId, opt => opt.MapFrom(src => src.ServiceZone!.Id))
            .ForMember(dest => dest.ServiceZone, opt => opt.Ignore()) // avoid duplicate insertion
            .EqualityComparison((dto, entity) => dto.ServiceZone != null && dto.ServiceZone.Id == entity.ServiceZoneId) // smart update
            .ReverseMap();

        //.EqualityComparison((dto, entity) => false) => considers that none match, then deletes everything first 
    }
}
