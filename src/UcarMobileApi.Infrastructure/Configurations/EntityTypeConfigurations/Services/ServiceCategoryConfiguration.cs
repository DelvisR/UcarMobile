using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UcarMobileApi.Core.Entities.Services;

namespace UcarMobileApi.Infrastructure.Configurations.EntityTypeConfigurations.Services;

/// <summary>
/// Entity Framework configuration for ServiceCategory entity.
/// </summary>
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

        builder.HasMany(sc => sc.TechnicianSpecialities)
            .WithOne(ts => ts.ServiceCategory)
            .HasForeignKey(ts => ts.ServiceCategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(sc => sc.ServiceTypeId);
        builder.HasIndex(sc => new { sc.ServiceTypeId, sc.Name }).IsUnique();
    }
}
