using Microsoft.EntityFrameworkCore;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Infrastructure.Data;
using UcarMobileApi.Infrastructure.Services;

namespace UcarMobileApi.Configuration;

/// <summary>
/// Configures and registers the EF Core database context (AppDbContext).
/// Uses <see cref="IDbConnectionFactory"/> to obtain the connection string asynchronously.
/// </summary>
public static class DbContextConfiguration
{
    /// <summary>
    /// Registers and configures the application's database context.
    /// Also registers IAppDbContext to resolve the DbContext via its interface.
    /// </summary>
    public static IServiceCollection AddAppDbContext(this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            var factory = serviceProvider.GetRequiredService<IDbConnectionFactory>();
            var connectionString = factory.GetConnectionStringAsync().GetAwaiter().GetResult();

            options.UseNpgsql(connectionString, o => o
                .UseNetTopologySuite()
                .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
        });

        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
        return services;
    }
}
