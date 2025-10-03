using AutoMapper;
using UcarMobileApi.Application.DTOs;
using UcarMobileApi.Application.DTOs.Users;
using UcarMobileApi.Core.Entities;
using UcarMobileApi.Core.Entities.Users;

namespace UcarMobileApi.Application.Mapping;

public class ClientProfile : Profile
{
    public ClientProfile()
    {
        // Inherits from the base mapping of User if you have it configured
        CreateMap<Client, ClientDto>()
            .IncludeBase<User, UserDto>() // takes advantage of UserDto mapping
            .ForMember(dest => dest.Vehicles, opt => opt.Ignore()) //ignore Vehicles
            .ReverseMap();
    }
}
