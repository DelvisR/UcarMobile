using AutoMapper;
using NetTopologySuite.Geometries;
using UcarMobileApi.Application.DTOs.Common;
using UcarMobileApi.Core.Entities.Common;

namespace UcarMobileApi.Application.Mapping.Common;

public class AddressInfoProfile : Profile
{
    public AddressInfoProfile()
    {
        // 1. Mapping from Entity (AddressInfo) to DTO (AddressInfoDto)
        CreateMap<AddressInfo, AddressInfoDto>()
            // Map the coordinates from the NTS Point object to separate doubles in the DTO

            // Map Longitude (X) from BasePoint to the DTO's Lng property
            .ForMember(dest => dest.Lng, opt => opt.MapFrom(src => src.BasePoint.X))

            // Map Latitude (Y) from BasePoint to the DTO's Lat property
            .ForMember(dest => dest.Lat, opt => opt.MapFrom(src => src.BasePoint.Y));
        // AutoMapper automatically maps FullAddress and ZipCode if names match.


        // 2. Mapping from DTO (AddressInfoDto) to Entity (AddressInfo)
        CreateMap<AddressInfoDto, AddressInfo>()
            // Map the separate Lat/Lng doubles from the DTO back into an NTS Point object
            // This is crucial for saving the data to the PostgreSQL geography column.
            .ForMember(dest => dest.BasePoint, opt => opt.MapFrom(src => new Point(src.Lng, src.Lat) { SRID = 4326 }))

            // AutoMapper automatically maps FullAddress and ZipCode if names match.

            // Ensure any computed properties in the Entity (Lat/Lng getters) are ignored during reverse mapping
            .ForMember(dest => dest.Lat, opt => opt.Ignore())
            .ForMember(dest => dest.Lng, opt => opt.Ignore());
    }
}
