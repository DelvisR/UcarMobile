using Newtonsoft.Json;

namespace UcarMobileApi.Infrastructure.Configurations.AutoZone.Models
{
    public class AutoZoneAuthDto
    {
        [JsonProperty("token_type")]
        public string? TokenType { get; set; }
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
        [JsonProperty("access_token")]
        public string? AccessToken { get; set; }
        [JsonProperty("scope")]
        public string? Scope { get; set; }

    }
}
