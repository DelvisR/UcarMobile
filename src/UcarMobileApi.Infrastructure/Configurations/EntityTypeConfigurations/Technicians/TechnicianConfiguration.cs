using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UcarMobileApi.Core.Entities.Technicians;
using UcarMobileApi.Infrastructure.Configurations.EntityTypeConfigurations.Common;

namespace UcarMobileApi.Infrastructure.Configurations.EntityTypeConfigurations.Technicians;

/// <summary>
/// Entity Framework configuration for Technician entity.
/// Note: Technician inherits from User (TPT inheritance).
/// </summary>
public class TechnicianConfiguration : IEntityTypeConfiguration<Technician>
{
    public void Configure(EntityTypeBuilder<Technician> builder)
    {
        // TPT (Table-Per-Type) inheritance
        builder.ToTable("Technician");

        // Properties
        builder.Property(t => t.IsFreelance)
            .IsRequired()
            .HasDefaultValue(false);

        builder.OwnsOne(t => t.BaseAddress).ConfigureAddressInfo();

        builder.Property(t => t.ProviderAccountId).HasMaxLength(65);
        builder.Property(t => t.ProviderDisplayName).HasMaxLength(255);
        builder.Property(t => t.ProviderPaymentsEnabled).HasDefaultValue(false);

        // Relationships

        // Relationship with TechnicianServiceZone
        builder.HasMany(t => t.ServiceZones)
            .WithOne(tsz => tsz.Technician)
            .HasForeignKey(tsz => tsz.TechnicianId)
            .OnDelete(DeleteBehavior.Cascade);

        // NEW: Relationship with TechnicianSpeciality
        builder.HasMany(t => t.Specialities)
            .WithOne(ts => ts.Technician)
            .HasForeignKey(ts => ts.TechnicianId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(t => t.IsFreelance);
    }
}
