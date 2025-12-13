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

        builder.HasOne(t => t.Technician)
            .WithMany(t => t.WorkSchedules)
            .HasForeignKey(t => t.TechnicianId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(ws => new { ws.TechnicianId, ws.Day }).HasFilter("\"IsActive\" = TRUE");

        builder.HasIndex(ws => new { ws.TechnicianId, ws.Day, ws.StartTime, ws.EndTime });
        builder.HasIndex(ws => ws.IsActive);





    }
}
