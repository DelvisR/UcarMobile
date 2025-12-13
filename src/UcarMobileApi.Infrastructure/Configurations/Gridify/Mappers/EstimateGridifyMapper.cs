using Gridify;
using UcarMobileApi.Core.Entities.Services;

namespace UcarMobileApi.Infrastructure.Configurations.Gridify.Mappers;

public class EstimateGridifyMapper : GridifyMapper<Estimate>
{
    public EstimateGridifyMapper()
    {
        GenerateMappings();

        // Custom mappings if needed
        AddMap("serviceName", e => e.Service.Name);
        AddMap("categoryName", e => e.Service.ServiceCategory.Name);
    }
}
