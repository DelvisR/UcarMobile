namespace UcarMobileApi.Application.DTOs.Services;

public class ServiceCategoryDto
{
    public int Id { get; set; }
    public string? Name { get; set; } = string.Empty;
    public ServiceTypeDto? ServiceType { get; set; } = null!;
}
