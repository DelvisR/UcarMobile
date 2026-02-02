using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UcarMobileApi.Application.Services.Security;
using UcarMobileApi.Infrastructure.Configurations.AutoZone.Models;
using UcarMobileApi.Infrastructure.Services.AutoZone.Interfaces;

namespace UcarMobileApi.Infrastructure.Providers
{
    public class AutoZoneTokenProvider : IAutoZoneTokenProvider
    {
        private readonly ICacheService _cacheService;
        private readonly HttpClient _httpClient;
        private readonly AutoZoneSettingsDto _autoZoneSettings;

        private const string tokenCacheKey = "AutoZoneBearerToken";

        public AutoZoneTokenProvider(HttpClient httpClient, AutoZoneSettingsDto autoZoneSettings, ICacheService cacheService)
        {
            _httpClient = httpClient;
            _autoZoneSettings = autoZoneSettings;
            _cacheService = cacheService;
        }

        /// <summary>
        /// Asynchronously retrieves an access token, using a distributed cache to minimize unnecessary token requests.
        /// </summary>
        /// <remarks>If the token is not present in the cache or has expired, a new token is requested and
        /// stored in the cache with an expiration slightly shorter than the token's actual lifetime to account for
        /// clock skew. This method is thread-safe if the underlying distributed cache implementation is
        /// thread-safe.</remarks>
        /// <returns>A string containing the access token. The token is retrieved from the cache if available; otherwise, a new
        /// token is requested and cached before being returned.</returns>
        public async Task<string> GetTokenAsync()
        {
            var token = await _cacheService.GetAsync<string>(tokenCacheKey);

            if (token is null)
            {
                var tokenResponse = await RequestToken();
                var cacheExpiration = Math.Max(30, (int)(tokenResponse.ExpiresIn * 0.9));

                await _cacheService.SetAsync(tokenCacheKey, tokenResponse.AccessToken, TimeSpan.FromSeconds(tokenResponse.ExpiresIn - 300), null);

                token = tokenResponse.AccessToken;
            }

            return token;
        }

        /// <summary>
        /// Requests a new authentication token from the AutoZone OAuth2 token endpoint using client credentials.
        /// </summary>
        /// <remarks>This method sends a POST request to the configured AutoZone OAuth2 token endpoint
        /// using the client credentials grant type. The returned token can be used to authenticate subsequent API
        /// requests. The method ensures the HTTP response indicates success before attempting to deserialize the
        /// response content.</remarks>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="AutoZoneAuthDto"/>
        /// object with the authentication token and related information.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the response from the token endpoint cannot be deserialized into an <see cref="AutoZoneAuthDto"/>
        /// object.</exception>
        private async Task<AutoZoneAuthDto> RequestToken()
        {

            var formData = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "scope", _autoZoneSettings.AuthScope },
                { "service", _autoZoneSettings.Service },
                { "client_id", _autoZoneSettings.ClientId },
                { "client_secret", _autoZoneSettings.ClientSecret }
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, _autoZoneSettings.OAuthUrl)
            {
                Content = new FormUrlEncodedContent(formData)
            };

            using var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var authResponse = JsonConvert.DeserializeObject<AutoZoneAuthDto>(responseBody);

            return authResponse
                ?? throw new InvalidOperationException($"Failed to deserialize response from AutoZone token endpoint. Response: {responseBody}");
        }
    }
}
