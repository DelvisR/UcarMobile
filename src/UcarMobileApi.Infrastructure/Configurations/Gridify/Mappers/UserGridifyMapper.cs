using Gridify;
using UcarMobileApi.Core.Entities.Users;

namespace UcarMobileApi.Infrastructure.Configurations.Gridify.Mappers;

public class UserGridifyMapper : GridifyMapper<User>
{
    public UserGridifyMapper()
    {
        // Map all regular properties
        GenerateMappings();

        // Map a virtual field "Global" that combines multiple columns
        AddMap("Global", l => l.FirstName + " " + l.LastName + " " + l.Phone + " " + l.Email + " " + l.LangKey);
    }
}
