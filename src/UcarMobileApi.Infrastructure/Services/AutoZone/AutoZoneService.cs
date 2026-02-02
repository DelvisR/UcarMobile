using System.Collections.Generic;
using System.Threading.Tasks;
using UcarMobileApi.Infrastructure.Configurations.AutoZone.Models;
using UcarMobileApi.Infrastructure.Services.AutoZone.Extensions;
using UcarMobileApi.Infrastructure.Services.AutoZone.Interfaces;

namespace UcarMobileApi.Infrastructure.Services.AutoZone
{
    public class AutoZoneService(IAutoZoneHttpClientProvider autoZoneHttpClientProvider) : IAutoZoneService
    {
          private readonly IAutoZoneHttpClientProvider _autoZoneHttpClientProvider = autoZoneHttpClientProvider;

        /// <summary>
        /// This method retrieves orders from the AutoZone system. 
        /// </summary>
        /// <returns>
        /// A <see cref="OrderDto"/> containing the orders retrieved from AutoZone.
        /// </returns>
        public async Task<List<OrderDto>> GetOrders(OrderRequestDto body)
        {
            var httpResponse = await _autoZoneHttpClientProvider.Run(c => c.Post<OrderRequestDto, List<OrderDto>>($"/commercial/electronic-ordering/v1/orders", body));
            return httpResponse;
        }
    }
}
