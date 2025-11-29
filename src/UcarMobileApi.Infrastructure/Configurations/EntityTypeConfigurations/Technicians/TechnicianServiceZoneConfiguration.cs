using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UcarMobileApi.Core.Entities.Technicians;

namespace UcarMobileApi.Infrastructure.Configurations.EntityTypeConfigurations.Technicians;

/// <summary>
/// Entity Framework configuration for the TechnicianServiceZone entity.
/// </summary>
public class TechnicianServiceZoneConfiguration : IEntityTypeConfiguration<TechnicianServiceZone>
{
    public void Configure(EntityTypeBuilder<TechnicianServiceZone> builder)
    {
        // Composite PK
        builder.HasKey(x => new { x.TechnicianId, x.ServiceZoneId });

        // Properties
        builder.Property(tsz => tsz.IsPrimaryZone)
            .IsRequired()
            .HasDefaultValue(false);

        // Relationships
        builder.HasOne(tsz => tsz.Technician)
            .WithMany(t => t.ServiceZones)
            .HasForeignKey(tsz => tsz.TechnicianId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(tsz => tsz.ServiceZone)
            .WithMany(sz => sz.Technicians)
            .HasForeignKey(tsz => tsz.ServiceZoneId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
