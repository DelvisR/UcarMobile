using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UcarMobileApi.Infrastructure.Services.AutoZone.Extensions
{
    public static class HttpClientAutoZoneExtensions
    {
        public static async Task<T> Get<T>(this HttpClient httpClient, string uri)
        {
            return await httpClient.GetFromJsonAsync<T>(uri)
                        ?? throw new InvalidOperationException($"Response from '{uri}' was null.");
        }

        public static async Task<TResponse> Post<TRequest, TResponse>(this HttpClient httpClient, string uri, TRequest body)
        {
            var response = await httpClient.PostAsJsonAsync(uri, body);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<TResponse>()
                ?? throw new InvalidOperationException($"Response from '{uri}' was null.");

            return result;
        }
    }
}

