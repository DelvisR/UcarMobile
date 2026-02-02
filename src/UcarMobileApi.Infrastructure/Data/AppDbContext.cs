using Microsoft.EntityFrameworkCore;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Infrastructure.Configurations.EntityTypeConfigurations;

namespace UcarMobileApi.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IAppDbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Tell Entity Framework Core that your PostgreSQL database uses the PostGIS extension.
        modelBuilder.HasPostgresExtension("postgis");

        // Apply all IEntityTypeConfiguration settings
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Apply the base configuration to all entities that inherit from EntityBase
        modelBuilder.ApplyAuditableConfiguration();

        base.OnModelCreating(modelBuilder);
    }
}
