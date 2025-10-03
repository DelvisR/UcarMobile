using AutoMapper;
using UcarMobileApi.Application.DTOs;
using UcarMobileApi.Core.Entities.Configurations;

namespace UcarMobileApi.Application.Mapping.Configurations;

public class BusinessParameterProfile : Profile
{
    public BusinessParameterProfile()
    {
        CreateMap<BusinessParameter, BusinessParameterDto>();
    }
}
