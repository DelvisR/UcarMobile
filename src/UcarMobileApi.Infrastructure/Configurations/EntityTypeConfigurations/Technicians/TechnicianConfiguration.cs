using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UcarMobileApi.Core.Entities.Technicians;

namespace UcarMobileApi.Infrastructure.Configurations.EntityTypeConfigurations.Technicians;

public class TechnicianConfiguration : IEntityTypeConfiguration<Technician>
{
    public void Configure(EntityTypeBuilder<Technician> builder)
    {
        builder.ToTable("Technician"); // TPT (Table-Per-Type)

        builder.Property(u => u.IsFreelance).IsRequired().HasDefaultValue(false);
    }
}
