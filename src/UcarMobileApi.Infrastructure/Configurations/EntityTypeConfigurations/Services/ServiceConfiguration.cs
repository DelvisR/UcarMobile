using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UcarMobileApi.Core.Entities.Services;

namespace UcarMobileApi.Infrastructure.Configurations.EntityTypeConfigurations.Services;

/// <summary>
/// Entity Framework configuration for Service entities.
/// </summary>
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
