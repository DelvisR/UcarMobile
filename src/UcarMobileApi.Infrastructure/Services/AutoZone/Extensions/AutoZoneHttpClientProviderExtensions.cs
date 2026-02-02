using System;
using System.Net.Http;
using System.Threading.Tasks;
using UcarMobileApi.Infrastructure.Services.AutoZone.Interfaces;

namespace UcarMobileApi.Infrastructure.Services.AutoZone.Extensions
{
    public static class AutoZoneHttpClientProviderExtensions
    {
        public static async Task<T> Run<T>(this IAutoZoneHttpClientProvider provider, Func<HttpClient, Task<T>> action)
        {
            var client = await provider.Provide();
            return await action(client);
        }

        public static async Task Run(this IAutoZoneHttpClientProvider provider, Func<HttpClient, Task> action)
        {
            var client = await provider.Provide();
            await action(client);
        }
    }
}
