using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UcarMobileApi.Core.Entities.Appointments;

namespace UcarMobileApi.Infrastructure.Configurations.EntityTypeConfigurations.Appointments;

/// <summary>
/// Entity Framework configurations for Appointment-related entities.
/// </summary>
public class AppointmentDocumentConfiguration : IEntityTypeConfiguration<AppointmentDocument>
{
    public void Configure(EntityTypeBuilder<AppointmentDocument> builder)
    {
        // Composite PK
        builder.HasKey(x => new { x.AppointmentId, x.StoredFileId });

        // Relationships
        builder.HasOne(ad => ad.Appointment)
            .WithMany(a => a.Documents)
            .HasForeignKey(ad => ad.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ad => ad.StoredFile)
            .WithMany()
            .HasForeignKey(ad => ad.StoredFileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ad => ad.AppointmentNote)
            .WithMany()
            .HasForeignKey(ad => ad.AppointmentNoteId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        //builder.HasIndex(ad => ad.AppointmentId);
        //builder.HasIndex(ad => ad.StoredFileId);
        builder.HasIndex(ad => ad.AppointmentNoteId);
    }
}

public class AppointmentNoteConfiguration : IEntityTypeConfiguration<AppointmentNote>
{
    public void Configure(EntityTypeBuilder<AppointmentNote> builder)
    {
        // Properties
        builder.Property(an => an.Content)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(an => an.Source)
            .IsRequired()
            .HasConversion<byte>();

        // Relationships
        builder.HasOne(an => an.Appointment)
            .WithMany(a => a.Notes)
            .HasForeignKey(an => an.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(an => an.AppointmentId);
        builder.HasIndex(an => an.Source);
        builder.HasIndex(an => an.CreatedDate);
    }
}

public class AppointmentServiceConfiguration : IEntityTypeConfiguration<AppointmentService>
{
    public void Configure(EntityTypeBuilder<AppointmentService> builder)
    {
        // Composite PK
        builder.HasKey(x => new { x.AppointmentVehicleId, x.ServiceId });

        // Properties
        builder.Property(s => s.Price)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(s => s.Notes)
            .HasMaxLength(500);

        // Relationships
        builder.HasOne(s => s.AppointmentVehicle)
            .WithMany(av => av.Services)
            .HasForeignKey(s => s.AppointmentVehicleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Service)
            .WithMany(av => av.Appointments)
            .HasForeignKey(s => s.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class AppointmentPartConfiguration : IEntityTypeConfiguration<AppointmentPart>
{
    public void Configure(EntityTypeBuilder<AppointmentPart> builder)
    {
        // Properties
        builder.Property(ap => ap.PartName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(ap => ap.PartNumber)
            .HasMaxLength(50);

        builder.Property(ap => ap.Quantity)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(ap => ap.UnitPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(ap => ap.Notes)
            .HasMaxLength(500);

        // Relationships
        builder.HasOne(ap => ap.AppointmentVehicle)
            .WithMany(av => av.Parts)
            .HasForeignKey(ap => ap.AppointmentVehicleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(ap => ap.AppointmentVehicleId);
        builder.HasIndex(ap => ap.PartNumber);
    }
}

public class AppointmentVehicleConfiguration : IEntityTypeConfiguration<AppointmentVehicle>
{
    public void Configure(EntityTypeBuilder<AppointmentVehicle> builder)
    {
        // Relationships
        builder.HasOne(av => av.Appointment)
            .WithMany(a => a.Vehicles)
            .HasForeignKey(av => av.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(av => av.Technician)
            .WithMany(t => t.Appointments)
            .HasForeignKey(av => av.TechnicianId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(av => av.Vehicle)
            .WithMany(cv => cv.Appointments)
            .HasForeignKey(av => av.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(av => av.AppointmentId);
        builder.HasIndex(av => av.TechnicianId);
        builder.HasIndex(av => av.VehicleId);

        // Composite unique constraint
        builder.HasIndex(av => new { av.AppointmentId, av.VehicleId, av.TechnicianId }).IsUnique();
    }
}
