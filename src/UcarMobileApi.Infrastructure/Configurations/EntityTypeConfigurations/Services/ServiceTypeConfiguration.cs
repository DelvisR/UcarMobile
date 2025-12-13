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

        builder.HasIndex(st => st.Title).IsUnique();
    }
}
