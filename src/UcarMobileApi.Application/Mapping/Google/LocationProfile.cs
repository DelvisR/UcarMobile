using AutoMapper;
using UcarMobileApi.Application.DTOs.Google.Location;

namespace UcarMobileApi.Application.Mapping.Google;

public class LocationProfile : Profile
{
    public LocationProfile()
    {
        CreateMap<AutocompleteSuggestion, AddressSuggestionDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.PlacePrediction.PlaceId))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.PlacePrediction.Text.Text));
    }
}
