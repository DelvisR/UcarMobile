using AutoMapper;
using UcarMobileApi.Application.Mapping.Users;

namespace UcarMobileApi.Tests.TestHelpers;

public static class TestMapperFactory
{
    public static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UserMappingProfile>();
            // More profile
        });
        return config.CreateMapper();
    }
}