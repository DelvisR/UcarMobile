using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using UcarMobileApi.Infrastructure.Configurations.EntityTypeConfigurations;
using UcarMobileApi.Infrastructure.Interceptors;

namespace UcarMobileApi.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor httpContextAccessor) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.AddInterceptors(new AuditInterceptor(httpContextAccessor));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Tell Entity Framework Core that your PostgreSQL database uses the PostGIS extension.
        modelBuilder.HasPostgresExtension("postgis");

        // Apply all IEntityTypeConfiguration settings
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Apply the base configuration to all entities that inherit from EntityBase
        modelBuilder.ApplyEntityBaseConfiguration();

        base.OnModelCreating(modelBuilder);
    }

}