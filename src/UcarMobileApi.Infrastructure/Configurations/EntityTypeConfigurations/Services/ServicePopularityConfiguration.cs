using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UcarMobileApi.Core.Entities.Services;

public class ServicePopularityConfiguration : IEntityTypeConfiguration<ServicePopularity>
{
    public void Configure(EntityTypeBuilder<ServicePopularity> builder)
    {
        // Composite key
        builder.HasKey(sp => new { sp.ServiceId, sp.VehicleId });

        // Relationships
        // Service 1 -> N ServicePopularity
        builder.HasOne(sp => sp.Service)
            .WithMany(s => s.PopularityEntries)
            .HasForeignKey(sp => sp.ServiceId)
            .OnDelete(DeleteBehavior.Cascade);

        // Vehicle 1 -> N ServicePopularity
        builder.HasOne(sp => sp.Vehicle)
            .WithMany(v => v.PopularServices)
            .HasForeignKey(sp => sp.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Count field
        builder.Property(sp => sp.Count)
            .IsRequired()
            .HasDefaultValue(0);
    }
}
