using System.Net.Http;
using System.Threading.Tasks;

namespace UcarMobileApi.Infrastructure.Services.AutoZone.Interfaces
{
    public interface IAutoZoneHttpClientProvider
    {
        Task<HttpClient> Provide();
    }
}
