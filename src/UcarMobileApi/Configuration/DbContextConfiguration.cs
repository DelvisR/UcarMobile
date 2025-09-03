using Microsoft.EntityFrameworkCore;
using UcarMobileApi.Infrastructure.Data;

namespace UcarMobileApi.Configuration;

public static class DbContextConfiguration
{
    public static IServiceCollection AddAppDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                o => o.UseNetTopologySuite()));
        return services;
    }
}