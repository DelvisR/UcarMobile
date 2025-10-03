using Microsoft.AspNetCore.Authorization;
using UcarMobileApi.Authorization;

namespace UcarMobileApi.Configuration
{
    /// <summary>
    /// Provides extension methods for configuring authorization services.
    /// </summary>
    /// <remarks>
    /// Registers the <see cref="ActionHandler"/> and <see cref="DynamicActionPolicyProvider"/> 
    /// and enables dynamic action-based policies.
    /// </remarks>
    public static class AuthorizationConfiguration
    {
        /// <summary>
        /// Adds and configures authorization services for dynamic action policies.
        /// </summary>
        /// <param name="services">The service collection to add services to.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddAuthorizationServices(this IServiceCollection services)
        {
            // Add authorization support
            services.AddAuthorization();

            // Register handler that validates actions
            services.AddScoped<IAuthorizationHandler, ActionHandler>();

            // Register dynamic policy provider
            services.AddSingleton<IAuthorizationPolicyProvider, DynamicActionPolicyProvider>();

            return services;
        }
    }
}
