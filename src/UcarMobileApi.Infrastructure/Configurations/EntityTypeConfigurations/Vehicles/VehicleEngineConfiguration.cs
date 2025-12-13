using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UcarMobileApi.Core.Entities.Vehicles;

namespace UcarMobileApi.Infrastructure.Configurations.EntityTypeConfigurations.Vehicles;

public class VehicleEngineConfiguration : IEntityTypeConfiguration<VehicleEngine>
{
    public void Configure(EntityTypeBuilder<VehicleEngine> builder)
    {
        // Properties
        builder.Property(e => e.Engine)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(e => e.VehicleType)
            .HasMaxLength(60);

        builder.Property(e => e.BodyType)
            .HasMaxLength(80);

        builder.Property(e => e.Fuel)
            .HasMaxLength(60);

        // Relationships
        builder.HasOne(e => e.SubModel)
            .WithMany(sm => sm.Engines)
            .HasForeignKey(e => e.SubModelId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(e => new { e.SubModelId, e.AzId }).IsUnique();
    }
}
