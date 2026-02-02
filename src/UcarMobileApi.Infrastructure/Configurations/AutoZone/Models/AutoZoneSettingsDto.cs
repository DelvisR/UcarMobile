namespace UcarMobileApi.Infrastructure.Configurations.AutoZone.Models
{
    public class AutoZoneSettingsDto
    {
        public required string ApiBaseUrl { get; set; }
        public required string OAuthUrl { get; set; }
        public required string AuthScope { get; set; }
        public required string ClientId { get; set; }
        public required string ClientSecret { get; set; }
        public required string Service { get; set; }
    }
}
