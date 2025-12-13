using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UcarMobileApi.Core.Entities.Vehicles;

namespace UcarMobileApi.Infrastructure.Configurations.EntityTypeConfigurations.Vehicles;

public class VehicleSubModelConfiguration : IEntityTypeConfiguration<VehicleSubModel>
{
    public void Configure(EntityTypeBuilder<VehicleSubModel> builder)
    {
        // Properties
        builder.Property(sm => sm.Name)
            .HasMaxLength(60)
            .IsRequired();

        // Relationships
        builder.HasOne(sm => sm.Vehicle)
            .WithMany(v => v.SubModels)
            .HasForeignKey(sm => sm.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(sm => new { sm.VehicleId, sm.AzId }).IsUnique();
    }
}
