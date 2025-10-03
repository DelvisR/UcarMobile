using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UcarMobileApi.Core.Entities.Configurations;

namespace UcarMobileApi.Infrastructure.Configurations.EntityTypeConfigurations;

public class BusinessParameterConfiguration : IEntityTypeConfiguration<BusinessParameter>
{
    public void Configure(EntityTypeBuilder<BusinessParameter> builder)
    {
        builder.HasIndex(x => x.Key).IsUnique();

        builder.Property(x => x.Key)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Value)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.ValueType)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(255);
    }
}
