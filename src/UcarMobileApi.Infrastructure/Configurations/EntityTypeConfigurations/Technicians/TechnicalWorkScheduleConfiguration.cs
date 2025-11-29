using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UcarMobileApi.Core.Entities.Technicians;

namespace UcarMobileApi.Infrastructure.Configurations.EntityTypeConfigurations.Technicians;

public class TechnicalWorkScheduleConfiguration : IEntityTypeConfiguration<TechnicalWorkSchedule>
{
    public void Configure(EntityTypeBuilder<TechnicalWorkSchedule> builder)
    {
        builder.Property(e => e.Day).IsRequired();
        builder.Property(e => e.StartTime).IsRequired();
        builder.Property(e => e.EndTime).IsRequired();
        builder.Property(e => e.IsActive).HasDefaultValue(true);

        builder.HasIndex(e => new { e.TechnicianId, e.Day, e.StartTime, e.EndTime });

        builder.HasOne(e => e.Technician)
            .WithMany(t => t.WorkSchedules)
            .HasForeignKey(e => e.TechnicianId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
