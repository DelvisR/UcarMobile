using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UcarMobileApi.Core.Entities.Services;

namespace UcarMobileApi.Infrastructure.Configurations.EntityTypeConfigurations.Services;

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
