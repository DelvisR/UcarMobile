using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UcarMobileApi.Core.Entities.Technicians;

namespace UcarMobileApi.Infrastructure.Configurations.EntityTypeConfigurations.Technicians;

/// <summary>
/// Entity Framework configuration for TechnicianSpeciality entity.
/// Configures the many-to-many relationship between Technician and ServiceCategory.
/// </summary>
public class TechnicianSpecialityConfiguration : IEntityTypeConfiguration<TechnicianSpeciality>
{
    public void Configure(EntityTypeBuilder<TechnicianSpeciality> builder)
    {
        // Table name (optional, EF will use TechnicianSpeciality by default)

        // Composite PK
        builder.HasKey(x => new { x.TechnicianId, x.ServiceCategoryId });

        builder.Property(a => a.SkillLevel)
            .IsRequired()
            .HasConversion<byte>();

        builder.Property(u => u.IsCertified).IsRequired().HasDefaultValue(false);

        // Relationships
        builder.HasOne(ts => ts.Technician)
            .WithMany(t => t.Specialities)
            .HasForeignKey(ts => ts.TechnicianId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ts => ts.ServiceCategory)
            .WithMany(sc => sc.TechnicianSpecialities)
            .HasForeignKey(ts => ts.ServiceCategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
