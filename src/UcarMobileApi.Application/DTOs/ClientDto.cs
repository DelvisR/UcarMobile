using System.Collections.Generic;
using UcarMobileApi.Application.DTOs.Users;

namespace UcarMobileApi.Application.DTOs;

public class ClientDto : UserDto
{
    public string Address { get; set; } = string.Empty;
    public bool IsPotential { get; set; }

    public List<VehicleDto> Vehicles { get; set; } = [];
}
