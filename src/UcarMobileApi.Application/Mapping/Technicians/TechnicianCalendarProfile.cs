using AutoMapper;
using UcarMobileApi.Application.DTOs.Technicians;
using UcarMobileApi.Core.Entities.Technicians;

namespace UcarMobileApi.Application.Mapping.Technicians;

/// <summary>Maps between DTOs and Calendar entities.</summary>
public class TechnicianCalendarProfile : Profile
{
    public TechnicianCalendarProfile()
    {
        CreateMap<TechnicalWorkScheduleDto, TechnicalWorkSchedule>()
            .ForMember(d => d.Id, opt => opt.Ignore());

        CreateMap<TechnicalWorkSchedule, TechnicalWorkScheduleReadDto>();

        CreateMap<TechnicalCalendarBlockDto, TechnicalCalendarBlock>()
            .ForMember(d => d.Id, opt => opt.Ignore());

        CreateMap<TechnicalCalendarBlock, TechnicalCalendarBlockReadDto>();
    }
}
