using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UcarMobileApi.Core.Entities.Services;

namespace UcarMobileApi.Infrastructure.Configurations.EntityTypeConfigurations.Services;

public class EstimateConfiguration : IEntityTypeConfiguration<Estimate>
{
    public void Configure(EntityTypeBuilder<Estimate> builder)
    {
        // Use decimal for monetary values
        builder.Property(e => e.LaborMaxCost)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(e => e.LaborMinCost)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(e => e.PartMaxCost)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(e => e.PartMinCost)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        // Relationships
        builder.HasOne(e => e.Service)
            .WithMany(s => s.Estimates)
            .HasForeignKey(e => e.ServiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Vehicle)
            .WithMany(v => v.Estimates)
            .HasForeignKey(e => e.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(e => e.ServiceId);
        builder.HasIndex(e => e.VehicleId);
        builder.HasIndex(e => new { e.ServiceId, e.VehicleId }).IsUnique();
    }
}
