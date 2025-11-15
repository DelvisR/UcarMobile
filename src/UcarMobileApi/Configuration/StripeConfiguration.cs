using UcarMobileApi.Infrastructure.Factories;

namespace UcarMobileApi.Configuration;

/// <summary>
/// Configures and registers Stripe integration services.
/// Uses <see cref="StripeClientFactory"/> to load and cache the secret key.
/// </summary>
public static class StripeConfiguration
{
    /// <summary>
    /// Initializes Stripe API configuration using the cached secret key.
    /// Must be called after the application has built the service provider.
    /// </summary>
    public static async Task InitializeStripeAsync(this IServiceProvider serviceProvider)
    {
        var stripeFactory = serviceProvider.GetRequiredService<StripeClientFactory>();

        // Initialize Stripe SDK using credentials from the factory
        await stripeFactory.InitializeStripeAsync();
    }
}
