using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UcarMobileApi.Core.Entities.Technicians;

namespace UcarMobileApi.Infrastructure.Configurations.EntityTypeConfigurations.Technicians;

public class TechnicianAvailableSlotConfiguration : IEntityTypeConfiguration<TechnicianAvailableSlot>
{
    public void Configure(EntityTypeBuilder<TechnicianAvailableSlot> builder)
    {
        builder.HasNoKey();
        builder.ToView(null); // Prevents EF from attempting to map to a table or view
    }
}
