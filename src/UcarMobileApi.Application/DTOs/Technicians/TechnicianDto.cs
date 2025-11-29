using System.Collections.Generic;
using UcarMobileApi.Application.DTOs.Services;
using UcarMobileApi.Application.DTOs.Users;

namespace UcarMobileApi.Application.DTOs.Technicians;

/// <summary>
/// Data Transfer Object for Technician entity.
/// Inherits from UserDto and includes technician-specific properties.
/// </summary>
public class TechnicianDto : UserAccountDto
{
    /// <summary>
    /// Gets or sets a value indicating whether the technician is a freelance worker.
    /// </summary>
    public bool IsFreelance { get; set; } = false;

    /// <summary>
    /// Gets or sets the list of service zones assigned to this technician.
    /// </summary>
    public List<TechnicianServiceZoneDto> ServiceZones { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of specialities (service categories) for this technician.
    /// </summary>
    public List<TechnicianSpecialityDto> Specialities { get; set; } = [];

    // Display name returned by the provider (optional).
    public string? ProviderDisplayName { get; set; }

    // Indicates whether provider payments/payouts are enabled for this technician.
    public bool ProviderPaymentsEnabled { get; set; }
}

/// <summary>
/// DTO representing a technician's service zone assignment.
/// </summary>
public class TechnicianServiceZoneDto
{
    /// <summary>
    /// Gets or sets the service zone details.
    /// </summary>
    public ServiceZoneDto? ServiceZone { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this is the technician's primary zone.
    /// </summary>
    public bool IsPrimaryZone { get; set; }
}

/// <summary>
/// DTO representing a technician's speciality (service category expertise).
/// </summary>
public class TechnicianSpecialityDto
{
    /// <summary>
    /// Gets or sets the service category ID.
    /// </summary>
    public ServiceCategoryDto? ServiceCategory { get; set; }

    /// <summary>
    /// Gets or sets the skill level (1=Junior, 2=Mid, 3=Senior, 4=Master).
    /// </summary>
    public byte SkillLevel { get; set; } = 1;

    /// <summary>
    /// Gets or sets a value indicating whether the technician is certified in this category.
    /// </summary>
    public bool IsCertified { get; set; }
}
