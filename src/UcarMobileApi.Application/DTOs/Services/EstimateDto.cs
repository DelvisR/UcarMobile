namespace UcarMobileApi.Application.DTOs.Services;

public class EstimateDto
{
    public int ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public int ServiceCategoryId { get; set; }
    public string ServiceCategoryName { get; set; } = string.Empty;
    public decimal LaborMaxCost { get; set; }
    public decimal LaborMinCost { get; set; }
    public decimal PartMaxCost { get; set; }
    public decimal PartMinCost { get; set; }
    public decimal LaborCost { get; set; }
    public decimal PartCost { get; set; }
    public decimal TotalCost { get; set; }
}
