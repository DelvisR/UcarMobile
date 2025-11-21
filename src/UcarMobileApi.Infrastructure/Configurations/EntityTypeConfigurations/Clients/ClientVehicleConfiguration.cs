using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UcarMobileApi.Core.Entities.Clients;

namespace UcarMobileApi.Infrastructure.Configurations.EntityTypeConfigurations.Clients;

/// <summary>
/// Entity Framework configuration for the ClientVehicle entity.
/// </summary>
public class ClientVehicleConfiguration : IEntityTypeConfiguration<ClientVehicle>
{
    public void Configure(EntityTypeBuilder<ClientVehicle> builder)
    {
        // Properties
        builder.Property(cv => cv.VIN)
            .HasMaxLength(17);

        builder.Property(cv => cv.LicensePlate)
            .HasMaxLength(20);

        builder.Property(cv => cv.Submodel)
            .HasMaxLength(50);

        builder.Property(cv => cv.EngineType)
            .HasMaxLength(50);

        builder.Property(cv => cv.VehicleType)
            .HasMaxLength(50);

        builder.Property(cv => cv.BodyType)
            .HasMaxLength(50);

        builder.Property(cv => cv.Notes)
            .HasMaxLength(1000);

        // Relationships
        builder.HasOne(cv => cv.Client)
            .WithMany(c => c.Vehicles)
            .HasForeignKey(cv => cv.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cv => cv.Vehicle)
            .WithMany()
            .HasForeignKey(cv => cv.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(cv => cv.Appointments)
            .WithOne(av => av.Vehicle)
            .HasForeignKey(av => av.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(cv => cv.ClientId);
        builder.HasIndex(cv => cv.VehicleId);
        builder.HasIndex(cv => cv.VIN).IsUnique();
        builder.HasIndex(cv => cv.LicensePlate);

        // Composite unique constraint for Client-Vehicle relationship
        builder.HasIndex(cv => new { cv.ClientId, cv.VehicleId }).IsUnique();
    }
}
