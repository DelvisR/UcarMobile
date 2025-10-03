using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UcarMobileApi.Core.Entities;

namespace UcarMobileApi.Infrastructure.Configurations.EntityTypeConfigurations;

public class TechnicianConfiguration : IEntityTypeConfiguration<Technician>
{
    public void Configure(EntityTypeBuilder<Technician> builder)
    {
        builder.ToTable("Technician"); // TPT (Table-Per-Type)

        builder.Property(u => u.IsFreelance).IsRequired().HasDefaultValue(false);
    }
}
