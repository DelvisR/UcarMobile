using AutoMapper;
using UcarMobileApi.Application.DTOs.Clients;
using UcarMobileApi.Application.DTOs.Users;
using UcarMobileApi.Core.Entities.Clients;
using UcarMobileApi.Core.Entities.Users;

namespace UcarMobileApi.Application.Mapping.Clients;

public class ClientProfile : Profile
{
    public ClientProfile()
    {
        // Inherits from the base mapping of User if you have it configured
        CreateMap<Client, ClientDto>()
            .IncludeBase<User, UserDto>() // takes advantage of UserDto mapping
            .ForMember(dest => dest.AuthProviderId, opt => opt.Ignore())
            .ReverseMap();
        //.ForMember(dest => dest.AuthProviderId, opt => opt.Ignore());
    }
}
