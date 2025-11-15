namespace UcarMobileApi.Application.DTOs.Clients;

public class LeadDto
{
    public int Id { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
