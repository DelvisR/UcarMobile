using Microsoft.EntityFrameworkCore;
using Serilog;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Infrastructure.Data;
using UcarMobileApi.Infrastructure.Factories;
using UcarMobileApi.Infrastructure.Interceptors;

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
    public static IServiceCollection AddAppDbContext(this IServiceCollection services, IWebHostEnvironment env)
    {
        services.AddScoped<PersistenceInterceptor>();

        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            var factory = serviceProvider.GetRequiredService<IDbConnectionFactory>();
            var connectionString = factory.GetConnectionStringAsync().GetAwaiter().GetResult();

            options.UseNpgsql(connectionString, o => o
                .UseNetTopologySuite()
                .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));

            options.AddInterceptors(serviceProvider.GetRequiredService<PersistenceInterceptor>());

            if (env.IsDevelopment())
            {
                options
                    .EnableSensitiveDataLogging()
                    .LogTo(Log.Information, LogLevel.Information);
            }
        });

        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
        return services;
    }
}
