using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UcarMobileApi.Core.Entities.Technicians;

namespace UcarMobileApi.Infrastructure.Configurations.EntityTypeConfigurations.Technicians;

public class TechnicalCalendarBlockConfiguration : IEntityTypeConfiguration<TechnicalCalendarBlock>
{
    public void Configure(EntityTypeBuilder<TechnicalCalendarBlock> builder)
    {
        builder.Property(e => e.AllDay).HasDefaultValue(false);
        builder.Property(e => e.IsActive).HasDefaultValue(true);
        builder.Property(e => e.Reason).IsRequired();

        builder.HasIndex(e => new { e.TechnicianId, e.SpecificDate });

        builder.HasOne(e => e.Technician)
            .WithMany(t => t.CalendarBlocks)
            .HasForeignKey(e => e.TechnicianId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
