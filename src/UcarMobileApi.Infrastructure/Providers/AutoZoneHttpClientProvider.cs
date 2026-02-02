using System.Net.Http;
using System.Threading.Tasks;
using UcarMobileApi.Infrastructure.Services.AutoZone.Interfaces;

namespace UcarMobileApi.Infrastructure.Providers
{
    public class AutoZoneHttpClientProvider(HttpClient httpClient, IAutoZoneTokenProvider autoZoneTokenProvider) : IAutoZoneHttpClientProvider
    {
        
        public async Task<HttpClient> Provide()
        {
           var token = await autoZoneTokenProvider.GetTokenAsync();
           httpClient.DefaultRequestHeaders.Remove("Authorization");
           httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
           return httpClient;
        }
    }
}
