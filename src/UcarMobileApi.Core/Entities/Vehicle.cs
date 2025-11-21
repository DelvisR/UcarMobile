using System.Collections.Generic;
using UcarMobileApi.Core.Entities.Services;

namespace UcarMobileApi.Core.Entities;

public class Vehicle : EntityBase
{
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }

    public ICollection<Estimate> Estimates { get; set; } = [];
}
