using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UcarMobileApi.Core.Entities.Services;

namespace UcarMobileApi.Infrastructure.Configurations.EntityTypeConfigurations;

/// <summary>
/// Entity Framework configuration for Service entities.
/// </summary>
public class ServiceTypeConfiguration : IEntityTypeConfiguration<ServiceType>
{
    public void Configure(EntityTypeBuilder<ServiceType> builder)
    {
        builder.Property(st => st.Title)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasMany(st => st.Categories)
            .WithOne(sc => sc.ServiceType)
            .HasForeignKey(sc => sc.ServiceTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(st => st.Title).IsUnique();
    }
}

public class ServiceCategoryConfiguration : IEntityTypeConfiguration<ServiceCategory>
{
    public void Configure(EntityTypeBuilder<ServiceCategory> builder)
    {
        builder.Property(sc => sc.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasOne(sc => sc.ServiceType)
            .WithMany(st => st.Categories)
            .HasForeignKey(sc => sc.ServiceTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(sc => sc.Services)
            .WithOne(s => s.ServiceCategory)
            .HasForeignKey(s => s.ServiceCategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(sc => sc.ServiceTypeId);
        builder.HasIndex(sc => new { sc.ServiceTypeId, sc.Name }).IsUnique();
    }
}

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.Property(s => s.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.HasOne(s => s.ServiceCategory)
            .WithMany(sc => sc.Services)
            .HasForeignKey(s => s.ServiceCategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Estimates)
            .WithOne(e => e.Service)
            .HasForeignKey(e => e.ServiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => s.ServiceCategoryId);
        builder.HasIndex(s => new { s.ServiceCategoryId, s.Name }).IsUnique();
    }
}

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
