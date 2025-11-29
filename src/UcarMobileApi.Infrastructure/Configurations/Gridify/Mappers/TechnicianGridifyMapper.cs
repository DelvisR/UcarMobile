using Gridify;
using UcarMobileApi.Core.Entities.Technicians;

namespace UcarMobileApi.Infrastructure.Configurations.Gridify.Mappers;

/// <summary>
/// Gridify mapper for Technician entity.
/// Configures filterable and sortable fields for pagination and search.
/// </summary>
public class TechnicianGridifyMapper : GridifyMapper<Technician>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TechnicianGridifyMapper"/> class.
    /// Generates mappings for all properties and adds a global search field.
    /// </summary>
    public TechnicianGridifyMapper()
    {
        // Automatically map all regular properties
        GenerateMappings();

        // Add a virtual "Global" field that combines multiple columns for global search
        AddMap("Global", t => t.FirstName + " " + t.LastName + " " + t.Phone + " " + t.Email + " " + t.LangKey);
    }
}
