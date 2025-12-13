using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UcarMobileApi.Core.Entities.Services;

namespace UcarMobileApi.Infrastructure.Configurations.EntityTypeConfigurations.Services;

/// <summary>
/// Entity configuration for ServiceZone.
/// </summary>
public class ServiceZoneConfiguration : IEntityTypeConfiguration<ServiceZone>
{
    public void Configure(EntityTypeBuilder<ServiceZone> builder)
    {
        builder.Property(z => z.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(z => z.BaseAddress)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(z => z.RadiusMiles)
            .IsRequired()
            .HasDefaultValue(25);

        builder.Property(z => z.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Lat / Lng as numeric (double precision)
        builder.Property(z => z.Lat)
            .HasColumnType("double precision");

        builder.Property(z => z.Lng)
            .HasColumnType("double precision");

        // ZipCodes list -> jsonb
        builder.Property(z => z.ZipCodes)
            .HasColumnType("text[]");

        // Useful indexes
        builder.HasIndex(z => z.IsActive);
        builder.HasIndex(z => z.ZipCodes).HasMethod("gin"); // GIN index for jsonb
    }
}
