using System.Collections.Generic;
using System.Threading.Tasks;
using UcarMobileApi.Infrastructure.Configurations.AutoZone.Models;


namespace UcarMobileApi.Infrastructure.Services.AutoZone.Interfaces
{
    public interface IAutoZoneService
    {
        Task<List<OrderDto>> GetOrders(OrderRequestDto request);
    }
}
