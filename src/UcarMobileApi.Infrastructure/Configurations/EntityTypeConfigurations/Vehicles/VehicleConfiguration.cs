using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UcarMobileApi.Core.Entities.Vehicles;

namespace UcarMobileApi.Infrastructure.Configurations.EntityTypeConfigurations.Vehicles;

/// <summary>
/// Entity Framework configuration for the Vehicle entity.
/// </summary>
public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        // Properties
        builder.Property(v => v.Make)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(v => v.Model)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(v => v.Year)
            .IsRequired();

        // Indexes
        builder.HasIndex(v => new { v.Make, v.Model, v.Year });
        builder.HasIndex(v => v.Make);
        builder.HasIndex(v => v.Year);
    }
}
