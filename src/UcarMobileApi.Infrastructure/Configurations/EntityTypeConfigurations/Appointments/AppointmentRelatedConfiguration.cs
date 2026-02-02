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

        builder.Property(an => an.Source)
            .IsRequired()
            .HasConversion<byte>();

        // Relationships
        builder.HasOne(ad => ad.Appointment)
            .WithMany(a => a.Documents)
            .HasForeignKey(ad => ad.AppointmentId)
            .OnDelete(DeleteBehavior.ClientCascade);

        builder.HasOne(ad => ad.StoredFile)
            .WithOne()
            .HasForeignKey<AppointmentDocument>(ad => ad.StoredFileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class AppointmentNoteDocumentConfiguration : IEntityTypeConfiguration<AppointmentNoteDocument>
{
    public void Configure(EntityTypeBuilder<AppointmentNoteDocument> builder)
    {
        // Composite PK
        builder.HasKey(x => new { x.AppointmentNoteId, x.StoredFileId });

        builder.HasOne(d => d.AppointmentNote)
            .WithMany(n => n.Documents)
            .HasForeignKey(d => d.AppointmentNoteId)
            .OnDelete(DeleteBehavior.ClientCascade);

        builder.HasOne(d => d.StoredFile)
            .WithOne()
            .HasForeignKey<AppointmentNoteDocument>(ad => ad.StoredFileId)
            .OnDelete(DeleteBehavior.Restrict);
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
            .OnDelete(DeleteBehavior.ClientCascade);

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
        // Properties
        builder.Property(s => s.Price)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(s => s.CustomService)
            .HasMaxLength(200);

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

        builder.Property(ap => ap.IsCustomerProvidedPart)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(ap => ap.Note)
            .HasMaxLength(500);

        // Relationships
        builder.HasOne(ap => ap.AppointmentService)
            .WithMany(av => av.Parts)
            .HasForeignKey(ap => ap.AppointmentServiceId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
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

public class AppointmentDiscountConfiguration : IEntityTypeConfiguration<AppointmentDiscount>
{
    public void Configure(EntityTypeBuilder<AppointmentDiscount> builder)
    {
        builder.Property(d => d.Category).HasConversion<byte>();
        builder.Property(d => d.Type).HasConversion<byte>();
        builder.Property(d => d.Source).HasConversion<byte>();

        builder.Property(d => d.Value)
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(d => d.Amount)
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(d => d.Code)
            .HasMaxLength(50);

        builder.Property(d => d.Reason)
            .HasMaxLength(200)
            .IsRequired();

        // RELATIONSHIP WITH APPOINTMENT
        builder.HasOne(d => d.Appointment)
            .WithMany(a => a.Discounts)
            .HasForeignKey(d => d.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(d => new { d.AppointmentId, d.Category })
            .IsUnique();
    }
}

