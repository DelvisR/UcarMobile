namespace UcarMobileApi.Application.DTOs.Users;

public class ActionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Resource { get; set; } = "RESOURCE_DEFAULT";
}
