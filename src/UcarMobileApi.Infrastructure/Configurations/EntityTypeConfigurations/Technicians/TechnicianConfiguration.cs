using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UcarMobileApi.Core.Entities.Technicians;

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

        // Relationships

        // Relationship with AppointmentVehicle (configured in AppointmentVehicleConfiguration)
        builder.HasMany(t => t.Appointments)
            .WithOne(av => av.Technician)
            .HasForeignKey(av => av.TechnicianId)
            .OnDelete(DeleteBehavior.Cascade);

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
