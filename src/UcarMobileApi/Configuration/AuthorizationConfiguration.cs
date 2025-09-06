using Microsoft.AspNetCore.Authorization;
using UcarMobileApi.Authorization;

namespace UcarMobileApi.Configuration
{
    /// <summary>
    /// Provides extension methods for configuring authorization services.
    /// </summary>
    /// <remarks>
    /// Registers the <see cref="PermissionHandler"/> and <see cref="DynamicPermissionPolicyProvider"/> 
    /// and enables dynamic permission-based policies.
    /// </remarks>
    public static class AuthorizationConfiguration
    {
        /// <summary>
        /// Adds and configures authorization services for dynamic permission policies.
        /// </summary>
        /// <param name="services">The service collection to add services to.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddAuthorizationServices(this IServiceCollection services)
        {
            // Add authorization support
            services.AddAuthorization();

            // Register handler that validates permissions
            services.AddScoped<IAuthorizationHandler, PermissionHandler>();

            // Register dynamic policy provider
            services.AddSingleton<IAuthorizationPolicyProvider, DynamicPermissionPolicyProvider>();

            return services;
        }
    }
}
