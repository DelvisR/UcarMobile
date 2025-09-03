namespace UcarMobileApi.Core.Entities
{
    public class Vehicle : EntityBase
    {
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
    }
}
