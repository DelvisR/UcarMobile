using UcarMobileApi.Core.Entities.Common;
using UcarMobileApi.Core.Entities.Services;

namespace UcarMobileApi.Core.Entities.Technicians;

/// <summary>
/// Represents the many-to-many relationship between Technician and ServiceCategory.
/// Defines which service categories a technician specializes in.
/// </summary>
public class TechnicianSpeciality : AuditableEntity
{
    public int TechnicianId { get; set; }
    public Technician Technician { get; set; } = null!;

    public int ServiceCategoryId { get; set; }
    public ServiceCategory ServiceCategory { get; set; } = null!;

    public SkillLevel SkillLevel { get; set; } = SkillLevel.Junior;
    public bool IsCertified { get; set; }
}

public enum SkillLevel : byte
{
    Junior = 1,
    Mid,
    Senior,
    Master
}
