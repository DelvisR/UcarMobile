namespace UcarMobileApi.Application.DTOs.Common;

public class AddressInfoDto
{
    public string FullAddress { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;

    public double Lat { get; set; }
    public double Lng { get; set; }
}
