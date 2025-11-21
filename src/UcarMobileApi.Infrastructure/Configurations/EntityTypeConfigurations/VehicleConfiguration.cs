using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UcarMobileApi.Core.Entities;

namespace UcarMobileApi.Infrastructure.Configurations.EntityTypeConfigurations;

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

        // Relationships
        builder.HasMany(v => v.Estimates)
            .WithOne(e => e.Vehicle)
            .HasForeignKey(e => e.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(v => new { v.Make, v.Model, v.Year });
        builder.HasIndex(v => v.Make);
        builder.HasIndex(v => v.Year);
    }
}
