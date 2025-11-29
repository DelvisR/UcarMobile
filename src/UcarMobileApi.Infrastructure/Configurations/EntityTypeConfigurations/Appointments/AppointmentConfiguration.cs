using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UcarMobileApi.Core.Entities.Appointments;

namespace UcarMobileApi.Infrastructure.Configurations.EntityTypeConfigurations.Appointments;

/// <summary>
/// Entity Framework configuration for the Appointment entity.
/// </summary>
public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        // Properties
        builder.Property(a => a.ServiceAddress)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(a => a.Lat)
            .HasColumnType("double precision")
            .IsRequired();

        builder.Property(a => a.Lng)
            .HasColumnType("double precision")
            .IsRequired();

        builder.Property(a => a.EstimatedTotal)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(a => a.Status)
            .IsRequired()
            .HasConversion<byte>();

        builder.Property(a => a.ScheduledStart)
            .IsRequired();

        // Relationships
        builder.HasOne(a => a.Client)
            .WithMany(c => c.Appointments)
            .HasForeignKey(a => a.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.Vehicles)
            .WithOne(av => av.Appointment)
            .HasForeignKey(av => av.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.Notes)
            .WithOne(an => an.Appointment)
            .HasForeignKey(an => an.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.Documents)
            .WithOne(ad => ad.Appointment)
            .HasForeignKey(ad => ad.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // NOTE: The relationship with Payments is configured in PaymentConfiguration.cs
        // to use DeleteBehavior.SetNull (preserving payment history if appointment is deleted)

        // Indexes
        builder.HasIndex(a => a.ClientId);
        builder.HasIndex(a => a.Status);
        builder.HasIndex(a => a.PaymentStatus); // Index for fast filtering by payment status
        builder.HasIndex(a => a.ScheduledStart);
        builder.HasIndex(a => new { a.Lat, a.Lng });
    }
}
