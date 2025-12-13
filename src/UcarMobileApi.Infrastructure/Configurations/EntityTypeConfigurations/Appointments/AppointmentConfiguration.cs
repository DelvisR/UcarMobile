using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UcarMobileApi.Core.Entities.Appointments;
using UcarMobileApi.Infrastructure.Configurations.EntityTypeConfigurations.Common;

namespace UcarMobileApi.Infrastructure.Configurations.EntityTypeConfigurations.Appointments;

/// <summary>
/// Entity Framework configuration for the Appointment entity.
/// </summary>
public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        // Properties
        builder.OwnsOne(e => e.ServiceAddress).ConfigureAddressInfo();

        builder.Property(a => a.EstimatedTotal)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(a => a.PaymentStatus)
            .IsRequired()
            .HasConversion<byte>();

        builder.Property(a => a.Status)
            .IsRequired()
            .HasConversion<byte>();

        builder.Property(a => a.ScheduledStart)
            .IsRequired();

        // Relationships
        builder.HasOne(a => a.Client)
            .WithMany(c => c.Appointments)
            .HasForeignKey(a => a.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        // NOTE: The relationship with Payments is configured in PaymentConfiguration.cs
        // to use DeleteBehavior.SetNull (preserving payment history if appointment is deleted)

        // Indexes
        builder.HasIndex(a => a.ClientId);
        builder.HasIndex(a => a.Status);
        builder.HasIndex(a => a.ScheduledStart);
        builder.HasIndex(a => new { a.ScheduledStart, a.ScheduledEnd });
    }
}
