namespace UcarMobileApi.Core.Entities.Clients;

public class Lead : EntityBase
{
    public string Phone { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
