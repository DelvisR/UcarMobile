using System.Threading.Tasks;

namespace UcarMobileApi.Infrastructure.Services.AutoZone.Interfaces
{
    public interface IAutoZoneTokenProvider
    {
        Task<string> GetTokenAsync();
    }
}
