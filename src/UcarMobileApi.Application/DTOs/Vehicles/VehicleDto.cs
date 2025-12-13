using System.Collections.Generic;

namespace UcarMobileApi.Application.DTOs.Vehicles;

public class VehicleDto
{
    public int Id { get; set; }
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
}

public class ModelDto
{
    public int VehicleId { get; set; }
    public string Model { get; set; } = string.Empty;
    public ICollection<SubModelDto> SubModels { get; set; } = [];
}

public class SubModelDto
{
    public string Name { get; set; } = string.Empty;
    public ICollection<EngineDto> Engines { get; set; } = [];
}

public class EngineDto
{
    public string Engine { get; set; } = string.Empty;
    public string? VehicleType { get; set; }
    public string? BodyType { get; set; }
    public string? Fuel { get; set; }
    public int? AzId { get; set; }
}
